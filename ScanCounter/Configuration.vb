Public Class Configuration
    Public Shared Server As String = "192.168.1.253"
    'Public Shared Server As String = "26.78.227.235"

    'Public Shared ConnectionString As String = "Data Source=SCANCOUNTER\SQLEXPRESS;Initial Catalog=Scancounter;Integrated Security=True"
    Public Shared ConnectionString As String = $"Server={Server};Database=Scancounter;User=sa;Password=Scan2021##;"
End Class
