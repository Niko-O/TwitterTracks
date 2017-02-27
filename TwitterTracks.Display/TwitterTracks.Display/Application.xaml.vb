Class Application

    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.

    Private Sub Application_Startup(sender As Object, e As System.Windows.StartupEventArgs) Handles Me.Startup
        GMap.NET.GMaps.Instance.CacheOnIdleRead = False
        GMap.NET.GMaps.Instance.BoostCacheEngine = True
    End Sub

    Private Sub Application_Exit(sender As Object, e As System.Windows.ExitEventArgs) Handles Me.Exit

    End Sub

End Class
