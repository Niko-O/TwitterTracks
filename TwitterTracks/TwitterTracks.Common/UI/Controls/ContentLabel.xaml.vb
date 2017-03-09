
Namespace UI.Controls

    <System.Windows.Markup.ContentProperty("MainContent")>
    Public Class ContentLabel

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
                                        GetType(ContentLabel), _
                                        New FrameworkPropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnMainContentDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = False})
        Private Shared Sub OnMainContentDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            DirectCast(d, ContentLabel).OnMainContentPropertyChanged()
        End Sub
        Private Sub OnMainContentPropertyChanged()
            MainContentPresenter.Content = MainContent
        End Sub

        Public Property LabelContent As Object
            Get
                Return DirectCast(GetValue(LabelContentProperty), Object)
            End Get
            Set(value As Object)
                SetValue(LabelContentProperty, value)
            End Set
        End Property
        Public Shared ReadOnly LabelContentProperty As DependencyProperty = _
            DependencyProperty.Register("LabelContent", _
                                        GetType(Object), _
                                        GetType(ContentLabel), _
                                        New FrameworkPropertyMetadata("Label:", New PropertyChangedCallback(AddressOf OnLabelContentDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = False})
        Private Shared Sub OnLabelContentDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            DirectCast(d, ContentLabel).OnLabelContentPropertyChanged()
        End Sub
        Private Sub OnLabelContentPropertyChanged()
            LabelContentPresenter.Content = LabelContent
        End Sub

        Public Sub New()
            InitializeComponent()
            OnMainContentPropertyChanged()
            OnLabelContentPropertyChanged()
        End Sub

    End Class

End Namespace
