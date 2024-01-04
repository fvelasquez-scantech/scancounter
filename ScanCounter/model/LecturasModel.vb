Imports System.Data.SqlClient
Imports System.Web.Caching
Imports System.Web.Management

Public Class LecturasModel

#Region "Propiedades"
    Public Property Id As Integer

    Public Property IdEquipo As Integer

    Public Property IdSensor As Integer
    Public Property FechaInsercion As Date

    Private Shared ReadOnly comandoCreacionTablaTemporalProductos As String =
                                "CREATE TABLE ##insertar_lecturas_temp(
                                    [id] [bigint] IDENTITY(1,1) NOT NULL PRIMARY KEY CLUSTERED , 
                                     [idSensor] [int] NULL,
                                     [fecha_insercion] [datetime] NULL
                                   )"
#End Region

#Region "Funciones"
    Public Async Function Insertar() As Task(Of Integer)
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        Try
            connection.ConnectionString = Configuration.ConnectionString
            command = New SqlCommand("Lecturas_Insertar") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }
            command.Parameters.AddWithValue("@id_sensor", IdSensor)
            command.Parameters.AddWithValue("@fecha_insercion", FechaInsercion)
            command.Parameters.AddWithValue("@id_equipo", IdEquipo)
            connection.Open()
            Await command.ExecuteNonQueryAsync
            Trace.WriteLine("wenas")
            Return 1
        Catch ex As Exception
            Trace.WriteLine("lecturas model : Insertar() :" & ex.Message)
            Return 0
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function
    'este procedimiento solo tiene un numero como respuesta y 
    Public Shared Function InsertarLecturaOffline(dt As DataTable) As Integer
        Dim connection As New SqlConnection
        Dim tbl As New DataTable
        Dim command As New SqlCommand
        Dim reader As SqlDataReader
        Dim result As New DataTable
        Dim resp As New Integer
        resp = 0
        Try

            connection.ConnectionString = Configuration.ConnectionString
            connection.Open()
            command.CommandText = comandoCreacionTablaTemporalProductos
            command.Connection = connection
            command.ExecuteNonQuery()
            Using s As SqlBulkCopy = New SqlBulkCopy(connection)
                s.DestinationTableName = "##insertar_lecturas_temp"
                s.BatchSize = dt.Rows.Count
                s.ColumnMappings.Add("id", "id")
                s.ColumnMappings.Add("idSensor", "idSensor")
                s.ColumnMappings.Add("fecha_insercion", "fecha_insercion")
                s.WriteToServer(dt)
                s.Close()
            End Using

            command = New SqlCommand("Lecturas_InsertarOffline") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }
            reader = command.ExecuteReader()
            While reader.Read()
                resp = reader.GetValue(0)
            End While
            Return resp
        Catch ex As Exception
            'Trace.WriteLine($"Error BalanzaInformacion 617: {ex.Message}")
            Return resp
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function


#End Region
End Class
