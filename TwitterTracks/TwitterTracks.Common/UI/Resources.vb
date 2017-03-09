
Namespace UI

    Public Class Resources

        Public Shared ReadOnly ErrorBackgroundBrush As SolidColorBrush = DirectCast(TypeDescriptor.GetConverter(GetType(SolidColorBrush)).ConvertFromString("#ffaaaa"), SolidColorBrush)
        Public Shared ReadOnly WarningBackgroundBrush As SolidColorBrush = DirectCast(TypeDescriptor.GetConverter(GetType(SolidColorBrush)).ConvertFromString("#ffccaa"), SolidColorBrush)
        Public Shared ReadOnly SuccessBackgroundBrush As SolidColorBrush = DirectCast(TypeDescriptor.GetConverter(GetType(SolidColorBrush)).ConvertFromString("#aaffaa"), SolidColorBrush)

#Const EnableDebugData = True

        Public Class DebugConstants
#If EnableDebugData Then

            Public Const StatusMessage As String = "" '"This is a StatusMessage" & Microsoft.VisualBasic.ControlChars.CrLf & "This is a StatusMessage"

            Public Const DatabaseHost As String = "localhost"
            Public Const DatabaseUserName As String = "root" '"TestDatabase01_1_Researcher"
            Public Const DatabaseResearcherIdText As String = "1"
            Public Const DatabaseResearcherId As Int64 = 1
            Public Const DatabasePassword As String = ""
            Public Const TrackDatabaseName As String = "BobsDatabase"
            Public Const ResearcherPassword As String = "Password1"

            Public Const TweetText As String = "This is a Test #TestIt #MoreTests"

#Else

            Public Const StatusMessage As String = ""

            Public Const DatabaseHost As String = ""
            Public Const DatabaseUserName As String = ""
            Public Const DatabaseResearcherIdText As String = ""
            Public Const DatabaseResearcherId As Int64 = 0
            Public Const DatabasePassword As String = ""
            Public Const TrackDatabaseName As String = ""
            Public Const ResearcherPassword As String = ""

            Public Const TweetText As String = ""

#End If
        End Class

    End Class

End Namespace
