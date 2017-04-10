
Namespace OpenTrackDialog

    Public Class OpenTrackDialogViewModel_TwitterConnection
        Inherits ViewModelBase

        Dim _AuthorizationPin As String = ""
        Public Property AuthorizationPin As String
            <DebuggerStepThrough()>
            Get
                Return _AuthorizationPin
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_AuthorizationPin, value, "AuthorizationPin")
            End Set
        End Property

        Dim _PinIsValid As Boolean = False
        Public Property PinIsValid As Boolean
            <DebuggerStepThrough()>
            Get
                Return _PinIsValid
            End Get
            Set(value As Boolean)
                ExtendedChangeIfDifferent(_PinIsValid, value, "PinIsValid")
            End Set
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

        <Dependency("PinIsValid")>
        Public ReadOnly Property IsValid As Boolean
            Get
                Return PinIsValid
            End Get
        End Property

    End Class

End Namespace
