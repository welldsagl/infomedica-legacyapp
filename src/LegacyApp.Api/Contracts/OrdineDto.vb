Imports System.Linq.Expressions
Imports LegacyApp.Data.Entities

Namespace Contracts

    Public Class OrdineDto

        Public Property Id As Integer
        Public Property ClienteId As Integer
        Public Property DataOrdine As Date
        Public Property Importo As Decimal
        Public Property Note As String

        Public Shared ReadOnly Projection As Expression(Of Func(Of Ordine, OrdineDto)) =
            Function(o) New OrdineDto With {
                .Id = o.Id,
                .ClienteId = o.ClienteId,
                .DataOrdine = o.DataOrdine,
                .Importo = o.Importo,
                .Note = o.Note
            }

    End Class

End Namespace
