Public Class MainWindowViewModel
    Inherits ViewModelBase

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

    <Dependency("OpenTweetInfo", "OpenTweetInfo")>
    Public ReadOnly Property CanPublish As Boolean
        Get
            Return OpenTweetInfo IsNot Nothing AndAlso OpenTweetInfo IsNot Nothing AndAlso Not OpenTweetInfo.IsPublished
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

    Dim _PublishStatusMessageVM As new TwitterTracks.Common.UI.StatusMessageViewModel
    Public ReadOnly Property PublishStatusMessageVM As TwitterTracks.Common.UI.StatusMessageViewModel
        <DebuggerStepThrough()>
        Get
            Return _PublishStatusMessageVM
        End Get
    End Property

    Dim _LoadConfigurationStatusMessageVM As New TwitterTracks.Common.UI.StatusMessageViewModel
    Public ReadOnly Property LoadConfigurationStatusMessageVM As TwitterTracks.Common.UI.StatusMessageViewModel
        <DebuggerStepThrough()>
        Get
            Return _LoadConfigurationStatusMessageVM
        End Get
    End Property

    Public Sub New()
        MyBase.New(True)
        If IsInDesignMode Then
            OpenTweetInfo = DebugConstants.OpenTweetInfo
        End If
    End Sub

    Private Sub OpenTweetInfo_IsPublishedChanged()
        OnPropertyChanged("CanPublish")
    End Sub

End Class
