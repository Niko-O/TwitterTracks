
Imports Sql = MySql.Data.MySqlClient

Public Class MysqlReaderSingleRow
    Implements IDisposable

    Dim _Reader As Sql.MySqlDataReader
    Public ReadOnly Property Reader As Sql.MySqlDataReader
        <DebuggerStepThrough()>
        Get
            Return _Reader
        End Get
    End Property

    Dim CommandToDispose As Sql.MySqlCommand

    Public Sub New(NewCommandToDispose As Sql.MySqlCommand, NewReader As Sql.MySqlDataReader)
        CommandToDispose = NewCommandToDispose
        _Reader = NewReader
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If Reader Is Nothing Then
            Return
        End If
        If Reader.Read Then
            Throw New DataException("The single-row-query returned multiple rows.")
        End If
        Reader.Dispose()
        CommandToDispose.Dispose()
        _Reader = Nothing
    End Sub

End Class
