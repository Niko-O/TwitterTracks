Public Class MainWindowViewModel
    Inherits ViewModelBase

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
            RootToolsVM.Owner_IsBusy = value
            AdministratorToolsVM.Owner_IsBusy = value
            ResearcherToolsVM.Owner_IsBusy = value
        End Set
    End Property

End Class
