/******************************************************************** 
 * Cerebrum Embedded System Design Automation Framework
 * Copyright (C) 2010  The Pennsylvania State University
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ********************************************************************/
/*********************************************************************************************************** 
 * FalconServer.cs
 * Name: Matthew Cotter
 * Date: 18 Jun 2010 
 * Description: Library to automate hardware synthesis and software compilation of an FPGA platform using Xilinx tools.
 * Notes:
 *     
 * History: 
 * >> (11 Aug 2010) Matthew Cotter: Moved to FalconGlobal project to ensure that all tools use a common Server object, 
 *                                      and that any required changes are reflected in all tools that use it.
 * >> (25 Jun 2010) Matthew Cotter: Added methods to parse and write an XmlNode for Synthesis servers.
 * >> (19 Jun 2010) Matthew Cotter: Added code and properties to create and represent a server on which work can be done remotely via SSH.
 * >> (19 Jun 2010) Matthew Cotter: Source file created -- Initial version.
 ***********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Security.Cryptography;
using System.IO;

namespace FalconGlobal
{
    /// <summary>
    /// Represents a remote server on which synthesis and compilation can be performed.
    /// </summary>
    public class FalconServer
    {
        private List<object> _AcquiredObjects;

        private string _eKey = "J*fg4F0-";  // Only first 8 characters are used
        private string _eIV = "57p;)9*f";    // Only first 8 characters are used

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
            this._MaxAvailable = 1;
            this._AcquiredObjects = new List<object>();
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
            this._MaxAvailable = 1;
            this._AcquiredObjects = new List<object>();
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
            this._MaxAvailable = 1;
            this._AcquiredObjects = new List<object>();
        }

        private string _Password;

        /// <summary>
        /// Get or set the ID of this server instance.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Get or set the user name used to connect to this server.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Indicates the maximum number of concurrent processes that should be permitted to run on this server.
        /// </summary>
        public int MaxAvailable
        { 
            get
            {
                return _MaxAvailable;
            }
        }
        private int _MaxAvailable;

        /// <summary>
        /// Allows another object to acquire access this server object.  Note: the Acquire/Release functions are used for convenience and do not forcefully grant/restrict access to the fields of this object.
        /// </summary>
        /// <param name="Token">The identification 'token' used to identify the asking party to this object.</param>
        /// <returns>True if access was granted, either with this call or from an unreleased previous call.  False, if access was denied.</returns>
        public bool Acquire(object Token)
        {
            bool bSuccess = false;
            lock (this)
            {
                if (_AcquiredObjects.Contains(Token))
                {
                    bSuccess = true;
                }
                else if (_AcquiredObjects.Count < _MaxAvailable)
                {
                    _AcquiredObjects.Remove(Token);
                    bSuccess = true;
                }
            }
            return bSuccess;
        }
        /// <summary>
        /// Releases a previously held access to this object.  Note: the Acquire/Release functions are used for convenience and do not forcefully grant/restrict access to the fields of this object.
        /// </summary>
        /// <param name="Token">The identification 'token' used to identify the releasing party to this object.</param>
        /// <returns>True if access was successfully revoked.  False, if the releasing party did not previously hold access.</returns>
        public bool Release(object Token)
        {
            bool bSuccess = false;
            lock (this)
            {
                if (_AcquiredObjects.Contains(Token))
                {
                    _AcquiredObjects.Remove(Token);
                    bSuccess = true;
                }
            }
            return bSuccess;
        }

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
                if (String.Compare(xElem.Name, "ID", true) == 0)
                {
                    this.ID = xElem.InnerText;
                }
                else if (String.Compare(xElem.Name, "UserName", true) == 0)
                {
                    this.UserName = xElem.InnerText;
                }
                else if (String.Compare(xElem.Name, "Address", true) == 0)
                {
                    this.Address = xElem.InnerText;
                }
                else if (String.Compare(xElem.Name, "Password", true) == 0)
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
                else if (String.Compare(xElem.Name, "Port", true) == 0)
                {
                    int val;
                    if (int.TryParse(xElem.InnerText, out val))
                        this.Port = val;
                }
                else if (String.Compare(xElem.Name, "LinuxHost", true) == 0)
                {
                    bool val;
                    if (bool.TryParse(xElem.InnerText, out val))
                        this.LinuxHost = val;
                }
                else if (String.Compare(xElem.Name, "MaxConnections", true) == 0)
                {
                    int val;
                    if (int.TryParse(xElem.InnerText, out val))
                        this._MaxAvailable = val;
                }
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
            XmlElement xEMax = ParentNode.OwnerDocument.CreateElement("MaxConnections");

            xEID.InnerText = this.ID;
            xEUser.InnerText = this.UserName;
            xEPass.InnerText = this._Password;
            xEAddr.InnerText = this.Address;
            xEPort.InnerText = this.Port.ToString();
            xELinux.InnerText = this.LinuxHost.ToString();
            xEMax.InnerText = this.MaxAvailable.ToString();

            xEServer.AppendChild(xEID);
            xEServer.AppendChild(xEUser);
            if (this.UseEncryption)
            {
                xEServer.AppendChild(xEPass);
            }
            xEServer.AppendChild(xEAddr);
            xEServer.AppendChild(xEPort);
            xEServer.AppendChild(xELinux);
            xEServer.AppendChild(xEMax);
            ParentNode.AppendChild(xEServer);
        }
    }
}
