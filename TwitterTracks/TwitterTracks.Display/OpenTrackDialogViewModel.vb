Public Class OpenTrackDialogViewModel
    Inherits ViewModelBase

    Dim WithEvents _DatabaseConnectionVM As New TwitterTracks.Common.UI.Controls.TrackSelectionInputViewModel(True, False)
    Public ReadOnly Property DatabaseConnectionVM As TwitterTracks.Common.UI.Controls.TrackSelectionInputViewModel
        <DebuggerStepThrough()>
        Get
            Return _DatabaseConnectionVM
        End Get
    End Property

    Dim _IsBusy As Boolean = DebugConstants.OpenTrackDialogIsBusy
    Public Property IsBusy As Boolean
        <DebuggerStepThrough()>
        Get
            Return _IsBusy
        End Get
        Set(value As Boolean)
            ExtendedChangeIfDifferent(_IsBusy, value, "IsBusy")
        End Set
    End Property

    Public ReadOnly Property CanCloseOk As Boolean
        Get
            Return Not IsBusy AndAlso DatabaseConnectionVM.IsValid
        End Get
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
    End Sub

    Private Sub DatabaseConnectionVM_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _DatabaseConnectionVM.PropertyChanged
        Select Case e.PropertyName
            Case "IsValid"
                OnPropertyChanged("CanCloseOk")
        End Select
    End Sub

End Class