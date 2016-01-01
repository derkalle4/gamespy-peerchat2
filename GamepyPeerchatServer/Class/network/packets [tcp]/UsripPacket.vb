Public Class UsripPacket
    Inherits PeerChatPacket
    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        Me.client.SendPacket(Me)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Me.ResponseId = IRC_RES_USRIP
        Return String.Format(IRC_FORMAT_USRIP,
                             Me.client.RemoteIPEP.Address.ToString())
    End Function
End Class