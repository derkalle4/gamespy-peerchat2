Public Class WhoEndPacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Return IRCStdFormat(IRC_RES_WHOEND, Me.client.NickName, Me.client.NickName, ":" & IRC_WHOEND_MSG)
    End Function

End Class
