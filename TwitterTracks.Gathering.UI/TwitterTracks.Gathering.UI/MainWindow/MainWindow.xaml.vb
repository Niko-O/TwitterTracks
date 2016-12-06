Class MainWindow 

    Dim ViewModel As MainWindowViewModel

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
    End Sub

    Private Sub OpenOrCreateTrack(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Dlg As New OpenTrackDialog.OpenTrackDialog
        If Dlg.ShowDialog() Then
            ViewModel.OpenTweetInfo = Dlg.GetOpenTweetInfo
        End If
    End Sub

    Private Sub PublishTweet(sender As System.Object, e As System.Windows.RoutedEventArgs)

    End Sub

End Class
