
Namespace Streaming

    Public Class TweetinviService

        Private Shared _Instance As New TweetinviService
        Public Shared ReadOnly Property Instance As TweetinviService
            <DebuggerStepThrough()>
            Get
                Return _Instance
            End Get
        End Property

        Public Function CountTweetLength(TweetText As String) As Integer
            Return Tweetinvi.Core.Extensions.StringExtension.TweetLength(TweetText, False)
        End Function

        Public Function ValidateAuthenticationToken(Token As Tweetinvi.Models.TwitterCredentials) As ValidationResult
            Return ValidationResult.Success
        End Function

        Public Event TweetReceived As EventHandler(Of TweetReceivedEventArgs)
        Public Event StreamStarted As EventHandler
        Public Event StreamStopped As EventHandler(Of StreamStoppedEventArgs)

        Dim WithEvents CurrentStream As New Stream

        Public Function PublishTweet(TweetText As String, MediaBinaries As IEnumerable(Of Byte()), AuthenticationToken As Tweetinvi.Models.TwitterCredentials) As PublishTweetResult
            Application.DebugPrint("TweetinviService.PublishTweet.")
            If CurrentStream.IsRunning Then
                Throw New NopeException("The Stream is already running. This method should not be called.")
            End If
            Dim OptionalParameters As New Tweetinvi.Parameters.PublishTweetOptionalParameters With
            {
                .AutoPopulateReplyMetadata = True,
                .MediaBinaries = MediaBinaries.ToList,
                .PossiblySensitive = False
            }
            Dim ResultTweet = Tweetinvi.Auth.ExecuteOperationWithCredentials(Of Tweetinvi.Models.ITweet)(AuthenticationToken, Function() Tweetinvi.Tweet.PublishTweet(TweetText, OptionalParameters))
            Return New PublishTweetResult(New TweetinviTweet(ResultTweet))
        End Function

        Public Function StartTwitterStream(TweetId As Int64, CreatedByUserId As Int64, RelevantKeywords As IEnumerable(Of String), AuthenticationToken As Tweetinvi.Models.TwitterCredentials) As StartTwitterStreamResult
            Application.DebugPrint("TweetinviService.StartTwitterStream.")
            If CurrentStream.IsRunning Then
                Throw New NopeException("The Stream is already running. This method should not be called.")
            End If
            CurrentStream.InitialTweetId = TweetId
            CurrentStream.InitialTweetCreatedByUserId = CreatedByUserId
            CurrentStream.RelevantKeywords = New ReadOnlyCollection(Of String)(RelevantKeywords.ToList)
            CurrentStream.AuthenticationToken = AuthenticationToken
            CurrentStream.Start()
            Return New StartTwitterStreamResult
        End Function

        Public Sub ResumeTwitterStream()
            Application.DebugPrint("TweetinviService.ResumeTwitterStream.")
            If CurrentStream Is Nothing Then
                Throw New NopeException("The Stream was not started before. This method should not be called.")
            End If
            If CurrentStream.IsRunning Then
                'ToDo: IsRunning is True when Stream is stopped due to exceeded limit.
                'Throw New NopeException("The Stream is already running. This method should not be called.")
            End If
            CurrentStream.Start()
        End Sub

        Public Sub StopTwitterStream()
            Application.DebugPrint("TweetinviService.StopTwitterStream.")
            If Not CurrentStream.IsRunning Then
                Throw New NopeException("The Stream is not running. This method should not be called.")
            End If
            CurrentStream.Stop()
        End Sub

        Private Sub CurrentStream_Started() Handles CurrentStream.Started
            Application.DebugPrint("TweetinviService.Started.")
            RaiseEvent StreamStarted(Me, EventArgs.Empty)
        End Sub

        Private Sub CurrentStream_Stopped(sender As Object, e As StreamStoppedEventArgs) Handles CurrentStream.Stopped
            Application.DebugPrint("TweetinviService.Stopped.")
            RaiseEvent StreamStopped(Me, e)
        End Sub

        Private Sub CurrentStream_TweetReceived(Tweet As Tweetinvi.Models.ITweet, MatchOn As Tweetinvi.Streaming.MatchOn, AdditionalData As String) Handles CurrentStream.TweetReceived
            RaiseEvent TweetReceived(Me, New TweetReceivedEventArgs(New TweetinviTweet(Tweet), MatchOn, AdditionalData))
        End Sub

    End Class

End Namespace
