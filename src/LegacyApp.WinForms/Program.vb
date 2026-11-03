Imports System.Net.Http
Imports LegacyApp.Client

Public Module Program

    <STAThread>
    Public Sub Main()
        Application.SetHighDpiMode(HighDpiMode.SystemAware)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim settings = ClientSettings.Load()

        ' Un solo HttpClient per tutta l'applicazione (riusa le connessioni).
        ' PooledConnectionLifetime: rilegge il DNS se il server cambia indirizzo.
        Dim handler = New SocketsHttpHandler With {.PooledConnectionLifetime = TimeSpan.FromMinutes(5)}
        Using http = New HttpClient(handler) With {
            .BaseAddress = New Uri(settings.ApiBaseUrl),
            .Timeout = TimeSpan.FromSeconds(30)
        }
            Dim api = New LegacyApiClient(http)
            Application.Run(New MainForm(api))
        End Using
    End Sub

End Module
