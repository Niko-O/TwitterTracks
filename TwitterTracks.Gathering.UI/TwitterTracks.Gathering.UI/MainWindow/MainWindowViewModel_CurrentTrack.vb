Public Class MainWindowViewModel_CurrentTrack
    Inherits ViewModelBase

    Dim _DatabaseInfo As String = "Buup"
    Public Property DatabaseInfo As String
        <DebuggerStepThrough()>
        Get
            Return _DatabaseInfo
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_DatabaseInfo, value, "DatabaseInfo")
        End Set
    End Property

    Dim _DatabaseInfoForeground As Brush = Brushes.Green
    Public Property DatabaseInfoForeground As Brush
        <DebuggerStepThrough()>
        Get
            Return _DatabaseInfoForeground
        End Get
        Set(value As Brush)
            ExtendedChangeIfDifferent(_DatabaseInfoForeground, value, "DatabaseInfoForeground")
        End Set
    End Property

    Dim _TwitterInfo As String = "Buup"
    Public Property TwitterInfo As String
        <DebuggerStepThrough()>
        Get
            Return _TwitterInfo
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_TwitterInfo, value, "TwitterInfo")
        End Set
    End Property

    Dim _TwitterInfoForeground As Brush = Brushes.Green
    Public Property TwitterInfoForeground As Brush
        <DebuggerStepThrough()>
        Get
            Return _TwitterInfoForeground
        End Get
        Set(value As Brush)
            ExtendedChangeIfDifferent(_TwitterInfoForeground, value, "TwitterInfoForeground")
        End Set
    End Property

    Dim _TweetId As String = "123123123123"
    Public Property TweetId As String
        <DebuggerStepThrough()>
        Get
            Return _TweetId
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_TweetId, value, "TweetId")
        End Set
    End Property

    Dim _TweetIdForeground As Brush = Brushes.Green
    Public Property TweetIdForeground As Brush
        <DebuggerStepThrough()>
        Get
            Return _TweetIdForeground
        End Get
        Set(value As Brush)
            ExtendedChangeIfDifferent(_TweetIdForeground, value, "TweetIdForeground")
        End Set
    End Property

    Dim _IsPublished As Boolean = False
    Public Property IsPublished As Boolean
        <DebuggerStepThrough()>
        Get
            Return _IsPublished
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_IsPublished, value, "IsPublished")
        End Set
    End Property

End Class
