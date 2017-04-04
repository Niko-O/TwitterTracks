
<DebuggerDisplay("EscapedIdentifier: {EscapedText}")>
Public Class EscapedIdentifier
    Implements IEquatable(Of EscapedIdentifier)

    Dim _EscapedText As String
    Public ReadOnly Property EscapedText As String
        <DebuggerStepThrough()>
        Get
            Return _EscapedText
        End Get
    End Property

    Public Sub New(NewEscapedText As String)
        _EscapedText = NewEscapedText
    End Sub

#Region "Comparison"

    Public Overrides Function GetHashCode() As Integer
        Return EscapedText.GetHashCode
    End Function

    Public Overloads Function Equals(other As EscapedIdentifier) As Boolean Implements IEquatable(Of EscapedIdentifier).Equals
        Return Me.EscapedText = other.EscapedText
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        If Not TypeOf obj Is EscapedIdentifier Then
            Return False
        End If
        Return Me.Equals(DirectCast(obj, EscapedIdentifier))
    End Function

    Public Shared Operator =(Left As EscapedIdentifier, Right As EscapedIdentifier) As Boolean
        Return Left.Equals(Right)
    End Operator

    Public Shared Operator <>(Left As EscapedIdentifier, Right As EscapedIdentifier) As Boolean
        Return Not Left.Equals(Right)
    End Operator

#End Region

End Class
