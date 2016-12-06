Public Class DebugConstants

#Const EnableDebugData = True

#If EnableDebugData Then
    Public Shared ReadOnly OpenTweetInfo As OpenTweetInformation = New OpenTweetInformation() _
        .ButAlso(Sub(o As OpenTweetInformation)
                     o.IsPublished = True

                     o.Database.Host = TwitterTracks.Common.UI.Resources.DebugConstants.DatabaseHost
                     o.Database.Name = TwitterTracks.Common.UI.Resources.DebugConstants.TrackDatabaseName
                     o.Database.ResearcherId = TwitterTracks.Common.UI.Resources.DebugConstants.DatabaseResearcherId
                     o.Database.Password = TwitterTracks.Common.UI.Resources.DebugConstants.ResearcherPassword
                     o.Database.Connection = Nothing

                     o.TweetData.Metadata = New TwitterTracks.DatabaseAccess.TrackMetadata(10, 20, TwitterTracks.Common.UI.Resources.DebugConstants.TweetText, {"#Test", "#Hashtag"})
                     o.TweetData.TweetText = TwitterTracks.Common.UI.Resources.DebugConstants.TweetText
                     o.TweetData.MediasToAdd = New List(Of String) From {"C:\Foo.jpg", "C:\Bar.png"}
                     o.TweetData.Keywords = New List(Of String) From {"#Test", "#Hashtag", "Quantenelektrodynamik"}

                     o.TwitterConnection.ConsumerKey = TwitterTracks.Common.UI.Resources.DebugConstants.Twitter.AccessToken.ConsumerKey
                     o.TwitterConnection.ConsumerSecret = TwitterTracks.Common.UI.Resources.DebugConstants.Twitter.AccessToken.ConsumerSecret
                     o.TwitterConnection.AccessToken = TwitterTracks.Common.UI.Resources.DebugConstants.Twitter.AccessToken.AccessToken
                     o.TwitterConnection.AccessTokenSecret = TwitterTracks.Common.UI.Resources.DebugConstants.Twitter.AccessToken.AccessTokenSecret
                 End Sub)
#Else
    Public Shared ReadOnly OpenTweetInfo As OpenTweetInformation = Nothing
#End If

End Class
