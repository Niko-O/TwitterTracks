Public Structure TweetLocation

    Private _LocationType As TweetLocationType
    Public ReadOnly Property LocationType As TweetLocationType
        <DebuggerStepThrough()>
        Get
            Return _LocationType
        End Get
    End Property

    Private _Latitude As Double
    Public ReadOnly Property Latitude As Double
        <DebuggerStepThrough()>
        Get
            Return _Latitude
        End Get
    End Property

    Private _Longitude As Double
    Public ReadOnly Property Longitude As Double
        <DebuggerStepThrough()>
        Get
            Return _Longitude
        End Get
    End Property

    Private _UserRegion As String
    Public ReadOnly Property UserRegion As String
        <DebuggerStepThrough()>
        Get
            Return _UserRegion
        End Get
    End Property

    Public Function ToDatabaseValue() As String
        Select Case LocationType
            Case TweetLocationType.None
                Return Nothing
            Case TweetLocationType.TweetCoordinates
                Return String.Format("{0};{1}", Latitude, Longitude)
            Case TweetLocationType.UserRegion
                Return UserRegion
            Case Else
                Throw New NopeException
        End Select
    End Function

    Public Shared Function FromNone() As TweetLocation
        Return New TweetLocation With {._LocationType = TweetLocationType.None}
    End Function

    Public Shared Function FromTweetCoordinates(NewLatitude As Double, NewLongitude As Double) As TweetLocation
        Return New TweetLocation With {._LocationType = TweetLocationType.TweetCoordinates, ._Latitude = NewLatitude, ._Longitude = NewLongitude}
    End Function

    Public Shared Function FromUserRegion(NewUserRegion As String) As TweetLocation
        Return New TweetLocation With {._LocationType = TweetLocationType.UserRegion, ._UserRegion = NewUserRegion}
    End Function

    Public Shared Function ParseDatabaseValue(LocationType As TweetLocationType, LocationString As String) As TweetLocation
        Select Case LocationType
            Case TweetLocationType.None
                Return TweetLocation.FromNone
            Case TweetLocationType.TweetCoordinates
                Dim Parts = LocationString.Split(";"c)
                If Parts.Length <> 2 Then
                    Throw New FormatException("The format is invalid for coordinates: " & LocationString)
                End If
                Return TweetLocation.FromTweetCoordinates(Double.Parse(Parts(0)), _
                                                          Double.Parse(Parts(1)))
            Case TweetLocationType.UserRegion
                Return TweetLocation.FromUserRegion(LocationString)
            Case Else
                Throw New NopeException
        End Select
    End Function

End Structure
