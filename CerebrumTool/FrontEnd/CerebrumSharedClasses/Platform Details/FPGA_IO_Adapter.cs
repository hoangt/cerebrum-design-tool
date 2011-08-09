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

namespace CerebrumSharedClasses.Platform_Details
{
    /// <summary>
    /// Represents an Adapter defined on an FPGA and its corresponding ports and parameters.
    /// </summary>
    public class FPGA_IO_Adapter
    {
        private List<Internal_IO_Parameter> _Parameters;
        private List<Internal_IO_Port> _Ports;

        /// <summary>
        /// Default constructor.  Initializes and empty IO Adapter.
        /// </summary>
        public FPGA_IO_Adapter()
        {
            _Parameters = new List<Internal_IO_Parameter>();
            _Ports = new List<Internal_IO_Port>();
        }
        /// <summary>
        /// Creates a adapter specification based on the contents of the specified XML IO_ADAPTER Node.
        /// </summary>
        /// <param name="xAdapterNode">The XML 'IO_ADAPTER' node that contains information about the adapter</param>
        public FPGA_IO_Adapter(XmlNode xAdapterNode)
        {
            ParseAdapterNode(xAdapterNode);
        }

        /// <summary>
        /// A pre-defined instance name associated with the adapter.
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// The CoreName that utilizes the adapter
        /// </summary>
        public string CoreName { get; set; }

        /// <summary>
        /// Indicates whether this adapter is available for use
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// List of parameters set in the adapter
        /// </summary>
        public List<Internal_IO_Parameter> Parameters
        {
            get
            {
                return _Parameters;
            }
        }

        /// <summary>
        /// List of ports used in the adapter
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
        /// <param name="reader">The StreamReader from which the IO_ADAPTER block is to be read.</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParseXBDAdapter(StreamReader reader)
        {
            try
            {
                this.Instance = string.Empty;
                this.CoreName = string.Empty;
                this.Available = false;

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
                        else if (String.Compare(attrName, "CORENAME", true) == 0)
                        {
                            this.CoreName = attrValue;
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
                this.Available = true;
                return true;
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
            return false;
        }

        /// <summary>
        /// Populate this object's properties based on the information contained in the specified XML Node.
        /// </summary>
        /// <param name="xAdapterNode">The XML 'IO_ADAPTER' node that contains information about the IO Adapter.</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParseAdapterNode(XmlNode xAdapterNode)
        {
            _Parameters = new List<Internal_IO_Parameter>();
            _Ports = new List<Internal_IO_Port>();
            this.Instance = string.Empty;
            this.CoreName = string.Empty;
            this.Available = false;

            if (String.Compare(xAdapterNode.Name, "IO_ADAPTER", true) == 0)
            {
                foreach (XmlAttribute xAttr in xAdapterNode.Attributes)
                {
                    if (String.Compare(xAttr.Name, "INSTANCE", true) == 0)
                    {
                        this.Instance = xAttr.Value.Trim();
                    }
                    else if (String.Compare(xAttr.Name, "CORENAME", true) == 0)
                    {
                        this.CoreName = xAttr.Value.Trim();
                    }
                }
                foreach (XmlNode xAdapterChild in xAdapterNode.ChildNodes)
                {
                    if (String.Compare(xAdapterChild.Name, "PARAMETER", true) == 0)
                    {
                        Internal_IO_Parameter param = new Internal_IO_Parameter();
                        if (param.ParseParameterNode(xAdapterChild))
                        {
                            _Parameters.Add(param);
                        }
                    }
                    else if (String.Compare(xAdapterChild.Name, "PORT", true) == 0)
                    {
                        Internal_IO_Port port = new Internal_IO_Port();
                        if (port.ParsePortNode(xAdapterChild))
                        {
                            _Ports.Add( port);
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
            this.Available = true;
            return true;
        }


        /// <summary>
        /// Writes this Adapter as an XML Element Node attached to xRoot.
        /// </summary>
        /// <param name="xRoot">The XML Element to which this Adapter's block should be attached.</param>
        public void WritePlatformAdapter(XmlElement xRoot)
        {
            XmlDocument xDoc = xRoot.OwnerDocument;

            XmlElement xItemRoot = xDoc.CreateElement("IO_ADAPTER");
            xItemRoot.SetAttribute("Instance", this.Instance);
            xItemRoot.SetAttribute("CoreName", this.CoreName);

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
