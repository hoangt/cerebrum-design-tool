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
 * Vortex_SAP_NIF.cs
 * Name: Matthew Cotter
 * Date: 18 Oct 2010 
 * Description: Implementation of ISAP_NIF as a Vortex SAP NIF object
 * History: 
 * >> (18 Oct 2010) Matthew Cotter: Defined implementation of ISAP_NIF interface corresponding to the current version of Vortex Router SAP NIF
 * >> (18 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VortexInterfaces;
using FalconResources;

namespace VortexObjects
{
    /// <summary>
    /// Implementation of ISAP_NIF interface to represent a Vortex SAP NIF object
    /// </summary>
    public class Vortex_SAP_NIF : ISAP_NIF
    {
        #region Properties

        /// <summary>
        /// Core Type name of the router
        /// </summary>
        public string Type
        {
            get
            {
                return "nif_sap";
            }
        }
        /// <summary>
        /// Instance name of the router
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// Version Info of the router
        /// </summary>
        public string Version
        {
            get
            {
                return "1.00.c";
            }
        }

        /// <summary>
        /// Boolean indicating whether the NIF is configurable (enabling its configuration interface)
        /// </summary>
        public bool IsConfigurable 
        { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Returns the set of required resources to synthesize the NIF in hardware
        /// </summary>
        /// <returns>A ResourceInfo object containing the required resources.</returns>
        public ResourceInfo GetResources()
        {
            ResourceInfo RI = new ResourceInfo();

            RI.SetResource("Slice Registers", 3257);
            RI.SetResource("Slice LUTs", 9401);
            RI.SetResource("BRAMs", 5);

            return RI;
        }

        /// <summary>
        /// Returns the path to a file containing configuration information for the NIF
        /// </summary>
        /// <returns></returns>
        public string GetNIFConfigs()
        {
            return string.Empty;
        }
        #endregion
    }
}
