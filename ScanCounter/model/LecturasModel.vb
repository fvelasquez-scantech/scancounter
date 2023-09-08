Imports System.Data.SqlClient

Public Class LecturasModel

#Region "Propiedades"
    Public Property Id As Integer
    Public Property IdSensor As Integer
    Public Property FechaInsercion As Date
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
            connection.Open()
            Await command.ExecuteNonQueryAsync
            Return 1
        Catch ex As Exception
            'MsgBox("lecturas model : Insertar() :" & ex.Message)
            Trace.WriteLine("lecturas model : Insertar() :" & ex.Message)
            Return 0
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function

    Public Async Function Listar() As Task(Of DataTable)
        Dim connection As New SqlConnection
        Dim command As SqlCommand
        Dim result As New DataTable
        Dim da As New SqlDataAdapter
        'Dim result As Boolean

        Try
            connection.ConnectionString = Configuration.ConnectionString
            command = New SqlCommand("Lecturas_Insertar") With {
                .CommandType = CommandType.StoredProcedure,
                .Connection = connection
            }
            command.Parameters.AddWithValue("@id_sensor", IdSensor)
            command.Parameters.AddWithValue("@fecha_insercion", FechaInsercion)

            Await connection.OpenAsync()

            da.SelectCommand = command
            da.Fill(result)
            'reader = Await command.ExecuteReaderAsync

            'If Await reader.ReadAsync Then

            'End If
            Return result
        Catch ex As Exception
            'MsgBox("lecturasmodel:Insertar():" & ex.Message)
            Trace.WriteLine("lecturasmodel:Insertar():" & ex.Message)
            Return Nothing
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function
#End Region
End Class
