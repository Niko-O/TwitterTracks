
Imports Sql = MySql.Data.MySqlClient

'ToDo: Use Transactions
'ToDo: Track consists of EntityId and TrackMetadata. If there is ever additional data stored in the Track table it has to be stored somewhere. Also create TrackPublicData structure and Nullable of that in Track structure.

Public Class TrackDatabase
    Inherits Database

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

    Friend Function GetTableIdentifier(TableName As EscapedIdentifier) As EscapedIdentifier
        Return New EscapedIdentifier(String.Format("{0}.{1}", DatabaseName.Escape.EscapedText, TableName.EscapedText))
    End Function

#Region "RowToModel"

    Private Shared Function RowToTrackMetadata(Reader As Sql.MySqlDataReader) As TrackMetadata
        Return New TrackMetadata(Reader.GetInt64("InitialTweetId"), _
                                 Reader.GetInt64("InitialTweetUserId"), _
                                 Reader.GetString("InitialTweetFullText"),
                                 Reader.GetString("RelevantHashtags").Split(" "c))
    End Function

    Private Shared Function RowToTweet(Reader As Sql.MySqlDataReader) As Tweet
        Return New Tweet(Reader.GetEntityId("Id"), _
                         Reader.GetInt64("TweetId"), _
                         Reader.GetString("ContentHash"), _
                         Reader.GetDateTime("PublishDateTime"), _
                         DirectCast(Reader.GetInt32("LocationType"), TweetLocationType), _
                         Reader.GetString("Location"))
    End Function

    ''Possibly obsolete. Replaced by .Select(Function(Row) RowToTweet(Row))
    'Private Function CreateQueryToTweetEnumerable(Query As IEnumerable(Of Sql.MySqlDataReader)) As IEnumerable(Of Tweet)
    '    Return New DelegateEnumerable(Of Tweet)( _
    '        Function()
    '            Return New MySqlRowEnumerator(Of Tweet)( _
    '                Query.GetEnumerator, _
    '                Function(Reader)
    '                    Return RowToTweet(Reader)
    '                End Function)
    '        End Function)
    'End Function

#End Region

#Region "Researcher"

    Public Function GetTrackMetadata(TrackEntityId As EntityId) As TrackMetadata?
        Dim TrackTableIdentifier = GetTableIdentifier(Relations.TableNames.MetadataTableName(TrackEntityId).Escape)
        Using Row = ExecuteSingleRowQuery(False, FormatSqlIdentifiers("SELECT * FROM {0}", TrackTableIdentifier))
            If Row Is Nothing Then
                Return Nothing
            Else
                Return RowToTrackMetadata(Row.Reader)
            End If
        End Using
    End Function

    Public Function GetAllTweets(TrackEntityId As EntityId) As IEnumerable(Of Tweet)
        Dim TweetTableIdentifier = GetTableIdentifier(Relations.TableNames.TweetTableName(TrackEntityId).Escape)
        Return ExecuteQuery(FormatSqlIdentifiers("SELECT * FROM {0}", TweetTableIdentifier)).Select(Function(Row) RowToTweet(Row))
    End Function

    Public Function GetTweetsSinceEntityId(TrackEntityId As EntityId, LastTweetEntityIdExclusiveToResultSet As EntityId) As IEnumerable(Of Tweet)
        Dim TweetTableIdentifier = GetTableIdentifier(Relations.TableNames.TweetTableName(TrackEntityId).Escape)
        Return ExecuteQuery(FormatSqlIdentifiers("SELECT * FROM {0} WHERE `Id` > @LastTweetEntityIdExclusiveToResultSet", TweetTableIdentifier),
                                                 New CommandParameter("@LastTweetEntityIdExclusiveToResultSet", LastTweetEntityIdExclusiveToResultSet)) _
                   .Select(Function(Row) RowToTweet(Row))
    End Function

#End Region

#Region "Administrator"

    Public Function CreateTrack(ResearcherPassword As String) As CreateTrackResult
        'Insert Track
        'Create Metadata Table
        'Create Tweet Table
        'Create User
        'Assign Select, Insert, Update, Delete Acces to Metadata, Tweet Table
        'Flush

        Dim TrackTableIdentifier = GetTableIdentifier(New VerbatimIdentifier("Track").Escape)
        Dim TrackEntityId = ExecuteNonQuery(FormatSqlIdentifiers("INSERT INTO {0}() VALUES ()", TrackTableIdentifier)).InsertId

        Dim MetadataTableIdentifier = GetTableIdentifier(Relations.TableNames.MetadataTableName(TrackEntityId).Escape)
        ExecuteNonQuery(New SqlQueryString( _
            "CREATE TABLE " & MetadataTableIdentifier.EscapedText & " (  " & _
            "    `InitialTweetId` BIGINT(20) NOT NULL,                   " & _
            "    `InitialTweetUserId` BIGINT(20) NOT NULL,               " & _
            "    `InitialTweetFullText` TEXT NOT NULL,                   " & _
            "    `RelevantHashtags` TEXT NOT NULL)                       " & _
            "    ENGINE = InnoDB;                                        "))

        Dim TweetTableIdentifier = GetTableIdentifier(Relations.TableNames.TweetTableName(TrackEntityId).Escape)
        ExecuteNonQuery(New SqlQueryString( _
            "CREATE TABLE " & TweetTableIdentifier.EscapedText & " (  " & _
            "    `Id` INT NOT NULL AUTO_INCREMENT,                    " & _
            "    `TweetId` BIGINT(20) NOT NULL,                       " & _
            "    `ContentHash` TEXT NOT NULL,                         " & _
            "    `PublishDateTime` DATETIME NOT NULL,                 " & _
            "    `LocationType` INT NOT NULL,                         " & _
            "    `Location` TEXT NULL,                                " & _
            "    PRIMARY KEY (`Id`))                                  " & _
            "    ENGINE = InnoDB;                                     "))

        Dim ResearcherName As String = Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId)
        ExecuteNonQuery(FormatSqlIdentifiers("CREATE USER @ResearcherName IDENTIFIED BY @ResearcherPassword;"), _
                        New CommandParameter("@ResearcherName", ResearcherName), _
                        New CommandParameter("@ResearcherPassword", ResearcherPassword))

        ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO @ResearcherName;", MetadataTableIdentifier), _
                        New CommandParameter("@ResearcherName", ResearcherName))
        ExecuteNonQuery(FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO @ResearcherName;", TweetTableIdentifier), _
                        New CommandParameter("@ResearcherName", ResearcherName))
        ExecuteNonQuery(FormatSqlIdentifiers("FLUSH PRIVILEGES;"))

        Return New CreateTrackResult(New Track(TrackEntityId, Nothing), New DatabaseUser(ResearcherName, ResearcherPassword))
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

        Public Sub New(NewTrack As Track, NewResearcherUser As DatabaseUser)
            _Track = NewTrack
            _ResearcherUser = NewResearcherUser
        End Sub

    End Structure

    Public Function GetAllTracksWithoutMetadata() As IEnumerable(Of Track)
        Dim TrackTableIdentifier = GetTableIdentifier(New VerbatimIdentifier("Track").Escape)
        Return ExecuteQuery(FormatSqlIdentifiers("SELECT * FROM {0}", TrackTableIdentifier)).Select(Function(Row) New Track(Row.GetEntityId("Id"), Nothing))
    End Function

    Public Sub DeleteTrack(TrackEntityId As EntityId)

    End Sub

#End Region

#Region "Root"

    Public Sub DeleteDatabase()
        Dim Tracks = GetAllTracksWithoutMetadata.ToList
        For Each i In Tracks
            Dim ResearcherToDrop = Relations.UserNames.ResearcherUserName(DatabaseName, i.EntityId)
            ExecuteNonQuery(FormatSqlIdentifiers("DROP USER @ResearcherToDrop"), New CommandParameter("@ResearcherToDrop", ResearcherToDrop))
        Next
        Dim AdministratorToDrop = Relations.UserNames.AdministratorUserName(DatabaseName)
        ExecuteNonQuery(FormatSqlIdentifiers("DROP USER @AdministratorToDrop"), New CommandParameter("@AdministratorToDrop", AdministratorToDrop))
        ExecuteNonQuery(FormatSqlIdentifiers("DROP DATABASE {0}", DatabaseName.Escape))
        ExecuteNonQuery(FormatSqlIdentifiers("FLUSH PRIVILEGES;"))
    End Sub

#End Region

End Class
