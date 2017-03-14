Public Class OpenTrackDialog

    Dim ViewModel As OpenTrackDialogViewModel

    Dim Connection As TwitterTracks.DatabaseAccess.DatabaseConnection
    Dim HasMetadata As Boolean = False
    Dim Metadata As TwitterTracks.DatabaseAccess.TrackMetadata

    Dim WithEvents Tasks As New TwitterTracks.Common.UI.Tasks.WindowTaskManager(Me.Dispatcher)
    Private Sub Tasks_IsBusyChanged() Handles Tasks.IsBusyChanged
        ViewModel.IsBusy = Tasks.IsBusy
    End Sub

    Public Sub New()
        InitializeComponent()
        ViewModel = DirectCast(Me.DataContext, OpenTrackDialogViewModel)
    End Sub

    Private Sub CloseOk(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Dim Host = ViewModel.DatabaseConnectionVM.DatabaseHost
        Dim DatabaseName = New TwitterTracks.DatabaseAccess.VerbatimIdentifier(ViewModel.DatabaseConnectionVM.DatabaseName)
        Dim TrackEntityId = New TwitterTracks.DatabaseAccess.EntityId(Int64.Parse(ViewModel.DatabaseConnectionVM.ResearcherIdText))
        Dim UserName = TwitterTracks.DatabaseAccess.Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId)
        Dim Password = ViewModel.DatabaseConnectionVM.Password

        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.StatusMessageVM, _
            "The metadata could not be read from the database.", _
            Sub()
                Connection = TwitterTracks.DatabaseAccess.DatabaseConnection.PlainConnection(Host, UserName, Password)
                Connection.Open()
                Dim Database As New TwitterTracks.DatabaseAccess.ResearcherDatabase(Connection, DatabaseName, TrackEntityId)
                Dim Temp = Database.TryGetTrackMetadata()
                HasMetadata = Temp IsNot Nothing
                If HasMetadata Then
                    Metadata = Temp.Value
                End If
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    If HasMetadata Then
                        If Metadata.IsPublished Then
                            Me.DialogResult = True
                            Me.Close()
                        Else
                            ViewModel.ShowContinueAnywaysButton = True
                            ViewModel.StatusMessageVM.SetStatus("The initial Tweet is not yet published.", Common.UI.StatusMessageKindType.Warning)
                        End If
                    Else
                        ViewModel.ShowContinueAnywaysButton = True
                        ViewModel.StatusMessageVM.SetStatus("The database does not contain any metadata.", Common.UI.StatusMessageKindType.Warning)
                    End If
                End If
                Return Nothing
            End Function)
    End Sub

    Private Sub ContinueAnyways(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Me.DialogResult = True
        Me.Close()
    End Sub

    Private Sub CancelContinueAnyways(sender As System.Object, e As System.Windows.RoutedEventArgs)
        ViewModel.ShowContinueAnywaysButton = False
    End Sub

    Private Sub CloseCancel(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Me.DialogResult = False
        Me.Close()
    End Sub

    Public Function GetOpenTrackInfo() As OpenTrackInformation
        Dim Result As New OpenTrackInformation

        If HasMetadata Then
            Result.Metadata = Metadata
        Else
            'Result.Metadata = TwitterTracks.DatabaseAccess.TrackMetadata.FromUnpublished( _
            '    ViewModel.TweetDataVM.TweetText, ViewModel.KeywordsVM.Keywords.Select(Function(i) i.Text), ViewModel.TweetDataVM.MediasToAdd.Select(Function(i) i.FilePath), _
            '    ViewModel.TwitterConnectionVM.ConsumerKey, ViewModel.TwitterConnectionVM.ConsumerSecret, ViewModel.TwitterConnectionVM.AccessToken, ViewModel.TwitterConnectionVM.AccessTokenSecret)
        End If

        Result.Database.Host = ViewModel.DatabaseConnectionVM.DatabaseHost
        Result.Database.Name = ViewModel.DatabaseConnectionVM.DatabaseName
        Result.Database.ResearcherId = If(ViewModel.DatabaseConnectionVM.ResearcherIdIsValid, Integer.Parse(ViewModel.DatabaseConnectionVM.ResearcherIdText), -1)
        Result.Database.Password = ViewModel.DatabaseConnectionVM.Password
        Result.Database.Connection = Connection

        Return Result
    End Function

End Class
