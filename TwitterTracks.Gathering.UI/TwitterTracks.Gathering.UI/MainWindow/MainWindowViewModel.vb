Public Class MainWindowViewModel
    Inherits ViewModelBase

    Dim _CurrentTrackVM As New MainWindowViewModel_CurrentTrack
    Public ReadOnly Property CurrentTrackVM As MainWindowViewModel_CurrentTrack
        <DebuggerStepThrough()>
        Get
            Return _CurrentTrackVM
        End Get
    End Property

    Dim _OpenTweetInfo As OpenTweetInformation = DebugConstants.OpenTweetInfo
    Public Property OpenTweetInfo As OpenTweetInformation
        <DebuggerStepThrough()>
        Get
            Return _OpenTweetInfo
        End Get
        Set(value As OpenTweetInformation)
            ExtendedChangeIfDifferent(_OpenTweetInfo, value, "OpenTweetInfo")
        End Set
    End Property

    Public Sub New()
        MyBase.New(True)
    End Sub

End Class
