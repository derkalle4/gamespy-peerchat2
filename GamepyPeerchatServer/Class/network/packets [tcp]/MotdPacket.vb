Public Class MotdPacket
    Inherits PeerChatPacket
    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function CompileResponse() As String
        Me.ResponseId = IRC_RES_MOTD
        Return IRC_FORMAT_MOTD
    End Function

End Class


