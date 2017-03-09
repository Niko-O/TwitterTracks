
Namespace TweetinviInterop

    Public Interface ITweet

        ReadOnly Property Id As Int64
        ReadOnly Property CreatedByUserId As Int64
        ReadOnly Property Text As String
        ReadOnly Property PublishDateTime As DateTime
        ReadOnly Property HasCoordinates As Boolean
        ReadOnly Property Latitude As Double
        ReadOnly Property Longitude As Double
        ReadOnly Property UserRegion As String

    End Interface

End Namespace
