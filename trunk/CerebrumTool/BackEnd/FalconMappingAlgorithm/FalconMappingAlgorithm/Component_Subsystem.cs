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
 * Component_Subsystem.cs
 * Name: Matthew Cotter
 * Date: 29 Dec 2010 
 * Description: Defines a class implementing a subsystem of components that are connected, but are not connected to any components in any other subsystem.
 * History: 
 * >> (29 Dec 2010) Matthew Cotter: Basic implementation of functions and properties for creating and managing a component subsystem
 * >> (29 Dec 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FalconResources;

namespace FalconMappingAlgorithm
{
    /// <summary>
    /// Class representing a subsystem of components
    /// </summary>
    class Component_Subsystem
    {
        private List<FalconMapping_Component> _Members;

        /// <summary>
        /// Default constructor.  Initializes the list of component members as empty.
        /// </summary>
        public Component_Subsystem()
        {
            this._Members = new List<FalconMapping_Component>();
        }

        /// <summary>
        /// Adds the specified component to this subsystem, if it is not already a member.
        /// </summary>
        /// <param name="newComponent">The component to be added to the subsystem</param>
        public void Add(FalconMapping_Component newComponent)
        {
            this._Members.Add(newComponent);
        }

        /// <summary>
        /// Removes the specified component from this subsystem, if it is a member.
        /// </summary>
        /// <param name="oldComponent">The component to be removed from the subsystem</param>
        public void Remove(FalconMapping_Component oldComponent)
        {
            this._Members.Remove(oldComponent);
        }

        /// <summary>
        /// Retrieves the list of components that are members of this subsystem.
        /// </summary>
        public List<FalconMapping_Component> Members
        {
            get
            {
                return this._Members;
            }
        }

        /// <summary>
        /// Merges all members of the specified subsystem into this subsystem with duplicate entries removed.
        /// </summary>
        /// <param name="otherSub">The subsystem whose members are to be merged with this one.</param>
        public void MergeWith(Component_Subsystem otherSub)
        {
            foreach (FalconMapping_Component comp in otherSub.Members)
            {
                if (!this.Members.Contains(comp))
                {
                    this.Members.Add(comp);
                }
            }
            otherSub.Members.Clear();
        }

        /// <summary>
        /// Determines if this subsystem shares a member with another subsystem.
        /// </summary>
        /// <param name="otherSub">The subsystem whose members are to be tested for membership in this subsystem.</param>
        /// <returns>True if otherSub shares at least one member with this subsystem.</returns>
        public bool SharesMemberWith(Component_Subsystem otherSub)
        {
            foreach (FalconMapping_Component comp in otherSub.Members)
            {
                if (this.Members.Contains(comp))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a ResourceInfo detailing the set of resources required by the component members of this subsystem.
        /// </summary>
        public ResourceInfo RequiredResources
        {
            get
            {
                ResourceInfo RI = new ResourceInfo();
                foreach (FalconMapping_Component comp in this.Members)
                {
                    RI.Add(comp.Resources);
                }
                return RI;
            }
        }
    }
}
