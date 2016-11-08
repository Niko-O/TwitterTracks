Friend Class Relations

    Public Class UserNames

        Public Shared Function AdministratorUserName(DatabaseName As VerbatimIdentifier) As String
            Return String.Format("{0}_Administrator", DatabaseName.UnescapedText)
        End Function

        Public Shared Function ResearcherUserName(DatabaseName As VerbatimIdentifier, TrackEntityId As EntityId) As String
            Return String.Format("{0}_{1}_Researcher", DatabaseName.UnescapedText, TrackEntityId.RawId)
        End Function

    End Class

    Public Class TableNames

        Public Shared Function MetadataTableName(TrackEntityId As EntityId) As VerbatimIdentifier
            Return New VerbatimIdentifier(String.Format("{0}_Metadata", TrackEntityId.RawId))
        End Function

        Public Shared Function TweetTableName(TrackEntityId As EntityId) As VerbatimIdentifier
            Return New VerbatimIdentifier(String.Format("{0}_Tweet", TrackEntityId.RawId))
        End Function

    End Class

End Class
