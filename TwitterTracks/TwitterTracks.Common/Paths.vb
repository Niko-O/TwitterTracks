Public Class Paths

    Public Shared ReadOnly ConfigurationDirectoryPath As String = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TwitterTracks")
    Public Shared ReadOnly GatheringConfigurationFilePath As String = System.IO.Path.Combine(ConfigurationDirectoryPath, "GatheringConfiguration.xml")

End Class
