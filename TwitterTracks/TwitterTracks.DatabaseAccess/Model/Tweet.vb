
Imports System.Collections.ObjectModel

Public Structure Tweet

    Private _EntityId As EntityId
    Public ReadOnly Property EntityId As EntityId
        <DebuggerStepThrough()>
        Get
            Return _EntityId
        End Get
    End Property

    Private _IsRetweet As Boolean
    Public ReadOnly Property IsRetweet As Boolean
        <DebuggerStepThrough()>
        Get
            Return _IsRetweet
        End Get
    End Property

    Private _MatchingKeywords As ReadOnlyCollection(Of String)
    Public ReadOnly Property MatchingKeywords As ReadOnlyCollection(Of String)
        <DebuggerStepThrough()>
        Get
            Return _MatchingKeywords
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

    Public Sub New(NewEntityId As EntityId, NewIsRetweet As Boolean, NewMatchingKeywords As IEnumerable(Of String), NewPublishDateTime As DateTime, NewLocation As TweetLocation, NewDebug_TweetId As Int64?, NewDebug_TweetContent As String)
        _EntityId = NewEntityId
        _IsRetweet = NewIsRetweet
        _MatchingKeywords = New ReadOnlyCollection(Of String)(NewMatchingKeywords.ToList)
        _PublishDateTime = NewPublishDateTime
        _Location = NewLocation
        _Debug_TweetId = NewDebug_TweetId
        _Debug_TweetContent = NewDebug_TweetContent
    End Sub

End Structure
