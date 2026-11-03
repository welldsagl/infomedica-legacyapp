Imports LegacyApp.Data.Entities
Imports Microsoft.EntityFrameworkCore

''' <summary>
''' DbContext dell'applicazione. Non contiene nulla di specifico di Access o SQL Server:
''' il provider viene scelto da fuori (DI, AppDbContextFactory o design-time factory).
''' </summary>
Partial Public Class AppDbContext
    Inherits DbContext

    Public Sub New(options As DbContextOptions(Of AppDbContext))
        MyBase.New(options)
    End Sub

    Public ReadOnly Property Clienti As DbSet(Of Cliente)
        Get
            Return [Set](Of Cliente)()
        End Get
    End Property

    Public ReadOnly Property Ordini As DbSet(Of Ordine)
        Get
            Return [Set](Of Ordine)()
        End Get
    End Property

    Protected Overrides Sub OnModelCreating(modelBuilder As ModelBuilder)
        ' Carica tutte le classi IEntityTypeConfiguration(Of T) della cartella Configurations.
        ' La mappatura sui nomi legacy (tblClienti, IDCliente, ...) vive li', non nelle entita'.
        modelBuilder.ApplyConfigurationsFromAssembly(GetType(AppDbContext).Assembly)

        ' Se serve una differenza per provider, si isola qui, ad esempio:
        '   If Database.ProviderName = "EntityFrameworkCore.Jet" Then ...
    End Sub

End Class
