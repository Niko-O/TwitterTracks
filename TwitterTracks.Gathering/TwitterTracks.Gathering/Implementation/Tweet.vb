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

    Public ReadOnly Property Coordinates As String Implements ITweet.Coordinates
        Get
            Return String.Format("Latitude={0};Longitude={1};UserLocation={2}",
                                 If(TweetinviTweet.Coordinates Is Nothing, "", TweetinviTweet.Coordinates.Latitude.ToString),
                                 If(TweetinviTweet.Coordinates Is Nothing, "", TweetinviTweet.Coordinates.Longitude.ToString),
                                 TweetinviTweet.CreatedBy.Location)
        End Get
    End Property

    Public Sub New(NewTweetinviTweet As Tweetinvi.Models.ITweet)
        TweetinviTweet = NewTweetinviTweet
    End Sub

End Class
