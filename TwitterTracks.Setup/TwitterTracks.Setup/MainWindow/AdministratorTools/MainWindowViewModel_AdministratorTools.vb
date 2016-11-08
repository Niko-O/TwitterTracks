Public Class MainWindowViewModel_AdministratorTools
    Inherits ViewModelBase

    Dim _DatabaseName As String = DebugConstants.DatabaseName
    Public Property DatabaseName As String
        <DebuggerStepThrough()>
        Get
            Return _DatabaseName
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_DatabaseName, value, "DatabaseName")
        End Set
    End Property

    Dim _DatabaseIsSelected As Boolean = DebugConstants.AdministratorDatabaseIsSelected
    Public Property DatabaseIsSelected As Boolean
        <DebuggerStepThrough()>
        Get
            Return _DatabaseIsSelected
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_DatabaseIsSelected, value, "DatabaseIsSelected")
        End Set
    End Property

    <Dependency("DatabaseIsSelected")>
    Public ReadOnly Property ToggleDatabaseSelectionButtonText As String
        Get
            If DatabaseIsSelected Then
                Return "Select a different database"
            Else
                Return "Select this database"
            End If
        End Get
    End Property

    Public ReadOnly Property ToggleDatabaseSelectionCommand As DelegateCommand
        Get
            Static Temp As New DelegateCommand(Sub() DatabaseIsSelected = Not DatabaseIsSelected)
            Return Temp
        End Get
    End Property

    Dim _SelectionStatusMessageVM As New StatusMessageViewModel
    Public ReadOnly Property SelectionStatusMessageVM As StatusMessageViewModel
        <DebuggerStepThrough()>
        Get
            Return _SelectionStatusMessageVM
        End Get
    End Property

    <Dependency("DatabaseIsSelected", "Owner_IsConnectedToDatabase")>
    Public ReadOnly Property ShowInnerBusyOverlay As Boolean
        Get
            Return Not DatabaseIsSelected AndAlso Owner_IsConnectedToDatabase
        End Get
    End Property

    Dim _TracksVM As New MainWindowViewModel_AdministratorTools_Tracks
    Public ReadOnly Property TracksVM As MainWindowViewModel_AdministratorTools_Tracks
        <DebuggerStepThrough()>
        Get
            Return _TracksVM
        End Get
    End Property

    Dim _CreateTrackVM As New MainWindowViewModel_AdministratorTools_CreateTrack
    Public ReadOnly Property CreateTrackVM As MainWindowViewModel_AdministratorTools_CreateTrack
        <DebuggerStepThrough()>
        Get
            Return _CreateTrackVM
        End Get
    End Property

    Dim _Owner_IsConnectedToDatabase As Boolean = DebugConstants.MainWindowIsConnected
    Public Property Owner_IsConnectedToDatabase As Boolean
        <DebuggerStepThrough()>
        Get
            Return _Owner_IsConnectedToDatabase
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_Owner_IsConnectedToDatabase, value, "Owner_IsConnectedToDatabase")
        End Set
    End Property

End Class
