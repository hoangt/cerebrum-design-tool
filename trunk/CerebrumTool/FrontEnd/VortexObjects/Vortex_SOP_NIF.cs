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
 * Vortex_SOP_NIF.cs
 * Name: Matthew Cotter
 * Date: 18 Oct 2010 
 * Description: Implementation of ISOP_NIF as a Vortex SAP NIF object
 * History: 
 * >> (18 Oct 2010) Matthew Cotter: Defined implementation of ISOP_NIF interface corresponding to the current version of Vortex Router SOP NIF
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
    /// Implementation of ISOP_NIF interface to represent a Vortex SOP NIF object
    /// </summary>
    public class Vortex_SOP_NIF : ISOP_NIF
    {
        #region Properties
        /// <summary>
        /// Core Type name of the NIF
        /// </summary>
        public string Type
        {
            get
            {
                return "nif_sop";
            }
        }
        /// <summary>
        /// Instance name of the NIF
        /// </summary>
        public string Instance { get; set; }
        /// <summary>
        /// Version Info of the NIF
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
        public bool IsConfigurable { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Returns the set of required resources to synthesize the NIF in hardware
        /// </summary>
        /// <returns>A ResourceInfo object containing the required resources.</returns>
        public ResourceInfo GetResources()
        {
            ResourceInfo RI = new ResourceInfo();

            RI.SetResource("Slice Registers", 765);
            RI.SetResource("Slice LUTs", 750);
            RI.SetResource("BRAMs", 10);

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
