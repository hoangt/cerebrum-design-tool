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
 * IVortexBridgeAttachment.cs
 * Name: Matthew Cotter
 * Date:  2 Feb 2011 
 * Description: Interface defining a Vortex-to-Vortex bridge 'end-cap'
 * History: 
 * >> ( 2 Feb 2011) Matthew Cotter: Created basic interface defining a Vortex Bridge attachment object.
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
    /// Defines the public interface for representing a Vortex Bridge attachment object.
    /// </summary>
    public interface IVortexBridgeAttachment : IVortexAttachment
    {
        /// <summary>
        /// Returns the set of required resources to synthesize the Bridge Attachment in hardware
        /// </summary>
        /// <returns>A ResourceInfo object containing the required resources.</returns>
        ResourceInfo GetResources();

        /// <summary>
        /// Get or set the IVortexBridge of which this attachment is a part.
        /// </summary>
        IVortexBridge Bridge { get; set; }
    }
}
