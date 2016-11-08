Public Class MainWindowViewModel_RootTools
    Inherits ViewModelBase

    Dim _DatabasesVM As New MainWindowViewModel_RootTools_Databases
    Public ReadOnly Property DatabasesVM As MainWindowViewModel_RootTools_Databases
        <DebuggerStepThrough()>
        Get
            Return _DatabasesVM
        End Get
    End Property

    Dim _CreateDatabaseVM As New MainWindowViewModel_RootTools_CreateDatabase
    Public ReadOnly Property CreateDatabaseVM As MainWindowViewModel_RootTools_CreateDatabase
        <DebuggerStepThrough()>
        Get
            Return _CreateDatabaseVM
        End Get
    End Property

End Class
