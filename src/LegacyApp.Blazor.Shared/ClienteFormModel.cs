using LegacyApp.Client.Contracts;

namespace LegacyApp.Blazor.Shared;

/// <summary>Stato del form (stringhe "grezze" dei campi); ClienteRequest viene costruita solo al submit.</summary>
public sealed class ClienteFormModel
{
    public string RagioneSociale { get; set; } = string.Empty;
    public string PartitaIva { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Attivo { get; set; } = true;

    public static ClienteFormModel FromDto(ClienteDto dto) => new()
    {
        RagioneSociale = dto.RagioneSociale,
        PartitaIva = dto.PartitaIva ?? string.Empty,
        Email = dto.Email ?? string.Empty,
        Attivo = dto.Attivo,
    };

    /// <summary>Normalizzazione (trim, vuoto -> null) delegata al client VB: una sola regola per tutti i frontend.</summary>
    public ClienteRequest ToRequest() => ClienteRequest.Create(RagioneSociale, PartitaIva, Email, Attivo);
}
