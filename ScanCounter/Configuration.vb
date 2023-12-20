Public Class Configuration
    Public Shared Server As String = "192.168.1.253"
    Public Shared User As String = "sa"
    'Public Shared Server As String = "192.168.0.4" 'S. DE CHILE 
    'Public Shared User As String = "Scancounter"
    'Public Shared ConnectionString As String = "Data Source=SCANCOUNTER\SQLEXPRESS;Initial Catalog=Scancounter;Integrated Security=True"
    Public Shared ConnectionString As String = $"Server={Server};Database=Scancounter;User={User};Password=Scan2021##;"
    Public Shared SourcePath As String = "C:\Scantech\data_scancounter_front.xml"
    Public Shared SaveDirectory As String = "C:\Scantech\"
    Public Shared Filename As String = System.IO.Path.GetFileName(SourcePath) 'obtiene el nombre del archivo original sin el path de origen
    Public Shared SavePath As String = System.IO.Path.Combine(SaveDirectory, Filename) 'ruta completa del archivo de guardado

End Class
