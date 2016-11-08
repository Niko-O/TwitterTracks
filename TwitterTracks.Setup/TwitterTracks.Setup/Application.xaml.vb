Class Application

    Public Shared ReadOnly ErrorBackgroundBrush As SolidColorBrush = DirectCast(TypeDescriptor.GetConverter(GetType(SolidColorBrush)).ConvertFromString("#ffaaaa"), SolidColorBrush)

    ' Ereignisse auf Anwendungsebene wie Startup, Exit und DispatcherUnhandledException
    ' können in dieser Datei verarbeitet werden.

End Class
