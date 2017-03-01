
Imports Sql = MySql.Data.MySqlClient

'ToDo: Track consists of EntityId and TrackMetadata. If there is ever additional data stored in the Track table it has to be stored somewhere. Also create TrackPublicData structure and Nullable of that in Track structure.

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
        _DatabaseName = NewDatabaseName
    End Sub

#Region "Administrator"

    Public Function CreateTrack(ResearcherPassword As String) As CreateTrackResult
        SyncLock LockObject
            'Insert Track
            'Create Metadata Table
            'Create Tweet Table
            'Create User
            'Assign Select, Insert, Update, Delete Acces to Metadata, Tweet Table
            'Flush

            Try
                BeginTransaction()

                Dim TrackTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, New VerbatimIdentifier("Track").Escape)
                Dim TrackEntityId = ExecuteNonQuery(FormatSqlIdentifiers("INSERT INTO {0}() VALUES ()", TrackTableIdentifier)).InsertId

                Dim MetadataTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.MetadataTableName(TrackEntityId).Escape)
                ExecuteNonQuery(New SqlQueryString( _
                    "CREATE TABLE " & MetadataTableIdentifier.EscapedText & " (  " & _
                    "    `InitialTweetId` INT8 NOT NULL,                         " & _
                    "    `InitialTweetUserId` INT8 NOT NULL,                     " & _
                    "    `InitialTweetFullText` TEXT NOT NULL,                   " & _
                    "    `RelevantKeywords` TEXT NOT NULL)                       " & _
                    "    ENGINE = InnoDB;                                        "))

                Dim TweetTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.TweetTableName(TrackEntityId).Escape)
                ExecuteNonQuery(New SqlQueryString( _
                    "CREATE TABLE " & TweetTableIdentifier.EscapedText & " (  " & _
                    "    `Id` INT NOT NULL AUTO_INCREMENT,                    " & _
                    "    `ContentHash` TEXT NOT NULL,                         " & _
                    "    `PublishDateTime` INT8 NOT NULL,                     " & _
                    "    `LocationType` INT NOT NULL,                         " & _
                    "    `UserRegion` TEXT NULL,                              " & _
                    "    `Latitude` DOUBLE NULL,                              " & _
                    "    `Longitude` DOUBLE NULL,                             " & _
                    "    `Debug_TweetId` INT8 NULL,                           " & _
                    "    `Debug_TweetContent` TEXT NULL,                      " & _
                    "    INDEX `Idx_PublishDateTime` (`PublishDateTime` ASC), " & _
                    "    INDEX `Idx_LocationType` (`LocationType` ASC),       " & _
                    "    INDEX `Idx_Latitude` (`Latitude` ASC),               " & _
                    "    INDEX `Idx_Longitude` (`Longitude` ASC),             " & _
                    "    PRIMARY KEY (`Id`))                                  " & _
                    "    ENGINE = InnoDB;                                     "))

                Dim ResearcherName As String = Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId)
                For Each Host In {"%", "localhost"}
                    Dim ResearcherIdentifier = Relations.UserNames.UserIdentifier(New VerbatimIdentifier(ResearcherName).Escape, New VerbatimIdentifier(Host).Escape)
                    ExecuteNonQuery(FormatSqlIdentifiers("CREATE USER {0} IDENTIFIED BY @ResearcherPassword;", ResearcherIdentifier), _
                                    New CommandParameter("@ResearcherPassword", ResearcherPassword))
                    ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO {1};", MetadataTableIdentifier, ResearcherIdentifier))
                    ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO {1};", TweetTableIdentifier, ResearcherIdentifier))
                Next

                'ExecuteNonQuery(FormatSqlIdentifiers("CREATE USER @ResearcherName IDENTIFIED BY @ResearcherPassword;"), _
                '        New CommandParameter("@ResearcherName", ResearcherName), _
                '        New CommandParameter("@ResearcherPassword", ResearcherPassword))
                'ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO @ResearcherName;", MetadataTableIdentifier), _
                '                New CommandParameter("@ResearcherName", ResearcherName))
                'ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO @ResearcherName;", TweetTableIdentifier), _
                '                New CommandParameter("@ResearcherName", ResearcherName))

                ExecuteNonQuery(FormatSqlIdentifiers("FLUSH PRIVILEGES;"))

                CommitTransaction()
                Return New CreateTrackResult(New Track(TrackEntityId, Nothing), New DatabaseUser(ResearcherName, ResearcherPassword), New TwitterTracks.DatabaseAccess.ResearcherDatabase(Connection, DatabaseName, TrackEntityId))
            Finally
                EndTransaction()
            End Try
        End SyncLock
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
        SyncLock LockObject
            Dim TrackTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, New VerbatimIdentifier("Track").Escape)
            Return ExecuteQuery(FormatSqlIdentifiers("SELECT * FROM {0}", TrackTableIdentifier)).Select(Function(Row) New Track(Row.GetEntityId("Id"), Nothing))
        End SyncLock
    End Function

#End Region

#Region "Root"

    Public Sub DeleteDatabase()
        SyncLock LockObject
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
        End SyncLock
    End Sub

#End Region

End Class
