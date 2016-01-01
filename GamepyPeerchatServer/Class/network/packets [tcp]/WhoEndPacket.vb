Public Class WhoEndPacket
    Inherits PeerChatPacket

    Public Property WhoParams As String = String.Empty

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function ManageData() As Boolean
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Me.ResponseId = IRC_RES_WHOEND
        Return String.Format(IRC_FORMAT_WHOEND, Me.WhoParams)
    End Function

End Class
