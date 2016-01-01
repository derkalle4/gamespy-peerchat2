Module constants
    'Generic
    Public Const PRODUCT_NAME As String = "GameMaster Peerchat Server"
    Public Const CFG_DIR As String = "/cfg"                     'Dir for Config files
    Public Const CFG_FILE As String = "/core.xml"               'Main Config File

    'TCP-Protocol
    Public Const TCP_CLIENT_TIMEOUT As Int32 = 2000             'Timeout for TCP-clients
    Public Const TCP_CLIENT_PSH_MAXCOUNT As Int32 = 10          'Max tcp-push p. client
    Public Const TCP_CLIENT_PSH_SLEEP As Int32 = 10             'Min. sleeptime btwn. PSH
    Public Const TCP_CLIENT_BUFFERSIZE As Int32 = 1024          'max. message length

    Public Const IRC_SPLITKEY As String = " "
    Public Const IRC_SERVER_REPLY_FORMAT As String = ":s {0} {1}"
    Public Const IRC_USER_REPLY_FORMAT As String = ":{0} {1} {2}"

    'Response formats
    Public Const IRC_FORMAT_USRIP As String = ":=+@{0}"

    Public Const IRC_STDRESPONSE_FORMAT As String = ":s {0} {1}"
    Public Const IRC_RAWRESPONSE_FORMAT As String = "{0} {1}"

    'IRC Response Codes
    Public Const IRC_RES_CRYPT As String = "705"
    Public Const IRC_RES_USRIP As String = "302"
    Public Const IRC_RES_WELCOME As String = "001"
    Public Const IRC_RES_MOTD As String = "375"
    Public Const IRC_RES_MOTD_END As String = "376"
    Public Const IRC_RES_LOGIN_OK As String = "707"
    Public Const IRC_RES_LOGIN_FAIL As String = "708"
    Public Const IRC_RES_CDKEY As String = "706"
    Public Const IRC_RES_WHO As String = "352"
    Public Const IRC_RES_WHOEND As String = "315"
    Public Const IRC_RES_TOPIC As String = "332"
    Public Const IRC_RES_NOTOPIC As String = "331"

    Public Const IRC_RES_NAMELIST As String = "353"
    Public Const IRC_RES_NAMELISTEND As String = "366"
    Public Const IRC_RES_GETCKEY As String = "702"
    Public Const IRC_RES_GETCKEYEND As String = "703"
    Public Const IRC_RES_GETCHANKEY As String = "704"

    Public Const IRC_RES_NOSUCHCHANNEL = "403"

    'IRC plain commands
    Public Const IRC_CMD_CRYPT As String = "CRYPT"
    Public Const IRC_CMD_USRIP As String = "USRIP"
    Public Const IRC_CMD_USER As String = "USER"
    Public Const IRC_CMD_NICK As String = "NICK"
    Public Const IRC_CMD_LOGIN As String = "LOGIN"
    Public Const IRC_CMD_CDKEY As String = "CDKEY"
    Public Const IRC_CMD_QUIT As String = "QUIT"
    Public Const IRC_CMD_JOIN As String = "JOIN"
    Public Const IRC_CMD_WHO As String = "WHO"
    Public Const IRC_CMD_PING As String = "PING"
    Public Const IRC_CMD_PONG As String = "PONG"
    Public Const IRC_CMD_PART As String = "PART"
    Public Const IRC_CMD_MODE As String = "MODE"
    Public Const IRC_CMD_GETCKEY As String = "GETCKEY"
    Public Const IRC_CMD_SETCKEY As String = "SETCKEY"
    Public Const IRC_CMD_TOPIC As String = "TOPIC"
    Public Const IRC_CMD_SETCHANKEY As String = "SETCHANKEY"
    Public Const IRC_CMD_GETCHANKEY As String = "GETCHANKEY"
    Public Const IRC_CMD_UTM As String = "UTM"
    Public Const IRC_CMD_PRIVMSG As String = "PRIVMSG"

    'IRC payload messages
    Public Const IRC_WHOEND_MSG As String = "End of Who"
    Public Const IRC_MOTD_MSG As String = "[motd] I like cookies :3"
    Public Const IRC_WELCOME_MSG As String = "Hello, {0}. Nice to meet you."
    Public Const IRC_NOTOPIC_MSG As String = "No Topic set."
    Public Const IRC_NOSUCHCHANNEL_MSG As String = "No such channel."
    Public Const IRC_NAMELISTEND_MSG As String = "End of NAMES list."
    Public Const IRC_GETCKEYEND_MSG As String = "End of GETCKEY"
    Public Const IRC_QUIT_MSG As String = "See ya!"

    'CDKey-Authentification
    Public Const GS_CDKEY_OK_MSG As String = "Authenticated"
    Public Const GS_CDKEY_OK_ID As Byte = 1
    Public Const GS_CDKEY_FAIL_MSG As String = "Failed"
    Public Const GS_CDKEY_FAIL_ID As Byte = 0

    'GS channel format
    Public Const GS_CHANNEL_PREFIX As String = "#"
    Public Const GS_CCREATE_FORMAT As String = "#GSP!{0}!"
    Public Const GS_ACCOUNT_SUFFIX As String = "-gs"

    'Multiserver-Protocol
    Public Const P2P_CMD_PING As Byte = &H0                     'Echo request
    Public Const P2P_CMD_PONG As Byte = &H1                     'Echo reply

    Public Const P2P_CMD_SENDMESSAGE As Byte = &H10             'Forward UDP packet
    Public Const P2P_CMD_IRCRELAY As Byte = &H20                'Forward IRC event

    Public Const P2P_FLAG_BCAST As Byte = 1 << 0
    Public Const P2P_FLAG_BCAST_TOSENDER As Byte = 1 << 1
    Public Const P2P_FLAG_USECIPHER As Byte = 1 << 2

    'Debugging
    Public Const DEBUGMODE_ENABLE As Boolean = True     'Enables verbose logging w/o config
    Public Const IGNORE_SYNTAX_ERR As Boolean = True    'don't kick client on faulty command

End Module