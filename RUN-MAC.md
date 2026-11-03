# Eseguire LegacyApp su macOS (Rider)

Su macOS non esiste il driver Access (ACE) e non si compila la WinForms. Tutto il resto gira:
API su **SQL Server in Docker**, frontend Blazor (Server e WebAssembly) e React.

## 1. Prerequisiti (una volta sola)

```bash
# .NET 10 SDK (global.json richiede 10.0.100 o superiore)
brew install --cask dotnet-sdk
dotnet --version

# Docker Desktop (o OrbStack). Su Apple Silicon: Settings -> General -> "Use Rosetta for x86_64/amd64 emulation".
```

## 2. SQL Server in Docker + database di esempio

```bash
export SA_PASSWORD='LegacyApp!2026'     # la stessa del profilo http-sqlserver-docker

docker run -d --name legacy-sql --platform linux/amd64 \
  -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD="$SA_PASSWORD" \
  -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest

# attendere ~20-30 s che il server sia pronto, poi:
docker exec -i legacy-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C \
  < tools/sample-db/create-sample-db.sql
# atteso: Clienti = 120, Ordini ~ 300
```

Riavvii successivi: `docker start legacy-sql` (i dati restano nel container).

## 3. Build

```bash
dotnet build src/LegacyApp.Api
dotnet build src/LegacyApp.Blazor.Wasm
dotnet build src/LegacyApp.Blazor.Server
```

(Non usare `dotnet build` sulla soluzione intera: la WinForms fallisce su macOS. In Rider: tasto destro su
`LegacyApp.WinForms` -> *Unload Project*.)

## 4. Avvio

Terminale 1 – API (porta 5080, provider SqlServer via variabili d'ambiente del profilo):
```bash
dotnet run --project src/LegacyApp.Api --launch-profile http-sqlserver-docker
# verifica: http://localhost:5080/api/health -> {"status":"ok","provider":"SqlServer",...}
```

Terminale 2 – uno dei frontend:
```bash
dotnet run --project src/LegacyApp.Blazor.Wasm   --launch-profile http   # http://localhost:5210
dotnet run --project src/LegacyApp.Blazor.Server --launch-profile http   # http://localhost:5200
cd clients/web && pnpm install && pnpm dev                                # http://localhost:5173
```

## In Rider

1. Aprire `LegacyApp.slnx`; unload di `LegacyApp.WinForms`.
2. Rider importa i profili di `launchSettings.json` come run configuration:
   scegliere `LegacyApp.Api: http-sqlserver-docker` e `LegacyApp.Blazor.Wasm: http` (o `.Server: http`).
3. Per lanciarli insieme: Run -> Edit Configurations -> *Compound* con le due configurazioni.
4. Debug WebAssembly: avviare con il debugger, Rider apre Chrome collegato al proxy di debug.

## Problemi tipici

| Sintomo | Causa / rimedio |
|---|---|
| `Login failed for user 'sa'` | password diversa tra `docker run` e il profilo; oppure il container non e' ancora pronto (`docker logs legacy-sql`) |
| health risponde `provider: Access` | l'API e' partita con il profilo `http`, non `http-sqlserver-docker` |
| Wasm: errore CORS in console | l'origine `http://localhost:5210` deve stare in `Cors:AllowedOrigins` dell'API (gia' aggiunta) |
| Wasm: errore su `Microsoft.VisualBasic` al caricamento | libreria VB nel browser: fallback = portare `LegacyApp.Client` in C# (vedi BLAZOR.md) |
| `NETSDK1045` / SDK non trovato | `dotnet --list-sdks` deve mostrare un 10.0.x; `global.json` accetta 10.0.100+ |
