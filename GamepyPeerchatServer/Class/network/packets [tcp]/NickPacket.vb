Public Class NickPacket
    Inherits PeerChatPacket
    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        If Params(0) <> "*" Then
            'TODO: check if nickname is already in use
            Me.client.NickName = Params(0)
        Else 'Unique Nick
            Me.client.GenerateUniqueNick()
        End If

        If client.Connected = False Then
            client.Connected = True
            Me.client.SendPacket(New Welcomepacket(Me.client))
            Me.client.SendPacket(New MotdPacket(Me.client))
        End If
        Return True
    End Function

End Class

