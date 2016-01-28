Public Class CoreConfig
    Public Property TCPQueryPort As Int32 = 6667            'Listen-Port for incoming TCP-connections
    Public Property TCPQueryAddress As String = "0.0.0.0"   'Listen-Address for incoming TCP-connections

    Public Property LogToFile As Boolean = False            'Enables File-logging
    Public Property LogFileName As String = "/log.txt"      'Lofile, Path relative to ./

    Public Property Loglevel As Byte = 2                    'Min. Loglevel

    Public Property MySQLHostname As String = "localhost"   'MySQL-Server hostname
    Public Property MySQLPort As Int32 = 3306               'MySQL-Server port
    Public Property MySQLDatabase As String = "gamemaster"  'Database name
    Public Property MySQLUsername As String = "root"        'Database user
    Public Property MySQLPwd As String = ""                 'User Password

    Public Property ServerID As Int32 = 0                   'Internal MS-ID for P2P

    Public Property P2PPort As Int32 = 14130                'Port for P2P-Communication
    Public Property P2PAddress As String = "0.0.0.0"        'Bind-Address for P2P-Communication
    Public Property P2PEnable As Boolean = False            'Enable P2P?
    Public Property P2PKey As String = "abcd"               'Key for P2P-Encryption

End Class