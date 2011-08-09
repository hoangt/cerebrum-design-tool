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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CerebrumSharedClasses;
using System.Xml;
using System.Diagnostics;

namespace CerebrumSharedClasses.Platform_Details
{
    /// <summary>
    /// Represents an IO_Interface defined on an FPGA and its corresponding ports and parameters.
    /// </summary>
    public class FPGA_IO_Interface
    {
        private List<Internal_IO_Parameter> _Parameters;
        private List<Internal_IO_Port> _Ports;

        /// <summary>
        /// Default constructor.  Initializes and empty IO Interface.
        /// </summary>
        public FPGA_IO_Interface()
        {
            _Parameters = new List<Internal_IO_Parameter>();
            _Ports = new List<Internal_IO_Port>();
        }
        /// <summary>
        /// Creates a interface specification based on the contents of the specified XML IO_INTERFACE Node.
        /// </summary>
        /// <param name="xInterfaceNode">The XML 'IO_INTERFACE' node that contains information about the interface</param>
        public FPGA_IO_Interface(XmlNode xInterfaceNode)
        {
            ParseInterfaceNode(xInterfaceNode);
        }

        /// <summary>
        /// A pre-defined instance name associated with the Interface.
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// The IO Type specification for the interface
        /// </summary>
        public string IOType { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public string Value_Note { get; set; }
        /// <summary>
        /// Unknown.
        /// </summary>
        public string Exclusive { get; set; }

        /// <summary>
        /// Indicates whether this interface is available for use
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// The list of parameters associated with the IO interface
        /// </summary>
        public List<Internal_IO_Parameter> Parameters
        {
            get
            {
                return _Parameters;
            }
        }

        /// <summary>
        /// The list of ports associated with the IO interface
        /// </summary>
        public List<Internal_IO_Port> Ports
        {
            get
            {
                return _Ports;
            }
        }


        /// <summary>
        /// Populate this object's properties based on the information contained in the specified XBD Document "Node".
        /// </summary>
        /// <param name="reader">The StreamReader from which the IO_INTERFACE block is to be read.</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParseXBDInterface(StreamReader reader)
        {
            try
            {
                this.Instance = string.Empty;
                this.IOType = string.Empty;
                this.Value_Note = string.Empty;
                this.Exclusive = string.Empty;

                bool AtEndofBlock = false;
                while (!AtEndofBlock)
                {
                    string line = reader.ReadLine().Trim();
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    if (line.Contains("#"))
                    {
                        line = line.Substring(0, line.IndexOf("#") - 1).Trim();
                    }
                    if (String.Compare(line, "END", true) == 0)
                    {
                        AtEndofBlock = true;
                        continue;
                    }
                    if (line.ToUpper().StartsWith("ATTRIBUTE"))
                    {
                        line = line.Substring(9).Trim();
                        string attrName = string.Empty;
                        string attrValue = string.Empty;
                        string[] pair = line.Split('=');
                        attrName = pair[0].Trim().ToUpper();
                        attrValue = pair[1].Trim();
                        if (String.Compare(attrName, "INSTANCE", true) == 0)
                        {
                            this.Instance = attrValue;
                        }
                        else if (String.Compare(attrName, "IOTYPE", true) == 0)
                        {
                            this.IOType = attrValue;
                        }
                        else if (String.Compare(attrName, "EXCLUSIVE", true) == 0)
                        {
                            this.Exclusive = attrValue;
                        }
                    }
                    else if (line.ToUpper().StartsWith("PARAMETER"))
                    {
                        Internal_IO_Parameter param = new Internal_IO_Parameter();
                        if (param.ParseXBDParameter(line))
                        {
                            _Parameters.Add(param);
                        }
                    }
                    else if (line.ToUpper().StartsWith("PORT"))
                    {
                        Internal_IO_Port port = new Internal_IO_Port();
                        if (port.ParseXBDPort(line))
                        {
                            _Ports.Add(port);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Instance = string.Empty;
                this.IOType = string.Empty;
                this.Value_Note = string.Empty;
                this.Exclusive = string.Empty;
                ErrorReporting.DebugException(ex);
            }
            return false;
        }

        /// <summary>
        /// Populate this object's properties based on the information contained in the specified XML Node.
        /// </summary>
        /// <param name="xInterfaceNode">The XML 'IO_INTERFACE' node that contains information about the IO Interface.</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParseInterfaceNode(XmlNode xInterfaceNode)
        {
            _Parameters = new List<Internal_IO_Parameter>();
            _Ports = new List<Internal_IO_Port>();
            this.Instance = string.Empty;
            this.IOType = string.Empty;
            this.Value_Note = string.Empty;
            this.Exclusive = string.Empty;
            this.Available = false;
            try
            {
                if (String.Compare(xInterfaceNode.Name, "IO_INTERFACE", true) == 0)
                {
                    foreach (XmlAttribute xAttr in xInterfaceNode.Attributes)
                    {
                        if (String.Compare(xAttr.Name, "INSTANCE", true) == 0)
                        {
                            this.Instance = xAttr.Value.Trim();
                        }
                        else if (String.Compare(xAttr.Name, "IOTYPE", true) == 0)
                        {
                            this.IOType = xAttr.Value.Trim();
                        }
                        else if (String.Compare(xAttr.Name, "VALUE_NOTE", true) == 0)
                        {
                            this.Value_Note = xAttr.Value.Trim();
                        }
                        else if (String.Compare(xAttr.Name, "EXCLUSIVE", true) == 0)
                        {
                            this.Exclusive = xAttr.Value.Trim();
                        }
                    }
                    foreach (XmlNode xInterfaceChild in xInterfaceNode.ChildNodes)
                    {
                        if (xInterfaceChild.NodeType == XmlNodeType.Comment)
                            continue;
                        if (String.Compare(xInterfaceChild.Name, "PARAMETER", true) == 0)
                        {
                            Internal_IO_Parameter param = new Internal_IO_Parameter();
                            if (param.ParseParameterNode(xInterfaceChild))
                            {
                                _Parameters.Add(param);
                            }
                        }
                        else if (String.Compare(xInterfaceChild.Name, "PORT", true) == 0)
                        {
                            Internal_IO_Port port = new Internal_IO_Port();
                            if (port.ParsePortNode(xInterfaceChild))
                            {
                                _Ports.Add(port);
                            }
                        }
                        else
                        {
                            _Parameters = new List<Internal_IO_Parameter>();
                            _Ports = new List<Internal_IO_Port>();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Instance = string.Empty;
                this.IOType = string.Empty;
                this.Value_Note = string.Empty;
                this.Exclusive = string.Empty;
                ErrorReporting.DebugException(ex);
                Debug.WriteLine("Parsed failed on port # {0}", (_Ports.Count + 1).ToString());
            }
            this.Available = true;
            return true;
        }


        /// <summary>
        /// Writes this Interface as an XML Element Node attached to xRoot.
        /// </summary>
        /// <param name="xRoot">The XML Element to which this Interfaces's block should be attached.</param>
        public void WritePlatformInterface(XmlElement xRoot)
        {
            XmlDocument xDoc = xRoot.OwnerDocument;

            XmlElement xItemRoot = xDoc.CreateElement("IO_INTERFACE");
            xItemRoot.SetAttribute("IOType", this.IOType);
            xItemRoot.SetAttribute("Instance", this.Instance);
            if (String.Compare(this.Exclusive, string.Empty) != 0)
                xItemRoot.SetAttribute("Exclusive", this.Exclusive);
            if (String.Compare(this.Value_Note, string.Empty) != 0)
                xItemRoot.SetAttribute("Value_Note", this.Value_Note);

            #region Parameters
            for (int i = 0; i < Parameters.Count; i++)
            {
                Internal_IO_Parameter iiop = Parameters[i];
                iiop.WritePlatformIOParameter(xItemRoot);
            }
            #endregion

            #region Ports
            for (int i = 0; i < Ports.Count; i++)
            {
                Internal_IO_Port iiop = Ports[i];
                iiop.WritePlatformIOPort(xItemRoot);
            }
            #endregion

            xRoot.AppendChild(xItemRoot);
        }
    }
}
