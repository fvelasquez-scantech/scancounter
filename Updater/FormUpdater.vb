Imports Newtonsoft.Json.Linq
Imports RestSharp
Imports System.ComponentModel
Imports System.Net
Imports Microsoft.Win32
Imports System.Xml

Public Class FormAutoUpdater
    Private ReadOnly Wrapper As SecurityWrapper

    Private bgwOpcion As String = ""
    Private bgwHelperResultado As Integer

    Private LinkDescargaNuevaActualizacion As String

    Private version As String = ""
    Private nombre As String = ""
    Private nombreInstalador As String = ""

    Delegate Sub myMethodDelegate()
    Dim delegadoDescarga As New myMethodDelegate(AddressOf InfoDescarga)
    Dim delegadoDesinstalar As New myMethodDelegate(AddressOf InfoDesinstalar)
    Dim delegadoFinalizar As New myMethodDelegate(AddressOf InfoFinalizar)

    Private bytesRecibidos As Long = 0
    Private bytesTotales As Long = 0
    Private bytesPorcentaje As Integer = 0

    Private IdProducto As String = ""

    Private SourcePath As String = "C:\Scantech\data_scancounter_front.xml"
    Private SaveDirectory As String = "C:\Scantech\"
    Private Filename As String = System.IO.Path.GetFileName(SourcePath) 'get the filename of the original file without the directory on it
    Private SavePath As String = System.IO.Path.Combine(SaveDirectory, Filename) 'combines the saveDirectory and the filename to get a fully qualified path.

#Region "Constructor"
    Sub New()

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        Wrapper = New SecurityWrapper
    End Sub
#End Region

#Region "Load"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        IniciaBgwHelper("Load")
    End Sub
    Sub RutinaLoad()
        If Not IO.File.Exists(SourcePath) Then
            bgwHelperResultado = 2
        Else
            Dim XmlDoc As XmlDocument = New XmlDocument()

            XmlDoc.Load(SavePath)

            If XmlDoc.DocumentElement("encrypted_key").InnerText <> "" Then
                IdProducto = Wrapper.DecryptData(XmlDoc.DocumentElement("encrypted_key").InnerText)
                Console.WriteLine($"IdProducto {IdProducto}")

                Dim client As New RestClient($"https://scantech.cl/api/productos/read_update_ruta_producto.php?id_producto={IdProducto}")

                Dim request = New RestRequest(Method.GET)
                Dim response As IRestResponse = client.Execute(request)
                Dim content As String = response.Content
                Console.WriteLine(content)
                If response.StatusCode = HttpStatusCode.OK Then
                    Dim json As JObject = JObject.Parse(content)

                    version = json.SelectToken("Producto.version")
                    LinkDescargaNuevaActualizacion = json.SelectToken("Producto.link")
                    nombre = json.SelectToken("Producto.nombre_programa")
                    nombreInstalador = json.SelectToken("Producto.link").ToString.Substring(json.SelectToken("Producto.link").ToString.LastIndexOf("/") + 1)
                    nombreInstalador = nombreInstalador.Substring(0, 16)
                    nombreInstalador += ".msi"
                    Console.WriteLine($"{nombreInstalador}")

                    bgwHelperResultado = 1
                End If
            Else
                bgwHelperResultado = 2
            End If
        End If

    End Sub
    Sub RutinaLoad_Completed()
        Select Case bgwHelperResultado
            Case 1
                WindowState = FormWindowState.Normal
                ShowIcon = True
                ShowInTaskbar = True

                Dim uri As New Uri(LinkDescargaNuevaActualizacion)

                Lblnfo.Location = New Point(63, 60)
                Lblnfo.Text = "Descargando instalador. Por favor, espere.."

                'nombre = IO.Path.GetFileName(uri.LocalPath)
                Dim wClient As New WebClient()
                AddHandler wClient.DownloadFileCompleted, AddressOf OnDownloadComplete
                AddHandler wClient.DownloadProgressChanged, AddressOf DownloadProgressCallback4

                wClient.DownloadFileAsync(New Uri($"{LinkDescargaNuevaActualizacion}"), $"C:\Scantech\{nombreInstalador}")
                wClient.Dispose()
                Console.WriteLine($"LinkDescargaNuevaActualizacion {LinkDescargaNuevaActualizacion}")
                Console.WriteLine($"nombreInstalador {nombreInstalador}")
                'NotifyIconUpdate.Text = $"Actualizando a versión {version}"
                'NotifyIconUpdate.ShowBalloonTip(3000, "Actualización en curso", $"Se está actualizando el sistema a la versión {version}.", ToolTipIcon.Info)
                'NotifyIconUpdate.Visible = True
            Case 2
                MsgBox($"Error al intentar obtener información del producto {IdProducto}")
        End Select
    End Sub
#End Region

#Region "BackgroundWorker Helper"
    Sub IniciaBgwHelper(Opcion As String)
        bgwOpcion = Opcion
        If Not bgwHelper.IsBusy Then
            bgwHelper.RunWorkerAsync()
        End If
    End Sub
    Private Sub bgwHelper_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgwHelper.DoWork
        Select Case bgwOpcion
            Case "Load"
                RutinaLoad()
        End Select
    End Sub

    Private Sub bgwHelper_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwHelper.RunWorkerCompleted
        Select Case bgwOpcion
            Case "Load"
                RutinaLoad_Completed()
        End Select
    End Sub
#End Region

#Region "Delegados"
    Sub InfoDescarga()
        LblProgresoDescarga.Text = $"{bytesRecibidos} / {bytesTotales}. {bytesPorcentaje} % descargado..."
    End Sub
    Sub InfoDesinstalar()
        Lblnfo.Location = New Point(125, 60)
        Lblnfo.Text = $"Actualizando ScanCounter (Front-End)..."
    End Sub
    Sub InfoFinalizar()
        Lblnfo.Location = New Point(142, 93)
        Lblnfo.Text = $"Actualización completada."
        PictureBox1.Hide()

        'Button1.Show()
        Timer1.Start()
    End Sub
#End Region

#Region "Descarga instalador"

    'Private Sub NotifyIconUpdate_BalloonTipClicked(sender As Object, e As EventArgs) Handles NotifyIconUpdate.BalloonTipClicked
    '    MsgBox("abrir scantech.cl/scanprocess/v1.0.2.10")
    'End Sub

    Private Sub OnDownloadComplete(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs)
        If Not e.Cancelled AndAlso e.Error Is Nothing Then
            LblProgresoDescarga.Hide()

            Invoke(delegadoDesinstalar)

            'IniciaBgwHelper("Desinstalar")
            RutinaDesinstalar()
        Else
            MessageBox.Show($"Error1: {e.Error.Message}")
        End If
    End Sub

    Private Sub DownloadProgressCallback4(sender As Object, e As DownloadProgressChangedEventArgs)
        'Displays the operation identifier, And the transfer progress.

        bytesRecibidos = e.BytesReceived
        bytesTotales = e.TotalBytesToReceive
        bytesPorcentaje = e.ProgressPercentage

        Invoke(delegadoDescarga)
        'Console.WriteLine("{0}    downloaded {1} of {2} bytes. {3} % complete...",
        'e.UserState,
        'e.BytesReceived,
        'e.TotalBytesToReceive,
        'e.ProgressPercentage)
        'Label1.Text = $"{e.UserState.ToString}    descargado {e.BytesReceived} of {e.TotalBytesToReceive} bytes. {e.ProgressPercentage} % complete..."
    End Sub

    Private Sub ActualizarScanProcessToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ActualizarScanProcessToolStripMenuItem.Click
        'WindowState = FormWindowState.Normal
        'ShowIcon = True
        'ShowInTaskbar = True
        'EjecutarComando("taskkill /IM ScanProcess.exe")

        'Dim uri As New Uri(LinkDescargaNuevaActualizacion)

        'Lblnfo.Location = New Point(61, 67)
        'Lblnfo.Text = "Descargando instalador. Por favor, espere.."

        'Dim wClient As New WebClient()
        'AddHandler wClient.DownloadFileCompleted, AddressOf OnDownloadComplete
        'wClient.DownloadFileAsync(New Uri($"{LinkDescargaNuevaActualizacion}"), $"C:\Scantech\{IO.Path.GetFileName(uri.LocalPath)}")
        'wClient.Dispose()
    End Sub
#End Region

#Region "Rutina desinstalar"
    Private myProcess As Process
    Private eventHandled As TaskCompletionSource(Of Boolean)

    Sub RutinaDesinstalar()
        ' Create a Task with AddressOf.
        Dim task = New Task(AddressOf ProcessDataAsync)
        ' Start and wait for task to end.
        task.Start()
        task.Wait()
    End Sub
    Async Sub ProcessDataAsync()
        ' Create a task Of Integer.
        ' ... Use HandleFileAsync method with a large file.
        Dim task As Task(Of Integer) = DesinstalarAsync("cmd.exe")
        ' This statement runs while HandleFileAsync executes.
        Console.WriteLine("Please wait, processing")
        ' Use await to wait for task to complete.
        Dim result As Integer = Await task
        Console.WriteLine("ExitCode: " + result.ToString())

        If result = 0 Then
            'Invoke(delegadoInstalar)

            RutinaInstalar()
        End If
    End Sub
    Public Async Function DesinstalarAsync(filename As String) As Task(Of Integer)

        eventHandled = New TaskCompletionSource(Of Boolean)

        Using myProcess = New Process()
            Try
                'Dim startInfo As New ProcessStartInfo()
                myProcess.StartInfo.FileName = filename
                myProcess.StartInfo.Arguments = "/c MsiExec.exe /X {D8940871-73B3-40D7-818B-8B889B331C43} /passive /qn ALLUSERS="""" /m MSISDHVG"
                myProcess.StartInfo.UseShellExecute = False
                myProcess.StartInfo.CreateNoWindow = True

                myProcess.EnableRaisingEvents = True

                'AddHandler myProcess.Exited, AddressOf desinstalarFinalizado

                'myProcess.StartInfo = startInfo
                myProcess.Start()
                myProcess.WaitForExit()
                Console.WriteLine($"Proceso de desinstalación iniciado con el identificador [{myProcess.Id}]")
            Catch ex As Exception
                MsgBox(ex.Message)

            End Try

            'Wait for Exited event, but Not more than 30 seconds.
            Await Task.WhenAny(eventHandled.Task, Task.Delay(10000))
            'Await Task.CompletedTask

            'If myProcess.HasExited Then
            Return myProcess.ExitCode
            'Else
            '    Return 0
            'End If

        End Using
    End Function
#End Region

#Region "Rutina Instalar"
    Private myProcess2 As Process
    Private eventHandled2 As TaskCompletionSource(Of Boolean)
    Public Async Function InstalarAsync(filename As String) As Task

        eventHandled2 = New TaskCompletionSource(Of Boolean)()

        Using myProcess2 = New Process()
            Try
                'Dim startInfo As New ProcessStartInfo()
                myProcess2.StartInfo.FileName = filename
                myProcess2.StartInfo.Arguments = $"/c MsiExec.exe /qn /i C:\Scantech\{nombreInstalador}"
                myProcess2.StartInfo.UseShellExecute = False
                myProcess2.StartInfo.CreateNoWindow = True

                myProcess2.EnableRaisingEvents = True

                'AddHandler myProcess2.Exited, AddressOf instalarFinalizado

                'myProcess.StartInfo = startInfo
                myProcess2.Start()

                Console.WriteLine($"Proceso de instalación iniciado con el identificador [{myProcess2.Id}]")
            Catch ex As Exception
                MsgBox(ex.Message)
                Return
            End Try

            'Wait for Exited event, but Not more than 30 seconds.
            Await Task.WhenAny(eventHandled2.Task, Task.Delay(30000))
        End Using
    End Function
    Async Sub RutinaInstalar()
        'Dim p As New Process
        'Dim startInfo As New ProcessStartInfo()

        'startInfo.FileName = "cmd.exe"
        'startInfo.Arguments = "MsiExec.exe /I C:\Scantech\SetupScanProcess.msi /qn"
        'startInfo.UseShellExecute = False
        'startInfo.CreateNoWindow = True
        'p.EnableRaisingEvents = True

        'AddHandler p.Exited, AddressOf instalarFinalizado

        'p.StartInfo = startInfo

        'p.Start()
        Invoke(delegadoFinalizar)
        Await InstalarAsync("cmd.exe")


    End Sub

    'Sub instalarFinalizado(sender As Object, e As System.EventArgs)

    'End Sub

#End Region

#Region "Funciones"
    Function GetUninstallCommandFor(productDisplayName As String) As String
        'Dim products As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Installer\\UserData\\S-1-5-18\\Products")
        Dim products As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Installer\\UserData\\S-1-5-21-1939167790-894468761-171128860-1001\\Products")
        'Dim products As RegistryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node")

        Dim productFolders() As String = products.GetSubKeyNames()

        For Each p As String In productFolders
            Dim installProperties As RegistryKey = products.OpenSubKey(p & "\\InstallProperties")

            If installProperties IsNot Nothing Then
                Dim displayName As String = installProperties.GetValue("DisplayName")

                If displayName IsNot Nothing And displayName.Contains(productDisplayName) Then
                    Dim uninstallCommand As String = installProperties.GetValue("UninstallString")

                    Return uninstallCommand
                End If
            End If
        Next

        Return ""
    End Function
    Sub EjecutarComando(command As String)
        Dim p As New Process
        Dim startInfo As New ProcessStartInfo()
        startInfo.FileName = "cmd.exe"
        startInfo.Arguments = "/c " & command
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = True
        p.StartInfo = startInfo
        p.Start()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Console.WriteLine($"Tick")
        EjecutarComando("C:\Scantech\ScanCounter.exe")
        Close()
    End Sub
#End Region

#Region ""
    'Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
    '    EjecutarComando("C:\Scantech\ScanProcess.exe")
    '    Close()
    'End Sub
#End Region

End Class
