Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Web.Services.Protocols


Public Class frm_Main

    Dim azWebService As New ETMYMNET.az_Services

    Dim MyWeekDay() As String = {"", "Monday", "Tuesday", "Wednesday", "Thurday", "Friday", "Saturday", "Sunday"}
    Dim MyMonth() As String = {"", "Jan", "Feb", "Mac", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"}
    Dim MyAnimation(4) As PictureBox

    Dim fg_Load As Integer = 1
    Dim fg_Ignore As Integer = 0
    Dim fg_IgnoreSelect As Integer = 0


    Private Sub frm_Main_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If MessageBox.Show("Are you very sure you want close the system application ?", "Laser Marking...", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then
            e.Cancel = 1
            Exit Sub
        End If

        With Me
            .tmr_Blink.Enabled = False
            .tmr_WS.Enabled = False
        End With

        With FCLSR
            Miyachi.Close()
        End With

    End Sub


    Private Sub frm_Main_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        With FCLSR
            .SysBsyCode = 0
            ReDim .Lotdata(1)
            ReDim .MarkingSetting(5)
        End With

        With Me
            .fg_Load = 1
            .fg_Ignore = 0

            .lbl_WarningMsg.Text = ""
            .Text = "Laser Marking Data Composer " & String.Format("<Ver. : {0:D4}.{1:D2}.{2:D2}.{3:D3}>", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.MinorRevision)
            .pic_Animation.Tag = "0"

            MyAnimation(0) = New PictureBox
            MyAnimation(1) = New PictureBox
            MyAnimation(2) = New PictureBox
            MyAnimation(3) = New PictureBox
            MyAnimation(4) = New PictureBox

            MyAnimation(0) = .pic_Marking1
            MyAnimation(1) = .pic_Marking2
            MyAnimation(2) = .pic_Marking3
            MyAnimation(3) = .pic_Marking4
            MyAnimation(4) = .pic_Marking5

            DispCalender()
            SetMode()


            If ReadRegData() = Func_Ret_Success Then
                If InitSerialPort() = Func_Ret_Success Then
                    With .tmr_Blink
                        .Interval = 250
                        .Enabled = True
                    End With
                End If

                DispMarkingSetting()
            End If

            With .tmr_WS
                .Interval = 30 * 1000
                .Enabled = True
            End With

            With .tmr_Marking
                .Interval = 30
                .Enabled = False
            End With
        End With

    End Sub

    Private Sub DispMarkingSetting()

        With FCLSR
            Me.lbl_LayoutNo.Text = .MarkingCondSetting.A_Layout.ToString.Trim

            Me.lbl_CurSetting.Text = String.Format("{0:F1}", Val(.MarkingCondSetting.E_Current) / 10)
            Me.lbl_QSWSetting.Text = String.Format("{0:F1}", Val(.MarkingCondSetting.F_QSW) / 10)
            Me.lbl_SpeedSetting.Text = String.Format("{0:F2}", Val(.MarkingCondSetting.G_Speed) / 100)

            Me.lbl_XoffsetSetting.Text = String.Format("{0:F3}", Val(.MarkingCondSetting.B_Xoffset) / 1000)
            Me.lbl_YoffsetSetting.Text = String.Format("{0:F3}", Val(.MarkingCondSetting.C_Yoffset) / 1000)
            Me.lbl_RotateSetting.Text = String.Format("{0:F6}", Val(.MarkingCondSetting.D_Rotation) / 1000000)
            Me.lbl_LayoutSetting.Text = .MarkingCondSetting.A_Layout

            Me.lbl_SetCurrent_.Text = String.Format("{0:F1}", Val(.MarkingCondSetting.E_Current) / 10)
            Me.lbl_SetQSW_.Text = String.Format("{0:F1}", Val(.MarkingCondSetting.F_QSW) / 10)
            Me.lbl_SetSpeed_.Text = String.Format("{0:F2}", Val(.MarkingCondSetting.G_Speed) / 100)

            Me.lbl_SetXoffset_.Text = String.Format("{0:F3}", Val(.MarkingCondSetting.B_Xoffset) / 1000)
            Me.lbl_SetYoffset_.Text = String.Format("{0:F3}", Val(.MarkingCondSetting.C_Yoffset) / 1000)
            Me.lbl_SetRotation_.Text = String.Format("{0:F6}", Val(.MarkingCondSetting.D_Rotation) / 1000000)

            Me.cbo_DrawType.SelectedIndex = Val(.MarkingSetting(0).A_DrawType)
            Me.cbo_TextAlign.SelectedIndex = Val(.MarkingSetting(0).E_TextAlign)
            Me.cbo_SpaceAlign.SelectedIndex = Val(.MarkingSetting(0).G_SpaceAlign)

            Me.lbl_Xaxis.Text = String.Format("{0:F3}", Val(.MarkingSetting(0).B_X_Axis) / 1000)
            Me.lbl_Yaxis.Text = String.Format("{0:F3}", Val(.MarkingSetting(0).C_Y_Axis) / 1000)
            Me.lbl_TextAngle.Text = String.Format("{0:F1}", Val(.MarkingSetting(0).D_TextAngle) / 10)
            Me.lbl_WidthAlign.Text = .MarkingSetting(0).F_WidthAlign
            Me.lbl_SpaceWidth.Text = String.Format("{0:F3}", Val(.MarkingSetting(0).H_SpaceWidth) / 1000)
            Me.lbl_XaxisOrg.Text = String.Format("{0:F3}", Val(.MarkingSetting(0).I_X_AxisOrg) / 1000)
            Me.lbl_YaxisOrg.Text = String.Format("{0:F3}", Val(.MarkingSetting(0).J_Y_AxisOrg) / 1000)
            Me.lbl_CharHeight.Text = String.Format("{0:F3}", Val(.MarkingSetting(0).K_CharHeight) / 1000)
            Me.lbl_Compress.Text = .MarkingSetting(0).L_Compress
            Me.lbl_OppDir.Text = .MarkingSetting(0).M_OppDir
            Me.lbl_CharAngle.Text = String.Format("{0:F1}", Val(.MarkingSetting(0).N_CharAngle) / 10)
            Me.lbl_Current.Text = String.Format("{0:F1}", Val(.MarkingSetting(0).O_Current) / 10)
            Me.lbl_QSW.Text = String.Format("{0:F1}", Val(.MarkingSetting(0).P_QSW) / 10)
            Me.lbl_Speed.Text = String.Format("{0:F2}", Val(.MarkingSetting(0).Q_Speed) / 100)
            Me.lbl_Repeat.Text = .MarkingSetting(0).R_Repeat
            Me.lbl_VarType.Text = .MarkingSetting(0).T_VarType
            Me.lbl_VarNo.Text = .MarkingSetting(0).U_VarNo
        End With

    End Sub

    Private Sub SetMode(Optional ByVal WriteMode As Boolean = True)

        With Me
            .tmr_Marking.Enabled = Not WriteMode

            If WriteMode Then
                .lbl_LotNo.Text = ""
                .lbl_EmpNo.Text = ""
                .lbl_WeekCode.Text = ""
                .lbl_WarningMsg.Visible = True
                .lbl_WarningMsg.Text = "Click 'Data Entry' Button To Set Marking Data..."
            End If


            .pic_PostDone.Visible = False
            .pic_Post.Visible = Not WriteMode
            .pic_Write.Visible = WriteMode

            .btn_Post.Visible = Not WriteMode
            .btn_Post.Enabled = Not WriteMode
            .btn_DataEntry.Visible = WriteMode

            .btn_Cancel.Enabled = Not WriteMode
        End With

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_DataEntry.Click

        frm_DataEntry.ShowDialog(Me)

        With FCLSR
            If Not .Lotdata(1).LotNo = "" And Not .Lotdata(1).IMINo = "" Then
                If Not .Lotdata(1).WeekCode = "" Then
                    If Me.cbo_Profiles.FindString(.Lotdata(1).Parameter) < 0 Then
                        MessageBox.Show("No profiles was setup for this product code.", "Profiles Not Found...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Exit Sub
                    End If

                    With .Lotdata(1)
                        Me.lbl_LotNo.Text = .LotNo
                        Me.lbl_EmpNo.Text = .EmpNo
                        Me.lbl_WeekCode.Text = .WeekCode
                        Me.lbl_VarNo.Text = .WeekCode
                        Me.lbl_SpecNo.Text = .IMINo
                    End With

                    Me.SetMode(False)
                    Me.cbo_Profiles.SelectedIndex = Me.cbo_Profiles.FindString(.Lotdata(1).Parameter)
                    .SelectedProfile = Me.cbo_Profiles.SelectedIndex
                Else
                    MessageBox.Show("Fail to generate marking data. Please refer to the system Web Service.", "FC_MarkCode...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                End If
            End If
        End With

    End Sub

    Private Sub StatusStrip1_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles StatusStrip1.MouseEnter

        Me.ToolTip1.Show(azWebService.HelloWorld, Me.StatusStrip1)

    End Sub

    Private Sub StatusStrip1_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles StatusStrip1.MouseLeave

        Me.ToolTip1.Show("", Me.StatusStrip1)

    End Sub

    Private Sub tmr_WS_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_WS.Tick

        With Me
            Try
                With .WS_Status
                    .ToolTipText = azWebService.HelloWorld
                    .Text = "Connected..."
                End With

                If .lbl_WeekCode.Text = "" Then
                    Dim sWkCd As String = azWebService.azWeekCode_FC("")

                    If Not .lbl_WarningMsg.Text = ImportantMsg1 & "<" & sWkCd & ">" Then
                        .lbl_WarningMsg.Text = ImportantMsg1 & "<" & sWkCd & ">"
                    End If
                End If
            Catch ex As Exception
                With .WS_Status
                    .ToolTipText = ""
                    .Text = "Disconnected..."
                End With
            End Try
        End With

    End Sub

    Private Sub tmr_Blink_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_Blink.Tick

        Static PicNo As Integer


        With Me
            .pic_Animation.BackgroundImage = .MyAnimation(PicNo).BackgroundImage
            PicNo += 1 : If PicNo > .MyAnimation.GetUpperBound(0) Then PicNo = 0

            .DispCalender()

            If .btn_Post.Visible = True And .btn_Post.Enabled = True Then
                .pic_Post.Visible = Not .pic_Post.Visible
            Else
                If Not .btn_Post.Enabled = True Then
                    If .pic_Post.Visible = True Then .pic_Post.Visible = False
                End If
            End If
        End With

    End Sub

    Private Sub DispCalender()

        Dim MyDay As Date = Now


        With Me
            .lbl_YearVal.Text = String.Format("{0:D4}", MyDay.Year)
            .lbl_MonthVal.Text = MyMonth(MyDay.Month)
            .lbl_DayVal.Text = String.Format("{0:D2}", MyDay.Day)
            .lbl_WeekDayVal.Text = MyWeekDay(MyDay.DayOfWeek)
            .lbl_TimeVal.Text = String.Format("{0:D2}:{1:D2}:{2:D2}", MyDay.Hour, MyDay.Minute, MyDay.Second)

            If .lbl_DayVal.Text.EndsWith("1") Then
                If MyDay.Day = 11 Then
                    .lbl_thLabel.Text = "th"
                Else
                    .lbl_thLabel.Text = "st"
                End If
            ElseIf .lbl_DayVal.Text.EndsWith("2") Then
                If MyDay.Day = 12 Then
                    .lbl_thLabel.Text = "th"
                Else
                    .lbl_thLabel.Text = "nd"
                End If
            ElseIf .lbl_DayVal.Text.EndsWith("3") Then
                If MyDay.Day = 13 Then
                    .lbl_thLabel.Text = "th"
                Else
                    .lbl_thLabel.Text = "rd"
                End If
            End If
        End With

    End Sub

    Private Sub btn_Cancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_Cancel.Click

        SetMode()

    End Sub

    Private Sub btn_ML_ErrRst_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_ErrRst.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        Dim ML_Cmd As String = "ERR"
        Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

        Me.pic_Animation.Tag = "0"

    End Sub

    Public Function SendMLCmd(ByVal ML_Cmd As String, Optional ByRef RepData As String = "") As Integer

        Dim RetData As String = String.Empty
        Dim RetCmd As Integer = ML7111A_cmd(ML_Cmd, RetData)


        With Me
            Dim NowDate As Date = Now
            Dim LogData As String = String.Format("{0:D2}-{1:D2}-{2:D4} {3:D2}:{4:D2}:{5:D2}", NowDate.Day, NowDate.Month, NowDate.Year, NowDate.Hour, NowDate.Minute, NowDate.Second) & vbTab

            LogData &= ML_Cmd

            If RetCmd >= 0 Then
                RepData = RetData
                LogData &= vbTab & "->" & vbTab & RetData & vbCrLf
            Else
                LogData &= vbTab & "->" & vbTab & "Fail..." & vbCrLf
            End If

            LogData &= .txt_LogData.Text
            .txt_LogData.Text = LogData
        End With

        Return RetCmd

    End Function

    Private Sub btn_ML_CurMode_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_ML_CurMode.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        Dim ML_Cmd As String = "RLR"
        Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_Send_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Send.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = Me.txt_CustomCmd.Text.Trim.ToUpper
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Public Function Set_ML_Mode(Optional ByVal IntExt As Integer = 0) As Integer

        Dim ML_Cmd As String = "RLW" & IntExt.ToString.Trim
        Return SendMLCmd(ML_Cmd)

    End Function

    Private Sub btn_ML_CurLayout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_CurLayout.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "LNR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_CheckLD_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_CheckLD.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "LMR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetQSW_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetQSW.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "PUR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetSpeed_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetSpeed.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "SPR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetCurrent_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetCurrent.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "CUR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetRotation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetRotation.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "RTR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetYoffset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetYoffset.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "YOR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetXoffset_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetXoffset.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "XOR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_ML_GetMarkingStatus_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_ML_GetMarkingStatus.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "MSR"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_Post_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Post.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
            .lbl_WarningMsg.Visible = False
        End With

        FCLSR.SysBsyCode = 1
        frm_SystemBusy.ShowDialog(Me)

        With Me
            .pic_Animation.Tag = "0"

            If FCLSR.SysBsyCode = 0 Then
                .pic_PostDone.Visible = True
                .btn_Post.Enabled = False
                .btn_Cancel.Text = "Reset"
                MessageBox.Show("Data has been completely transfered !", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                .btn_Post.Enabled = True
                .btn_Cancel.Text = "Cancel"
                .lbl_WarningMsg.Visible = True
                MessageBox.Show("Data not being transfered.", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End With

    End Sub

    Private Sub btn_SetLD_ON_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_SetLD_ON.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "LMW1"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub btn_Marking_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Marking.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        If Set_ML_Mode() >= 0 Then
            Dim ML_Cmd As String = "MSW1"
            Dim RetCmd As Integer = SendMLCmd(ML_Cmd)

            Set_ML_Mode(1)
        End If

        Me.pic_Animation.Tag = "0"

    End Sub

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged

        With FCLSR
            If Not Me.TabControl1.SelectedIndex = 0 Then
                .AuthenticalAccess = ""
                .TempCondSetting = .MarkingCondSetting
                .TempSetting = .MarkingSetting(0)

                With Me
                    Me.DispMarkingSetting()
                    .btn_Save.Enabled = False
                    .btn_PostSetting.Enabled = False
                End With

                frm_Access.ShowDialog(Me)

                If .AuthenticalAccess = .AuthenticationCode Then
                    'If Me.TabControl1.SelectedIndex = 1 Then
                    'End If
                Else
                    Me.TabControl1.SelectedIndex = 0
                End If
            End If
        End With

    End Sub

    Private Sub tmr_Marking_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_Marking.Tick

        Static WarnMsg As Integer


        With Me
            If .btn_Post.Visible = True And .btn_Post.Enabled = True Then
                .lbl_WarningMsg.Text = WarningMsg1.Substring(0, WarnMsg + 1)
                WarnMsg += 1
                If WarnMsg >= WarningMsg1.Length Then WarnMsg = 0
            Else
                If Not .btn_Post.Enabled = True Or Not .btn_Post.Visible = True Then
                    WarnMsg = 0
                End If
            End If
        End With

    End Sub

    Private Sub lbl_SetCurrent__DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbl_SetCurrent_.DoubleClick, lbl_SetQSW_.DoubleClick, lbl_SetSpeed_.DoubleClick, lbl_SetXoffset_.DoubleClick, lbl_SetYoffset_.DoubleClick, lbl_SetRotation_.DoubleClick

        With FCLSR
            If sender.Equals(Me.lbl_SetCurrent_) Then
                .EditParameter.IdxNo = 0
                .EditParameter.OldData = .MarkingCondSetting.E_Current
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetCurrent_.Text = .EditParameter.NewData
                    .TempCondSetting.E_Current = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SetQSW_) Then
                .EditParameter.IdxNo = 1
                .EditParameter.OldData = .MarkingCondSetting.F_QSW
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetQSW_.Text = .EditParameter.NewData
                    .TempCondSetting.F_QSW = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SetSpeed_) Then
                .EditParameter.IdxNo = 2
                .EditParameter.OldData = .MarkingCondSetting.G_Speed
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetSpeed_.Text = .EditParameter.NewData
                    .TempCondSetting.G_Speed = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SetXoffset_) Then
                .EditParameter.IdxNo = 3
                .EditParameter.OldData = .MarkingCondSetting.B_Xoffset
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetXoffset_.Text = .EditParameter.NewData
                    .TempCondSetting.B_Xoffset = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SetYoffset_) Then
                .EditParameter.IdxNo = 4
                .EditParameter.OldData = .MarkingCondSetting.C_Yoffset
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetYoffset_.Text = .EditParameter.NewData
                    .TempCondSetting.C_Yoffset = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SetRotation_) Then
                .EditParameter.IdxNo = 5
                .EditParameter.OldData = .MarkingCondSetting.D_Rotation
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SetRotation_.Text = .EditParameter.NewData
                    .TempCondSetting.D_Rotation = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            End If
        End With

    End Sub

    Private Sub btn_Save_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_Save.Click

        With FCLSR
            .MarkingCondSetting = .TempCondSetting
            .MarkingSetting(0) = .TempSetting

            Dim NewSetting As String = FormPostStream(.MarkingSetting(0), 1)
            SaveRegMarkingConditionSetting(.MarkingCondSetting, NewSetting)
        End With

        With Me
            .btn_Save.Enabled = False
            .btn_PostSetting.Enabled = True
            .DispMarkingSetting()
        End With

    End Sub

    Private Sub btn_PostSetting_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btn_PostSetting.Click

        With Me
            If Not .pic_Animation.Tag = "0" Then Exit Sub
            .pic_Animation.Tag = "1"
        End With

        FCLSR.SysBsyCode = 2
        frm_SystemBusy.ShowDialog(Me)

        With Me
            .pic_Animation.Tag = "0"

            If FCLSR.SysBsyCode = 0 Then
                .btn_PostSetting.Enabled = False
                MessageBox.Show("Setting Data has been completely transfered !", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                .btn_PostSetting.Enabled = True
                MessageBox.Show("Setting Data not being transfered.", "Laser Marking...", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End With

    End Sub

    Private Sub cbo_DrawType_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cbo_DrawType.MouseDown

        If e.Button = Windows.Forms.MouseButtons.Right Then
            With Me
                .ContextMenuStrip1.Show(.cbo_DrawType, e.X, e.Y)
            End With
        End If

    End Sub

    Private Sub cbo_DrawType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbo_DrawType.SelectedIndexChanged

        If Me.fg_Ignore = 1 Or Me.fg_Load = 1 Then Exit Sub

        With FCLSR
            If .MarkingSetting(0).A_DrawType = .TempSetting.A_DrawType Then
                If Not Me.cbo_DrawType.SelectedIndex = Val(.MarkingSetting(0).A_DrawType) Then
                    Me.cbo_DrawType.SelectedIndex = Val(.MarkingSetting(0).A_DrawType)
                End If
            Else
                If Not Me.cbo_DrawType.SelectedIndex = Val(.TempSetting.A_DrawType) Then
                    Me.cbo_DrawType.SelectedIndex = Val(.TempSetting.A_DrawType)
                End If
            End If
        End With

    End Sub

    Private Sub cbo_TextAlign_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cbo_TextAlign.MouseDown

        If e.Button = Windows.Forms.MouseButtons.Right Then
            With Me
                .ContextMenuStrip2.Show(.cbo_TextAlign, e.X, e.Y)
            End With
        End If

    End Sub

    Private Sub cbo_TextAlign_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbo_TextAlign.SelectedIndexChanged

        If Me.fg_Ignore = 1 Or Me.fg_Load = 1 Then Exit Sub

        With FCLSR
            If .MarkingSetting(0).E_TextAlign = .TempSetting.E_TextAlign Then
                If Not Me.cbo_TextAlign.SelectedIndex = Val(.MarkingSetting(0).E_TextAlign) Then
                    Me.cbo_TextAlign.SelectedIndex = Val(.MarkingSetting(0).E_TextAlign)
                End If
            Else
                If Not Me.cbo_TextAlign.SelectedIndex = Val(.TempSetting.E_TextAlign) Then
                    Me.cbo_TextAlign.SelectedIndex = Val(.TempSetting.E_TextAlign)
                End If
            End If
        End With

    End Sub

    Private Sub cbo_SpaceAlign_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cbo_SpaceAlign.MouseDown

        If e.Button = Windows.Forms.MouseButtons.Right Then
            With Me
                .ContextMenuStrip3.Show(.cbo_SpaceAlign, e.X, e.Y)
            End With
        End If

    End Sub

    Private Sub cbo_SpaceAlign_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbo_SpaceAlign.SelectedIndexChanged

        If Me.fg_Ignore = 1 Or Me.fg_Load = 1 Then Exit Sub

        With FCLSR
            If .MarkingSetting(0).G_SpaceAlign = .TempSetting.G_SpaceAlign Then
                If Not Me.cbo_SpaceAlign.SelectedIndex = Val(.MarkingSetting(0).G_SpaceAlign) Then
                    Me.cbo_SpaceAlign.SelectedIndex = Val(.MarkingSetting(0).G_SpaceAlign)
                End If
            Else
                If Not Me.cbo_SpaceAlign.SelectedIndex = Val(.TempSetting.G_SpaceAlign) Then
                    Me.cbo_SpaceAlign.SelectedIndex = Val(.TempSetting.G_SpaceAlign)
                End If
            End If
        End With

    End Sub

    Private Sub ContextMenuStrip1_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening

    End Sub

    Private Sub sub_DrawType_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles sub_DrawType.Click, sub_TextAlign.Click, sub_SpaceAlign.Click

        With FCLSR
            If sender.Equals(Me.sub_DrawType) Then
                .EditParameter.IdxNo = 0
                .EditParameter.OldData = .MarkingSetting(0).A_DrawType
                frm_SetOptionVal.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.fg_Ignore = 1

                    Me.cbo_DrawType.SelectedIndex = Val(.EditParameter.NewData)
                    .TempSetting.A_DrawType = .EditParameter.NewData
                    Me.fg_Ignore = 0
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.sub_TextAlign) Then
                .EditParameter.IdxNo = 1
                .EditParameter.OldData = .MarkingSetting(0).E_TextAlign
                frm_SetOptionVal.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.fg_Ignore = 1

                    Me.cbo_TextAlign.SelectedIndex = Val(.EditParameter.NewData)
                    .TempSetting.E_TextAlign = .EditParameter.NewData
                    Me.fg_Ignore = 0
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.sub_SpaceAlign) Then
                .EditParameter.IdxNo = 2
                .EditParameter.OldData = .MarkingSetting(0).G_SpaceAlign
                frm_SetOptionVal.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.fg_Ignore = 1

                    Me.cbo_SpaceAlign.SelectedIndex = Val(.EditParameter.NewData)
                    .TempSetting.G_SpaceAlign = .EditParameter.NewData
                    Me.fg_Ignore = 0
                    Me.btn_Save.Enabled = True
                End If
            End If
        End With

    End Sub

    Private Sub lbl_Xaxis_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbl_Xaxis.Click

    End Sub

    Private Sub lbl_Xaxis_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbl_Xaxis.DoubleClick, lbl_Yaxis.DoubleClick, lbl_TextAngle.DoubleClick, lbl_WidthAlign.DoubleClick, lbl_SpaceWidth.DoubleClick, lbl_XaxisOrg.DoubleClick, lbl_YaxisOrg.DoubleClick, lbl_CharHeight.DoubleClick, lbl_Compress.DoubleClick

        With FCLSR
            If sender.Equals(Me.lbl_Xaxis) Then
                .EditParameter.IdxNo = 6
                .EditParameter.OldData = .MarkingSetting(0).B_X_Axis
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_Xaxis.Text = .EditParameter.NewData
                    .TempSetting.B_X_Axis = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_Yaxis) Then
                .EditParameter.IdxNo = 7
                .EditParameter.OldData = .MarkingSetting(0).C_Y_Axis
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_Yaxis.Text = .EditParameter.NewData
                    .TempSetting.C_Y_Axis = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_TextAngle) Then
                .EditParameter.IdxNo = 8
                .EditParameter.OldData = .MarkingSetting(0).D_TextAngle
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_TextAngle.Text = .EditParameter.NewData
                    .TempSetting.D_TextAngle = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_WidthAlign) Then
                .EditParameter.IdxNo = 9
                .EditParameter.OldData = .MarkingSetting(0).F_WidthAlign
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_WidthAlign.Text = .EditParameter.NewData
                    .TempSetting.F_WidthAlign = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SpaceWidth) Then
                .EditParameter.IdxNo = 10
                .EditParameter.OldData = .MarkingSetting(0).H_SpaceWidth
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SpaceWidth.Text = .EditParameter.NewData
                    .TempSetting.H_SpaceWidth = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_XaxisOrg) Then
                .EditParameter.IdxNo = 11
                .EditParameter.OldData = .MarkingSetting(0).I_X_AxisOrg
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_XaxisOrg.Text = .EditParameter.NewData
                    .TempSetting.I_X_AxisOrg = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_YaxisOrg) Then
                .EditParameter.IdxNo = 12
                .EditParameter.OldData = .MarkingSetting(0).J_Y_AxisOrg
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_YaxisOrg.Text = .EditParameter.NewData
                    .TempSetting.J_Y_AxisOrg = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_CharHeight) Then
                .EditParameter.IdxNo = 13
                .EditParameter.OldData = .MarkingSetting(0).K_CharHeight
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_CharHeight.Text = .EditParameter.NewData
                    .TempSetting.K_CharHeight = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_Compress) Then
                .EditParameter.IdxNo = 14
                .EditParameter.OldData = .MarkingSetting(0).L_Compress
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_Compress.Text = .EditParameter.NewData
                    .TempSetting.L_Compress = .EditParameter.NewData
                    Me.btn_Save.Enabled = True
                End If
            End If
        End With

    End Sub

    Private Sub frm_Main_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        If fg_Load = 1 Then
            fg_Load = 0

            With Me
                .SQL_Status.Text = FCLSR.DataBase_.Server
                Dim FuncRet As Integer = CheckDatabase()

                If FuncRet < 0 Then
                    Select Case FuncRet
                        Case Is = -1
                            MessageBox.Show("The system database is currently not attached to the system.", "az_ActiveProc", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Case Is = -2
                            MessageBox.Show("Ping command issued to the Database Server (" & FCLSR.DataBase_.Server & ") was fail.", "az_ActiveProc", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Case Is = -3
                            MessageBox.Show("The network is currently not available. Kindly check with your system engineer.", "az_ActiveProc", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Case Is = -4
                            MessageBox.Show("The system could not reach the Database server (" & FCLSR.DataBase_.Server & ").", "az_ActiveProc", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Select
                Else
                    FuncRet = GetMarkingSetting()

                    If FuncRet = Func_Ret_Success Then
                        .cbo_Profiles.Items.Clear()

                        For ilp As Integer = 0 To Profiles.GetUpperBound(0)
                            Application.DoEvents()
                            .cbo_Profiles.Items.Add(Profiles(ilp).Spec)
                        Next

                        .cbo_Profiles.SelectedIndex = 0

                        If Profiles.GetUpperBound(0) < 1 Then
                            .btn_Delete.Enabled = False
                        Else
                            .btn_Delete.Enabled = True
                        End If
                    Else
                        If FCLSR.CtrlNo = "M00000" Or FuncRet = -9999 Then
                            MessageBox.Show("No profile was being attached to the system. The system will loading up the default setting instead.", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            frm_Support.ShowDialog(Me)

                            'Reload
                            FuncRet = GetMarkingSetting()

                            If FuncRet = Func_Ret_Success Then
                                .Text = "Laser Marking - Active Procedure " & String.Format("<Ver. : {0:D4}.{1:D2}.{2:D2}.{3:D3}>", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.MinorRevision) & " --> #" & FCLSR.CtrlNo
                                .cbo_Profiles.Items.Clear()

                                For ilp As Integer = 0 To Profiles.GetUpperBound(0)
                                    Application.DoEvents()
                                    .cbo_Profiles.Items.Add(Profiles(ilp).Spec)
                                Next

                                .cbo_Profiles.SelectedIndex = 0

                                If Profiles.GetUpperBound(0) < 1 Then
                                    .btn_Delete.Enabled = False
                                Else
                                    .btn_Delete.Enabled = True
                                End If
                            End If
                        End If
                    End If
                End If


                Try
                    With .WS_Status
                        .ToolTipText = azWebService.HelloWorld
                        .Text = "Connected..."
                    End With
                Catch ex As Exception
                    With .WS_Status
                        .ToolTipText = ""
                        .Text = "Disconnected..."
                    End With
                End Try
            End With

            With FCLSR
                '.SysBsyCode = 3
                'frm_SystemBusy.ShowDialog(Me)
                'Dim NewSetting As String = String.Empty

                'If .SysBsyCode = 0 Then
                '    NewSetting = FormPostStream(.MarkingSetting(0), 1)
                'Else
                '    NewSetting = ""
                'End If

                'SaveRegMarkingConditionSetting(.MarkingCondSetting, NewSetting)
                'Me.DispMarkingSetting()
            End With

            Me.pic_Animation.Tag = "0"
        End If

    End Sub

    Private Sub CustomMarkingCodeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CustomMarkingCodeToolStripMenuItem.Click

        frm_Custom.ShowDialog()

        With FCLSR.Lotdata(1)
            Me.lbl_WeekCode.Text = .WeekCode
            Me.lbl_VarNo.Text = .WeekCode
        End With

    End Sub

    Private Sub cbo_Profiles_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbo_Profiles.SelectedIndexChanged

        With Me
            .btn_Save.Enabled = False
            ParseParamData(.cbo_Profiles.SelectedIndex)

            Me.DispMarkingSetting()
            DispMarkingSetting()

            .GroupBox5.Text = "Laser Setting ~ " & .cbo_Profiles.Text
        End With

    End Sub

    Private Sub btn_Copy_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_Copy.Click

        frm_NewProfiles.ShowDialog(Me)

        With FCLSR
            If Not .DataTrans = "" And Not .NewLayoutNo = "" Then
                If Not Me.cbo_Profiles.FindString(.DataTrans) < 0 Then
                    MessageBox.Show("The profile name already exists in the system Database. Duplicated name can not be used.", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If

                Me.fg_IgnoreSelect = 1
                Dim NewProfile As ParameterProfile = Profiles(Me.cbo_Profiles.SelectedIndex)
                .CurSelection = .DataTrans

                Dim AffectedRec As Integer = 0

                NewProfile.Spec = .DataTrans
                NewProfile.SettingCond.A_Layout = .NewLayoutNo
                AffectedRec = InsertNewProfile_sql(NewProfile)


                Dim FuncRet As Integer = GetMarkingSetting()

                With Me
                    If FuncRet = Func_Ret_Success Then
                        .cbo_Profiles.Items.Clear()

                        For ilp As Integer = 0 To Profiles.GetUpperBound(0)
                            Application.DoEvents()
                            .cbo_Profiles.Items.Add(Profiles(ilp).Spec)
                        Next

                        .fg_IgnoreSelect = 0
                        .cbo_Profiles.SelectedIndex = .cbo_Profiles.FindString(FCLSR.DataTrans)
                    End If
                End With
            End If

            Me.fg_IgnoreSelect = 0
        End With

    End Sub

End Class
