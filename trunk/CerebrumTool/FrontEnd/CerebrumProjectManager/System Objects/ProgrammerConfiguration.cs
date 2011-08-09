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
/******************************************************************** 
 * ProgrammerConfiguration.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: Representation of JTAG programming cable.
 * History: 
 * >> (22 Oct 2010) Matthew Cotter: Created simple representation of JTAG programming cable used for programming BIT and ELF files to FPGA.
 *                                  Implementation of Save-to/Load-from XML functions.
 * >> (22 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CerebrumProjectManager
{
    /// <summary>
    /// Enumeration of JTAG Cable types
    /// </summary>
    public enum JTAGCableType
    {
        /// <summary>
        /// No JTAG cable specified (Not programmable via JTAG)
        /// </summary>
        None,
        /// <summary>
        /// Invalid JTAG cable specified (Unknown cable type)
        /// </summary>
        Invalid,
        /// <summary>
        /// Xilinx Platform USB JTAG Cable
        /// </summary>
        Xilinx_PlatformUSB
    }
    /// <summary>
    /// Object representing cable and port information used for programming FPGAs
    /// </summary>
    public class ProgrammerConfiguration
    {
        /// <summary>
        /// Default constructor.  Initializes as a Platform USB cable with no port.
        /// </summary>
        public ProgrammerConfiguration()
        {
            this.CablePort = string.Empty;
            this.CableType = JTAGCableType.Xilinx_PlatformUSB;
        }

        /// <summary>
        /// Get or set the JTAG cable type
        /// </summary>
        public JTAGCableType CableType { get; set; }
        /// <summary>
        /// Get or set the JTAG cable port
        /// </summary>
        public string CablePort { get; set; }
        
        /// <summary>
        /// Loads programming information from the specified Program node
        /// </summary>
        /// <param name="ProgramNode">The XML node containing programming information</param>
        public void LoadFromXml(XmlNode ProgramNode)
        {
            foreach (XmlNode xProp in ProgramNode.ChildNodes)
            {
                // OS and Type are Fixed for now
                if (String.Compare(xProp.Name, "CablePort", true) == 0)
                {
                    this.CablePort = xProp.InnerText;
                }
                else if (String.Compare(xProp.Name, "CableType", true) == 0)
                {
                    this.CableType = JTAGCableType.Xilinx_PlatformUSB;      // Fixed for now
                }
            }
        }

        /// <summary>
        /// Saves programming information to a new Program node
        /// </summary>
        /// <param name="TargetDoc">The XML document in which the new node will be saved</param>
        /// <param name="DeviceID">The ID of the device associated with this programming information</param>
        /// <returns>The XML node containing programming information</returns>
        public XmlNode SaveToXml(XmlDocument TargetDoc, string DeviceID)
        {
            XmlElement xProgNode = TargetDoc.CreateElement("Program");
            xProgNode.SetAttribute("Device", DeviceID);

            XmlNode xCableType = TargetDoc.CreateElement("CableType");
            xCableType.InnerText = this.CableType.ToString().ToLower();
            xProgNode.AppendChild(xCableType);

            XmlNode xCablePort = TargetDoc.CreateElement("CablePort");
            xCablePort.InnerText = this.CablePort;
            xProgNode.AppendChild(xCablePort);

            return xProgNode;
        }
    }
}
