Public Class MainWindowViewModel
    Inherits ViewModelBase

    Dim _ConnectionVM As New MainWindowViewModel_Connection
    Public ReadOnly Property ConnectionVM As MainWindowViewModel_Connection
        <DebuggerStepThrough()>
        Get
            Return _ConnectionVM
        End Get
    End Property

    Dim _RootToolsVM As New MainWindowViewModel_RootTools
    Public ReadOnly Property RootToolsVM As MainWindowViewModel_RootTools
        <DebuggerStepThrough()>
        Get
            Return _RootToolsVM
        End Get
    End Property

    Dim _AdministratorToolsVM As New MainWindowViewModel_AdministratorTools
    Public ReadOnly Property AdministratorToolsVM As MainWindowViewModel_AdministratorTools
        <DebuggerStepThrough()>
        Get
            Return _AdministratorToolsVM
        End Get
    End Property

    Dim _ResearcherToolsVM As New MainWindowViewModel_ResearcherTools
    Public ReadOnly Property ResearcherToolsVM As MainWindowViewModel_ResearcherTools
        <DebuggerStepThrough()>
        Get
            Return _ResearcherToolsVM
        End Get
    End Property

    Dim _IsBusy As Boolean = DebugConstants.MainWindowIsBusy
    Public Property IsBusy As Boolean
        <DebuggerStepThrough()>
        Get
            Return _IsBusy
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_IsBusy, value, "IsBusy")
        End Set
    End Property

    Dim _IsConnectedToDatabase As Boolean = DebugConstants.MainWindowIsConnected
    Public Property IsConnectedToDatabase As Boolean
        <DebuggerStepThrough()>
        Get
            Return _IsConnectedToDatabase
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_IsConnectedToDatabase, value, "IsConnectedToDatabase")
            AdministratorToolsVM.Owner_IsConnectedToDatabase = value
        End Set
    End Property

    <Dependency("IsConnectedToDatabase", "IsBusy")>
    Public ReadOnly Property ShowToolsBusyOverlay As Boolean
        Get
            Return Not IsConnectedToDatabase OrElse IsBusy
        End Get
    End Property

    <Dependency("IsConnectedToDatabase")>
    Public ReadOnly Property ToolsBusyOverlayText As String
        Get
            If Not IsConnectedToDatabase Then
                Return "Please connect to a database first."
            End If
            Return "Loading..."
        End Get
    End Property

End Class
