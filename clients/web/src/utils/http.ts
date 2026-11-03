import { Effect, Either, Schema, pipe } from 'effect'
import { config } from '@/config'
import { HttpError, NetworkError, ParseError } from './apiErrors'
import type { ProblemDetails } from './apiErrors'

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE'

type RequestOptions = {
  readonly method?: HttpMethod
  readonly body?: unknown
  readonly signal?: AbortSignal | undefined
}

const buildUrl = (path: string): string => `${config.apiBaseUrl}${path}`

const readProblem = (response: Response): Effect.Effect<ProblemDetails | null> =>
  pipe(
    Effect.tryPromise(() => response.json() as Promise<ProblemDetails>),
    Effect.orElseSucceed(() => null)
  )

const send = (
  path: string,
  options: RequestOptions = {}
): Effect.Effect<Response, NetworkError | HttpError> =>
  pipe(
    Effect.tryPromise({
      try: () =>
        fetch(buildUrl(path), {
          method: options.method ?? 'GET',
          headers: {
            Accept: 'application/json',
            ...(options.body === undefined ? {} : { 'Content-Type': 'application/json' }),
          },
          ...(options.body === undefined ? {} : { body: JSON.stringify(options.body) }),
          ...(options.signal === undefined ? {} : { signal: options.signal }),
        }),
      catch: (e) => new NetworkError({ message: String(e) }),
    }),
    Effect.flatMap((response) =>
      response.ok
        ? Effect.succeed(response)
        : pipe(
            readProblem(response),
            Effect.flatMap((problem) =>
              Effect.fail(
                new HttpError({
                  message: problem?.title ?? response.statusText,
                  status: response.status,
                  problem,
                })
              )
            )
          )
    )
  )

/** Chiamata con risposta JSON validata a runtime contro uno Schema. */
export const requestJson = <A, I>(
  schema: Schema.Schema<A, I>,
  path: string,
  options: RequestOptions = {}
): Effect.Effect<A, NetworkError | HttpError | ParseError> =>
  pipe(
    send(path, options),
    Effect.flatMap((response) =>
      Effect.tryPromise({
        try: () => response.json() as Promise<unknown>,
        catch: (e) => new ParseError({ message: String(e) }),
      })
    ),
    Effect.flatMap((json) =>
      pipe(
        Schema.decodeUnknown(schema)(json),
        Effect.mapError((e) => new ParseError({ message: e.message }))
      )
    )
  )

/** Chiamata senza corpo di risposta (204 No Content). */
export const requestVoid = (
  path: string,
  options: RequestOptions = {}
): Effect.Effect<void, NetworkError | HttpError> => pipe(send(path, options), Effect.asVoid)

/**
 * Esegue un Effect come Promise e, in caso di errore, rifiuta con l'errore tipizzato ORIGINALE.
 * (Effect.runPromise lo avvolgerebbe in un FiberFailure: instanceof HttpError non funzionerebbe piu'.)
 * Va chiamato solo ai confini: queryFn / mutationFn negli hook.
 */
export const runPromise = <A, E extends Error>(effect: Effect.Effect<A, E>): Promise<A> =>
  Effect.runPromise(Effect.either(effect)).then((result) =>
    Either.isRight(result) ? result.right : Promise.reject(result.left)
  )
