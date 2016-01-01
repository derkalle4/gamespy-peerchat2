Imports System.Security.Cryptography
Imports System.Text
Public Class WhoPacket
    Inherits PeerChatPacket

    Sub New(ByVal client As PeerChatClient)
        MyBase.New(client)
    End Sub

    Private queryName As String = String.Empty

    Public Overrides Function ManageData() As Boolean
        If Me.Params.Length < 1 Then Return IGNORE_SYNTAX_ERR
        Me.client.SendPacket(Me)
        Dim wep As New WhoEndPacket(Me.client)
        Me.client.SendPacket(wep)
        Return True
    End Function

    Public Overrides Function CompileResponse() As String
        Dim u As IRCUser = Me.client.server.MySQL.GetUserByUniqueNick(Params(0))
        'TODO: fix
        If u.NickName.Contains("test") Then
            u.NickName = "@" & u.NickName
            u.UserName = "192.168.178.35"
        Else
            u.UserName = "192.168.178.57"
        End If

        Return IRCStdFormat(IRC_RES_WHO,
                            u.Channel.ChannelName,
                            u.NickName,
                            u.UserName,
                            "s",
                            u.NickName,
                            "*",
                            ":0",
                            MD5StringHash(u.KeyHash))
    End Function


    Public Function MD5StringHash(ByVal strString As String) As String
        Dim MD5 As New MD5CryptoServiceProvider
        Dim Data As Byte()
        Dim Result As Byte()
        Dim Res As String = ""
        Dim Tmp As String = ""

        Data = Encoding.ASCII.GetBytes(strString)
        Result = MD5.ComputeHash(Data)
        For i As Integer = 0 To Result.Length - 1
            Tmp = Hex(Result(i))
            If Len(Tmp) = 1 Then Tmp = "0" & Tmp
            Res += Tmp
        Next
        Return Res
    End Function
End Class
