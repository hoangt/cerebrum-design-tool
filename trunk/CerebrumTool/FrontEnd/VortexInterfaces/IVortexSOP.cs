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
 * IVortexSOP.cs
 * Name: Matthew Cotter
 * Date:  7 Oct 2010 
 * Description: Interface to implement a Vortex SOP object
 * History: 
 * >> ( 7 Oct 2010) Matthew Cotter: Created basic interface defining a Vortex SOP object.
 * >> ( 7 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VortexInterfaces
{
    /// <summary>
    /// Defines the public interface for configuring and manipulating the parameters of a hardware core that 
    /// conforms to the Vortex Streaming-Operator hardware interface
    /// </summary>
    public interface IVortexSOP :  IVortexAttachment
    {
        /// <summary>
        /// Boolean indicating whether the SOP is configurable (accepting configuration information)
        /// </summary>
        bool IsConfigurable { get; set; }

        ///// <summary>
        ///// Returns an object representing the NIF this SOP uses to communicate with a Vortex switch.
        ///// </summary>
        //IVortexNIF NIF { get; }

        /// <summary>
        /// Returns the path to a file containing configuration information for the SOP.
        /// </summary>
        string GetSOPConfigs();

        /// <summary>
        /// Assigns the specified flow to this SOP.
        /// </summary>
        /// <param name="f">The Flow object representing the flow assigned to the SOP.</param>
        void AddFlow(VortexCommon.Flow f);
        /// <summary>
        /// Removes the specified flow from this SOP.
        /// </summary>
        /// <param name="f">The Flow object representing the flow removed from the SOP.</param>
        void RemoveFlow(VortexCommon.Flow f);
        /// <summary>
        /// Removes the specified flow from this SOP.
        /// </summary>
        /// <param name="FlowID">The ID of the flow to be removed from the SOP.</param>
        void RemoveFlow(int FlowID);

        /// <summary>
        /// Gets a list of all flows currently assigned to the SOP.
        /// </summary>
        /// <returns>A List of all Flow objects representing flows assigned to the SOP</returns>
        List<VortexCommon.Flow> GetFlows();
    }
}
