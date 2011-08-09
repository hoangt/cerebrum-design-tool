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
 * FPGA_Subsystem.cs
 * Name: Matthew Cotter
 * Date: 29 Dec 2010 
 * Description: Defines a class implementing a subsystem of FPGAs that are connected, but are not connected to any FPGAs in any other subsystem.
 * History: 
 * >> (29 Dec 2010) Matthew Cotter: Basic implementation of functions and properties for creating and managing an FPGA subsystem
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
    /// Class representing a subsystem of FPGAs
    /// </summary>
    class FPGA_Subsystem
    {
        private List<FalconMapping_FPGA> _Members;
        private List<Component_Subsystem> _AllocatedSubsystems;

        /// <summary>
        /// Default constructor.  Initializes the list of FPGA members and allocated component subsystems as empty
        /// </summary>
        public FPGA_Subsystem()
        {
            this._Members = new List<FalconMapping_FPGA>();
            this._AllocatedSubsystems = new List<Component_Subsystem>();
        }

        /// <summary>
        /// Adds the specified FPGA to this subsystem, if it is not already a member.
        /// </summary>
        /// <param name="newFPGA">The FPGA to be added to the subsystem</param>
        public void Add(FalconMapping_FPGA newFPGA)
        {
            this._Members.Add(newFPGA);
        }

        /// <summary>
        /// Removes the specified FPGA from this subsystem, if it is a member.
        /// </summary>
        /// <param name="oldFPGA">The FPGA to be removed from the subsystem</param>
        public void Remove(FalconMapping_FPGA oldFPGA)
        {
            this._Members.Remove(oldFPGA);
        }

        /// <summary>
        /// Retrieves the list of FPGAs that are members of this subsystem.
        /// </summary>
        public List<FalconMapping_FPGA> Members
        {
            get
            {
                return this._Members;
            }
        }

        /// <summary>
        /// Gets a ResourceInfo detailing the set of unallocated resources available on FPGAs in this subsystem.
        /// </summary>
        public ResourceInfo AvailableResources
        {
            get
            {
                ResourceInfo RI = new ResourceInfo();
                foreach (FalconMapping_FPGA fpga in this.Members)
                {
                    RI.Add(fpga.GetTotalResourceInfo());
                }
                foreach (Component_Subsystem comp_sub in this._AllocatedSubsystems)
                {
                    RI.Remove(comp_sub.RequiredResources);
                }
                return RI;
            }
        }

        /// <summary>
        /// Determines whether the specified component subsystem could possibly be supported by the resources available in this subsystem if the specified subsystem is not 
        /// already allocated to this FPGA subsystem.   If the subsystem is already allocated, this function returns True.
        /// </summary>
        /// <param name="sub">The component subsystem to test for support in this FPGA subsystem.</param>
        /// <returns>True if the specified subsystem is already allocated on this subsystem or there are sufficient available resources to support it.  False, if not.</returns>
        public bool CanSupport(Component_Subsystem sub)
        {
            if (this.IsComponentSystemAllocated(sub))
                return true;
            ResourceInfo RI = sub.RequiredResources;
            return this.AvailableResources.CanSupport(RI);
        }

        /// <summary>
        /// Merges all members of the specified subsystem into this subsystem with duplicate entries removed.
        /// </summary>
        /// <param name="otherSub">The subsystem whose members are to be merged with this one.</param>
        public void MergeWith(FPGA_Subsystem otherSub)
        {
            foreach (FalconMapping_FPGA fpga in otherSub.Members)
            {
                if (!this.Members.Contains(fpga))
                {
                    this.Members.Add(fpga);
                }
            }
            otherSub.Members.Clear();
        }

        /// <summary>
        /// Determines if this subsystem shares a member with another subsystem.
        /// </summary>
        /// <param name="otherSub">The subsystem whose members are to be tested for membership in this subsystem.</param>
        /// <returns>True if otherSub shares at least one member with this subsystem.</returns>
        public bool SharesMemberWith(FPGA_Subsystem otherSub)
        {
            foreach (FalconMapping_FPGA fpga in otherSub.Members)
            {
                if (this.Members.Contains(fpga))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds the specified component subsystem to the set of subsystems allocated to this one.
        /// </summary>
        /// <param name="comp_sys">The component subsystem to be allocated.</param>
        public void AllocateComponentSystem(Component_Subsystem comp_sys)
        {
            if (!this._AllocatedSubsystems.Contains(comp_sys))
                this._AllocatedSubsystems.Add(comp_sys);
        }
        /// <summary>
        /// Removes the specified component subsystem from the set of subsystems allocated to this one.
        /// </summary>
        /// <param name="comp_sys">The component subsystem to be removed.</param>
        public void DeAllocateComponentSystem(Component_Subsystem comp_sys)
        {
            if (this._AllocatedSubsystems.Contains(comp_sys))
                this._AllocatedSubsystems.Remove(comp_sys);
        }
        /// <summary>
        /// Tests whether the specified component subsystem is allocated to this one.
        /// </summary>
        /// <param name="comp_sys">The component subsystem to be tested.</param>
        public bool IsComponentSystemAllocated(Component_Subsystem comp_sys)
        {
            return (this._AllocatedSubsystems.Contains(comp_sys));
        }
    }
}
