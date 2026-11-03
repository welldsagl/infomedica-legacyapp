using LegacyApp.Blazor.Wasm;
using LegacyApp.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// wwwroot/appsettings.json viene caricato automaticamente nella Configuration.
// Senza ApiBaseUrl si assume API e app sulla stessa origine (es. dietro un reverse proxy).
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl),
    Timeout = TimeSpan.FromSeconds(30),
});
builder.Services.AddScoped<LegacyApiClient>();   // riceve l'HttpClient dal costruttore

await builder.Build().RunAsync();
