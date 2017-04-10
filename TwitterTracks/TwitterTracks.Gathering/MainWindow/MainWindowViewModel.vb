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

    <Dependency("OpenTrackInfo", "TrackingStreamIsRunning")>
    Public ReadOnly Property TrackingSignal As Boolean
        <DebuggerStepThrough()>
        Get
            If OpenTrackInfo Is Nothing Then
                Return True
            End If
            If TrackingStreamIsRunning Then
                Return True
            End If
            If Not String.IsNullOrEmpty(StreamDisconnectReason) Then
                Return False
            End If
            Return True
        End Get
    End Property

    <Dependency("OpenTrackInfo", "TrackingStreamIsRunning", "StreamDisconnectReason")>
    Public ReadOnly Property TrackingInfo As String
        Get
            If OpenTrackInfo Is Nothing Then
                Return "No Track Data loaded."
            End If
            If TrackingStreamIsRunning Then
                Return "Stream connected and tracking."
            End If
            If Not String.IsNullOrEmpty(StreamDisconnectReason) Then
                Return "Disconnected: " & StreamDisconnectReason
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

    Dim _OpenTrackInfo As OpenTrackInformation = Nothing
    Public Property OpenTrackInfo As OpenTrackInformation
        <DebuggerStepThrough()>
        Get
            Return _OpenTrackInfo
        End Get
        Set(NewValue As OpenTrackInformation)
            Dim OldValue = _OpenTrackInfo
            If OldValue IsNot NewValue Then
                If OldValue IsNot Nothing Then
                    RemoveHandler OldValue.MetadataChanged, AddressOf OpenTrackInfo_MetadataChanged
                End If
                _OpenTrackInfo = NewValue
                If NewValue IsNot Nothing Then
                    AddHandler NewValue.MetadataChanged, AddressOf OpenTrackInfo_MetadataChanged
                End If
                OnExtendedPropertyChanged("OpenTrackInfo", OldValue, NewValue)
            End If
        End Set
    End Property

    <Dependency("OpenTrackInfo")>
    Public ReadOnly Property CanPublish As Boolean
        Get
            Return OpenTrackInfo IsNot Nothing AndAlso Not OpenTrackInfo.Metadata.IsPublished
        End Get
    End Property

    <Dependency("OpenTrackInfo", "TrackingStreamIsRunning")>
    Public ReadOnly Property CanStartStream As Boolean
        Get
            Return OpenTrackInfo IsNot Nothing AndAlso OpenTrackInfo.Metadata.IsPublished AndAlso Not TrackingStreamIsRunning
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
            OpenTrackInfo = DebugConstants.OpenTrackInfo
        End If
    End Sub

    Private Sub OpenTrackInfo_MetadataChanged()
        OnPropertyChanged("CanPublish", "CanStartStream")
    End Sub

End Class
