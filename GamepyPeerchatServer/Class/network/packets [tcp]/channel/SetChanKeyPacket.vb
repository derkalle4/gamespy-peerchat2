Public Class SetChanKeyPacket
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

        Dim sets() As String = Split(Mid(Params(1), 2, Params(1).Length - 1), "\")
        For i = 1 To sets.Length - 1 Step 2
            Me.channel.ChanParams.PushOrUpdate(sets(i), sets(i + 1))
        Next

        Me.client.server.MySQL.UpdateChannelKey(Me.channel)

        'Me.client.server.BCastToChannel(Me.channel, Me, Me.client)
        Return True
    End Function

    'Public Overrides Function CompileResponse() As String
    '   Return IRCStdFormat(IRC_RES_GETCHANKEY, Me.client.NickName, Me.channel.ChannelName, Me.cookie, ":" & Me.channel.ChanParams.ToChKeyString())
    '   Return ""
    '   Return IRCStdFormat(IRC_RES_GETCHANKEY, Me.client.NickName, Me.channel.ChannelName, "BCAST", ":" & Me.Payload)
    'End Function

End Class