/// <reference types="vite/client" />

// Module augmentation di Vite: richiede "interface" (unica eccezione alla regola "solo type").
/* eslint-disable @typescript-eslint/consistent-type-definitions */
interface ImportMetaEnv {
  readonly VITE_API_BASE_URL: string
  readonly VITE_APP_NAME: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
