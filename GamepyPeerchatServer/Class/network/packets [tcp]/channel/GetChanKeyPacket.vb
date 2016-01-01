Public Class GetChanKeyPacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Private channel As IRCChannel
    Private cookie As String
    Private filter() As String = Nothing

    Public Overrides Function ManageData() As Boolean
        Me.channel = Me.client.server.MySQL.GetIRCChannel(Me.Params(0))
        Me.cookie = Me.Params(1)
        Me.filter = Split(Me.Payload, "\")
        Array.Copy(Me.filter, 1, Me.filter, 0, Me.filter.Length - 1)
        Array.Resize(Me.filter, Me.filter.Length - 1)

        If Me.channel Is Nothing Then
            Me.client.SendPacket(New NoSuchChannelPacket(Me.client))
            Return True
        End If

        Me.client.SendPacket(Me)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        'TODO: check if there's another res
        'channel = message->params[1];
        'cookie = message->params[2];
        'flags = message->params[3];
        Return IRCStdFormat(IRC_RES_GETCHANKEY, Me.client.NickName, Me.channel.ChannelName, Me.cookie, ":" & Me.channel.ChanParams.ToChKeyString(Me.filter))
    End Function

End Class