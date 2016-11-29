Class MainWindow 

    Dim ViewModel As MainWindowViewModel

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
    End Sub

    Protected Overrides Sub OnContentRendered(e As System.EventArgs)
        MyBase.OnContentRendered(e)
        Dim Dlg As New OpenTrackDialog.OpenTrackDialog
        Dlg.ShowDialog()
        Me.Close()
    End Sub

    Private Sub TestStuff(sender As System.Object, e As System.Windows.RoutedEventArgs)
        
    End Sub

End Class
