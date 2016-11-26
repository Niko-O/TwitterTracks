Public Class MainWindowViewModel_Connection
    Inherits ViewModelBase

    Dim _DatabaseConnectionVM As New MainWindowViewModel_Connection_DatabaseConnection
    Public ReadOnly Property DatabaseConnectionVM As MainWindowViewModel_Connection_DatabaseConnection
        <DebuggerStepThrough()>
        Get
            Return _DatabaseConnectionVM
        End Get
    End Property

    Dim _TwitterConnectionVM As New MainWindowViewModel_Connection_TwitterConnection
    Public ReadOnly Property TwitterConnectionVM As MainWindowViewModel_Connection_TwitterConnection
        <DebuggerStepThrough()>
        Get
            Return _TwitterConnectionVM
        End Get
    End Property

End Class
