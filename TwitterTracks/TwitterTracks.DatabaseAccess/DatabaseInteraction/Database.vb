
Imports Sql = MySql.Data.MySqlClient

Public Class Database
    Inherits DatabaseBase

    Public Sub New(NewConnection As DatabaseConnection)
        MyBase.New(NewConnection)
    End Sub

    Public Function CreateTrackDatabase(DatabaseName As VerbatimIdentifier, AdministratorPassword As String) As CreateTrackDatabaseResult
        'Create Database
        'Create Track Table
        'Create User
        'Grant Create, Drop to Database.* Table
        'Grant Select, Insert, Update, Delete to Database.Track
        'Flush

        Try
            BeginTransaction()

            Dim TrackDB As New TrackDatabase(Connection, DatabaseName)
            ExecuteNonQuery(FormatSqlIdentifiers("CREATE DATABASE {0}", DatabaseName.Escape))

            Dim TrackTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, New VerbatimIdentifier(Relations.TableNames.TrackTableName).Escape)
            ExecuteNonQuery(New SqlQueryString( _
                "CREATE TABLE " & TrackTableIdentifier.EscapedText & " ( " & _
                "  `Id` INT NOT NULL AUTO_INCREMENT,                     " & _
                "  PRIMARY KEY (`Id`))                                   " & _
                "ENGINE = InnoDB;                                        "))

            Dim AdministratorName As String = Relations.UserNames.AdministratorUserName(DatabaseName)
            For Each Host In {"%", "localhost"}
                Dim AdministratorIdentifier = Relations.UserNames.UserIdentifier(New VerbatimIdentifier(AdministratorName).Escape, New VerbatimIdentifier(Host).Escape)
                ExecuteNonQuery(FormatSqlIdentifiers("CREATE USER {0} IDENTIFIED BY @AdministratorPassword;", AdministratorIdentifier), _
                                New CommandParameter("@AdministratorPassword", AdministratorPassword))
                ExecuteNonQuery(FormatSqlIdentifiers("GRANT CREATE, DROP ON {0} TO {1};", Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.WildcardTable), AdministratorIdentifier))
                ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO {1};", TrackTableIdentifier, AdministratorIdentifier))
            Next
            'ExecuteNonQuery(FormatSqlIdentifiers("CREATE USER @AdministratorName IDENTIFIED BY @AdministratorPassword;"), _
            '        New CommandParameter("@AdministratorName", AdministratorName), _
            '        New CommandParameter("@AdministratorPassword", AdministratorPassword))
            'ExecuteNonQuery(FormatSqlIdentifiers("GRANT CREATE, DROP ON {0} TO @AdministratorName;", Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.WildcardTable)), _
            '                New CommandParameter("@AdministratorName", AdministratorName))
            'ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO @AdministratorName;", TrackTableIdentifier), _
            '                New CommandParameter("@AdministratorName", AdministratorName))

            ExecuteNonQuery(FormatSqlIdentifiers("FLUSH PRIVILEGES;"))

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

    Public Function GetAllDatabaseNames() As IEnumerable(Of VerbatimIdentifier)
        Return ExecuteQuery(FormatSqlIdentifiers("SHOW DATABASES;")).Select(Function(Row) New VerbatimIdentifier(Row.GetString(0)))
    End Function

End Class
