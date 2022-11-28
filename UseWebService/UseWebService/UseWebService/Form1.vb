Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Web.Services.Protocols


Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim azWebService As New localhost.Service1

        Me.Label1.Text = azWebService.azWeekCode("")
        'Me.Label1.Text = azWebService.HelloWorld


    End Sub

End Class
