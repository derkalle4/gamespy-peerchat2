'Multimasterserver P2P-Protocol
'IRC-Relay packet used to transmit realtime IRC events to the other servers
'JW "LeKeks" 02/2015
Public Class P2PIRCRelayPacket
    Inherits P2PUdpPacket

    'Packet-Syntax:
    '<byte>  flags
    '<int32> cause-id
    '<int32> recv.-id
    '<str>   payload   \x0

    'flags:
    '1 << 0   -> broadcast to channel 
    '1 << 1   -> broadcast to sender
    '1 << 2   -> use cipher


    Sub New(ByVal server As UdpServer, ByVal remoteIPEP As Net.IPEndPoint)
        MyBase.New(server, remoteIPEP)
    End Sub

    Public Overrides Sub ManageData()
        Dim p As New RelayTXPacket(Nothing)

        Dim flags As Byte = Me.data(0)
        Me.bytesParsed += 1

        'Dim senderId As Int32 = BitConverter.ToInt32(Me.data, Me.bytesParsed)
        Me.bytesParsed += 4

        Dim recvId As Int32 = BitConverter.ToInt32(Me.data, Me.bytesParsed)
        Me.bytesParsed += 4

        Dim payload As String = ArrayFunctions.GetCStyleString(Me.data, Me.bytesParsed)

        p.UseCipher = flags And P2P_FLAG_USECIPHER
        p.Contents = payload

        If flags And P2P_FLAG_BCAST Then
            Dim channel As IRCChannel = Me.Server.Server.MySQL.GetIRCChannel(recvId)
            If channel Is Nothing Then Return
            Me.Server.Server.BCastToChannel(channel, p, Nothing, flags And P2P_FLAG_BCAST_TOSENDER)
        Else
            Dim client As PeerChatClient = Me.Server.Server.GetClientByUserId(recvId)
            If client Is Nothing Then Return
            client.SendPacket(p)
        End If
    End Sub



    Public Overrides Function CompileResponse() As Byte()
        Return MyBase.CompileResponse()
    End Function



End Class
