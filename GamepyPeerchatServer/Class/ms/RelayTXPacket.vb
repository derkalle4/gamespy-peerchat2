Public Class RelayTXPacket
    Inherits PeerChatPacket

    Public Property Contents As String = String.Empty

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Public Overrides Function CompileResponse() As String
        Return Me.Contents
    End Function
End Class