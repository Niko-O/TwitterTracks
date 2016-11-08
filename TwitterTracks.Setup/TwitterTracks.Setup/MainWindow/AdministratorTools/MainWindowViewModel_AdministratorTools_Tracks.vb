Public Class MainWindowViewModel_AdministratorTools_Tracks
    Inherits ViewModelBase

    Dim WithEvents _AvailableTracks As New TypedObservableCollection(Of TwitterTracks.DatabaseAccess.Track)
    Public ReadOnly Property AvailableTracks As TypedObservableCollection(Of TwitterTracks.DatabaseAccess.Track)
        <DebuggerStepThrough()>
        Get
            Return _AvailableTracks
        End Get
    End Property

    Dim _SelectedAvailableTrack As TwitterTracks.DatabaseAccess.Track = Nothing
    Public Property SelectedAvailableTrack As TwitterTracks.DatabaseAccess.Track
        <DebuggerStepThrough()>
        Get
            Return _SelectedAvailableTrack
        End Get
        Set(value As TwitterTracks.DatabaseAccess.Track)
            ExtendedChangeIfDifferent(_SelectedAvailableTrack, value, "SelectedAvailableTrack")
        End Set
    End Property

    Dim _StatusMessageVM As New StatusMessageViewModel
    Public ReadOnly Property StatusMessageVM As StatusMessageViewModel
        <DebuggerStepThrough()>
        Get
            Return _StatusMessageVM
        End Get
    End Property

    Public Sub New()
        If IsInDesignMode Then
            For i As Int64 = 1 To 6
                AvailableTracks.Add(New TwitterTracks.DatabaseAccess.Track(New TwitterTracks.DatabaseAccess.EntityId(i), Nothing))
            Next
            SelectedAvailableTrack = AvailableTracks(0)
        End If
    End Sub

End Class
