Public Class CryptPacket
    Inherits PeerChatPacket
    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
        Me.Type = CommandType.nocipher
    End Sub

    Public Overrides Function ManageData() As Boolean
        If Not Me.InitializeClient(Me.Params) Then Return False
        Me.client.SendPacket(Me)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Me.ResponseId = IRC_RES_CRYPT
        Return String.Format(IRC_FORMAT_CRYPT,
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

        Me.GenerateKeys()

        Logger.Log("Setting up client at {0} playing {1} with {2} ", LogLevel.Verbose, Me.client.RemoteIPEP.ToString(), Me.client.GameName, Me.client.CipherAlgorithm)
        Return True
    End Function

    Private Sub GenerateKeys()
        Dim RXKey As New DESCryptKey
        Dim TXKey As New DESCryptKey

        RXKey.key = GetBytes(Me.RandStr(16))
        TXKey.key = GetBytes(Me.RandStr(16))
        RXKey.defaultKey = New Byte(15) {}
        TXKey.defaultKey = New Byte(15) {}

        Array.Copy(RXKey.key, RXKey.defaultKey, RXKey.key.Length)
        Array.Copy(TXKey.key, TXKey.defaultKey, TXKey.key.Length)

        Me.client.RXCipherKey = RXKey
        Me.client.TXCipherKey = TXKey

        DESCipher.DESXCodeBuffer(Me.client.RXCipherKey.key, Me.client.RXCipherKey.key.Length, Me.client.ServerKey)
        DESCipher.DESPrepareKey(Me.client.RXCipherKey.key, Me.client.RXCipherKey.key.Length, Me.client.RXCipherKey)

        DESCipher.DESXCodeBuffer(Me.client.TXCipherKey.key, Me.client.TXCipherKey.key.Length, Me.client.ServerKey)
        DESCipher.DESPrepareKey(Me.client.TXCipherKey.key, Me.client.TXCipherKey.key.Length, Me.client.TXCipherKey)
    End Sub

    Private Function RandStr(ByVal length As Int32) As String
        Dim sb As New System.Text.StringBuilder
        Dim chars() As String = {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", _
               "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "X", _
               "1", "2", "3", "4", "5", "6", "7", "8", "9", "0"}
        Dim upperBound As Integer = UBound(chars)

        For x As Integer = 1 To length
            sb.Append(chars(Int(Rnd() * upperBound)))
        Next

        Return sb.ToString

    End Function
End Class
