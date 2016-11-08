Public Class DebugConnectionStringSource
    Implements TwitterTracks.DatabaseAccess.IConnectionStringSource

    Public Function GetConnectionString() As String Implements TwitterTracks.DatabaseAccess.IConnectionStringSource.GetConnectionString
        Return String.Format("Server=127.0.0.1;Uid=root;Pwd=;SslMode=Preferred;")
    End Function

End Class
