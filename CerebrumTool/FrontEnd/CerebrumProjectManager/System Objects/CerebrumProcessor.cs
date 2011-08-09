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
 * CerebrumProcessor.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: Representation of on-FPGA processor (PPC or Microblaze)
 * History: 
 * >> (22 Oct 2010) Matthew Cotter: Created simple representation of processor properties used for compiling the embedded Linux.
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
    /// Enumeration of processor types
    /// </summary>
    public enum ProcessorType
    {
        /// <summary>
        /// None (No Processor specified)
        /// </summary>
        None,
        /// <summary>
        /// Power PC
        /// </summary>
        PowerPC,
        /// <summary>
        /// Microblaze
        /// </summary>
        Microblaze
    }

    /// <summary>
    /// Object representing a processor to be configured for Linux compilation
    /// </summary>
    public class CerebrumProcessor
    {
        /// <summary>
        /// Constructor.  Creates a new processor object based on the type of the core defining it
        /// </summary>
        /// <param name="CoreType">The type name of the core defining the processor</param>
        public CerebrumProcessor(string CoreType)
        {
            this.FPGA = string.Empty;
            this.Instance = string.Empty;
            this.CoreType = CoreType;

            this.ConsoleDevice = string.Empty;
            this.DTSFile = string.Empty;
            this.LinuxSourcePath = string.Empty;
            this.MakeConfig = string.Empty;
            this.CompilerArguments = string.Empty;
            this.CerebrumID = string.Empty;

            if ((this.CoreType.ToLower().StartsWith("ppc440_")) ||
                (this.CoreType.ToLower().StartsWith("ppc405_")))
            {
                this.Type = ProcessorType.PowerPC;
            }
            else if (String.Compare(this.CoreType.ToLower(), "microblaze") == 0)
            {
                this.Type = ProcessorType.Microblaze;
            }
        }
        /// <summary>
        /// Get or set the ID of the FPGA device on which the processor will be synthesized
        /// </summary>
        public string FPGA { get; set; }
        /// <summary>
        /// Get or set the instance name of the processor
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// Get or set the type name of the processor
        /// </summary>
        public string CoreType { get; set; }
        /// <summary>
        /// Get or set the generic type of the processor
        /// </summary>
        public ProcessorType Type { get; set; }

        /// <summary>
        /// Get or set the device to be used for console I/O by the processor
        /// </summary>
        public string ConsoleDevice { get; set; }
        /// <summary>
        /// Get or set the name of the DTS file to be used for compilation
        /// </summary>
        public string DTSFile { get; set; }
        /// <summary>
        /// Get or set the processor-specific Linux source path to be used for Linux compilation
        /// </summary>
        public string LinuxSourcePath { get; set; }
        /// <summary>
        /// Get or set the name of the make-configuration to be used for Linux compilation
        /// </summary>
        public string MakeConfig { get; set; }
        /// <summary>
        /// Get or set the arguments to be passed to the compiler
        /// </summary>
        public string CompilerArguments { get; set; }
        /// <summary>
        /// Get or set the CerebrumID to be associated with the processor
        /// </summary>
        public string CerebrumID { get; set; }
        
        /// <summary>
        /// Loads processor configuration information from the specified XML node
        /// </summary>
        /// <param name="ProcessorNode">The Processor node from which the configuration data is to be loaded</param>
        public void LoadFromXml(XmlNode ProcessorNode)
        {
            foreach (XmlNode xProp in ProcessorNode.ChildNodes)
            {
                // OS and Type are Fixed for now
                if (String.Compare(xProp.Name, "ConsoleDevice", true) == 0)
                {
                    this.ConsoleDevice = xProp.InnerText;
                }
                else if (String.Compare(xProp.Name, "FPGA", true) == 0)
                {
                    this.FPGA = xProp.InnerText;
                }
                else if (String.Compare(xProp.Name, "DTS", true) == 0)
                {
                    this.DTSFile = xProp.InnerText;
                }
                else if (String.Compare(xProp.Name, "MakeConfig", true) == 0)
                {
                    this.MakeConfig = xProp.InnerText;
                }
                else if (String.Compare(xProp.Name, "CompilerArgs", true) == 0)
                {
                    this.CompilerArguments = xProp.InnerText;
                }
                else if (String.Compare(xProp.Name, "LinuxSource", true) == 0)
                {
                    this.LinuxSourcePath = xProp.InnerText;
                }
                else if (String.Compare(xProp.Name, "CerebrumID", true) == 0)
                {
                    this.CerebrumID = xProp.InnerText;
                }
            }
        }

        /// <summary>
        /// Saves processor configuration information to an XML node in the schema of the target XML document
        /// </summary>
        /// <param name="TargetDoc">The target XML document</param>
        /// <returns>An XmlNode object containing the this processor's configuration information</returns>
        public XmlNode SaveToXml(XmlDocument TargetDoc)
        {
            XmlElement xProcNode = TargetDoc.CreateElement("Processor");
            xProcNode.SetAttribute("Instance", this.Instance);

            if ((this.FPGA != null) && (this.FPGA != string.Empty))
            {
                XmlNode xFPGA = TargetDoc.CreateElement("FPGA");
                xFPGA.InnerText = this.FPGA;
                xProcNode.AppendChild(xFPGA);
            }

            XmlNode xOS = TargetDoc.CreateElement("OS");
            xOS.InnerText = "Linux";
            xProcNode.AppendChild(xOS);

            XmlNode xType = TargetDoc.CreateElement("Type");
            xType.InnerText = this.Type.ToString();
            xProcNode.AppendChild(xType);

            XmlNode xConsole = TargetDoc.CreateElement("ConsoleDevice");
            xConsole.InnerText = this.ConsoleDevice;
            xProcNode.AppendChild(xConsole);

            XmlNode xDTS = TargetDoc.CreateElement("DTS");
            xDTS.InnerText = this.DTSFile;
            xProcNode.AppendChild(xDTS);

            if ((this.LinuxSourcePath != null) && (this.LinuxSourcePath != string.Empty))
            {
                XmlNode xLinuxSource = TargetDoc.CreateElement("LinuxSource");
                xLinuxSource.InnerText = this.LinuxSourcePath;
                xProcNode.AppendChild(xLinuxSource);
            }

            XmlNode xMakeConfig = TargetDoc.CreateElement("MakeConfig");
            xMakeConfig.InnerText = this.MakeConfig;
            xProcNode.AppendChild(xMakeConfig);

            XmlNode xCompilerArgs = TargetDoc.CreateElement("CompilerArgs");
            xCompilerArgs.InnerText = this.CompilerArguments;
            xProcNode.AppendChild(xCompilerArgs);

            XmlNode xCerebrumID = TargetDoc.CreateElement("CerebrumID");
            xCerebrumID.InnerText = this.CerebrumID;
            xProcNode.AppendChild(xCerebrumID);

            return xProcNode;
        }
    }
}
