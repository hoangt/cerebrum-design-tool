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
 * PCorePort.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: Defines a basic PCore IO Port, as parsed from the MPD.
 * History: 
// >> (15 Feb 2010) Matthew Cotter: Migrated PCorePort class from XPSBuilder to CerebrumSharedClasses where it may be used by all libraries rather than each having its own copies.
 * >> (22 Oct 2010) Matthew Cotter: Created basic definition of PCore IO Port, as parsed from the MPD.
 * >> (22 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CerebrumSharedClasses.MPD_Interfaces
{
    /// <summary>
    /// Represents a Port Definition within a PCore file
    /// </summary>
    public class PCorePort
    {
        /// <summary>
        /// Default constructor.  Initializes and empty port specification.
        /// </summary>
        public PCorePort()
        {
            this.PortName = string.Empty;
            this.Direction = string.Empty;
            this.SigIs = string.Empty;
            this.Sensitivity = string.Empty;
            this.Interrupt_Priority = string.Empty;
            this.IO_IF = new List<string>();
            this.IO_IS = string.Empty;
            this.InitialVal = string.Empty;
            this.ValidCond = string.Empty;
            ClearVector();
        }

        /// <summary>
        /// The Name of the Port
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// The IO direction of the port
        /// </summary>
        public string Direction { get; set; }
        /// <summary>
        /// Indicates whether the port is a multi-pin vector
        /// </summary>
        public bool IsVector { get; set; }
        /// <summary>
        /// If the port is a multi-pin vector, indicates the low pin of the vector
        /// </summary>
        public int VectorLow { get; set; }
        /// <summary>
        /// If the port is a multi-pin vector, indicates the high pin of the vector
        /// </summary>
        public int VectorHigh { get; set; }

        /// <summary>
        /// The raw vector string as read in from the MPD file.
        /// </summary>
        public string VectorString { get; set; }
        /// <summary>
        /// Indicates the type of the signal
        /// </summary>
        public string SigIs { get; set; }
        /// <summary>
        /// Indicates the sensitivity of the signal
        /// </summary>
        public string Sensitivity { get; set; }
        /// <summary>
        /// Indicates the interrupt priority of the signal
        /// </summary>
        public string Interrupt_Priority { get; set; }

        /// <summary>
        /// Indicates the IO interfaces of which the port is a part.
        /// </summary>
        public List<string> IO_IF { get; set; }
        /// <summary>
        /// Indicates the interface port to which this port corresponds
        /// </summary>
        public string IO_IS { get; set; }

        /// <summary>
        /// Indicates the initial value of the signal
        /// </summary>
        public string InitialVal { get; set; }
        /// <summary>
        /// Indicates conditions under which setting this port is valid
        /// </summary>
        public string ValidCond { get; set; }

        /// <summary>
        /// Parses a string as a vector and if successful, sets the corresponding parameters in the port.
        /// </summary>
        /// <param name="VectorSpec">The string representing a vector specification</param>
        public void ParseVector(string VectorSpec)
        {
            ClearVector();
            if ((VectorSpec == null) || (VectorSpec == string.Empty))
                return;
            this.VectorString = VectorSpec;
            VectorSpec = VectorSpec.Trim('[').Trim(']');
            int split = VectorSpec.IndexOf(':');
            string VLow = VectorSpec.Substring(0, split);
            string VHigh = VectorSpec.Substring(split + 1);
            int low = Conditions.EvaluateAsInteger(VLow);
            int high = Conditions.EvaluateAsInteger(VHigh);
            if ((low != high) && (low >= 0) && (high >= 0))
            {
                if (high < low)
                {
                    int t = low;
                    low = high;
                    high = t;
                }
                this.VectorLow = low;
                this.VectorHigh = high;
                this.IsVector = true;
            }
        }

        /// <summary>
        /// Resets any vector-related parameters in the port
        /// </summary>
        public void ClearVector()
        {
            this.VectorString = string.Empty;
            this.IsVector = false;
            this.VectorLow = 0;
            this.VectorHigh = 0;
        }

        /// <summary>
        /// Parses a PORT line from the MPD file
        /// </summary>
        /// <param name="PortLine">The line to parse</param>
        public void ParseMPDPort(string PortLine)
        {
            string portPropertyValue = string.Empty;
            string portPropertyName = string.Empty;
            ClearVector();

            this.IO_IF = new List<string>();
            while (PortLine.Length > 0)
            {
                int cIdx = PortLine.IndexOf(',');
                string partialLine = string.Empty;

                if (cIdx < 0)
                {
                    partialLine = PortLine.Trim();
                    PortLine = string.Empty;
                }
                else
                {
                    partialLine = PortLine.Substring(0, cIdx).Trim();
                    PortLine = PortLine.Substring(cIdx + 1).Trim();
                }
                int eqIdx = partialLine.IndexOf("=");
                if (eqIdx < 0)
                    return;
                portPropertyName = partialLine.Substring(0, eqIdx - 1).Trim();
                portPropertyValue = partialLine.Substring(eqIdx + 1).Trim();

                if (portPropertyName.StartsWith("PORT"))
                {
                    int spIdx = portPropertyName.IndexOf(" ");
                    if (spIdx < 0)
                        return;
                    portPropertyValue = portPropertyName.Substring(spIdx + 1).Trim();
                    portPropertyName = portPropertyName.Substring(0, spIdx).Trim();

                    this.PortName = portPropertyValue;
                }
                else if (portPropertyName == "DIR")
                {
                    this.Direction = portPropertyValue;
                }
                else if (portPropertyName == "SIGIS")
                {
                    this.SigIs = portPropertyValue;
                }
                else if (portPropertyName == "INITIALVAL")
                {
                    this.InitialVal = portPropertyValue;
                }
                else if (portPropertyName == "SENSITIVITY")
                {
                    this.Sensitivity = portPropertyValue;
                }
                else if (portPropertyName == "INTERRUPT_PRIORITY")
                {
                    this.Interrupt_Priority = portPropertyValue;
                }
                else if (portPropertyName == "IO_IF")
                {
                    string[] IO_IFs = portPropertyValue.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    this.IO_IF.AddRange(IO_IFs);
                }
                else if (portPropertyName == "VEC")
                {
                    this.VectorString = portPropertyValue;
                    //this.ParseVector(portPropertyValue);
                }
                else if (portPropertyName == "IO_IS")
                {
                    this.IO_IS = portPropertyValue;
                }
                else if (portPropertyName == "ISVALID")
                {
                    this.ValidCond = portPropertyValue;
                }
            }
        }
    }
}
