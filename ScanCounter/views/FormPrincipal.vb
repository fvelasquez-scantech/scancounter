Imports System.Diagnostics.Eventing.Reader
Imports System.Drawing.Color
Imports System.IO
Imports System.Net
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Xml
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports RestSharp

Public Class FormPrincipal
    'Variables de ayuda para backgroundworker
    Private bgOpcion As String = ""
    Private bgwHelperResultado As Integer = 0

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
    'se utilizara la carga a travez de  hilos  para poder cargar mas rapido y actualizarlo durante la consulta nueva
    'se añadira un timer para ver cuando actualize los sensores y envie datos al arudino

    Private ComandoEjecutado As Boolean = False

    Private BatchOffline As DataTable
    Private registrosOffline As DataTable
    Private Sensor1Datatable As New DataTable
    Private Sensor2Datatable As New DataTable
    Private Equipo1Datatable As New DataTable
    Private Equipo2Datatable As New DataTable
    Private Sensor1AltDatatable As New DataTable
    Private Sensor2AltDatatable As New DataTable
    Private ConfiguracionesDatatable As New DataTable
    'Private TimerComprobarEntradas As timer

    Private Errores As String = ""
    Private ContErrores As Integer = 0

    Private EstadoFormActivacion As Boolean = False
    'variables de cambio de paleta
    Private EstadoPaletaAsingadaPlc As Boolean = False
    Private EstadoPaleta As Integer = 0 ''esta apagada
    Private PrimeraAsignacionPaleta As Boolean = False

    'tipos de cambio paleta desde front o back
    Private CambioPaletaDesdeFront As Boolean = False
    Private CambioPaletaDesdeBack As Boolean = False


    'Delegado para lectura desde serial port arduino
    Delegate Sub myMethodDelegate()
    Private DelegadoArduino As New myMethodDelegate(AddressOf DataArduino)
    Private DelegadoMensajes As New myMethodDelegate(AddressOf MuestraPanel)

    Private logErrores As String = "C:\Scantech\log_counter.text"
    Private PathBatch As String = "C:\Scantech\batch.json"
    Private PathLecturas As String = "C:\Scantech\lecturas.json"

    'Variables globales
    Private conexionDb As Boolean = False
    Private contadorLecturasSinBd As Integer = 0

    Private UltimaFechaInicioBatch As Date

    Private UltimacuentaOnline1 As Integer
    Private UltimacuentaOnline2 As Integer
    ' arreglo de problema por que paletea una y otra vez desde que se deja de paletear
    Private EstadoCrearBatchTimer As Boolean = False


    'sensores
    Private IdSensor1 As Integer = 5
    Private IdSensor2 As Integer = 6

    Private EntradaSensor1 As String
    Private EntradaSensor2 As String

    Private TiempoEspera As Integer = 10000 ' default 10 segundos para cmabio de paleta  hasta que se desaga todo


    Private PeticionBatch As Integer = 0 ' estado normal
    Private ProcesandoSolicitudBatch As Boolean = False
    Private BatchDeSolicitudInsertado As Boolean = False
    'equipos
    Private IdEquipo1 As Byte = 1
    Private IdEquipo2 As Byte = 2

    Private NombreEquipo1 As String
    Private NombreEquipo2 As String

    Private LimiteBatch1 As Integer
    Private LimiteBatch2 As Integer

    Private ConteoBatch1 As Integer
    Private ConteoBatch2 As Integer

    Private CambiandoPaleta As Boolean = False
    Private SalidaSensor As String


    Private FechaInicioBatch1 As DateTime
    Private FechaInicioBatch2 As DateTime
    Private FechaBatch As DateTime

    Private FsErrores As FileStream
    Private FsBatch As FileStream
    Private FsLecturas As FileStream



    Private HiloSensores As Thread
    Private HiloCancelacionBatch As Thread
    Private COM As String = "COM17" ' DEBUGT OFFLINE
    'Private COM As String = "COM3" ' se inicia con este de principio para que la conexion se realize si o si
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

    Private MaximoAlcanzado1 As Boolean = False
    Private MaximoAlcanzado2 As Boolean = False


#Region "Constructor"
    Sub New()

        AddHandler Application.ThreadException, AddressOf Application_ThreadException
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException

        registrosOffline = New DataTable
        BatchOffline = New DataTable

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
        AddHandler TimerJson.Tick, AddressOf TimerJson_Tick
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

        'EnviarCambioPines("I0_12", 0) ' I0_11
        'EnviarCambioPines("I0_11", 1) ' I0_12
        'EnviarCambioPines("Q0_0", 2)
        getPrevInstance()
        'vista de letras
        'Button1.Visible = False
        'Button2.Visible = False
        'constuyendo la tabla batch

        ConfiguracionesDatatable.Columns.Add("id", GetType(Integer))
        ConfiguracionesDatatable.Columns.Add("puerto", GetType(String))
        ConfiguracionesDatatable.Columns.Add("actualizacion_disponible", GetType(Integer))
        ConfiguracionesDatatable.Columns.Add("deploy_actualizacion", GetType(Integer))
        ConfiguracionesDatatable.Columns.Add("lectura_minima_producto", GetType(Integer))
        ConfiguracionesDatatable.Columns.Add("lectura_maxima_producto", GetType(Integer))
        ConfiguracionesDatatable.Columns.Add("salida_plc", GetType(String))
        ConfiguracionesDatatable.Columns.Add("TiempoEspera", GetType(Integer))
        'ConfiguracionesDatatable.Rows.Add(1, "COM3", 0, 0, 40, 1000, "Q0_0", 10000)
        ConfiguracionesDatatable.Rows.Add(1, "COM17", 0, 0, 40, 1000, "Q0_0", 10000) ' DEBUG OFFLINE

        Equipo1Datatable.Columns.Add("Id", GetType(Integer))
        Equipo1Datatable.Columns.Add("Nombre", GetType(String))
        Equipo1Datatable.Columns.Add("Conteo", GetType(Integer))
        Equipo1Datatable.Columns.Add("Estado", GetType(String))

        Equipo1Datatable.Rows.Add(1, "Offline", 10, "Esperando") ' debug 10 lecturas solamente
        'Equipo1Datatable.Rows.Add(1, "Offline", 3500, "Esperando") 'nomal
        ' definicion normal de cuantas piezas aguanta un camion

        Equipo2Datatable.Columns.Add("Id", GetType(Integer))
        Equipo2Datatable.Columns.Add("Nombre", GetType(String))
        Equipo2Datatable.Columns.Add("Conteo", GetType(Integer))
        Equipo2Datatable.Columns.Add("Estado", GetType(String))
        Equipo2Datatable.Rows.Add(2, "Offline", 3500, "Esperando")


        registrosOffline.Columns.Add("id", GetType(Integer))
        registrosOffline.Columns.Add("id_sensor", GetType(Integer))
        registrosOffline.Columns.Add("fecha_insercion", GetType(DateTime))
        registrosOffline.Columns.Add("id_equipo", GetType(Integer))

        registrosOffline.Rows.Add(1, 1, Now, 1)


        BatchOffline.Columns.Add("id", GetType(Integer))
        BatchOffline.Columns.Add("id_equipo", GetType(Integer))
        BatchOffline.Columns.Add("nombre_equipo", GetType(String))
        BatchOffline.Columns.Add("fecha_inicio", GetType(DateTime))
        BatchOffline.Columns.Add("fecha_termino", GetType(DateTime))



        Sensor1AltDatatable.Columns.Add("Id", GetType(Integer))
        Sensor1AltDatatable.Columns.Add("Nombre", GetType(String))
        Sensor1AltDatatable.Columns.Add("Estado", GetType(String))
        Sensor1AltDatatable.Columns.Add("Plc", GetType(String))

        Sensor1AltDatatable.Rows.Add(1, "Grader 1", "Iniciado", "I0_4") ' inductores

        Sensor2AltDatatable.Columns.Add("Id", GetType(Integer))
        Sensor2AltDatatable.Columns.Add("Nombre", GetType(String))
        Sensor2AltDatatable.Columns.Add("Estado", GetType(String))
        Sensor2AltDatatable.Columns.Add("Plc", GetType(String))

        Sensor2AltDatatable.Rows.Add(2, "Grader 2", "Iniciado", "I0_5") ' inductores

        'ya definido los batch offline  podemos ver cual es la ultima fecha para que la cuenta pueda ser arreglada

        Sensor1Datatable.Columns.Add("IdSensor", GetType(Integer))
        Sensor1Datatable.Columns.Add("NombreSensor", GetType(String))
        Sensor1Datatable.Columns.Add("IdEstado", GetType(Integer))
        Sensor1Datatable.Columns.Add("NombreEstado", GetType(String))
        Sensor1Datatable.Columns.Add("lecturas", GetType(Integer))
        Sensor1Datatable.Columns.Add("Plc", GetType(String))
        'offline siempre se deja habilitado para poder iniciar la cuenta si no se ha validado
        Sensor1Datatable.Rows.Add(1, "Grader 1", 1, "Iniciado", 0, "I0_4")

        Sensor2Datatable.Columns.Add("IdSensor", GetType(Integer))
        Sensor2Datatable.Columns.Add("NombreSensor", GetType(String))
        Sensor2Datatable.Columns.Add("IdEstado", GetType(Integer))
        Sensor2Datatable.Columns.Add("NombreEstado", GetType(String))
        Sensor2Datatable.Columns.Add("lecturas", GetType(Integer))
        Sensor2Datatable.Columns.Add("Plc", GetType(String))

        Sensor2Datatable.Rows.Add(2, "Grader 2", 1, "Iniciado", 0, "I0_5")

        '<---------------------------archivos------------------->


        'If Not IO.File.Exists(PathBatch) Then
        '    FsBatch = IO.File.Create(PathBatch)
        'Else
        '    FsBatch = IO.File.Open(PathBatch, FileMode.Open)
        'End If
        'FsBatch.Close()
        Dim dtba = ReadJson(PathBatch)
        If dtba IsNot Nothing Then
            If dtba.Columns.Count > 2 Or dtba.Rows.Count > 1 Then
                BatchOffline = dtba
                UltimaFechaInicioBatch = BatchOffline.AsEnumerable().Max(Function(row) row.Field(Of DateTime)("fecha_inicio"))
            Else
                'BatchOffline.Rows.Add(BatchOffline.Rows.Count + 1, IdEquipo1, NombreEquipo1, Now.ToString("dd-MM-yyyy HH:mm:ss"), DBNull.Value)
                'UltimaFechaInicioBatch = Now
            End If
        Else
            'BatchOffline.Rows.Add(BatchOffline.Rows.Count + 1, IdEquipo1, NombreEquipo1, Now.ToString("dd-MM-yyyy HH:mm:ss"), DBNull.Value)
            'UltimaFechaInicioBatch = Now
            'Trace.WriteLine("2" & UltimaFechaInicioBatch)
        End If


        'aqui setear el el datatable en proceso para continuar

        'If Not IO.File.Exists(PathLecturas) Then
        '    FsLecturas = IO.File.Create(PathLecturas)
        'Else
        '    FsLecturas = IO.File.Open(PathLecturas, FileMode.Open)
        'End If
        'FsLecturas.Close()
        Dim dtreg = ReadJson(PathLecturas)
        If dtreg IsNot Nothing Then

            registrosOffline = dtreg
        End If

        'aqui continaur con las lecutas y asignar el coneo segun lo obtenido offline

        If Not IO.File.Exists(logErrores) Then
            FsErrores = IO.File.Create(logErrores)
            FsErrores.Close()
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

        bgwHelper.WorkerSupportsCancellation = True

        IniciaBackgroundworker("ValidaLicencia")
    End Sub
    Public Function ReadJson(path As String) As DataTable
        Dim dt As New DataTable
        Try
            Dim json As String
            Using reader As New StreamReader(path)
                json = reader.ReadToEnd()
            End Using
            Dim dt_result As DataTable = JsonConvert.DeserializeObject(Of DataTable)(json)
            If dt_result IsNot Nothing Then
                If dt_result.Rows.Count > 0 Then
                    dt = dt_result
                End If
            Else
                dt = Nothing
            End If
            Return dt
        Catch ex As Exception
            LogERR("Error FormPrincipal 442 : " & ex.Message)
            Return Nothing
        End Try
    End Function

    'Public Sub WriteJsonFs(dt As DataTable, fs As FileStream)

    '    Try
    '        Dim JSONString As String = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented)
    '        Trace.WriteLine("json = " & JSONString)
    '        Using writer As New StreamWriter()
    '            writer.WriteLine(JSONString)
    '        End Using
    '        Trace.WriteLine("completado")
    '    Catch exio As IOException
    '        MuestraMensaje("Error 532", 0)
    '        LogERR("Writejson :" & exio.Message)
    '    Catch ex As Exception
    '        LogERR("writejson ex: " & ex.Message)
    '        MuestraMensaje("Error 522", 0)
    '    End Try

    'End Sub

    Public Sub WriteJson(dt As DataTable, path As String)
        Try
            Dim JSONString As String = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented)
            'Trace.WriteLine("json = " & JSONString)
            Dim fsll = IO.File.Open(path, FileMode.Open)
            Using writer As New StreamWriter(path, False)
                'Trace.WriteLine("1")
                writer.WriteLine(JSONString)
                'Trace.WriteLine("2")
            End Using
            fsll.Close()
            'Trace.WriteLine("completado")
        Catch exio As IOException
            Trace.WriteLine(exio.Message)
            'MuestraMensaje("Error 532", 0)
            LogERR("Writejson :" & exio.Message)
        Catch ex As Exception
            'Trace.WriteLine(ex.Message)
            LogERR("writejson ex: " & ex.Message)
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
                TiempoEspera = ConfiguracionesDatatable.Rows(0)(7)

                COM = ConfiguracionesDatatable.Rows(0)(PuertoIndex)
                'COM = "COM16"
                Console.WriteLine($"Iniciando conexión al puerto {COM}")
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
            bgResultado.Rows.Add("Ok")
            'se remplaza ya que no importa si no tiene conexion deberia contar al inicio
            'bgResultado.Rows.Add("Error2")
        End If
    End Sub

    Private Sub TimerOffline_Tick(sender As Object, e As EventArgs)

        If TimerOffline.Enabled = True Then
            TimerOffline.Stop()
        End If
        If conexionDb Then
            'sensores
            If registrosOffline IsNot Nothing Then
                If registrosOffline.Rows.Count > 0 Then

                    'ImprimeDatatable()
                    'Trace.WriteLine("insertando lecturas offlines")
                    Dim resp As Integer = Lecturas.InsertarLecturaOffline(registrosOffline)
                    If resp = 1 Then
                        'Trace.WriteLine("completado lec")
                        registrosOffline.Clear()
                        WriteJson(registrosOffline, PathLecturas)
                    Else
                    End If
                End If
            End If

            If BatchOffline IsNot Nothing Then
                If BatchOffline.Rows.Count > 0 Then
                    'Trace.WriteLine("insertando batch offlines")
                    'ImprimeDatatable(BatchOffline, "desde timer offline")
                    Dim resp As Integer = Batch.InsertarBatchOffline(BatchOffline)
                    If resp = 1 Then
                        'Trace.WriteLine("completado batch")
                        BatchOffline.Clear()
                        WriteJson(BatchOffline, PathBatch)
                    Else
                        TimerOffline.Start()
                    End If
                End If

            End If
            'batchs es para que cuando se genere los batch solo tengan lo necesario y si se
        End If

        TimerOffline.Start()

    End Sub

    Private Async Sub TimerRed_tick(sender As Object, e As EventArgs)

        Try
            Dim pings = New System.Net.NetworkInformation.Ping()
            Dim reply = Await pings.SendPingAsync(Configuration.Server, 100)
            If reply.Status = System.Net.NetworkInformation.IPStatus.Success Then
                'Trace.WriteLine("hay red")
                conexionDb = True
                contadorLecturasSinBd = 0
            Else
                'Trace.WriteLine("no hay  red")
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
            Panel6.BackColor = CadetBlue
            PbxNetworkStatus.Show()
        Else
            PbxNetworkStatus.Hide()
            PanelLoadingS1.BackColor = Navy
            PanelLoadingS2.BackColor = SlateBlue
            Panel4.BackColor = MidnightBlue
            Panel6.BackColor = MidnightBlue
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
            If conexionDb Then
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
            Else
                If XmlDoc.DocumentElement("encrypted_key").InnerText <> "" Then
                    IdProducto = Wrapper.DecryptData(XmlDoc.DocumentElement("encrypted_key").InnerText)
                    'Trace.WriteLine("wenas el  id producto es el siguiente" & IdProducto)
                    Dim regexCodCounter As New Regex("^SCANCOUNTER-[0-9]{1,3}$") ' ardbox 20
                    'si el producto cumple con el patron regex una vez desencriptado entonces
                    'puede continuar en offline ya que se supone que anterior mente paso
                    'por la etapa de gestion de activación
                    If regexCodCounter.IsMatch(IdProducto) Then
                        bgwHelperResultado = 1 ' permite la carga normal con modo offline
                    Else
                        bgwHelperResultado = 0 ' error en la creacion pero no en la activacion de archivo
                    End If
                Else
                    bgwHelperResultado = 2
                End If
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
                If EstadoFormActivacion = False Then
                    EstadoFormActivacion = True
                    MsgBox("Su producto está sin licencia, por favor contacte a soporte@scantech.cl")
                    FormActivacion.ShowDialog()
                End If
            Case 3
                Cursor.Show()
                If EstadoFormActivacion = False Then
                    EstadoFormActivacion = True
                    MsgBox("Su producto ha sido desactivado, por favor contacte a soporte@scantech.cl")
                    FormActivacion.ShowDialog()
                End If
            Case 4
                Cursor.Show()
                If EstadoFormActivacion = False Then
                    EstadoFormActivacion = True
                    MsgBox("Para activar su licencia, por favor contacte a soporte@scantech.cl")
                    FormActivacion.ShowDialog()
                End If
        End Select
    End Sub
#End Region

#Region "Listar sensores (Lista estado de sensores según intervalo de timer. Envía dato a PLC según estado configurado en el backend) "
    Sub RutinaListarSensor()

        bgResultado.Rows.Clear()
        If conexionDb Then
            Dim dtconf = Configuraciones.Listar()
            If dtconf IsNot Nothing Then
                ConfiguracionesDatatable = dtconf  'solamente cambiar cuando se necesite
            End If
        Else
            'actualizar cuenta con los registro del offline

        End If

        'siempre tiene que iniciar cargada

        'cargar configuraciones default

        If ConfiguracionesDatatable.Rows.Count > 0 And ConfiguracionesDatatable IsNot DBNull.Value And ConfiguracionesDatatable.Columns(0).ToString <> "Error" Then
            If ConfiguracionesDatatable.Rows(0)(4) <> LecturaMinimaProducto Then 'si es distinto a lo asignado en load
                LecturaMinimaProducto = ConfiguracionesDatatable.Rows(0)(4)
            End If

            If ConfiguracionesDatatable.Rows(0)(5) <> LecturaMaximaProducto Then
                LecturaMaximaProducto = ConfiguracionesDatatable.Rows(0)(5)
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
        'Trace.WriteLine("fecha_busqeuda = " & UltimaFechaInicioBatch)
        Select Case Sensores.Id
            Case IdSensor1
                If conexionDb Then
                    Dim dtsen1 = Sensores.Listar
                    If dtsen1 IsNot Nothing Then
                        If dtsen1.Columns.Count > 1 Then
                            Sensor1Datatable = dtsen1
                            UltimacuentaOnline1 = Sensor1Datatable.Rows(0)(4)
                        End If
                    End If
                Else
                    Dim countOfRows As Integer
                    If registrosOffline IsNot Nothing Then
                        If registrosOffline.Rows.Count > 0 Then
                            'Trace.WriteLine("fecha fachera" & UltimaFechaInicioBatch)



                            countOfRows = registrosOffline.AsEnumerable().
                                Where(Function(row) row.Field(Of DateTime)("fecha_insercion") > UltimaFechaInicioBatch AndAlso row.Field(Of Integer)("id_sensor") = IdSensor1).
                                Count()


                        Else
                            countOfRows = 0
                        End If
                    Else
                        countOfRows = 0
                    End If

                    If Sensor1Datatable IsNot Nothing Then
                        'Trace.WriteLine("ult cunt 1 " & UltimacuentaOnline1)
                        Sensor1Datatable.Rows(0)(4) = UltimacuentaOnline1 + countOfRows
                    End If
                End If

                If Sensor1Datatable.Rows.Count > 0 And Sensor1Datatable IsNot Nothing Then
                    bgResultado.Rows.Add("Ok")
                Else
                    bgResultado.Rows.Add("Error1")
                End If
            Case IdSensor2
                If conexionDb Then
                    Dim dtsen2 = Sensores.Listar
                    If dtsen2 IsNot Nothing Then
                        If dtsen2.Columns.Count > 1 Then
                            Sensor2Datatable = dtsen2
                            UltimacuentaOnline2 = Sensor2Datatable.Rows(0)(4)
                        End If
                    End If
                Else
                    Dim countOfRows As Integer
                    Dim fecha As DateTime = Now
                    If registrosOffline IsNot Nothing Then
                        If registrosOffline.Rows.Count > 0 Then
                            countOfRows = registrosOffline.AsEnumerable().
                                Where(Function(row) row.Field(Of DateTime)("fecha_insercion") > UltimaFechaInicioBatch AndAlso row.Field(Of Integer)("id_sensor") = IdSensor2).
                                Count()
                        Else
                            countOfRows = 0
                        End If
                    Else
                        countOfRows = 0
                    End If

                    If Sensor2Datatable IsNot Nothing Then
                        Sensor2Datatable.Rows(0)(4) = UltimacuentaOnline2 + countOfRows
                    End If
                End If

                'como obtener un conteo de inicio si no existe una cuenta' entonces iniciar un batch
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
                        If Sensor1Datatable IsNot Nothing Then
                            If Sensor1Datatable.Columns.Count > 1 Then

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
                            End If
                        End If
                        Sensores.Id = IdSensor2
                        IniciaBackgroundworker("ListarSensor")
                    Case IdSensor2

                        'ImprimeDatatable(Sensor2Datatable, "wenas")

                        'If Iniciando Then

                        'If Sensor2Datatable.Rows(0)(4) <> ConteoBatch2 Then
                        '    ConteoBatch2 = Sensor2Datatable.Rows(0)(4)
                        'End If

                        If Sensor2Datatable IsNot Nothing Then
                            If Sensor2Datatable.Columns.Count > 1 Then

                                LblContador2.Text = Contador2(0)
                                AcomodaLabel("Contador2")
                                'End If

                                LblSensor2.Text = Sensor2Datatable.Rows(0)(SensorNombreIndex)
                                LblSensor2Estado.Text = $"En estado [{Sensor2Datatable.Rows(0)(SensorNombreEstadoIndex)}]"
                                Sensor2Estado = Sensor2Datatable.Rows(0)(SensorIdEstadoIndex)
                                Contador2(0) = Sensor2Datatable.Rows(0)(4)
                            End If
                        End If



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
            Case "Error1"
                MuestraMensaje("Error 220", 2)
            Case "Error2"
                MuestraMensaje("Error 223", 2)
                'Console.WriteLine("Error listando sensor 2")
                'Case "Update"
                '    EjecutarComando("C:\Scantech\ScanUpdater.exe")

                '    Close()
        End Select

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
        Await InsertarAsync()
        'InsertarAsync()
    End Sub
    Public Async Function InsertarAsync() As Task
        Dim x = Await Lecturas.Insertar
        If x = 0 Then
            ' los no insertados se hiran en  el offline para poder se insertados asi no mas
            Dim row As DataRow = registrosOffline.NewRow
            row(0) = registrosOffline.Rows.Count + 1
            row(1) = IdSensor1
            row(2) = Now.ToString("dd-MM-yyyy HH:mm:ss")
            row(3) = IdEquipo1
            registrosOffline.Rows.Add(row) ' que despues se asignes por medio de offline json tick
        End If
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
                                        If registrosOffline.Columns.Count = 0 Then
                                            registrosOffline.Columns.Add("id", GetType(Integer))
                                            registrosOffline.Columns.Add("id_sensor", GetType(Integer))
                                            registrosOffline.Columns.Add("fecha_insercion", GetType(DateTime))
                                            registrosOffline.Columns.Add("id_equipo", GetType(Integer))
                                        End If
                                        Dim row As DataRow = registrosOffline.NewRow
                                        row(0) = registrosOffline.Rows.Count + 1
                                        row(1) = IdSensor1
                                        row(2) = Now.ToString("dd-MM-yyyy HH:mm:ss")
                                        row(3) = IdEquipo1
                                        registrosOffline.Rows.Add(row)
                                        LblContador1.Text = Contador1(0)
                                    End If
                                    'ImprimeDatatable(registrosOffline, "registros")
                                    'Trace.WriteLine("limites 1 = " & Contador1(0) + Contador2(0) & " == " & CInt(LblTotal.Text) & "")
                                    If (Contador1(0) + Contador2(0)) >= LimiteBatch1 Then
                                        'If EstadoCrearBatchTimer = False Then
                                        'puede el sensor crear el batch
                                        'EstadoCrearBatchTimer = True ' no creara mas batchs
                                        CrearBatch(IdEquipo1, NombreEquipo1)
                                        'End If
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
                                        'Trace.WriteLine("UNICO DATATABLE QUE QUEDA VAICO")
                                        'ImprimeDatatable(registrosOffline, "bhg")
                                        If registrosOffline.Columns.Count = 0 Then
                                            registrosOffline.Columns.Add("id", GetType(Integer))
                                            registrosOffline.Columns.Add("id_sensor", GetType(Integer))
                                            registrosOffline.Columns.Add("fecha_insercion", GetType(DateTime))
                                            registrosOffline.Columns.Add("id_equipo", GetType(Integer))
                                        End If

                                        Dim row As DataRow = registrosOffline.NewRow
                                        row(0) = registrosOffline.Rows.Count + 1
                                        row(1) = IdSensor2
                                        row(2) = Now.ToString("dd-MM-yyyy HH:mm:ss")
                                        row(3) = IdEquipo1
                                        registrosOffline.Rows.Add(row)
                                        LblContador2.Text = Contador2(0)

                                    End If
                                    'Trace.WriteLine($" x = {Contador1(0) + Contador2(0)} vs y =  {LimiteBatch1} ")
                                    If (Contador2(0) + Contador1(0)) >= LimiteBatch1 Then
                                        'si el batch fue creado recientemente  entonces
                                        'If EstadoCrearBatchTimer = False Then
                                        '    EstadoCrearBatchTimer = True ' se otorgara false cuando haya pasado un tiempo minimo esperado  y luego leera la paleta
                                        CrearBatch(IdEquipo1, NombreEquipo1)
                                            'End If
                                        End If

                            End Select
                        End If

                        TiempoLecturaTotal2 = 0
                    Case "Y"
                        TimerTiempoLectura2.Enabled = True
                        TimerTiempoLectura2.Start()
                    Case "L"

                        If EstadoPaletaAsingadaPlc = False Then
                            EstadoPaletaAsingadaPlc = True
                        End If
                        'dejar asingar solamente cuando no esta viendo de otro
                        'lado el cambio
                        If CambioPaletaDesdeBack = False Then
                            EstadoPaleta = 1
                        End If
                        PbxEstadoPaleta.Image = My.Resources.PaletaAbierta
                        Button1.BackColor = Color.LightYellow
                        Button2.BackColor = Color.White
                    Case "S"

                        If EstadoPaletaAsingadaPlc = False Then
                            EstadoPaletaAsingadaPlc = True
                        End If
                        If CambioPaletaDesdeBack = False Then
                            EstadoPaleta = 0
                        End If

                        Button1.BackColor = Color.White
                        Button2.BackColor = Color.LightYellow
                        PbxEstadoPaleta.Image = My.Resources.PaletaCerrada
                End Select

                LblTotal.Text = CInt(LblContador1.Text) + CInt(LblContador2.Text)
            Else
                MuestraMensaje("Error 332", 2)
            End If
            'Trace.WriteLine("wenas")
            'Mueve texto del contador para centrarlo
            AcomodaLabel("Contador1")
            AcomodaLabel("Contador2")
            AcomodaLabel("Total")
        Catch ex As Exception
            'se trata de hacer esto por que se sospecha sin conexion
            Sensor1Estado = 0
            Sensor2Estado = 0
            'Trace.WriteLine("final de error  552 " & ex.Message)
            MuestraMensaje("Error 552", 2)
        End Try
    End Sub
    'se necesita ver cuando se prende en que estado paleta estubo la ultima vez
    Private Sub AsignarPrimerEstadoPaleta()
        If conexionDb Then
            Configuraciones.Id = 1
            Configuraciones.EstadoPaleta = EstadoPaleta
            Dim resp = Configuraciones.ActualizarPaleta()
            'ImprimeDatatable(resp, "wen")
            If resp.Rows(0)(0) = 1 Then
                If CambioPaletaDesdeFront = True Then
                    CambioPaletaDesdeFront = False ' para que la proxima vez no lo realize
                End If
                PrimeraAsignacionPaleta = True
            Else
                If CambioPaletaDesdeFront = False Then
                    PrimeraAsignacionPaleta = False
                End If
            End If
        Else
            If CambioPaletaDesdeFront = False Then
                PrimeraAsignacionPaleta = False
            End If
        End If
    End Sub

    Private Sub CrearBatch(IdEquipo As Integer, NombreEquipo As String)
        'Trace.WriteLine("creando batch")
        Try
            'intentar ver si el json de batchs se encuentra bien
            Dim FechaInicioBatchLocal As New DateTime
            If IdEquipo = 1 Then
                FechaInicioBatchLocal = FechaInicioBatch1
            Else
                FechaInicioBatchLocal = FechaInicioBatch2
            End If

            If conexionDb Then
                'Trace.WriteLine("batch por online")
                If registrosOffline IsNot Nothing Then
                    If registrosOffline.Rows.Count > 0 Then
                        'Trace.WriteLine("insertando lecturas offlines")
                        Dim respme As Integer = Lecturas.InsertarLecturaOffline(registrosOffline)
                        If respme = 1 Then
                            'Trace.WriteLine("completado lec")
                            registrosOffline.Clear()
                            WriteJson(registrosOffline, PathLecturas)
                        End If
                    End If
                End If

                If BatchOffline IsNot Nothing Then
                    If BatchOffline.Rows.Count > 0 Then
                        'ImprimeDatatable(BatchOffline, "desde creacion de batch")
                        Dim respme As Integer = Batch.InsertarBatchOffline(BatchOffline)
                        If respme = 1 Then
                            BatchOffline.Clear()
                            WriteJson(BatchOffline, PathBatch)
                        End If
                    End If
                End If
                Batch.IdEquipo = IdEquipo
                Batch.FechaInicio = FechaInicioBatchLocal
                Batch.NombreEquipo = NombreEquipo

                Dim resp = Batch.InsertarBatch()

                If resp = 1 Then
                    Contador1(0) = 0
                    Contador2(0) = 0
                    UltimacuentaOnline1 = 0
                    UltimacuentaOnline2 = 0
                    If IdEquipo = 1 Then
                        FechaInicioBatch1 = Now
                    Else
                        FechaInicioBatch2 = Now
                    End If
                    'solo liverado cuando  se hacer online
                    BatchDeSolicitudInsertado = True
                End If
            Else
                'Trace.WriteLine("batch  por offline")
                If BatchOffline.Rows.Count > 0 Then
                    'cambiar ultimo registro a la fecha actual
                    Dim UltimaFila = BatchOffline.AsEnumerable().OrderByDescending(Function(row) row.Field(Of DateTime)("fecha_inicio")).First()
                    UltimaFila(4) = Now ' fila fecha termino cambiada
                End If

                BatchOffline.Rows.Add(BatchOffline.Rows.Count + 1, IdEquipo1, NombreEquipo, Now, DBNull.Value)

                ' escribir de inmediato al json, ya que los batch tienen poca probabilidad de
                ' elimiarse durante lo que es el corte de luz en el json
                Dim dtlll = BatchOffline
                WriteJson(dtlll, PathBatch)
                ' se añaden las lecturas para cuando se necesiten 
                WriteJson(registrosOffline, PathLecturas)
            End If

            'se necesita probar lo que  es el no se actualiza en la base de datos como back end

            'xxx




            If CambiandoPaleta = False Then
                CambiandoPaleta = True
                Dim HiloCambioPaleta = New Thread(AddressOf CambiarPaleta)
                HiloCambioPaleta.Start()
            End If


            UltimaFechaInicioBatch = Now ' que es mantenga localmente siempre
            UltimacuentaOnline1 = 0
            UltimacuentaOnline2 = 0



            'HiloCancelacionBatch = New Thread(AddressOf HabilitarBatch)
            'HiloCancelacionBatch.Start()


        Catch ex As Exception
            LogERR("Error FormPrincipal 276 : " & ex.Message)
            Trace.WriteLine("exception = " & ex.Message)
        End Try

    End Sub

    Public Sub HabilitarBatch()
        Thread.Sleep(30000) ' 30 segundos despues de el primer batch para que no se aga a cada rato
        EstadoCrearBatchTimer = False ' se vuelve a habilitar despues de un tiempo 

    End Sub

    Public Sub CambiarPaleta()
        Trace.WriteLine("comenzando a dormir" & TiempoEspera)
        Thread.Sleep(TiempoEspera)
        ' despues de esperar para el modo offline y se pueda actualizar asimismo, y si no tiene  entonces  que lo cambie  local, luego online
        If conexionDb Then
            Trace.WriteLine("despertando")
            'If EstadoPaleta
            Trace.WriteLine("asignando estado inverso  y actualizando")
            If EstadoPaleta = 0 Then
                EstadoPaleta = 1
            Else
                EstadoPaleta = 0
            End If
            AsignarPrimerEstadoPaleta()
            Trace.WriteLine("actualizacion completada")
            CambiandoPaleta = False
        Else
            If EstadoPaleta = 1 Then
                EnviaCaracterArduino("k") 'abierta
            Else
                EnviaCaracterArduino("j") 'cerrada
            End If
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
                'Trace.WriteLine("cambiando")
                Select Case LblContador1.Text.Length
                    Case 1 ' < 10
                        LblContador1.Location = New Point(370, 220)
                    Case 2 ' >= 10 < 100
                        LblContador1.Location = New Point(330, 220)
                    Case 3 ' >= 100 < 1000
                        LblContador1.Location = New Point(290, 220)
                    Case 4 ' >= 1000 < 10000
                        LblContador1.Location = New Point(250, 220)
                    Case 5 ' >= 10000 < 100000
                        LblContador1.Location = New Point(210, 220)
                End Select
            Case "Contador2"
                Select Case LblContador2.Text.Length
                    Case 1 ' < 10
                        LblContador2.Location = New Point(420, 220)
                    Case 2 ' >= 10 < 100
                        LblContador2.Location = New Point(380, 220)
                    Case 3 ' >= 100 < 1000
                        LblContador2.Location = New Point(340, 220)
                    Case 4 ' >= 1000 < 10000
                        LblContador2.Location = New Point(300, 220)
                    Case 5 ' >= 10000 < 100000
                        LblContador2.Location = New Point(260, 220)
                End Select
            Case "Total"
                'Trace.WriteLine("reacomodando label text")
                Select Case LblTotal.Text.Length
                    Case 1 ' < 100 Pzs
                        LblTotal.Location = New Point(880, 107)
                    Case 2 ' >= 100 < 1000 Pzs
                        LblTotal.Location = New Point(780, 107)
                    Case 3 ' >= 1000 < 10000 Pzs
                        LblTotal.Location = New Point(680, 107)
                    Case 4 ' >= 10000 < 100000 Pzs
                        LblTotal.Location = New Point(580, 107)
                    Case 5 ' >= 100000 < 1000000 Pzs
                        LblTotal.Location = New Point(480, 107)
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



            'Trace.WriteLine(COM)
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
        Errores = Errores & vbCrLf & mensaje & " [" & Now.ToString("yyyy-MM-dd HH:mm:ss") & "]"
        If ContErrores < 3 Then
            'Trace.WriteLine("escibiendo  con esto")
            'FsErrores = IO.File.Open(logErrores, FileMode.Append)
            Using writer As New StreamWriter(logErrores, True)
                writer.WriteLine(Errores)
            End Using
            'FsErrores.Close()
            Errores = ""
        Else
            ContErrores += 1
        End If
    End Sub
    Private Sub Application_ThreadException(ByVal sender As Object, ByVal e As ThreadExceptionEventArgs)
        ContErrores = 5
        LogERR("Error Crash Tread: reiniciando con hora " & Now & " Error: " & e.Exception.Message & " / " & e.Exception.TargetSite.ToString)
        If ComandoEjecutado = False Then
            ComandoEjecutado = True
            lazaro()
        End If
    End Sub
    Private Sub CurrentDomain_UnhandledException(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        'Dim ex As Exception = DirectCast(e.ExceptionObject, Exception)
        ContErrores = 5
        LogERR("Error Crash Exep: reiniciando con hora " & Now & " Error: " & e.ExceptionObject.Message & " / " & e.ExceptionObject.TargetSite.ToString)
        If ComandoEjecutado = False Then
            ComandoEjecutado = True
            lazaro()
        End If
    End Sub

    Public Sub lazaro()

        'Trace.WriteLine("lazaro 1")
        Dim path_batch As String = "C:\Scantech\batch_counter.bat"
        Dim fs As FileStream
        If Not IO.File.Exists(path_batch) Then
            fs = IO.File.Create(path_batch)
        Else
            fs = IO.File.Open(path_batch, FileMode.Open)
        End If
        'Trace.WriteLine("lazaro 2")
        Dim path_batch_txt As String = "@echo off
                                        taskkill /f /im SC100PLUS.exe
                                        timeout /t 10 /nobreak >nul
                                        start """" ""C:\Scantech\ScanCounter.exe"" "
        'se amplio a 10 segundos por un error que al crashear este volvia a  caer soamente para poder caerse otravez
        Using writer As New StreamWriter(fs)
            writer.Write(path_batch_txt)
        End Using
        'Trace.WriteLine("lazaro 3")
        'Me.Close() ' cerrar la aplicacion por que si
        ejecutarComando(path_batch) ' eliminar la aplicacion de inmediato para que no se siga repitiendo
        Dim currPrsName As String = Process.GetCurrentProcess().ProcessName
        'Trace.WriteLine("lazaro 4")


        'descomentar esto para cuando no este debugueando
        'Dim allProcessWithThisName() As Process = Process.GetProcessesByName(currPrsName)
        'For Each proc As Process In allProcessWithThisName
        '    proc.Kill()
        'Next

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
        'hay que  hacer que este filtro para el ardbox
        'inicio spartan 21
        'patronar regex con lo envviado
        'Dim Caracter As Char
        'Try
        '    Dim regexIO As New Regex("^(([IQ])[0-1]_(1[0-2]|[0-9]))$")
        '    If regexIO.IsMatch(Valor) Then
        '        Dim ValorFinal As Integer = 0
        '        Dim Largo = 2
        '        'constantes
        '        Dim BaseIs As Integer = 65
        '        Dim BaseQs As Integer = BaseIs + 13
        '        If Valor.Length <= 4 Then
        '            Largo -= 1
        '        End If

        '        ValorFinal = CInt(Valor.Substring(3, Largo))
        '        If Valor.Contains("Q") Then
        '            ValorFinal += BaseQs
        '        ElseIf Valor.Contains("I") Then
        '            ValorFinal += BaseIs
        '        End If

        '        If ValorFinal >= 65 And ValorFinal <= 85 Then
        '            Caracter = ChrW(ValorFinal)
        '            'Trace.WriteLine("caracter obtenido por filtro  = " & Caracter)
        '        Else
        '            Caracter = Nothing
        '        End If
        '    Else
        '        Caracter = Nothing
        '    End If
        'Catch ex As Exception
        '    LogERR($"Error FormPrincipal Model 601: {ex.Message}")
        '    'Trace.WriteLine("error en la asignacion de variable normal" & ex.Message)
        '    Caracter = Nothing
        'End Try
        'Return Caracter
        'spartan  21
        'ardbox  inicio
        Dim Caracter As Char
        Try
            Dim regexIO As New Regex("^(([IQ])[0-1]_([0-9]))$")
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
            LogERR("Error FormPrincipal 777:" & ex.Message)
            'Trace.WriteLine("error en la asignacion de variable normal" & ex.Message)
            Caracter = Nothing
        End Try
        Return Caracter
        'ardbox fin
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
                EnviaCaracterArduino(Paso1)
                Thread.Sleep(300)
                Paso2 = FiltroLetraEntradaSalida(valor)
                If Paso2 IsNot Nothing Then
                    EnviaCaracterArduino(Paso2)
                    Thread.Sleep(300)
                    Paso3 = "0"
                    EnviaCaracterArduino(Paso3)
                End If
            End If
        Catch ex As Exception
            LogERR("Error FormPrincipal 101: " & ex.Message)
            'Trace.WriteLine("Error envaindo cambio pines detalles: " & ex.Message)
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

        'peticion batch
        If PeticionBatch = 1 Then
            If ProcesandoSolicitudBatch = False Then ' se hace para que no procese mas de 1 vez
                'solo se livera le procesado cuando esta en estado 2 o 0
                ProcesandoSolicitudBatch = True
                Trace.WriteLine("procesando solicitud  desde el backend  batch")
                CrearBatch(IdEquipo1, NombreEquipo1)
            End If
        End If

        If BatchDeSolicitudInsertado Then
            Configuraciones.SolicitudBatch = 2 ' activa la solicitud
            Dim x = Configuraciones.CambiarEstadBatch()
            If x = 1 Then
                BatchDeSolicitudInsertado = False ' se reinicia todo
            End If

        End If




        'cambio de paleta con backend
        If CambioPaletaDesdeBack Then

            If EstadoPaleta = 1 Then
                Trace.WriteLine("abriendo paleta")
                EnviaCaracterArduino("k")
                CambioPaletaDesdeBack = False
            Else
                Trace.WriteLine("cerrando paleta")
                EnviaCaracterArduino("j")
                CambioPaletaDesdeBack = False
            End If
        End If


        If conexionDb Then
            'Private EstadoPaleta As Boolean
            'Private PrimeraAsignacionPaleta As Boolean = False
            If CambioPaletaDesdeFront Then
                AsignarPrimerEstadoPaleta() ' se asigna como el acutal ya que 
                'si se presiono el boton de backend se debe cambiar o acutalizar en el fornt end
            End If

            If EstadoPaletaAsingadaPlc And PrimeraAsignacionPaleta = False Then
                'intentar  asignar en base de datos el valor sin importar su estado desde ella, luego de eso  recibir actualizaciones
                'puede inclusive insertar el primer estado en el que la base de datos
                'ya de por si representaba
                'CambiarPaleta()
                AsignarPrimerEstadoPaleta()
            End If

            HiloSensores = New Thread(AddressOf ObtenerSensores)
            HiloSensores.Start()
        End If
        Dim resp As Boolean = False
        Try
            TimerSensores.Stop() 'hasta que se cambien  o si detectan alguncambio que lo hacan y que no aga otro por detras
            'mientras tengan datos entonces que compare
            If Sensor1AltDatatable IsNot Nothing And Sensor2AltDatatable IsNot Nothing Then
                'cuando se levante la primera vez, como puedo saber si el pin dado existe,
                'solamente cuando tenga conexion con la base de datos y la no se interrumpa
                'Trace.WriteLine("se1")
                'ImprimeDatatable(Sensor1AltDatatable)
                If Sensor1AltDatatable.Columns.Count > 1 Then
                    resp = verificacionAtrib(EntradaSensor1, Sensor1AltDatatable, 3)
                    If resp Then
                        EnviarCambioPines(EntradaSensor1, 0)
                        resp = False
                    End If
                End If
                'Trace.WriteLine("se2")
                If Sensor2AltDatatable.Columns.Count > 1 Then
                    resp = verificacionAtrib(EntradaSensor2, Sensor2AltDatatable, 3)
                    If resp Then
                        EnviarCambioPines(EntradaSensor2, 1)
                        resp = False
                    End If
                End If
            End If
            'Trace.WriteLine("conf2")
            If ConfiguracionesDatatable IsNot Nothing Then
                If ConfiguracionesDatatable.Columns.Count > 1 Then

                    'ImprimeDatatable(ConfiguracionesDatatable, "wenas")

                    Dim i = verificacionAtrib(SalidaSensor, ConfiguracionesDatatable, 6)
                    If i Then
                        EnviarCambioPines(SalidaSensor, 2)
                    End If
                    verificacionAtrib(TiempoEspera, ConfiguracionesDatatable, 7)
                    verificacionAtrib(LecturaMinimaProducto, ConfiguracionesDatatable, 4)
                    verificacionAtrib(LecturaMaximaProducto, ConfiguracionesDatatable, 5)
                    verificacionAtrib(COM, ConfiguracionesDatatable, PuertoIndex)
                    'Boolean con un integer
                    Dim x = verificacionAtrib(EstadoPaleta, ConfiguracionesDatatable, 8)

                    'Trace.WriteLine("estado paleta " & EstadoPaleta & " en dt = " & ConfiguracionesDatatable.Rows(0)(8))
                    If x Then
                        Trace.WriteLine("cambiado")
                        CambioPaletaDesdeBack = True
                    End If


                    verificacionAtrib(PeticionBatch, ConfiguracionesDatatable, 9)
                    If ProcesandoSolicitudBatch Then
                        If PeticionBatch = 0 Or PeticionBatch = 2 Then
                            Trace.WriteLine("proceso para liverar terminado con codigo " & PeticionBatch)
                            ProcesandoSolicitudBatch = False ' se livera lo ultimo
                        End If
                    End If
                End If
            End If
            'Trace.WriteLine("eq2")
            If Equipo1Datatable IsNot Nothing And Equipo2Datatable IsNot Nothing Then
                If Equipo1Datatable.Columns.Count > 1 Then
                    verificacionAtrib(NombreEquipo1, Equipo1Datatable, 1)
                    verificacionAtrib(LimiteBatch1, Equipo1Datatable, 2)
                End If
                If Equipo2Datatable.Columns.Count > 1 Then
                    verificacionAtrib(NombreEquipo2, Equipo2Datatable, 1)
                    verificacionAtrib(LimiteBatch2, Equipo2Datatable, 2)
                End If
            End If
        Catch ex As Exception
            LogERR("Error TimerSensores 223 " & ex.Message)
            Trace.WriteLine("Error TimerSensores 223 " & ex.Message)
        Finally
            TimerSensores.Start()
        End Try
    End Sub

    Public Function verificacionAtrib(ByRef variable As Object, ByRef datatable As DataTable, posicionDatatable As Integer) As Boolean
        Dim resp = False

        'If posicionDatatable = 8 Then
        '    'Trace.WriteLine("variable  = " & variable & " datattable = " & datatable.Rows(0)(posicionDatatable))
        'End If
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
        CambioPaletaDesdeFront = True

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        EnviaCaracterArduino("k") 'ESTADO PALETA = TRUE
        CambioPaletaDesdeFront = True
    End Sub

    Private Sub TimerJson_Tick(sender As Object, e As EventArgs) Handles TimerJson.Tick
        'verificar lo que viene sinedo escribir datos de la datatable al json como respaldo
        Try
            If conexionDb = False Then
                'Trace.WriteLine("guardar en json2")
                'si no hay conexion entonces actualiza en ambos lados solamente los datos que no fueron ingresados
                'por la base de datos, ya que mientras no exista conexion se seguiran almacenando  cada minuto o 
                '5 minutos
                Dim lecturas As DataTable
                Dim batchs As DataTable
                lecturas = ReadJson(PathLecturas)
                batchs = ReadJson(PathBatch)
                'hacer un merge de los datatables ' en batch seria revisar cada unas de las fechas
                'las lecturas puedo hacer le un gorup by por fecha solamente 
                ' Combine the two tables into one ' se combinan por fecha de inicio o por fecha de insercion ya que 
                ' son datos unicos que no pueden existir en otro tipo de registro
                Dim TablaRegistroCom

                If lecturas IsNot Nothing Then
                    If lecturas.Rows.Count > 0 And lecturas.Columns.Count > 1 Then
                        TablaRegistroCom = registrosOffline.AsEnumerable().Union(lecturas.AsEnumerable())
                    Else
                        TablaRegistroCom = registrosOffline.AsEnumerable()
                    End If

                Else
                    TablaRegistroCom = registrosOffline.AsEnumerable()
                End If
                ' Group the records by DateTime and select the first record from each group
                Dim resultRegsitros = TablaRegistroCom.GroupBy(Function(row) row.Field(Of DateTime)("fecha_insercion")).Select(Function(group) group.First())
                registrosOffline = resultRegsitros 'unidos
                WriteJson(registrosOffline, PathLecturas)
                Dim TablaBatchCom
                If batchs IsNot Nothing Then
                    If batchs.Rows.Count > 1 And batchs.Columns.Count > 1 Then
                        TablaBatchCom = BatchOffline.AsEnumerable().Union(batchs.AsEnumerable())
                    Else
                        TablaBatchCom = BatchOffline.AsEnumerable()
                    End If
                Else
                    TablaBatchCom = BatchOffline.AsEnumerable()
                End If

                ' Group the records by DateTime and select the first record from each group
                Dim resultBatch = TablaBatchCom.GroupBy(Function(row) row.Field(Of DateTime)("fecha_inicio")).Select(Function(group) group.First())
                BatchOffline = resultBatch 'unidos
                WriteJson(BatchOffline, PathBatch)
            Else
                'poner todos los registros en online
                If registrosOffline IsNot Nothing Then
                    If registrosOffline.Rows.Count > 0 Then
                        'ImprimeDatatable(registrosOffline, "wenas")
                        'Trace.WriteLine("insertando lecturas offlines")
                        Dim resp As Integer = Lecturas.InsertarLecturaOffline(registrosOffline)
                        If resp = 1 Then
                            'Trace.WriteLine("completado lec")
                            registrosOffline.Clear()
                            WriteJson(registrosOffline, PathLecturas)
                        End If
                    End If
                End If

                If BatchOffline IsNot Nothing Then
                    If BatchOffline.Rows.Count > 0 Then
                        'Trace.WriteLine("insertando batch offlines")
                        'ImprimeDatatable(BatchOffline, "desde timer json")
                        Dim resp As Integer = Batch.InsertarBatchOffline(BatchOffline)
                        If resp = 1 Then
                            'Trace.WriteLine("completado batch")
                            BatchOffline.Clear()
                            WriteJson(BatchOffline, PathBatch)
                        End If
                    End If

                End If
            End If

        Catch ex As Exception
            LogERR("Error FormPrincipal TimerJson  555:" & ex.Message)
        End Try

    End Sub

#End Region

End Class
