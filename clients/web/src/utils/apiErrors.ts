import { Data } from 'effect'

// Errori tipizzati del layer HTTP. Data.TaggedError produce istanze di Error:
// TanStack Query e gli ErrorBoundary li gestiscono senza adattatori.

/** Formato RFC 9457 restituito da ASP.NET Core (ProblemDetails / ValidationProblemDetails). */
export type ProblemDetails = {
  readonly title?: string
  readonly detail?: string
  readonly status?: number
  readonly errors?: Readonly<Record<string, readonly string[]>>
}

export class NetworkError extends Data.TaggedError('NetworkError')<{
  readonly message: string
}> {}

export class HttpError extends Data.TaggedError('HttpError')<{
  readonly message: string
  readonly status: number
  readonly problem: ProblemDetails | null
}> {}

export class ParseError extends Data.TaggedError('ParseError')<{
  readonly message: string
}> {}

export type ApiError = NetworkError | HttpError | ParseError

const isApiError = (e: unknown): e is ApiError =>
  e instanceof NetworkError || e instanceof HttpError || e instanceof ParseError

/** Messaggio leggibile per l'utente finale. */
export const toUserMessage = (e: unknown): string => {
  if (!isApiError(e)) return e instanceof Error ? e.message : 'Errore sconosciuto'

  switch (e._tag) {
    case 'NetworkError':
      return "Impossibile contattare l'API. Verificare che LegacyApp.Api sia in esecuzione."
    case 'ParseError':
      return "Risposta dell'API in un formato inatteso."
    case 'HttpError':
      return e.problem?.detail ?? e.problem?.title ?? `Errore HTTP ${e.status}`
  }
}

/** Errori di validazione lato server (400), con chiave normalizzata in camelCase. */
export const toFieldErrors = (e: unknown): ReadonlyArray<readonly [string, string]> => {
  if (!(e instanceof HttpError) || e.status !== 400 || !e.problem?.errors) return []

  return Object.entries(e.problem.errors).flatMap(([key, messages]) => {
    const first = messages[0]
    const field = key.charAt(0).toLowerCase() + key.slice(1)
    return first === undefined ? [] : [[field, first] as const]
  })
}
