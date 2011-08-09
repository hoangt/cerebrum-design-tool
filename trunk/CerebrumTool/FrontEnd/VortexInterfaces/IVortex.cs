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
 * IVortex.cs
 * Name: Matthew Cotter
 * Date:  7 Oct 2010 
 * Description: Interface to implement a Vortex object
 * History: 
 * >> (22 Mar 2011) Matthew Cotter: Added configuration attachment property for use in determining what attachment, if any, is to be used for configuration.
 * >> (18 Oct 2010) Matthew Cotter: Added type, instance and version to generic properties for use in generating XPS Projects.
 * >> ( 7 Oct 2010) Matthew Cotter: Created basic interface defining a Vortex object.
 * >> ( 7 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CerebrumSharedClasses;
using FalconResources;

namespace VortexInterfaces
{
    /// <summary>
    /// Defines the public interface for configuring and manipulating the parameters of a Vortex FPGA Switch/Router component in software
    /// </summary>
    public interface IVortex 
    {
        #region Properties
        /// <summary>
        /// Core Type name of the router
        /// </summary>
        string Type { get; }
        /// <summary>
        /// Instance name of the router
        /// </summary>
        string Instance { get; set; }
        /// <summary>
        /// Version Info of the router
        /// </summary>
        string Version { get; }

        /// <summary>
        /// This value should indicate the Local Switch ID of the Vortex Router.
        /// </summary>
        int LocalSwitchID { get; set; }

        /// <summary>
        /// Boolean indicating whether the Vortex switch is configurable (enabling its configuration interface)
        /// </summary>
        bool IsConfigurable { get; set; }

        /// <summary>
        /// Get or set the attachment to be used for configuring the Vortex.
        /// </summary>
        IVortexAttachment ConfigurationAttachment { get; set; }

        /// <summary>
        /// Indicates the Bus ID of the Vortex switch
        /// </summary>
        int BusID { get; }
        /// <summary>
        /// Indicates the Switch ID of the Vortex switch
        /// </summary>
        int SwitchID { get; }
        /// <summary>
        /// Indicates the number of ports on the Vortex switch
        /// </summary>
        int NumPorts { get; }
        /// <summary>
        /// Indicates the number of currently unattached ports on the Vortex switch
        /// </summary>
        int AvailablePorts { get; }

        /// <summary>
        /// List of devices that have been attached to the Vortex switch
        /// </summary>
        List<IVortexAttachment> AttachedDevices { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the set of required resources to synthesize the Vortex switch in hardware
        /// </summary>
        /// <returns></returns>
        ResourceInfo GetResources();
        /// <summary>
        /// Attaches the specified IVortexAttachment to the Vortex switch, if it is not already attached.
        /// </summary>
        /// <param name="Device">The IVortexAttachment to attach.</param>
        void AttachDevice(IVortexAttachment Device);
        /// <summary>
        /// Detaches the specified IVortexAttachment from the Vortex switch, if it is attached.
        /// </summary>
        /// <param name="Device">The IVortexAttachment to detach.</param>
        void DetachDevice(IVortexAttachment Device);
        #endregion
    }
}
