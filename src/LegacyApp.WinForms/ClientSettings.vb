Imports System.IO
Imports System.Text.Json

''' <summary>
''' Impostazioni lette da appsettings.json accanto all'eseguibile.
''' La variabile d'ambiente LEGACYAPP_API_URL ha la precedenza (utile per puntare a un altro server).
''' </summary>
Public NotInheritable Class ClientSettings

    Public Property ApiBaseUrl As String = "http://localhost:5080/"

    Public Shared Function Load() As ClientSettings
        Dim settings = New ClientSettings()
        Dim path = IO.Path.Combine(AppContext.BaseDirectory, "appsettings.json")

        If File.Exists(path) Then
            Dim options = New JsonSerializerOptions With {.PropertyNameCaseInsensitive = True}
            settings = If(JsonSerializer.Deserialize(Of ClientSettings)(File.ReadAllText(path), options), settings)
        End If

        Dim fromEnv = Environment.GetEnvironmentVariable("LEGACYAPP_API_URL")
        If Not String.IsNullOrWhiteSpace(fromEnv) Then settings.ApiBaseUrl = fromEnv

        ' HttpClient.BaseAddress richiede la "/" finale per comporre i percorsi relativi
        If Not settings.ApiBaseUrl.EndsWith("/"c) Then settings.ApiBaseUrl &= "/"
        Return settings
    End Function

End Class
