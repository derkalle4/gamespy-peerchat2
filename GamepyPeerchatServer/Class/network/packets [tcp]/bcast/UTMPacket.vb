Public Class UTMPacket
    Inherits PeerChatPacket

    Private channel As IRCChannel
    Private target As String

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        Me.target = Me.Params(0)
        If Me.target.StartsWith(GS_CHANNEL_PREFIX) Then
            Me.SendToChannel()
        Else
            Me.SendToUser()
        End If
        Return True
    End Function

    Private Sub SendToUser()
        Me.client.server.SendToUserByUqName(Me.Params(0), Me)
    End Sub

    Private Sub SendToChannel()
        Dim sender As PeerChatClient = Me.client
        Me.channel = Me.client.server.MySQL.GetIRCChannel(Me.Params(0))
        If Me.channel Is Nothing Then
            Me.client.SendPacket(New NoSuchChannelPacket(Me.client))
            Return
        End If
        Logger.Log("Broadcasting UTM for channel {0}", LogLevel.Verbose, Me.channel.ChannelName)
        Me.client.server.BCastToChannel(Me.channel, Me, Me.client, False)
        Me.client = sender
    End Sub

    Public Overrides Function CompileResponse() As String
        Return ":" & Me.client.NickName & "!*@*" & " " & IRC_CMD_UTM & " " & Me.target & " :" & Me.Payload
        'Return IRCUserFormat(IRC_CMD_UTM, Me.target, ":" & Me.Payload)
    End Function

End Class