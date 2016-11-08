
Imports System.Runtime.CompilerServices

'<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>
'<DebuggerNonUserCode()>
Friend Module Extensions

    <Extension()>
    Public Function GetEntityId(Target As MySql.Data.MySqlClient.MySqlDataReader, ColumnIndex As Integer) As EntityId
        Return New EntityId(Target.GetInt64(ColumnIndex))
    End Function

    <Extension()>
    Public Function GetEntityId(Target As MySql.Data.MySqlClient.MySqlDataReader, ColumnName As String) As EntityId
        Return New EntityId(Target.GetInt64(ColumnName))
    End Function

End Module
