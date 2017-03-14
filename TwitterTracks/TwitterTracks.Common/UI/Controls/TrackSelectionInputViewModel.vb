
Namespace UI.Controls

    Public Class TrackSelectionInputViewModel
        Inherits ViewModelBase

        Dim _DatabaseHost As String = TwitterTracks.Common.UI.Resources.DebugConstants.DatabaseHost
        Public Property DatabaseHost As String
            <DebuggerStepThrough()>
            Get
                Return _DatabaseHost
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_DatabaseHost, value, "DatabaseHost")
            End Set
        End Property

        Dim _DatabaseName As String = TwitterTracks.Common.UI.Resources.DebugConstants.TrackDatabaseName
        Public Property DatabaseName As String
            <DebuggerStepThrough()>
            Get
                Return _DatabaseName
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_DatabaseName, value, "DatabaseName")
            End Set
        End Property

        Dim _ResearcherIdText As String = TwitterTracks.Common.UI.Resources.DebugConstants.DatabaseResearcherIdText
        Public Property ResearcherIdText As String
            <DebuggerStepThrough()>
            Get
                Return _ResearcherIdText
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_ResearcherIdText, value, "ResearcherIdText")
            End Set
        End Property

        <Dependency("ResearcherIdText")>
        Public ReadOnly Property ResearcherIdIsValid As Boolean
            Get
                Dim Temp As Int64
                Return Not String.IsNullOrWhiteSpace(ResearcherIdText) AndAlso Int64.TryParse(ResearcherIdText, Temp) AndAlso Temp > 0
            End Get
        End Property

        Dim _Password As String = TwitterTracks.Common.UI.Resources.DebugConstants.ResearcherPassword
        Public Property Password As String
            <DebuggerStepThrough()>
            Get
                Return _Password
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_Password, value, "Password")
            End Set
        End Property

        <Dependency("ResearcherIdIsValid")>
        Public ReadOnly Property IsValid As Boolean
            Get
                Return ResearcherIdIsValid
            End Get
        End Property

        Public Sub New()
            MyBase.New(True)
        End Sub

    End Class

End Namespace
