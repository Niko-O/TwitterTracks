Public Interface ITweet

    ReadOnly Property Id As Int64
    ReadOnly Property CreatedByUserId As Int64
    ReadOnly Property Text As String
    ReadOnly Property PublishDateTime As DateTime
    ReadOnly Property Coordinates As String 'ToDo

End Interface
