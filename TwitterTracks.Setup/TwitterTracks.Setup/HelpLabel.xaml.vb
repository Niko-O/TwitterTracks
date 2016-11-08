
<System.Windows.Markup.ContentProperty("MainContent")>
Public Class HelpLabel

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
                                    GetType(HelpLabel), _
                                    New FrameworkPropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnMainContentDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = True})
    Private Shared Sub OnMainContentDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, HelpLabel).OnMainContentPropertyChanged()
    End Sub
    Private Sub OnMainContentPropertyChanged()
        MainContentPresenter.Content = MainContent
    End Sub

    Public Property ButtonContent As Object
        Get
            Return DirectCast(GetValue(ButtonContentProperty), Object)
        End Get
        Set(value As Object)
            SetValue(ButtonContentProperty, value)
        End Set
    End Property
    Public Shared ReadOnly ButtonContentProperty As DependencyProperty = _
        DependencyProperty.Register("ButtonContent", _
                                    GetType(Object), _
                                    GetType(HelpLabel), _
                                    New FrameworkPropertyMetadata("?", New PropertyChangedCallback(AddressOf OnButtonContentDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = True})
    Private Shared Sub OnButtonContentDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, HelpLabel).OnButtonContentPropertyChanged()
    End Sub
    Private Sub OnButtonContentPropertyChanged()
        ToolTipButton.Content = ButtonContent
    End Sub

    Public Property HelpContent As Object
        Get
            Return DirectCast(GetValue(HelpContentProperty), Object)
        End Get
        Set(value As Object)
            SetValue(HelpContentProperty, value)
        End Set
    End Property
    Public Shared ReadOnly HelpContentProperty As DependencyProperty = _
        DependencyProperty.Register("HelpContent", _
                                    GetType(Object), _
                                    GetType(HelpLabel), _
                                    New FrameworkPropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnHelpContentDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = True})
    Private Shared Sub OnHelpContentDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, HelpLabel).OnHelpContentPropertyChanged()
    End Sub
    Private Sub OnHelpContentPropertyChanged()
        HelpToolTip.Content = HelpContent
    End Sub

    Public Property HelpIsVisible As Boolean
        Get
            Return DirectCast(GetValue(HelpIsVisibleProperty), Boolean)
        End Get
        Set(value As Boolean)
            SetValue(HelpIsVisibleProperty, value)
        End Set
    End Property
    Public Shared ReadOnly HelpIsVisibleProperty As DependencyProperty = _
        DependencyProperty.Register("HelpIsVisible", _
                                    GetType(Boolean), _
                                    GetType(HelpLabel), _
                                    New FrameworkPropertyMetadata(True, New PropertyChangedCallback(AddressOf OnHelpIsVisibleDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = False})
    Private Shared Sub OnHelpIsVisibleDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, HelpLabel).OnHelpIsVisiblePropertyChanged()
    End Sub
    Private Sub OnHelpIsVisiblePropertyChanged()
        ToolTipButton.Visibility = If(HelpIsVisible, Windows.Visibility.Visible, Windows.Visibility.Collapsed)
    End Sub

    Public Sub New()
        InitializeComponent()
        OnMainContentPropertyChanged()
        OnButtonContentPropertyChanged()
        OnHelpContentPropertyChanged()
        OnHelpIsVisiblePropertyChanged()
    End Sub

End Class
