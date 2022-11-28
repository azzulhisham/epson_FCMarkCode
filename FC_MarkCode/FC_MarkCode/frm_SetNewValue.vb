Public Class frm_SetNewValue

    Dim fg_Load As Integer = 1

    Private Sub frm_SetNewValue_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated

        If fg_Load = 0 Then Exit Sub
        fg_Load = 0

        With Me
            .txt_DataInput.Focus()
        End With

    End Sub


    Private Sub frm_SetNewValue_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        fg_Load = 1

        With FCLSR
            On Error Resume Next

            Me.lbl_Title.Text = EditTitle(.EditParameter.IdxNo)

            If .EditParameter.IdxNo = 14 Then
                Me.lbl_OldSetting.Text = .EditParameter.OldData
                Me.lbl_Default.Text = EditDefault(.EditParameter.IdxNo)
            Else
                Me.lbl_OldSetting.Text = String.Format(EditStrFmt(.EditParameter.IdxNo), Val(.EditParameter.OldData) / EditModifier(.EditParameter.IdxNo))
                Me.lbl_Default.Text = String.Format(EditStrFmt(.EditParameter.IdxNo), Val(EditDefault(.EditParameter.IdxNo)) / EditModifier(.EditParameter.IdxNo))
            End If

            Me.lbl_Range.Text = EditRng(.EditParameter.IdxNo)
            Me.txt_DataInput.Text = Me.lbl_OldSetting.Text
        End With

    End Sub

    Private Sub txt_DataInput_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt_DataInput.GotFocus

        With Me
            With .txt_DataInput
                .SelectionStart = 0
                .SelectionLength = .Text.Length
            End With
        End With

    End Sub

    Private Sub btn_Close_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Close.Click

        FCLSR.EditParameter.NewData = "-"
        Me.Close()

    End Sub

    Private Sub txt_DataInput_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt_DataInput.KeyDown

        If e.KeyCode = Keys.Escape Then
            FCLSR.EditParameter.NewData = "-"
            Me.Close()
        ElseIf e.KeyCode = Keys.Enter Then
            If ValidateData() Then
                FCLSR.EditParameter.NewData = Me.txt_DataInput.Text.Trim
                Me.Close()
            End If
        End If

    End Sub

    Private Function ValidateData() As Boolean

        With FCLSR
            If Not IsNumeric(Me.txt_DataInput.Text) Then
                MessageBox.Show("The input data is not a numeric value.", "Laser Marking...Invalid Data!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Return False
            End If

            If Not EditRng(.EditParameter.IdxNo) = "" Then
                Dim sRng As String = EditRng(.EditParameter.IdxNo).Trim
                Dim dcmMin As Decimal = CType(EditRng(.EditParameter.IdxNo).Substring(EditRng(.EditParameter.IdxNo).IndexOf("(") + 1, EditRng(.EditParameter.IdxNo).IndexOf("~") - (EditRng(.EditParameter.IdxNo).IndexOf("(") + 1)), Decimal)
                Dim dcmMax As Decimal = CType(EditRng(.EditParameter.IdxNo).Substring(EditRng(.EditParameter.IdxNo).IndexOf("~") + 1, EditRng(.EditParameter.IdxNo).IndexOf(")") - (EditRng(.EditParameter.IdxNo).IndexOf("~") + 1)), Decimal)
                Dim Comp As Decimal = CType(Me.txt_DataInput.Text.Trim, Decimal)

                If Comp < dcmMin Or Comp > dcmMax Then
                    MessageBox.Show("The input data is out of it range.", "Laser Marking...Data Out Of Range!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return False
                End If
            End If
        End With

        Return True

    End Function

    Private Sub btn_OK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_OK.Click

        If ValidateData() Then
            FCLSR.EditParameter.NewData = Me.txt_DataInput.Text.Trim
            Me.Close()
        End If

    End Sub

End Class