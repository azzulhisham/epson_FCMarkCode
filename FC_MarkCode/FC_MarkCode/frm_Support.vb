Public Class frm_Support

    Dim fg_Load As Integer = 1
    Dim SeqNo As Integer = 0
    Dim MsgLbl() As String = {"Please Machine Control No. (M00000)...", _
                              "Kindly Wait For A Moment..."}


    Private Sub frm_Support_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        With Me
            .fg_Load = 1
            .SeqNo = 0

            With .txt_KeyIn
                .Text = ""
                .Visible = True
            End With

            .lbl_Progress.Visible = False
            DispMsg()
        End With

    End Sub

    Private Sub DispMsg()

        With Me
            .lbl_MsgBox.Text = MsgLbl(SeqNo)

            If SeqNo = .MsgLbl.GetUpperBound(0) Then
                .txt_KeyIn.Visible = False
                .lbl_Progress.Visible = True
            Else
                .lbl_Progress.Visible = False
                .txt_KeyIn.Visible = True
            End If
        End With

    End Sub

    Private Sub frm_Support_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        With Me
            .fg_Load = 0

            With .txt_KeyIn
                .Text = ""
                .Focus()
            End With
        End With

    End Sub

    Private Sub txt_KeyIn_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt_KeyIn.KeyDown

        With FCLSR
            If e.KeyCode = Keys.Escape Then
                With Me
                    .Close()
                End With
            ElseIf e.KeyCode = Keys.Enter Then
                Dim CtrlNoVal As String = Me.txt_KeyIn.Text.ToUpper.Trim

                If CtrlNoVal.StartsWith("M") And CtrlNoVal.Length >= 6 Then
                    .CtrlNo = CtrlNoVal
                    Me.txt_KeyIn.Text = ""
                    SeqNo += 1
                    DispMsg()

                    Me.lbl_Progress.Text = "Insert Default Data..."

                    Dim GetOldPmf As Integer = 0
                    Dim SQLcmd As String = _
                                "SELECT * FROM Setting WHERE CtrlNo='" & FCLSR.CtrlNo & "' " & _
                                "ORDER BY Spec"


                    If GetProfileDetailsFromServer(SQLcmd) < 0 Then
                        If Not InsertNewProfile_sql(DefaultProfile) < 0 Then
                            MessageBox.Show("Insert default parameter data completed successfully.", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            regSubKey.SetValue("CtrlNo", .CtrlNo)

                            Me.Close()
                        Else
                            MessageBox.Show("Insert default parameter data not successfully completed!", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                            Me.Close()
                        End If
                    Else
                        regSubKey.SetValue("CtrlNo", .CtrlNo)
                        Me.Close()
                    End If
                Else
                    MessageBox.Show("The control no. is invalid. It should have the following format (M00000).", "Invalid Control No.", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        End With

    End Sub



End Class