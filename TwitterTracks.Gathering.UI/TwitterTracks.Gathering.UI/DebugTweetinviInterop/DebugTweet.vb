
Imports TwitterTracks.TweetinviInterop

Namespace DebugTweetinviInterop

    Public Class DebugTweet
        Implements ITweet

        Dim _Id As Int64
        Public ReadOnly Property Id As Int64 Implements TweetinviInterop.ITweet.Id
            Get
                Return _Id
            End Get
        End Property

        Dim _CreatedByUserId As Int64
        Public ReadOnly Property CreatedByUserId As Int64 Implements TweetinviInterop.ITweet.CreatedByUserId
            Get
                Return _CreatedByUserId
            End Get
        End Property

        Dim _Text As String
        Public ReadOnly Property Text As String Implements TweetinviInterop.ITweet.Text
            Get
                Return _Text
            End Get
        End Property

        Dim _PublishDateTime As DateTime
        Public ReadOnly Property PublishDateTime As DateTime Implements TweetinviInterop.ITweet.PublishDateTime
            Get
                Return _PublishDateTime
            End Get
        End Property

        Dim _Coordinates As String
        Public ReadOnly Property Coordinates As String Implements TweetinviInterop.ITweet.Coordinates
            Get
                Return _Coordinates
            End Get
        End Property

        Public Sub New(NewId As Int64, NewCreatedByUserId As Int64, NewText As String, NewPublishDateTime As DateTime, NewCoordinates As String)
            _Id = NewId
            _CreatedByUserId = NewCreatedByUserId
            _Text = NewText
            _PublishDateTime = NewPublishDateTime
            _Coordinates = NewCoordinates
        End Sub

    End Class

End Namespace
