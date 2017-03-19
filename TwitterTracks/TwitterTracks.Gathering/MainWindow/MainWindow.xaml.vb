Class MainWindow

    <MultithreadingAwareness()>
    Dim KeywordRegexes As New KeywordRegexStore

    Dim ViewModel As MainWindowViewModel
    Dim Database As TwitterTracks.DatabaseAccess.ResearcherDatabase = Nothing

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
        AddHandler Streaming.TweetinviService.Instance.StreamStarted, AddressOf StreamStarted
        AddHandler Streaming.TweetinviService.Instance.StreamStopped, AddressOf StreamStopped
        AddHandler Streaming.TweetinviService.Instance.TweetReceived, AddressOf TweetReceived
    End Sub

    Protected Overrides Sub OnClosing(e As System.ComponentModel.CancelEventArgs)
        If ViewModel.OpenTrackInfo IsNot Nothing Then
            If MessageBox.Show("There are things loaded in the application. If you close it those things will be lost.", "Closing application", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) = MessageBoxResult.Cancel Then
                e.Cancel = True
            End If
        End If
        MyBase.OnClosing(e)
        If Not e.Cancel Then
            If ViewModel.TrackingStreamIsRunning Then
                DebugPrint("MainWindow.OnClosing")
                Streaming.TweetinviService.Instance.StopTwitterStream()
            End If
        End If
    End Sub

    Private Sub OpenOrCreateTrack(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Dlg As New OpenTrackDialog.OpenTrackDialog With {.Owner = Me}
        If Dlg.ShowDialog() Then
            ViewModel.OpenTrackInfo = Dlg.GetOpenTrackInfo
            Database = New TwitterTracks.DatabaseAccess.ResearcherDatabase(ViewModel.OpenTrackInfo.Database.Connection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier(ViewModel.OpenTrackInfo.Database.Name), New TwitterTracks.DatabaseAccess.EntityId(ViewModel.OpenTrackInfo.Database.ResearcherId))
            'ToDo: Execute this as task
            Database.UpdateOrCreateTrackMetadata(ViewModel.OpenTrackInfo.Metadata)
            ViewModel.NumberOfTrackedTweets = Database.CountAllTweets()
            If ViewModel.OpenTrackInfo.Metadata.IsPublished Then
                StartStream()
            End If
            '/
        End If
    End Sub

    Private Sub PublishTweet(sender As System.Object, e As System.Windows.RoutedEventArgs)
        DebugPrint("MainWindow.PublishTweet")

        Dim MissingFiles = ViewModel.OpenTrackInfo.Metadata.MediaFilePathsToAdd.Where(Function(i) Not System.IO.File.Exists(i)).ToList
        If MissingFiles.Any Then
            MessageBox.Show(String.Format("The following files which were selected to be attached to the Tweet do not exist:{0}{1}{0}The Tweet was not published.{0}If the files were moved, move them back to their original position.", Environment.NewLine, String.Join(Environment.NewLine, MissingFiles)))
            Return
        End If

        Dim MediaBinaries As New List(Of Byte())
        For Each i In ViewModel.OpenTrackInfo.Metadata.MediaFilePathsToAdd
            MediaBinaries.Add(System.IO.File.ReadAllBytes(i))
        Next

        Dim AuthenticationToken As New Tweetinvi.Models.TwitterCredentials( _
                ViewModel.OpenTrackInfo.Metadata.ConsumerKey, ViewModel.OpenTrackInfo.Metadata.ConsumerSecret, _
                ViewModel.OpenTrackInfo.Metadata.AccessToken, ViewModel.OpenTrackInfo.Metadata.AccessTokenSecret)

        Dim PublishResult = Streaming.TweetinviService.Instance.PublishTweet(ViewModel.OpenTrackInfo.Metadata.TweetText, MediaBinaries, AuthenticationToken)
        If Not PublishResult.Success Then
            Dim ErrorMessage = "Publishing failed:"
            If PublishResult.ErrorException IsNot Nothing Then
                ErrorMessage &= " " & PublishResult.ErrorException.GetType.Name & ": " & PublishResult.ErrorException.Message
            End If
            ViewModel.StatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
            Return
        End If

        Dim StartStreamResult = Streaming.TweetinviService.Instance.StartTwitterStream(PublishResult.ResultTweet.Id, PublishResult.ResultTweet.CreatedByUserId, ViewModel.OpenTrackInfo.Metadata.RelevantKeywords, AuthenticationToken)
        If Not StartStreamResult.Success Then
            Dim ErrorMessage = "Starting the Twitter API Stream failed after publishing the Tweet (this would be a good time to panic!):"
            If StartStreamResult.ErrorException IsNot Nothing Then
                ErrorMessage &= " " & StartStreamResult.ErrorException.GetType.Name & ": " & StartStreamResult.ErrorException.Message
            End If
            ViewModel.StatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
            Return
        End If

        Dim NewMetadata = ViewModel.OpenTrackInfo.Metadata.WithPublishedData(PublishResult.ResultTweet.Id, PublishResult.ResultTweet.CreatedByUserId)
        Database.UpdateOrCreateTrackMetadata(NewMetadata)
        ViewModel.OpenTrackInfo.Metadata = NewMetadata

        ViewModel.StatusMessageVM.ClearStatus()
    End Sub

    Private Sub ManuallyStartTrackingStream(sender As System.Object, e As System.Windows.RoutedEventArgs)
        DebugPrint("MainWindow.ManuallyStartTrackingStream")
        StartStream()
    End Sub

    Private Sub StartStream()
        DebugPrint("MainWindow.StartStream")
        Dim AuthenticationToken As New Tweetinvi.Models.TwitterCredentials( _
            ViewModel.OpenTrackInfo.Metadata.ConsumerKey, ViewModel.OpenTrackInfo.Metadata.ConsumerSecret, _
            ViewModel.OpenTrackInfo.Metadata.AccessToken, ViewModel.OpenTrackInfo.Metadata.AccessTokenSecret)
        Dim StartStreamResult = Streaming.TweetinviService.Instance.StartTwitterStream(ViewModel.OpenTrackInfo.Metadata.TweetId, ViewModel.OpenTrackInfo.Metadata.CreatedByUserId, ViewModel.OpenTrackInfo.Metadata.RelevantKeywords, AuthenticationToken)
        If StartStreamResult.Success Then
            ViewModel.StatusMessageVM.ClearStatus()
        Else
            Dim ErrorMessage = "Starting the Twitter API Stream failed:"
            If StartStreamResult.ErrorException IsNot Nothing Then
                ErrorMessage &= " " & StartStreamResult.ErrorException.GetType.Name & ": " & StartStreamResult.ErrorException.Message
            End If
            ViewModel.StatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
        End If
    End Sub

    <MultithreadingAwareness()>
    Private Sub StreamStarted(sender As Object, e As EventArgs)
        DebugPrint("MainWindow.StreamStarted")
        Dispatcher.Invoke(Sub() ViewModel.TrackingStreamIsRunning = True)
    End Sub

    <MultithreadingAwareness()>
    Private Sub StreamStopped(sender As Object, e As Streaming.StreamStoppedEventArgs)
        Dim WasIntentional = False
        Dim DisconnectReasonString As String
        Select Case e.Reason
            Case Streaming.StreamStopReason.Disconnected
                DisconnectReasonString = String.Format("Twitter sent a disconnect message:{0}Code: {1}{0}Reason: {2}", Environment.NewLine, e.Code, e.ReasonName)
            Case Streaming.StreamStopReason.LimitReached
                DisconnectReasonString = String.Format("Limit was reached. {0} Tweets were not received.", e.NumberOfTweetsNotReceived)
            Case Streaming.StreamStopReason.Stopped
                If e.Exception Is Nothing Then
                    DisconnectReasonString = "The stream was stopped manually."
                    WasIntentional = True
                Else
                    DisconnectReasonString = String.Format("The stream was stopped due to an Exception:{0}{1}{0}{2}", Environment.NewLine, e.Exception.GetType.Name, e.Exception.Message)
                    If e.ReasonName IsNot Nothing Then
                        DisconnectReasonString &= String.Format("{0}Code: {1}{0}Reason: {2}", Environment.NewLine, e.Code, e.ReasonName)
                    End If
                End If
            Case Else
                Throw New NopeException
        End Select
        DebugPrint("MainWindow.StreamStopped. (Intentional: {0}) {1}", WasIntentional, DisconnectReasonString)
        Dispatcher.Invoke(Sub()
                              DebugPrint("MainWindow.StreamStopped->Invoke.")
                              ViewModel.TrackingStreamIsRunning = False
                              ViewModel.StreamDisconnectReason = DisconnectReasonString
                              If Not WasIntentional Then
                                  Streaming.TweetinviService.Instance.ResumeTwitterStream()
                              End If
                          End Sub)
    End Sub

    <MultithreadingAwareness()>
    Private Sub TweetReceived(sender As Object, e As Streaming.TweetReceivedEventArgs)
        If Database Is Nothing Then
            Throw New NopeException
            Return
        End If
        Dim Location As TwitterTracks.DatabaseAccess.TweetLocation
        If e.Tweet.HasCoordinates Then
            Location = TwitterTracks.DatabaseAccess.TweetLocation.FromTweetCoordinates(e.Tweet.Latitude, e.Tweet.Longitude)
        ElseIf e.Tweet.UserRegion Is Nothing Then
            Location = TwitterTracks.DatabaseAccess.TweetLocation.FromNone
        Else
            Location = TwitterTracks.DatabaseAccess.TweetLocation.FromUserRegionWithPotentialForCoordinates(e.Tweet.UserRegion)
        End If
        Dim MatchingKeywords = ViewModel.OpenTrackInfo.Metadata.RelevantKeywords.Where(Function(i) KeywordRegexes.GetRegex(i).IsMatch(e.Tweet.Text))
        Database.CreateTweet(e.IsRetweet, MatchingKeywords, e.Tweet.PublishDateTime, Location, e.Tweet.Id, e.Tweet.Text)
        Dispatcher.Invoke(Sub() ViewModel.NumberOfTrackedTweets += 1)
    End Sub

    Public Shared Sub DebugPrint(Text As String, ParamArray Args As Object())
        With DateTime.Now
            Helpers.DebugPrint("[{0}] @ {1}:{2}:{3}.{4}: {5}",
                               System.Threading.Thread.CurrentThread.ManagedThreadId,
                               .Hour, .Minute, .Second, .Millisecond,
                               String.Format(Text, Args))
        End With
    End Sub

    Dim TestDataIndex As Integer = 0
    Private ReadOnly TestData As New List(Of Action(Of TwitterTracks.DatabaseAccess.ResearcherDatabase)) From _
    {
        Sub(Database As TwitterTracks.DatabaseAccess.ResearcherDatabase) Database.CreateTweet(True, {}, Helpers.UnixTimestampToUtc(1490000001), TwitterTracks.DatabaseAccess.TweetLocation.FromUserRegionWithPotentialForCoordinates("England, Vereinigtes Königreich"), Nothing, Nothing), _
        Sub(Database As TwitterTracks.DatabaseAccess.ResearcherDatabase) Database.CreateTweet(False, {"#Test"}, Helpers.UnixTimestampToUtc(1490000002), TwitterTracks.DatabaseAccess.TweetLocation.FromUserRegionWithPotentialForCoordinates("Österreich"), Nothing, Nothing), _
        Sub(Database As TwitterTracks.DatabaseAccess.ResearcherDatabase) Database.CreateTweet(True, {"#Twitter", "#Test"}, Helpers.UnixTimestampToUtc(1490000003), TwitterTracks.DatabaseAccess.TweetLocation.FromUserRegionWithPotentialForCoordinates("New York"), Nothing, Nothing)
    }

    Private Sub InsertTestData(sender As System.Object, e As System.Windows.RoutedEventArgs)
        TestData(TestDataIndex)(Database)
        ViewModel.NumberOfTrackedTweets += 1
        TestDataIndex = (TestDataIndex + 1) Mod TestData.Count
    End Sub

    Private Sub InsertTestDataMultiple(sender As System.Object, e As System.Windows.RoutedEventArgs)
        For i = 1 To 10
            InsertTestData(sender, e)
        Next
    End Sub

End Class
