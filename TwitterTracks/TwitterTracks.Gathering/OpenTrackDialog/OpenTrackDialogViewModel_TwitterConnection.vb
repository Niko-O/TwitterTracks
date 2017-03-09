
Namespace OpenTrackDialog

    Public Class OpenTrackDialogViewModel_TwitterConnection
        Inherits ViewModelBase

        Dim _ConsumerKey As String = DebugConstants.AccessToken.ConsumerKey
        Public Property ConsumerKey As String
            <DebuggerStepThrough()>
            Get
                Return _ConsumerKey
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_ConsumerKey, value, "ConsumerKey")
            End Set
        End Property

        <Dependency("ConsumerKey")>
        Public ReadOnly Property ConsumerKeyIsValid As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace(ConsumerKey)
            End Get
        End Property

        Dim _ConsumerSecret As String = DebugConstants.AccessToken.ConsumerSecret
        Public Property ConsumerSecret As String
            <DebuggerStepThrough()>
            Get
                Return _ConsumerSecret
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_ConsumerSecret, value, "ConsumerSecret")
            End Set
        End Property

        <Dependency("ConsumerSecret")>
        Public ReadOnly Property ConsumerSecretIsValid As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace(ConsumerSecret)
            End Get
        End Property

        Dim _AccessToken As String = DebugConstants.AccessToken.AccessToken
        Public Property AccessToken As String
            <DebuggerStepThrough()>
            Get
                Return _AccessToken
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_AccessToken, value, "AccessToken")
            End Set
        End Property

        <Dependency("AccessToken")>
        Public ReadOnly Property AccessTokenIsValid As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace(AccessToken)
            End Get
        End Property

        Dim _AccessTokenSecret As String = DebugConstants.AccessToken.AccessTokenSecret
        Public Property AccessTokenSecret As String
            <DebuggerStepThrough()>
            Get
                Return _AccessTokenSecret
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_AccessTokenSecret, value, "AccessTokenSecret")
            End Set
        End Property

        <Dependency("AccessTokenSecret")>
        Public ReadOnly Property AccessTokenSecretIsValid As Boolean
            Get
                Return Not String.IsNullOrWhiteSpace(AccessTokenSecret)
            End Get
        End Property

        <Dependency("ConsumerKey", "ConsumerSecret", "AccessToken", "AccessTokenSecret")>
        Public ReadOnly Property IsValid As Boolean
            Get
                Return ConsumerKeyIsValid AndAlso ConsumerSecretIsValid AndAlso AccessTokenIsValid AndAlso AccessTokenSecretIsValid
            End Get
        End Property

    End Class

End Namespace
