Class MainWindow 

    Dim ViewModel As MainWindowViewModel
    Dim Database As TwitterTracks.DatabaseAccess.ResearcherDatabase = Nothing

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
    End Sub

    Private Sub OpenOrCreateTrack(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Dlg As New OpenTrackDialog.OpenTrackDialog With {.Owner = Me}
        If Dlg.ShowDialog() Then
            ViewModel.OpenTweetInfo = Dlg.GetOpenTweetInfo
        End If
    End Sub

    Private Sub PublishTweet(sender As System.Object, e As System.Windows.RoutedEventArgs)
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
            ViewModel.PublishStatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
            Return
        End If

        Dim StartStreamResult = TwitterTracks.TweetinviInterop.ServiceProvider.Service.StartTwitterStream(PublishResult.ResultTweet.Id, PublishResult.ResultTweet.CreatedByUserId, ViewModel.OpenTweetInfo.TweetData.Keywords, ViewModel.OpenTweetInfo.TwitterConnection.ToAuthenticationToken)
        If Not StartStreamResult.Success Then
            Dim ErrorMessage = "Starting the Twitter API Stream failed after publishing the Tweet (this would be a good time to panic!):"
            If StartStreamResult.ErrorException IsNot Nothing Then
                ErrorMessage &= " " & StartStreamResult.ErrorException.GetType.Name & ": " & StartStreamResult.ErrorException.Message
            End If
            ViewModel.PublishStatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
            Return
        End If

        Dim NewMetadata As New TwitterTracks.DatabaseAccess.TrackMetadata(PublishResult.ResultTweet.Id, PublishResult.ResultTweet.CreatedByUserId, ViewModel.OpenTweetInfo.TweetData.TweetText, ViewModel.OpenTweetInfo.TweetData.Keywords)
        Dim NewDatabase = New TwitterTracks.DatabaseAccess.ResearcherDatabase(ViewModel.OpenTweetInfo.Database.Connection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier(ViewModel.OpenTweetInfo.Database.Name), New TwitterTracks.DatabaseAccess.EntityId(ViewModel.OpenTweetInfo.Database.ResearcherId))
        NewDatabase.UpdateOrCreateTrackMetadata(NewMetadata)
        ViewModel.OpenTweetInfo.TweetData.Metadata = NewMetadata

        AddHandler TwitterTracks.TweetinviInterop.ServiceProvider.Service.StreamStarted, AddressOf StreamStarted
        AddHandler TwitterTracks.TweetinviInterop.ServiceProvider.Service.StreamStopped, AddressOf StreamStopped
        AddHandler TwitterTracks.TweetinviInterop.ServiceProvider.Service.TweetReceived, AddressOf TweetReceived

        Database = NewDatabase
        ViewModel.PublishStatusMessageVM.ClearStatus()
        ViewModel.OpenTweetInfo.IsPublished = True
    End Sub

    Private Sub LoadConfiguration(sender As System.Object, e As System.Windows.RoutedEventArgs)
        If Not System.IO.File.Exists(TwitterTracks.Common.Paths.GatheringConfigurationFilePath) Then
            ViewModel.LoadConfigurationStatusMessageVM.SetStatus("The configuration file does not exist.", Common.UI.StatusMessageKindType.Warning)
            Return
        End If
        Dim Root = XElement.Load(TwitterTracks.Common.Paths.GatheringConfigurationFilePath)

        Dim Result As New OpenTweetInformation

        Dim Dlg As New PasswordDialog.PasswordDialog With {.Owner = Me}
        If Not Dlg.ShowDialog Then
            ViewModel.LoadConfigurationStatusMessageVM.ClearStatus()
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
            ViewModel.LoadConfigurationStatusMessageVM.SetStatus("The database connection could not be opened. The password is probably wrong." & Environment.NewLine & ex.Message, Common.UI.StatusMessageKindType.Error)
            Return
        End Try

        Dim NewDatabase As New TwitterTracks.DatabaseAccess.ResearcherDatabase(Result.Database.Connection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier(Result.Database.Name), New TwitterTracks.DatabaseAccess.EntityId(Result.Database.ResearcherId))
        Dim ExistingMetadata = NewDatabase.TryGetTrackMetadata
        Dim MetadataExistsAndThereforeTweetIsPublished = ExistingMetadata IsNot Nothing
        If MetadataExistsAndThereforeTweetIsPublished <> Result.IsPublished Then
            Dim ErrorMessage = String.Format("The configuration file says the Tweet is {0} published but the database says the opposite.", If(Result.IsPublished, "already", "not yet"))
            If MetadataExistsAndThereforeTweetIsPublished Then
                Dim Metadata = ExistingMetadata.Value
                ErrorMessage &= String.Format("The database stores the Tweet {1}, created by {2}:{0}{3}", Environment.NewLine, Metadata.InitialTweetId, Metadata.InitialTweetUserId, Metadata.InitialTweetFullText)
            End If
            ViewModel.LoadConfigurationStatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
            Return
        End If
        If MetadataExistsAndThereforeTweetIsPublished Then
            Database = NewDatabase
        End If

        ViewModel.LoadConfigurationStatusMessageVM.ClearStatus()
        ViewModel.OpenTweetInfo = Result
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
                           <%= If(ViewModel.OpenTweetInfo.IsPublished, _
                               <Metadata>
                                   <InitialTweetId><%= ViewModel.OpenTweetInfo.TweetData.Metadata.InitialTweetId %></InitialTweetId>
                                   <InitialTweetUserId><%= ViewModel.OpenTweetInfo.TweetData.Metadata.InitialTweetUserId %></InitialTweetUserId>
                                   <InitialTweetFullText><%= ViewModel.OpenTweetInfo.TweetData.Metadata.InitialTweetFullText %></InitialTweetFullText>
                                   <RelevantKeywords><%= ViewModel.OpenTweetInfo.TweetData.Metadata.RelevantKeywords.Select(Function(i) <Keyword><%= i %></Keyword>) %></RelevantKeywords>
                               </Metadata>, _
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

    Protected Overrides Sub OnClosing(e As System.ComponentModel.CancelEventArgs)
        If ViewModel.OpenTweetInfo IsNot Nothing Then
            If MessageBox.Show("There are things loaded in the application. If you close it those things will be lost.", "Closing application", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) = MessageBoxResult.Cancel Then
                e.Cancel = True
            End If
        End If
        MyBase.OnClosing(e)
    End Sub

    <TwitterTracks.TweetinviInterop.MultithreadingAwareness()>
    Private Sub StreamStarted(sender As Object, e As EventArgs)
        'ToDo
    End Sub

    <TwitterTracks.TweetinviInterop.MultithreadingAwareness()>
    Private Sub StreamStopped(sender As Object, e As TweetinviInterop.StreamStoppedEventArgs)
        'ToDo
    End Sub

    <TwitterTracks.TweetinviInterop.MultithreadingAwareness()>
    Private Sub TweetReceived(sender As Object, e As TweetinviInterop.TweetReceivedEventArgs)
        Dim Location As TwitterTracks.DatabaseAccess.TweetLocation
        If e.Tweet.HasCoordinates Then
            Location = TwitterTracks.DatabaseAccess.TweetLocation.FromTweetCoordinates(e.Tweet.Latitude, e.Tweet.Longitude)
        ElseIf e.Tweet.UserRegion Is Nothing Then
            Location = TwitterTracks.DatabaseAccess.TweetLocation.FromNone
        Else
            Location = TwitterTracks.DatabaseAccess.TweetLocation.FromUserRegion(e.Tweet.UserRegion)
        End If
        Database.CreateTweet(e.Tweet.Id, e.Tweet.Text, e.Tweet.PublishDateTime, Location)
        ViewModel.NumberOfTrackedTweets += 1
    End Sub

    Private Sub Button_Click(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim StartStreamResult = TwitterTracks.TweetinviInterop.ServiceProvider.Service.StartTwitterStream(ViewModel.OpenTweetInfo.TweetData.Metadata.InitialTweetId, ViewModel.OpenTweetInfo.TweetData.Metadata.InitialTweetUserId, ViewModel.OpenTweetInfo.TweetData.Keywords, ViewModel.OpenTweetInfo.TwitterConnection.ToAuthenticationToken)
        If Not StartStreamResult.Success Then
            Dim ErrorMessage = "Starting the Twitter API Stream failed:"
            If StartStreamResult.ErrorException IsNot Nothing Then
                ErrorMessage &= " " & StartStreamResult.ErrorException.GetType.Name & ": " & StartStreamResult.ErrorException.Message
            End If
            ViewModel.PublishStatusMessageVM.SetStatus(ErrorMessage, Common.UI.StatusMessageKindType.Error)
            Return
        End If

        AddHandler TwitterTracks.TweetinviInterop.ServiceProvider.Service.StreamStarted, AddressOf StreamStarted
        AddHandler TwitterTracks.TweetinviInterop.ServiceProvider.Service.StreamStopped, AddressOf StreamStopped
        AddHandler TwitterTracks.TweetinviInterop.ServiceProvider.Service.TweetReceived, AddressOf TweetReceived

        ViewModel.PublishStatusMessageVM.ClearStatus()
    End Sub

End Class
