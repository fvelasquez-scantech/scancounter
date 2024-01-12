Imports System.Data.SqlClient
Imports System.Diagnostics.Eventing
Imports System.Web.Caching

Public Class BatchModel


    Dim Reader As SqlDataReader
    Public Property IdEquipo As Integer
    'Public Property FechaInsercion As DateTime
    Public Property NombreEquipo As String
    Public Property FechaInicio As DateTime
    Public Property FechaTermino As DateTime
    Public Property TotalPiezas As Integer

    Private Shared ReadOnly comandoCrecionTablaBatchJson As String =
                                "CREATE TABLE ##insertar_batch_json_temp(
                                    [id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED , 
                                    [id_equipo] [int] NULL,
                                    [nombre_equipo] [Varchar](40) NULL,
                                    [fecha_inicio] [datetime] NULL,
                                    [fecha_termino] [datetime] NULL
                                   )"
    'se debera hacer un bulk copy de todos los  contadores


    Public Function InsertarBatch() As Integer
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        'Dim connection As New SqlConnection
        'Dim command As SqlCommand
        Dim da As New SqlDataAdapter
        Dim result As New DataTable

        Try
            connection.ConnectionString = Configuration.ConnectionString
            command = New SqlCommand("Batch_Insertar") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }
            command.Parameters.AddWithValue("@id_equipo", IdEquipo)
            command.Parameters.AddWithValue("@fecha", Now)
            command.Parameters.AddWithValue("@nombre_equipo", NombreEquipo)
            connection.Open()



            da.SelectCommand = command
            da.Fill(result)


            'command.ExecuteNonQuery()
            'Trace.WriteLine("wenas")
            'Return 1
            Dim resp As Integer
            resp = result.Rows(0)(0)
            'Reader = command.ExecuteReader()
            'While Reader.Read()
            '    resp = Reader.GetValue(0)
            'End While
            Return resp
        Catch ex As Exception

            Trace.WriteLine("batch model : Insertar() :" & ex.Message)
            Return 0
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function


    Public Function InsertarBatchOffline(dt As DataTable) As Integer
        Trace.WriteLine("WENAS")
        Dim connection As New SqlConnection
        Dim tbl As New DataTable
        Dim command As New SqlCommand
        Dim reader As SqlDataReader
        Dim result As New DataTable
        Dim resp As New Integer
        resp = 0


        FormPrincipal.ImprimeDatatable(dt, "tabla nomarl")
        Try

            connection.ConnectionString = Configuration.ConnectionString
            connection.Open()
            command.CommandText = comandoCrecionTablaBatchJson
            command.Connection = connection
            command.ExecuteNonQuery()
            Using s As SqlBulkCopy = New SqlBulkCopy(connection)
                s.DestinationTableName = "##insertar_batch_json_temp"
                s.BatchSize = dt.Rows.Count
                s.ColumnMappings.Add("id", "id")
                s.ColumnMappings.Add("id_equipo", "id_equipo")
                s.ColumnMappings.Add("nombre_equipo", "nombre_equipo")
                s.ColumnMappings.Add("fecha_inicio", "fecha_inicio")
                s.ColumnMappings.Add("fecha_termino", "fecha_termino")
                'id_equipo,nombre_equipo,fecha_inicio,fecha_termino
                s.WriteToServer(dt)
                s.Close()
            End Using

            command = New SqlCommand("Batch_InsertarOffline") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }
            reader = command.ExecuteReader()
            While reader.Read()
                resp = reader.GetValue(0)
            End While
            Trace.WriteLine("ses " & resp)
            Return resp
        Catch ex As Exception
            Trace.WriteLine($"Error batchmodel 617: {ex.Message}")
            Return resp
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function



End Class
