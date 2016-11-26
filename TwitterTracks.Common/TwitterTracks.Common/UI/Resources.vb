
Namespace UI

    Public Class Resources

        Public Shared ReadOnly ErrorBackgroundBrush As SolidColorBrush = DirectCast(TypeDescriptor.GetConverter(GetType(SolidColorBrush)).ConvertFromString("#ffaaaa"), SolidColorBrush)

#Const EnableDebugData = True
        Public Class DebugConstants
#If EnableDebugData Then

            Public Const StatusMessage As String = "This is a StatusMessage This is a StatusMessage This is a StatusMessage" & Microsoft.VisualBasic.ControlChars.CrLf & "This is a StatusMessage"

            Public Const DatabaseHost As String = "localhost"
            Public Const DatabaseUserName As String = "root" '"TestDatabase01_1_Researcher"
            Public Const DatabaseResearcherIdText As String = "1"
            Public Const DatabasePassword As String = ""

            Class Twitter
                Public Shared ReadOnly TwitterConsumerKey As String = ""
                Public Shared ReadOnly TwitterConsumerSecret As String = ""
                Public Shared ReadOnly TwitterAccessToken As String = ""
                Public Shared ReadOnly TwitterAccessTokenSecret As String = ""
                Shared Sub New()
                    'Case 1: When the code is executed in the XAML designer this is the absolute directory path of the solution directory of the project which calls this code (e.g. "C:\Blah\Solution").
                    'Case 2: But when the code is executed in the debugger or by running an exe directly, this is the working directory of the exe (e.g. "C:\Blah\Solution\Project\bin\Debug").
                    'The file to find is in the TwitterTracks.Common solution directory.
                    Dim CurrentDirectory As New System.IO.DirectoryInfo(Environment.CurrentDirectory)
                    If (CurrentDirectory.Name = "Debug" OrElse CurrentDirectory.Name = "Release") AndAlso CurrentDirectory.Parent.Name = "bin" Then
                        'Case 1.
                        CurrentDirectory = CurrentDirectory.Parent.Parent.Parent
                    End If
                    CurrentDirectory = CurrentDirectory.Parent
                    Dim FilePath = System.IO.Path.Combine(CurrentDirectory.FullName, "TwitterTracks.Common", "TwitterAuthToken.txt.DonutUpload")
                    If Not System.IO.File.Exists(FilePath) Then
                        MessageBox.Show("This is a problem. Look in TwitterTracks.Common.UI.Resources.DebugConstants.Twitter. The file containing the Twitter Authentication Token Data was not found. The file path is:" & Environment.NewLine & _
                                        FilePath & Environment.NewLine & _
                                        "Environment.CurrentDirectory is:" & Environment.NewLine & _
                                        Environment.CurrentDirectory)
                    End If
                    Dim Lines = System.IO.File.ReadAllLines(FilePath, System.Text.Encoding.UTF8)
                    'Lines(0) leaves room for a comment.
                    TwitterConsumerKey = Lines(1)
                    TwitterConsumerSecret = Lines(2)
                    TwitterAccessToken = Lines(3)
                    TwitterAccessTokenSecret = Lines(4)
                End Sub
            End Class



#Else

            Public Const StatusMessage As String = ""

            Public Const DatabaseHost As String = ""
            Public Const DatabaseUserName As String = ""
            Public Const DatabaseResearcherIdText As String = ""
            Public Const DatabasePassword As String = ""

            Class Twitter
                Public Const TwitterConsumerKey As String = ""
                Public Const TwitterConsumerSecret As String = ""
                Public Const TwitterAccessToken As String = ""
                Public Const TwitterAccessTokenSecret As String = ""
            End Class

#End If
        End Class

    End Class

End Namespace
