Imports LegacyApp.Data
Imports Microsoft.AspNetCore.Http
Imports Microsoft.AspNetCore.Mvc

Namespace Controllers

    ''' <summary>
    ''' Verifica rapida della connessione: utile per controllare driver ACE, bitness e percorso del file .accdb.
    ''' </summary>
    <ApiController>
    <Route("api/health")>
    Public Class HealthController
        Inherits ControllerBase

        Private ReadOnly _db As AppDbContext
        Private ReadOnly _options As DataOptions

        Public Sub New(db As AppDbContext, options As DataOptions)
            _db = db
            _options = options
        End Sub

        <HttpGet>
        Public Async Function [Get](Optional cancellationToken As CancellationToken = Nothing) As Task(Of IActionResult)
            Dim connesso = Await _db.Database.CanConnectAsync(cancellationToken)

            Dim body = New With {
                .Status = If(connesso, "ok", "unreachable"),
                .Provider = _options.Provider.ToString(),
                .Is64BitProcess = Environment.Is64BitProcess
            }

            If connesso Then Return Ok(body)
            Return StatusCode(StatusCodes.Status503ServiceUnavailable, body)
        End Function

    End Class

End Namespace
