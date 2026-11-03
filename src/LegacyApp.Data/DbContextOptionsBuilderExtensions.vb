Imports System.Runtime.CompilerServices
Imports EntityFrameworkCore.Jet.Data
Imports Microsoft.EntityFrameworkCore

''' <summary>
''' L'UNICO punto della soluzione che conosce i provider concreti.
''' Il passaggio Access -> SQL Server si fa cambiando Provider e connection string in configurazione.
''' </summary>
Public Module DbContextOptionsBuilderExtensions

    <Extension>
    Public Function UseAppDatabase(builder As DbContextOptionsBuilder,
                                   provider As DatabaseProvider,
                                   connectionString As String,
                                   Optional jetDualTable As String = Nothing) As DbContextOptionsBuilder

        ArgumentNullException.ThrowIfNull(builder)

        If String.IsNullOrWhiteSpace(connectionString) Then
            Throw New InvalidOperationException(
                $"Connection string mancante per il provider '{provider}'. " &
                $"Valorizzare Data:ConnectionString oppure ConnectionStrings:{provider}.")
        End If

        Select Case provider
            Case DatabaseProvider.Access
                ' "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=...accdb"
                ' Il provider Jet riconosce OLE DB o ODBC dalla connection string.
                '
                ' Access non ha una tabella DUAL: per le query senza FROM (es. EXISTS generati da AnyAsync)
                ' il provider genera "FROM (SELECT COUNT(*) FROM `#Dual`)". Sui DB legacy "#Dual" non esiste:
                ' o la si crea (tools\access\ensure-dual-table.ps1) o si indica qui una tabella esistente.
                ' NB: e' un'impostazione statica del provider (vale per tutto il processo).
                If Not String.IsNullOrWhiteSpace(jetDualTable) Then
                    JetConfiguration.CustomDualTableName = jetDualTable
                End If
                Return builder.UseJet(connectionString)

            Case DatabaseProvider.SqlServer
                Return builder.UseSqlServer(connectionString)

            Case Else
                Throw New NotSupportedException($"Provider non supportato: {provider}")
        End Select
    End Function

End Module
