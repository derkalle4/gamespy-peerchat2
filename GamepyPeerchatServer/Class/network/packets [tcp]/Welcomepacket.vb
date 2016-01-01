Public Class WelcomePacket

    Inherits PeerChatPacket
    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function CompileResponse() As String
        Me.ResponseId = IRC_RES_WELCOME
        Return String.Format(IRC_FORMAT_WELCOME, Me.client.NickName)
    End Function

End Class

