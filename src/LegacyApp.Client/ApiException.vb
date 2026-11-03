Imports LegacyApp.Client.Contracts

''' <summary>
''' Errore restituito dall'API (status 4xx/5xx) con il contenuto ProblemDetails gia' interpretato.
''' </summary>
Public Class ApiException
    Inherits Exception

    Private Shared ReadOnly NoErrors As IReadOnlyDictionary(Of String, String()) =
        New Dictionary(Of String, String())()

    Public Sub New(statusCode As Integer, problem As ProblemDetailsDto)
        MyBase.New(BuildMessage(statusCode, problem))
        Me.StatusCode = statusCode
        Title = problem?.Title
        Detail = problem?.Detail
        ValidationErrors = If(problem?.Errors Is Nothing,
                              NoErrors,
                              New Dictionary(Of String, String())(problem.Errors, StringComparer.OrdinalIgnoreCase))
    End Sub

    Public ReadOnly Property StatusCode As Integer
    Public ReadOnly Property Title As String
    Public ReadOnly Property Detail As String

    ''' <summary>Errori per campo (chiave = nome proprieta', es. "PartitaIva"), solo per le risposte 400.</summary>
    Public ReadOnly Property ValidationErrors As IReadOnlyDictionary(Of String, String())

    Public ReadOnly Property IsValidationError As Boolean
        Get
            Return StatusCode = 400 AndAlso ValidationErrors.Count > 0
        End Get
    End Property

    Private Shared Function BuildMessage(statusCode As Integer, problem As ProblemDetailsDto) As String
        If Not String.IsNullOrWhiteSpace(problem?.Detail) Then Return problem.Detail
        If Not String.IsNullOrWhiteSpace(problem?.Title) Then Return problem.Title
        Return $"Errore HTTP {statusCode}"
    End Function

End Class
