Public Class frm_SetOptionVal

    Dim fg_Load As Integer = 1


    Private Sub btn_Close_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Close.Click

        Me.Close()

    End Sub

    Private Sub frm_SetOptionVal_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        fg_Load = 1

        With FCLSR
            Select Case .EditParameter.IdxNo
                Case Is = 0
                    Me.lbl_Title.Text = EditOption(.EditParameter.IdxNo)
                    Me.lbl_OldSetting.Text = DrawOption(Val(.EditParameter.OldData))
                    Me.lbl_Default.Text = DrawOption(Val(DrawOptionDefault))
                    Me.cbo_NewValue.Items.Clear()

                    For iLp As Integer = 0 To DrawOption.GetUpperBound(0)
                        Application.DoEvents()
                        Me.cbo_NewValue.Items.Add(DrawOption(iLp))
                    Next

                    Me.cbo_NewValue.SelectedIndex = Val(.EditParameter.OldData)
                Case Is = 1
                    Me.lbl_Title.Text = EditOption(.EditParameter.IdxNo)
                    Me.lbl_OldSetting.Text = TextAlignOption(Val(.EditParameter.OldData))
                    Me.lbl_Default.Text = TextAlignOption(Val(TextAlignOptionDefault))
                    Me.cbo_NewValue.Items.Clear()

                    For iLp As Integer = 0 To TextAlignOption.GetUpperBound(0)
                        Application.DoEvents()
                        Me.cbo_NewValue.Items.Add(TextAlignOption(iLp))
                    Next

                    Me.cbo_NewValue.SelectedIndex = Val(.EditParameter.OldData)
                Case Is = 2
                    Me.lbl_Title.Text = EditOption(.EditParameter.IdxNo)
                    Me.lbl_OldSetting.Text = SpaceAlignOption(Val(.EditParameter.OldData))
                    Me.lbl_Default.Text = SpaceAlignOption(Val(SpaceAlignOptionDefault))
                    Me.cbo_NewValue.Items.Clear()

                    For iLp As Integer = 0 To SpaceAlignOption.GetUpperBound(0)
                        Application.DoEvents()
                        Me.cbo_NewValue.Items.Add(SpaceAlignOption(iLp))
                    Next

                    Me.cbo_NewValue.SelectedIndex = Val(.EditParameter.OldData)
            End Select

            .EditParameter.NewData = "-"
        End With

    End Sub

    Private Sub btn_OK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_OK.Click

        With FCLSR
            .EditParameter.NewData = Me.cbo_NewValue.SelectedIndex.ToString
            Me.Close()
        End With

    End Sub

End Class