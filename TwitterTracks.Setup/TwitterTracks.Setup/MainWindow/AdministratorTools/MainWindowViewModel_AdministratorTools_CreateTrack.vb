Public Class MainWindowViewModel_AdministratorTools_CreateTrack
    Inherits ViewModelBase

    Dim _Password As String = ""
    Public Property Password As String
        <DebuggerStepThrough()>
        Get
            Return _Password
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_Password, value, "Password")
        End Set
    End Property

    Dim _RetypePassword As String = ""
    Public Property RetypePassword As String
        <DebuggerStepThrough()>
        Get
            Return _RetypePassword
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_RetypePassword, value, "RetypePassword")
        End Set
    End Property

    <Dependency("Password", "RetypePassword")>
    Public ReadOnly Property PasswordsMatch As Boolean
        Get
            Return Password = RetypePassword
        End Get
    End Property

    <Dependency("PasswordsMatch")>
    Public ReadOnly Property CanCreateTrack As Boolean
        Get
            Return PasswordsMatch
        End Get
    End Property

    Dim _StatusMessageVM As New StatusMessageViewModel
    Public ReadOnly Property StatusMessageVM As StatusMessageViewModel
        <DebuggerStepThrough()>
        Get
            Return _StatusMessageVM
        End Get
    End Property

    Dim _CreatedResearcherId As String = ""
    Public Property CreatedResearcherId As String
        <DebuggerStepThrough()>
        Get
            Return _CreatedResearcherId
        End Get
        Set(value As String)
            If ExtendedChangeIfDifferent(_CreatedResearcherId, value, "CreatedResearcherId") Then
                CopyCreatedResearcherIdCommand.IsEnabled = Not String.IsNullOrWhiteSpace(_CreatedResearcherId)
            End If
        End Set
    End Property

    Public ReadOnly Property CopyCreatedResearcherIdCommand As DelegateCommand
        Get
            Static Temp As New DelegateCommand(Sub() Helpers.SetClipboard(CreatedResearcherId))
            Return Temp
        End Get
    End Property

End Class
