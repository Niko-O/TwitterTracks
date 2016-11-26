Class MainWindow 

    Dim ViewModel As MainWindowViewModel

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
    End Sub

    Private Sub AddMediaToAdd(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Dlg As New Microsoft.Win32.OpenFileDialog With {.CheckFileExists = True, .CheckPathExists = True, .Multiselect = True, .RestoreDirectory = True, .Title = "Select a file to attach to the Tweet"}
        If Dlg.ShowDialog Then
            For Each i In Dlg.FileNames
                ViewModel.TweetDataVM.MediasToAdd.Add(New TweetMediaToAdd(i))
            Next
        End If
    End Sub

End Class
