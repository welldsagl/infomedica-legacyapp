Imports LegacyApp.Data
Imports Microsoft.AspNetCore.Builder
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports Microsoft.Extensions.Hosting

Public Module Program

    Public Sub Main(args As String())
        Dim builder = WebApplication.CreateBuilder(args)

        ' --- Layer dati: provider e connection string da configurazione ---------------
        ' NB: VB non distingue maiuscole/minuscole, quindi una variabile "dataOptions"
        '     nasconderebbe il tipo DataOptions. Da qui il nome "dbOptions".
        Dim dbOptions = If(builder.Configuration.GetSection(DataOptions.SectionName).Get(Of DataOptions)(),
                           New DataOptions())

        If String.IsNullOrWhiteSpace(dbOptions.ConnectionString) Then
            dbOptions.ConnectionString = builder.Configuration.GetConnectionString(dbOptions.Provider.ToString())
        End If

        builder.Services.AddAppData(dbOptions)

        ' --- Web API --------------------------------------------------------------
        builder.Services.AddControllers()
        builder.Services.AddProblemDetails()   ' errori in formato RFC 9457 (application/problem+json)
        builder.Services.AddOpenApi()          ' documento su /openapi/v1.json

        ' --- CORS: necessario solo per client browser su un'altra origine (es. la SPA React in produzione).
        '     In sviluppo la SPA passa dal proxy di Vite; la WinForms non e' soggetta a CORS.
        Dim corsOrigins = If(builder.Configuration.GetSection("Cors:AllowedOrigins").Get(Of String())(),
                             Array.Empty(Of String)())
        builder.Services.AddCors(
            Sub(cors) cors.AddDefaultPolicy(
                Sub(policy) policy.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("Location")))

        Dim app = builder.Build()

        app.UseExceptionHandler()
        app.UseCors()

        If app.Environment.IsDevelopment() Then
            app.MapOpenApi()
        End If

        app.MapControllers()

        app.Run()
    End Sub

End Module
