Public Class MainWindowViewModel_AdministratorTools
    Inherits ViewModelBase

#Region "Database Connection"

    Dim WithEvents _DatabaseConnectionVM As New TwitterTracks.Common.UI.Controls.TrackSelectionInputViewModel(False, False)
    Public ReadOnly Property DatabaseConnectionVM As TwitterTracks.Common.UI.Controls.TrackSelectionInputViewModel
        <DebuggerStepThrough()>
        Get
            Return _DatabaseConnectionVM
        End Get
    End Property
    Private Sub DatabaseConnectionVM_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _DatabaseConnectionVM.PropertyChanged
        Select Case e.PropertyName
            Case "IsValid"
                OnPropertyChanged("ToggleConnectionButtonIsEnabled")
        End Select
    End Sub

    <Dependency("Owner_IsBusy", "IsConnectedToDatabase")>
    Public ReadOnly Property ToggleConnectionButtonIsEnabled As Boolean
        Get
            Return Not Owner_IsBusy AndAlso (DatabaseConnectionVM.IsValid OrElse IsConnectedToDatabase)
        End Get
    End Property

    <Dependency("IsConnectedToDatabase")>
    Public ReadOnly Property ToggleConnectionButtonText As String
        Get
            If IsConnectedToDatabase Then
                Return "Disconnect"
            Else
                Return "Connect"
            End If
        End Get
    End Property

    Dim _DatabaseConnectionStatusMessageVM As New TwitterTracks.Common.UI.StatusMessageViewModel
    Public ReadOnly Property DatabaseConnectionStatusMessageVM As TwitterTracks.Common.UI.StatusMessageViewModel
        <DebuggerStepThrough()>
        Get
            Return _DatabaseConnectionStatusMessageVM
        End Get
    End Property

    Dim _IsConnectedToDatabase As Boolean = DebugConstants.MainWindowIsConnected
    Public Property IsConnectedToDatabase As Boolean
        <DebuggerStepThrough()>
        Get
            Return _IsConnectedToDatabase
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_IsConnectedToDatabase, value, "IsConnectedToDatabase")
        End Set
    End Property

#End Region

    Dim _TracksVM As New MainWindowViewModel_AdministratorTools_Tracks
    Public ReadOnly Property TracksVM As MainWindowViewModel_AdministratorTools_Tracks
        <DebuggerStepThrough()>
        Get
            Return _TracksVM
        End Get
    End Property

    Dim _ApplicationTokenVM As New MainWindowViewModel_AdministratorTools_ApplicationToken
    Public ReadOnly Property ApplicationTokenVM As MainWindowViewModel_AdministratorTools_ApplicationToken
        <DebuggerStepThrough()>
        Get
            Return _ApplicationTokenVM
        End Get
    End Property

    Dim _CreateTrackVM As New MainWindowViewModel_AdministratorTools_CreateTrack
    Public ReadOnly Property CreateTrackVM As MainWindowViewModel_AdministratorTools_CreateTrack
        <DebuggerStepThrough()>
        Get
            Return _CreateTrackVM
        End Get
    End Property

    <Dependency("Owner_IsBusy", "IsConnectedToDatabase")>
    Public ReadOnly Property ShowBusyOverlay As Boolean
        Get
            Return Owner_IsBusy OrElse Not IsConnectedToDatabase
        End Get
    End Property

    <Dependency("IsConnectedToDatabase")>
    Public ReadOnly Property BusyOverlayText As String
        Get
            If IsConnectedToDatabase Then
                Return "Loading..."
            Else
                Return "Please connect to a database first."
            End If
        End Get
    End Property

    Dim _Owner_IsBusy As Boolean = DebugConstants.MainWindowIsBusy
    Public Property Owner_IsBusy As Boolean
        <DebuggerStepThrough()>
        Get
            Return _Owner_IsBusy
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_Owner_IsBusy, value, "Owner_IsBusy")
        End Set
    End Property

End Class
