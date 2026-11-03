Namespace Contracts

    Public Class PagedResult(Of T)
        Public Property Items As List(Of T) = New List(Of T)()
        Public Property Page As Integer
        Public Property PageSize As Integer
        Public Property TotalCount As Integer

        Public ReadOnly Property TotalPages As Integer
            Get
                If PageSize <= 0 Then Return 1
                Return Math.Max(1, CInt(Math.Ceiling(TotalCount / PageSize)))
            End Get
        End Property
    End Class

End Namespace
