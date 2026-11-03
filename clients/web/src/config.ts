import { Schema } from 'effect'

const EnvSchema = Schema.Struct({
  // Vuoto = stesso host (proxy di Vite in sviluppo, reverse proxy in produzione)
  VITE_API_BASE_URL: Schema.optionalWith(Schema.String, { default: () => '' }),
  VITE_APP_NAME: Schema.optionalWith(Schema.String, { default: () => 'LegacyApp' }),
})

// Fallisce all'avvio (non a runtime in mezzo all'uso) se la configurazione e' invalida
const env = Schema.decodeUnknownSync(EnvSchema)(import.meta.env)

export const config = {
  apiBaseUrl: env.VITE_API_BASE_URL.replace(/\/+$/, ''),
  appName: env.VITE_APP_NAME,
} as const
