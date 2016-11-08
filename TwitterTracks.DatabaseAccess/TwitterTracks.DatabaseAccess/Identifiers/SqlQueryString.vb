
<DebuggerDisplay("SqlQueryString: {QueryText}")>
Public Structure SqlQueryString

    Private _QueryText As String
    Public ReadOnly Property QueryText As String
        <DebuggerStepThrough()>
        Get
            Return _QueryText
        End Get
    End Property

    Public Sub New(NewQueryText As String)
        _QueryText = NewQueryText
    End Sub

End Structure
