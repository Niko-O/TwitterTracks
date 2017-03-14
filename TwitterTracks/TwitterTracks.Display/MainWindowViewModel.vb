Public Class MainWindowViewModel
    Inherits ViewModelBase

    Dim _Zoom As Double = 1.4
    Public Property Zoom As Double
        <DebuggerStepThrough()>
        Get
            Return _Zoom
        End Get
        Set(value As Double)
            ExtendedChangeIfDifferent(_Zoom, value, "Zoom")
        End Set
    End Property

    Dim _TrackIsLoaded As Boolean = False
    Public Property TrackIsLoaded As Boolean
        <DebuggerStepThrough()>
        Get
            Return _TrackIsLoaded
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_TrackIsLoaded, value, "TrackIsLoaded")
        End Set
    End Property

    Dim _TotalNumberOfTweets As Integer = 0
    Public Property TotalNumberOfTweets As Integer
        <DebuggerStepThrough()>
        Get
            Return _TotalNumberOfTweets
        End Get
        Set(value As Integer)
            ExtendedChangeIfDifferent(_TotalNumberOfTweets, value, "TotalNumberOfTweets")
        End Set
    End Property

    Dim _NumberOfTweetsWithCoordinates As Integer = 0
    Public Property NumberOfTweetsWithCoordinates As Integer
        <DebuggerStepThrough()>
        Get
            Return _NumberOfTweetsWithCoordinates
        End Get
        Set(value As Integer)
            ExtendedChangeIfDifferent(_NumberOfTweetsWithCoordinates, value, "NumberOfTweetsWithCoordinates")
        End Set
    End Property

    Public Sub New()
        MyBase.New(True)
    End Sub

End Class
