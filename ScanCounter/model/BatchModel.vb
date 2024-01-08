Imports System.Data.SqlClient
Imports System.Diagnostics.Eventing

Public Class BatchModel


    Dim Reader As SqlDataReader
    Public Property IdEquipo As Integer
    'Public Property FechaInsercion As DateTime
    Public Property NombreEquipo As String
    Public Property FechaInicio As DateTime
    Public Property FechaTermino As DateTime

    Public Function InsertarBatch() As Integer
        Dim connection As New SqlConnection
        Dim command As SqlCommand
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
            command.ExecuteNonQuery()
            'Trace.WriteLine("wenas")
            'Return 1
            Dim resp As Integer
            Reader = command.ExecuteReader()
            While Reader.Read()
                resp = Reader.GetValue(0)
            End While
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

End Class
