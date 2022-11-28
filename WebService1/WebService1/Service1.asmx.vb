Imports System
Imports System.Globalization
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://localhost/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class Service1
    Inherits System.Web.Services.WebService

    <WebMethod(Description:="Return 'About The Web Service'")> _
    Public Function HelloWorld() As String
        Return "This application connected to a Web Service which composing Week Code for FC product." & vbCrLf & "Crerated By Zulhisham Tan of Epson Toyocom Malaysia @ Jan 2010."
    End Function

    <WebMethod(Description:="Return A Week Code")> _
    Public Function azWeekCode(ByVal sFormat As String) As String

        Dim sFmt As String = sFormat.ToLower
        Dim WebDate As Date = Now
        Dim sRetVal As String = String.Empty

        Dim WebMonth As String = "123456789XYZ"
        Dim WebDay As String = "123456789ABCDEFGHJKLMNPQRSTUVWXYZ"


        Select Case sFmt
            Case Is = "ymd"
                sRetVal = Right(Trim(Str(Year(WebDate))), 1) & Mid(WebMonth, Month(WebDate), 1) & Mid(WebDay, Day(WebDate), 1)
            Case Is = "ydm"
                sRetVal = Right(Trim(Str(Year(WebDate))), 1) & Mid(WebDay, Day(WebDate), 1) & Mid(WebMonth, Month(WebDate), 1)
            Case Is = "dmy"
                sRetVal = Mid(WebDay, Day(WebDate), 1) & Mid(WebMonth, Month(WebDate), 1) & Right(Trim(Str(Year(WebDate))), 1)
            Case Else
                Dim myCI As New CultureInfo("en-US")
                Dim myCal As Calendar = myCI.Calendar
                Dim YrVal As String = String.Format("{0:D4}", WebDate.Year)

                sRetVal = "A" & YrVal.Substring(3) & String.Format("{0:D2}", myCal.GetWeekOfYear(Now, CalendarWeekRule.FirstDay, DayOfWeek.Monday)) & "L"
        End Select

        Return sRetVal

    End Function

    <WebMethod(Description:="Return A Week Code (Extended Version)")> _
    Public Function azWeekCodeEx(ByVal SpecNo As String, ByVal sFormat As String) As String

        Dim sFmt As String = sFormat.ToLower
        Dim WebDate As Date = Now
        Dim sRetVal As String = String.Empty

        Dim WebMonth As String = "123456789XYZ"
        Dim WebDay As String = "123456789ABCDEFGHJKLMNPQRSTUVWXYZ"


        Select Case sFmt
            Case Is = "ymd"
                sRetVal = Right(Trim(Str(Year(WebDate))), 1) & Mid(WebMonth, Month(WebDate), 1) & Mid(WebDay, Day(WebDate), 1)
            Case Is = "ydm"
                sRetVal = Right(Trim(Str(Year(WebDate))), 1) & Mid(WebDay, Day(WebDate), 1) & Mid(WebMonth, Month(WebDate), 1)
            Case Is = "dmy"
                sRetVal = Mid(WebDay, Day(WebDate), 1) & Mid(WebMonth, Month(WebDate), 1) & Right(Trim(Str(Year(WebDate))), 1)
            Case Else
                Dim myCI As New CultureInfo("en-US")
                Dim myCal As Calendar = myCI.Calendar
                Dim YrVal As String = String.Format("{0:D4}", WebDate.Year)

                With My.Computer
                    Dim SpecFile As String = "c:\FC_MarkCode\MI\" & SpecNo & ".dat"

                    If Not .FileSystem.FileExists(SpecFile) Then
                        Return ""
                    End If

                    Dim FileContent As String = .FileSystem.ReadAllText(SpecFile)
                    Dim ContentItems() As String = FileContent.Split(vbCr)

                    Dim Freq() As String = ContentItems.Where(Function(n) n.Contains("L001")).ToArray
                    Dim DateFmt() As String = ContentItems.Where(Function(n) n.Contains("L002")).ToArray
                    Dim Plant() As String = ContentItems.Where(Function(n) n.Contains("L003")).ToArray

                    If Freq.Length <> 1 Or DateFmt.Length <> 1 Or Plant.Length <> 1 Then
                        Return ""
                    End If

                    Dim Freq_() As String = Freq(0).Split(","c)
                    Dim Plant_() As String = Plant(0).Split(","c)

                    sRetVal = Freq_(2).Trim & YrVal.Substring(3) & String.Format("{0:D2}", myCal.GetWeekOfYear(Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday)) & Plant_(2).Trim
                End With
        End Select

        Return sRetVal

    End Function

End Class