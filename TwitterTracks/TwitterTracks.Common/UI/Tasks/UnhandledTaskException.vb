
Namespace UI.Tasks

    Public Class UnhandledTaskException
        Inherits Exception

        Public Sub New(NewInnerException As Exception)
            MyBase.New("A running Task threw an Exception which was not handled by the task code and was therefore rethrown in the GUI thread. If you see this exception in a release build it means that something unexpected happened which was not handled by the developer. Please note as much information about the problem as possible (including this exception) and report it.", NewInnerException)
        End Sub

    End Class

End Namespace
