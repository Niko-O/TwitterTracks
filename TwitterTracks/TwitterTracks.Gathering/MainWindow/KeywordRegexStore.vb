
Imports System.Text.RegularExpressions

Public Class KeywordRegexStore

    Dim Cache As New Dictionary(Of String, Regex)

    Public Function GetRegex(Keyword As String) As Regex
        Return Cache.GetOrNew(Keyword, AddressOf CreateNewRegex)
    End Function

    Private Shared Function CreateNewRegex(Keyword As String) As Regex
        Return New Regex(GetPattern(Keyword), RegexOptions.Compiled Or RegexOptions.CultureInvariant Or RegexOptions.IgnoreCase)
    End Function

    Private Shared Function GetPattern(Keyword As String) As String
        Return "(^|\s)" & Regex.Escape(Keyword) & "($|\s)"
    End Function

End Class
