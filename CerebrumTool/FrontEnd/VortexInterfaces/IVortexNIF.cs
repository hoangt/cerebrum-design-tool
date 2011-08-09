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
 * IVortexNIF.cs
 * Name: Matthew Cotter
 * Date: 19 Oct 2010 
 * Description: Interface to implement a generic Vortex NIF
 * History: 
 * >> (19 Oct 2010) Matthew Cotter: Moved Generic NIF properties to from ISAPNIF and ISOPNIF interfaces.
 * >> (19 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FalconResources;

namespace VortexInterfaces
{
    /// <summary>
    /// Interface defining a generic Vortex Network interface object.
    /// </summary>
    public interface IVortexNIF
    {
        #region Properties
        /// <summary>
        /// Core Type name of the NIF
        /// </summary>
        string Type { get; }
        /// <summary>
        /// Instance name of the NIF
        /// </summary>
        string Instance { get; set; }
        /// <summary>
        /// Version Info of the NIF
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Boolean indicating whether the NIF is configurable (enabling its configuration interface)
        /// </summary>
        bool IsConfigurable { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the set of required resources to synthesize the NIF in hardware
        /// </summary>
        /// <returns>A ResourceInfo object containing the required resources.</returns>
        ResourceInfo GetResources();

        /// <summary>
        /// Returns the path to a file containing configuration information for the NIF
        /// </summary>
        /// <returns></returns>
        string GetNIFConfigs();
        #endregion
    }
}
