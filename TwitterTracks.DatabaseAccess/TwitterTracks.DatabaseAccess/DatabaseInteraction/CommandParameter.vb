Public Class CommandParameter

    Dim _Name As String
    Public ReadOnly Property Name As String
        <DebuggerStepThrough()>
        Get
            Return _Name
        End Get
    End Property

    Dim _Value As Object
    Public ReadOnly Property Value As Object
        <DebuggerStepThrough()>
        Get
            Return _Value
        End Get
    End Property

    Public Sub New(NewName As String, NewValue As Object)
        _Name = NewName
        _Value = NewValue
    End Sub

End Class
