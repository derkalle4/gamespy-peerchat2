Public Class SetCKeyPacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Private channel As IRCChannel
    Private userName As String = String.Empty

    Public Overrides Function ManageData() As Boolean
        Dim user As IRCUser = Me.client.server.MySQL.GetUserByClient(Me.client)
        Me.channel = Me.client.server.MySQL.GetIRCChannel(Me.Params(0))

        Logger.Log("Updating CKEY-Flags {0} > {1}", LogLevel.Verbose, user.UserName, Params(0))

        'sprintf(buffer, "SETCKEY  %s %s 0 :", channel, user);

        If Not user Is Nothing Then
            Dim sets() As String = Split(Me.Payload, "\")
            For i = 1 To sets.Length - 1 Step 2
                user.UserParams.PushOrUpdate(sets(i), sets(i + 1))
            Next
            Me.client.server.MySQL.UpdateCkey(user, channel)
        End If

        Me.userName = Params(1)

        Me.client.server.BCastToChannel(Me.channel, Me, Me.client, True)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Return IRCStdFormat(IRC_RES_GETCKEY, Me.channel.ChannelName, Me.channel.ChannelName, Me.userName, "BCAST", ":" & Me.Payload)
    End Function
End Class