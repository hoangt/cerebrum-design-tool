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
 * IVortexAttachment.cs
 * Name: Matthew Cotter
 * Date:  7 Oct 2010 
 * Description: Interface to implement a generic Vortex-attached object
 * History: 
 * >> (19 Oct 2010) Matthew Cotter: Added type, instance and version to generic properties for use in generating XPS Projects.
 * >> (18 Oct 2010) Matthew Cotter: Added NIF property to allow top level object access to implied NIF's methods and resources.
 * >> ( 7 Oct 2010) Matthew Cotter: Created basic interface defining a generic Vortex-attached object.
 * >> ( 7 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VortexInterfaces
{
    /// <summary>
    /// Defines the generics of an attachment to the Vortex switch
    /// </summary>
    public interface IVortexAttachment
    {
        #region Properties
        /// <summary>
        /// Core Type name of the Attachment
        /// </summary>
        string Type { get; set; }
        /// <summary>
        /// Native Core Instance name of the Attachment
        /// </summary>
        string CoreInstance { get; set; }
        /// <summary>
        /// Instance name of the Attachment
        /// </summary>
        string Instance { get; set; }
        /// <summary>
        /// Version Info of the Attachment
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// Flag indicating whether the attachment is capable of configuring the Vortex to which it is attached
        /// </summary>
        bool ConfiguresVortex { get; set; }

        /// <summary>
        /// This value should indicate the port the attachment is connected to on a Vortex switch.   If the device
        /// is not attached, this value should be negative.
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// This value should indicate the full Target Device Address of this attachment, including the addresses of both itself
        /// and the Vortex to which it is attached.  If the device is not attached, this value should be negative.
        /// </summary>
        int TDA { get; set; }

        /// <summary>
        /// Boolean indicating whether this device is attached to a Vortex switch.
        /// </summary>
        bool Attached { get; }

        /// <summary>
        /// Get or set the IVortex object to which the device is attached, if any.
        /// </summary>
        IVortex AttachedTo { get; set; }

        /// <summary>
        /// Returns the NIF used by the Attachment
        /// </summary>
        IVortexNIF NIF { get; }
        #endregion
    }
}
