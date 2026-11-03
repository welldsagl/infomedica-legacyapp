<#
.SYNOPSIS
  Aggiunge a un database Access esistente la tabella "#Dual" richiesta da EntityFrameworkCore.Jet.

.DESCRIPTION
  Access non ha una tabella DUAL (come Oracle). Per le query senza FROM - ad esempio gli EXISTS generati
  da Any()/AnyAsync() - il provider EF Core per Access genera:

      SELECT IIF(EXISTS (...), TRUE, FALSE) FROM (SELECT COUNT(*) FROM `#Dual`)

  EF Core crea "#Dual" solo nei database che crea lui. Su un database esistente (legacy) manca, e la query fallisce con:
      "Impossibile trovare la tabella o la query di input '#Dual'"

  Questo script crea la tabella con la stessa struttura usata da EF Core:
    - una sola colonna ID, una sola riga (ID = 1)
    - vincolo CHECK che impedisce di aggiungere altre righe
    - nascosta nell'interfaccia di Access
  E' idempotente: se la tabella esiste gia' la verifica e non tocca nulla.
  Prima di modificare il file ne crea una copia di backup (disattivabile con -NoBackup).

  Alternativa senza modificare il database: impostare in appsettings.json dell'API
      "Data": { "JetDualTable": "<nome di una tabella esistente>" }
  (vedi README, sezione "Access e la tabella #Dual").

  Requisiti: Windows, driver ACE, PowerShell con la stessa bitness del driver.
  Il database non deve essere aperto in modo esclusivo (chiudere Access e l'API).

.PARAMETER Path
  File .accdb / .mdb da aggiornare.

.PARAMETER Provider
  Provider OLE DB. Default Microsoft.ACE.OLEDB.12.0.

.PARAMETER NoBackup
  Non crea la copia di backup prima della modifica.

.EXAMPLE
  .\ensure-dual-table.ps1 -Path C:\Dati\Legacy.accdb
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $Path,

    [string] $Provider = 'Microsoft.ACE.OLEDB.12.0',

    [switch] $NoBackup
)

$ErrorActionPreference = 'Stop'
$DualTable = '#Dual'

if (-not ('System.Data.OleDb.OleDbConnection' -as [type])) {
    try { Add-Type -AssemblyName System.Data.OleDb }
    catch { Add-Type -AssemblyName System.Data }
}

function Invoke-NonQuery([System.Data.OleDb.OleDbConnection] $connection, [string] $sql) {
    $cmd = $connection.CreateCommand()
    try {
        $cmd.CommandText = $sql
        [void]$cmd.ExecuteNonQuery()
    }
    finally { $cmd.Dispose() }
}

function Invoke-Scalar([System.Data.OleDb.OleDbConnection] $connection, [string] $sql) {
    $cmd = $connection.CreateCommand()
    try {
        $cmd.CommandText = $sql
        return $cmd.ExecuteScalar()
    }
    finally { $cmd.Dispose() }
}

$fullPath = (Resolve-Path -LiteralPath $Path).Path
$connectionString = "Provider=$Provider;Data Source=$fullPath;"

Write-Host "Database      : $fullPath"
Write-Host "Processo 64bit: $([Environment]::Is64BitProcess)  (deve coincidere con la bitness del driver ACE)"

$connection = [System.Data.OleDb.OleDbConnection]::new($connectionString)
try {
    $connection.Open()

    # --- Esiste gia'? -----------------------------------------------------------------------
    $righe = $null
    try { $righe = [int](Invoke-Scalar $connection "SELECT COUNT(*) FROM [$DualTable]") }
    catch { $righe = $null }   # tabella assente

    if ($null -ne $righe) {
        if ($righe -ne 1) {
            throw "La tabella '$DualTable' esiste ma contiene $righe righe: deve contenerne esattamente 1."
        }
        Write-Host "La tabella '$DualTable' esiste gia' ed e' corretta. Nessuna modifica." -ForegroundColor Green
        return
    }

    # --- Backup ------------------------------------------------------------------------------
    if (-not $NoBackup) {
        $connection.Close()
        $dir = [System.IO.Path]::GetDirectoryName($fullPath)
        $name = [System.IO.Path]::GetFileNameWithoutExtension($fullPath)
        $ext = [System.IO.Path]::GetExtension($fullPath)
        $backup = Join-Path $dir ("{0}.bak-{1:yyyyMMdd-HHmmss}{2}" -f $name, (Get-Date), $ext)
        Copy-Item -LiteralPath $fullPath -Destination $backup
        Write-Host "Backup        : $backup"
        $connection.Open()
    }

    # --- Creazione (stessa struttura di EF Core) ----------------------------------------------
    Invoke-NonQuery $connection "CREATE TABLE [$DualTable] ([ID] INTEGER NOT NULL CONSTRAINT [PrimaryKey] PRIMARY KEY)"
    Invoke-NonQuery $connection "INSERT INTO [$DualTable] ([ID]) VALUES (1)"
    try {
        Invoke-NonQuery $connection "ALTER TABLE [$DualTable] ADD CONSTRAINT [SingleRecord] CHECK ((SELECT COUNT(*) FROM [$DualTable]) = 1)"
    }
    catch {
        Write-Warning "Vincolo CHECK non creato ($($_.Exception.Message)). La tabella funziona comunque: non aggiungervi righe."
    }

    # --- Verifica con la stessa forma di query generata da EF Core -----------------------------
    $test = Invoke-Scalar $connection "SELECT IIF(EXISTS (SELECT 1 FROM [$DualTable]), 1, 0) FROM (SELECT COUNT(*) FROM [$DualTable])"
    if ([int]$test -ne 1) { throw "Verifica non riuscita: risultato inatteso '$test'." }
    Write-Host "Tabella '$DualTable' creata e verificata." -ForegroundColor Green
}
finally {
    $connection.Dispose()
}

# --- Facoltativo: nasconde la tabella nell'interfaccia di Access (solo estetico) ----------------
$catalog = $null
try {
    $catalog = New-Object -ComObject ADOX.Catalog
    $catalog.ActiveConnection = $connectionString
    $catalog.Tables.Item($DualTable).Properties.Item('Jet OLEDB:Table Hidden In Access').Value = $true
    Write-Host "Tabella '$DualTable' nascosta in Access."
}
catch {
    Write-Warning "Non e' stato possibile nascondere '$DualTable' in Access (non influisce sul funzionamento): $($_.Exception.Message)"
}
finally {
    if ($null -ne $catalog) {
        try { $catalog.ActiveConnection.Close() } catch { }
        [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($catalog)
    }
    [System.GC]::Collect()
    [System.GC]::WaitForPendingFinalizers()
}
