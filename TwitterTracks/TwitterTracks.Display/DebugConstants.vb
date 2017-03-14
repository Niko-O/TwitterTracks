
#Const UseDebugValues = (Config = "Debug")

Public Class DebugConstants

#If UseDebugValues Then
    Public Const OpenTrackDialogIsBusy As Boolean = False
    Public Const ShowContinueAnywaysButton As Boolean = False
#Else
    Public Const OpenTrackDialogIsBusy As Boolean = False
    Public Const ShowContinueAnywaysButton As Boolean = False
#End If

End Class
