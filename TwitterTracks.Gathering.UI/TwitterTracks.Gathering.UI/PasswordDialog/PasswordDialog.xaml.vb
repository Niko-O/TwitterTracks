
Namespace PasswordDialog

    Public Class PasswordDialog

        Public ReadOnly Property Password As String
            Get
                Return PasswordBox.Password
            End Get
        End Property

        Private Sub CloseOk(sender As System.Object, e As System.Windows.RoutedEventArgs)
            Me.DialogResult = True
            Me.Close()
        End Sub

        Private Sub CloseCancel(sender As System.Object, e As System.Windows.RoutedEventArgs)
            Me.DialogResult = False
            Me.Close()
        End Sub

    End Class

End Namespace
