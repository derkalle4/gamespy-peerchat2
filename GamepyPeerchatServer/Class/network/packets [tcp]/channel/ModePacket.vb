        Public Class ModePacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Private channel As IRCChannel

    Public Overrides Function ManageData() As Boolean
        Me.channel = Me.client.server.MySQL.GetIRCChannel(Me.Params(0))

        If Me.channel Is Nothing Then
            Me.client.SendPacket(New NoSuchChannelPacket(Me.client))
            Return True
        End If

        Me.client.SendPacket(New TopicPacket(Me.client, Me.channel))
        'Me.client.SendPacket(New WeirdPacket(Me.client))
        Me.client.SendPacket(New NameListPacket(Me.client, Me.channel))
        Me.client.SendPacket(New NameListEndPacket(Me.client, Me.channel))

        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        'TODO: check if there's another res
        'If Me.channel Is Nothing Then
        Return IRCStdFormat("324", Me.client.NickName, Me.channel.ChannelName, "+tnp")
        'End If
    End Function

End Class