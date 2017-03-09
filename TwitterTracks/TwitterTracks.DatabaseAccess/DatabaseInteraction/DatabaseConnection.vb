
Imports Sql = MySql.Data.MySqlClient

Public Class DatabaseConnection

    Dim _ConnectionStringSource As IConnectionStringSource
    Public ReadOnly Property ConnectionStringSource As IConnectionStringSource
        <DebuggerStepThrough()>
        Get
            Return _ConnectionStringSource
        End Get
    End Property

    Dim _Connection As Sql.MySqlConnection
    Public ReadOnly Property Connection As Sql.MySqlConnection
        <DebuggerStepThrough()>
        Get
            Return _Connection
        End Get
    End Property

    Public ReadOnly Property IsOpen As Boolean
        Get
            Return Connection IsNot Nothing
        End Get
    End Property

    Public Sub New(NewConnectionStringSource As IConnectionStringSource)
        _ConnectionStringSource = NewConnectionStringSource
    End Sub

    Public Sub Open()
        If _Connection IsNot Nothing Then
            _Connection.Close()
            _Connection.Dispose()
        End If
        Dim NewConnection = New Sql.MySqlConnection(ConnectionStringSource.GetConnectionString)
        If DatabaseBase.DebugPrintQueries Then
            Helpers.DebugPrint("Opening Connection: {0}", NewConnection.ConnectionString)
        End If
        NewConnection.Open()
        _Connection = NewConnection
    End Sub

    Public Sub Close()
        If _Connection Is Nothing Then
            Return
        End If
        _Connection.Close()
        _Connection.Dispose()
        _Connection = Nothing
    End Sub

    Public Function CreateCommand() As Sql.MySqlCommand
        Return Connection.CreateCommand
    End Function

    Public Function BeginTransaction() As Sql.MySqlTransaction
        Return Connection.BeginTransaction(IsolationLevel.Serializable)
    End Function

    Public Shared Function PlainConnection(Host As String, UserName As String, Password As String) As DatabaseConnection
        Dim ConnectionStringSource As New TwitterTracks.DatabaseAccess.MySqlConnectionStringBuilderSource( _
         New MySql.Data.MySqlClient.MySqlConnectionStringBuilder With { _
             .Server = Host, _
             .CharacterSet = "utf8", _
             .SslMode = MySql.Data.MySqlClient.MySqlSslMode.Preferred, _
             .Password = Password, _
             .UserID = UserName})
        Return New DatabaseConnection(ConnectionStringSource)
    End Function

End Class
