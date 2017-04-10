
Namespace UI.Controls

    Public Class TrackSelectionInputViewModel
        Inherits ViewModelBase

        Dim _ShowResearcherIdInput As Boolean
        Public ReadOnly Property ShowResearcherIdInput As Boolean
            <DebuggerStepThrough()>
            Get
                Return _ShowResearcherIdInput
            End Get
        End Property

        Dim _TrackDatabaseNameIsUsedAsUserNameInstead As Boolean
        Public ReadOnly Property TrackDatabaseNameIsUsedAsUserNameInstead As Boolean
            <DebuggerStepThrough()>
            Get
                Return _TrackDatabaseNameIsUsedAsUserNameInstead
            End Get
        End Property

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

        Dim _DatabaseNameOrUserName As String = TwitterTracks.Common.UI.Resources.DebugConstants.TrackDatabaseName
        Public Property DatabaseNameOrUserName As String
            <DebuggerStepThrough()>
            Get
                Return _DatabaseNameOrUserName
            End Get
            Set(value As String)
                ExtendedChangeIfDifferent(_DatabaseNameOrUserName, value, "DatabaseNameOrUserName")
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
                If Not ShowResearcherIdInput Then
                    Return True
                End If
                Dim Temp As Int64
                Return Not String.IsNullOrWhiteSpace(ResearcherIdText) AndAlso _
                       Int64.TryParse(ResearcherIdText, Temp) AndAlso _
                       Temp > 0
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

        Public Sub New(NewShowResearcherIdInput As Boolean, NewTrackDatabaseNameIsUsedAsUserNameInstead As Boolean)
            MyBase.New(True)
            _ShowResearcherIdInput = NewShowResearcherIdInput
            _TrackDatabaseNameIsUsedAsUserNameInstead = NewTrackDatabaseNameIsUsedAsUserNameInstead
        End Sub

        Public Sub New()
            Me.New(True, False)
        End Sub

    End Class

End Namespace
