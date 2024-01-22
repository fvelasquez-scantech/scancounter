Imports System.Data.SqlClient

Public Class ConfiguracionesModel

#Region "Propiedades"
    Public Property Id As Integer
    Public Property Puerto As String
    Public Property ActualizacionDisponible As Boolean
    Public Property DeployActualizacion As Boolean
    Public Property EstadoPaleta As Integer
    Public Property SolicitudBatch As Integer
#End Region

#Region "Funciones"

    Public Function ActualizarPaleta() As DataTable
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        Dim da As New SqlDataAdapter
        Dim result As New DataTable
        Try
            connection.ConnectionString = Configuration.ConnectionString
            command = New SqlCommand("Configuraciones_ActualizarPaleta") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }
            command.Parameters.AddWithValue("@id", Id)
            command.Parameters.AddWithValue("@estado_paleta", EstadoPaleta) ' tru false
            connection.Open()
            da.SelectCommand = command
            da.Fill(result)
            Return result
        Catch ex As Exception
            result.Columns.Add("Error")
            result.Rows.Add(ex.Message)
            FormPrincipal.LogERR($"Error Configuraciones Model 164: {ex.Message}")
            Trace.WriteLine($"Error Configuraciones Model 164: {ex.Message}")
            Return result
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function


    Public Function Listar() As DataTable
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        Dim da As New SqlDataAdapter
        Dim result As New DataTable

        Try
            connection.ConnectionString = Configuration.ConnectionString
            command = New SqlCommand("Configuraciones_Listar_SegunId") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }
            command.Parameters.AddWithValue("@id", Id)
            connection.Open()
            da.SelectCommand = command
            da.Fill(result)
            Return result
        Catch ex As Exception
            result.Columns.Add("Error")
            result.Rows.Add(ex.Message)
            Return result
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function

    Public Function ActualizarActualizacionDisponible() As Boolean
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        Dim result As Boolean

        Try
            connection.ConnectionString = Configuration.ConnectionString

            command = New SqlCommand("Configuraciones_ActualizarActualizacionDisponible") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }

            command.Parameters.AddWithValue("@actualizacion_disponible", ActualizacionDisponible)

            connection.Open()

            If command.ExecuteNonQuery Then
                result = True
            Else
                result = False
            End If
        Catch ex As Exception
            result = False
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try

        Return result
    End Function
    Public Function ActualizarDeployActualizacion() As Boolean
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        Dim result As Boolean

        Try
            connection.ConnectionString = Configuration.ConnectionString

            command = New SqlCommand("Configuraciones_ActualizarDeployActualizacion") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }

            command.Parameters.AddWithValue("@deploy_actualizacion", DeployActualizacion)

            connection.Open()

            If command.ExecuteNonQuery Then
                result = True
            Else
                result = False
            End If
        Catch ex As Exception
            FormPrincipal.LogERR($"Error Configuraciones Model depl: {ex.Message}")
            Trace.WriteLine($"Error Configuraciones Model depl: {ex.Message}")
            result = False
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try

        Return result
    End Function


    Public Function CambiarEstadBatch() As Integer
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        Dim da As New SqlDataAdapter
        Dim result As New DataTable

        Try
            connection.ConnectionString = Configuration.ConnectionString
            command = New SqlCommand("Configuraciones_SolicitudBatch") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }

            command.Parameters.AddWithValue("@solicitud_batch", SolicitudBatch)
            command.CommandTimeout = 1
            connection.Open()

            da.SelectCommand = command
            da.Fill(result)
            Return result.Rows(0)(0)
        Catch ex As Exception
            Trace.WriteLine($"Error (SensorModel 12) {ex.Message}")
            Return 0
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function



#End Region

End Class
