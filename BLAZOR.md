# LegacyApp – frontend Blazor

Alternativa .NET al frontend React, pensata per un team VB/.NET: stessa soluzione, stesso IDE, stesso
client dell'API. Tre progetti:

| Progetto | Cosa contiene | Linguaggio |
|---|---|---|
| `src/LegacyApp.Blazor.Shared` | **tutta la UI**: pagine, componenti, layout, CSS (Razor Class Library) | C# (solo i componenti Razor) |
| `src/LegacyApp.Blazor.Server` | host **Blazor Server** (UI sul server, SignalR verso il browser) | C#, ~30 righe |
| `src/LegacyApp.Blazor.Wasm` | host **Blazor WebAssembly** (UI nel browser, output = file statici) | C#, ~20 righe |

La libreria condivisa dipende da `LegacyApp.Client` (VB): DTO, client HTTP tipizzato e `ClienteRequestValidator`
sono gli stessi della WinForms. **Nessun contratto e' duplicato in C#.** La logica applicativa resta in VB;
il C# e' confinato al markup dei componenti e a poche righe di code-behind.

```
Blazor.Server ──┐
                ├──> Blazor.Shared ──> Client (VB) ──HTTP──> Api (VB) ──> Data (VB)
Blazor.Wasm ────┘
WinForms (VB) ─────────────────────> Client (VB) ──HTTP──┘
```

## Avvio

```bash
# 1. API (dalla root della soluzione)
dotnet run --project src/LegacyApp.Api --launch-profile http          # -> http://localhost:5080

# 2a. Blazor Server
dotnet run --project src/LegacyApp.Blazor.Server --launch-profile http   # -> http://localhost:5200

# 2b. Blazor WebAssembly
dotnet run --project src/LegacyApp.Blazor.Wasm --launch-profile http     # -> http://localhost:5210
```

URL dell'API: `appsettings.json` del progetto Server (`ApiBaseUrl`), `wwwroot/appsettings.json` del progetto Wasm.
Per il Wasm l'origine `http://localhost:5210` e' gia' in `Cors:AllowedOrigins` della API.

## Pagine (le stesse della SPA React)

| Route | Contenuto |
|---|---|
| `/clienti?page=N` | elenco paginato (pagina nell'URL) |
| `/clienti/nuovo` | creazione |
| `/clienti/{id}` | dettaglio + ordini con totale + eliminazione (con conferma) |
| `/clienti/{id}/modifica` | modifica |

L'header mostra lo stato dell'API (`/api/health`).

## Server vs WebAssembly: quando usare quale

| | Server | WebAssembly |
|---|---|---|
| Dove gira la UI | server | browser |
| Primo caricamento | immediato | download runtime .NET (qualche MB, poi in cache) |
| Connessione | WebSocket persistente per utente | nessuna, solo chiamate REST |
| Scalabilita' | sticky session / Azure SignalR | file statici, scala come una SPA |
| CORS | no (chiama l'API dal server) | si' |
| Separazione FE/BE | puo' essere aggirata (si potrebbe chiamare il DB direttamente) | garantita: il browser vede solo l'API |
| Debug | come una normale app .NET | nel browser, un po' meno comodo |

Per l'obiettivo "FE sostituibile, BE indipendente" la scelta consigliata e' **WebAssembly**. Il progetto Server
esiste per confronto e come opzione se il primo caricamento o il debug pesano piu' della separazione.

## Blazor Hybrid (WinForms)

Gli stessi componenti di `Blazor.Shared` si possono ospitare *dentro* la WinForms con il controllo `BlazorWebView`
(pacchetto `Microsoft.AspNetCore.Components.WebView.WindowsForms`): migrazione schermata per schermata
mantenendo la shell desktop (multi-monitor, integrazione Windows). Non incluso in questo scheletro.

## Note

- **VB in WebAssembly**: Microsoft non supporta *app* Blazor in VB, ma le *librerie* VB referenziate da un'app
  WebAssembly C# funzionano (`Microsoft.VisualBasic.Core` e' nel runtime browser-wasm). Se in fase di build/trimming
  dovessero emergere problemi, il fallback e' spostare `LegacyApp.Client` in C# (e' piccolo) lasciando VB nel backend.
- **Validazione**: lato client con `ClienteRequestValidator` (VB), poi il server rivalida; gli errori 400 (ProblemDetails)
  tornano sugli stessi campi.
- **Cultura**: date/importi formattati in `it-IT`. In WebAssembly i dati ICU per l'italiano sono inclusi di default.
- **Niente librerie di componenti**: la UI usa HTML + CSS minimale per non aggiungere dipendenze allo scheletro.
  Per una app reale valutare MudBlazor, Fluent UI Blazor o Radzen (griglie con sort/filtri, dialog, date picker).
