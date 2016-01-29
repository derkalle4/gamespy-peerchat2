'MySQLHandler-Wrapper for Peerchatserver SQL-actions
'JW "LeKeks" 01/2015
Imports MySql.Data.MySqlClient
Imports System.Security.Cryptography

Public Class PCMysqlHandler
    Inherits MySQLHandler

    Public Property MasterServerID As Int32
    Public Property DefaultChannelID As Int32

    Public Function FetchMasterserver(ByVal rIPEP As Net.IPEndPoint) As MasterServer
        Dim sql As String = _
            "select `id`, `server_name` from `masterserver` " & _
            "where `server_address` = '" & rIPEP.Address.ToString & "' and " & _
            "`server_port` = " & rIPEP.Port.ToString
        SyncLock Me.connection
            Using res As MySqlDataReader = Me.DoQuery(sql)
                res.Read()
                If res.HasRows Then
                    Dim ms As New MasterServer
                    ms.id = res("id")
                    ms.msName = res("server_name")
                    ms.rIPEP = rIPEP
                    res.Close()
                    Return ms
                Else
                    res.Close()
                    Return Nothing
                End If
            End Using
        End SyncLock
    End Function
    Public Function GetManagingMasterserver(ByVal rIPEP As Net.IPEndPoint) As MasterServer
        SyncLock Me.connection
            Dim sql As String =
            "select `masterserver`, `server_name`, `server_address`, `server_port` " & _
            "from `gameserver`, `masterserver` where " & _
            "`address` = '" & rIPEP.Address.ToString & "'" & " and " & _
            "`port` = " & rIPEP.Port.ToString & " and " & _
            "`masterserver` = `masterserver`.`id`"

            Using res As MySqlDataReader = Me.DoQuery(sql)
                res.Read()

                If Not res.HasRows Then
                    res.Close()
                    Return Nothing
                End If

                Dim ms As New MasterServer
                ms.id = res("masterserver")
                ms.msName = res("server_name")
                ms.rIPEP = New Net.IPEndPoint(Net.IPAddress.Parse(res("server_address")), UInt16.Parse(res("server_port")))
                res.Close()
                Return ms
            End Using
        End SyncLock
    End Function
    Public Function GetMasterServers() As List(Of MasterServer)
        Dim sql As String = _
            "select * from `masterserver`"
        Dim servers As New List(Of MasterServer)
        SyncLock Me.connection
            Using res As MySqlDataReader = Me.DoQuery(sql)
                If Not res Is Nothing Then
                    While res.Read
                        Dim ms As New MasterServer
                        ms.id = res("id")
                        ms.msName = res("server_name")
                        ms.rIPEP = New Net.IPEndPoint(Net.IPAddress.Parse(res("server_address")), UInt16.Parse(res("server_port")))
                        servers.Add(ms)
                    End While
                    res.Close()
                End If
            End Using
        End SyncLock
        Return servers
    End Function
    Public Function FetchServerKey(ByVal gamename As String) As String
        gamename = Me.CorrectGameType(gamename)
        SyncLock Me.connection
            Dim sql As String = "select `key_key` from  `serverkeys` where " & _
                                "`key_gamename` = '" & EscapeString(gamename) & "'"

            Using res As MySqlDataReader = Me.DoQuery(sql)
                If Not res Is Nothing Then
                    res.Read()
                    If res.HasRows Then
                        Dim key As String = res("key_key")
                        res.Close()
                        Return key
                    End If
                    res.Close()
                End If
                Return String.Empty
            End Using
        End SyncLock
    End Function
    Private Function CorrectGameType(ByVal type As String) As String
        If type = "swbfrontps2p" Then type = "swbfrontps2"
        If type = "swbfront2ps2p" Then type = "swbfront2ps2"
        Return type
    End Function

    Public Function CheckLogin(ByVal username As String, ByVal password As String, Optional ByVal email As String = "") As Int32
        Dim sql As String = "select `id` from users where " & _
            "`name` = '" & Me.EscapeString(username) & _
            "' and `password` = '" & EscapeString(password) & "'"

        If email <> String.Empty Then sql &= " and `email` = '" & EscapeString(email) & "'"

        Using res As MySqlDataReader = Me.DoQuery(sql)
            res.Read()
            If res.HasRows() Then
                Dim id As Int32 = res("id")
                res.Close()
                Return id
            Else
                res.Close()
                Return -1
            End If

        End Using
    End Function



    Public Sub InitUser(ByVal client As PeerChatClient)
        Dim uid As String = Me.GetClientID(client)

        Dim sql As String =
               "`peerchat_users` set" &
               "`user_online` = 1," &
               "`user_lastseen` = UNIX_TIMESTAMP(), " &
               "`user_masterserver` = " & Me.MasterServerID.ToString() & ", " &
               "`user_channel` = " & Me.DefaultChannelID.ToString()

        If client.GameSpyLoggedIn Then
            sql &= "," &
                   "`user_profile` = " & client.GameSpyAccountID.ToString() & "," &
                   "`user_loggedin` = 1"
        Else
            sql &= "," &
                   "`user_loggedin` = 0, " &
                   "`user_profile` = -1"
        End If

        If uid <> -1 Then
            sql = "update " & sql
            sql &= " where `id` = " & uid.ToString
        Else
            sql = "insert into " & sql
            sql &= ", `user_username` = '" & EscapeString(client.UserName) & "'"
        End If

        Me.NonQuery(sql)

        If uid <> -1 Then
            client.ClientId = uid
        Else
            client.ClientId = Me.GetClientID(client)
        End If

    End Sub
    Public Sub UpdateNick(ByVal client As PeerChatClient)
        Dim sql As String = "update `peerchat_users` set" &
                              "`user_nickname` = '" & EscapeString(client.NickName) & "'" &
                              " where `id` = " & client.ClientId
        Me.NonQuery(sql)
    End Sub
    Public Sub UpdateKeyHash(ByVal client As PeerChatClient)
        Dim sql As String = "update `peerchat_users` set" &
                              "`user_keyhash` = '" & Me.MD5StringHash(EscapeString(client.CDKey)) & "'" &
                              " where `id` = " & client.ClientId
        Me.NonQuery(sql)
    End Sub
    Public Function GetClientID(ByVal client As PeerChatClient)
        Dim sql As String = "select `id` from `peerchat_users`" &
                    " where `user_username` = '" & EscapeString(client.UserName) & "'"

        Dim uid As Integer = -1

        SyncLock Me.connection
            Using res As MySqlDataReader = Me.DoQuery(sql)
                res.Read()
                If res.HasRows() Then
                    uid = res("id")
                End If
                res.Close()
            End Using
        End SyncLock
        Return uid
    End Function


    Public Sub UpdateChannelTopic(ByVal channel As IRCChannel)
        Dim sql As String = "update peerchat_channels set `channel_topic` = '" & EscapeString(channel.Topic) & "'" &
                            " where `id` = " & channel.Id
        Me.NonQuery(sql)
    End Sub
    Public Sub UpdateChannelKey(ByVal channel As IRCChannel)
        channel.Key = channel.ChanParams.ToParameterString()
        Dim sql As String = "update peerchat_channels set `channel_key` = '" & EscapeString(channel.Key) & "'" &
                            " where `id` = " & channel.Id
        Me.NonQuery(sql)
    End Sub
    Public Sub CreateChannel(ByVal channelName As String, ByVal hostId As Integer)
        Dim sql As String = "insert into `peerchat_channels` set" &
                            "`channel_name` = '" & EscapeString(channelName) & "', " &
                            "`channel_host` = " & hostId.ToString() & "," &
                            "`channel_temp` = 1"
        Me.NonQuery(sql)
    End Sub
    
    
    Public Sub UpdateCkey(ByVal user As IRCUser, ByVal channel As IRCChannel)
        user.Key = user.UserParams.ToParameterString()
        Dim sql As String = "update `peerchat_join` set" &
                          "`join_flags` ='" & EscapeString(user.Key) & "'" &
                          " where `join_user` = " & user.ID & " and `join_channel` = " & channel.Id
        Me.NonQuery(sql)
    End Sub

    Public Function GetIRCChannel(ByVal channelName As String) As IRCChannel
        Dim sql As String = "select * from `peerchat_channels` where `channel_name` = '" & EscapeString(channelName) & "'"
        SyncLock Me.connection
            Using res As MySqlDataReader = Me.DoQuery(sql)
                res.Read()
                If res.HasRows() Then
                    Dim c As New IRCChannel()
                    c.Id = res("id")
                    If Not IsDBNull(res("channel_name")) Then c.ChannelName = res("channel_name")
                    If Not IsDBNull(res("channel_topic")) Then c.Topic = res("channel_topic")
                    If Not IsDBNull(res("channel_key")) Then c.Key = res("channel_key")
                    If Not IsDBNull(res("channel_host")) Then c.HostId = res("channel_host")
                    res.Close()
                    Return c
                Else
                    res.Close()
                    Return Nothing
                End If
            End Using
        End SyncLock
    End Function



    Public Function GetUserById(ByVal id As Integer, Optional ByVal channel As IRCChannel = Nothing) As IRCUser
        Dim sql As String = "select * from `v_peerchat_users`" &
            " where `id` = " & id.ToString()
        If Not channel Is Nothing Then
            sql &= " and `join_channel` = " & channel.Id.ToString()
        End If

        SyncLock Me.connection
            Using res As MySqlDataReader = Me.DoQuery(sql)
                res.Read()
                If res.HasRows() Then
                    Dim u As IRCUser = GetUser(res)
                    res.Close()
                    Return u
                End If
                res.Close()
            End Using
        End SyncLock
        Return Nothing
    End Function
    Public Function GetUserByClient(ByVal client As PeerChatClient) As IRCUser
        Return Me.GetUserById(client.ClientId)
    End Function
    Public Function GetUserByUniqueNick(ByVal uqNick As String, Optional ByVal channel As IRCChannel = Nothing) As IRCUser
        Dim sql As String = "select * from `v_peerchat_users`" &
        " where `user_nickname` = '" & EscapeString(uqNick) & "'"

        If Not channel Is Nothing Then
            sql &= " and `join_channel` = " & channel.Id.ToString()
        End If

        SyncLock Me.connection
            Using res As MySqlDataReader = Me.DoQuery(sql)
                res.Read()
                Dim u As IRCUser = GetUser(res)
                res.Close()
                Return u
            End Using
        End SyncLock
        Return Nothing
    End Function

    Public Function GetUserList(ByVal channel As IRCChannel) As List(Of IRCUser)
        'name is filtered as it's fetched from the db
        Dim sql As String = "select * from `v_peerchat_users` " &
            "where `join_channel` = " & channel.Id.ToString()

        SyncLock Me.connection
            Using res As MySqlDataReader = Me.DoQuery(sql)
                res.Read()
                If res.HasRows() Then
                    Dim users As New List(Of IRCUser)
                    Do
                        Dim u As IRCUser = Me.GetUser(res)
                        If Not u Is Nothing Then users.Add(u)
                    Loop While res.Read()
                    res.Close()
                    Return users
                Else
                    res.Close()
                    Return Nothing
                End If
            End Using
        End SyncLock
    End Function

    Private Function GetUser(ByVal r As MySqlDataReader) As IRCUser
        Dim u As New IRCUser
        If (IsDBNull(r("user_username"))) Then Return Nothing
        u.ID = r("id")
        u.GameSpyId = GetVal(r, "user_profile")
        u.NickName = GetVal(r, "user_nickname")
        u.UserName = GetVal(r, "user_username")
        u.KeyHash = GetVal(r, "user_keyhash")
        u.Key = GetVal(r, "join_flags")
        u.Role = GetVal(r, "join_role")

        If (Not IsDBNull(r("channel_name"))) Then
            u.Channel = New IRCChannel()
            u.Channel.ChannelName = GetVal(r, "channel_name")
            u.Channel.Id = GetVal(r, "join_channel")
            u.Channel.Key = GetVal(r, "channel_key")
        End If

        Return u
    End Function

    Private Function GetVal(ByVal r As MySqlDataReader, ByVal fieldName As String)
        If IsDBNull(r(fieldName)) Then
            Return String.Empty
        Else
            Return r(fieldName)
        End If
    End Function


    Public Sub RunCleanup()
        Dim sql As String = "update `peerchat_users` set `user_online` = 0  where `user_masterserver` = " & Me.MasterServerID.ToString()
        Me.NonQuery(sql)
        sql = "truncate `peerchat_join`"
        Me.NonQuery(sql)
    End Sub

    Public Sub ClearJoins(ByVal client As PeerChatClient)
        Dim sql As String = "delete from `peerchat_join` where " &
                          "`join_user` = " & client.ClientId.ToString()
        Me.NonQuery(sql)
    End Sub


    Public Sub JoinChannel(ByVal client As PeerChatClient, ByVal newChannel As IRCChannel, Optional ByVal role As String = "")
        Dim sql As String = "replace into `peerchat_join` set " &
                            "`join_channel` = " & newChannel.Id.ToString() & " , " &
                            "`join_user` = " & client.ClientId.ToString() & " , " &
                            "`join_role` = '" & EscapeString(role) & "'"
        Me.NonQuery(sql)
    End Sub

    Public Sub PartChannel(ByVal user As IRCUser, ByVal channel As IRCChannel)
        Dim sql As String = "delete from `peerchat_join` where " &
                        "`join_user` = " & user.ID.ToString() & " and `join_channel` = " & channel.Id.ToString
        Me.NonQuery(sql)
        If channel.HostId = user.ID Then
            Me.DeleteChannel(channel)
        End If
    End Sub


    Public Sub DeleteChannel(ByVal channel As IRCChannel)
        Dim sql As String = "delete from `peerchat_channels` where " &
                  "`id` = " & channel.Id

        Me.NonQuery(sql)
    End Sub

    Public Function MD5StringHash(ByVal strString As String) As String
        Dim MD5 As New MD5CryptoServiceProvider
        Dim Data As Byte()
        Dim Result As Byte()
        Dim Res As String = ""
        Dim Tmp As String = ""

        Data = System.Text.Encoding.ASCII.GetBytes(strString)
        Result = MD5.ComputeHash(Data)
        For i As Integer = 0 To Result.Length - 1
            Tmp = Hex(Result(i))
            If Len(Tmp) = 1 Then Tmp = "0" & Tmp
            Res += Tmp
        Next
        Return Res
    End Function
End Class