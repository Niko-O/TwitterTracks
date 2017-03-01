
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

    Private Sub DebugPrint(Text As String, ParamArray Args As Object())
        Program.DebugPrint("(StreamState {0}) : {1}", If(Stream Is Nothing, "null", Stream.StreamState.ToString), String.Format(Text, Args))
    End Sub

    Public Sub Start()
        If Stream Is Nothing Then
            DebugPrint("Stream.Start (new)")
            Stream = Tweetinvi.Stream.CreateFilteredStream(TwitterCredentials)
            Stream.AddFollow(OriginalTweetCreatedByUserId, Nothing) 'AddressOf UserPublishedTweetCallback
            Stream.AddTrack(String.Join(" ", RelevantKeywords), Nothing) 'AddressOf TweetReceivedByKeywordsCallback
            Stream.StartStreamMatchingAnyConditionAsync()
        Else
            DebugPrint("Stream.Start (resumed)")
            Stream.ResumeStream()
        End If
    End Sub

    Public Sub [Stop]()
        DebugPrint("Stream.Stop")
        Stream.StopStream()
        Stream = Nothing
    End Sub

    'Private Sub UserPublishedTweetCallback(Tweet As Tweetinvi.Models.ITweet)
    '    DebugPrint("Stream.UserPublishedTweetCallback")
    '    OnTweetReceived(Tweet, TweetinviInterop.TweetinviMatchOn.None, "UserPublishedTweetCallback")
    'End Sub

    'Private Sub TweetReceivedByKeywordsCallback(Tweet As Tweetinvi.Models.ITweet)
    '    DebugPrint("Stream.TweetReceivedByKeywordsCallback")
    '    OnTweetReceived(Tweet, TweetinviInterop.TweetinviMatchOn.None, "TweetReceivedByKeywordsCallback")
    'End Sub

    Private Sub Stream_MatchingTweetReceived(sender As Object, e As Tweetinvi.Events.MatchedTweetReceivedEventArgs) Handles Stream.MatchingTweetReceived
        DebugPrint("Stream.Stream_MatchingTweetReceived")
        OnTweetReceived(e.Tweet, DirectCast(e.MatchOn, TwitterTracks.TweetinviInterop.TweetinviMatchOn), "Stream_MatchingTweetReceived")
        'OnTweetReceived(Tweetinvi.Auth.ExecuteOperationWithCredentials(TwitterCredentials, Function() Tweetinvi.Tweet.GetTweet(820325401079619584)), TweetinviInterop.TweetinviMatchOn.TweetText, "Manual")
    End Sub

    Private Sub Stream_DisconnectMessageReceived(sender As Object, e As Tweetinvi.Events.DisconnectedEventArgs) Handles Stream.DisconnectMessageReceived
        DebugPrint("Stream.DisconnectMessageReceived: {2} Code {0}: {1} ", e.DisconnectMessage.Code, e.DisconnectMessage.Reason, e.DisconnectMessage.StreamName)
        OnStopped(TwitterTracks.TweetinviInterop.StreamStoppedEventArgs.Disconnect(e.DisconnectMessage.Code, e.DisconnectMessage.Reason, e.DisconnectMessage.StreamName))
    End Sub

    Private Sub Stream_LimitReached(sender As Object, e As Tweetinvi.Events.LimitReachedEventArgs) Handles Stream.LimitReached
        DebugPrint("Stream.LimitReached: ", e.NumberOfTweetsNotReceived)
        OnStopped(TwitterTracks.TweetinviInterop.StreamStoppedEventArgs.LimitReached(e.NumberOfTweetsNotReceived))
    End Sub

    Private Sub Stream_StreamStopped(sender As Object, e As Tweetinvi.Events.StreamExceptionEventArgs) Handles Stream.StreamStopped
        If e.DisconnectMessage Is Nothing Then
            DebugPrint("Stream.StreamStopped: (No DisconnectMessage). Exception: {0}", If(e.Exception Is Nothing, "null", String.Format("{0}: {1}", e.Exception.GetType.Name, e.Exception.Message)))
            OnStopped(TwitterTracks.TweetinviInterop.StreamStoppedEventArgs.Stopped(e.Exception, -1, Nothing, Nothing))
        Else
            DebugPrint("Stream.StreamStopped: {0} Code {1}: {2}. Exception: {3}", e.DisconnectMessage.StreamName, e.DisconnectMessage.Code, e.DisconnectMessage.Reason, If(e.Exception Is Nothing, "null", String.Format("{0}: {1}", e.Exception.GetType.Name, e.Exception.Message)))
            OnStopped(TwitterTracks.TweetinviInterop.StreamStoppedEventArgs.Stopped(e.Exception, e.DisconnectMessage.Code, e.DisconnectMessage.Reason, e.DisconnectMessage.StreamName))
        End If
    End Sub

    Private Sub Stream_NonMatchingTweetReceived(sender As Object, e As Tweetinvi.Events.TweetEventArgs) Handles Stream.NonMatchingTweetReceived
        'ToDo: Should not matter.
    End Sub

    Private Sub Stream_StreamPaused(sender As Object, e As EventArgs) Handles Stream.StreamPaused
        DebugPrint("Stream.StreamPaused.")
        Throw New NopeException("The Stream should never be paused.")
    End Sub

    Private Sub Stream_StreamResumed(sender As Object, e As EventArgs) Handles Stream.StreamResumed
        'ToDo: Happens on start too.
        DebugPrint("Stream.StreamResumed.")
        OnStarted()
    End Sub

    Private Sub Stream_StreamStarted(sender As Object, e As EventArgs) Handles Stream.StreamStarted
        'ToDo: This or Resumed?
        'OnStarted()
        DebugPrint("Stream.StreamStarted.")
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
