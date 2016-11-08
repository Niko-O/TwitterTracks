<System.Windows.Markup.ContentProperty("MainContent")>
Public Class BusyOverlay

    Public Property MainContent As Object
        Get
            Return DirectCast(GetValue(MainContentProperty), Object)
        End Get
        Set(value As Object)
            SetValue(MainContentProperty, value)
        End Set
    End Property
    Public Shared ReadOnly MainContentProperty As DependencyProperty = _
        DependencyProperty.Register("MainContent", _
                                    GetType(Object), _
                                    GetType(BusyOverlay), _
                                    New FrameworkPropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnMainContentDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = False})
    Private Shared Sub OnMainContentDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, BusyOverlay).OnMainContentPropertyChanged()
    End Sub
    Private Sub OnMainContentPropertyChanged()
        MainContentPresenter.Content = MainContent
    End Sub

    Public Property ShowOverlay As Boolean
        Get
            Return DirectCast(GetValue(ShowOverlayProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(ShowOverlayProperty, value)
        End Set
    End Property
    Public Shared ReadOnly ShowOverlayProperty As DependencyProperty = _
        DependencyProperty.Register("ShowOverlay", _
                                    GetType(Boolean), _
                                    GetType(BusyOverlay), _
                                    New FrameworkPropertyMetadata(False, New PropertyChangedCallback(AddressOf OnShowOverlayDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = False})
    Private Shared Sub OnShowOverlayDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, BusyOverlay).OnShowOverlayPropertyChanged()
    End Sub
    Private Sub OnShowOverlayPropertyChanged()
        Overlay.Visibility = If(ShowOverlay, Windows.Visibility.Visible, Windows.Visibility.Collapsed)
        IsEnabled = Not ShowOverlay
    End Sub

    Public Property OverlayText As String
        Get
            Return DirectCast(GetValue(OverlayTextProperty), String)
        End Get
        Set(value As String)
            SetValue(OverlayTextProperty, value)
        End Set
    End Property
    Public Shared ReadOnly OverlayTextProperty As DependencyProperty = _
        DependencyProperty.Register("OverlayText", _
                                    GetType(String), _
                                    GetType(BusyOverlay), _
                                    New FrameworkPropertyMetadata("Loading...", New PropertyChangedCallback(AddressOf OnOverlayTextDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = False})
    Private Shared Sub OnOverlayTextDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, BusyOverlay).OnOverlayTextPropertyChanged()
    End Sub
    Private Sub OnOverlayTextPropertyChanged()
        OverlayTextBlock.Text = OverlayText
    End Sub

    Public Sub New()
        InitializeComponent()
        OnMainContentPropertyChanged()
        OnShowOverlayPropertyChanged()
        OnOverlayTextPropertyChanged()
    End Sub

End Class
