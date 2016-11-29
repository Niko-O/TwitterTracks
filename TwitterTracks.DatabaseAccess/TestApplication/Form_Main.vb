
Imports TwitterTracks.DatabaseAccess

Public Class Form_Main

    Public Sub New()
        InitializeComponent()

    End Sub

    Private Sub Foo()
        Using Connection = New MySql.Data.MySqlClient.MySqlConnection(New MySql.Data.MySqlClient.MySqlConnectionStringBuilder() With { _
            .Server = "localhost", _
            .CharacterSet = "utf8", _
            .SslMode = MySql.Data.MySqlClient.MySqlSslMode.Preferred, _
            .Password = "", _
            .UserID = "root"}.ConnectionString)

            Connection.Open()

            Using Command = Connection.CreateCommand
                Command.CommandText = "CREATE USER `Nik``o`@`localhost` IDENTIFIED BY @Password;"
                Command.Prepare()
                'Command.Parameters.AddWithValue("@Name", "Niko")
                Command.Parameters.AddWithValue("@Password", "Geheim")
                Command.ExecuteNonQuery()
            End Using

        End Using
        Dim bp = 0
    End Sub

    Protected Overrides Sub OnShown(e As EventArgs)
        MyBase.OnShown(e)

        Helpers.DebugPrint("")
        Helpers.DebugPrint("")
        Helpers.DebugPrint("New Session -----------------------------------------------")

        DatabaseBase.DebugPrintQueries = True
        Foo()
        Helpers.DebugPrint("Opening Connection")
        Dim Connection = DatabaseConnection.PlainConnection("localhost", "root", "")
        Connection.Open()

        Dim Database As New Database(Connection)
        Dim Accessor As New PublicDatabaseAccessor(Database)
        Dim ExecuteNonQuery = Sub(QueryText As SqlQueryString, Parameters As CommandParameter())
                                  Using Command = Accessor.PrepareCommand(QueryText, Parameters)
                                      Command.ExecuteNonQuery()
                                  End Using
                              End Sub

        'Dim DatabaseName As New VerbatimIdentifier("BobsDatabase")
        'Dim AdministratorName As String = "Sepp" 'Relations.UserNames.AdministratorUserName(DatabaseName)
        'Dim AdministratorHost As String = "localhost" 'Relations.UserNames.AdministratorUserName(DatabaseName)
        'Dim AdministratorPassword As String = "asdf"
        'Dim TrackTableIdentifier = Relations.TableNames.TableIdentifier(DatabaseName.Escape, New VerbatimIdentifier("Track").Escape)

        'ExecuteNonQuery(PublicDatabaseAccessor.FormatSqlIdentifiers("CREATE DATABASE {0}", DatabaseName.Escape), {})

        'ExecuteNonQuery(New SqlQueryString( _
        '    "CREATE TABLE " & TrackTableIdentifier.EscapedText & " ( " & _
        '    "  `Id` INT NOT NULL AUTO_INCREMENT,                     " & _
        '    "  PRIMARY KEY (`Id`))                                   " & _
        '    "ENGINE = InnoDB;                                        "), {})

        'Dim bp = 0


        'ExecuteNonQuery(PublicDatabaseAccessor.FormatSqlIdentifiers("CREATE USER 'Sepp'@'localhost' IDENTIFIED BY @AdministratorPassword;"), _
        '                    {New CommandParameter("@AdministratorName", AdministratorName), _
        '                     New CommandParameter("@AdministratorHost", AdministratorHost), _
        '                     New CommandParameter("@AdministratorPassword", AdministratorPassword)})


        'ExecuteNonQuery(PublicDatabaseAccessor.FormatSqlIdentifiers("GRANT ALL ON `*`.`*` TO @AdministratorName;"), _
        '                    {New CommandParameter("@AdministratorName", AdministratorName)})

        'ExecuteNonQuery(PublicDatabaseAccessor.FormatSqlIdentifiers("GRANT CREATE, DROP ON {0} TO @AdministratorName;", Relations.TableNames.TableIdentifier(DatabaseName.Escape, Relations.WildcardTable)), _
        '                {New CommandParameter("@AdministratorName", AdministratorName)})
        'ExecuteNonQuery(PublicDatabaseAccessor.FormatSqlIdentifiers("GRANT SELECT, INSERT, UPDATE, DELETE ON {0} TO @AdministratorName;", TrackTableIdentifier), _
        '                {New CommandParameter("@AdministratorName", AdministratorName)})
        'ExecuteNonQuery(PublicDatabaseAccessor.FormatSqlIdentifiers("FLUSH PRIVILEGES;"), {})

    End Sub



End Class
