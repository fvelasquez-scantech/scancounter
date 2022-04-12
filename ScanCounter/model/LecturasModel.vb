Imports System.Data.SqlClient

Public Class LecturasModel

#Region "Propiedades"
    Public Property Id As Integer
    Public Property IdSensor As Integer
    Public Property FechaInsercion As Date
#End Region

#Region "Funciones"
    Public Async Function Insertar() As Task
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        'Dim result As Boolean

        Try
            connection.ConnectionString = Configuration.ConnectionString
            command = New SqlCommand("Lecturas_Insertar") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }
            command.Parameters.AddWithValue("@id_sensor", IdSensor)
            command.Parameters.AddWithValue("@fecha_insercion", FechaInsercion)

            connection.Open()

            Await command.ExecuteNonQueryAsync
        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function
#End Region
End Class
