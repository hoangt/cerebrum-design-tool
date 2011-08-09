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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VortexInterfaces;
using CerebrumSharedClasses;
using CerebrumNetronObjects;

namespace FalconMappingAlgorithm
{
    /// <summary>
    /// Class used for constructing a data flow from components and connections
    /// </summary>
    public class DataFlow
    {
        /// <summary>
        /// Public constructor.  Initializes empty linked lists
        /// </summary>
        public DataFlow()
        {
            this._SubComponents = new LinkedList<ComponentCore>();
            this._VortexInterfaces = new LinkedList<IVortexAttachment>();
        }

        private LinkedList<ComponentCore> _SubComponents;
        private LinkedList<IVortexAttachment> _VortexInterfaces;

        /// <summary>
        /// Get the ComponentCore that initiates the flow
        /// </summary>
        public ComponentCore StartComponent
        {
            get
            {
                return this._SubComponents.First.Value;
            }
        }
        /// <summary>
        /// Get the ComponentCore currently at the tail end of the flow
        /// </summary>
        public ComponentCore EndComponent
        {
            get
            {
                return this._SubComponents.Last.Value;
            }
        }
        /// <summary>
        /// Get the VortexAttachment that initiates the flow
        /// </summary>
        public IVortexAttachment StartDevice
        {
            get
            {
                return this._VortexInterfaces.First.Value;
            }
        }
        /// <summary>
        /// Get the VortexAttachment currently at the tail end of the flow
        /// </summary>
        public IVortexAttachment EndDevice
        {
            get
            {
                return this._VortexInterfaces.Last.Value;
            }
        }
        /// <summary>
        /// Adds the specified ComponentCore and its corresponding VortexAttachment to the tail of the flow
        /// </summary>
        /// <param name="tailComponent">The ComponentCore to be added to the tail of the flow</param>
        /// <param name="tailAttach">The VortexAttachment to be added to the tail of the flow</param>
        public void Add(ComponentCore tailComponent, IVortexAttachment tailAttach)
        {
            this._SubComponents.AddLast(tailComponent);
            this._VortexInterfaces.AddLast(tailAttach);
        }
    }
}
