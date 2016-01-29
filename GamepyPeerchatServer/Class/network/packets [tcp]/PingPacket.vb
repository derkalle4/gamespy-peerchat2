Public Class PingPacket
    Inherits PeerChatPacket
    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        Me.client.SendPacket(Me)
        Return False
    End Function

    Public Overrides Function CompileResponse() As String
        'Me.Type = CommandType.raw
        Return IRC_CMD_PONG
    End Function
End Class