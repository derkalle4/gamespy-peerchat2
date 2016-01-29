Public Class WhoPacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Private queryName As String = String.Empty

    Public Overrides Function ManageData() As Boolean
        If Me.Params.Length < 1 Then Return IGNORE_SYNTAX_ERR
        Me.client.SendPacket(Me)
        Dim wep As New WhoEndPacket(Me.client)
        Me.client.SendPacket(wep)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Dim u As IRCUser = Me.client.server.MySQL.GetUserByUniqueNick(Params(0))

        Return IRCStdFormat(IRC_RES_WHO,
                            u.Channel.ChannelName,
                            u.NickName,
                            u.UserName,
                            "s",
                            u.ModeName,
                            "*",
                            ":0",
                            u.KeyHash)
    End Function

End Class
