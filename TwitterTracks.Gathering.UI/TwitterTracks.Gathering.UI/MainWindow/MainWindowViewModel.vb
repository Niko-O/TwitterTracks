Public Class MainWindowViewModel
    Inherits ViewModelBase

    Dim _IsBusy As Boolean = False
    Public Property IsBusy As Boolean
        <DebuggerStepThrough()>
        Get
            Return _IsBusy
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_IsBusy, value, "IsBusy")
        End Set
    End Property

    <Dependency("OpenTweetInfo")>
    Public ReadOnly Property TrackDataSignal As SignalKind
        <DebuggerStepThrough()>
        Get
            If OpenTweetInfo Is Nothing Then
                Return SignalKind.Error
            End If
            Return SignalKind.OK
        End Get
    End Property

    <Dependency("OpenTweetInfo", "TrackingStreamIsRunning")>
    Public ReadOnly Property TrackingSignal As SignalKind
        <DebuggerStepThrough()>
        Get
            If OpenTweetInfo Is Nothing Then
                Return SignalKind.Error
            End If
            If Not TrackingStreamIsRunning Then
                Return SignalKind.Error
            End If
            Return SignalKind.OK
        End Get
    End Property

    <Dependency("OpenTweetInfo", "TrackingStreamIsRunning", "StreamDisconnectReason")>
    Public ReadOnly Property TrackingInfo As String
        Get
            If OpenTweetInfo Is Nothing Then
                Return "This should never be visible."
            End If
            If TrackingStreamIsRunning Then
                Return "Stream connected and tracking."
            End If
            If Not String.IsNullOrEmpty(StreamDisconnectReason) Then
                Return StreamDisconnectReason
            End If
            Return "Waiting for start."
        End Get
    End Property

    Dim _StreamDisconnectReason As String = Nothing
    Public Property StreamDisconnectReason As String
        <DebuggerStepThrough()>
        Get
            Return _StreamDisconnectReason
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_StreamDisconnectReason, value, "StreamDisconnectReason")
        End Set
    End Property

    Dim _TrackingStreamIsRunning As Boolean = False
    Public Property TrackingStreamIsRunning As Boolean
        <DebuggerStepThrough()>
        Get
            Return _TrackingStreamIsRunning
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_TrackingStreamIsRunning, value, "TrackingStreamIsRunning")
        End Set
    End Property

    Dim _OpenTweetInfo As OpenTweetInformation = Nothing
    Public Property OpenTweetInfo As OpenTweetInformation
        <DebuggerStepThrough()>
        Get
            Return _OpenTweetInfo
        End Get
        Set(NewValue As OpenTweetInformation)
            Dim OldValue = _OpenTweetInfo
            If OldValue IsNot NewValue Then
                If OldValue IsNot Nothing Then
                    RemoveHandler OldValue.IsPublishedChanged, AddressOf OpenTweetInfo_IsPublishedChanged
                End If
                _OpenTweetInfo = NewValue
                If NewValue IsNot Nothing Then
                    AddHandler NewValue.IsPublishedChanged, AddressOf OpenTweetInfo_IsPublishedChanged
                End If
                OnExtendedPropertyChanged("OpenTweetInfo", OldValue, NewValue)
            End If
        End Set
    End Property

    <Dependency("OpenTweetInfo")>
    Public ReadOnly Property CanPublish As Boolean
        Get
            Return OpenTweetInfo IsNot Nothing AndAlso Not OpenTweetInfo.IsPublished
        End Get
    End Property

    <Dependency("OpenTweetInfo", "TrackingStreamIsRunning")>
    Public ReadOnly Property CanStartStream As Boolean
        Get
            Return OpenTweetInfo IsNot Nothing AndAlso OpenTweetInfo.IsPublished AndAlso Not TrackingStreamIsRunning
        End Get
    End Property

    Dim _NumberOfTrackedTweets As Int64 = 0
    Public Property NumberOfTrackedTweets As Int64
        <DebuggerStepThrough()>
        Get
            Return _NumberOfTrackedTweets
        End Get
        Set(value As Int64)
            ExtendedChangeIfDifferent(_NumberOfTrackedTweets, value, "NumberOfTrackedTweets")
        End Set
    End Property

    Dim _StatusMessageVM As New TwitterTracks.Common.UI.StatusMessageViewModel
    Public ReadOnly Property StatusMessageVM As TwitterTracks.Common.UI.StatusMessageViewModel
        <DebuggerStepThrough()>
        Get
            Return _StatusMessageVM
        End Get
    End Property

    Public Sub New()
        MyBase.New(True)
        If IsInDesignMode Then
            OpenTweetInfo = DebugConstants.OpenTweetInfo
        End If
    End Sub

    Private Sub OpenTweetInfo_IsPublishedChanged()
        OnPropertyChanged("CanPublish", "CanStartStream")
    End Sub

End Class
