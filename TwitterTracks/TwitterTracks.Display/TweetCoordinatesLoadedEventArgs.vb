Public Class TweetCoordinatesLoadedEventArgs
    Inherits EventArgs

    Dim _Tweet As TwitterTracks.DatabaseAccess.Tweet
    Public ReadOnly Property Tweet As TwitterTracks.DatabaseAccess.Tweet
        <DebuggerStepThrough()>
        Get
            Return _Tweet
        End Get
    End Property

    Dim _Latitude As Double
    Public ReadOnly Property Latitude As Double
        <DebuggerStepThrough()>
        Get
            Return _Latitude
        End Get
    End Property

    Dim _Longitude As Double
    Public ReadOnly Property Longitude As Double
        <DebuggerStepThrough()>
        Get
            Return _Longitude
        End Get
    End Property

    Public Sub New(NewTweet As TwitterTracks.DatabaseAccess.Tweet, NewLatitude As Double, NewLongitude As Double)
        _Tweet = NewTweet
        _Latitude = NewLatitude
        _Longitude = NewLongitude
    End Sub

End Class
