Public Class MotdPacket
    Inherits PeerChatPacket
    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Property EndCommand As Boolean = False

    Public Overrides Function CompileResponse() As String
        Return IRCStdFormat(IRC_RES_MOTD, Me.client.NickName, ":" & IRC_MOTD_MSG)
        Me.client.SendPacket(Me)
    End Function

End Class