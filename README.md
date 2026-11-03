# LegacyApp – layer dati EF Core su Access, pronto per SQL Server

Scheletro di soluzione **VB.NET / .NET 10** che:

- astrae il database Access legacy con **Entity Framework Core 10** (provider community `EntityFrameworkCore.Jet`);
- espone i dati tramite una **REST API ASP.NET Core** (controller in VB);
- include tre frontend di esempio sulla stessa API: **web React**, **web Blazor** (Server e WebAssembly, vedi `BLAZOR.md`) e **desktop WinForms in VB**;
- permette di passare a **SQL Server** cambiando solo la configurazione.

## Struttura

```
LegacyApp.slnx
├─ Directory.Build.props        net10.0, Option Strict On, PlatformTarget x64
├─ Directory.Packages.props     versioni NuGet centralizzate
├─ src/
│  ├─ LegacyApp.Data/           ORM: entità, DbContext, mappature, scelta del provider
│  │  ├─ Entities/              POCO puliti (Cliente, Ordine = ESEMPI da sostituire)
│  │  ├─ Configurations/        Fluent API: classi -> tabelle/colonne legacy
│  │  ├─ AppDbContext.vb
│  │  ├─ DbContextOptionsBuilderExtensions.vb   <- unico punto che conosce Access/SQL Server
│  │  ├─ ServiceCollectionExtensions.vb         <- registrazione in DI (API, worker)
│  │  ├─ AppDbContextFactory.vb                 <- uso senza DI (codice WinForms)
│  │  └─ DesignTimeDbContextFactory.vb          <- per gli strumenti "dotnet ef"
│  ├─ LegacyApp.Api/            REST API
│  │  ├─ Controllers/           ClientiController (CRUD + paginazione), HealthController
│  │  ├─ Contracts/             DTO: le entità EF non escono mai dall'API
│  │  └─ LegacyApp.Api.http     richieste di prova (Visual Studio le esegue direttamente)
│  ├─ LegacyApp.Client/         client HTTP tipizzato della API (VB, nessuna dipendenza NuGet)
│  ├─ LegacyApp.WinForms/       frontend desktop VB: elenco paginato, ordini, crea/modifica/elimina
│  ├─ LegacyApp.Blazor.Shared/  UI Blazor condivisa (pagine/componenti, C#) -> usa LegacyApp.Client
│  ├─ LegacyApp.Blazor.Server/  host Blazor Server (http://localhost:5200)
│  └─ LegacyApp.Blazor.Wasm/    host Blazor WebAssembly (http://localhost:5210)
├─ clients/web/                 frontend web React + TypeScript (vedi clients/web/README.md)
└─ tools/
   ├─ scaffold/scaffold-access.ps1     reverse engineering del DB Access
   ├─ sample-db/create-sample-db.ps1   crea un .accdb di prova (tblClienti/tblOrdini + dati)
   └─ access/ensure-dual-table.ps1     aggiunge la tabella tecnica #Dual a un DB Access esistente
```

Dipendenze:

```
WinForms ──────> Client ──HTTP──┐
Blazor (Server|Wasm) ─> Client ──HTTP──┤
Web (React) ────────────HTTP────┼──> Api ──> Data ──> EF Core (Jet | SqlServer)
```

Il provider concreto compare **in un solo file**; i frontend non sanno nemmeno che esiste un database.

## Prerequisiti

1. **Visual Studio 2026** (workload "Sviluppo ASP.NET e Web") con **.NET 10 SDK**.
2. **Driver Access (ACE)** con la stessa bitness del processo:
   - se Office è a 64 bit, va bene il driver già presente;
   - altrimenti installare *Microsoft Access Database Engine 2016 Redistributable* x64.
     Con Office a 32 bit già installato serve l'installazione silenziosa (`accessdatabaseengine_X64.exe /quiet`),
     oppure si porta tutta la soluzione a `x86` in `Directory.Build.props`.
3. Accesso a NuGet.org (le versioni sono in `Directory.Packages.props`).
4. Solo per il frontend web: **Node.js 20+** e **pnpm** (`corepack enable`).

## Database Access di prova

Lo scheletro non contiene un database. Due opzioni:

- **DB reale**: una **copia** del `.accdb`/`.mdb` dell'applicazione legacy (il percorso è nella sua connection string:
  `app.config`, `My.Settings` o cercando `Data Source=` nel codice). Poi rigenerare le entità (vedi *Scaffolding*).
- **DB di esempio** coerente con le entità `Cliente`/`Ordine` già presenti:

  ```powershell
  # PowerShell con la stessa bitness del driver ACE (di norma 64 bit)
  powershell -ExecutionPolicy Bypass -File tools\sample-db\create-sample-db.ps1 -Path C:\Dati\Legacy.accdb
  ```

  Crea `tblClienti` e `tblOrdini` (PK, FK, indici) con 120 clienti e circa 300 ordini (dati riproducibili, seed fisso);
  circa un terzo dei clienti non ha ordini, così si può provare sia l'eliminazione riuscita sia il 409.
  Parametri: `-Clienti 500`, `-Force` per sovrascrivere, `-Provider Microsoft.ACE.OLEDB.16.0`.

## Access e la tabella `#Dual` (DB esistenti)

Access non ha una tabella `DUAL`. Per le query senza `FROM`, come gli `EXISTS` generati da `Any()`/`AnyAsync()`,
il provider Jet genera `... FROM (SELECT COUNT(*) FROM #Dual)`. La tabella `#Dual` esiste solo nei database
**creati da EF Core**. Su un DB legacy manca, e la query fallisce con:

> Impossibile trovare la tabella o la query di input '#Dual'

Ci sono due soluzioni:

1. **Creare `#Dual`** (consigliato). Lo script fa prima un backup, è idempotente e crea la stessa struttura di EF:
   una riga, un vincolo CHECK, la tabella nascosta in Access.
   ```powershell
   tools\access\ensure-dual-table.ps1 -Path C:\Dati\Legacy.accdb
   ```
   Il DB di esempio creato da `create-sample-db.ps1` la include già.
2. **Non modificare il DB**: indicare in `appsettings.json` una tabella esistente qualsiasi, meglio se piccola
   (il provider la usa solo dentro `SELECT COUNT(*)`):
   ```json
   "Data": { "Provider": "Access", "JetDualTable": "tblClienti" }
   ```

## Avvio rapido

1. Aprire `LegacyApp.slnx` in Visual Studio.
2. In `src/LegacyApp.Api/appsettings.json` puntare `ConnectionStrings:Access` al file `.accdb`/`.mdb`
   (lo script del DB di esempio stampa la stringa già pronta).
3. Sostituire le entità di esempio con quelle reali (vedi *Scaffolding*), oppure adattare
   `Configurations/*.vb` ai nomi reali di tabelle e colonne.
4. Impostare **LegacyApp.Api** come progetto di avvio e premere F5.
5. Aprire `GET /api/health`: deve rispondere `"status": "ok"` e `"is64BitProcess": true`.
6. Provare gli endpoint con `LegacyApp.Api.http`. In Development il documento OpenAPI è su `/openapi/v1.json`
   e si può importare in Postman, Bruno o Insomnia.

| Metodo | Endpoint | Note |
|---|---|---|
| GET | `/api/clienti?page=1&pageSize=50` | paginato, max 200 |
| GET | `/api/clienti/{id}` | 404 se assente |
| GET | `/api/clienti/{id}/ordini` | relazione 1:N |
| POST | `/api/clienti` | 201 + header Location |
| PUT | `/api/clienti/{id}` | 204 |
| DELETE | `/api/clienti/{id}` | 409 se il cliente ha ordini |

Gli errori sono restituiti in formato `application/problem+json` (RFC 9457).

## Frontend

### Web (React)

```bash
cd clients/web && pnpm install && pnpm dev     # http://localhost:5173, API su http://localhost:5080
```

Dettagli in [`clients/web/README.md`](clients/web/README.md). Per servirlo da un'altra origine in produzione,
aggiungerla a `Cors:AllowedOrigins` in `appsettings.json` dell'API.

### Desktop (WinForms VB)

1. Avviare l'API.
2. Impostare **più progetti di avvio** (tasto destro sulla soluzione → *Configura progetti di avvio*):
   `LegacyApp.Api` e `LegacyApp.WinForms`, oppure avviare la WinForms da sola con l'API già attiva.
3. L'indirizzo dell'API è in `src/LegacyApp.WinForms/appsettings.json` (o variabile d'ambiente `LEGACYAPP_API_URL`).

Funzioni: elenco paginato (F5 per aggiornare), ordini del cliente selezionato con totale,
nuovo/modifica (doppio clic sulla riga) con validazione accanto ai campi, eliminazione con conferma.
Le form hanno i file `.Designer.vb` standard e si aprono nel designer di Visual Studio.

La WinForms usa **`LegacyApp.Client`**, che non contiene codice WinForms: è lo stesso client che può usare
l'**applicazione legacy** per leggere i dati tramite API invece che direttamente da Access, una form alla volta.
Se l'app legacy è ancora su .NET Framework 4.x, basta il multi-target `net10.0;net48` (vedi commento nel `.vbproj`).

## Passare a SQL Server

```json
"Data": { "Provider": "SqlServer" }
```

Più la relativa `ConnectionStrings:SqlServer`. Non si modifica nessuna riga di codice.

Il passaggio funziona solo se si rispettano queste regole:

- **solo LINQ**: niente `FromSqlRaw` o `ExecuteSql` con SQL scritto a mano, né funzioni specifiche di Jet;
- tutte le differenze di schema stanno in `Configurations/*.vb`;
- tipi con precisione esplicita (`HasPrecision(19, 4)` per il tipo Valuta, `HasMaxLength` per i testi).

## Scaffolding (generare le entità dal DB Access)

EF Core 10 genera codice **solo in C#**: il pacchetto `EntityFrameworkCore.VisualBasic` è fermo a EF Core 8.
La procedura quindi è:

```powershell
cd tools\scaffold
.\scaffold-access.ps1 -AccessFile C:\Dati\Legacy.accdb                        # tutte le tabelle
.\scaffold-access.ps1 -AccessFile C:\Dati\Legacy.accdb -Tables tblClienti,tblOrdini
```

1. Lo script crea un progetto C# usa-e-getta in `artifacts\scaffold-cs` e ci genera entità e DbContext.
2. Il codice va convertito in VB con l'estensione Visual Studio **"Code Converter (VB - C#)"** (IC#Code), oppure a mano.
   Si tratta di POCO e Fluent API, quindi la conversione è meccanica.
3. Le classi vanno in `Entities/`. Il contenuto di `OnModelCreating` si divide in una classe
   `IEntityTypeConfiguration(Of T)` per entità, dentro `Configurations/`.
4. Da quel momento il modello VB è la fonte di verità e si mantiene a mano: lo schema legacy è stabile.

## Usarlo dall'applicazione WinForms legacy

- **App ancora su .NET Framework 4.x**: non può referenziare una libreria `net10.0`. Può però chiamare la REST API
  con `HttpClient`. È un buon primo passo verso lo *strangler pattern*: la logica dati si sposta dietro l'API, form per form.
- **App portata su .NET 10** (Upgrade Assistant): si aggiunge il riferimento a `LegacyApp.Data` e si usa

  ```vb
  Using db = AppDbContextFactory.Create(DatabaseProvider.Access, connectionString)
      Dim attivi = db.Clienti.Where(Function(c) c.Attivo).OrderBy(Function(c) c.RagioneSociale).ToList()
  End Using
  ```

  Regola: un `DbContext` per operazione o form, mai un'istanza globale condivisa.

## Note VB per chi arriva da C#/Java

- **Il RootNamespace viene anteposto** a ogni `Namespace` nei file: `Namespace Entities` nel progetto Data
  diventa `LegacyApp.Data.Entities`. Negli `Imports` si scrive invece il nome completo.
- **VB non distingue maiuscole e minuscole**: una variabile `dataOptions` nasconde il tipo `DataOptions` (vedi `Program.vb`).
- `Option Strict On` è attivo per tutta la soluzione: niente conversioni implicite né late binding.
- VB non ha i *nullable reference types*: per EF ogni `String` è nullable, a meno di `.IsRequired()` nella configurazione.
- Le lambda sono `Function(x) ...` (con valore di ritorno) o `Sub(x) ...` (senza). I nomi di membri che sono
  parole chiave vanno tra parentesi quadre: `[Set](Of T)()`, `[Get]`.
- Gli `Imports` globali (l'equivalente dei `global using` di C#) sono nei `.vbproj`, negli elementi `<Import Include=... />`.

## Limiti di Access dietro una REST API (da leggere)

- Microsoft **non supporta** il driver ACE in scenari server o di servizio. Funziona, ma va considerato una soluzione **transitoria**.
- Il locking è a livello di file (`.laccdb`): con poche richieste concorrenti va bene, sotto carico no.
- Il file `.accdb` deve stare su un disco locale del server API, non su una share di rete.
- Se l'API gira in IIS, l'Application Pool deve avere la stessa bitness del driver ("Enable 32-bit Applications").
- OLE DB funziona solo su Windows. Dopo il passaggio a SQL Server l'API può girare anche su Linux o in container
  (e si torna ad AnyCPU).

## Roadmap di migrazione suggerita

1. **Oggi**: Access dietro `LegacyApp.Data` e la REST API; i nuovi client usano l'API.
2. **Migrazione dati**: **SSMA for Access** (SQL Server Migration Assistant) per schema e dati.
3. **Verifica del modello**: si confronta lo schema generato da SSMA con quello che EF si aspetta.
   Questo comando dovrebbe funzionare anche su un progetto VB, perché non genera codice:
   ```powershell
   $env:LEGACYAPP_DB_PROVIDER = "SqlServer"
   dotnet ef dbcontext script -p src\LegacyApp.Data -o artifacts\schema-ef.sql
   ```
4. **Switch**: `Data:Provider = SqlServer` e regressione sugli stessi endpoint.
5. **Dopo**: refactoring dello schema (nomi, tipi, vincoli). Cambiano solo `Configurations/*.vb`; DTO e client restano invariati.
