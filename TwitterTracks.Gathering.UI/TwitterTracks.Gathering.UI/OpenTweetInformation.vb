Public Class OpenTweetInformation

    Public Event IsPublishedChanged()

    Dim _IsPublished As Boolean
    Public Property IsPublished As Boolean
        Get
            Return _IsPublished
        End Get
        Set(value As Boolean)
            If _IsPublished <> value Then
                _IsPublished = value
                RaiseEvent IsPublishedChanged()
            End If
        End Set
    End Property

    Dim _Database As New DatabaseContainer
    Public ReadOnly Property Database As DatabaseContainer
        <DebuggerStepThrough()>
        Get
            Return _Database
        End Get
    End Property

    Dim _TweetData As New TweetDataContainer
    Public ReadOnly Property TweetData As TweetDataContainer
        <DebuggerStepThrough()>
        Get
            Return _TweetData
        End Get
    End Property

    Dim _TwitterConnection As New TwitterConnectionContianer
    Public ReadOnly Property TwitterConnection As TwitterConnectionContianer
        <DebuggerStepThrough()>
        Get
            Return _TwitterConnection
        End Get
    End Property

    Public Class DatabaseContainer
        Public Property Host As String
        Public Property Name As String
        Public Property ResearcherId As Int64
        Public Property Password As String
        Public Property Connection As TwitterTracks.DatabaseAccess.DatabaseConnection
    End Class

    Public Class TweetDataContainer
        Public Property Metadata As TwitterTracks.DatabaseAccess.TrackMetadata
        Public Property TweetText As String
        Public Property MediasToAdd As List(Of String)
        Public Property Keywords As List(Of String)
    End Class

    Public Class TwitterConnectionContianer
        Public Property ConsumerKey As String
        Public Property ConsumerSecret As String
        Public Property AccessToken As String
        Public Property AccessTokenSecret As String
        Public Function ToAuthenticationToken() As TwitterTracks.TweetinviInterop.AuthenticationToken
            Return New TwitterTracks.TweetinviInterop.AuthenticationToken(ConsumerKey, ConsumerSecret, AccessToken, AccessTokenSecret)
        End Function
    End Class

End Class
