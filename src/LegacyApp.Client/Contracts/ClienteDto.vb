Namespace Contracts

    ''' <summary>Specchio di LegacyApp.Api.Contracts.ClienteDto (JSON camelCase).</summary>
    Public Class ClienteDto
        Public Property Id As Integer
        Public Property RagioneSociale As String = String.Empty
        Public Property PartitaIva As String
        Public Property Email As String
        Public Property DataInserimento As Date
        Public Property Attivo As Boolean
    End Class

End Namespace
