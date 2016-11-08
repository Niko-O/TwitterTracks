Public Class MainWindowViewModel_RootTools_Databases
    Inherits ViewModelBase

    Dim WithEvents _AvailableDatabases As New TypedObservableCollection(Of String)
    Public ReadOnly Property AvailableDatabases As TypedObservableCollection(Of String)
        <DebuggerStepThrough()>
        Get
            Return _AvailableDatabases
        End Get
    End Property

    Dim _SelectedAvailableDatabase As String = Nothing
    Public Property SelectedAvailableDatabase As String
        <DebuggerStepThrough()>
        Get
            Return _SelectedAvailableDatabase
        End Get
        Set(value As String)
            ExtendedChangeIfDifferent(_SelectedAvailableDatabase, value, "SelectedAvailableDatabase")
        End Set
    End Property

    Dim _StatusMessageVM As New StatusMessageViewModel
    Public ReadOnly Property StatusMessageVM As StatusMessageViewModel
        <DebuggerStepThrough()>
        Get
            Return _StatusMessageVM
        End Get
    End Property

    Public Sub New()
        If IsInDesignMode Then
            For i = 1 To 6
                AvailableDatabases.Add("DesignTimeDatabase" & i)
            Next
            SelectedAvailableDatabase = AvailableDatabases(0)
        End If
    End Sub

End Class
