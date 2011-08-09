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
 * FalconMapping_Group.cs
 * Name: Matthew Cotter
 * Date: 2 Apr 2010 
 * Description: This class implements the methods and properties required to represent
 * a logical component for use during the Component-to-FPGA mapping algorithm.
 * History: 
 * >> ( 7 Oct 2010) Matthew Cotter: Changes required due to migration of ResourceInfo & ResourceSet structures to separate library.
 *                                  Converted generic Hashtables to TypedHashtables<> for readability and verifiability.
 * >> (13 May 2010) Matthew Cotter: Updated FalconGroupComparer Class to support weighting of I/O and Resources in Scoring
 * >>                                 Updated scoring of resources and I/O to be more accurate/sensitive
 * >> (12 May 2010) Matthew Cotter: Moved FalconGroupComparer Class to this file from FalconMapping_Algorithm.cs
 * >>                                 Moved static function PrintGroupScores() to FalconMapping_FPGA Class from FalconMapping_Algorithm
 * >> ( 6 May 2010) Matthew Cotter: Modified ArrayList member utilization to store objects rather than IDs
 * >> (26 Apr 2010) Matthew Cotter: Added Debug Print Functions to print resources to System.Diagnostics.Debug Output
 * >> (25 Apr 2010) Matthew Cotter: Updated FPGA/Resource Scoring, Implemented TargetFPGA property
 * >> (16 Apr 2010) Matthew Cotter: Implemented Highest/Lowest Resource Functions
 * >> (16 Apr 2010) Matthew Cotter: Implemented Functions to Score FPGAs based on Resources
 * >> ( 2 Apr 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using CerebrumSharedClasses;
using FalconResources;

namespace FalconMappingAlgorithm
{


    /// <summary>
    /// IComparer class to compare two FalconMapping_Group objects for sorting order.
    /// The comparison is made by weighting scores from total resources compared against a baseline, 
    /// as well as physical link-distance from Input.
    /// </summary>
    public class FalconGroupComparer : IComparer<FalconMapping_Group>
    {
        private Dictionary<string, long> htComparisonSet;

        private double dResourceWeight = 0.5F;
        private double dIOWeight = 0.5F;

        /// <summary>
        /// Constructor to initialize the FalconGroupComparer object.
        /// </summary>
        public FalconGroupComparer()
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
        /// Get or set the weighting applied to I/O connectivity in scoring for comparison
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
        /// Get or set the weighting applied to resource requirements in scoring for comparison
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
        /// Implementation of IComparer.Compare() method to compare 2 FalconMapping_Group objects.  If either
        /// object is not such an object, the function always returns 0.  Scoring ties are broken using
        /// String.CompareOrdinal() using the IDs of the two Groups.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>Returns a negative value if 'x has a lower score than y', positive if 'x has a better score than y', 
        /// and 0 if either x or y is not a FalconMapping_Group object.</returns>
        int IComparer<FalconMapping_Group>.Compare(FalconMapping_Group x, FalconMapping_Group y)
        {
            if (htComparisonSet == null)
                return 0;

            if ((x.GetType() == typeof(FalconMapping_Group)) &&
                (y.GetType() == typeof(FalconMapping_Group)))
            {
                FalconMapping_Group groupX = x;
                FalconMapping_Group groupY = y;
                if (x == y)
                    return 0;

                double scoreX = WeightedScore(groupX);
                double scoreY = WeightedScore(groupY);

                // x is less than y, if x is "smaller" than y
                // x is greater than y, if x is "larger" than y
                if (scoreX == scoreY)
                {
                    // Break tie by Group ID
                    return (int)(String.CompareOrdinal(groupX.ID, groupY.ID));
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
        /// Calculates the weighted score of the specified FalconMapping_Group
        /// </summary>
        /// <param name="groupX">The FalconMapping_Group object to score.</param>
        /// <returns>Returns a score, with IO and Resource requirements weighted as specified in IOWeight and ResourceWeight</returns>
        public double WeightedScore(FalconMapping_Group groupX)
        {
            double scoreX;
            double resScoreX;
            double IOScoreX;
            resScoreX = groupX.ScoreResourceSet(htComparisonSet);           // Smaller score => Tighter fit, more desirable
            if (resScoreX < 0) resScoreX = 0;
            IOScoreX = (0.1 / (double)(groupX.AverageDistanceFromInput() + 1));       // 1 / (d + 1)      
            //IOScoreX = groupX.AverageDistanceFromInput();
            scoreX = (dResourceWeight * resScoreX) + (dIOWeight * IOScoreX);
            //System.Diagnostics.Debug.WriteLine(
            //    String.Format("\t\tGroup {0}:\t\t\t\t\t\tRS {1}, \tWRS {3},\t\t\tIOS {2},WIOS {4}",
            //    groupX.ID, resScoreX.ToString("000.0000"), IOScoreX.ToString("000.0000"), (dResourceWeight * resScoreX).ToString("000.0000"), (dIOWeight * IOScoreX).ToString("000.0000")));
            return scoreX;
        }

    }


    /// <summary>
    /// Represents a forced physical co-location or grouping of components in the system design.
    /// </summary>
    public class FalconMapping_Group
    {
        private string strID;
        private string strName;
        private string strTargetFPGA;
        private List<FalconMapping_Component> _MemberComponents;

        private bool bIsMapped;
        private List<string> _SupportedArchitectures;


        /// <summary>
        /// Creates a new FalconMapping_Group object representing a logical group in the design.
        /// </summary>
        /// <param name="sID">The ID of the new group</param>
        /// <param name="sName">The Name of the new Group</param>
        public FalconMapping_Group(string sID, string sName)
        {
            strID = sID;
            strName = sName;
            _MemberComponents = new List<FalconMapping_Component>();
        }

        /// <summary>
        /// Creates a new FalconMapping_Group object representing a logical Group in the design.
        /// The name of the group will be generated based on the ID.
        /// </summary>
        /// <param name="sID">The ID of the new group</param>
        public FalconMapping_Group(string sID)
        {
            strID = sID;
            strName = "group_" + strID;
            _MemberComponents = new List<FalconMapping_Component>();
        }

        #region Properties
        /// <summary>
        /// Get or set the ID of this group.
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
                foreach (object o in _MemberComponents)
                {
                    FalconMapping_Component memberComp = (FalconMapping_Component)o;
                    if (memberComp.IsGrouped)
                        if (memberComp.GroupID == oldID)
                            memberComp.GroupID = newID;
                }
                strID = newID;
            }
        }

        /// <summary>
        /// Gets or set Name of this group.
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
        /// Get or set a boolean flag indicating whether this group has mapped.
        /// Note: This property should only be written by the MapGroup() call of the FalconMapping_FPGA object this
        /// group is being mapped to.  Modifying this property in another situation could create an unpredictable state.
        /// </summary>
        public bool IsMapped
        {
            get
            {
                return bIsMapped;
            }
            set
            {
                bIsMapped = value;
            }
        }

        /// <summary>
        /// Get or set the group's copy of the ID of the FPGA to which it has been mapped.
        /// If it's not mapped, this should return string.Empty.
        /// Note: This property should only be written by the MapGroup() call of the FalconMapping_FPGA object this
        /// group is being mapped to.  Modifying this property in another situation could create an unpredictable state.
        /// </summary>
        public string TargetFPGA
        {
            get
            {
                return strTargetFPGA;
            }
            set
            {
                strTargetFPGA = value;
            }
        }

        /// <summary>
        /// Get the list of architecture families supported by ALL components in this group.   This list is the INTERSECTION of the corresponding lists of each component in the group.  
        /// (i.e. virtex4, virtex5, virtex6, etc)  If this list is null, then there are no common supported architectures.
        /// </summary>
        public List<string> SupportedArchitectures
        {
            get
            {
                return _SupportedArchitectures;
            }
        }
        #endregion

        /// <summary>
        /// Calculates the list of architectures that shared among all member components.
        /// </summary>
        private void IntersectArchitectures()
        {
            List<string> Intersect = new List<string>();
            foreach (FalconMapping_Component comp in _MemberComponents)
            {
                foreach (string arch in comp.SupportedArchitectures)
                {
                    bool bAddArch = true;
                    foreach (FalconMapping_Component comp2 in _MemberComponents)
                    {
                        if (comp != comp2)
                        {
                            if (!comp2.SupportedArchitectures.Contains(arch))
                            {
                                bAddArch = false;
                                break;
                            }
                        }
                    }
                    if (bAddArch)
                    {
                        Intersect.Add(arch);
                    }
                }
            }
            if (Intersect.Count == 0)
                _SupportedArchitectures = null;
            else
                _SupportedArchitectures = Intersect;
        }
        /// <summary>
        /// Gets the ResourceInfo object containing required resources for this group's members, which includes their NIFs
        /// </summary>
        public ResourceInfo RequiredResources
        {
            get
            {
                ResourceInfo RIRequired = new ResourceInfo();
                foreach (FalconMapping_Component Comp in this._MemberComponents)
                {
                    RIRequired.Add(Comp.Resources);
                }
                return RIRequired;
            }
        }


        /// <summary>
        /// Function to forcibly clear the associated group information from a component. 
        /// Note: This function is only intended to be used as cleanup in the event that this group cannot be properly unmapped with
        /// a call to its associated FPGA's UnmapGroup() function.  Calling this in another situation could create an unpredictable state.
        /// </summary>
        public void ClearMapping()
        {
            bIsMapped = false;
            strTargetFPGA = String.Empty;
        }


        /// <summary>
        /// Gets the average of the pre-calculated minimum number of logical connections between each member component and its corresponding 
        /// nearest input component.
        /// If this value is 0, it indicates that all members are input components.
        /// If this value is less than 0, it indicates that distance calculation has not, been or could not be done for at least one member component, 
        /// or there are no member components in the group.
        /// </summary>
        public float AverageDistanceFromInput()
        {
            if (_MemberComponents.Count == 0)
                return -1.0F;
            int total = 0;
            foreach (FalconMapping_Component fmComp in _MemberComponents)
            {
                if (fmComp.DistanceFromInput < 0)
                    return -1.0F;
                else
                    total = total + fmComp.DistanceFromInput;
            }
            return ((float)total / (float)(_MemberComponents.Count));
        }


        
        /// <summary>
        /// Add a component to this group, combining its resources with those requried by other members of the group.
        /// If this component ID is already a member of another group, a ComponentAlreadyGroupedException is thrown.
        /// Otherwise, the component is processed and added to the group, if it is not already a member.
        /// </summary>
        /// <param name="NewMemberComponent">The FalconMapping_Component which is to be added to this group.</param>
        public void AddComponent(FalconMapping_Component NewMemberComponent)
        {
            if ((NewMemberComponent.IsGrouped) && (NewMemberComponent.GroupID != this.ID))
                throw new Exceptions.ComponentAlreadyGroupedException(
                    String.Format("Unable to add component to group {0}.  Component {1} is already a member of group {2}.",
                     this.ID, NewMemberComponent.ID, NewMemberComponent.GroupID));

            if ((NewMemberComponent.IsGrouped) && (NewMemberComponent.GroupID == this.ID))
                return;
            if (_MemberComponents.Contains(NewMemberComponent))
            {
                NewMemberComponent.IsGrouped = true;
                NewMemberComponent.GroupID = this.ID;
                return;
            }

            _MemberComponents.Add(NewMemberComponent);
            IntersectArchitectures();

            NewMemberComponent.IsGrouped = true;
            NewMemberComponent.GroupID = this.ID;
        }

        /// <summary>
        /// Remove a component from this group, removing its required resources from those of the group.
        /// If the component is grouped, but not a member of this group, a ComponentNotGroupedException is thrown.
        /// </summary>
        /// <param name="ExMemberComponent">The component which is to be removed from the group.</param>
        public void RemoveComponent(FalconMapping_Component ExMemberComponent)
        {
            if ((!ExMemberComponent.IsGrouped) || (ExMemberComponent.IsGrouped && (ExMemberComponent.GroupID != this.ID)))
                throw new Exceptions.ComponentNotGroupedException(
                     String.Format("Unable to remove component from group {0}.  Component {1} is is not a member of this group.",
                     this.ID, ExMemberComponent.ID));

            if ((ExMemberComponent.IsGrouped) && (ExMemberComponent.GroupID == this.ID))
            {
                _MemberComponents.Remove(ExMemberComponent);
                ExMemberComponent.ClearGroup();
                IntersectArchitectures();
            }
        }

        /// <summary>
        /// Remove any and all components that have been assigned to this group.
        /// </summary>
        public void RemoveAll()
        {
            while (_MemberComponents.Count > 0)
            {
                RemoveComponent(_MemberComponents[0]);
            }
        }



        /// <summary>
        /// Get a copy of the set of FalconMapping_Components that have been added to this group, for enumeration.
        /// Note: While only a copy of the list is returned, the elements of that list are not copies.  Avoid 
        /// unintentionally modifying the elements of the ArrayList.
        /// </summary>
        /// <returns>Returns a copy of the Arraylist containing all of the FalconMapping_Components that have been added to this group.</returns>
        public List<FalconMapping_Component> GetGroupedComponents()
        {
            List<FalconMapping_Component> l = new List<FalconMapping_Component>();
            l.AddRange(_MemberComponents);
            return l;
        }

        /// <summary>
        /// Gets the amount required of a particular resource for this group's members.
        /// </summary>
        /// <param name="ResourceName">Name of the resource.</param>
        /// <returns>The amount of the specified resource required for this group's members.</returns>
        public long GetRequiredResource(string ResourceName)
        {
            return this.RequiredResources.GetResource(ResourceName);
        }

        /// <summary>
        /// Gets a Hashtable set of the amount required of a all resources for this group's members.
        /// </summary>
        /// <returns>The set of all resources required for this group's members.</returns>
        public Dictionary<string, long> GetRequiredResources()
        {
            return this.RequiredResources.GetResources();
        }


        /// <summary>
        /// Returns the name of the most demanding resource for this group's members.
        /// </summary>
        /// <returns>The name of the most demanding resource for this group's members.</returns>
        public string GetHighestResourceName()
        {
            string RName = string.Empty;
            long RValue = 0;
            this.RequiredResources.GetHighestResource(ref RName, ref RValue);
            return RName;
        }

        /// <summary>
        /// Returns the amount of the most demanding resource for this group's members.
        /// </summary>
        /// <returns>The amount of the most demanding resource for this group's members.</returns>
        public long GetHighestResourceAmount()
        {
            string RName = string.Empty;
            long RValue = 0;
            this.RequiredResources.GetHighestResource(ref RName, ref RValue);
            return RValue;
        }

        /// <summary>
        /// Returns the name of the least demanding resource for this group's members
        /// </summary>
        /// <returns>The name of the least demanding resource for this group's members</returns>
        public string GetLowestResourceName()
        {
            string RName = string.Empty;
            long RValue = 0;
            this.RequiredResources.GetLowestResource(ref RName, ref RValue);
            return RName;
        }

        /// <summary>
        /// Returns the amount of the least demanding resource for this group's members.
        /// </summary>
        /// <returns>The amount of the least demanding resource within this group's members.</returns>
        public long GetLowestResourceAmount()
        {
            string RName = string.Empty;
            long RValue = 0;
            this.RequiredResources.GetLowestResource(ref RName, ref RValue);
            return RValue;
        }

        /// <summary>
        /// Scores the resources required by this group against those available on the FPGA to determine
        /// how well it fits onto the FPGA.  Lower scores indicate a greater excess of resources.  Higher scores
        /// indicate a better fit.
        /// </summary>
        /// <param name="TestFPGA">The FalconMapping_FPGA object upon which this group's requirements should be scored.</param>
        /// <returns>A value indicating how well this group's requirements would be met by
        /// the resources available on the FPGA, or -1.0 if the requirements cannot be met by the FPGA.</returns>
        public double ScoreFPGA(FalconMapping_FPGA TestFPGA)
        {
            // Score based on available resources
            double fScore = -1.0F;
            Dictionary<string, long> htAvailable = TestFPGA.GetAvailableResources();
            if (TestFPGA.HasSufficientResources(this))
                fScore = this.ScoreResourceSet(htAvailable);            
            return fScore;
        }
        
        /// <summary>
        /// Scores the resources required by this group against those specified to determine 
        /// how well the group would fits into those resources.  Lower scores indicate a greater excess of resources.  Higher scores
        /// indicate a better fit.
        /// </summary>
        /// <param name="htResources">The hashtable set of resources upon which this group's requirements should be scored.</param>
        /// <returns>A value indicating how well this group's requirements are accomodated by
        /// the resources specified resource set, or -1.0 if the requirements cannot be met by the specified resources.</returns>
        public double ScoreResourceSet(Dictionary<string, long> htResources)
        {
            return FalconMapping_ResourceScoring.ScoreResourceSet(htResources, this.GetRequiredResources());
        }


        /// <summary>
        /// Print out group resource requirements and mapping status to System.Diagnostics.Debug
        /// </summary>
        internal void DebugPrintResources()
        {
            Dictionary<string, long> htTotal = this.RequiredResources.GetResources();
            System.Diagnostics.Debug.WriteLine("--------------------------");
            System.Diagnostics.Debug.WriteLine("Group ID: " + this.ID + ", Required Resources:");
            foreach (string resName in htTotal.Keys)
            {
                long req = this.RequiredResources.GetResource(resName);

                System.Diagnostics.Debug.WriteLine(resName + ": " + req.ToString());
            }
            if (this.IsMapped)
                System.Diagnostics.Debug.WriteLine("   Mapped to FPGA: " + this.TargetFPGA);
            else
                System.Diagnostics.Debug.WriteLine("   UNMAPPED");

            System.Diagnostics.Debug.WriteLine("--------------------------");
        }

        /// <summary>
        /// Print out all Groups and their scores to System.Diagnostics.Debug
        /// </summary>
        /// <param name="lGroups">The list of FalconMapping_Groups</param>
        /// <param name="GroupComparer">The group comparer object that calculates and compares the Group scoring</param>
        internal static void PrintGroupScores(List<FalconMapping_Group> lGroups, FalconGroupComparer GroupComparer)
        {
            System.Diagnostics.Debug.WriteLine("Group Scores");
            for (int iIndex = 0; iIndex < lGroups.Count; iIndex++)
            {
                FalconMapping_Group fmg = lGroups[iIndex];
                StringBuilder strRes = new StringBuilder();
                foreach (string resName in fmg.RequiredResources.GetResources().Keys)
                {
                    strRes.Append(String.Format("{0} {1}\t", (long)fmg.RequiredResources.GetResource(resName), resName));
                }
                System.Diagnostics.Debug.WriteLine(
                    String.Format("\t    GID: {0} -\tScore: {1}\t{2}",
                    fmg.ID, GroupComparer.WeightedScore(fmg).ToString("#000.000000"), strRes.ToString()));
            }
        }
    }
}
