Public Class JoinPacket
    Inherits PeerChatPacket

    Private channel As IRCChannel

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        Me.channel = Me.client.server.MySQL.GetIRCChannel(Me.Params(0))

        If Me.channel Is Nothing And Not _
        Me.Params(0).StartsWith(String.Format(GS_CCREATE_FORMAT, Me.client.GameName)) Then

            Me.client.SendPacket(New NoSuchChannelPacket(Me.client))
            Return True
        End If

        If Me.channel Is Nothing Then

            Logger.Log("Creating temp channel '{0}' (host: {1})", LogLevel.Verbose, Me.Params(0), Me.client.UserName)
            Me.client.server.MySQL.CreateChannel(Me.Params(0), Me.client.ClientId)
            Me.channel = Me.client.server.MySQL.GetIRCChannel(Me.Params(0))
            Me.client.server.MySQL.JoinChannel(Me.client, Me.channel, "@")
        Else
            Me.client.server.MySQL.JoinChannel(Me.client, Me.channel)
        End If


        Logger.Log("{1} joined channel {0}", LogLevel.Verbose, Me.Params(0), Me.client.UserName)

        Me.client.server.BCastToChannel(Me.channel, Me, Me.client, True)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String

        Return IRCUserFormat(IRC_CMD_JOIN, ":" & Me.Params(0))
    End Function

End Class