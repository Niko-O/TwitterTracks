Public Class DebugConstants

#Const EnableDebugData = False

#If EnableDebugData Then

    Public Shared ReadOnly OpenTrackInfo As OpenTrackInformation 
    Public Shared ReadOnly AccessToken As Tweetinvi.Models.TwitterCredentials
    Shared Sub New()
        'Case 1: When the code is executed in the XAML designer this is the absolute directory path of the solution directory of the project which calls this code (e.g. "C:\Blah\Solution").
        'Case 2: But when the code is executed in the debugger or by running an exe directly, this is the working directory of the exe (e.g. "C:\Blah\Solution\Project\bin\Debug").
        'The file to find is in the TwitterTracks.Common solution directory.
        Dim CurrentDirectory As New System.IO.DirectoryInfo(Environment.CurrentDirectory)
        If (CurrentDirectory.Name = "Debug" OrElse CurrentDirectory.Name = "Release") AndAlso CurrentDirectory.Parent.Name = "bin" Then
            'Case 2.
            CurrentDirectory = CurrentDirectory.Parent.Parent.Parent
        End If
        Dim FilePath = System.IO.Path.Combine(CurrentDirectory.FullName, "TwitterAuthToken.txt.DonutUpload")
        If Not System.IO.File.Exists(FilePath) Then
            MessageBox.Show("This is a problem. Look in TwitterTracks.Common.UI.Resources.DebugConstants.Twitter. The file containing the Twitter Authentication Token Data was not found. The file path is:" & Environment.NewLine & _
                            FilePath & Environment.NewLine & _
                            "Environment.CurrentDirectory is:" & Environment.NewLine & _
                            Environment.CurrentDirectory)
        End If
        Dim Lines = System.IO.File.ReadAllLines(FilePath, System.Text.Encoding.UTF8)
        'Lines(0) leaves room for a comment.
        AccessToken = New Tweetinvi.Models.TwitterCredentials(Lines(1), Lines(2), Lines(3), Lines(4))

        OpenTrackInfo = New OpenTrackInformation()
        OpenTrackInfo.Database.Host = TwitterTracks.Common.UI.Resources.DebugConstants.DatabaseHost
        OpenTrackInfo.Database.Name = TwitterTracks.Common.UI.Resources.DebugConstants.TrackDatabaseName
        OpenTrackInfo.Database.ResearcherId = TwitterTracks.Common.UI.Resources.DebugConstants.DatabaseResearcherId
        OpenTrackInfo.Database.Password = TwitterTracks.Common.UI.Resources.DebugConstants.ResearcherPassword
        OpenTrackInfo.Database.Connection = Nothing
        OpenTrackInfo.Metadata = New TwitterTracks.DatabaseAccess.TrackMetadata( _
            True, 10, 20, TwitterTracks.Common.UI.Resources.DebugConstants.TweetText, _
            {"#Test", "#Hashtag", "Quantenelektrodynamik"}, {"C:\Foo.jpg", "C:\Bar.png"}, _
            AccessToken.AccessToken, AccessToken.AccessTokenSecret)
    End Sub

#Else

    Public Shared ReadOnly OpenTrackInfo As OpenTrackInformation = Nothing
    Public Shared ReadOnly AccessToken As New Tweetinvi.Models.TwitterCredentials("", "", "", "")

#End If

End Class
