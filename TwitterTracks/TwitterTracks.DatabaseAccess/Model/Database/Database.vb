
Imports Sql = MySql.Data.MySqlClient

Public Class Database
    Inherits ConnectionAccessor

    Public Shared Function CreateTrackDatabase(Connection As DatabaseConnection, DatabaseName As String) As Database
        Dim Result As New Database(Connection, DatabaseName)
        Result.ExecuteNonQuery(String.Format("CREATE DATABASE `{0}`", DatabaseName))
        Return Result
    End Function

    Dim _DatabaseName As String
    Public ReadOnly Property DatabaseName As String
        <DebuggerStepThrough()>
        Get
            Return _DatabaseName
        End Get
    End Property

    Public Sub New(NewConnection As DatabaseConnection, NewDatabaseName As String)
        MyBase.New(NewConnection)
        _DatabaseName = NewDatabaseName
    End Sub

    Private Function GetWildcardTableIdentifier() As String
        Return String.Format("`{0}`.*", DatabaseName)
    End Function

    Private Function GetTableIdentifier(TableName As String) As String
        Return String.Format("`{0}`.`{1}`", DatabaseName, TableName)
    End Function

    Private Function GetTableIdentifier(TableNameFormat As String, ParamArray Arguments As Object()) As String
        Return String.Format("`{0}`.`{1}`", DatabaseName, String.Format(TableNameFormat, Arguments))
    End Function

#Region "RowToModel"

    Private Shared Function RowToTrackMetadata(Reader As Sql.MySqlDataReader) As TrackMetadata
        Return New TrackMetadata(Reader.GetInt64("InitialTweetId"), _
                                 Reader.GetInt64("InitialTweetUserId"), _
                                 Reader.GetString("InitialTweetFullText"),
                                 Reader.GetString("RelevantHashtags").Split(" "c))
    End Function

    Private Shared Function RowToTweet(Reader As Sql.MySqlDataReader) As Tweet
        Return New Tweet(Reader.GetInt64("Id"), _
                         Reader.GetInt64("TweetId"), _
                         Reader.GetString("ContentHash"), _
                         Reader.GetDateTime("PublishDateTime"), _
                         DirectCast(Reader.GetInt32("LocationType"), TweetLocationType), _
                         Reader.GetString("Location"))
    End Function

    Private Function CreateQueryToTweetEnumerable(Query As IEnumerable(Of Sql.MySqlDataReader)) As IEnumerable(Of Tweet)
        Return New DelegateEnumerable(Of Tweet)( _
            Function()
                Return New MySqlRowEnumerator(Of Tweet)( _
                    Query.GetEnumerator, _
                    Function(Reader)
                        Return RowToTweet(Reader)
                    End Function)
            End Function)
    End Function

#End Region

#Region "Researcher"

    Public Function GetTrackMetadata(TrackEntityId As Int64) As TrackMetadata?
        Using Row = ExecuteSingleRowQuery(False, String.Format("SELECT * FROM {0}", GetTableIdentifier(Relations.MetadataTableName(TrackEntityId))))
            If Row Is Nothing Then
                Return Nothing
            Else
                Return RowToTrackMetadata(Row.Reader)
            End If
        End Using
    End Function

    Public Function GetAllTweets(TrackEntityId As Int64) As IEnumerable(Of Tweet)
        Return CreateQueryToTweetEnumerable(ExecuteQuery(String.Format("SELECT * FROM {0}", GetTableIdentifier(Relations.TweetTableName(TrackEntityId)))))
    End Function

    Public Function GetTweetsSinceEntityId(TrackEntityId As Int64, LastTweetEntityIdExclusiveToResultSet As Int64) As IEnumerable(Of Tweet)
        Return CreateQueryToTweetEnumerable(ExecuteQuery(String.Format("SELECT * FROM {0} WHERE `Id` > @LastTweetEntityIdExclusiveToResultSet", GetTableIdentifier(Relations.TweetTableName(TrackEntityId))),
                                                         New CommandParameter("@LastTweetEntityIdExclusiveToResultSet", "LastTweetEntityIdExclusiveToResultSet")))
    End Function

#End Region

#Region "Administrator"

    Public Function CreateResearcherObjects(ResearcherPassword As String) As CreateResearcherObjectsResult
        'Insert Track
        'Create Metadata Table
        'Create Tweet Table
        'Create User
        'Assign Select, Insert, Update, Delete Acces to Metadata, Tweet Table
        'Flush

        Dim TrackEntityId = ExecuteNonQuery(String.Format("INSERT INTO {0}() VALUES ()", GetTableIdentifier("Track"))).InsertId

        Dim MetadataTableIdentifier = GetTableIdentifier(Relations.MetadataTableName(TrackEntityId))
        ExecuteNonQuery("CREATE TABLE " & MetadataTableIdentifier & " (  " & _
                        "    `InitialTweetId` BIGINT(20) NOT NULL,       " & _
                        "    `InitialTweetUserId` BIGINT(20) NOT NULL,   " & _
                        "    `InitialTweetFullText` TEXT NOT NULL,       " & _
                        "    `RelevantHashtags` TEXT NOT NULL)           " & _
                        "    ENGINE = InnoDB;                            ")

        Dim TweetTableIdentifier = GetTableIdentifier(Relations.TweetTableName(TrackEntityId))
        ExecuteNonQuery("CREATE TABLE " & TweetTableIdentifier & " (  " & _
                        "    `Id` INT NOT NULL AUTO_INCREMENT,        " & _
                        "    `TweetId` BIGINT(20) NOT NULL,           " & _
                        "    `ContentHash` TEXT NOT NULL,             " & _
                        "    `PublishDateTime` DATETIME NOT NULL,     " & _
                        "    `LocationType` INT NOT NULL,             " & _
                        "    `Location` TEXT NULL,                    " & _
                        "    PRIMARY KEY (`Id`))                      " & _
                        "    ENGINE = InnoDB;                         ")

        Dim ResearcherName = String.Format("{0}_{1}_Researcher", DatabaseName, TrackEntityId)
        Dim ResearcherIdentifier = String.Format("'{0}'@'%'", ResearcherName)
        ExecuteNonQuery(String.Format("CREATE USER {0} IDENTIFIED BY '{1}';", ResearcherIdentifier, ResearcherPassword))

        ExecuteNonQuery(String.Format("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO {1};", MetadataTableIdentifier, ResearcherIdentifier))
        ExecuteNonQuery(String.Format("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO {1};", TweetTableIdentifier, ResearcherIdentifier))
        ExecuteNonQuery("FLUSH PRIVILEGES;")

        Return New CreateResearcherObjectsResult(TrackEntityId, New DatabaseUser(ResearcherName, ResearcherPassword))
    End Function

    Public Structure CreateResearcherObjectsResult

        Private _TrackEntityId As Int64
        Public ReadOnly Property TrackEntityId As Int64
            <DebuggerStepThrough()>
            Get
                Return _TrackEntityId
            End Get
        End Property

        Private _ResearcherUser As DatabaseUser
        Public ReadOnly Property ResearcherUser As DatabaseUser
            <DebuggerStepThrough()>
            Get
                Return _ResearcherUser
            End Get
        End Property

        Public Sub New(NewTrackEntityId As Int64, NewResearcherUser As DatabaseUser)
            _TrackEntityId = NewTrackEntityId
            _ResearcherUser = NewResearcherUser
        End Sub

    End Structure

    Public Function GetAllTracks() As IEnumerable(Of Track)
        Dim Ids = ExecuteQuery(String.Format("SELECT * FROM {0}", GetTableIdentifier("Track"))).Select(Function(i) i.GetInt64("Id")).ToList
        Return Ids.Select( _
            Function(Id)
                Dim Metadata = GetTrackMetadata(Id)
                Return New Track(Id, Metadata)
            End Function)
    End Function

#End Region

    'ToDo: BeginTransaction

#Region "Root"

    Public Function CreateAdministratorObjects(AdministratorPassword As String) As CreateAdministratorObjectsResult
        'Create Track Table
        'Create User
        'Grant Create, Drop to Database.* Table
        'Grant Select, Insert, Update, Delete to Database.Track
        'Flush

        Dim TrackTableIdentifier = GetTableIdentifier("Track")
        ExecuteNonQuery("CREATE TABLE " & TrackTableIdentifier & " ( " & _
                        "  `Id` INT NOT NULL AUTO_INCREMENT,         " & _
                        "  PRIMARY KEY (`Id`))                       " & _
                        "ENGINE = InnoDB;                            ")

        Dim AdministratorName = Relations.AdministratorUserName(DatabaseName)
        Dim AdministratorIdentifier = String.Format("'{0}'@'%'", AdministratorName)
        ExecuteNonQuery(String.Format("CREATE USER {0} IDENTIFIED BY '{1}';", AdministratorIdentifier, AdministratorPassword))

        ExecuteNonQuery(String.Format("GRANT CREATE, DROP ON {0} TO {1};", GetWildcardTableIdentifier, AdministratorIdentifier))
        ExecuteNonQuery(String.Format("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO {1};", TrackTableIdentifier, AdministratorIdentifier))
        ExecuteNonQuery("FLUSH PRIVILEGES;")

        Return New CreateAdministratorObjectsResult(New DatabaseUser(AdministratorName, AdministratorPassword))
    End Function

    Public Structure CreateAdministratorObjectsResult

        Private _AdministratorUser As DatabaseUser
        Public ReadOnly Property AdministratorUser As DatabaseUser
            <DebuggerStepThrough()>
            Get
                Return _AdministratorUser
            End Get
        End Property

        Public Sub New(NewAdministratorUser As DatabaseUser)
            _AdministratorUser = NewAdministratorUser
        End Sub

    End Structure

#End Region

End Class
