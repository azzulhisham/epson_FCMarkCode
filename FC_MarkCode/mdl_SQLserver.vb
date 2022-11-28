Imports System.Data.SqlClient
Imports Microsoft.Win32


Module mdl_SQLserver

    Private Function GetProfilesFromServer(ByRef SettingCondition() As ParameterProfile) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
                    "SERVER=" & FCLSR.DataBase_.Server & "; " &
                    "DataBase=" & FCLSR.DataBase_.Name & "; " &
                    "uid=" & FCLSR.DataBase_.uid & ";" &
                    "pwd=" & FCLSR.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _
            "SELECT * FROM Setting WHERE CtrlNo='" & FCLSR.CtrlNo & "' " & _
            "ORDER BY Spec"

        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            'cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()

            With sqlReader
                Dim iFieldCnt As Integer = .FieldCount
                Dim iRecNo As Integer = 0

                If .HasRows Then
                    Dim sRetData(iFieldCnt - 1) As String
                    ReDim SettingCondition(iRecNo)


                    Do While .Read()
                        Application.DoEvents()

                        ReDim Preserve SettingCondition(iRecNo)
                        ReDim SettingCondition(iRecNo).ParamData(5)

                        With SettingCondition(iRecNo)
                            .Spec = sqlReader.GetString(1)

                            With .SettingCond
                                .A_Layout = sqlReader.GetString(2)
                                .B_Xoffset = sqlReader.GetString(3)
                                .C_Yoffset = sqlReader.GetString(4)
                                .D_Rotation = sqlReader.GetString(5)
                                .E_Current = sqlReader.GetString(6)
                                .F_QSW = sqlReader.GetString(7)
                                .G_Speed = sqlReader.GetString(8)
                            End With

                            Dim StartLine As String = sqlReader.GetString(9)
                            .StartNo = StartLine

                            For iLp As Integer = 0 To .ParamData.GetUpperBound(0)
                                Application.DoEvents()
                                .ParamData(iLp).LineNo = (Val(StartLine) + iLp).ToString.Trim
                                .ParamData(iLp).SettingString = sqlReader.GetString(10 + iLp)
                            Next

                            .UseDot = sqlReader.GetString(16)
                            .UseBlock = sqlReader.GetString(17)
                        End With

                        iRecNo += 1
                    Loop
                Else
                    RetVal = -1
                End If
            End With
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Public Function CheckDatabase() As Integer

        Dim RetVal As Integer = 0


        Dim sConnStr As String = _
            "SERVER=" & FCLSR.DataBase_.Server & "; " & _
            "DataBase=" & "; " & _
            "uid=" & FCLSR.DataBase_.uid & ";" & _
            "pwd=" & FCLSR.DataBase_.pwd
        '"Integrated Security=SSPI"

        Try
            If My.Computer.Network.Ping(FCLSR.DataBase_.Server.Substring(0, FCLSR.DataBase_.Server.IndexOf("\"))) = False Then
                Return -2
            End If
        Catch ex As Exception
            If My.Computer.Network.IsAvailable Then
                Return -4
            Else
                Return -3
            End If
        End Try


        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _
            "IF NOT EXISTS (SELECT * FROM Sys.DATABASES WHERE Name='" & _
            FCLSR.DataBase_.Name & "') " & _
            "CREATE DATABASE [" & FCLSR.DataBase_.Name & "]"

        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            cmd.ExecuteNonQuery()

        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()

        Return RetVal

    End Function

    Public Function GetMarkingSetting() As Integer

        RestoreDefault()
        Return FromServer()

    End Function

    Private Function FromServer() As Integer

        Dim CreateTblString As String = String.Empty


        With DefaultProfile
            CreateTblString = "[CTRLNo] [nvarchar](12) NOT NULL CONSTRAINT [DF_Setting_CtrlNo]  DEFAULT (N'" & FCLSR.CtrlNo & "')," & _
                "[Spec] [nvarchar](30) NOT NULL CONSTRAINT [DF_Setting_Spec]  DEFAULT (N'" & .Spec & "')," & _
                "[LayoutNo] [nvarchar](2) NOT NULL CONSTRAINT [DF_Setting_LayoutNo]  DEFAULT (N'" & .SettingCond.A_Layout & "')," & _
                "[Xoffset] [nvarchar](8) NOT NULL CONSTRAINT [DF_Setting_Xoffset]  DEFAULT (N'" & .SettingCond.B_Xoffset & "')," & _
                "[Yoffset] [nvarchar](8) NOT NULL CONSTRAINT [DF_Setting_Yoffset]  DEFAULT (N'" & .SettingCond.C_Yoffset & "')," & _
                "[Rotate] [nvarchar](10) NOT NULL CONSTRAINT [DF_Setting_Rotate]  DEFAULT (N'" & .SettingCond.D_Rotation & "')," & _
                "[Current] [nvarchar](8) NOT NULL CONSTRAINT [DF_Setting_Current]  DEFAULT (N'" & .SettingCond.E_Current & "')," & _
                "[QSW] [nvarchar](8) NOT NULL CONSTRAINT [DF_Setting_QSW]  DEFAULT (N'" & .SettingCond.F_QSW & "')," & _
                "[Speed] [nvarchar](8) NOT NULL CONSTRAINT [DF_Setting_Speed]  DEFAULT (N'" & .SettingCond.G_Speed & "')," & _
                "[StartLine] [nvarchar](2) NOT NULL CONSTRAINT [DF_Setting_StartLine]  DEFAULT (N'" & .StartNo & "')," & _
                "[Block1] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block1]  DEFAULT (N'" & .ParamData(0).SettingString & "')," & _
                "[Block2] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block2]  DEFAULT (N'" & .ParamData(1).SettingString & "')," & _
                "[Block3] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block3]  DEFAULT (N'" & .ParamData(2).SettingString & "')," & _
                "[Block4] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block4]  DEFAULT (N'" & .ParamData(3).SettingString & "')," & _
                "[Block5] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block5]  DEFAULT (N'" & .ParamData(4).SettingString & "')," & _
                "[Block6] [nvarchar](100) NOT NULL CONSTRAINT [DF_Setting_Block6]  DEFAULT (N'" & .ParamData(5).SettingString & "')," & _
                "[UseDot] [nvarchar](1) NOT NULL CONSTRAINT [DF_Setting_UseDot]  DEFAULT (N'" & .UseDot & "')," & _
                "[UseBlock] [nvarchar](2) NOT NULL CONSTRAINT [DF_Setting_UseBlock]  DEFAULT (N'" & .UseBlock & "')"
        End With


        Dim RetVal As Integer = Check_dboTables("Setting", CreateTblString)

        If Not RetVal < 0 Then
            RetVal = GetProfilesFromServer(Profiles)

            If RetVal < 0 Then
                RetVal = -9999
            End If
        End If

        Return RetVal

    End Function

    Private Function Check_dboTables(ByVal TableName As String, ByVal CreateTblStr As String) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
                    "SERVER=" & FCLSR.DataBase_.Server & "; " &
                    "DataBase=" & "; " &
                    "uid=" & FCLSR.DataBase_.uid & ";" &
                    "pwd=" & FCLSR.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _
            "USE [" & FCLSR.DataBase_.Name & "]" & vbCrLf & _
            "IF NOT EXISTS (SELECT * FROM sys.objects " & _
            "WHERE object_id=OBJECT_ID(N'[dbo].[" & TableName & "]') AND type in (N'U')) " & _
            "CREATE Table [" & TableName & "] (" & _
            CreateTblStr & ")"

        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            cmd.ExecuteNonQuery()
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Public Function GetProfileDetailsFromServer(ByVal SQLcmd As String, Optional ByRef SettingCondition() As ParameterProfile = Nothing) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
        "SERVER=" & FCLSR.DataBase_.Server & "; " &
        "DataBase=" & FCLSR.DataBase_.Name & "; " &
        "uid=" & FCLSR.DataBase_.uid & ";" &
        "pwd=" & FCLSR.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = SQLcmd


        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            'cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()

            With sqlReader
                Dim iFieldCnt As Integer = .FieldCount
                Dim iRecNo As Integer = 0

                If .HasRows Then
                    Dim sRetData(iFieldCnt - 1) As String
                    ReDim SettingCondition(iRecNo)


                    Do While .Read()
                        Application.DoEvents()

                        ReDim Preserve SettingCondition(iRecNo)
                        ReDim SettingCondition(iRecNo).ParamData(5)

                        With SettingCondition(iRecNo)
                            .Spec = sqlReader.GetString(1)

                            With .SettingCond
                                .A_Layout = sqlReader.GetString(2)
                                .B_Xoffset = sqlReader.GetString(3)
                                .C_Yoffset = sqlReader.GetString(4)
                                .D_Rotation = sqlReader.GetString(5)
                                .E_Current = sqlReader.GetString(6)
                                .F_QSW = sqlReader.GetString(7)
                                .G_Speed = sqlReader.GetString(8)
                            End With

                            Dim StartLine As String = sqlReader.GetString(9)
                            .StartNo = StartLine

                            For iLp As Integer = 0 To .ParamData.GetUpperBound(0)
                                Application.DoEvents()
                                .ParamData(iLp).LineNo = (Val(StartLine) + iLp).ToString.Trim
                                .ParamData(iLp).SettingString = sqlReader.GetString(10 + iLp)
                            Next

                            .UseDot = sqlReader.GetString(16)
                            .UseBlock = sqlReader.GetString(17)
                        End With

                        iRecNo += 1
                    Loop
                Else
                    RetVal = -1
                End If
            End With
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Public Function InsertNewProfile_sql(ByVal NewProfileData As ParameterProfile, Optional ByVal MachineNo As String = "") As Integer

        Dim regSubKeyComm_ As RegistryKey = regKey.CreateSubKey("Software\az_Logic\ActiveProc\DefaultMarkingParameter")

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
                    "SERVER=" & FCLSR.DataBase_.Server & "; " &
                    "DataBase=" & FCLSR.DataBase_.Name & "; " &
                    "uid=" & FCLSR.DataBase_.uid & ";" &
                    "pwd=" & FCLSR.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = String.Empty

        With NewProfileData
            strSQL = "INSERT INTO Setting " & _
                "(CTRLNo, Spec, LayoutNo, Xoffset, Yoffset, Rotate, [Current], QSW, Speed, StartLine, Block1, Block2, Block3, Block4, Block5, Block6, UseDot, UseBlock) VALUES(" & _
                ch & IIf(MachineNo = "", FCLSR.CtrlNo, MachineNo) & ch & ", " & _
                ch & .Spec & ch & ", " & _
                ch & .SettingCond.A_Layout & ch & ", " & _
                ch & .SettingCond.B_Xoffset & ch & ", " & _
                ch & .SettingCond.C_Yoffset & ch & ", " & _
                ch & .SettingCond.D_Rotation & ch & ", " & _
                ch & .SettingCond.E_Current & ch & ", " & _
                ch & .SettingCond.F_QSW & ch & ", " & _
                ch & .SettingCond.G_Speed & ch & ", " & _
                ch & .StartNo & ch & ", " & _
                ch & .ParamData(0).SettingString & ch & ", " & _
                ch & .ParamData(1).SettingString & ch & ", " & _
                ch & .ParamData(2).SettingString & ch & ", " & _
                ch & .ParamData(3).SettingString & ch & ", " & _
                ch & .ParamData(4).SettingString & ch & ", " & _
                ch & .ParamData(5).SettingString & ch & ", " & _
                ch & .UseDot & ch & ", " & _
                ch & .UseBlock & ch & ")"
        End With

        'Debug.Print(strSQL)

        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            'cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()
            RetVal = sqlReader.RecordsAffected
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Public Function SQL_Server_Proc(ByVal SQLcmd As String, ByVal DataBaseName As String) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
        "SERVER=" & FCLSR.DataBase_.Server & "; " &
        "DataBase=" & DataBaseName & "; " &
        "uid=" & FCLSR.DataBase_.uid & ";" &
        "pwd=" & FCLSR.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = SQLcmd

        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)
            'cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()
            RetVal = sqlReader.RecordsAffected
        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
            RetVal = -1
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Public Function GetRecordsFromServer(ByVal Lot_No As String, ByVal SpecNo As String, ByRef RecData As Rec) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
        "SERVER=" & FCLSR.DataBase_.Server & "; " &
        "DataBase=" & FCLSR.DataBase_.Name & "; " &
        "uid=" & FCLSR.DataBase_.uid & ";" &
        "pwd=" & FCLSR.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _
            "SELECT TOP 1 * FROM Records WHERE Lot_No='" & Lot_No & "' AND IMI_No='" & SpecNo & "' " & _
            "ORDER BY RecDate DESC"

        Try
            dbConnection.Open()

            Dim cmd As New SqlCommand(strSQL, dbConnection)
            cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()

            With sqlReader
                Dim iFieldCnt As Integer = .FieldCount
                Dim iRecNo As Integer = 0

                If .HasRows Then
                    Dim sRetData(iFieldCnt - 1) As String

                    Do While .Read()
                        With RecData
                            .Lot_No = sqlReader.GetString(0)
                            .IMI_No = sqlReader.GetString(1)
                            .FreqVal = sqlReader.GetString(2)
                            .Opt = sqlReader.GetString(3)
                            .RecDate = sqlReader.GetDateTime(4).ToString
                            .Profile = sqlReader.GetString(5)
                            .CtrlNo = sqlReader.GetString(6)
                            .MacNo = sqlReader.GetString(7)
                            .MData1 = sqlReader.GetString(8)
                            .MData2 = sqlReader.GetString(9)
                            .MData3 = sqlReader.GetString(10)
                            .MData4 = sqlReader.GetString(11)
                            .MData5 = sqlReader.GetString(12)
                            .MData6 = sqlReader.GetString(13)
                        End With

                        iRecNo += 1
                    Loop

                    RetVal = iRecNo
                Else
                    RetVal = 0
                End If
            End With
        Catch sqlExc As SqlException
            RetVal = 0
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

    Public Function GetRecordsFromServer(ByVal Lot_No As String, ByRef RecData As Rec) As Integer

        Dim RetVal As Integer = 0
        Dim sConnStr As String =
        "SERVER=" & FCLSR.DataBase_.Server & "; " &
        "DataBase=" & FCLSR.DataBase_.Name & "; " &
        "uid=" & FCLSR.DataBase_.uid & ";" &
        "pwd=" & FCLSR.DataBase_.pwd
        '"Integrated Security=SSPI"

        Dim dbConnection As New SqlConnection(sConnStr)
        Dim ch As Char = ChrW(39)
        Dim strSQL As String = _
            "SELECT TOP 1 * FROM Records WHERE Lot_No='" & Lot_No & "' " & _
            "ORDER BY RecDate DESC"

        Try
            dbConnection.Open()

            Dim cmd As New SqlCommand(strSQL, dbConnection)
            cmd.ExecuteNonQuery()

            Dim sqlReader As SqlDataReader = cmd.ExecuteReader()

            With sqlReader
                Dim iFieldCnt As Integer = .FieldCount
                Dim iRecNo As Integer = 0

                If .HasRows Then
                    Dim sRetData(iFieldCnt - 1) As String

                    Do While .Read()
                        With RecData
                            .Lot_No = sqlReader.GetString(0)
                            .IMI_No = sqlReader.GetString(1)
                            .FreqVal = sqlReader.GetString(2)
                            .Opt = sqlReader.GetString(3)
                            .RecDate = sqlReader.GetDateTime(4).ToString
                            .Profile = sqlReader.GetString(5)
                            .CtrlNo = sqlReader.GetString(6)
                            .MacNo = sqlReader.GetString(7)
                            .MData1 = sqlReader.GetString(8)
                            .MData2 = sqlReader.GetString(9)
                            .MData3 = sqlReader.GetString(10)
                            .MData4 = sqlReader.GetString(11)
                            .MData5 = sqlReader.GetString(12)
                            .MData6 = sqlReader.GetString(13)
                        End With

                        iRecNo += 1
                    Loop

                    RetVal = iRecNo
                Else
                    RetVal = 0
                End If
            End With
        Catch sqlExc As SqlException
            RetVal = 0
        End Try

        dbConnection.Close()
        Return RetVal

    End Function

End Module
