﻿Imports System.Data.SqlClient

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

            connection.Open()

            da.SelectCommand = command
            da.Fill(result)
            'For Each r In result.Rows
            '    Console.WriteLine(r(0))
            'Next
            Return result
        Catch ex As Exception
            'MsgBox($"Error (SensorModel 12) {ex.Message}")
            Trace.WriteLine($"Error (SensorModel 12) {ex.Message}")
            Return result
        Finally
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If
        End Try
    End Function

#End Region

End Class
