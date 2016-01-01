Public Class WelcomePacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function CompileResponse() As String
        Return IRCStdFormat(IRC_RES_WELCOME,
                            Me.client.NickName,
                            ":" & String.Format(IRC_WELCOME_MSG, Me.client.NickName))
    End Function

End Class