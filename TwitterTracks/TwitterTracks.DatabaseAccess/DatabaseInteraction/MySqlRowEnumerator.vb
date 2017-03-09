
Imports Sql = MySql.Data.MySqlClient

Public Class MySqlRowEnumerator(Of T)
    Implements IEnumerator(Of T)

    Dim ReaderEnumerator As IEnumerator(Of Sql.MySqlDataReader)
    Dim RowToObjectConverter As Func(Of Sql.MySqlDataReader, T)
    Dim _Current As T

    Public Sub New(NewReaderEnumerator As IEnumerator(Of Sql.MySqlDataReader), NewRowToObjectConverter As Func(Of Sql.MySqlDataReader, T))
        ReaderEnumerator = NewReaderEnumerator
        RowToObjectConverter = NewRowToObjectConverter
    End Sub

    Public ReadOnly Property Current As T Implements IEnumerator(Of T).Current
        Get
            Return _Current
        End Get
    End Property

    Private ReadOnly Property IEnumerator_Current As Object Implements IEnumerator.Current
        Get
            Return Current
        End Get
    End Property

    Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
        If ReaderEnumerator Is Nothing Then
            Throw New ObjectDisposedException(Me.GetType.Name)
        End If
        If Not ReaderEnumerator.MoveNext Then
            Return False
        End If
        _Current = RowToObjectConverter(ReaderEnumerator.Current)
        Return True
    End Function

    Public Sub Reset() Implements IEnumerator.Reset
        Throw New NotSupportedException
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If ReaderEnumerator Is Nothing Then
            Return
        End If
        ReaderEnumerator.Dispose()
        ReaderEnumerator = Nothing
        RowToObjectConverter = Nothing
        _Current = Nothing
    End Sub

End Class
