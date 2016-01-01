Public Class IRCUser
    Public Property UserName As String
    Public Property NickName As String
    Public Property Key As String
    Public Property ID As Integer = -1
    Public Property GameSpyId As Integer = -1
    Public Property KeyHash As String = String.Empty
    Public Property MasterServer As Integer = -1
    Public Property Channel As IRCChannel

    Dim dfs As DynamicFieldStorage

    Public ReadOnly Property UserParams As DynamicFieldStorage
        Get
            If Me.dfs Is Nothing Then
                Me.dfs = New DynamicFieldStorage()
                Me.dfs.ParseParameterString(Me.Key)
            End If
            Return Me.dfs
        End Get
    End Property


End Class