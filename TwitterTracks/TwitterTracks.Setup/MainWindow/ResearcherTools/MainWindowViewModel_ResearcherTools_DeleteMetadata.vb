Public Class MainWindowViewModel_ResearcherTools_DeleteMetadata
    Inherits ViewModelBase

    Dim _Metadata As TwitterTracks.DatabaseAccess.TrackMetadata? = Nothing
    Public Property Metadata As TwitterTracks.DatabaseAccess.TrackMetadata?
        <DebuggerStepThrough()>
        Get
            Return _Metadata
        End Get
        Set(value As TwitterTracks.DatabaseAccess.TrackMetadata?)
            ExtendedChangeIfDifferent(_Metadata, value, "Metadata")
        End Set
    End Property

    <Dependency("Metadata")>
    Public ReadOnly Property HasMetadata As Boolean
        Get
            Return Metadata IsNot Nothing
        End Get
    End Property

    <Dependency("Metadata")>
    Public ReadOnly Property IsPublished As Boolean
        Get
            Return Metadata IsNot Nothing AndAlso Metadata.Value.IsPublished
        End Get
    End Property

End Class
