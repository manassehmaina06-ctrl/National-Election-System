<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    Private components As System.ComponentModel.IContainer

    Friend WithEvents PanelLogin As System.Windows.Forms.Panel
    Friend WithEvents txtLoginName As System.Windows.Forms.TextBox
    Friend WithEvents txtLoginID As System.Windows.Forms.TextBox
    Friend WithEvents btnLogin As System.Windows.Forms.Button
    Friend WithEvents btnGoToRegister As System.Windows.Forms.Button
    Friend WithEvents btnViewPublicResults As System.Windows.Forms.Button
    Friend WithEvents btnGoToAdmin As System.Windows.Forms.Button

    Friend WithEvents PanelRegister As System.Windows.Forms.Panel
    Friend WithEvents txtRegName As System.Windows.Forms.TextBox
    Friend WithEvents txtRegID As System.Windows.Forms.TextBox
    Friend WithEvents btnRegister As System.Windows.Forms.Button
    Friend WithEvents btnBackToLogin As System.Windows.Forms.Button

    Friend WithEvents PanelVote As System.Windows.Forms.Panel
    Friend WithEvents flowBallot As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnCastVote As System.Windows.Forms.Button

    Friend WithEvents PanelResults As System.Windows.Forms.Panel
    Friend WithEvents lstResults As System.Windows.Forms.ListBox
    Friend WithEvents btnBackFromResults As System.Windows.Forms.Button

    Friend WithEvents PanelAdmin As System.Windows.Forms.Panel
    Friend WithEvents txtAdminPass As System.Windows.Forms.TextBox
    Friend WithEvents btnAdminLogin As System.Windows.Forms.Button
    Friend WithEvents lstAdminCandidates As System.Windows.Forms.ListBox
    Friend WithEvents txtCandName As System.Windows.Forms.TextBox
    Friend WithEvents txtCandParty As System.Windows.Forms.TextBox
    Friend WithEvents cmbCandPosition As System.Windows.Forms.ComboBox
    Friend WithEvents btnAddCandidate As System.Windows.Forms.Button
    Friend WithEvents btnDeleteCandidate As System.Windows.Forms.Button
    Friend WithEvents btnExportResults As System.Windows.Forms.Button
    Friend WithEvents btnViewAdminResults As System.Windows.Forms.Button
    Friend WithEvents btnPublishResults As System.Windows.Forms.Button
    Friend WithEvents btnBackFromAdmin As System.Windows.Forms.Button

    Friend WithEvents PanelSuccess As System.Windows.Forms.Panel
    Friend WithEvents lblSuccessTitle As System.Windows.Forms.Label
    Friend WithEvents lblToken As System.Windows.Forms.Label
    Friend WithEvents btnFinish As System.Windows.Forms.Button

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then components.Dispose()
        MyBase.Dispose(disposing)
    End Sub

    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Text = "National Election System"
        Me.ClientSize = New System.Drawing.Size(800, 650)
        Me.BackColor = System.Drawing.Color.FromArgb(13, 17, 23)

        Dim fontTitle = New System.Drawing.Font("Segoe UI", 20, System.Drawing.FontStyle.Bold)
        Dim fontLabel = New System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Regular)
        Dim fontInput = New System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Regular)
        Dim colorText = System.Drawing.Color.White
        Dim colorInputBg = System.Drawing.Color.FromArgb(22, 27, 34)
        Dim colorBtnBlue = System.Drawing.Color.FromArgb(88, 166, 255)
        Dim colorBtnGreen = System.Drawing.Color.FromArgb(35, 134, 54)

        ' --- LOGIN PANEL ---
        Me.PanelLogin = New System.Windows.Forms.Panel() With {.Dock = System.Windows.Forms.DockStyle.Fill, .Visible = True}
        Dim lblTitle As New System.Windows.Forms.Label() With {.Text = "Official Voter Login", .ForeColor = colorText, .Font = fontTitle, .Location = New System.Drawing.Point(250, 100), .Size = New System.Drawing.Size(300, 40)}
        Dim lblName As New System.Windows.Forms.Label() With {.Text = "Full Official Name:", .ForeColor = System.Drawing.Color.LightGray, .Font = fontLabel, .Location = New System.Drawing.Point(250, 170), .Size = New System.Drawing.Size(300, 20)}
        Me.txtLoginName = New System.Windows.Forms.TextBox() With {.Location = New System.Drawing.Point(250, 195), .Size = New System.Drawing.Size(300, 30), .Font = fontInput, .BackColor = colorInputBg, .ForeColor = colorText, .BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle}
        Dim lblID As New System.Windows.Forms.Label() With {.Text = "ID Number (Password):", .ForeColor = System.Drawing.Color.LightGray, .Font = fontLabel, .Location = New System.Drawing.Point(250, 240), .Size = New System.Drawing.Size(300, 20)}
        
        ' Login TextBox
        Me.txtLoginID = New System.Windows.Forms.TextBox() With {.Location = New System.Drawing.Point(250, 265), .Size = New System.Drawing.Size(300, 30), .Font = fontInput, .BackColor = colorInputBg, .ForeColor = colorText, .BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle, .UseSystemPasswordChar = True, .MaxLength = 10}
        AddHandler Me.txtLoginID.KeyPress, AddressOf OnlyAllowNumbers
        
        Me.btnLogin = New System.Windows.Forms.Button() With {.Text = "Login to Vote", .Location = New System.Drawing.Point(250, 320), .Size = New System.Drawing.Size(300, 40), .BackColor = colorBtnBlue, .ForeColor = colorText, .FlatStyle = System.Windows.Forms.FlatStyle.Flat, .Font = New System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold)}
        Me.btnGoToRegister = New System.Windows.Forms.Button() With {.Text = "New Voter? Register Here", .Location = New System.Drawing.Point(250, 380), .Size = New System.Drawing.Size(150, 30), .BackColor = System.Drawing.Color.Transparent, .ForeColor = colorBtnBlue, .FlatStyle = System.Windows.Forms.FlatStyle.Flat, .Font = New System.Drawing.Font("Segoe UI", 10)}
        
        ' NEW BUTTON: View Public Results
        Me.btnViewPublicResults = New System.Windows.Forms.Button() With {.Text = "View Public Results", .Location = New System.Drawing.Point(410, 380), .Size = New System.Drawing.Size(140, 30), .BackColor = System.Drawing.Color.Transparent, .ForeColor = System.Drawing.Color.Gold, .FlatStyle = System.Windows.Forms.FlatStyle.Flat, .Font = New System.Drawing.Font("Segoe UI", 10)}
        
        Me.btnGoToAdmin = New System.Windows.Forms.Button() With {.Text = "Admin Portal", .Location = New System.Drawing.Point(700, 600), .Size = New System.Drawing.Size(80, 30), .BackColor = System.Drawing.Color.Transparent, .ForeColor = System.Drawing.Color.DimGray, .FlatStyle = System.Windows.Forms.FlatStyle.Flat, .Font = New System.Drawing.Font("Segoe UI", 8)}

        Me.PanelLogin.Controls.AddRange(New System.Windows.Forms.Control() {lblTitle, lblName, Me.txtLoginName, lblID, Me.txtLoginID, Me.btnLogin, Me.btnGoToRegister, Me.btnViewPublicResults, Me.btnGoToAdmin})

        ' --- REGISTER PANEL ---
        Me.PanelRegister = New System.Windows.Forms.Panel() With {.Dock = System.Windows.Forms.DockStyle.Fill, .Visible = False}
        Dim lblRegTitle As New System.Windows.Forms.Label() With {.Text = "Voter Registration", .ForeColor = colorText, .Font = fontTitle, .Location = New System.Drawing.Point(250, 100), .Size = New System.Drawing.Size(300, 40)}
        Dim lblRegName As New System.Windows.Forms.Label() With {.Text = "Full Official Name:", .ForeColor = System.Drawing.Color.LightGray, .Location = New System.Drawing.Point(250, 170), .Size = New System.Drawing.Size(300, 20)}
        Me.txtRegName = New System.Windows.Forms.TextBox() With {.Location = New System.Drawing.Point(250, 195), .Size = New System.Drawing.Size(300, 30), .Font = fontInput, .BackColor = colorInputBg, .ForeColor = colorText, .BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle}
        Dim lblRegID As New System.Windows.Forms.Label() With {.Text = "ID Number (This will be your password):", .ForeColor = System.Drawing.Color.LightGray, .Location = New System.Drawing.Point(250, 240), .Size = New System.Drawing.Size(300, 20)}
        
        ' Register TextBox
        Me.txtRegID = New System.Windows.Forms.TextBox() With {.Location = New System.Drawing.Point(250, 265), .Size = New System.Drawing.Size(300, 30), .Font = fontInput, .BackColor = colorInputBg, .ForeColor = colorText, .BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle, .MaxLength = 10}
        AddHandler Me.txtRegID.KeyPress, AddressOf OnlyAllowNumbers
        
        Me.btnRegister = New System.Windows.Forms.Button() With {.Text = "Register", .Location = New System.Drawing.Point(250, 320), .Size = New System.Drawing.Size(300, 40), .BackColor = colorBtnGreen, .ForeColor = colorText, .FlatStyle = System.Windows.Forms.FlatStyle.Flat, .Font = New System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold)}
        Me.btnBackToLogin = New System.Windows.Forms.Button() With {.Text = "Back to Login", .Location = New System.Drawing.Point(250, 380), .Size = New System.Drawing.Size(150, 30), .BackColor = System.Drawing.Color.Transparent, .ForeColor = colorText, .FlatStyle = System.Windows.Forms.FlatStyle.Flat}

        Me.PanelRegister.Controls.AddRange(New System.Windows.Forms.Control() {lblRegTitle, lblRegName, Me.txtRegName, lblRegID, Me.txtRegID, Me.btnRegister, Me.btnBackToLogin})

        ' --- VOTING PANEL ---
        Me.PanelVote = New System.Windows.Forms.Panel() With {.Dock = System.Windows.Forms.DockStyle.Fill, .Visible = False}
        Me.flowBallot = New System.Windows.Forms.FlowLayoutPanel() With {.Location = New System.Drawing.Point(50, 20), .Size = New System.Drawing.Size(680, 480), .AutoScroll = True, .BackColor = System.Drawing.Color.FromArgb(13, 17, 23), .BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle}
        Me.btnCastVote = New System.Windows.Forms.Button() With {.Text = "Cast Final Vote", .Location = New System.Drawing.Point(500, 550), .Size = New System.Drawing.Size(250, 50), .BackColor = colorBtnBlue, .ForeColor = colorText, .FlatStyle = System.Windows.Forms.FlatStyle.Flat, .Font = New System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold)}
        Me.PanelVote.Controls.AddRange(New System.Windows.Forms.Control() {Me.flowBallot, Me.btnCastVote})

        ' --- RESULTS PANEL (Public) ---
        Me.PanelResults = New System.Windows.Forms.Panel() With {.Dock = System.Windows.Forms.DockStyle.Fill, .Visible = False}
        Dim lblResTitle As New System.Windows.Forms.Label() With {.Text = "Official Election Results", .ForeColor = colorText, .Font = fontTitle, .Location = New System.Drawing.Point(50, 20), .Size = New System.Drawing.Size(400, 40)}
        Me.lstResults = New System.Windows.Forms.ListBox() With {.Location = New System.Drawing.Point(50, 70), .Size = New System.Drawing.Size(700, 450), .BackColor = colorInputBg, .ForeColor = colorText, .Font = New System.Drawing.Font("Consolas", 12), .BorderStyle = System.Windows.Forms.BorderStyle.None}
        Me.btnBackFromResults = New System.Windows.Forms.Button() With {.Text = "Back to Login", .Location = New System.Drawing.Point(50, 540), .Size = New System.Drawing.Size(150, 40), .BackColor = colorBtnBlue, .ForeColor = colorText, .FlatStyle = System.Windows.Forms.FlatStyle.Flat}
        Me.PanelResults.Controls.AddRange(New System.Windows.Forms.Control() {lblResTitle, Me.lstResults, Me.btnBackFromResults})

        ' --- ADMIN PANEL ---
        Me.PanelAdmin = New System.Windows.Forms.Panel() With {.Dock = System.Windows.Forms.DockStyle.Fill, .Visible = False}
        Dim lblAdminTitle As New System.Windows.Forms.Label() With {.Text = "IEBC Admin Portal", .ForeColor = System.Drawing.Color.Gold, .Font = fontTitle, .Location = New System.Drawing.Point(50, 20), .Size = New System.Drawing.Size(300, 40)}
        
        Me.txtAdminPass = New System.Windows.Forms.TextBox() With {.Location = New System.Drawing.Point(50, 70), .Size = New System.Drawing.Size(200, 30), .Font = fontInput, .BackColor = colorInputBg, .ForeColor = colorText, .UseSystemPasswordChar = True}
        Me.btnAdminLogin = New System.Windows.Forms.Button() With {.Text = "Login", .Location = New System.Drawing.Point(260, 70), .Size = New System.Drawing.Size(80, 30), .BackColor = colorBtnBlue, .ForeColor = colorText, .FlatStyle = System.Windows.Forms.FlatStyle.Flat}
        
        ' Admin Add Candidate Controls
        Me.txtCandName = New System.Windows.Forms.TextBox() With {.Location = New System.Drawing.Point(50, 120), .Size = New System.Drawing.Size(200, 30), .Font = fontInput, .BackColor = colorInputBg, .ForeColor = colorText}
        Me.txtCandParty = New System.Windows.Forms.TextBox() With {.Location = New System.Drawing.Point(260, 120), .Size = New System.Drawing.Size(150, 30), .Font = fontInput, .BackColor = colorInputBg, .ForeColor = colorText}
        Me.cmbCandPosition = New System.Windows.Forms.ComboBox() With {.Location = New System.Drawing.Point(420, 120), .Size = New System.Drawing.Size(150, 30), .Font = fontInput, .BackColor = colorInputBg, .ForeColor = colorText, .DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList}
        Me.cmbCandPosition.Items.AddRange(New Object() {"President", "Member of Parliament", "Finance Secretary", "Welfare Representative"})
        Me.btnAddCandidate = New System.Windows.Forms.Button() With {.Text = "Add Candidate", .Location = New System.Drawing.Point(580, 120), .Size = New System.Drawing.Size(120, 30), .BackColor = colorBtnGreen, .ForeColor = colorText, .FlatStyle = System.Windows.Forms.FlatStyle.Flat}
        
        Me.lstAdminCandidates = New System.Windows.Forms.ListBox() With {.Location = New System.Drawing.Point(50, 170), .Size = New System.Drawing.Size(650, 200), .BackColor = colorInputBg, .ForeColor = colorText, .Font = New System.Drawing.Font("Consolas", 10)}
        Me.btnDeleteCandidate = New System.Windows.Forms.Button() With {.Text = "Delete Selected", .Location = New System.Drawing.Point(50, 380), .Size = New System.Drawing.Size(150, 30), .BackColor = System.Drawing.Color.DarkRed, .ForeColor = colorText, .FlatStyle = System.Windows.Forms.FlatStyle.Flat}
        
        Me.btnViewAdminResults = New System.Windows.Forms.Button() With {.Text = "View Live Results", .Location = New System.Drawing.Point(220, 380), .Size = New System.Drawing.Size(150, 30), .BackColor = colorBtnBlue, .ForeColor = colorText, .FlatStyle = System.Windows.Forms.FlatStyle.Flat}
        Me.btnExportResults = New System.Windows.Forms.Button() With {.Text = "Export to TXT", .Location = New System.Drawing.Point(390, 380), .Size = New System.Drawing.Size(120, 30), .BackColor = System.Drawing.Color.DarkOrange, .ForeColor = colorText, .FlatStyle = System.Windows.Forms.FlatStyle.Flat}
        Me.btnPublishResults = New System.Windows.Forms.Button() With {.Text = "PUBLISH RESULTS", .Location = New System.Drawing.Point(520, 380), .Size = New System.Drawing.Size(180, 30), .BackColor = System.Drawing.Color.Gold, .ForeColor = System.Drawing.Color.Black, .FlatStyle = System.Windows.Forms.FlatStyle.Flat, .Font = New System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)}
        Me.btnBackFromAdmin = New System.Windows.Forms.Button() With {.Text = "Logout Admin", .Location = New System.Drawing.Point(50, 440), .Size = New System.Drawing.Size(150, 30), .BackColor = System.Drawing.Color.Transparent, .ForeColor = colorText, .FlatStyle = System.Windows.Forms.FlatStyle.Flat}

        Me.PanelAdmin.Controls.AddRange(New System.Windows.Forms.Control() {lblAdminTitle, Me.txtAdminPass, Me.btnAdminLogin, Me.txtCandName, Me.txtCandParty, Me.cmbCandPosition, Me.btnAddCandidate, Me.lstAdminCandidates, Me.btnDeleteCandidate, Me.btnViewAdminResults, Me.btnExportResults, Me.btnPublishResults, Me.btnBackFromAdmin})

        ' --- SUCCESS PANEL ---
        Me.PanelSuccess = New System.Windows.Forms.Panel() With {.Dock = System.Windows.Forms.DockStyle.Fill, .Visible = False, .BackColor = System.Drawing.Color.FromArgb(13, 17, 23)}
        
        Me.lblSuccessTitle = New System.Windows.Forms.Label() With {.Text = "Vote Cast Successfully!", .ForeColor = System.Drawing.Color.FromArgb(35, 134, 54), .Font = New System.Drawing.Font("Segoe UI", 24, System.Drawing.FontStyle.Bold), .Location = New System.Drawing.Point(200, 100), .Size = New System.Drawing.Size(400, 50), .TextAlign = ContentAlignment.MiddleCenter}
        
        Dim lblTokenInfo As New System.Windows.Forms.Label() With {.Text = "Keep this code to confirm your vote was counted:", .ForeColor = System.Drawing.Color.LightGray, .Font = New System.Drawing.Font("Segoe UI", 12), .Location = New System.Drawing.Point(200, 180), .Size = New System.Drawing.Size(400, 30), .TextAlign = ContentAlignment.MiddleCenter}
        
        Me.lblToken = New System.Windows.Forms.Label() With {.Text = "XXXX-XXXX", .ForeColor = System.Drawing.Color.Gold, .Font = New System.Drawing.Font("Consolas", 26, System.Drawing.FontStyle.Bold), .Location = New System.Drawing.Point(200, 220), .Size = New System.Drawing.Size(400, 50), .TextAlign = ContentAlignment.MiddleCenter}
        
        Me.btnFinish = New System.Windows.Forms.Button() With {.Text = "Finish", .Location = New System.Drawing.Point(300, 320), .Size = New System.Drawing.Size(200, 40), .BackColor = System.Drawing.Color.FromArgb(88, 166, 255), .ForeColor = System.Drawing.Color.White, .FlatStyle = System.Windows.Forms.FlatStyle.Flat, .Font = New System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold)}

        Me.PanelSuccess.Controls.AddRange(New System.Windows.Forms.Control() {Me.lblSuccessTitle, lblTokenInfo, Me.lblToken, Me.btnFinish})

        ' Add all panels to the form
        Me.Controls.Add(Me.PanelLogin)
        Me.Controls.Add(Me.PanelRegister)
        Me.Controls.Add(Me.PanelVote)
        Me.Controls.Add(Me.PanelResults)
        Me.Controls.Add(Me.PanelAdmin)
        Me.Controls.Add(Me.PanelSuccess)
    End Sub
End Class