Imports LegacyApp.Data.Entities
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace Configurations

    ''' <summary>
    ''' Mappatura Fluent API: nomi di classi e proprieta' "puliti" verso i nomi legacy del DB.
    ''' Se in SQL Server rinominerete tabelle e colonne, cambia solo questo file.
    ''' </summary>
    Public NotInheritable Class ClienteConfiguration
        Implements IEntityTypeConfiguration(Of Cliente)

        Public Sub Configure(builder As EntityTypeBuilder(Of Cliente)) _
            Implements IEntityTypeConfiguration(Of Cliente).Configure

            builder.ToTable("tblClienti")

            builder.HasKey(Function(c) c.Id)

            ' Contatore (AutoNumber) in Access, IDENTITY in SQL Server
            builder.Property(Function(c) c.Id) _
                .HasColumnName("IDCliente") _
                .ValueGeneratedOnAdd()

            builder.Property(Function(c) c.RagioneSociale) _
                .HasColumnName("RagioneSociale") _
                .HasMaxLength(255) _
                .IsRequired()

            builder.Property(Function(c) c.PartitaIva) _
                .HasColumnName("PartitaIVA") _
                .HasMaxLength(11)

            builder.Property(Function(c) c.Email) _
                .HasColumnName("Email") _
                .HasMaxLength(255)

            builder.Property(Function(c) c.DataInserimento) _
                .HasColumnName("DataInserimento")

            ' Si/No in Access, bit in SQL Server
            builder.Property(Function(c) c.Attivo) _
                .HasColumnName("Attivo")

            builder.HasIndex(Function(c) c.PartitaIva)
        End Sub

    End Class

End Namespace
