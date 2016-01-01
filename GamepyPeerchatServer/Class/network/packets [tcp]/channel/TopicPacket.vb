Public Class TopicPacket
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
        Me.channel = Me.client.server.MySQL.GetIRCChannel(Me.Params(0))
        'check permissions
        If Me.channel.HostId <> Me.client.ClientId Then Return True

        If Me.channel Is Nothing Then
            Me.client.SendPacket(New NoSuchChannelPacket(Me.client))
            Return True
        End If
        Me.channel.Topic = Split(Params(1), ":")(1)
        Me.client.server.MySQL.UpdateChannelTopic(Me.channel)

        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        If Me.channel.Topic <> String.Empty Then
            Return IRCStdFormat(IRC_RES_TOPIC, Me.client.NickName, Me.channel.ChannelName, ":" & Me.channel.Topic)
        Else
            Return IRCStdFormat(IRC_RES_NOTOPIC, Me.client.NickName, Me.channel.ChannelName, ":" & IRC_NOTOPIC_MSG)
        End If
    End Function

End Class