Namespace Contracts

    ''' <summary>Corpo degli errori RFC 9457 restituiti da ASP.NET Core.</summary>
    Public Class ProblemDetailsDto
        Public Property Title As String
        Public Property Detail As String
        Public Property Status As Integer?
        Public Property Errors As Dictionary(Of String, String())
    End Class

End Namespace
