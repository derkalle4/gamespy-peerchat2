Public Class JoinPacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        If Me.Params.Length < 1 Then Return IGNORE_SYNTAX_ERR
        Dim channelName As String = Me.Params(0)

        Logger.Log("'{0}' joined {1}", LogLevel.Verbose, Me.client.NickName, channelName)
        Me.client.CurrentChannel = New IRCChannel()
        'TODO: implement channel list
        Me.client.CurrentChannel.ChannelName = "#GSP!swempire"
        Me.client.SendPacket(Me)
        Dim msgp As New MessagePacket(Me.client)
        Me.client.SendPacket(msgp)

        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Me.Type = CommandType.user
        Me.ResponseCommand = "JOIN"
        Return ":" & Me.client.CurrentChannel.ChannelName
    End Function

End Class
