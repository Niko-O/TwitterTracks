
Namespace OpenTrackDialog

    Public Class DialogTabIndexToIntConverter
        Implements IValueConverter

        Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
            If ViewModelBase.IsInDesignMode Then
                Return Binding.DoNothing
            End If
            If Not TypeOf value Is DialogTabIndex Then
                Return Binding.DoNothing
            End If
            Return DirectCast(DirectCast(value, DialogTabIndex), Integer)
        End Function

        Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
            If Not TypeOf value Is Integer Then
                Return Binding.DoNothing
            End If
            Return DirectCast(DirectCast(value, Integer), DialogTabIndex)
        End Function

    End Class

End Namespace
