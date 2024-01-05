Imports System.Data.SqlClient

Public Class BatchModel

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
            command.Parameters.AddWithValue("@fecha", FechaInicio)
            command.Parameters.AddWithValue("@nombre_equipo", NombreEquipo)
            connection.Open()
            command.ExecuteNonQuery()
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

End Class
