
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

        Dim _HasCoordinates As Boolean
        Public ReadOnly Property HasCoordinates As Boolean Implements TweetinviInterop.ITweet.HasCoordinates
            Get
                Return _HasCoordinates
            End Get
        End Property

        Dim _Latitude As Double
        Public ReadOnly Property Latitude As Double Implements TweetinviInterop.ITweet.Latitude
            Get
                Return _Latitude
            End Get
        End Property

        Dim _Longitude As Double
        Public ReadOnly Property Longitude As Double Implements TweetinviInterop.ITweet.Longitude
            Get
                Return _Latitude
            End Get
        End Property

        Dim _UserRegion As String
        Public ReadOnly Property UserRegion As String Implements TweetinviInterop.ITweet.UserRegion
            Get
                Return _UserRegion
            End Get
        End Property

        Public Sub New(NewId As Int64, NewCreatedByUserId As Int64, NewText As String, NewPublishDateTime As DateTime, NewHasCoordinates As Boolean, NewLatitude As Double, NewLongitude As Double, NewUserRegion As String)
            _Id = NewId
            _CreatedByUserId = NewCreatedByUserId
            _Text = NewText
            _PublishDateTime = NewPublishDateTime
            _HasCoordinates = NewHasCoordinates
            _Latitude = NewLatitude
            _Longitude = NewLongitude
            _UserRegion = NewUserRegion
        End Sub

    End Class

End Namespace
