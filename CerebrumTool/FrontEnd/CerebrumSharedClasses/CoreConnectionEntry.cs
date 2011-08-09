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
 * CoreConnectionEntry.cs
 * Name: Matthew Cotter
 * Date: 19 Oct 2010 
 * Description: Defines a core connection used in generating, defining and building the XPS project(s).
 * History: 
 * >> (19 Oct 2010) Matthew Cotter: Created basic definition of a core connection (PORT or BUS_INTERFACE)
 * >> (19 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CerebrumSharedClasses
{
    /// <summary>
    /// Object used to represent a pcore connection of either a port or bus in generating XPS projects.
    /// </summary>
    public class CoreConnectionEntry
    {
        /// <summary>
        /// Enumeration of connection types
        /// </summary>
        public enum CoreConnectionType
        {
            /// <summary>
            /// Indicates that the connection is a port/signal interface
            /// </summary>
            PORT,
            /// <summary>
            /// Indicates that the connection is a bus interface
            /// </summary>
            BUS_INTERFACE
        }

        /// <summary>
        /// Get or set the type of connection defined by this object
        /// </summary>
        public CoreConnectionType ConnectionType { get; set; }
        /// <summary>
        /// Get or set the name of the connection port/bus
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get or set the name of the signal the port/bus connects to
        /// </summary>
        public string Signal { get; set; }

        /// <summary>
        /// Indicates whether the port/bus is connected to a signal
        /// </summary>
        public bool Connected
        {
            get
            {
                return (this.Signal != string.Empty);
            }
        }

        /// <summary>
        /// Constructor creates a pre-connected connection
        /// </summary>
        /// <param name="Type">The type of the connection</param>
        /// <param name="Name">The name of the connection</param>
        /// <param name="Signal">The name of the signal the connection is attached to</param>
        public CoreConnectionEntry(CoreConnectionType Type, string Name, string Signal)
        {
            this.ConnectionType = Type;
            this.Name = Name;
            this.Signal = Signal;
        }
        /// <summary>
        /// Default constructor.  Creates an unnamed, unconnected, port-type connection
        /// </summary>
        public CoreConnectionEntry()
        {
            this.ConnectionType = CoreConnectionType.PORT;
            this.Name = string.Empty;
            this.Signal = string.Empty;
        }
    }
}
