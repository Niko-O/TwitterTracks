Public Structure Tweet

    Private _EntityId As EntityId
    Public ReadOnly Property EntityId As EntityId
        <DebuggerStepThrough()>
        Get
            Return _EntityId
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

    Private _Location As TweetLocation
    Public ReadOnly Property Location As TweetLocation
        <DebuggerStepThrough()>
        Get
            Return _Location
        End Get
    End Property

    Dim _Debug_TweetId As Int64?
    Public ReadOnly Property Debug_TweetId As Int64?
        <DebuggerStepThrough()>
        Get
            Return _Debug_TweetId
        End Get
    End Property

    Dim _Debug_TweetContent As String
    Public ReadOnly Property Debug_TweetContent As String
        <DebuggerStepThrough()>
        Get
            Return _Debug_TweetContent
        End Get
    End Property

    Public Sub New(NewEntityId As EntityId, NewContentHash As String, NewPublishDateTime As DateTime, NewLocation As TweetLocation, NewDebug_TweetId As Int64?, NewDebug_TweetContent As String)
        _EntityId = NewEntityId
        _ContentHash = NewContentHash
        _PublishDateTime = NewPublishDateTime
        _Location = NewLocation
        _Debug_TweetId = NewDebug_TweetId
        _Debug_TweetContent = NewDebug_TweetContent
    End Sub

End Structure
