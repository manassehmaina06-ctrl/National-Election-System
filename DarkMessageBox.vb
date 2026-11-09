Imports System.Drawing
Imports System.Windows.Forms
Imports System.ComponentModel

Public Class DarkMessageBox
    Inherits Form

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property MessageText As String

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property TitleText As String

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Result As DialogResult = DialogResult.Cancel

    Public Sub New(text As String, title As String, showYesNo As Boolean)
        Me.MessageText = text
        Me.TitleText = title
        Me.Text = title
        Me.Size = New Size(450, 200)
        Me.BackColor = Color.FromArgb(13, 17, 23) ' Dark background
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.StartPosition = FormStartPosition.CenterParent
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        ' 1. Create the Message Label
        Dim lblMsg As New Label() With {
            .Text = text,
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 11),
            .Location = New Point(20, 20),
            .Size = New Size(400, 100)
        }

        ' 2. Create the Button(s)
        If showYesNo Then
            ' YES Button
            Dim btnYes As New Button() With {
                .Text = "Yes",
                .Location = New Point(120, 120),
                .Size = New Size(90, 35),
                .BackColor = Color.FromArgb(88, 166, 255),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }
            AddHandler btnYes.Click, Sub()
                                         Me.Result = DialogResult.Yes
                                         Me.Close()
                                     End Sub

            ' NO Button
            Dim btnNo As New Button() With {
                .Text = "No",
                .Location = New Point(230, 120),
                .Size = New Size(90, 35),
                .BackColor = Color.FromArgb(35, 134, 54),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }
            AddHandler btnNo.Click, Sub()
                                        Me.Result = DialogResult.No
                                        Me.Close()
                                    End Sub

            Me.Controls.Add(btnYes)
            Me.Controls.Add(btnNo)
        Else
            ' OK Button (if it's just a warning)
            Dim btnOk As New Button() With {
                .Text = "OK",
                .Location = New Point(175, 120),
                .Size = New Size(100, 35),
                .BackColor = Color.FromArgb(88, 166, 255),
                .ForeColor = Color.White,
                .FlatStyle = FlatStyle.Flat
            }
            AddHandler btnOk.Click, Sub()
                                        Me.Result = DialogResult.OK
                                        Me.Close()
                                    End Sub
            Me.Controls.Add(btnOk)
        End If

        Me.Controls.Add(lblMsg)
    End Sub
End Class