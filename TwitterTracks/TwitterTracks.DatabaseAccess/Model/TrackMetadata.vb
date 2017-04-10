
Imports System.Collections.ObjectModel

<Serializable()>
Public Structure TrackMetadata

    Private _IsPublished As Boolean
    Public ReadOnly Property IsPublished As Boolean
        <DebuggerStepThrough()>
        Get
            Return _IsPublished
        End Get
    End Property

    Private _TweetId As Int64
    Public ReadOnly Property TweetId As Int64
        <DebuggerStepThrough()>
        Get
            Return _TweetId
        End Get
    End Property

    Private _CreatedByUserId As Int64
    Public ReadOnly Property CreatedByUserId As Int64
        <DebuggerStepThrough()>
        Get
            Return _CreatedByUserId
        End Get
    End Property

    Private _TweetText As String
    Public ReadOnly Property TweetText As String
        <DebuggerStepThrough()>
        Get
            Return _TweetText
        End Get
    End Property

    Private _RelevantKeywords As ReadOnlyCollection(Of String)
    Public ReadOnly Property RelevantKeywords As ReadOnlyCollection(Of String)
        <DebuggerStepThrough()>
        Get
            Return _RelevantKeywords
        End Get
    End Property

    Private _MediaFilePathsToAdd As ReadOnlyCollection(Of String)
    Public ReadOnly Property MediaFilePathsToAdd As ReadOnlyCollection(Of String)
        <DebuggerStepThrough()>
        Get
            Return _MediaFilePathsToAdd
        End Get
    End Property

    Private _AccessToken As String
    Public ReadOnly Property AccessToken As String
        <DebuggerStepThrough()>
        Get
            Return _AccessToken
        End Get
    End Property

    Private _AccessTokenSecret As String
    Public ReadOnly Property AccessTokenSecret As String
        <DebuggerStepThrough()>
        Get
            Return _AccessTokenSecret
        End Get
    End Property

    Public Sub New(NewIsPublished As Boolean, _
                   NewTweetId As Int64, _
                   NewCreatedByUserId As Int64, _
                   NewTweetText As String, _
                   NewRelevantKeywords As IEnumerable(Of String), _
                   NewMediaFilePathsToAdd As IEnumerable(Of String), _
                   NewAccessToken As String, _
                   NewAccessTokenSecret As String)
        If NewAccessToken Is Nothing Then
            Throw New ArgumentNullException("NewAccessToken")
        End If
        If NewAccessTokenSecret Is Nothing Then
            Throw New ArgumentNullException("NewAccessTokenSecret")
        End If
        _IsPublished = NewIsPublished
        _TweetId = NewTweetId
        _CreatedByUserId = NewCreatedByUserId
        _TweetText = NewTweetText
        _RelevantKeywords = New ReadOnlyCollection(Of String)(NewRelevantKeywords.ToList)
        _MediaFilePathsToAdd = New ReadOnlyCollection(Of String)(NewMediaFilePathsToAdd.ToList)
        _AccessToken = NewAccessToken
        _AccessTokenSecret = NewAccessTokenSecret
    End Sub

    Public Function WithPublishedData(NewTweetId As Int64, NewCreatedByUserId As Int64) As TrackMetadata
        Dim Copy = Me
        Copy._IsPublished = True
        Copy._TweetId = NewTweetId
        Copy._CreatedByUserId = NewCreatedByUserId
        Return Copy
    End Function

    Public Shared Function FromUnpublished(NewTweetText As String, _
                                           NewRelevantKeywords As IEnumerable(Of String), _
                                           NewMediaFilePathsToAdd As IEnumerable(Of String), _
                                           NewAccessToken As String, _
                                           NewAccessTokenSecret As String) As TrackMetadata
        Return New TrackMetadata(False, 0, 0, _
                                 NewTweetText, _
                                 NewRelevantKeywords, _
                                 NewMediaFilePathsToAdd, _
                                 NewAccessToken, _
                                 NewAccessTokenSecret)
    End Function

    Public Shared Function FromPublished(NewTweetId As Int64, _
                                         NewCreatedByUserId As Int64, _
                                         NewTweetText As String, _
                                         NewRelevantKeywords As IEnumerable(Of String), _
                                         NewMediaFilePathsToAdd As IEnumerable(Of String), _
                                         NewAccessToken As String, _
                                         NewAccessTokenSecret As String) As TrackMetadata
        Return New TrackMetadata(True, _
                                 NewTweetId, _
                                 NewCreatedByUserId, _
                                 NewTweetText, _
                                 NewRelevantKeywords, _
                                 NewMediaFilePathsToAdd, _
                                 NewAccessToken, _
                                 NewAccessTokenSecret)
    End Function

End Structure
