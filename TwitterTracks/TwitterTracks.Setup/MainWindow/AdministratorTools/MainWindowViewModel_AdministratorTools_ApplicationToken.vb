Public Class MainWindowViewModel_AdministratorTools_ApplicationToken
    Inherits ViewModelBase

    Dim _ConsumerKeyInDatabase As String = ""
    Public Property ConsumerKeyInDatabase As String
        <DebuggerStepThrough()>
        Get
            Return _ConsumerKeyInDatabase
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_ConsumerKeyInDatabase, value, "ConsumerKeyInDatabase")
        End Set
    End Property

    Dim _ConsumerSecretInDatabase As String = ""
    Public Property ConsumerSecretInDatabase As String
        <DebuggerStepThrough()>
        Get
            Return _ConsumerSecretInDatabase
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_ConsumerSecretInDatabase, value, "ConsumerSecretInDatabase")
        End Set
    End Property

    Dim _ConsumerKeyToSet As String = ""
    Public Property ConsumerKeyToSet As String
        <DebuggerStepThrough()>
        Get
            Return _ConsumerKeyToSet
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_ConsumerKeyToSet, value, "ConsumerKeyToSet")
        End Set
    End Property

    Dim _ConsumerSecretToSet As String = ""
    Public Property ConsumerSecretToSet As String
        <DebuggerStepThrough()>
        Get
            Return _ConsumerSecretToSet
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_ConsumerSecretToSet, value, "ConsumerSecretToSet")
        End Set
    End Property

    <Dependency("ConsumerKeyToSet")>
    Public ReadOnly Property ConsumerKeyToSetIsValid As Boolean
        Get
            Return Not String.IsNullOrEmpty(ConsumerKeyToSet)
        End Get
    End Property

    <Dependency("ConsumerSecretToSet")>
    Public ReadOnly Property ConsumerSecretToSetIsValid As Boolean
        Get
            Return Not String.IsNullOrEmpty(ConsumerSecretToSet)
        End Get
    End Property

    <Dependency("ConsumerKeyToSetIsValid", "ConsumerSecretToSetIsValid")>
    Public ReadOnly Property CanSave As Boolean
        Get
            Return ConsumerKeyToSetIsValid AndAlso ConsumerSecretToSetIsValid
        End Get
    End Property

    Dim _IsStoredInDatabase As Boolean = False
    Public Property IsStoredInDatabase As Boolean
        <DebuggerStepThrough()>
        Get
            Return _IsStoredInDatabase
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_IsStoredInDatabase, value, "IsStoredInDatabase")
        End Set
    End Property

    Dim _StatusMessageVM As New TwitterTracks.Common.UI.StatusMessageViewModel
    Public ReadOnly Property StatusMessageVM As TwitterTracks.Common.UI.StatusMessageViewModel
        <DebuggerStepThrough()>
        Get
            Return _StatusMessageVM
        End Get
    End Property

End Class
