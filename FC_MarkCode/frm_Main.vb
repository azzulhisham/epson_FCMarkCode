Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Web.Services.Protocols


Public Class frm_Main

    Dim azWebService As New ETMYMNET.az_Services

    Dim MyWeekDay() As String = {"", "Monday", "Tuesday", "Wednesday", "Thurday", "Friday", "Saturday", "Sunday"}
    Dim MyMonth() As String = {"", "Jan", "Feb", "Mac", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"}
    Dim MyAnimation(4) As PictureBox
    Dim SettingPtr(2) As Label

    Dim fg_Load As Integer = 1
    Dim fg_Ignore As Integer = 0
    Dim fg_IgnoreSelect As Integer = 0
    Dim fg_Update_ As Integer = 0


    Private Sub frm_Main_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        If Me.fg_Update_ = 0 Then
            If MessageBox.Show("Are you very sure you want close the system application ?", "Laser Marking...", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.No Then
                e.Cancel = 1
                Exit Sub
            End If
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
            .fg_Update_ = 0

            .lbl_WarningMsg.Text = ""
            .Text = "Laser Marking System " & String.Format("<Ver. : {0:D4}.{1:D2}.{2:D2}.{3:D3}>", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.Revision) & IIf(FCLSR.CtrlNo = "", "", " - #" & FCLSR.CtrlNo)
            .pic_Animation.Tag = "0"

            .lbl_thLabel.BringToFront()
            .lbl_YearVal.BringToFront()

            For iLp As Integer = 0 To MyAnimation.GetUpperBound(0)
                MyAnimation(iLp) = New PictureBox
            Next

            For iLp As Integer = 0 To SettingPtr.GetUpperBound(0)
                SettingPtr(iLp) = New Label
            Next

            MyAnimation(0) = .pic_Marking1
            MyAnimation(1) = .pic_Marking2
            MyAnimation(2) = .pic_Marking3
            MyAnimation(3) = .pic_Marking4
            MyAnimation(4) = .pic_Marking5

            SettingPtr(0) = .lbl_Set1
            SettingPtr(1) = .lbl_Set2
            SettingPtr(2) = .lbl_Set3


            With .ComboBox1
                .Items.Clear()
                .Refresh()

                For iLp As Integer = 0 To SettingPtr.GetUpperBound(0)
                    .Items.Add(String.Format("{0:D1}", iLp + 1))
                Next
            End With

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
                '.Enabled = True
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


            Dim _SetNo As Integer = Me.ComboBox1.SelectedIndex
            If _SetNo < 0 Then _SetNo = 0

            Me.cbo_DrawType.SelectedIndex = Val(.MarkingSetting(_SetNo).A_DrawType)
            Me.cbo_TextAlign.SelectedIndex = Val(.MarkingSetting(_SetNo).E_TextAlign)
            Me.cbo_SpaceAlign.SelectedIndex = Val(.MarkingSetting(_SetNo).G_SpaceAlign)

            Me.lbl_Xaxis.Text = String.Format("{0:F3}", Val(.MarkingSetting(_SetNo).B_X_Axis) / 1000)
            Me.lbl_Yaxis.Text = String.Format("{0:F3}", Val(.MarkingSetting(_SetNo).C_Y_Axis) / 1000)
            Me.lbl_TextAngle.Text = String.Format("{0:F1}", Val(.MarkingSetting(_SetNo).D_TextAngle) / 10)
            Me.lbl_WidthAlign.Text = .MarkingSetting(_SetNo).F_WidthAlign
            Me.lbl_SpaceWidth.Text = String.Format("{0:F3}", Val(.MarkingSetting(_SetNo).H_SpaceWidth) / 1000)
            Me.lbl_XaxisOrg.Text = String.Format("{0:F3}", Val(.MarkingSetting(_SetNo).I_X_AxisOrg) / 1000)
            Me.lbl_YaxisOrg.Text = String.Format("{0:F3}", Val(.MarkingSetting(_SetNo).J_Y_AxisOrg) / 1000)
            Me.lbl_CharHeight.Text = String.Format("{0:F3}", Val(.MarkingSetting(_SetNo).K_CharHeight) / 1000)
            Me.lbl_Compress.Text = .MarkingSetting(_SetNo).L_Compress
            Me.lbl_OppDir.Text = .MarkingSetting(_SetNo).M_OppDir
            Me.lbl_CharAngle.Text = String.Format("{0:F1}", Val(.MarkingSetting(_SetNo).N_CharAngle) / 10)
            Me.lbl_Current.Text = String.Format("{0:F1}", Val(.MarkingSetting(_SetNo).O_Current) / 10)
            Me.lbl_QSW.Text = String.Format("{0:F1}", Val(.MarkingSetting(_SetNo).P_QSW) / 10)
            Me.lbl_Speed.Text = String.Format("{0:F2}", Val(.MarkingSetting(_SetNo).Q_Speed) / 100)
            Me.lbl_Repeat.Text = .MarkingSetting(_SetNo).R_Repeat
            Me.lbl_VarType.Text = .MarkingSetting(_SetNo).T_VarType
            Me.lbl_VarNo.Text = .MarkingSetting(_SetNo).U_VarNo
        End With

    End Sub

    Private Sub SetMode(Optional ByVal WriteMode As Boolean = True)

        With Me
            .tmr_Marking.Enabled = Not WriteMode

            If WriteMode Then
                '.lbl_LotNo.Text = ""
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
                Dim fg_Extend As String = String.Empty

                'bypass re-scan lot
                If .Lotdata(1).LotNo.Length > 10 And (.Lotdata(1).LotNo.ToUpper.EndsWith("EX") Or .Lotdata(1).LotNo.ToUpper.EndsWith("NW")) Then
                    fg_Extend = .Lotdata(1).LotNo
                    .Lotdata(1).LotNo = .Lotdata(1).LotNo.Substring(0, .Lotdata(1).LotNo.Length - 2)
                End If


                If Not .Lotdata(1).WeekCode = "" Then
                    If Me.cbo_Profiles.FindString(.Lotdata(1).Parameter) < 0 Then
                        MessageBox.Show("No profiles was setup for this product code.", "Profiles Not Found...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Exit Sub
                    End If

                    Dim WeekCode As String = .Lotdata(1).WeekCode

                    Me.SetMode(False)
                    Me.cbo_Profiles.SelectedIndex = Me.cbo_Profiles.FindString(.Lotdata(1).Parameter)
                    .SelectedProfile = Me.cbo_Profiles.SelectedIndex

                    With .Lotdata(1)
                        Dim _wkcd() As String = {}
                        Dim prevRecCnt As Rec = New Rec
                        Dim prevRec As Rec = New Rec
                        Dim chkPrevCnt As Integer = GetRecordsFromServer(.LotNo, prevRecCnt)
                        Dim chkPrevRec As Integer = GetRecordsFromServer(.LotNo, .IMINo, prevRec)

                        If String.IsNullOrEmpty(fg_Extend) And chkPrevCnt > 0 Then
                            If Convert.ToInt32(FCLSR.Restriction) > 0 Then
                                MessageBox.Show("This lot already exists in the system. Sorry the system cannot proceed the marking.", "FC_MarkCode...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                SetMode()
                                Exit Sub
                            End If
                        Else
                            If Not String.IsNullOrEmpty(fg_Extend) And chkPrevCnt > 0 Then
                                If fg_Extend.EndsWith("NW") Then
                                    chkPrevRec = 0
                                ElseIf fg_Extend.EndsWith("EX") Then
                                    If Not chkPrevRec > 0 Then
                                        MessageBox.Show("Illegal operation - IMI not match.", "FC_MarkCode...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                                        SetMode()
                                        Exit Sub
                                    End If
                                End If
                            End If
                        End If


                        If Not IsNothing(WeekCode) AndAlso WeekCode.Contains(","c) Then
                            _wkcd = WeekCode.Split(",")

                            If chkPrevRec > 0 Then
                                prevRec.MData2 = prevRec.MData2.Replace("o", "")
                                _wkcd(0) = prevRec.MData2.Trim()

                                If .Parameter.ToLower.EndsWith("ex") Then
                                    _wkcd(1) = prevRec.MData1.Trim()
                                End If
                            End If

                            If .Parameter.ToLower.EndsWith("ex") Then
                                FCLSR.Lotdata(1).WeekCode = _wkcd(0) & vbCrLf & "o " & _wkcd(1)

                                SettingPtr(1).Text = _wkcd(0)
                                SettingPtr(2).Text = _wkcd(1)
                            Else
                                FCLSR.Lotdata(1).WeekCode = _wkcd(1) & vbCrLf & "o " & _wkcd(0)

                                SettingPtr(1).Text = _wkcd(1)
                                SettingPtr(2).Text = _wkcd(0)
                            End If
                        Else
                            If IsNothing(WeekCode) Then
                                SettingPtr(1).Text = "3030"
                                SettingPtr(2).Text = "AymdS"
                            Else
                                If chkPrevRec > 0 Then
                                    If WeekCode.Contains(vbCrLf) Then
                                        If chkPrevRec > 0 Then
                                            'prevRec.MData2 = prevRec.MData2.Replace("o", "")
                                            .WeekCode = .WeekCode.Substring(0, .WeekCode.IndexOf("o")) & prevRec.MData2.Trim
                                        End If
                                    Else
                                        prevRec.MData2 = prevRec.MData2.Replace("o", "")
                                        .WeekCode = prevRec.MData2.Trim()
                                    End If
                                End If
                            End If
                        End If


                        If Profiles(Me.cbo_Profiles.SelectedIndex).UseDot = "0" Then
                            .WeekCode = .WeekCode.Replace("o ", "")
                        End If

                        Me.lbl_LotNo.Text = .LotNo
                        Me.lbl_EmpNo.Text = .EmpNo
                        Me.lbl_WeekCode.Text = .WeekCode
                        Me.lbl_VarNo.Text = .WeekCode
                        Me.lbl_SpecNo.Text = .IMINo
                    End With
                Else
                    MessageBox.Show("Fail to generate marking data. Please do an audit to the system Web Service data. " & _
                                    "It may due to the illegal operation with mismatched 'Spec. No.'.", "FC_MarkCode...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
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

    Private Sub AppReStart()

        'RemoveHandler System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateCompleted, AddressOf Me.AppReStart
        'MessageBox.Show("The Application has completely update.", "Network Deployment...", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Application.Restart()

    End Sub

    Private Sub tmr_WS_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_WS.Tick

        With My.Computer
            If .Network.IsAvailable = True Then
                If .Network.Ping("172.16.59.254") Then
                    Try
                        If System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed AndAlso System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CheckForUpdate Then
                            If Me.btn_DataEntry.Visible = True Then
                                With Me
                                    .tmr_WS.Enabled = False
                                    .Opacity = 0.7
                                End With

                                MessageBox.Show("The program detected newer version is available on the publisher's network ! Please click 'OK' to proceed the update now!", "Network Deployment...", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                Me.fg_Update_ = 1

                                AddHandler System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateCompleted, AddressOf AppReStart
                                System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateAsync()

                                Exit Sub
                            End If
                        End If
                    Catch ex As Exception

                    End Try
                End If
            End If
        End With

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

        If My.Settings.AppSet = 0 Then
            Dim ML_Cmd As String = "RLW" & IntExt.ToString.Trim
            Return SendMLCmd(ML_Cmd)
        Else
            Return 1
        End If

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

        FCLSR.SysBsyCode = 2
        frm_SystemBusy.ShowDialog(Me)

        With Me
            .pic_Animation.Tag = "0"

            If FCLSR.SysBsyCode = 0 Then
                .pic_PostDone.Visible = True
                .btn_Post.Enabled = False
                .btn_Cancel.Text = "Reset"

                Dim DateRecord As Date = Now
                FCLSR.Lotdata(1).RecDate = String.Format("{0:D2}-{1:D2}-{2:D4} {3:D2}:{4:D2}:{5:D2}", DateRecord.Month, DateRecord.Day, DateRecord.Year, DateRecord.Hour, DateRecord.Minute, DateRecord.Second)

                Dim FuncRet As Integer = 0
                Dim FormMarking(13) As String
                Dim WeekCode(2) As String


                With FCLSR.Lotdata(1)
                    If .WeekCode.Contains(vbCrLf) Then
                        .WeekCode = .WeekCode.Replace(vbCr, "")
                        .WeekCode = .WeekCode.Replace(vbLf, "-")


                        If .Parameter.ToLower.EndsWith("ex") Then
                            WeekCode = .WeekCode.Split("-")

                            Dim tmpLoc As String = WeekCode(1)
                            WeekCode(1) = WeekCode(0)
                            WeekCode(0) = tmpLoc
                        Else
                            .WeekCode = .WeekCode.Substring(.WeekCode.IndexOf("-") + 1)
                            WeekCode(0) = "-"
                            WeekCode(1) = .WeekCode
                        End If
                    Else
                        WeekCode(0) = "-"
                        WeekCode(1) = .WeekCode
                    End If

                    FormMarking(0) = .LotNo
                    FormMarking(1) = .IMINo
                    FormMarking(2) = String.Format("{0:F6}", Val(.Freq))
                    FormMarking(3) = .EmpNo
                    FormMarking(4) = .RecDate
                    FormMarking(5) = .Parameter
                    FormMarking(6) = FCLSR.CtrlNo
                    FormMarking(7) = "-"
                    FormMarking(8) = WeekCode(0)
                    FormMarking(9) = WeekCode(1)
                    FormMarking(10) = "-"
                    FormMarking(11) = "-"
                    FormMarking(12) = "-"
                    FormMarking(13) = "-"
                End With

                FuncRet = azWebService.AddOrUpdateRecords(FormMarking)
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
                    If Me.TabControl1.SelectedIndex = 1 And .SelectedProfile < 0 Then
                        MessageBox.Show("Sorry...! No profile being selected...", "ActiveProc...", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Me.TabControl1.SelectedIndex = 0
                    End If
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

        'Setting of RA8130
        '2,0,380,2700,,,1,320,,,,490,62,,0,,,,,,,0,B49UK
        '2,660,680,2700,,,1,320,,,,490,62,,0,,,,,,,0,R8130
        '2,100,480,0,,,,,,,,350,100,,0,,,,,,,2,1100001



        'With FCLSR
        '    .MarkingCondSetting = .TempCondSetting
        '    .MarkingSetting(0) = .TempSetting

        '    Dim NewSetting As String = FormPostStream(.MarkingSetting(0), 1)
        '    SaveRegMarkingConditionSetting(.MarkingCondSetting, NewSetting)
        'End With

        'With Me
        '    .btn_Save.Enabled = False
        '    .btn_PostSetting.Enabled = True
        '    .DispMarkingSetting()
        'End With


        Dim _SetNo As Integer = Me.ComboBox1.SelectedIndex
        If _SetNo < 0 Then _SetNo = 0


        With FCLSR
            .MarkingCondSetting = .TempCondSetting
            .MarkingSetting(_SetNo) = .TempSetting
            .MarkingSetting(_SetNo).LineNo = _SetNo + Profiles(.SelectedProfile).StartNo

            Dim NewSetting As String = FormPostStream(.MarkingSetting(_SetNo), 1, Me.cbo_Profiles.SelectedItem)

            'If Not Me.lbl_MarkChar4.Text = "" Then
            '    If Me.cbo_BlockNo.SelectedIndex = 3 Then
            '        If .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).B_X_Axis = "0" Or .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).C_Y_Axis = "0" Or _
            '            .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).K_CharHeight = "" Or .MarkingSetting(Me.cbo_BlockNo.SelectedIndex).L_Compress = "" Then
            '            .MarkingSetting(Me.cbo_BlockNo.SelectedIndex) = .MarkingSetting(Me.cbo_BlockNo.SelectedIndex - 1)
            '            NewSetting = FormPostStream(.MarkingSetting(Me.cbo_BlockNo.SelectedIndex), 1)
            '            NewSetting = NewSetting.Substring(0, NewSetting.LastIndexOf(",") + 1) & "_"
            '        End If
            '    End If
            'End If

            With Profiles(Me.cbo_Profiles.SelectedIndex)
                .ParamData(_SetNo).SettingString = NewSetting
                .SettingCond.A_Layout = FCLSR.MarkingCondSetting.A_Layout
                .SettingCond.B_Xoffset = FCLSR.MarkingCondSetting.B_Xoffset
                .SettingCond.C_Yoffset = FCLSR.MarkingCondSetting.C_Yoffset
                .SettingCond.D_Rotation = FCLSR.MarkingCondSetting.D_Rotation
                .SettingCond.E_Current = FCLSR.MarkingCondSetting.E_Current
                .SettingCond.F_QSW = FCLSR.MarkingCondSetting.F_QSW
                .SettingCond.G_Speed = FCLSR.MarkingCondSetting.G_Speed



                Dim SQLcmd As String = "UPDATE Setting SET " & _
                                        "LayoutNo='" & .SettingCond.A_Layout & "', " & _
                                        "Xoffset='" & .SettingCond.B_Xoffset & "', " & _
                                        "Yoffset='" & .SettingCond.C_Yoffset & "', " & _
                                        "Rotate='" & .SettingCond.D_Rotation & "', " & _
                                        "[Current]='" & .SettingCond.E_Current & "', " & _
                                        "QSW='" & .SettingCond.F_QSW & "', " & _
                                        "Speed='" & .SettingCond.G_Speed & "', "

                SQLcmd &= String.Format("Block{0:D1}='", _SetNo + 1)
                SQLcmd &= .ParamData(_SetNo).SettingString & "', " & _
                    "UseDot='" & .UseDot & "', " & _
                    "UseBlock='0' " & _
                    "WHERE CtrlNo='" & FCLSR.CtrlNo & "' AND Spec='" & .Spec & "'"


                Dim SQLrslt As Integer = SQL_Server_Proc(SQLcmd, "Marking")

                If SQLrslt < 0 Then
                    MessageBox.Show("The SQL command fail to execute correctly. The Data is not being saved!", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Else
                    If SQLrslt > 1 Then
                        MessageBox.Show("There are more than 1 records affected which is incorrect. Please check with your system engineer.", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    End If
                End If
            End With

            'SaveRegMarkingConditionSetting(.MarkingCondSetting, NewSetting, Me.cbo_BlockNo.SelectedIndex)
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
            Dim _SetNo As Integer = Me.ComboBox1.SelectedIndex
            If _SetNo < 0 Then _SetNo = 0

            If sender.Equals(Me.sub_DrawType) Then
                .EditParameter.IdxNo = 0
                .EditParameter.OldData = .MarkingSetting(_SetNo).A_DrawType
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
                .EditParameter.OldData = .MarkingSetting(_SetNo).E_TextAlign
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
                .EditParameter.OldData = .MarkingSetting(_SetNo).G_SpaceAlign
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

    Private Sub lbl_Xaxis_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbl_Xaxis.DoubleClick, lbl_Yaxis.DoubleClick, lbl_TextAngle.DoubleClick, lbl_WidthAlign.DoubleClick, lbl_SpaceWidth.DoubleClick, lbl_XaxisOrg.DoubleClick, lbl_YaxisOrg.DoubleClick, lbl_CharHeight.DoubleClick, lbl_Compress.DoubleClick

        With FCLSR
            Dim _SetNo As Integer = Me.ComboBox1.SelectedIndex
            If _SetNo < 0 Then _SetNo = 0

            If sender.Equals(Me.lbl_Xaxis) Then
                .EditParameter.IdxNo = 6
                .EditParameter.OldData = .MarkingSetting(_SetNo).B_X_Axis
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_Xaxis.Text = .EditParameter.NewData
                    .TempSetting.B_X_Axis = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_Yaxis) Then
                .EditParameter.IdxNo = 7
                .EditParameter.OldData = .MarkingSetting(_SetNo).C_Y_Axis
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_Yaxis.Text = .EditParameter.NewData
                    .TempSetting.C_Y_Axis = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_TextAngle) Then
                .EditParameter.IdxNo = 8
                .EditParameter.OldData = .MarkingSetting(_SetNo).D_TextAngle
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_TextAngle.Text = .EditParameter.NewData
                    .TempSetting.D_TextAngle = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_WidthAlign) Then
                .EditParameter.IdxNo = 9
                .EditParameter.OldData = .MarkingSetting(_SetNo).F_WidthAlign
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_WidthAlign.Text = .EditParameter.NewData
                    .TempSetting.F_WidthAlign = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_SpaceWidth) Then
                .EditParameter.IdxNo = 10
                .EditParameter.OldData = .MarkingSetting(_SetNo).H_SpaceWidth
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_SpaceWidth.Text = .EditParameter.NewData
                    .TempSetting.H_SpaceWidth = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_XaxisOrg) Then
                .EditParameter.IdxNo = 11
                .EditParameter.OldData = .MarkingSetting(_SetNo).I_X_AxisOrg
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_XaxisOrg.Text = .EditParameter.NewData
                    .TempSetting.I_X_AxisOrg = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_YaxisOrg) Then
                .EditParameter.IdxNo = 12
                .EditParameter.OldData = .MarkingSetting(_SetNo).J_Y_AxisOrg
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_YaxisOrg.Text = .EditParameter.NewData
                    .TempSetting.J_Y_AxisOrg = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_CharHeight) Then
                .EditParameter.IdxNo = 13
                .EditParameter.OldData = .MarkingSetting(_SetNo).K_CharHeight
                frm_SetNewValue.ShowDialog(Me)

                If Not .EditParameter.NewData = "-" And Not .EditParameter.NewData = "" Then
                    Me.lbl_CharHeight.Text = .EditParameter.NewData
                    .TempSetting.K_CharHeight = (Val(.EditParameter.NewData) * EditModifier(.EditParameter.IdxNo)).ToString
                    Me.btn_Save.Enabled = True
                End If
            ElseIf sender.Equals(Me.lbl_Compress) Then
                .EditParameter.IdxNo = 14
                .EditParameter.OldData = .MarkingSetting(_SetNo).L_Compress
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
                        .Text = "Laser Marking - Active Procedure " & String.Format("<Ver. : {0:D4}.{1:D2}.{2:D2}.{3:D3}>", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.Revision) & " --> #" & FCLSR.CtrlNo
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
                                .Text = "Laser Marking - Active Procedure " & String.Format("<Ver. : {0:D4}.{1:D2}.{2:D2}.{3:D3}>", My.Application.Info.Version.Major, My.Application.Info.Version.Minor, My.Application.Info.Version.Build, My.Application.Info.Version.Revision) & " --> #" & FCLSR.CtrlNo
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

            Dim _wkcd() As String = {}

            If Not IsNothing(FCLSR.Lotdata(1).WeekCode) AndAlso FCLSR.Lotdata(1).WeekCode.Contains(","c) Then
                _wkcd = FCLSR.Lotdata(1).WeekCode.Split(",")
                FCLSR.Lotdata(1).WeekCode = _wkcd(1) & vbCrLf & "o " & _wkcd(0)
                SettingPtr(1).Text = _wkcd(1)
                SettingPtr(2).Text = _wkcd(0)
            Else
                If IsNothing(FCLSR.Lotdata(1).WeekCode) Then
                    SettingPtr(1).Text = "3030"
                    SettingPtr(2).Text = "AymdS"
                End If
            End If


            If .cbo_Profiles.SelectedItem.Contains("SG") Or
                .cbo_Profiles.SelectedItem.Contains("RA") Or
                .cbo_Profiles.SelectedItem.ToString.EndsWith("EX") Then
                With Me
                    With .lbl_WeekCode
                        Dim _font As System.Drawing.Font = New Font("Tahoma", 28)
                        .Font = _font
                        .TextAlign = ContentAlignment.MiddleLeft
                    End With


                    If .cbo_Profiles.SelectedItem.ToString.EndsWith("EX") Then
                        .lbl_Set1.Visible = False
                        .lbl_Set2.Visible = True
                        .lbl_Set3.Visible = True

                        With .ComboBox1
                            .Items.Clear()
                            .Refresh()
                            .Items.Add("1")
                            .Items.Add("2")
                        End With
                    Else
                        .lbl_Set1.Visible = True
                        .lbl_Set2.Visible = True
                        .lbl_Set3.Visible = True
                    End If

                    .pic_Logic.Visible = False
                    .pic_setting.Visible = True

                    .lbl_Set3.BringToFront()
                    .lbl_Set2.BringToFront()

                    With .ComboBox1
                        .SelectedIndex = 0
                        .Visible = True
                    End With
                End With
            Else
                With Me
                    With .lbl_WeekCode
                        Dim _font As System.Drawing.Font = New Font("Tahoma", 48)
                        .Font = _font
                        .TextAlign = ContentAlignment.MiddleCenter
                    End With

                    .lbl_Set1.Visible = False
                    .lbl_Set2.Visible = False
                    .lbl_Set3.Visible = False
                    .pic_Logic.Visible = True
                    .pic_setting.Visible = False

                    With .ComboBox1
                        .SelectedIndex = 0
                        .Visible = False
                    End With
                End With
            End If

            ParseParamData(.cbo_Profiles.SelectedIndex)

            Me.DispMarkingSetting()
            DispMarkingSetting()

            With FCLSR
                .TempCondSetting = .MarkingCondSetting
                .TempSetting = .MarkingSetting(Me.ComboBox1.SelectedIndex)

                If IsNothing(.Lotdata(1).WeekCode) Then
                    .SelectedProfile = Me.cbo_Profiles.SelectedIndex
                End If
            End With

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

                If Not .DataTrans.StartsWith("#") Then
                    NewProfile.Spec = .DataTrans
                End If

                NewProfile.SettingCond.A_Layout = .NewLayoutNo
                AffectedRec = InsertNewProfile_sql(NewProfile, IIf(.DataTrans.StartsWith("#"), .DataTrans.Replace("#", ""), ""))

                If AffectedRec > 0 Then
                    MessageBox.Show(AffectedRec.ToString & " Records Affected...", "az_Active...", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If


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

    Private Sub btn_Delete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_Delete.Click

        If MessageBox.Show("Are you sure you want to permenantly delete this profile (" & Profiles(Me.cbo_Profiles.SelectedIndex).Spec & ") ?", "az_Active...", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
            Dim SQLrslt As Integer = 0
            Dim SQLcmd As String = "DELETE FROM Setting " & _
                                "WHERE Spec='" & Profiles(Me.cbo_Profiles.SelectedIndex).Spec & "' " & _
                                "AND CtrlNo='" & FCLSR.CtrlNo & "'"


            SQLrslt = SQL_Server_Proc(SQLcmd, "Marking")

            Dim FuncRet As Integer = GetMarkingSetting()

            With Me
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
                End If
            End With
        End If

    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim DateRecord As Date = Now
        FCLSR.Lotdata(1).RecDate = String.Format("{0:D2}-{1:D2}-{2:D4} {3:D2}:{4:D2}:{5:D2}", DateRecord.Month, DateRecord.Day, DateRecord.Year, DateRecord.Hour, DateRecord.Minute, DateRecord.Second)

        Dim FuncRet As Integer = 0
        Dim FormMarking(13) As String


        With FCLSR.Lotdata(1)
            FormMarking(0) = .LotNo
            FormMarking(1) = .IMINo
            FormMarking(2) = .Freq
            FormMarking(3) = .EmpNo
            FormMarking(4) = .RecDate
            FormMarking(5) = .Parameter
            FormMarking(6) = FCLSR.CtrlNo
            FormMarking(7) = "-"
            FormMarking(8) = .WeekCode
            FormMarking(9) = "-"
            FormMarking(10) = "-"
            FormMarking(11) = "-"
            FormMarking(12) = "-"
            FormMarking(13) = "-"
        End With

        FuncRet = azWebService.UpdateRecords(FormMarking)

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged

        With Me
            ParseParamData(.cbo_Profiles.SelectedIndex)

            Me.DispMarkingSetting()
            DispMarkingSetting()

            With FCLSR
                .TempSetting = .MarkingSetting(Me.ComboBox1.SelectedIndex)
            End With

            .GroupBox5.Text = "Laser Setting ~ " & .cbo_Profiles.Text

            For iLp As Integer = 0 To SettingPtr.GetUpperBound(0)
                .SettingPtr(iLp).ForeColor = Color.Black
            Next

            Dim selector As Integer = .ComboBox1.SelectedIndex

            If .cbo_Profiles.SelectedItem.ToString.EndsWith("EX") Then
                selector += 1
            End If

            .SettingPtr(selector).ForeColor = Color.Red
        End With

    End Sub

End Class
