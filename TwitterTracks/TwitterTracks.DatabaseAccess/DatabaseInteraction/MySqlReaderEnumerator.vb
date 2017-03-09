
Imports Sql = MySql.Data.MySqlClient

Public Class MySqlReaderEnumerator
    Implements IEnumerator(Of Sql.MySqlDataReader)

    Dim CommandToDispose As Sql.MySqlCommand
    Dim Reader As Sql.MySqlDataReader
  
    Public Sub New(NewCommandToDispose As Sql.MySqlCommand, NewReader As Sql.MySqlDataReader)
        CommandToDispose = NewCommandToDispose
        Reader = NewReader
    End Sub

    Public ReadOnly Property Current As Sql.MySqlDataReader Implements IEnumerator(Of Sql.MySqlDataReader).Current
        Get
            If Reader Is Nothing Then
                Throw New ObjectDisposedException(Me.GetType.Name)
            End If
            Return Reader
        End Get
    End Property

    Private ReadOnly Property IEnumerator_Current As Object Implements IEnumerator.Current
        Get
            Return Current
        End Get
    End Property

    Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
        If Reader Is Nothing Then
            Throw New ObjectDisposedException(Me.GetType.Name)
        End If
        Return Reader.Read
    End Function

    Public Sub Reset() Implements IEnumerator.Reset
        Throw New NotSupportedException
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If Reader Is Nothing Then
            Return
        End If
        Reader.Dispose()
        CommandToDispose.Dispose()
        Reader = Nothing
        CommandToDispose = Nothing
    End Sub

End Class