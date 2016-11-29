
Namespace OpenTrackDialog

    Public Class OpenTrackDialog

        Dim Connection As TwitterTracks.DatabaseAccess.DatabaseConnection
        Dim ExistingTweetMetadata As TwitterTracks.DatabaseAccess.TrackMetadata
        
        Dim ViewModel As OpenTrackDialogViewModel
        Dim WithEvents Tasks As New TwitterTracks.Common.UI.Tasks.WindowTaskManager(Me.Dispatcher)
        Private Sub Tasks_IsBusyChanged() Handles Tasks.IsBusyChanged
            ViewModel.IsBusy = Tasks.IsBusy
        End Sub

        Public Sub New()
            InitializeComponent()
            ViewModel = DirectCast(Me.DataContext, OpenTrackDialogViewModel)
        End Sub

        Private Sub AddMediaToAdd(sender As System.Object, e As System.Windows.RoutedEventArgs)
            Dim Dlg As New Microsoft.Win32.OpenFileDialog With {.CheckFileExists = True, .CheckPathExists = True, .Multiselect = True, .RestoreDirectory = True, .Title = "Select a file to attach to the Tweet"}
            If Dlg.ShowDialog Then
                For Each i In Dlg.FileNames
                    Dim i2 = i
                    If Not ViewModel.TweetDataVM.MediasToAdd.Any(Function(p) p.FilePath = i2) Then
                        ViewModel.TweetDataVM.MediasToAdd.Add(New TweetMediaToAdd(i))
                    End If
                Next
            End If
        End Sub

        Private Sub GoBack(sender As System.Object, e As System.Windows.RoutedEventArgs)
            Select Case ViewModel.CurrentTabIndex
                Case DialogTabIndex.DatabaseConnection
                    Throw New NopeException
                Case DialogTabIndex.TweetData
                    ViewModel.CurrentTabIndex = DialogTabIndex.DatabaseConnection
                Case DialogTabIndex.TwitterConnection
                    If ViewModel.SummaryVM.TweetAlreadyPublished Then
                        ViewModel.CurrentTabIndex = DialogTabIndex.DatabaseConnection
                    Else
                        ViewModel.CurrentTabIndex = DialogTabIndex.TweetData
                    End If
                Case DialogTabIndex.Summary
                    ViewModel.CurrentTabIndex = DialogTabIndex.TwitterConnection
                Case Else
                    Throw New NopeException
            End Select
        End Sub

        Private Sub CloseOkOrGoForward(sender As System.Object, e As System.Windows.RoutedEventArgs)
            Select Case ViewModel.CurrentTabIndex
                Case DialogTabIndex.DatabaseConnection
                    BeginDatabaseGoForward()
                Case DialogTabIndex.TweetData
                    BeginTweetDataGoForward()
                Case DialogTabIndex.TwitterConnection
                    BeginTwitterConnectionGoForward()
                Case DialogTabIndex.Summary
                    Me.DialogResult = True
                    Me.Close()
                Case Else
                    Throw New NopeException
            End Select
        End Sub

        Private Sub CloseCancel(sender As System.Object, e As System.Windows.RoutedEventArgs)
            Me.DialogResult = False
            Me.Close()
        End Sub

        Private Sub BeginDatabaseGoForward()
            Tasks.StartTask(Sub()
                                Dim DatabaseName As New TwitterTracks.DatabaseAccess.VerbatimIdentifier(ViewModel.DatabaseConnectionVM.DatabaseName)
                                Dim TrackEntityId = New TwitterTracks.DatabaseAccess.EntityId(Int64.Parse(ViewModel.DatabaseConnectionVM.ResearcherIdText))
                                Dim ResearcherUserName = TwitterTracks.DatabaseAccess.Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId)
                                Dim ResultConnection = TwitterTracks.DatabaseAccess.DatabaseConnection.PlainConnection( _
                                    ViewModel.DatabaseConnectionVM.DatabaseHost, _
                                    ResearcherUserName, _
                                    ViewModel.DatabaseConnectionVM.Password)
                                Try
                                    ResultConnection.Open()
                                Catch ex As MySql.Data.MySqlClient.MySqlException
                                    Tasks.FinishTask(Sub() ViewModel.StatusMessageVM.SetStatus("The connection could not be opened:" & Environment.NewLine & TwitterTracks.Common.UI.Tasks.WindowTaskManager.MySqlExceptionToErrorMessage(ex), Common.UI.StatusMessageKindType.Error))
                                    Return
                                End Try
                                Dim Database As New TwitterTracks.DatabaseAccess.ResearcherDatabase(ResultConnection, DatabaseName, TrackEntityId)
                                Dim Metadata As TwitterTracks.DatabaseAccess.TrackMetadata?
                                Try
                                    Metadata = Database.TryGetTrackMetadata
                                Catch ex As MySql.Data.MySqlClient.MySqlException
                                    Tasks.FinishTask(Sub() ViewModel.StatusMessageVM.SetStatus("The Track Metadata could not be read:" & Environment.NewLine & TwitterTracks.Common.UI.Tasks.WindowTaskManager.MySqlExceptionToErrorMessage(ex), Common.UI.StatusMessageKindType.Error))
                                    Return
                                End Try
                                Tasks.FinishTask(Sub()
                                                     Connection = ResultConnection
                                                     ViewModel.SummaryVM.TweetAlreadyPublished = Metadata IsNot Nothing
                                                     If ViewModel.SummaryVM.TweetAlreadyPublished Then
                                                         ExistingTweetMetadata = Metadata.Value
                                                         ViewModel.SummaryVM.PublishedTweetId = ExistingTweetMetadata.InitialTweetId
                                                         ViewModel.SummaryVM.TweetText = ExistingTweetMetadata.InitialTweetFullText
                                                         ViewModel.StatusMessageVM.SetStatus("The initial Tweet already exists in the database.", Common.UI.StatusMessageKindType.JustText)
                                                         ViewModel.CurrentTabIndex = DialogTabIndex.TwitterConnection
                                                     Else
                                                         ViewModel.StatusMessageVM.SetStatus("The initial Tweet does not yet exist in the database.", Common.UI.StatusMessageKindType.JustText)
                                                         ViewModel.CurrentTabIndex = DialogTabIndex.TweetData
                                                     End If
                                                 End Sub)
                            End Sub)
        End Sub

        Private Sub BeginTweetDataGoForward()
            If Not ViewModel.SummaryVM.TweetAlreadyPublished Then
                ViewModel.SummaryVM.TweetText = ViewModel.TweetDataVM.TweetText
            End If
            ViewModel.StatusMessageVM.ClearStatus()
            ViewModel.CurrentTabIndex = DialogTabIndex.TwitterConnection
        End Sub

        Private Sub BeginTwitterConnectionGoForward()
            Dim Token As New TwitterTracks.TweetinviInterop.AuthenticationToken(ViewModel.TwitterConnectionVM.ConsumerKey, ViewModel.TwitterConnectionVM.ConsumerSecret, ViewModel.TwitterConnectionVM.AccessToken, ViewModel.TwitterConnectionVM.AccessTokenSecret)
            Tasks.StartTask(Sub()
                                Dim ValidationResult = TwitterTracks.TweetinviInterop.ServiceProvider.Service.ValidateAuthenticationToken(Token)
                                If ValidationResult.IsValid Then
                                    Tasks.FinishTask(Sub()
                                                         ViewModel.StatusMessageVM.ClearStatus()
                                                         ViewModel.CurrentTabIndex = DialogTabIndex.Summary
                                                     End Sub)
                                Else
                                    Tasks.FinishTask(Sub() ViewModel.StatusMessageVM.SetStatus(ValidationResult.ErrorMessage, Common.UI.StatusMessageKindType.Error))
                                End If
                            End Sub)
        End Sub

    End Class

End Namespace
