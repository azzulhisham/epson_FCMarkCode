Public Class frm_Custom

    Private Sub frm_Custom_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        With Me
            .txt_Custom.Text = FCLSR.Lotdata(1).WeekCode
        End With

    End Sub

    Private Sub frm_Custom_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        Me.txt_Custom.Focus()

    End Sub

    Private Sub txt_Custom_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_Custom.GotFocus

        With Me.txt_Custom
            .SelectAll()
        End With

    End Sub

    Private Sub txt_Custom_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt_Custom.KeyDown

        If e.KeyValue = Keys.Enter Then
            Me.btn_OK.PerformClick()
        End If

    End Sub

    Private Sub btn_OK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_OK.Click

        FCLSR.Lotdata(1).WeekCode = Me.txt_Custom.Text
        Me.Close()

    End Sub

End Class