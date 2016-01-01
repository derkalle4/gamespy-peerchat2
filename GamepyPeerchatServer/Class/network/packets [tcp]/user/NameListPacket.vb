Public Class NameListPacket
    Inherits PeerChatPacket

    Private channel As IRCChannel

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Sub New(ByVal client As PeerChatClient, channel As IRCChannel)
        MyBase.New(client)
        Me.channel = channel
    End Sub

    Public Overrides Function ManageData() As Boolean
        Me.client.SendPacket(Me)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Dim userList As String = String.Empty
        Dim users As List(Of IRCUser) = Me.client.server.MySQL.GetuserList(Me.channel)
        If Not users Is Nothing Then
            For Each user As IRCUser In users
                'TODO: optimize
                'If Me.client.server.GetClientByUserId(user.ID) Is Nothing Then Continue For
                'TODO: fix
                If user.NickName.Contains("test") Then userList &= "@"
                userList &= user.NickName & " "
            Next
            If userList.Length > 0 Then userList = Mid(userList, 1, userList.Length - 1)
        End If
        Return IRCStdFormat(IRC_RES_NAMELIST, Me.client.NickName, "*", Me.channel.ChannelName, ":" & userList & " ")
    End Function

End Class