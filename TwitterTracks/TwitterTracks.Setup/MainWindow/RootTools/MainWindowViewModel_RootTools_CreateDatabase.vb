Public Class MainWindowViewModel_RootTools_CreateDatabase
    Inherits ViewModelBase

    Dim _DatabaseName As String = TwitterTracks.Common.UI.Resources.DebugConstants.TrackDatabaseName
    Public Property DatabaseName As String
        <DebuggerStepThrough()>
        Get
            Return _DatabaseName
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_DatabaseName, value, "DatabaseName")
        End Set
    End Property

    <Dependency("DatabaseName")>
    Public ReadOnly Property DatabaseNameIsValid As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(DatabaseName)
        End Get
    End Property

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

    <Dependency("DatabaseNameIsValid", "PasswordsMatch")>
    Public ReadOnly Property CanCreateDatabase As Boolean
        Get
            Return DatabaseNameIsValid AndAlso PasswordsMatch
        End Get
    End Property

    Dim _StatusMessageVM As New TwitterTracks.Common.UI.StatusMessageViewModel
    Public ReadOnly Property StatusMessageVM As TwitterTracks.Common.UI.StatusMessageViewModel
        <DebuggerStepThrough()>
        Get
            Return _StatusMessageVM
        End Get
    End Property

    Dim _CreatedAdministratorId As String = ""
    Public Property CreatedAdministratorId As String
        <DebuggerStepThrough()>
        Get
            Return _CreatedAdministratorId
        End Get
        Set(value As String)
            If ExtendedChangeIfDifferent(_CreatedAdministratorId, value, "CreatedAdministratorId") Then
                CopyCreatedAdministratorIdCommand.IsEnabled = Not String.IsNullOrWhiteSpace(_CreatedAdministratorId)
            End If
        End Set
    End Property

    Public ReadOnly Property CopyCreatedAdministratorIdCommand As DelegateCommand
        Get
            Static Temp As New DelegateCommand(Sub() Helpers.SetClipboard(CreatedAdministratorId))
            Return Temp
        End Get
    End Property

    Dim WithEvents _PreviouslyCreatedDatabases As New TypedObservableCollection(Of String)
    Public ReadOnly Property PreviouslyCreatedDatabases As TypedObservableCollection(Of String)
        <DebuggerStepThrough()>
        Get
            Return _PreviouslyCreatedDatabases
        End Get
    End Property

    Dim _SelectedPreviouslyCreatedDatabase As String = Nothing
    Public Property SelectedPreviouslyCreatedDatabase As String
        <DebuggerStepThrough()>
        Get
            Return _SelectedPreviouslyCreatedDatabase
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_SelectedPreviouslyCreatedDatabase, value, "SelectedPreviouslyCreatedDatabase")
        End Set
    End Property

End Class
