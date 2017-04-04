
Imports Sql = MySql.Data.MySqlClient

Public Class DatabaseServer
    Inherits DatabaseBase

    Public Sub New(NewConnection As DatabaseConnection)
        MyBase.New(NewConnection)
    End Sub

    Public Function CreateTrackDatabase(DatabaseName As VerbatimIdentifier, AdministratorPassword As String) As CreateTrackDatabaseResult
        Try
            BeginTransaction()

            Dim TrackDB As New TrackDatabase(Connection, DatabaseName)
            ExecuteNonQuery(FormatSqlIdentifiers("CREATE DATABASE {0}", DatabaseName.Escape))

            Dim TrackTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, New VerbatimIdentifier(Relations.TableNames.TrackTableName).Escape)
            ExecuteNonQuery(FormatSqlIdentifiers( _
                "CREATE TABLE {0} (                  " & _
                "  `Id` INT NOT NULL AUTO_INCREMENT, " & _
                "  PRIMARY KEY (`Id`))               " & _
                "ENGINE = InnoDB;                    ", TrackTableIdentifier))

            Dim AdministratorName As String = Relations.UserNames.AdministratorUserName(DatabaseName)
            For Each Host In {"%", "localhost"}
                Dim AdministratorIdentifier = Relations.UserNames.UserIdentifier(New VerbatimIdentifier(AdministratorName).Escape, New VerbatimIdentifier(Host).Escape)
                ExecuteNonQuery(FormatSqlIdentifiers("CREATE USER {0} IDENTIFIED BY @AdministratorPassword;", AdministratorIdentifier), _
                                New CommandParameter("@AdministratorPassword", AdministratorPassword))
                ExecuteNonQuery(FormatSqlIdentifiers("GRANT CREATE, DROP, SELECT, INSERT, UPDATE, DELETE ON {0} TO {1};", Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.WildcardTable), AdministratorIdentifier))
                ExecuteNonQuery(FormatSqlIdentifiers("GRANT CREATE USER ON {0} TO {1} WITH GRANT OPTION;", Relations.TableNames.TableIdentifier(Relations.WildcardDatabase, Relations.WildcardTable), AdministratorIdentifier))
            Next

            CommitTransaction()
            Return New CreateTrackDatabaseResult(TrackDB, New DatabaseUser(AdministratorName, AdministratorPassword))
        Finally
            EndTransaction()
        End Try
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

    ''' <summary>
    ''' Returns a list of the names of all databases in the database server.
    ''' This includes databases which are not TrackDatabases.
    ''' </summary>
    Public Function GetAllDatabaseNames() As IEnumerable(Of VerbatimIdentifier)
        Return ExecuteQuery(FormatSqlIdentifiers("SHOW DATABASES;")).Select(Function(Row) New VerbatimIdentifier(Row.GetString(0)))
    End Function

End Class
