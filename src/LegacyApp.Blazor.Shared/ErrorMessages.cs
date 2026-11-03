using LegacyApp.Client;

namespace LegacyApp.Blazor.Shared;

/// <summary>Eccezione del client API -> messaggio per l'utente (equivalente di utils/apiErrors.ts).</summary>
public static class ErrorMessages
{
    public static string From(Exception ex) => ex switch
    {
        ApiException { StatusCode: 404 } => "Elemento non trovato.",
        ApiException api => api.Message,
        HttpRequestException => "API non raggiungibile.",
        TaskCanceledException => "Timeout: l'API non ha risposto in tempo.",
        _ => ex.Message,
    };

    /// <summary>Errori per campo di una risposta 400: primo messaggio per ogni proprieta'.</summary>
    public static IReadOnlyDictionary<string, string> FieldErrors(ApiException ex) =>
        ex.ValidationErrors.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.FirstOrDefault() ?? "Valore non valido.",
            StringComparer.OrdinalIgnoreCase);
}
