<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ClienteEditForm
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
        layoutCampi = New TableLayoutPanel()
        lblRagioneSociale = New Label()
        txtRagioneSociale = New TextBox()
        lblPartitaIva = New Label()
        txtPartitaIva = New TextBox()
        lblEmail = New Label()
        txtEmail = New TextBox()
        chkAttivo = New CheckBox()
        layoutPulsanti = New FlowLayoutPanel()
        btnAnnulla = New Button()
        btnSalva = New Button()
        errValidazione = New ErrorProvider(components)
        layoutCampi.SuspendLayout()
        layoutPulsanti.SuspendLayout()
        CType(errValidazione, System.ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        '
        'layoutCampi
        '
        layoutCampi.ColumnCount = 2
        layoutCampi.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        layoutCampi.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        layoutCampi.Controls.Add(lblRagioneSociale, 0, 0)
        layoutCampi.Controls.Add(txtRagioneSociale, 1, 0)
        layoutCampi.Controls.Add(lblPartitaIva, 0, 1)
        layoutCampi.Controls.Add(txtPartitaIva, 1, 1)
        layoutCampi.Controls.Add(lblEmail, 0, 2)
        layoutCampi.Controls.Add(txtEmail, 1, 2)
        layoutCampi.Controls.Add(chkAttivo, 1, 3)
        layoutCampi.Dock = DockStyle.Fill
        layoutCampi.Location = New Point(12, 12)
        layoutCampi.Name = "layoutCampi"
        layoutCampi.Padding = New Padding(0, 0, 20, 0)
        layoutCampi.RowCount = 5
        layoutCampi.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutCampi.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutCampi.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutCampi.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        layoutCampi.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        layoutCampi.Size = New Size(436, 150)
        layoutCampi.TabIndex = 0
        '
        'lblRagioneSociale
        '
        lblRagioneSociale.Anchor = AnchorStyles.Left
        lblRagioneSociale.AutoSize = True
        lblRagioneSociale.Name = "lblRagioneSociale"
        lblRagioneSociale.TabIndex = 0
        lblRagioneSociale.Text = "&Ragione sociale *"
        '
        'txtRagioneSociale
        '
        txtRagioneSociale.Dock = DockStyle.Fill
        txtRagioneSociale.MaxLength = 255
        txtRagioneSociale.Name = "txtRagioneSociale"
        txtRagioneSociale.TabIndex = 1
        '
        'lblPartitaIva
        '
        lblPartitaIva.Anchor = AnchorStyles.Left
        lblPartitaIva.AutoSize = True
        lblPartitaIva.Name = "lblPartitaIva"
        lblPartitaIva.TabIndex = 2
        lblPartitaIva.Text = "&Partita IVA"
        '
        'txtPartitaIva
        '
        txtPartitaIva.Dock = DockStyle.Fill
        txtPartitaIva.MaxLength = 11
        txtPartitaIva.Name = "txtPartitaIva"
        txtPartitaIva.TabIndex = 3
        '
        'lblEmail
        '
        lblEmail.Anchor = AnchorStyles.Left
        lblEmail.AutoSize = True
        lblEmail.Name = "lblEmail"
        lblEmail.TabIndex = 4
        lblEmail.Text = "&Email"
        '
        'txtEmail
        '
        txtEmail.Dock = DockStyle.Fill
        txtEmail.MaxLength = 255
        txtEmail.Name = "txtEmail"
        txtEmail.TabIndex = 5
        '
        'chkAttivo
        '
        chkAttivo.AutoSize = True
        chkAttivo.Checked = True
        chkAttivo.CheckState = CheckState.Checked
        chkAttivo.Name = "chkAttivo"
        chkAttivo.TabIndex = 6
        chkAttivo.Text = "Cliente &attivo"
        chkAttivo.UseVisualStyleBackColor = True
        '
        'layoutPulsanti
        '
        layoutPulsanti.AutoSize = True
        layoutPulsanti.Controls.Add(btnAnnulla)
        layoutPulsanti.Controls.Add(btnSalva)
        layoutPulsanti.Dock = DockStyle.Bottom
        layoutPulsanti.FlowDirection = FlowDirection.RightToLeft
        layoutPulsanti.Location = New Point(12, 162)
        layoutPulsanti.Name = "layoutPulsanti"
        layoutPulsanti.Size = New Size(436, 31)
        layoutPulsanti.TabIndex = 1
        '
        'btnAnnulla
        '
        btnAnnulla.DialogResult = DialogResult.Cancel
        btnAnnulla.Name = "btnAnnulla"
        btnAnnulla.Size = New Size(90, 25)
        btnAnnulla.TabIndex = 1
        btnAnnulla.Text = "Annulla"
        btnAnnulla.UseVisualStyleBackColor = True
        '
        'btnSalva
        '
        btnSalva.Name = "btnSalva"
        btnSalva.Size = New Size(90, 25)
        btnSalva.TabIndex = 0
        btnSalva.Text = "&Salva"
        btnSalva.UseVisualStyleBackColor = True
        '
        'errValidazione
        '
        errValidazione.BlinkStyle = ErrorBlinkStyle.NeverBlink
        errValidazione.ContainerControl = Me
        '
        'ClienteEditForm
        '
        AcceptButton = btnSalva
        AutoScaleDimensions = New SizeF(7.0F, 15.0F)
        AutoScaleMode = AutoScaleMode.Font
        CancelButton = btnAnnulla
        ClientSize = New Size(460, 205)
        Controls.Add(layoutCampi)
        Controls.Add(layoutPulsanti)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "ClienteEditForm"
        Padding = New Padding(12)
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Cliente"
        layoutCampi.ResumeLayout(False)
        layoutCampi.PerformLayout()
        layoutPulsanti.ResumeLayout(False)
        CType(errValidazione, System.ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents layoutCampi As TableLayoutPanel
    Friend WithEvents lblRagioneSociale As Label
    Friend WithEvents txtRagioneSociale As TextBox
    Friend WithEvents lblPartitaIva As Label
    Friend WithEvents txtPartitaIva As TextBox
    Friend WithEvents lblEmail As Label
    Friend WithEvents txtEmail As TextBox
    Friend WithEvents chkAttivo As CheckBox
    Friend WithEvents layoutPulsanti As FlowLayoutPanel
    Friend WithEvents btnAnnulla As Button
    Friend WithEvents btnSalva As Button
    Friend WithEvents errValidazione As ErrorProvider

End Class
