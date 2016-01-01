Public Class MessagePacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Me.Type = CommandType.raw
        Me.ResponseId = IRC_RES_MESSAGE
        Return ":s 332 LeKeks " & Me.client.CurrentChannel.ChannelName & " :Hello World."
    End Function

End Class
