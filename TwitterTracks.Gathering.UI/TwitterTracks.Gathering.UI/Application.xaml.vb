Class Application

    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.

    Private Sub Application_Startup(sender As Object, e As System.Windows.StartupEventArgs) Handles Me.Startup
        Debug.Print("")
        With DateTime.Now
            Debug.Print("------------------------- New Instance  {0}:{1}:{2} ----------------------------", .Hour, .Minute, .Second)
        End With
    End Sub

End Class
