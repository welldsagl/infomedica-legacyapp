<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue e' richiesta da Progettazione Windows Form
    'Puo' essere modificata in Progettazione Windows Form.
    'Non modificarla con l'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New System.ComponentModel.Container()
        toolMain = New ToolStrip()
        btnAggiorna = New ToolStripButton()
        btnNuovo = New ToolStripButton()
        btnModifica = New ToolStripButton()
        btnElimina = New ToolStripButton()
        sepPaging = New ToolStripSeparator()
        btnPrecedente = New ToolStripButton()
        lblPagina = New ToolStripLabel()
        btnSuccessiva = New ToolStripButton()
        statusMain = New StatusStrip()
        lblStato = New ToolStripStatusLabel()
        lblApi = New ToolStripStatusLabel()
        splitMain = New SplitContainer()
        gridClienti = New DataGridView()
        colId = New DataGridViewTextBoxColumn()
        colRagioneSociale = New DataGridViewTextBoxColumn()
        colPartitaIva = New DataGridViewTextBoxColumn()
        colEmail = New DataGridViewTextBoxColumn()
        colDataInserimento = New DataGridViewTextBoxColumn()
        colAttivo = New DataGridViewCheckBoxColumn()
        grpOrdini = New GroupBox()
        gridOrdini = New DataGridView()
        colOrdId = New DataGridViewTextBoxColumn()
        colOrdData = New DataGridViewTextBoxColumn()
        colOrdImporto = New DataGridViewTextBoxColumn()
        colOrdNote = New DataGridViewTextBoxColumn()
        lblTotale = New Label()
        bsClienti = New BindingSource(components)
        bsOrdini = New BindingSource(components)
        toolMain.SuspendLayout()
        statusMain.SuspendLayout()
        CType(splitMain, System.ComponentModel.ISupportInitialize).BeginInit()
        splitMain.Panel1.SuspendLayout()
        splitMain.Panel2.SuspendLayout()
        splitMain.SuspendLayout()
        CType(gridClienti, System.ComponentModel.ISupportInitialize).BeginInit()
        grpOrdini.SuspendLayout()
        CType(gridOrdini, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(bsClienti, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(bsOrdini, System.ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        '
        'toolMain
        '
        toolMain.GripStyle = ToolStripGripStyle.Hidden
        toolMain.ImageScalingSize = New Size(20, 20)
        toolMain.Items.AddRange(New ToolStripItem() {btnAggiorna, btnNuovo, btnModifica, btnElimina, sepPaging, btnPrecedente, lblPagina, btnSuccessiva})
        toolMain.Location = New Point(0, 0)
        toolMain.Name = "toolMain"
        toolMain.Padding = New Padding(6, 2, 6, 2)
        toolMain.Size = New Size(1000, 27)
        toolMain.TabIndex = 0
        '
        'btnAggiorna
        '
        btnAggiorna.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnAggiorna.Name = "btnAggiorna"
        btnAggiorna.Size = New Size(62, 20)
        btnAggiorna.Text = "Aggiorna"
        btnAggiorna.ToolTipText = "Ricarica la pagina corrente (F5)"
        '
        'btnNuovo
        '
        btnNuovo.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnNuovo.Name = "btnNuovo"
        btnNuovo.Size = New Size(48, 20)
        btnNuovo.Text = "Nuovo"
        '
        'btnModifica
        '
        btnModifica.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnModifica.Name = "btnModifica"
        btnModifica.Size = New Size(60, 20)
        btnModifica.Text = "Modifica"
        '
        'btnElimina
        '
        btnElimina.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnElimina.Name = "btnElimina"
        btnElimina.Size = New Size(50, 20)
        btnElimina.Text = "Elimina"
        '
        'sepPaging
        '
        sepPaging.Name = "sepPaging"
        sepPaging.Size = New Size(6, 23)
        '
        'btnPrecedente
        '
        btnPrecedente.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnPrecedente.Name = "btnPrecedente"
        btnPrecedente.Size = New Size(23, 20)
        btnPrecedente.Text = "◀"
        btnPrecedente.ToolTipText = "Pagina precedente"
        '
        'lblPagina
        '
        lblPagina.Name = "lblPagina"
        lblPagina.Size = New Size(80, 20)
        lblPagina.Text = "Pagina 1 di 1"
        '
        'btnSuccessiva
        '
        btnSuccessiva.DisplayStyle = ToolStripItemDisplayStyle.Text
        btnSuccessiva.Name = "btnSuccessiva"
        btnSuccessiva.Size = New Size(23, 20)
        btnSuccessiva.Text = "▶"
        btnSuccessiva.ToolTipText = "Pagina successiva"
        '
        'statusMain
        '
        statusMain.ImageScalingSize = New Size(20, 20)
        statusMain.Items.AddRange(New ToolStripItem() {lblStato, lblApi})
        statusMain.Location = New Point(0, 638)
        statusMain.Name = "statusMain"
        statusMain.Size = New Size(1000, 22)
        statusMain.TabIndex = 2
        '
        'lblStato
        '
        lblStato.Name = "lblStato"
        lblStato.Size = New Size(855, 17)
        lblStato.Spring = True
        lblStato.Text = "Pronto"
        lblStato.TextAlign = ContentAlignment.MiddleLeft
        '
        'lblApi
        '
        lblApi.Name = "lblApi"
        lblApi.Size = New Size(130, 17)
        lblApi.Text = "API: verifica in corso…"
        '
        'splitMain
        '
        splitMain.Dock = DockStyle.Fill
        splitMain.Location = New Point(0, 27)
        splitMain.Name = "splitMain"
        splitMain.Orientation = Orientation.Horizontal
        '
        'splitMain.Panel1
        '
        splitMain.Panel1.Controls.Add(gridClienti)
        splitMain.Panel1.Padding = New Padding(6)
        '
        'splitMain.Panel2
        '
        splitMain.Panel2.Controls.Add(grpOrdini)
        splitMain.Panel2.Padding = New Padding(6)
        splitMain.Size = New Size(1000, 611)
        splitMain.SplitterDistance = 380
        splitMain.TabIndex = 1
        '
        'gridClienti
        '
        gridClienti.AllowUserToAddRows = False
        gridClienti.AllowUserToDeleteRows = False
        gridClienti.AutoGenerateColumns = False
        gridClienti.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        gridClienti.BackgroundColor = SystemColors.Window
        gridClienti.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        gridClienti.Columns.AddRange(New DataGridViewColumn() {colId, colRagioneSociale, colPartitaIva, colEmail, colDataInserimento, colAttivo})
        gridClienti.DataSource = bsClienti
        gridClienti.Dock = DockStyle.Fill
        gridClienti.Location = New Point(6, 6)
        gridClienti.MultiSelect = False
        gridClienti.Name = "gridClienti"
        gridClienti.ReadOnly = True
        gridClienti.RowHeadersVisible = False
        gridClienti.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        gridClienti.Size = New Size(988, 368)
        gridClienti.TabIndex = 0
        '
        'colId
        '
        colId.DataPropertyName = "Id"
        colId.FillWeight = 30.0F
        colId.HeaderText = "#"
        colId.Name = "colId"
        colId.ReadOnly = True
        '
        'colRagioneSociale
        '
        colRagioneSociale.DataPropertyName = "RagioneSociale"
        colRagioneSociale.FillWeight = 180.0F
        colRagioneSociale.HeaderText = "Ragione sociale"
        colRagioneSociale.Name = "colRagioneSociale"
        colRagioneSociale.ReadOnly = True
        '
        'colPartitaIva
        '
        colPartitaIva.DataPropertyName = "PartitaIva"
        colPartitaIva.HeaderText = "Partita IVA"
        colPartitaIva.Name = "colPartitaIva"
        colPartitaIva.ReadOnly = True
        '
        'colEmail
        '
        colEmail.DataPropertyName = "Email"
        colEmail.FillWeight = 140.0F
        colEmail.HeaderText = "Email"
        colEmail.Name = "colEmail"
        colEmail.ReadOnly = True
        '
        'colDataInserimento
        '
        colDataInserimento.DataPropertyName = "DataInserimento"
        colDataInserimento.DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "d"}
        colDataInserimento.FillWeight = 70.0F
        colDataInserimento.HeaderText = "Inserito il"
        colDataInserimento.Name = "colDataInserimento"
        colDataInserimento.ReadOnly = True
        '
        'colAttivo
        '
        colAttivo.DataPropertyName = "Attivo"
        colAttivo.FillWeight = 40.0F
        colAttivo.HeaderText = "Attivo"
        colAttivo.Name = "colAttivo"
        colAttivo.ReadOnly = True
        '
        'grpOrdini
        '
        grpOrdini.Controls.Add(gridOrdini)
        grpOrdini.Controls.Add(lblTotale)
        grpOrdini.Dock = DockStyle.Fill
        grpOrdini.Location = New Point(6, 6)
        grpOrdini.Name = "grpOrdini"
        grpOrdini.Padding = New Padding(8)
        grpOrdini.Size = New Size(988, 215)
        grpOrdini.TabIndex = 0
        grpOrdini.TabStop = False
        grpOrdini.Text = "Ordini"
        '
        'gridOrdini
        '
        gridOrdini.AllowUserToAddRows = False
        gridOrdini.AllowUserToDeleteRows = False
        gridOrdini.AutoGenerateColumns = False
        gridOrdini.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        gridOrdini.BackgroundColor = SystemColors.Window
        gridOrdini.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        gridOrdini.Columns.AddRange(New DataGridViewColumn() {colOrdId, colOrdData, colOrdImporto, colOrdNote})
        gridOrdini.DataSource = bsOrdini
        gridOrdini.Dock = DockStyle.Fill
        gridOrdini.Location = New Point(8, 24)
        gridOrdini.Name = "gridOrdini"
        gridOrdini.ReadOnly = True
        gridOrdini.RowHeadersVisible = False
        gridOrdini.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        gridOrdini.Size = New Size(972, 160)
        gridOrdini.TabIndex = 0
        '
        'colOrdId
        '
        colOrdId.DataPropertyName = "Id"
        colOrdId.FillWeight = 30.0F
        colOrdId.HeaderText = "#"
        colOrdId.Name = "colOrdId"
        colOrdId.ReadOnly = True
        '
        'colOrdData
        '
        colOrdData.DataPropertyName = "DataOrdine"
        colOrdData.DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "d"}
        colOrdData.FillWeight = 60.0F
        colOrdData.HeaderText = "Data"
        colOrdData.Name = "colOrdData"
        colOrdData.ReadOnly = True
        '
        'colOrdImporto
        '
        colOrdImporto.DataPropertyName = "Importo"
        colOrdImporto.DefaultCellStyle = New DataGridViewCellStyle() With {.Alignment = DataGridViewContentAlignment.MiddleRight, .Format = "N2"}
        colOrdImporto.FillWeight = 60.0F
        colOrdImporto.HeaderText = "Importo (€)"
        colOrdImporto.Name = "colOrdImporto"
        colOrdImporto.ReadOnly = True
        '
        'colOrdNote
        '
        colOrdNote.DataPropertyName = "Note"
        colOrdNote.FillWeight = 200.0F
        colOrdNote.HeaderText = "Note"
        colOrdNote.Name = "colOrdNote"
        colOrdNote.ReadOnly = True
        '
        'lblTotale
        '
        lblTotale.Dock = DockStyle.Bottom
        lblTotale.Font = New Font(Font, FontStyle.Bold)
        lblTotale.Location = New Point(8, 184)
        lblTotale.Name = "lblTotale"
        lblTotale.Size = New Size(972, 23)
        lblTotale.TabIndex = 1
        lblTotale.TextAlign = ContentAlignment.MiddleRight
        '
        'MainForm
        '
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1000, 660)
        Controls.Add(splitMain)
        Controls.Add(statusMain)
        Controls.Add(toolMain)
        KeyPreview = True
        MinimumSize = New Size(700, 450)
        Name = "MainForm"
        StartPosition = FormStartPosition.CenterScreen
        Text = "LegacyApp – Clienti"
        toolMain.ResumeLayout(False)
        toolMain.PerformLayout()
        statusMain.ResumeLayout(False)
        statusMain.PerformLayout()
        splitMain.Panel1.ResumeLayout(False)
        splitMain.Panel2.ResumeLayout(False)
        CType(splitMain, System.ComponentModel.ISupportInitialize).EndInit()
        splitMain.ResumeLayout(False)
        CType(gridClienti, System.ComponentModel.ISupportInitialize).EndInit()
        grpOrdini.ResumeLayout(False)
        CType(gridOrdini, System.ComponentModel.ISupportInitialize).EndInit()
        CType(bsClienti, System.ComponentModel.ISupportInitialize).EndInit()
        CType(bsOrdini, System.ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents toolMain As ToolStrip
    Friend WithEvents btnAggiorna As ToolStripButton
    Friend WithEvents btnNuovo As ToolStripButton
    Friend WithEvents btnModifica As ToolStripButton
    Friend WithEvents btnElimina As ToolStripButton
    Friend WithEvents sepPaging As ToolStripSeparator
    Friend WithEvents btnPrecedente As ToolStripButton
    Friend WithEvents lblPagina As ToolStripLabel
    Friend WithEvents btnSuccessiva As ToolStripButton
    Friend WithEvents statusMain As StatusStrip
    Friend WithEvents lblStato As ToolStripStatusLabel
    Friend WithEvents lblApi As ToolStripStatusLabel
    Friend WithEvents splitMain As SplitContainer
    Friend WithEvents gridClienti As DataGridView
    Friend WithEvents colId As DataGridViewTextBoxColumn
    Friend WithEvents colRagioneSociale As DataGridViewTextBoxColumn
    Friend WithEvents colPartitaIva As DataGridViewTextBoxColumn
    Friend WithEvents colEmail As DataGridViewTextBoxColumn
    Friend WithEvents colDataInserimento As DataGridViewTextBoxColumn
    Friend WithEvents colAttivo As DataGridViewCheckBoxColumn
    Friend WithEvents grpOrdini As GroupBox
    Friend WithEvents gridOrdini As DataGridView
    Friend WithEvents colOrdId As DataGridViewTextBoxColumn
    Friend WithEvents colOrdData As DataGridViewTextBoxColumn
    Friend WithEvents colOrdImporto As DataGridViewTextBoxColumn
    Friend WithEvents colOrdNote As DataGridViewTextBoxColumn
    Friend WithEvents lblTotale As Label
    Friend WithEvents bsClienti As BindingSource
    Friend WithEvents bsOrdini As BindingSource

End Class
