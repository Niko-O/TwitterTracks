
Namespace OpenTrackDialog

    Public Class TweetMediaToAdd

        Public Event Remove(Sender As TweetMediaToAdd)
        Public ReadOnly Property RemoveCommand As DelegateCommand
            Get
                Static Temp As New DelegateCommand(Sub() RaiseEvent Remove(Me))
                Return Temp
            End Get
        End Property

        Dim _FilePath As String
        Public ReadOnly Property FilePath As String
            <DebuggerStepThrough()>
            Get
                Return _FilePath
            End Get
        End Property

        Public Sub New(NewFilePath As String)
            _FilePath = NewFilePath
        End Sub

    End Class

End Namespace
