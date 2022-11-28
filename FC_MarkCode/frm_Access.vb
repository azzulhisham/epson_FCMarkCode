Public Class frm_Access

    Private Sub txt_Scan_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt_Scan.KeyDown

        With Me
            If e.KeyCode = Keys.Escape Then
                .Close()
            ElseIf e.KeyCode = Keys.Enter Then
                FCLSR.AuthenticalAccess = Me.txt_Scan.Text
                .txt_Scan.Text = ""
                .Close()
            End If
        End With

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Me.Close()

    End Sub

    Private Sub frm_Access_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        Me.txt_Scan.Focus()

    End Sub

End Class