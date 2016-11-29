
Namespace DebugTweetinviInterop

    Public Class DebugService
        Implements TwitterTracks.TweetinviInterop.ITwitterService

        Public Function CountTweetLength(TweetText As String) As Integer Implements TweetinviInterop.ITwitterService.CountTweetLength
            Return TweetText.Length
        End Function

        Public Function ValidateAuthenticationToken(Token As TweetinviInterop.AuthenticationToken) As TweetinviInterop.ValidationResult Implements TweetinviInterop.ITwitterService.ValidateAuthenticationToken
            System.Threading.Thread.Sleep(100)
            Static Rnd As New Random
            If Rnd.NextBoolean Then
                Return TweetinviInterop.ValidationResult.Success
            Else
                Return TweetinviInterop.ValidationResult.Fail("This is a debug fail message.")
            End If
        End Function

    End Class

End Namespace
