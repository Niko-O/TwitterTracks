Imports TwitterTracks.TweetinviInterop

Public Class Tweet
    Implements ITweet

    Public ReadOnly Property TweetinviTweet As Tweetinvi.Models.ITweet

    Public ReadOnly Property Id As Int64 Implements ITweet.Id
        Get
            Return TweetinviTweet.Id
        End Get
    End Property

    Public ReadOnly Property CreatedByUserId As Int64 Implements ITweet.CreatedByUserId
        Get
            Return TweetinviTweet.CreatedBy.Id
        End Get
    End Property

    Public ReadOnly Property Text As String Implements ITweet.Text
        Get
            Return TweetinviTweet.FullText
        End Get
    End Property

    Public ReadOnly Property PublishTimeStamp As DateTime Implements ITweet.PublishDateTime
        Get
            Return TweetinviTweet.CreatedAt
        End Get
    End Property

    Public ReadOnly Property HasCoordinates As Boolean Implements ITweet.HasCoordinates
        Get
            Return TweetinviTweet.Coordinates IsNot Nothing
        End Get
    End Property

    Public ReadOnly Property Latitude As Double Implements ITweet.Latitude
        Get
            If TweetinviTweet.Coordinates Is Nothing Then
                Return 0
            End If
            Return TweetinviTweet.Coordinates.Latitude
        End Get
    End Property

    Public ReadOnly Property Longitude As Double Implements ITweet.Longitude
        Get
            If TweetinviTweet.Coordinates Is Nothing Then
                Return 0
            End If
            Return TweetinviTweet.Coordinates.Longitude
        End Get
    End Property

    Public ReadOnly Property UserRegion As String Implements ITweet.UserRegion
        Get
            Return TweetinviTweet.CreatedBy.Location
        End Get
    End Property

    Public Sub New(NewTweetinviTweet As Tweetinvi.Models.ITweet)
        TweetinviTweet = NewTweetinviTweet
    End Sub

End Class
