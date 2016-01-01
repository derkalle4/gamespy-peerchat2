Public Class CryptPacket
    Inherits PeerChatPacket
    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
        Me.UseCipher = False
    End Sub

    Public Overrides Function ManageData() As Boolean
        If Not Me.InitializeClient(Me.Params) Then Return False
        Me.client.SendPacket(Me)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Return IRCStdFormat(IRC_RES_CRYPT, "*",
                            GetString(Me.client.RXCipherKey.defaultKey),
                            GetString(Me.client.TXCipherKey.defaultKey))
    End Function

    Private Function InitializeClient(ByVal initParams() As String) As Boolean
        If initParams.Length < 3 Then Return False
        Me.client.CipherActive = True
        Me.client.CipherAlgorithm = initParams(0)
        '1: some number (version?)
        Me.client.GameName = initParams(2)
        Me.client.ServerKey = ArrayFunctions.GetBytes(Me.client.server.MySQL.FetchServerKey(Me.client.GameName))

        If Me.client.ServerKey.Length = 0 Then
            Logger.Log("{0}: unknown game '{1}' - dropping client", LogLevel.Verbose, Me.client.RemoteIPEP.ToString, Me.client.GameName)
            Return False
        End If

        Me.client.RXCipherKey = DESCipher.DESGenerateKey(Me.client.ServerKey)
        Me.client.TXCipherKey = DESCipher.DESGenerateKey(Me.client.ServerKey)

        Logger.Log("Setting up client at {0} playing {1} with {2} ", LogLevel.Verbose, Me.client.RemoteIPEP.ToString(), Me.client.GameName, Me.client.CipherAlgorithm)
        Return True
    End Function

End Class