Public Class Form_Main

    Public Sub New()
        InitializeComponent()

    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)

        Debug.Print("")
        Debug.Print("")
        Debug.Print("New Session -----------------------------------------------")

        Debug.Print("Opening Connection")
        Dim Connection As New TwitterTracks.DatabaseAccess.DatabaseConnection(New DebugConnectionStringSource)
        Connection.Open()

        Debug.Print("Getting Tables")
        Dim Database As New TwitterTracks.DatabaseAccess.Database(Connection)
        Dim AllDatabaseNames = Database.GetAllDatabaseNames.ToList

        Dim TrackDatabase As TwitterTracks.DatabaseAccess.TrackDatabase
        Dim TestDatabaseName As New TwitterTracks.DatabaseAccess.VerbatimIdentifier("Bo'b`sT''ra``cks")
        If AllDatabaseNames.Contains(TestDatabaseName) Then
            Debug.Print("Table exists")
            TrackDatabase = New TwitterTracks.DatabaseAccess.TrackDatabase(Connection, TestDatabaseName)
        Else
            Debug.Print("Creating Table")
            TrackDatabase = Database.CreateTrackDatabase(TestDatabaseName, "").TrackDatabase
        End If

        Debug.Print("Deleting Table")
        TrackDatabase.DeleteDatabase()

        Debug.Print("Creating Researcher Objects")
        Dim ResearcherData = TrackDatabase.CreateTrack("Pas's`wo''r``d2")

        Debug.Print("Getting all Tweets")
        Dim Tracks = From Track In TrackDatabase.GetAllTracksWithoutMetadata.ToList
                     Let Tweets = TrackDatabase.GetAllTweets(Track.EntityId).ToList

        Debug.Print("Getting new Tweets")
        Dim NewTweets = TrackDatabase.GetTweetsSinceEntityId(Tracks(0).Track.EntityId, Tracks(0).Tweets.Last.EntityId)

        Dim bp = 0
    End Sub

End Class
