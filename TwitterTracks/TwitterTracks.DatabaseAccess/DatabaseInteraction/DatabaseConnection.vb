
Imports Sql = MySql.Data.MySqlClient

Public Class DatabaseConnection

    Public ReadOnly Property IsOpen As Boolean
        Get
            Return MySqlConnection IsNot Nothing
        End Get
    End Property

    Dim ConnectionStringSource As MySql.Data.MySqlClient.MySqlConnectionStringBuilder
    Dim MySqlConnection As Sql.MySqlConnection = Nothing

    Public Sub New(Host As String, UserName As String, Password As String)
        ConnectionStringSource = New MySql.Data.MySqlClient.MySqlConnectionStringBuilder With _
        {
            .Server = Host, _
            .CharacterSet = "utf8", _
            .SslMode = MySql.Data.MySqlClient.MySqlSslMode.Preferred, _
            .Password = Password, _
            .UserID = UserName
        }
    End Sub

    Public Sub Open()
        If MySqlConnection IsNot Nothing Then
            MySqlConnection.Close()
            MySqlConnection.Dispose()
        End If
        Dim NewConnection = New Sql.MySqlConnection(ConnectionStringSource.ConnectionString)
        If DatabaseBase.DebugPrintQueries Then
            Helpers.DebugPrint("Opening Connection: {0}", NewConnection.ConnectionString)
        End If
        NewConnection.Open()
        MySqlConnection = NewConnection
    End Sub

    Public Sub Close()
        If MySqlConnection Is Nothing Then
            Return
        End If
        MySqlConnection.Close()
        MySqlConnection.Dispose()
        MySqlConnection = Nothing
    End Sub

    Public Function CreateCommand() As Sql.MySqlCommand
        Return MySqlConnection.CreateCommand
    End Function

    Public Function BeginTransaction() As Sql.MySqlTransaction
        Return MySqlConnection.BeginTransaction(IsolationLevel.Serializable)
    End Function

End Class
