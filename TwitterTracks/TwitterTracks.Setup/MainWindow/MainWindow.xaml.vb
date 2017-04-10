
Imports System.Threading.Tasks

Class MainWindow

    Dim ViewModel As MainWindowViewModel

    Dim RootConnection As TwitterTracks.DatabaseAccess.DatabaseConnection = Nothing
    Dim AdministratorConnection As TwitterTracks.DatabaseAccess.DatabaseConnection = Nothing
    Dim AdministratorDatabaseName As TwitterTracks.DatabaseAccess.VerbatimIdentifier = Nothing
    Dim ResearcherConnection As TwitterTracks.DatabaseAccess.DatabaseConnection = Nothing
    Dim ResearcherDatabaseName As TwitterTracks.DatabaseAccess.VerbatimIdentifier = Nothing
    Dim ResearcherEntityId As TwitterTracks.DatabaseAccess.EntityId = Nothing

    Dim WithEvents Tasks As New TwitterTracks.Common.UI.Tasks.WindowTaskManager(Me.Dispatcher)
    Private Sub Tasks_IsBusyChanged() Handles Tasks.IsBusyChanged
        ViewModel.IsBusy = Tasks.IsBusy
    End Sub

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
    End Sub

#Region "Connection"

    Private Sub RootTools_ToggleConnection(sender As System.Object, e As System.Windows.RoutedEventArgs)
        If ViewModel.RootToolsVM.IsConnectedToDatabase Then
            RootConnection.Close()
            RootConnection = Nothing
            ViewModel.RootToolsVM.IsConnectedToDatabase = False
        Else
            Dim NewConnection As TwitterTracks.DatabaseAccess.DatabaseConnection = Nothing
            Tasks.DoSqlTaskWithStatusMessage( _
                ViewModel.RootToolsVM.DatabaseConnectionStatusMessageVM, _
                "The connection could not be opened", _
                Sub()
                    NewConnection = New TwitterTracks.DatabaseAccess.DatabaseConnection( _
                        ViewModel.RootToolsVM.DatabaseConnectionVM.DatabaseHost, _
                        ViewModel.RootToolsVM.DatabaseConnectionVM.DatabaseNameOrUserName, _
                        ViewModel.RootToolsVM.DatabaseConnectionVM.Password)
                    NewConnection.Open()
                End Sub, _
                Function(Success As Boolean)
                    If Success Then
                        RootConnection = NewConnection
                        ViewModel.RootToolsVM.IsConnectedToDatabase = True
                    End If
                    Return Nothing
                End Function, _
                Sub(Success As Boolean)
                    If Success Then
                        RefreshDatabaseList(sender, e)
                    End If
                End Sub)
        End If
    End Sub

    Private Sub AdministratorTools_ToggleConnection(sender As System.Object, e As System.Windows.RoutedEventArgs)
        If ViewModel.AdministratorToolsVM.IsConnectedToDatabase Then
            AdministratorConnection.Close()
            AdministratorConnection = Nothing
            ViewModel.AdministratorToolsVM.IsConnectedToDatabase = False
            'ToDo: Clear state depending on selected database.
            ViewModel.AdministratorToolsVM.TracksVM.AvailableTracks.Clear()
            ViewModel.AdministratorToolsVM.TracksVM.SelectedAvailableTrack = Nothing
        Else
            Dim Host = ViewModel.AdministratorToolsVM.DatabaseConnectionVM.DatabaseHost
            Dim Password = ViewModel.AdministratorToolsVM.DatabaseConnectionVM.Password
            Dim NewConnection As TwitterTracks.DatabaseAccess.DatabaseConnection = Nothing
            AdministratorDatabaseName = New TwitterTracks.DatabaseAccess.VerbatimIdentifier(ViewModel.AdministratorToolsVM.DatabaseConnectionVM.DatabaseNameOrUserName)
            Tasks.DoSqlTaskWithStatusMessage( _
                ViewModel.AdministratorToolsVM.DatabaseConnectionStatusMessageVM, _
                "The connection could not be opened", _
                Sub()
                    NewConnection = New TwitterTracks.DatabaseAccess.DatabaseConnection(Host, TwitterTracks.DatabaseAccess.Relations.UserNames.AdministratorUserName(AdministratorDatabaseName), Password)
                    NewConnection.Open()
                End Sub, _
                Function(Success As Boolean)
                    If Success Then
                        AdministratorConnection = NewConnection
                        ViewModel.AdministratorToolsVM.IsConnectedToDatabase = True
                    End If
                    Return Nothing
                End Function, _
                Sub(Success As Boolean)
                    If Success Then
                        BeginRefreshTrackList(True)
                    End If
                End Sub)
        End If
    End Sub

    Private Sub ResearcherTools_ToggleConnection(sender As System.Object, e As System.Windows.RoutedEventArgs)
        If ViewModel.ResearcherToolsVM.IsConnectedToDatabase Then
            ResearcherConnection.Close()
            ResearcherConnection = Nothing
            ViewModel.ResearcherToolsVM.IsConnectedToDatabase = False
        Else
            Dim Host = ViewModel.ResearcherToolsVM.DatabaseConnectionVM.DatabaseHost
            Dim Password = ViewModel.ResearcherToolsVM.DatabaseConnectionVM.Password
            Dim NewConnection As TwitterTracks.DatabaseAccess.DatabaseConnection = Nothing
            ResearcherDatabaseName = New TwitterTracks.DatabaseAccess.VerbatimIdentifier(ViewModel.ResearcherToolsVM.DatabaseConnectionVM.DatabaseNameOrUserName)
            ResearcherEntityId = New TwitterTracks.DatabaseAccess.EntityId(Int64.Parse(ViewModel.ResearcherToolsVM.DatabaseConnectionVM.ResearcherIdText))
            Tasks.DoSqlTaskWithStatusMessage( _
                ViewModel.ResearcherToolsVM.DatabaseConnectionStatusMessageVM, _
                "The connection could not be opened", _
                Sub()
                    NewConnection = New TwitterTracks.DatabaseAccess.DatabaseConnection(Host, TwitterTracks.DatabaseAccess.Relations.UserNames.ResearcherUserName(ResearcherDatabaseName, ResearcherEntityId), Password)
                    NewConnection.Open()
                End Sub, _
                Function(Success As Boolean)
                    If Success Then
                        ResearcherConnection = NewConnection
                        ViewModel.ResearcherToolsVM.IsConnectedToDatabase = True
                    End If
                    Return Nothing
                End Function, _
                Sub(Success As Boolean)
                    If Success Then
                        UpdateTrackMetadata(sender, e)
                    End If
                End Sub)
        End If
    End Sub

#End Region

#Region "Root Tools"

    Private Sub RefreshDatabaseList(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Names As List(Of String) = Nothing
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.RootToolsVM.DatabasesVM.StatusMessageVM, _
            "The database names could not be read", _
            Sub()
                Dim Database As New TwitterTracks.DatabaseAccess.DatabaseServer(RootConnection)
                Names = Database.GetAllDatabaseNames.Select(Function(i) i.UnescapedText).ToList
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    ViewModel.RootToolsVM.DatabasesVM.AvailableDatabases.Clear()
                    ViewModel.RootToolsVM.DatabasesVM.AvailableDatabases.AddRange(Names)
                End If
                Return Nothing
            End Function)
    End Sub

    Private Sub DeleteSelectedDatabase(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim DatabaseName = ViewModel.RootToolsVM.DatabasesVM.SelectedAvailableDatabase
        If MessageBox.Show(String.Format("Are you sure you want to delete database ""{0}""?", DatabaseName), _
                           "Delete database?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) <> MessageBoxResult.Yes Then
            Return
        End If
        If MessageBox.Show(String.Format("Seriously, it's gone if you delete it.{0}Forever.{0}Cannot be undone.{0}You need to be absolutely sure that you want to delete database ""{1}"". You could make a lot of people very angry if you delete the wrong database!", Environment.NewLine, DatabaseName), _
                           "Really delete database?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) <> MessageBoxResult.Yes Then
            Return
        End If
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.RootToolsVM.DatabasesVM.StatusMessageVM, _
            "The database could not be deleted", _
            Sub()
                Dim TrackDatabase As New TwitterTracks.DatabaseAccess.TrackDatabase(RootConnection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier(DatabaseName))
                TrackDatabase.DeleteDatabase()
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    ViewModel.RootToolsVM.DatabasesVM.AvailableDatabases.Remove(DatabaseName)
                    ViewModel.RootToolsVM.DatabasesVM.SelectedAvailableDatabase = Nothing
                    MessageBox.Show("Aaaaand... It's gone. Don't blame me if you find out it was the wrong one.", "It's gone", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK)
                End If
                Return Nothing
            End Function)
    End Sub

    Private Sub CreateDatabase(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim DatabaseName = ViewModel.RootToolsVM.CreateDatabaseVM.DatabaseName
        Dim Password = ViewModel.RootToolsVM.CreateDatabaseVM.Password
        Dim ResultUserName As String = Nothing
        Tasks.DoSqlTaskWithStatusMessage( _
           ViewModel.RootToolsVM.CreateDatabaseVM.StatusMessageVM, _
           "The database could not be created", _
           Sub()
               Dim Database As New TwitterTracks.DatabaseAccess.DatabaseServer(RootConnection)
               Dim TrackDatabaseStuff = Database.CreateTrackDatabase(New TwitterTracks.DatabaseAccess.VerbatimIdentifier(DatabaseName), Password)
               ResultUserName = TrackDatabaseStuff.AdministratorUser.Name
           End Sub, _
           Function(Success As Boolean)
               If Success Then
                   ViewModel.RootToolsVM.DatabasesVM.AvailableDatabases.Insert(0, DatabaseName)
                   ViewModel.RootToolsVM.DatabasesVM.SelectedAvailableDatabase = DatabaseName
                   Return Tuple.Create("Additional information: The created Administrator's account name is: " & ResultUserName, TwitterTracks.Common.UI.StatusMessageKindType.JustText)
               End If
               Return Nothing
           End Function)
    End Sub

#End Region

#Region "Administrator Tools"

    Private Sub UpdateApplicationTokenStoredInDatabase()
        Dim ApplicationToken As TwitterTracks.DatabaseAccess.ApplicationToken? = Nothing
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.AdministratorToolsVM.ApplicationTokenVM.StatusMessageVM, _
            "The Application Token could not be read", _
            Sub()
                Dim Database As New TwitterTracks.DatabaseAccess.TrackDatabase(AdministratorConnection, AdministratorDatabaseName)
                ApplicationToken = Database.TryGetApplicationToken()
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    Dim Exists = ApplicationToken IsNot Nothing
                    ViewModel.AdministratorToolsVM.ApplicationTokenVM.IsStoredInDatabase = Exists
                    If Exists Then
                        ViewModel.AdministratorToolsVM.ApplicationTokenVM.ConsumerKeyInDatabase = ApplicationToken.Value.ConsumerKey
                        ViewModel.AdministratorToolsVM.ApplicationTokenVM.ConsumerSecretInDatabase = ApplicationToken.Value.ConsumerSecret
                    End If
                End If
                Return Nothing
            End Function)
    End Sub

    Private Sub OpenTwitterAppsPage(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Using P As New Process
            P.StartInfo.FileName = "https://apps.twitter.com/app/new"
            P.Start()
        End Using
    End Sub

    Private Sub SaveApplicationKeys(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim ApplicationToken As New TwitterTracks.DatabaseAccess.ApplicationToken(ViewModel.AdministratorToolsVM.ApplicationTokenVM.ConsumerKeyToSet, ViewModel.AdministratorToolsVM.ApplicationTokenVM.ConsumerSecretToSet)
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.AdministratorToolsVM.ApplicationTokenVM.StatusMessageVM, _
            "The Application Token could not be read", _
            Sub()
                Dim Database As New TwitterTracks.DatabaseAccess.TrackDatabase(AdministratorConnection, AdministratorDatabaseName)
                Database.UpdateOrCreateApplicationToken(ApplicationToken)
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    ViewModel.AdministratorToolsVM.ApplicationTokenVM.IsStoredInDatabase = True
                    ViewModel.AdministratorToolsVM.ApplicationTokenVM.ConsumerKeyInDatabase = ApplicationToken.ConsumerKey
                    ViewModel.AdministratorToolsVM.ApplicationTokenVM.ConsumerSecretInDatabase = ApplicationToken.ConsumerSecret
                End If
                Return Nothing
            End Function)
    End Sub

    Private Sub DeleteApplicationKeys(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.AdministratorToolsVM.ApplicationTokenVM.StatusMessageVM, _
            "The Application Token could not be read", _
            Sub()
                Dim Database As New TwitterTracks.DatabaseAccess.TrackDatabase(AdministratorConnection, AdministratorDatabaseName)
                Database.DeleteApplicationToken()
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    ViewModel.AdministratorToolsVM.ApplicationTokenVM.IsStoredInDatabase = False
                    ViewModel.AdministratorToolsVM.ApplicationTokenVM.ConsumerKeyInDatabase = ""
                    ViewModel.AdministratorToolsVM.ApplicationTokenVM.ConsumerSecretInDatabase = ""
                End If
                Return Nothing
            End Function)
    End Sub

    Private Sub RefreshTrackList(sender As System.Object, e As System.Windows.RoutedEventArgs)
        BeginRefreshTrackList(False)
    End Sub

    Private Sub BeginRefreshTrackList(UpdateApplicationTokenOnEnd As Boolean)
        Dim Tracks As List(Of TwitterTracks.DatabaseAccess.Track) = Nothing
        Tasks.DoSqlTaskWithStatusMessage( _
           ViewModel.AdministratorToolsVM.TracksVM.StatusMessageVM, _
           "The Tracks could not be read", _
           Sub()
               Dim TrackDatabase As New TwitterTracks.DatabaseAccess.TrackDatabase(AdministratorConnection, AdministratorDatabaseName)
               Tracks = TrackDatabase.GetAllTracksWithoutMetadata.ToList
           End Sub, _
           Function(Success As Boolean)
               If Success Then
                   ViewModel.AdministratorToolsVM.TracksVM.AvailableTracks.Clear()
                   ViewModel.AdministratorToolsVM.TracksVM.AvailableTracks.AddRange(Tracks)
               End If
               Return Nothing
           End Function, _
           Sub(Success As Boolean)
               If UpdateApplicationTokenOnEnd Then
                   UpdateApplicationTokenStoredInDatabase()
               End If
           End Sub)
    End Sub

    Private Sub DeleteSelectedTrack(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Track = ViewModel.AdministratorToolsVM.TracksVM.SelectedAvailableTrack
        If MessageBox.Show(String.Format("Are you sure you want to delete Track ""{0}""?", Track.EntityId.RawId), _
                           "Delete Track?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) <> MessageBoxResult.Yes Then
            Return
        End If
        If MessageBox.Show(String.Format("Seriously, it's gone if you delete it.{0}Forever.{0}Cannot be undone.{0}You need to be absolutely sure that you want to delete Track ""{1}"". You could make a lot of people very angry if you delete the wrong Track!", Environment.NewLine, Track.EntityId.RawId), _
                           "Really delete Track?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) <> MessageBoxResult.Yes Then
            Return
        End If
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.AdministratorToolsVM.TracksVM.StatusMessageVM, _
            "The Track could not be deleted", _
            Sub()
                Dim ResearcherDatabase As New TwitterTracks.DatabaseAccess.ResearcherDatabase(AdministratorConnection, AdministratorDatabaseName, Track.EntityId)
                ResearcherDatabase.DeleteTrack()
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    ViewModel.AdministratorToolsVM.TracksVM.AvailableTracks.Remove(Track)
                    ViewModel.AdministratorToolsVM.TracksVM.SelectedAvailableTrack = Nothing
                    MessageBox.Show("Aaaaand... It's gone. Don't blame me if you find out it was the wrong one.", "It's gone", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK)
                End If
                Return Nothing
            End Function)
    End Sub

    Private Sub CreateTrack(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim CreateTrackResult As TwitterTracks.DatabaseAccess.TrackDatabase.CreateTrackResult = Nothing
        Dim Password = ViewModel.AdministratorToolsVM.CreateTrackVM.Password
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.AdministratorToolsVM.CreateTrackVM.StatusMessageVM, _
            "The Track could not be created", _
            Sub()
                Dim TrackDatabase As New TwitterTracks.DatabaseAccess.TrackDatabase(AdministratorConnection, AdministratorDatabaseName)
                CreateTrackResult = TrackDatabase.CreateTrack(Password)
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    ViewModel.AdministratorToolsVM.TracksVM.AvailableTracks.Add(CreateTrackResult.Track)
                    ViewModel.AdministratorToolsVM.TracksVM.SelectedAvailableTrack = CreateTrackResult.Track
                    ViewModel.AdministratorToolsVM.CreateTrackVM.CreatedResearcherId = CreateTrackResult.ResearcherUser.Name
                    ViewModel.AdministratorToolsVM.CreateTrackVM.Password = ""
                    ViewModel.AdministratorToolsVM.CreateTrackVM.RetypePassword = ""
                End If
                Return Nothing
            End Function)
    End Sub

#End Region

#Region "Researcher Tools"

    Private Sub UpdateTrackMetadata(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim NewMetadata As TwitterTracks.DatabaseAccess.TrackMetadata? = Nothing
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.AdministratorToolsVM.CreateTrackVM.StatusMessageVM, _
            "The metadata could not be read", _
            Sub()
                Dim ResearcherDatabase As New TwitterTracks.DatabaseAccess.ResearcherDatabase(ResearcherConnection, ResearcherDatabaseName, ResearcherEntityId)
                NewMetadata = ResearcherDatabase.TryGetTrackMetadata
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    ViewModel.ResearcherToolsVM.DeleteMetadataVM.Metadata = NewMetadata
                End If
                Return Nothing
            End Function)
    End Sub

    Private Sub UnpublishInitialTweet(sender As System.Object, e As System.Windows.RoutedEventArgs)
        If MessageBox.Show("Are you sure you want to unpublish the initial Tweet? This cannot be undone.", "Unpublish initial Tweet?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) <> MessageBoxResult.Yes Then
            Return
        End If
        Dim NewMetadata As TwitterTracks.DatabaseAccess.TrackMetadata? = Nothing
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.AdministratorToolsVM.CreateTrackVM.StatusMessageVM, _
            "The initial Tweet could not be unpublished", _
            Sub()
                Dim ResearcherDatabase As New TwitterTracks.DatabaseAccess.ResearcherDatabase(ResearcherConnection, ResearcherDatabaseName, ResearcherEntityId)
                Dim ExistingMetadata = ResearcherDatabase.TryGetTrackMetadata
                If ExistingMetadata Is Nothing OrElse Not ExistingMetadata.Value.IsPublished Then
                    Return
                End If
                With ExistingMetadata.Value
                    NewMetadata = TwitterTracks.DatabaseAccess.TrackMetadata.FromUnpublished(.TweetText, .RelevantKeywords, .MediaFilePathsToAdd, .AccessTokenSecret, .AccessTokenSecret)
                End With
                ResearcherDatabase.UpdateOrCreateTrackMetadata(NewMetadata.Value)
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    ViewModel.ResearcherToolsVM.DeleteMetadataVM.Metadata = NewMetadata
                End If
                Return Nothing
            End Function)
    End Sub

    Private Sub DeleteAllMetadata(sender As System.Object, e As System.Windows.RoutedEventArgs)
        If MessageBox.Show("Are you sure you want to unpublish the initial Tweet and delete all information about keywords, files and so on? This cannot be undone.", "Delete information?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) <> MessageBoxResult.Yes Then
            Return
        End If
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.AdministratorToolsVM.CreateTrackVM.StatusMessageVM, _
            "The metadata could not be deleted", _
            Sub()
                Dim ResearcherDatabase As New TwitterTracks.DatabaseAccess.ResearcherDatabase(ResearcherConnection, ResearcherDatabaseName, ResearcherEntityId)
                ResearcherDatabase.DeleteTrackMetadata()
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    ViewModel.ResearcherToolsVM.DeleteMetadataVM.Metadata = Nothing
                End If
                Return Nothing
            End Function)
    End Sub

    Private Sub DeleteAllTweets(sender As System.Object, e As System.Windows.RoutedEventArgs)
        If MessageBox.Show("Are you sure you want to delete all of the Tweets in this Track? This cannot be undone.", "Delete all Tweets?", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) <> MessageBoxResult.Yes Then
            Return
        End If
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.AdministratorToolsVM.CreateTrackVM.StatusMessageVM, _
            "The Tweets could not be deleted", _
            Sub()
                Dim ResearcherDatabase As New TwitterTracks.DatabaseAccess.ResearcherDatabase(ResearcherConnection, ResearcherDatabaseName, ResearcherEntityId)
                ResearcherDatabase.DeleteAllTweets()
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    ViewModel.ResearcherToolsVM.DeleteMetadataVM.Metadata = Nothing
                End If
                Return Nothing
            End Function)
    End Sub

#End Region

End Class
