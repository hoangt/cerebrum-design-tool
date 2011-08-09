/*********************************************************************************************************** 
 * FalconPlatformSynthesis\FalconServer.cs
 * Name: Matthew Cotter
 * Date: 18 Jun 2010 
 * Description: Library to automate hardware synthesis and software compilation of an FPGA platform using Xilinx tools.
 * Notes:
 *     
 * History: 
 * >> (25 Jun 2010): Added methods to parse and write an XmlNode for Synthesis servers.
 * >> (19 Jun 2010): Added code and properties to create and represent a server on which remote synthesis can be done.
 * >> (19 Jun 2010): Source file created -- Initial version.
 ***********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.IO;

namespace FalconPlatformSynthesis
{
    /// <summary>
    /// Represents a remote server on which synthesis and compilation can be performed.
    /// </summary>
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/CoreServerCompiler Specification.pdf">
    /// Core Server Compiler Documentation</seealso>
    public class FalconServer
    {
        private string _eKey = "FalconProjectKey";  // Only first 8 characters are used
        private string _eIV = "FalconProjectIV";    // Only first 8 characters are used

        /// <summary>
        /// Constructor initializes all fields to default (empty) values.
        /// </summary>
        public FalconServer()
        {
            this.ID = string.Empty;
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.Address = string.Empty;
            this.LinuxHost = true;
            this.Port = 22;
        }

        /// <summary>
        /// Constructor initializes all fields to specified values, using the default SSH port of 22.
        /// </summary>
        public FalconServer(string ID, string User, string Pass, string Addr, bool IsLinux)
        {
            this.ID = ID;
            this.UserName = User;
            this.Password = Pass;
            this.Address = Addr;
            this.LinuxHost = IsLinux;
            this.Port = 22;
        }

        /// <summary>
        /// Constructor initializes all fields to specified values.
        /// </summary>
        public FalconServer(string ID, string User, string Pass, string Addr, bool IsLinux, int Port)
        {
            this.ID = ID;
            this.UserName = User;
            this.Password = Pass;
            this.Address = Addr;
            this.LinuxHost = IsLinux;
            this.Port = Port;
        }

        private string _Password;

        /// <summary>
        /// Get or set the ID of this synthesis server instance.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Get or set the user name used to connect to this server.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Get or set whether the stored password should be encrypted.
        /// </summary>
        public bool UseEncryption { get; set; }
        /// <summary>
        /// Get or set the password used to connect to this server.
        /// </summary>
        public string Password 
        {
            get
            {
                if (UseEncryption)
                    return this.DecryptPassword(_Password);
                else 
                    return _Password;
            }
            set
            {
                if (UseEncryption)
                    _Password = this.EncryptPassword(value);
                else
                    _Password = value;
            }
        }

        /// <summary>
        /// Decrypts the user password into plaintext using 64 bit symmetric key DES.
        /// </summary>
        /// <param name="EncryptedPass">The encrypted password string.</param>
        /// <returns>The decrypted password string as plaintext.</returns>
        private string DecryptPassword(string EncryptedPass)
        {
            byte[] bytePass = Convert.FromBase64String(EncryptedPass); 
            DESCryptoServiceProvider cryptDES = new DESCryptoServiceProvider();
            MemoryStream memStream = new MemoryStream();
            if (_eKey.Length > 8)
                _eKey = _eKey.Substring(0, 8);
            else
                _eKey = _eKey.PadRight(8);
            if (_eIV.Length > 8)
                _eIV = _eKey.Substring(0, 8);
            else
                _eIV = _eKey.PadRight(8);

            CryptoStream cryptStream = new CryptoStream(memStream, 
                    cryptDES.CreateDecryptor(
                        Encoding.UTF8.GetBytes(_eKey), 
                        Encoding.UTF8.GetBytes(_eIV)), 
                    CryptoStreamMode.Write);
            cryptStream.Write(bytePass, 0, bytePass.Length);
            cryptStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(memStream.ToArray());
        }

        /// <summary>
        /// Encrypts the user password as plaintext into ciphertext using 64 bit symmetric key DES.
        /// </summary>
        /// <param name="ClearPassword">The plaintext password string.</param>
        /// <returns>The encrypted password string as ciphertext.</returns>
        private string EncryptPassword(string ClearPassword)
        {
            byte[] bytePass = Encoding.UTF8.GetBytes(ClearPassword);
            DESCryptoServiceProvider cryptDES = new DESCryptoServiceProvider();
            MemoryStream memStream = new MemoryStream();
            if (_eKey.Length > 8)
                _eKey = _eKey.Substring(0, 8);
            else
                _eKey = _eKey.PadRight(8);
            if (_eIV.Length > 8)
                _eIV = _eKey.Substring(0, 8);
            else
                _eIV = _eKey.PadRight(8);

            CryptoStream cryptStream = new CryptoStream(memStream,
                    cryptDES.CreateEncryptor(
                        Encoding.UTF8.GetBytes(_eKey),
                        Encoding.UTF8.GetBytes(_eIV)),
                    CryptoStreamMode.Write);
            cryptStream.Write(bytePass, 0, bytePass.Length);
            cryptStream.FlushFinalBlock();
            return Convert.ToBase64String(memStream.ToArray());
        }

        /// <summary>
        /// Get or set the address of the remote server to connect.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Get or set the port to connect to on the remote server.
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Get or set whether this server is a linux host or a windows host.
        /// </summary>
        public bool LinuxHost { get; set; }

        /// <summary>
        /// Parse a {Server} node from XML file as a FalconServer object.
        /// </summary>
        /// <param name="ServerNode">The XmlNode from an XmlDocument containing the server node information.</param>
        public void ParseServerNode(XmlNode ServerNode)
        {
            foreach (XmlNode xElem in ServerNode.ChildNodes)
            {
                string xEName = xElem.Name.ToLower();
                if (xEName == "id")
                {
                    this.ID = xElem.InnerText;
                }
                else if (xEName == "username")
                {
                    this.UserName = xElem.InnerText;
                }
                else if (xEName == "address")
                {
                    this.Address = xElem.InnerText;
                }
                else if (xEName == "password")
                {
                    this.UseEncryption = false;
                    _Password = string.Empty;
                    try
                    {
                        if (xElem.InnerText.Length > 0)
                        {
                            DecryptPassword(xElem.InnerText);
                            this.UseEncryption = true;
                            _Password = xElem.InnerText;
                        }
                    }
                    catch
                    {
                        this.UseEncryption = false;
                        _Password = string.Empty;
                    }
                }
                else if (xEName == "port")
                {
                    int val;
                    if (int.TryParse(xElem.InnerText, out val))
                        this.Port = val;
                }
                else if (xEName == "linuxhost")
                {
                    bool val;
                    if (bool.TryParse(xElem.InnerText, out val))
                        this.LinuxHost = val;
                }
                //Password is not parsed from server file, required at run-time.
                //else if (xEName == "password")
                //{
                //    this._Password = xElem.InnerText;
                //}
            }
        }

        /// <summary>
        /// Write this FalconServer object to an XmlDocument under the specified parent node as a {Server} element.
        /// </summary>
        /// <param name="ParentNode">The node under which this object should be written as a {Server} element.</param>
        public void WriteServerNode(XmlNode ParentNode)
        {
            XmlElement xEServer = ParentNode.OwnerDocument.CreateElement("Server");
            XmlElement xEID = ParentNode.OwnerDocument.CreateElement("ID");
            XmlElement xEUser = ParentNode.OwnerDocument.CreateElement("UserName");
            XmlElement xEPass = ParentNode.OwnerDocument.CreateElement("Password");
            XmlElement xEAddr = ParentNode.OwnerDocument.CreateElement("Address");
            XmlElement xEPort = ParentNode.OwnerDocument.CreateElement("Port");
            XmlElement xELinux = ParentNode.OwnerDocument.CreateElement("LinuxHost");

            xEID.InnerText = this.ID;
            xEUser.InnerText = this.UserName;
            xEPass.InnerText = this._Password;
            xEAddr.InnerText = this.Address;
            xEPort.InnerText = this.Port.ToString();
            xELinux.InnerText = this.LinuxHost.ToString();

            xEServer.AppendChild(xEID);
            xEServer.AppendChild(xEUser);
            if (this.UseEncryption)
            {
                xEServer.AppendChild(xEPass);
            }
            xEServer.AppendChild(xEAddr);
            xEServer.AppendChild(xEPort);
            xEServer.AppendChild(xELinux);
            ParentNode.AppendChild(xEServer);
        }
    }
}
