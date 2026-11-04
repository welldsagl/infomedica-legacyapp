# CI e deploy su Railway

## CI (GitHub Actions)

`.github/workflows/ci.yml`, su ogni push su `main` e su ogni pull request:

| Job | Cosa fa |
|---|---|
| `dotnet-linux` | build Release di API, Blazor Wasm e Blazor Server; publish del Wasm e dimensione nel riepilogo del job |
| `dotnet-windows` | `dotnet build LegacyApp.slnx` su Windows (unico posto dove compila anche la WinForms) |
| `web` | React: `pnpm lint` + `pnpm build` |
| `docker` | build (senza push) dei tre Dockerfile, gli stessi usati da Railway |

Il deploy non passa da Actions: Railway e' collegato al repository GitHub e ricostruisce i servizi
ad ogni push su `main`. Se la CI fallisce il push c'e' comunque: per bloccare i deploy rotti attivare
in Railway *Settings -> Deploy -> "Wait for CI"* (usa i check di GitHub) o proteggere `main` con i check obbligatori.

## Servizi su Railway

Un progetto Railway con questi servizi, tutti nello stesso ambiente (rete privata condivisa):

| Servizio | Sorgente | Variabili |
|---|---|---|
| `sqlserver` | immagine Docker `mcr.microsoft.com/mssql/server:2022-latest` + volume su `/var/opt/mssql` | `ACCEPT_EULA=Y`, `MSSQL_SA_PASSWORD=<password robusta>`, `MSSQL_PID=Express` |
| `api` | repo GitHub, Dockerfile `src/LegacyApp.Api/Dockerfile` | `RAILWAY_DOCKERFILE_PATH=src/LegacyApp.Api/Dockerfile`, `ConnectionStrings__SqlServer`, `Cors__AllowedOrigins__0` |
| `web` | repo GitHub, Dockerfile `src/LegacyApp.Blazor.Wasm/Dockerfile` | `RAILWAY_DOCKERFILE_PATH=src/LegacyApp.Blazor.Wasm/Dockerfile`, `API_BASE_URL` |
| `blazor-server` (opzionale) | repo GitHub, Dockerfile `src/LegacyApp.Blazor.Server/Dockerfile` | `RAILWAY_DOCKERFILE_PATH=src/LegacyApp.Blazor.Server/Dockerfile`, `ApiBaseUrl` |

Per i servizi dal repo lasciare *Root Directory* = `/`: i Dockerfile usano la root come build context
(servono `Directory.*.props`, `global.json` e i progetti referenziati). Per non ricostruire tutto ad ogni push,
impostare *Watch Paths* per servizio, es. per `api`:
`src/LegacyApp.Api/**`, `src/LegacyApp.Data/**`, `Directory.*.props`, `global.json`.

### Passi

1. **SQL Server**: *New -> Docker Image* `mcr.microsoft.com/mssql/server:2022-latest`; aggiungere un volume montato su
   `/var/opt/mssql`; variabili come in tabella. Abilitare *TCP Proxy* (Settings -> Networking) sulla porta 1433
   per raggiungerlo da fuori, solo per il caricamento iniziale dello schema.
2. **Schema e dati**: dal proprio PC, con l'host/porta pubblici del TCP proxy (es. `roundhouse.proxy.rlwy.net:12345`):
   ```bash
   docker run --rm -i --platform linux/amd64 mcr.microsoft.com/mssql-tools18 \
     /opt/mssql-tools18/bin/sqlcmd -S roundhouse.proxy.rlwy.net,12345 -U sa -P "$SA_PASSWORD" -C \
     < tools/sample-db/create-sample-db.sql
   ```
   Poi disabilitare il TCP proxy: l'API usa la rete privata.
3. **API**: *New -> GitHub Repo*, variabili:
   ```
   RAILWAY_DOCKERFILE_PATH = src/LegacyApp.Api/Dockerfile
   ConnectionStrings__SqlServer = Server=sqlserver.railway.internal,1433;Database=LegacyApp;User Id=sa;Password=${{sqlserver.MSSQL_SA_PASSWORD}};TrustServerCertificate=True;Encrypt=False;
   Cors__AllowedOrigins__0 = https://${{web.RAILWAY_PUBLIC_DOMAIN}}
   ```
   (`${{servizio.VARIABILE}}` e' la sintassi Railway per riferire variabili di altri servizi.)
   Generare un dominio pubblico (Settings -> Networking). Healthcheck: path `/api/health`.
4. **Web (Blazor Wasm)**: *New -> GitHub Repo* (stesso repo), variabili:
   ```
   RAILWAY_DOCKERFILE_PATH = src/LegacyApp.Blazor.Wasm/Dockerfile
   API_BASE_URL = https://${{api.RAILWAY_PUBLIC_DOMAIN}}/
   ```
   Generare un dominio pubblico. Lo slash finale in `API_BASE_URL` e' necessario (il client usa path relativi).
5. Verifica: `https://<api>/api/health` -> `"provider": "SqlServer"`; aprire `https://<web>/clienti`.

### Note

- **`Data__Provider`** e' fissato a `SqlServer` nel Dockerfile dell'API: su Linux Access non esiste.
- **Blazor Server** su Railway funziona (WebSocket supportati) finche' c'e' una sola replica; con piu' repliche
  servono sticky session o Azure SignalR, per questo e' marcato opzionale.
- **React**: non ha un Dockerfile in questo scheletro; si pubblica come sito statico (`pnpm build` -> `dist/`)
  con lo stesso schema del Wasm (Caddy) se serve.
- **Costi**: SQL Server Express in container richiede ~1 GB RAM a riposo; e' la voce principale. Per un demo
  si puo' fermare il servizio quando non serve (i dati restano nel volume).
- **Prova in locale degli stessi container**:
  ```bash
  docker build -f src/LegacyApp.Api/Dockerfile -t legacyapp-api .
  docker run --rm -p 5080:8080 \
    -e ConnectionStrings__SqlServer='Server=host.docker.internal,1433;Database=LegacyApp;User Id=sa;Password=LegacyApp!2026;TrustServerCertificate=True;Encrypt=False;' \
    -e Cors__AllowedOrigins__0=http://localhost:5210 legacyapp-api
  docker build -f src/LegacyApp.Blazor.Wasm/Dockerfile -t legacyapp-wasm .
  docker run --rm -p 5210:8080 -e API_BASE_URL=http://localhost:5080/ legacyapp-wasm
  ```
