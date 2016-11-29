Class Application

    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.

    Private Sub Application_Startup(sender As Object, e As System.Windows.StartupEventArgs) Handles Me.Startup
        Debug.Print("")
        With DateTime.Now
            Helpers.DebugPrint("------------------------- New Instance  {0}:{1}:{2};   GUI Thread ID: {3} ----------------------------", .Hour, .Minute, .Second, System.Threading.Thread.CurrentThread.ManagedThreadId)
        End With
    End Sub

End Class
