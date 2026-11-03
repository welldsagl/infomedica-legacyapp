Imports System.Net.Http
Imports LegacyApp.Client
Imports LegacyApp.Client.Contracts

''' <summary>
''' Elenco clienti paginato + ordini del cliente selezionato.
''' Tutti i dati arrivano dalla REST API: la form non sa nulla di Access o SQL Server.
''' </summary>
Public Class MainForm

    Private Const PageSize As Integer = 50

    Private ReadOnly _api As LegacyApiClient
    Private _page As Integer = 1
    Private _totalPages As Integer = 1
    Private _isBusy As Boolean

    ' Annulla il caricamento ordini precedente se l'utente cambia riga velocemente
    Private _ordiniCts As CancellationTokenSource

    Public Sub New(api As LegacyApiClient)
        InitializeComponent()
        _api = api
    End Sub

    ' --- Ciclo di vita ----------------------------------------------------------

    Private Async Sub MainForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Await EseguiAsync("Caricamento clienti…",
                          Async Function()
                              Await AggiornaStatoApiAsync()
                              Await CaricaPaginaAsync(1)
                          End Function)
    End Sub

    Private Sub MainForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        _ordiniCts?.Cancel()
        _ordiniCts?.Dispose()
    End Sub

    Private Async Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.F5 Then
            e.Handled = True
            Await EseguiAsync("Aggiornamento…", Function() CaricaPaginaAsync(_page))
        End If
    End Sub

    ' --- Comandi della toolbar --------------------------------------------------

    Private Async Sub btnAggiorna_Click(sender As Object, e As EventArgs) Handles btnAggiorna.Click
        Await EseguiAsync("Aggiornamento…",
                          Async Function()
                              Await AggiornaStatoApiAsync()
                              Await CaricaPaginaAsync(_page)
                          End Function)
    End Sub

    Private Async Sub btnPrecedente_Click(sender As Object, e As EventArgs) Handles btnPrecedente.Click
        Await EseguiAsync("Caricamento…", Function() CaricaPaginaAsync(_page - 1))
    End Sub

    Private Async Sub btnSuccessiva_Click(sender As Object, e As EventArgs) Handles btnSuccessiva.Click
        Await EseguiAsync("Caricamento…", Function() CaricaPaginaAsync(_page + 1))
    End Sub

    Private Async Sub btnNuovo_Click(sender As Object, e As EventArgs) Handles btnNuovo.Click
        Using dialog = New ClienteEditForm(_api, Nothing)
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return
            Dim creato = dialog.Risultato
            Await EseguiAsync("Aggiornamento…", Function() CaricaPaginaAsync(_page, selezionaId:=creato?.Id))
            lblStato.Text = $"Cliente creato: {creato?.RagioneSociale}"
        End Using
    End Sub

    Private Async Sub btnModifica_Click(sender As Object, e As EventArgs) Handles btnModifica.Click
        Await ModificaSelezionatoAsync()
    End Sub

    Private Async Sub gridClienti_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles gridClienti.CellDoubleClick
        If e.RowIndex >= 0 Then Await ModificaSelezionatoAsync()
    End Sub

    Private Async Sub btnElimina_Click(sender As Object, e As EventArgs) Handles btnElimina.Click
        Dim cliente = ClienteSelezionato()
        If cliente Is Nothing Then Return

        Dim risposta = MessageBox.Show(Me,
                                       $"Eliminare definitivamente ""{cliente.RagioneSociale}""?",
                                       "Conferma eliminazione",
                                       MessageBoxButtons.YesNo,
                                       MessageBoxIcon.Warning,
                                       MessageBoxDefaultButton.Button2)
        If risposta <> DialogResult.Yes Then Return

        ' Un 409 (cliente con ordini) arriva come ApiException e viene mostrato da EseguiAsync
        Await EseguiAsync("Eliminazione…",
                          Async Function()
                              Await _api.DeleteClienteAsync(cliente.Id)
                              Await CaricaPaginaAsync(_page)
                              lblStato.Text = $"Cliente eliminato: {cliente.RagioneSociale}"
                          End Function)
    End Sub

    ' --- Selezione: carica gli ordini del cliente corrente ----------------------

    Private Async Sub bsClienti_CurrentChanged(sender As Object, e As EventArgs) Handles bsClienti.CurrentChanged
        AggiornaPulsanti()

        _ordiniCts?.Cancel()
        _ordiniCts?.Dispose()
        _ordiniCts = New CancellationTokenSource()
        Dim token = _ordiniCts.Token

        Dim cliente = ClienteSelezionato()
        If cliente Is Nothing Then
            MostraOrdini(Nothing, Array.Empty(Of OrdineDto)())
            Return
        End If

        grpOrdini.Text = $"Ordini di {cliente.RagioneSociale} (caricamento…)"
        Try
            Dim ordini = Await _api.GetOrdiniAsync(cliente.Id, token)
            If Not token.IsCancellationRequested Then MostraOrdini(cliente, ordini)
        Catch ex As OperationCanceledException
            ' l'utente ha selezionato un'altra riga: risultato non piu' utile
        Catch ex As Exception When TypeOf ex Is ApiException OrElse TypeOf ex Is HttpRequestException
            grpOrdini.Text = $"Ordini di {cliente.RagioneSociale} (errore: {ex.Message})"
        End Try
    End Sub

    ' --- Logica ------------------------------------------------------------------

    Private Async Function CaricaPaginaAsync(page As Integer, Optional selezionaId As Integer? = Nothing) As Task
        Dim risultato = Await _api.GetClientiAsync(Math.Max(1, page), PageSize)

        ' Se la pagina e' diventata vuota (es. dopo un'eliminazione) si torna all'ultima valida
        If risultato.Items.Count = 0 AndAlso risultato.Page > 1 Then
            risultato = Await _api.GetClientiAsync(risultato.TotalPages, PageSize)
        End If

        _page = risultato.Page
        _totalPages = risultato.TotalPages
        bsClienti.DataSource = risultato.Items

        If selezionaId.HasValue Then
            Dim indice = risultato.Items.FindIndex(Function(c) c.Id = selezionaId.Value)
            If indice >= 0 Then bsClienti.Position = indice
        End If

        lblPagina.Text = $"Pagina {_page} di {_totalPages}"
        lblStato.Text = $"{risultato.TotalCount} clienti"
        AggiornaPulsanti()
    End Function

    Private Async Function AggiornaStatoApiAsync() As Task
        Try
            Dim health = Await _api.GetHealthAsync()
            lblApi.Text = $"API: {health.Provider} · {health.Status}"
            lblApi.ForeColor = If(health.Status = "ok", Color.DarkGreen, Color.DarkOrange)
        Catch ex As Exception When TypeOf ex Is ApiException OrElse TypeOf ex Is HttpRequestException
            lblApi.Text = "API non raggiungibile"
            lblApi.ForeColor = Color.Firebrick
        End Try
    End Function

    Private Async Function ModificaSelezionatoAsync() As Task
        Dim cliente = ClienteSelezionato()
        If cliente Is Nothing OrElse _isBusy Then Return

        Using dialog = New ClienteEditForm(_api, cliente)
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return
        End Using

        Await EseguiAsync("Aggiornamento…", Function() CaricaPaginaAsync(_page, selezionaId:=cliente.Id))
        lblStato.Text = $"Modifiche salvate: cliente #{cliente.Id}"
    End Function

    ''' <summary>
    ''' Esegue un'operazione asincrona con cursore di attesa e gestione errori centralizzata.
    ''' Negli "Async Sub" (event handler) un'eccezione non gestita chiuderebbe l'applicazione:
    ''' per questo tutte le chiamate all'API passano da qui.
    ''' </summary>
    Private Async Function EseguiAsync(messaggio As String, operazione As Func(Of Task)) As Task
        If _isBusy Then Return
        _isBusy = True
        UseWaitCursor = True
        toolMain.Enabled = False
        lblStato.Text = messaggio

        Try
            Await operazione()
        Catch ex As ApiException
            lblStato.Text = "Operazione non riuscita"
            MessageBox.Show(Me, ex.Message, $"Errore dall'API ({ex.StatusCode})", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Catch ex As HttpRequestException
            lblStato.Text = "API non raggiungibile"
            MessageBox.Show(Me,
                            $"Impossibile contattare l'API su {_api.BaseAddress}." & Environment.NewLine &
                            "Verificare che LegacyApp.Api sia in esecuzione.",
                            "Connessione non riuscita", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As TaskCanceledException
            lblStato.Text = "Timeout"
            MessageBox.Show(Me, "L'API non ha risposto in tempo.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            _isBusy = False
            UseWaitCursor = False
            toolMain.Enabled = True
            AggiornaPulsanti()
        End Try
    End Function

    Private Function ClienteSelezionato() As ClienteDto
        Return TryCast(bsClienti.Current, ClienteDto)
    End Function

    Private Sub MostraOrdini(cliente As ClienteDto, ordini As IReadOnlyList(Of OrdineDto))
        bsOrdini.DataSource = ordini.ToList()
        grpOrdini.Text = If(cliente Is Nothing, "Ordini", $"Ordini di {cliente.RagioneSociale}")
        lblTotale.Text = If(ordini.Count = 0,
                            "Nessun ordine",
                            $"Totale: {ordini.Sum(Function(o) o.Importo):C2}")
    End Sub

    Private Sub AggiornaPulsanti()
        Dim haSelezione = ClienteSelezionato() IsNot Nothing
        btnModifica.Enabled = haSelezione
        btnElimina.Enabled = haSelezione
        btnPrecedente.Enabled = _page > 1
        btnSuccessiva.Enabled = _page < _totalPages
    End Sub

End Class
