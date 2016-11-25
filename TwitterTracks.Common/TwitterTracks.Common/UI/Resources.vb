
Namespace UI

    Public Class Resources

        Public Shared ReadOnly ErrorBackgroundBrush As SolidColorBrush = DirectCast(TypeDescriptor.GetConverter(GetType(SolidColorBrush)).ConvertFromString("#ffaaaa"), SolidColorBrush)

#Const EnableDebugData = True
        Public Class DebugConstants
#If EnableDebugData Then
            Public Const StatusMessage As String = "This is a StatusMessage This is a StatusMessage This is a StatusMessage" & Microsoft.VisualBasic.ControlChars.CrLf & "This is a StatusMessage"
#Else
            Public Const StatusMessage As String = ""
#End If
        End Class

    End Class

End Namespace
