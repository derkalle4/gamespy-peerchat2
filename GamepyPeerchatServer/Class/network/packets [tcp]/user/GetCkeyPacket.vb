Public Class GetCkeyPacket
    Inherits PeerChatPacket
    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Private getAllPlayers As Boolean = False
    Private cookie As String = String.Empty
    Private flags() As String
    Private channel As IRCChannel

    Private userList As New Queue(Of IRCUser)


    Public Overrides Function ManageData() As Boolean
        'GETCKEY #GSP!swempire * 000 0 :\username\b_flags
        '<channel> <nick> <cookie> 0 :<flags>
        'sprintf(buffer, "GETCKEY %s %s %s 0 :", channel, nick, cookie);

        Me.channel = Me.client.server.MySQL.GetIRCChannel(Me.Params(0))
        If Me.channel Is Nothing Then
            Me.client.SendPacket(New NoSuchChannelPacket(Me.client))
            Return True
        End If

        Me.getAllPlayers = (Me.Params(1) = "*")
        Me.cookie = Me.Params(2)
        Me.flags = Split(Mid(Me.Params(4), 2, Me.Params(4).Length - 1), "\")

        If Me.getAllPlayers Then
            Dim users As List(Of IRCUser) = Me.client.server.MySQL.GetUserList(Me.channel)
            Me.userList = New Queue(Of IRCUser)(users)

            If Not users Is Nothing Then
                For Each user As IRCUser In users
                    Me.client.SendPacket(Me)
                Next
            End If

        Else

            Dim user As IRCUser = Me.client.server.MySQL.GetUserByUniqueNick(Me.Params(1), Me.channel)
            If Not user Is Nothing Then
                Me.userList = New Queue(Of IRCUser)
                Me.userList.Enqueue(user)
                Me.client.SendPacket(Me)
            End If
        End If

        Me.client.SendPacket(New GetCKeyEndPacket(Me.client, Me.channel, Me.cookie))
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        ':s 702 test #GSP!swempire user1 000 :\id|123\
        Dim user As IRCUser = Me.userList.Dequeue()

        Return IRCStdFormat(IRC_RES_GETCKEY,
                            Me.client.NickName,
                            Me.channel.ChannelName,
                            user.NickName,
                            Me.cookie,
                            ":" & Me.BuildParamStr(user))
    End Function

    Private Function BuildParamStr(ByVal user As IRCUser) As String

        Dim res As String = String.Empty
        For Each s As String In Me.flags
            If s = String.Empty Then Continue For
            Select Case s
                Case "username"
                    res &= "\" & user.UserName & "|" & user.GameSpyId.ToString()
                Case Else
                    res &= "\" & user.UserParams.GetValue(s)
            End Select
        Next
        Return res
    End Function

End Class



