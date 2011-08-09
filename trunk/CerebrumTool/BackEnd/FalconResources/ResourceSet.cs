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
 * ResourceSet.cs
 * Name: Matthew Cotter
 * Date: 7 Oct 2010 
 * Description: Class for managing tracking and managing available/used/total resources on an FPGA.
 * History: 
 * >> (29 Dec 2010) Matthew Cotter: Added methods to get Total, Available, and Used ResourceInfo objects representing the corresponding resources within the set.
 * >> (20 Dec 2010) Matthew Cotter: Corrected bug that occurs when attempting to allocate or deallocate an empty set of resources.
 * >> ( 7 Oct 2010) Matthew Cotter: Imported code from FalconMappingAlgorithm Library.
 * >> ( 7 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CerebrumSharedClasses;

namespace FalconResources
{
    /// <summary>
    /// Object representing a pool of resources, to which other resources may be allocated.
    /// This object details the workings of total, available, and used resources within a system.
    /// </summary>
    public class ResourceSet
    {
        private ResourceInfo riTotal;
        private ResourceInfo riUsed;
        private ResourceInfo riAvailable;

        /// <summary>
        /// Creates a new ResourceSet object with no associated resources--total, used or available.
        /// </summary>
        public ResourceSet()
        {
            riTotal = new ResourceInfo();
            riUsed = new ResourceInfo();
            riAvailable = new ResourceInfo();
        }

        /// <summary>
        /// Sets the total amount of a given resource.
        /// If changing this value would cause the amount used to exceed the total for the given resource,
        /// a ResourceAllocationException will be thrown. 
        /// </summary>
        /// <param name="ResourceName">The name of the resource.</param>
        /// <param name="Amount">The total value to set for the given resource.</param>
        public bool SetTotalResource(string ResourceName, long Amount)
        {
            ResourceName = ResourceName.ToLower();
            if ((riUsed.GetResource(ResourceName)) > Amount)
                return false;

            riTotal.SetResource(ResourceName, Amount);
            riAvailable.SetResource(ResourceName, riTotal.GetResource(ResourceName) - riUsed.GetResource(ResourceName));
            return true;
        }
        
        /// <summary>
        /// Increases the amount used of a specific resource by a given amount.
        /// If changing this value would cause the amount used to exceed the total for the given resource,
        /// a ResourceAllocationException will be thrown. 
        /// </summary>
        /// <param name="ResourceName">The name of the resource</param>
        /// <param name="Amount">The amount of the resource to allocate</param>
        public bool AllocateResource(string ResourceName, long Amount)
        {
            ResourceName = ResourceName.ToLower();
            if ((riUsed.GetResource(ResourceName) + Amount) > riTotal.GetResource(ResourceName))
                return false;

            riUsed.SetResource(ResourceName, riUsed.GetResource(ResourceName) + Amount);
            riAvailable.SetResource(ResourceName, riTotal.GetResource(ResourceName) - riUsed.GetResource(ResourceName));

            //string AllocMessage = String.Format("+++ Allocated {0} '{1}'\n\t\tTotal {2}\n\t\tUsed: {3}\n\t\tRemaining: {4}",
            //        Amount, ResourceName, riTotal.GetResource(ResourceName), riUsed.GetResource(ResourceName), riAvailable.GetResource(ResourceName));
            //System.Diagnostics.Debug.WriteLine(AllocMessage);
            return true;
        }

        /// <summary>
        /// Decreases the amount used of a specific resource by a given amount.
        /// If changing this value would cause the amount used to fall below 0, a ResourceAllocationException will be thrown.
        /// </summary>
        /// <param name="ResourceName">The name of the resource</param>
        /// <param name="Amount">The amount of the resource to deallocate</param>
        public bool DeallocateResource(string ResourceName, long Amount)
        {
            ResourceName = ResourceName.ToLower();
            long newAmount = (riUsed.GetResource(ResourceName) - Amount);
            if (newAmount < 0)
                return false;

            riUsed.SetResource(ResourceName, riUsed.GetResource(ResourceName) - Amount);
            riAvailable.SetResource(ResourceName, riTotal.GetResource(ResourceName) - riUsed.GetResource(ResourceName));

            //string DeAllocMessage = String.Format("--- Deallocated {0} '{1}'\n\t\tTotal {2}\n\t\tUsed: {3}\n\t\tRemaining: {4}",
            //        Amount, ResourceName, riTotal.GetResource(ResourceName), riUsed.GetResource(ResourceName), riAvailable.GetResource(ResourceName));
            //System.Diagnostics.Debug.WriteLine(DeAllocMessage);
            return true;
        }

        /// <summary>
        /// Increases the set of used resources by a corresponding set of values.
        /// If changing any of these values causes an amount used to exceed the corresponding
        /// total for that resource, and the ResourceSet will revert to its state prior to the call
        /// and an exception will be thrown.
        /// </summary>
        /// <param name="ResInfo">The set of resources to be allocated.</param>
        public bool AllocateResourceInfo(ResourceInfo ResInfo)
        {
            ResourceInfo rsCurrent = new ResourceInfo();
            rsCurrent.Add(riUsed);

            Dictionary<string, long> htRequired = ResInfo.GetResources();
            if (htRequired.Count == 0)
                return true;

            foreach (string resource in htRequired.Keys)
            {
                try
                {
                    this.AllocateResource(resource, ResInfo.GetResource(resource));
                }
                catch (Exception ex)
                {
                    // Rollback / Restore the previous state
                    riUsed = rsCurrent;
                    throw ex;
                }
            }
            return true;
        }

        /// <summary>
        /// Increases the set of used resources by a corresponding set of values.
        /// If changing any of these values causes an amount used to fall below 0,
        /// the ResourceSet will revert to its state prior to the call and
        /// an exception will be thrown.
        /// </summary>
        /// <param name="ResInfo">The set of resources to be deallocated.</param>
        public bool DeallocateResourceInfo(ResourceInfo ResInfo)
        {
            ResourceInfo rsCurrent = new ResourceInfo();
            rsCurrent.Add(riUsed);

            Dictionary<string, long> htRequired = ResInfo.GetResources();
            if (htRequired.Count == 0)
                return true;
            foreach (string resource in htRequired.Keys)
            {
                try
                {
                    this.DeallocateResource(resource, ResInfo.GetResource(resource));
                }
                catch (Exception ex)
                {
                    // Rollback / Restore the previous state
                    riUsed = rsCurrent;
                    throw ex;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Gets the total amount of resources of a given type.
        /// </summary>
        /// <param name="ResourceName">The name of the resource</param>
        /// <returns>Returns the used amount associated with that resource, if it exists in the set.
        /// Zero, otherwise.</returns>
        public long GetTotalResource(string ResourceName)
        {
            ResourceName = ResourceName.ToLower();
            return riTotal.GetResource(ResourceName);
        }
        
        /// <summary>
        /// Gets the used amount of resources of a given type.
        /// </summary>
        /// <param name="ResourceName">The name of the resource</param>
        /// <returns>Returns the used amount associated with that resource, if it exists in the set.
        /// Zero, otherwise.</returns>
        public long GetUsedResource(string ResourceName)
        {
            ResourceName = ResourceName.ToLower();
            return riUsed.GetResource(ResourceName);
        }
        
        /// <summary>
        /// Gets the available amount of resources of a given type.
        /// </summary>
        /// <param name="ResourceName">The name of the resource</param>
        /// <returns>Returns the available amount associated with that resource, if it exists in the set.
        /// Zero, otherwise.</returns>
        public long GetAvailableResource(string ResourceName)
        {
            ResourceName = ResourceName.ToLower();
            return riAvailable.GetResource(ResourceName);
        }

        /// <summary>
        /// Gets a copy of the total resource set Hashtable, with the resource names as keys, and their corresponding amounts as values.
        /// </summary>
        /// <returns>A Hashtable that is a copy of the hashtable used to store the total resource information.</returns>
        public Dictionary<string, long> GetTotalResources()
        {
            return riTotal.GetResources();
        }

        /// <summary>
        /// Gets a copy of the used resource set Hashtable, with the resource names as keys, and their corresponding amounts as values.
        /// </summary>
        /// <returns>A Hashtable that is a copy of the hashtable used to store the used resource information.</returns>
        public Dictionary<string, long> GetUsedResources()
        {
            return riUsed.GetResources();
        }

        /// <summary>
        /// Gets a copy of the available resource set Hashtable, with the resource names as keys, and their corresponding amounts as values.
        /// </summary>
        /// <returns>A Hashtable that is a copy of the hashtable used to store the available resource information.</returns>
        public Dictionary<string, long> GetAvailableResources()
        {
            return riAvailable.GetResources();
        }

        /// <summary>
        /// Gets a copy of the total resource set information
        /// </summary>
        /// <returns>The ResourceInfo object used to store the total resource information.</returns>
        public ResourceInfo GetTotalResourceInfo()
        {
            return riTotal;
        }

        /// <summary>
        /// Gets a copy of the used resource set information
        /// </summary>
        /// <returns>The ResourceInfo object used to store the used resource information.</returns>
        public ResourceInfo GetUsedResourceInfo()
        {
            return riUsed;
        }

        /// <summary>
        /// Gets a copy of the available resource set information
        /// </summary>
        /// <returns>The ResourceInfo object used to store the available resource information.</returns>
        public ResourceInfo GetAvailableResourceInfo()
        {
            return riAvailable;
        }

        /// <summary>
        /// Generate a multi-line string, listing all total resources stored, along with the corresponding amount; one resource and amount per line.
        /// </summary>
        /// <returns>Returns a string, listing all total resource/amount pairs stored, each on its own line.</returns>
        public string ListTotalResources()
        {
            return riTotal.ListResources();
        }

        /// <summary>
        /// Generate a multi-line string, listing all used resources stored, along with the corresponding amount; one resource and amount per line.
        /// </summary>
        /// <returns>Returns a string, listing all used resource/amount pairs stored, each on its own line.</returns>
        public string ListUsedResources()
        {
            return riUsed.ListResources();
        }

        /// <summary>
        /// Generate a multi-line string, listing all available resources stored, along with the corresponding amount; one resource and amount per line.
        /// </summary>
        /// <returns>Returns a string, listing all available resource/amount pairs stored, each on its own line.</returns>
        public string ListAvailableResources()
        {
            return riAvailable.ListResources();
        }
    }
}
