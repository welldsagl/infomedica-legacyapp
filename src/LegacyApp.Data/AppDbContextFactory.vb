Imports Microsoft.EntityFrameworkCore

''' <summary>
''' Crea un AppDbContext senza Dependency Injection.
''' Utile nel codice WinForms legacy (una volta portato su .NET 10):
'''
'''   Using db = AppDbContextFactory.Create(DatabaseProvider.Access, My.Settings.AccessConnection)
'''       Dim attivi = db.Clienti.Where(Function(c) c.Attivo).ToList()
'''   End Using
'''
''' Regola d'oro: un DbContext per unita' di lavoro (form, operazione), mai condiviso a lungo.
''' </summary>
Public NotInheritable Class AppDbContextFactory

    Private Sub New()
    End Sub

    Public Shared Function Create(provider As DatabaseProvider, connectionString As String,
                                  Optional jetDualTable As String = Nothing) As AppDbContext
        Dim builder = New DbContextOptionsBuilder(Of AppDbContext)()
        builder.UseAppDatabase(provider, connectionString, jetDualTable)
        Return New AppDbContext(builder.Options)
    End Function

    Public Shared Function Create(options As DataOptions) As AppDbContext
        ArgumentNullException.ThrowIfNull(options)
        Return Create(options.Provider, options.ConnectionString, options.JetDualTable)
    End Function

End Class
