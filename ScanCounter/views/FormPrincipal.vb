Imports System.Net
Imports System.Xml
Imports Newtonsoft.Json.Linq
Imports RestSharp
Imports System.Drawing.Color
Imports System.Web.Management
Imports System.Data.SqlClient
Imports System.ComponentModel

Public Class FormPrincipal
    'Variables de ayuda para backgroundworker
    Private bgOpcion As String = ""
    Private bgResultado As DataTable
    Private bgwHelperResultado As Integer

    'Variables de ayuda para timer
    Private timerOpcion As String = ""

    'Instancias
    Private ReadOnly Configuraciones As ConfiguracionesModel
    Private ReadOnly Sensores As SensoresModel
    Private ReadOnly Lecturas As LecturasModel
    Private ReadOnly Wrapper As SecurityWrapper

    'Datatables
    Private ConfiguracionesDatatable As DataTable
    Private Sensor1Datatable As DataTable
    Private Sensor2Datatable As DataTable
    Private registrosOffline As New DataTable

    'Delegado para lectura desde serial port arduino
    Delegate Sub myMethodDelegate()
    Private DelegadoArduino As New myMethodDelegate(AddressOf DataArduino)
    Private DelegadoMensajes As New myMethodDelegate(AddressOf MuestraPanel)

    'Variables globales
    Private conexionDb As Boolean = False

    Private contadorLecturasSinBd As Integer = 0

    Private IdSensor1 As Byte = 3
    Private IdSensor2 As Byte = 4
    Private COM As String = ""
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
    Private LecturaMinimaProducto As Integer = 0
    Private LecturaMaximaProducto As Integer = 0
    Private MaximoAlcanzado1 As Boolean = False
    Private MaximoAlcanzado2 As Boolean = False




#Region "Constructor"
    Sub New()

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        Configuraciones = New ConfiguracionesModel
        Sensores = New SensoresModel
        Lecturas = New LecturasModel
        Wrapper = New SecurityWrapper
    End Sub
#End Region

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
        Iniciando = True
        Cursor.Hide()

        PbxLoadingSensor1.Show()
        PbxLoadingSensor1.BringToFront()

        PbxLoadingSensor2.Show()
        PbxLoadingSensor2.BringToFront()

        PbxLoading3.BringToFront()


        registrosOffline.Columns.Add("sensor", GetType(Integer))
        registrosOffline.Columns.Add("fecha", GetType(DateTime))


        bgwHelper.WorkerSupportsCancellation = True

        IniciaBackgroundworker("ValidaLicencia")
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


        Try
            conexionDb = My.Computer.Network.Ping(Configuration.Server, 100)
            If contadorLecturasSinBd > 0 Then
                'estilos debueltos
                'Trace.WriteLine("estilos debueltos")
                contadorLecturasSinBd = 0
                PanelLoadingS1.BackColor = Navy
                PanelLoadingS2.BackColor = SlateBlue
                Panel4.BackColor = SlateBlue
            End If
        Catch ex As Exception
            'contadorLecturasSinBd
            If contadorLecturasSinBd > 6 Then
                'Trace.WriteLine("sinconexion")
                PanelLoadingS1.BackColor = Gray
                PanelLoadingS2.BackColor = LightSlateGray
                Panel4.BackColor = SteelBlue
            End If
            contadorLecturasSinBd += 1
            conexionDb = False
            Trace.WriteLine("catchcone")
        Catch exql As SqlException
            'contadorLecturasSinBd
            If contadorLecturasSinBd > 6 Then
                'Trace.WriteLine("sinconexion")
                PanelLoadingS1.BackColor = Gray
                PanelLoadingS2.BackColor = LightSlateGray
                Panel4.BackColor = SteelBlue
                'Panel1.BackColor = Gray
                'Panel2.BackColor = Gray
                'Panel3.BackColor = Gray
                'Panel5.BackColor = Gray
            End If
            contadorLecturasSinBd += 1
            conexionDb = False
            Trace.WriteLine("catchconesql")
        End Try

        If conexionDb Then
            Configuraciones.Id = 2

            ConfiguracionesDatatable = Configuraciones.Listar

            If ConfiguracionesDatatable.Rows.Count > 0 And ConfiguracionesDatatable IsNot DBNull.Value And ConfiguracionesDatatable.Columns(0).ToString <> "Error" Then

                LecturaMinimaProducto = ConfiguracionesDatatable.Rows(0)(4)
                Console.WriteLine($"LecturaMinimaProducto {LecturaMinimaProducto}")
                LecturaMaximaProducto = ConfiguracionesDatatable.Rows(0)(5)
                Console.WriteLine($"LecturaMaximaProducto {LecturaMaximaProducto}")

                COM = ConfiguracionesDatatable.Rows(0)(PuertoIndex)

                Console.WriteLine($"Iniciando conexión al puerto {COM}")

                If ConnectPort(COM) Then
                    'EnviaCaracterArduino("g") 'Imprime cont1 y cont2

                    'Actualiza ultimo inicio de lecturas de ambos sensores
                    'If Sensores.ActualizarUltimoInicioLecturas Then

                    bgResultado.Rows.Add("Ok")
                    'Else
                    '    bgResultado.Rows.Add("Error3")
                    'End If
                Else
                    bgResultado.Rows.Add("Error")
                End If
            Else
                bgResultado.Rows.Add("Error1")
            End If
        Else
            bgResultado.Rows.Add("Error2")
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
    End Sub

    Function ChequeaNuevaVersionEnApi() As Boolean
        Dim client As New RestClient($"https://scantech.cl/api/productos/read_update_ruta_producto.php?id_producto={IdProducto}")

        Dim request = New RestRequest(Method.GET)
        Dim response As IRestResponse = client.Execute(request)
        Dim content As String = response.Content

        Dim result As Boolean
        If response.StatusCode = HttpStatusCode.OK Then
            Dim json As JObject = JObject.Parse(content)

            Console.WriteLine($"versión: {json.SelectToken("Producto.version")}")
            Console.WriteLine($"link: {json.SelectToken("Producto.link")}")
            Console.WriteLine($"programa: {json.SelectToken("Producto.nombre_programa")}")

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
        bgwHelperResultado = 1

        'If Not IO.File.Exists(Configuration.SourcePath) Then
        '    bgwHelperResultado = 2
        'Else
        '    Dim XmlDoc As XmlDocument = New XmlDocument()
        '    XmlDoc.Load(Configuration.SavePath)
        '    If XmlDoc.DocumentElement("encrypted_key").InnerText <> "" Then
        '        IdProducto = Wrapper.DecryptData(XmlDoc.DocumentElement("encrypted_key").InnerText)
        '        Console.WriteLine($"IdProducto {IdProducto}")

        '        ' Lee licencia segun el id del producto
        '        Dim client As New RestClient($"https://scantech.cl/api/licencias/read_by_id_producto.php?id_producto={IdProducto}")

        '        Dim request = New RestRequest(Method.GET)
        '        Dim response As IRestResponse = client.Execute(request)
        '        Dim content As String = response.Content

        '        If response.StatusCode = HttpStatusCode.OK Then
        '            Dim json As JObject = JObject.Parse(content)

        '            '¿Licencia está activa? (key_estado ACT || DIS)
        '            Console.WriteLine($"Licencia N°: {json.SelectToken("Licencia.id_licencia")}")
        '            Console.WriteLine($"Estado: {json.SelectToken("Licencia.key_estado")}")

        '            Dim result As String = json.SelectToken("Licencia.key_estado").ToString

        '            Select Case result
        '            'Case "DIS"
        '            '    bgwHelperResultado = 2
        '                Case "DES"
        '                    bgwHelperResultado = 3
        '                Case "ACT", "DIS"
        '                    Console.WriteLine($"{json.SelectToken("Licencia.validez")}")
        '                    Select Case json.SelectToken("Licencia.validez").ToString.ToUpper
        '                        Case "PRO"
        '                            bgwHelperResultado = 1
        '                        Case "STANDAR"
        '                            Dim fechaActivacion As Date = json.SelectToken("Licencia.fecha_activacion")

        '                            Dim desface As Long = DateDiff("d", fechaActivacion, Now)

        '                            If desface >= 7 Then
        '                                bgwHelperResultado = 4
        '                            End If
        '                    End Select
        '            End Select
        '        End If
        '    Else
        '        bgwHelperResultado = 2
        '    End If
        'End If
    End Sub
    Sub RutinaValidaLicencia_Completed()
        Select Case bgwHelperResultado
            Case 0
                Cursor.Show()
                MsgBox("Error (RutinaValidaLicencia 232)")
            Case 1
                IniciaBackgroundworker("Load")
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
                        'If Iniciando Then
                        Contador1(0) = Sensor1Datatable.Rows(0)(4)
                            LblContador1.Text = Contador1(0)
                            AcomodaLabel("Contador1")
                        'End If

                        LblSensor1.Text = Sensor1Datatable.Rows(0)(SensorNombreIndex)


                        LblSensor1Estado.Text = $"En estado [{Sensor1Datatable.Rows(0)(SensorNombreEstadoIndex)}]"

                        Sensor1Estado = Sensor1Datatable.Rows(0)(SensorIdEstadoIndex)

                        PbxLoadingSensor1.Hide()

                        Sensores.Id = IdSensor2

                        IniciaBackgroundworker("ListarSensor")
                    Case IdSensor2
                        'If Iniciando Then
                        Contador2(0) = Sensor2Datatable.Rows(0)(4)
                            LblContador2.Text = Contador2(0)
                            AcomodaLabel("Contador2")
                        'End If

                        LblSensor2.Text = Sensor2Datatable.Rows(0)(SensorNombreIndex)
                        LblSensor2Estado.Text = $"En estado [{Sensor2Datatable.Rows(0)(SensorNombreEstadoIndex)}]"

                        Sensor2Estado = Sensor2Datatable.Rows(0)(SensorIdEstadoIndex)

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
                If LblContador1.Text = "" Then
                    cont1 = 0
                Else
                    cont1 = CInt(LblContador1.Text)
                End If

                If LblContador2.Text = "" Then
                    cont2 = 0
                Else
                    cont2 = CInt(LblContador2.Text)
                End If
                LblTotal.Text = "" & (Contador1(0) + Contador2(0)) & " Pzs"

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
        Trace.WriteLine("terminar de listar")
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
    Async Sub RutinaInsertar(IdSensor As Integer)
        Lecturas.IdSensor = IdSensor
        Lecturas.FechaInsercion = Now

        Await InsertarAsync()

    End Sub
    Public Async Function InsertarAsync() As Task(Of Integer)
        Dim respuesta = Await Lecturas.Insertar
        If respuesta = 0 Then ' retorna con error, si este ya tiene el erro entonces ahora se debe configurar la vista
            'Error DataGridViewElement registro se debe contar
            Dim row As DataRow = registrosOffline.NewRow
            row(0) = Lecturas.IdSensor
            row(1) = Lecturas.FechaInsercion
            registrosOffline.Rows.Add(row)
        End If
        Return respuesta
    End Function
#End Region

#Region "Timer para controlar tiempo de lectura de cada producto"
    Private Sub TimerTiempoLectura_Tick(sender As Object, e As EventArgs) Handles TimerTiempoLectura1.Tick
        'If TiempoLecturaTotal1 < LecturaMaximaProducto Then
        TiempoLecturaTotal1 += TimerTiempoLectura1.Interval
        LblLectura1.Text = TiempoLecturaTotal1
        'Else
        '    TimerTiempoLectura1.Stop()
        'MaximoAlcanzado1 = True
        'End If
    End Sub
    Private Sub TimerTiempoLectura2_Tick(sender As Object, e As EventArgs) Handles TimerTiempoLectura2.Tick
        'If TiempoLecturaTotal2 < LecturaMaximaProducto Then
        TiempoLecturaTotal2 += TimerTiempoLectura2.Interval
        LblLectura2.Text = TiempoLecturaTotal2
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

        End Try
    End Sub
#End Region

#Region "Delegado para puerto serial y función DataReceived de SerialPort"
    Sub DataArduino()
        Try
            'Lee desde arduino
            Dim lectura As String = SerialPort1.ReadExisting

            'Console.WriteLine($"{lectura}")
            If Not String.IsNullOrWhiteSpace(lectura) Then
                lectura = lectura.Trim
                'Trace.WriteLine("lectura:" & lectura)
                Select Case lectura
                    Case "A" 'Sensor 1 - I0_0
                        ' Sensor dejó de leer (I0_0 en 0V)
                        TimerTiempoLectura1.Stop()
                        'Trace.WriteLine("lec min " & LecturaMinimaProducto & " / lect total " & TiempoLecturaTotal1)
                        'If TiempoLecturaTotal1 >= LecturaMinimaProducto Then ' este formulario no existe
                        'Trace.WriteLine("l")
                        Select Case Sensor1Estado
                            Case 1
                                'Trace.WriteLine("l")
                                RutinaInsertar(IdSensor1)
                        End Select
                        Trace.WriteLine(conexionDb)

                        'End If

                        TiempoLecturaTotal1 = 0
                    Case "Z"
                        ' Sensor se encuentra leyendo (I0_0 en 24V)
                        TimerTiempoLectura1.Enabled = True
                        TimerTiempoLectura1.Start()

                    Case "B" 'Sensor 2 - I0_1
                        ' Sensor dejó de leer (I0_1 en 0V)
                        TimerTiempoLectura2.Stop()
                        'If TiempoLecturaTotal2 >= LecturaMinimaProducto Then
                        Select Case Sensor2Estado
                            Case 1
                                RutinaInsertar(IdSensor2)
                        End Select
                        'End If

                        TiempoLecturaTotal2 = 0
                    Case "Y"
                        TimerTiempoLectura2.Enabled = True
                        TimerTiempoLectura2.Start()
                End Select

                LblTotal.Text = $"{CInt(LblContador1.Text) + CInt(LblContador2.Text)} Pzs"
            Else
                MuestraMensaje("Error 332", 2)
                'Console.WriteLine("lectura desde SerialPort con error (332)")
            End If

            'Mueve texto del contador para centrarlo
            AcomodaLabel("Contador1")
            AcomodaLabel("Contador2")
            AcomodaLabel("ContadorTotal")
        Catch ex As Exception
            MuestraMensaje("Error 552", 2)
        End Try
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
            MuestraMensaje($"Error 353", 2)
            'Console.WriteLine($"Proceso aún en marcha")
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
        If SerialPort1.IsOpen Then
            Try
                SerialPort1.WriteTimeout = 3000
                SerialPort1.WriteLine(Caracter)
            Catch ex As Exception
                DisconnectPort()
                MuestraMensaje($"Error 374", 2)
                'Console.WriteLine("Serialport is with error, system will reboot")
            End Try
        Else
            MuestraMensaje("Error 378", 2)
            'Console.WriteLine("Serialport is closed")
        End If
    End Sub

    Sub MuestraMensaje(Mensaje As String, Tipo As Byte)
        'Console.WriteLine($"Mensaje {Mensaje}")
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

    'Private Sub bgwListar_DoWork(sender As Object, e As DoWorkEventArgs) Handles bgwListar.DoWork

    'End Sub

    'Private Sub bgwListar_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles bgwListar.RunWorkerCompleted
    '    If Not bgwListar.IsBusy Then

    '    End If

    'End Sub




    'Private Sub TimerUpdater_Tick(sender As Object, e As EventArgs) Handles TimerUpdater.Tick
    '    Console.WriteLine($"Iniciando ScanUpdater")
    '    EjecutarComando("C:\Scantech\ScanUpdater.exe")

    '    Close()
    'End Sub
#End Region

End Class
