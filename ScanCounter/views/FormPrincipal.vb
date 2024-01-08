Imports System.Net
Imports System.Xml
Imports Newtonsoft.Json.Linq
Imports RestSharp
Imports System.Drawing.Color
Imports System.Web.Management
Imports System.Data.SqlClient
Imports System.ComponentModel
Imports System.Web.ModelBinding
Imports System.Runtime.Remoting.Messaging
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Threading
Imports System.Security.Cryptography.X509Certificates
Imports System.Data.SqlTypes
Imports System.Net.NetworkInformation
Imports System.Text.RegularExpressions
Imports Newtonsoft.Json
Imports System.Text
Imports System.Web.UI
Imports System.Runtime.InteropServices.ComTypes

Public Class FormPrincipal
    'Variables de ayuda para backgroundworker
    Private bgOpcion As String = ""
    Private bgwHelperResultado As Integer

    'Variables de ayuda para timer
    Private timerOpcion As String = ""

    'Instancias
    Private ReadOnly Configuraciones As ConfiguracionesModel
    Private ReadOnly Sensores As SensoresModel
    Private ReadOnly Lecturas As LecturasModel
    Private ReadOnly Wrapper As SecurityWrapper
    Private ReadOnly Batch As BatchModel
    Private ReadOnly Equipos As EquiposModel

    'Datatables
    Private bgResultado As DataTable
    'estos sensores Tienen el inicio con chos datos por que se carga directamente 
    Private Sensor1Datatable As DataTable
    Private Sensor2Datatable As DataTable
    'se utilizara la carga a travez de  hilos  para poder cargar mas rapido y actualizarlo durante la consulta nueva
    'se añadira un timer para ver cuando actualize los sensores y envie datos al arudino


    Private Equipo1Datatable As DataTable
    Private Equipo2Datatable As DataTable

    Private Sensor1AltDatatable As DataTable
    Private Sensor2AltDatatable As DataTable
    'Private TimerComprobarEntradas As timer

    Private EstadoPaleta As Boolean

    Private registrosOffline As New DataTable
    Private ConfiguracionesDatatable As DataTable
    Private SensorOfflineRegDataTable As DataTable


    Private BatchsDatatable As New DataTable


    'Delegado para lectura desde serial port arduino
    Delegate Sub myMethodDelegate()
    Private DelegadoArduino As New myMethodDelegate(AddressOf DataArduino)
    Private DelegadoMensajes As New myMethodDelegate(AddressOf MuestraPanel)

    Private PathBatch As String = "C:\Scantech\batch.json"

    'Variables globales
    Private conexionDb As Boolean = False

    Private contadorLecturasSinBd As Integer = 0

    'sensores
    Private IdSensor1 As Byte = 5
    Private IdSensor2 As Byte = 6

    Private EntradaSensor1 As String
    Private EntradaSensor2 As String

    'equipos
    Private IdEquipo1 As Byte = 1
    Private IdEquipo2 As Byte = 2

    Private NombreEquipo1 As String
    Private NombreEquipo2 As String

    Private LimiteBatch1 As Integer
    Private LimiteBatch2 As Integer

    Private ConteoBatch1 As Integer
    Private ConteoBatch2 As Integer


    Private SalidaSensor As String


    Private FechaInicioBatch1 As DateTime
    Private FechaInicioBatch2 As DateTime
    Private FechaBatch As DateTime

    Private fs As FileStream
    Private sw As StreamWriter


    Private HiloSensores As Thread

    Private COM As String = "COM3" ' se inicia con este de principio para que la conexion se realize si o si
    Private PuertoIndex As Byte = 1 'Index para consulta sql
    ' se transformaron  en arreglos ya que necesito que los contadores representen su estado offline el cual sera contadorx(1)
    Private Contador1 As Integer() = {0, 0} 'cuenta sensor 1 (identificador A desde arduino)
    Private Contador2 As Integer() = {0, 0} 'cuenta sensor 2 (identificador B desde arduino)

    Private Sensor1Estado As Byte = 2 'maneja estado del sensor 1 para actualizar variable sensor1_Estado en arduino (1: iniciado, 2: detenido, 3:reset)
    Private Sensor2Estado As Byte = 2 'maneja estado del sensor 2 para actualizar variable sensor1_Estado en arduino (1: iniciado, 2: detenido, 3:reset)
    Private SensorNombreIndex As Byte = 1 'index según consulta sql
    Private SensorIdEstadoIndex As Byte = 2 'index según consulta sql
    Private SensorNombreEstadoIndex As Byte = 3 'index según consulta sql
    Private TimerEstado As Boolean = False 'true: timer ya inició, false: timer aún no inicia
    Private IdProducto As String = ""
    Private Iniciando As Boolean = False
    Private TiempoLecturaTotal1 As Integer = 0
    Private TiempoLecturaTotal2 As Integer = 0
    'en offline segun velocidad  de cinta estas son las que deberian por defecto
    Private LecturaMinimaProducto As Integer = 500 'valores predeterminados minimos
    Private LecturaMaximaProducto As Integer = 3000 'valor predeterminado maximo 
    Private ProductoValidado As Boolean = False
    Private logErrores As String = "C:\Scantech\log_counter.text"
    Private fsErrores As FileStream
    Private MaximoAlcanzado1 As Boolean = False
    Private MaximoAlcanzado2 As Boolean = False




#Region "Constructor"
    Sub New()

        AddHandler Application.ThreadException, AddressOf Application_ThreadException
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException


        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        Configuraciones = New ConfiguracionesModel
        Sensores = New SensoresModel
        Lecturas = New LecturasModel
        Wrapper = New SecurityWrapper
        Batch = New BatchModel
        Equipos = New EquiposModel

        'AddHandler TimerSensores.Tick, AddressOf sensore
        AddHandler TimerOffline.Tick, AddressOf TimerOffline_Tick
        AddHandler TimerRed.Tick, AddressOf TimerRed_tick ' unico tick constante, el resto se llama por eventos
    End Sub
#End Region
    Private Sub getPrevInstance()
        Dim currPrsName As String = Process.GetCurrentProcess().ProcessName
        Dim allProcessWithThisName() As Process = Process.GetProcessesByName(currPrsName)
        For Each proc As Process In allProcessWithThisName
            If proc.Id <> Process.GetCurrentProcess.Id Then
                proc.Kill()
            End If
        Next
    End Sub

#Region "Backgroundworker Helper"
    Private Sub bgwHelper_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgwHelper.DoWork
        Select Case bgOpcion
            Case "Load"
                RutinaLoad()
            Case "ListarSensor"
                RutinaListarSensor()
            Case "ValidaLicencia"
                RutinaValidaLicencia()
        End Select

    End Sub
    Private Sub bgwHelper_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwHelper.RunWorkerCompleted
        Select Case bgOpcion
            Case "Load"
                RutinaLoad_Completed()
            Case "ListarSensor"
                RutinaListarSensor_Completed()
            Case "ValidaLicencia"
                RutinaValidaLicencia_Completed()
        End Select
    End Sub
#End Region

#Region "Load"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'constuyendo la tabla batch
        BatchsDatatable.Columns.Add("id_equipo", GetType(Integer))
        BatchsDatatable.Columns.Add("nombre_equipo", GetType(Integer))
        BatchsDatatable.Columns.Add("fecha_inicio", GetType(DateTime))
        BatchsDatatable.Columns.Add("fecha_termino", GetType(DateTime))

        If Not IO.File.Exists(PathBatch) Then
            fs = IO.File.Create(PathBatch)
        Else
            fs = IO.File.Open(PathBatch, FileMode.Open)
        End If

        'fs ya seteado para escribir
        getPrevInstance()
        If Not IO.File.Exists(logErrores) Then
            fsErrores = IO.File.Create(logErrores)
            fsErrores.Close()
        End If

        TimerSensores.Start()
        TimerUpdater.Start()
        TimerRed.Start()
        Iniciando = True
        'Cursor.Hide()

        PbxLoadingSensor1.Show()
        PbxLoadingSensor1.BringToFront()

        PbxLoadingSensor2.Show()
        PbxLoadingSensor2.BringToFront()

        PbxLoading3.BringToFront()

        registrosOffline.Columns.Add("id", GetType(Integer))
        registrosOffline.Columns.Add("idSensor", GetType(Integer))
        registrosOffline.Columns.Add("fecha_insercion", GetType(DateTime))
        registrosOffline.Columns.Add("idEquipo", GetType(Integer))

        bgwHelper.WorkerSupportsCancellation = True

        IniciaBackgroundworker("ValidaLicencia")
    End Sub
    Public Function ReadJson() As DataTable
        Dim json As String
        Using reader As New StreamReader(fs)
            json = reader.ReadToEnd()
        End Using
        Dim dt_result As DataTable = JsonConvert.DeserializeObject(Of DataTable)(json)
        Dim dt As New DataTable
        If dt_result IsNot Nothing Then
            If dt_result.Rows.Count > 0 Then
                dt = dt_result
            End If
        Else
            dt = Nothing
        End If
        Return dt
    End Function
    Public Sub WriteJson(dt As DataTable)
        Try
            Dim JSONString As String = JsonConvert.SerializeObject(dt, Xml.Formatting.Indented)
            Using writer As New StreamWriter(fs)
                writer.WriteLine(JSONString)
            End Using
        Catch exio As IOException
            MuestraMensaje("Error 532", 0)
        Catch ex As Exception
            MuestraMensaje("Error 522", 0)
        End Try
    End Sub


    Sub RutinaLoad()
        'If ChequeaNuevaVersionEnApi() Then
        '    Console.WriteLine($"Nueva versión disponible")
        '    Configuraciones.ActualizacionDisponible = True
        '    If Not Configuraciones.ActualizarActualizacionDisponible() Then
        '        bgResultado.Rows.Add("Error3")
        '    End If
        'Else
        '    Console.WriteLine($"No hay versión nueva")
        '    Configuraciones.ActualizacionDisponible = False
        '    If Not Configuraciones.ActualizarActualizacionDisponible() Then
        '        bgResultado.Rows.Add("Error4")
        '    End If
        'End If

        bgResultado = New DataTable
        bgResultado.Columns.Add("Result")
        'cambiar por un estado de conexion mas que la ejecucion del mismo

        If conexionDb Then
            Configuraciones.Id = 2

            ConfiguracionesDatatable = Configuraciones.Listar

            If ConfiguracionesDatatable.Rows.Count > 0 And ConfiguracionesDatatable IsNot DBNull.Value And ConfiguracionesDatatable.Columns(0).ToString <> "Error" Then

                LecturaMinimaProducto = ConfiguracionesDatatable.Rows(0)(4)
                LecturaMaximaProducto = ConfiguracionesDatatable.Rows(0)(5)
                SalidaSensor = ConfiguracionesDatatable.Rows(0)(6)

                COM = ConfiguracionesDatatable.Rows(0)(PuertoIndex)

                'Console.WriteLine($"Iniciando conexión al puerto {COM}")
                'Console.WriteLine($"lmax  {LecturaMaximaProducto}")
                'Console.WriteLine($"lmin  {LecturaMinimaProducto}")

                'If ConnectPort(COM) Then
                'EnviaCaracterArduino("g") 'Imprime cont1 y cont2

                'Actualiza ultimo inicio de lecturas de ambos sensores
                'If Sensores.ActualizarUltimoInicioLecturas Then

                bgResultado.Rows.Add("Ok")
                'Else
                '    bgResultado.Rows.Add("Error3")
                'End If
                'Else
                '    bgResultado.Rows.Add("Error")
                'End If
            Else
                bgResultado.Rows.Add("Error1")
            End If
        Else
            bgResultado.Rows.Add("Error2")
        End If
    End Sub

    Private Sub TimerOffline_Tick(sender As Object, e As EventArgs)
        If TimerOffline.Enabled = True Then
            TimerOffline.Stop()
        End If
        If conexionDb Then
            If registrosOffline.Rows.Count > 0 Then
                Dim resp As Integer = Lecturas.InsertarLecturaOffline(registrosOffline)

                If resp = 1 Then
                    registrosOffline.Clear()
                Else
                    TimerOffline.Start()
                End If
            End If
        Else
            TimerOffline.Start()
        End If
    End Sub

    Private Async Sub TimerRed_tick(sender As Object, e As EventArgs)

        Try
            Dim pings = New System.Net.NetworkInformation.Ping()
            Dim reply = Await pings.SendPingAsync(Configuration.Server, 100)
            If reply.Status = System.Net.NetworkInformation.IPStatus.Success Then
                conexionDb = True
                contadorLecturasSinBd = 0
            Else
                conexionDb = False
                If contadorLecturasSinBd < 6 Then
                    contadorLecturasSinBd += 1
                End If
            End If
        Catch ex As Exception
            conexionDb = False
            If contadorLecturasSinBd < 6 Then
                contadorLecturasSinBd += 1
            End If
        End Try



        If contadorLecturasSinBd = 6 Then
            If TimerOffline.Enabled = False Then
                TimerOffline.Start()
                'Trace.WriteLine("timer offline iniciado")
            End If
            PanelLoadingS1.BackColor = Gray
            PanelLoadingS2.BackColor = LightSlateGray
            Panel4.BackColor = CadetBlue
            PbxNetworkStatus.Show()
        Else
            PbxNetworkStatus.Hide()
            PanelLoadingS1.BackColor = Navy
            PanelLoadingS2.BackColor = SlateBlue
            Panel4.BackColor = MidnightBlue
        End If

        'Trace.WriteLine("ping " & conexionDb & " contador " & contadorLecturasSinBd)

        If registrosOffline IsNot Nothing Then
            If registrosOffline.Rows.Count > 0 Then
                If contadorLecturasSinBd = 0 Then
                    If TimerOffline.Enabled Then
                        'Trace.WriteLine("iniciando offline")
                        TimerOffline.Start()
                    End If
                End If
                'Trace.WriteLine("conteo oofline  por mientras" & registrosOffline.Rows.Count)
            End If
        End If

    End Sub

    Sub RutinaLoad_Completed()
        Select Case bgResultado.Rows(0)(0)
            Case "Ok"
                PbxComStatus.Image = My.Resources.green_dot
                PbxNetworkStatus.Hide()
                PbxLoading3.Hide()
                PbxLoading4.Hide()
                PbxLoading5.Hide()
                LblSensor1.Show()
                LblSensor2.Show()
                LblVersion.Text = Application.ProductVersion
                TimerEstado = False
                TimerHelper.Stop()
                Sensores.Id = IdSensor1
                IniciaBackgroundworker("ListarSensor")

            Case "Error1"
                PbxComStatus.Image = My.Resources.red_dot
                PbxNetworkStatus.Hide()
                If Not TimerEstado Then
                    TimerEstado = True
                    IniciaTimer("Load")
                End If

                MuestraMensaje("Error 127", 2)
                'Console.WriteLine("No hay datos en configuraciones")
            Case "Error"
                PbxComStatus.Image = My.Resources.red_dot
                PbxNetworkStatus.Hide()

                If Not TimerEstado Then
                    TimerEstado = True
                    IniciaTimer("Load")
                End If

                MuestraMensaje("Error 136", 2)
                'Console.WriteLine("Error al intentar conectar el serialport")
            Case "Error2"
                PbxComStatus.Image = My.Resources.red_dot
                PbxNetworkStatus.Hide()

                If Not TimerEstado Then
                    TimerEstado = True
                    IniciaTimer("Load")
                End If

                MuestraMensaje("Error 145", 2)
                'Console.WriteLine("Sin conexión al server")
            Case "Error3"
                MuestraMensaje("Error 190", 2)
                'Console.WriteLine("No se pudo actualizar actualizacion_disponible a true")
            Case "Error4"
                MuestraMensaje("Error 197", 2)
                'Console.WriteLine("No se pudo actualizar actualizacion_disponible a false")
        End Select

        ProductoValidado = True
    End Sub

    Function ChequeaNuevaVersionEnApi() As Boolean

        Dim client As New RestClient($"https://scantech.cl/api/productos/read_update_ruta_producto.php?id_producto={IdProducto}")

        Dim request = New RestRequest(Method.GET)
        Dim response As IRestResponse = client.Execute(request)
        Dim content As String = response.Content

        Dim result As Boolean
        If response.StatusCode = HttpStatusCode.OK Then
            Dim json As JObject = JObject.Parse(content)
            If Application.ProductVersion <> json.SelectToken("Producto.version") Then
                result = True
            Else
                result = False
            End If
        Else
            result = False
        End If

        Return result
    End Function
#End Region

#Region "Valida licencia"
    Sub RutinaValidaLicencia()
        'bgwHelperResultado = 1

        If Not IO.File.Exists(Configuration.SourcePath) Then
            bgwHelperResultado = 2
        Else
            Dim XmlDoc As XmlDocument = New XmlDocument()
            XmlDoc.Load(Configuration.SavePath)
            If XmlDoc.DocumentElement("encrypted_key").InnerText <> "" Then
                IdProducto = Wrapper.DecryptData(XmlDoc.DocumentElement("encrypted_key").InnerText)
                'Console.WriteLine($"IdProducto {IdProducto}")

                ' Lee licencia segun el id del producto
                Dim client As New RestClient($"https://scantech.cl/api/licencias/read_by_id_producto.php?id_producto={IdProducto}")

                Dim request = New RestRequest(Method.GET)
                Dim response As IRestResponse = client.Execute(request)
                Dim content As String = response.Content

                If response.StatusCode = HttpStatusCode.OK Then
                    Dim json As JObject = JObject.Parse(content)

                    '¿Licencia está activa? (key_estado ACT || DIS)
                    'Console.WriteLine($"Licencia N°: {json.SelectToken("Licencia.id_licencia")}")
                    'Console.WriteLine($"Estado: {json.SelectToken("Licencia.key_estado")}")

                    Dim result As String = json.SelectToken("Licencia.key_estado").ToString
                    Select Case result
                    'Case "DIS"
                    '    bgwHelperResultado = 2
                        Case "DES"
                            ProductoValidado = False
                            bgwHelperResultado = 3
                        Case "ACT", "DIS"
                            'Console.WriteLine($"{json.SelectToken("Licencia.validez")}")
                            Select Case json.SelectToken("Licencia.validez").ToString.ToUpper
                                Case "PRO"
                                    bgwHelperResultado = 1
                                Case "STANDAR"
                                    Dim fechaActivacion As Date = json.SelectToken("Licencia.fecha_activacion")

                                    Dim desface As Long = DateDiff("d", fechaActivacion, Now)

                                    If desface >= 7 Then
                                        bgwHelperResultado = 4
                                    End If
                            End Select
                    End Select
                End If
            Else
                bgwHelperResultado = 2
            End If
        End If
    End Sub
    Sub RutinaValidaLicencia_Completed()
        Select Case bgwHelperResultado
            Case 0
                MuestraMensaje("Error LIC 232", 2)
            Case 1
                If ProductoValidado = False Then
                    IniciaBackgroundworker("Load")
                End If
            Case 2
                Cursor.Show()
                MsgBox("Su producto está sin licencia, por favor contacte a soporte@scantech.cl")
                FormActivacion.ShowDialog()
            Case 3
                Cursor.Show()
                MsgBox("Su producto ha sido desactivado, por favor contacte a soporte@scantech.cl")
                FormActivacion.ShowDialog()
            Case 4
                Cursor.Show()
                MsgBox("Para activar su licencia, por favor contacte a soporte@scantech.cl")
                FormActivacion.ShowDialog()
        End Select
    End Sub
#End Region

#Region "Listar sensores (Lista estado de sensores según intervalo de timer. Envía dato a PLC según estado configurado en el backend) "
    Sub RutinaListarSensor()
        bgResultado.Rows.Clear()

        ConfiguracionesDatatable = Configuraciones.Listar

        If ConfiguracionesDatatable.Rows.Count > 0 And ConfiguracionesDatatable IsNot DBNull.Value And ConfiguracionesDatatable.Columns(0).ToString <> "Error" Then
            If ConfiguracionesDatatable.Rows(0)(4) <> LecturaMinimaProducto Then 'si es distinto a lo asignado en load
                LecturaMinimaProducto = ConfiguracionesDatatable.Rows(0)(4)
                'Console.WriteLine($"LecturaMinimaProducto {LecturaMinimaProducto}")
            End If

            If ConfiguracionesDatatable.Rows(0)(5) <> LecturaMaximaProducto Then
                LecturaMaximaProducto = ConfiguracionesDatatable.Rows(0)(5)
                'Console.WriteLine($"LecturaMaximaProducto {LecturaMaximaProducto}")
            End If

        End If
        'Console.WriteLine($"ConfiguracionesDatatable.Rows(0)(3) {ConfiguracionesDatatable.Rows(0)(3).ToString}")
        'ConfiguracionesDatatable = Configuraciones.Listar
        'If ConfiguracionesDatatable.Rows(0)(3) = True Then 'deploy_actualizacion
        '    Configuraciones.DeployActualizacion = False
        '    Configuraciones.ActualizarDeployActualizacion()
        '    bgResultado.Rows.Add("Update")
        '    Exit Sub
        'End If
        Select Case Sensores.Id
            Case IdSensor1
                Sensor1Datatable = Sensores.Listar
                If Sensor1Datatable.Rows.Count > 0 And Sensor1Datatable IsNot Nothing Then
                    bgResultado.Rows.Add("Ok")
                Else
                    bgResultado.Rows.Add("Error1")
                End If
            Case IdSensor2
                Sensor2Datatable = Sensores.Listar
                If Sensor2Datatable.Rows.Count > 0 And Sensor2Datatable IsNot Nothing Then
                    bgResultado.Rows.Add("Ok")
                Else
                    bgResultado.Rows.Add("Error2")
                End If
        End Select
    End Sub
    Sub RutinaListarSensor_Completed()

        Select Case bgResultado.Rows(0)(0)
            Case "Ok"
                Select Case Sensores.Id
                    Case IdSensor1

                        'ImprimeDatatable(Sensor1Datatable, "we")
                        'If Iniciando Then
                        Contador1(0) = Sensor1Datatable.Rows(0)(4)
                        LblContador1.Text = Contador1(0)
                            'aqui
                            'If Sensor1Datatable.Rows(0)(4) <> ConteoBatch1 Then
                            '    ConteoBatch1 = Sensor1Datatable.Rows(0)(4)
                            'End If

                            AcomodaLabel("Contador1")
                                'End If
                                'Trace.WriteLine("wenisimas")
                                LblSensor1.Text = Sensor1Datatable.Rows(0)(SensorNombreIndex)

                                'If Sensor1Datatable.Rows(5)(1) Then
                                LblSensor1Estado.Text = $"En estado [{Sensor1Datatable.Rows(0)(SensorNombreEstadoIndex)}]"
                                Sensor1Estado = Sensor1Datatable.Rows(0)(SensorIdEstadoIndex)
                                PbxLoadingSensor1.Hide()


                        Sensores.Id = IdSensor2
                        IniciaBackgroundworker("ListarSensor")
                    Case IdSensor2

                        'ImprimeDatatable(Sensor2Datatable, "wenas")

                        'If Iniciando Then

                        'If Sensor2Datatable.Rows(0)(4) <> ConteoBatch2 Then
                        '    ConteoBatch2 = Sensor2Datatable.Rows(0)(4)
                        'End If
                        LblContador2.Text = Contador2(0)
                            AcomodaLabel("Contador2")
                            'End If

                            LblSensor2.Text = Sensor2Datatable.Rows(0)(SensorNombreIndex)
                            LblSensor2Estado.Text = $"En estado [{Sensor2Datatable.Rows(0)(SensorNombreEstadoIndex)}]"


                        Sensor2Estado = Sensor2Datatable.Rows(0)(SensorIdEstadoIndex)
                        Contador2(0) = Sensor2Datatable.Rows(0)(4)

                        PbxLoadingSensor2.Hide()

                            If Not TimerEstado Then
                            TimerEstado = True
                            IniciaTimer("ListarSensor")
                        Else
                            Select Case Sensor1Estado
                                Case 1
                                    EnviaCaracterArduino("a") 'sensor1_Estado = 1 (Comienza a aumentar contador)
                                Case 2
                                    EnviaCaracterArduino("b") 'sensor1_Estado = 2 (Sólo imprime contador (en pausa))
                                Case 3
                                    'EnviaCaracterArduino("c") 'Reset de contador y lo pone en pausa para no realizar conteo
                                    EnviaCaracterArduino("b") 'Reset de contador y lo pone en pausa para no realizar conteo

                                    Contador1(0) = 0
                                    LblContador1.Text = Contador1(0)

                                    AcomodaLabel("Contador1")
                            End Select

                            Select Case Sensor2Estado
                                Case 1
                                    EnviaCaracterArduino("d") 'sensor2_Estado = 1 (Comienza a aumentar contador)
                                Case 2
                                    EnviaCaracterArduino("e") 'sensor2_Estado = 2 (Sólo imprime contador (en pausa))
                                Case 3
                                    'EnviaCaracterArduino("f") 'Reset de contador y lo pone en pausa para no realizar conteo
                                    EnviaCaracterArduino("e") 'Reset de contador y lo pone en pausa para no realizar conteo

                                    Contador2(0) = 0
                                    LblContador2.Text = Contador2(0)
                                    AcomodaLabel("Contador2")
                            End Select

                            Iniciando = False
                        End If
                End Select

                Dim cont1 As Integer = 0
                Dim cont2 As Integer = 0
                If LblContador1.Text <> "" Then
                    cont1 = CInt(LblContador1.Text)
                End If

                If LblContador2.Text <> "" Then
                    cont2 = CInt(LblContador2.Text)
                End If
                Dim total As Integer
                total = Contador1(0) + Contador2(0)

                LblTotal.Text = total
                'If total >= LimiteBatch1 Then
                '    EnviaCaracterArduino("k")
                'Else
                '    EnviaCaracterArduino("j")
                'End If



            Case "Error1"
                MuestraMensaje("Error 220", 2)
                'Console.WriteLine("Error listando sensor 1")
            Case "Error2"
                MuestraMensaje("Error 223", 2)
                'Console.WriteLine("Error listando sensor 2")
                'Case "Update"
                '    EjecutarComando("C:\Scantech\ScanUpdater.exe")

                '    Close()
        End Select
        'Trace.WriteLine("terminar de listar")

    End Sub
#End Region

#Region "Timer Helper"
    Private Sub TimerListarSensorEstado_Tick(sender As Object, e As EventArgs) Handles TimerHelper.Tick
        Select Case timerOpcion
            Case "ListarSensor"
                Sensores.Id = IdSensor1

                IniciaBackgroundworker("ListarSensor")
            Case "Load"
                IniciaBackgroundworker("Load")
        End Select
    End Sub
#End Region

#Region "Insertar lectura"
    Async Sub RutinaInsertar(IdSensor As Integer, IdEquipo As Integer)
        Lecturas.IdSensor = IdSensor
        Lecturas.FechaInsercion = Now
        Lecturas.IdEquipo = IdEquipo
        'Trace.WriteLine("wenas")
        Await InsertarAsync()
        'InsertarAsync()
    End Sub
    Public Async Function InsertarAsync() As Task
        Await Lecturas.Insertar
    End Function
#End Region

#Region "Timer para controlar tiempo de lectura de cada producto"
    Private Sub TimerTiempoLectura_Tick(sender As Object, e As EventArgs) Handles TimerTiempoLectura1.Tick
        'If TiempoLecturaTotal1 < LecturaMaximaProducto Then
        TiempoLecturaTotal1 += TimerTiempoLectura1.Interval
        LblLectura1.Text = TiempoLecturaTotal1
        If Sensor1Estado <> 1 Then
            TimerTiempoLectura1.Stop()
            LblLectura1.Text = "0"
        End If

        'Else
        '    TimerTiempoLectura1.Stop()
        'MaximoAlcanzado1 = True
        'End If
    End Sub
    Private Sub TimerTiempoLectura2_Tick(sender As Object, e As EventArgs) Handles TimerTiempoLectura2.Tick
        'If TiempoLecturaTotal2 < LecturaMaximaProducto Then
        TiempoLecturaTotal2 += TimerTiempoLectura2.Interval
        LblLectura2.Text = TiempoLecturaTotal2
        If Sensor2Estado <> 1 Then
            TimerTiempoLectura2.Stop()
            LblLectura2.Text = "0"
        End If
        'Else
        '    TimerTiempoLectura2.Stop()
        '    MaximoAlcanzado2 = True
        'End If
    End Sub

#End Region

#Region "Arduino"
    Private Function ConnectPort(puerto As String) As Boolean
        Dim result As Boolean = False

        Try
            With SerialPort1
                .PortName = puerto
                .BaudRate = 115200
                .DataBits = 8
                .Parity = IO.Ports.Parity.None
                .StopBits = IO.Ports.StopBits.One
                .Handshake = IO.Ports.Handshake.None
                .Open()
                If SerialPort1.IsOpen Then
                    result = True
                Else
                    result = False
                End If
            End With
        Catch ex As Exception
            result = False
        End Try
        Return result
    End Function

    Private Sub DisconnectPort()
        Try
            If SerialPort1.IsOpen Then
                SerialPort1.Close()
                PbxComStatus.Image = My.Resources.red_dot
            End If
        Catch ex As Exception
            'Trace.WriteLine("error al desconectar puerto")
        End Try
    End Sub
#End Region

#Region "Delegado para puerto serial y función DataReceived de SerialPort"
    Sub DataArduino()
        Try

            'Lee desde arduino
            Dim lectura As String = SerialPort1.ReadExisting
            If Not String.IsNullOrWhiteSpace(lectura) Then
                lectura = lectura.Trim
                Select Case lectura
                    Case "A" 'Sensor 1 - I0_0
                        ' Sensor dejó de leer (I0_0 en 0V)
                        TimerTiempoLectura1.Stop()
                        'Trace.WriteLine("lec min " & LecturaMinimaProducto & " / lect total " & TiempoLecturaTotal1)
                        If TiempoLecturaTotal1 >= LecturaMinimaProducto And TiempoLecturaTotal1 <= LecturaMaximaProducto Then ' este formulario no existe
                            Select Case Sensor1Estado
                                Case 1
                                    Contador1(0) += 1

                                    If conexionDb Then
                                        RutinaInsertar(IdSensor1, 1)
                                    Else
                                        Dim row As DataRow = registrosOffline.NewRow
                                        row(0) = registrosOffline.Rows.Count + 1
                                        row(1) = IdSensor1
                                        row(2) = Now
                                        row(3) = IdEquipo1
                                        registrosOffline.Rows.Add(row)
                                    End If
                                    Trace.WriteLine("limites 1 = " & Contador1(0) + Contador2(0) & " == " & CInt(LblTotal.Text) & "")
                                    If (Contador1(0) + Contador2(0)) >= LimiteBatch1 Then
                                        CrearBatch(IdEquipo1, NombreEquipo1)
                                    End If

                            End Select
                        End If
                        TiempoLecturaTotal1 = 0
                    Case "Z"
                        ' Sensor se encuentra leyendo (I0_0 en 24V)
                        TimerTiempoLectura1.Enabled = True
                        TimerTiempoLectura1.Start()
                    Case "B" 'Sensor 2 - I0_1
                        ' Sensor dejó de leer (I0_1 en 0V)
                        TimerTiempoLectura2.Stop()
                        If TiempoLecturaTotal2 >= LecturaMinimaProducto And TiempoLecturaTotal2 <= LecturaMaximaProducto Then
                            Select Case Sensor2Estado
                                Case 1
                                    Contador2(0) += 1
                                    If conexionDb Then
                                        RutinaInsertar(IdSensor2, 2)
                                    Else
                                        Dim row As DataRow = registrosOffline.NewRow
                                        row(0) = registrosOffline.Rows.Count + 1
                                        row(1) = IdSensor2
                                        row(2) = Now
                                        row(3) = IdEquipo1
                                        registrosOffline.Rows.Add(row)
                                    End If
                                    Trace.WriteLine("limites 1 = " & LimiteBatch1 & " == " & CInt(LblTotal.Text) & "")
                                    If (Contador2(0) + Contador1(0)) >= LimiteBatch1 Then
                                        CrearBatch(IdEquipo1, NombreEquipo1)

                                    End If

                            End Select
                        End If

                        TiempoLecturaTotal2 = 0
                    Case "Y"
                        TimerTiempoLectura2.Enabled = True
                        TimerTiempoLectura2.Start()


                    Case "L"
                        EstadoPaleta = True
                        PbxEstadoPaleta.Image = My.Resources.PaletaAbierta
                    Case "S"
                        EstadoPaleta = False
                        PbxEstadoPaleta.Image = My.Resources.PaletaCerrada
                End Select

                LblTotal.Text = CInt(LblContador1.Text) + CInt(LblContador2.Text)
            Else
                MuestraMensaje("Error 332", 2)

            End If

            'Mueve texto del contador para centrarlo
            AcomodaLabel("Contador1")
            AcomodaLabel("Contador2")
            AcomodaLabel("ContadorTotal")
        Catch ex As Exception
            'se trata de hacer esto por que se sospecha sin conexion
            Sensor1Estado = 0
            Sensor2Estado = 0
            Trace.WriteLine("final de error  552 " & ex.Message)
            MuestraMensaje("Error 552", 2)
        End Try
    End Sub
    Private Sub CrearBatch(IdEquipo As Integer, NombreEquipo As String)
        'intentar ver si el json de batchs se encuentra bien
        Dim FechaInicioBatchLocal As New DateTime
        If IdEquipo = 1 Then
            FechaInicioBatchLocal = FechaInicioBatch1
        Else
            FechaInicioBatchLocal = FechaInicioBatch2
        End If

        Dim FechaTerminoBatch As New DateTime
        Dim Fechasegundos As New DateTime

        FechaBatch = Now
        'se añade tiempo siempre pensandoen que la ejecucion  del  ultimo pesaje se esta realizando
        Fechasegundos = Now.AddSeconds(1)

        If conexionDb Then
            Batch.IdEquipo = IdEquipo
            Batch.FechaInicio = FechaInicioBatchLocal
            Batch.NombreEquipo = NombreEquipo

            Dim resp = Batch.InsertarBatch()
            Trace.WriteLine("resp batch  = " & resp)
            If resp = 1 Then
                If EstadoPaleta Then
                    EnviaCaracterArduino("j")
                Else
                    EnviaCaracterArduino("k")
                End If
            End If
            Contador1(0) = 0
            Contador2(0) = 0
            'Else
            '    counBatchsDatatable.Rows.Add(IdEquipo, NombreEquipo, FechaInicioBatchLocal, Nothing)

        End If
        'Insertar
        ' insertando normalmente en la tabla de 
        ' como respuesta registrandolo lo ultimo en le  json


        'Reseteando la insersion del batch
        If IdEquipo = 1 Then
            FechaInicioBatch1 = Now
        Else
            FechaInicioBatch2 = Now
        End If

    End Sub



    Private Sub SerialPort_DataReceived(sender As Object, e As IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        Invoke(DelegadoArduino)
    End Sub
#End Region

#Region "Funciones"
    Sub AcomodaLabel(Cual As String)
        Select Case Cual
            Case "Contador1"
                Select Case LblContador1.Text.Length
                    Case 1 ' < 10
                        LblContador1.Location = New Point(340, 220)
                    Case 2 ' >= 10 < 100
                        LblContador1.Location = New Point(250, 220)
                    Case 3 ' >= 100 < 1000
                        LblContador1.Location = New Point(180, 220)
                    Case 4 ' >= 1000 < 10000
                        LblContador1.Location = New Point(110, 220)
                    Case 5 ' >= 10000 < 100000
                        LblContador1.Location = New Point(40, 220)
                End Select
            Case "Contador2"
                Select Case LblContador2.Text.Length
                    Case 1 ' < 10
                        LblContador2.Location = New Point(380, 220)
                    Case 2 ' >= 10 < 100
                        LblContador2.Location = New Point(290, 220)
                    Case 3 ' >= 100 < 1000
                        LblContador2.Location = New Point(220, 220)
                    Case 4 ' >= 1000 < 10000
                        LblContador2.Location = New Point(140, 220)
                    Case 5 ' >= 10000 < 100000
                        LblContador2.Location = New Point(70, 220)
                End Select
            Case "Total"
                Select Case LblTotal.Text.Length
                    Case 1, 2 ' < 100 Pzs
                        LblTotal.Location = New Point(805, 80)
                    Case 3 ' >= 100 < 1000 Pzs
                        LblTotal.Location = New Point(780, 80)
                    Case 4 ' >= 1000 < 10000 Pzs
                        LblTotal.Location = New Point(720, 80)
                    Case 5 ' >= 10000 < 100000 Pzs
                        LblTotal.Location = New Point(690, 80)
                    Case 6 ' >= 100000 < 1000000 Pzs
                        LblTotal.Location = New Point(640, 80)
                End Select
        End Select
    End Sub
    Sub IniciaBackgroundworker(Opcion As String)
        bgOpcion = Opcion

        If bgwHelper.IsBusy Then
            'Trace.WriteLine(bgOpcion)
            If bgOpcion <> "ValidaLicencia" And bgOpcion <> "ListarSensor" Then
                MuestraMensaje($"Error 353", 2)
            End If

            bgwHelper.CancelAsync()
        Else
            bgwHelper.RunWorkerAsync()
        End If
    End Sub

    Sub IniciaTimer(Opcion As String)
        timerOpcion = Opcion

        TimerHelper.Start()
    End Sub
    Dim wenas As String
    Public Function convertirDatosArduino(sensor As String, posicion As String) As String
        Try
            sensor = "1"
            posicion = "I0_1"
            Dim x As String = posicion.Substring(0, 2)
            Dim medio As String = "1"
            Select Case x
                Case "I0"
                    medio = "1"
                Case "I1"
                    medio = "2"
            End Select
            Dim final As String = ""
            If posicion.Length > 4 Then
                final = posicion.Substring(2, 1)
            Else
                final = posicion.Substring(2, 2)
            End If
            wenas = sensor & ":" & medio & ":" & final
            Return wenas
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Sub EnviaCaracterArduino(Caracter As String)
        'Trace.WriteLine("we" & Caracter)
        If SerialPort1.IsOpen Then
            Try
                SerialPort1.WriteTimeout = 3000
                SerialPort1.WriteLine(Caracter)
                'If Not Caracter.Contains("a") Or Not Caracter.Contains("b") Then
                '    Trace.WriteLine("caracter enviado (" & Caracter & ")")
                'End If
                PbxComStatus.Image = My.Resources.green_dot
            Catch ex As Exception
                DisconnectPort()
                MuestraMensaje($"Error 374", 2)
                PbxComStatus.Image = My.Resources.red_dot
                If TimerTiempoLectura1.Enabled Then
                    TimerTiempoLectura1.Stop()
                End If
                If TimerTiempoLectura2.Enabled Then
                    TimerTiempoLectura2.Stop()
                End If
                ConnectPort(COM)
            End Try
        Else
            DisconnectPort()
            MuestraMensaje("Error 378", 2)
            If TimerTiempoLectura1.Enabled Then
                TimerTiempoLectura1.Stop()
            End If
            If TimerTiempoLectura2.Enabled Then
                TimerTiempoLectura2.Stop()
            End If
            PbxComStatus.Image = My.Resources.red_dot
            ConnectPort(COM)
        End If
    End Sub

    Sub MuestraMensaje(Mensaje As String, Tipo As Byte)
        LblError.Text = Mensaje
        If Tipo = 1 Then
            PanelError.BackColor = Color.Green
        Else
            PanelError.BackColor = Color.Red
        End If
        Invoke(DelegadoMensajes)
        TimerMensaje.Start()
    End Sub
    Sub MuestraPanel()
        PanelError.Show()
    End Sub

    Private Sub TimerMensaje_Tick(sender As Object, e As EventArgs) Handles TimerMensaje.Tick
        PanelError.Hide()
    End Sub

    Private Sub TimerUpdater_Tick(sender As Object, e As EventArgs) Handles TimerUpdater.Tick
        'sender hace esto por que si no tiene conexikno cuando inica y luego lo recupera no vuelve
        IniciaBackgroundworker("ValidaLicencia")
    End Sub

    Public Sub LogERR(mensaje As String)
        'fsErrores = IO.File.Open(logErrores, FileMode.Open)
        Using writer As New StreamWriter(logErrores, True)
            writer.WriteLine(mensaje)
        End Using
        'fsErrores.Close()
    End Sub
    Private Sub Application_ThreadException(ByVal sender As Object, ByVal e As ThreadExceptionEventArgs)
        LogERR("Error Crash Tread: reiniciando con hora " & Now & " Error: " & e.Exception.Message & " / " & e.Exception.TargetSite.ToString)
        lazaro()
    End Sub
    Private Sub CurrentDomain_UnhandledException(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        'Dim ex As Exception = DirectCast(e.ExceptionObject, Exception)
        LogERR("Error Crash Exep: reiniciando con hora " & Now & " Error: " & e.ExceptionObject.Message & " / " & e.ExceptionObject.TargetSite.ToString)
        lazaro()
    End Sub

    Public Sub lazaro()
        Dim path_batch As String = "C:\Scantech\batch_counter.bat"
        Dim fs As FileStream
        If Not IO.File.Exists(path_batch) Then
            fs = IO.File.Create(path_batch)
        Else
            fs = IO.File.Open(path_batch, FileMode.Open)
        End If
        Dim path_batch_txt As String = "@echo off
                                        taskkill /f /im SC100PLUS.exe
                                        timeout /t 4 /nobreak >nul
                                        start """" ""C:\Scantech\ScanCounter.exe"" "

        Using writer As New StreamWriter(fs)
            writer.Write(path_batch_txt)
        End Using
        ejecutarComando(path_batch)
    End Sub
    Public Sub ejecutarComando(command As String)
        Dim p As New Process
        Dim startInfo As New ProcessStartInfo()
        startInfo.FileName = "cmd.exe"
        startInfo.Arguments = "/c " & command
        startInfo.UseShellExecute = False
        startInfo.CreateNoWindow = True
        p.StartInfo = startInfo
        p.Start()
    End Sub

    Public Function FiltroLetraEntradaSalida(Valor As String) As Char
        'patronar regex con lo envviado
        Dim Caracter As Char
        Try
            Dim regexIO As New Regex("^(([IQ])[0-1]_(1[0-2]|[0-9]))$")
            If regexIO.IsMatch(Valor) Then
                Dim ValorFinal As Integer = 0
                Dim Largo = 2
                'constantes
                Dim BaseIs As Integer = 65
                Dim BaseQs As Integer = BaseIs + 13
                If Valor.Length <= 4 Then
                    Largo -= 1
                End If

                ValorFinal = CInt(Valor.Substring(3, Largo))
                If Valor.Contains("Q") Then
                    ValorFinal += BaseQs
                ElseIf Valor.Contains("I") Then
                    ValorFinal += BaseIs
                End If

                If ValorFinal >= 65 And ValorFinal <= 85 Then
                    Caracter = ChrW(ValorFinal)
                    Trace.WriteLine("caracter obtenido por filtro  = " & Caracter)
                Else
                    Caracter = Nothing
                End If
            Else
                Caracter = Nothing
            End If
        Catch ex As Exception
            Trace.WriteLine("error en la asignacion de variable normal" & ex.Message)
            Caracter = Nothing
        End Try
        Return Caracter
    End Function

    Public Sub EnviarCambioPines(valor As String, posicion As Integer)
        Try
            Dim Paso1, Paso2, Paso3 As String
            If posicion >= 0 And posicion <= 2 Then '0,1,2
                Select Case posicion
                    Case 0
                        Paso1 = "x"
                    Case 1
                        Paso1 = "y"
                    Case 2
                        Paso1 = "z"
                End Select
                Trace.WriteLine(Paso1 & " es lo que envia")

                EnviaCaracterArduino(Paso1)
                Trace.WriteLine("paso1 enviado ")
                Thread.Sleep(300)
                Trace.WriteLine("esperado 1 segundo")
                Paso2 = FiltroLetraEntradaSalida(valor)
                If Paso2 IsNot Nothing Then
                    EnviaCaracterArduino(Paso2)
                    Trace.WriteLine("paso2 enviado ")
                    Thread.Sleep(300)

                    Trace.WriteLine("DORMIDO 1 SEG")
                    Paso3 = "0"
                    EnviaCaracterArduino(Paso3)
                    Trace.WriteLine("terminando con el ultimo paso completado")
                End If
            End If
            Trace.WriteLine("funcion terminada buien")

        Catch ex As Exception
            Trace.WriteLine("Error envaindo cambio pines detalles: " & ex.Message)
        End Try

        'se subira primero la direccion addres de la EEPROM x,y,z(entrada1,entrada2,salida1)
        'luego se obtendra segun el datatable si fue cambiado desde la base de datos  el 
        'numero de pin representado por una letra mayuscula de la A - U
        'si estos 2 pasos no generaron errores entonces mandar el 0 de confirmacion, 
        'i cambiando el valor en la memoria EEPROM junto con cargar dentro del loop los pines
        'si da error 3 veces en la comunicacion entonces el plc dira que se quedo sin confirmacion despues de completar 100 ciclos, 1 segundo
        'se reiniciara a los valores a 0 evitando el cambio  y por programa se quedara como el anterior dando mensaje de error y seteando al 
        'valor anterior del que se cambio, tambien añadir un loader  y alertas

    End Sub

    Private Sub TimerSensores_Tick(sender As Object, e As EventArgs) Handles TimerSensores.Tick
        HiloSensores = New Thread(AddressOf ObtenerSensores)
        HiloSensores.Start()
        TimerSensores.Stop() 'hasta que se cambien  o si detectan alguncambio que lo hacan y que no aga otro por detras
        Dim resp As Boolean = False
        Try
            'mientras tengan datos entonces que compare
            If Sensor1AltDatatable IsNot Nothing And Sensor2AltDatatable IsNot Nothing Then
                'cuando se levante la primera vez, como puedo saber si el pin dado existe,
                'solamente cuando tenga conexion con la base de datos y la no se interrumpa

                resp = verificacionAtrib(EntradaSensor1, Sensor1AltDatatable, 3)
                If resp Then
                    Trace.WriteLine("wenas")
                    EnviarCambioPines(EntradaSensor1, 0)
                    resp = False
                    'Else
                    '    Trace.WriteLine("no enviar")
                End If
                'If EntradaSensor1 IsNot Nothing Then
                '    If EntradaSensor1 <> Sensor1AltDatatable(0)(3) Then
                '        EntradaSensor1 = Sensor1AltDatatable(0)(3)
                '    End If
                'Else
                '    EntradaSensor1 = Sensor1AltDatatable(0)(3)
                'End If


                resp = verificacionAtrib(EntradaSensor2, Sensor2AltDatatable, 3)
                If resp Then
                    Trace.WriteLine("wenas2")
                    EnviarCambioPines(EntradaSensor2, 1)
                    resp = False
                    'Else
                    '    Trace.WriteLine("no enviar")
                End If
                'If EntradaSensor2 IsNot Nothing Then
                '    If EntradaSensor2 <> Sensor2AltDatatable(0)(3) Then
                '        EntradaSensor2 = Sensor2AltDatatable(0)(3)
                '    End If
                'Else
                '    EntradaSensor2 = Sensor2AltDatatable(0)(3)
                'End If

                'Trace.WriteLine("sensor 1 " & EntradaSensor1)
                'Trace.WriteLine("sensor 2 " & EntradaSensor2)
            End If



            If ConfiguracionesDatatable IsNot Nothing Then
                Dim i = verificacionAtrib(SalidaSensor, ConfiguracionesDatatable, 6)
                If i Then
                    EnviarCambioPines(SalidaSensor, 2)
                    'Else
                    '    Trace.WriteLine("no enviar")
                End If
                'Trace.WriteLine("salida 1 " & SalidaSensor)
                'If SalidaSensor <> ConfiguracionesDatatable.Rows(0)(6) Then
                '    SalidaSensor = ConfiguracionesDatatable.Rows(0)(6)
                'End If

            End If

            If Equipo1Datatable IsNot Nothing And Equipo2Datatable IsNot Nothing Then

                verificacionAtrib(NombreEquipo1, Equipo1Datatable, 1)
                'If NombreEquipo1 IsNot Nothing Then
                '    If NombreEquipo1 <> Equipo1Datatable.Rows(0)(1) Then
                '        NombreEquipo1 = Equipo1Datatable.Rows(0)(1)
                '    End If
                'Else
                '    NombreEquipo1 = Equipo1Datatable.Rows(0)(1)
                'End If

                verificacionAtrib(NombreEquipo2, Equipo2Datatable, 1)

                'If NombreEquipo2 IsNot Nothing Then
                '    If NombreEquipo2 <> Equipo2Datatable.Rows(0)(1) Then
                '        NombreEquipo2 = Equipo2Datatable.Rows(0)(1)
                '    End If
                'Else
                '    NombreEquipo2 = Equipo2Datatable.Rows(0)(1)
                'End If

                verificacionAtrib(LimiteBatch1, Equipo1Datatable, 2)
                'If LimiteBatch1 = Nothing Then
                '    If LimiteBatch1 <> Equipo1Datatable.Rows(0)(2) Then
                '        LimiteBatch1 = Equipo1Datatable.Rows(0)(2)
                '    End If
                'Else
                '    LimiteBatch1 = Equipo1Datatable.Rows(0)(2)
                'End If
                verificacionAtrib(LimiteBatch2, Equipo2Datatable, 2)
                'If LimiteBatch2 = Nothing Then
                '    If LimiteBatch2 <> Equipo2Datatable.Rows(0)(2) Then
                '        LimiteBatch2 = Equipo2Datatable.Rows(0)(2)
                '    End If
                'Else
                '    LimiteBatch2 = Equipo2Datatable.Rows(0)(2)
                'End If

                'Trace.WriteLine("Equipo 1 " & NombreEquipo1)
                'Trace.WriteLine("Equipo 2 " & NombreEquipo2)
                'Trace.WriteLine("LimiteBatch 1 " & LimiteBatch1)
                'Trace.WriteLine("LimiteBatch 2 " & LimiteBatch2)
            End If

        Catch ex As Exception
            Trace.WriteLine("Error la ingresar la wea" & ex.Message)
        Finally
            TimerSensores.Start()
        End Try
    End Sub

    Public Function verificacionAtrib(ByRef variable As Object, ByRef datatable As DataTable, posicionDatatable As Integer) As Boolean
        Dim resp = False
        If variable IsNot Nothing Then
            If variable <> datatable.Rows(0)(posicionDatatable) Then
                variable = datatable.Rows(0)(posicionDatatable)
                resp = True
            End If
        Else
            variable = datatable.Rows(0)(posicionDatatable)
        End If
        Return resp
    End Function

    Public Sub ImprimeDatatable(dt As DataTable, nombre_tabla As String)
        Trace.WriteLine("Imprimiendo datatable '" & nombre_tabla & "'")
        If dt IsNot Nothing Then
            Trace.WriteLine("Total Filas: " & dt.Rows.Count)
            Trace.WriteLine("Total Columnas: " & dt.Columns.Count)
            For j As Integer = 0 To dt.Columns.Count - 1
                Trace.Write(CStr(j) & ":'" & dt.Columns(j).ColumnName & "'(" & dt.Columns(j).DataType.Name & ") ")
            Next
            'Imprimo el datatable
            For i As Integer = 0 To dt.Rows.Count - 1
                Trace.WriteLine("")
                Trace.Write("fila " & i & " | ")
                For j As Integer = 0 To dt.Columns.Count - 1

                    If dt.Rows(i)(j) IsNot DBNull.Value Then
                        Trace.Write("(" & CStr(j) & "): " & CStr(dt.Rows(i)(j)) & "; ")
                    Else
                        Trace.Write("(" & CStr(j) & "): " & "Null; ")
                    End If
                Next

            Next
        Else
            Trace.WriteLine("La tabla es Nothing")
        End If
    End Sub

    'caracteresArduino
    Public Sub ObtenerSensores() ' al final obtendra todo 
        Equipos.id = IdEquipo1
        Equipo1Datatable = Equipos.ListarById()
        Equipos.id = IdEquipo2
        Equipo2Datatable = Equipos.ListarById()
        Configuraciones.Id = 1
        ConfiguracionesDatatable = Configuraciones.Listar()
        Sensores.Id = IdSensor1
        Sensor1AltDatatable = Sensores.ListarAlt
        Sensores.Id = IdSensor2
        Sensor2AltDatatable = Sensores.ListarAlt
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        EnviaCaracterArduino("j") 'ESTADO PALETA = FALSE
        Trace.WriteLine("caracter j enviado")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        EnviaCaracterArduino("k") 'ESTADO PALETA = TRUE
        Trace.WriteLine("caracter k enviado")
    End Sub

#End Region

End Class
