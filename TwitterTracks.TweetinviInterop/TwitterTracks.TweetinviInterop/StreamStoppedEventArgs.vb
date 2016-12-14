Public Class StreamStoppedEventArgs
    Inherits EventArgs
    'LimitReached
    '  NumberOfTweetsNotReceived
    'Stopped
    ' Exception
    ' Message
    '  Code Integer
    '  Reason String
    '  StreamName

    Dim _Reason As StreamStopReason
    Public ReadOnly Property Reason As StreamStopReason
        <DebuggerStepThrough()>
        Get
            Return _Reason
        End Get
    End Property

    Dim _Code As Integer
    Public ReadOnly Property Code As Integer
        <DebuggerStepThrough()>
        Get
            Return _Code
        End Get
    End Property

    Dim _ReasonName As String
    Public ReadOnly Property ReasonName As String
        <DebuggerStepThrough()>
        Get
            Return _ReasonName
        End Get
    End Property

    Dim _StreamName As String
    Public ReadOnly Property StreamName As String
        <DebuggerStepThrough()>
        Get
            Return _StreamName
        End Get
    End Property

    Dim _NumberOfTweetsNotReceived As Integer
    Public ReadOnly Property NumberOfTweetsNotReceived As Integer
        <DebuggerStepThrough()>
        Get
            Return _NumberOfTweetsNotReceived
        End Get
    End Property

    Dim _Exception As Exception
    Public ReadOnly Property Exception As Exception
        <DebuggerStepThrough()>
        Get
            Return _Exception
        End Get
    End Property

    Private Sub New(NewReason As StreamStopReason, NewCode As Integer, NewReasonName As String, NewStreamName As String, NewNumberOfTweetsNotReceived As Integer, NewException As Exception)
        _Reason = NewReason
        _Code = NewCode
        _ReasonName = NewReasonName
        _StreamName = NewStreamName
        _NumberOfTweetsNotReceived = NewNumberOfTweetsNotReceived
        _Exception = NewException
    End Sub

    Public Shared Function Disconnect(Code As Integer, Reason As String, StreamName As String) As StreamStoppedEventArgs
        Return New StreamStoppedEventArgs(StreamStopReason.Disconnected, Code, Reason, StreamName, -1, Nothing)
    End Function

    Public Shared Function LimitReached(NumberOfTweetsNotReceived As Integer) As StreamStoppedEventArgs
        Return New StreamStoppedEventArgs(StreamStopReason.LimitReached, -1, Nothing, Nothing, NumberOfTweetsNotReceived, Nothing)
    End Function

    Public Shared Function Stopped(Exception As Exception, Code As Integer, Reason As String, StreamName As String) As StreamStoppedEventArgs
        Return New StreamStoppedEventArgs(StreamStopReason.Stopped, Code, Reason, StreamName, -1, Exception)
    End Function

End Class
