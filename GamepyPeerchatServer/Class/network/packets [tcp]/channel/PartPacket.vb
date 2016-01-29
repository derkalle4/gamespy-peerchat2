Public Class PartPacket
    Inherits PeerChatPacket

    Private channel As IRCChannel

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        Me.channel = Me.client.server.MySQL.GetIRCChannel(Me.Params(0))

        If Me.channel Is Nothing Then
            Me.client.SendPacket(New NoSuchChannelPacket(Me.client))
            Return True
        End If

        Dim u As IRCUser = Me.client.server.MySQL.GetUserById(Me.client.ClientId, Me.channel)

        Me.client.server.MySQL.PartChannel(u, Me.channel)
        'Me.client.CurrentChannel = Me.channel
        Logger.Log("{1} left channel {0}", LogLevel.Verbose, Me.Params(0), Me.client.UserName)
        Me.client.server.BCastToChannel(Me.channel, Me, Me.client, True)

        Return True
    End Function

    Public Overrides Function CompileResponse() As String

        Return IRCUserFormat(IRC_CMD_PART, ":" & Me.Params(0))
    End Function

End Class
