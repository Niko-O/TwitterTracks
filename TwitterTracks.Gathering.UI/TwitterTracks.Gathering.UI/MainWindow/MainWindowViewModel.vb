Public Class MainWindowViewModel
    Inherits ViewModelBase

    Dim _ConnectionVM As New MainWindowViewModel_Connection
    Public ReadOnly Property ConnectionVM As MainWindowViewModel_Connection
        <DebuggerStepThrough()>
        Get
            Return _ConnectionVM
        End Get
    End Property

    Dim _TweetDataVM As New MainWindowViewModel_TweetData
    Public ReadOnly Property TweetDataVM As MainWindowViewModel_TweetData
        <DebuggerStepThrough()>
        Get
            Return _TweetDataVM
        End Get
    End Property

    Public Sub New()
        MyBase.New(True)
    End Sub

End Class
