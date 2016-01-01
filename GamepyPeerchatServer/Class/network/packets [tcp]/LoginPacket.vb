Public Class LoginPacket
    Inherits PeerChatPacket
    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)

    End Sub
    Private loginOK As Boolean = False

    Public Overrides Function ManageData() As Boolean
        Dim namespaceId As String = Me.Params(0)


        If Me.Params(1) = "*" Then 'Login
            'Syntax:  LOGIN <namespaceID> * <pwd-hash> :<nickname>@<e-mail>

            Me.Params(3) = Replace(Me.Params(3), ":", String.Empty)
            Dim nickSplit() As String = Split(Me.Params(3), "@")
            If nickSplit.Length <> 3 Then Return IGNORE_SYNTAX_ERR

            Me.loginOK = Me.client.PerformLogin(nickSplit(0), nickSplit(1) & nickSplit(2), Me.Params(2))

            'TODO: check if that's the right nick
            Me.client.NickName = nickSplit(0)

        Else 'Anonymous
            Me.client.NickName = Me.Params(1)
            Me.client.GameSpyPasswordHash = Me.Params(2)
        End If

        Me.client.SendPacket(Me)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String

        If Me.loginOK Then
            Me.ResponseId = IRC_RES_LOGIN_OK
            Return String.Format(IRC_FORMAT_LOGIN, Me.client.GameSpyAccountID.ToString(), Me.client.GameSpyProfileID.ToString())
        Else
            Me.ResponseId = IRC_RES_LOGIN_FAIL
            Return String.Empty
        End If

    End Function

End Class
