
Namespace OpenTrackDialog

    Public Class OpenTrackDialogViewModel_Summary
        Inherits ViewModelBase

        Dim _TweetText As String = If(IsInDesignMode, TwitterTracks.Common.UI.Resources.DebugConstants.TweetText, "")
        Public Property TweetText As String
            <DebuggerStepThrough()>
            Get
                Return _TweetText
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_TweetText, value, "TweetText")
            End Set
        End Property

        Dim _TweetAlreadyPublished As Boolean = False
        Public Property TweetAlreadyPublished As Boolean
            <DebuggerStepThrough()>
            Get
                Return _TweetAlreadyPublished
            End Get
            Set(value As Boolean)
                ExtendedChangeIfDifferent(_TweetAlreadyPublished, value, "TweetAlreadyPublished")
            End Set
        End Property

        Dim _PublishedTweetId As Int64
        Public Property PublishedTweetId As Int64
            <DebuggerStepThrough()>
            Get
                Return _PublishedTweetId
            End Get
            Set(value As Int64)
                ExtendedChangeIfDifferent(_PublishedTweetId, value, "PublishedTweetId")
            End Set
        End Property

        Dim _Keywords As String = ""
        Public Property Keywords As String
            <DebuggerStepThrough()>
            Get
                Return _Keywords
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_Keywords, value, "Keywords")
            End Set
        End Property

    End Class

End Namespace
