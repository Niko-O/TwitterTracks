Imports System.Windows.Controls

Namespace UI.Controls

    Public Class IfContent

        Public Property TrueContent As Object
            Get
                Return DirectCast(GetValue(TrueContentProperty), Object)
            End Get
            Set(value As Object)
                SetValue(TrueContentProperty, value)
            End Set
        End Property
        Public Shared ReadOnly TrueContentProperty As DependencyProperty = _
            DependencyProperty.Register("TrueContent", _
                                        GetType(Object), _
                                        GetType(IfContent), _
                                        New FrameworkPropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnTrueContentDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = False})
        Private Shared Sub OnTrueContentDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            DirectCast(d, IfContent).OnTrueContentPropertyChanged()
        End Sub
        Private Sub OnTrueContentPropertyChanged()
            TrueContentPresenter.Content = TrueContent
        End Sub

        Public Property FalseContent As Object
            Get
                Return DirectCast(GetValue(FalseContentProperty), Object)
            End Get
            Set(value As Object)
                SetValue(FalseContentProperty, value)
            End Set
        End Property
        Public Shared ReadOnly FalseContentProperty As DependencyProperty = _
            DependencyProperty.Register("FalseContent", _
                                        GetType(Object), _
                                        GetType(IfContent), _
                                        New FrameworkPropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnFalseContentDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = False})
        Private Shared Sub OnFalseContentDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            DirectCast(d, IfContent).OnFalseContentPropertyChanged()
        End Sub
        Private Sub OnFalseContentPropertyChanged()
            FalseContentPresenter.Content = FalseContent
        End Sub

        Public Property Condition As Boolean
            Get
                Return DirectCast(GetValue(ConditionProperty), Boolean)
            End Get
            Set(value As Boolean)
                SetValue(ConditionProperty, value)
            End Set
        End Property
        Public Shared ReadOnly ConditionProperty As DependencyProperty = _
            DependencyProperty.Register("Condition", _
                                        GetType(Boolean), _
                                        GetType(IfContent), _
                                        New FrameworkPropertyMetadata(False, New PropertyChangedCallback(AddressOf OnConditionDependencyPropertyChanged)) With {.DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, .BindsTwoWayByDefault = False})
        Private Shared Sub OnConditionDependencyPropertyChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            DirectCast(d, IfContent).OnConditionPropertyChanged()
        End Sub
        Private Sub OnConditionPropertyChanged()
            TrueContentPresenter.Visibility = Condition.ToVisibility
            FalseContentPresenter.Visibility = (Not Condition).ToVisibility
        End Sub

        Public Sub New()
            InitializeComponent()
            OnTrueContentPropertyChanged()
            OnFalseContentPropertyChanged()
            OnConditionPropertyChanged()
        End Sub

    End Class

End Namespace
