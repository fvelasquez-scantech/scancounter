Imports System.Data.SqlClient

Public Class SensoresModel

#Region "Propiedades"
    Public Property Id As Integer
    Public Property Nombre As String
    Public Property IdSensorEstado As Integer
    Public Property UltimoInicioLecturas As Date
#End Region

#Region "Funciones"
    Public Function Listar() As DataTable
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        Dim da As New SqlDataAdapter
        Dim result As New DataTable

        Try
            connection.ConnectionString = Configuration.ConnectionString
            command = New SqlCommand("Sensores_Listar") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }

            command.Parameters.AddWithValue("@id", Id)
            command.CommandTimeout = 1
            connection.Open()

            da.SelectCommand = command
            da.Fill(result)
            Return result
        Catch ex As Exception
            'Trace.WriteLine($"Error (SensorModel 12) {ex.Message}")
            Return result
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function



    Public Function ListarAlt() As DataTable
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        Dim da As New SqlDataAdapter
        Dim result As New DataTable

        Try
            connection.ConnectionString = Configuration.ConnectionString

            command = New SqlCommand("Sensores_Listar_Alt") With {
                .CommandType = CommandType.StoredProcedure,
                .connection = connection
            }
            command.Parameters.AddWithValue("@id", Id)
            command.CommandTimeout = 1
            connection.Open()

            da.SelectCommand = command
            da.Fill(result)
            Return result
        Catch ex As Exception
            'Trace.WriteLine($"Error (SensorModel 12) {ex.Message}")
            Return result
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function


#End Region

End Class
