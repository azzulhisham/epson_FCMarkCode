Public Class frm_NewProfiles

    Dim SeqNo As Integer = 0
    Dim MsgLbl() As String = {"Enter New Profile Name...", _
                              "Enter Layout No..."}


    Private Sub DispMsg()

        With Me
            .lbl_Msg.Text = MsgLbl(SeqNo)

            'If SeqNo = .MsgLbl.GetUpperBound(0) Then
            '    .TextBox1.Visible = False
            'Else
            '    .TextBox1.Visible = True
            'End If
        End With

    End Sub

    Private Sub frm_NewProfiles_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        With FCLSR
            .DataTrans = ""
            .NewLayoutNo = ""
        End With

        SeqNo = 0
        DispMsg()

    End Sub

    Private Sub TextBox1_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox1.GotFocus

        With Me
            .TextBox1.SelectionStart = 0
            .TextBox1.SelectionLength = .TextBox1.Text.Length
        End With

    End Sub

    Private Sub TextBox1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown

        With FCLSR
            If e.KeyValue = Keys.Enter Then
                Select Case SeqNo
                    Case Is = 0
                        If Me.TextBox1.Text = "" Then
                            MessageBox.Show("Profile name can not be empty!", "Inavlid Name...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                        End If

                        .DataTrans = Me.TextBox1.Text.ToUpper
                        Me.TextBox1.Text = ""
                        SeqNo += 1
                        DispMsg()
                    Case Is = 1
                        If Me.TextBox1.Text = "" Or Val(Me.TextBox1.Text) = 0 Then
                            MessageBox.Show("Please specify Layout No. to be mapped to. The Layout No. can not be empty or zero.", "Inavlid Layout No...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                        End If

                        .NewLayoutNo = Me.TextBox1.Text.ToUpper
                        Me.TextBox1.Text = ""
                        Me.Close()
                End Select
            ElseIf e.KeyValue = Keys.Escape Then
                .DataTrans = ""
                Me.TextBox1.Text = ""
                Me.Close()
            End If
        End With

    End Sub

    Private Sub frm_NewProfiles_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        With Me
            .TextBox1.Focus()
        End With

    End Sub

    Private Sub Button1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.Click

        With Me
            .TextBox1.Text = ""
            .Close()
        End With

    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged

        With Me
            If SeqNo = 1 Then
                If Not IsNumeric(.TextBox1.Text) Then
                    .ErrorProvider1.SetError(.TextBox1, "Must be a numeric value!")
                Else
                    .ErrorProvider1.SetError(.TextBox1, "")
                End If
            End If
        End With

    End Sub

End Class