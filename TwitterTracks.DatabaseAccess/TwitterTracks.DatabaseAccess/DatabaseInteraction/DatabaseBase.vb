
Imports Sql = MySql.Data.MySqlClient

'ToDo: Use Transactions

Public Class DatabaseBase

    Public Shared Property DebugPrintQueries As Boolean = False

    Dim _Connection As DatabaseConnection
    Public ReadOnly Property Connection As DatabaseConnection
        <DebuggerStepThrough()>
        Get
            Return _Connection
        End Get
    End Property

    Dim CurrentTransaction As Sql.MySqlTransaction = Nothing
    Dim CurrentTransactionIsCommitted As Boolean
    Dim CurrentTransactionSyncLockObject As New Object
    <ThreadStatic()>
    Dim CurrentTransactionWasLockedByThread As Boolean = False

    Public Sub New(NewConnection As DatabaseConnection)
        _Connection = NewConnection
    End Sub

    Protected Friend Shared Function FormatSqlIdentifiers(Format As String, ParamArray EscapedArguments As EscapedIdentifier()) As SqlQueryString
        Return New SqlQueryString(String.Format(Format, EscapedArguments.Select(Function(i) i.EscapedText)))
    End Function

    Protected Sub BeginTransaction()
        System.Threading.Monitor.Enter(CurrentTransactionSyncLockObject, CurrentTransactionWasLockedByThread)
        CurrentTransaction = Connection.BeginTransaction
        CurrentTransactionIsCommitted = False
    End Sub

    Protected Sub CommitTransaction()
        CurrentTransaction.Commit()
        CurrentTransactionIsCommitted = True
    End Sub

    Protected Sub EndTransaction()
        If Not CurrentTransactionIsCommitted Then
            CurrentTransaction.Rollback()
        End If
        CurrentTransaction.Dispose()
        CurrentTransaction = Nothing
        If CurrentTransactionWasLockedByThread Then
            CurrentTransactionWasLockedByThread = False
            System.Threading.Monitor.Exit(CurrentTransactionSyncLockObject)
        End If
    End Sub

    Protected Friend Function PrepareCommand(QueryText As SqlQueryString, ParamArray Parameters As CommandParameter()) As Sql.MySqlCommand
        If DebugPrintQueries Then
            Helpers.DebugPrint("PrepareCommand:")
            Helpers.DebugPrint(QueryText.QueryText)
            Helpers.DebugPrint("{0} Parameters:", Parameters.Length)
            For Each i In Parameters
                Helpers.DebugPrint("{0}: {1}", i.Name, If(i.Value Is Nothing, "NULL", i.Value.GetType.FullName))
            Next
            Helpers.DebugPrint("")
        End If

        Dim Command = Connection.CreateCommand
        If CurrentTransaction IsNot Nothing Then
            Command.Transaction = CurrentTransaction
        End If
        Command.CommandText = QueryText.QueryText
        Command.Prepare()
        For Each i In Parameters
            Command.Parameters.AddWithValue(i.Name, i.Value)
        Next
        Return Command
    End Function

    Protected Function ExecuteNonQuery(QueryText As SqlQueryString, ParamArray Parameters As CommandParameter()) As NonQueryResult
        Using Command = PrepareCommand(QueryText, Parameters)
            Dim AffectedRowCount = Command.ExecuteNonQuery()
            Dim Result As New NonQueryResult(New EntityId(Command.LastInsertedId), AffectedRowCount)
            If DebugPrintQueries Then
                Helpers.DebugPrint("Executed as NonQuery: InsertId = {0}", Result.InsertId.RawId)
            End If
            Return Result
        End Using
    End Function

    Protected Structure NonQueryResult

        Private _InsertId As EntityId
        Public ReadOnly Property InsertId As EntityId
            <DebuggerStepThrough()>
            Get
                Return _InsertId
            End Get
        End Property

        Private _AffectedRowCount As Integer
        Public ReadOnly Property AffectedRowCount As Integer
            <DebuggerStepThrough()>
            Get
                Return _AffectedRowCount
            End Get
        End Property

        Public Sub New(NewInsertId As EntityId, NewAffectedRowCount As Integer)
            _InsertId = NewInsertId
            _AffectedRowCount = NewAffectedRowCount
        End Sub

    End Structure

    Protected Function ExecuteScalar(Of T)(QueryText As SqlQueryString, ParamArray Parameters As CommandParameter()) As T
        Using Command = PrepareCommand(QueryText, Parameters)
            If DebugPrintQueries Then
                Helpers.DebugPrint("Executed as Scalar.")
            End If
            Return DirectCast(Command.ExecuteScalar, T)
        End Using
    End Function

    Protected Function ExecuteSingleRowQuery(ThrowOnEmptyResult As Boolean, QueryText As SqlQueryString, ParamArray Parameters As CommandParameter()) As MysqlReaderSingleRow
        Dim Command = PrepareCommand(QueryText, Parameters)
        If DebugPrintQueries Then
            Helpers.DebugPrint("Executed as SingleRow.")
        End If
        Dim Reader = Command.ExecuteReader(CommandBehavior.SingleRow)
        If Not Reader.Read Then
            Reader.Dispose()
            Command.Dispose()
            If ThrowOnEmptyResult Then
                Throw New DataException("The query did not return any results.")
            Else
                Return Nothing
            End If
        End If
        Return New MysqlReaderSingleRow(Command, Reader)
    End Function

    Protected Function ExecuteQuery(QueryText As SqlQueryString, ParamArray Parameters As CommandParameter()) As DelegateEnumerable(Of Sql.MySqlDataReader)
        Return New DelegateEnumerable(Of Sql.MySqlDataReader)( _
            Function()
                Dim Command = PrepareCommand(QueryText, Parameters)
                If DebugPrintQueries Then
                    Helpers.DebugPrint("Executed as Query.")
                End If
                Dim Reader = Command.ExecuteReader(CommandBehavior.SequentialAccess)
                Return New MySqlReaderEnumerator(Command, Reader)
            End Function)
    End Function

End Class
