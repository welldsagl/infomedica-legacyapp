using LegacyApp.Blazor.Server.Components;
using LegacyApp.Blazor.Shared;
using LegacyApp.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Client tipizzato dell'API (VB): un HttpClient gestito da IHttpClientFactory (pooling, DNS refresh).
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5080/";
builder.Services.AddHttpClient<LegacyApiClient>(http =>
{
    http.BaseAddress = new Uri(apiBaseUrl);
    http.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(UiAssembly).Assembly);   // le @page stanno nella libreria condivisa

app.Run();
