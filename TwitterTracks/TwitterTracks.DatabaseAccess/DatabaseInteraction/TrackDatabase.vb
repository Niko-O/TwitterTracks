
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

#Region "Administrator"

    Public Function CreateTrack(ResearcherPassword As String) As CreateTrackResult
        Try
            BeginTransaction()

            Dim TrackTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, New VerbatimIdentifier(Relations.TableNames.TrackTableName).Escape)
            Dim TrackEntityId = ExecuteNonQuery(FormatSqlIdentifiers("INSERT INTO {0}() VALUES ()", TrackTableIdentifier)).InsertId

            Dim MetadataTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.MetadataTableName(TrackEntityId).Escape)
            ExecuteNonQuery(FormatSqlIdentifiers( _
                "CREATE TABLE {0} (                       " & _
                "    `TweetId`             INT8     NULL, " & _
                "    `CreatedByUserId`     INT8     NULL, " & _
                "    `TweetText`           TEXT NOT NULL, " & _
                "    `RelevantKeywords`    TEXT NOT NULL, " & _
                "    `MediaFilePathsToAdd` TEXT NOT NULL, " & _
                "    `ConsumerKey`         TEXT NOT NULL, " & _
                "    `ConsumerSecret`      TEXT NOT NULL, " & _
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
                "    `Debug_TweetId`      INT8       NULL,                " & _
                "    `Debug_TweetContent` TEXT       NULL,                " & _
                "    INDEX `Idx_PublishDateTime` (`PublishDateTime` ASC), " & _
                "    INDEX `Idx_LocationType`    (`LocationType`    ASC), " & _
                "    INDEX `Idx_Latitude`        (`Latitude`        ASC), " & _
                "    INDEX `Idx_Longitude`       (`Longitude`       ASC), " & _
                "    PRIMARY KEY                 (`Id`)                   " & _
                ") ENGINE = InnoDB;                                       ", TweetTableIdentifier))

            Dim ResearcherName As String = Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId)
            For Each Host In {"%", "localhost"}
                Dim ResearcherIdentifier = Relations.UserNames.UserIdentifier(New VerbatimIdentifier(ResearcherName).Escape, New VerbatimIdentifier(Host).Escape)
                ExecuteNonQuery(FormatSqlIdentifiers("CREATE USER {0} IDENTIFIED BY @ResearcherPassword;", ResearcherIdentifier), _
                                New CommandParameter("@ResearcherPassword", ResearcherPassword))
                ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO {1};", MetadataTableIdentifier, ResearcherIdentifier))
                ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO {1};", TweetTableIdentifier, ResearcherIdentifier))
            Next


            ExecuteNonQuery(FormatSqlIdentifiers("FLUSH PRIVILEGES;"))

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
        Dim TrackTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, New VerbatimIdentifier(Relations.TableNames.TrackTableName).Escape)
        Return ExecuteQuery(FormatSqlIdentifiers("SELECT * FROM {0}", TrackTableIdentifier)).Select(Function(Row) New Track(Row.GetEntityId("Id"), Nothing))
    End Function

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
            ExecuteNonQuery(FormatSqlIdentifiers("FLUSH PRIVILEGES;"))

            CommitTransaction()
        Finally
            EndTransaction()
        End Try
    End Sub

#End Region

End Class
