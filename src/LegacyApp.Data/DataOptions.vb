''' <summary>
''' Opzioni del layer dati, lette dalla sezione "Data" di appsettings.json.
''' </summary>
Public NotInheritable Class DataOptions

    Public Const SectionName As String = "Data"

    ''' <summary>Access (default) oppure SqlServer.</summary>
    Public Property Provider As DatabaseProvider = DatabaseProvider.Access

    ''' <summary>
    ''' Connection string esplicita. Se vuota, l'host usa
    ''' ConnectionStrings:{Provider} (es. ConnectionStrings:Access).
    ''' </summary>
    Public Property ConnectionString As String = String.Empty

    ''' <summary>
    ''' Solo Access. Tabella "DUAL" usata dal provider Jet per le query senza FROM (es. EXISTS da AnyAsync).
    ''' Vuoto = "#Dual" (da creare con tools\access\ensure-dual-table.ps1).
    ''' Se il database non si puo' modificare, indicare una tabella esistente qualsiasi
    ''' (meglio se piccola): il provider la usa solo dentro "SELECT COUNT(*) FROM tabella".
    ''' </summary>
    Public Property JetDualTable As String = String.Empty

    ''' <summary>Logga i valori dei parametri SQL. Solo in sviluppo.</summary>
    Public Property EnableSensitiveDataLogging As Boolean

End Class
