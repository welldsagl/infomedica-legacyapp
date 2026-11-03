Imports System.ComponentModel.DataAnnotations
Imports System.Linq.Expressions
Imports LegacyApp.Data.Entities

Namespace Contracts

    ''' <summary>
    ''' Contratto REST in lettura. Le entita' EF non vengono mai esposte direttamente:
    ''' cosi' lo schema del DB (legacy o nuovo) puo' cambiare senza rompere i client.
    ''' </summary>
    Public Class ClienteDto

        Public Property Id As Integer
        Public Property RagioneSociale As String
        Public Property PartitaIva As String
        Public Property Email As String
        Public Property DataInserimento As Date
        Public Property Attivo As Boolean

        ''' <summary>
        ''' Proiezione LINQ tradotta in SQL dal provider: vengono lette solo le colonne necessarie.
        ''' </summary>
        Public Shared ReadOnly Projection As Expression(Of Func(Of Cliente, ClienteDto)) =
            Function(c) New ClienteDto With {
                .Id = c.Id,
                .RagioneSociale = c.RagioneSociale,
                .PartitaIva = c.PartitaIva,
                .Email = c.Email,
                .DataInserimento = c.DataInserimento,
                .Attivo = c.Attivo
            }

        ' Compilata una sola volta (i campi Shared si inizializzano in ordine di dichiarazione).
        Private Shared ReadOnly _fromEntity As Func(Of Cliente, ClienteDto) = Projection.Compile()

        ''' <summary>Stessa mappatura, in memoria (es. dopo un insert).</summary>
        Public Shared Function FromEntity(entity As Cliente) As ClienteDto
            Return _fromEntity(entity)
        End Function

    End Class

    ''' <summary>Contratto REST in scrittura (POST/PUT), validato da [ApiController].</summary>
    Public Class ClienteRequest

        <Required, StringLength(255)>
        Public Property RagioneSociale As String

        <StringLength(11, MinimumLength:=11)>
        Public Property PartitaIva As String

        <EmailAddress, StringLength(255)>
        Public Property Email As String

        Public Property Attivo As Boolean = True

    End Class

End Namespace
