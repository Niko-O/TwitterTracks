﻿
Namespace TweetinviInterop

    Public Class TweetReceivedEventArgs
        Inherits EventArgs

        Dim _Tweet As ITweet
        Public ReadOnly Property Tweet As ITweet
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

        Dim _AdditionalData As String
        Public ReadOnly Property AdditionalData As String
            <DebuggerStepThrough()>
            Get
                Return _AdditionalData
            End Get
        End Property

        Public Sub New(NewTweet As ITweet, NewTweetinviMatchOn As Tweetinvi.Streaming.MatchOn, NewAdditionalData As String)
            _Tweet = NewTweet
            _TweetinviMatchOn = NewTweetinviMatchOn
            _AdditionalData = NewAdditionalData
        End Sub

    End Class

End Namespace
