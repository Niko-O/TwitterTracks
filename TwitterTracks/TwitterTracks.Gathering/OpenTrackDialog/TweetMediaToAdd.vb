
Namespace OpenTrackDialog

    Public Class TweetMediaToAdd
        Inherits NotifyPropertyChanged

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

        Dim _Exists As Boolean = False
        Public Property Exists As Boolean
            <DebuggerStepThrough()>
            Get
                Return _Exists
            End Get
            Private Set(value As Boolean)
                ExtendedChangeIfDifferent(_Exists, value, "Exists")
            End Set
        End Property

        Public Sub New(NewFilePath As String)
            _FilePath = NewFilePath
            UpdateExists()
        End Sub

        Public Sub UpdateExists()
            Exists = System.IO.File.Exists(FilePath)
        End Sub

    End Class

End Namespace
