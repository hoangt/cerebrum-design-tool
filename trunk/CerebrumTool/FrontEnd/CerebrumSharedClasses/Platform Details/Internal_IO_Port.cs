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
    /// Represents a PORT associated with an IO_INTERFACE or an IO_ADAPTER in the Platform FPGA Specification.
    /// </summary>
    public class Internal_IO_Port
    {
        /// <summary>
        /// Default constructor.  Creates an empty port specification.
        /// </summary>
        public Internal_IO_Port()
        {
            this.PortName = string.Empty;
            this.SignalName = string.Empty;
            this.IO_IS = string.Empty;
            this.SigIs = string.Empty;
            this.Sensitivity = string.Empty;
            this.InitialVal = string.Empty;
            this.Interrupt_Priority = string.Empty;
        }
        /// <summary>
        /// Creates a port specification based on the contents of the specified XML PORT Node.
        /// </summary>
        /// <param name="xPortNode">The XML 'PORT' node that contains information about the port</param>
        public Internal_IO_Port(XmlNode xPortNode)
        {
            ParsePortNode(xPortNode);
        }

        /// <summary>
        /// The name of the port
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// The Name of the signal associated with the port
        /// </summary>
        public string SignalName { get; set; }
        /// <summary>
        /// Indicator of how the port may be exposed externally from the FPGA
        /// </summary>
        public string IO_IS { get; set; }
        /// <summary>
        /// String indicating what the signal is.
        /// </summary>
        public string SigIs { get; set; }
        /// <summary>
        /// String indicating the sensitivity level of the signal
        /// </summary>
        public string Sensitivity { get; set; }
        /// <summary>
        /// String indicating the initial value of the signal, if applicable.
        /// </summary>
        public string InitialVal { get; set; }
        /// <summary>
        /// String indicating interrupt priority, if the signal is an interrupt.
        /// </summary>
        public string Interrupt_Priority { get; set; }
        /// <summary>
        /// String representing the conditions under which this interface signal is applicable to the core
        /// </summary>
        public string ValidCondition { get; set; }

        
        /// <summary>
        /// Populate this object's properties based on the information contained in the specified XBD Port entry.
        /// </summary>
        /// <param name="PortLine">The XBD 'PORT' string that contains information about the port</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParseXBDPort(string PortLine)
        {
            try
            {
                this.PortName = string.Empty;
                this.SignalName = string.Empty;
                this.IO_IS = string.Empty;
                this.SigIs = string.Empty;
                this.Sensitivity = string.Empty;
                this.InitialVal = string.Empty;
                this.Interrupt_Priority = string.Empty;
                this.ValidCondition = string.Empty;

                PortLine = PortLine.Substring(5).Trim();

                string[] PortAttributes = PortLine.Split(',');

                for (int i = 0; i < PortAttributes.Length; i++)
                {
                    string PortAttribute = PortAttributes[i];
                    string[] KeyValue = PortAttribute.Split('=');
                    string Key = KeyValue[0].Trim();
                    string Value = KeyValue[1].Trim();
                    if (i == 0)
                    {
                        // Port Name
                        this.PortName = Key.Replace('[', '_').Replace(']', '_');
                        this.SignalName = Value;
                    }
                    else
                    {
                        if (String.Compare(Key, "IO_IS", true) == 0)
                        {
                            this.IO_IS = Value;
                        }
                        else if (String.Compare(Key, "SIGIS", true) == 0)
                        {
                            this.SigIs = Value;
                        }
                        else if (String.Compare(Key, "SENSITIVITY", true) == 0)
                        {
                            this.Sensitivity = Value;
                        }
                        else if (String.Compare(Key, "INTERRUPT_PRIORITY", true) == 0)
                        {
                            this.Interrupt_Priority = Value;
                        }
                        else if (String.Compare(Key, "VALID", true) == 0)
                        {
                            this.ValidCondition = Value;
                        }
                        else if (String.Compare(Key, "INITIALVAL", true) == 0)
                        {
                            this.InitialVal = Value;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                this.PortName = string.Empty;
                this.SignalName = string.Empty;
                this.IO_IS = string.Empty;
                this.SigIs = string.Empty;
                this.Sensitivity = string.Empty;
                this.InitialVal = string.Empty;
                this.Interrupt_Priority = string.Empty;
                this.ValidCondition = string.Empty;
                return false;
            }
        }
        /// <summary>
        /// Populate this object's properties based on the information contained in the specified XML Node.
        /// </summary>
        /// <param name="xPortNode">The XML 'PORT' node that contains information about the port</param>
        /// <returns>True if parsing was successful.  False, otherwise.</returns>
        public bool ParsePortNode(XmlNode xPortNode)
        {
            this.PortName = string.Empty;
            this.SignalName = string.Empty;
            this.IO_IS = string.Empty;
            this.SigIs = string.Empty;
            this.Sensitivity = string.Empty;
            this.InitialVal = string.Empty;
            this.Interrupt_Priority = string.Empty;

            if (String.Compare(xPortNode.Name, "Port", true) == 0)
            {
                foreach (XmlAttribute xAttr in xPortNode.Attributes)
                {
                    if (String.Compare(xAttr.Name, "IO_IS", true) == 0)
                    {
                        this.IO_IS = xAttr.Value.Trim();
                    }
                    else if (String.Compare(xAttr.Name, "SIGIS", true) == 0)
                    {
                        this.SigIs = xAttr.Value.Trim();
                    }
                    else if (String.Compare(xAttr.Name, "SENSITIVITY", true) == 0)
                    {
                        this.Sensitivity = xAttr.Value.Trim();
                    }
                    else if (String.Compare(xAttr.Name, "INTERRUPT_PRIORITY", true) == 0)
                    {
                        this.Interrupt_Priority = xAttr.Value.Trim();
                    }
                    else if (String.Compare(xAttr.Name, "VALID", true) == 0)
                    {
                        this.ValidCondition = xAttr.Value.Trim();
                    }
                    else if (String.Compare(xAttr.Name, "INITIALVAL", true) == 0)
                    {
                        this.InitialVal = xAttr.Value.Trim();
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
        /// Writes this IO Port as an XML Element Node attached to xRoot.
        /// </summary>
        /// <param name="xRoot">The XML Element to which this IO Port's block should be attached.</param>
        public void WritePlatformIOPort(XmlElement xRoot)
        {
            XmlDocument xDoc = xRoot.OwnerDocument;

            XmlElement xItemRoot = xDoc.CreateElement("PORT");
            xItemRoot.SetAttribute(this.PortName, this.SignalName);

            if (String.Compare(this.IO_IS, string.Empty) != 0)
                xItemRoot.SetAttribute("IO_IS", this.IO_IS);

            if (String.Compare(this.SigIs, string.Empty) != 0)
                xItemRoot.SetAttribute("SIGIS", this.SigIs);

            if (String.Compare(this.Sensitivity, string.Empty) != 0)
                xItemRoot.SetAttribute("SENSITIVITY", this.Sensitivity);

            if (String.Compare(this.InitialVal, string.Empty) != 0)
                xItemRoot.SetAttribute("INITIALVAL", this.InitialVal);

            if (String.Compare(this.Interrupt_Priority, string.Empty) != 0)
                xItemRoot.SetAttribute("INTERRUPT_PRIORITY", this.Interrupt_Priority);

            if (String.Compare(this.ValidCondition, string.Empty) != 0)
                xItemRoot.SetAttribute("VALID", this.ValidCondition);

            xRoot.AppendChild(xItemRoot);
        }
    }
}
