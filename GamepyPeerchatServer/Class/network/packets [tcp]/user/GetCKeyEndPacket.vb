Public Class GetCKeyEndPacket
    Inherits PeerChatPacket

    Private channel As IRCChannel
    Private cookie As String = "000"

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Sub New(ByVal client As PeerChatClient, channel As IRCChannel, ByVal cookie As String)
        MyBase.New(client)
        Me.channel = channel
        Me.cookie = cookie
    End Sub

    Public Overrides Function ManageData() As Boolean
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Return IRCStdFormat(IRC_RES_GETCKEYEND,
                            Me.client.NickName,
                            Me.channel.ChannelName,
                            cookie,
                            ":" & IRC_GETCKEYEND_MSG)
    End Function

End Class