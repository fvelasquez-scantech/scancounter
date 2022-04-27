<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormAutoUpdater
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormAutoUpdater))
        Me.bgwHelper = New System.ComponentModel.BackgroundWorker()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ActualizarScanProcessToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Lblnfo = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.LblProgresoDescarga = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'bgwHelper
        '
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ActualizarScanProcessToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(195, 26)
        '
        'ActualizarScanProcessToolStripMenuItem
        '
        Me.ActualizarScanProcessToolStripMenuItem.Name = "ActualizarScanProcessToolStripMenuItem"
        Me.ActualizarScanProcessToolStripMenuItem.Size = New System.Drawing.Size(194, 22)
        Me.ActualizarScanProcessToolStripMenuItem.Text = "Actualizar ScanProcess"
        '
        'Lblnfo
        '
        Me.Lblnfo.AutoSize = True
        Me.Lblnfo.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Lblnfo.ForeColor = System.Drawing.Color.MidnightBlue
        Me.Lblnfo.Location = New System.Drawing.Point(142, 93)
        Me.Lblnfo.Name = "Lblnfo"
        Me.Lblnfo.Size = New System.Drawing.Size(229, 22)
        Me.Lblnfo.TabIndex = 0
        Me.Lblnfo.Text = "Actualización completada."
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.ScanUpdater.My.Resources.Resources.loading
        Me.PictureBox1.Location = New System.Drawing.Point(207, 159)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(99, 99)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'LblProgresoDescarga
        '
        Me.LblProgresoDescarga.AutoSize = True
        Me.LblProgresoDescarga.Font = New System.Drawing.Font("Arial", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblProgresoDescarga.ForeColor = System.Drawing.Color.MidnightBlue
        Me.LblProgresoDescarga.Location = New System.Drawing.Point(63, 115)
        Me.LblProgresoDescarga.Name = "LblProgresoDescarga"
        Me.LblProgresoDescarga.Size = New System.Drawing.Size(107, 22)
        Me.LblProgresoDescarga.TabIndex = 2
        Me.LblProgresoDescarga.Text = "0 de 50 mb"
        '
        'Button1
        '
        Me.Button1.BackColor = System.Drawing.Color.MidnightBlue
        Me.Button1.FlatAppearance.BorderSize = 0
        Me.Button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button1.Font = New System.Drawing.Font("Arial", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button1.ForeColor = System.Drawing.Color.White
        Me.Button1.Location = New System.Drawing.Point(172, 190)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(162, 34)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "Iniciar"
        Me.Button1.UseVisualStyleBackColor = False
        Me.Button1.Visible = False
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 5000
        '
        'FormAutoUpdater
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(507, 317)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.LblProgresoDescarga)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Lblnfo)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "FormAutoUpdater"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ScanProcessUpdater"
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents bgwHelper As System.ComponentModel.BackgroundWorker
    Friend WithEvents Lblnfo As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ActualizarScanProcessToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LblProgresoDescarga As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Timer1 As Timer
End Class
