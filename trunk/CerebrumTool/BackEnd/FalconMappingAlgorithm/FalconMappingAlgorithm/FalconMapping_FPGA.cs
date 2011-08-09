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
 * FalconMapping_FPGA.cs
 * Name: Matthew Cotter
 * Date: 2 Apr 2010 
 * Description: This class implements the methods and properties required to represent
 * a physical FPGA for use during the Component-to-FPGA mapping algorithm.
 * History: 
 * >> (14 Jun 2010) Matthew Cotter: Modified code to float Vortex configuration ports when a configuration attachment is not available.
 * >> (26 May 2010) Matthew Cotter: Corrected project parameter related bug that was causing clock generators to have difficulty in identifying a 100MHz input clock.
 * >> (17 May 2010) Matthew Cotter: Implemented support for capping the number of signals driven by any one clock signal.  While supported, this feature may not be used.
 * >> (16 May 2010) Matthew Cotter: Updated generation of mapping output file.
 * >> ( 9 May 2010) Matthew Cotter: Implemented support for configuring edge/bridge components.
 *                                      Refactored method to generate system clock signal.
 * >> ( 8 Apr 2010) Matthew Cotter: Additional work on integration of Vortex infrastructure resource management for mapping.
 * >> ( 7 Apr 2011) Matthew Cotter: Added calculations to account for variable infrastructure resource cost when adding and removing components.
 * >> (23 Mar 2011) Matthew Cotter: Added initial support for Vortex-attached devices that are capable of configuring the vortex.
 * >> (23 Feb 2011) Matthew Cotter: Corrected bug that caused a core-generated clock used to feed ONLY the clock generator to not be properly connected.
 * >> (18 Feb 2011) Matthew Cotter: Implemented support for variable frequency Vortex via parameter specified in project file.
 * >> (17 Feb 2011) Matthew Cotter: Hopefully have finally nailed down issue in clock assignments to required component cores.
 * >> (16 Feb 2011) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Due to redesign of clock signals, additional bugs were introduced and corrected.
 *                                      Modified code to use improved properties system for loading, saving, and managing core configurations.
 * >> ( 7 Feb 2011) Matthew Cotter: Corrected bug in automatic assignment and generation of clock signals for Vortex components.
 * >> (25 Jan 2011) Matthew Cotter: Added GetVortexRouters() in support of mapping/TDA report generation.
 * >> (18 Jan 2011) Matthew Cotter: Added support for GUI-added platform components overriding those instantiated by the platform.
 * >> (22 Dec 2010) Matthew Cotter: Added additional support for customizable clock management.
 * >> (16 Dec 2010) Matthew Cotter: Added and implemented multi-SAP support to load, identifiy and correctly attach all Vortex interfaces exposed by component.
 * >> ( 1 Dec 2010) Matthew Cotter: Integration of Multiple-SAP Components into mapping.
 *                                  Initial support for Required Cores on FPGA
 * >> (18 Oct 2010) Matthew Cotter: Continued integration of Vortex-based SAP/SOP communication infrastructure
 *                                  Added vortex switch to FPGA
 * >> (11 Oct 2010) Matthew Cotter: Added handling of subcomponent PCores for integration into Vortex-based SAP/SOP communication infrastructure
 *                                  Added management of system clocks for PCores.
 * >> ( 7 Oct 2010) Matthew Cotter: Changes required due to migration of ResourceInfo & ResourceSet structures to separate library.
 *                                  Converted generic Hashtables to TypedHashtables<> for readability and verifiability.
 * >> ( 8 Jul 2010) Matthew Cotter: Initialize Architecture property (also required in the constructor) defining what architecure family the FPGA is a member of.
 * >> ( 7 Jul 2010) Matthew Cotter: Initialize Address property as an empty string, as an invalid (or not required/specified) value.
 * >> ( 6 Jul 2010) Matthew Cotter: Added Address property to be used for external communication with the FPGA platform.
 *                                  Added Board property to be used hierarchical structuring of FPGA platforms.
 * >> (14 Jun 2010) Matthew Cotter: Corrected a bug that resulted in a failed score if an FPGA did not possess any of a resource that a group required 0 of.
 * >> (13 May 2010) Matthew Cotter: Updated FalconFPGAComparer Class to support weighting of I/O and Resources in Scoring
 * >>                               Updated scoring of resources and I/O to be more accurate/sensitive
 * >> (12 May 2010) Matthew Cotter: Moved FalconFPGAComparer Class to this file from FalconMapping_Algorithm.cs
 * >>                               Moved static function GetAverageFPGAResources() to FalconMapping_FPGA Class from FalconMapping_Algorithm
 * >>                               Moved static function PrintFPGAList() to FalconMapping_FPGA Class from FalconMapping_Algorithm
 * >> ( 6 May 2010) Matthew Cotter: Added functionality to store/calculate Distance from I/O
 *                                  Added property to return the cluster ID, if any, the FPGA is a member of
 *                                  Added ability to Un-Map a component group from the FPGA
 * >> (26 Apr 2010) Matthew Cotter: Added Debug Print Functions to print resources to System.Diagnostics.Debug Output
 * >> (25 Apr 2010) Matthew Cotter: Updated FPGA/Resource Scoring, Implemented MapGroup method
 * >> (17 Apr 2010) Matthew Cotter: Implemented Functions to access Total/Available/Used Resource Sets
 * >> ( 2 Apr 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CerebrumSharedClasses;
using CerebrumNetronObjects;
using FalconResources;
using FalconClockManager;
using VortexInterfaces;
using VortexObjects;
using System.Xml;
using FalconPathManager;

namespace FalconMappingAlgorithm
{


    /// <summary>
    /// IComparer class to compare two FalconMapping_FPGA objects for sorting order.
    /// The comparison is made by weighting scores from total resources compared against a baseline, 
    /// as well as physical link-distance from Input.
    /// </summary>
    public class FalconFPGAComparer : IComparer<FalconMapping_FPGA>
    {
        private Dictionary<string, long> htComparisonSet;

        private double dResourceWeight = 0.5F;
        private double dIOWeight = 0.5F;


        /// <summary>
        /// Constructor to initialize the FalconFPGAComparer object.
        /// </summary>
        public FalconFPGAComparer()
        {
            htComparisonSet = null;
        }

        /// <summary>
        /// Get or set the Hashtable set of resources for use as a comparison baseline.
        /// </summary>
        public Dictionary<string, long> ComparisonSet
        {
            get
            {
                return htComparisonSet;
            }
            set
            {
                htComparisonSet = value;
            }
        }


        /// <summary>
        /// Get or set the weighting applied to I/O connectivity in scoring for comparison.
        /// </summary>
        public double IOWeight
        {
            get
            {
                return dIOWeight;
            }
            set
            {
                if (value < 0)
                    value = 0;
                dIOWeight = value;
            }
        }

        /// <summary>
        /// Get or set the weighting applied to resource requirements in scoring for comparison.
        /// </summary>
        public double ResourceWeight
        {
            get
            {
                return dResourceWeight;
            }
            set
            {
                if (value < 0)
                    value = 0;
                dResourceWeight = value;
            }
        }


        /// <summary>
        /// Implementation of IComparer.Compare() method to compare 2 FalconMapping_FPGA objects.  If either
        /// object is not such an object, the function always returns 0.  Scoring ties are broken using
        /// String.CompareOrdinal() using the IDs of the two FPGAs.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>Returns a negative value if 'x has a lower score than y', positive if 'x has a better score than y', 
        /// and 0 if either x or y is not a FalconMapping_FPGA object.</returns>
        int IComparer<FalconMapping_FPGA>.Compare(FalconMapping_FPGA x, FalconMapping_FPGA y)
        {
            if ((x.GetType() == typeof(FalconMapping_FPGA)) &&
                (y.GetType() == typeof(FalconMapping_FPGA)))
            {
                FalconMapping_FPGA fpgaX = x;
                FalconMapping_FPGA fpgaY = y;
                if (x == y)
                    return 0;

                double scoreX = WeightedScore(fpgaX);
                double scoreY = WeightedScore(fpgaY);

                // x is less than y, if x is "smaller/farther" than y
                // x is greater than y, if x is "larger/closer" than y
                if (scoreX == scoreY)
                {
                    // Break tie by Group ID
                    return (int)(String.CompareOrdinal(fpgaX.ID, fpgaY.ID));
                }
                else
                {
                    // x is "smaller" than y, if it fits the set better, meaning it's score is lower
                    return (int)((scoreX - scoreY) * 1000); // Rounds to 3 decimal digits
                }
            }
            return 0;
        }

        /// <summary>
        /// Calculates the weighted score of the specified FalconMapping_FPGA.
        /// </summary>
        /// <param name="fpgaX">The FalconMapping_FPGA object to score.</param>
        /// <returns>Returns a score, with IO and Resource requirements weighted as specified in IOWeight and ResourceWeight.</returns>
        public double WeightedScore(FalconMapping_FPGA fpgaX)
        {
            double scoreX;
            double resScoreX;
            double IOScoreX;
            if (htComparisonSet == null)
                resScoreX = 0;
            else 
                resScoreX = FalconMapping_ResourceScoring.ScoreResourceSet(htComparisonSet, fpgaX.GetTotalResources());
            if (resScoreX < 0) resScoreX = 0;
            IOScoreX = fpgaX.DistanceFromInput + 1;
            scoreX = (dResourceWeight * resScoreX) + (dIOWeight * IOScoreX);
            return scoreX;
        }
    }



    /// <summary>
    /// Represents a physical FPGA within the system platform.
    /// </summary>
    public class FalconMapping_FPGA
    {
        private string strID;
        private string strName;
        private ResourceSet rsResources;
        private List<FalconMapping_Group> _MappedGroups;
        private bool bIsClustered;
        private string sClusterID;
        private int iDistanceFromInput;

        /// <summary>
        /// Creates a new FalconMapping_FPGA object representing an FPGA in the platform.
        /// </summary>
        /// <param name="sID">The ID of the new FPGA</param>
        /// <param name="sName">The Name of the new FPGA</param>
        /// <param name="sFamily">The architecture family of the FPGA (i.e. virtex4, spartan5, etc)</param>
        public FalconMapping_FPGA(string sID, string sName, string sFamily)
        {
            strID = sID;
            strName = sName;
            bIsClustered = false;
            rsResources = new ResourceSet();
            _MappedGroups = new List<FalconMapping_Group>();
            iDistanceFromInput = -1;
            IPAddress = string.Empty;
            MACAddress = string.Empty;
            this.Architecture = sFamily;
        }
        
        #region Properties

        /// <summary>
        /// Get or set the ID of this FPGA.
        /// </summary>
        public string ID
        {
            get
            {
                return strID;
            }
            set
            {
                string oldID = strID;
                string newID = value;
                foreach (object o in _MappedGroups)
                {
                    FalconMapping_Group mappedGroup = (FalconMapping_Group)o;
                    if (mappedGroup.IsMapped)
                        if (mappedGroup.TargetFPGA == oldID)
                            mappedGroup.TargetFPGA = newID;
                }
                strID = newID;
            }
        }

        /// <summary>
        /// Get or set the name of this FPGA.
        /// </summary>
        public string Name
        {
            get
            {
                return strName;
            }
            set
            {
                strName = value;
            }
        }

        /// <summary>
        /// Function to forcibly clear the associated group information from a component. 
        /// Note: This function is only intended to be used as cleanup in the event that this FPGA cannot be properly unclustered with
        /// a call to its associated cluster's RemoveFPGA() function.  Calling this in another situation could create an unpredictable state.
        /// </summary>
        public void ClearCluster()
        {
            bIsClustered = false;
            sClusterID = String.Empty;
        }

        /// <summary>
        /// Get or set a boolean flag indicating whether this FPGA has clustered.
        /// Note: This property should only be written by the AddFPGA() call of the FalconMapping_Cluster object this
        /// FPGA is being added to.  Modifying this property in another situation could create an unpredictable state..
        /// </summary>
        public bool IsClustered
        {
            get
            {
                return bIsClustered;
            }
            set
            {
                bIsClustered = value;
            }
        }

        /// <summary>
        /// Get or set the FPGA's copy of the ID of the cluster to which it is assigned.  
        /// If it's not grouped, this should return string.Empty.
        /// Note: This property should only be written by the AddFPGA() call of the FalconMapping_Cluster object this
        /// FPGA is being added to.  Modifying this property in another situation could create an unpredictable state.
        /// </summary>
        public string ClusterID
        {
            get
            {
                return sClusterID;
            }
            set
            {
                sClusterID = value;
            }
        }

        /// <summary>
        /// Gets the pre-calculated minimum number of physical link between this component and nearest input FPGA.
        /// If this value is 0, it indicates that this FPGA is the an source/FPGA.
        /// If this value is less than 0, it indicates that distance calculation has not, been or could not be done for this FPGA.
        /// </summary>
        public int DistanceFromInput
        {
            get
            {
                return iDistanceFromInput;
            }
            set
            {
                iDistanceFromInput = value;
            }
        }

        /// <summary>
        /// Get or set the IP Address of this FPGA for communication purposes.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Get or set the MAC Address of this FPGA for communication purposes.
        /// </summary>
        public string MACAddress { get; set; }

        /// <summary>
        /// Get or set the ID of the board to which this FPGA belongs.
        /// </summary>
        public string BoardID { get; set; }

        /// <summary>
        /// Get or set the architecture family of this FPGA (i.e. virtex4, virtex5, virtex6, etc)
        /// </summary>
        public string Architecture { get; set; }

        /// <summary>
        /// Reference to the project PathManager object loaded for mapping the system.
        /// </summary>
        public PathManager PathManager { get; set; }
        #endregion

        #region Resources
        /// <summary>
        /// Sets the total amount of a particular resource on the FPGA.
        /// </summary>
        /// <param name="ResourceName">Name of the resource.</param>
        /// <param name="ResourceAmount">Value indicating how much of the specified resource is available.</param>
        public void SetTotalResource(string ResourceName, long ResourceAmount)
        {
            if (!rsResources.SetTotalResource(ResourceName, ResourceAmount))
            {
                throw new Exceptions.ResourceAllocationException(
                    String.Format("Unable to set total resource {0} to {1}.  Used resource total ({2}) currently exceeds that value.",
                    ResourceName, ResourceAmount, rsResources.GetUsedResource(ResourceName)));
            }
        }

        /// <summary>
        /// Gets the total amount of a particular resource on the FPGA.
        /// </summary>
        /// <param name="ResourceName">Name of the resource</param>
        /// <returns>The total amount of the specified resource on the FPGA.</returns>
        public long GetTotalResource(string ResourceName)
        {
            return rsResources.GetTotalResource(ResourceName);
        }        
        /// <summary>
        /// Gets a collection of the total amount of all resources on the FPGA.
        /// </summary>
        /// <returns>A Hashtable collection of the total amount of all resources on the FPGA.</returns>
        public Dictionary<string, long> GetTotalResources()
        {
            return rsResources.GetTotalResources();
        }
        /// <summary>
        /// Gets a collection of the total amount of all resources on the FPGA.
        /// </summary>
        /// <returns>A ResourceInfo of the total amount of all resources on the FPGA.</returns>
        public ResourceInfo GetTotalResourceInfo()
        {
            return rsResources.GetTotalResourceInfo();
        }

        /// <summary>
        /// Gets the available amount of a particular resource on the FPGA.
        /// </summary>
        /// <param name="ResourceName">Name of the resource</param>
        /// <returns>The available amount of the specified resource on the FPGA.</returns>
        public long GetAvailableResource(string ResourceName)
        {
            return rsResources.GetAvailableResource(ResourceName);
        }
        /// <summary>
        /// Gets a collection of the total available amount of all resources on the FPGA.
        /// </summary>
        /// <returns>A Hashtable collection of the total available amount of all resources on the FPGA.</returns>
        public Dictionary<string, long> GetAvailableResources()
        {
            return rsResources.GetAvailableResources();
        }
        /// <summary>
        /// Gets a collection of the total available amount of all resources on the FPGA.
        /// </summary>
        /// <returns>A ResourceInfo of the total available amount of all resources on the FPGA.</returns>
        public ResourceInfo GetAvailableResourceInfo()
        {
            return rsResources.GetAvailableResourceInfo();
        }

        /// <summary>
        /// Gets the used amount of a particular resource on the FPGA.
        /// </summary>
        /// <param name="ResourceName">Name of the resource</param>
        /// <returns>The used amount of the specified resource on the FPGA.</returns>
        public long GetUsedResource(string ResourceName)
        {
            return rsResources.GetUsedResource(ResourceName);
        }        
        /// <summary>
        /// Gets a collection of the total used amount of all resources on the FPGA.
        /// </summary>
        /// <returns>A Hashtable collection of the total used amount of all resources on the FPGA.</returns>
        public Dictionary<string, long> GetUsedResources()
        {
            return rsResources.GetUsedResources();
        }
        /// <summary>
        /// Gets a collection of the total used amount of all resources on the FPGA.
        /// </summary>
        /// <returns>A ResourceInfo of the total used amount of all resources on the FPGA.</returns>
        public ResourceInfo GetUsedResourceInfo()
        {
            return rsResources.GetUsedResourceInfo();
        }

        /// <summary>
        /// Allocates a set of resources on the FPGA for a given group, if there are sufficient 
        /// resources available.  If the group has already been allocated to the FPGA, a GroupAlreadyMappedException is thrown.
        /// If there are not enough resources available, an InsufficientResourcesException is thrown.
        /// </summary>
        /// <param name="MapGrp">The FalconMapping_Group that is being allocated to this FPGA</param>
        private void AllocateResources(FalconMapping_Group MapGrp)
        {
            if (_MappedGroups.Contains(MapGrp))
                throw new Exceptions.GroupAlreadyMappedException(
                    String.Format("Unable to allocate group {0} resources to FPGA {1}.  Group {0} is already mapped to FPGA {2}.",
                    MapGrp.ID, this.ID, MapGrp.TargetFPGA));

            if (this.HasSufficientResources(MapGrp))
            {
                ResourceInfo riToAllocate = new ResourceInfo();
                riToAllocate.Add(MapGrp.RequiredResources);

                int nCurrentPorts = 0;
                int nRequiredPorts = 0;

                // Determine what is already in place
                foreach (FalconMapping_Group g in this.GetMappedGroups())
                {
                    foreach (FalconMapping_Component c in g.GetGroupedComponents())
                    {
                        foreach (IVortexAttachment IVA in c.VortexDevices)
                        {
                            nRequiredPorts++;
                        }
                    }
                }
                // Add what is new
                nRequiredPorts = nCurrentPorts;
                foreach (FalconMapping_Component comp in MapGrp.GetGroupedComponents())
                {
                    foreach (IVortexAttachment IVA in comp.VortexDevices)
                    {
                        nRequiredPorts++;
                    }
                }
                riToAllocate.Add(InfrastructureCostToSupport(MapGrp));

                //System.Diagnostics.Debug.WriteLine(String.Format("Allocating group '{0}' to '{1}'", MapGrp.ID, this.ID));
                if (!rsResources.AllocateResourceInfo(riToAllocate))
                {
                    StringBuilder ResourceMessage = new StringBuilder();
                    ResourceInfo Available = this.GetAvailableResourceInfo();
                    ResourceInfo Required = MapGrp.RequiredResources;
                    ResourceInfo Infra = this.InfrastructureCostToSupport(MapGrp);
                    Infra.Add(Required);
                    ResourceMessage.AppendFormat("{0} : {1} / {2}\n", "Resource", "Needed", "Available");
                    foreach (string ResKey in Infra.GetResources().Keys)
                    {
                        ResourceMessage.AppendFormat("'{0}' : {1} / {2}\n", ResKey, Infra.GetResource(ResKey), Available.GetResource(ResKey));
                    }
                    throw new Exceptions.InsufficientResourcesException(
                        String.Format("Unable to map group {1} to FPGA {0} due to insufficient physical resources.\n{2}",
                        this.ID, MapGrp.ID, ResourceMessage.ToString()));
                }
                _MappedGroups.Add(MapGrp);
            }
            else
            {
                StringBuilder ResourceMessage = new StringBuilder();
                ResourceInfo Available = this.GetAvailableResourceInfo();
                ResourceInfo Required = MapGrp.RequiredResources;
                ResourceInfo Infra = this.InfrastructureCostToSupport(MapGrp);
                Infra.Add(Required);
                ResourceMessage.AppendFormat("{0} : {1} / {2}\n", "Resource", "Needed", "Available");
                foreach (string ResKey in Infra.GetResources().Keys)
                {
                    ResourceMessage.AppendFormat("'{0}' : {1} / {2}\n", ResKey, Infra.GetResource(ResKey), Available.GetResource(ResKey));
                }
                throw new Exceptions.InsufficientResourcesException(
                    String.Format("Unable to map group {1} to FPGA {0} due to insufficient physical resources.\n{2}",
                    this.ID, MapGrp.ID, ResourceMessage.ToString()));
            }
        }

        /// <summary>
        /// Deallocates a set of resources from the FPGA for a given group, if that group's resources are currently allocated
        /// to this FPGA.
        /// </summary>
        /// <param name="MapGrp">The FalconMapping_Group that is being deallocated from this FPGA</param>
        private void DeallocateResources(FalconMapping_Group MapGrp)
        {
            if (_MappedGroups.Contains(MapGrp))
            {
                ResourceInfo riToRemove = new ResourceInfo();
                riToRemove.Add(MapGrp.RequiredResources);

                int nCurrentPorts = 0;
                int nRequiredPorts = 0;

                // Determine what is already in place
                foreach (FalconMapping_Group g in this.GetMappedGroups())
                {
                    foreach (FalconMapping_Component c in g.GetGroupedComponents())
                    {
                        foreach (IVortexAttachment IVA in c.VortexDevices)
                        {
                            nCurrentPorts++;
                        }
                    }
                }
                // Add what is new
                nRequiredPorts = nCurrentPorts;
                foreach (FalconMapping_Component comp in MapGrp.GetGroupedComponents())
                {
                    foreach (IVortexAttachment IVA in comp.VortexDevices)
                    {
                        nRequiredPorts--;
                    }
                }
                ResourceInfo ifCurrent = CalculateCurrentInfrastructureCost(nCurrentPorts);
                ResourceInfo ifRequired = CalculateInfrastructureCost(nRequiredPorts);
                ifCurrent.Remove(ifRequired);
                riToRemove.Add(ifCurrent);

                //System.Diagnostics.Debug.WriteLine(String.Format("Deallocating group '{0}' from '{1}'", MapGrp.ID, this.ID));
                if (!rsResources.DeallocateResourceInfo(riToRemove))
                {
                    throw new Exceptions.ResourceAllocationException(
                        String.Format("Unable to deallocate {1} resources from FPGA {0}.  Deallocation would result in a negative allocation.",
                        MapGrp.ID, this.ID));
                }
                _MappedGroups.Remove(MapGrp);
            }
        }

        /// <summary>
        /// Print out all FPGA resources (Total/Used/Available) to System.Diagnostics.Debug
        /// </summary>
        public void DebugPrintResources()
        {
            Dictionary<string, long> htTotal = rsResources.GetTotalResources();
            System.Diagnostics.Debug.WriteLine("--------------------------");
            System.Diagnostics.Debug.WriteLine("FPGA ID: " + this.ID);
            foreach (string resName in htTotal.Keys)
            {
                long total = rsResources.GetTotalResource(resName);
                long used = rsResources.GetUsedResource(resName);
                long avail = rsResources.GetAvailableResource(resName);

                System.Diagnostics.Debug.WriteLine(resName + " -- Total: " + total.ToString() + ", Used: " + used.ToString() + ", Available: " + avail.ToString());
            }
            System.Diagnostics.Debug.WriteLine("--------------------------");
        }


        /// <summary>
        /// Calculates the current infrastructure resource cost, based on the current number of Vortex ports in use.
        /// </summary>
        /// <param name="nCurrentPorts">The number of ports currently in use.</param>
        /// <returns>A ResourceInfo object containing the resources currently used for infrastructure.</returns>
        private ResourceInfo CalculateCurrentInfrastructureCost(int nCurrentPorts)
        {
            VortexRouter vr = new VortexRouter(0, 0);
            VortexBridge VB = new VortexBridge();
            VortexBridgeAttachment VBA = new VortexBridgeAttachment();
            ResourceInfo ifCurrent = new ResourceInfo();
            if (nCurrentPorts <= 0)
                return ifCurrent;

            int nCurrentRouters = _VortexRouters.Count;
            int nOverflowPorts = 0;

            // Calculate the current number of routers required
            nOverflowPorts = nCurrentPorts - (vr.NumPorts * (nCurrentRouters - 1));
            for (int i = 1; i <= nCurrentRouters; i++)
            {
                if (i < nCurrentRouters)
                {
                    ifCurrent.Add(VortexRouter.GetResources(vr.NumPorts));      // Resources required for each router except the last (full)
                    if (nCurrentRouters > 1)
                    {
                        ifCurrent.Add(VB.GetResources());      // Resources required a bridge for each router beyond the first
                        ifCurrent.Add(VBA.GetResources());     // Resources required for TWO bridge attachments for each router beyond the first
                        ifCurrent.Add(VBA.GetResources());     // Resources required for TWO bridge attachments for each router beyond the first
                    }
                }
                else
                {
                    ifCurrent.Add(VortexRouter.GetResources(nOverflowPorts));   // Resources required for the last router (excess ports)
                }
            }
            return ifCurrent;
        }
        /// <summary>
        /// Calculates the expected infrastructure resource cost, based on the required number of Vortex ports in use.
        /// </summary>
        /// <param name="nRequiredPorts">The number of ports required to be in use.</param>
        /// <returns>A ResourceInfo object containing the resources expected to be used for infrastructure.</returns>
        private ResourceInfo CalculateInfrastructureCost(int nRequiredPorts)
        {
            VortexRouter vr = new VortexRouter(0, 0);
            VortexBridge VB = new VortexBridge();
            VortexBridgeAttachment VBA = new VortexBridgeAttachment();
            ResourceInfo ifRequired = new ResourceInfo();
            if (nRequiredPorts <= 0)
                return ifRequired;

            int nRequiredRouters = 0;
            int nCurrentRouters = 0;
            int nAdditionalRouters = 0;
            int nOverflowPorts = 0;

            bool bConverged = false;
            while (!bConverged)
            {
                // Calculate the expected number of routers required
                nRequiredRouters = (int)Math.Ceiling((double)nRequiredPorts / (double)vr.NumPorts);

                nAdditionalRouters = nRequiredRouters - nCurrentRouters;
                if (nRequiredRouters > 1)
                {
                    nRequiredPorts += (2 * nAdditionalRouters);
                }
                if (nRequiredPorts <= (nRequiredRouters * vr.NumPorts))
                    bConverged = true;
            }
            nRequiredRouters = (int)Math.Ceiling((double)nRequiredPorts / (double)vr.NumPorts);
            nAdditionalRouters = nRequiredRouters - nCurrentRouters;
            nOverflowPorts = nRequiredPorts - (vr.NumPorts * (nRequiredRouters - 1));
            
            for (int i = 1; i <= nAdditionalRouters; i++)
            {
                if (i < nAdditionalRouters)
                {
                    ifRequired.Add(VortexRouter.GetResources(vr.NumPorts));      // Resources required for each new router (full)
                
                }
                else
                {
                    ifRequired.Add(VortexRouter.GetResources(nOverflowPorts));   // Resources required for each new router (excess ports)
                }

                if (nRequiredRouters > 1)
                {
                    ifRequired.Add(VB.GetResources());      // Resources required a bridge for each new router
                    ifRequired.Add(VBA.GetResources());     // Resources required for TWO bridge attachments for each new router
                    ifRequired.Add(VBA.GetResources());     // Resources required for TWO bridge attachments for each new router
                }
            }
            return ifRequired;
        }        
        /// <summary>
        /// Calculates the additional infrastructure resource cost required to support the specified group of components.
        /// </summary>
        /// <param name="group">The group of components to be supported.</param>
        /// <returns>A ResourceInfo object containing the additional resources required to implement infrastructure support for the group of components.</returns>
        public ResourceInfo InfrastructureCostToSupport(FalconMapping_Group group)
        {
            int nCurrentPorts = 0;
            int nRequiredPorts = 0;

            // Determine what is already in place
            foreach (FalconMapping_Group g in this.GetMappedGroups())
            {
                foreach (FalconMapping_Component c in g.GetGroupedComponents())
                {
                    foreach (IVortexAttachment IVA in c.VortexDevices)
                    {
                        nCurrentPorts++;
                    }
                }
            }
            // Add what is new
            nRequiredPorts = nCurrentPorts;
            foreach (FalconMapping_Component comp in group.GetGroupedComponents())
            {
                foreach (IVortexAttachment IVA in comp.VortexDevices)
                {
                    nRequiredPorts++;
                }
            }
            ResourceInfo ifRequired = CalculateInfrastructureCost(nRequiredPorts);
            ResourceInfo ifCurrent = CalculateCurrentInfrastructureCost(nCurrentPorts);
            ifRequired.Remove(ifCurrent);
            return ifRequired;
        }
        /// <summary>
        /// Calculates the additional infrastructure resource cost required to support the specified component.
        /// </summary>
        /// <param name="component">The component to be supported.</param>
        /// <returns>A ResourceInfo object containing the additional resources required to implement infrastructure support for the component.</returns>
        public ResourceInfo InfrastructureCostToSupport(FalconMapping_Component component)
        {
            int nCurrentPorts = 0;
            int nRequiredPorts = 0;

            // Determine what is already in place
            foreach (FalconMapping_Group g in this.GetMappedGroups())
            {
                foreach (FalconMapping_Component c in g.GetGroupedComponents())
                {
                    foreach (IVortexAttachment IVA in c.VortexDevices)
                    {
                        nRequiredPorts++;
                    }
                }
            }
            // Add what is new
            nRequiredPorts = nCurrentPorts;
            foreach (IVortexAttachment IVA in component.VortexDevices)
            {
                nRequiredPorts++;
            }
            ResourceInfo ifRequired = CalculateInfrastructureCost(nRequiredPorts);
            ResourceInfo ifCurrent = CalculateCurrentInfrastructureCost(nCurrentPorts);
            ifRequired.Remove(ifCurrent);
            return ifRequired;
        }
        
        /// <summary>
        /// Determines if this FPGA has enough resources to allow the specified group to be mapped.
        /// </summary>
        /// <param name="TestGroup">FalconMapping_Group object to be tested.</param>
        /// <returns>True, if this FPGA has sufficient resources for ALL resources required by TestGroup, as well as any additional infrastructure support needed.
        /// False, otherwise.</returns>
        public bool HasSufficientResources(FalconMapping_Group TestGroup)
        {
            ResourceInfo RequiredResources = new ResourceInfo();
            // Get the Resources for all Components in the group
            RequiredResources.Add(TestGroup.RequiredResources);

            // Add the resources required by infrastructure additions needed to support the group
            RequiredResources.Add(InfrastructureCostToSupport(TestGroup));

            // Test whether what is remaining can support the requirement
            return this.GetAvailableResourceInfo().CanSupport(RequiredResources);
        }
        /// <summary>
        /// Determines if this FPGA has enough resources to allow the specified component to be mapped.
        /// </summary>
        /// <param name="TestComponent">FalconMapping_Group object to be tested.</param>
        /// <returns>True, if this FPGA has sufficient resources for ALL resources required by TestComponent, as well as any additional infrastructure support needed.
        /// False, otherwise.</returns>
        public bool HasSufficientResources(FalconMapping_Component TestComponent)
        {
            ResourceInfo RequiredResources = new ResourceInfo();
            // Get the Resources for the Component
            RequiredResources.Add(TestComponent.Resources);

            // Add the resources required by infrastructure additions needed to support the Component
            RequiredResources.Add(InfrastructureCostToSupport(TestComponent));

            // Test whether what is remaining can support the requirement
            return this.GetAvailableResourceInfo().CanSupport(RequiredResources);
        }

        /// <summary>
        /// Calculates the average amount of available resources on all FPGAs.  This set is used 
        /// to score/sort the set of groups prior to mapping.
        /// </summary>
        /// <param name="FPGAs">A Hashtable collection of FalconMapping_FPGA objects in the system.</param>
        /// <returns>Returns the set of resources, averaged over the number of FPGAs.</returns>
        internal static Dictionary<string, long> GetAverageFPGAResources(Dictionary<string, FalconMapping_FPGA> FPGAs)
        {
            Dictionary<string, long> htAverage = new Dictionary<string, long>();
            Dictionary<string, long> htTemp = new Dictionary<string, long>();
            int iFPGACount = 0;

            foreach (string fID in FPGAs.Keys)
            {
                FalconMapping_FPGA fmf = (FalconMapping_FPGA)FPGAs[fID];
                Dictionary<string, long> htTotal = fmf.GetTotalResources();
                iFPGACount++;

                foreach (string resName in htTotal.Keys)
                {
                    if (htTemp.ContainsKey(resName))
                    {
                        htTemp[resName] = (long)htTemp[resName] + (long)htTotal[resName];
                    }
                    else
                    {
                        htTemp.Add(resName, (long)htTotal[resName]);
                    }
                }
            }
            foreach (string resName in htTemp.Keys)
            {
                htAverage.Add(resName, (long)htTemp[resName] / iFPGACount);
            }
            return htAverage;
        }

        #endregion

        #region Mapping
        /// <summary>
        /// Maps the specified group onto this FPGA Instance.
        /// If the group is already mapped to another FPGA, a GroupAlreadyMappedException is thrown.
        /// Otherwise, the group is processed and mapped to this FPGA, if it is not already.
        /// </summary>
        /// <param name="MapGrp">The FalconMapping_Group that is to be mapped to this FPGA.</param>
        public void MapGroup(FalconMapping_Group MapGrp)
        {
            if ((MapGrp.IsMapped) && (MapGrp.TargetFPGA != this.ID))
                throw new Exceptions.GroupAlreadyMappedException(
                    String.Format("Unable to map group to FPGA {0}.  Group {1} is already mapped to FPGA {2}.", 
                    this.ID, MapGrp.ID, MapGrp.TargetFPGA));
            else
            {
                if ((MapGrp.IsMapped) && (MapGrp.TargetFPGA == this.ID))
                    return;

                if (this.HasSufficientResources(MapGrp))
                {
                    this.AllocateResources(MapGrp);
                    this.AttachToVortex(MapGrp);
                    MapGrp.TargetFPGA = this.ID;
                    MapGrp.IsMapped = true;
                }
                else
                {
                    StringBuilder ResourceMessage = new StringBuilder();
                    ResourceInfo Available = this.GetAvailableResourceInfo();
                    ResourceInfo Required = MapGrp.RequiredResources;
                    ResourceInfo Infra = this.InfrastructureCostToSupport(MapGrp);
                    Infra.Add(Required);
                    ResourceMessage.AppendFormat("{0} : {1} / {2}\n", "Resource", "Needed", "Available");
                    foreach (string ResKey in Infra.GetResources().Keys)
                    {
                        ResourceMessage.AppendFormat("'{0}' : {1} / {2}\n", ResKey, Infra.GetResource(ResKey), Available.GetResource(ResKey));
                    }
                    throw new Exceptions.InsufficientResourcesException(
                        String.Format("Unable to map group {1} to FPGA {0} due to insufficient physical resources.\n{2}",
                        this.ID, MapGrp.ID, ResourceMessage.ToString()));
                }
            }
        }

        /// <summary>
        /// Un-Maps the specified group from this FPGA.
        /// If the group is mapped, but not mapped to this FPGA, a GroupNotMappedException is thrown.
        /// </summary>
        /// <param name="MapGrp">The FalconMapping_Group that is to be removed from this FPGA.</param>
        public void UnMapGroup(FalconMapping_Group MapGrp)
        {
            if ((!MapGrp.IsMapped) || (MapGrp.IsMapped && (MapGrp.TargetFPGA != this.ID)))
                throw new Exceptions.GroupNotMappedException(
                    String.Format("Unable to unmap group.  Group {0} is not mapped to this FPGA.",
                    this.ID));
            else
            {
                if ((MapGrp.IsMapped) && (MapGrp.TargetFPGA == this.ID))
                {
                    this.DeallocateResources(MapGrp);
                    MapGrp.ClearMapping();

                    this.DetachFromVortex(MapGrp);
                }
            }
        }

        /// <summary>
        /// Get a copy of the set of FalconMapping_Groups that have been mapped to this FPGA, for enumeration.
        /// Note: While only a copy of the list is returned, the elements of that list are not copies.  Avoid 
        /// unintentionally modifying the elements of the List.
        /// </summary>
        /// <returns>Returns a copy of the List containing all of the FalconMapping_Groups that have been mapped to this FPGA.</returns>
        public List<FalconMapping_Group> GetMappedGroups()
        {
            List<FalconMapping_Group> l = new List<FalconMapping_Group>();
            l.AddRange(_MappedGroups);
            return l;
        }

        /// <summary>
        /// Get a copy of the set of FalconMapping_Components that have been mapped to this FPGA, for enumeration.
        /// Note: While only a copy of the list is returned, the elements of that list are not copies.  Avoid 
        /// unintentionally modifying the elements of the List.
        /// </summary>
        /// <returns>Returns a copy of the List containing all of the FalconMapping_Components that have been mapped to this FPGA.</returns>
        public List<FalconMapping_Component> GetMappedComponents()
        {
            List<FalconMapping_Component> l = new List<FalconMapping_Component>();
            foreach (FalconMapping_Group g in this.GetMappedGroups())
            {
                l.AddRange(g.GetGroupedComponents());
            }
            return l;
        }
        #endregion
        
        /// <summary>
        /// Print out all FPGAs and their scores to System.Diagnostics.Debug
        /// </summary>
        /// <param name="lFPGAs">The list of FalconMapping_FPGAs</param>
        /// <param name="FPGAComparer">The FPGA comparer object that calculates and compares the FPGA scoring</param>
        internal static void PrintFPGAList(List<FalconMapping_FPGA> lFPGAs, FalconFPGAComparer FPGAComparer)
        {
            System.Diagnostics.Debug.WriteLine("FPGA List");
            for (int iIndex = 0; iIndex < lFPGAs.Count; iIndex++)
            {
                FalconMapping_FPGA fmf = lFPGAs[iIndex];
                System.Diagnostics.Debug.WriteLine(
                    String.Format("    FPGA ID: {0} \tScore: {1}",
                        fmf.ID, FPGAComparer.WeightedScore(fmf).ToString("#000.000000")));
            }
        }
        
        #region System Clock Management
        private ClockGenerator _ClockGen;

        /// <summary>
        /// Get the ClockGenerator object available on this FPGA.
        /// </summary>
        public ClockGenerator ClockGenerator
        {
            get
            {
                return _ClockGen;
            }
        }
        /// <summary>
        /// Initializes the Clock Generator to be used for this FPGA.
        /// </summary>
        private void InitializeClockGenerator(PathManager PathMan)
        {
            _ClockGen = new ClockGenerator("clock_generator", "clock_generator", this.ID.Replace(".", "_"), "3.02.a", 16);
            // Clock generator assumes a 100MHz input clock
            _ClockGen.InputFrequency = 100000000;
            // Set the clock generator locked signal port
            _ClockGen.LockedSignal = "LOCKED";
            // Set the clock generator reset signal port
            _ClockGen.ResetSignal = "RST";

            // Initialize the Max # of Output signals driven by a signal ClockGen signal (Default = Int32.MaxValue)
            int MaxDriverPerClock = Int32.MaxValue;
            string strMaxDriverPerClock = MaxDriverPerClock.ToString();
            if (PathMan.HasPath("MaxDriversPerClockGenSignal"))
            {
                strMaxDriverPerClock = PathMan["MaxDriversPerClockGenSignal"];
            }
            int.TryParse(strMaxDriverPerClock, out MaxDriverPerClock);
            _ClockGen.MaxDrivePerClock = MaxDriverPerClock;
        }
        /// <summary>
        /// Finalizes the input signal source for the clock generator, if one was required on the FPGA
        /// </summary>
        public void FinalizeClockGenerator()
        {
            if (_ClockGen != null)
            {
                // Find a good input signal for the clock generator
                bool bFoundClock = false;
                ClockSignal CGInput = new ClockSignal();
                ClockSignal CGSource = null;
                CGInput.SignalDirection = ClockDirection.INPUT;
                foreach (FalconMapping_Group OutGrp in this.GetMappedGroups())
                {
                    foreach (FalconMapping_Component OutCmp in OutGrp.GetGroupedComponents())
                    {
                        foreach (ClockSignal OutCS in OutCmp.OutputClocks)
                        {
                            OutCS.GetFrequencyFromParameter = GetParameterMethod;
                            if (OutCS.IsCompatibleWith(CGInput))
                            {
                                CGSource = OutCS;
                                bFoundClock = true;
                                OutCmp.ComponentCores[OutCS.CoreInstance].Properties.SetValue(CerebrumPropertyTypes.PORT, OutCS.Port, OutCS.GenerateCoreSignal(), true);
                                break;
                            }
                        }
                        if (bFoundClock)
                            break;
                    }
                    if (bFoundClock)
                        break;
                }
                if (bFoundClock)
                {
                    _ClockGen.ClockInSignal = CGSource.GenerateCoreSignal();
                }
            }
        }


        private object GetParameterMethod(ParameterSourceTypes Source, string ComponentInstance, string CoreInstance, string ParameterName)
        {
            switch (Source)
            {
                case ParameterSourceTypes.PARAMETER_PROJECT:
                    // Get PARAMETER from project (PathManager)
                    return this.PathManager[ParameterName];
                case ParameterSourceTypes.PARAMETER_COMPONENT:
                    // Get PARAMETER from CerebrumCore
                    foreach (FalconMapping_Component Component in this.GetMappedComponents())
                    {
                        if (String.Compare(Component.ID, ComponentInstance, true) == 0)
                        {
                            return Component.InternalComponentObject.Properties.GetValue(CerebrumPropertyTypes.CEREBRUMPROPERTY, ParameterName);
                        }
                    }
                    break;
                case ParameterSourceTypes.PARAMETER_CORE:
                    // Get PARAMETER from ComponentCore within CerebrumCore
                    foreach (FalconMapping_Component Component in this.GetMappedComponents())
                    {
                        if (String.Compare(Component.ID, ComponentInstance, true) == 0)
                        {
                            foreach (ComponentCore CompCore in Component.InternalComponentObject.ComponentCores.Values)
                            {
                                if (String.Compare(CompCore.NativeInstance, CoreInstance, true) == 0)
                                {
                                    return CompCore.Properties.GetValue(CerebrumPropertyTypes.PARAMETER, ParameterName);
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return string.Empty;
        }

        /// <summary>
        /// Automatically generate and assign clocks and clock signals to input clocks that have not yet been assigned.
        /// </summary>
        public void AutoAssignClocks()
        {
            // Unable to locate a matching clock on the components.  Try the clock generator.
            if (_ClockGen == null)
                InitializeClockGenerator(this.PathManager);

            Dictionary<string, FalconMapping_Component> ComponentsOnFPGA = new Dictionary<string, FalconMapping_Component>();
            foreach (FalconMapping_Component Comp in this.GetMappedComponents())
            {
                if (!ComponentsOnFPGA.ContainsKey(Comp.ID))
                {
                    ComponentsOnFPGA.Add(Comp.ID, Comp);
                }
            }
            foreach (FalconMapping_Component ReqComp in this.RequiredComponents)
            {
                if (!ComponentsOnFPGA.ContainsKey(ReqComp.ID))
                {
                    ComponentsOnFPGA.Add(ReqComp.ID, ReqComp);
                }
            }
            foreach (FalconMapping_Component ComponentNeedingClock in ComponentsOnFPGA.Values)
            {
                foreach (ClockSignal NeedClock in ComponentNeedingClock.InputClocks)
                {
                    NeedClock.GetFrequencyFromParameter = GetParameterMethod;

                    bool bFoundClock = false;
                    if (NeedClock.SignalDirection != ClockDirection.INPUT)
                        continue;
                    //ClockInfoStruct ExistingInputInfo = (ClockInfoStruct)ComponentNeedingClock.ComponentCores[NeedClock.CoreInstance].Properties.GetValue(CerebrumPropertyTypes.INPUT_CLOCK, NeedClock.Port);
                    //if (!ComponentsOnFPGA.ContainsKey(ExistingInputInfo.SourceComponent))
                    //{
                    //    // If the Core generating this clock is not already on the FPGA, then its the clock generator (auto-generated)
                    //    // Clear the property, and if needed, the clock generator will reassign it.
                    //    ComponentNeedingClock.ComponentCores[NeedClock.CoreInstance].Properties.DeleteValue(CerebrumPropertyTypes.INPUT_CLOCK, NeedClock.Port);
                    //}

                    foreach (FalconMapping_Component ComponentHavingClock in ComponentsOnFPGA.Values)
                    {
                        foreach (ClockSignal OutputClock in ComponentHavingClock.OutputClocks)
                        {
                            OutputClock.GetFrequencyFromParameter = GetParameterMethod;
                            if (OutputClock.SignalDirection != ClockDirection.OUTPUT)
                                continue;

                            if (OutputClock.IsCompatibleWith(NeedClock))
                            {
                                NeedClock.ConnectToSource(OutputClock);
                                ComponentNeedingClock.ComponentCores[NeedClock.CoreInstance].Properties.SetValue(CerebrumPropertyTypes.INPUT_CLOCK, NeedClock.Port,
                                    new ClockInfoStruct(NeedClock.Port, OutputClock.ComponentInstance, OutputClock.CoreInstance, OutputClock.Port), true);
                                ComponentNeedingClock.ComponentCores[NeedClock.CoreInstance].Properties.SetValue(CerebrumPropertyTypes.PORT, NeedClock.Port, OutputClock.GenerateCoreSignal(), true);
                                ComponentHavingClock.ComponentCores[OutputClock.CoreInstance].Properties.SetValue(CerebrumPropertyTypes.PORT, OutputClock.Port, OutputClock.GenerateCoreSignal(), true);                                   

                                bFoundClock = true;

                                if ((NeedClock.LockedPort != string.Empty) && (OutputClock.LockedPort != string.Empty))
                                {
                                    string OutputLockedSignal = OutputClock.GenerateLockSignal();
                                    ComponentNeedingClock.ComponentCores[NeedClock.CoreInstance].Properties.SetValue(CerebrumPropertyTypes.PORT, NeedClock.LockedPort, OutputLockedSignal, true);
                                    ComponentHavingClock.ComponentCores[OutputClock.CoreInstance].Properties.SetValue(CerebrumPropertyTypes.PORT, OutputClock.LockedPort, OutputLockedSignal, true);
                                }
                                else
                                {
                                    ComponentNeedingClock.ComponentCores[NeedClock.CoreInstance].Properties.DeleteValue(CerebrumPropertyTypes.PORT, NeedClock.LockedPort);                                    
                                }
                                break;
                            }
                        }
                        if (bFoundClock)
                            break;
                    }
                    if (!bFoundClock)
                    {                        
                        ClockSignal MatchedClock;

                        if (!this.ClockGenerator.HasAvailableClock(NeedClock))
                        {
                            this.ClockGenerator.AddClock(NeedClock);
                        }
                        MatchedClock = this.ClockGenerator.GetAvailableClock(NeedClock);
                        this.ClockGenerator.AcquireClockSignal(MatchedClock);

                        NeedClock.ConnectToSource(MatchedClock);
                        ComponentNeedingClock.ComponentCores[NeedClock.CoreInstance].Properties.SetValue(CerebrumPropertyTypes.PORT, NeedClock.Port, MatchedClock.GenerateCoreSignal(), true);
                               
                        if (NeedClock.LockedPort != string.Empty)
                        {
                            string OutputLockedSignal = this.ClockGenerator.GenerateLockSignal();
                            ComponentNeedingClock.ComponentCores[NeedClock.CoreInstance].Properties.SetValue(CerebrumPropertyTypes.PORT, NeedClock.LockedPort, OutputLockedSignal, true);
                        }
                        else
                        {
                            ComponentNeedingClock.ComponentCores[NeedClock.CoreInstance].Properties.DeleteValue(CerebrumPropertyTypes.PORT, NeedClock.LockedPort);
                        }
                    }
                }
            }
        }

        ///// <summary>
        ///// Determine whether the clock generator on the FPGA can support the clocks required by the component group.
        ///// </summary>
        ///// <param name="Group">The component group to test for clock compatibility.</param>
        ///// <returns>True if the FPGA can support the required clocks, False otherwise.</returns>
        //public bool CanSupportClocks(FalconMapping_Group Group)
        //{
        //    return true;
        //    //ClockSet requiredClocks = new ClockSet();

        //    //foreach (FalconMapping_Component Comp in Group.GetGroupedComponents())
        //    //{
        //    //    foreach (ComponentCore SC in Comp.SubComponents.Values)
        //    //    {
        //    //        foreach (ClockSignal clk in SC.ClockSet.GetClocks().Values)
        //    //        {
        //    //            if (_ClockGen.HasMatchingClock(clk))
        //    //                continue;
        //    //            if (requiredClocks.HasMatchingClock(clk))
        //    //                continue;
        //    //            requiredClocks.AddClock(clk.SignalName, clk);
        //    //        }
        //    //    }
        //    //}
        //    //return (requiredClocks.GetClocks().Count <= _ClockGen.UnusedClocks);
        //}
        ///// <summary>
        ///// Determine whether the clock generator on the FPGA can support the clocks required by the component.
        ///// </summary>
        ///// <param name="Comp">The component to test for clock compatibility.</param>
        ///// <returns>True if the FPGA can support the required clocks, False otherwise.</returns>
        //public bool CanSupportClocks(FalconMapping_Component Comp)
        //{
        //    return true;
        //    //ClockSet requiredClocks = new ClockSet();

        //    //foreach (ComponentCore SC in Comp.SubComponents.Values)
        //    //{
        //    //    foreach (ClockSignal clk in SC.ClockSet.GetClocks().Values)
        //    //    {
        //    //        if (_ClockGen.HasMatchingClock(clk))
        //    //            continue;
        //    //        if (requiredClocks.HasMatchingClock(clk))
        //    //            continue;
        //    //        requiredClocks.AddClock(String.Format("{0}_{1}", SC.HardwareInstanceName, clk.SignalName), clk);
        //    //    }
        //    //}
        //    //return (requiredClocks.GetClocks().Count <= _ClockGen.UnusedClocks);
        //}

        ///// <summary>
        ///// Allocates the clocks required by the component group, if the FPGA can support them.
        ///// </summary>
        ///// <param name="Group">The component group to allocate clocks for.</param>
        ///// <returns>True if the FPGA can support the required clocks and they have been allocated, False otherwise.</returns>
        //public bool AllocateClocks(FalconMapping_Group Group)
        //{
        //    return true;
        //    //if (this.CanSupportClocks(Group))
        //    //{
        //    //    foreach (FalconMapping_Component Comp in Group.GetGroupedComponents())
        //    //    {
        //    //        foreach (ComponentCore SC in Comp.SubComponents.Values)
        //    //        {
        //    //            foreach (ClockSignal clk in SC.ClockSet.GetClocks().Values)
        //    //            {
        //    //                if (_ClockGen.HasMatchingClock(clk))
        //    //                    continue;
        //    //                _ClockGen.AddClock(clk);
        //    //            }
        //    //        }
        //    //    }
        //    //    return true;
        //    //}
        //    //return false;
        //}
        ///// <summary>
        ///// Allocates the clocks required by the component, if the FPGA can support them.
        ///// </summary>
        ///// <param name="Comp">The component to allocate clocks for.</param>
        ///// <returns>True if the FPGA can support the required clocks and they have been allocated, False otherwise.</returns>
        //public bool AllocateClocks(FalconMapping_Component Comp)
        //{
        //    return true;
        //    //if (this.CanSupportClocks(Comp))
        //    //{
        //    //    foreach (ComponentCore SC in Comp.SubComponents.Values)
        //    //    {
        //    //        foreach (ClockSignal clk in SC.ClockSet.GetClocks().Values)
        //    //        {
        //    //            if (_ClockGen.HasMatchingClock(clk))
        //    //                continue;
        //    //            _ClockGen.AddClock(clk);
        //    //        }
        //    //    }
        //    //    return true;
        //    //}
        //    //return false;
        //}

        #endregion

        #region Vortex Management
        private Dictionary<int, IVortex> _VortexRouters;
        private int VortexSwitchID { get; set; }

        /// <summary>
        /// Initializes properties required by Mapping algorithm to account for the Vortex communication architecture.  As Vortex routers are instantiated, they are assigned 
        /// IDs (Bus, Switch) starting with (BusID, 1) for the first and incrementing Switch by 1 for each subsequent routers.
        /// </summary>
        /// <param name="SwitchID">The Switch ID assigned to the FPGA (Currently assumed one-and-only-one) Switch ID per FPGA, with no two FPGAs sharing a Switch ID</param>
        public void InitializeVortex(int SwitchID)
        {
            _VortexRouters = new Dictionary<int, IVortex>();
            this.VortexSwitchID = SwitchID;
        }

        /// <summary>
        /// Gets a list of Vortex Routers as IVortex objects instantiated on this FPGA.
        /// </summary>
        /// <returns>A list of Vortex Routers as IVortex objects instantiated on this FPGA.</returns>
        public List<IVortex> GetVortexRouters()
        {
            List<IVortex> Routers = new List<IVortex>();
            foreach (IVortex IV in _VortexRouters.Values)
                Routers.Add(IV);
            return Routers;
        }
        
        /// <summary>
        /// Adds a new Vortex Router to the FPGA.
        /// </summary>
        /// <returns>The Vortex Router object that was added.</returns>
        private IVortex AddVortex()
        {
            int BusID = 0;
            while (_VortexRouters.ContainsKey(BusID))
                BusID++;
            VortexRouter VR = new VortexRouter(BusID, this.VortexSwitchID);
            VR.Instance = String.Format("vortex_b{0}_s{1}", VR.BusID.ToString(), VR.SwitchID.ToString());
            _VortexRouters.Add(BusID, VR);
            return VR;
        }

        /// <summary>
        /// Finds any Vortex with enough ports.
        /// </summary>
        /// <param name="RequiredPorts">The number of ports needed.</param>
        /// <returns>An IVortex object, if one was found, with sufficient ports.</returns>
        internal IVortex GetAvailableVortex(int RequiredPorts)
        {
            foreach (IVortex vr in _VortexRouters.Values)
            {
                if (vr.AvailablePorts >= RequiredPorts)
                    return vr;
            }
            return null;
        }
        /// <summary>
        /// Finds a Vortex with the fewest free ports
        /// </summary>
        internal IVortex GetFewestFreeVortex()
        {
            int MinPorts = 3000;
            IVortex FullVortex = null;
            foreach (IVortex vr in _VortexRouters.Values)
            {
                if (vr.AvailablePorts < MinPorts)
                {
                    FullVortex = vr;
                    MinPorts = vr.AvailablePorts;
                    if (MinPorts == 0)
                        break;
                }
            }
            return FullVortex;
        }
        internal void AttachToVortex(FalconMapping_Component comp)
        {
            if ((_VortexRouters == null) || (_VortexRouters.Count == 0))
            {
                AddVortex();
            }
            IVortex AvailVR = GetAvailableVortex(comp.VortexDevices.Count);
            IVortex FewestFreeVR = null;
            if (AvailVR == null)
            {
                FewestFreeVR = GetFewestFreeVortex();
                AvailVR = AddVortex();
            }
            if (FewestFreeVR != null)
            {
                if (FewestFreeVR.AvailablePorts == 0)
                {
                    // Find something we can pick off this router to make room for the bridge
                    // Pick off a component from the 'fewest free' vortex
                    // Also, need a way to "lock" a component to a Vortex (i.e. Edge Component)
                    // Enforce a MAXIMUM number of ports a component may have?? (i.e. 3?)
                    FalconMapping_Component PickedOffComponent = null;
                    foreach (FalconMapping_Component c in this.GetMappedComponents())
                    {
                        foreach (IVortexAttachment iva in c.VortexDevices)
                        {
                            if (iva.AttachedTo == FewestFreeVR)
                            {
                                PickedOffComponent = c;
                                break;
                            }
                        }
                    }
                    if (PickedOffComponent != null)
                    {
                        DetachFromVortex(PickedOffComponent);

                        VortexBridge newBridge = new VortexBridge();
                        newBridge.Bridge(AvailVR, FewestFreeVR);

                        AttachToVortex(PickedOffComponent, AvailVR);
                        if (FewestFreeVR.AvailablePorts >= comp.VortexDevices.Count)
                        {
                            AttachToVortex(comp, FewestFreeVR);
                        }
                        else
                        {
                            AttachToVortex(comp, AvailVR);
                        }
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                AttachToVortex(comp, AvailVR);
            }
        }
        internal void AttachToVortex(FalconMapping_Component comp, IVortex router)
        {
            if (comp.VortexDevices.Count > router.AvailablePorts)
            {
            }
            else 
            {
                foreach (IVortexAttachment iva in comp.VortexDevices)
                {
                    router.AttachDevice(iva);
                }
            }
        }
        internal void AttachToVortex(FalconMapping_Group group)
        {
            foreach (FalconMapping_Component c in group.GetGroupedComponents())
            {
                AttachToVortex(c);
            }
        }
        internal void DetachFromVortex(FalconMapping_Component comp)
        {
            foreach (IVortexAttachment VortexDevice in comp.VortexDevices)
            {
                if (VortexDevice.Attached)
                {
                    VortexDevice.AttachedTo.DetachDevice(VortexDevice);
                }
            }
        }
        internal void DetachFromVortex(FalconMapping_Group group)
        {
            foreach (FalconMapping_Component c in group.GetGroupedComponents())
            {
                DetachFromVortex(c);
            }
        }

        internal void WriteVortexToXPSMap(XmlDocument XDoc, XmlElement xRoot)
        {
            foreach (IVortex VR in _VortexRouters.Values)
            {
                XmlElement XERouter = XDoc.CreateElement("PCore");
                XERouter.SetAttribute("Type", VR.Type);
                XERouter.SetAttribute("Location", String.Format("{0}_v{1}", VR.Type, VR.Version.Replace(".", "_")));
                XERouter.SetAttribute("Instance", VR.Instance);
                XERouter.SetAttribute("Version", VR.Version);
                XERouter.SetAttribute("Native", VR.Instance);
                xRoot.AppendChild(XERouter);
                foreach (IVortexAttachment VA in VR.AttachedDevices)
                {
                    if (VA.NIF != null)
                    {
                        XmlElement XENIF = XDoc.CreateElement("PCore");
                        XENIF.SetAttribute("Type", VA.NIF.Type);
                        XENIF.SetAttribute("Location", String.Format("{0}_v{1}", VA.NIF.Type, VA.NIF.Version.Replace(".", "_")));
                        XENIF.SetAttribute("Instance", VA.NIF.Instance);
                        XENIF.SetAttribute("Version", VA.NIF.Version);
                        XENIF.SetAttribute("Native", VA.NIF.Instance);
                        xRoot.AppendChild(XENIF);
                    }
                    else if (VA is IVortexBridgeAttachment)
                    {
                        VortexBridgeAttachment VBA = VA as VortexBridgeAttachment;
                        VortexBridge VB = VBA.Bridge as VortexBridge;
                        if (VB != null)
                        {
                            if (VBA.TDA == VB.TDA_1)
                            {
                                // If Attachment is Bridge Attachment #1, Write the Bridge
                                XmlElement XEBridge = XDoc.CreateElement("PCore");
                                XEBridge.SetAttribute("Type", VB.Type);
                                XEBridge.SetAttribute("Location", String.Format("{0}_v{1}", VB.Type, VB.Version.Replace(".", "_")));
                                XEBridge.SetAttribute("Instance", VB.Instance);
                                XEBridge.SetAttribute("Version", VB.Version);
                                XEBridge.SetAttribute("Native", VB.Instance);
                                xRoot.AppendChild(XEBridge);
                            }

                            // Write the Bridge Attachment
                            XmlElement XEBridgeAttachment = XDoc.CreateElement("PCore");
                            XEBridgeAttachment.SetAttribute("Type", VBA.Type);
                            XEBridgeAttachment.SetAttribute("Location", String.Format("{0}_v{1}", VBA.Type, VBA.Version.Replace(".", "_")));
                            XEBridgeAttachment.SetAttribute("Instance", VBA.Instance);
                            XEBridgeAttachment.SetAttribute("Version", VBA.Version);
                            XEBridgeAttachment.SetAttribute("Native", VBA.Instance);
                            xRoot.AppendChild(XEBridgeAttachment);
                        }
                    }
                    else if (VA is IVortexEdgeAttachment)
                    {
                        VortexEdgeAttachment VEA = VA as VortexEdgeAttachment;
                        // Write the Edge Attachment
                        XmlElement XEEdgeAttachment = XDoc.CreateElement("PCore");
                        XEEdgeAttachment.SetAttribute("Type", VEA.Type);
                        XEEdgeAttachment.SetAttribute("Location", String.Format("{0}_v{1}", VEA.Type, VEA.Version.Replace(".", "_")));
                        XEEdgeAttachment.SetAttribute("Instance", VEA.Instance);
                        XEEdgeAttachment.SetAttribute("Version", VEA.Version);
                        XEEdgeAttachment.SetAttribute("Native", VEA.Instance);
                        xRoot.AppendChild(XEEdgeAttachment);
                    }
                }
            }
        }
        /// <summary>
        /// Writes Vortex Configurations to the CoreConfig files for the project
        /// </summary>
        /// <param name="PathMan">The Project Path manager to be used for saving the core configurations.</param>
        internal void WriteVortexConfigurations(PathManager PathMan)
        {
            foreach (IVortex VR in _VortexRouters.Values)
            {
                WriteVortexConfig(PathMan, VR);
            }
        }
        /// <summary>
        /// Write Vortex Attachment and NIF configurations for the CoreConfig files for the project.
        /// </summary>
        /// <param name="PathMan">The Project Path manager to be used for saving the core configurations.</param>
        /// <param name="VA">The Vortex Attachment whose configurations are to be written.</param>
        private void WriteVortexNIFEdgeCoreConfig(PathManager PathMan, IVortexAttachment VA)
        {
            if (VA is IVortexSAP)
            {
                WriteVortexSAPCoreConfigs(PathMan, VA);
            }
            else if (VA is IVortexSOP)
            {
                WriteVortexSOPCoreConfigs(PathMan, VA);
            }
            else if (VA is IVortexEdgeAttachment)
            {
                WriteVortexEdgeCoreConfigs(PathMan, VA);
            }
        }
        /// <summary>
        /// Write Vortex SAP and NIF configurations for the CoreConfig files for the project.
        /// </summary>
        /// <param name="PathMan">The Project Path manager to be used for saving the core configurations.</param>
        /// <param name="VA">The Vortex SAP whose configurations are to be written.</param>
        private void WriteVortexSAPCoreConfigs(PathManager PathMan, IVortexAttachment VA)
        {
            string ProjectDir = PathMan["LocalProjectRoot"];
            string ChannelSignal = String.Format("{0}_{1}_CHANNEL", VA.Instance, VA.NIF.Type);
            string ManageSignal = String.Format("{0}_{1}_CHANNEL_MANAGE", VA.Instance, VA.NIF.Type);
            string MasterCmdSignal = String.Format("{0}_{1}_MASTER_CMD_IF", VA.Instance, VA.NIF.Type);
            string MasterDataSignal = String.Format("{0}_{1}_MASTER_DATA_IF", VA.Instance, VA.NIF.Type);
            string SlaveSignal = String.Format("{0}_{1}_SLAVE_IF", VA.Instance, VA.NIF.Type);

            string ClockSignal = String.Format("{0}_{1}_clk", VA.Instance, VA.NIF.Type);
            string ResetSignal = String.Format("net_gnd", VA.Instance, VA.NIF.Type);


            #region Save the NIF

            CerebrumPropertyCollection CPC = new CerebrumPropertyCollection(VA.NIF.Instance, VA.NIF.Instance, VA.NIF.Type);
            CPC.SetValue(CerebrumPropertyTypes.PARAMETER, "C_CLK_SEL", 0, true);
            CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "MASTER_CMD_IF", MasterCmdSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "MASTER_DATA_IF", MasterDataSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "SLAVE_IF", SlaveSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "CHANNEL", ChannelSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "CHANNEL_MANAGE", ManageSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.PORT, "sap_clk", ClockSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.PORT, "sap_rst", ResetSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.PORT, "rst", "net_gnd", true);
            CPC.SavePropertyCollection(ProjectDir);

            //XmlDocument xNIFDoc = new XmlDocument();
            //xNIFDoc.AppendChild(xNIFDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            //XmlElement XERoot = xNIFDoc.CreateElement("CoreConfig");

            //XmlElement xNIFCfg = xNIFDoc.CreateElement("MPD");
            //xNIFCfg.SetAttribute("Instance", VA.NIF.Instance);
            //xNIFCfg.SetAttribute("Core", VA.NIF.Type);

            //// Write the NIF Configs
            //XmlElement xVortexClockSel = xNIFDoc.CreateElement("PARAMETER");
            //xVortexClockSel.SetAttribute("Name", String.Format("C_CLK_SEL"));
            //xVortexClockSel.SetAttribute("Value", "0");
            //xNIFCfg.AppendChild(xVortexClockSel);

            //XmlElement xVortexChannel = xNIFDoc.CreateElement("BUS_INTERFACE");
            //xVortexChannel.SetAttribute("Name", String.Format("CHANNEL"));
            //xVortexChannel.SetAttribute("Value", ChannelSignal);
            //xNIFCfg.AppendChild(xVortexChannel);

            //XmlElement xVortexChannelManage = xNIFDoc.CreateElement("BUS_INTERFACE");
            //xVortexChannelManage.SetAttribute("Name", String.Format("CHANNEL_MANAGE"));
            //xVortexChannelManage.SetAttribute("Value", ManageSignal);
            //xNIFCfg.AppendChild(xVortexChannelManage);

            //XmlElement xVortexMasterCmd = xNIFDoc.CreateElement("BUS_INTERFACE");
            //xVortexMasterCmd.SetAttribute("Name", String.Format("MASTER_CMD_IF"));
            //xVortexMasterCmd.SetAttribute("Value", MasterCmdSignal);
            //xNIFCfg.AppendChild(xVortexMasterCmd);

            //XmlElement xVortexMasterData = xNIFDoc.CreateElement("BUS_INTERFACE");
            //xVortexMasterData.SetAttribute("Name", String.Format("MASTER_DATA_IF"));
            //xVortexMasterData.SetAttribute("Value", MasterDataSignal);
            //xNIFCfg.AppendChild(xVortexMasterData);

            //XmlElement xVortexSlave = xNIFDoc.CreateElement("BUS_INTERFACE");
            //xVortexSlave.SetAttribute("Name", String.Format("SLAVE_IF"));
            //xVortexSlave.SetAttribute("Value", SlaveSignal);
            //xNIFCfg.AppendChild(xVortexSlave);

            //XmlElement xVortexClock = xNIFDoc.CreateElement("PORT");
            //xVortexClock.SetAttribute("Name", String.Format("sap_clk"));
            //xVortexClock.SetAttribute("Value", ClockSignal);
            //xNIFCfg.AppendChild(xVortexClock);

            //XmlElement xVortexReset = xNIFDoc.CreateElement("PORT");
            //xVortexReset.SetAttribute("Name", String.Format("sap_rst"));
            //xVortexReset.SetAttribute("Value", ResetSignal);
            //xNIFCfg.AppendChild(xVortexReset);

            //XERoot.AppendChild(xNIFCfg);
            //xNIFDoc.AppendChild(XERoot);

            //string FilePath = String.Format("{0}\\{1}\\{2}.xml", ProjectDir, "core_config", VA.NIF.Instance);
            //xNIFDoc.Save(FilePath);
            #endregion

            // Write the port and bus connections to the PCore
            ComponentCore PCore = null;
            foreach(FalconMapping_Group Group in this.GetMappedGroups())
            {
                foreach (FalconMapping_Component Component in Group.GetGroupedComponents())
                {
                    if (Component.ComponentCores.ContainsKey(VA.CoreInstance))
                    {
                        foreach (ComponentCore pcore in Component.ComponentCores.Values)
                        {
                            if (String.Compare(pcore.CoreInstance, VA.Instance) == 0)
                            {
                                PCore = pcore; 
                                break;
                            }
                        }
                        if (PCore != null)
                            break;
                    }
                }
                if (PCore != null)
                    break;
            }
            if (PCore != null)
            {
                foreach (string RstPort in PCore.ResetPorts)
                {
                    PCore.Properties.SetValue(CerebrumPropertyTypes.PORT, RstPort, ResetSignal, true);
                }
                PCore.Properties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "MASTER_CMD_IF", MasterCmdSignal, true);
                PCore.Properties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "MASTER_DATA_IF", MasterDataSignal, true);
                PCore.Properties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "SLAVE_IF", SlaveSignal, true);
                PCore.Properties.SetValue(CerebrumPropertyTypes.PORT, "sap_clk", ClockSignal, true);
                PCore.Properties.SetValue(CerebrumPropertyTypes.PORT, "sap_rst", ResetSignal, true);

                PCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "EGRESS");
                PCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "INGRESS");
                PCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "CONFIG");
                PCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sop_clk");
                PCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sop_rst");
            }
        }
        /// <summary>
        /// Write Vortex SOP and NIF configurations for the CoreConfig files for the project.
        /// </summary>
        /// <param name="PathMan">The Project Path manager to be used for saving the core configurations.</param>
        /// <param name="VA">The Vortex SOP whose configurations are to be written.</param>
        private void WriteVortexSOPCoreConfigs(PathManager PathMan, IVortexAttachment VA)
        {
            string ProjectDir = PathMan["LocalProjectRoot"];
            string EgressSignal = String.Format("{0}_{1}_EGRESS", VA.Instance, VA.NIF.Type);
            string IngressSignal = String.Format("{0}_{1}_INGRESS", VA.Instance, VA.NIF.Type);
            string ConfigSignal = String.Format("{0}_{1}_CONFIG", VA.Instance, VA.NIF.Type);
            string ChannelSignal = String.Format("{0}_{1}_CHANNEL", VA.Instance, VA.NIF.Type);

            string ClockSignal = String.Format("{0}_{1}_clk", VA.Instance, VA.NIF.Type);
            string ResetSignal = String.Format("net_gnd", VA.Instance, VA.NIF.Type);

            #region Save the NIF
            CerebrumPropertyCollection CPC = new CerebrumPropertyCollection(VA.NIF.Instance, VA.NIF.Instance, VA.NIF.Type);
            CPC.SetValue(CerebrumPropertyTypes.PARAMETER, "C_CLK_SEL", 0, true);
            CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "EGRESS", EgressSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "INGRESS", IngressSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "CONFIG", ConfigSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "CHANNEL", ChannelSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.PORT, "sop_clk", ClockSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.PORT, "sop_rst", ResetSignal, true);
            CPC.SetValue(CerebrumPropertyTypes.PORT, "rst", "net_gnd", true);
            CPC.SavePropertyCollection(ProjectDir);

            //XmlDocument xNIFDoc = new XmlDocument();
            //xNIFDoc.AppendChild(xNIFDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            //XmlElement XERoot = xNIFDoc.CreateElement("CoreConfig");

            //XmlElement xNIFCfg = xNIFDoc.CreateElement("MPD");
            //xNIFCfg.SetAttribute("Instance", VA.NIF.Instance);
            //xNIFCfg.SetAttribute("Core", VA.NIF.Type);

            //// Write the NIF Configs
            //XmlElement xVortexClockSel = xNIFDoc.CreateElement("PARAMETER");
            //xVortexClockSel.SetAttribute("Name", String.Format("C_CLK_SEL", VA.Port));
            //xVortexClockSel.SetAttribute("Value", "0");
            //xNIFCfg.AppendChild(xVortexClockSel);

            //XmlElement xVortexChannel = xNIFDoc.CreateElement("BUS_INTERFACE");
            //xVortexChannel.SetAttribute("Name", String.Format("EGRESS"));
            //xVortexChannel.SetAttribute("Value", EgressSignal);
            //xNIFCfg.AppendChild(xVortexChannel);

            //XmlElement xVortexChannelManage = xNIFDoc.CreateElement("BUS_INTERFACE");
            //xVortexChannelManage.SetAttribute("Name", String.Format("INGRESS"));
            //xVortexChannelManage.SetAttribute("Value", IngressSignal);
            //xNIFCfg.AppendChild(xVortexChannelManage);

            //XmlElement xVortexMasterCmd = xNIFDoc.CreateElement("BUS_INTERFACE");
            //xVortexMasterCmd.SetAttribute("Name", String.Format("CONFIG"));
            //xVortexMasterCmd.SetAttribute("Value", ConfigSignal);
            //xNIFCfg.AppendChild(xVortexMasterCmd);

            //XmlElement xVortexMasterData = xNIFDoc.CreateElement("BUS_INTERFACE");
            //xVortexMasterData.SetAttribute("Name", String.Format("CHANNEL"));
            //xVortexMasterData.SetAttribute("Value", ChannelSignal);
            //xNIFCfg.AppendChild(xVortexMasterData);

            //XmlElement xVortexClock = xNIFDoc.CreateElement("PORT");
            //xVortexClock.SetAttribute("Name", String.Format("sop_clk"));
            //xVortexClock.SetAttribute("Value", ClockSignal);
            //xNIFCfg.AppendChild(xVortexClock);

            //XmlElement xVortexReset = xNIFDoc.CreateElement("PORT");
            //xVortexReset.SetAttribute("Name", String.Format("sop_rst"));
            //xVortexReset.SetAttribute("Value", ResetSignal);
            //xNIFCfg.AppendChild(xVortexReset);

            //XERoot.AppendChild(xNIFCfg);
            //xNIFDoc.AppendChild(XERoot);

            //string FilePath = String.Format("{0}\\{1}\\{2}.xml", ProjectDir, "core_config", VA.NIF.Instance);
            //xNIFDoc.Save(FilePath);
            #endregion

            // Write the port and bus connections to the PCore
            ComponentCore PCore = null;
            foreach (FalconMapping_Group Group in this.GetMappedGroups())
            {
                foreach (FalconMapping_Component Component in Group.GetGroupedComponents())
                {
                    if (Component.ComponentCores.ContainsKey(VA.CoreInstance))
                    {
                        foreach (ComponentCore pcore in Component.ComponentCores.Values)
                        {
                            if (String.Compare(pcore.CoreInstance, VA.Instance) == 0)
                            {
                                PCore = pcore;
                                break;
                            }
                        }
                        if (PCore != null)
                            break;
                    }
                }
                if (PCore != null)
                    break;
            }
            if (PCore != null)
            {
                foreach (string RstPort in PCore.ResetPorts)
                {
                    PCore.Properties.SetValue(CerebrumPropertyTypes.PORT, RstPort, ResetSignal, true);
                }
                PCore.Properties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "EGRESS", EgressSignal, true);
                PCore.Properties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "INGRESS", IngressSignal, true);
                PCore.Properties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "CONFIG", ConfigSignal, true);
                PCore.Properties.SetValue(CerebrumPropertyTypes.PORT, "sop_clk", ClockSignal, true);
                PCore.Properties.SetValue(CerebrumPropertyTypes.PORT, "sop_rst", ResetSignal, true);

                PCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "MASTER_CMD_IF");
                PCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "MASTER_DATA_IF");
                PCore.Properties.DeleteValue(CerebrumPropertyTypes.BUS_INTERFACE, "SLAVE_IF");
                PCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sap_clk");
                PCore.Properties.DeleteValue(CerebrumPropertyTypes.PORT, "sap_rst");
            }
        }
        /// <summary>
        /// Write Vortex Edge Core configurations for the CoreConfig files for the project.
        /// </summary>
        /// <param name="PathMan">The Project Path manager to be used for saving the core configurations.</param>
        /// <param name="VA">The Vortex Edge whose configurations are to be written.</param>
        private void WriteVortexEdgeCoreConfigs(PathManager PathMan, IVortexAttachment VA)
        {
            string ProjectDir = PathMan["LocalProjectRoot"];
            if (VA is IVortexEdgeAttachment)
            {
                VortexEdgeAttachment VEA = (VA as VortexEdgeAttachment);
                ComponentCore PCore = VEA.EdgeComponent as ComponentCore;                
                if (PCore != null)
                {
                    VEA.Instance = String.Format("{0}_interlink", PCore.CoreInstance);

                    string ChannelSignal = String.Format("{0}_CHANNEL", VEA.Instance);
                    string FIFO_TX_Signal = String.Format("{0}_FIFO_TX", VA.Instance);
                    string FIFO_RX_Signal = String.Format("{0}_FIFO_RX", VA.Instance);

                    #region Save the Attachment
                    CerebrumPropertyCollection CPC = new CerebrumPropertyCollection(VEA.Instance, VEA.Instance, VEA.Type);
                    CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "CHANNEL", ChannelSignal, true);
                    CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "TX_FIFO", FIFO_TX_Signal, true);
                    CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "RX_FIFO", FIFO_RX_Signal, true);
                    
                    CPC.SetValue(CerebrumPropertyTypes.PARAMETER, "CLK_SEL", 0, true);
                    CPC.SetValue(CerebrumPropertyTypes.PARAMETER, "BUS_ID", VEA.AttachedTo.BusID, true);
                    CPC.SetValue(CerebrumPropertyTypes.PARAMETER, "SWITCH_ID", VEA.AttachedTo.SwitchID, true);
                    CPC.SetValue(CerebrumPropertyTypes.PARAMETER, "PORT_ID", VEA.Port, true);

                    if (VEA.AttachedTo.ConfigurationAttachment == VEA)
                    {
                        string ConfigCh = String.Format("{0}_config", VEA.AttachedTo.Instance);
                        CPC.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "CONFIGURATION", ConfigCh, true);
                    }
                    CPC.SavePropertyCollection(ProjectDir);
                    #endregion

                    // Write the port and bus connections to the PCore
                    PCore.Properties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "FIFO_TX_IF", FIFO_TX_Signal, true);
                    PCore.Properties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "FIFO_RX_IF", FIFO_RX_Signal, true);
                    
                }
            }
        }

        /// <summary>
        /// Write Vortex Router configurations for the CoreConfig files for the project.
        /// </summary>
        /// <param name="PathMan">The Project Path manager to be used for saving the core configurations.</param>
        /// <param name="VR">The Vortex Router whose configurations are to be written.</param>
        private void WriteVortexConfig(PathManager PathMan, IVortex VR)
        {
            CerebrumPropertyCollection VortexProperties = new CerebrumPropertyCollection(VR.Instance, VR.Instance, VR.Type);
            VortexProperties.SetValue(CerebrumPropertyTypes.PARAMETER, "C_NUM_CHANNELS", VR.AttachedDevices.Count, true);
            VortexProperties.SetValue(CerebrumPropertyTypes.PARAMETER, "C_ROUTER_NUM_VCHANNELS", VR.NumPorts, true);
            VortexProperties.SetValue(CerebrumPropertyTypes.PARAMETER, "C_CHANNEL_INDEX_WIDTH", ((int)(Math.Ceiling(Math.Log(VR.NumPorts, 2)))), true);
            VortexProperties.SetValue(CerebrumPropertyTypes.PARAMETER, "C_LOCAL_BUS_ID", VR.BusID, true);
            VortexProperties.SetValue(CerebrumPropertyTypes.PARAMETER, "C_LOCAL_SWITCH_ID", VR.SwitchID, true);
 
            #region Clock Ports
            string VortexClock = GenerateSystemClockSignal(PathMan);
            VortexProperties.SetValue(CerebrumPropertyTypes.PORT, "router_clk1", VortexClock, true);
            //VortexProperties.SetValue(CerebrumPropertyTypes.PORT, "router_clk2", VortexClock, true);
            //VortexProperties.SetValue(CerebrumPropertyTypes.PORT, "router_clk3", VortexClock, true);
            //VortexProperties.SetValue(CerebrumPropertyTypes.PORT, "router_clk4", VortexClock, true);
            #endregion

            string VortexReset = "net_gnd";
            VortexProperties.SetValue(CerebrumPropertyTypes.PORT, "router_rst", VortexReset, true);

            #region Vortex NIF Interfaces
            for (int i = 0; i < VR.AttachedDevices.Count; i++)
            {
                IVortexAttachment VA = VR.AttachedDevices[i];
                int VortexPort = VA.Port;

                string ChannelSignal = string.Empty;
                string ManageSignal = string.Empty;

                if ((VA is IVortexSAP) ||
                    (VA is IVortexSOP))
                {
                    ChannelSignal = String.Format("{0}_{1}_CHANNEL", VA.Instance, VA.NIF.Type);
                    ManageSignal = String.Format("{0}_{1}_CHANNEL_MANAGE", VA.Instance, VA.NIF.Type);
                }
                else if (VA is IVortexBridgeAttachment)
                {
                    VortexBridgeAttachment VBA = (VA as VortexBridgeAttachment);
                    ChannelSignal = String.Format("{0}_CHANNEL", VBA.CoreInstance);
                }
                else if (VA is IVortexEdgeAttachment)
                {
                    VortexEdgeAttachment VEA = (VA as VortexEdgeAttachment);
                    ComponentCore PCore = VEA.EdgeComponent as ComponentCore;
                    VEA.Instance = String.Format("{0}_interlink", PCore.CoreInstance);
                    if (PCore != null)
                    {
                        ChannelSignal = String.Format("{0}_CHANNEL", VEA.Instance);
                    }
                }

                VortexProperties.SetValue(CerebrumPropertyTypes.PARAMETER, String.Format("C_CHANNEL{0}_TYPE", VortexPort), 2, true);
                VortexProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, String.Format("CHANNEL{0}", VortexPort), ChannelSignal, true);

                if (VA is IVortexSAP)
                {
                    VortexProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, String.Format("CHANNEL{0}_MANAGE", VortexPort), ManageSignal, true);
                }
                if (VA is IVortexBridgeAttachment)
                {
                    VortexBridgeAttachment VBA = VA as VortexBridgeAttachment;                    
                    VortexBridge VB = VBA.Bridge as VortexBridge;
                    if (VB != null)
                    {
                        CerebrumPropertyCollection BridgeAttachmentProperties = new CerebrumPropertyCollection(VBA.Instance, VBA.Instance, VBA.Type);
                        string BridgeTXFIFOCh0 = String.Format("{0}_tx_fifo_0", VB.Instance);
                        string BridgeTXFIFOCh1 = String.Format("{0}_tx_fifo_1", VB.Instance);
                        string BridgeRXFIFOCh0 = String.Format("{0}_rx_fifo_0", VB.Instance);
                        string BridgeRXFIFOCh1 = String.Format("{0}_rx_fifo_1", VB.Instance);
                        if (VBA.TDA == VB.TDA_1)
                        {
                            // If Attachment is Bridge Attachment #1, Write the Bridge Config
                            CerebrumPropertyCollection BridgeProperties = new CerebrumPropertyCollection(VB.Instance, VB.Instance, VB.Type);
                            BridgeProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "Vortex_0_TX_FIFO", BridgeTXFIFOCh0, true);
                            BridgeProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "Vortex_1_TX_FIFO", BridgeTXFIFOCh1, true);
                            BridgeProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "Vortex_0_RX_FIFO", BridgeRXFIFOCh0, true);
                            BridgeProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "Vortex_1_RX_FIFO", BridgeRXFIFOCh1, true);
                            BridgeProperties.SavePropertyCollection(PathMan["LocalProjectRoot"]);

                        }
                        // Either way, write the Bridge Attachment config
                        BridgeAttachmentProperties.SetValue(CerebrumPropertyTypes.PARAMETER, "CLK_SEL", 0, true);
                        BridgeAttachmentProperties.SetValue(CerebrumPropertyTypes.PARAMETER, "BUS_ID", VBA.AttachedTo.BusID, true);
                        BridgeAttachmentProperties.SetValue(CerebrumPropertyTypes.PARAMETER, "SWITCH_ID", VBA.AttachedTo.SwitchID, true);
                        BridgeAttachmentProperties.SetValue(CerebrumPropertyTypes.PARAMETER, "PORT_ID", VBA.Port, true);

                        if (VBA.TDA == VB.TDA_1)
                        {
                            BridgeAttachmentProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "CHANNEL", ChannelSignal, true);
                            BridgeAttachmentProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "TX_FIFO", BridgeTXFIFOCh0, true);
                            BridgeAttachmentProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "RX_FIFO", BridgeRXFIFOCh0, true);
                        }
                        else if (VBA.TDA == VB.TDA_2)
                        {
                            BridgeAttachmentProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "CHANNEL", ChannelSignal, true);
                            BridgeAttachmentProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "TX_FIFO", BridgeTXFIFOCh1, true);
                            BridgeAttachmentProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "RX_FIFO", BridgeRXFIFOCh1, true);
                        }
                        BridgeAttachmentProperties.SavePropertyCollection(PathMan["LocalProjectRoot"]);
                    }
                }

                if (VR.ConfigurationAttachment != null)
                {
                    string ConfigCh = String.Format("{0}_config", VR.Instance);
                    VortexProperties.SetValue(CerebrumPropertyTypes.BUS_INTERFACE, "CONFIGURATION", ConfigCh, true);
                }
                else
                {
                    //VortexProperties.SetValue(CerebrumPropertyTypes.PORT, "config_address", "net_gnd", true);
                    //VortexProperties.SetValue(CerebrumPropertyTypes.PORT, "config_wren", "net_gnd", true);
                    //VortexProperties.SetValue(CerebrumPropertyTypes.PORT, "config_rden", "net_gnd", true);
                    //VortexProperties.SetValue(CerebrumPropertyTypes.PORT, "config_be", "net_gnd", true);
                    //VortexProperties.SetValue(CerebrumPropertyTypes.PORT, "config_din", "net_gnd", true);
                }

                WriteVortexNIFEdgeCoreConfig(PathMan, VA);
            }
            #endregion

            VortexProperties.SavePropertyCollection(PathMan["LocalProjectRoot"]);
        }

        /// <summary>
        /// Gets a signal name that is associated with a clock signal matching the frequency of the project system clock.
        /// </summary>
        /// <param name="PathMan">The Project PathManager object containing the parameter for the system clock frequency.</param>
        /// <returns>A string indicating the signal name that corresponds to a signal that matches the frequency of the project system clock.</returns>
        private string GenerateSystemClockSignal(PathManager PathMan)
        {
            string SystemClock = string.Empty;
            ClockSignal DesiredClock = new ClockSignal();
            long DesiredFrequency = 100000000;
            string DefaultFrequency = "100MHZ";
            string ProjectFrequency = DefaultFrequency;
            if (PathMan.HasPath("SystemFrequency"))
            {
                ProjectFrequency = PathMan["SystemFrequency"].ToUpper();
            }
            ProjectFrequency = ProjectFrequency.Replace("GHZ", "000MHZ");
            ProjectFrequency = ProjectFrequency.Replace("MHZ", "000KHZ");
            ProjectFrequency = ProjectFrequency.Replace("KHZ", "000HZ");
            ProjectFrequency = ProjectFrequency.Replace("HZ", "");
            long.TryParse(ProjectFrequency, out DesiredFrequency);

            DesiredClock.Frequency = DesiredFrequency;
            DesiredClock.Phase = 0;
            DesiredClock.Group = ClockGroup.NONE;
            DesiredClock.Buffered = true;
            DesiredClock.SignalDirection = ClockDirection.INPUT;
            foreach (FalconMapping_Group Grp in this.GetMappedGroups())
            {
                foreach (FalconMapping_Component Cmp in Grp.GetGroupedComponents())
                {
                    foreach (ClockSignal CS in Cmp.OutputClocks)
                    {
                        if (CS.IsCompatibleWith(DesiredClock))
                        {
                            SystemClock = CS.GenerateCoreSignal();
                            break;
                        }
                    }
                    if (SystemClock != string.Empty)
                        break;
                }
                if (SystemClock != string.Empty)
                    break;
            }
            if (SystemClock == string.Empty)
            {
                if (!_ClockGen.HasAvailableClock(DesiredClock))
                {
                    this.ClockGenerator.AddClock(DesiredClock);
                }
                ClockSignal CS = _ClockGen.GetAvailableClock(DesiredClock);
                SystemClock = CS.GenerateCoreSignal();
                _ClockGen.AcquireClockSignal(CS);
            }
            return SystemClock;
        }
        #endregion

        #region Handling for Required Components

        private List<FalconMapping_Component> _RequiredComponents;
        /// <summary>
        /// Get a list of CerebrumComponents that MUST be instantiated on the FPGA
        /// </summary>
        public List<FalconMapping_Component> RequiredComponents
        {
            get
            {
                if (_RequiredComponents == null)
                    _RequiredComponents = new List<FalconMapping_Component>();
                return _RequiredComponents;
            }
        }

        //internal void ReadRequiredComponentFromXML(PathManager PathMan, XmlNode RequiredComponentNode)
        //{
        //    string TrueInstance = string.Empty;
        //    foreach (XmlNode xProperty in RequiredComponentNode.ChildNodes)
        //    {
        //        if (String.Compare(xProperty.Name, "Parameter", true) == 0)
        //        {
        //            foreach (XmlAttribute xAttr in xProperty.Attributes)
        //            {
        //                if (String.Compare(xAttr.Name, "Instance", true) == 0)
        //                {
        //                    TrueInstance = String.Format("{0}_{1}", this.ID.Replace(".", "_"), xAttr.Value);
        //                    break;
        //                }
        //            }
        //            if (TrueInstance != string.Empty)
        //                break;
        //        }
        //    }
        //    if (TrueInstance != string.Empty)
        //    {
        //        FalconMapping_Component newComponent = new FalconMapping_Component(TrueInstance, TrueInstance, this.Architecture);
        //        string SourceFolder = string.Empty;
        //        string SourceType = string.Empty;
        //        string SourceVer = string.Empty;
        //        foreach (XmlAttribute xAttr in RequiredComponentNode.Attributes)
        //        {
        //            if (String.Compare(xAttr.Name, "Source", true) == 0)
        //            {
        //                SourceFolder = xAttr.Value;
        //            }
        //            else if (String.Compare(xAttr.Name, "Type", true) == 0)
        //            {
        //                SourceType = xAttr.Value;
        //            }
        //            else if (String.Compare(xAttr.Name, "Version", true) == 0)
        //            {
        //                SourceVer = xAttr.Value;
        //            }
        //        }

        //        newComponent.Source = SourceFolder;
        //        if ((newComponent.Source == null) || (newComponent.Source == string.Empty))
        //        {
        //            throw new Exception();
        //        }
        //        newComponent.LoadComponentSource(PathMan);
        //        foreach (XmlNode xProperty in RequiredComponentNode.ChildNodes)
        //        {
        //            if ((String.Compare(xProperty.Name, "Parameter", true) == 0) ||
        //                (String.Compare(xProperty.Name, "CerebrumProperty", true) == 0))
        //            {
        //                string Core = string.Empty;
        //                string Name = string.Empty;
        //                string Value = string.Empty;
        //                CerebrumPropertyEntry CPE = new CerebrumPropertyEntry();

        //                foreach (XmlAttribute xAttr in xProperty.Attributes)
        //                {
        //                    if (String.Compare(xAttr.Name, "core", true) == 0)
        //                    {
        //                        Core = xAttr.Value;
        //                    }
        //                    else if (String.Compare(xAttr.Name, "name", true) == 0)
        //                    {
        //                        Name = xAttr.Value;
        //                    }
        //                    else if (String.Compare(xAttr.Name, "value", true) == 0)
        //                    {
        //                        Value = xAttr.Value;
        //                    }
        //                }
        //                if (Name == string.Empty)
        //                    continue;

        //                CPE.AssociatedCore = Core;
        //                CPE.PropertyName = Name;
        //                CPE.PropertyValue = Value;
                        
        //                if (String.Compare(xProperty.Name, "Parameter", true) == 0)
        //                {
        //                    CPE.PropertyType = CerebrumPropertyTypes.PARAMETER;
        //                    if ((Core != null) && (Core != string.Empty))
        //                    {
        //                        newComponent.AddNativeCoreProperty(Core, CPE);
        //                    }
        //                    else
        //                    {
        //                        newComponent.Properties.SetValue(CPE, true);
        //                    }
        //                }
        //                else if (String.Compare(xProperty.Name, "CerebrumProperty", true) == 0)
        //                {
        //                    CPE.PropertyType = CerebrumPropertyTypes.CEREBRUMPROPERTY;
        //                    if ((Core != null) && (Core != string.Empty))
        //                    {
        //                        newComponent.AddNativeCoreCerebrumProperty(Core, CPE);
        //                    }
        //                }
        //            }
        //        }
        //        this.RequiredComponents.Add(newComponent);
        //    }
        //}
        #endregion
    }
}
