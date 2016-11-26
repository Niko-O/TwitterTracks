Public Class MainWindowViewModel_TweetData
    Inherits ViewModelBase

    Dim _TweetText As String = ""
    Public Property TweetText As String
        <DebuggerStepThrough()>
        Get
            Return _TweetText
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_TweetText, value, "TweetText")
        End Set
    End Property

    <Dependency("TweetText")>
    Public ReadOnly Property TweetTextIsValid As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(TweetText)
        End Get
    End Property

    Dim WithEvents _MediasToAdd As New TypedObservableCollection(Of TweetMediaToAdd)
    Public ReadOnly Property MediasToAdd As TypedObservableCollection(Of TweetMediaToAdd)
        <DebuggerStepThrough()>
        Get
            Return _MediasToAdd
        End Get
    End Property
    Private Sub MediasToAdd_ItemAdded(sender As Object, e As ItemEventArgs(Of TweetMediaToAdd)) Handles _MediasToAdd.ItemAdded
        AddHandler e.Item.Remove, AddressOf Medias_Item_Remove
    End Sub
    Private Sub MediasToAdd_ItemRemoved(sender As Object, e As ItemEventArgs(Of TweetMediaToAdd)) Handles _MediasToAdd.ItemRemoved
        RemoveHandler e.Item.Remove, AddressOf Medias_Item_Remove
    End Sub
    Private Sub MediasToAdd_TypedCollectionChanged(sender As Object, e As EventArgs) Handles _MediasToAdd.TypedCollectionChanged
        OnPropertyChanged("TooManyMediasToAdd")
    End Sub
    Private Sub Medias_Item_Remove(Sender As TweetMediaToAdd)
        MediasToAdd.Remove(Sender)
    End Sub

    Public ReadOnly Property TooManyMediasToAdd As Boolean
        Get
            Return MediasToAdd.Count > 4
        End Get
    End Property

    <Dependency("TweetTextIsValid")>
    Public ReadOnly Property IsValid As Boolean
        Get
            Return TweetTextIsValid
        End Get
    End Property

    Public Sub New()
        If IsInDesignMode Then
            MediasToAdd.Add(New TweetMediaToAdd("C:\Foo.png"))
            MediasToAdd.Add(New TweetMediaToAdd("C:\Bar.png"))
            MediasToAdd.Add(New TweetMediaToAdd("C:\Baz.png"))
        End If
    End Sub

End Class
