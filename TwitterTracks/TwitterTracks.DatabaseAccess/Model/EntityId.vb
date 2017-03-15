
<DebuggerDisplay("EntityId: {RawId}")>
Public Structure EntityId
    Implements IEquatable(Of EntityId)

    Public Shared ReadOnly Unstored As New EntityId(-1)

    Private _RawId As Int64
    Public ReadOnly Property RawId As Int64
        <DebuggerStepThrough()>
        Get
            Return _RawId
        End Get
    End Property

    Public Sub New(NewRawId As Int64)
        If NewRawId < -1 Then
            Throw New ArgumentOutOfRangeException("NewRawId", NewRawId, "The RawId must either not be negative to represent an Entity or must be -1 to represent EntityId.Unstored.")
        End If
        _RawId = NewRawId
    End Sub

#Region "Comparison"

    Public Overrides Function GetHashCode() As Integer
        Return RawId.GetHashCode
    End Function

    Public Overloads Function Equals(other As EntityId) As Boolean Implements System.IEquatable(Of EntityId).Equals
        Return Me.RawId = other.RawId
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        If Not TypeOf obj Is EntityId Then
            Return False
        End If
        Return Me.Equals(DirectCast(obj, EntityId))
    End Function

    Public Shared Operator =(Left As EntityId, Right As EntityId) As Boolean
        Return Left.Equals(Right)
    End Operator

    Public Shared Operator <>(Left As EntityId, Right As EntityId) As Boolean
        Return Not Left.Equals(Right)
    End Operator

#End Region

End Structure
