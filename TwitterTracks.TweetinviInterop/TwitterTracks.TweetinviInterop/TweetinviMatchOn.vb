Public Enum TweetinviMatchOn As Integer
    None = 0
    Everything = 1
    TweetText = 2
    Follower = 4
    TweetLocation = 8
    FollowerInReplyTo = 16
    AllEntities = 32
    URLEntities = 64
    HashTagEntities = 128
    UserMentionEntities = 256
End Enum
