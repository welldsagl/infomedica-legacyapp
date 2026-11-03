Namespace Contracts

    ''' <summary>Corpo di POST/PUT api/clienti.</summary>
    Public Class ClienteRequest
        Public Property RagioneSociale As String = String.Empty
        Public Property PartitaIva As String
        Public Property Email As String
        Public Property Attivo As Boolean = True

        ''' <summary>Costruisce la richiesta normalizzando l'input: trim e stringhe vuote -> Nothing.</summary>
        Public Shared Function Create(ragioneSociale As String, partitaIva As String, email As String, attivo As Boolean) As ClienteRequest
            Return New ClienteRequest With {
                .RagioneSociale = If(ragioneSociale, String.Empty).Trim(),
                .PartitaIva = EmptyToNothing(partitaIva),
                .Email = EmptyToNothing(email),
                .Attivo = attivo
            }
        End Function

        Private Shared Function EmptyToNothing(value As String) As String
            Dim trimmed = If(value, String.Empty).Trim()
            Return If(trimmed.Length = 0, Nothing, trimmed)
        End Function
    End Class

End Namespace
