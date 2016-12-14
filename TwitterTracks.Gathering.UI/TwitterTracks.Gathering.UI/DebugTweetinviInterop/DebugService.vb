
Imports TwitterTracks.TweetinviInterop

Namespace DebugTweetinviInterop

    Public Class DebugService
        Implements ITwitterService

        Private Shared Rnd As New Random

        Public Function CountTweetLength(TweetText As String) As Integer Implements ITwitterService.CountTweetLength
            Return TweetText.Length
        End Function

        Public Function ValidateAuthenticationToken(Token As AuthenticationToken) As ValidationResult Implements ITwitterService.ValidateAuthenticationToken
            System.Threading.Thread.Sleep(100)
            If Rnd.NextBoolean Then
                Return ValidationResult.Success
            Else
                Return ValidationResult.Fail("This is a debug fail message.")
            End If
        End Function

        Public Event StreamStarted(sender As Object, e As System.EventArgs) Implements TweetinviInterop.ITwitterService.StreamStarted
        Public Event StreamStopped(sender As Object, e As TweetinviInterop.StreamStoppedEventArgs) Implements TweetinviInterop.ITwitterService.StreamStopped
        Public Event TweetReceived(sender As Object, e As TweetinviInterop.TweetReceivedEventArgs) Implements TweetinviInterop.ITwitterService.TweetReceived

        Public Function PublishTweet(TweetText As String, MediaBinaries As System.Collections.Generic.IEnumerable(Of Byte()), AuthenticationToken As TweetinviInterop.AuthenticationToken) As TweetinviInterop.PublishTweetResult Implements TweetinviInterop.ITwitterService.PublishTweet
            If Rnd.NextBoolean Then
                Try
                    Throw New InvalidWmpVersionException("This is a test.")
                Catch ex As Exception
                    Return New PublishTweetResult(PublishTweetFailReason.None, ex)
                End Try
            Else
                Return New PublishTweetResult(New DebugTweet(Rnd.Next(100, 1000000), Rnd.Next(100, 100000), "This is a debug Tweet.", Helpers.UnixTimestampToUtc(Rnd.Next(10000, 100000000)), "Over there"))
            End If
        End Function

        Public Function StartTwitterStream(TweetId As Int64, CreatedByUserId As Int64, RelevantKeywords As System.Collections.Generic.IEnumerable(Of String), AuthenticationToken As TweetinviInterop.AuthenticationToken) As TweetinviInterop.StartTwitterStreamResult Implements TweetinviInterop.ITwitterService.StartTwitterStream
            If Rnd.NextBoolean Then
                Try
                    Throw New InvalidWmpVersionException("This is a test.")
                Catch ex As Exception
                    Return New StartTwitterStreamResult(PublishTweetFailReason.None, ex)
                End Try
            Else
                Return New StartTwitterStreamResult
            End If
        End Function

    End Class

End Namespace
