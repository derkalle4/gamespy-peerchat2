Module DebugHelper
    Public Function BuildNiceString(ByVal data() As Byte) As String
        Dim line1 As String = String.Empty
        Dim line2 As String = String.Empty

        For i = 0 To data.Length - 1
            Dim charAsString As String = data(i).ToString
            If charAsString.Length = 1 Then
                line1 &= "00"
            ElseIf charAsString.Length = 2 Then
                line1 &= "0"
            End If
            line1 &= charAsString & ":"

            line2 &= ArrayFunctions.GetString({data(i)}) & "   "
        Next
        Return line1 & vbCrLf & line2
    End Function

    Public Function BuildInlineArray(ByVal data() As Byte) As String
        Dim line1 As String = String.Empty
        Dim line2 As String = String.Empty

        For i = 0 To data.Length - 1
            Dim charAsString As String = data(i).ToString
            line1 &= charAsString & ","
        Next
        line1 = Mid(line1, 1, line1.Length - 1)
        Return "{" & line1 & "}"
    End Function

End Module