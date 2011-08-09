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
using CerebrumSharedClasses;
using System.Xml;

namespace CerebrumSharedClasses.Platform_Details
{
    /// <summary>
    /// Represents an IO Port specified on the FPGA.
    /// </summary>
    public class FPGA_IO_Port
    {
        /// <summary>
        /// Default constructor.  Initializes and empty IO Port.
        /// </summary>
        public FPGA_IO_Port()
        {
            this.PortName = string.Empty;
            this.SignalName = string.Empty;
            this.UCF_Net_String = string.Empty;
            this.ValidCond = string.Empty;
        }
        /// <summary>
        /// Creates a port specification based on the contents of the specified XML PORT Node.
        /// </summary>
        /// <param name="xAdapterNode">The XML 'PORT' node that contains information about the port</param>
        public FPGA_IO_Port(XmlNode xAdapterNode)
        {
            ParsePortNode(xAdapterNode);
        }

        /// <summary>
        /// The name of the port
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// The name of the signal associated with the port
        /// </summary>
        public string SignalName { get; set; }

        /// <summary>
        /// The UCF Net binding enforced by utilizing this port
        /// </summary>
        public string UCF_Net_String { get; set; }

        /// <summary>
        /// A string indicating the core conditions under which this port may be connected
        /// </summary>
        public string ValidCond { get; set; }


        /// <summary>
        /// Populate this object's properties based on the information contained in the specified XBD Port entry.
        /// </summary>
        /// <param name="PortLine">The XBD 'PORT' string that contains information about the port</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParseXBDPort(string PortLine)
        {
            try
            {
                if (PortLine.Contains('#'))
                {
                    PortLine = PortLine.Substring(0, PortLine.IndexOf('#') - 1);
                }
                PortLine = PortLine.Trim();

                this.PortName = string.Empty;
                this.SignalName = string.Empty;
                this.UCF_Net_String = string.Empty;
                this.ValidCond = string.Empty;

                while (PortLine.Contains(" = "))
                    PortLine = PortLine.Replace(" = ", "=");

                string Keyword = string.Empty;
                while (PortLine.Length > 0)
                {
                    int spIdx = PortLine.IndexOf(' ');
                    int eqIdx = PortLine.IndexOf('=');
                    int cmIdx = PortLine.IndexOf(',');
                    int lParIdx = PortLine.IndexOf('(');
                    int rParIdx = PortLine.IndexOf(')');

                    if (PortLine.StartsWith("PORT"))
                    {
                        string PortInfo = PortLine.Substring(spIdx, cmIdx - spIdx).Trim();
                        string[] PortInfoSplit = PortInfo.Split('=');
                        this.PortName = PortInfoSplit[0].Trim();
                        this.SignalName = PortInfoSplit[1].Trim();
                        PortLine = PortLine.Substring(cmIdx + 1).Trim();
                    }
                    else if (PortLine.StartsWith("UCF_NET_STRING"))
                    {
                        string UCFString = PortLine.Substring(eqIdx + 1).Trim();
                        if (UCFString.StartsWith("(") && UCFString.EndsWith(")"))
                        {
                            UCFString = UCFString.Substring(1, UCFString.Length - 2);
                            while (UCFString.Contains("\t"))
                                UCFString = UCFString.Replace("\t", " ").Trim();
                            while (UCFString.Contains("  "))
                                UCFString = UCFString.Replace("  ", " ").Trim();
                            while (UCFString.Contains("\", \""))
                                UCFString = UCFString.Replace("\", \"", " | ").Trim();
                            while (UCFString.Contains("\",\""))
                                UCFString = UCFString.Replace("\",\"", " | ").Trim();
                            while (UCFString.Contains("\""))
                                UCFString = UCFString.Replace("\"", string.Empty).Trim();
                            while (UCFString.Contains(" = "))
                                UCFString = UCFString.Replace(" = ", "=").Trim();

                            this.UCF_Net_String = UCFString.Trim();
                            break;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                this.PortName = string.Empty;
                this.SignalName = string.Empty;
                this.UCF_Net_String = string.Empty;
                this.ValidCond = string.Empty;
                return false;
            }
        }
        /// <summary>
        /// Populate this object's properties based on the information contained in the specified XML Node.
        /// </summary>
        /// <param name="xPortNode">The XML 'PORT' node that contains information about the port.</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParsePortNode(XmlNode xPortNode)
        {
            this.PortName = string.Empty;
            this.SignalName = string.Empty;
            this.UCF_Net_String = string.Empty;
            this.ValidCond = string.Empty;

            if (String.Compare(xPortNode.Name, "PORT", true) == 0)
            {
                foreach (XmlAttribute xAttr in xPortNode.Attributes)
                {
                    if (String.Compare(xAttr.Name, "UCF_NET_STRING", true) == 0)
                    {
                        this.UCF_Net_String = xAttr.Value.Trim();
                    }
                    else if (String.Compare(xAttr.Name, "Valid", true) == 0)
                    {
                        this.ValidCond = xAttr.Value.Trim();
                    }
                    else //if (String.Compare(xAttr.Name, "NAME", true) == 0)
                    {
                        this.PortName = xAttr.Name.Trim();
                        this.SignalName = xAttr.Value.Trim();
                    }
                    //else if (String.Compare(xAttr.Name, "SIGNAL", true) == 0)
                    //{
                    //    this.SignalName = xAttr.Value;
                    //}
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// Writes this FPGA IO Port/pin as an XML Element Node attached to xRoot.
        /// </summary>
        /// <param name="xRoot">The XML Element to which this FPGA IO Port/pin's block should be attached.</param>
        public void WritePlatformFPGAIOPort(XmlElement xRoot)
        {
            XmlDocument xDoc = xRoot.OwnerDocument;

            XmlElement xItemRoot = xDoc.CreateElement("PORT");
            xItemRoot.SetAttribute(this.PortName, this.SignalName);
            xItemRoot.SetAttribute("UCF_NET_STRING", this.UCF_Net_String);

            if (String.Compare(this.ValidCond, string.Empty) != 0)
                xItemRoot.SetAttribute("VALID", this.ValidCond);

            xRoot.AppendChild(xItemRoot);
        }
    }
}
