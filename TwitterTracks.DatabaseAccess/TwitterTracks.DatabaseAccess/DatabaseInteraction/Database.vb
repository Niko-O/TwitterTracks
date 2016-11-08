
Imports Sql = MySql.Data.MySqlClient

Public Class Database

    Public Shared ReadOnly WildcardTable As New EscapedIdentifier("*")

    Dim _Connection As DatabaseConnection
    Public ReadOnly Property Connection As DatabaseConnection
        <DebuggerStepThrough()>
        Get
            Return _Connection
        End Get
    End Property

    Public Sub New(NewConnection As DatabaseConnection)
        _Connection = NewConnection
    End Sub

    Protected Shared Function FormatSqlIdentifiers(Format As String, ParamArray EscapedArguments As EscapedIdentifier()) As SqlQueryString
        Return New SqlQueryString(String.Format(Format, EscapedArguments.Select(Function(i) i.EscapedText)))
    End Function

#Region "Queries"

    Protected Function PrepareCommand(QueryText As SqlQueryString, ParamArray Parameters As CommandParameter()) As Sql.MySqlCommand
        Dim Command = Connection.CreateCommand
        Command.CommandText = QueryText.QueryText
        Command.Prepare()
        For Each i In Parameters
            Command.Parameters.AddWithValue(i.Name, i.Value)
        Next
        Return Command
    End Function

    Protected Function ExecuteNonQuery(QueryText As SqlQueryString, ParamArray Parameters As CommandParameter()) As NonQueryResult
        Using Command = PrepareCommand(QueryText, Parameters)
            Command.ExecuteNonQuery()
            Return New NonQueryResult(New EntityId(Command.LastInsertedId))
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

        Public Sub New(NewInsertId As EntityId)
            _InsertId = NewInsertId
        End Sub

    End Structure

    Protected Function ExecuteScalar(Of T)(QueryText As SqlQueryString, ParamArray Parameters As CommandParameter()) As T
        Using Command = PrepareCommand(QueryText, Parameters)
            Return DirectCast(Command.ExecuteScalar, T)
        End Using
    End Function

    Protected Function ExecuteSingleRowQuery(ThrowOnEmptyResult As Boolean, QueryText As SqlQueryString, ParamArray Parameters As CommandParameter()) As MysqlReaderSingleRow
        Dim Command = PrepareCommand(QueryText, Parameters)
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
                Dim Reader = Command.ExecuteReader(CommandBehavior.SequentialAccess)
                Return New MySqlReaderEnumerator(Command, Reader)
            End Function)
    End Function

#End Region

    Public Function CreateTrackDatabase(DatabaseName As VerbatimIdentifier, AdministratorPassword As String) As CreateTrackDatabaseResult
        'Create Database
        'Create Track Table
        'Create User
        'Grant Create, Drop to Database.* Table
        'Grant Select, Insert, Update, Delete to Database.Track
        'Flush

        Dim TrackDB As New TrackDatabase(Connection, DatabaseName)
        ExecuteNonQuery(FormatSqlIdentifiers("CREATE DATABASE {0}", DatabaseName.Escape))

        Dim TrackTableIdentifier = TrackDB.GetTableIdentifier(New VerbatimIdentifier("Track").Escape)
        ExecuteNonQuery(New SqlQueryString( _
            "CREATE TABLE " & TrackTableIdentifier.EscapedText & " ( " & _
            "  `Id` INT NOT NULL AUTO_INCREMENT,                     " & _
            "  PRIMARY KEY (`Id`))                                   " & _
            "ENGINE = InnoDB;                                        "))

        Dim AdministratorName As String = Relations.UserNames.AdministratorUserName(DatabaseName)
        ExecuteNonQuery(FormatSqlIdentifiers("CREATE USER @AdministratorName IDENTIFIED BY @AdministratorPassword;"), _
                        New CommandParameter("@AdministratorName", AdministratorName), _
                        New CommandParameter("@AdministratorPassword", AdministratorPassword))

        ExecuteNonQuery(FormatSqlIdentifiers("GRANT CREATE, DROP ON {0} TO @AdministratorName;", TrackDB.GetTableIdentifier(WildcardTable)), _
                        New CommandParameter("@AdministratorName", AdministratorName))
        ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO @AdministratorName;", TrackTableIdentifier), _
                        New CommandParameter("@AdministratorName", AdministratorName))
        ExecuteNonQuery(FormatSqlIdentifiers("FLUSH PRIVILEGES;"))

        Return New CreateTrackDatabaseResult(TrackDB, New DatabaseUser(AdministratorName, AdministratorPassword))
    End Function

    Public Structure CreateTrackDatabaseResult

        Private _TrackDatabase As TrackDatabase
        Public ReadOnly Property TrackDatabase As TrackDatabase
            <DebuggerStepThrough()>
            Get
                Return _TrackDatabase
            End Get
        End Property

        Private _AdministratorUser As DatabaseUser
        Public ReadOnly Property AdministratorUser As DatabaseUser
            <DebuggerStepThrough()>
            Get
                Return _AdministratorUser
            End Get
        End Property

        Public Sub New(NewTrackDatabase As TrackDatabase, NewAdministratorUser As DatabaseUser)
            _TrackDatabase = NewTrackDatabase
            _AdministratorUser = NewAdministratorUser
        End Sub

    End Structure

    Public Function GetAllDatabaseNames() As IEnumerable(Of VerbatimIdentifier)
        Dim Databases = ExecuteQuery(FormatSqlIdentifiers("SHOW DATABASES;"))
        Return New DelegateEnumerable(Of VerbatimIdentifier)( _
           Function()
               Return New MySqlRowEnumerator(Of VerbatimIdentifier)( _
                   Databases.GetEnumerator, _
                   Function(Reader)
                       Return New VerbatimIdentifier(Reader.GetString(0))
                   End Function)
           End Function)
    End Function

End Class
