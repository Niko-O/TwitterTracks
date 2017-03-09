
Namespace TweetinviInterop

    Public Class ServiceProvider

        Private Shared _Service As ITwitterService = Nothing
        Public Shared ReadOnly Property Service As ITwitterService
            <DebuggerStepThrough()>
            Get
                Return _Service
            End Get
        End Property

        Public Shared Sub RegisterService(NewService As ITwitterService)
            If _Service IsNot Nothing Then
                Throw New InvalidOperationException("The service has already been registered.")
            End If
            _Service = NewService
        End Sub

    End Class

End Namespace
