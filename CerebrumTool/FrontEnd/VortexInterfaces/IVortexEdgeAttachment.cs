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
 * IVortexEdgeAttachment.cs
 * Name: Matthew Cotter
 * Date: 20 Apr 2011 
 * Description: Interface defining a Vortex-to-Physical Edge Component 'bridge'
 * History: 
 * >> (20 Apr 2011) Matthew Cotter: Created basic interface defining a Vortex Edge attachment object.
 * >> (20 Apr 2011) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FalconResources;

namespace VortexInterfaces
{
    /// <summary>
    /// Defines the public interface for representing a Vortex Edge attachment object.
    /// </summary>
    public interface IVortexEdgeAttachment : IVortexAttachment
    {
        /// <summary>
        /// Returns the set of required resources to synthesize the Bridge Attachment in hardware
        /// </summary>
        /// <returns>A ResourceInfo object containing the required resources.</returns>
        ResourceInfo GetResources();

        /// <summary>
        /// Get or set the Edge Component object to which this attachment is connected.
        /// </summary>
        object EdgeComponent { get; set; }
    }
}
