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
 * ResourceInfo.cs
 * Name: Matthew Cotter
 * Date: 7 Oct 2010 
 * Description: Class for managing tracking and managing resources.
 * History: 
 * >> ( 7 Apr 2011) Matthew Cotter: Implemented functions to determine "largest" and "smallest" resources within a set.
 * >> (29 Dec 2010) Matthew Cotter: Implemented function to test whether one resource set is "larger" than another.
 * >> (18 Oct 2010) Matthew Cotter: Implemented support for adding resources.
 * >> ( 7 Oct 2010) Matthew Cotter: Imported code from FalconMappingAlgorithm Library.
 * >> ( 7 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CerebrumSharedClasses;

namespace FalconResources
{

    /// <summary>
    /// Represents a single collection of resources.
    /// </summary>
    public class ResourceInfo
    {
        private Dictionary<string, long> myResources;

        /// <summary>
        /// Create an empty ResourceInfo object, with no contained resources.
        /// </summary>
        public ResourceInfo()
        {
            myResources = new Dictionary<string, long>();
        }

        /// <summary>
        /// Sets the amount of a particular resource.
        /// </summary>
        /// <param name="ResourceName">The name of the resource.</param>
        /// <param name="Amount">The amount of the resource to set.</param>
        public void SetResource(string ResourceName, long Amount)
        {
            ResourceName = ResourceName.ToLower();
            if (myResources.ContainsKey(ResourceName))
            {
                myResources[ResourceName] = Amount;
            }
            else
            {
                myResources.Add(ResourceName, Amount);
            }
        }

        /// <summary>
        /// Gets the amount of a particular resource stored.
        /// </summary>
        /// <param name="ResourceName">The name of the resource.</param>
        /// <returns>The amount of the resource stored, if it exists in the set.  Otherwise, zero.</returns>
        public long GetResource(string ResourceName)
        {
            ResourceName = ResourceName.ToLower();
            if (myResources.ContainsKey(ResourceName))
            {
                long amount = (long)(myResources[ResourceName]);
                if (amount >= 0)
                    return amount;
                else
                    return 0;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the Hashtable resource set, with the resource names as keys, and their corresponding amounts as values.
        /// </summary>
        /// <returns>The Hashtable used to store the resource information.</returns>
        public Dictionary<string, long> GetResources()
        {
            return myResources;
        }

        /// <summary>
        /// Generate a multi-line string, listing all resources stored, along with the corresponding amount; one resource and amount per line.
        /// </summary>
        /// <returns>Returns a string, listing all resource/amount pairs stored, each on its own line.</returns>
        public string ListResources()
        {
            StringBuilder sb = new StringBuilder();

            List<string> resources = new List<string>();
            resources.AddRange(myResources.Keys);

            foreach (string res in resources)
            {
                sb.AppendLine(res + ": " + ((long)(myResources[res])).ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Add the values of one ResourceInfo object to the corresponding values stored in this one.
        /// </summary>
        /// <param name="ResInfo">The ResourceInfo object whose values to add.</param>
        public void Add(ResourceInfo ResInfo)
        {
            this.Add(ResInfo.GetResources());
        }

        /// <summary>
        /// Add the values of one ResourceInfo object to the corresponding values stored in this one.
        /// </summary>
        /// <param name="ResInfo">A Dictionary of resource pairs to add.</param>
        public void Add(Dictionary<string, long> ResInfo)
        {
            foreach (string resource in ResInfo.Keys)
                this.SetResource(resource, this.GetResource(resource) + ResInfo[resource]);
        }
        /// <summary>
        /// Merges the contents of ResInfo with those already contained in this object into a new set of resources
        /// </summary>
        /// <param name="ResInfo">The object with which to merge resources</param>
        /// <returns>A new ResourceInfo object containing the sum of the resources contained in this plus ResInfo.</returns>
        public ResourceInfo Merge(ResourceInfo ResInfo)
        {
            ResourceInfo RI = new ResourceInfo();
            RI.Add(this);
            RI.Add(ResInfo);
            return RI;
        }

        /// <summary>
        /// Subtract the values of one ResourceInfo object from the values stored in this one.  If the removal would result in a 
        /// negative value, a 0 is stored instead.
        /// </summary>
        /// <param name="ResInfo">The ResourceInfo object whose values to remove</param>
        public void Remove(ResourceInfo ResInfo)
        {
            Dictionary<string, long> htRemoved = ResInfo.GetResources();
            foreach (string resource in htRemoved.Keys)
            {
                long newAmount = this.GetResource(resource) - ResInfo.GetResource(resource);
                if (newAmount < 0)
                    this.SetResource(resource, 0);
                else
                    this.SetResource(resource, newAmount);
            }
        }
        /// <summary>
        /// Function used to determine whether the resources defined by one resource set can support those required by another
        /// </summary>
        /// <param name="ResInfo">The resources to test against the current</param>
        /// <returns>True if these resources define enough to support the test resources</returns>
        public bool CanSupport(ResourceInfo ResInfo)
        {
            Dictionary<string, long> htRes = ResInfo.GetResources();
            foreach (string resource in htRes.Keys)
            {
                if (this.GetResource(resource) < ResInfo.GetResource(resource))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the name and amount of the resource with the highest amount within the resource set.
        /// </summary>
        /// <param name="ResourceName">(ref) The name of the highest resource in the set.</param>
        /// <param name="ResourceAmount">(ref) The amount of the highest resource in the set.</param>
        /// <returns>True if a highest resource was obtained.</returns>
        public bool GetHighestResource(ref string ResourceName, ref long ResourceAmount)
        {
            bool bSuccess = false;
            long HighestValue = long.MinValue;
            foreach (KeyValuePair<string, long> Pair in myResources)
            {
                if (Pair.Value > HighestValue)
                {
                    ResourceName = Pair.Key;
                    ResourceAmount = Pair.Value;
                    bSuccess = true;
                }
            }
            return bSuccess;
        }

        /// <summary>
        /// Gets the name and amount of the resource with the lowest amount within the resource set.
        /// </summary>
        /// <param name="ResourceName">(ref) The name of the lowest resource in the set.</param>
        /// <param name="ResourceAmount">(ref) The amount of the lowest resource in the set.</param>
        /// <returns>True if a lowest resource was obtained.</returns>
        public bool GetLowestResource(ref string ResourceName, ref long ResourceAmount)
        {
            bool bSuccess = false;
            long HighestValue = long.MaxValue;
            foreach (KeyValuePair<string, long> Pair in myResources)
            {
                if (Pair.Value < HighestValue)
                {
                    ResourceName = Pair.Key;
                    ResourceAmount = Pair.Value;
                    bSuccess = true;
                }
            }
            return bSuccess;
        }
    }
}
