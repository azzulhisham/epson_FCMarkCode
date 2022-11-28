Imports System
Imports System.Globalization
Imports System.Math
Imports System.Threading
Imports System.IO
Imports System.IO.Ports
Imports System.Management
Imports System.Runtime.InteropServices
Imports Microsoft.Win32


Module mdl_FC_MarkCode

    Public Const Func_Ret_Success = 0
    Public Const Func_Ret_Fail = -1

    Public Const ch_STX = &H2
    Public Const ch_ETX = &H3
    Public Const ch_ACK = &H6
    Public Const ch_NAK = &H15

    Public Const WarningMsg1 = "Click The 'Post Data' Button To Re-Fresh Laser Marking Data !!!"
    Public Const ImportantMsg1 = "Click 'Data Entry' Button To Set Marking Data... "

    Public EditTitle() As String = {"Edit Current Setting (A)", _
                                    "Edit QSW Setting (kHz)", _
                                    "Edit Speed Setting (mm/s)", _
                                    "Edit X-Axis Offset Setting (mm)", _
                                    "Edit Y-Axis Offset Setting (mm)", _
                                    "Edit Rotation Setting (deg)", _
                                    "Edit X-Axis Positioning (mm)", _
                                    "Edit Y-Axis Positioning (mm)", _
                                    "Edit Text Angle (deg)", _
                                    "Edit Width Alignment (mm)", _
                                    "Edit Space Width (mm)", _
                                    "Edit X-Axis Org. (mm)", _
                                    "Edit Y-Axis Org. (mm)", _
                                    "Edit Character Height (mm)", _
                                    "Edit Compress Rate (%)"}

    Public EditStrFmt() As String = {"{0:F1}", "{0:F1}", "{0:F2}", "{0:F3}", "{0:F3}", "{0:F6}", _
                                     "{0:F3}", "{0:F3}", "{0:F1}", "{0:D2}", "{0:F3}", "{0:F3}", "{0:F3}", _
                                     "{0:F3}", "{0:D1}"}
    Public EditDefault() As String = {"183", "200", "20000", "3430", "550", "359900000", _
                                      "0", "0", "2700", "", "", "", "", _
                                      "300", "100"}
    Public EditModifier() As Integer = {10, 10, 100, 1000, 1000, 1000000, _
                                        1000, 1000, 10, 1, 1000, 1000, 1000, _
                                        1000, 1}
    Public EditRng() As String = {"(5.0 ~ 30.0)", _
                                  "(0.0 ~ 199.9)", _
                                  "(10.0 ~ 300.00)", _
                                  "(-70 ~ 70)", _
                                  "(-70 ~ 70)", _
                                  "(0.0 ~ 360.0)", _
                                  "(-70 ~ 70)", _
                                  "(-70 ~ 70)", _
                                  "(0.0 ~ 360.0)", _
                                  "-", _
                                  "(-5.00 ~ 5.00)", _
                                  "(-70 ~ 70)", _
                                  "(-70 ~ 70)", _
                                  "(0.001 ~ 5.000)", _
                                  "(1 ~ 199)"}

    Public EditOption() As String = {"Select Draw Type", _
                                     "Select Text Align Type", _
                                     "Select Space Align Type"}

    Public DrawOption() As String = {"Arc (IL)", "Arc (OL)", "Line"}
    Public DrawOptionDefault As String = "2"

    Public TextAlignOption() As String = {"No Set", "Left", "Center", "Right", "Zoom"}
    Public TextAlignOptionDefault As String = "0"

    Public SpaceAlignOption() As String = {"No Set", "Pitch", "Space"}
    Public SpaceAlignOptionDefault As String = "0"


    Public Structure CommPortData
        Public PortName As String
        Public DataBits As Integer
        Public BaudRate As Integer
        Public StopBits As System.IO.Ports.StopBits
        Public Parity As System.IO.Ports.Parity
    End Structure

    Public Structure SystemData
        Public LotNo As String
        Public IMINo As String
        Public EmpNo As String
        Public WeekCode As String
        Public Freq As String
        Public Plant As String
        Public Parameter As String
        Public WkCdFmt As String
        Public RecDate As String
    End Structure

    Public Structure DB_Data
        Public Server As String
        Public Name As String
        Public uid As String
        Public pwd As String
    End Structure

    Public Structure SystemStruc
        Public MarkingSetting() As MarkingParameter
        Public TempSetting As MarkingParameter

        Public MarkingCondSetting As MarkingCondition
        Public TempCondSetting As MarkingCondition

        Public Lotdata() As SystemData
        Public ML7111A As CommPortData
        Public EditParameter As UpdateData
        Public DataBase_ As DB_Data

        Public AuthenticationCode As String
        Public AuthenticalAccess As String
        Public CtrlNo As String
        Public Restriction As String
        Public DataTrans As String
        Public NewLayoutNo As String
        Public CurSelection As String

        Public SelectedProfile As Integer
        Public SysBsyCode As Integer
    End Structure

    Public Structure UpdateData
        Public OldData As String
        Public NewData As String
        Public IdxNo As Integer
    End Structure

    Public Structure MarkingParameter
        Public A_DrawType As String
        Public B_X_Axis As String
        Public C_Y_Axis As String
        Public D_TextAngle As String
        Public E_TextAlign As String
        Public F_WidthAlign As String
        Public G_SpaceAlign As String
        Public H_SpaceWidth As String
        Public I_X_AxisOrg As String
        Public J_Y_AxisOrg As String
        Public K_CharHeight As String
        Public L_Compress As String
        Public M_OppDir As String
        Public N_CharAngle As String
        Public O_Current As String
        Public P_QSW As String
        Public Q_Speed As String
        Public R_Repeat As String
        Public S_Mirror As String
        Public T_VarType As String
        Public U_VarNo As String
        Public LineNo As String
        Public SettingString As String
    End Structure

    Public Structure MarkingCondition
        Public A_Layout As String
        Public B_Xoffset As String
        Public C_Yoffset As String
        Public D_Rotation As String
        Public E_Current As String
        Public F_QSW As String
        Public G_Speed As String
    End Structure

    Public Structure ParameterProfile
        Public Spec As String
        Public StartNo As String
        Public UseDot As String
        Public UseBlock As String
        Public SettingCond As MarkingCondition
        Public ParamData() As MarkingParameter
    End Structure

    Public Structure Rec
        Public Lot_No As String
        Public IMI_No As String
        Public FreqVal As String
        Public Opt As String
        Public RecDate As String
        Public Profile As String
        Public CtrlNo As String
        Public MacNo As String
        Public MData1 As String
        Public MData2 As String
        Public MData3 As String
        Public MData4 As String
        Public MData5 As String
        Public MData6 As String
    End Structure

    Public WithEvents Miyachi As SerialPort = New SerialPort

    Public regKey As RegistryKey = Registry.CurrentUser
    Public regSubKey As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR")

    Public FCLSR As SystemStruc
    Public DefaultProfile As ParameterProfile
    Public Profiles() As ParameterProfile = Nothing


    Public Function ReadRegData() As Integer

        Try
            Dim regSubKeyComm_1 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\Miyachi_ML7111A")
            Dim regSubKeyComm_2 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\MarkingParameter1")
            Dim regSubKeyComm_3 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\MarkingParameter2")
            Dim regSubKeyComm_4 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\MarkingParameter3")
            Dim regSubKeyComm_5 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\MarkingParameter4")
            Dim regSubKeyComm_6 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\MarkingParameter5")


            With FCLSR
                .AuthenticationCode = regSubKey.GetValue("AuthenticationAccess", "azActive")
                .CtrlNo = regSubKey.GetValue("CtrlNo", "Mxxxxx")
                .Restriction = regSubKey.GetValue("Restriction", "1")

                With .DataBase_
                    '.Server = regSubKey.GetValue("Database_server", "172.16.59.254\SQLEXPRESS")
                    '.Name = regSubKey.GetValue("Database_name", "Marking")
                    '.uid = regSubKey.GetValue("Database_uid", "VB-SQL")
                    '.pwd = regSubKey.GetValue("Database_pwd", "Anyn0m0us")

                    .Server = regSubKey.GetValue("Database_server", "DESKTOP-TLVFD7V\SQLEXPRESS")
                    .Name = regSubKey.GetValue("Database_name", "Marking")
                    .uid = regSubKey.GetValue("Database_uid", "sa")
                    .pwd = regSubKey.GetValue("Database_pwd", "Az@HoePinc0615")
                End With

                With .ML7111A
                    .PortName = regSubKeyComm_1.GetValue("CommPortName", "COM1")

                    .BaudRate = CType(regSubKeyComm_1.GetValue("CommBaudRate"), Integer)
                    .BaudRate = IIf((.DataBits = 0), 9600, .BaudRate)

                    .DataBits = CType(regSubKeyComm_1.GetValue("CommDataBits"), Integer)
                    .DataBits = IIf((.DataBits = 0), 8, .DataBits)

                    .StopBits = CType(regSubKeyComm_1.GetValue("CommStopBits"), System.IO.Ports.StopBits)
                    .StopBits = IIf((.StopBits = 0), Ports.StopBits.One, .StopBits)

                    Dim sPrty As String = regSubKeyComm_1.GetValue("CommParity", "")
                    .Parity = IIf((sPrty = ""), Ports.Parity.Even, Val(sPrty))
                End With


                '2,0,0,2700,,,,,,,,300,100,,0,,,,,,,0,A018L
                '2,0,0,2700,,,,,,,,450,100,,0,,,,,,,0,A141L
                Dim MarkingParameter As String = regSubKeyComm_2.GetValue("Setting", "2,0,0,2700,,,,,,,,300,100,,0,,,,,,,0,A018L")
                .MarkingSetting(0).LineNo = regSubKeyComm_2.GetValue("LineNo", "2")
                ReadParameter(.MarkingSetting(0), MarkingParameter)

                With .MarkingCondSetting
                    .A_Layout = regSubKeyComm_2.GetValue("LayoutNo", "1")
                    .B_Xoffset = regSubKeyComm_2.GetValue("X_Offset", "3430")
                    .C_Yoffset = regSubKeyComm_2.GetValue("Y_Offset", "550")
                    .D_Rotation = regSubKeyComm_2.GetValue("Rotation", "359900000")
                    .E_Current = regSubKeyComm_2.GetValue("Current", "183")
                    .F_QSW = regSubKeyComm_2.GetValue("QSW", "200")
                    .G_Speed = regSubKeyComm_2.GetValue("Speed", "20000")
                End With
            End With
        Catch ex As Exception
            Return Func_Ret_Fail
        End Try

        Return Func_Ret_Success

    End Function

    Public Sub RestoreDefault()

        Dim regSubKeyComm_ As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\DefaultMarkingParameter")

        With DefaultProfile
            .Spec = regSubKeyComm_.GetValue("Spec", "FC-12M")
            .UseBlock = regSubKeyComm_.GetValue("UseBlock", "0")
            .UseDot = regSubKeyComm_.GetValue("UseDot", "0")
            .StartNo = regSubKeyComm_.GetValue("StartLine", "2")

            With .SettingCond
                .A_Layout = regSubKeyComm_.GetValue("LayoutNo", "1")
                .B_Xoffset = regSubKeyComm_.GetValue("X_Offset", "3430")
                .C_Yoffset = regSubKeyComm_.GetValue("Y_Offset", "550")
                .D_Rotation = regSubKeyComm_.GetValue("Rotation", "359900000")
                .E_Current = regSubKeyComm_.GetValue("Current", "183")
                .F_QSW = regSubKeyComm_.GetValue("QSW", "200")
                .G_Speed = regSubKeyComm_.GetValue("Speed", "20000")
            End With

            ReDim .ParamData(5)
            .ParamData(0).SettingString = regSubKeyComm_.GetValue("Block1", "2,0,0,2700,,,,,,,,300,100,,0,,,,,,,0,A018L")
            .ParamData(1).SettingString = regSubKeyComm_.GetValue("Block2", "2,0,0,2700,,,,,,,,300,100,,0,,,,,,,0,A123L")
            .ParamData(2).SettingString = regSubKeyComm_.GetValue("Block3", "2,0,0,2700,,,,,,,,300,100,,0,,,,,,,0,A123L")
            .ParamData(3).SettingString = regSubKeyComm_.GetValue("Block4", "2,0,0,2700,,,,,,,,300,100,,0,,,,,,,0,A123L")
            .ParamData(4).SettingString = regSubKeyComm_.GetValue("Block5", "2,0,0,2700,,,,,,,,300,100,,0,,,,,,,0,A123L")
            .ParamData(5).SettingString = regSubKeyComm_.GetValue("Block6", "2,0,0,2700,,,,,,,,300,100,,0,,,,,,,0,A123L")

            For ilp As Integer = 0 To .ParamData.GetUpperBound(0)
                Application.DoEvents()

                .ParamData(ilp).LineNo = .StartNo
                ReadParameter(.ParamData(ilp), .ParamData(ilp).SettingString)
            Next
        End With

    End Sub

    Public Sub ParseParamData(ByVal IdxNo As Integer)

        With FCLSR
            Dim MarkingParameter() As String = {"", "", "", "", "", ""}

            For ilp As Integer = 0 To Profiles(IdxNo).ParamData.GetUpperBound(0)
                Application.DoEvents()

                .MarkingSetting(ilp).LineNo = Profiles(IdxNo).ParamData(ilp).LineNo
                MarkingParameter(ilp) = Profiles(IdxNo).ParamData(ilp).SettingString
                ReadParameter(.MarkingSetting(ilp), MarkingParameter(ilp))
            Next

            With .MarkingCondSetting
                .A_Layout = Profiles(IdxNo).SettingCond.A_Layout
                .B_Xoffset = Profiles(IdxNo).SettingCond.B_Xoffset
                .C_Yoffset = Profiles(IdxNo).SettingCond.C_Yoffset
                .D_Rotation = Profiles(IdxNo).SettingCond.D_Rotation
                .E_Current = Profiles(IdxNo).SettingCond.E_Current
                .F_QSW = Profiles(IdxNo).SettingCond.F_QSW
                .G_Speed = Profiles(IdxNo).SettingCond.G_Speed
            End With
        End With

    End Sub

    Public Function SaveRegMarkingConditionSetting(ByVal ConditionSetting As MarkingCondition, Optional ByVal NewMarkingSetting As String = "") As Integer

        Try
            Dim regSubKeyComm_1 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\Miyachi_ML7111A")
            Dim regSubKeyComm_2 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\MarkingParameter1")
            Dim regSubKeyComm_3 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\MarkingParameter2")
            Dim regSubKeyComm_4 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\MarkingParameter3")
            Dim regSubKeyComm_5 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\MarkingParameter4")
            Dim regSubKeyComm_6 As RegistryKey = regKey.CreateSubKey("Software\az_Logic\FC_LSR\MarkingParameter5")


            If Not NewMarkingSetting = "" Then regSubKeyComm_2.SetValue("Setting", NewMarkingSetting)

            With ConditionSetting
                regSubKeyComm_2.SetValue("X_Offset", .B_Xoffset)
                regSubKeyComm_2.SetValue("Y_Offset", .C_Yoffset)
                regSubKeyComm_2.SetValue("Rotation", .D_Rotation)
                regSubKeyComm_2.SetValue("Current", .E_Current)
                regSubKeyComm_2.SetValue("QSW", .F_QSW)
                regSubKeyComm_2.SetValue("Speed", .G_Speed)
            End With
        Catch ex As Exception
            Return Func_Ret_Fail
        End Try

    End Function

    Public Sub ReadParameter(ByRef MarkingParam As MarkingParameter, ByVal Setting As String)

        Dim MarkingParameter As String = Setting


        With MarkingParam
            .A_DrawType = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .A_DrawType = "" Then
                .A_DrawType = "2"
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .B_X_Axis = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .B_X_Axis = "" Then
                .B_X_Axis = "0"
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .C_Y_Axis = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .C_Y_Axis = "" Then
                .C_Y_Axis = "0"
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .D_TextAngle = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .D_TextAngle = "" Then
                .D_TextAngle = "0"
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .E_TextAlign = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .E_TextAlign = "" Then
                .E_TextAlign = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .F_WidthAlign = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .F_WidthAlign = "" Then
                .F_WidthAlign = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .G_SpaceAlign = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .G_SpaceAlign = "" Then
                .G_SpaceAlign = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .H_SpaceWidth = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .H_SpaceWidth = "" Then
                .H_SpaceWidth = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            Dim sDmy As String = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If sDmy = "" Then
                sDmy = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .I_X_AxisOrg = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .I_X_AxisOrg = "" Then
                .I_X_AxisOrg = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .J_Y_AxisOrg = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .J_Y_AxisOrg = "" Then
                .J_Y_AxisOrg = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .K_CharHeight = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .K_CharHeight = "" Then
                .K_CharHeight = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .L_Compress = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .L_Compress = "" Then
                .L_Compress = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .M_OppDir = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .M_OppDir = "" Then
                .M_OppDir = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .N_CharAngle = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .N_CharAngle = "" Then
                .N_CharAngle = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .O_Current = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .O_Current = "" Then
                .O_Current = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .P_QSW = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .P_QSW = "" Then
                .P_QSW = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .Q_Speed = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .Q_Speed = "" Then
                .Q_Speed = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .R_Repeat = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .R_Repeat = "" Then
                .R_Repeat = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .S_Mirror = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .S_Mirror = "" Then
                .S_Mirror = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            sDmy = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If sDmy = "" Then
                sDmy = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .T_VarType = MarkingParameter.Substring(0, MarkingParameter.IndexOf(","))

            If .T_VarType = "" Then
                .T_VarType = ""
                MarkingParameter = MarkingParameter.Substring(1)
            Else
                MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
            End If

            .U_VarNo = MarkingParameter.Trim

            If .U_VarNo = "" Then
                .U_VarNo = ""

                If MarkingParameter.IndexOf(",") < 0 Then
                    MarkingParameter = ""
                Else
                    MarkingParameter = MarkingParameter.Substring(1)
                End If
            Else
                If MarkingParameter.IndexOf(",") < 0 Then
                    MarkingParameter = ""
                Else
                    MarkingParameter = MarkingParameter.Substring(MarkingParameter.IndexOf(",") + 1)
                End If
            End If
        End With

    End Sub

    Public Function InitSerialPort() As Integer

        Dim lng_RetVal As Long = Func_Ret_Fail


        'Initialize Comm. Port For Pick & Place Unit (FZ3)
        Try
            With Miyachi
                If .IsOpen = True Then
                    .Close()
                End If

                .PortName = FCLSR.ML7111A.PortName
                .Parity = FCLSR.ML7111A.Parity
                .BaudRate = FCLSR.ML7111A.BaudRate
                .StopBits = FCLSR.ML7111A.StopBits
                .DataBits = FCLSR.ML7111A.DataBits
                .ReceivedBytesThreshold = 1
                .ReadBufferSize = 1024

                '.Open()
            End With
        Catch
            Return lng_RetVal
        End Try

        Return Func_Ret_Success

    End Function

    Public Function CalCheckSum(ByVal strCmd As String, Optional ByVal StringLength As Integer = 2) As String

        Dim CheckSum As Integer = 0


        For iLp As Integer = 0 To strCmd.Length - 1
            Application.DoEvents()
            CheckSum += Asc(strCmd.Substring(iLp, 1))
        Next

        Dim RetString As String = String.Format("{0:X2}", CheckSum)

        If RetString.Length > StringLength Then
            RetString = RetString.Substring(RetString.Length - StringLength)
        End If

        Return RetString

    End Function

    Public Function ML7111A_cmd(ByRef strCmd As String, Optional ByRef strRepMsg As String = "") As Integer

        Dim int_WaitDlyTimer As Long = 0
        Dim SuccessError As Integer = 0
        Dim int_Dmy As Integer = 0


        With Miyachi
            If .IsOpen = False Then
                Try
                    .Open()
                Catch ex As Exception
                    Return Func_Ret_Fail
                End Try
            End If

            strCmd = Chr(ch_STX) & strCmd & Chr(ch_ETX)
            strCmd = strCmd & CalCheckSum(strCmd)
            .Write(strCmd & vbCrLf)

            Dim RecvBytesFlg As Integer = Func_Ret_Success
            Dim WaitReplyTimer As Integer = My.Computer.Clock.TickCount


            Do While .BytesToRead = 0
                Application.DoEvents()
                If My.Computer.Clock.TickCount > WaitReplyTimer + 3000 Then RecvBytesFlg = Func_Ret_Fail : Exit Do
            Loop

            If RecvBytesFlg < 0 Then Return Func_Ret_Fail

            Dim ReadByteSize As Integer = .BytesToRead
            Dim str_Buffer As String = String.Empty
            Dim Buffer() As Char

            WaitReplyTimer = My.Computer.Clock.TickCount

            Do Until ReadByteSize = 0 And (Not str_Buffer.IndexOf(Chr(ch_ACK)) < 0 Or Not str_Buffer.IndexOf(Chr(ch_NAK)) < 0 Or Not str_Buffer.IndexOf(Chr(ch_ETX)) < 0)
                Application.DoEvents()
                If My.Computer.Clock.TickCount > WaitReplyTimer + 3000 Then Return Func_Ret_Fail

                ReDim Buffer(ReadByteSize)
                .Read(Buffer, 0, ReadByteSize)

                For int_Dmy = 0 To Buffer.GetUpperBound(0)
                    Application.DoEvents()

                    If Not Buffer(int_Dmy) = Nothing Then
                        If Buffer(int_Dmy) = vbCr Then
                            str_Buffer &= vbCr
                        ElseIf Buffer(int_Dmy) = vbLf Then
                            str_Buffer &= vbLf
                        Else
                            str_Buffer &= Buffer(int_Dmy)
                        End If
                    End If
                Next

                ReadByteSize = .BytesToRead
            Loop

            If Not str_Buffer.IndexOf(Chr(ch_NAK)) < 0 Then
                Return Func_Ret_Fail
            End If

            str_Buffer = str_Buffer.Trim
            strRepMsg = str_Buffer
            SuccessError = str_Buffer.Length

            Return SuccessError
        End With

    End Function

    Public Function GetWeekNoOfYear() As String

        Dim myCI As New CultureInfo("en-US")
        Dim myCal As Calendar = myCI.Calendar

        Return myCal.GetWeekOfYear(Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString

    End Function

    Public Function FormPostStream(ByVal MarkingSetting As MarkingParameter, Optional ByVal SaveMode As Integer = 0, Optional ByVal ProfileName As String = "") As String

        Dim SetMarkingParameter As String = String.Empty


        With MarkingSetting
            If SaveMode = 0 Then SetMarkingParameter &= .LineNo & ","
            SetMarkingParameter &= .A_DrawType & ","
            SetMarkingParameter &= .B_X_Axis & ","
            SetMarkingParameter &= .C_Y_Axis & ","
            SetMarkingParameter &= .D_TextAngle & ","
            SetMarkingParameter &= .E_TextAlign & ","
            SetMarkingParameter &= .F_WidthAlign & ","
            SetMarkingParameter &= .G_SpaceAlign & ","
            SetMarkingParameter &= .H_SpaceWidth & ","
            SetMarkingParameter &= "" & ","
            SetMarkingParameter &= .I_X_AxisOrg & ","
            SetMarkingParameter &= .J_Y_AxisOrg & ","
            SetMarkingParameter &= .K_CharHeight & ","
            SetMarkingParameter &= .L_Compress & ","
            SetMarkingParameter &= .M_OppDir & ","
            SetMarkingParameter &= .N_CharAngle & ","
            SetMarkingParameter &= .O_Current & ","
            SetMarkingParameter &= .P_QSW & ","
            SetMarkingParameter &= .Q_Speed & ","
            SetMarkingParameter &= .R_Repeat & ","
            SetMarkingParameter &= .S_Mirror & ","
            SetMarkingParameter &= "" & ","
            SetMarkingParameter &= .T_VarType & ","


            If ProfileName.ToUpper.Contains("SG") Or
                ProfileName.ToUpper.Contains("RA") Or
                ProfileName.ToUpper.EndsWith("EX") Then
                If IsNothing(FCLSR.Lotdata(1).WeekCode) OrElse Not FCLSR.Lotdata(1).WeekCode.Length > 5 Then
                    FCLSR.Lotdata(1).WeekCode = "3030" & vbCrLf & "o " & "A333S"
                End If

                If FCLSR.Lotdata(1).WeekCode.Length > 5 And Convert.ToInt32(.LineNo) = 2 And Not ProfileName.ToUpper.EndsWith("EX") Then
                    SetMarkingParameter &= .U_VarNo
                Else
                    If FCLSR.Lotdata(1).WeekCode.Length > 5 Then
                        If ProfileName.ToUpper.EndsWith("EX") Then
                            Dim _markingData As String = FCLSR.Lotdata(1).WeekCode.Replace(vbCrLf, ",")
                            _markingData = _markingData.Replace("-", ",")
                            Dim _mrkdata() As String = _markingData.Split(",")

                            SetMarkingParameter &= _mrkdata(Convert.ToInt32(.LineNo) - 2).Trim
                        Else
                            Dim _mrkdata() As String = FCLSR.Lotdata(1).WeekCode.Split("o")

                            Select Case CType(.LineNo, Integer)
                                Case Is = 3
                                    If ProfileName.ToUpper.Contains("RA") Then
                                        SetMarkingParameter &= _mrkdata(0).Trim
                                    Else
                                        SetMarkingParameter &= _mrkdata(0).Substring(0, 4).Trim
                                    End If
                                Case Is = 4
                                    SetMarkingParameter &= _mrkdata(1).Trim
                            End Select
                        End If
                    Else
                        SetMarkingParameter &= FCLSR.Lotdata(1).WeekCode
                    End If
                End If
            Else
                If Not IsNothing(FCLSR.Lotdata(1).WeekCode) AndAlso Not FCLSR.Lotdata(1).WeekCode.Length = 0 Then
                    SetMarkingParameter &= FCLSR.Lotdata(1).WeekCode
                Else
                    SetMarkingParameter &= .U_VarNo
                End If
            End If
        End With

        Return SetMarkingParameter

    End Function

End Module
