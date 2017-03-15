Class MainWindow 

    Dim ViewModel As MainWindowViewModel
    Dim Database As TwitterTracks.DatabaseAccess.ResearcherDatabase = Nothing

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
        AddHandler TwitterTracks.Gathering.TweetinviInterop.ServiceProvider.Service.StreamStarted, AddressOf StreamStarted
        AddHandler TwitterTracks.Gathering.TweetinviInterop.ServiceProvider.Service.StreamStopped, AddressOf StreamStopped
        AddHandler TwitterTracks.Gathering.TweetinviInterop.ServiceProvider.Service.TweetReceived, AddressOf TweetReceived
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
                TwitterTracks.Gathering.TweetinviInterop.ServiceProvider.Service.StopTwitterStream()
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

    Private Sub LoadConfiguration(sender As System.Object, e As System.Windows.RoutedEventArgs)
        If Not System.IO.File.Exists(TwitterTracks.Common.Paths.GatheringConfigurationFilePath) Then
            ViewModel.StatusMessageVM.SetStatus("The configuration file does not exist.", Common.UI.StatusMessageKindType.Warning)
            Return
        End If
        Dim Root = XElement.Load(TwitterTracks.Common.Paths.GatheringConfigurationFilePath)

        Dim Result As New OpenTrackInformation

        Dim Dlg As New PasswordDialog.PasswordDialog With {.Owner = Me}
        If Not Dlg.ShowDialog Then
            ViewModel.StatusMessageVM.ClearStatus()
            Return
        End If

        Result.Metadata = New TwitterTracks.DatabaseAccess.TrackMetadata(
            Boolean.Parse(Root.<Metadata>.<IsPublished>.Value), _
            Int64.Parse(Root.<Metadata>.<TweetId>.Value), _
            Int64.Parse(Root.<Metadata>.<CreatedByUserId>.Value), _
            Root.<Metadata>.<TweetText>.Value, _
            Root.<Metadata>.<RelevantKeywords>.<Keyword>.Select(Function(i) i.Value), _
            Root.<Metadata>.<MediaFilePathsToAdd>.<MediaToAdd>.Select(Function(i) i.Value), _
            Root.<Metadata>.<ConsumerKey>.Value, _
            Root.<Metadata>.<ConsumerSecret>.Value, _
            Root.<Metadata>.<AccessToken>.Value, _
            Root.<Metadata>.<AccessTokenSecret>.Value)

        Result.Database.Host = Root.<Database>.<Host>.Value
        Result.Database.Name = Root.<Database>.<Name>.Value
        Result.Database.ResearcherId = Int64.Parse(Root.<Database>.<ResearcherId>.Value)
        Result.Database.Password = Dlg.Password

        Result.Database.Connection = New TwitterTracks.DatabaseAccess.DatabaseConnection _
        (
            Result.Database.Host, _
            TwitterTracks.DatabaseAccess.Relations.UserNames.ResearcherUserName _
            (
                New TwitterTracks.DatabaseAccess.VerbatimIdentifier(Result.Database.Name), _
                New TwitterTracks.DatabaseAccess.EntityId(Result.Database.ResearcherId)
            ), _
            Result.Database.Password
        )

        Try
            Result.Database.Connection.Open()
        Catch ex As System.Data.Common.DbException
            ViewModel.StatusMessageVM.SetStatus("The database connection could not be opened. The password is probably wrong or the database is not running." & Environment.NewLine & ex.Message, Common.UI.StatusMessageKindType.Error)
            Return
        End Try

        Database = New TwitterTracks.DatabaseAccess.ResearcherDatabase(Result.Database.Connection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier(Result.Database.Name), New TwitterTracks.DatabaseAccess.EntityId(Result.Database.ResearcherId))
        Dim ExistingMetadata = Database.TryGetTrackMetadata
        Dim TweetIsPublished = ExistingMetadata IsNot Nothing AndAlso ExistingMetadata.Value.IsPublished
        If TweetIsPublished <> Result.Metadata.IsPublished Then
            Dim ErrorMessage = String.Format("The configuration file says the Tweet is {0} published but the database says the opposite.", If(Result.Metadata.IsPublished, "already", "not yet"))
            If TweetIsPublished Then
                Dim Metadata = ExistingMetadata.Value
                ErrorMessage &= String.Format("The database stores the Tweet {1}, created by {2}:{0}{3}", Environment.NewLine, Result.Metadata.TweetId, Result.Metadata.CreatedByUserId, Result.Metadata.TweetText)
            End If
            ViewModel.StatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
            Return
        End If

        ViewModel.StatusMessageVM.ClearStatus()
        ViewModel.OpenTrackInfo = Result

        ViewModel.NumberOfTrackedTweets = Database.CountAllTweets()
        If TweetIsPublished Then
            StartStream()
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

        Dim AuthenticationToken As New TwitterTracks.Gathering.TweetinviInterop.AuthenticationToken( _
                ViewModel.OpenTrackInfo.Metadata.ConsumerKey, ViewModel.OpenTrackInfo.Metadata.ConsumerSecret, _
                ViewModel.OpenTrackInfo.Metadata.AccessToken, ViewModel.OpenTrackInfo.Metadata.AccessTokenSecret)

        Dim PublishResult = TwitterTracks.Gathering.TweetinviInterop.ServiceProvider.Service.PublishTweet(ViewModel.OpenTrackInfo.Metadata.TweetText, MediaBinaries, AuthenticationToken)
        If Not PublishResult.Success Then
            Dim ErrorMessage = "Publishing failed:"
            If PublishResult.ErrorException IsNot Nothing Then
                ErrorMessage &= " " & PublishResult.ErrorException.GetType.Name & ": " & PublishResult.ErrorException.Message
            End If
            ViewModel.StatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
            Return
        End If

        Dim StartStreamResult = TwitterTracks.Gathering.TweetinviInterop.ServiceProvider.Service.StartTwitterStream(PublishResult.ResultTweet.Id, PublishResult.ResultTweet.CreatedByUserId, ViewModel.OpenTrackInfo.Metadata.RelevantKeywords, AuthenticationToken)
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

    Private Sub SaveConfiguration(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Root = <GatheringConfiguration>
                       <Database>
                           <Host><%= ViewModel.OpenTrackInfo.Database.Host %></Host>
                           <Name><%= ViewModel.OpenTrackInfo.Database.Name %></Name>
                           <ResearcherId><%= ViewModel.OpenTrackInfo.Database.ResearcherId %></ResearcherId>
                       </Database>
                       <Metadata>
                           <IsPublished><%= ViewModel.OpenTrackInfo.Metadata.IsPublished %></IsPublished>
                           <TweetId><%= ViewModel.OpenTrackInfo.Metadata.TweetId %></TweetId>
                           <CreatedByUserId><%= ViewModel.OpenTrackInfo.Metadata.CreatedByUserId %></CreatedByUserId>
                           <TweetText><%= ViewModel.OpenTrackInfo.Metadata.TweetText %></TweetText>
                           <RelevantKeywords>
                               <%= ViewModel.OpenTrackInfo.Metadata.RelevantKeywords.Select(Function(i) <Keyword><%= i %></Keyword>) %>
                           </RelevantKeywords>
                           <MediaFilePathsToAdd>
                               <%= ViewModel.OpenTrackInfo.Metadata.MediaFilePathsToAdd.Select(Function(i) <MediaFilePath><%= i %></MediaFilePath>) %>
                           </MediaFilePathsToAdd>
                           <ConsumerKey><%= ViewModel.OpenTrackInfo.Metadata.ConsumerKey %></ConsumerKey>
                           <ConsumerSecret><%= ViewModel.OpenTrackInfo.Metadata.ConsumerSecret %></ConsumerSecret>
                           <AccessToken><%= ViewModel.OpenTrackInfo.Metadata.AccessToken %></AccessToken>
                           <AccessTokenSecret><%= ViewModel.OpenTrackInfo.Metadata.AccessTokenSecret %></AccessTokenSecret>
                       </Metadata>
                   </GatheringConfiguration>
        Root.Save(TwitterTracks.Common.Paths.GatheringConfigurationFilePath)
    End Sub

    Private Sub ManuallyStartTrackingStream(sender As System.Object, e As System.Windows.RoutedEventArgs)
        DebugPrint("MainWindow.ManuallyStartTrackingStream")
        StartStream()
    End Sub

    Private Sub StartStream()
        DebugPrint("MainWindow.StartStream")
        Dim AuthenticationToken As New TwitterTracks.Gathering.TweetinviInterop.AuthenticationToken( _
            ViewModel.OpenTrackInfo.Metadata.ConsumerKey, ViewModel.OpenTrackInfo.Metadata.ConsumerSecret, _
            ViewModel.OpenTrackInfo.Metadata.AccessToken, ViewModel.OpenTrackInfo.Metadata.AccessTokenSecret)
        Dim StartStreamResult = TwitterTracks.Gathering.TweetinviInterop.ServiceProvider.Service.StartTwitterStream(ViewModel.OpenTrackInfo.Metadata.TweetId, ViewModel.OpenTrackInfo.Metadata.CreatedByUserId, ViewModel.OpenTrackInfo.Metadata.RelevantKeywords, AuthenticationToken)
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
    Private Sub StreamStopped(sender As Object, e As TweetinviInterop.StreamStoppedEventArgs)
        Dim WasIntentional = False
        Dim DisconnectReasonString As String
        Select Case e.Reason
            Case TweetinviInterop.StreamStopReason.Disconnected
                DisconnectReasonString = String.Format("Twitter sent a disconnect message:{0}Code: {1}{0}Reason: {2}", Environment.NewLine, e.Code, e.ReasonName)
            Case TweetinviInterop.StreamStopReason.LimitReached
                DisconnectReasonString = String.Format("Limit was reached. {0} Tweets were not received.", e.NumberOfTweetsNotReceived)
            Case TweetinviInterop.StreamStopReason.Stopped
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
                                  TwitterTracks.Gathering.TweetinviInterop.ServiceProvider.Service.ResumeTwitterStream()
                              End If
                          End Sub)
    End Sub

    <MultithreadingAwareness()>
    Private Sub TweetReceived(sender As Object, e As TweetinviInterop.TweetReceivedEventArgs)
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
        Database.CreateTweet(e.Tweet.Text.GetHashCode.ToString, e.Tweet.PublishDateTime, Location, e.Tweet.Id, e.Tweet.Text)
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
        Sub(Database As TwitterTracks.DatabaseAccess.ResearcherDatabase) Database.CreateTweet("1001", Helpers.UnixTimestampToUtc(1490000001), TwitterTracks.DatabaseAccess.TweetLocation.FromUserRegionWithPotentialForCoordinates("England, Vereinigtes Königreich"), Nothing, Nothing), _
        Sub(Database As TwitterTracks.DatabaseAccess.ResearcherDatabase) Database.CreateTweet("1002", Helpers.UnixTimestampToUtc(1490000002), TwitterTracks.DatabaseAccess.TweetLocation.FromUserRegionWithPotentialForCoordinates("Österreich"), Nothing, Nothing), _
        Sub(Database As TwitterTracks.DatabaseAccess.ResearcherDatabase) Database.CreateTweet("1003", Helpers.UnixTimestampToUtc(1490000003), TwitterTracks.DatabaseAccess.TweetLocation.FromUserRegionWithPotentialForCoordinates("New York"), Nothing, Nothing)
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
