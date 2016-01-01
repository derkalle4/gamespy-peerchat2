Public Class UserPacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        'Syntax: USER <username> 127.0.0.1 <servername> <connection-name>

        If Me.Params.Length < 4 Then
            Return IGNORE_SYNTAX_ERR
        End If

        Dim userSplit() As String = Split(Me.Params(0), "|")
        Me.client.UserName = userSplit(0)

        Me.client.Hostname = Me.Params(1)
        Me.client.Servername = Me.Params(2)
        Me.client.ConnectionName = Me.Params(3)

        Me.client.server.MySQL.InitUser(Me.client)
        Logger.Log("{0} connected with User-ID '{1}', connection identifier: '{2}'", LogLevel.Verbose, Me.client.Hostname, Me.client.UserName, Me.client.CDKey)
        Return True
    End Function

End Class