Imports LegacyApp.Data.Entities
Imports Microsoft.EntityFrameworkCore
Imports Microsoft.EntityFrameworkCore.Metadata.Builders

Namespace Configurations

    Public NotInheritable Class OrdineConfiguration
        Implements IEntityTypeConfiguration(Of Ordine)

        Public Sub Configure(builder As EntityTypeBuilder(Of Ordine)) _
            Implements IEntityTypeConfiguration(Of Ordine).Configure

            builder.ToTable("tblOrdini")

            builder.HasKey(Function(o) o.Id)

            builder.Property(Function(o) o.Id) _
                .HasColumnName("IDOrdine") _
                .ValueGeneratedOnAdd()

            builder.Property(Function(o) o.ClienteId) _
                .HasColumnName("IDCliente")

            builder.Property(Function(o) o.DataOrdine) _
                .HasColumnName("DataOrdine")

            ' Precisione esplicita: vale sia per Currency (Access) sia per decimal(19,4) (SQL Server)
            builder.Property(Function(o) o.Importo) _
                .HasColumnName("Importo") _
                .HasPrecision(19, 4)

            ' Nessuna lunghezza massima: Memo / nvarchar(max)
            builder.Property(Function(o) o.Note) _
                .HasColumnName("Note")

            builder.HasOne(Function(o) o.Cliente) _
                .WithMany(Function(c) c.Ordini) _
                .HasForeignKey(Function(o) o.ClienteId) _
                .OnDelete(DeleteBehavior.Restrict)
        End Sub

    End Class

End Namespace
