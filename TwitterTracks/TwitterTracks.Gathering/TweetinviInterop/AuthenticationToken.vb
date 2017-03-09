
Namespace TweetinviInterop

    Public Structure AuthenticationToken

        Private _ConsumerKey As String
        Public ReadOnly Property ConsumerKey As String
            <DebuggerStepThrough()>
            Get
                Return _ConsumerKey
            End Get
        End Property

        Private _ConsumerSecret As String
        Public ReadOnly Property ConsumerSecret As String
            <DebuggerStepThrough()>
            Get
                Return _ConsumerSecret
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

        Public Sub New(NewConsumerKey As String, NewConsumerSecret As String, NewAccessToken As String, NewAccessTokenSecret As String)
            _ConsumerKey = NewConsumerKey
            _ConsumerSecret = NewConsumerSecret
            _AccessToken = NewAccessToken
            _AccessTokenSecret = NewAccessTokenSecret
        End Sub

    End Structure

End Namespace
