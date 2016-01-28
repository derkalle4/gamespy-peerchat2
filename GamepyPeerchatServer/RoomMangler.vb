'GS Peerchat Roomname-mangler .NET-Implementation
'JW "LeKeks" 05/2015
Public Class RoomMangler

    Private Const hexCards As String = "0123456789abcdef"
    Private Const baseCards As String = "aFl4uOD9sfWq1vGp"
    Private Const newBaseCards As String = "qJ1h4N9cP3lzD0Ka"
    Private Const ip_xormask As UInt32 = &HC3801DC7

    Public Shared Function GetRoomHash(ByVal publicIP As Byte(), ByVal privateIP As Byte(), ByVal port As UInt32) As String
        Array.Reverse(publicIP)
        Array.Reverse(privateIP)
        Dim pba As UInt32 = BitConverter.ToUInt32(publicIP, 0)
        Dim pra As UInt32 = BitConverter.ToUInt32(privateIP, 0)

        'The encoding-base is generated using public ip, localip and the local port
        Dim res As UInt32
        res = (((pra >> 24) And &HFF) Or ((pra >> 8) And &HFF00) Or ((pra << 8) And &HFF0000) Or ((pra << 24) And &HFF000000))
        res = (res Xor pba)
        res = (res Xor port Or (port << 16))

        Return String.Format("M{0}M", EncodeIP(res, True))
    End Function

    Public Shared Function EncodeIP(ByVal ip As UInt32, ByVal newCrypt As Boolean) As String
        Dim cards As Byte() = GetBytes(IIf(newCrypt, newBaseCards, baseCards).ToString())
        Dim hexc As Byte() = GetBytes(hexCards)
        ip = ip Xor ip_xormask

        Dim cryptBuffer() As Byte = GetBytes(String.Format("{0,8:X8}", ip).ToLower())
        Dim digit_idx As Int32

        For i = 0 To 7
            digit_idx = Array.IndexOf(hexc, cryptBuffer(i))
            If (digit_idx < 0 Or digit_idx > 15) Then
                cryptBuffer = GetBytes("14saFv19") '/0
                Exit For
            End If
            cryptBuffer(i) = cards(digit_idx)
        Next

        Return GetString(cryptBuffer)
    End Function

End Class