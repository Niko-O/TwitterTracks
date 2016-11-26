﻿
#Const UseDebugValues = True And (Config = "Debug")

Public Class DebugConstants

#If UseDebugValues Then
    Public Const DatabaseName As String = "Bob's Database"
    Public Const AdministratorDatabaseIsSelected As Boolean = False
    Public Const MainWindowIsBusy As Boolean = False
    Public Shared ReadOnly MainWindowIsConnected As Boolean = ViewModelBase.IsInDesignMode
#Else
    Public Const DatabaseName As String = ""
    Public Shared ReadOnly AdministratorDatabaseIsSelected As Boolean = ViewModelBase.IsInDesignMode
    Public Const MainWindowIsBusy As Boolean = False
    Public Shared ReadOnly MainWindowIsConnected As Boolean = ViewModelBase.IsInDesignMode
#End If

End Class
