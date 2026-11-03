<#
.SYNOPSIS
  Crea un database Access di prova (tblClienti + tblOrdini) coerente con le entita' di esempio
  di LegacyApp.Data (Cliente, Ordine) e lo popola con dati fittizi.

.DESCRIPTION
  - Il file .accdb/.mdb viene creato con ADOX (COM).
  - Schema e dati vengono scritti con System.Data.OleDb (.NET), con parametri tipizzati:
    funziona allo stesso modo in Windows PowerShell 5.1 e in PowerShell 7.

  Requisiti:
    - Windows
    - Driver ACE installato (Office oppure "Microsoft Access Database Engine 2016 Redistributable")
    - PowerShell con la STESSA bitness del driver:
        ACE 64 bit -> pwsh (64 bit) oppure C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe
        ACE 32 bit -> C:\Windows\SysWOW64\WindowsPowerShell\v1.0\powershell.exe

  Schema creato (stessi nomi mappati in src\LegacyApp.Data\Configurations):
    tblClienti(IDCliente COUNTER PK, RagioneSociale TEXT(255) NOT NULL, PartitaIVA TEXT(11),
               Email TEXT(255), DataInserimento DATETIME NOT NULL, Attivo BIT NOT NULL)
    tblOrdini (IDOrdine COUNTER PK, IDCliente LONG NOT NULL -> FK tblClienti, DataOrdine DATETIME NOT NULL,
               Importo CURRENCY NOT NULL, Note MEMO)
    #Dual     (tabella tecnica di una riga richiesta da EntityFrameworkCore.Jet, nascosta in Access)

  I dati sono generati con un seed fisso: ogni esecuzione produce lo stesso database.

.PARAMETER Path
  Percorso del file da creare (.accdb consigliato, .mdb supportato). Default: C:\Dati\Legacy.accdb

.PARAMETER Clienti
  Numero di clienti da generare (default 120). Circa un terzo non ha ordini, quindi si puo' eliminare.

.PARAMETER Force
  Sovrascrive il file se esiste gia'.

.PARAMETER Provider
  Provider OLE DB. Default Microsoft.ACE.OLEDB.12.0 (con Office recenti funziona anche 16.0).

.EXAMPLE
  .\create-sample-db.ps1
  .\create-sample-db.ps1 -Path D:\Test\Legacy.accdb -Clienti 500 -Force
#>
[CmdletBinding()]
param(
    [string] $Path = 'C:\Dati\Legacy.accdb',

    [ValidateRange(1, 100000)]
    [int] $Clienti = 120,

    [switch] $Force,

    [string] $Provider = 'Microsoft.ACE.OLEDB.12.0'
)

$ErrorActionPreference = 'Stop'

function Clear-ComObject([object] $obj) {
    if ($null -ne $obj) { [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($obj) }
}

# --- System.Data.OleDb: incluso in PowerShell 7 e nel .NET Framework di Windows PowerShell 5.1 ---
if (-not ('System.Data.OleDb.OleDbConnection' -as [type])) {
    try { Add-Type -AssemblyName System.Data.OleDb }
    catch { Add-Type -AssemblyName System.Data }
}
if (-not ('System.Data.OleDb.OleDbConnection' -as [type])) {
    throw 'System.Data.OleDb non disponibile in questa versione di PowerShell. Usare pwsh 7 oppure Windows PowerShell 5.1.'
}

# --- Preparazione file -------------------------------------------------------------------------
$fullPath = [System.IO.Path]::GetFullPath($Path)
$extension = [System.IO.Path]::GetExtension($fullPath).ToLowerInvariant()
if ($extension -notin @('.accdb', '.mdb')) { throw "Estensione non supportata: '$extension' (usare .accdb o .mdb)" }

Write-Host "Database      : $fullPath"
Write-Host "Provider      : $Provider"
Write-Host "PowerShell    : $($PSVersionTable.PSVersion)"
Write-Host "Processo 64bit: $([Environment]::Is64BitProcess)  (deve coincidere con la bitness del driver ACE)"

if (Test-Path -LiteralPath $fullPath) {
    if (-not $Force) { throw "Il file esiste gia'. Usare -Force per sovrascriverlo." }
    Remove-Item -LiteralPath $fullPath
}
New-Item -ItemType Directory -Force -Path ([System.IO.Path]::GetDirectoryName($fullPath)) | Out-Null

# Engine Type 6 = formato .accdb (Access 2007+), 5 = formato .mdb (Access 2000-2003)
$engineType = if ($extension -eq '.accdb') { 6 } else { 5 }
$connectionString = "Provider=$Provider;Data Source=$fullPath;"

# --- 1. Creazione del file con ADOX (COM) ------------------------------------------------------
$catalog = $null
try {
    $catalog = New-Object -ComObject ADOX.Catalog
    [void]$catalog.Create("$connectionString" + "Jet OLEDB:Engine Type=$engineType;")
    $catalog.ActiveConnection.Close()
}
catch {
    throw ("Creazione del database non riuscita: $($_.Exception.Message)`n" +
           "Cause tipiche: driver ACE non installato, oppure bitness di PowerShell diversa da quella del driver " +
           "(vedi la sezione DESCRIPTION dello script).")
}
finally {
    Clear-ComObject $catalog
    $catalog = $null
    [System.GC]::Collect()
    [System.GC]::WaitForPendingFinalizers()
}

# --- 2. Schema e dati con System.Data.OleDb ----------------------------------------------------
$OleDbType = [System.Data.OleDb.OleDbType]

$connection = [System.Data.OleDb.OleDbConnection]::new($connectionString)
$transaction = $null
try {
    $connection.Open()

    $ddl = @(
        'CREATE TABLE tblClienti (
            IDCliente       COUNTER CONSTRAINT PK_tblClienti PRIMARY KEY,
            RagioneSociale  TEXT(255) NOT NULL,
            PartitaIVA      TEXT(11),
            Email           TEXT(255),
            DataInserimento DATETIME NOT NULL,
            Attivo          BIT NOT NULL
        )',
        'CREATE INDEX IX_tblClienti_PartitaIVA ON tblClienti (PartitaIVA)',
        'CREATE TABLE tblOrdini (
            IDOrdine   COUNTER CONSTRAINT PK_tblOrdini PRIMARY KEY,
            IDCliente  LONG NOT NULL,
            DataOrdine DATETIME NOT NULL,
            Importo    CURRENCY NOT NULL,
            [Note]     MEMO,
            CONSTRAINT FK_tblOrdini_tblClienti FOREIGN KEY (IDCliente) REFERENCES tblClienti (IDCliente)
        )',
        'CREATE INDEX IX_tblOrdini_IDCliente ON tblOrdini (IDCliente)',
        # Tabella "DUAL" usata da EntityFrameworkCore.Jet per le query senza FROM (es. EXISTS da AnyAsync):
        # EF la crea solo nei DB che crea lui. Stessa struttura: una riga, nascosta in Access.
        'CREATE TABLE [#Dual] ([ID] INTEGER NOT NULL CONSTRAINT [PrimaryKey] PRIMARY KEY)',
        'INSERT INTO [#Dual] ([ID]) VALUES (1)'
    )
    foreach ($sql in $ddl) {
        $cmd = $connection.CreateCommand()
        $cmd.CommandText = $sql
        [void]$cmd.ExecuteNonQuery()
        $cmd.Dispose()
    }
    try {
        $cmd = $connection.CreateCommand()
        $cmd.CommandText = 'ALTER TABLE [#Dual] ADD CONSTRAINT [SingleRecord] CHECK ((SELECT COUNT(*) FROM [#Dual]) = 1)'
        [void]$cmd.ExecuteNonQuery()
        $cmd.Dispose()
    }
    catch {
        Write-Warning "Vincolo CHECK su #Dual non creato ($($_.Exception.Message)): la tabella funziona comunque."
    }
    Write-Host 'Schema creato : tblClienti, tblOrdini (PK, FK, indici), #Dual'

    # Generatore con seed fisso: dati riproducibili
    $rnd = [System.Random]::new(42)
    $oggi = (Get-Date).Date

    $nomi = @('Rossi', 'Bianchi', 'Esposito', 'Romano', 'Colombo', 'Ricci', 'Marino', 'Greco', 'Bruno', 'Gallo',
              'Conti', 'De Luca', 'Mancini', 'Costa', 'Giordano', 'Rizzo', 'Lombardi', 'Moretti', 'Barbieri', 'Fontana')
    $settori = @('Costruzioni', 'Impianti', 'Logistica', 'Alimentari', 'Meccanica', 'Informatica', 'Trasporti',
                 'Ceramiche', 'Arredamenti', 'Elettronica', 'Tessile', 'Consulting', 'Servizi', 'Energia')
    $forme = @('S.r.l.', 'S.p.A.', 'S.n.c.', 'S.a.s.', 'S.r.l.s.')
    $noteOrdini = @('Consegna urgente', 'Pagamento a 60 gg', 'Ritiro in sede', 'Fattura differita',
                    'Merce fragile', 'Rif. offerta n. {0}', 'Consegna al piano')

    # Transazione: centinaia di insert in un attimo invece che uno alla volta su disco
    $transaction = $connection.BeginTransaction()

    # --- tblClienti: query parametrizzata con tipi espliciti (niente conversioni di formato date/decimali)
    $cmdCliente = $connection.CreateCommand()
    $cmdCliente.Transaction = $transaction
    $cmdCliente.CommandText = 'INSERT INTO tblClienti (RagioneSociale, PartitaIVA, Email, DataInserimento, Attivo) VALUES (?, ?, ?, ?, ?)'
    $pRagioneSociale  = $cmdCliente.Parameters.Add('RagioneSociale', $OleDbType::VarWChar, 255)
    $pPartitaIva      = $cmdCliente.Parameters.Add('PartitaIVA', $OleDbType::VarWChar, 11)
    $pEmail           = $cmdCliente.Parameters.Add('Email', $OleDbType::VarWChar, 255)
    $pDataInserimento = $cmdCliente.Parameters.Add('DataInserimento', $OleDbType::Date)
    $pAttivo          = $cmdCliente.Parameters.Add('Attivo', $OleDbType::Boolean)

    for ($i = 1; $i -le $Clienti; $i++) {
        $nome = $nomi[$rnd.Next($nomi.Count)]
        $settore = $settori[$rnd.Next($settori.Count)]
        $forma = $forme[$rnd.Next($forme.Count)]
        $ragioneSociale = "$nome $settore $forma"

        # Qualche caso particolare utile nei test: accenti, apostrofi, nomi lunghi
        switch ($i) {
            3 { $ragioneSociale = "Società Agricola L'Uliveto S.s." }
            7 { $ragioneSociale = 'Caffè & Pâtisserie Ünal S.r.l.' }
            11 { $ragioneSociale = 'Consorzio Interregionale per la Valorizzazione dei Prodotti Tipici del Mezzogiorno S.c.a r.l.' }
        }

        # ~15% senza partita IVA, ~10% senza email, ~20% non attivi
        $partitaIva = if ($rnd.NextDouble() -lt 0.15) { $null } else { '{0:D11}' -f [long]($rnd.NextDouble() * 99999999999) }
        $slug = ($nome + $settore).ToLowerInvariant() -replace '[^a-z]', ''
        $email = if ($rnd.NextDouble() -lt 0.10) { $null } else { "info$i@$slug.example" }
        $dataInserimento = $oggi.AddDays(-$rnd.Next(1, 5 * 365)).AddMinutes($rnd.Next(8 * 60, 18 * 60))
        $attivo = $rnd.NextDouble() -ge 0.20

        $pRagioneSociale.Value  = $ragioneSociale
        $pPartitaIva.Value      = if ($null -eq $partitaIva) { [System.DBNull]::Value } else { $partitaIva }
        $pEmail.Value           = if ($null -eq $email) { [System.DBNull]::Value } else { $email }
        $pDataInserimento.Value = $dataInserimento
        $pAttivo.Value          = $attivo
        [void]$cmdCliente.ExecuteNonQuery()
    }
    $cmdCliente.Dispose()

    # --- tblOrdini: 1-8 ordini per circa 2/3 dei clienti
    $cmdOrdine = $connection.CreateCommand()
    $cmdOrdine.Transaction = $transaction
    $cmdOrdine.CommandText = 'INSERT INTO tblOrdini (IDCliente, DataOrdine, Importo, [Note]) VALUES (?, ?, ?, ?)'
    $pIdCliente  = $cmdOrdine.Parameters.Add('IDCliente', $OleDbType::Integer)
    $pDataOrdine = $cmdOrdine.Parameters.Add('DataOrdine', $OleDbType::Date)
    $pImporto    = $cmdOrdine.Parameters.Add('Importo', $OleDbType::Currency)
    $pNote       = $cmdOrdine.Parameters.Add('Note', $OleDbType::LongVarWChar)

    $totaleOrdini = 0
    # In un DB appena creato i COUNTER partono da 1: gli ID clienti sono 1..N
    for ($idCliente = 1; $idCliente -le $Clienti; $idCliente++) {
        if ($rnd.NextDouble() -lt 0.33) { continue }

        $numeroOrdini = $rnd.Next(1, 9)
        for ($k = 0; $k -lt $numeroOrdini; $k++) {
            $dataOrdine = $oggi.AddDays(-$rnd.Next(0, 730))
            $importo = [decimal]::Round([decimal]($rnd.NextDouble() * 14950 + 50), 2)
            $note = if ($rnd.NextDouble() -lt 0.4) { $noteOrdini[$rnd.Next($noteOrdini.Count)] -f $rnd.Next(1000, 9999) } else { $null }

            $pIdCliente.Value  = $idCliente
            $pDataOrdine.Value = $dataOrdine
            $pImporto.Value    = $importo
            $pNote.Value       = if ($null -eq $note) { [System.DBNull]::Value } else { $note }
            [void]$cmdOrdine.ExecuteNonQuery()
            $totaleOrdini++
        }
    }
    $cmdOrdine.Dispose()

    $transaction.Commit()
    $transaction = $null

    Write-Host "Dati inseriti : $Clienti clienti, $totaleOrdini ordini" -ForegroundColor Green
    Write-Host ''
    Write-Host 'Connection string per src\LegacyApp.Api\appsettings.json (ConnectionStrings:Access):'
    $jsonPath = $fullPath.Replace('\', '\\')
    Write-Host "  `"Provider=$Provider;Data Source=$jsonPath;Persist Security Info=False;`"" -ForegroundColor Cyan
}
catch {
    if ($null -ne $transaction) {
        try { $transaction.Rollback() } catch { }
    }
    throw
}
finally {
    $connection.Dispose()
}

# --- 3. Facoltativo: nasconde #Dual nell'interfaccia di Access (solo estetico) ------------------
$catalog = $null
try {
    $catalog = New-Object -ComObject ADOX.Catalog
    $catalog.ActiveConnection = $connectionString
    $catalog.Tables.Item('#Dual').Properties.Item('Jet OLEDB:Table Hidden In Access').Value = $true
}
catch {
    Write-Warning "Tabella #Dual non nascosta in Access (non influisce sul funzionamento): $($_.Exception.Message)"
}
finally {
    if ($null -ne $catalog) {
        try { $catalog.ActiveConnection.Close() } catch { }
        Clear-ComObject $catalog
    }
    [System.GC]::Collect()
    [System.GC]::WaitForPendingFinalizers()
}
