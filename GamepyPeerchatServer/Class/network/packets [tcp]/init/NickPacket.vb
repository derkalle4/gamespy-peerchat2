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
            ' Me.client.NickName = "*"
            Me.client.GenerateUniqueNick()
        End If

        If Me.client.GameSpyLoggedIn Then
            Me.client.NickName &= GS_ACCOUNT_SUFFIX
        End If

        If client.Connected = False Then
            client.Connected = True
            Me.client.SendPacket(New WelcomePacket(Me.client))
            Me.client.SendPacket(New MotdPacket(Me.client))
        End If

        Me.client.server.MySQL.UpdateNick(Me.client)
        Return True
    End Function

End Class