Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Web.Services.Protocols


Public Class frm_DataEntry

    Dim azWebService As New ETMYMNET.az_Services
    Dim fg_Load As Integer = 1
    Dim SeqNo As Integer = 0
    Dim MsgLbl() As String = {"Please Enter Lot No. ...", _
                              "Please Enter IMI No. ...", _
                              "Please Enter Emp. No. ...", _
                              "Kindly Wait For A Moment..."}
    Dim BlinkLbl() As String = {"Lot No.", "IMI No.", "Emp. No.", ""}


    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        With Me
            FCLSR.Lotdata(1).LotNo = ""
            .Close()
        End With

    End Sub

    Private Sub frm_DataEntry_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated


    End Sub

    Private Sub frm_DataEntry_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing

        With Me
            .tmr_Blink.Enabled = False
            .SeqNo = 0
        End With

    End Sub

    Private Sub frm_DataEntry_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        With FCLSR
            .Lotdata(1).LotNo = ""
        End With

        With Me
            .fg_Load = 1
            .SeqNo = 0

            With .txt_Scan
                .Text = ""
                .Visible = True
            End With

            DispMsg()

            With .tmr_Blink
                .Interval = 250
                .Enabled = True
            End With
        End With

    End Sub

    Private Sub DispMsg()

        With Me
            .lbl_Msg.Text = MsgLbl(SeqNo)
            .lbl_Label.Text = .BlinkLbl(SeqNo)

            If SeqNo = .MsgLbl.GetUpperBound(0) Then
                .txt_Scan.Visible = False
            Else
                .txt_Scan.Visible = True
            End If
        End With

    End Sub

    Private Sub tmr_Blink_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmr_Blink.Tick

        With Me
            .lbl_Label.Visible = Not .lbl_Label.Visible
        End With

    End Sub

    Private Sub txt_Scan_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt_Scan.KeyDown

        If e.KeyCode = Keys.Escape Then
            With Me
                .Close()
            End With
        ElseIf e.KeyCode = Keys.Enter Then
            With FCLSR
                Select Case SeqNo
                    Case Is = 0
                        .Lotdata(1).LotNo = Me.txt_Scan.Text.ToUpper.Trim
                        Me.txt_Scan.Text = ""
                        SeqNo += 1
                        DispMsg()
                    Case Is = 1
                        .Lotdata(1).IMINo = Me.txt_Scan.Text.ToUpper.Trim
                        Me.txt_Scan.Text = ""
                        SeqNo += 1
                        DispMsg()
                    Case Is = 2
                        .Lotdata(1).EmpNo = Me.txt_Scan.Text.ToUpper.Trim
                        Me.txt_Scan.Text = ""
                        SeqNo += 1
                        DispMsg()

                        Dim MarkData() As String = {}

                        Try
                            '.Lotdata(1).WeekCode = azWebService.azWeekCode("")
                            '.Lotdata(1).WeekCode = azWebService.azWeekCodeEx_FC(.Lotdata(1).IMINo, "")

                            Dim WebCall As Integer = azWebService.az_FCweekcode_ad(.Lotdata(1).LotNo, .Lotdata(1).IMINo, MarkData)

                            If Not WebCall < 0 And MarkData.Length = 7 Then
                                With .Lotdata(1)
                                    .LotNo = MarkData(0)
                                    .IMINo = MarkData(1)
                                    .WeekCode = MarkData(2)
                                    .Parameter = MarkData(3)
                                    .Freq = MarkData(4)
                                    .WkCdFmt = MarkData(5)
                                    .Plant = MarkData(6)
                                End With
                            Else
                                .Lotdata(1).WeekCode = ""
                            End If
                        Catch ex As Exception
                            .Lotdata(1).WeekCode = ""
                        End Try

                        Me.Close()
                End Select
            End With
        End If

    End Sub

    Private Sub frm_DataEntry_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown

        If fg_Load = 0 Then Exit Sub
        fg_Load = 0

        Me.txt_Scan.Focus()

    End Sub

End Class