import { useQuery } from '@tanstack/react-query'
import { Schema } from 'effect'
import { requestJson, runPromise } from '@/utils/http'

const HealthSchema = Schema.Struct({
  status: Schema.String,
  provider: Schema.String,
  is64BitProcess: Schema.Boolean,
})

export type ApiHealth = Schema.Schema.Type<typeof HealthSchema>

/** Stato dell'API e del provider DB (Access / SqlServer), aggiornato ogni 30 secondi. */
export const useApiHealth = () =>
  useQuery({
    queryKey: ['health'],
    queryFn: ({ signal }) => runPromise(requestJson(HealthSchema, '/api/health', { signal })),
    refetchInterval: 30_000,
    retry: false,
  })
