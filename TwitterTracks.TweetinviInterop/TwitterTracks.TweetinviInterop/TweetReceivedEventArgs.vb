Public Class TweetReceivedEventArgs
    Inherits EventArgs

    Dim _Tweet As ITweet
    Public ReadOnly Property Tweet As ITweet
        <DebuggerStepThrough()>
        Get
            Return _Tweet
        End Get
    End Property

    Dim _TweetinviMatchOn As TweetinviMatchOn
    Public ReadOnly Property TweetinviMatchOn As TweetinviMatchOn
        <DebuggerStepThrough()>
        Get
            Return _TweetinviMatchOn
        End Get
    End Property

    Dim _AdditionalData As String
    Public ReadOnly Property AdditionalData As String
        <DebuggerStepThrough()>
        Get
            Return _AdditionalData
        End Get
    End Property

    Public Sub New(NewTweet As ITweet, NewTweetinviMatchOn As TweetinviMatchOn, NewAdditionalData As String)
        _Tweet = NewTweet
        _TweetinviMatchOn = NewTweetinviMatchOn
        _AdditionalData = NewAdditionalData
    End Sub

End Class
