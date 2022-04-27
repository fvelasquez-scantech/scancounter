<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormActivacion
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormActivacion))
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.BtnActivar = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TxtActivarProducto = New System.Windows.Forms.TextBox()
        Me.PbxLoading = New System.Windows.Forms.PictureBox()
        Me.BgwHelper = New System.ComponentModel.BackgroundWorker()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        CType(Me.PbxLoading, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.MidnightBlue
        Me.Panel1.Controls.Add(Me.Label1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Panel1.Location = New System.Drawing.Point(0, 0)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(633, 93)
        Me.Panel1.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Location = New System.Drawing.Point(141, 35)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(351, 22)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Su software aún no ha sido activado."
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.Label4)
        Me.Panel2.Controls.Add(Me.BtnActivar)
        Me.Panel2.Controls.Add(Me.Label2)
        Me.Panel2.Controls.Add(Me.TxtActivarProducto)
        Me.Panel2.Controls.Add(Me.PbxLoading)
        Me.Panel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel2.Location = New System.Drawing.Point(0, 93)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(633, 286)
        Me.Panel2.TabIndex = 1
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.ForeColor = System.Drawing.Color.Navy
        Me.Label4.Location = New System.Drawing.Point(28, 24)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(577, 44)
        Me.Label4.TabIndex = 19
        Me.Label4.Text = "Puede encontrar la clave de su producto en la licencia digital " & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "entregada por Sc" &
    "antech"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'BtnActivar
        '
        Me.BtnActivar.BackColor = System.Drawing.Color.MidnightBlue
        Me.BtnActivar.FlatAppearance.BorderSize = 0
        Me.BtnActivar.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnActivar.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.BtnActivar.ForeColor = System.Drawing.Color.White
        Me.BtnActivar.Location = New System.Drawing.Point(231, 227)
        Me.BtnActivar.Name = "BtnActivar"
        Me.BtnActivar.Size = New System.Drawing.Size(167, 35)
        Me.BtnActivar.TabIndex = 18
        Me.BtnActivar.Text = "Activar ahora"
        Me.BtnActivar.UseVisualStyleBackColor = False
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(170, 114)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(85, 17)
        Me.Label2.TabIndex = 17
        Me.Label2.Text = "Licence key"
        '
        'TxtActivarProducto
        '
        Me.TxtActivarProducto.Font = New System.Drawing.Font("Arial", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TxtActivarProducto.Location = New System.Drawing.Point(173, 134)
        Me.TxtActivarProducto.Name = "TxtActivarProducto"
        Me.TxtActivarProducto.Size = New System.Drawing.Size(292, 25)
        Me.TxtActivarProducto.TabIndex = 16
        '
        'PbxLoading
        '
        Me.PbxLoading.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PbxLoading.Image = Global.ScanCounter.My.Resources.Resources.loading
        Me.PbxLoading.Location = New System.Drawing.Point(0, 0)
        Me.PbxLoading.Name = "PbxLoading"
        Me.PbxLoading.Size = New System.Drawing.Size(633, 286)
        Me.PbxLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.PbxLoading.TabIndex = 20
        Me.PbxLoading.TabStop = False
        Me.PbxLoading.Visible = False
        '
        'FormActivacion
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(633, 379)
        Me.ControlBox = False
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormActivacion"
        Me.Text = "Activación de ScanCounter"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        CType(Me.PbxLoading, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Panel1 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Label4 As Label
    Friend WithEvents BtnActivar As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents TxtActivarProducto As TextBox
    Friend WithEvents PbxLoading As PictureBox
    Friend WithEvents BgwHelper As System.ComponentModel.BackgroundWorker
End Class
