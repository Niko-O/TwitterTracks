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
        Dim DatabaseName = New TwitterTracks.DatabaseAccess.VerbatimIdentifier(ViewModel.DatabaseConnectionVM.DatabaseNameOrUserName)
        Dim TrackEntityId = New TwitterTracks.DatabaseAccess.EntityId(Int64.Parse(ViewModel.DatabaseConnectionVM.ResearcherIdText))
        Dim UserName = TwitterTracks.DatabaseAccess.Relations.UserNames.ResearcherUserName(DatabaseName, TrackEntityId)
        Dim Password = ViewModel.DatabaseConnectionVM.Password

        Dim MetadataInDatabase As TwitterTracks.DatabaseAccess.TrackMetadata? = Nothing
        Tasks.DoSqlTaskWithStatusMessage( _
            ViewModel.StatusMessageVM, _
            "The metadata could not be read from the database.", _
            Sub()
                Connection = New TwitterTracks.DatabaseAccess.DatabaseConnection(Host, UserName, Password)
                Connection.Open()
                Dim Database As New TwitterTracks.DatabaseAccess.ResearcherDatabase(Connection, DatabaseName, TrackEntityId)
                MetadataInDatabase = Database.TryGetTrackMetadata()
            End Sub, _
            Function(Success As Boolean)
                If Success Then
                    HasMetadata = MetadataInDatabase IsNot Nothing
                    If HasMetadata Then
                        Metadata = MetadataInDatabase.Value
                    End If
                    Me.DialogResult = True
                    Me.Close()
                End If
                Return Nothing
            End Function)
    End Sub

    Private Sub CloseCancel(sender As System.Object, e As System.Windows.RoutedEventArgs)
        Me.DialogResult = False
        Me.Close()
    End Sub

    Public Function GetOpenTrackInfo() As OpenTrackInformation
        Dim Result As New OpenTrackInformation

        If HasMetadata Then
            Result.Metadata = Metadata
        End If

        Result.Database.Host = ViewModel.DatabaseConnectionVM.DatabaseHost
        Result.Database.Name = ViewModel.DatabaseConnectionVM.DatabaseNameOrUserName
        Result.Database.ResearcherId = If(ViewModel.DatabaseConnectionVM.ResearcherIdIsValid, Integer.Parse(ViewModel.DatabaseConnectionVM.ResearcherIdText), -1)
        Result.Database.Password = ViewModel.DatabaseConnectionVM.Password
        Result.Database.Connection = Connection

        Return Result
    End Function

End Class
