
#Const UseDebugValues = True And (Config = "Debug")

Public Class DebugConstants

#If UseDebugValues Then
    Public Const DatabaseName As String = "Bob's Database"
    Public Const AdministratorDatabaseIsSelected As Boolean = False
    Public Const DatabaseHost As String = "localhost"
    Public Const DatabaseUserName As String = "root" '"TestDatabase01_1_Researcher"
    Public Const DatabasePassword As String = ""
    Public Const StatusMessage As String = "This is a StatusMessage This is a StatusMessage This is a StatusMessage" & Microsoft.VisualBasic.ControlChars.CrLf & "This is a StatusMessage"
    Public Const MainWindowIsBusy As Boolean = False
    Public Shared ReadOnly MainWindowIsConnected As Boolean = ViewModelBase.IsInDesignMode
#Else
    Public Const DatabaseName As String = ""
    Public Shared ReadOnly AdministratorDatabaseIsSelected As Boolean = ViewModelBase.IsInDesignMode
    Public Const DatabaseHost As String = ""
    Public Const DatabaseUserName As String = ""
    Public Const DatabasePassword As String = ""
    Public Const StatusMessage As String = ""
    Public Const MainWindowIsBusy As Boolean = False
    Public Shared ReadOnly MainWindowIsConnected As Boolean = ViewModelBase.IsInDesignMode
#End If

End Class
