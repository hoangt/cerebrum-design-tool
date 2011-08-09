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
 * IVortexBridge.cs
 * Name: Matthew Cotter
 * Date:  2 Feb 2010 
 * Description: Interface to represent and implemention of a Vortex-to-Vortex 'bridge'
 * History: 
 * >> ( 2 Feb 2011) Matthew Cotter: Created basic interface defining a Vortex 'Bridge' object.
 * >> ( 2 Feb 2011) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FalconResources;

namespace VortexInterfaces
{
    /// <summary>
    /// Defines the public interface for abstracting interface between two Vortex routers
    /// </summary>
    public interface IVortexBridge : IVortexAttachment
    {        
        /// <summary>
        /// This value should indicate the port the attachment is connected to on the first Vortex switch.   If the device
        /// is not attached, this value should be negative.
        /// </summary>
        int Port_1 { get; }
        /// <summary>
        /// This value should indicate the full Target Device Address of this attachment, including the addresses of both itself
        /// and the first Vortex to which it is attached.  If the device is not attached, this value should be negative.
        /// </summary>
        int TDA_1 { get; }
        /// <summary>
        /// This value should indicate the port the attachment is connected to on the second Vortex switch.   If the device
        /// is not attached, this value should be negative.
        /// </summary>
        int Port_2 { get; }
        /// <summary>
        /// This value should indicate the full Target Device Address of this attachment, including the addresses of both itself
        /// and the second Vortex to which it is attached.  If the device is not attached, this value should be negative.
        /// </summary>
        int TDA_2 { get; }
        
        /// <summary>
        /// Boolean indicating whether this device is attached to two Vortex routers.
        /// </summary>
        bool Bridged { get; }

        /// <summary>
        /// Boolean indicating whether this device is attached to the 'first' Vortex router.
        /// </summary>
        bool Attached_1 { get; }
        /// <summary>
        /// Boolean indicating whether this device is attached to the 'second' Vortex router.
        /// </summary>
        bool Attached_2 { get; }

        /// <summary>
        /// Get the first IVortex object to which the device is attached, if any.
        /// </summary>
        IVortex AttachedTo_1 { get; }

        /// <summary>
        /// Get the second IVortex object to which the device is attached, if any.
        /// </summary>
        IVortex AttachedTo_2 { get; }

        /// <summary>
        /// Returns the set of required resources to synthesize the Bridge in hardware
        /// </summary>
        /// <returns>A ResourceInfo object containing the required resources.</returns>
        ResourceInfo GetResources();

        /// <summary>
        /// Creates a bridge between the two specified Vortex routers, but attaching the corresponding ends to the specified routers.
        /// </summary>
        /// <param name="Vortex1">The 'first' Vortex in the bridge.</param>
        /// <param name="Vortex2">The 'second' Vortex in the bridge.</param>
        void Bridge(IVortex Vortex1, IVortex Vortex2);

        /// <summary>
        /// Disconnects both ends of the bridge from any attached vortex routers.
        /// </summary>
        void Disconnect();
    }
}
