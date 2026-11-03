Namespace Contracts

    Public Class PagedResult(Of T)

        Public Sub New(items As IReadOnlyList(Of T), page As Integer, pageSize As Integer, totalCount As Integer)
            Me.Items = items
            Me.Page = page
            Me.PageSize = pageSize
            Me.TotalCount = totalCount
        End Sub

        Public ReadOnly Property Items As IReadOnlyList(Of T)
        Public ReadOnly Property Page As Integer
        Public ReadOnly Property PageSize As Integer
        Public ReadOnly Property TotalCount As Integer

    End Class

End Namespace
