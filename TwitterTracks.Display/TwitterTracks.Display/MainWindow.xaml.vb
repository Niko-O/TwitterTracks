Class MainWindow

    Dim ViewModel As MainWindowViewModel

    Dim MainConnection As TwitterTracks.DatabaseAccess.DatabaseConnection
    Dim MainDatabase As TwitterTracks.DatabaseAccess.ResearcherDatabase

    Dim NewestTweetId As Int64 = -1

    Dim CoordinatesProcessingConnection As TwitterTracks.DatabaseAccess.DatabaseConnection
    Dim CoordinatesProcessingDatabase As TwitterTracks.DatabaseAccess.ResearcherDatabase
    Dim TweetsToProcess As New System.Collections.Concurrent.BlockingCollection(Of TwitterTracks.DatabaseAccess.Tweet)
    Dim CoordinatesProcessingThread As New System.Threading.Thread(AddressOf MultithreadedLoadLocations) With {.IsBackground = True}

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
        MapControl.IgnoreMarkerOnMouseWheel = True
        MapControl.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance
        MainConnection = TwitterTracks.DatabaseAccess.DatabaseConnection.PlainConnection("localhost", "root", "")
        MainConnection.Open()
        MainDatabase = New TwitterTracks.DatabaseAccess.ResearcherDatabase(MainConnection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier("BobsDatabase"), New TwitterTracks.DatabaseAccess.EntityId(1))
        CoordinatesProcessingThread.Start()
        PullTweets()
    End Sub

    Protected Overrides Sub OnClosing(e As System.ComponentModel.CancelEventArgs)
        MyBase.OnClosing(e)
        GMap.NET.GMaps.Instance.CancelTileCaching()
    End Sub

    Private Sub PullTweets()
        Dim NewTweets As IEnumerable(Of TwitterTracks.DatabaseAccess.Tweet)
        If NewestTweetId = -1 Then
            NewTweets = MainDatabase.GetAllTweets
        Else
            NewTweets = MainDatabase.GetTweetsSinceEntityId(New TwitterTracks.DatabaseAccess.EntityId(NewestTweetId))
        End If
        For Each i In NewTweets
            If i.EntityId.RawId > NewestTweetId Then
                NewestTweetId = i.EntityId.RawId
            End If
            Select Case i.Location.LocationType
                Case DatabaseAccess.TweetLocationType.None, _
                     DatabaseAccess.TweetLocationType.UserRegionNoCoordinates
                    'Nothing
                Case DatabaseAccess.TweetLocationType.TweetCoordinates, _
                     DatabaseAccess.TweetLocationType.UserRegionWithCoordinates
                    AddMarker(i.Location.Latitude, i.Location.Longitude)
                Case DatabaseAccess.TweetLocationType.UserRegionWithPotentialForCoordinates
                    TweetsToProcess.Add(i)
            End Select
        Next
    End Sub

    Private Sub MultithreadedLoadLocations()
        CoordinatesProcessingConnection = TwitterTracks.DatabaseAccess.DatabaseConnection.PlainConnection("localhost", "root", "")
        CoordinatesProcessingConnection.Open()
        CoordinatesProcessingDatabase = New TwitterTracks.DatabaseAccess.ResearcherDatabase(CoordinatesProcessingConnection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier("BobsDatabase"), New TwitterTracks.DatabaseAccess.EntityId(1))
        Do
            Dim Tweet = TweetsToProcess.Take
            Dim Status As GMap.NET.GeoCoderStatusCode
            Dim Point = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetPoint(Tweet.Location.UserRegion, Status)
            If Status = GMap.NET.GeoCoderStatusCode.G_GEO_SUCCESS AndAlso Point.HasValue Then
                If Not CoordinatesProcessingDatabase.TryUpdateTweetUserRegionWithCoordinates(Tweet.EntityId, Point.Value.Lat, Point.Value.Lng) Then
                    Stop
                End If
                Dispatcher.BeginInvoke(Sub()
                                           AddMarker(Point.Value.Lat, Point.Value.Lng)
                                       End Sub)
            Else
                If Not CoordinatesProcessingDatabase.TryUpdateTweetToUserRegionNoCoordinates(Tweet.EntityId) Then
                    Stop
                End If
            End If
        Loop
    End Sub

    Private Sub AddMarker(Latitude As Double, Longitude As Double)
        MapControl.Markers.Add(New GMap.NET.WindowsPresentation.GMapMarker(New GMap.NET.PointLatLng(Latitude, Longitude)) With _
                               {
                                   .Shape = New Ellipse With _
                                   {
                                       .Width = 10,
                                       .Height = 10,
                                       .RenderTransform = New TranslateTransform(-5, -5),
                                       .Fill = Brushes.Red
                                   }
                               })
    End Sub

End Class
