
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

    Private Function GetTweetTableIdentifier() As EscapedIdentifier
        Return Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.TweetTableName(TrackEntityId).Escape)
    End Function

    Private Function GetMetadataTableIdentifier() As EscapedIdentifier
        Return Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.TableNames.MetadataTableName(TrackEntityId).Escape)
    End Function

#Region "RowToModel"

    Private Shared Function RowToTrackMetadata(Reader As Sql.MySqlDataReader) As TrackMetadata
        Dim TweetId = Reader.GetNullableInt64("TweetId")
        Dim CreatedByUserId = Reader.GetNullableInt64("CreatedByUserId")
        If (TweetId Is Nothing) <> (CreatedByUserId Is Nothing) Then
            Throw New DataException("The nullity of TweetId and CreatedByUserId does not match.")
        End If
        Dim IsPublished = TweetId IsNot Nothing
        Return New TrackMetadata(IsPublished, _
                                 If(IsPublished, TweetId.Value, 0), _
                                 If(IsPublished, CreatedByUserId.Value, 0), _
                                 Reader.GetString("TweetText"), _
                                 Reader.GetString("RelevantKeywords").Split(" "c), _
                                 Reader.GetString("MediaFilePathsToAdd").Split("|"c), _
                                 Reader.GetString("ConsumerKey"),
                                 Reader.GetString("ConsumerSecret"),
                                 Reader.GetString("AccessToken"),
                                 Reader.GetString("AccessTokenSecret"))
    End Function

    Private Shared Function RowToTweet(Reader As Sql.MySqlDataReader) As Tweet
        Return New Tweet(Reader.GetEntityId("Id"), _
                         Reader.GetString("ContentHash"), _
                         Helpers.UnixTimestampToUtc(Reader.GetInt64("PublishDateTime")), _
                         TweetLocation.ParseDatabaseValue(DirectCast(Reader.GetInt32("LocationType"), TweetLocationType), _
                                                          Reader.GetNullableString("UserRegion"), _
                                                          Reader.GetNullableDouble("Latitude"), _
                                                          Reader.GetNullableDouble("Longitude")), _
                         Reader.GetNullableInt64("Debug_TweetId"), _
                         Reader.GetNullableString("Debug_TweetContent"))
    End Function

#End Region

    Public Function TryGetTrackMetadata() As TrackMetadata?
        Using Row = ExecuteSingleRowQuery(False, FormatSqlIdentifiers("SELECT * FROM {0}", GetMetadataTableIdentifier))
            If Row Is Nothing Then
                Return Nothing
            Else
                Return RowToTrackMetadata(Row.Reader)
            End If
        End Using
    End Function

    Public Sub UpdateOrCreateTrackMetadata(Metadata As TrackMetadata)
        Try
            BeginTransaction()

            Dim MetadataTableIdentifier = GetMetadataTableIdentifier()

            Dim FormattedQueryString As SqlQueryString
            If TryGetTrackMetadata() Is Nothing Then
                FormattedQueryString = FormatSqlIdentifiers( _
                    "INSERT INTO {0} (             " & _
                    "    `TweetId`,                " & _
                    "    `CreatedByUserId`,        " & _
                    "    `TweetText`,              " & _
                    "    `RelevantKeywords`,       " & _
                    "    `MediaFilePathsToAdd`,    " & _
                    "    `ConsumerKey`,            " & _
                    "    `ConsumerSecret`,         " & _
                    "    `AccessToken`,            " & _
                    "    `AccessTokenSecret`)      " & _
                    "VALUES (@TweetId,             " & _
                    "        @CreatedByUserId,     " & _
                    "        @TweetText,           " & _
                    "        @RelevantKeywords,    " & _
                    "        @MediaFilePathsToAdd, " & _
                    "        @ConsumerKey,         " & _
                    "        @ConsumerSecret,      " & _
                    "        @AccessToken,         " & _
                    "        @AccessTokenSecret)   ", MetadataTableIdentifier)
            Else
                FormattedQueryString = FormatSqlIdentifiers( _
                    "UPDATE {0}                                         " & _
                    "SET `TweetId`             = @TweetId,              " & _
                    "    `CreatedByUserId`     = @CreatedByUserId,      " & _
                    "    `TweetText`           = @TweetText,            " & _
                    "    `RelevantKeywords`    = @RelevantKeywords,     " & _
                    "    `MediaFilePathsToAdd` = @MediaFilePathsToAdd,  " & _
                    "    `ConsumerKey`         = @ConsumerKey,          " & _
                    "    `ConsumerSecret`      = @ConsumerSecret,       " & _
                    "    `AccessToken`         = @AccessToken,          " & _
                    "    `AccessTokenSecret`   = @AccessTokenSecret     ", MetadataTableIdentifier)
            End If
            ExecuteNonQuery(FormattedQueryString, _
                            New CommandParameter("@TweetId", If(Metadata.IsPublished, DirectCast(Metadata.TweetId, Object), Nothing)), _
                            New CommandParameter("@CreatedByUserId", If(Metadata.IsPublished, DirectCast(Metadata.CreatedByUserId, Object), Nothing)), _
                            New CommandParameter("@TweetText", Metadata.TweetText), _
                            New CommandParameter("@RelevantKeywords", String.Join(" ", Metadata.RelevantKeywords)), _
                            New CommandParameter("@MediaFilePathsToAdd", String.Join("|", Metadata.MediaFilePathsToAdd)), _
                            New CommandParameter("@ConsumerKey", Metadata.ConsumerKey), _
                            New CommandParameter("@ConsumerSecret", Metadata.ConsumerSecret), _
                            New CommandParameter("@AccessToken", Metadata.AccessToken), _
                            New CommandParameter("@AccessTokenSecret", Metadata.AccessTokenSecret))

            CommitTransaction()
        Finally
            EndTransaction()
        End Try
    End Sub

    Public Function GetAllTweets() As IEnumerable(Of Tweet)
        Dim TweetTableIdentifier = GetTweetTableIdentifier()
        Return ExecuteQuery(FormatSqlIdentifiers("SELECT * FROM {0}", TweetTableIdentifier)).Select(Function(Row) RowToTweet(Row))
    End Function

    Public Function CountAllTweets() As Int64
        Dim TweetTableIdentifier = GetTweetTableIdentifier()
        Return ExecuteScalar(Of Int64)(FormatSqlIdentifiers("SELECT COUNT(*) FROM {0}", TweetTableIdentifier))
    End Function

    Public Sub CreateTweet(ContentHash As String, PublishDateTime As DateTime, Location As TweetLocation, Debug_TweetId As Int64?, Debug_TweetContent As String)
        Dim TweetTableIdentifier = GetTweetTableIdentifier()
        ExecuteNonQuery(FormatSqlIdentifiers("INSERT INTO {0} " & _
                                             "(`ContentHash`, `PublishDateTime`, `LocationType`, `UserRegion`, `Latitude`, `Longitude`, `Debug_TweetId`, `Debug_TweetContent`) " & _
                                             "VALUES (@ContentHash, @PublishDateTime, @LocationType, @UserRegion, @Latitude, @Longitude, @Debug_TweetId, @Debug_TweetContent)", TweetTableIdentifier), _
                        New CommandParameter("@ContentHash", ContentHash), _
                        New CommandParameter("@PublishDateTime", PublishDateTime.ToUnixTimestamp), _
                        New CommandParameter("@LocationType", Location.LocationType), _
                        New CommandParameter("@UserRegion", Location.UserRegion), _
                        New CommandParameter("@Latitude", Location.GetDatabaseValueLatitude), _
                        New CommandParameter("@Longitude", Location.GetDatabaseValueLongitude), _
                        New CommandParameter("@Debug_TweetId", Debug_TweetId), _
                        New CommandParameter("@Debug_TweetContent", Debug_TweetContent))
    End Sub

    Public Function TryUpdateTweetUserRegionWithCoordinates(Id As EntityId, Latitude As Double, Longitude As Double) As Boolean
        Try
            BeginTransaction()
            Using Row = ExecuteSingleRowQuery(True, FormatSqlIdentifiers("SELECT * FROM {0} WHERE `Id` = @Id", GetTweetTableIdentifier), New CommandParameter("@Id", Id.RawId))
                If DirectCast(Row.Reader.GetInt32("LocationType"), TweetLocationType) <> TweetLocationType.UserRegionWithPotentialForCoordinates Then
                    Return False
                End If
            End Using
            ExecuteNonQuery(FormatSqlIdentifiers("UPDATE {0} SET `LocationType` = @LocationType, `Latitude` = @Latitude, `Longitude` = @Longitude WHERE `Id` = @Id", GetTweetTableIdentifier), _
                            New CommandParameter("@LocationType", TweetLocationType.UserRegionWithCoordinates), _
                            New CommandParameter("@Latitude", Latitude), _
                            New CommandParameter("@Longitude", Longitude), _
                            New CommandParameter("@Id", Id.RawId))
            CommitTransaction()
        Finally
            EndTransaction()
        End Try
        Return True
    End Function

    Public Function TryUpdateTweetToUserRegionNoCoordinates(Id As EntityId) As Boolean
        Try
            BeginTransaction()
            Using Row = ExecuteSingleRowQuery(True, FormatSqlIdentifiers("SELECT * FROM {0} WHERE `Id` = @Id", GetTweetTableIdentifier), New CommandParameter("@Id", Id.RawId))
                If DirectCast(Row.Reader.GetInt32("LocationType"), TweetLocationType) <> TweetLocationType.UserRegionWithPotentialForCoordinates Then
                    Return False
                End If
            End Using
            ExecuteNonQuery(FormatSqlIdentifiers("UPDATE {0} SET `LocationType` = @LocationType WHERE `Id` = @Id", GetTweetTableIdentifier), _
                            New CommandParameter("@LocationType", TweetLocationType.UserRegionNoCoordinates), _
                            New CommandParameter("@Id", Id.RawId))
            CommitTransaction()
        Finally
            EndTransaction()
        End Try
        Return True
    End Function

    Public Function GetTweetsSinceEntityId(LastTweetEntityIdExclusiveToResultSet As EntityId) As IEnumerable(Of Tweet)
        Dim TweetTableIdentifier = GetTweetTableIdentifier()
        Return ExecuteQuery(FormatSqlIdentifiers("SELECT * FROM {0} WHERE `Id` > @LastTweetEntityIdExclusiveToResultSet", TweetTableIdentifier),
                                                 New CommandParameter("@LastTweetEntityIdExclusiveToResultSet", LastTweetEntityIdExclusiveToResultSet.RawId)) _
                   .Select(Function(Row) RowToTweet(Row))
    End Function

    Public Sub DeleteTrack()
        Try
            BeginTransaction()

            Dim ResearcherToDrop = Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId)
            For Each Host In {"%", "localhost"}
                Dim ResearcherIdentifier = Relations.UserNames.UserIdentifier(New VerbatimIdentifier(ResearcherToDrop).Escape, New VerbatimIdentifier(Host).Escape)
                ExecuteNonQuery(FormatSqlIdentifiers("DROP USER {0}", ResearcherIdentifier))
            Next
            ExecuteNonQuery(FormatSqlIdentifiers("DROP TABLE {0};", GetTweetTableIdentifier))
            ExecuteNonQuery(FormatSqlIdentifiers("DROP TABLE {0};", GetMetadataTableIdentifier))
            Dim TrackTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, New VerbatimIdentifier(Relations.TableNames.TrackTableName).Escape)
            ExecuteNonQuery(FormatSqlIdentifiers("DELETE FROM {0} WHERE `Id` = @Id;", TrackTableIdentifier), New CommandParameter("@Id", TrackEntityId.RawId))
            ExecuteNonQuery(FormatSqlIdentifiers("FLUSH PRIVILEGES;"))

            CommitTransaction()
        Finally
            EndTransaction()
        End Try
    End Sub

End Class
