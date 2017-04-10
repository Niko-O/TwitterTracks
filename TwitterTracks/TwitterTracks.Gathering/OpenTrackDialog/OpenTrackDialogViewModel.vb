
Namespace OpenTrackDialog

    Public Class OpenTrackDialogViewModel
        Inherits ViewModelBase

        Dim WithEvents _DatabaseConnectionVM As New TwitterTracks.Common.UI.Controls.TrackSelectionInputViewModel(True, False)
        Public ReadOnly Property DatabaseConnectionVM As TwitterTracks.Common.UI.Controls.TrackSelectionInputViewModel
            <DebuggerStepThrough()>
            Get
                Return _DatabaseConnectionVM
            End Get
        End Property

        Dim WithEvents _TwitterConnectionVM As New OpenTrackDialogViewModel_TwitterConnection
        Public ReadOnly Property TwitterConnectionVM As OpenTrackDialogViewModel_TwitterConnection
            <DebuggerStepThrough()>
            Get
                Return _TwitterConnectionVM
            End Get
        End Property

        Dim WithEvents _TweetDataVM As New OpenTrackDialogViewModel_TweetData
        Public ReadOnly Property TweetDataVM As OpenTrackDialogViewModel_TweetData
            <DebuggerStepThrough()>
            Get
                Return _TweetDataVM
            End Get
        End Property

        Dim WithEvents _KeywordsVM As New OpenTrackDialogViewModel_Keywords
        Public ReadOnly Property KeywordsVM As OpenTrackDialogViewModel_Keywords
            <DebuggerStepThrough()>
            Get
                Return _KeywordsVM
            End Get
        End Property

        Dim WithEvents _SummaryVM As New OpenTrackDialogViewModel_Summary
        Public ReadOnly Property SummaryVM As OpenTrackDialogViewModel_Summary
            <DebuggerStepThrough()>
            Get
                Return _SummaryVM
            End Get
        End Property

        Public ReadOnly Property MainTabControlItemStyle As Style
            <DebuggerStepThrough()>
            Get
                If IsInDesignMode Then
                    Return Nothing
                End If
                Static Temp As Style = New Style().ButAlso(Sub(o)
                                                               o.Setters.Add(New Setter With {.Property = FrameworkElement.VisibilityProperty, .Value = Visibility.Collapsed})
                                                           End Sub)
                Return Temp
            End Get
        End Property

        Dim _CurrentTabIndex As DialogTabIndex = DialogTabIndex.Initial
        Public Property CurrentTabIndex As DialogTabIndex
            <DebuggerStepThrough()>
            Get
                Return _CurrentTabIndex
            End Get
            Set(value As DialogTabIndex)
                ExtendedChangeIfDifferent(_CurrentTabIndex, value, "CurrentTabIndex")
            End Set
        End Property

        <Dependency("CurrentTabIndex")>
        Public ReadOnly Property CanGoBack As Boolean
            Get
                Return CurrentTabIndex > DialogTabIndex.Initial
            End Get
        End Property

        <Dependency("CurrentTabIndex")>
        Public ReadOnly Property CanGoForward As Boolean
            Get
                Select Case CurrentTabIndex
                    Case DialogTabIndex.DatabaseConnection
                        Return DatabaseConnectionVM.IsValid
                    Case DialogTabIndex.TwitterConnection
                        Return TwitterConnectionVM.IsValid
                    Case DialogTabIndex.TweetData
                        Return TweetDataVM.IsValid
                    Case DialogTabIndex.Keywords
                        Return KeywordsVM.IsValid
                    Case DialogTabIndex.Summary
                        Return True
                    Case Else
                        Throw New NopeException
                End Select
            End Get
        End Property

        <Dependency("CurrentTabIndex")>
        Public ReadOnly Property ForwardButtonText As String
            Get
                If CurrentTabIndex = DialogTabIndex.Summary Then
                    Return "OK"
                Else
                    Return "Weiter >"
                End If
            End Get
        End Property

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

        Private Sub ChildVM_PropertyChanged(sender As Object, e As System.ComponentModel.PropertyChangedEventArgs) Handles _DatabaseConnectionVM.PropertyChanged, _TwitterConnectionVM.PropertyChanged, _TweetDataVM.PropertyChanged, _KeywordsVM.PropertyChanged, _SummaryVM.PropertyChanged
            Select Case e.PropertyName
                Case "IsValid"
                    OnPropertyChanged("CanGoForward")
            End Select
        End Sub

    End Class

End Namespace
