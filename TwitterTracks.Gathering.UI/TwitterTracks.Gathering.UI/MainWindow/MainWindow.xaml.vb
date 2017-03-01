Class MainWindow 

    Dim ViewModel As MainWindowViewModel
    Dim Database As TwitterTracks.DatabaseAccess.ResearcherDatabase = Nothing

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
        AddHandler TwitterTracks.TweetinviInterop.ServiceProvider.Service.StreamStarted, AddressOf StreamStarted
        AddHandler TwitterTracks.TweetinviInterop.ServiceProvider.Service.StreamStopped, AddressOf StreamStopped
        AddHandler TwitterTracks.TweetinviInterop.ServiceProvider.Service.TweetReceived, AddressOf TweetReceived
    End Sub

    Protected Overrides Sub OnClosing(e As System.ComponentModel.CancelEventArgs)
        If ViewModel.OpenTweetInfo IsNot Nothing Then
            If MessageBox.Show("There are things loaded in the application. If you close it those things will be lost.", "Closing application", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) = MessageBoxResult.Cancel Then
                e.Cancel = True
            End If
        End If
        MyBase.OnClosing(e)
        If Not e.Cancel Then
            If ViewModel.TrackingStreamIsRunning Then
                DebugPrint("MainWindow.OnClosing")
                TwitterTracks.TweetinviInterop.ServiceProvider.Service.StopTwitterStream()
            End If
        End If
    End Sub

    Private Sub OpenOrCreateTrack(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Dlg As New OpenTrackDialog.OpenTrackDialog With {.Owner = Me}
        If Dlg.ShowDialog() Then
            ViewModel.OpenTweetInfo = Dlg.GetOpenTweetInfo
            Database = New TwitterTracks.DatabaseAccess.ResearcherDatabase(ViewModel.OpenTweetInfo.Database.Connection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier(ViewModel.OpenTweetInfo.Database.Name), New TwitterTracks.DatabaseAccess.EntityId(ViewModel.OpenTweetInfo.Database.ResearcherId))
            ViewModel.NumberOfTrackedTweets = Database.CountAllTweets()
            If ViewModel.OpenTweetInfo.IsPublished Then
                StartStream()
            End If
        End If
    End Sub

    Private Sub LoadConfiguration(sender As System.Object, e As System.Windows.RoutedEventArgs)
        If Not System.IO.File.Exists(TwitterTracks.Common.Paths.GatheringConfigurationFilePath) Then
            ViewModel.StatusMessageVM.SetStatus("The configuration file does not exist.", Common.UI.StatusMessageKindType.Warning)
            Return
        End If
        Dim Root = XElement.Load(TwitterTracks.Common.Paths.GatheringConfigurationFilePath)

        Dim Result As New OpenTweetInformation

        Dim Dlg As New PasswordDialog.PasswordDialog With {.Owner = Me}
        If Not Dlg.ShowDialog Then
            ViewModel.StatusMessageVM.ClearStatus()
            Return
        End If

        Result.IsPublished = Boolean.Parse(Root.<IsPublished>.Value)

        Result.Database.Host = Root.<Database>.<Host>.Value
        Result.Database.Name = Root.<Database>.<Name>.Value
        Result.Database.ResearcherId = Int64.Parse(Root.<Database>.<ResearcherId>.Value)
        Result.Database.Password = Dlg.Password

        If Root.<TweetData>.<Metadata>.Single.Descendants.Any Then
            Result.TweetData.Metadata = New TwitterTracks.DatabaseAccess.TrackMetadata _
            (
                Int64.Parse(Root.<TweetData>.<Metadata>.<InitialTweetId>.Value),
                Int64.Parse(Root.<TweetData>.<Metadata>.<InitialTweetUserId>.Value),
                Root.<TweetData>.<Metadata>.<InitialTweetFullText>.Value,
                Root.<TweetData>.<Metadata>.<RelevantKeywords>.Descendants.Select(Function(i) i.Value)
            )
        End If

        Result.TweetData.TweetText = Root.<TweetData>.<TweetText>.Value
        Result.TweetData.MediasToAdd = Root.<TweetData>.<MediasToAdd>.Descendants.Select(Function(i) i.Value).ToList
        Result.TweetData.Keywords = Root.<TweetData>.<Keywords>.Descendants.Select(Function(i) i.Value).ToList

        Result.TwitterConnection.ConsumerKey = Root.<TwitterConnection>.<ConsumerKey>.Value
        Result.TwitterConnection.ConsumerSecret = Root.<TwitterConnection>.<ConsumerSecret>.Value
        Result.TwitterConnection.AccessToken = Root.<TwitterConnection>.<AccessToken>.Value
        Result.TwitterConnection.AccessTokenSecret = Root.<TwitterConnection>.<AccessTokenSecret>.Value

        Result.Database.Connection = TwitterTracks.DatabaseAccess.DatabaseConnection.PlainConnection _
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
        Catch ex As MySql.Data.MySqlClient.MySqlException
            ViewModel.StatusMessageVM.SetStatus("The database connection could not be opened. The password is probably wrong or the database is not running." & Environment.NewLine & ex.Message, Common.UI.StatusMessageKindType.Error)
            Return
        End Try

        Database = New TwitterTracks.DatabaseAccess.ResearcherDatabase(Result.Database.Connection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier(Result.Database.Name), New TwitterTracks.DatabaseAccess.EntityId(Result.Database.ResearcherId))
        Dim ExistingMetadata = Database.TryGetTrackMetadata
        Dim MetadataExistsAndThereforeTweetIsPublished = ExistingMetadata IsNot Nothing
        If MetadataExistsAndThereforeTweetIsPublished <> Result.IsPublished Then
            Dim ErrorMessage = String.Format("The configuration file says the Tweet is {0} published but the database says the opposite.", If(Result.IsPublished, "already", "not yet"))
            If MetadataExistsAndThereforeTweetIsPublished Then
                Dim Metadata = ExistingMetadata.Value
                ErrorMessage &= String.Format("The database stores the Tweet {1}, created by {2}:{0}{3}", Environment.NewLine, Metadata.InitialTweetId, Metadata.InitialTweetUserId, Metadata.InitialTweetFullText)
            End If
            ViewModel.StatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
            Return
        End If

        ViewModel.StatusMessageVM.ClearStatus()
        ViewModel.OpenTweetInfo = Result

        ViewModel.NumberOfTrackedTweets = Database.CountAllTweets()
        If MetadataExistsAndThereforeTweetIsPublished Then
            StartStream()
        End If
    End Sub

    Private Sub PublishTweet(sender As System.Object, e As System.Windows.RoutedEventArgs)
        DebugPrint("MainWindow.PublishTweet")

        Dim MediaBinaries As New List(Of Byte())
        For Each i In ViewModel.OpenTweetInfo.TweetData.MediasToAdd
            MediaBinaries.Add(System.IO.File.ReadAllBytes(i))
        Next

        Dim PublishResult = TwitterTracks.TweetinviInterop.ServiceProvider.Service.PublishTweet(ViewModel.OpenTweetInfo.TweetData.TweetText, _
                                                                                                MediaBinaries, _
                                                                                                ViewModel.OpenTweetInfo.TwitterConnection.ToAuthenticationToken)
        If Not PublishResult.Success Then
            Dim ErrorMessage = "Publishing failed:"
            If PublishResult.ErrorException IsNot Nothing Then
                ErrorMessage &= " " & PublishResult.ErrorException.GetType.Name & ": " & PublishResult.ErrorException.Message
            End If
            ViewModel.StatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
            Return
        End If

        Dim StartStreamResult = TwitterTracks.TweetinviInterop.ServiceProvider.Service.StartTwitterStream(PublishResult.ResultTweet.Id, PublishResult.ResultTweet.CreatedByUserId, ViewModel.OpenTweetInfo.TweetData.Keywords, ViewModel.OpenTweetInfo.TwitterConnection.ToAuthenticationToken)
        If Not StartStreamResult.Success Then
            Dim ErrorMessage = "Starting the Twitter API Stream failed after publishing the Tweet (this would be a good time to panic!):"
            If StartStreamResult.ErrorException IsNot Nothing Then
                ErrorMessage &= " " & StartStreamResult.ErrorException.GetType.Name & ": " & StartStreamResult.ErrorException.Message
            End If
            ViewModel.StatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
            Return
        End If

        Dim NewMetadata As New TwitterTracks.DatabaseAccess.TrackMetadata(PublishResult.ResultTweet.Id, PublishResult.ResultTweet.CreatedByUserId, ViewModel.OpenTweetInfo.TweetData.TweetText, ViewModel.OpenTweetInfo.TweetData.Keywords)
        Database.UpdateOrCreateTrackMetadata(NewMetadata)
        ViewModel.OpenTweetInfo.TweetData.Metadata = NewMetadata

        ViewModel.StatusMessageVM.ClearStatus()
        ViewModel.OpenTweetInfo.IsPublished = True
    End Sub

    Private Sub SaveConfiguration(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Root = <GatheringConfiguration>
                       <IsPublished><%= ViewModel.OpenTweetInfo.IsPublished %></IsPublished>
                       <Database>
                           <Host><%= ViewModel.OpenTweetInfo.Database.Host %></Host>
                           <Name><%= ViewModel.OpenTweetInfo.Database.Name %></Name>
                           <ResearcherId><%= ViewModel.OpenTweetInfo.Database.ResearcherId %></ResearcherId>
                       </Database>
                       <TweetData>
                           <%= If(ViewModel.OpenTweetInfo.IsPublished,
                               <Metadata>
                                   <InitialTweetId><%= ViewModel.OpenTweetInfo.TweetData.Metadata.InitialTweetId %></InitialTweetId>
                                   <InitialTweetUserId><%= ViewModel.OpenTweetInfo.TweetData.Metadata.InitialTweetUserId %></InitialTweetUserId>
                                   <InitialTweetFullText><%= ViewModel.OpenTweetInfo.TweetData.Metadata.InitialTweetFullText %></InitialTweetFullText>
                                   <RelevantKeywords><%= ViewModel.OpenTweetInfo.TweetData.Metadata.RelevantKeywords.Select(Function(i) <Keyword><%= i %></Keyword>) %></RelevantKeywords>
                               </Metadata>,
                               <Metadata/>
                               ) %>
                           <TweetText><%= ViewModel.OpenTweetInfo.TweetData.TweetText %></TweetText>
                           <MediasToAdd><%= ViewModel.OpenTweetInfo.TweetData.MediasToAdd.Select(Function(i) <FilePath><%= i %></FilePath>) %></MediasToAdd>
                           <Keywords><%= ViewModel.OpenTweetInfo.TweetData.Keywords.Select(Function(i) <Keyword><%= i %></Keyword>) %></Keywords>
                       </TweetData>
                       <TwitterConnection>
                           <ConsumerKey><%= ViewModel.OpenTweetInfo.TwitterConnection.ConsumerKey %></ConsumerKey>
                           <ConsumerSecret><%= ViewModel.OpenTweetInfo.TwitterConnection.ConsumerSecret %></ConsumerSecret>
                           <AccessToken><%= ViewModel.OpenTweetInfo.TwitterConnection.AccessToken %></AccessToken>
                           <AccessTokenSecret><%= ViewModel.OpenTweetInfo.TwitterConnection.AccessTokenSecret %></AccessTokenSecret>
                       </TwitterConnection>
                   </GatheringConfiguration>
        Root.Save(TwitterTracks.Common.Paths.GatheringConfigurationFilePath)
    End Sub

    Private Sub ManuallyStartTrackingStream(sender As System.Object, e As System.Windows.RoutedEventArgs)
        DebugPrint("MainWindow.ManuallyStartTrackingStream")
        StartStream()
    End Sub

    Private Sub StartStream()
        DebugPrint("MainWindow.StartStream")
        Dim StartStreamResult = TwitterTracks.TweetinviInterop.ServiceProvider.Service.StartTwitterStream(ViewModel.OpenTweetInfo.TweetData.Metadata.InitialTweetId, ViewModel.OpenTweetInfo.TweetData.Metadata.InitialTweetUserId, ViewModel.OpenTweetInfo.TweetData.Keywords, ViewModel.OpenTweetInfo.TwitterConnection.ToAuthenticationToken)
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

    <TwitterTracks.TweetinviInterop.MultithreadingAwareness()>
    Private Sub StreamStarted(sender As Object, e As EventArgs)
        DebugPrint("MainWindow.StreamStarted")
        Dispatcher.Invoke(Sub() ViewModel.TrackingStreamIsRunning = True)
    End Sub

    <TwitterTracks.TweetinviInterop.MultithreadingAwareness()>
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
                                  TwitterTracks.TweetinviInterop.ServiceProvider.Service.ResumeTwitterStream()
                              End If
                          End Sub)
    End Sub

    <TwitterTracks.TweetinviInterop.MultithreadingAwareness()>
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

End Class
