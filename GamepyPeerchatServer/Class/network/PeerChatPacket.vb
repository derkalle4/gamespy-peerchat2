Public Enum CommandType
    raw = 1
    server = 2
    user = 3
    nocipher = 4
End Enum

Public Class PeerChatPacket

    Public Property Command As String
    Public Property Payload As String
    Public Property Params As String() = {}
    Public Property UseCipher As Boolean = True

    Public Property RawData As String
        Get
            Return Me._rawData
        End Get
        Set(value As String)
            Me._rawData = value
            Dim pStr() As String = Split(Me._rawData, " ")
            Array.Resize(Me.Params, pStr.Length - 1)
            Array.Copy(pStr, 1, Me.Params, 0, pStr.Length - 1)
            'The IRC packet's payload begins after a :-indicator and extends until the end of the line
            Dim plIndex As Integer = InStr(Me._rawData, ":")
            If plIndex > 0 Then
                Me.Payload = Mid(Me._rawData, plIndex + 1, Me._rawData.Length - plIndex)
            End If
        End Set
    End Property

    Private _rawData As String = String.Empty
    Friend client As PeerChatClient

    Sub New(ByVal client As PeerChatClient)
        Me.client = client
    End Sub

    Public Overridable Function CompileResponse() As String
        Return String.Empty
    End Function

    Public Overridable Function ManageData() As Boolean
        Return True
    End Function

    Friend Function IRCStdFormat(ByVal command As String, ByVal ParamArray params() As String)
        Dim paramStr As String = String.Empty
        For i = 0 To params.Length - 1
            paramStr &= params(i) & " "
        Next
        If paramStr <> String.Empty Then paramStr = Mid(paramStr, 1, paramStr.Length - 1)
        Return String.Format(IRC_STDRESPONSE_FORMAT, command, paramStr)
    End Function

    Friend Function IRCUserFormat(ByVal command As String, ByVal ParamArray params() As String)
        Dim paramStr As String = String.Empty
        For i = 0 To params.Length - 1
            paramStr &= params(i) & " "
        Next

        If paramStr <> String.Empty Then paramStr = Mid(paramStr, 1, paramStr.Length - 1)
        Dim formatedCommand As String = String.Empty

        'TODO: check if I can replace * with the real host
        If client.NickName <> String.Empty Then
            formatedCommand = ":" & client.NickName & "!" & client.UserName
            If client.GameSpyLoggedIn Then formatedCommand &= "|" & client.GameSpyAccountID.ToString()
            formatedCommand &= "@* " & command
        End If

        Return String.Format(IRC_RAWRESPONSE_FORMAT, formatedCommand, paramStr)
    End Function
End Class