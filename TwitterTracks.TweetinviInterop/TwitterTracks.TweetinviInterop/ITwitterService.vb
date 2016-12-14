Public Interface ITwitterService

    Function CountTweetLength(TweetText As String) As Integer
    Function ValidateAuthenticationToken(Token As AuthenticationToken) As ValidationResult
    
    <MultithreadingAwareness()> Event TweetReceived As EventHandler(Of TweetReceivedEventArgs)
    <MultithreadingAwareness()> Event StreamStarted As EventHandler
    <MultithreadingAwareness()> Event StreamStopped As EventHandler(Of StreamStoppedEventArgs)

    Function PublishTweet(TweetText As String, MediaBinaries As IEnumerable(Of Byte()), AuthenticationToken As AuthenticationToken) As PublishTweetResult
    Function StartTwitterStream(TweetId As Int64, CreatedByUserId As Int64, RelevantKeywords As IEnumerable(Of String), AuthenticationToken As AuthenticationToken) As StartTwitterStreamResult

End Interface
