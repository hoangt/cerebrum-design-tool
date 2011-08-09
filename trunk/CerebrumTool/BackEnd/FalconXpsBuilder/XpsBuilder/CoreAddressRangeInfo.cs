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
 * CoreAddressRangeInfo.cs
 * Name: Matthew Cotter
 * Date: 15 Feb 2011 
 * Description: Simple object representing an address range on a bus, reserved for a core.
 * History: 
 * >> (15 Feb 2011) Matthew Cotter: Added references to property entry objects for parameters.
 * >> (15 Feb 2011) Matthew Cotter: Imported basic implementation of address range information from CoreAddrInfo class previously defined in deprecated file CoreObject.cs.
 * >> (15 Feb 2011) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CerebrumSharedClasses;
using CerebrumNetronObjects;

namespace FalconXpsBuilder
{
    /// <summary>
    /// Defines all parameters related to an address range assigned to a core.
    /// </summary>
    public class CoreAddressRangeInfo
    {
        /// <summary>
        /// Default constructor.  Initializes all fields to empty strings and lists.
        /// </summary>
        public CoreAddressRangeInfo()
        {
            this.BaseName = string.Empty;
            this.BaseParameter = null;
            this.HighName = string.Empty;
            this.HighParameter = null;
            this.LegalBusList = new List<string>();
            this.MinSize = string.Empty;
            this.Required = false;
            this.IsValidCond = string.Empty;
        }
        /// <summary>
        /// The name of the base address parameter.
        /// </summary>
        public string BaseName { get; set; }
        /// <summary>
        /// The property entry of the base address parameter.
        /// </summary>
        public CerebrumPropertyEntry BaseParameter { get; set; }
        /// <summary>
        /// The name of the high address parameter.
        /// </summary>
        public string HighName { get; set; }
        /// <summary>
        /// The property entry of the high address parameter.
        /// </summary>
        public CerebrumPropertyEntry HighParameter { get; set; }
        /// <summary>
        /// The list of busses to which this address space may be assigned.
        /// </summary>
        public List<string> LegalBusList { get; set; }
        /// <summary>
        /// The minimum size of the address space, if allocated.
        /// </summary>
        public string MinSize { get; set; }
        /// <summary>
        /// A parameter-based formula-style condition that determines whether this parameter is valid or not.
        /// </summary>
        public string IsValidCond { get; set; }
        /// <summary>
        /// A boolean flag indicating whether this address space is required (must be defined) or not.
        /// </summary>
        public bool Required { get; set; }
    }
}
