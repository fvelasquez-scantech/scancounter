Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Web.Caching
Imports System.Web.Management

Public Class EquiposModel
    Public Property id As Integer
    Public Property nombre As String
    Public Property LimiteBatch As Integer

    Public Function ListarById() As DataTable
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        Dim da As New SqlDataAdapter
        Dim result As New DataTable

        Try
            connection.ConnectionString = Configuration.ConnectionString

            command = New SqlCommand("Equipos_Listar_SegunId") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }

            command.Parameters.AddWithValue("@id", id)
            command.CommandTimeout = 1
            connection.Open()

            da.SelectCommand = command
            da.Fill(result)
            Return result
        Catch ex As Exception
            Return result
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function
End Class
