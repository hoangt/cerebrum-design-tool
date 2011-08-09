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
using System.Xml;

namespace CerebrumSharedClasses.Platform_Details
{
    /// <summary>
    /// Public class representing a Top-level, External FPGA Port to be included in the XPS Project
    /// </summary>
    public class FPGA_External_Port
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public FPGA_External_Port()
        {
            this.Name = string.Empty;
            this.Attributes = new Dictionary<string, string>();
        }
        /// <summary>
        /// XML Constructor.  Populates object fields from the specified XML Node
        /// </summary>
        /// <param name="ExternalPortNode">XML Node defining an external port</param>
        public FPGA_External_Port(XmlNode ExternalPortNode)
        {
            this.Name = string.Empty;
            this.Attributes = new Dictionary<string, string>();
            foreach (XmlAttribute xAttr in ExternalPortNode.Attributes)
            {
                if (String.Compare(xAttr.Name, "Name", true) == 0)
                {
                    this.Name = xAttr.Value;
                }
                else
                {
                    string key = xAttr.Name.ToUpper();
                    string val = xAttr.Value;
                    this.Attributes.Add(key, val);
                }
            }
        }
        /// <summary>
        /// The name of the port.  This will be both the internal and external name of the port
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// MHS Port Attributes to be assigned to the declaration (i.e. DIR, VEC, SIGIS, etc)
        /// </summary>
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// Writes this External Port as an XML Element Node attached to xRoot.
        /// </summary>
        /// <param name="xRoot">The XML Element to which this External Port's block should be attached.</param>
        public void WritePlatformExternalPort(XmlElement xRoot)
        {
            XmlDocument xDoc = xRoot.OwnerDocument;

            XmlElement xItemRoot = xDoc.CreateElement("ExternalPort");
            xItemRoot.SetAttribute("Name", this.Name);
            foreach (KeyValuePair<string, string> PortAttr in this.Attributes)
            {
                xItemRoot.SetAttribute(PortAttr.Key, PortAttr.Value);
            }

            xRoot.AppendChild(xItemRoot);
        }
    }
}
