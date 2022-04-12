Public Class FormPrincipal
    'Variables de ayuda para backgroundworker
    Private bgOpcion As String = ""
    Private bgResultado As DataTable

    'Variables de ayuda para timer
    Private timerOpcion As String = ""

    'Instancias
    Private ReadOnly Configuraciones As ConfiguracionesModel
    Private ReadOnly Sensores As SensoresModel
    Private ReadOnly Lecturas As LecturasModel

    'Datatables
    Private ConfiguracionesDatatable As DataTable
    Private Sensor1Datatable As DataTable
    Private Sensor2Datatable As DataTable

    'Delegado para lectura desde serial port arduino
    Delegate Sub myMethodDelegate()
    Private DelegadoArduino As New myMethodDelegate(AddressOf DataArduino)

    'Variables globales
    Private PuertoIndex As Byte = 1 'Index para consulta sql
    Private Contador1 As Integer = 0 'cuenta sensor 1 (identificador A desde arduino)
    Private Contador2 As Integer = 0 'cuenta sensor 2 (identificador B desde arduino)
    Private Sensor1Estado As Byte = 2 'maneja estado del sensor 1 para actualizar variable sensor1_Estado en arduino (1: iniciado, 2: detenido, 3:reset)
    Private Sensor2Estado As Byte = 2 'maneja estado del sensor 2 para actualizar variable sensor1_Estado en arduino (1: iniciado, 2: detenido, 3:reset)
    Private SensorNombreIndex As Byte = 1 'index según consulta sql
    Private SensorIdEstadoIndex As Byte = 2 'index según consulta sql
    Private SensorNombreEstadoIndex As Byte = 3 'index según consulta sql
    Private TimerEstado As Boolean = False 'true: timer ya inició, false: timer aún no inicia

#Region "Constructor"
    Sub New()

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        Configuraciones = New ConfiguracionesModel
        Sensores = New SensoresModel
        Lecturas = New LecturasModel
    End Sub
#End Region

#Region "Backgroundworker Helper"
    Private Sub bgwHelper_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgwHelper.DoWork
        Select Case bgOpcion
            Case "Load"
                RutinaLoad()
            Case "ListarSensor"
                RutinaListarSensor()
        End Select
    End Sub
    Private Sub bgwHelper_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwHelper.RunWorkerCompleted
        Select Case bgOpcion
            Case "Load"
                RutinaLoad_Completed()
            Case "ListarSensor"
                RutinaListarSensor_Completed()
        End Select
    End Sub
#End Region

#Region "Load"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PbxLoadingSensor1.Show()
        PbxLoadingSensor1.BringToFront()

        PbxLoadingSensor2.Show()
        PbxLoadingSensor2.BringToFront()

        PbxLoading3.BringToFront()

        bgwHelper.WorkerSupportsCancellation = True

        IniciaBackgroundworker("Load")
    End Sub

    Sub RutinaLoad()
        bgResultado = New DataTable
        bgResultado.Columns.Add("Result")

        If My.Computer.Network.Ping(Configuration.Server, 3000) Then
            ConfiguracionesDatatable = Configuraciones.Listar

            If ConfiguracionesDatatable.Rows.Count > 0 And ConfiguracionesDatatable IsNot DBNull.Value And ConfiguracionesDatatable.Columns(0).ToString <> "Error" Then
                If ConnectPort(ConfiguracionesDatatable.Rows(0)(PuertoIndex)) Then
                    EnviaCaracterArduino("g") 'Imprime cont1 y cont2

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

                TimerEstado = False
                TimerHelper.Stop()

                Sensores.Id = 1

                IniciaBackgroundworker("ListarSensor")
            Case "Error1"
                PbxComStatus.Image = My.Resources.red_dot
                PbxNetworkStatus.Hide()

                If Not TimerEstado Then
                    TimerEstado = True
                    IniciaTimer("Load")
                End If

                MuestraMensaje("Error 127")
                Console.WriteLine("No hay datos en configuraciones")
            Case "Error"
                PbxComStatus.Image = My.Resources.red_dot
                PbxNetworkStatus.Hide()

                If Not TimerEstado Then
                    TimerEstado = True
                    IniciaTimer("Load")
                End If

                MuestraMensaje("Error 136")
                Console.WriteLine("Error al intentar conectar el serialport")
            Case "Error2"
                PbxComStatus.Image = My.Resources.red_dot
                PbxNetworkStatus.Hide()

                If Not TimerEstado Then
                    TimerEstado = True
                    IniciaTimer("Load")
                End If

                MuestraMensaje("Error 145")
                Console.WriteLine("Sin conexión al server")
                'Case "Error3"
                '    PbxComStatus.Image = My.Resources.red_dot
                '    PbxNetworkStatus.Hide()

                '    If Not TimerEstado Then
                '        TimerEstado = True
                '        IniciaTimer("Load")
                '    End If

                '    MuestraMensaje("Error 156")
                '    Console.WriteLine("No se pudo actualizar ultimo_inicio_lecturas")
        End Select
    End Sub
#End Region

#Region "Listar sensores"
    Sub RutinaListarSensor()
        bgResultado.Rows.Clear()

        Select Case Sensores.Id
            Case 1
                Sensor1Datatable = Sensores.Listar

                If Sensor1Datatable.Rows.Count > 0 And Sensor1Datatable IsNot Nothing Then
                    bgResultado.Rows.Add("Ok")
                Else
                    bgResultado.Rows.Add("Error1")
                End If
            Case 2
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
                    Case 1
                        LblSensor1.Text = Sensor1Datatable.Rows(0)(SensorNombreIndex)
                        LblSensor1Estado.Text = $"En estado [{Sensor1Datatable.Rows(0)(SensorNombreEstadoIndex)}]"

                        Sensor1Estado = Sensor1Datatable.Rows(0)(SensorIdEstadoIndex)

                        PbxLoadingSensor1.Hide()

                        Sensores.Id = 2

                        IniciaBackgroundworker("ListarSensor")
                    Case 2
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
                                    EnviaCaracterArduino("c") 'Reset de contador y lo pone en pausa para no realizar conteo

                                    LblContador1.Text = "0"
                                    LblContador1.Location = New Point(340, 220)
                            End Select

                            Select Case Sensor2Estado
                                Case 1
                                    EnviaCaracterArduino("d") 'sensor2_Estado = 1 (Comienza a aumentar contador)
                                Case 2
                                    EnviaCaracterArduino("e") 'sensor2_Estado = 2 (Sólo imprime contador (en pausa))
                                Case 3
                                    EnviaCaracterArduino("f") 'Reset de contador y lo pone en pausa para no realizar conteo

                                    LblContador2.Text = "0"
                                    LblContador2.Location = New Point(380, 220)
                            End Select
                        End If
                End Select
            Case "Error1"
                MuestraMensaje("Error 220")
                Console.WriteLine("Error listando sensor 1")
            Case "Error2"
                MuestraMensaje("Error 223")
                Console.WriteLine("Error listando sensor 2")
        End Select
    End Sub
#End Region

#Region "Timer Helper"
    Private Sub TimerListarSensorEstado_Tick(sender As Object, e As EventArgs) Handles TimerHelper.Tick
        Select Case timerOpcion
            Case "ListarSensor"
                Sensores.Id = 1

                IniciaBackgroundworker("ListarSensor")
            Case "Load"
                IniciaBackgroundworker("Load")
        End Select
    End Sub

#End Region

#Region "Insertar lectura"
    Public Async Function InsertarAsync() As Task
        Await Lecturas.Insertar
    End Function
    Async Sub RutinaInsertar(IdSensor As Integer)
        Lecturas.IdSensor = IdSensor
        Lecturas.FechaInsercion = Now

        Await InsertarAsync()
    End Sub
#End Region

#Region "Arduino"
    Private Function ConnectPort(puerto As String) As Boolean
        Dim result As Boolean = False

        Try
            With SerialPort1
                .PortName = puerto
                .BaudRate = 9600
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
        'Lee desde arduino
        Dim lectura As String = SerialPort1.ReadLine

        'Incoming data
        'A:15
        'B:2
        Console.WriteLine($"{lectura}")
        If Not String.IsNullOrWhiteSpace(lectura) Then
            lectura = lectura.Trim

            Select Case lectura.Substring(0, 1)
                Case "A" 'Sensor 1
                    LblContador1.Text = lectura.Split(":")(1)

                    Select Case Sensor1Estado
                        Case 1
                            RutinaInsertar(1)
                    End Select

                Case "B" 'Sensor 2
                    LblContador2.Text = lectura.Split(":")(1)

                    Select Case Sensor2Estado
                        Case 1
                            RutinaInsertar(2)
                    End Select
                Case Else
                    MuestraMensaje("Error (326)")
                    Console.WriteLine("identificador del sensor desde plc desconocido (268)")
            End Select

            LblTotal.Text = $"{CInt(LblContador1.Text) + CInt(LblContador2.Text)} Pzs"
        Else
            MuestraMensaje("Error 332")
            Console.WriteLine("lectura desde SerialPort con error (273)")
        End If



        'Mueve texto del contador para centrarlo
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

    End Sub

    Private Sub SerialPort_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        Invoke(DelegadoArduino)
    End Sub
#End Region

#Region "Funciones"
    Sub IniciaBackgroundworker(Opcion As String)
        bgOpcion = Opcion

        If bgwHelper.IsBusy Then
            MuestraMensaje($"Error 353")
            Console.WriteLine($"Proceso aún en marcha")
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
                MuestraMensaje($"Error 374")
                Console.WriteLine("Serialport is with error, system will reboot")
            End Try
        Else
            MuestraMensaje("Error 378")
            Console.WriteLine("Serialport is closed")
        End If
    End Sub

    Sub MuestraMensaje(Mensaje As String)
        LblError.Text = Mensaje
        PanelError.Show()
        TimerMensaje.Start()
    End Sub

    Private Sub TimerMensaje_Tick(sender As Object, e As EventArgs) Handles TimerMensaje.Tick
        PanelError.Hide()
    End Sub
#End Region

End Class
