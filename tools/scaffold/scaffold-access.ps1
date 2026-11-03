<#
.SYNOPSIS
  Reverse engineering (database-first) del file Access in un progetto C# TEMPORANEO.

.DESCRIPTION
  EF Core 10 genera codice solo in C#; il pacchetto EntityFrameworkCore.VisualBasic
  e' fermo a EF Core 8. Quindi:
    1. questo script genera entita' e mappature in C# in artifacts\scaffold-cs
    2. il codice si converte in VB (estensione "Code Converter (VB - C#)" di IC#Code
       per Visual Studio, oppure a mano: sono POCO e Fluent API, la conversione e' meccanica)
    3. si copiano le entita' in src\LegacyApp.Data\Entities e la Fluent API
       nelle classi IEntityTypeConfiguration di src\LegacyApp.Data\Configurations

  Il progetto temporaneo non fa parte della soluzione e si puo' cancellare.

.PARAMETER AccessFile
  Percorso del file .accdb / .mdb.

.PARAMETER Tables
  Tabelle da importare (opzionale). Se omesso, importa tutte le tabelle.

.EXAMPLE
  .\scaffold-access.ps1 -AccessFile C:\Dati\Legacy.accdb
  .\scaffold-access.ps1 -AccessFile C:\Dati\Legacy.accdb -Tables tblClienti,tblOrdini
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $AccessFile,

    [string[]] $Tables = @(),

    [string] $OutDir = (Join-Path $PSScriptRoot '..\..\artifacts\scaffold-cs'),

    [string] $JetVersion = '10.0.1',
    [string] $EfVersion  = '10.0.3'
)

$ErrorActionPreference = 'Stop'

$accessPath = (Resolve-Path $AccessFile).Path
Write-Host "File Access : $accessPath"
Write-Host "Processo x64: $([Environment]::Is64BitProcess)  (deve coincidere con la bitness del driver ACE)"

# 1. Strumento dotnet-ef allineato a EF Core 10
dotnet tool update --global dotnet-ef --version $EfVersion
if ($LASTEXITCODE -ne 0) { throw 'Installazione dotnet-ef fallita' }

# 2. Progetto C# usa-e-getta
New-Item -ItemType Directory -Force -Path $OutDir | Out-Null
$project = Join-Path $OutDir 'ScaffoldTmp'
dotnet new classlib --language 'C#' --framework net10.0 --output $project --force
Remove-Item (Join-Path $project 'Class1.cs') -ErrorAction SilentlyContinue

# Il progetto temporaneo non deve usare la Central Package Management della soluzione
@'
<Project>
  <PropertyGroup><ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally></PropertyGroup>
</Project>
'@ | Set-Content (Join-Path $OutDir 'Directory.Packages.props')
'<Project></Project>' | Set-Content (Join-Path $OutDir 'Directory.Build.props')

Push-Location $project
try {
    dotnet add package EntityFrameworkCore.Jet.OleDb --version $JetVersion
    dotnet add package Microsoft.EntityFrameworkCore.Design --version $EfVersion

    $connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$accessPath;Persist Security Info=False;"

    $efArgs = @(
        'ef', 'dbcontext', 'scaffold', $connectionString, 'EntityFrameworkCore.Jet',
        '--context', 'AppDbContext',
        '--output-dir', 'Entities',
        '--context-dir', '.',
        '--no-onconfiguring',   # niente connection string nel codice generato
        '--no-pluralize',
        '--force'
        # '--use-database-names' # decommentare per mantenere i nomi legacy (tblClienti, IDCliente, ...)
    )
    foreach ($t in $Tables) { $efArgs += @('--table', $t) }

    dotnet @efArgs
    if ($LASTEXITCODE -ne 0) { throw 'Scaffold fallito (controllare driver ACE e bitness)' }

    Write-Host ''
    Write-Host "Codice C# generato in: $project" -ForegroundColor Green
    Write-Host 'Prossimi passi: convertire in VB e spostare entita'' e Fluent API in LegacyApp.Data (vedi README).'
}
finally {
    Pop-Location
}
