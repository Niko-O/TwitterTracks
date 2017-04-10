
Imports System.Threading.Tasks

Namespace UI.Tasks

    Public Class WindowTaskManager
        Inherits NotifyPropertyChanged

        Public Event IsBusyChanged()

        Dim _IsBusy As Boolean = False
        Public Property IsBusy As Boolean
            <DebuggerStepThrough()>
            Get
                Return _IsBusy
            End Get
            Set(value As Boolean)
                If ExtendedChangeIfDifferent(_IsBusy, value, "IsBusy") Then
                    RaiseEvent IsBusyChanged()
                End If
            End Set
        End Property

        Dim Dispatcher As System.Windows.Threading.Dispatcher
        Dim CurrentTask As Task = Nothing

        Public Sub New(NewDispatcher As System.Windows.Threading.Dispatcher)
            Dispatcher = NewDispatcher
        End Sub

        Public Sub StartTask(BackgroundThreadMethod As Action)
            If CurrentTask IsNot Nothing Then
                Throw New NopeException("There is already a task running.")
            End If
            IsBusy = True
            CurrentTask = Task.Factory.StartNew(Sub()
                                                    Try
                                                        BackgroundThreadMethod()
                                                    Catch ex As Exception When Not Debugger.IsAttached
                                                        Dispatcher.BeginInvoke(Sub() Throw New UnhandledTaskException(ex))
                                                    End Try
                                                End Sub)
        End Sub

        Public Sub FinishTask(GuiThreadMethod As Action, Optional FollowupMethod As Action = Nothing)
            Dim A = Sub()
                        If CurrentTask Is Nothing Then
                            Throw New NopeException("There is currently no task running.")
                        End If
                        GuiThreadMethod()
                        CurrentTask = Nothing
                        IsBusy = False
                        If FollowupMethod IsNot Nothing Then
                            FollowupMethod()
                        End If
                    End Sub
            Dispatcher.BeginInvoke(A)
        End Sub

        Public Sub DoSqlTaskWithStatusMessage(StatusMessageVM As StatusMessageViewModel, _
                                              ErrorDescriptor As String, _
                                              BackgroundThreadMethod As Action, _
                                              GuiThreadMethodOnFinish As Func(Of Boolean, Tuple(Of String, StatusMessageKindType)), _
                                              Optional GuiThreadMethodOnEnd As Action(Of Boolean) = Nothing)
            StartTask(Sub()
                          Dim ErrorException As System.Data.Common.DbException = Nothing
                          Try
                              BackgroundThreadMethod()
                          Catch ex As System.Data.Common.DbException
                              ErrorException = ex
                          End Try
                          Dim Success = ErrorException Is Nothing
                          FinishTask(Sub()
                                         Dim ResultMessageOverride = GuiThreadMethodOnFinish(Success)
                                         If ErrorException Is Nothing Then
                                             If ResultMessageOverride Is Nothing Then
                                                 StatusMessageVM.ClearStatus()
                                             Else
                                                 StatusMessageVM.SetStatus(If(ResultMessageOverride.Item2 = StatusMessageKindType.Error, ErrorDescriptor, "") & ":" & Environment.NewLine & ResultMessageOverride.Item1, ResultMessageOverride.Item2)
                                             End If
                                         Else
                                             Dim ErrorMessage = ErrorDescriptor & ":" & Environment.NewLine & SqlExceptionToErrorMessage(ErrorException)
                                             If ResultMessageOverride IsNot Nothing AndAlso ResultMessageOverride.Item1 IsNot Nothing Then
                                                 ErrorMessage &= Environment.NewLine & "Additional information: " & ResultMessageOverride.Item1
                                             End If
                                             StatusMessageVM.SetStatus(ErrorMessage, StatusMessageKindType.Error)
                                         End If
                                     End Sub, _
                                     If(GuiThreadMethodOnEnd Is Nothing, Nothing, New Action(Sub() GuiThreadMethodOnEnd(Success))))
                      End Sub)
        End Sub

        Public Shared Function SqlExceptionToErrorMessage(ErrorException As System.Data.Common.DbException) As String
            Return ErrorException.GetType.Name & " (Error Code " & ErrorException.ErrorCode & "): " & ErrorException.Message
        End Function

        Public Shared Function ExceptionToErrorMessage(ErrorException As Exception) As String
            Return ErrorException.GetType.Name & ": " & ErrorException.Message
        End Function

    End Class

End Namespace
