
#Const UseDebugValues = False

Public Class DebugConstants

#If UseDebugValues Then
    Public Const AdministratorDatabaseIsSelected As Boolean = False
    Public Const MainWindowIsBusy As Boolean = False
    Public Shared ReadOnly MainWindowIsConnected As Boolean = ViewModelBase.IsInDesignMode
#Else
    Public Const AdministratorDatabaseIsSelected As Boolean = False
    Public Const MainWindowIsBusy As Boolean = False
    Public Const MainWindowIsConnected As Boolean = False
#End If

End Class
