Public Class StatusMessageViewModel
    Inherits ViewModelBase

    Dim _StatusMessage As String = DebugConstants.StatusMessage
    Public Property StatusMessage As String
        <DebuggerStepThrough()>
        Get
            Return _StatusMessage
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_StatusMessage, value, "StatusMessage")
        End Set
    End Property

    Dim _StatusMessageKind As StatusMessageKindType = StatusMessageKindType.JustText
    Public Property StatusMessageKind As StatusMessageKindType
        <DebuggerStepThrough()>
        Get
            Return _StatusMessageKind
        End Get
        Set(value As StatusMessageKindType)
            ExtendedChangeIfDifferent(_StatusMessageKind, value, "StatusMessageKind")
        End Set
    End Property

    <Dependency("StatusMessageKind")>
    Public ReadOnly Property StatusMessageBackground As Brush
        <DebuggerStepThrough()>
        Get
            Select Case StatusMessageKind
                Case StatusMessageKindType.JustText
                    Return Brushes.White
                Case StatusMessageKindType.Error
                    Return Application.ErrorBackgroundBrush
                Case Else
                    Throw New NopeException
            End Select
        End Get
    End Property

    Public Sub SetStatus(Message As String, Kind As StatusMessageKindType)
        StatusMessage = Message
        StatusMessageKind = Kind
    End Sub

    Public Sub ClearStatus()
        StatusMessage = ""
        StatusMessageKind = StatusMessageKindType.JustText
    End Sub

End Class
