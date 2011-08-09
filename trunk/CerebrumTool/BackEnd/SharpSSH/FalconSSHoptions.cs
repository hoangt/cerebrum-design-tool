using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;


namespace Tamir.SharpSsh
{
    class FalconSSHoptions
    {
        #region Fields
        //Retrieved from http://www.ssh.com/support/documentation/online/ssh/winadminguide/32/Configuration_File_Reference.html#SEC-REFERENCE
        //on 5/31/10
        private string _AllowedAuthentications = "publickey,password";        
        private bool _AllowHosts;
        private bool _AllowTcpForwarding = false;
        private bool _AllowTcpForwardingForUsers;
        private Hashtable _AllowUsers;
        private struct _AuthKbd
        {
            public int NumOptional;
            public Hashtable Optional;
            public string Plugin;
            public Hashtable Required;
            public int Retries;
        }
        private _AuthKbd _AuthKbdInt;
        private string _AuthorizationFile;
        private string _BannerMessageFile;
        //!!private bool _Cert.RSA.Compat.HashScheme -->Possibly antiquated, run it by the others
        private string _Ciphers = "AnyStdCipher";
        private Hashtable _DenyHosts;
        private Hashtable _DenyTcpForwardingForUsers;
        private Hashtable _DenyUsers;
        private bool _DisableVersionFallback = false;
        private bool _DoubleBackSpace = true;
        private Array _EventLogFilter; //Three booleans: information, warning, and error
        private string _HostCertificateFile;
        private string _HostKeyFile;
        private string _HostSpecificConfig;
        private int _IdleTimeout;
        private bool _KeepAlive = true;
        private Array _LdapServers;
        private int _ListenAddress;
        private int _LoginGraceTime;
        private Array _MACs;
        private string _MapFile;
        private int _MaxConnections;
        private int _MaxBroadcastsPerSecond;
        private bool _NoDelay = false;
        private int _PasswordGuesses = 3;
        private bool _PermitEmptyPasswords = false;
        private bool _PermitRootLogin = true;
        private bool _PermitUserTerminal = true;
        private int _Pki;
        private bool _PkiDisableCrls = false;
        private uint _Port = 22;
        private bool _PrivateWindowsStation = true;
        private string _ProtocolVersionString = "SSH-2.0-3.2.0 SSH Secure Shell Windows NT Server";
        private string _PublicHostKeyFile;
        private string _RandomSeedFile = "server_random_seed";
        private int _RekeyIntervalSeconds = 0;
        private Array _RequiredAuthentications;
        private bool _RequireReverseMapping = false;
        private bool _ResolveClientHostName = true;
        private string _SftpAdminDirList = "HOME=%D, C:=C:, D:=D:";
        private string _SftpAdminUsers;
        private string _SftpDirList = "HOME=%D";
        private string _SftpHome = "%D";
        private int _SftpLogCategory = 16;
        private string _SocksServer;
        private string _TerminalProvider = "cmd.exe";
        private string _UserConfigDirectory = "%D/.ssh2";
        private string _UserSpecificConfig;
        private int _ZombieTimeout = 15;
        #endregion

        #region Properties
        /// <summary>
        /// This keyword specifies the authentications methods 
        /// that are allowed. This is a comma-separated list 
        /// currently consisting of the following words: password,
        /// publickey, and keyboard-interactive. Each specifies an 
        /// authentication method. The default is "publickey,password"
        ///With RequiredAuthentications, the administrator can 
        ///force users to complete several authentications before 
        ///they are considered authenticated. 
        /// </summary>
        public string AllowedAuthentications
        {
            get { return _AllowedAuthentications; }
            set { _AllowedAuthentications = value; }
        }
        /// <summary>
        /// This keyword can be followed by any number of host name 
        /// patterns, separated by commas. If specified, login is allowed 
        /// only from hosts whose name matches one of the patterns. 
        /// Patterns are matched using the egrep syntax.
        /// Normal name servers are used to map the client's host into 
        /// a canonical host name. If the name cannot be mapped, its 
        /// IP address is used as the host name. By default all hosts 
        /// are allowed to connect. 
        /// Also note that to prevent people going around this parameter 
        /// and logging in from hosts like 130.233.evilcrackers.org, 
        /// you should use the [:isdigit:] or similar constructs. 
        /// Alternatively, the prefix \i can be used in the pattern to 
        /// specify that the pattern should only match IP addresses. For 
        /// example, \i10\.1\.0\.25 would match only to host 
        /// 10.1.0.25. 
        /// </summary>
        public bool AllowHosts
        {
            get { return _AllowHosts; }
            set { _AllowHosts = value; }
        }
        /// <summary>
        /// Specifies whether TCP forwarding is permitted. Note that 
        /// disabling TCP forwarding does not improve security in any 
        /// way, as users can always install their own forwarders. The 
        /// argument must be yes or no. The default value is no. 
        /// </summary>
        public bool AllowTcpForwarding
        {
            get { return _AllowTcpForwarding; }
            set { _AllowTcpForwarding = value; }
        }
        /// <summary>
        /// Syntax is the same as in AllowUsers, but instead of login, this 
        /// controls the ability to forward ports, in remote or local 
        /// forwarding. Note that disabling TCP forwarding does not 
        /// improve security in any way, as users can always install 
        /// their own forwarders. This does, however, help if you deny 
        /// the user terminal access at the same time. 
        /// </summary>
        public bool AllowTcpForwardingForUsers
        {
            get { return _AllowTcpForwardingForUsers; }
            set { _AllowTcpForwardingForUsers = value; }
        }
        /// <summary>
        ///This keyword can be followed by any number of user name 
        ///patterns or user@host patterns, separated by commas. Host 
        ///name is handled as a pattern, so rules below apply. Host 
        ///name can also be a pure DNS name or the IP address. If 
        ///specified, login is allowed as users whose name matches one 
        ///of the patterns. Patterns are matched using the egrep 
        ///syntax (see section Host and User Restriction Syntax).
        ///
        ///You can use the comma (,) character in the patterns by 
        ///escaping it with backslash (\). If you want to use the 
        ///escape character in the patterns, you have to escape it 
        ///(\\). By default, logins as all users are allowed. 
        ///
        ///Note that the all other login authentication steps must 
        ///still be successfully completed. AllowUsers and DenyUsers 
        ///are additional restrictions. 
        /// </summary>
        public Hashtable AllowUsers
        {
            get { return _AllowUsers; }
            set { _AllowUsers = value; }
        }
        /// <summary>
        /// AuthKbdInt.NumOptional
        /// Specifies how many optional submethods must be passed 
        /// before the authentication is considered a success (note 
        /// that all required submethods must always be passed). See 
        /// AuthKbdInt.Optional for specifying optional submethods, 
        /// and AuthKbdInt.Required for required submethods. The 
        /// default is 0, although if no required submethods are 
        /// specified, the client must always pass at least one 
        /// optional submethod. 
        /// 
        /// AuthKbd.Optional
        /// Specifies the optional submethods Keyboard-Interactive will 
        /// use. Currently submethods securid, plugin, and password 
        /// are defined. Note that SecurID requires RSA ACE/Server or 
        /// RSA ACE/Agent to be installed and configured on the system. 
        /// AuthKbdInt.NumOptional specifies how many optional 
        /// submethods must be passed. The Keyboard-Interactive 
        /// authentication method is considered a success when the 
        /// specified amount of optional submethods and all required 
        /// submethods are passed. The plugin sub- method is special, 
        /// it can be used if a sysadmin wants to create a new 
        /// authentication method. See the option AuthKbdInt.Plugin. 
        /// See also AuthKbdInt.NumOptional and AuthKbdInt.Required. 
        /// 
        /// AuthKbdInt.Plugin
        /// Specify this to point to a program which is used by the 
        /// plugin submethod in Keyboard-Interactive. SSH Secure Shell 
        /// converses with this program using a line-based protocol, 
        /// so it is easy to implement it. If the plugin submethod is 
        /// used, and this is not set, or the specified program does 
        /// not exist, or cannot be run, the sub-method will fail, 
        /// which may cause the whole authentication for the user to 
        /// fail. This will not be set by default. 
        /// 
        /// AuthKbdInt.Required
        /// Specifies the required submethods that must be passed before 
        /// the keyboard-interactive authentication method can succeed. 
        /// See AuthKbdInt.Optional. 
        /// 
        /// Auth.KbdInt.Retries
        /// Specifies how many times the user can retry Keyboard-
        /// Interactive. The default is 3
        /// </summary>
        private _AuthKbd AuthKbdInt
        {
            get { return _AuthKbdInt; }
            set { _AuthKbdInt = value; }
        }
        /// <summary>
        /// Specifies the name of the user's authorization file.
        /// </summary>
        public string AuthorizationFile
        {
            get { return _AuthorizationFile; }
            set { _AuthorizationFile = value; }
        }
        /// <summary>
        /// Banner message that is displayed in the client before the 
        /// login. Please note that this file should be located on a 
        /// local drive. Network or mapped drives should not be used, 
        /// as the server program may not have proper access rights 
        /// for them. 
        /// </summary>
        public string BannerMessageFile
        {
            get { return _BannerMessageFile; }
            set { _BannerMessageFile = value; }
        }
        /// <summary>
        /// Specifies the ciphers to use for encrypting the session. 
        /// Currently, DES, 3DES, Blowfish, Arcfour, Twofish, and 
        /// CAST-128 are supported. Multiple ciphers can be specified 
        /// as a comma-separated list. Special values to this option 
        /// are Any, AnyStd, that allows only standard (see below) 
        /// ciphers, and AnyCipher that allows either any available 
        /// cipher or excludes nonencrypting cipher mode none but 
        /// allows all others. AnyStdCipher is the same as above, 
        /// but includes only those ciphers mentioned in the 
        /// IETF-SecSH-draft (excluding 'none'). The default is 
        /// AnyStdCipher. 
        /// </summary>
        public string Ciphers
        {
            get { return _Ciphers; }
            set { _Ciphers = value; }
        }
        /// <summary>
        /// This keyword can be followed by any number of host name 
        /// patterns, separated by commas. If specified, login is 
        /// disallowed from the hosts whose name matches any of the 
        /// patterns. See AllowHosts. 
        /// </summary>
        public Hashtable DenyHosts
        {
            get { return _DenyHosts; }
            set { _DenyHosts = value; }
        }
        /// <summary>
        /// The syntax is the same as in DenyUsers, but instead of 
        /// login, this controls the ability to forward ports, in 
        /// remote or local forwarding. Note that disabling TCP 
        /// forwarding does not improve security in any way, as users 
        /// can always install their own forwarders. This does, 
        /// however, help if you deny the user terminal access at the 
        /// same time. 
        /// </summary>
        public Hashtable DenyTcpForwardingForUsers
        {
            get { return _DenyTcpForwardingForUsers; }
            set { _DenyTcpForwardingForUsers = value; }
        }
        /// <summary>
        /// This keyword can be followed by any number of user name 
        /// patterns or user@host patterns, separated by commas. Host 
        /// name is handled as a pattern, so rules below apply. Host 
        /// name can also be a pure DNS name or the IP address. 
        ///If specified, login is disallowed as users whose name 
        ///matches one of the patterns. Patterns are matched using the 
        ///egrep syntax (see section Host and User Restriction Syntax). 
        ///By default, logins by all users are allowed. 
        ///If a user's name matches with both a pattern in DenyUsers 
        ///and AllowUsers, login is denied. 
        ///Note that the all other login authentication steps must 
        ///still be successfully completed. AllowUsers and DenyUsers 
        ///are additional restrictions. 
        /// </summary>
        public Hashtable DenyUsers
        {
            get { return _DenyUsers; }
            set { _DenyUsers = value; }
        }
        /// <summary>
        /// Whether to disable fallback compatibility code for older, or 
        /// otherwise incompatible versions of the software. Do not 
        /// disable unless you know what you are doing. The default 
        /// value is no. 
        /// </summary>
        public bool DisableVersionFallback
        {
            get { return _DisableVersionFallback; }
            set { _DisableVersionFallback = value; }
        }
        /// <summary>
        /// Relevant when the Windows Server is being run on a Japanese 
        /// Windows platform. Change this setting if a client does not 
        /// display backspace correctly in its terminal window. When 
        /// set to yes: when a backspace is pressed on the client, 
        /// server replies with two backspace characters for each
        /// twobyte Japanese character. When set to no: when a backspace is 
        /// pressed on the client, server replies with one backspace 
        /// character for each twobyte Japanese character. The default 
        /// is yes. 
        /// </summary>
        public bool DoubleBackSpace
        {
            get { return _DoubleBackSpace; }
            set { _DoubleBackSpace = value; }
        }

        /// <summary>
        /// Specifies the filter for event log messages. The values are 
        /// information, warning, and error, respectively. It is a boolean
        /// array.
        /// </summary>
        public Array EventLogFilter
        {
            get { return _EventLogFilter; }
            set { _EventLogFilter = value; }
        }
        /// <summary>
        /// This keyword works very much like PublicHostKeyFile, except 
        /// that the file is assumed to contain an X.509 certificate 
        /// in binary format. The keyword must be paired with a 
        /// corresponding HostKeyFile option. If multiple certificates 
        /// with same public key type (DSS or RSA) are specified, only 
        /// the first one is used. 
        /// </summary>
        public string HostCertificateFile
        {
            get { return _HostCertificateFile; }
            set { _HostCertificateFile = value; }
        }
        /// <summary>
        /// Specifies the file containing the private host key. 
        /// Please note that this file should be located on a local 
        /// drive. Network or mapped drives should not be used, as the 
        /// server program may not have proper access rights for them. 
        /// The default is the hostkey file located in the Secure 
        /// Shell installation directory. 
        /// </summary>
        public string HostKeyFile
        {
            get { return _HostKeyFile; }
            set { _HostKeyFile = value; }
        }
        /// <summary>
        /// Specifies a subconfiguration file. The syntax for this 
        /// option is pattern subconfig-file, where pattern will be 
        /// used to match the client host, as specified under option 
        /// AllowHosts. The file subconfig- file will then be read, 
        /// and configuration data amended accordingly. The file is 
        /// read before any actual protocol transactions begin, and 
        /// you can specify most of the options allowed in the main 
        /// configuration file. You can specify more than one 
        /// subconfiguration file, in which case the patterns are 
        /// matched and the files read in the order specified. Later 
        /// defined values of configuration options will either 
        /// override or amend the previous value depending on which 
        /// option it is. The effect of redefining an option is 
        /// described in the documentation for that option. For 
        /// example,setting Ciphers in the subconfiguration file will 
        /// override the old value, but setting AllowUsers will amend 
        /// the value. See Section Subconfiguration for information 
        /// on what you can set in the subconfiguration file. 
        /// </summary>
        public string HostSpecificConfig
        {
            get { return _HostSpecificConfig; }
            set { _HostSpecificConfig = value; }
        }
        /// <summary>
        /// Specifies the allowed time (in seconds) after which the 
        /// server disconnects if there is no activity from the user. 
        /// If set to 0 (zero), idle timeout is disabled. 
        /// </summary>
        public int IdleTimeout
        {
            get { return _IdleTimeout; }
            set { _IdleTimeout = value; }
        }
        /// <summary>
        /// Specifies whether the system should send keepalive messages 
        /// to the other side. If they are sent, termination of the 
        /// connection or crash of one of the machines will be 
        /// properly noticed. However, this means that connections 
        /// will die if the route is down temporarily, and this can be 
        /// annoying in some situations. On the other hand, if 
        /// keepalive messages are not send, sessions may hang 
        /// indefinitely on the server, leaving "ghost" users and 
        /// consuming server resources. 
        /// The default is yes (to send keepalive messages), and the 
        /// server will notice if the network goes down or the client 
        /// host reboots. This avoids infinitely hanging sessions. 
        /// To disable keepalives, the value should be set to no in 
        /// both the server and the client configuration files. 
        /// </summary>
        public bool KeepAlive
        {
            get { return _KeepAlive; }
            set { _KeepAlive = value; }
        }
        /// <summary>
        /// CRLs are automatically retrieved from the CRL distribution 
        /// point defined in the certificate to be checked, if it 
        /// exists. Otherwise the comma-separated server list given 
        /// by option LdapServers is used. If intermediate CA 
        /// certificates are needed in certificate validity checking, 
        /// this option must be used or retrieving the certificates 
        /// will fail. 
        /// </summary>
        public Array LdapServers
        {
            get { return _LdapServers; }
            set { _LdapServers = value; }
        }
        /// <summary>
        /// Specifies the IP address of the network interface card 
        /// where the Secure Shell server socket is bound. The server 
        /// has to be restarted in order to use the changed setting. 
        /// </summary>
        public int ListenAddress
        {
            get { return _ListenAddress; }
            set { _ListenAddress = value; }
        }
        /// <summary>
        /// The server disconnects after this time if the user has 
        /// not successfully logged in. If the value is 0 (zero), 
        /// there is no time limit. The default is 600 seconds (10 
        /// minutes). 
        /// </summary>
        public int LoginGraceTime
        {
            get { return _LoginGraceTime; }
            set { _LoginGraceTime = value; }
        }
        /// <summary>
        /// Specifies the MAC (Message Authentication Code) algorithm 
        /// to use for data integrity verification. Currently, 
        /// hmac-sha1, hmac-sha1-96, hmac- md5, hmac-md5-96, 
        /// hmac-ripemd160, and hmac-ripemd160-96 are supported, of 
        /// which hmac-sha1, hmac-sha1-96, hmac-md5, and hmac-md5-96 
        /// are included in all distributions. Multiple MACs can be 
        /// specified as a comma-separated list. 
        /// Special values to this option are Any, AnyStd, that allows 
        /// only standard (see below) MACs, and AnyMac that allows 
        /// either any available MAC or excludes none but allows all 
        /// others. AnyStdMac is the same as above, but includes only 
        /// those MACs mentioned in the IETF-SecSH-draft (excluding 
        /// none). AnyStdMac is the default. 
        /// </summary>
        public Array MACs
        {
            get { return _MACs; }
            set { _MACs = value; }
        }
        /// <summary>
        /// This keyword specifies a mapping file for the preceding 
        /// Pki keyword. Multiple mapping files are permitted per one 
        /// Pki keyword. The mapping file format is described in 
        /// section Certificate User Mapping File. 
        /// </summary>
        public string MapFile
        {
            get { return _MapFile; }
            set { _MapFile = value; }
        }
        /// <summary>
        /// Specifies the maximum number of connections SSH Secure 
        /// Shell for Windows Servers will handle simultaneously.
        /// This is useful in systems where spamming the server 
        /// program with new connections can cause the system to 
        /// become unstable or crash. The argument is a positive 
        /// number. A value of 0 (zero) means that number of 
        /// connections is unlimited (by the program). The server has 
        /// to be restarted in order to use the changed setting. 
        /// </summary>
        public int MaxConnections
        {
            get { return _MaxConnections; }
            set { _MaxConnections = value; }
        }
        /// <summary>
        /// Specifies how many UDP broadcasts server handles per 
        /// second. The default value is 0 (zero), meaning that no 
        /// broadcasts are handled at all. Broadcasts that exceed the 
        /// limit are silently ignored. Also unrecognized UDP 
        /// datagrams received consume the capacity defined by this 
        /// option. 
        /// </summary>
        public int MaxBroadcastsPerSecond
        {
            get { return _MaxBroadcastsPerSecond; }
            set { _MaxBroadcastsPerSecond = value; }
        }
        /// <summary>
        /// If yes, enable socket option TCP_NODELAY. The argument must 
        /// be yes or no. The default is yes. 
        /// </summary>
        public bool NoDelay
        {
            get { return _NoDelay; }
            set { _NoDelay = value; }
        }
        /// <summary>
        /// Specifies the number of tries that the user has when using 
        /// password authentication. The default is 3. 
        /// </summary>
        public int PasswordGuesses
        {
            get { return _PasswordGuesses; }
            set { _PasswordGuesses = value; }
        }
        /// <summary>
        /// When password authentication is allowed, it specifies 
        /// whether the server allows login to accounts with empty 
        /// password strings. The argument must be yes or no. The 
        /// default is no. 
        /// </summary>
        public bool PermitEmptyPasswords
        {
            get { return _PermitEmptyPasswords; }
            set { _PermitEmptyPasswords = value; }
        }
        /// <summary>
        /// Specifies whether the administrator can log in using 
        /// Secure Shell. May be set to yes or no. The default is yes, 
        /// allowing admin logins through any of the authentication 
        /// types allowed for other users. The no value disables 
        /// admin logins. 
        /// </summary>
        public bool PermitRootLogin
        {
            get { return _PermitRootLogin; }
            set { _PermitRootLogin = value; }
        }
        /// <summary>
        /// Specifies whether the user can access the terminal. Valid 
        /// values are yes, no, and admin. The default is yes. 
        /// </summary>
        public bool PermitUserTerminal
        {
            get { return _PermitUserTerminal; }
            set { _PermitUserTerminal = value; }
        }
        /// <summary>
        /// This keyword enables user authentication using 
        /// certificates. The argument must be an X.509 certificate 
        /// in binary format. This keyword must be followed by one or 
        /// more MapFile keywords. 
        /// The validity of a received certificate is checked 
        /// separately using each of the defined Pki keywords in turn 
        /// until they are exhausted (in which case the authentication 
        /// fails) or a positive result is achieved. If the 
        /// certificate is valid, the mapping files are examined to 
        /// determine whether the certificate allows the user to log 
        /// in (of course, correct signature generated by a matching 
        /// private key is always required in addition to everything 
        /// else). 
        /// </summary>
        public int Pki
        {
            get { return _Pki; }
            set { _Pki = value; }
        }
        /// <summary>
        /// This keywords disables CRL checking for the preceding Pki 
        /// keyword, if the argument is y. By default, CRL checking is 
        /// on. 
        /// </summary>
        public bool PkiDisableCrls
        {
            get { return _PkiDisableCrls; }
            set { _PkiDisableCrls = value; }
        }
        /// <summary>
        /// Specifies the port number that the Secure Shell server 
        /// listens on (allowed values are 1 - 65535). The server has 
        /// to be restarted in order to use the changed setting. The 
        /// current default is 22. 
        /// </summary>
        public uint Port
        {
            get { return _Port; }
            set { _Port = value; }
        }
        /// <summary>
        /// Specifies whether the terminal is created in a fully 
        /// private window station or not. For security reasons it is 
        /// recommended that this value is set to yes. If login takes 
        /// too much time, try setting this value to no. This 
        /// parameter has no effect for SFTP-only server. The default 
        /// is yes. 
        /// </summary>
        public bool PrivateWindowsStation
        {
            get { return _PrivateWindowsStation; }
            set { _PrivateWindowsStation = value; }
        }
        /// <summary>
        /// Specifies the character string that will be used as the 
        /// version string output by the server. By altering the 
        /// version string you can mask the identity of the server. 
        /// This gives added protection against attackers who may use 
        /// the server's version information to their advantage. On 
        /// the other hand, hiding the version string may damage the 
        /// functionality of some clients,as they may use the server 
        /// version information to determine compatibility. The 
        /// default is SSH-2.0-3.2.0 SSH Secure Shell Windows NT 
        /// Server. 
        /// </summary>
        public string ProtocolVersionString
        {
            get { return _ProtocolVersionString; }
            set { _ProtocolVersionString = value; }
        }
        /// <summary>
        /// Specifies the file containing the public host key. Please 
        /// note that this file should be located on a local drive. 
        /// Network or mapped drives should not be used, as the server 
        /// program may not have proper access rights for them. The 
        /// default value is the hostkey.pub file located in the 
        /// Secure Shell installation directory. 
        /// Note: In most cases the order of config parameters is not 
        /// an issue. Here it is safe if you specify HostKeyFile first 
        /// before this parameter.
        /// </summary>
        public string PublicHostKeyFile
        {
            get { return _PublicHostKeyFile; }
            set { _PublicHostKeyFile = value; }
        }
        /// <summary>
        /// Specifies the name of the random seed file. The default is 
        /// "server_random_seed". 
        /// </summary>
        public string RandomSeedFile
        {
            get { return _RandomSeedFile; }
            set { _RandomSeedFile = value; }
        }
        /// <summary>
        /// Specifies the number of seconds after which the key 
        /// exchange is done again. A value of 0 (zero) turns rekey 
        /// requests off. This does not prevent the client from 
        /// requesting rekeys. Note that all clients do not support 
        /// this function. The default is 0.
        /// </summary>
        public int RekeyIntervalSeconds
        {
            get { return _RekeyIntervalSeconds; }
            set { _RekeyIntervalSeconds = value; }
        }
        /// <summary>
        /// Related to AllowedAuthentications, this is used to specify 
        /// what authentication methods the users must complete before 
        /// continuing. If this value is left empty, it does not mean 
        /// that no authentications are required. It means that the 
        /// client can authenticate itself with any of the 
        /// authentications given in AllowedAuthentications. This 
        /// parameter has no default. 
        /// If this option is set, AllowedAuthentications is ignored.
        /// </summary>
        public Array RequiredAuthentications
        {
            get { return _RequiredAuthentications; }
            set { _RequiredAuthentications = value; }
        }
        /// <summary>
        /// This is used to check whether host name DNS lookup must 
        /// succeed when checking whether connections from host are 
        /// allowed using AllowHosts and DenyHosts. If this is set to 
        /// yes, then if name lookup fails, the connection is denied. 
        /// If set to no, if name lookup fails, the remote host's IP 
        /// address is used to check whether it is allowed to connect. 
        /// This is probably not what you want, if you have specified 
        /// only host names (not IP addresses) with {Allow,Deny}Hosts. 
        /// The argument must be either yes or no. The default value 
        /// is no. 
        /// </summary>
        public bool RequireReverseMapping
        {
            get { return _RequireReverseMapping; }
            set { _RequireReverseMapping = value; }
        }
        /// <summary>
        /// This parameter controls whether Windows server will try to 
        /// resolve the client IP at all, or not. This is useful when 
        /// you know that the DNS cannot be reached, and the query 
        /// would cause additional delay in logging in. Note that if 
        /// you set this to no, you should not set 
        /// RequireReverseMapping to yes. The default is yes. 
        /// </summary>
        public bool ResolveClientHostName
        {
            get { return _ResolveClientHostName; }
            set { _ResolveClientHostName = value; }
        }
        /// <summary>
        /// Specifies the accessible directories for administrators. 
        /// See Sftp-DirList. The default is HOME=%D, C:=C:, D:=D:. 
        /// </summary>
        public string SftpAdminDirList
        {
            get { return _SftpAdminDirList; }
            set { _SftpAdminDirList = value; }
        }
        /// <summary>
        /// Specifies the list of administrators for the SFTP server. 
        /// Names are separated with commas. Names can include 
        /// wildcards. 
        /// </summary>
        public string SftpAdminUsers
        {
            get { return _SftpAdminUsers; }
            set { _SftpAdminUsers = value; }
        }
        /// <summary>
        /// Specifies the directories that are available for a 
        /// regular user in the SFTP server. Format is "virtual 
        /// dir=real dir". Virtual directory name can be anything, 
        /// and it must point into a real and existing directory on 
        /// network or local drive. The following pattern strings can 
        /// be used in the real directory: %D is user profile 
        /// directory, %U is user login name. The default is "HOME=%D". 
        /// </summary>
        public string SftpDirList
        {
            get { return _SftpDirList; }
            set { _SftpDirList = value; }
        }
        /// <summary>
        /// Specifies the SFTP home directory for all SFTP users. The 
        /// home directory must be specified as an accessible 
        /// directory by the Sftp-DirList (or Sftp-AdminDirList for 
        /// an administrator) keyword. The default is %D. 
        /// </summary>
        public string SftpHome
        {
            get { return _SftpHome; }
            set { _SftpHome = value; }
        }
        /// <summary>
        /// Specifies the SFTP operations that are logged to the NT 
        /// event log by the SFTP server. This is recommended to be 
        /// set through the configuration program. See the SFTP Server 
        /// tab in the configuration GUI. The default is 16, which 
        /// means that only user logins and logouts are logged in the 
        /// event log. 
        /// </summary>
        public int SftpLogCategory
        {
            get { return _SftpLogCategory; }
            set { _SftpLogCategory = value; }
        }
        /// <summary>
        /// Specifies the name of a socks server, used when fetching 
        /// certificates or CRLs from remote servers. 
        /// </summary>
        public string SocksServer
        {
            get { return _SocksServer; }
            set { _SocksServer = value; }
        }
        /// <summary>
        /// Specifies the name of the executable that provides 
        /// terminal access. The default is "cmd.exe". 
        /// </summary>
        public string TerminalProvider
        {
            get { return _TerminalProvider; }
            set { _TerminalProvider = value; }
        }
        /// <summary>
        /// Specifies where user-specific configuration data should 
        /// be fetched from. With this the administration can control 
        /// whatever configuration parameters they wish that are 
        /// normally the users' domain. This is given as a pattern 
        /// string. %D is the user's home directory, %U is user's 
        /// login name. The default value is %D/.ssh2.
        /// </summary>
        public string UserConfigDirectory
        {
            get { return _UserConfigDirectory; }
            set { _UserConfigDirectory = value; }
        }
        /// <summary>
        /// As HostSpecificConfig, but these configuration files are 
        /// read later, when the user name that client is trying to 
        /// log into is known. Also the range of configuration options 
        /// available is smaller, due to the fact that they would not 
        /// make sense in these files. You can use patterns of form 
        /// user[@host], where user is matched with the user name and 
        /// UID, and host is matched as described under option 
        /// AllowHosts.
        /// See Section Subconfiguration for more information on what 
        /// you can set in the subconfiguration file. 
        /// </summary>
        public string UserSpecificConfig
        {
            get { return UserSpecificConfig; }
            set { UserSpecificConfig = value; }
        }
        /// <summary>
        /// Specifies a 'zombie timeout' value. The default is 15 
        /// seconds. This parameter may be useful with clients that 
        /// do not communicate with the server before querying 
        /// password from the user, causing the server to enter a 
        /// 'zombie' state. 
        /// </summary>
        public int ZombieTimeout
        {
            get { return ZombieTimeout; }
            set { ZombieTimeout = value; }
        }
        #endregion
    }
}
