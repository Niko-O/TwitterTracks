
Imports System.Runtime.CompilerServices

'<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
'<DebuggerNonUserCode()>
Friend Module Extensions

    <Extension()>
    Public Function GetEntityId(Target As MySql.Data.MySqlClient.MySqlDataReader, ColumnName As String) As EntityId
        Return New EntityId(Target.GetInt64(ColumnName))
    End Function

    <Extension()>
    Public Function GetNullableString(Target As MySql.Data.MySqlClient.MySqlDataReader, ColumnName As String) As String
        If Target.IsDBNull(Target.GetOrdinal(ColumnName)) Then
            Return Nothing
        End If
        Return Target.GetString(ColumnName)
    End Function

    <Extension()>
    Public Function GetNullableInt64(Target As MySql.Data.MySqlClient.MySqlDataReader, ColumnName As String) As Int64?
        If Target.IsDBNull(Target.GetOrdinal(ColumnName)) Then
            Return Nothing
        End If
        Return Target.GetInt64(ColumnName)
    End Function

    <Extension()>
    Public Function GetNullableDouble(Target As MySql.Data.MySqlClient.MySqlDataReader, ColumnName As String) As Double?
        If Target.IsDBNull(Target.GetOrdinal(ColumnName)) Then
            Return Nothing
        End If
        Return Target.GetDouble(ColumnName)
    End Function

End Module
