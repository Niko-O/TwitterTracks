
Namespace Streaming

    Public Structure ValidationResult

        Private _IsValid As Boolean
        Public ReadOnly Property IsValid As Boolean
            <DebuggerStepThrough()>
            Get
                Return _IsValid
            End Get
        End Property

        Private _ErrorMessage As String
        Public ReadOnly Property ErrorMessage As String
            <DebuggerStepThrough()>
            Get
                Return _ErrorMessage
            End Get
        End Property

        Public Shared Function Success() As ValidationResult
            Return New ValidationResult With {._IsValid = True, ._ErrorMessage = Nothing}
        End Function

        Public Shared Function Fail(NewErrorMessage As String) As ValidationResult
            Return New ValidationResult With {._IsValid = False, ._ErrorMessage = NewErrorMessage}
        End Function

    End Structure

End Namespace
