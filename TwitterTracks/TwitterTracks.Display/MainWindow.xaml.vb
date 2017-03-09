Class MainWindow

    Private Shared ReadOnly DatabaseName As New TwitterTracks.DatabaseAccess.VerbatimIdentifier("BobsDatabase")
    Private Shared ReadOnly TrackEntityId As New TwitterTracks.DatabaseAccess.EntityId(2)

    Dim ViewModel As MainWindowViewModel

    Dim MainConnection As TwitterTracks.DatabaseAccess.DatabaseConnection
    Dim MainDatabase As TwitterTracks.DatabaseAccess.ResearcherDatabase
    Dim WithEvents PullTweetsTimer As New System.Windows.Threading.DispatcherTimer With {.Interval = TimeSpan.FromSeconds(5), .IsEnabled = False}
    Dim NewestTweetId As Int64 = -1

    <ThreadStatic()>
    Private Shared CoordinatesProcessingRnd As Random
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
        MainDatabase = New TwitterTracks.DatabaseAccess.ResearcherDatabase(MainConnection, DatabaseName, TrackEntityId)
        CoordinatesProcessingThread.Start()
        PullTweets()
        PullTweetsTimer.IsEnabled = True
    End Sub

    Protected Overrides Sub OnClosing(e As System.ComponentModel.CancelEventArgs)
        MyBase.OnClosing(e)
        GMap.NET.GMaps.Instance.CancelTileCaching()
    End Sub

    Private Sub PullTweets() Handles PullTweetsTimer.Tick
        Dim OldTimerState = PullTweetsTimer.IsEnabled
        PullTweetsTimer.IsEnabled = False
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
        PullTweetsTimer.IsEnabled = OldTimerState
    End Sub

    Private Sub MultithreadedLoadLocations()
        If CoordinatesProcessingRnd Is Nothing Then
            CoordinatesProcessingRnd = New Random
        End If
        CoordinatesProcessingConnection = TwitterTracks.DatabaseAccess.DatabaseConnection.PlainConnection("localhost", "root", "")
        CoordinatesProcessingConnection.Open()
        CoordinatesProcessingDatabase = New TwitterTracks.DatabaseAccess.ResearcherDatabase(CoordinatesProcessingConnection, DatabaseName, TrackEntityId)
        Do
            Dim Tweet = TweetsToProcess.Take
            Dim Status As GMap.NET.GeoCoderStatusCode
            Dim Point = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetPoint(Tweet.Location.UserRegion, Status)

            If Status = GMap.NET.GeoCoderStatusCode.G_GEO_SUCCESS AndAlso Point.HasValue Then
                Const MaximumDistance As Double = 3
                Dim Distance = Math.Sqrt(CoordinatesProcessingRnd.NextDouble) * MaximumDistance
                Dim Angle = CoordinatesProcessingRnd.NextDouble * Math.PI * 2
                Dim Latitude = Point.Value.Lat + Math.Sin(Angle) * Distance * 0.5
                Dim Longitude = Point.Value.Lng + Math.Cos(Angle) * Distance
                If Not CoordinatesProcessingDatabase.TryUpdateTweetUserRegionWithCoordinates(Tweet.EntityId, Latitude, Longitude) Then
                    Stop
                End If
                Dispatcher.BeginInvoke(Sub()
                                           AddMarker(Latitude, Longitude)
                                       End Sub)
            Else
                If Not CoordinatesProcessingDatabase.TryUpdateTweetToUserRegionNoCoordinates(Tweet.EntityId) Then
                    Stop
                End If
            End If
        Loop
    End Sub

    Private Shared ReadOnly MarkerBrush As New SolidColorBrush(Color.FromArgb(128, 255, 0, 0))
    Private Sub AddMarker(Latitude As Double, Longitude As Double)
        MapControl.Markers.Add(New GMap.NET.WindowsPresentation.GMapMarker(New GMap.NET.PointLatLng(Latitude, Longitude)) With _
                               {
                                   .Shape = New Ellipse With _
                                   {
                                       .Width = 10,
                                       .Height = 10,
                                       .RenderTransform = New TranslateTransform(-5, -5),
                                       .Fill = MarkerBrush
                                   }
                               })
    End Sub

End Class
