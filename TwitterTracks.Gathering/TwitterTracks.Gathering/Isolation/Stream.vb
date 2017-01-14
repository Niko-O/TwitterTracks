
Imports System.Threading.Tasks

Public Class Stream

    Public Event TweetReceived(Tweet As Tweetinvi.Models.ITweet, MatchOn As TwitterTracks.TweetinviInterop.TweetinviMatchOn, AdditionalData As String)
    Private Sub OnTweetReceived(Tweet As Tweetinvi.Models.ITweet, MatchOn As TwitterTracks.TweetinviInterop.TweetinviMatchOn, AdditionalData As String)
        RaiseEvent TweetReceived(Tweet, MatchOn, AdditionalData)
    End Sub

    Public Event Started()
    Private Sub OnStarted()
        RaiseEvent Started()
    End Sub

    Public Event Stopped As EventHandler(Of TwitterTracks.TweetinviInterop.StreamStoppedEventArgs)
    Protected Overridable Sub OnStopped(e As TwitterTracks.TweetinviInterop.StreamStoppedEventArgs)
        RaiseEvent Stopped(Me, e)
    End Sub

    Public ReadOnly Property IsRunning As Boolean
        Get
            Return Stream IsNot Nothing AndAlso Stream.StreamState = Tweetinvi.Models.StreamState.Running
        End Get
    End Property

    Public Property OriginalTweetId As Int64
    Public Property OriginalTweetCreatedByUserId As Int64
    Public Property RelevantKeywords As ReadOnlyCollection(Of String)
    Public Property TwitterCredentials As Tweetinvi.Models.TwitterCredentials

    Dim WithEvents Stream As Tweetinvi.Streaming.IFilteredStream = Nothing
    Dim StreamTask As Task = Nothing

    Public Sub Start()
        'ToDo: Handle Resume
        Stream = Tweetinvi.Stream.CreateFilteredStream(TwitterCredentials)
        Stream.AddFollow(OriginalTweetCreatedByUserId, AddressOf UserPublishedTweetCallback)
        Stream.AddTrack(String.Join(" ", RelevantKeywords), AddressOf TweetReceivedByKeywordsCallback)
        StreamTask = Stream.StartStreamMatchingAnyConditionAsync()
    End Sub

    Public Sub [Stop]()
        Stream.StopStream()
        Stream = Nothing
    End Sub

    Private Sub UserPublishedTweetCallback(Tweet As Tweetinvi.Models.ITweet)
        OnTweetReceived(Tweet, TweetinviInterop.TweetinviMatchOn.None, "UserPublishedTweetCallback")
    End Sub

    Private Sub TweetReceivedByKeywordsCallback(Tweet As Tweetinvi.Models.ITweet)
        OnTweetReceived(Tweet, TweetinviInterop.TweetinviMatchOn.None, "TweetReceivedByKeywordsCallback")
    End Sub

    Private Sub Stream_MatchingTweetReceived(sender As Object, e As Tweetinvi.Events.MatchedTweetReceivedEventArgs) Handles Stream.MatchingTweetReceived
        OnTweetReceived(e.Tweet, DirectCast(e.MatchOn, TwitterTracks.TweetinviInterop.TweetinviMatchOn), "Stream_MatchingTweetReceived")

        'Command.Parameters.AddWithValue("@TweetId", e.Tweet.Id)
        'Command.Parameters.AddWithValue("@UserId", e.Tweet.CreatedBy.Id)
        'Command.Parameters.AddWithValue("@UserName", e.Tweet.CreatedBy.Name)
        'Command.Parameters.AddWithValue("@FullText", e.Tweet.FullText)
        'If e.Tweet.RetweetedTweet Is Nothing Then
        '    Command.Parameters.AddWithValue("@RetweetTweetId", Nothing)
        'Else
        '    Command.Parameters.AddWithValue("@RetweetTweetId", e.Tweet.RetweetedTweet.Id)
        'End If
        'If e.Tweet.Coordinates Is Nothing Then
        '    Command.Parameters.AddWithValue("@LocationLatitude", Nothing)
        '    Command.Parameters.AddWithValue("@LocationLongitude", Nothing)
        'Else
        '    Command.Parameters.AddWithValue("@LocationLatitude", e.Tweet.Coordinates.Latitude)
        '    Command.Parameters.AddWithValue("@LocationLongitude", e.Tweet.Coordinates.Longitude)
        'End If
        'Command.Parameters.AddWithValue("@UserLocation", e.Tweet.CreatedBy.Location)
    End Sub

    Private Sub Stream_DisconnectMessageReceived(sender As Object, e As Tweetinvi.Events.DisconnectedEventArgs) Handles Stream.DisconnectMessageReceived
        OnStopped(TwitterTracks.TweetinviInterop.StreamStoppedEventArgs.Disconnect(e.DisconnectMessage.Code, e.DisconnectMessage.Reason, e.DisconnectMessage.StreamName))
    End Sub

    Private Sub Stream_LimitReached(sender As Object, e As Tweetinvi.Events.LimitReachedEventArgs) Handles Stream.LimitReached
        OnStopped(TwitterTracks.TweetinviInterop.StreamStoppedEventArgs.LimitReached(e.NumberOfTweetsNotReceived))
    End Sub

    Private Sub Stream_StreamStopped(sender As Object, e As Tweetinvi.Events.StreamExceptionEventArgs) Handles Stream.StreamStopped
        If e.DisconnectMessage Is Nothing Then
            OnStopped(TwitterTracks.TweetinviInterop.StreamStoppedEventArgs.Stopped(e.Exception, -1, Nothing, Nothing))
        Else
            OnStopped(TwitterTracks.TweetinviInterop.StreamStoppedEventArgs.Stopped(e.Exception, e.DisconnectMessage.Code, e.DisconnectMessage.Reason, e.DisconnectMessage.StreamName))
        End If
    End Sub

    Private Sub Stream_NonMatchingTweetReceived(sender As Object, e As Tweetinvi.Events.TweetEventArgs) Handles Stream.NonMatchingTweetReceived
        'ToDo: Should not matter.
    End Sub

    Private Sub Stream_StreamPaused(sender As Object, e As EventArgs) Handles Stream.StreamPaused
        Throw New NopeException("The Stream should never be paused.")
    End Sub

    Private Sub Stream_StreamResumed(sender As Object, e As EventArgs) Handles Stream.StreamResumed
        'ToDo: Happens on start too.
    End Sub

    Private Sub Stream_StreamStarted(sender As Object, e As EventArgs) Handles Stream.StreamStarted
        OnStarted()
    End Sub

    Private Sub Stream_TweetDeleted(sender As Object, e As Tweetinvi.Events.TweetDeletedEventArgs) Handles Stream.TweetDeleted
        'ToDo. Do I need this?
        'TweetDeletedWindow.AddMessage(e.TweetDeletedInfo.Id.ToString)
    End Sub

    Private Sub Stream_TweetLocationInfoRemoved(sender As Object, e As Tweetinvi.Events.TweetLocationDeletedEventArgs) Handles Stream.TweetLocationInfoRemoved
        'ToDo. Do I need this?
        'TweetLocationInfoRemovedWindow.AddMessage(e.TweetLocationRemovedInfo.UpToStatusId.ToString)
    End Sub

    Private Sub Stream_TweetWitheld(sender As Object, e As Tweetinvi.Events.TweetWitheldEventArgs) Handles Stream.TweetWitheld
        'ToDo. Do I need this?
        'TweetWitheldWindow.AddMessage(e.TweetWitheldInfo.Id & ", " & e.TweetWitheldInfo.UserId)
    End Sub

    Private Sub Stream_UnmanagedEventReceived(sender As Object, e As Tweetinvi.Events.UnmanagedMessageReceivedEventArgs) Handles Stream.UnmanagedEventReceived
        'ToDo. Do I need this?
        'UnmanagedEventReceivedWindow.AddMessage(e.JsonMessageReceived)
    End Sub

    Private Sub Stream_UserWitheld(sender As Object, e As Tweetinvi.Events.UserWitheldEventArgs) Handles Stream.UserWitheld
        'ToDo. Do I need this?
        'UserWitheldWindow.AddMessage(e.UserWitheldInfo.Id.ToString)
    End Sub

    Private Sub Stream_WarningFallingBehindDetected(sender As Object, e As Tweetinvi.Events.WarningFallingBehindEventArgs) Handles Stream.WarningFallingBehindDetected
        'ToDo. What to do here?
        'WarningFallingBehindDetectedWindow.AddMessage(e.WarningMessage.Code & ", " & e.WarningMessage.PercentFull & ", " & e.WarningMessage.Message)
    End Sub

End Class
