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
    /// Represents a PARAMETER associated with an IO_INTERFACE or an IO_ADAPTER in the Platform FPGA Specification.
    /// </summary>
    public class Internal_IO_Parameter
    {
        /// <summary>
        /// Default constructor.  Creates an empty parameter specification.
        /// </summary>
        public Internal_IO_Parameter()
        {
            this.ParameterName = string.Empty;
            this.ParameterValue = string.Empty;
            this.IO_IS = string.Empty;
        }
        /// <summary>
        /// Creates a parameter specification based on the contents of the specified XML PORT Node.
        /// </summary>
        /// <param name="xParameterNode">The XML 'PARAMETER' node that contains information about the port</param>
        public Internal_IO_Parameter(XmlNode xParameterNode)
        {
            ParseParameterNode(xParameterNode);
        }


        /// <summary>
        /// The name of the parameter
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// The Value of the parameter
        /// </summary>
        public string ParameterValue { get; set; }
        /// <summary>
        /// Unknown
        /// </summary>
        public string IO_IS { get; set; }
        
        /// <summary>
        /// Populate this object's properties based on the information contained in the specified XBD Parameter entry.
        /// </summary>
        /// <param name="ParameterLine">The XBD 'PARAMETER' string that contains information about the parameter.</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParseXBDParameter(string ParameterLine)
        {
            try
            {
                this.ParameterName = string.Empty;
                this.ParameterValue = string.Empty;
                this.IO_IS = string.Empty;

                ParameterLine = ParameterLine.Substring(9).Trim();
                this.ParameterName = ParameterLine.Substring(0, ParameterLine.IndexOf("=")).Trim();
                ParameterLine = ParameterLine.Substring(ParameterLine.IndexOf("=") + 1).Trim();
                if (ParameterLine.Contains(","))
                {
                    this.ParameterValue = ParameterLine.Substring(0, ParameterLine.IndexOf(",")).Trim();
                    ParameterLine = ParameterLine.Substring(ParameterLine.IndexOf(",") + 1).Trim();

                    string attrName = string.Empty;
                    string attrValue = string.Empty;
                    attrName = ParameterLine.Substring(0, ParameterLine.IndexOf("=")).Trim();
                    ParameterLine = ParameterLine.Substring(ParameterLine.IndexOf("=") + 1).Trim();
                   
                    if (String.Compare(attrName, "IO_IS", true) == 0)
                    {
                        if (ParameterLine.Contains(","))
                        {
                            this.IO_IS = ParameterLine.Substring(0, ParameterLine.IndexOf(",")).Trim();
                        }
                        else
                        {
                            this.IO_IS = ParameterLine;
                            
                        }
                    }
                }
                else
                {
                    this.ParameterValue = ParameterLine;
                }
                while (this.ParameterValue.Contains("\""))
                    this.ParameterValue = this.ParameterValue.Replace("\"", string.Empty).Trim();
                return true;
            }
            catch (Exception ex)
            {
                this.ParameterName = string.Empty;
                this.ParameterValue = string.Empty;
                this.IO_IS = string.Empty;
                return false;
            }
        }
        /// <summary>
        /// Populate this object's properties based on the information contained in the specified XML Node.
        /// </summary>
        /// <param name="xParameterNode">The XML 'PARAMETER' node that contains information about the parameter.</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParseParameterNode(XmlNode xParameterNode)
        {
            this.ParameterName = string.Empty;
            this.ParameterValue = string.Empty;
            this.IO_IS = string.Empty;

            if (String.Compare(xParameterNode.Name, "Parameter", true) == 0)
            {
                foreach (XmlAttribute xAttr in xParameterNode.Attributes)
                {
                    if (String.Compare(xAttr.Name, "IO_IS", true) == 0)
                    {
                        this.IO_IS = xAttr.Value.Trim();
                    }
                    else //if (String.Compare(xAttr.Name, "NAME", true) == 0)
                    {
                        this.ParameterName = xAttr.Name.Trim();
                        this.ParameterValue = xAttr.Value.Trim();
                    }
                    //else if (String.Compare(xAttr.Name, "VALUE", true) == 0)
                    //{
                    //    this.ParameterValue = xAttr.Value;
                    //}
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// Writes this IO Parameter as an XML Element Node attached to xRoot.
        /// </summary>
        /// <param name="xRoot">The XML Element to which this IO Parameter's block should be attached.</param>
        public void WritePlatformIOParameter(XmlElement xRoot)
        {
            XmlDocument xDoc = xRoot.OwnerDocument;

            XmlElement xItemRoot = xDoc.CreateElement("PARAMETER");
            xItemRoot.SetAttribute(this.ParameterName, this.ParameterValue);

            if (String.Compare(this.IO_IS, string.Empty) != 0)
                xItemRoot.SetAttribute("IO_IS", this.IO_IS);

            xRoot.AppendChild(xItemRoot);
        }
    }
}
