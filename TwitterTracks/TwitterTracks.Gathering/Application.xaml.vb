Class Application

    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.

    Private Sub Application_Startup(sender As Object, e As System.Windows.StartupEventArgs) Handles Me.Startup
        Helpers.DebugPrint("")
        With DateTime.Now
            Helpers.DebugPrint("------------------------- New Instance  {0}:{1}:{2};   GUI Thread ID: {3} ----------------------------", .Hour, .Minute, .Second, System.Threading.Thread.CurrentThread.ManagedThreadId)
        End With
        TwitterTracks.Gathering.TweetinviInterop.ServiceProvider.RegisterService(TweetinviService.Instance)
        'If the service is already set the application is run from the VS 2015 project. So don't use the DebugService then.
        'If TwitterTracks.TweetinviInterop.ServiceProvider.Service Is Nothing Then
        '    TwitterTracks.TweetinviInterop.ServiceProvider.RegisterService(New DebugTweetinviInterop.DebugService)
        'End If
        TwitterTracks.DatabaseAccess.DatabaseBase.DebugPrintQueries = False
    End Sub

    Public Shared Sub DebugPrint(Text As String, ParamArray Args As Object())
        With DateTime.Now
            Helpers.DebugPrint("[{0}] @ {1}:{2}:{3}.{4}: {5}",
                               System.Threading.Thread.CurrentThread.ManagedThreadId,
                               .Hour, .Minute, .Second, .Millisecond,
                               String.Format(Text, Args))
        End With
    End Sub

End Class
