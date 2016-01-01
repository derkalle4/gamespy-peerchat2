Public Class QuitPacket
    Inherits PeerChatPacket
    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        Logger.Log("Client {0} [{1}] quit.", LogLevel.Verbose, Me.client.NickName, Me.client.RemoteIPEP.ToString())
        Me.client.SendPacket(Me)
        Return False
    End Function

    Public Overrides Function CompileResponse() As String 
        Return IRC_CMD_QUIT & ":" & IRC_QUIT_MSG
    End Function
End Class

