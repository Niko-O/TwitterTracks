Class MainWindow

    Dim ViewModel As MainWindowViewModel

    Dim Connection As TwitterTracks.DatabaseAccess.DatabaseConnection
    Dim Database As TwitterTracks.DatabaseAccess.ResearcherDatabase

    Dim NewestTweetId As Int64 = -1

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
        MapControl.IgnoreMarkerOnMouseWheel = True
        MapControl.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance
        Connection = TwitterTracks.DatabaseAccess.DatabaseConnection.PlainConnection("localhost", "root", "")
        Connection.Open()
        Database = New TwitterTracks.DatabaseAccess.ResearcherDatabase(Connection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier("BobsDatabase"), New TwitterTracks.DatabaseAccess.EntityId(1))
        PullTweets()
    End Sub

    Protected Overrides Sub OnClosing(e As System.ComponentModel.CancelEventArgs)
        MyBase.OnClosing(e)
        GMap.NET.GMaps.Instance.CancelTileCaching()
    End Sub

    Private Sub PullTweets()
        Dim NewTweets As IEnumerable(Of TwitterTracks.DatabaseAccess.Tweet)
        If NewestTweetId = -1 Then
            NewTweets = Database.GetAllTweets
        Else
            NewTweets = Database.GetTweetsSinceEntityId(New TwitterTracks.DatabaseAccess.EntityId(NewestTweetId))
        End If
        Dim ToLoad As New List(Of TwitterTracks.DatabaseAccess.Tweet)
        For Each i In NewTweets
            If i.EntityId.RawId > NewestTweetId Then
                NewestTweetId = i.EntityId.RawId
            End If
            Select Case i.Location.LocationType
                Case DatabaseAccess.TweetLocationType.None
                    'Nothing
                Case DatabaseAccess.TweetLocationType.TweetCoordinates
                    AddMarker(i.Location.Latitude, i.Location.Longitude)
                Case DatabaseAccess.TweetLocationType.UserRegion
                    ToLoad.Add(i)
            End Select
        Next
        LoadLocationsAsync(ToLoad)
    End Sub

    Private Sub LoadLocationsAsync(Tweets As List(Of TwitterTracks.DatabaseAccess.Tweet))
        Dim Th As New System.Threading.Thread(Sub() MultithreadedLoadLocations(Tweets)) With {.IsBackground = True}
        Th.Start()
    End Sub

    Private Sub MultithreadedLoadLocations(Tweets As List(Of TwitterTracks.DatabaseAccess.Tweet))
        For Each i In Tweets
            Dim Status As GMap.NET.GeoCoderStatusCode
            Dim Point = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetPoint(i.Location.UserRegion, Status)
            If Status = GMap.NET.GeoCoderStatusCode.G_GEO_SUCCESS AndAlso Point.HasValue Then
                Dispatcher.BeginInvoke(Sub() AddMarker(Point.Value.Lat, Point.Value.Lng))
            End If
        Next
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
