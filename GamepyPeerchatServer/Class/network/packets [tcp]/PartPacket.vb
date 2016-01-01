Public Class PartPacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub
    Private channelName As String
    Public Overrides Function ManageData() As Boolean
        If Me.Params.Length < 1 Then Return IGNORE_SYNTAX_ERR
        Me.channelName = Me.Params(0)
        Logger.Log("'{0}' left {1}", LogLevel.Verbose, Me.client.NickName, channelName)

        Me.client.CurrentChannel.ChannelName = Nothing
        Me.client.SendPacket(Me)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Me.Type = CommandType.user
        Me.ResponseCommand = IRC_CMD_PART
        Return Me.channelName & " :"
    End Function

End Class
