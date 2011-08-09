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
 * PCoreIOInterface.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: Defines a basic PCore IO Interface, as parsed from the MPD.
 * History: 
// >> (15 Feb 2010) Matthew Cotter: Migrated PCoreIOInterface class from XPSBuilder to CerebrumSharedClasses where it may be used by all libraries rather than each having its own copies.
 * >> (22 Oct 2010) Matthew Cotter: Created basic definition of PCore IO Interface, as parsed from the MPD.
 * >> (22 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CerebrumSharedClasses.MPD_Interfaces
{
    /// <summary>
    /// Represents a simple IO definition as defined in the PCore's MPD IO_INTERFACE line
    /// </summary>
    public class PCoreIOInterface
    {
        /// <summary>
        /// Default constructor.  Initializes an empty PCoreIOInterface
        /// </summary>
        public PCoreIOInterface()
        {
            this.IO_IF = string.Empty;
            this.IO_TYPE = string.Empty;
            this.Ports = new List<PCorePort>();
            this.Parameters = new List<PCoreInterfaceParameter>();
        }

        /// <summary>
        /// The name of the IO interface
        /// </summary>
        public string IO_IF { get; set; }
        /// <summary>
        /// The type of the IO interface
        /// </summary>
        public string IO_TYPE { get; set; }

        /// <summary>
        /// The list of PCore ports associated with the IO interface
        /// </summary>
        public List<PCorePort> Ports { get; set; }
        /// <summary>
        /// The list of PCore parameters associated with the IO interface
        /// </summary>
        public List<PCoreInterfaceParameter> Parameters { get; set; }
    }
}
