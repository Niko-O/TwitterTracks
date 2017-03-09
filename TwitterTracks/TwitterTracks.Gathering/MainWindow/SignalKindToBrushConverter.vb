Public Class SignalKindToBrushConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If Not TypeOf value Is SignalKind Then
            Return Binding.DoNothing
        End If
        Dim Signal = DirectCast(value, SignalKind)
        Select Case Signal
            Case SignalKind.OK
                Return TwitterTracks.Common.UI.Resources.SuccessBackgroundBrush
            Case SignalKind.Warning
                Return TwitterTracks.Common.UI.Resources.WarningBackgroundBrush
            Case SignalKind.Error
                Return TwitterTracks.Common.UI.Resources.ErrorBackgroundBrush
            Case Else
                Throw New NopeException
        End Select
    End Function

    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Return Binding.DoNothing
    End Function

End Class
