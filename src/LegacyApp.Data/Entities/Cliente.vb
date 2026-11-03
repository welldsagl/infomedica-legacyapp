Namespace Entities

    ''' <summary>
    ''' ESEMPIO: tabella legacy "tblClienti". Sostituire con le entita' reali
    ''' (scritte a mano o convertite dallo scaffold, vedi tools/scaffold).
    ''' Le entita' sono POCO puri: nessun attributo, nessun riferimento a EF.
    ''' </summary>
    Public Class Cliente

        Public Property Id As Integer
        Public Property RagioneSociale As String = String.Empty
        Public Property PartitaIva As String
        Public Property Email As String
        Public Property DataInserimento As Date
        Public Property Attivo As Boolean

        Public Property Ordini As ICollection(Of Ordine) = New List(Of Ordine)()

    End Class

End Namespace
