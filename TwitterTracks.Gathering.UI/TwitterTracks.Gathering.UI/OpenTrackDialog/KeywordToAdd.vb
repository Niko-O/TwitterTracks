
Namespace OpenTrackDialog

    Public Class KeywordToAdd
        Inherits NotifyPropertyChanged

        Public Event Remove(Sender As KeywordToAdd)
        Public ReadOnly Property RemoveCommand As DelegateCommand
            Get
                Static Temp As New DelegateCommand(Sub() RaiseEvent Remove(Me))
                Return Temp
            End Get
        End Property

        Dim _Text As String = ""
        Public Property Text As String
            <DebuggerStepThrough()>
            Get
                Return _Text
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_Text, value, "Text")
            End Set
        End Property

        Dim _IsCustom As Boolean = False
        Public Property IsCustom As Boolean
            <DebuggerStepThrough()>
            Get
                Return _IsCustom
            End Get
            Set(value As Boolean)
                ExtendedChangeIfDifferent(_IsCustom, value, "IsCustom")
            End Set
        End Property

    End Class

End Namespace
