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
 * IVortexSAP.cs
 * Name: Matthew Cotter
 * Date:  7 Oct 2010 
 * Description: Interface to implement a Vortex SAP object
 * History: 
 * >> ( 7 Oct 2010) Matthew Cotter: Created basic interface defining a Vortex SAP object.
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
    /// conforms to the Vortex Switch-Attached-Proessor hardware interface
    /// </summary>
    public interface IVortexSAP : IVortexAttachment
    {
        /// <summary>
        /// Boolean indicating whether the SAP is configurable (accepting configuration information)
        /// </summary>
        bool IsConfigurable { get; set; }

        ///// <summary>
        ///// Returns an object representing the NIF this SAP uses to communicate with a Vortex switch.
        ///// </summary>
        //IVortexNIF NIF { get; }

        /// <summary>
        /// Sets the path to a file specifying a Falcon Language command sequence to be utilized by the SAP.
        /// </summary>
        /// <param name="FilePath">The local file path containing the command sequence.</param>
        void SetCommandSource(string FilePath);
        
        /// <summary>
        /// Returns the path to a file specifying a Falcon Language command sequence to be utilized by the SAP.
        /// </summary>
        string GetCommandSource();

        /// <summary>
        /// Returns the path to a file containing configuration information for the SAP.
        /// </summary>
        string GetSAPConfigs();
    }
}
