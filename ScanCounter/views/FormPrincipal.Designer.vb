<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormPrincipal
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormPrincipal))
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.LblContador1 = New System.Windows.Forms.Label()
        Me.TimerHelper = New System.Windows.Forms.Timer(Me.components)
        Me.LblContador2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.PanelError = New System.Windows.Forms.Panel()
        Me.LblError = New System.Windows.Forms.Label()
        Me.LblVersion = New System.Windows.Forms.Label()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.PbxLoading3 = New System.Windows.Forms.PictureBox()
        Me.PbxComStatus = New System.Windows.Forms.PictureBox()
        Me.PbxNetworkStatus = New System.Windows.Forms.PictureBox()
        Me.LblTotal = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.PbxEstadoPaleta = New System.Windows.Forms.PictureBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.LblLectura1 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.LblSensor1Estado = New System.Windows.Forms.Label()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.PanelLoadingS1 = New System.Windows.Forms.Panel()
        Me.LblSensor1 = New System.Windows.Forms.Label()
        Me.PbxLoading4 = New System.Windows.Forms.PictureBox()
        Me.PbxLoadingSensor1 = New System.Windows.Forms.PictureBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.LblLectura2 = New System.Windows.Forms.Label()
        Me.LblSensor2Estado = New System.Windows.Forms.Label()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.PanelLoadingS2 = New System.Windows.Forms.Panel()
        Me.LblSensor2 = New System.Windows.Forms.Label()
        Me.PbxLoading5 = New System.Windows.Forms.PictureBox()
        Me.PbxLoadingSensor2 = New System.Windows.Forms.PictureBox()
        Me.bgwHelper = New System.ComponentModel.BackgroundWorker()
        Me.TimerMensaje = New System.Windows.Forms.Timer(Me.components)
        Me.TimerUpdater = New System.Windows.Forms.Timer(Me.components)
        Me.TimerTiempoLectura1 = New System.Windows.Forms.Timer(Me.components)
        Me.TimerTiempoLectura2 = New System.Windows.Forms.Timer(Me.components)
        Me.bgwListar = New System.ComponentModel.BackgroundWorker()
        Me.TimerRed = New System.Windows.Forms.Timer(Me.components)
        Me.TimerOffline = New System.Windows.Forms.Timer(Me.components)
        Me.TimerSensores = New System.Windows.Forms.Timer(Me.components)
        Me.TimerJson = New System.Windows.Forms.Timer(Me.components)
        Me.Panel4.SuspendLayout()
        Me.PanelError.SuspendLayout()
        Me.Panel6.SuspendLayout()
        CType(Me.PbxLoading3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PbxComStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PbxNetworkStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PbxEstadoPaleta, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.PanelLoadingS1.SuspendLayout()
        CType(Me.PbxLoading4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PbxLoadingSensor1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel2.SuspendLayout()
        Me.Panel5.SuspendLayout()
        Me.PanelLoadingS2.SuspendLayout()
        CType(Me.PbxLoading5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PbxLoadingSensor2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SerialPort1
        '
        '
        'LblContador1
        '
        Me.LblContador1.AutoSize = True
        Me.LblContador1.BackColor = System.Drawing.Color.Transparent
        Me.LblContador1.Font = New System.Drawing.Font("Arial", 100.0!, System.Drawing.FontStyle.Bold)
        Me.LblContador1.ForeColor = System.Drawing.Color.Black
        Me.LblContador1.Location = New System.Drawing.Point(358, 200)
        Me.LblContador1.Name = "LblContador1"
        Me.LblContador1.Size = New System.Drawing.Size(440, 155)
        Me.LblContador1.TabIndex = 12
        Me.LblContador1.Text = "99999"
        '
        'TimerHelper
        '
        Me.TimerHelper.Interval = 1000
        '
        'LblContador2
        '
        Me.LblContador2.AutoSize = True
        Me.LblContador2.BackColor = System.Drawing.Color.Transparent
        Me.LblContador2.Font = New System.Drawing.Font("Arial", 100.0!, System.Drawing.FontStyle.Bold)
        Me.LblContador2.ForeColor = System.Drawing.Color.Black
        Me.LblContador2.Location = New System.Drawing.Point(367, 200)
        Me.LblContador2.Name = "LblContador2"
        Me.LblContador2.Size = New System.Drawing.Size(440, 155)
        Me.LblContador2.TabIndex = 12
        Me.LblContador2.Text = "99999"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.BackColor = System.Drawing.Color.Transparent
        Me.Label3.Font = New System.Drawing.Font("Arial", 60.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.Location = New System.Drawing.Point(241, 386)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(451, 89)
        Me.Label3.TabIndex = 17
        Me.Label3.Text = "UNIDADES"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Transparent
        Me.Label4.Font = New System.Drawing.Font("Arial", 60.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Black
        Me.Label4.Location = New System.Drawing.Point(284, 386)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(451, 89)
        Me.Label4.TabIndex = 18
        Me.Label4.Text = "UNIDADES"
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.MidnightBlue
        Me.Panel4.Controls.Add(Me.PanelError)
        Me.Panel4.Controls.Add(Me.LblVersion)
        Me.Panel4.Controls.Add(Me.Panel6)
        Me.Panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel4.Location = New System.Drawing.Point(0, 358)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(1386, 430)
        Me.Panel4.TabIndex = 18
        '
        'PanelError
        '
        Me.PanelError.BackColor = System.Drawing.Color.Salmon
        Me.PanelError.Controls.Add(Me.LblError)
        Me.PanelError.Font = New System.Drawing.Font("Arial", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PanelError.Location = New System.Drawing.Point(806, 6)
        Me.PanelError.Name = "PanelError"
        Me.PanelError.Size = New System.Drawing.Size(333, 54)
        Me.PanelError.TabIndex = 25
        Me.PanelError.Visible = False
        '
        'LblError
        '
        Me.LblError.AutoSize = True
        Me.LblError.BackColor = System.Drawing.Color.Transparent
        Me.LblError.Font = New System.Drawing.Font("Arial", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblError.ForeColor = System.Drawing.Color.White
        Me.LblError.Location = New System.Drawing.Point(103, 12)
        Me.LblError.Name = "LblError"
        Me.LblError.Size = New System.Drawing.Size(124, 27)
        Me.LblError.TabIndex = 24
        Me.LblError.Text = "Error 1000"
        Me.LblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'LblVersion
        '
        Me.LblVersion.AutoSize = True
        Me.LblVersion.BackColor = System.Drawing.Color.Transparent
        Me.LblVersion.Font = New System.Drawing.Font("Arial", 15.0!, System.Drawing.FontStyle.Bold)
        Me.LblVersion.ForeColor = System.Drawing.Color.White
        Me.LblVersion.Location = New System.Drawing.Point(12, 460)
        Me.LblVersion.Name = "LblVersion"
        Me.LblVersion.Size = New System.Drawing.Size(83, 24)
        Me.LblVersion.TabIndex = 23
        Me.LblVersion.Text = "v1.0.0.0"
        '
        'Panel6
        '
        Me.Panel6.BackColor = System.Drawing.Color.MidnightBlue
        Me.Panel6.Controls.Add(Me.Label1)
        Me.Panel6.Controls.Add(Me.Button1)
        Me.Panel6.Controls.Add(Me.Button2)
        Me.Panel6.Controls.Add(Me.PbxLoading3)
        Me.Panel6.Controls.Add(Me.PbxComStatus)
        Me.Panel6.Controls.Add(Me.PbxNetworkStatus)
        Me.Panel6.Controls.Add(Me.LblTotal)
        Me.Panel6.Controls.Add(Me.Label5)
        Me.Panel6.Location = New System.Drawing.Point(0, 28)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(1891, 399)
        Me.Panel6.TabIndex = 24
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.BackColor = System.Drawing.Color.Transparent
        Me.Label1.Font = New System.Drawing.Font("Arial", 110.0!, System.Drawing.FontStyle.Bold)
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(1165, 170)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(367, 169)
        Me.Label1.TabIndex = 23
        Me.Label1.Text = " Pzs"
        '
        'PbxLoading3
        '
        Me.PbxLoading3.Location = New System.Drawing.Point(53, 77)
        Me.PbxLoading3.Name = "PbxLoading3"
        Me.PbxLoading3.Size = New System.Drawing.Size(450, 139)
        Me.PbxLoading3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.PbxLoading3.TabIndex = 22
        Me.PbxLoading3.TabStop = False
        '
        'PbxComStatus
        '
        Me.PbxComStatus.Image = Global.ScanCounter.My.Resources.Resources.red_dot
        Me.PbxComStatus.Location = New System.Drawing.Point(16, 330)
        Me.PbxComStatus.Name = "PbxComStatus"
        Me.PbxComStatus.Size = New System.Drawing.Size(60, 60)
        Me.PbxComStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PbxComStatus.TabIndex = 20
        Me.PbxComStatus.TabStop = False
        '
        'PbxNetworkStatus
        '
        Me.PbxNetworkStatus.Image = Global.ScanCounter.My.Resources.Resources.network
        Me.PbxNetworkStatus.Location = New System.Drawing.Point(82, 330)
        Me.PbxNetworkStatus.Name = "PbxNetworkStatus"
        Me.PbxNetworkStatus.Size = New System.Drawing.Size(60, 60)
        Me.PbxNetworkStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PbxNetworkStatus.TabIndex = 21
        Me.PbxNetworkStatus.TabStop = False
        '
        'LblTotal
        '
        Me.LblTotal.AutoSize = True
        Me.LblTotal.BackColor = System.Drawing.Color.Transparent
        Me.LblTotal.Font = New System.Drawing.Font("Arial", 180.0!, System.Drawing.FontStyle.Bold)
        Me.LblTotal.ForeColor = System.Drawing.Color.White
        Me.LblTotal.Location = New System.Drawing.Point(879, 108)
        Me.LblTotal.Name = "LblTotal"
        Me.LblTotal.Size = New System.Drawing.Size(780, 274)
        Me.LblTotal.TabIndex = 13
        Me.LblTotal.Text = "99999"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Transparent
        Me.Label5.Font = New System.Drawing.Font("Arial", 36.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Location = New System.Drawing.Point(766, 49)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(354, 56)
        Me.Label5.TabIndex = 19
        Me.Label5.Text = "TOTAL BATCH"
        '
        'PbxEstadoPaleta
        '
        Me.PbxEstadoPaleta.Image = Global.ScanCounter.My.Resources.Resources.PaletaAbierta
        Me.PbxEstadoPaleta.Location = New System.Drawing.Point(8, 494)
        Me.PbxEstadoPaleta.Name = "PbxEstadoPaleta"
        Me.PbxEstadoPaleta.Size = New System.Drawing.Size(85, 68)
        Me.PbxEstadoPaleta.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PbxEstadoPaleta.TabIndex = 23
        Me.PbxEstadoPaleta.TabStop = False
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Lavender
        Me.Panel1.Controls.Add(Me.LblLectura1)
        Me.Panel1.Controls.Add(Me.Label3)
        Me.Panel1.Controls.Add(Me.LblSensor1Estado)
        Me.Panel1.Controls.Add(Me.Panel3)
        Me.Panel1.Controls.Add(Me.LblContador1)
        Me.Panel1.Controls.Add(Me.PbxLoadingSensor1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Left
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(960, 358)
        Me.Panel1.TabIndex = 19
        '
        'LblLectura1
        '
        Me.LblLectura1.AutoSize = True
        Me.LblLectura1.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLectura1.Location = New System.Drawing.Point(909, 173)
        Me.LblLectura1.Name = "LblLectura1"
        Me.LblLectura1.Size = New System.Drawing.Size(18, 20)
        Me.LblLectura1.TabIndex = 25
        Me.LblLectura1.Text = "0"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(162, 338)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 27
        Me.Button1.Text = "Cerrar Paleta"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'LblSensor1Estado
        '
        Me.LblSensor1Estado.AutoSize = True
        Me.LblSensor1Estado.BackColor = System.Drawing.Color.Transparent
        Me.LblSensor1Estado.Font = New System.Drawing.Font("Arial", 21.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSensor1Estado.ForeColor = System.Drawing.Color.DimGray
        Me.LblSensor1Estado.Location = New System.Drawing.Point(330, 475)
        Me.LblSensor1Estado.Name = "LblSensor1Estado"
        Me.LblSensor1Estado.Size = New System.Drawing.Size(285, 33)
        Me.LblSensor1Estado.TabIndex = 23
        Me.LblSensor1Estado.Text = "En estado [Detenido]"
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.Navy
        Me.Panel3.Controls.Add(Me.PanelLoadingS1)
        Me.Panel3.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel3.Location = New System.Drawing.Point(0, 0)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(960, 158)
        Me.Panel3.TabIndex = 22
        '
        'PanelLoadingS1
        '
        Me.PanelLoadingS1.Controls.Add(Me.LblSensor1)
        Me.PanelLoadingS1.Controls.Add(Me.PbxLoading4)
        Me.PanelLoadingS1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelLoadingS1.Location = New System.Drawing.Point(0, 0)
        Me.PanelLoadingS1.Name = "PanelLoadingS1"
        Me.PanelLoadingS1.Size = New System.Drawing.Size(960, 158)
        Me.PanelLoadingS1.TabIndex = 24
        '
        'LblSensor1
        '
        Me.LblSensor1.AutoSize = True
        Me.LblSensor1.BackColor = System.Drawing.Color.Transparent
        Me.LblSensor1.Font = New System.Drawing.Font("Arial", 50.0!)
        Me.LblSensor1.ForeColor = System.Drawing.Color.White
        Me.LblSensor1.Location = New System.Drawing.Point(315, 28)
        Me.LblSensor1.Name = "LblSensor1"
        Me.LblSensor1.Size = New System.Drawing.Size(404, 75)
        Me.LblSensor1.TabIndex = 18
        Me.LblSensor1.Text = "Recepción 1"
        Me.LblSensor1.Visible = False
        '
        'PbxLoading4
        '
        Me.PbxLoading4.Image = Global.ScanCounter.My.Resources.Resources.waiting
        Me.PbxLoading4.Location = New System.Drawing.Point(186, 28)
        Me.PbxLoading4.Name = "PbxLoading4"
        Me.PbxLoading4.Size = New System.Drawing.Size(533, 79)
        Me.PbxLoading4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.PbxLoading4.TabIndex = 23
        Me.PbxLoading4.TabStop = False
        '
        'PbxLoadingSensor1
        '
        Me.PbxLoadingSensor1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PbxLoadingSensor1.Image = Global.ScanCounter.My.Resources.Resources.loading
        Me.PbxLoadingSensor1.Location = New System.Drawing.Point(0, 0)
        Me.PbxLoadingSensor1.Name = "PbxLoadingSensor1"
        Me.PbxLoadingSensor1.Size = New System.Drawing.Size(960, 358)
        Me.PbxLoadingSensor1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.PbxLoadingSensor1.TabIndex = 24
        Me.PbxLoadingSensor1.TabStop = False
        Me.PbxLoadingSensor1.Visible = False
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.Snow
        Me.Panel2.Controls.Add(Me.PbxEstadoPaleta)
        Me.Panel2.Controls.Add(Me.LblLectura2)
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.LblSensor2Estado)
        Me.Panel2.Controls.Add(Me.Panel5)
        Me.Panel2.Controls.Add(Me.LblContador2)
        Me.Panel2.Controls.Add(Me.PbxLoadingSensor2)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(960, 0)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(426, 358)
        Me.Panel2.TabIndex = 20
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(162, 367)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 28
        Me.Button2.Text = "Abrir"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'LblLectura2
        '
        Me.LblLectura2.AutoSize = True
        Me.LblLectura2.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblLectura2.Location = New System.Drawing.Point(32, 173)
        Me.LblLectura2.Name = "LblLectura2"
        Me.LblLectura2.Size = New System.Drawing.Size(18, 20)
        Me.LblLectura2.TabIndex = 26
        Me.LblLectura2.Text = "0"
        '
        'LblSensor2Estado
        '
        Me.LblSensor2Estado.AutoSize = True
        Me.LblSensor2Estado.BackColor = System.Drawing.Color.Transparent
        Me.LblSensor2Estado.Font = New System.Drawing.Font("Arial", 21.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblSensor2Estado.ForeColor = System.Drawing.Color.DimGray
        Me.LblSensor2Estado.Location = New System.Drawing.Point(356, 475)
        Me.LblSensor2Estado.Name = "LblSensor2Estado"
        Me.LblSensor2Estado.Size = New System.Drawing.Size(285, 33)
        Me.LblSensor2Estado.TabIndex = 24
        Me.LblSensor2Estado.Text = "En estado [Detenido]"
        '
        'Panel5
        '
        Me.Panel5.BackColor = System.Drawing.Color.SlateBlue
        Me.Panel5.Controls.Add(Me.PanelLoadingS2)
        Me.Panel5.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel5.Location = New System.Drawing.Point(0, 0)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(426, 158)
        Me.Panel5.TabIndex = 23
        '
        'PanelLoadingS2
        '
        Me.PanelLoadingS2.Controls.Add(Me.LblSensor2)
        Me.PanelLoadingS2.Controls.Add(Me.PbxLoading5)
        Me.PanelLoadingS2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelLoadingS2.Location = New System.Drawing.Point(0, 0)
        Me.PanelLoadingS2.Name = "PanelLoadingS2"
        Me.PanelLoadingS2.Size = New System.Drawing.Size(426, 158)
        Me.PanelLoadingS2.TabIndex = 25
        '
        'LblSensor2
        '
        Me.LblSensor2.AutoSize = True
        Me.LblSensor2.BackColor = System.Drawing.Color.Transparent
        Me.LblSensor2.Font = New System.Drawing.Font("Arial", 50.0!)
        Me.LblSensor2.ForeColor = System.Drawing.Color.White
        Me.LblSensor2.Location = New System.Drawing.Point(322, 32)
        Me.LblSensor2.Name = "LblSensor2"
        Me.LblSensor2.Size = New System.Drawing.Size(404, 75)
        Me.LblSensor2.TabIndex = 18
        Me.LblSensor2.Text = "Recepción 2"
        Me.LblSensor2.Visible = False
        '
        'PbxLoading5
        '
        Me.PbxLoading5.Image = Global.ScanCounter.My.Resources.Resources.waiting
        Me.PbxLoading5.Location = New System.Drawing.Point(247, 32)
        Me.PbxLoading5.Name = "PbxLoading5"
        Me.PbxLoading5.Size = New System.Drawing.Size(533, 79)
        Me.PbxLoading5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.PbxLoading5.TabIndex = 23
        Me.PbxLoading5.TabStop = False
        '
        'PbxLoadingSensor2
        '
        Me.PbxLoadingSensor2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PbxLoadingSensor2.Image = Global.ScanCounter.My.Resources.Resources.loading
        Me.PbxLoadingSensor2.Location = New System.Drawing.Point(0, 0)
        Me.PbxLoadingSensor2.Name = "PbxLoadingSensor2"
        Me.PbxLoadingSensor2.Size = New System.Drawing.Size(426, 358)
        Me.PbxLoadingSensor2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.PbxLoadingSensor2.TabIndex = 25
        Me.PbxLoadingSensor2.TabStop = False
        Me.PbxLoadingSensor2.Visible = False
        '
        'bgwHelper
        '
        '
        'TimerMensaje
        '
        Me.TimerMensaje.Interval = 1600
        '
        'TimerUpdater
        '
        Me.TimerUpdater.Enabled = True
        Me.TimerUpdater.Interval = 10000
        '
        'TimerTiempoLectura1
        '
        Me.TimerTiempoLectura1.Interval = 10
        '
        'TimerTiempoLectura2
        '
        Me.TimerTiempoLectura2.Interval = 10
        '
        'TimerRed
        '
        Me.TimerRed.Interval = 1000
        '
        'TimerOffline
        '
        Me.TimerOffline.Interval = 20000
        '
        'TimerSensores
        '
        Me.TimerSensores.Interval = 1000
        '
        'TimerJson
        '
        Me.TimerJson.Interval = 60000
        '
        'FormPrincipal
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.WhiteSmoke
        Me.ClientSize = New System.Drawing.Size(1386, 788)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.Panel4)
        Me.Cursor = System.Windows.Forms.Cursors.AppStarting
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormPrincipal"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Scancounter"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.PanelError.ResumeLayout(False)
        Me.PanelError.PerformLayout()
        Me.Panel6.ResumeLayout(False)
        Me.Panel6.PerformLayout()
        CType(Me.PbxLoading3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PbxComStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PbxNetworkStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PbxEstadoPaleta, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.PanelLoadingS1.ResumeLayout(False)
        Me.PanelLoadingS1.PerformLayout()
        CType(Me.PbxLoading4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PbxLoadingSensor1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel5.ResumeLayout(False)
        Me.PanelLoadingS2.ResumeLayout(False)
        Me.PanelLoadingS2.PerformLayout()
        CType(Me.PbxLoading5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PbxLoadingSensor2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SerialPort1 As IO.Ports.SerialPort
    Friend WithEvents LblContador1 As Label
    Friend WithEvents TimerHelper As Timer
    Friend WithEvents LblContador2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Panel4 As Panel
    Friend WithEvents Label5 As Label
    Friend WithEvents LblTotal As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents PbxComStatus As PictureBox
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Panel5 As Panel
    Friend WithEvents LblSensor2 As Label
    Friend WithEvents bgwHelper As System.ComponentModel.BackgroundWorker
    Friend WithEvents LblSensor1Estado As Label
    Friend WithEvents LblSensor2Estado As Label
    Friend WithEvents PbxLoadingSensor2 As PictureBox
    Friend WithEvents PbxNetworkStatus As PictureBox
    Friend WithEvents PbxLoading3 As PictureBox
    Friend WithEvents PanelLoadingS2 As Panel
    Friend WithEvents PbxLoading5 As PictureBox
    Friend WithEvents PanelError As Panel
    Friend WithEvents LblError As Label
    Friend WithEvents TimerMensaje As Timer
    Friend WithEvents TimerUpdater As Timer
    Friend WithEvents LblVersion As Label
    Friend WithEvents TimerTiempoLectura1 As Timer
    Friend WithEvents LblLectura1 As Label
    Friend WithEvents TimerTiempoLectura2 As Timer
    Friend WithEvents LblLectura2 As Label
    Friend WithEvents bgwListar As System.ComponentModel.BackgroundWorker
    Friend WithEvents TimerRed As Timer
    Friend WithEvents TimerOffline As Timer
    Friend WithEvents PanelLoadingS1 As Panel
    Friend WithEvents LblSensor1 As Label
    Friend WithEvents PbxLoading4 As PictureBox
    Friend WithEvents PbxLoadingSensor1 As PictureBox
    Friend WithEvents Panel6 As Panel
    Friend WithEvents TimerSensores As Timer
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents PbxEstadoPaleta As PictureBox
    Friend WithEvents Label1 As Label
    Friend WithEvents TimerJson As Timer
End Class
