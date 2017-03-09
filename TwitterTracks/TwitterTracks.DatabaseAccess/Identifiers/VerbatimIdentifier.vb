
<DebuggerDisplay("VerbatimIdentifier: {UnescapedText}")>
Public Class VerbatimIdentifier
    Implements IEquatable(Of VerbatimIdentifier)

    Dim _UnescapedText As String
    Public ReadOnly Property UnescapedText As String
        <DebuggerStepThrough()>
        Get
            Return _UnescapedText
        End Get
    End Property

    Public Sub New(NewUnescapedText As String)
        _UnescapedText = NewUnescapedText
    End Sub

    Public Function Escape() As EscapedIdentifier
        Static Result As New EscapedIdentifier(EscapeString(UnescapedText))
        Return Result
    End Function

#Region "Comparison"

    Public Overrides Function GetHashCode() As Integer
        Return UnescapedText.GetHashCode
    End Function

    Public Overloads Function Equals(other As VerbatimIdentifier) As Boolean Implements System.IEquatable(Of VerbatimIdentifier).Equals
        Return Me.UnescapedText = other.UnescapedText
    End Function

    Public Overrides Function Equals(obj As Object) As Boolean
        If Not TypeOf obj Is VerbatimIdentifier Then
            Return False
        End If
        Return Me.Equals(DirectCast(obj, VerbatimIdentifier))
    End Function

    Public Shared Operator =(Left As VerbatimIdentifier, Right As VerbatimIdentifier) As Boolean
        Return Left.Equals(Right)
    End Operator

    Public Shared Operator <>(Left As VerbatimIdentifier, Right As VerbatimIdentifier) As Boolean
        Return Not Left.Equals(Right)
    End Operator

#End Region

#Region "Shared"

    Public Const QuoteCharacter As Char = "`"c

    ''' <summary>
    ''' Escapes an identifier according to http://dev.mysql.com/doc/refman/5.7/en/identifiers.html
    ''' This function will always return a quoted string, quoted with <see cref="QuoteCharacter"/> and it assumes that <paramref name="Unescaped"/> is not quoted.
    ''' Since all Unicode characters in the BMP (except the 0-character, see below) are supported, this function simply doubles quotes in <paramref name="Unescaped"/>, puts quotes around it and returns the resulting string.
    ''' Since the 0-character is not supported and it cannot be escaped, this function throws an <see cref="ArgumentException"/> if <paramref name="Unescaped"/> contains any of those.
    ''' </summary>
    ''' <param name="Unescaped">The unescaped, unquoted identifier which must not contain 0-characters.</param>
    Public Shared Function EscapeString(Unescaped As String) As String
        If Unescaped Is Nothing Then
            Return QuoteCharacter & QuoteCharacter
        End If
        If Unescaped.Contains(Microsoft.VisualBasic.ControlChars.NullChar) Then
            Throw New ArgumentException("Null Characters are not valid in identifiers and they cannot be escaped.")
        End If
        Return QuoteCharacter & Unescaped.Replace(QuoteCharacter, QuoteCharacter & QuoteCharacter) & QuoteCharacter
    End Function

#End Region

End Class
