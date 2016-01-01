'GS Peerchat Cipher Algorithm .NET Implementation
'JW "LeKeks" 11/2014
Public Structure DESCryptKey
    Dim key() As Byte
    Dim defaultKey() As Byte
    Dim state() As Byte
    Dim x As Byte
    Dim y As Byte
End Structure

Module DESCipher
    Private Sub RefSwap(ByRef x As Byte, ByRef y As Byte, ByRef swaptemp As Byte)
        'Swaps two bytes by reference, uses static tempstore var so we don't need to allocate more memory
        swaptemp = x
        x = y
        y = swaptemp
    End Sub

    Public Sub DESPrepareKey(ByVal cryptKey() As Byte, ByVal keyLen As Int32, ByRef key As DESCryptKey)
        Dim index1 As Byte = 0
        Dim index2 As Byte = 0
        Dim swaptmp As Byte

        Array.Resize(key.state, 256)

        'Fill with descending cards
        For i = 0 To 255
            key.state(255 - i) = i
        Next

        key.x = 0
        key.y = 0

        'Some "randomness" with modulo
        For i = 0 To 255
            index2 = Convert.ToByte((cryptKey(index1) + key.state(i) + index2) Mod 256)
            RefSwap(key.state(i), key.state(index2), swaptmp)
            index1 = Convert.ToByte((index1 + 1) Mod keyLen)
        Next
    End Sub

    Public Sub DESCrypt(ByRef buffer() As Byte, ByVal bufferLen As Int32, ByRef key As DESCryptKey)
        Dim swaptemp, x, y, xorIndex As Byte
        x = key.x
        y = key.y

        For i = 0 To bufferLen - 1
            x = Convert.ToByte((x + 1) Mod 256)
            y = Convert.ToByte((key.state(x) + y) Mod 256)
            RefSwap(key.state(x), key.state(y), swaptemp)
            xorIndex = Convert.ToByte((key.state(x) + key.state(y)) Mod 256)
            buffer(i) = buffer(i) Xor key.state(xorIndex)
        Next

        key.x = x
        key.y = y
    End Sub

    Public Sub DESXCodeBuffer(ByRef buffer() As Byte, ByVal bufferLen As Int32, ByVal enckey() As Byte)
        For i = 0 To bufferLen - 1
            Dim b As Byte = i Mod (enckey.Length)
            buffer(i) = buffer(i) Xor enckey(b)
        Next
    End Sub

    Public Function DESGenerateKey(ByVal sKey() As Byte) As DESCryptKey
        Dim xKey As New DESCryptKey()
        xKey.key = ArrayFunctions.GetBytes(DESCipher.RandStr(16))
        xKey.defaultKey = {}
        Array.Resize(xKey.defaultKey, xKey.key.Length)
        Array.Copy(xKey.key, xKey.defaultKey, xKey.key.Length)
        DESCipher.DESXCodeBuffer(xKey.key, xKey.key.Length, sKey)
        DESCipher.DESPrepareKey(xKey.key, xKey.key.Length, xKey)
        Return xKey
    End Function

    Private Function RandStr(ByVal length As Int32) As String
        Dim rand As String = String.Empty
        Dim chars() As String =
              {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", _
               "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "X", _
               "1", "2", "3", "4", "5", "6", "7", "8", "9", "0"}

        For i = 1 To length
            rand &= chars(Int(Rnd() * (chars.Length - 1)))
        Next

        Return rand
    End Function
End Module