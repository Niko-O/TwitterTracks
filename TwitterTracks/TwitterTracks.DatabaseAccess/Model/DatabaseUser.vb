Public Class DatabaseUser

    Dim _Name As String
    Public ReadOnly Property Name As String
        <DebuggerStepThrough()>
        Get
            Return _Name
        End Get
    End Property

    Dim _Password As String
    Public ReadOnly Property Password As String
        <DebuggerStepThrough()>
        Get
            Return _Password
        End Get
    End Property

    Public Sub New(NewName As String, NewPassword As String)
        _Name = NewName
        _Password = NewPassword
    End Sub

End Class
