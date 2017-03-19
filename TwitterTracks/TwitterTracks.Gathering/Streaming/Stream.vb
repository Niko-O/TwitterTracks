
Namespace Streaming

    Public Class Stream

        Public Event TweetReceived(Tweet As Tweetinvi.Models.ITweet, MatchOn As Tweetinvi.Streaming.MatchOn, AdditionalData As String)
        Private Sub OnTweetReceived(Tweet As Tweetinvi.Models.ITweet, MatchOn As Tweetinvi.Streaming.MatchOn, AdditionalData As String)
            RaiseEvent TweetReceived(Tweet, MatchOn, AdditionalData)
        End Sub

        Public Event Started()
        Private Sub OnStarted()
            RaiseEvent Started()
        End Sub

        Public Event Stopped As EventHandler(Of TwitterTracks.Gathering.Streaming.StreamStoppedEventArgs)
        Protected Overridable Sub OnStopped(e As TwitterTracks.Gathering.Streaming.StreamStoppedEventArgs)
            RaiseEvent Stopped(Me, e)
        End Sub

        Public ReadOnly Property IsRunning As Boolean
            Get
                Return Stream IsNot Nothing AndAlso Stream.StreamState = Tweetinvi.Models.StreamState.Running
            End Get
        End Property

        Public Property InitialTweetId As Int64
        Public Property InitialTweetCreatedByUserId As Int64
        Public Property RelevantKeywords As ReadOnlyCollection(Of String)
        Public Property AuthenticationToken As Tweetinvi.Models.TwitterCredentials

        Dim WithEvents Stream As Tweetinvi.Streaming.IFilteredStream = Nothing

        Private Sub DebugPrint(Text As String, ParamArray Args As Object())
            Dim FormattedText As String
            If Args Is Nothing OrElse Args.Length = 0 Then
                FormattedText = Text
            Else
                FormattedText = String.Format(Text, Args)
            End If
            Application.DebugPrint("(StreamState {0}) : {1}", If(Stream Is Nothing, "null", Stream.StreamState.ToString), FormattedText)
        End Sub

        Public Sub Start()
            If Stream Is Nothing Then
                Dim KeywordsString = String.Join(" ", RelevantKeywords)
                DebugPrint("Stream.Start (new):")
                DebugPrint("    OriginalTweetId             : " & InitialTweetId)
                DebugPrint("    OriginalTweetCreatedByUserId: " & InitialTweetCreatedByUserId)
                DebugPrint("    RelevantKeywords            : " & KeywordsString)
                Stream = Tweetinvi.Stream.CreateFilteredStream(AuthenticationToken)
                Stream.AddFollow(InitialTweetCreatedByUserId, Nothing)
                If RelevantKeywords.Count <> 0 Then
                    Stream.AddTrack(KeywordsString, AddressOf TweetReceivedByKeywordsCallback)
                End If
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

        Private Sub TweetReceivedByKeywordsCallback(Tweet As Tweetinvi.Models.ITweet)
            DebugPrint("Stream.TweetReceivedByKeywordsCallback: " & Tweet.Id)
            OnTweetReceived(Tweet, Tweetinvi.Streaming.MatchOn.None, "TweetReceivedByKeywordsCallback")
        End Sub

        Private Sub Stream_NonMatchingTweetReceived(sender As Object, e As Tweetinvi.Events.TweetEventArgs) Handles Stream.NonMatchingTweetReceived
            'Retweets of the initial tweet are not considered "matching" by Tweetinvi. The matching is done manually here.
            'See https://github.com/linvi/tweetinvi/issues/356#issuecomment-251837195
            If IsChildOfOrOriginalTweet(e.Tweet) Then
                DebugPrint("Strema.Stream_NonMatchingTweetReceived: " & e.Tweet.Id)
                DebugPrint("    Is retweet of initial Tweet.")
                OnTweetReceived(e.Tweet, Tweetinvi.Streaming.MatchOn.None, "Retweet")
            End If
        End Sub

        '@Robustness Eliminate recursion. Unlikely but possible StackOverflowException.
        Private Function IsChildOfOrOriginalTweet(Tweet As Tweetinvi.Models.ITweet) As Boolean
            If Tweet Is Nothing Then
                Return False
            End If
            If Tweet.Id = InitialTweetId Then
                Return True
            End If
            If Tweet.RetweetedTweet Is Nothing Then
                Return False
            End If
            Return IsChildOfOrOriginalTweet(Tweet.RetweetedTweet)
        End Function

        Private Sub Stream_DisconnectMessageReceived(sender As Object, e As Tweetinvi.Events.DisconnectedEventArgs) Handles Stream.DisconnectMessageReceived
            DebugPrint("Stream.DisconnectMessageReceived: {2} Code {0}: {1} ", e.DisconnectMessage.Code, e.DisconnectMessage.Reason, e.DisconnectMessage.StreamName)
            OnStopped(TwitterTracks.Gathering.Streaming.StreamStoppedEventArgs.Disconnect(e.DisconnectMessage.Code, e.DisconnectMessage.Reason, e.DisconnectMessage.StreamName))
        End Sub

        Private Sub Stream_LimitReached(sender As Object, e As Tweetinvi.Events.LimitReachedEventArgs) Handles Stream.LimitReached
            DebugPrint("Stream.LimitReached: ", e.NumberOfTweetsNotReceived)
            OnStopped(TwitterTracks.Gathering.Streaming.StreamStoppedEventArgs.LimitReached(e.NumberOfTweetsNotReceived))
        End Sub

        Private Sub Stream_StreamStopped(sender As Object, e As Tweetinvi.Events.StreamExceptionEventArgs) Handles Stream.StreamStopped
            If e.DisconnectMessage Is Nothing Then
                DebugPrint("Stream.StreamStopped: (No DisconnectMessage). Exception: {0}", If(e.Exception Is Nothing, "null", String.Format("{0}: {1}", e.Exception.GetType.Name, e.Exception.Message)))
                OnStopped(TwitterTracks.Gathering.Streaming.StreamStoppedEventArgs.Stopped(e.Exception, -1, Nothing, Nothing))
            Else
                DebugPrint("Stream.StreamStopped: {0} Code {1}: {2}. Exception: {3}", e.DisconnectMessage.StreamName, e.DisconnectMessage.Code, e.DisconnectMessage.Reason, If(e.Exception Is Nothing, "null", String.Format("{0}: {1}", e.Exception.GetType.Name, e.Exception.Message)))
                OnStopped(TwitterTracks.Gathering.Streaming.StreamStoppedEventArgs.Stopped(e.Exception, e.DisconnectMessage.Code, e.DisconnectMessage.Reason, e.DisconnectMessage.StreamName))
            End If
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

End Namespace
