Imports System.Deployment.Application
Imports System.Reflection


Namespace My

    ' The following events are available for MyApplication:
    ' 
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup

            If frm_Main.IsHandleCreated = True Then End
            AutoStartShortCut()

        End Sub

        Private Sub AutoStartShortCut()

            'System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed AndAlso _

            Try
                If System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed AndAlso _
                    System.Deployment.Application.ApplicationDeployment.CurrentDeployment.IsFirstRun Then

                    Dim code As Assembly = Assembly.GetExecutingAssembly()
                    Dim company As String = String.Empty
                    Dim description As String = String.Empty

                    If (Attribute.IsDefined(code, GetType(AssemblyCompanyAttribute))) Then
                        Dim ascompany As AssemblyCompanyAttribute = _
                            CType(Attribute.GetCustomAttribute(code, _
                            GetType(AssemblyCompanyAttribute)), AssemblyCompanyAttribute)
                        company = ascompany.Company
                    End If

                    If (Attribute.IsDefined(code, GetType(AssemblyDescriptionAttribute))) Then
                        Dim asdescription As AssemblyDescriptionAttribute = _
                            CType(Attribute.GetCustomAttribute(code, _
                            GetType(AssemblyDescriptionAttribute)), AssemblyDescriptionAttribute)
                        description = asdescription.Description
                    End If

                    Dim startupPath As String = Environment.GetFolderPath(Environment.SpecialFolder.Startup)
                    startupPath = System.IO.Path.Combine(startupPath, description) + ".appref-ms"

                    If Not System.IO.File.Exists(startupPath) Then
                        Dim allProgramsPath As String = Environment.GetFolderPath(Environment.SpecialFolder.Programs)
                        Dim shortcutPath As String = System.IO.Path.Combine(allProgramsPath, company)

                        shortcutPath = System.IO.Path.Combine(shortcutPath, description) + ".appref-ms"
                        System.IO.File.Copy(shortcutPath, startupPath)
                    End If
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message, "az_Logics...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            End Try

        End Sub

        Private Sub MyApplication_UnhandledException(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException

            e.ExitApplication = False
            MessageBox.Show(e.Exception.Message & vbCrLf & "Please call your system engineer to resolve the issue.", "System Crash !", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)

        End Sub

    End Class

End Namespace

