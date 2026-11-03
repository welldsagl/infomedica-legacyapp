Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Json
Imports System.Text.Json
Imports LegacyApp.Client.Contracts

''' <summary>
''' Client tipizzato di LegacyApp.Api.
''' Tutti i metodi lanciano ApiException per le risposte di errore e HttpRequestException
''' se l'API non e' raggiungibile.
''' L'HttpClient va creato UNA volta per applicazione (vedi Program.vb della WinForms).
''' </summary>
Public NotInheritable Class LegacyApiClient

    ' camelCase + nomi case-insensitive: lo stesso formato usato da ASP.NET Core
    Private Shared ReadOnly JsonOptions As New JsonSerializerOptions(JsonSerializerDefaults.Web)

    Private ReadOnly _http As HttpClient

    Public Sub New(http As HttpClient)
        ArgumentNullException.ThrowIfNull(http)
        If http.BaseAddress Is Nothing Then
            Throw New ArgumentException("HttpClient.BaseAddress e' obbligatorio.", NameOf(http))
        End If
        _http = http
    End Sub

    Public ReadOnly Property BaseAddress As Uri
        Get
            Return _http.BaseAddress
        End Get
    End Property

    ' --- Letture --------------------------------------------------------------

    Public Function GetHealthAsync(Optional cancellationToken As CancellationToken = Nothing) As Task(Of HealthDto)
        Return GetJsonAsync(Of HealthDto)("api/health", cancellationToken)
    End Function

    Public Function GetClientiAsync(page As Integer, pageSize As Integer,
                                    Optional cancellationToken As CancellationToken = Nothing) As Task(Of PagedResult(Of ClienteDto))
        Return GetJsonAsync(Of PagedResult(Of ClienteDto))($"api/clienti?page={page}&pageSize={pageSize}", cancellationToken)
    End Function

    ''' <summary>Restituisce Nothing se il cliente non esiste (404).</summary>
    Public Async Function GetClienteAsync(id As Integer,
                                          Optional cancellationToken As CancellationToken = Nothing) As Task(Of ClienteDto)
        Using response = Await _http.GetAsync($"api/clienti/{id}", cancellationToken)
            If response.StatusCode = HttpStatusCode.NotFound Then Return Nothing
            Await EnsureSuccessAsync(response, cancellationToken)
            Return Await response.Content.ReadFromJsonAsync(Of ClienteDto)(JsonOptions, cancellationToken)
        End Using
    End Function

    Public Async Function GetOrdiniAsync(clienteId As Integer,
                                         Optional cancellationToken As CancellationToken = Nothing) As Task(Of IReadOnlyList(Of OrdineDto))
        Dim ordini = Await GetJsonAsync(Of List(Of OrdineDto))($"api/clienti/{clienteId}/ordini", cancellationToken)
        Return If(ordini, New List(Of OrdineDto)())
    End Function

    ' --- Scritture ------------------------------------------------------------

    Public Async Function CreateClienteAsync(request As ClienteRequest,
                                             Optional cancellationToken As CancellationToken = Nothing) As Task(Of ClienteDto)
        Using response = Await _http.PostAsJsonAsync("api/clienti", request, JsonOptions, cancellationToken)
            Await EnsureSuccessAsync(response, cancellationToken)
            Return Await response.Content.ReadFromJsonAsync(Of ClienteDto)(JsonOptions, cancellationToken)
        End Using
    End Function

    Public Async Function UpdateClienteAsync(id As Integer, request As ClienteRequest,
                                             Optional cancellationToken As CancellationToken = Nothing) As Task
        Using response = Await _http.PutAsJsonAsync($"api/clienti/{id}", request, JsonOptions, cancellationToken)
            Await EnsureSuccessAsync(response, cancellationToken)
        End Using
    End Function

    Public Async Function DeleteClienteAsync(id As Integer,
                                             Optional cancellationToken As CancellationToken = Nothing) As Task
        Using response = Await _http.DeleteAsync($"api/clienti/{id}", cancellationToken)
            Await EnsureSuccessAsync(response, cancellationToken)
        End Using
    End Function

    ' --- Infrastruttura -------------------------------------------------------

    Private Async Function GetJsonAsync(Of T)(path As String, cancellationToken As CancellationToken) As Task(Of T)
        Using response = Await _http.GetAsync(path, cancellationToken)
            Await EnsureSuccessAsync(response, cancellationToken)
            Return Await response.Content.ReadFromJsonAsync(Of T)(JsonOptions, cancellationToken)
        End Using
    End Function

    Private Shared Async Function EnsureSuccessAsync(response As HttpResponseMessage,
                                                     cancellationToken As CancellationToken) As Task
        If response.IsSuccessStatusCode Then Return

        Dim problem As ProblemDetailsDto = Nothing
        Try
            problem = Await response.Content.ReadFromJsonAsync(Of ProblemDetailsDto)(JsonOptions, cancellationToken)
        Catch ex As JsonException
            ' corpo assente o non JSON: si usa solo lo status code
        Catch ex As NotSupportedException
            ' content-type non JSON
        End Try

        Throw New ApiException(CInt(response.StatusCode), problem)
    End Function

End Class
