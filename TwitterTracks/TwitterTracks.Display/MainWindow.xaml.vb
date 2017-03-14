Class MainWindow

    Private Shared ReadOnly MarkerBrush As New SolidColorBrush(Color.FromArgb(128, 255, 0, 0))

    Dim ViewModel As MainWindowViewModel

    Dim MainConnection As TwitterTracks.DatabaseAccess.DatabaseConnection
    Dim MainDatabase As TwitterTracks.DatabaseAccess.ResearcherDatabase

    Dim WithEvents PullTweetsTimer As New System.Windows.Threading.DispatcherTimer With {.Interval = TimeSpan.FromSeconds(5), .IsEnabled = False}
    Dim NewestTweetId As Int64 = -1

    Dim WithEvents CoordinatesLoader As TweetCoordinatesLoader = Nothing

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
        MapControl.IgnoreMarkerOnMouseWheel = True
        MapControl.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance
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
        Dim NumberOfTweets = 0
        Dim NumberOfTweetsWithCoordinates = 0
        For Each i In NewTweets
            NumberOfTweets += 1
            If i.EntityId.RawId > NewestTweetId Then
                NewestTweetId = i.EntityId.RawId
            End If
            Select Case i.Location.LocationType
                Case DatabaseAccess.TweetLocationType.None, _
                     DatabaseAccess.TweetLocationType.UserRegionNoCoordinates
                    'Nothing
                Case DatabaseAccess.TweetLocationType.TweetCoordinates, _
                     DatabaseAccess.TweetLocationType.UserRegionWithCoordinates
                    NumberOfTweetsWithCoordinates += 1
                    AddMarker(i.Location.Latitude, i.Location.Longitude)
                Case DatabaseAccess.TweetLocationType.UserRegionWithPotentialForCoordinates
                    CoordinatesLoader.EnqueueTweet(i)
            End Select
        Next
        ViewModel.TotalNumberOfTweets += NumberOfTweets
        ViewModel.NumberOfTweetsWithCoordinates += NumberOfTweetsWithCoordinates
        PullTweetsTimer.IsEnabled = OldTimerState
    End Sub

    Private Sub OpenTrack(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim OldTimerState = PullTweetsTimer.IsEnabled
        PullTweetsTimer.IsEnabled = False
        Dim Dlg As New OpenTrackDialog With {.Owner = Me}
        If Dlg.ShowDialog Then
            If CoordinatesLoader IsNot Nothing Then
                CoordinatesLoader.Stop()
            End If
            CoordinatesLoader = Nothing

            MapControl.Markers.Clear()

            Dim Info = Dlg.GetOpenTrackInfo
            Dim DatabaseName As New TwitterTracks.DatabaseAccess.VerbatimIdentifier(Info.Database.Name)
            Dim TrackEntityId As New TwitterTracks.DatabaseAccess.EntityId(Info.Database.ResearcherId)
            Dim UserName = TwitterTracks.DatabaseAccess.Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId)

            MainConnection = TwitterTracks.DatabaseAccess.DatabaseConnection.PlainConnection(Info.Database.Host, UserName, Info.Database.Password)
            MainConnection.Open()
            MainDatabase = New TwitterTracks.DatabaseAccess.ResearcherDatabase(MainConnection, DatabaseName, TrackEntityId)

            CoordinatesLoader = New TweetCoordinatesLoader(Me.Dispatcher, Info.Database.Host, UserName, Info.Database.Password, DatabaseName, TrackEntityId)
            CoordinatesLoader.Start()

            NewestTweetId = -1
            ViewModel.TotalNumberOfTweets = 0
            ViewModel.NumberOfTweetsWithCoordinates = 0
            ViewModel.TrackIsLoaded = True
            PullTweets()
            PullTweetsTimer.IsEnabled = True
        Else
            PullTweetsTimer.IsEnabled = OldTimerState
        End If
    End Sub

    Private Sub CoordinatesLoader_TweetCoordinatesLoaded(sender As Object, e As TweetCoordinatesLoadedEventArgs) Handles CoordinatesLoader.TweetCoordinatesLoaded
        ViewModel.NumberOfTweetsWithCoordinates += 1
        AddMarker(e.Latitude, e.Longitude)
    End Sub

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
