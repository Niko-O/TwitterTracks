Public Structure TweetLocation

    Private _LocationType As TweetLocationType
    Public ReadOnly Property LocationType As TweetLocationType
        <DebuggerStepThrough()>
        Get
            Return _LocationType
        End Get
    End Property

    Private _UserRegion As String
    Public ReadOnly Property UserRegion As String
        <DebuggerStepThrough()>
        Get
            Return _UserRegion
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

    Public Shared Function FromNone() As TweetLocation
        Return New TweetLocation With {._LocationType = TweetLocationType.None}
    End Function

    Public Shared Function FromTweetCoordinates(NewLatitude As Double, NewLongitude As Double) As TweetLocation
        Return New TweetLocation With {._LocationType = TweetLocationType.TweetCoordinates, ._Latitude = NewLatitude, ._Longitude = NewLongitude}
    End Function

    Public Shared Function FromUserRegionWithPotentialForCoordinates(NewUserRegion As String) As TweetLocation
        Return New TweetLocation With {._LocationType = TweetLocationType.UserRegionWithPotentialForCoordinates, ._UserRegion = NewUserRegion}
    End Function

    Public Shared Function FromUserRegionNoCoordinates(NewUserRegion As String) As TweetLocation
        Return New TweetLocation With {._LocationType = TweetLocationType.UserRegionNoCoordinates, ._UserRegion = NewUserRegion}
    End Function

    Public Shared Function FromUserRegionWithCoordinates(NewUserRegion As String, NewLatitude As Double, NewLongitude As Double) As TweetLocation
        Return New TweetLocation With {._LocationType = TweetLocationType.UserRegionWithCoordinates, ._UserRegion = NewUserRegion, ._Latitude = NewLatitude, ._Longitude = NewLongitude}
    End Function

    Public Shared Function ParseDatabaseValue(LocationType As TweetLocationType, UserRegion As String, Latitude As Double?, Longitude As Double?) As TweetLocation
        Select Case LocationType
            Case TweetLocationType.TweetCoordinates, _
                 TweetLocationType.UserRegionWithCoordinates
                If Not Latitude.HasValue Then
                    Throw New ArgumentNullException("Latitude", "Latitude is required for this LocationType but was NULL")
                End If
                If Not Longitude.HasValue Then
                    Throw New ArgumentNullException("Longitude", "Longitude is required for this LocationType but was NULL")
                End If
        End Select
        Select Case LocationType
            Case TweetLocationType.UserRegionWithPotentialForCoordinates, _
                 TweetLocationType.UserRegionNoCoordinates, _
                 TweetLocationType.UserRegionWithCoordinates
                If UserRegion Is Nothing Then
                    Throw New ArgumentNullException("UserRegion", "UserRegion is required for this LocationType but was NULL")
                End If
        End Select
        Select Case LocationType
            Case TweetLocationType.None
                Return TweetLocation.FromNone
            Case TweetLocationType.TweetCoordinates
                Return TweetLocation.FromTweetCoordinates(Latitude.Value, Longitude.Value)
            Case TweetLocationType.UserRegionWithPotentialForCoordinates
                Return TweetLocation.FromUserRegionWithPotentialForCoordinates(UserRegion)
            Case TweetLocationType.UserRegionNoCoordinates
                Return TweetLocation.FromUserRegionNoCoordinates(UserRegion)
            Case TweetLocationType.UserRegionWithCoordinates
                Return TweetLocation.FromUserRegionWithCoordinates(UserRegion, Latitude.Value, Longitude.Value)
            Case Else
                Throw New NopeException
        End Select
    End Function

    Public Function GetDatabaseValueLatitude() As Object
        Select Case LocationType
            Case TweetLocationType.TweetCoordinates, _
                 TweetLocationType.UserRegionWithCoordinates
                Return Latitude
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function GetDatabaseValueLongitude() As Object
        Select Case LocationType
            Case TweetLocationType.TweetCoordinates, _
                 TweetLocationType.UserRegionWithCoordinates
                Return Longitude
            Case Else
                Return Nothing
        End Select
    End Function

End Structure
