Public Class NameListEndPacket
    Inherits PeerChatPacket

    Private channel As IRCChannel

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Sub New(ByVal client As PeerChatClient, channel As IRCChannel)
        MyBase.New(client)
        Me.channel = channel
    End Sub

    Public Overrides Function ManageData() As Boolean
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Return IRCStdFormat(IRC_RES_NAMELISTEND, Me.client.NickName, Me.channel.ChannelName, ":" & IRC_NAMELISTEND_MSG)
    End Function

End Class