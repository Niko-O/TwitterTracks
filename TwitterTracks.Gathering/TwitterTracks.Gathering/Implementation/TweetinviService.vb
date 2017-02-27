
Imports TwitterTracks.TweetinviInterop

Public Class TweetinviService
    Implements ITwitterService

    Private Shared _Instance As New TweetinviService
    Public Shared ReadOnly Property Instance As TweetinviService
        <DebuggerStepThrough()>
        Get
            Return _Instance
        End Get
    End Property

    Public Function CountTweetLength(TweetText As String) As Integer Implements ITwitterService.CountTweetLength
        Return Tweetinvi.Core.Extensions.StringExtension.TweetLength(TweetText, False)
    End Function

    Public Function ValidateAuthenticationToken(Token As AuthenticationToken) As ValidationResult Implements ITwitterService.ValidateAuthenticationToken
        Return ValidationResult.Success
    End Function

    Public Event TweetReceived As EventHandler(Of TweetReceivedEventArgs) Implements ITwitterService.TweetReceived
    Public Event StreamStarted As EventHandler Implements ITwitterService.StreamStarted
    Public Event StreamStopped As EventHandler(Of StreamStoppedEventArgs) Implements ITwitterService.StreamStopped

    Dim WithEvents CurrentStream As New Stream

    Public Function PublishTweet(TweetText As String, MediaBinaries As IEnumerable(Of Byte()), AuthenticationToken As AuthenticationToken) As PublishTweetResult Implements ITwitterService.PublishTweet
        If CurrentStream.IsRunning Then
            Throw New NopeException("The Stream is already running. This method should not be called.")
        End If
        Dim TwitterCredentials As New Tweetinvi.Models.TwitterCredentials(AuthenticationToken.ConsumerKey, AuthenticationToken.ConsumerSecret, AuthenticationToken.AccessToken, AuthenticationToken.AccessTokenSecret)
        Dim OptionalParameters As New Tweetinvi.Parameters.PublishTweetOptionalParameters With
        {
            .AutoPopulateReplyMetadata = True,
            .MediaBinaries = MediaBinaries.ToList,
            .PossiblySensitive = False
        }
        Dim ResultTweet = Tweetinvi.Auth.ExecuteOperationWithCredentials(Of Tweetinvi.Models.ITweet)(TwitterCredentials, Function() Tweetinvi.Tweet.PublishTweet(TweetText, OptionalParameters))
        Return New PublishTweetResult(New Tweet(ResultTweet))
    End Function

    Public Function StartTwitterStream(TweetId As Int64, CreatedByUserId As Int64, RelevantKeywords As IEnumerable(Of String), AuthenticationToken As AuthenticationToken) As StartTwitterStreamResult Implements ITwitterService.StartTwitterStream
        If CurrentStream.IsRunning Then
            Throw New NopeException("The Stream is already running. This method should not be called.")
        End If
        CurrentStream.OriginalTweetId = TweetId
        CurrentStream.OriginalTweetCreatedByUserId = CreatedByUserId
        CurrentStream.RelevantKeywords = New ReadOnlyCollection(Of String)(RelevantKeywords.ToList)
        CurrentStream.TwitterCredentials = New Tweetinvi.Models.TwitterCredentials(AuthenticationToken.ConsumerKey, AuthenticationToken.ConsumerSecret, AuthenticationToken.AccessToken, AuthenticationToken.AccessTokenSecret)
        CurrentStream.Start()
        Return New StartTwitterStreamResult
    End Function

    Public Sub ResumeTwitterStream() Implements ITwitterService.ResumeTwitterStream
        If CurrentStream Is Nothing Then
            Throw New NopeException("The Stream was not started before. This method should not be called.")
        End If
        If CurrentStream.IsRunning Then
            'ToDo: IsRunning is True when Stream is stopped due to exceeded limit.
            'Throw New NopeException("The Stream is already running. This method should not be called.")
        End If
        CurrentStream.Start()
    End Sub

    Public Sub StopTwitterStream() Implements ITwitterService.StopTwitterStream
        If Not CurrentStream.IsRunning Then
            Throw New NopeException("The Stream is not running. This method should not be called.")
        End If
        CurrentStream.Stop()
    End Sub

    Private Sub CurrentStream_Started() Handles CurrentStream.Started
        RaiseEvent StreamStarted(Me, EventArgs.Empty)
    End Sub

    Private Sub CurrentStream_Stopped(sender As Object, e As StreamStoppedEventArgs) Handles CurrentStream.Stopped
        RaiseEvent StreamStopped(Me, e)
    End Sub

    Private Sub CurrentStream_TweetReceived(Tweet As Tweetinvi.Models.ITweet, MatchOn As TweetinviMatchOn, AdditionalData As String) Handles CurrentStream.TweetReceived
        RaiseEvent TweetReceived(Me, New TweetReceivedEventArgs(New Tweet(Tweet), MatchOn, AdditionalData))
    End Sub

End Class
