Public Structure TrackMetadata

    Private _InitialTweetId As Int64
    Public ReadOnly Property InitialTweetId As Int64
        <DebuggerStepThrough()>
        Get
            Return _InitialTweetId
        End Get
    End Property

    Private _InitialTweetUserId As Int64
    Public ReadOnly Property InitialTweetUserId As Int64
        <DebuggerStepThrough()>
        Get
            Return _InitialTweetUserId
        End Get
    End Property

    Private _InitialTweetFullText As String
    Public ReadOnly Property InitialTweetFullText As String
        <DebuggerStepThrough()>
        Get
            Return _InitialTweetFullText
        End Get
    End Property

    Private _RelevantKeywords As System.Collections.ObjectModel.ReadOnlyCollection(Of String)
    Public ReadOnly Property RelevantKeywords As System.Collections.ObjectModel.ReadOnlyCollection(Of String)
        <DebuggerStepThrough()>
        Get
            Return _RelevantKeywords
        End Get
    End Property

    Public Sub New(NewInitialTweetId As Int64, NewInitialTweetUserId As Int64, NewInitialTweetFullText As String, NewRelevantKeywords As IEnumerable(Of String))
        _InitialTweetId = NewInitialTweetId
        _InitialTweetUserId = NewInitialTweetUserId
        _InitialTweetFullText = NewInitialTweetFullText
        _RelevantKeywords = New System.Collections.ObjectModel.ReadOnlyCollection(Of String)(New List(Of String)(NewRelevantKeywords))
    End Sub

End Structure
