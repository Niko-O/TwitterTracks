
Imports Sql = MySql.Data.MySqlClient

''' <summary>
''' For debug-purposes only.
''' </summary>
Public Class PublicDatabaseAccessor

    Dim _Database As DatabaseBase
    Public ReadOnly Property Database As DatabaseBase
        <DebuggerStepThrough()>
        Get
            Return _Database
        End Get
    End Property

    Public Sub New(NewDatabase As DatabaseBase)
        _Database = NewDatabase
    End Sub

    Public Shared Function FormatSqlIdentifiers(Format As String, ParamArray EscapedArguments As EscapedIdentifier()) As SqlQueryString
        Return DatabaseBase.FormatSqlIdentifiers(Format, EscapedArguments)
    End Function

    Public Function PrepareCommand(QueryText As SqlQueryString, ParamArray Parameters As CommandParameter()) As Sql.MySqlCommand
        Return _Database.PrepareCommand(QueryText, Parameters)
    End Function

End Class
