Imports Microsoft.EntityFrameworkCore.Design

''' <summary>
''' Usata solo dagli strumenti "dotnet ef" (es. "dotnet ef dbcontext script").
''' Legge provider e connection string da variabili d'ambiente:
'''   LEGACYAPP_DB_PROVIDER    = Access | SqlServer   (default SqlServer)
'''   LEGACYAPP_DB_CONNECTION  = connection string     (default: SQL Server locale)
''' </summary>
Public NotInheritable Class DesignTimeDbContextFactory
    Implements IDesignTimeDbContextFactory(Of AppDbContext)

    Public Function CreateDbContext(args As String()) As AppDbContext _
        Implements IDesignTimeDbContextFactory(Of AppDbContext).CreateDbContext

        Dim provider = DatabaseProvider.SqlServer
        Dim providerText = Environment.GetEnvironmentVariable("LEGACYAPP_DB_PROVIDER")
        If Not String.IsNullOrWhiteSpace(providerText) Then
            provider = [Enum].Parse(Of DatabaseProvider)(providerText, ignoreCase:=True)
        End If

        Dim connectionString = Environment.GetEnvironmentVariable("LEGACYAPP_DB_CONNECTION")
        If String.IsNullOrWhiteSpace(connectionString) Then
            connectionString = "Server=localhost;Database=LegacyApp;Trusted_Connection=True;TrustServerCertificate=True;"
        End If

        Return AppDbContextFactory.Create(provider, connectionString)
    End Function

End Class
