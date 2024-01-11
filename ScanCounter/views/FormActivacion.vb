Imports RestSharp
Imports System.Net
Imports Newtonsoft.Json.Linq
Imports System.Security.Cryptography
Imports System.Xml

Public Class FormActivacion
    Private ReadOnly Wrapper As SecurityWrapper

    Private IdProducto As String = ""
    Private Serial As String = ""

    Private bgwOpcion As String = ""
    Private bgwResultado As Integer = 0

#Region "Constructor"
    Sub New()

        ' Esta llamada es exigida por el diseñador.
        InitializeComponent()

        ' Agregue cualquier inicialización después de la llamada a InitializeComponent().
        Wrapper = New SecurityWrapper
    End Sub
#End Region

#Region "Load"
    Private Sub FormActivacion_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
#End Region

#Region "BackgroundWorker"
    Private Sub bgwHelper_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgwHelper.DoWork
        Select Case bgwOpcion
            Case "Activar"
                RutinaActivar()
        End Select
    End Sub

    Private Sub bgwHelper_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles bgwHelper.RunWorkerCompleted
        Select Case bgwOpcion
            Case "Activar"
                RutinaActivar_Completed()
        End Select
    End Sub
#End Region

#Region "Rutina activación"
    Private Sub BtnActivar_Click(sender As Object, e As EventArgs) Handles BtnActivar.Click
        PbxLoading.Show()
        PbxLoading.BringToFront()

        Serial = TxtActivarProducto.Text

        IniciaBgwHelper("Activar")
    End Sub

    Sub RutinaActivar()
        Dim claveEncriptada As String = MD5EncryptPass(Serial)

        'Busca información de licencia según serial encriptado
        Dim client As New RestClient("http://scantech.cl/api/licencias/read_licence.php?key_certificado=" & claveEncriptada)

        Dim request = New RestRequest(Method.GET)
        Dim response As IRestResponse = client.Execute(request)
        Dim content As String = response.Content

        If response.StatusCode = HttpStatusCode.OK Then
            Dim json As JObject = JObject.Parse(content)

            Select Case json.SelectToken("Licencia.key_estado")
                Case "DIS"
                    'Chequea que total de equipos activos sea menor al disponible
                    If CInt(json.SelectToken("Licencia.total_activo")) < CInt(json.SelectToken("Licencia.total_equipo")) Then

                        'Obtiene listado de codigos de productos asignados a esta licencia
                        client = New RestClient($"http://scantech.cl/api/productos/read_products_by_licence.php?id_licencia={json.SelectToken("Licencia.id_licencia")}")

                        request = New RestRequest(Method.GET)
                        response = client.Execute(request)
                        content = response.Content

                        If response.StatusCode = HttpStatusCode.OK Then
                            'Listado de productos en objeto json, obtiene primer codigo de producto disponible (activado = 0)
                            Dim json2 As JObject = JObject.Parse(content)

                            Console.WriteLine($"{json2.SelectToken("Productos.0.id_producto")}")
                            IdProducto = json2.SelectToken("Productos.0.id_producto")

                            'Actualiza licencia en producto
                            client = New RestClient($"http://scantech.cl/api/productos/update_producto_licencia.php?id_producto={IdProducto}&id_licencia={json.SelectToken("Licencia.id_licencia")}")

                            request = New RestRequest(Method.GET)
                            response = client.Execute(request)

                            If response.StatusCode = HttpStatusCode.OK Then

                                'Actualiza total de equipos activos
                                client = New RestClient($"http://scantech.cl/api/licencias/update_total_activo.php?id_licencia={json.SelectToken("Licencia.id_licencia")}")

                                request = New RestRequest(Method.GET)
                                response = client.Execute(request)

                                If response.StatusCode = HttpStatusCode.OK Then
                                    If CInt(json.SelectToken("Licencia.total_activo")) + 1 = CInt(json.SelectToken("Licencia.total_equipo")) Then

                                        ' Actualiza estado de licencia (ACT)
                                        client = New RestClient($"http://scantech.cl/api/licencias/update_key_estado.php?id_licencia={json.SelectToken("Licencia.id_licencia")}")

                                        request = New RestRequest(Method.GET)
                                        response = client.Execute(request)

                                        If response.StatusCode = HttpStatusCode.OK Then
                                            bgwResultado = 1
                                        Else
                                            bgwResultado = 9
                                        End If
                                    Else
                                        bgwResultado = 1
                                    End If
                                Else
                                    bgwResultado = 8
                                End If
                            Else
                                bgwResultado = 5
                            End If
                        Else
                            bgwResultado = 7
                        End If
                    Else
                        bgwResultado = 6
                    End If
                Case "DES"
                    bgwResultado = 3
                Case "ACT"
                    bgwResultado = 4
            End Select
        Else
            bgwResultado = 2
        End If
    End Sub
    Sub RutinaActivar_Completed()
        Select Case bgwResultado
            Case 0
                MsgBox("Error (RutinaActivar 42)")
            Case 1
                CrearXML()

                ' Guarda en XML
                Dim XmlDoc As XmlDocument = New XmlDocument()

                XmlDoc.Load(Configuration.SourcePath)
                XmlDoc.DocumentElement("encrypted_key").InnerText = Wrapper.EncryptData(IdProducto)
                XmlDoc.Save(Configuration.SourcePath)

                MsgBox($"Su producto {IdProducto} ha sido activado correctamente")

                Application.Restart()
            Case 2
                MsgBox("Licencia inválida, por favor contacte con soporte@scantech.cl")
            Case 3
                MsgBox("Licencia desactivada, por favor contacte a soporte@scantech.cl")
            Case 4
                MsgBox("Licencia en uso. Por favor contacte con soporte@scantech.cl")
            Case 5
                MsgBox("Error (bgwResultado 5)")
            Case 6
                MsgBox("Limite de equipos alcanzado para su licencia. Por favor contacte con soporte@scantech.cl")
            Case 7
                MsgBox("Error (bgwResultado 7)")
            Case 8
                MsgBox("Error (bgwResultado 8)")
            Case 9
                MsgBox("Error (bgwResultado 9)")
        End Select
        PbxLoading.Hide()
    End Sub
#End Region

#Region "Funciones"
    Sub IniciaBgwHelper(Opcion As String)
        bgwOpcion = Opcion
        If BgwHelper.IsBusy Then
            MsgBox("Proceso aún en marcha, intente nuevamente")
        Else
            BgwHelper.RunWorkerAsync()
        End If
    End Sub

    Private Function MD5EncryptPass(ByVal StrPass As String) As String
        Dim result As String = ""
        Dim md5 As New MD5CryptoServiceProvider
        Dim bytValue() As Byte
        Dim bytHash() As Byte
        Dim i As Integer

        bytValue = System.Text.Encoding.UTF8.GetBytes(StrPass)

        bytHash = md5.ComputeHash(bytValue)
        md5.Clear()

        For i = 0 To bytHash.Length - 1
            result &= bytHash(i).ToString("x").PadLeft(2, "0")
        Next

        Return result
    End Function

    ' XMl
    Sub CrearXML()
        If IO.Directory.Exists(Configuration.SaveDirectory) Then 'Chequea carpeta
            If IO.File.Exists(Configuration.SavePath) Then 'Chequea archivo
                Dim XmlDoc As XmlDocument = New XmlDocument()

                XmlDoc.Load(Configuration.SavePath)
            Else
                Dim writer As New XmlTextWriter(Configuration.SavePath, System.Text.Encoding.UTF8)
                writer.WriteStartDocument(True)
                writer.Formatting = Formatting.Indented
                writer.Indentation = 2
                writer.WriteStartElement("data")
                createNode("encrypted_key", 0, writer)
                writer.WriteEndElement()
                writer.WriteEndDocument()
                writer.Close()
            End If
        Else
            IO.Directory.CreateDirectory(Configuration.SaveDirectory)

            If IO.File.Exists(Configuration.SavePath) Then 'Chequea archivo
                Dim XmlDoc As XmlDocument = New XmlDocument()

                XmlDoc.Load(Configuration.SavePath)
            Else
                Dim writer As New XmlTextWriter(Configuration.SavePath, System.Text.Encoding.UTF8)
                writer.WriteStartDocument(True)
                writer.Formatting = Formatting.Indented
                writer.Indentation = 2
                writer.WriteStartElement("data")
                createNode("encrypted_key", 0, writer)
                writer.WriteEndElement()
                writer.WriteEndDocument()
                writer.Close()
            End If
        End If
    End Sub
    Private Sub createNode(name As String, value As String, writer As XmlTextWriter)
        writer.WriteStartElement(name)
        writer.WriteString(value)
        writer.WriteEndElement()
    End Sub

#End Region

End Class