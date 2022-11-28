Public Class frm_SystemBusy

    Dim fg_Load As Integer = 1

    Private Sub frm_SystemBusy_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated

        If fg_Load = 0 Then Exit Sub
        fg_Load = 0

        With frm_Main.Progress1
            .Value = 0
            .Visible = True
        End With

        With FCLSR
            Select Case .SysBsyCode
                Case Is = 1     'Set Marking Data
                    If frm_Main.Set_ML_Mode() >= 0 Then
                        frm_Main.Progress1.Value = 10

                        With FCLSR
                            Dim ML_Cmd As String = "MRW" & .MarkingCondSetting.A_Layout & "," & FormPostStream(.MarkingSetting(0))
                            Dim RetCmd As Integer = frm_Main.SendMLCmd(ML_Cmd)

                            frm_Main.Progress1.Value = 70
                            RetCmd = RetCmd Or frm_Main.Set_ML_Mode(1)
                            frm_Main.Progress1.Value = 100

                            If Not RetCmd < 0 Then
                                .SysBsyCode = 0
                            End If
                        End With
                    Else
                        MessageBox.Show("Unabled to set System Mode...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If

                    frm_Main.Progress1.Visible = False
                    Me.Close()
                Case Is = 2     'Set Condition Setting 
                    If frm_Main.Set_ML_Mode() >= 0 Then
                        Dim RetCmd As Integer = 0
                        Dim ML_Cmd As String = String.Empty

                        frm_Main.Progress1.Value = 10

                        If My.Settings.AppSet = 0 Then
                            'Select Layout No.
                            ML_Cmd = "LNW" & .MarkingCondSetting.A_Layout
                            RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                            frm_Main.Progress1.Value = 15

                            If RetCmd < 0 Then
                                frm_Main.Progress1.Visible = False
                                MessageBox.Show("Unabled to select appropriate Marking Layout..." & vbCrLf & "Please refer to your system engineer.", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)

                                Me.Close()
                                Exit Sub
                            End If
                        End If

                        If My.Settings.AppSet = 0 Then
                            'Set Laser Current
                            ML_Cmd = "CUW" & .MarkingCondSetting.E_Current
                            RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                            frm_Main.Progress1.Value = 20
                        Else
                            RetCmd = 1
                        End If

                        If RetCmd < 0 Then
                            MessageBox.Show("Unabled to set Laser Current...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Else
                            If My.Settings.AppSet = 0 Then
                                'set Laser QSW
                                ML_Cmd = "PUW" & .MarkingCondSetting.F_QSW
                                RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                                frm_Main.Progress1.Value = 30
                            Else
                                RetCmd = 1
                            End If

                            If RetCmd < 0 Then
                                MessageBox.Show("Unabled to set QSW Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Else
                                If My.Settings.AppSet = 0 Then
                                    'Set Laser Speed
                                    ML_Cmd = "SPW" & .MarkingCondSetting.G_Speed
                                    RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                                    frm_Main.Progress1.Value = 40
                                Else
                                    RetCmd = 1
                                End If

                                If RetCmd < 0 Then
                                    MessageBox.Show("Unabled to set Laser Speed...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Else
                                    If My.Settings.AppSet = 0 Then
                                        'Set Rotation Angle
                                        ML_Cmd = "RTW" & .MarkingCondSetting.D_Rotation
                                        RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                                        frm_Main.Progress1.Value = 50
                                    Else
                                        RetCmd = 1
                                    End If

                                    If RetCmd < 0 Then
                                        MessageBox.Show("Unabled to set Laser Rotation Angle...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Else
                                        If My.Settings.AppSet = 0 Then
                                            'Set X-Axis Offset
                                            ML_Cmd = "XOW" & .MarkingCondSetting.B_Xoffset
                                            RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                                            frm_Main.Progress1.Value = 60
                                        Else
                                            RetCmd = 1
                                        End If

                                        If RetCmd < 0 Then
                                            MessageBox.Show("Unabled to set X-Axis Offset...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        Else
                                            If My.Settings.AppSet = 0 Then
                                                'Set Y-Axis Offset
                                                ML_Cmd = "YOW" & .MarkingCondSetting.C_Yoffset
                                                RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                                                frm_Main.Progress1.Value = 70
                                            Else
                                                RetCmd = 1
                                            End If

                                            If RetCmd < 0 Then
                                                MessageBox.Show("Unabled to set Y-Axis Offset...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                            Else
                                                With FCLSR
                                                    Dim StartBlock As Integer = 0
                                                    Dim LastBlock As Integer = IIf(Val(Profiles(.SelectedProfile).UseDot) <> 0, 2, 0) + Val(Profiles(.SelectedProfile).UseBlock)

                                                    If Profiles(.SelectedProfile).Spec.ToUpper.EndsWith("EX") Then
                                                        LastBlock = 1
                                                    End If

                                                    For iLp As Integer = StartBlock To LastBlock
                                                        ML_Cmd = "MRW" & .MarkingCondSetting.A_Layout & "," & FormPostStream(.MarkingSetting(iLp), , Profiles(.SelectedProfile).Spec)

                                                        If My.Settings.AppSet = 0 Then
                                                            RetCmd = frm_Main.SendMLCmd(ML_Cmd)
                                                        Else
                                                            RetCmd = 1
                                                        End If

                                                        If frm_Main.Progress1.Value < 90 Then
                                                            frm_Main.Progress1.Value += 5
                                                        End If

                                                        If RetCmd < 0 Then
                                                            MessageBox.Show("Unabled to set Marking Data on Block : " & String.Format("{0:D1}", iLp), "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                            Exit For
                                                        End If
                                                    Next

                                                    If RetCmd < 0 Then
                                                    Else
                                                        frm_Main.Progress1.Value = 90

                                                        If My.Settings.AppSet = 0 Then
                                                            RetCmd = RetCmd Or frm_Main.Set_ML_Mode(1)
                                                        Else
                                                            RetCmd = 1
                                                        End If

                                                        If Not RetCmd < 0 Then
                                                            frm_Main.Progress1.Value = 100
                                                            .SysBsyCode = 0
                                                        End If
                                                    End If
                                                End With
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Else
                        MessageBox.Show("Unabled to set System Mode...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If

                    frm_Main.Progress1.Visible = False
                    Me.Close()
                Case Is = 3     'Get Condition Setting 
                    If frm_Main.Set_ML_Mode() >= 0 Then
                        Dim Data As String = String.Empty
                        frm_Main.Progress1.Value = 10

                        'Get Laser Current
                        Dim ML_Cmd As String = "CUR"
                        Dim RetCmd As Integer = frm_Main.SendMLCmd(ML_Cmd, Data)
                        frm_Main.Progress1.Value = 20

                        If RetCmd < 0 Then
                            MessageBox.Show("Unabled to read Laser Current Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Else
                            Dim STXpos As Integer = Data.IndexOf(Chr(ch_STX))
                            Dim ETXpos As Integer = Data.IndexOf(Chr(ch_ETX))

                            If Not STXpos < 0 And Not ETXpos < 0 Then
                                Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                .MarkingCondSetting.E_Current = Data
                            End If

                            'Get Laser QSW
                            ML_Cmd = "PUR"
                            RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                            frm_Main.Progress1.Value = 30

                            If RetCmd < 0 Then
                                MessageBox.Show("Unabled to read Laser QSW Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Else
                                STXpos = Data.IndexOf(Chr(ch_STX))
                                ETXpos = Data.IndexOf(Chr(ch_ETX))

                                If Not STXpos < 0 And Not ETXpos < 0 Then
                                    Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                    .MarkingCondSetting.F_QSW = Data
                                End If

                                'Get Laser Speed
                                ML_Cmd = "SPR"
                                RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                                frm_Main.Progress1.Value = 40

                                If RetCmd < 0 Then
                                    MessageBox.Show("Unabled to read Laser Speed Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Else
                                    STXpos = Data.IndexOf(Chr(ch_STX))
                                    ETXpos = Data.IndexOf(Chr(ch_ETX))

                                    If Not STXpos < 0 And Not ETXpos < 0 Then
                                        Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                        .MarkingCondSetting.G_Speed = Data
                                    End If

                                    'Get Laser Rotation
                                    ML_Cmd = "RTR"
                                    RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                                    frm_Main.Progress1.Value = 50

                                    If RetCmd < 0 Then
                                        MessageBox.Show("Unabled to read Laser Rotation Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                    Else
                                        STXpos = Data.IndexOf(Chr(ch_STX))
                                        ETXpos = Data.IndexOf(Chr(ch_ETX))

                                        If Not STXpos < 0 And Not ETXpos < 0 Then
                                            Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                            .MarkingCondSetting.D_Rotation = Data
                                        End If

                                        'Get X-Axis Offset
                                        ML_Cmd = "XOR"
                                        RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                                        frm_Main.Progress1.Value = 60

                                        If RetCmd < 0 Then
                                            MessageBox.Show("Unabled to read X-Axis Offset Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        Else
                                            STXpos = Data.IndexOf(Chr(ch_STX))
                                            ETXpos = Data.IndexOf(Chr(ch_ETX))

                                            If Not STXpos < 0 And Not ETXpos < 0 Then
                                                Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                                .MarkingCondSetting.B_Xoffset = Data
                                            End If

                                            'Get Y-Axis Offset
                                            ML_Cmd = "YOR"
                                            RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                                            frm_Main.Progress1.Value = 70

                                            If RetCmd < 0 Then
                                                MessageBox.Show("Unabled to read X-Axis Offset Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                            Else
                                                STXpos = Data.IndexOf(Chr(ch_STX))
                                                ETXpos = Data.IndexOf(Chr(ch_ETX))

                                                If Not STXpos < 0 And Not ETXpos < 0 Then
                                                    Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                                    .MarkingCondSetting.C_Yoffset = Data
                                                End If

                                                'Get Marking Setting
                                                ML_Cmd = "MRR" & .MarkingCondSetting.A_Layout & "," & .MarkingSetting(0).LineNo
                                                RetCmd = frm_Main.SendMLCmd(ML_Cmd, Data)
                                                frm_Main.Progress1.Value = 80

                                                If RetCmd < 0 Then
                                                    MessageBox.Show("Unabled to read Marking Setting...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                Else
                                                    STXpos = Data.IndexOf(Chr(ch_STX))
                                                    ETXpos = Data.IndexOf(Chr(ch_ETX))

                                                    If Not STXpos < 0 And Not ETXpos < 0 Then
                                                        Data = Data.Substring(STXpos + 1, ETXpos - (STXpos + 1))
                                                        ReadParameter(.MarkingSetting(0), Data)
                                                        frm_Main.Progress1.Value = 100
                                                        .SysBsyCode = 0
                                                    End If
                                                End If
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Else
                        MessageBox.Show("Unabled to set System Mode...", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If

                    frm_Main.Progress1.Visible = False
                    Me.Close()
            End Select
        End With

    End Sub

    Private Sub frm_SystemBusy_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        With Me
            .tmr_Blink.Enabled = False
        End With

    End Sub

    Private Sub frm_SystemBusy_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        With Me
            fg_Load = 1

            With .tmr_Blink
                .Interval = 250
                .Enabled = True
            End With
        End With

    End Sub

End Class