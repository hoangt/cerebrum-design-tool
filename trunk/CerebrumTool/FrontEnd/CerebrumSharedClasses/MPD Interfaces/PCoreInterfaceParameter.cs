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
 * PCoreInterfaceParameter.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: Defines a basic PCore Interface Parameter, as parsed from the MPD.
 * History: 
// >> (15 Feb 2010) Matthew Cotter: Migrated PCoreInterfaceParameter class from XPSBuilder to CerebrumSharedClasses where it may be used by all libraries rather than each having its own copies.
 * >> (22 Oct 2010) Matthew Cotter: Created basic definition of PCore Interface Parameter, as parsed from the MPD.
 * >> (22 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CerebrumSharedClasses.MPD_Interfaces
{
    /// <summary>
    /// Represents a Parameter Definition within a PCore file
    /// </summary>
    public class PCoreInterfaceParameter
    {
        /// <summary>
        /// Default constructor.  Initializes and empty parameter specification.
        /// </summary>
        public PCoreInterfaceParameter()
        {
            this.ParameterName = string.Empty;
            this.IO_IF = new List<string>();
            this.IO_IS = string.Empty;
            this.ValidCond = string.Empty;
        }

        /// <summary>
        /// The Name of the parameter
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// Indicates the IO interfaces of which the parameter is a part.
        /// </summary>
        public List<string> IO_IF { get; set; }
        /// <summary>
        /// Indicates the interface port to which this parameter corresponds
        /// </summary>
        public string IO_IS { get; set; }
        /// <summary>
        /// Indicates conditions under which setting this parameter is valid
        /// </summary>
        public string ValidCond { get; set; }

        /// <summary>
        /// Parses a PARAMETER line from the MPD file
        /// </summary>
        /// <param name="ParameterLine">The line to parse</param>
        public void ParseMPDParameter(string ParameterLine)
        {
            string parameterPropertyValue = string.Empty;
            string parameterPropertyName = string.Empty;

            this.IO_IF = new List<string>();
            while (ParameterLine.Length > 0)
            {
                int cIdx = ParameterLine.IndexOf(',');
                string partialLine = string.Empty;

                if (cIdx < 0)
                {
                    partialLine = ParameterLine.Trim();
                    ParameterLine = string.Empty;
                }
                else
                {
                    partialLine = ParameterLine.Substring(0, cIdx).Trim();
                    ParameterLine = ParameterLine.Substring(cIdx + 1).Trim();
                }
                int eqIdx = partialLine.IndexOf("=");
                if (eqIdx < 0)
                    return;
                parameterPropertyName = partialLine.Substring(0, eqIdx - 1).Trim();
                parameterPropertyValue = partialLine.Substring(eqIdx + 1).Trim();

                if (parameterPropertyName.StartsWith("PARAMETER"))
                {
                    int spIdx = parameterPropertyName.IndexOf(" ");
                    if (spIdx < 0)
                        return;
                    parameterPropertyValue = parameterPropertyName.Substring(spIdx + 1).Trim();
                    parameterPropertyName = parameterPropertyName.Substring(0, spIdx).Trim();

                    this.ParameterName = parameterPropertyValue;
                }
                else if (parameterPropertyName == "IO_IF")
                {
                    string[] IO_IFs = parameterPropertyValue.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    this.IO_IF.AddRange(IO_IFs);
                }
                else if (parameterPropertyName == "IO_IS")
                {
                    this.IO_IS = parameterPropertyValue;
                }
                else if (parameterPropertyName == "ISVALID")
                {
                    this.ValidCond = parameterPropertyValue;
                }
            }
        }
    }
}
