Public Interface ITwitterService

    Function CountTweetLength(TweetText As String) As Integer
    Function ValidateAuthenticationToken(Token As AuthenticationToken) As ValidationResult

End Interface
