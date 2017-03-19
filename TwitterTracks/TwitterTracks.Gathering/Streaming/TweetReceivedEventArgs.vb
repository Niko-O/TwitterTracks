
Namespace Streaming

    Public Class TweetReceivedEventArgs
        Inherits EventArgs

        Dim _Tweet As TweetinviTweet
        Public ReadOnly Property Tweet As TweetinviTweet
            <DebuggerStepThrough()>
            Get
                Return _Tweet
            End Get
        End Property

        Dim _TweetinviMatchOn As Tweetinvi.Streaming.MatchOn
        Public ReadOnly Property TweetinviMatchOn As Tweetinvi.Streaming.MatchOn
            <DebuggerStepThrough()>
            Get
                Return _TweetinviMatchOn
            End Get
        End Property

        Dim _IsRetweet As Boolean
        Public ReadOnly Property IsRetweet As Boolean
            <DebuggerStepThrough()>
            Get
                Return _IsRetweet
            End Get
        End Property

        Public Sub New(NewTweet As TweetinviTweet, NewTweetinviMatchOn As Tweetinvi.Streaming.MatchOn, NewIsRetweet As Boolean)
            _Tweet = NewTweet
            _TweetinviMatchOn = NewTweetinviMatchOn
            _IsRetweet = NewIsRetweet
        End Sub

    End Class

End Namespace
