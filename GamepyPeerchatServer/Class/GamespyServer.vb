Public Class GamespyServer
    Public Property GSTcpServer As TcpServer

    Public Property MSP2PHandler As P2PServerHandler

    Public Property Clients As List(Of PeerChatClient)
    Public Property MySQL As PCMysqlHandler

    Public Property Config As CoreConfig
    Private configMan As ConfigSerializer

#Region "Program"
    Public Sub Run()
        Me.PreInit()
        Me.Execution()
        Me.PostInit()
    End Sub
    Private Sub PreInit()
        Logger.Log("Setting up components...", LogLevel.Info)

        Me.ConfigMan = New ConfigSerializer(GetType(CoreConfig))
        Me.Config = Me.ConfigMan.LoadFromFile(CFG_FILE, CurDir() & CFG_DIR)

        Logger.MinLogLevel = Me.Config.Loglevel
        Logger.LogToFile = Me.Config.LogToFile
        Logger.LogFileName = Me.Config.LogFileName

        Me.MySQL = New PCMysqlHandler()
        Me.MySQL.Hostname = Me.Config.MySQLHostname
        Me.MySQL.Port = Me.Config.MySQLPort
        Me.MySQL.DbName = Me.Config.MySQLDatabase
        Me.MySQL.DbUser = Me.Config.MySQLUsername
        Me.MySQL.DbPwd = Me.Config.MySQLPwd
        Me.MySQL.MasterServerID = Me.Config.ServerID

        Me.Clients = New List(Of PeerChatClient)
        Me.GSTcpServer = New TcpServer()

        Me.GSTcpServer.Port = Me.Config.TCPQueryPort
        Me.GSTcpServer.Address = Net.IPAddress.Parse(Me.Config.TCPQueryAddress)

        AddHandler Me.GSTcpServer.ClientConnected, AddressOf Me.GSTcpServer_OnClientConnect

        If Me.Config.P2PEnable Then
            Me.MSP2PHandler = New P2PServerHandler(Me)
            Me.MSP2PHandler.Address = Net.IPAddress.Parse(Me.Config.P2PAddress)
            Me.MSP2PHandler.Port = Me.Config.P2PPort
            Me.MSP2PHandler.EncKey = ArrayFunctions.GetBytes(Me.Config.P2PKey)
        End If
    End Sub
    Private Sub Execution()
        If Me.Config.LogToFile Then Logger.OpenLogFile()

        Logger.Log("Starting components...", LogLevel.Info)
        Me.MySQL.Connect()
        Me.GSTcpServer.Open()
        If Me.Config.P2PEnable Then Me.MSP2PHandler.Open()

        Logger.Log("Initializing User List", LogLevel.Info)
        Me.MySQL.RunCleanup()

        Logger.Log("Launch OK. Server is up.", LogLevel.Info)
        Logger.Log("Press [Return] to exit", LogLevel.Info)
        Console.ReadLine()
        Logger.Log("Shutting down...", LogLevel.Info)
    End Sub
    Private Sub PostInit()
        'Prevent new clients from being added to the client-list
        RemoveHandler Me.GSTcpServer.ClientConnected, AddressOf Me.GSTcpServer_OnClientConnect

        'Drop all remaining clients before shutting down the main infrastructure
        For Each c As PeerChatClient In Me.Clients
            c.Dispose()
        Next

        Me.Clients.Clear()

        Me.GSTcpServer.Close()
        If Me.Config.P2PEnable Then Me.MSP2PHandler.Close()
        Me.MySQL.Close()

        Me.MSP2PHandler = Nothing
        Me.ConfigMan = Nothing
        Me.Config = Nothing
        Me.GSTcpServer = Nothing
        Me.Clients = Nothing
        Me.MySQL = Nothing
        GC.Collect()
        Logger.Log("Server stopped.", LogLevel.Info)
        Logger.CloseLogFile()
        End
    End Sub
#End Region

#Region "IRC"
    Public Sub BCastToChannel(ByVal channel As IRCChannel, ByVal packet As PeerChatPacket, ByVal sender As PeerChatClient, Optional ByVal castToSender As Boolean = False)
        Dim users As List(Of IRCUser) = Me.MySQL.GetUserList(channel)
        If users Is Nothing Then Return

        For Each user In users
            If user.ID = sender.ClientId And Not castToSender Then Continue For
            SyncLock Me.Clients
                Dim c As PeerChatClient = GetClientByUserId(user.ID)
                If Not c Is Nothing Then c.SendPacket(packet)
            End SyncLock
        Next
    End Sub
    Public Sub SendToUserByUqName(ByVal uqName As String, ByVal packet As PeerChatPacket)
        Dim user As IRCUser = Me.MySQL.GetUserByUniqueNick(uqName)
        If Not user Is Nothing Then
            Dim c As PeerChatClient = Me.GetClientByUserId(user.ID)
            If Not c Is Nothing Then c.SendPacket(packet)
        End If
    End Sub
    Public Function GetClientByUserId(ByVal userId As Integer) As PeerChatClient
        For Each c As PeerChatClient In Me.Clients
            If c.ClientId = userId Then Return c
        Next
        Return Nothing
    End Function
#End Region

#Region "Events"
    Private Sub GSTcpServer_OnClientConnect(ByVal sender As TcpServer, ByVal client As Net.Sockets.TcpClient)
        Me.SetupClient(client)
    End Sub
    Private Sub Client_ConnectionLost(ByVal sender As PeerChatClient)
        Me.RemoveClient(sender)
    End Sub
#End Region

#Region "Client/Server Funktionen"
    Private Sub SetupClient(ByVal client As Net.Sockets.TcpClient)
        Dim gsc As New PeerChatClient(Me, client)
        Clients.Add(gsc)
        AddHandler gsc.ConnectionLost, AddressOf Me.Client_ConnectionLost
    End Sub
    Private Sub RemoveClient(ByVal client As PeerChatClient)
        RemoveHandler client.ConnectionLost, AddressOf Me.Client_ConnectionLost
        Me.Clients.Remove(client)
    End Sub
#End Region
End Class