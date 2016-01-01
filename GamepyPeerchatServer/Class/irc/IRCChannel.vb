Public Class IRCChannel
    Public Property ChannelName As String = String.Empty
    Public Property Topic As String = String.Empty
    Public Property Id As Integer = -1
    Public Property Key As String
    Public Property HostId As Integer = -1

    Dim dfs As DynamicFieldStorage

    Public ReadOnly Property ChanParams As DynamicFieldStorage
        Get
            If Me.dfs Is Nothing Then
                Me.dfs = New DynamicFieldStorage()
                Me.dfs.ParseParameterString(Me.Key)
            End If
            Return Me.dfs
        End Get
    End Property

End Class