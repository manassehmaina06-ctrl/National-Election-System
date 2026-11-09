Imports MySqlConnector
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO
Imports Microsoft.VisualBasic

Public Class Form1
    Private connectionString As String = "server=localhost;user=root;password=Maina@411;database=election_db;"
    
    Private currentUserID As Integer = -1
    Private currentUserName As String = ""
    Private currentUserIDNumber As String = ""
    Private selectedChoices As New Dictionary(Of String, Integer)()
    Private isAdminLoggedIn As Boolean = False

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
        lstResults.Items.Clear()
        
        If Not isAdminLoggedIn Then
            Using conn = GetConnection()
                conn.Open()
                Using cmd As New MySqlCommand("SELECT SettingValue FROM Settings WHERE SettingKey = 'PublishResults'", conn)
                    If cmd.ExecuteScalar().ToString() <> "true" Then
                        lstResults.Items.Add("Results are currently hidden by the IEBC.")
                        lstResults.Items.Add("Please check back later.")
                        Return
                    End If
                End Using
            End Using
        End If

        lstResults.Items.Add("--- OFFICIAL RESULTS ---")

        Try
            Using conn = GetConnection()
                conn.Open()
                Dim query As String = "SELECT c.Position, c.Name, c.Party, COUNT(v.VoteID) AS VoteCount " &
                                      "FROM Candidates c LEFT JOIN Votes v ON c.CandidateID = v.CandidateID " &
                                      "GROUP BY c.CandidateID, c.Position, c.Name, c.Party " &
                                      "ORDER BY FIELD(c.Position, 'President', 'Member of Parliament', 'Finance Secretary', 'Welfare Representative'), VoteCount DESC"
                Using cmd As New MySqlCommand(query, conn)
                    Using reader = cmd.ExecuteReader()
                        Dim currentPos As String = ""
                        While reader.Read()
                            Dim pos As String = reader("Position").ToString()
                            
                            If pos <> currentPos Then
                                currentPos = pos
                                lstResults.Items.Add("")
                                lstResults.Items.Add("=== " & currentPos.ToUpper() & " ===")
                            End If
                            
                            Dim name As String = reader("Name").ToString() & " (" & reader("Party").ToString() & ")"
                            Dim votes As String = reader("VoteCount").ToString()
                            
                            Dim bar As String = New String("█", Math.Min(Convert.ToInt32(votes), 30))
                            lstResults.Items.Add(name.PadRight(40) & " | " & votes & " votes " & bar)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Error loading results: " & ex.Message)
        End Try
    End Sub

    Private Sub btnBackFromResults_Click(sender As Object, e As EventArgs) Handles btnBackFromResults.Click
        PanelResults.Visible = False
        If isAdminLoggedIn Then
            PanelAdmin.Visible = True
        Else
            PanelLogin.Visible = True
        End If
    End Sub
    Private Sub btnFinish_Click(sender As Object, e As EventArgs) Handles btnFinish.Click
        ' Hide the success screen
        PanelSuccess.Visible = False
        
        ' Show the login screen again
        PanelLogin.Visible = True
        
        ' Optional: Clear the token text so it doesn't show the old one next time
        lblToken.Text = "XXXX-XXXX"
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