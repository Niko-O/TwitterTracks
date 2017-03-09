Public Class MainWindowViewModel_Connection
    Inherits ViewModelBase

    Dim _OpenConnectionVM As New MainWindowViewModel_Connection_OpenConnection
    Public ReadOnly Property OpenConnectionVM As MainWindowViewModel_Connection_OpenConnection
        <DebuggerStepThrough()>
        Get
            Return _OpenConnectionVM
        End Get
    End Property

End Class
