
Namespace Streaming

    Public Class StartTwitterStreamResult

        Dim _Success As Boolean
        Public ReadOnly Property Success As Boolean
            <DebuggerStepThrough()>
            Get
                Return _Success
            End Get
        End Property

        Dim _FailReason As PublishTweetFailReason
        Public ReadOnly Property FailReason As PublishTweetFailReason
            <DebuggerStepThrough()>
            Get
                Return _FailReason
            End Get
        End Property

        Dim _ErrorException As Exception
        Public ReadOnly Property ErrorException As Exception
            <DebuggerStepThrough()>
            Get
                Return _ErrorException
            End Get
        End Property

        Public Sub New()
            _Success = True
            _FailReason = PublishTweetFailReason.None
            _ErrorException = Nothing
        End Sub

        Public Sub New(NewFailReason As PublishTweetFailReason, NewErrorException As Exception)
            _Success = False
            _FailReason = NewFailReason
            _ErrorException = NewErrorException
        End Sub

    End Class

End Namespace
