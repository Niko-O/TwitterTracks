Public Class PublishTweetResult

    Dim _Success As Boolean
    Public ReadOnly Property Success As Boolean
        <DebuggerStepThrough()>
        Get
            Return _Success
        End Get
    End Property

    Dim _ResultTweet As ITweet
    Public ReadOnly Property ResultTweet As ITweet
        <DebuggerStepThrough()>
        Get
            Return _ResultTweet
        End Get
    End Property

    Dim _FailReason As PublishTweetFailReason
    Public ReadOnly Property FailReason As PublishTweetFailReason
        <DebuggerStepThrough()>
        Get
            Return _FailReason
        End Get
    End Property

    Dim _ErrorException As Exception
    Public ReadOnly Property ErrorException As Exception
        <DebuggerStepThrough()>
        Get
            Return _ErrorException
        End Get
    End Property

    Public Sub New(NewResultTweet As ITweet)
        _Success = True
        _ResultTweet = NewResultTweet
        _FailReason = PublishTweetFailReason.None
        _ErrorException = Nothing
    End Sub

    Public Sub New(NewFailReason As PublishTweetFailReason, NewErrorException As Exception)
        _Success = False
        _ResultTweet = Nothing
        _FailReason = NewFailReason
        _ErrorException = NewErrorException
    End Sub

End Class
