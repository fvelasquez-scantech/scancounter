Imports System.Data.SqlClient

Public Class ConfiguracionesModel

#Region "Propiedades"
    Public Property Id As Integer
    Public Property Puerto As String
#End Region

#Region "Funciones"
    Public Function Listar() As DataTable
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        Dim da As New SqlDataAdapter
        Dim result As New DataTable

        Try
            connection.ConnectionString = Configuration.ConnectionString

            command = New SqlCommand("Configuraciones_Listar") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }

            connection.Open()

            da.SelectCommand = command
            da.Fill(result)

            Return result
        Catch ex As Exception
            result.Columns.Add("Error")
            result.Rows.Add(ex.Message)
            'MsgBox($"Error (ConfiguracionesModel 10) {ex.Message}")
            Return result
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function
#End Region

End Class
