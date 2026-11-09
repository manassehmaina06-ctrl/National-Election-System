Imports MySqlConnector
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO
Imports Microsoft.VisualBasic
Imports System.Drawing.Drawing2D

Public Class Form1
    Private connectionString As String = "server=localhost;user=root;password=Maina@411;database=election_db;"
    
    Private currentUserID As Integer = -1
    Private currentUserName As String = ""
    Private currentUserIDNumber As String = ""
    Private selectedChoices As New Dictionary(Of String, Integer)()
    Private isAdminLoggedIn As Boolean = False


        ' Variables to hold the chart data
    Private chartBarData As New List(Of Tuple(Of String, Integer))()
    Private chartPieData As New List(Of Tuple(Of String, Integer))()

    Private Function GetConnection() As MySqlConnection
        Return New MySqlConnection(connectionString)
    End Function

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim name As String = txtLoginName.Text.Trim()
        Dim idNum As String = txtLoginID.Text.Trim()

        If String.IsNullOrEmpty(name) Or String.IsNullOrEmpty(idNum) Then
            ShowDarkMessage("Please enter both your name and ID number.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT UserID, HasVoted FROM Users WHERE FullName = @name AND IDNumber = @id"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@name", name)
                    cmd.Parameters.AddWithValue("@id", idNum)
                    
                    Using reader = cmd.ExecuteReader()
                        If reader.Read() Then
                            ' Check if they already voted
                            If Convert.ToBoolean(reader("HasVoted")) Then
                                ShowDarkMessage("You have already cast your vote!", "Voting Blocked", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Return
                            End If
                            
                            ' Save user details
                            currentUserID = Convert.ToInt32(reader("UserID"))
                            currentUserName = name
                            currentUserIDNumber = idNum
                        Else
                            ShowDarkMessage("Account not found. Please register first.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Return
                        End If
                    End Using 
                End Using 
            End Using 

            ' --- IF WE GET HERE, LOGIN WAS SUCCESSFUL ---
            Try
                LoadBallot()
                PanelLogin.Visible = False
                PanelVote.Visible = True
            Catch ex As Exception
                ShowDarkMessage("Failed to load ballot: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

        Catch ex As Exception
            ShowDarkMessage("Database Login Error: " & ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnGoToRegister_Click(sender As Object, e As EventArgs) Handles btnGoToRegister.Click
        PanelLogin.Visible = False
        PanelRegister.Visible = True
    End Sub
        Private Sub btnViewPublicResults_Click(sender As Object, e As EventArgs) Handles btnViewPublicResults.Click
        ' Make sure we are viewing as a public user, not admin
        isAdminLoggedIn = False 
        
        ' Hide login, show results
        PanelLogin.Visible = False
        PanelResults.Visible = True
        
        ' Call the LoadResults function
        LoadResults()
    End Sub

    Private Sub btnBackToLogin_Click(sender As Object, e As EventArgs) Handles btnBackToLogin.Click
        PanelRegister.Visible = False
        PanelLogin.Visible = True
    End Sub

    Private Sub btnRegister_Click(sender As Object, e As EventArgs) Handles btnRegister.Click
        Dim name As String = txtRegName.Text.Trim()
        Dim idNum As String = txtRegID.Text.Trim()

        If String.IsNullOrEmpty(name) Or String.IsNullOrEmpty(idNum) Then
            ShowDarkMessage("Please fill in all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "INSERT INTO Users (FullName, IDNumber) VALUES (@name, @id)"
                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@name", name)
                    cmd.Parameters.AddWithValue("@id", idNum)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            ShowDarkMessage("Registration Successful! You can now login.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            txtRegName.Clear()
            txtRegID.Clear()
            PanelRegister.Visible = False
            PanelLogin.Visible = True
        Catch ex As MySqlException
            If ex.Number = 1062 Then ' Duplicate entry error
                ShowDarkMessage("This ID Number is already registered!", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                ShowDarkMessage("Database Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Try
    End Sub

    ' ================= VOTING LOGIC =================
    Private Sub LoadBallot()
        flowBallot.Controls.Clear()
        selectedChoices.Clear()

        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT CandidateID, Name, Party, Position FROM Candidates ORDER BY FIELD(Position, 'President', 'Member of Parliament', 'Finance Secretary', 'Welfare Representative')"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader = cmd.ExecuteReader()
                        Dim currentPos As String = ""
                        Dim currentGroupBox As GroupBox = Nothing
                        
                        While reader.Read()
                            Dim pos As String = reader("Position").ToString()
                            
                            If pos <> currentPos Then
                                currentPos = pos
                                currentGroupBox = New GroupBox() With {
                                    .Text = currentPos.ToUpper(),
                                    .ForeColor = Color.White,
                                    .Font = New Font("Segoe UI", 14, FontStyle.Bold),
                                    .Size = New Size(640, 120),
                                    .Margin = New Padding(0, 10, 0, 10)
                                }
                                flowBallot.Controls.Add(currentGroupBox)
                            End If

                            Dim rb As New RadioButton() With {
                                .Text = reader("Name").ToString() & " - " & reader("Party").ToString(),
                                .Tag = New Tuple(Of Integer, String)(reader("CandidateID"), pos),
                                .ForeColor = Color.LightGray,
                                .Font = New Font("Segoe UI", 11),
                                .Size = New Size(600, 30),
                                .BackColor = Color.FromArgb(22, 27, 34),
                                .Location = New Point(10, 30 + (currentGroupBox.Controls.Count * 30))
                            }
                            
                            AddHandler rb.CheckedChanged, AddressOf CandidateSelected
                            currentGroupBox.Controls.Add(rb)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ShowDarkMessage("Error loading ballot: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CandidateSelected(sender As Object, e As EventArgs)
        Dim rb As RadioButton = CType(sender, RadioButton)
        If rb.Checked Then
            Dim data As Tuple(Of Integer, String) = CType(rb.Tag, Tuple(Of Integer, String))
            selectedChoices(data.Item2) = data.Item1
        End If
    End Sub

    Private Sub btnCastVote_Click(sender As Object, e As EventArgs) Handles btnCastVote.Click
        ' 1. Check for missing positions
        Dim missingPositions As New List(Of String)
        Dim allPositions As New List(Of String)

        Try
            Using conn = GetConnection()
                conn.Open()
                Using cmd As New MySqlCommand("SELECT DISTINCT Position FROM Candidates", conn)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            allPositions.Add(reader("Position").ToString())
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            ShowDarkMessage("Error checking positions: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        For Each pos In allPositions
            If Not selectedChoices.ContainsKey(pos) Then
                missingPositions.Add(pos)
            End If
        Next

        ' ONLY show the error if there actually are missing positions!
        If missingPositions.Count > 0 Then
            ShowDarkMessage("Please make a selection for the following positions:" & vbCrLf & vbCrLf & String.Join(vbCrLf, missingPositions), "Incomplete Ballot", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' 2. Confirm
        Dim confirm As DialogResult = ShowDarkMessage("Are you sure? You cannot change your vote after this.", "Confirm Vote", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If confirm = DialogResult.No Then Return

        ' 3. Save to Database
        Try
            Using conn = GetConnection()
                conn.Open()
                Using transaction = conn.BeginTransaction()
                    Try
                        For Each kvp In selectedChoices
                            Dim query As String = "INSERT INTO Votes (UserID, CandidateID, Position) VALUES (@uid, @cid, @pos)"
                            Using cmd As New MySqlCommand(query, conn, transaction)
                                cmd.Parameters.AddWithValue("@uid", currentUserID)
                                cmd.Parameters.AddWithValue("@cid", kvp.Value)
                                cmd.Parameters.AddWithValue("@pos", kvp.Key)
                                cmd.ExecuteNonQuery()
                            End Using
                        Next

                        Dim updateQuery As String = "UPDATE Users SET HasVoted = TRUE WHERE UserID = @uid"
                        Using cmd As New MySqlCommand(updateQuery, conn, transaction)
                            cmd.Parameters.AddWithValue("@uid", currentUserID)
                            cmd.ExecuteNonQuery()
                        End Using

                        transaction.Commit()
                    Catch ex As Exception
                        transaction.Rollback()
                        Throw New Exception("Database Save Error: " & ex.Message)
                    End Try
                End Using
            End Using

            ' 4. Check if results are published
            Dim isPublished As Boolean = False
            Using conn = GetConnection()
                conn.Open()
                Using cmd As New MySqlCommand("SELECT SettingValue FROM Settings WHERE SettingKey = 'PublishResults'", conn)
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso result.ToString() = "true" Then
                        isPublished = True
                    End If
                End Using
            End Using

            ' 5. Show final message
                       ' 5. Generate the secure Token
            Dim random As New Random()
            Dim tokenPart1 As String = random.Next(1000, 9999).ToString()
            Dim tokenPart2 As String = random.Next(1000, 9999).ToString()
            Dim voteToken As String = tokenPart1 & "-" & tokenPart2

            ' 6. Show the Success Screen
            lblToken.Text = voteToken
            PanelVote.Visible = False
            PanelSuccess.Visible = True

            ' 7. Reset login forms for next user
            txtLoginName.Clear()
            txtLoginID.Clear()

        Catch ex As Exception
            ShowDarkMessage("Voting Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ================= ADMIN LOGIC =================
    Private Sub btnGoToAdmin_Click(sender As Object, e As EventArgs) Handles btnGoToAdmin.Click
        PanelLogin.Visible = False
        PanelAdmin.Visible = True
        txtAdminPass.Clear()
        lstAdminCandidates.Visible = False
        txtCandName.Visible = False
        txtCandParty.Visible = False
        cmbCandPosition.Visible = False
        btnAddCandidate.Visible = False
        btnDeleteCandidate.Visible = False
        btnViewAdminResults.Visible = False
        btnPublishResults.Visible = False
        isAdminLoggedIn = False
    End Sub

    Private Sub btnAdminLogin_Click(sender As Object, e As EventArgs) Handles btnAdminLogin.Click
        If txtAdminPass.Text = "admin123" Then
            isAdminLoggedIn = True
            lstAdminCandidates.Visible = True
            txtCandName.Visible = True
            txtCandParty.Visible = True
            cmbCandPosition.Visible = True
            btnAddCandidate.Visible = True
            btnDeleteCandidate.Visible = True
            btnViewAdminResults.Visible = True
            btnPublishResults.Visible = True
            LoadAdminCandidates()
        Else
            ShowDarkMessage("Incorrect Admin Password. (Hint: admin123)", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub LoadAdminCandidates()
        lstAdminCandidates.Items.Clear()
        Using conn = GetConnection()
            conn.Open()
            Using cmd As New MySqlCommand("SELECT CandidateID, Name, Party, Position FROM Candidates ORDER BY Position", conn)
                Using reader = cmd.ExecuteReader()
                    While reader.Read()
                        lstAdminCandidates.Items.Add(New ComboItem(reader("CandidateID").ToString(), reader("Name").ToString() & " - " & reader("Position").ToString() & " (" & reader("Party").ToString() & ")"))
                    End While
                End Using
            End Using
        End Using
    End Sub

    Private Sub btnAddCandidate_Click(sender As Object, e As EventArgs) Handles btnAddCandidate.Click
        If String.IsNullOrEmpty(txtCandName.Text) Or cmbCandPosition.SelectedIndex = -1 Then
            ShowDarkMessage("Fill in all candidate details.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Using conn = GetConnection()
            conn.Open()
            Dim query As String = "INSERT INTO Candidates (Name, Party, Position) VALUES (@name, @party, @pos)"
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@name", txtCandName.Text)
                cmd.Parameters.AddWithValue("@party", txtCandParty.Text)
                cmd.Parameters.AddWithValue("@pos", cmbCandPosition.SelectedItem.ToString())
                cmd.ExecuteNonQuery()
            End Using
        End Using
        ShowDarkMessage("Candidate Added Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        txtCandName.Clear()
        txtCandParty.Clear()
        LoadAdminCandidates()
    End Sub

    Private Sub btnDeleteCandidate_Click(sender As Object, e As EventArgs) Handles btnDeleteCandidate.Click
        If lstAdminCandidates.SelectedIndex = -1 Then Return
        Dim selected As ComboItem = CType(lstAdminCandidates.SelectedItem, ComboItem)
        
        If ShowDarkMessage("Delete this candidate?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Using conn = GetConnection()
                    conn.Open()
                    Using cmd As New MySqlCommand("DELETE FROM Votes WHERE CandidateID = @id", conn)
                        cmd.Parameters.AddWithValue("@id", selected.ID)
                        cmd.ExecuteNonQuery()
                    End Using
                    Using cmd As New MySqlCommand("DELETE FROM Candidates WHERE CandidateID = @id", conn)
                        cmd.Parameters.AddWithValue("@id", selected.ID)
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
                LoadAdminCandidates()
            Catch ex As Exception
                ShowDarkMessage("Error deleting: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub btnViewAdminResults_Click(sender As Object, e As EventArgs) Handles btnViewAdminResults.Click
        PanelAdmin.Visible = False
        PanelResults.Visible = True
        LoadResults()
    End Sub

    Private Sub btnPublishResults_Click(sender As Object, e As EventArgs) Handles btnPublishResults.Click
        If ShowDarkMessage("Publish results to the public? Voters will be able to see them after logging in.", "Publish", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Using conn = GetConnection()
                conn.Open()
                Using cmd As New MySqlCommand("UPDATE Settings SET SettingValue = 'true' WHERE SettingKey = 'PublishResults'", conn)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
            ShowDarkMessage("Results are now live for the public!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub btnBackFromAdmin_Click(sender As Object, e As EventArgs) Handles btnBackFromAdmin.Click
        PanelAdmin.Visible = False
        PanelLogin.Visible = True
    End Sub

    ' ================= RESULTS LOGIC =================
    Private Sub LoadResults()
        ' Clear previous data
        chartBarData.Clear()
        chartPieData.Clear()

        ' Check if results are published
        If Not isAdminLoggedIn Then
            Using conn = GetConnection()
                conn.Open()
                Using cmd As New MySqlCommand("SELECT SettingValue FROM Settings WHERE SettingKey = 'PublishResults'", conn)
                    If cmd.ExecuteScalar().ToString() <> "true" Then
                        Dim lblHidden As New Label() With {.Text = "Results are currently hidden by the IEBC.", .ForeColor = Color.LightGray, .Font = New Font("Segoe UI", 16), .Location = New Point(100, 150), .AutoSize = True}
                        PanelChart.Controls.Clear()
                        PanelChart.Controls.Add(lblHidden)
                        Return
                    End If
                End Using
            End Using
        End If

        PanelChart.Controls.Clear()

        ' Fetch Data for Bar Chart and Line Graph
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT c.Name, COUNT(v.VoteID) AS VoteCount " &
                                      "FROM Candidates c LEFT JOIN Votes v ON c.CandidateID = v.CandidateID " &
                                      "GROUP BY c.CandidateID, c.Name ORDER BY VoteCount DESC"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            chartBarData.Add(New Tuple(Of String, Integer)(reader("Name").ToString(), Convert.ToInt32(reader("VoteCount"))))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading bar data: " & ex.Message)
        End Try

        ' Fetch Data for Pie Chart (Total votes per Position)
        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT c.Position, COUNT(v.VoteID) AS TotalVotes " &
                                      "FROM Candidates c LEFT JOIN Votes v ON c.CandidateID = v.CandidateID " &
                                      "GROUP BY c.Position"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            chartPieData.Add(New Tuple(Of String, Integer)(reader("Position").ToString(), Convert.ToInt32(reader("TotalVotes"))))
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading pie data: " & ex.Message)
        End Try

        ' Trigger the drawing of the charts
        PanelChart.Invalidate()
    End Sub
       Private Sub btnExportResults_Click(sender As Object, e As EventArgs) Handles btnExportResults.Click
        Try
            Dim filePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Election_Results_Report.txt")
            Using writer As New StreamWriter(filePath)
                writer.WriteLine("=== OFFICIAL ELECTION RESULTS ===")
                writer.WriteLine("Date: " & Date.Now.ToString())
                writer.WriteLine("=================================")
                For Each item As String In lstResults.Items
                    writer.WriteLine(item)
                Next
            End Using
            ShowDarkMessage("Report exported successfully to your Desktop!" & vbCrLf & filePath, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            ShowDarkMessage("Error exporting file: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub PanelChart_Paint(sender As Object, e As PaintEventArgs) Handles PanelChart.Paint
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit

        ' Colors for the charts
        Dim colors As Color() = {Color.FromArgb(88, 166, 255), Color.FromArgb(35, 134, 54), Color.FromArgb(210, 153, 34), Color.FromArgb(218, 54, 51), Color.FromArgb(163, 113, 247)}

        ' ==========================================
        ' 1. DRAW BAR CHART (Top Left)
        ' ==========================================
        Dim barX As Integer = 20
        Dim barY As Integer = 30
        Dim maxBarVotes As Integer = If(chartBarData.Count > 0, Math.Max(chartBarData.Max(Function(x) x.Item2), 1), 1)

        g.DrawString("Vote Distribution", New Font("Segoe UI", 12, FontStyle.Bold), Brushes.Gold, barX, 10)

        For i As Integer = 0 To Math.Min(chartBarData.Count - 1, 4) ' Show top 5 max
            Dim name As String = chartBarData(i).Item1
            Dim votes As Integer = chartBarData(i).Item2
            Dim barWidth As Integer = CInt((votes / maxBarVotes) * 150) ' Max width 150px

            ' Draw Name
            g.DrawString(name, New Font("Segoe UI", 9), Brushes.LightGray, barX, barY + (i * 25))
            ' Draw Bar
            g.FillRectangle(New SolidBrush(colors(i Mod colors.Length)), barX + 100, barY + (i * 25), barWidth, 15)
            ' Draw Votes
            g.DrawString(votes.ToString(), New Font("Segoe UI", 9, FontStyle.Bold), Brushes.White, barX + 100 + barWidth + 5, barY + (i * 25))
        Next

        ' ==========================================
        ' 2. DRAW PIE CHART (Top Right)
        ' ==========================================
        Dim pieX As Integer = 350
        Dim pieY As Integer = 30
        Dim pieSize As Integer = 180
        Dim totalVotes As Integer = If(chartPieData.Count > 0, chartPieData.Sum(Function(x) x.Item2), 1)
        Dim startAngle As Single = 0

        g.DrawString("Turnout by Position", New Font("Segoe UI", 12, FontStyle.Bold), Brushes.Gold, pieX, 10)

        For i As Integer = 0 To chartPieData.Count - 1
            Dim posName As String = chartPieData(i).Item1
            Dim votes As Integer = chartPieData(i).Item2
            Dim sweepAngle As Single = CSng(votes / totalVotes) * 360.0F

            ' Draw the pie slice
            g.FillPie(New SolidBrush(colors(i Mod colors.Length)), pieX, pieY, pieSize, pieSize, startAngle, sweepAngle)
            
            ' Draw a little legend box
            g.FillRectangle(New SolidBrush(colors(i Mod colors.Length)), pieX + pieSize + 20, pieY + (i * 25), 15, 15)
            g.DrawString(posName, New Font("Segoe UI", 8), Brushes.White, pieX + pieSize + 40, pieY + (i * 25))

            startAngle += sweepAngle
        Next

        ' ==========================================
        ' 3. DRAW LINE GRAPH (Bottom)
        ' ==========================================
        Dim lineX As Integer = 20
        Dim lineY As Integer = 280
        Dim lineWidth As Integer = 650
        Dim lineHeight As Integer = 150

        g.DrawString("Trend of Votes", New Font("Segoe UI", 12, FontStyle.Bold), Brushes.Gold, lineX, lineY - 20)
        
        ' Draw Axis Lines
        g.DrawLine(Pens.Gray, lineX, lineY, lineX, lineY + lineHeight) ' Y axis
        g.DrawLine(Pens.Gray, lineX, lineY + lineHeight, lineX + lineWidth, lineY + lineHeight) ' X axis

        If chartBarData.Count > 1 Then
            Dim pointDistance As Integer = lineWidth \ (chartBarData.Count - 1)
            Dim points As New List(Of Point)()

            For i As Integer = 0 To chartBarData.Count - 1
                Dim votes As Integer = chartBarData(i).Item2
                ' Calculate Y position (invert because Y goes down in graphics)
                Dim ptY As Integer = lineY + lineHeight - CInt((votes / maxBarVotes) * lineHeight)
                Dim ptX As Integer = lineX + (i * pointDistance)
                points.Add(New Point(ptX, ptY))
            Next

            ' Draw the Line
            If points.Count > 1 Then
                g.DrawLines(New Pen(Color.FromArgb(88, 166, 255), 3), points.ToArray())
            End If

            ' Draw the Dots on the line
            For Each pt In points
                g.FillEllipse(Brushes.White, pt.X - 4, pt.Y - 4, 8, 8)
            Next
        End If
    End Sub
    ' ================= HELPER FUNCTIONS =================
    ' This MUST be inside the Form1 class!
    Private Sub OnlyAllowNumbers(sender As Object, e As KeyPressEventArgs)
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then
            e.Handled = True ' Ignore letters/symbols
        End If
    End Sub

End Class ' <--- THIS CLOSES FORM1. Everything below here is OUTSIDE Form1.

' ================= INHERITANCE CLASSES =================
Public Class Person
    Public Property FullName As String
    Public Property IDNumber As String
End Class

Public Class Voter
    Inherits Person
    Public Property HasVoted As Boolean
End Class

Public Class Admin
    Inherits Person
    Public Property AdminPassword As String
End Class

' ================= HELPER CLASSES & MODULES =================
Public Class ComboItem
    Public Property ID As String
    Public Property DisplayText As String
    Public Sub New(id As String, text As String)
        Me.ID = id
        Me.DisplayText = text
    End Sub
    Public Overrides Function ToString() As String
        Return Me.DisplayText
    End Function
End Class

Public Module MessageBoxHelper
    Public Function ShowDarkMessage(text As String, title As String, buttons As MessageBoxButtons, icon As MessageBoxIcon) As DialogResult
        Dim isYesNo As Boolean = (buttons = MessageBoxButtons.YesNo)
        Dim msg As New DarkMessageBox(text, title, isYesNo)
        msg.ShowDialog()
        Return msg.Result
    End Function
End Module