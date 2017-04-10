
Imports Sql = MySql.Data.MySqlClient

Public Class TrackDatabase
    Inherits DatabaseBase

    Dim _DatabaseName As VerbatimIdentifier
    Public ReadOnly Property DatabaseName As VerbatimIdentifier
        <DebuggerStepThrough()>
        Get
            Return _DatabaseName
        End Get
    End Property

    Public Sub New(NewConnection As DatabaseConnection, NewDatabaseName As VerbatimIdentifier)        
        MyBase.New(NewConnection)
        If NewDatabaseName Is Nothing Then
            Throw New ArgumentNullException("NewDatabaseName")
        End If
        _DatabaseName = NewDatabaseName
    End Sub

    Private Function GetApplicationTokenIdentifier() As EscapedIdentifier
        Return Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.ApplicationTokenTableName.Escape)
    End Function

#Region "RowToModel"

    Private Shared Function RowToApplicationToken(Reader As Sql.MySqlDataReader) As ApplicationToken
        Return New ApplicationToken(Reader.GetString("ConsumerKey"), Reader.GetString("ConsumerSecret"))
    End Function

#End Region

#Region "Administrator"

    Public Function CreateTrack(ResearcherPassword As String) As CreateTrackResult
        Try
            BeginTransaction()

            Dim TrackTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.TrackTableName.Escape)
            Dim TrackEntityId = ExecuteNonQuery(FormatSqlIdentifiers("INSERT INTO {0}() VALUES ()", TrackTableIdentifier)).InsertId

            Dim MetadataTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.MetadataTableName(TrackEntityId).Escape)
            ExecuteNonQuery(FormatSqlIdentifiers( _
                "CREATE TABLE {0} (                       " & _
                "    `TweetId`             INT8     NULL, " & _
                "    `CreatedByUserId`     INT8     NULL, " & _
                "    `TweetText`           TEXT NOT NULL, " & _
                "    `RelevantKeywords`    TEXT NOT NULL, " & _
                "    `MediaFilePathsToAdd` TEXT NOT NULL, " & _
                "    `AccessToken`         TEXT NOT NULL, " & _
                "    `AccessTokenSecret`   TEXT NOT NULL  " & _
                ") ENGINE = InnoDB;                       ", MetadataTableIdentifier))

            Dim TweetTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.TweetTableName(TrackEntityId).Escape)
            ExecuteNonQuery(FormatSqlIdentifiers( _
                "CREATE TABLE {0} (                                       " & _
                "    `Id`                 INT    NOT NULL AUTO_INCREMENT, " & _
                "    `IsRetweet`          BOOL   NOT NULL,                " & _
                "    `MatchingKeywords`   TEXT   NOT NULL,                " & _
                "    `PublishDateTime`    INT8   NOT NULL,                " & _
                "    `LocationType`       INT    NOT NULL,                " & _
                "    `UserRegion`         TEXT       NULL,                " & _
                "    `Latitude`           DOUBLE     NULL,                " & _
                "    `Longitude`          DOUBLE     NULL,                " & _
                "    INDEX `Idx_PublishDateTime` (`PublishDateTime` ASC), " & _
                "    INDEX `Idx_LocationType`    (`LocationType`    ASC), " & _
                "    INDEX `Idx_Latitude`        (`Latitude`        ASC), " & _
                "    INDEX `Idx_Longitude`       (`Longitude`       ASC), " & _
                "    PRIMARY KEY                 (`Id`)                   " & _
                ") ENGINE = InnoDB;                                       ", TweetTableIdentifier))

            Dim ApplicationTokenTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.ApplicationTokenTableName.Escape)
            Dim ResearcherName As String = Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId)
            For Each Host In {"%", "localhost"}
                Dim ResearcherIdentifier = Relations.UserNames.UserIdentifier(New VerbatimIdentifier(ResearcherName).Escape, New VerbatimIdentifier(Host).Escape)
                ExecuteNonQuery(FormatSqlIdentifiers("CREATE USER {0} IDENTIFIED BY @ResearcherPassword;", ResearcherIdentifier), _
                                New CommandParameter("@ResearcherPassword", ResearcherPassword))
                ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO {1};", MetadataTableIdentifier, ResearcherIdentifier))
                ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO {1};", TweetTableIdentifier, ResearcherIdentifier))
                ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT ON {0} TO {1};", ApplicationTokenTableIdentifier, ResearcherIdentifier))
            Next

            CommitTransaction()
            Return New CreateTrackResult(New Track(TrackEntityId, Nothing), New DatabaseUser(ResearcherName, ResearcherPassword), New TwitterTracks.DatabaseAccess.ResearcherDatabase(Connection, DatabaseName, TrackEntityId))
        Finally
            EndTransaction()
        End Try
    End Function

    Public Structure CreateTrackResult

        Private _Track As Track
        Public ReadOnly Property Track As Track
            <DebuggerStepThrough()>
            Get
                Return _Track
            End Get
        End Property

        Private _ResearcherUser As DatabaseUser
        Public ReadOnly Property ResearcherUser As DatabaseUser
            <DebuggerStepThrough()>
            Get
                Return _ResearcherUser
            End Get
        End Property

        Dim _ResearcherDatabase As ResearcherDatabase
        Public ReadOnly Property ResearcherDatabase As ResearcherDatabase
            <DebuggerStepThrough()>
            Get
                Return _ResearcherDatabase
            End Get
        End Property

        Public Sub New(NewTrack As Track, NewResearcherUser As DatabaseUser, NewResearcherDatabase As ResearcherDatabase)
            _Track = NewTrack
            _ResearcherUser = NewResearcherUser
            _ResearcherDatabase = NewResearcherDatabase
        End Sub

    End Structure

    Public Function GetAllTracksWithoutMetadata() As IEnumerable(Of Track)
        Dim TrackTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.TrackTableName.Escape)
        Return ExecuteQuery(FormatSqlIdentifiers("SELECT * FROM {0}", TrackTableIdentifier)).Select(Function(Row) New Track(Row.GetEntityId("Id"), Nothing))
    End Function

    Public Function TryGetApplicationToken() As ApplicationToken?
        Using Row = ExecuteSingleRowQuery(False, FormatSqlIdentifiers("SELECT * FROM {0}", GetApplicationTokenIdentifier))
            If Row Is Nothing Then
                Return Nothing
            Else
                Return RowToApplicationToken(Row.Reader)
            End If
        End Using
    End Function

    Public Sub UpdateOrCreateApplicationToken(Token As ApplicationToken)
        Try
            BeginTransaction()

            Dim ApplicationTokenIdentifier = GetApplicationTokenIdentifier()

            Dim FormattedQueryString As SqlQueryString
            If TryGetApplicationToken() Is Nothing Then
                FormattedQueryString = FormatSqlIdentifiers( _
                    "INSERT INTO {0} (        " & _
                    "    `ConsumerKey`,       " & _
                    "    `ConsumerSecret`)    " & _
                    "VALUES (@ConsumerKey,    " & _
                    "        @ConsumerSecret) ", ApplicationTokenIdentifier)
            Else
                FormattedQueryString = FormatSqlIdentifiers( _
                    "UPDATE {0}                             " & _
                    "SET `ConsumerKey`    = @ConsumerKey,   " & _
                    "    `ConsumerSecret` = @ConsumerSecret ", ApplicationTokenIdentifier)
            End If
            ExecuteNonQuery(FormattedQueryString, _
                            New CommandParameter("@ConsumerKey", Token.ConsumerKey), _
                            New CommandParameter("@ConsumerSecret", Token.ConsumerSecret))

            CommitTransaction()
        Finally
            EndTransaction()
        End Try
    End Sub

    Public Sub DeleteApplicationToken()
        ExecuteNonQuery(FormatSqlIdentifiers("DELETE FROM {0}", GetApplicationTokenIdentifier))
    End Sub

#End Region

#Region "Root"

    Public Sub DeleteDatabase()
        Try
            BeginTransaction()

            Dim Tracks = GetAllTracksWithoutMetadata.ToList
            For Each i In Tracks
                Dim ResearcherToDrop = Relations.UserNames.ResearcherUserName(DatabaseName, i.EntityId)
                ExecuteNonQuery(FormatSqlIdentifiers("DROP USER @ResearcherToDrop"), New CommandParameter("@ResearcherToDrop", ResearcherToDrop))
            Next
            Dim AdministratorToDrop = Relations.UserNames.AdministratorUserName(DatabaseName)
            For Each Host In {"%", "localhost"}
                Dim AdministratorIdentifier = Relations.UserNames.UserIdentifier(New VerbatimIdentifier(AdministratorToDrop).Escape, New VerbatimIdentifier(Host).Escape)
                ExecuteNonQuery(FormatSqlIdentifiers("DROP USER {0}", AdministratorIdentifier))
            Next
            ExecuteNonQuery(FormatSqlIdentifiers("DROP DATABASE {0}", DatabaseName.Escape))

            CommitTransaction()
        Finally
            EndTransaction()
        End Try
    End Sub

#End Region

End Class
