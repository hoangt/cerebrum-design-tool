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
 * FalconMapping_Cluster.cs
 * Name: Matthew Cotter
 * Date: 16 Apr 2010 
 * Description: This class implements the methods and properties required to represent
 * a logical cluster of FPGAs for use during the Component-to-FPGA mapping algorithm.
 * History: 
 * >> ( 7 Oct 2010) Matthew Cotter: Changes required due to migration of ResourceInfo & ResourceSet structures to separate library.
 *                                  Converted generic Hashtables to TypedHashtables<> for readability and verifiability.
 * >> ( 6 May 2010) Matthew Cotter: Modified ArrayList member utilization to store objects rather than IDs
 * >> (16 Apr 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace FalconMappingAlgorithm
{
    /// <summary>
    /// Represents a clustering, either physical or logical, of FPGAs in the system design. This object is primarily to accomodate
    /// a logical representation of multiple FPGAs on a single board, within a multi-board platform.
    /// </summary>
    public class FalconMapping_Cluster
    {
        private string strID;
        private string strName;
        private ArrayList alMemberFPGAs;

        /// <summary>
        /// Creates a new FalconMapping_Cluster object representing an FPGA cluster in the design.
        /// </summary>
        /// <param name="sID">The ID of this cluster</param>
        /// <param name="sName">The name of this cluster</param>
        public FalconMapping_Cluster(string sID, string sName)
        {
            strID = sID;
            strName = sName;
            alMemberFPGAs = new ArrayList();
        }

        /// <summary>
        /// Creates a new FalconMapping_Cluster object representing an FPGA cluster in the design.
        /// The name of the cluster will be generated based on the ID.
        /// </summary>
        /// <param name="sID">The ID of this cluster</param>
        public FalconMapping_Cluster(string sID)
        {
            strID = sID;
            strName = "cluster_" + strID;
            alMemberFPGAs = new ArrayList();
        }

        #region Properties

        /// <summary>
        /// Get or set the ID of this cluster.
        /// Note: This write mode of this property is provided only for ID re-assignment.  Changing this value without removing/re-adding it to
        /// any collections based on this ID can have unexpected or unknown results.
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
                foreach (object o in alMemberFPGAs)
                {
                    FalconMapping_FPGA memberFPGA = (FalconMapping_FPGA)o;
                    if (memberFPGA.IsClustered)
                        if (memberFPGA.ClusterID == oldID)
                            memberFPGA.ClusterID = newID;
                }
                strID = newID;
            }
        }

        /// <summary>
        /// Gets or set Name of this cluster.
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

        #endregion

        /// <summary>
        /// Assign an FPGA to this cluster.
        /// If this FPGA is already assigned to another cluster, other than this one, an FPGAAlreadyClusteredException is thrown.  
        /// Otherwise, the FPGA is assigned to this cluster, if it's not already a member.
        /// </summary>
        /// <param name="NewMemberFPGA">The FPGA which is to be assigned to this cluster.</param>
        public void AddFPGA(FalconMapping_FPGA NewMemberFPGA)
        {
            if ((NewMemberFPGA.IsClustered) && (NewMemberFPGA.ClusterID != this.ID))
                throw new Exceptions.FPGAAlreadyClusteredException(
                                String.Format("Unable to add FPGA {0} to cluster {1}.  FPGA {0} is already a member of cluster {2}",
                                NewMemberFPGA.ID, this.ID, NewMemberFPGA.ClusterID));
            
            if (!alMemberFPGAs.Contains(NewMemberFPGA))
                alMemberFPGAs.Add(NewMemberFPGA);
            NewMemberFPGA.IsClustered = true;
            NewMemberFPGA.ClusterID = this.ID;
        }

        /// <summary>
        /// Unassign an FPGA from this cluster.
        /// If this FGPA is not assigned to this cluster, an FPGANotClusteredException is thrown.  
        /// Otherwise, the FPGA is unassigned from this cluster, if it's a member.
        /// </summary>
        /// <param name="ExMemberFPGA">The FPGA which is to be unassigned from this cluster.</param>
        public void RemoveFPGA(FalconMapping_FPGA ExMemberFPGA)
        {
            if (ExMemberFPGA.IsClustered && (ExMemberFPGA.ClusterID != this.ID))
                throw new Exceptions.FPGANotClusteredException(
                                String.Format("Unable to remove FPGA {0} from cluster {1}.  FPGA {0} is not a member of cluster {1}",
                                ExMemberFPGA.ID, this.ID));

            if (alMemberFPGAs.Contains(ExMemberFPGA))
                alMemberFPGAs.Remove(ExMemberFPGA);

            ExMemberFPGA.ClearCluster();
        }
        
        /// <summary>
        /// Unassign any and all FPGAs that have been assigned to this cluster.
        /// </summary>
        public void RemoveAll()
        {
            while (alMemberFPGAs.Count > 0)
            {
                RemoveFPGA((FalconMapping_FPGA)alMemberFPGAs[0]);
            }
        }
        
        /// <summary>
        /// Get a copy of the set of FalconMapping_FPGAs that have been assigned to this cluster, for enumeration.
        /// Note: While only a copy of the list is returned, the elements of that list are not copies.  Avoid 
        /// unintentionally modifying the elements of the ArrayList.
        /// </summary>
        /// <returns>Returns a copy of the Arraylist containing all of the FalconMapping_FPGAs that have been assigned to this cluster.</returns>
        public ArrayList GetMembers()
        {
            return (ArrayList)alMemberFPGAs.Clone();
        }

    }
}
