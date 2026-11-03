using System.Globalization;

namespace LegacyApp.Blazor.Shared;

/// <summary>Stessi formati del frontend React (utils/format.ts): date e importi in italiano.</summary>
public static class Formatting
{
    private static readonly CultureInfo It = CultureInfo.GetCultureInfo("it-IT");

    public static string Data(DateTime value) => value.ToString("d", It);

    public static string DataOra(DateTime value) => value.ToString("g", It);

    public static string Importo(decimal value) => value.ToString("C", It);

    public static string OrDash(string? value) => string.IsNullOrEmpty(value) ? "—" : value;
}
