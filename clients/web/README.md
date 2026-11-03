# LegacyApp Web – frontend React

SPA che mostra e modifica i clienti esposti da `LegacyApp.Api`.

**Stack**: React 19, TypeScript (strict), Vite, Mantine, TanStack Router + Query, React Hook Form, Effect (Schema ed errori tipizzati), Tailwind (solo layout).

## Avvio

```bash
# 1. Avviare l'API (dalla root della soluzione)
dotnet run --project src/LegacyApp.Api --launch-profile http     # -> http://localhost:5080

# 2. Avviare il frontend (Node 20+ e pnpm: corepack enable)
cd clients/web
pnpm install
pnpm dev                                                          # -> http://localhost:5173
```

In sviluppo le chiamate a `/api/*` passano dal **proxy di Vite** verso `API_PROXY_TARGET` (file `.env`): niente problemi di CORS.

## Script

| Comando | Cosa fa |
|---|---|
| `pnpm dev` | dev server con hot reload |
| `pnpm build` | type-check + build di produzione in `dist/` |
| `pnpm preview` | serve la build su http://localhost:4173 |
| `pnpm lint` | ESLint (strict type-checked, zero warning) |
| `pnpm type-check` | solo TypeScript |
| `pnpm format` | Prettier |

## Pagine

| Route | Contenuto |
|---|---|
| `/clienti?page=N` | elenco paginato (la pagina sta nell'URL: link condivisibili, tasto "indietro") |
| `/clienti/nuovo` | creazione |
| `/clienti/:id` | dettaglio + ordini con totale + eliminazione (con conferma) |
| `/clienti/:id/modifica` | modifica |

L'header mostra lo stato dell'API e il provider in uso (`Access · ok` / `SqlServer · ok`), letto da `/api/health`.

## Come è fatto

```
src/
├─ config.ts                 env validate all'avvio (Effect Schema)
├─ utils/http.ts             fetch -> Effect con errori tipizzati (NetworkError | HttpError | ParseError)
├─ utils/apiErrors.ts        ProblemDetails RFC 9457 -> messaggi utente / errori per campo
├─ lib/router.tsx            route tipizzate, pagine lazy (code splitting)
└─ features/clienti/
   ├─ types.ts               Schema dei contratti API (validati a runtime) + schema del form
   ├─ utils/clientiApi.ts    chiamate HTTP come Effect (pure, non eseguite)
   ├─ hooks/                 TUTTA la logica: query, mutation, form, navigazione
   └─ components/            solo rendering
```

- **Validazione a runtime delle risposte**: se l'API cambia contratto, l'errore è esplicito ("Risposta dell'API in un formato inatteso") invece di un `undefined` in giro per la UI.
- **Errori di validazione dal server** (400 `ValidationProblemDetails`): mappati sui campi del form, accanto a quelli client.
- **Errori di dominio** (409 "cliente con ordini"): notifica con il `detail` restituito dall'API.
- **Error boundary** per ogni pagina e per la sezione ordini: se gli ordini non si caricano, l'anagrafica resta visibile.

## Produzione

```bash
VITE_API_BASE_URL=https://api.example.it pnpm build
```

- Se la SPA è servita da **un'altra origine** rispetto all'API, aggiungere l'origine in `Cors:AllowedOrigins` dell'API.
- In alternativa si serve `dist/` dietro lo **stesso reverse proxy** dell'API (IIS, nginx, YARP): `VITE_API_BASE_URL` vuoto e niente CORS.
