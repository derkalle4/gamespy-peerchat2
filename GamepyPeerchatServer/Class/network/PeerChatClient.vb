'TCP-Client management class
'JW "LeKeks" 05/2014

Imports System.Net.Sockets
Imports System.Threading

Public Class PeerChatClient
    Public Property server As GamespyServer         'Ref. to serverobj.
    Public Property RemoteIPEP As Net.IPEndPoint    'client's remote endpoint

    Private readThread As Thread    'workthread
    Private writeThread As Thread
    Private client As TcpClient     'client
    Private stream As NetworkStream
    Private running As Boolean      'true if client is ok

    Public Property CipherActive As Boolean = False
    Public Property GameName As String = String.Empty
    Public Property CipherAlgorithm As String = String.Empty
    Public Property ServerKey As Byte() = {}
    Public Property Connected As Boolean = False

    Public Property Servername As String
    Public Property UserName As String
    Public Property Hostname As String

    Public Property NickName As String
    Public Property CDKey As String

    'Public Property ClientMode As Byte = 0
    Public Property ClientId As Int32 = -1

    Public Property GameSpyAccountID As Int32 = -1
    Public Property GameSpyProfileID As Int32 = -1
    Public Property GameSpyProfileName As String
    Public Property GameSpyPasswordHash As String
    Public Property GameSpyEmailAddr As String
    Public Property GameSpyUniqueNick As String
    Public Property GameSpyLoggedIn As Boolean = False

    Public Property ConnectionName As String
    Public Property Host As String = "*"

    Public Property RXCipherKey As DESCryptKey
    Public Property TXCipherKey As DESCryptKey

    Private packetQueue As Queue(Of PeerChatPacket)

    'Public Property CurrentChannel As IRCChannel

    Public Event ConnectionLost(ByVal sender As PeerChatClient)  'Used to pass connection loss to server

    Sub New(ByVal server As GamespyServer, client As TcpClient)
        Me.packetQueue = New Queue(Of PeerChatPacket)
        Me.server = server
        Me.running = True
        Me.stream = client.GetStream()
        Me.client = client
        Me.RemoteIPEP = DirectCast(client.Client.RemoteEndPoint, Net.IPEndPoint)
        Me.readThread = New Threading.Thread(AddressOf Me.Listen)
        Me.readThread.Start()
        Me.writeThread = New Threading.Thread(AddressOf Me.SendThreadRun)
        Me.writeThread.Start()
    End Sub

    Public Sub Dispose()
        On Error Resume Next 'continue disposing the object in any case
        Me.server.MySQL.ClearJoins(Me)
        Me.packetQueue.Clear()
        Me.running = False   'thread will exit
        Me.stream.Close()
        Me.client.Close()    'send FIN-ACK
        Me.client = Nothing  'kill client-ref
        Me.server = Nothing  'kill server-ref+
        RaiseEvent ConnectionLost(Me) 'throw event
    End Sub

    Private Sub Listen()
        Try
            'TODO: ping the clients and implement a custom timeout
            'Me.stream.ReadTimeout = TCP_CLIENT_TIMEOUT
            Dim buffer() As Byte = {}

            While Me.ReadTcpStream(buffer) And Me.running

                If Me.CipherActive = True Then
                    DESCrypt(buffer, buffer.Length, Me.RXCipherKey)
                End If

                'The client might have sent multiple PSHs before we read the stream
                'in IRC a \r\n indicates the end of a command row
                Dim commands() As String = Split(ArrayFunctions.GetString(buffer), vbCrLf)
                For i = 0 To commands.Length - 2
                    If Not Me.HandlePacket(commands(i)) Then
                        Exit Try
                    End If
                Next

                'Prevent heavy CPU load
                Threading.Thread.Sleep(TCP_CLIENT_PSH_SLEEP)
            End While

            If Me.running Then
                Logger.Log("{0}: Reached end of stream - FIN", LogLevel.Verbose, Me.RemoteIPEP.ToString)
            End If

        Catch ex As Exception

            If ex.InnerException Is Nothing Then
                Logger.Log("{0}: FIN: caused an Exception:" & vbCrLf & ex.ToString, LogLevel.Info, Me.RemoteIPEP.ToString())
            ElseIf ex.InnerException.GetType().IsEquivalentTo(GetType(SocketException)) Then
                Dim se As SocketException = DirectCast(ex.InnerException, SocketException)

                Select Case se.ErrorCode
                    Case SocketError.TimedOut
                        Logger.Log("{0}: read timed out after {1} seconds", LogLevel.Verbose, Me.RemoteIPEP.ToString, (stream.ReadTimeout / 1000).ToString())
                    Case SocketError.ConnectionAborted
                        Logger.Log("{0}: sent FIN -> closing connection", LogLevel.Verbose, Me.RemoteIPEP.ToString)
                    Case Else
                        Logger.Log("{0}: connection jammed - closing", LogLevel.Verbose, Me.RemoteIPEP.ToString)
                End Select
            Else
                Logger.Log("{0}: FIN: caused an Exception:" & vbCrLf & ex.ToString, LogLevel.Info, Me.RemoteIPEP.ToString())
            End If
        End Try

        Me.Dispose()
    End Sub
    Private Function ReadTcpStream(ByRef buffer() As Byte) As Boolean
        Array.Resize(buffer, TCP_CLIENT_BUFFERSIZE)

        Try
            'Just buffering 1k instead of seeking for the \r\n within the TCP-stream (faster)
            'NOTE: packet fragmentation is in theory possible, however it doesn't seem like
            'they push out fragmented data so it should be fine
            Dim bufferLen As Int32 = stream.Read(buffer, 0, TCP_CLIENT_BUFFERSIZE)
            Array.Resize(buffer, bufferLen)
            If bufferLen = 0 Then Return False 'end of stream
        Catch ex As Exception
            Return False
        End Try

        Logger.Log("{0}: fetched {1} bytes from stream", LogLevel.Verbose, Me.RemoteIPEP.ToString(), buffer.Length.ToString())
        Return True
    End Function
    Private Function HandlePacket(ByVal inputString As String) As Boolean

        'filter the \r\n indicating the end of the commandline
        InputString = Replace(InputString, vbCrLf, String.Empty)
        Logger.Log("{0}->{1}", LogLevel.Protocol, Me.NickName, inputString)

        'TODO: clean that up
        'IRC params are split by a simple space
        Dim inputParams() As String = Split(inputString, IRC_SPLITKEY)
        'The first string is the actual command
        Dim command As String = inputParams(0)

        'moved to PeerChatPacket-class
        'If inputParams.Length > 1 Then
        '   Array.Copy(inputParams, 1, inputParams, 0, inputParams.Length - 1)
        '   Array.Resize(inputParams, inputParams.Length - 1)
        'Else
        '   inputParams = {}
        'End If

        Dim packet As PeerChatPacket = Nothing

        'assign the correct packethandling-class
        Select Case command
            Case IRC_CMD_CRYPT
                packet = New CryptPacket(Me)
            Case IRC_CMD_USRIP
                packet = New UsripPacket(Me)
            Case IRC_CMD_USER
                packet = New UserPacket(Me)
            Case IRC_CMD_NICK
                packet = New NickPacket(Me)
            Case IRC_CMD_LOGIN
                packet = New LoginPacket(Me)
            Case IRC_CMD_CDKEY
                packet = New CDKeyPacket(Me)
            Case IRC_CMD_QUIT
                packet = New QuitPacket(Me)
            Case IRC_CMD_JOIN
                packet = New JoinPacket(Me)
            Case IRC_CMD_WHO
                packet = New WhoPacket(Me)
            Case IRC_CMD_PING
                packet = New PingPacket(Me)
            Case IRC_CMD_MODE
                packet = New ModePacket(Me)
            Case IRC_CMD_GETCKEY
                packet = New GetCkeyPacket(Me)
            Case IRC_CMD_TOPIC
                packet = New TopicPacket(Me)
            Case IRC_CMD_SETCHANKEY
                packet = New SetChanKeyPacket(Me)
            Case IRC_CMD_GETCHANKEY
                packet = New GetChanKeyPacket(Me)
            Case IRC_CMD_SETCKEY
                packet = New SetCKeyPacket(Me)
            Case IRC_CMD_UTM
                packet = New UTMPacket(Me)
            Case IRC_CMD_PRIVMSG
                packet = New PrivMsgPacket(Me)
            Case IRC_CMD_PART
                packet = New PartPacket(Me)
            Case Else
                Logger.Log("Dropping unknown TCP packet ({0})", LogLevel.Verbose, command)
        End Select

        If Not packet Is Nothing Then
            packet.RawData = inputString
            packet.Command = command
            Return packet.ManageData()
        End If

        Return True
    End Function

    Public Sub SendString(ByVal str As String, Optional ByVal UseCipher As Boolean = False)
        str = str & vbCrLf                  'attach \r\n to indicate the end of the packet
        Dim buf() As Byte = GetBytes(str)   'convert to binary data

        'check if encryption is required
        If Me.CipherActive And UseCipher Then
            DESCrypt(buf, buf.Length, Me.TXCipherKey) 'encrypt the buffer using the TX-key
        ElseIf UseCipher Then
            Logger.Log("{0} requested encryption before setting up the keys.", LogLevel.Warning, Me.RemoteIPEP.ToString())
        End If

        Logger.Log("{0}<-{1}", LogLevel.Protocol, Me.NickName, str)
        Me.stream.Write(buf, 0, buf.Length)
        Me.stream.Flush()
    End Sub

    Public Sub SendPacket(ByVal packet As PeerChatPacket)
        Me.packetQueue.Enqueue(packet)
    End Sub

    Private Sub SendThreadRun()
        While Me.running
            If Me.packetQueue.Count > 0 Then
                Dim packet As PeerChatPacket = Me.packetQueue.Dequeue
                Me.SendString(packet.CompileResponse(), packet.UseCipher)
            End If
            Thread.Sleep(constants.TCP_CLIENT_PSH_SLEEP)
        End While
    End Sub

    Public Function PerformLogin(ByVal username As String, ByVal password As String, Optional ByVal email As String = "") As Boolean
        Dim id As Int32 = Me.server.MySQL.CheckLogin(username, password, email)
        If id = -1 Then Return False
        Me.GameSpyAccountID = id
        Me.GameSpyProfileID = id

        Me.GameSpyProfileName = username
        Me.GameSpyEmailAddr = email
        Me.GameSpyPasswordHash = password
        Me.GameSpyLoggedIn = True

        'TODO: implement "real" unique nicks
        Me.GameSpyUniqueNick = username
        Return True
    End Function

    Public Sub GenerateUniqueNick()
        If Me.GameSpyLoggedIn Then
            Me.NickName = Me.GameSpyUniqueNick
        Else
            Me.NickName = "GM-TEMP-" & Me.readThread.ManagedThreadId  'use a temp nick for now"
        End If
    End Sub

End Class