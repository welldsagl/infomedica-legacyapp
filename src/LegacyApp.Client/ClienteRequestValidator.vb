Imports System.Net.Mail
Imports System.Text.RegularExpressions
Imports LegacyApp.Client.Contracts

''' <summary>
''' Stesse regole dei DataAnnotations di ClienteRequest lato API: l'utente vede subito l'errore,
''' ma il server resta l'autorita' (rivalida comunque).
''' Le chiavi sono i nomi delle proprieta', come nelle risposte 400 dell'API.
''' </summary>
Public Module ClienteRequestValidator

    Private ReadOnly PartitaIvaPattern As New Regex("^\d{11}$", RegexOptions.Compiled)

    Public Function Validate(request As ClienteRequest) As IReadOnlyDictionary(Of String, String)
        ArgumentNullException.ThrowIfNull(request)

        Dim errors = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        If String.IsNullOrWhiteSpace(request.RagioneSociale) Then
            errors(NameOf(ClienteRequest.RagioneSociale)) = "La ragione sociale è obbligatoria."
        ElseIf request.RagioneSociale.Length > 255 Then
            errors(NameOf(ClienteRequest.RagioneSociale)) = "Massimo 255 caratteri."
        End If

        If request.PartitaIva IsNot Nothing AndAlso Not PartitaIvaPattern.IsMatch(request.PartitaIva) Then
            errors(NameOf(ClienteRequest.PartitaIva)) = "La partita IVA deve avere 11 cifre."
        End If

        If request.Email IsNot Nothing Then
            Dim parsed As MailAddress = Nothing
            If request.Email.Length > 255 OrElse Not MailAddress.TryCreate(request.Email, parsed) Then
                errors(NameOf(ClienteRequest.Email)) = "Indirizzo email non valido."
            End If
        End If

        Return errors
    End Function

End Module
