Public Class PrivMsgPacket
    Inherits PeerChatPacket

    Private channel As IRCChannel
    Private targetName As String

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        Me.targetName = Me.Params(0)

        If Me.targetName.StartsWith(GS_CHANNEL_PREFIX) Then
            Me.SendToChannel()
        Else
            Me.SendToUser()
        End If
        Return True
    End Function

    Private Sub SendToChannel()
        Me.channel = Me.client.server.MySQL.GetIRCChannel(Me.targetName)
        If Me.channel Is Nothing Then
            Me.client.SendPacket(New NoSuchChannelPacket(Me.client))
            Return
        End If
        Me.client.server.BCastToChannel(Me.channel, Me, Me.client, False)
    End Sub

    Private Sub SendToUser()
        Me.client.server.SendToUserByUqName(Me.Params(0), Me)
    End Sub

    Public Overrides Function CompileResponse() As String

        Return ":" & Me.client.NickName & " " & IRC_CMD_PRIVMSG & " " & Me.targetName & " :" & Me.Payload
    End Function

End Class

