Public Class MySqlConnectionStringBuilderSource
    Implements IConnectionStringSource

    Dim _ConnectionStringBuilder As MySql.Data.MySqlClient.MySqlConnectionStringBuilder
    Public ReadOnly Property ConnectionStringBuilder As MySql.Data.MySqlClient.MySqlConnectionStringBuilder
        <DebuggerStepThrough()>
        Get
            Return _ConnectionStringBuilder
        End Get
    End Property

    Public Sub New(NewConnectionStringBuilder As MySql.Data.MySqlClient.MySqlConnectionStringBuilder)
        _ConnectionStringBuilder = NewConnectionStringBuilder
    End Sub

    Public Function GetConnectionString() As String Implements IConnectionStringSource.GetConnectionString
        Return ConnectionStringBuilder.ConnectionString
    End Function

End Class
