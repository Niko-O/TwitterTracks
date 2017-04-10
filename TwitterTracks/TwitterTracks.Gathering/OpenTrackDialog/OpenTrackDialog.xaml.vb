
Namespace OpenTrackDialog

    Public Class OpenTrackDialog

        Private Shared ReadOnly HashtagRegex As New System.Text.RegularExpressions.Regex("(^|\s)(?<Hashtag>#.*?)(\s|$)", Text.RegularExpressions.RegexOptions.Compiled)

        Dim Connection As TwitterTracks.DatabaseAccess.DatabaseConnection
        Dim DatabaseContainsApplicationToken As Boolean = False
        Dim DatabaseContainsMetadata As Boolean = False
        Dim ExistingApplicationToken As TwitterTracks.DatabaseAccess.ApplicationToken
        Dim ExistingTweetMetadata As TwitterTracks.DatabaseAccess.TrackMetadata

        Dim AuthenticationContext As Tweetinvi.Models.IAuthenticationContext = Nothing
        Dim AuthenticationToken As Tweetinvi.Models.ITwitterCredentials = Nothing

        Dim ViewModel As OpenTrackDialogViewModel
        Dim WithEvents Tasks As New TwitterTracks.Common.UI.Tasks.WindowTaskManager(Me.Dispatcher)
        Private Sub Tasks_IsBusyChanged() Handles Tasks.IsBusyChanged
            ViewModel.IsBusy = Tasks.IsBusy
        End Sub

        Public Sub New()
            InitializeComponent()
            ViewModel = DirectCast(Me.DataContext, OpenTrackDialogViewModel)
        End Sub

        Protected Overrides Sub OnClosing(e As System.ComponentModel.CancelEventArgs)
            MyBase.OnClosing(e)
            ViewModel.TweetDataVM.DisableUpdateMediaExistsTimer()
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

        Private Sub AddKeywordToAdd(sender As System.Object, e As System.Windows.RoutedEventArgs)
            ViewModel.KeywordsVM.Keywords.Add(New KeywordToAdd With {.IsCustom = True})
        End Sub

        Private Sub GoBack(sender As System.Object, e As System.Windows.RoutedEventArgs)
            Select Case ViewModel.CurrentTabIndex
                Case DialogTabIndex.DatabaseConnection
                    Throw New NopeException
                Case DialogTabIndex.TwitterConnection
                    ViewModel.CurrentTabIndex = DialogTabIndex.DatabaseConnection
                Case DialogTabIndex.TweetData
                    ViewModel.CurrentTabIndex = DialogTabIndex.TwitterConnection
                Case DialogTabIndex.Keywords
                    ViewModel.CurrentTabIndex = DialogTabIndex.TweetData
                Case DialogTabIndex.Summary
                    If ViewModel.SummaryVM.TweetAlreadyPublished Then
                        ViewModel.CurrentTabIndex = DialogTabIndex.DatabaseConnection
                    Else
                        ViewModel.CurrentTabIndex = DialogTabIndex.Keywords
                    End If
                Case Else
                    Throw New NopeException
            End Select
        End Sub

        Private Sub CloseOkOrGoForward(sender As System.Object, e As System.Windows.RoutedEventArgs)
            Select Case ViewModel.CurrentTabIndex
                Case DialogTabIndex.DatabaseConnection
                    BeginDatabaseGoForward()
                Case DialogTabIndex.TwitterConnection
                    BeginTwitterConnectionGoForward()
                Case DialogTabIndex.TweetData
                    BeginTweetDataGoForward()
                Case DialogTabIndex.Keywords
                    BeginKeywordsGoForward()
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
            Dim DatabaseName As New TwitterTracks.DatabaseAccess.VerbatimIdentifier(ViewModel.DatabaseConnectionVM.DatabaseNameOrUserName)
            Dim TrackEntityId = New TwitterTracks.DatabaseAccess.EntityId(Int64.Parse(ViewModel.DatabaseConnectionVM.ResearcherIdText))
            Dim ResearcherUserName = TwitterTracks.DatabaseAccess.Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId)
            Dim Host = ViewModel.DatabaseConnectionVM.DatabaseHost
            Dim Password = ViewModel.DatabaseConnectionVM.Password
            Tasks.StartTask(Sub()
                                Dim ResultConnection As New TwitterTracks.DatabaseAccess.DatabaseConnection(Host, ResearcherUserName, Password)
                                Try
                                    ResultConnection.Open()
                                Catch ex As System.Data.Common.DbException
                                    Tasks.FinishTask(Sub() ViewModel.StatusMessageVM.SetStatus("The connection could not be opened:" & Environment.NewLine & TwitterTracks.Common.UI.Tasks.WindowTaskManager.SqlExceptionToErrorMessage(ex), Common.UI.StatusMessageKindType.Error))
                                    Return
                                End Try
                                Dim Database As New TwitterTracks.DatabaseAccess.ResearcherDatabase(ResultConnection, DatabaseName, TrackEntityId)
                                Dim ApplicationToken As TwitterTracks.DatabaseAccess.ApplicationToken?
                                Try
                                    ApplicationToken = Database.TryGetApplicationToken
                                Catch ex As System.Data.Common.DbException
                                    Tasks.FinishTask(Sub() ViewModel.StatusMessageVM.SetStatus("The Application Token could not be read:" & Environment.NewLine & TwitterTracks.Common.UI.Tasks.WindowTaskManager.SqlExceptionToErrorMessage(ex), Common.UI.StatusMessageKindType.Error))
                                    Return
                                End Try
                                Dim Metadata As TwitterTracks.DatabaseAccess.TrackMetadata?
                                Try
                                    Metadata = Database.TryGetTrackMetadata
                                Catch ex As System.Data.Common.DbException
                                    Tasks.FinishTask(Sub() ViewModel.StatusMessageVM.SetStatus("The Track Metadata could not be read:" & Environment.NewLine & TwitterTracks.Common.UI.Tasks.WindowTaskManager.SqlExceptionToErrorMessage(ex), Common.UI.StatusMessageKindType.Error))
                                    Return
                                End Try
                                Dim NewAuthenticationContext As Tweetinvi.Models.IAuthenticationContext = Nothing
                                If ApplicationToken IsNot Nothing AndAlso Metadata Is Nothing Then
                                    Try
                                        NewAuthenticationContext = Tweetinvi.AuthFlow.InitAuthentication(New Tweetinvi.Models.TwitterCredentials(ApplicationToken.Value.ConsumerKey, ApplicationToken.Value.ConsumerSecret))
                                    Catch ex As Tweetinvi.Exceptions.TwitterException
                                        Tasks.FinishTask(Sub() ViewModel.StatusMessageVM.SetStatus("The Application Token is invalid. Please contact your Administrator to check back on the Twitter App." & Environment.NewLine & ex.GetType.Name & ": " & ex.Message, Common.UI.StatusMessageKindType.Error))
                                        Return
                                    End Try
                                    If NewAuthenticationContext Is Nothing Then
                                        Tasks.FinishTask(Sub() ViewModel.StatusMessageVM.SetStatus("The Application Token is invalid. Please contact your Administrator to check back on the Twitter App.", Common.UI.StatusMessageKindType.Error))
                                        Return
                                    End If
                                End If
                                Tasks.FinishTask(Sub()
                                                     Connection = ResultConnection

                                                     DatabaseContainsApplicationToken = ApplicationToken IsNot Nothing
                                                     If Not DatabaseContainsApplicationToken Then
                                                         ViewModel.StatusMessageVM.SetStatus("No Application Token was found in the database. Please contact your Administrator to register a Twitter App with the Setup program.", Common.UI.StatusMessageKindType.Error)
                                                         Return
                                                     End If
                                                     ExistingApplicationToken = ApplicationToken.Value

                                                     DatabaseContainsMetadata = Metadata IsNot Nothing
                                                     If Not DatabaseContainsMetadata Then
                                                         ViewModel.TwitterConnectionVM.PinIsValid = False
                                                         AuthenticationContext = NewAuthenticationContext
                                                         ViewModel.SummaryVM.TweetAlreadyPublished = False
                                                         ViewModel.StatusMessageVM.SetStatus("No metadata was found in the database.", Common.UI.StatusMessageKindType.JustText)
                                                         ViewModel.CurrentTabIndex = DialogTabIndex.TwitterConnection
                                                         Return
                                                     End If
                                                     ExistingTweetMetadata = Metadata.Value
                                                     ViewModel.TwitterConnectionVM.AccessToken = ExistingTweetMetadata.AccessToken
                                                     ViewModel.TwitterConnectionVM.AccessTokenSecret = ExistingTweetMetadata.AccessTokenSecret
                                                     ViewModel.TweetDataVM.TweetText = ExistingTweetMetadata.TweetText
                                                     ViewModel.TweetDataVM.MediasToAdd.Clear()
                                                     ViewModel.TweetDataVM.MediasToAdd.AddRange(ExistingTweetMetadata.MediaFilePathsToAdd.Select(Function(i) New TweetMediaToAdd(i)))
                                                     ViewModel.KeywordsVM.Keywords.Clear()
                                                     ViewModel.KeywordsVM.Keywords.AddRange(ExistingTweetMetadata.RelevantKeywords.Select(Function(i) New KeywordToAdd With {.IsCustom = True, .Text = i}))
                                                     ViewModel.SummaryVM.TweetAlreadyPublished = ExistingTweetMetadata.IsPublished

                                                     If ExistingTweetMetadata.IsPublished Then
                                                         ViewModel.SummaryVM.PublishedTweetId = ExistingTweetMetadata.TweetId
                                                         ViewModel.SummaryVM.TweetText = ExistingTweetMetadata.TweetText
                                                         ViewModel.SummaryVM.Keywords = String.Join(" ", ExistingTweetMetadata.RelevantKeywords)
                                                         AuthenticationToken = New Tweetinvi.Models.TwitterCredentials(ExistingApplicationToken.ConsumerKey, ExistingApplicationToken.ConsumerSecret, ExistingTweetMetadata.AccessToken, ExistingTweetMetadata.AccessTokenSecret)

                                                         ViewModel.StatusMessageVM.SetStatus("Metadata was found in the database. The initial Tweet is already published", Common.UI.StatusMessageKindType.Warning)
                                                         ViewModel.CurrentTabIndex = DialogTabIndex.Summary
                                                     Else
                                                         ViewModel.TwitterConnectionVM.PinIsValid = False
                                                         AuthenticationContext = NewAuthenticationContext
                                                         ViewModel.StatusMessageVM.SetStatus("Metadata was found in the database. The initial Tweet is not yet published", Common.UI.StatusMessageKindType.JustText)
                                                         ViewModel.CurrentTabIndex = DialogTabIndex.TwitterConnection
                                                     End If
                                                 End Sub)
                            End Sub)
        End Sub

        Private Sub OpenAuthenticationPage(sender As System.Object, e As System.Windows.RoutedEventArgs)
            Try
                Using P As New Process
                    P.StartInfo.FileName = AuthenticationContext.AuthorizationURL
                    P.Start()
                End Using
            Catch ex As Exception
                Clipboard.SetText(AuthenticationContext.AuthorizationURL)
                MessageBox.Show("The browser could not be started automatically for some reason. The URL was copied to the clipboard. Please open the website manually.")
            End Try
        End Sub

        Private Sub ValidatePin(sender As System.Object, e As System.Windows.RoutedEventArgs)
            Dim Pin = ViewModel.TwitterConnectionVM.AuthorizationPin
            Dim Context = AuthenticationContext
            Tasks.StartTask(Sub()
                                Dim NewAuthenticationToken As Tweetinvi.Models.ITwitterCredentials = Nothing
                                Try
                                    NewAuthenticationToken = Tweetinvi.AuthFlow.CreateCredentialsFromVerifierCode(Pin, Context)
                                Catch ex As Tweetinvi.Exceptions.TwitterException
                                    Tasks.FinishTask(Sub() ViewModel.StatusMessageVM.SetStatus("The PIN could not be validated. Please make sure there are no typos and that you have indeed authorized the Twitter App." & Environment.NewLine & ex.GetType.Name & ": " & ex.Message, Common.UI.StatusMessageKindType.Error))
                                    Return
                                End Try
                                If NewAuthenticationToken Is Nothing Then
                                    Tasks.FinishTask(Sub() ViewModel.StatusMessageVM.SetStatus("The PIN could not be validated. Please make sure there are no typos and that you have indeed authorized the Twitter App.", Common.UI.StatusMessageKindType.Error))
                                    Return
                                End If
                                Tasks.FinishTask(Sub()
                                                     ViewModel.StatusMessageVM.ClearStatus()
                                                     AuthenticationToken = NewAuthenticationToken
                                                     ViewModel.TwitterConnectionVM.PinIsValid = True
                                                     ViewModel.TwitterConnectionVM.AccessToken = AuthenticationToken.AccessToken
                                                     ViewModel.TwitterConnectionVM.AccessTokenSecret = AuthenticationToken.AccessTokenSecret
                                                 End Sub)
                            End Sub)
        End Sub

        Private Sub BeginTwitterConnectionGoForward()
            ViewModel.CurrentTabIndex = DialogTabIndex.TweetData
        End Sub

        Private Sub BeginTweetDataGoForward()
            ViewModel.SummaryVM.TweetText = ViewModel.TweetDataVM.TweetText
            If DatabaseContainsMetadata AndAlso ExistingTweetMetadata.TweetText = ViewModel.TweetDataVM.TweetText Then
                ViewModel.KeywordsVM.Keywords.Clear()
                ViewModel.KeywordsVM.Keywords.AddRange(ExistingTweetMetadata.RelevantKeywords.Select(Function(i) New KeywordToAdd With {.IsCustom = True, .Text = i}))
            Else
                ViewModel.KeywordsVM.Keywords.Clear()
                Dim Match = HashtagRegex.Match(ViewModel.TweetDataVM.TweetText)
                Do While Match IsNot Nothing AndAlso Match.Success
                    Dim Group = Match.Groups("Hashtag")
                    ViewModel.KeywordsVM.Keywords.Add(New KeywordToAdd With {.Text = Group.Value, .IsCustom = False})
                    Match = HashtagRegex.Match(ViewModel.TweetDataVM.TweetText, Group.Index + Group.Length)
                Loop
            End If
            ViewModel.StatusMessageVM.ClearStatus()
            ViewModel.CurrentTabIndex = DialogTabIndex.Keywords
        End Sub

        Private Sub BeginKeywordsGoForward()
            If Not ViewModel.SummaryVM.TweetAlreadyPublished Then
                ViewModel.SummaryVM.Keywords = String.Join(" ", ViewModel.KeywordsVM.Keywords.Select(Function(i) i.Text))
            End If
            ViewModel.StatusMessageVM.ClearStatus()
            ViewModel.CurrentTabIndex = DialogTabIndex.Summary
        End Sub

        Public Function GetOpenTrackInfo() As OpenTrackInformation
            Dim Result As New OpenTrackInformation

            Result.ApplicationToken = ExistingApplicationToken

            If DatabaseContainsMetadata AndAlso ViewModel.SummaryVM.TweetAlreadyPublished Then
                Result.Metadata = ExistingTweetMetadata
            Else
                Result.Metadata = TwitterTracks.DatabaseAccess.TrackMetadata.FromUnpublished( _
                    ViewModel.TweetDataVM.TweetText, ViewModel.KeywordsVM.Keywords.Select(Function(i) i.Text), ViewModel.TweetDataVM.MediasToAdd.Select(Function(i) i.FilePath), _
                    ViewModel.TwitterConnectionVM.AccessToken, ViewModel.TwitterConnectionVM.AccessTokenSecret)
            End If

            Result.Database.Host = ViewModel.DatabaseConnectionVM.DatabaseHost
            Result.Database.Name = ViewModel.DatabaseConnectionVM.DatabaseNameOrUserName
            Result.Database.ResearcherId = If(ViewModel.DatabaseConnectionVM.ResearcherIdIsValid, Integer.Parse(ViewModel.DatabaseConnectionVM.ResearcherIdText), -1)
            Result.Database.Password = ViewModel.DatabaseConnectionVM.Password
            Result.Database.Connection = Connection

            Return Result
        End Function

    End Class

End Namespace
