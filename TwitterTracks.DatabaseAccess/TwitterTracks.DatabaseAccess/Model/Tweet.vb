Public Structure Tweet

    Private _EntityId As EntityId
    Public ReadOnly Property EntityId As EntityId
        <DebuggerStepThrough()>
        Get
            Return _EntityId
        End Get
    End Property

    Private _TweetId As Int64
    Public ReadOnly Property TweetId As Int64
        <DebuggerStepThrough()>
        Get
            Return _TweetId
        End Get
    End Property

    Private _ContentHash As String
    Public ReadOnly Property ContentHash As String
        <DebuggerStepThrough()>
        Get
            Return _ContentHash
        End Get
    End Property

    Private _PublishDateTime As DateTime
    Public ReadOnly Property PublishDateTime As DateTime
        <DebuggerStepThrough()>
        Get
            Return _PublishDateTime
        End Get
    End Property

    Private _LocationType As TweetLocationType
    Public ReadOnly Property LocationType As TweetLocationType
        <DebuggerStepThrough()>
        Get
            Return _LocationType
        End Get
    End Property

    Private _Location As String
    Public ReadOnly Property Location As String
        <DebuggerStepThrough()>
        Get
            Return _Location
        End Get
    End Property

    Public Sub New(NewEntityId As EntityId, NewTweetId As Int64, NewContentHash As String, NewPublishDateTime As DateTime, NewLocationType As TweetLocationType, NewLocation As String)
        _EntityId = NewEntityId
        _TweetId = NewTweetId
        _ContentHash = NewContentHash
        _PublishDateTime = NewPublishDateTime
        _LocationType = NewLocationType
        _Location = NewLocation
    End Sub

End Structure
