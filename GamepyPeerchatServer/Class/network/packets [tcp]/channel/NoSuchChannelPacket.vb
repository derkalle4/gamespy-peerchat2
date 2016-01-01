Public Class NoSuchChannelPacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Return IRCStdFormat(IRC_RES_NOSUCHCHANNEL, ":" & IRC_NOSUCHCHANNEL_MSG)
    End Function
End Class