Public Class TweetCoordinatesLoader

    Public Event TweetCoordinatesLoaded As EventHandler(Of TweetCoordinatesLoadedEventArgs)

    Dim Dispatcher As System.Windows.Threading.Dispatcher

    Dim Host As String
    Dim Password As String

    Dim DatabaseName As TwitterTracks.DatabaseAccess.VerbatimIdentifier
    Dim TrackEntityId As TwitterTracks.DatabaseAccess.EntityId

    Dim TweetsToLoad As New System.Collections.Concurrent.BlockingCollection(Of TwitterTracks.DatabaseAccess.Tweet)
    Dim CancellationTokenSource As New System.Threading.CancellationTokenSource

    Dim WorkerThread As System.Threading.Thread

    Public Sub New(NewDispatcher As System.Windows.Threading.Dispatcher, NewHost As String, NewUserName As String, NewPassword As String, NewDatabaseName As TwitterTracks.DatabaseAccess.VerbatimIdentifier, NewTrackEntityId As TwitterTracks.DatabaseAccess.EntityId)
        Dispatcher = NewDispatcher
        Host = NewHost
        Password = NewPassword
        DatabaseName = NewDatabaseName
        TrackEntityId = NewTrackEntityId
    End Sub

    Public Sub Start()
        WorkerThread = New System.Threading.Thread(AddressOf MultithreadedLoadCoordinates) With {.IsBackground = True}
        WorkerThread.Start()
    End Sub

    Public Sub [Stop]()
        CancellationTokenSource.Cancel()
    End Sub

    Public Sub EnqueueTweet(Tweet As TwitterTracks.DatabaseAccess.Tweet)
        TweetsToLoad.Add(Tweet)
    End Sub

    <MultithreadingAwareness()>
    Private Sub MultithreadedLoadCoordinates()
        Dim Token = CancellationTokenSource.Token

        Dim Connection As TwitterTracks.DatabaseAccess.DatabaseConnection = Nothing
        Try
            Connection = New TwitterTracks.DatabaseAccess.DatabaseConnection(Host, TwitterTracks.DatabaseAccess.Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId), Password)
            Connection.Open()
            Dim Database As New TwitterTracks.DatabaseAccess.ResearcherDatabase(Connection, DatabaseName, TrackEntityId)
            Dim Rnd As New Random
            Do Until Token.IsCancellationRequested
                Dim Tweet As TwitterTracks.DatabaseAccess.Tweet = Nothing
                Dim GotTweet As Boolean
                Try
                    GotTweet = TweetsToLoad.TryTake(Tweet, System.Threading.Timeout.Infinite, Token)
                Catch ex As OperationCanceledException
                    GotTweet = False
                End Try
                If GotTweet Then
                    Dim Status As GMap.NET.GeoCoderStatusCode
                    Dim Point = GMap.NET.MapProviders.GoogleMapProvider.Instance.GetPoint(Tweet.Location.UserRegion, Status)

                    If Status = GMap.NET.GeoCoderStatusCode.G_GEO_SUCCESS AndAlso Point.HasValue Then
                        Const MaximumDistance As Double = 3
                        Dim Distance = Math.Sqrt(Rnd.NextDouble) * MaximumDistance
                        Dim Angle = Rnd.NextDouble * Math.PI * 2

                        Dim Latitude = Point.Value.Lat + Math.Sin(Angle) * Distance * 0.5
                        Dim Longitude = Point.Value.Lng + Math.Cos(Angle) * Distance

                        If Not Database.TryUpdateTweetUserRegionWithCoordinates(Tweet.EntityId, Latitude, Longitude) Then
                            Throw New InvalidOperationException(String.Format("TryUpdateTweetUserRegionWithCoordinates cannot process Tweet {0} with LocationType {1}.", Tweet.EntityId.RawId, Tweet.Location.LocationType.ToString))
                        End If
                        Dispatcher.BeginInvoke(Sub()
                                                   If Not Token.IsCancellationRequested Then
                                                       RaiseEvent TweetCoordinatesLoaded(Me, New TweetCoordinatesLoadedEventArgs(Tweet, Latitude, Longitude))
                                                   End If
                                               End Sub)
                    Else
                        If Not Database.TryUpdateTweetToUserRegionNoCoordinates(Tweet.EntityId) Then
                            Stop
                        End If
                    End If
                End If
            Loop
        Finally
            If Connection IsNot Nothing Then
                Connection.Close()
            End If
        End Try
    End Sub

End Class
