
Namespace Streaming

    Public Class TweetinviTweet

        Dim _TweetinviTweet As Tweetinvi.Models.ITweet
        Public ReadOnly Property TweetinviTweet As Tweetinvi.Models.ITweet
            Get
                Return _TweetinviTweet
            End Get
        End Property

        Public ReadOnly Property Id As Int64
            Get
                Return TweetinviTweet.Id
            End Get
        End Property

        Public ReadOnly Property CreatedByUserId As Int64
            Get
                Return TweetinviTweet.CreatedBy.Id
            End Get
        End Property

        Public ReadOnly Property Text As String
            Get
                Return TweetinviTweet.FullText
            End Get
        End Property

        Public ReadOnly Property PublishDateTime As DateTime
            Get
                Return TweetinviTweet.CreatedAt
            End Get
        End Property

        Public ReadOnly Property HasCoordinates As Boolean
            Get
                Return TweetinviTweet.Coordinates IsNot Nothing
            End Get
        End Property

        Public ReadOnly Property Latitude As Double
            Get
                If TweetinviTweet.Coordinates Is Nothing Then
                    Return 0
                End If
                Return TweetinviTweet.Coordinates.Latitude
            End Get
        End Property

        Public ReadOnly Property Longitude As Double
            Get
                If TweetinviTweet.Coordinates Is Nothing Then
                    Return 0
                End If
                Return TweetinviTweet.Coordinates.Longitude
            End Get
        End Property

        Public ReadOnly Property UserRegion As String
            Get
                Return TweetinviTweet.CreatedBy.Location
            End Get
        End Property

        Public Sub New(NewTweetinviTweet As Tweetinvi.Models.ITweet)
            _TweetinviTweet = NewTweetinviTweet
        End Sub

    End Class

End Namespace
