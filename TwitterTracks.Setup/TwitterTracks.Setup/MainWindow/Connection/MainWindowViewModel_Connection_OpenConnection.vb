Public Class MainWindowViewModel_Connection_OpenConnection
    Inherits ViewModelBase

    Dim _DatabaseHost As String = DebugConstants.DatabaseHost
    Public Property DatabaseHost As String
        <DebuggerStepThrough()>
        Get
            Return _DatabaseHost
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_DatabaseHost, value, "DatabaseHost")
        End Set
    End Property

    Dim _UserName As String = DebugConstants.DatabaseUserName
    Public Property UserName As String
        <DebuggerStepThrough()>
        Get
            Return _UserName
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_UserName, value, "UserName")
        End Set
    End Property

    Dim _Password As String = DebugConstants.DatabasePassword
    Public Property Password As String
        <DebuggerStepThrough()>
        Get
            Return _Password
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_Password, value, "Password")
        End Set
    End Property

    <Dependency("Owner_IsConnectedToDatabase")>
    Public ReadOnly Property ToggleConnectionButtonText As String
        Get
            If Owner_IsConnectedToDatabase Then
                Return "Disconnect"
            Else
                Return "Connect"
            End If
        End Get
    End Property
    
    Dim _StatusMessageVM As New StatusMessageViewModel
    Public ReadOnly Property StatusMessageVM As StatusMessageViewModel
        <DebuggerStepThrough()>
        Get
            Return _StatusMessageVM
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
