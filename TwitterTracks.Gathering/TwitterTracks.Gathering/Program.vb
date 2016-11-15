Public Class Program

    Public Shared Sub Main(Args As String())

        Dim App As New TwitterTracks.Gathering.UI.Application
        App.StartupUri = New Uri("pack://application:,,,/TwitterTracks.Gathering.UI;component/MainWindow.xaml", UriKind.Absolute)
        App.Run()
    End Sub

End Class