Public Class Program

    Public Shared Sub Main(Args As String())
        TwitterTracks.TweetinviInterop.ServiceProvider.RegisterService(TweetinviService.Instance)
        TwitterTracks.Gathering.UI.Application.Main()
        'Dim App As New TwitterTracks.Gathering.UI.Application
        'App.StartupUri = New Uri("pack://application:,,,/TwitterTracks.Gathering.UI;component/MainWindow/MainWindow.xaml", UriKind.Absolute)
        'App.Run()
    End Sub

End Class