Public Class MainWindowViewModel
    Inherits ViewModelBase

    Dim _Zoom As Double = 1.4
    Public Property Zoom As Double
        <DebuggerStepThrough()>
        Get
            Return _Zoom
        End Get
        Set(value As Double)
            ExtendedChangeIfDifferent(_Zoom, value, "Zoom")
        End Set
    End Property

    Public Sub New()
        MyBase.New(True)
    End Sub

End Class
