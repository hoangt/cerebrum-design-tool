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
 * CommunicationConfiguration.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: Representation of ethernet communication interface.
 * History: 
 * >> (22 Oct 2010) Matthew Cotter: Created simple representation of ethernet communication properties used for configuring ethernet in the embedded Linux.
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
    /// Dialog used to allow for simple editing of communication (Ethernet) interfaces
    /// </summary>
    public class CommunicationConfiguration
    {
        /// <summary>
        /// Default constructor.  Initializes an unconfigured ethernet interface.
        /// </summary>
        public CommunicationConfiguration()
        {
            this.UseDHCP = true;
            this.MACAddress = string.Empty;
            this.IPAddress = string.Empty;
            this.MappedFPGA = string.Empty;
        }
        /// <summary>
        /// Get or set whether this interface uses DHCP for IP configuration
        /// </summary>
        public bool UseDHCP { get; set; }
        /// <summary>
        /// Get or set the ID of the FPGA device to which this interface is mapped
        /// </summary>
        public string MappedFPGA { get; set; }
        /// <summary>
        /// Get or set the MAC address of the Ethernet interface
        /// </summary>
        public string MACAddress { get; set; }
        /// <summary>
        /// Get or set the IP assigned, or expected to be assigned, to the interface
        /// </summary>
        public string IPAddress { get; set; }
        /// <summary>
        /// Get or set the instance name of the hardware object associated with this interface
        /// </summary>
        public string HardwareInstance { get; set; }

        /// <summary>
        /// Loads communication interface configuration information from the specified XML node
        /// </summary>
        /// <param name="InterfaceNode">The Interface node from which the configuration data is to be loaded</param>
        public void LoadFromXml(XmlNode InterfaceNode)
        {
            foreach (XmlAttribute xAttr in InterfaceNode.Attributes)
            {
                if (String.Compare(xAttr.Name, "Instance", true) == 0)
                {
                    this.HardwareInstance = xAttr.Value;
                    break;
                }
            }
            foreach (XmlNode xProp in InterfaceNode.ChildNodes)
            {
                if (String.Compare(xProp.Name, "FPGA", true) == 0)
                {
                    this.MappedFPGA = xProp.InnerText;
                }
                else if (String.Compare(xProp.Name, "DHCP", true) == 0)
                {
                    bool val = true;
                    bool.TryParse(xProp.InnerText, out val);
                    this.UseDHCP = val;
                }
                else if (String.Compare(xProp.Name, "EthernetMAC", true) == 0)
                {
                    this.MACAddress = xProp.InnerText;
                }
                else if (String.Compare(xProp.Name, "IPAddress", true) == 0)
                {
                    this.IPAddress = xProp.InnerText;
                }
            }
        }

        /// <summary>
        /// Saves communication configuration information to an XML node in the schema of the target XML document
        /// </summary>
        /// <param name="TargetDoc">The target XML document</param>
        /// <returns>An XmlNode object containing the this communication interface's configuration information</returns>
        public XmlNode SaveToXml(XmlDocument TargetDoc)
        {
            XmlElement xIFNode = TargetDoc.CreateElement("Interface");
            xIFNode.SetAttribute("Instance", HardwareInstance);

            if ((this.MappedFPGA != string.Empty) && (this.MappedFPGA != null))
            {
                XmlNode xFPGA = TargetDoc.CreateElement("FPGA");
                xFPGA.InnerText = this.MappedFPGA;
                xIFNode.AppendChild(xFPGA);
            }

            XmlNode xDHCP = TargetDoc.CreateElement("DHCP");
            xDHCP.InnerText = this.UseDHCP.ToString().ToLower();
            xIFNode.AppendChild(xDHCP);

            XmlNode xMAC = TargetDoc.CreateElement("EthernetMAC");
            xMAC.InnerText = this.MACAddress.ToLower();
            xIFNode.AppendChild(xMAC);

            XmlNode xIP = TargetDoc.CreateElement("IPAddress");
            xIP.InnerText = this.IPAddress;
            xIFNode.AppendChild(xIP);

            return xIFNode;
        }
    }
}
