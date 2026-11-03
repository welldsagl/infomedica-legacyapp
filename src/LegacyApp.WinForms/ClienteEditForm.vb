Imports System.Net.Http
Imports LegacyApp.Client
Imports LegacyApp.Client.Contracts

''' <summary>
''' Dialog di creazione/modifica. Salva da sola, cosi' in caso di errore di validazione
''' (client o server) resta aperta e mostra l'errore accanto al campo.
''' </summary>
Public Class ClienteEditForm

    Private ReadOnly _api As LegacyApiClient
    Private ReadOnly _clienteId As Integer?

    ''' <summary>Cliente creato (solo in creazione, dopo DialogResult.OK).</summary>
    Public ReadOnly Property Risultato As ClienteDto

    ''' <param name="cliente">Nothing = nuovo cliente; altrimenti il cliente da modificare.</param>
    Public Sub New(api As LegacyApiClient, cliente As ClienteDto)
        InitializeComponent()
        _api = api

        If cliente Is Nothing Then
            Text = "Nuovo cliente"
        Else
            _clienteId = cliente.Id
            Text = $"Modifica cliente #{cliente.Id}"
            txtRagioneSociale.Text = cliente.RagioneSociale
            txtPartitaIva.Text = cliente.PartitaIva
            txtEmail.Text = cliente.Email
            chkAttivo.Checked = cliente.Attivo
        End If
    End Sub

    Private Async Sub btnSalva_Click(sender As Object, e As EventArgs) Handles btnSalva.Click
        errValidazione.Clear()

        Dim request = ClienteRequest.Create(txtRagioneSociale.Text, txtPartitaIva.Text, txtEmail.Text, chkAttivo.Checked)

        ' 1. Validazione locale: feedback immediato, senza chiamare l'API
        Dim erroriLocali = ClienteRequestValidator.Validate(request)
        If erroriLocali.Count > 0 Then
            MostraErrori(erroriLocali.Select(Function(kv) (kv.Key, kv.Value)))
            Return
        End If

        ' 2. Salvataggio: il server rivalida e puo' restituire 400 con errori per campo
        ImpostaOccupato(True)
        Try
            If _clienteId.HasValue Then
                Await _api.UpdateClienteAsync(_clienteId.Value, request)
            Else
                _Risultato = Await _api.CreateClienteAsync(request)
            End If
            DialogResult = DialogResult.OK
            Close()

        Catch ex As ApiException When ex.IsValidationError
            MostraErrori(ex.ValidationErrors.Select(Function(kv) (kv.Key, kv.Value.FirstOrDefault())))
        Catch ex As ApiException
            MessageBox.Show(Me, ex.Message, $"Errore dall'API ({ex.StatusCode})", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As HttpRequestException
            MessageBox.Show(Me, "Impossibile contattare l'API: " & ex.Message, "Connessione non riuscita", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As TaskCanceledException
            MessageBox.Show(Me, "L'API non ha risposto in tempo.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ImpostaOccupato(False)
        End Try
    End Sub

    ''' <summary>Mostra gli errori (chiave = nome proprieta' di ClienteRequest) accanto ai campi.</summary>
    Private Sub MostraErrori(errori As IEnumerable(Of (Campo As String, Messaggio As String)))
        Dim primo As Control = Nothing

        For Each errore In errori
            Dim controllo = ControlloPerCampo(errore.Campo)
            If controllo Is Nothing Then
                ' Errore non legato a un campo: lo si mostra sul pulsante Salva
                errValidazione.SetError(btnSalva, errore.Messaggio)
                Continue For
            End If
            errValidazione.SetError(controllo, errore.Messaggio)
            If primo Is Nothing Then primo = controllo
        Next

        primo?.Focus()
    End Sub

    Private Function ControlloPerCampo(campo As String) As Control
        Select Case campo?.ToLowerInvariant()
            Case "ragionesociale" : Return txtRagioneSociale
            Case "partitaiva" : Return txtPartitaIva
            Case "email" : Return txtEmail
            Case "attivo" : Return chkAttivo
            Case Else : Return Nothing
        End Select
    End Function

    Private Sub ImpostaOccupato(occupato As Boolean)
        UseWaitCursor = occupato
        btnSalva.Enabled = Not occupato
        layoutCampi.Enabled = Not occupato
    End Sub

End Class
