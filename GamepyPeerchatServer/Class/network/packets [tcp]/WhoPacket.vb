Public Class WhoPacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Private queryName As String = String.Empty

    Public Overrides Function ManageData() As Boolean
        If Me.Params.Length < 1 Then Return IGNORE_SYNTAX_ERR
        Me.queryName = Me.Params(0)
        Me.client.SendPacket(Me)
        Dim wep As New WhoEndPacket(Me.client)
        wep.WhoParams = Me.queryName
        Me.client.SendPacket(wep)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        ' Syntax: :s 352 <?> <channel> <user> <address> <nick> <mode> <?> <?>
        '[mode = "@"->"op" || "+"->"voice"] 
        '<channel> <user> <host> <server> <nick> <H|G>[*][@|+] :<hopcount> <real_name>

        Dim res As String
        res = String.Format(IRC_FORMAT_WHO,
                            Me.client.CurrentChannel.ChannelName,
                            Me.client.UserName,
                            Me.client.Host,
                            "s",
                            Me.client.NickName,
                            "+",
                            ":0",
                            "realname")

        res = "* " & Me.client.UserName & " LeKeks s H :0 rn"

        'Me.client.NickName,
        '  Me.client.Servername,
        Me.ResponseId = IRC_RES_WHO
        Return res
    End Function

End Class
