'Static Logger
'JW "LeKeks" 05/2014
Imports System.IO

Public Enum LogLevel
    Verbose = 1
    Info = 2
    Warning = 3
    Exception = 4
    Protocol = 100
End Enum

Public Class Logger
    Public Shared Property LogToFile As Boolean = False
    Public Shared Property LogFileName As String = "\log.txt"
    Public Shared Property MinLogLevel As Byte = 2
    Private Shared writer As StreamWriter = Nothing

    Public Shared Sub OpenLogFile()
        writer = New StreamWriter(CurDir() & LogFileName)
    End Sub

    Public Shared Sub CloseLogFile()
        If Not writer Is Nothing Then
            writer.Close()
            writer = Nothing
        End If
    End Sub

    Public Shared Sub Log(ByVal message As String, ByVal level As LogLevel, ParamArray tags() As String)
        If Not level >= MinLogLevel And Not DEBUGMODE_ENABLE Then Return

        For i = 0 To tags.Length - 1
            message = message.Replace("{" & i.ToString() & "}", tags(i))
        Next

        Select Case level
            Case LogLevel.Verbose
                message = "DEBUG | " & message
            Case LogLevel.Warning
                message = "WARN  | " & message
            Case LogLevel.Exception
                message = "EX    | " & message
            Case LogLevel.Info
                message = "INFO  | " & message
        End Select

        message = "[" & Now.ToString() & "] " & message

        Console.WriteLine(message)
        Debug.WriteLine(message)

        If LogToFile = True Then
            SyncLock writer
                writer.WriteLine(message)
                writer.Flush()
            End SyncLock
        End If
        If level = LogLevel.Exception Then
            'Console.ReadLine()
            End 'Just exit for now, will savely relaunch
        End If
    End Sub

End Class