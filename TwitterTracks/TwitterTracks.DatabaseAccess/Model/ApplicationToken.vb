Public Structure ApplicationToken

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

    Public Sub New(NewConsumerKey As String, NewConsumerSecret As String)
        If NewConsumerKey Is Nothing Then
            Throw New ArgumentNullException("NewConsumerKey")
        End If
        If NewConsumerSecret Is Nothing Then
            Throw New ArgumentNullException("NewConsumerSecret")
        End If
        _ConsumerKey = NewConsumerKey
        _ConsumerSecret = NewConsumerSecret
    End Sub

End Structure
