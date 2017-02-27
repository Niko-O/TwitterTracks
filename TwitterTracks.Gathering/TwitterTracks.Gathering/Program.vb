Public Class Program

    Public Shared Sub DebugPrint(Text As String, ParamArray Args As Object())
        With DateTime.Now
            Helpers.DebugPrint("[{0}] @ {1}:{2}:{3}.{4}: {5}",
                               System.Threading.Thread.CurrentThread.ManagedThreadId,
                               .Hour, .Minute, .Second, .Millisecond,
                               String.Format(Text, Args))
        End With
    End Sub

    Public Shared Sub Main(Args As String())
        TwitterTracks.TweetinviInterop.ServiceProvider.RegisterService(TweetinviService.Instance)
        TwitterTracks.Gathering.UI.Application.Main()
        'Dim App As New TwitterTracks.Gathering.UI.Application
        'App.StartupUri = New Uri("pack://application:,,,/TwitterTracks.Gathering.UI;component/MainWindow/MainWindow.xaml", UriKind.Absolute)
        'App.Run()
    End Sub

End Class