
Imports Sql = MySql.Data.MySqlClient

Public Class ResearcherDatabase
    Inherits DatabaseBase

    Dim _DatabaseName As VerbatimIdentifier
    Public ReadOnly Property DatabaseName As VerbatimIdentifier
        <DebuggerStepThrough()>
        Get
            Return _DatabaseName
        End Get
    End Property

    Dim _TrackEntityId As EntityId
    Public ReadOnly Property TrackEntityId As EntityId
        <DebuggerStepThrough()>
        Get
            Return _TrackEntityId
        End Get
    End Property

    Public Sub New(NewConnection As DatabaseConnection, NewDatabaseName As VerbatimIdentifier, NewTrackEntityId As EntityId)
        MyBase.New(NewConnection)
        _DatabaseName = NewDatabaseName
        _TrackEntityId = NewTrackEntityId
    End Sub

#Region "RowToModel"

    Private Shared Function RowToTrackMetadata(Reader As Sql.MySqlDataReader) As TrackMetadata
        Return New TrackMetadata(Reader.GetInt64("InitialTweetId"), _
                                 Reader.GetInt64("InitialTweetUserId"), _
                                 Reader.GetString("InitialTweetFullText"),
                                 Reader.GetString("RelevantKeywords").Split(" "c))
    End Function

    Private Shared Function RowToTweet(Reader As Sql.MySqlDataReader) As Tweet
        Return New Tweet(Reader.GetEntityId("Id"), _
                         Reader.GetInt64("TweetId"), _
                         Reader.GetString("ContentHash"), _
                         Reader.GetDateTime("PublishDateTime"), _
                         DirectCast(Reader.GetInt32("LocationType"), TweetLocationType), _
                         Reader.GetString("Location"))
    End Function

#End Region

    Public Function TryGetTrackMetadata() As TrackMetadata?
        Dim TrackTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.MetadataTableName(TrackEntityId).Escape)
        Using Row = ExecuteSingleRowQuery(False, FormatSqlIdentifiers("SELECT * FROM {0}", TrackTableIdentifier))
            If Row Is Nothing Then
                Return Nothing
            Else
                Return RowToTrackMetadata(Row.Reader)
            End If
        End Using
    End Function

    Public Function GetAllTweets() As IEnumerable(Of Tweet)
        Dim TweetTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.TweetTableName(TrackEntityId).Escape)
        Return ExecuteQuery(FormatSqlIdentifiers("SELECT * FROM {0}", TweetTableIdentifier)).Select(Function(Row) RowToTweet(Row))
    End Function

    Public Function GetTweetsSinceEntityId(LastTweetEntityIdExclusiveToResultSet As EntityId) As IEnumerable(Of Tweet)
        Dim TweetTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.TweetTableName(TrackEntityId).Escape)
        Return ExecuteQuery(FormatSqlIdentifiers("SELECT * FROM {0} WHERE `Id` > @LastTweetEntityIdExclusiveToResultSet", TweetTableIdentifier),
                                                 New CommandParameter("@LastTweetEntityIdExclusiveToResultSet", LastTweetEntityIdExclusiveToResultSet)) _
                   .Select(Function(Row) RowToTweet(Row))
    End Function

    Public Sub DeleteTrack()
        Dim ResearcherToDrop = Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId)
        For Each Host In {"%", "localhost"}
            Dim ResearcherIdentifier = Relations.UserNames.UserIdentifier(New VerbatimIdentifier(ResearcherToDrop).Escape, New VerbatimIdentifier(Host).Escape)
            ExecuteNonQuery(FormatSqlIdentifiers("DROP USER {0}", ResearcherIdentifier))
        Next        
        ExecuteNonQuery(FormatSqlIdentifiers("FLUSH PRIVILEGES;"))
    End Sub

End Class
