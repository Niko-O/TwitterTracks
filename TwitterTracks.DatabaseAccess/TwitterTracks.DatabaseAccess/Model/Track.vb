Public Class Track

    Dim _EntityId As EntityId
    Public ReadOnly Property EntityId As EntityId
        <DebuggerStepThrough()>
        Get
            Return _EntityId
        End Get
    End Property

    Dim _Metadata As TrackMetadata?
    Public ReadOnly Property Metadata As TrackMetadata?
        <DebuggerStepThrough()>
        Get
            Return _Metadata
        End Get
    End Property

    Public Sub New(NewEntityId As EntityId, NewMetadata As TrackMetadata?)
        _EntityId = NewEntityId
        _Metadata = NewMetadata
    End Sub

End Class
