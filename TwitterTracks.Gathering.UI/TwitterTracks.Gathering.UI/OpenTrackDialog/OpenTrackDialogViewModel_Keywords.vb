
Namespace OpenTrackDialog

    Public Class OpenTrackDialogViewModel_Keywords
        Inherits ViewModelBase

        Dim WithEvents _Keywords As New TypedObservableCollection(Of KeywordToAdd)
        Public ReadOnly Property Keywords As TypedObservableCollection(Of KeywordToAdd)
            <DebuggerStepThrough()>
            Get
                Return _Keywords
            End Get
        End Property
        Private Sub Keywords_ItemAdded(sender As Object, e As ItemEventArgs(Of KeywordToAdd)) Handles _Keywords.ItemAdded
            AddHandler e.Item.Remove, AddressOf Keywords_Item_Remove
        End Sub
        Private Sub Keywords_ItemRemoved(sender As Object, e As ItemEventArgs(Of KeywordToAdd)) Handles _Keywords.ItemRemoved
            RemoveHandler e.Item.Remove, AddressOf Keywords_Item_Remove
        End Sub
        Private Sub Keywords_Item_Remove(Sender As KeywordToAdd)
            Keywords.Remove(Sender)
        End Sub

        Public ReadOnly Property IsValid As Boolean
            Get
                Return True
            End Get
        End Property

        Public Sub New()
            If IsInDesignMode Then
                Keywords.Add(New KeywordToAdd With {.Text = "#Test", .IsCustom = False})
                Keywords.Add(New KeywordToAdd With {.Text = "#Baum", .IsCustom = True})
                Keywords.Add(New KeywordToAdd With {.Text = "Gurke", .IsCustom = True})
            End If
        End Sub

    End Class

End Namespace
