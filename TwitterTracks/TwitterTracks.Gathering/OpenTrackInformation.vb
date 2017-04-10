Public Class OpenTrackInformation

    Public Event MetadataChanged()

    Public Property ApplicationToken As TwitterTracks.DatabaseAccess.ApplicationToken
    Public Property Metadata As TwitterTracks.DatabaseAccess.TrackMetadata

    Dim _Database As New DatabaseContainer
    Public ReadOnly Property Database As DatabaseContainer
        <DebuggerStepThrough()>
        Get
            Return _Database
        End Get
    End Property

    Public Class DatabaseContainer
        Public Property Host As String
        Public Property Name As String
        Public Property ResearcherId As Int64
        Public Property Password As String
        Public Property Connection As TwitterTracks.DatabaseAccess.DatabaseConnection
    End Class

End Class
