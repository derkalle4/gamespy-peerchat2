Public Class CDKeyPacket
    Inherits PeerChatPacket
    Private keyOk As Boolean = False

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        If Me.Params.Length < 1 Then Return IGNORE_SYNTAX_ERR
        keyOk = Me.CheckKey(Me.Params(0))
        Me.client.CDKey = Me.Params(0)
        Me.client.server.MySQL.UpdateKeyHash(Me.client)
        Me.client.SendPacket(Me)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        If Me.keyOk Then
            Return IRCStdFormat(IRC_RES_CDKEY, "*", GS_CDKEY_OK_ID, GS_CDKEY_OK_MSG)
        Else
            Return IRCStdFormat(IRC_RES_CDKEY, "*", GS_CDKEY_FAIL_ID, GS_CDKEY_FAIL_MSG)
        End If
    End Function

    Private Function CheckKey(ByVal key As String) As Boolean
        Return True
    End Function

End Class