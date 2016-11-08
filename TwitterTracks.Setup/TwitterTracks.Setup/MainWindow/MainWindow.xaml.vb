﻿
Imports System.Threading.Tasks

Class MainWindow

    Dim ViewModel As MainWindowViewModel
    Dim Connection As TwitterTracks.DatabaseAccess.DatabaseConnection

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, MainWindowViewModel)
    End Sub

#Region "Tasks"

    Dim CurrentTask As Task = Nothing

    Private Sub StartTask(BackgroundThreadMethod As Action)
        If CurrentTask IsNot Nothing Then
            Throw New NopeException("There is already a task running.")
        End If
        ViewModel.IsBusy = True
        CurrentTask = Task.Factory.StartNew(BackgroundThreadMethod)
    End Sub

    Private Sub FinishTask(GuiThreadMethod As Action)
        Dispatcher.BeginInvoke(Sub()
                                   If CurrentTask Is Nothing Then
                                       Throw New NopeException("There is currently no task running.")
                                   End If
                                   GuiThreadMethod()
                                   CurrentTask = Nothing
                                   ViewModel.IsBusy = False
                               End Sub)
    End Sub

#End Region

    Private Sub DoMySqlTaskWithStatusMessage(StatusMessageVM As StatusMessageViewModel, ErrorDescriptor As String, BackgroundThreadMethod As Action, GuiThreadMethod As Func(Of Boolean, Tuple(Of String, StatusMessageKindType)))
        StartTask(Sub()
                      Dim RethrowException As Exception = Nothing
                      Dim ErrorException As MySql.Data.MySqlClient.MySqlException = Nothing
                      Try
                          BackgroundThreadMethod()
                      Catch ex As MySql.Data.MySqlClient.MySqlException
                          ErrorException = ex
                      Catch ex As Exception
                          RethrowException = ex
                      End Try
                      FinishTask(Sub()
                                     If RethrowException IsNot Nothing Then
                                         Throw ErrorException
                                     End If
                                     Dim ResultMessageOverride = GuiThreadMethod(ErrorException Is Nothing)
                                     If ErrorException Is Nothing Then
                                         If ResultMessageOverride Is Nothing Then
                                             StatusMessageVM.ClearStatus()
                                         Else
                                             StatusMessageVM.SetStatus(If(ResultMessageOverride.Item2 = StatusMessageKindType.Error, ErrorDescriptor, "") & ":" & Environment.NewLine & ResultMessageOverride.Item1, ResultMessageOverride.Item2)
                                         End If
                                     Else
                                         Dim ErrorMessage = ErrorDescriptor & ":" & Environment.NewLine & ErrorException.GetType.Name & " (Error Code " & ErrorException.Number & "): " & ErrorException.Message
                                         If ResultMessageOverride.Item1 IsNot Nothing Then
                                             ErrorMessage &= Environment.NewLine & "Additional information: " & ResultMessageOverride.Item1
                                         End If
                                         StatusMessageVM.SetStatus(ErrorMessage, StatusMessageKindType.Error)
                                     End If
                                 End Sub)
                  End Sub)
    End Sub

#Region "Connection"

    Private Sub ConnectToDatabaseOrCloseConnection(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim NewConnection As TwitterTracks.DatabaseAccess.DatabaseConnection = Nothing
        DoMySqlTaskWithStatusMessage( _
            ViewModel.ConnectionVM.OpenConnectionVM.StatusMessageVM, _
            "The connection could not be opened", _
            Sub()
                NewConnection = TwitterTracks.DatabaseAccess.DatabaseConnection.PlainConnection(ViewModel.ConnectionVM.OpenConnectionVM.DatabaseHost, ViewModel.ConnectionVM.OpenConnectionVM.UserName, ViewModel.ConnectionVM.OpenConnectionVM.Password)
                NewConnection.Open()
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    Connection = NewConnection
                    ViewModel.IsConnectedToDatabase = True
                End If
                Return Nothing
            End Function)

        'If ViewModel.IsConnectedToDatabase Then
        '    Connection.Close()
        '    Connection = Nothing
        '    ViewModel.IsConnectedToDatabase = False
        'Else
        '    Dim NewConnection = TwitterTracks.DatabaseAccess.DatabaseConnection.PlainConnection(ViewModel.ConnectionVM.OpenConnectionVM.DatabaseHost, ViewModel.ConnectionVM.OpenConnectionVM.UserName, ViewModel.ConnectionVM.OpenConnectionVM.Password)
        '    StartTask(Sub()
        '                  Dim RethrowException As Exception = Nothing
        '                  Dim ErrorException As MySql.Data.MySqlClient.MySqlException = Nothing
        '                  Try
        '                      NewConnection.Open()
        '                  Catch ex As MySql.Data.MySqlClient.MySqlException
        '                      ErrorException = ex
        '                  Catch ex As Exception
        '                      RethrowException = ex
        '                  End Try
        '                  FinishTask(Sub()
        '                                 If RethrowException IsNot Nothing Then
        '                                     Throw ErrorException
        '                                 End If
        '                                 If ErrorException Is Nothing Then
        '                                     Connection = NewConnection
        '                                     ViewModel.IsConnectedToDatabase = True
        '                                     ViewModel.ConnectionVM.OpenConnectionVM.StatusMessageVM.ClearStatus()
        '                                 Else
        '                                     ViewModel.ConnectionVM.OpenConnectionVM.StatusMessageVM.SetStatus("The connection could not be opened:" & Environment.NewLine & ErrorException.GetType.Name & ": " & ErrorException.Message, StatusMessageKindType.Error)
        '                                 End If
        '                             End Sub)
        '              End Sub)
        'End If
    End Sub

#End Region

#Region "Root Tools"

    Private Sub RefreshDatabaseList(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Names As List(Of String) = Nothing
        DoMySqlTaskWithStatusMessage( _
            ViewModel.RootToolsVM.DatabasesVM.StatusMessageVM, _
            "The database names could not be read", _
            Sub()
                Dim Database As New TwitterTracks.DatabaseAccess.Database(Connection)
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
        DoMySqlTaskWithStatusMessage( _
            ViewModel.RootToolsVM.DatabasesVM.StatusMessageVM, _
            "The database could not be deleted", _
            Sub()
                Dim TrackDatabase As New TwitterTracks.DatabaseAccess.TrackDatabase(Connection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier(DatabaseName))
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
        DoMySqlTaskWithStatusMessage( _
           ViewModel.RootToolsVM.CreateDatabaseVM.StatusMessageVM, _
           "The database could not be created", _
           Sub()
               Dim Database As New TwitterTracks.DatabaseAccess.Database(Connection)
               Dim TrackDatabaseStuff = Database.CreateTrackDatabase(New TwitterTracks.DatabaseAccess.VerbatimIdentifier(DatabaseName), Password)
               ResultUserName = TrackDatabaseStuff.AdministratorUser.Name
           End Sub, _
           Function(Success As Boolean)
               If Success Then
                   ViewModel.RootToolsVM.DatabasesVM.AvailableDatabases.Insert(0, DatabaseName)
                   ViewModel.RootToolsVM.DatabasesVM.SelectedAvailableDatabase = DatabaseName
                   Return Tuple.Create("Additional information: The created Administrator's account name is: " & ResultUserName, StatusMessageKindType.JustText)
               End If
               Return Nothing
           End Function)
    End Sub

#End Region

#Region "Administrator Tools"

    Private Sub ToggleSelectedAdministratorDatabase(sender As System.Object, e As System.Windows.RoutedEventArgs)
        If ViewModel.AdministratorToolsVM.DatabaseIsSelected Then
            ViewModel.AdministratorToolsVM.DatabaseIsSelected = False
            'ToDo: Clear state depending on selected database.
        Else
            Dim DatabaseName = ViewModel.AdministratorToolsVM.DatabaseName
            Dim DatabaseExists As Boolean = False
            DoMySqlTaskWithStatusMessage( _
               ViewModel.AdministratorToolsVM.SelectionStatusMessageVM, _
               "The database could not be selected", _
               Sub()
                   Dim Database As New TwitterTracks.DatabaseAccess.Database(Connection)
                   Dim DatabaseNames = Database.GetAllDatabaseNames.ToList
                   DatabaseExists = DatabaseNames.Contains(New TwitterTracks.DatabaseAccess.VerbatimIdentifier(DatabaseName))
               End Sub, _
               Function(Success As Boolean)
                   If Success Then
                       If DatabaseExists Then
                           ViewModel.AdministratorToolsVM.DatabaseIsSelected = True
                       Else
                           Return Tuple.Create(String.Format("Database ""{0}"" does not exist.", DatabaseName), StatusMessageKindType.Error)
                       End If
                   End If
                   Return Nothing
               End Function)
        End If
    End Sub

    Private Sub RefreshTrackList(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim DatabaseName As New TwitterTracks.DatabaseAccess.VerbatimIdentifier(ViewModel.AdministratorToolsVM.DatabaseName)
        Dim Tracks As List(Of TwitterTracks.DatabaseAccess.Track) = Nothing
        DoMySqlTaskWithStatusMessage( _
            ViewModel.AdministratorToolsVM.TracksVM.StatusMessageVM, _
            "The Tracks could not be read", _
            Sub()
                Dim TrackDatabase As New TwitterTracks.DatabaseAccess.TrackDatabase(Connection, DatabaseName)
                Tracks = TrackDatabase.GetAllTracksWithoutMetadata.ToList
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    ViewModel.AdministratorToolsVM.TracksVM.AvailableTracks.Clear()
                    ViewModel.AdministratorToolsVM.TracksVM.AvailableTracks.AddRange(Tracks)
                End If
                Return Nothing
            End Function)
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
        DoMySqlTaskWithStatusMessage( _
            ViewModel.AdministratorToolsVM.TracksVM.StatusMessageVM, _
            "The Track could not be deleted", _
            Sub()
                Dim TrackDatabase As New TwitterTracks.DatabaseAccess.TrackDatabase(Connection, New TwitterTracks.DatabaseAccess.VerbatimIdentifier(ViewModel.AdministratorToolsVM.DatabaseName))
                TrackDatabase.DeleteTrack(Track.EntityId)
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

    End Sub

#End Region

End Class
