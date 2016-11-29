Public Class Relations

    Public Const RelationSeparationCharacter As Char = "~"c
    Public Shared ReadOnly WildcardTable As New EscapedIdentifier("*")

    Public Class UserNames

        Public Shared Function UserIdentifier(UserName As EscapedIdentifier, Host As EscapedIdentifier) As EscapedIdentifier
            Return New EscapedIdentifier(String.Format("{0}@{1}", UserName.EscapedText, Host.EscapedText))
        End Function

        Public Shared Function AdministratorUserName(DatabaseName As VerbatimIdentifier) As String
            Return String.Format("{1}{0}Administrator", RelationSeparationCharacter, DatabaseName.UnescapedText)
        End Function

        Public Shared Function ResearcherUserName(DatabaseName As VerbatimIdentifier, TrackEntityId As EntityId) As String
            Return String.Format("{1}{0}{2}{0}Researcher", RelationSeparationCharacter, DatabaseName.UnescapedText, TrackEntityId.RawId)
        End Function

    End Class

    Public Class TableNames

        Public Shared Function TableIdentifier(DatabaseName As EscapedIdentifier, TableName As EscapedIdentifier) As EscapedIdentifier
            Return New EscapedIdentifier(String.Format("{0}.{1}", DatabaseName.EscapedText, TableName.EscapedText))
        End Function

        Public Shared Function MetadataTableName(TrackEntityId As EntityId) As VerbatimIdentifier
            Return New VerbatimIdentifier(String.Format("{1}{0}Metadata", RelationSeparationCharacter, TrackEntityId.RawId))
        End Function

        Public Shared Function TweetTableName(TrackEntityId As EntityId) As VerbatimIdentifier
            Return New VerbatimIdentifier(String.Format("{1}{0}Tweet", RelationSeparationCharacter, TrackEntityId.RawId))
        End Function

    End Class

End Class
