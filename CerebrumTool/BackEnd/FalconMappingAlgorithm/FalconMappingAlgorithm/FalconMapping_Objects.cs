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
 * FalconMapping_Objects.cs
 * Name: Matthew Cotter
 * Date: 18 Apr 2010 
 * Description: This class represents a singular object containing collections of each the required objects for
 * the Component-to-FPGA mapping algorithm, and interface functions to read and write these objects to and from XML files
 * functions defined in the FalconMapping_XML static class library.
 * History: 
 * >> (25 May 2010) Matthew Cotter: Corrected bug in implementation of mapping algorithm that biased the mapping towards FPGAs which appear later in the list.
 * >> ( 9 May 2010) Matthew Cotter: Implemented support for configuring and supporting edge/bridge components read from the platform.
 * >> (11 Apr 2011) Matthew Cotter: Tweaked application of IO Weighting to have more impactful consideration on mapping.
 * >> (23 Mar 2011) Matthew Cotter: Identified and corrected a bug that would cause an infinite loop in a system design with at least 1 connection and 1 or more components
 *                                      with no inward or outward connections.
 *                                  Added configuration device information to the generated mapping report.
 * >> (17 Feb 2011) Matthew Cotter: Hopefully have finally nailed down issue in clock assignments to required component cores.
 * >> (16 Feb 2011) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Modified code to use improved properties system for loading, saving, and managing core configurations.
 * >> ( 7 Feb 2011) Matthew Cotter: Corrected bug in automatic validation and assignment of clock signals to Vortex components.
 * >> (28 Jan 2011) Matthew Cotter: Began work on supporting/defining Links that are backed by Vortex-based hardware cores in the platform.
 *                                  Implemented InsertEdgeComponents() method, with examines each connection in the design, and in conjunction with the routing algorithm, determines how to
 *                                      'break' the connections to insert connections to/from edge components.
 * >> (25 Jan 2011) Matthew Cotter: Added mapping report, generated at completion detailing where each component was mapped, and the Vortex Port/TDA to which it was assigned, if any.
 *                                  Implemented initial support for auto-saving intermediate mapping state.
 * >> (18 Jan 2011) Matthew Cotter: Added support for GUI-added platform components overriding those instantiated by the platform.
 * >> (22 Dec 2010) Matthew Cotter: Corrected but that prevented a component from seeing and utilizing clocks exposed by itself.
 * >> (16 Dec 2010) Matthew Cotter: Temporarily disabled automatic flow identification pending revamp of algorithm and support of identifying flows involving components 
 *                                      with multiple interfaces.
 * >> ( 1 Dec 2010) Matthew Cotter: Integration of Multiple-SAP Components into mapping.
 * >> ( 7 Oct 2010) Matthew Cotter: Changes required due to migration of ResourceInfo & ResourceSet structures to separate library.
 *                                  Converted generic Hashtables to TypedHashtables<> for readability and verifiability.
 * >> (27 Sep 2010) Matthew Cotter: Added method to retrieve the Architecture of an FPGA (GetFPGAArchitecture()).
 * >> (10 Aug 2010) Matthew Cotter: Corrected infinite loop in situation where no FPGA (or Component) can be identified as an input source when calculating I/O Distances.
 *                                  Added functions LinkPathExists() and GroupMembersVisibleToNeighbors() to verify communication between components remains possible.
 *                                      Ensuring that no connected components are placed on disconnected FPGAs.
 *                                  Added functions to begin implementation of connection-aware mapping.  A few code changes in DoMapping() were made to support this;
 *                                      these changes are denoted by the comment "CONNECTION MANAGEMENT"
 *                                      The new functions are listed here:
 *                                          AverageLinkUsage()
 *                                          ApplyGroupConnections()
 *                                          ApplyGrpConnections()
 *                                          ProjectedAverageLinkUsage()
 *                                          ApplyConnectionOnPath()
 *                                          RemoveConnectionOnPath()
 *                                          LinksOnPath()
 * >> ( 2 Aug 2010) Matthew Cotter: Corrected a bug that resulted in desynchronization of resources when adding a component to a group that is already mapped.
 * >> (21 Jul 2010) Matthew Cotter: Verified that XML-generated Documentation is current.
 * >> ( 7 Jul 2010) Matthew Cotter: Updated WriteComponentAddressMap() to only write a core to the address map file if its port is valid (currently > 1200).
 * >> ( 6 Jul 2010) Matthew Cotter: Added WriteComponentAddressMap() function to facilitate communication interfaces between boards.
 * >> (30 Jun 2010) Matthew Cotter: Added Properties (PlatformFile, DesignFile, CommunicationsFile) and Methods (ReadPDCFiles(), CreateMappingInput() to facilitate input from 
 *                                  Cerebrum design system.
 * >> (15 Jun 2010) Matthew Cotter: Removed automatic generation of groups at Load-time.  Auto-generation is now only done at 'execution' time.
 *                                  Corrected bug in resource management that arose when mapped groups had components added or removed.
 *                                  Corrected minor issue with automatic generation of groups for ungrouped components during mapping.
 * >> (10 Jun 2010) Matthew Cotter: Corrected reading/appending of Mapping XML files.
 * >> ( 4 Jun 2010) Matthew Cotter: Added/Implemented Reset(), UnMapAll(), UnGroupAll(), and UnClusterAll() methods for completeness.
 *                                  Added Implementation of FalconGlobal.IFalconLibrary Interface.
 * >> (17 May 2010) Matthew Cotter: Added WriteRoutingFile() to create a file populated with all required connection information to
 *                                  generate FPGA routing tables.
 *                                  Added WriteComponentMapFile() and WriteFPGAMapFile() to create files populated with all components and the FPGAs they are mapped to
 *                                  grouped by component and FPGA, respectively.
 * >> (12 May 2010) Matthew Cotter: Moved FalconFPGAComparer Class to FalconMapping_FPGA.cs
 *                                  Moved static function GetAverageFPGAResources() to FalconMapping_FPGA Class from FalconMapping_Algorithm
 *                                  Moved static function PrintFPGAList() to FalconMapping_FPGA Class from FalconMapping_Algorithm
 *                                  Moved FalconGroupComparer Class to FalconMapping_Group.cs
 *                                  Moved static function PrintGroupScores() to FalconMapping_FPGA Class from FalconMapping_Algorithm
 *                                  Moved DoMapping() function into FalconMapping_Objects class
 *                                  Moved IO Distance Calculation and Display functions into FalconMapping_Objects class
 *                                  FalconMapping_Objects class now supports a default Exception-notification/processing function (currently barebones)
 *                                  as well as the option (through constructor or property) of setting a custom exception handling delegate function.
 *                                     This will facilitate both a GUI- and Command-line version of exception display and notification.
 *                                  In support of a command-line/script style interface, added functions to Add/Remove/Modify/Get Objects 
 *                                  and properties for all mapping system objects                      
 * >> ( 6 May 2010) Matthew Cotter: Modified ArrayList member utilization to store objects rather than IDs
 * >> ( 6 May 2010) Matthew Cotter: Added class to sort FPGAs, made sorting/scoring connection-aware
 *                                  Added "collective" object FalconMapping_Objects, that houses all 6 key 
 *                                  objects in the mapping system.
 *                                  Added functions to parse collections and calculate distance from I/O
 * >> (26 Apr 2010) Matthew Cotter: Added Debug Print Statements to System.Diagnostics.Debug Output
 * >> (25 Apr 2010) Matthew Cotter: Created core functionality of Mapping Algorithm
 *                                  Verified that simple Input produces usable ouput
 * >> (18 Apr 2010) Matthew Cotter: Implemented function to average total resources across FPGAs.
 * >> (18 Apr 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;
using FalconPathManager;
using FalconGlobal;
using FalconResources;
using FalconGraph;
using VortexInterfaces;
using FalconClockManager;
using CerebrumSharedClasses;
using CerebrumNetronObjects;

namespace FalconMappingAlgorithm
{
    /// <summary>
    /// Global class containing all Mapping System Objects.
    /// </summary>
    /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/Mapping Algorithm Documentation.pdf">
    /// Mapping Algorithm Documentation</seealso>
    public class FalconMapping_Objects : FalconGlobal.IFalconLibrary
    {
        #region MessageEvent Manager
        /// <summary>
        /// Delegate for MessageEvent event
        /// </summary>
        /// <param name="Message">The message to be transmitted</param>
        public delegate void MessageEventDelegate(string Message);
        /// <summary>
        /// Event fired when a message has been generated by the library
        /// </summary>
        public event MessageEventDelegate MessageEvent;
        /// <summary>
        /// Raises an event indicating that a message has been generated.
        /// </summary>
        /// <param name="Message">The message to be transmitted</param>
        /// <param name="Args">List of replacements for token placeholders in the message.</param>
        public void RaiseMessageEvent(string Message, params object[] Args)
        {
            string outputText = Message;
            for (int i = 0; i < Args.Length; i++)
            {
                if (Args[i] != null)
                {
                    outputText = outputText.Replace("{" + i.ToString() + "}", Args[i].ToString());
                }
                else
                {
                    outputText = outputText.Replace("{" + i.ToString() + "}", string.Empty);
                }
            }
            if (MessageEvent != null)
            {
                MessageEvent(outputText);
            }
            else 
            {
                Console.WriteLine(outputText);
            }
        }

        #endregion

        #region Algorithm Exception Processing

        /// <summary>
        /// Delegate to be used for defining a function to customize the processing, handling, or display of any exception 
        /// thrown by FalconMappingAlgorithm calls.   The entire list of algorithm-specific exceptions is defined in the
        /// FalconMappingAlgorithms.Exceptions namespace, but other exceptions will be passed to the handler as well.
        /// </summary>
        /// <param name="ex">The exception being thrown.  The exact type is generalized to Exception and may be 
        /// obtained with a call to ex.GetType().</param>
        /// <param name="InfoString">Any additional information passed by the excepting function.</param>
        public delegate void ExceptionProcessor(Exception ex, string InfoString);

        private ExceptionProcessor exHandler;

        /// <summary>
        /// Pre-processor function to route exception handling to the default or user-specified processor delegate.
        /// </summary>
        /// <param name="ex">The exception to be processed.</param>
        /// <param name="AdditionalInfo">Additional information to be reported.</param>
        private void InternalExceptionPreProcessor(Exception ex, string AdditionalInfo)
        {
            if (exHandler != null)
            {
                ErrorReporting.DebugException(ex);
                RaiseMessageEvent(ErrorReporting.ExceptionDetails(ex));
                exHandler(ex, AdditionalInfo);
            }
            else
            {
                ErrorReporting.DebugException(ex);
                RaiseMessageEvent(ErrorReporting.ExceptionDetails(ex));
            }
        }

        /// <summary>
        /// Default exception handler function, writing the output to the Debug Console.
        /// </summary>
        /// <param name="ex">The exception to be processed.</param>
        /// <param name="AdditionalInfo">Additional information to be reported.</param>
        private void InternalExceptionProcessor(Exception ex, string AdditionalInfo)
        {
            System.Type exType = ex.GetType();
            System.Diagnostics.Debug.WriteLine(
                String.Format("Exception {0}thrown!\n\tMessage: {1}\n\t Additional Info: {2}\n", 
                    ex.GetType().ToString(), 
                    ex.Message, 
                    AdditionalInfo));
        }
        
        /// <summary>
        /// Gets or Sets the delegate function used to handle processing of exceptions generated by FalconMappingAlgorithm
        /// </summary>
        public ExceptionProcessor ExProcessor
        {
            get
            {
                return exHandler;
            }
            set
            {
                exHandler = value;
            }
        }
        #endregion

        #region Constructors, Initializers, and Reset
        /// <summary>
        /// Default constructor.  Initializes internal collections to empty/null state.  Assigns default 
        /// processing routine
        /// </summary>
        public FalconMapping_Objects()
        {
            Reset();
            exHandler = new ExceptionProcessor(InternalExceptionProcessor);
            this.UseIOWeighting = true;
            this.IOWeight = 0.5;
        }

        /// <summary>
        /// Default constructor.  Initializes internal collections to empty/null state.  Assigns passed routine as 
        /// custom exception processor routine
        /// </summary>
        public FalconMapping_Objects(ExceptionProcessor epf)
        {
            Reset();
            exHandler = epf;
            this.UseIOWeighting = true;
            this.IOWeight = 0.5;
        }

        /// <summary>
        /// Completely resets the system to an initial state with no objects or components read.  Useful for initialization or
        /// explicitly ensuring that any and all traces of a previously loaded system or mapping are gone before proceeding with a new system mapping.
        /// </summary>
        public void Reset()
        {
            try
            {
                if (Components != null)
                    Components.Clear();
                else
                    Components = new Dictionary<string, FalconMapping_Component>();

                if (FPGAs != null)
                    FPGAs.Clear();
                else
                    FPGAs = new Dictionary<string, FalconMapping_FPGA>();

                if (Groups != null)
                    Groups.Clear();
                else
                    Groups = new Dictionary<string, FalconMapping_Group>();

                if (Clusters != null)
                    Clusters.Clear();
                else
                    Clusters = new Dictionary<string, FalconMapping_Cluster>();

                if (Connections != null)
                    Connections.Clear();
                else
                    Connections = new Dictionary<string, FalconMapping_Connection>();

                if (MappedConnections != null)
                    MappedConnections.Clear();
                else
                    MappedConnections = new Dictionary<string, FalconMapping_Connection>();

                if (Links != null)
                    Links.Clear();
                else
                    Links = new Dictionary<string, FalconMapping_Link>();
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Fatal Error: Exception generated during Reset()");
            }
        }

        #endregion

        #region Properties
        private bool bUseIOWeighting;
        private double dIOWeight;

        /// <summary>
        /// FalconPathManager object used for maintaining a coherent set of project folder and file paths.
        /// </summary>
        public PathManager PathMan;

        /// <summary>
        /// Indicates whether the mapping algorithm will employ I/O distance weighting in determining what order to map groups.
        /// </summary>
        public bool UseIOWeighting
        {
            get
            {
                return bUseIOWeighting;
            }
            set
            {
                bUseIOWeighting = value;
            }
        }
        /// <summary>
        /// Set the weight, 0.0 to 1.0 to be applied to IO Connectivity in the mapping.
        /// </summary>
        public double IOWeight
        {
            get
            {
                if (bUseIOWeighting)
                    return dIOWeight;
                else
                    return 0;
            }
            set
            {
                if (value < 0)
                    dIOWeight = 0;
                else if (value > 1)
                    dIOWeight = 1;
                else 
                    dIOWeight = value;
            }
        }
        #endregion

        #region Mapping System Object Hashtables
        /// <summary>
        /// A Hashtable collection of the logical Processing-Element components in the system.
        /// </summary>
        internal Dictionary<string, FalconMapping_Component> Components;
        /// <summary>
        /// A Hashtable collection of the physical FPGAs in the system.
        /// </summary>
        internal Dictionary<string, FalconMapping_FPGA> FPGAs;

        /// <summary>
        /// A Hashtable collection of the pre-allocated logical groupings of components in the system.
        /// </summary>
        internal Dictionary<string, FalconMapping_Group> Groups;
        /// <summary>
        /// A Hashtable collection of logical groupings of FPGAs in the system.
        /// </summary>
        internal Dictionary<string, FalconMapping_Cluster> Clusters;
                
        /// <summary>
        /// A Hashtable collection of the logical connections between components in the system.
        /// </summary>
        internal Dictionary<string, FalconMapping_Connection> Connections;

        /// <summary>
        /// A Hashtable collection of the physical/logical connections between components in the system once mapping has been taken into consideration.
        /// This set of connections does not span FPGAs, but instead each connection terminates at an FPGA-boundary, with the assumption that inter-FPGA
        /// communication is handled by the component responsible for managing the link.
        /// </summary>
        internal Dictionary<string, FalconMapping_Connection> MappedConnections;

        /// <summary>
        /// A Hashtable collection of the physical connections between FPGAs in the system.
        /// </summary>
        internal Dictionary<string, FalconMapping_Link> Links;
        #endregion

        #region DoMapping() Algorithm implementation
       
        /// <summary>
        /// Primary function to map components/component groups to the set of available FPGAs.
        /// Each FalconMapping_Group in MappingObjects.Groups will be updated/modified to indicate that it has been mapped
        /// to an FPGA.  Each FalconMapping_FPGA in MappingObjects.FPGAs will be updated/modified to indicate that the corresponding
        /// groups have been mapped to it and the resources have been used.
        /// </summary>
        /// <returns>True if mapping was successful, false otherwise.</returns>
        public bool DoMapping()
        {
            bool rVal;            
            rVal = DoMapping_v2();
            return rVal;
        }

        /// <summary>
        /// Deprecated.  Initial version of mapping implementation. This method has been deprecated with DoMapping_v2().
        /// </summary>
        /// <returns>True if mapping was successful, False otherwise.</returns>
        private bool DoMapping_v1()
        {
            RaiseMessageEvent("\n\nStarting Component Map Process...");
            bool success = false;
            try
            {
                success = GroupUngroupedComponents();
                if (!success)
                {
                    throw new Exceptions.MappingFailedException(
                        String.Format("Mapping failed.  Unable to group all ungrouped components."));
                }
                //_Flows = DetectFlows();

                #region Map FPGA-Required Dynamic Components
                //foreach (FalconMapping_FPGA FPGA in FPGAs.Values)
                //{
                //    FalconMapping_Group DynamicGroup = new FalconMapping_Group(String.Format("dynamic_required_components_{0}", FPGA.ID));
                //    foreach (FalconMapping_Component Component in FPGA.DynamicComponents)
                //    {
                //        RequiredGroup.AddComponent(Component);
                //    }
                //    FPGA.MapGroup(DynamicGroup);
                //}
                #endregion

                #region Configure FPGA and Group Scoring and Comparison Objects
                Dictionary<string, long> htA = FalconMapping_FPGA.GetAverageFPGAResources(FPGAs);
                FalconGroupComparer gComp = new FalconGroupComparer();
                FalconFPGAComparer fComp = new FalconFPGAComparer();

                if (bUseIOWeighting)
                    gComp.IOWeight = 0.5;
                else
                    gComp.IOWeight = 0.0F;
                gComp.ResourceWeight = 0.5;
                gComp.ComparisonSet = htA;

                if (bUseIOWeighting)
                    fComp.IOWeight = 0.5;
                else
                    fComp.IOWeight = 0.0F;
                fComp.ResourceWeight = 0.5;
                fComp.ComparisonSet = htA;

                CalculateComponentIODistances();
                CalculateFPGAIODistances();
                #endregion

                #region Convert Hashtables to Arrays for Indexing and Sorting
                //Sort_groups(array of groups)
                List<FalconMapping_Group> lGroups = new List<FalconMapping_Group>();
                lGroups.AddRange(Groups.Values);
                FalconMapping_Group.PrintGroupScores(lGroups, gComp);
                lGroups.Sort(gComp);
                FalconMapping_Group.PrintGroupScores(lGroups, gComp);

                List<FalconMapping_FPGA> lFPGAs = new List<FalconMapping_FPGA>();
                lFPGAs.AddRange(FPGAs.Values);
                //FalconMapping_FPGA.PrintFPGAList(lFPGAs, fComp);
                lFPGAs.Sort(fComp);
                FalconMapping_FPGA.PrintFPGAList(lFPGAs, fComp);
                #endregion

                #region Create FPGA Adjacency Matrix
                Object[,] adjMatrix = CreateFPGAAdjacencyMatrix();
                #endregion


                #region Score Groups against FPGAs and ensure that communication remains possible between components
                string bestFPGA = string.Empty;
                double bestScore = -1.0F;
                double bestAverageUsage = -1.0F;
                //For each group in list
                // High score to Low
                for (int iGroupIndex = (lGroups.Count - 1); iGroupIndex >= 0; iGroupIndex--)
                {
                    bestFPGA = string.Empty;
                    bestScore = -1.0F;
                    bestAverageUsage = double.MaxValue;

                    FalconMapping_Group fmGrp = lGroups[iGroupIndex];
                    fmGrp.DebugPrintResources();
                    double thisScore;
                    double projectedAverageUsage;

                    if (!fmGrp.IsMapped)
                    {
                        #region Score the group
                        ////score all FPGAs' Available resources
                        for (int iFPGAIndex = 0; iFPGAIndex < lFPGAs.Count; iFPGAIndex++)
                        {
                            FalconMapping_FPGA thisFPGA = lFPGAs[iFPGAIndex];
                            if (fmGrp.SupportedArchitectures == null)   // This group cannot be supported
                            {
                                throw new Exceptions.MappingFailedException(String.Format("Group {0} could not be mapped, as no components share a common supported architecture.", fmGrp.ID));
                            }
                            if ((fmGrp.SupportedArchitectures.Contains(thisFPGA.Architecture)) ||   // ALL member components support the architecture...
                                ((fmGrp.SupportedArchitectures.Count == 1) && (fmGrp.SupportedArchitectures.Contains(string.Empty)))) // OR ALL member components support ANY architecture
                            {
                                thisScore = fmGrp.ScoreFPGA(thisFPGA);
                                projectedAverageUsage = ProjectedAverageLinkUsage(adjMatrix, fmGrp, thisFPGA);

                                //System.Diagnostics.Debug.WriteLine("Group " + fmGrp.ID + ": FPGA " + thisFPGA.ID + " score = " + thisScore.ToString());

                                if (thisScore < 0)
                                {
                                    // This FPGA cannot support this group due to insufficient resources
                                }
                                else if (thisScore > bestScore)
                                {
                                    // Determine how close the difference is
                                    if (bestScore > 0)
                                    {
                                        if (thisScore > bestScore) // New score is better than the previous best
                                        {
                                            // This FPGA is a much tighter fit
                                            // But, if this group is placed here, can it's neighbor components communicate with it.
                                            //FalconMapping_FPGA neighborFPGA = null;
                                            //if (IsMemberPartOfMappedFlow(fmGrp, _Flows, out neighborFPGA))
                                            //{
                                            //if (CanXCommunicateWithY(adjMatrix, thisFPGA, neighborFPGA))
                                            //{
                                            bestScore = thisScore;
                                            bestFPGA = thisFPGA.ID;
                                            //}
                                            //}
                                        }
                                        else if (thisScore >= bestScore) // Check how it will impact the link utilization
                                        {
                                            if (projectedAverageUsage < bestAverageUsage)   // Better link usage
                                            {
                                                //FalconMapping_FPGA neighborFPGA = null;
                                                //if (IsMemberPartOfMappedFlow(fmGrp, _Flows, out neighborFPGA))
                                                //{
                                                //if (CanXCommunicateWithY(adjMatrix, thisFPGA, neighborFPGA))
                                                //{
                                                bestScore = thisScore;
                                                bestFPGA = thisFPGA.ID;
                                                //}
                                                //}
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // This FPGA is the best fit so far
                                        //FalconMapping_FPGA neighborFPGA = null;
                                        //if (_Flows.Count > 0)
                                        //{
                                        //if (IsMemberPartOfMappedFlow(fmGrp, _Flows, out neighborFPGA))
                                        //{
                                        //if (CanXCommunicateWithY(adjMatrix, thisFPGA, neighborFPGA))
                                        //{
                                        bestScore = thisScore;
                                        bestFPGA = thisFPGA.ID;
                                        //}
                                        //}
                                        //}
                                        //else
                                        //{
                                        //    bestScore = thisScore;
                                        //    bestFPGA = thisFPGA.ID;
                                        //}
                                    }
                                }
                                // else // We've already found a better fit
                            }
                        }
                        #endregion

                        //place group on best FPGA
                        if (bestFPGA == string.Empty)
                        {
                            // No fit was found
                        }
                        else
                        {
                            //update space
                            FalconMapping_FPGA targetFPGA = FPGAs[bestFPGA];
                            //targetFPGA.DebugPrintResources();
                            targetFPGA.MapGroup(fmGrp);
                            ApplyGroupConnections(adjMatrix, fmGrp, targetFPGA);        // CONNECTION MANAGEMENT

                            System.Diagnostics.Debug.WriteLine("Mapping Group " + fmGrp.ID + " to FPGA " + targetFPGA.ID);
                            //targetFPGA.DebugPrintResources();
                        }
                    }
                }
                #endregion


                // Print out mappings to DEBUG
                System.Diagnostics.Debug.WriteLine("Mapping Results");
                foreach (FalconMapping_Component Comp in Components.Values)
                {
                    string TargetFPGA = "UNMAPPED!";
                    if (IsComponentMapped(Comp.ID))
                    {
                        TargetFPGA = GetComponentFPGAID(Comp.ID);
                    }
                    System.Diagnostics.Debug.WriteLine(String.Format("Component {0} mapped to {1}", Comp.ID, TargetFPGA));
                }

                #region Verify that all groups have been mapped
                //Verify that all groups have been mapped
                // High score to Low
                for (int iGroupIndex = (lGroups.Count - 1); iGroupIndex >= 0; iGroupIndex--)
                {
                    FalconMapping_Group fmGrp = lGroups[iGroupIndex];
                    if (!fmGrp.IsMapped)
                    {
                        throw new Exceptions.MappingFailedException(
                            String.Format("Mapping failed.  Unable to allocate all component-groups to FPGAs.  First-failure on group {0}",
                            fmGrp.ID));
                    }
                }
                #endregion

                success = true;
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in DoMapping(), During Mapping");
                success = false;
            }
            if (success)
            {
                RaiseMessageEvent("\tComplete!");
            }
            return success;
        }

        /// <summary>
        /// Implementation of mapping algorithm.  Processes components, FPGAs, and Groups to determine how to map all objects
        /// together in the system.
        /// </summary>
        /// <returns>True if mapping was successful, False otherwise.</returns>
        private bool DoMapping_v2()
        {
            RaiseMessageEvent("\n\nStarting Component Map Process...");
            bool success = false;
            try
            {
                #region Ensure that all required components are mapped to their required FPGAs
                EnforcePlatformRequiredMappings();
                #endregion

                #region Ensure that all other components are members of a group and remove all empty groups
                success = GroupUngroupedComponents();
                if (!success)
                {
                    throw new Exceptions.MappingFailedException(
                        String.Format("Mapping failed.  Unable to group all ungrouped components."));
                }
                List<FalconMapping_Group> EmptyGroups = new List<FalconMapping_Group>();
                foreach (FalconMapping_Group Group in Groups.Values)
                {
                    if (Group.GetGroupedComponents().Count == 0)
                        EmptyGroups.Add(Group);
                }
                foreach (FalconMapping_Group EGroup in EmptyGroups)
                {
                    Groups.Remove(EGroup.ID);
                }
                #endregion

                #region Identify Connected FPGA and Component Subsystems
                List<FPGA_Subsystem> FPGA_Systems = new List<FPGA_Subsystem>();
                List<Component_Subsystem> Component_Systems = new List<Component_Subsystem>();
                IdentifySubsystems(ref FPGA_Systems, ref Component_Systems);
                #endregion

                #region Configure FPGA and Group Scoring and Comparison Objects
                Dictionary<string, long> htA = FalconMapping_FPGA.GetAverageFPGAResources(FPGAs);
                FalconGroupComparer gComp = new FalconGroupComparer();
                FalconFPGAComparer fComp = new FalconFPGAComparer();

                gComp.IOWeight = IOWeight;
                gComp.ResourceWeight = 1 - gComp.IOWeight;
                gComp.ComparisonSet = htA;

                fComp.IOWeight = IOWeight;
                fComp.ResourceWeight = 1 - fComp.IOWeight;
                fComp.ComparisonSet = htA;

                CalculateComponentIODistances();
                CalculateFPGAIODistances();
                #endregion

                #region Convert Hashtables to Arrays for Indexing and Sorting
                //Sort_groups(array of groups)
                List<FalconMapping_Group> lGroups = new List<FalconMapping_Group>();
                lGroups.AddRange(Groups.Values);
                FalconMapping_Group.PrintGroupScores(lGroups, gComp);
                lGroups.Sort(gComp);
                FalconMapping_Group.PrintGroupScores(lGroups, gComp);

                List<FalconMapping_FPGA> lFPGAs = new List<FalconMapping_FPGA>();
                lFPGAs.AddRange(FPGAs.Values);
                //FalconMapping_FPGA.PrintFPGAList(lFPGAs, fComp);
                lFPGAs.Sort(fComp);
                FalconMapping_FPGA.PrintFPGAList(lFPGAs, fComp);
                #endregion

                #region Create FPGA Adjacency Matrix
                Object[,] adjMatrix = CreateFPGAAdjacencyMatrix();
                #endregion

                #region Score Groups against FPGAs and ensure that communication remains possible between components
                string bestFPGA = string.Empty;
                double bestScore = -1.0F;
                double bestAverageUsage = -1.0F;
                //For each group in list
                // Low score to High
                for (int iGroupIndex = 0; iGroupIndex < lGroups.Count; iGroupIndex++)
                // High score to Low
                //for (int iGroupIndex = (lGroups.Count - 1); iGroupIndex >= 0; iGroupIndex--)
                {
                    FalconMapping_Group fmGrp = lGroups[iGroupIndex];
                    if (!fmGrp.IsMapped)
                    {
                        bestFPGA = string.Empty;
                        bestScore = -1.0F;
                        bestAverageUsage = double.MaxValue;

                        //fmGrp.DebugPrintResources();
                        double thisScore;
                        double projectedAverageUsage;

                        FPGA_Subsystem fpga_sys = null;
                        Component_Subsystem group_sys = null;

                        #region Identify Group/Component Subsystem
                        // Locate Component Subsystem in which a member of this group is located
                        foreach (Component_Subsystem group_sub in Component_Systems)
                        {
                            if (fmGrp.GetGroupedComponents().Count > 0)
                            {
                                if (group_sub.Members.Contains(fmGrp.GetGroupedComponents()[0]))
                                {
                                    group_sys = group_sub;
                                    break;
                                }
                            }
                        }
                        #endregion

                        #region Evaluate the Group across available FPGAs
                        ////score all FPGAs' Available resources
                        for (int iFPGAIndex = 0; iFPGAIndex < lFPGAs.Count; iFPGAIndex++)
                        {
                            FalconMapping_FPGA thisFPGA = lFPGAs[iFPGAIndex];
                            
                            // Locate FPGA Subsystem in which this FPGA is located
                            foreach (FPGA_Subsystem fpga_sub in FPGA_Systems)
                            {
                                if (fpga_sub.Members.Contains(thisFPGA))
                                {
                                    fpga_sys = fpga_sub;
                                    break;
                                }
                            }
                            // Check if the FPGA subsystem has had this group system allocated already
                            if (!fpga_sys.IsComponentSystemAllocated(group_sys))
                            {
                                // If the component subsystem isn't already allocated to this subsystem
                                // First verify that the entire subsystem COULD support the entire component 
                                // subsystem
                                if (!fpga_sys.CanSupport(group_sys))
                                {
                                    continue;
                                }
                            }
                            // else
                            // If the component subsystem is already allocated to this FPGA subsystem
                            // This FPGA is already a legal target
                            if (fmGrp.SupportedArchitectures == null)   // This group cannot be supported
                            {
                                throw new Exceptions.MappingFailedException(String.Format("Group {0} could not be mapped, as no components share a common supported architecture.", fmGrp.ID));
                            }
                            if ((fmGrp.SupportedArchitectures.Contains(thisFPGA.Architecture)) ||   // ALL member components support the architecture OR ...
                                ((fmGrp.SupportedArchitectures.Count == 1) && (fmGrp.SupportedArchitectures.Contains(string.Empty)))) // ALL member components support ANY architecture
                            {
                                thisScore = fmGrp.ScoreFPGA(thisFPGA);
                                System.Diagnostics.Debug.WriteLine(String.Format("Group {0} scored {1} on FPGA {2}", fmGrp.ID, thisScore, thisFPGA.ID));
                                projectedAverageUsage = ProjectedAverageLinkUsage(adjMatrix, fmGrp, thisFPGA);

                                if (thisScore < 0)
                                {
                                    // This FPGA cannot support this group due to insufficient resources
                                    continue;
                                }
                                else if (thisScore > bestScore) // Check how it will impact the link utilization
                                {
                                    System.Diagnostics.Debug.WriteLine(String.Format("\tGroup {0} is a better fit on FPGA {1} because ({2} > {3})", fmGrp.ID, thisFPGA.ID, thisScore, bestScore));
                                    bestScore = thisScore;
                                    bestFPGA = thisFPGA.ID;
                                    //if (projectedAverageUsage < bestAverageUsage)   // Better link usage
                                    //{
                                    //    bestScore = thisScore;
                                    //    bestFPGA = thisFPGA.ID;
                                    //}
                                }
                                else if (bestScore < 0)
                                {
                                    // This FPGA is the best fit so far
                                    bestScore = thisScore;
                                    bestFPGA = thisFPGA.ID;
                                }
                                // else // We've already found a better fit
                            }
                        }
                        #endregion

                        #region Map the Group, if possible
                        //place group on best FPGA
                        if (bestFPGA == string.Empty)
                        {
                            // No fit was found
                        }
                        else
                        {
                            //update space
                            FalconMapping_FPGA targetFPGA = FPGAs[bestFPGA];

                            // Locate FPGA Subsystem in which this FPGA is located
                            foreach (FPGA_Subsystem fpga_sub in FPGA_Systems)
                            {
                                if (fpga_sub.Members.Contains(targetFPGA))
                                {
                                    fpga_sys = fpga_sub;
                                    break;
                                }
                            }

                            //targetFPGA.DebugPrintResources();
                            targetFPGA.MapGroup(fmGrp);
                            ApplyGroupConnections(adjMatrix, fmGrp, targetFPGA);        // CONNECTION MANAGEMENT
                            fpga_sys.AllocateComponentSystem(group_sys);
                            System.Diagnostics.Debug.WriteLine("Mapping Group " + fmGrp.ID + " to FPGA " + targetFPGA.ID);
                            //targetFPGA.DebugPrintResources();
                        }
                        #endregion
                    }
                }
                #endregion 

                success = true;
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in DoMapping_v2(), During Mapping");
                success = false;
            }
            if (success)
            {
                RaiseMessageEvent("\tComplete!");
            }
            return success;            
        }

        /// <summary>
        /// Verifies and enforces platform-designated component mappings (Required components)
        /// </summary>
        private void EnforcePlatformRequiredMappings()
        {
            foreach (FalconMapping_FPGA FPGA in FPGAs.Values)
            {
                foreach (FalconMapping_Component Component in FPGA.RequiredComponents)
                {
                    // Only Design-Visible Required Components will be added to the global component list
                    // So we force-add them here
                    if (!Components.ContainsKey(Component.ID))
                        Components.Add(Component.ID, Component);

                    if (String.Compare(GetComponentFPGAID(Component.ID), FPGA.ID) != 0)
                    {
                        // The Component is not mapped to the correct FPGA
                        string ReqGroupID = String.Format("required_{0}", FPGA.ID);
                        if (!Groups.ContainsKey(ReqGroupID))
                        {
                            // The group does not exist, create it
                            AddGroup(ReqGroupID, ReqGroupID);
                        }
                        //else
                        //{
                            // The group exists
                        //}

                        // Remove the component from it's existing group, if any
                        if (Component.IsGrouped)
                            RemoveComponentFromGroup(Component.ID);

                        // Add it to the correct group
                        AddComponentToGroup(Component.ID, ReqGroupID);

                        // Is that group already mapped to the FPGA?
                        if (String.Compare(GetGroupTargetFPGAID(ReqGroupID), FPGA.ID) != 0)
                        {
                            // It's not
                            // Unmap the group from it's current FPGA, if any
                            if (IsGroupMapped(ReqGroupID))
                                UnMapGroup(ReqGroupID);
                            // Map it to the correct FPGA
                            MapGroupToFPGA(ReqGroupID, FPGA.ID);
                        }
                    }
                }
            }
        }
        #endregion

        #region I/O Distance and Data/Link Density Calculations
        /// <summary>
        /// Debug Output.  Prints calculated I/O distances for all contained Components to System.Diagnostics.Debug.
        /// </summary>
        private void PrintComponentIODistances()
        {
            try
            {
                foreach (string ComponentID in Components.Keys)
                {
                    FalconMapping_Component Comp = (FalconMapping_Component)Components[ComponentID];
                    System.Diagnostics.Debug.WriteLine("Component " + ComponentID + ": I/O Distance: " + Comp.DistanceFromInput);
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in PrintComponentIODistances()");
            }
        }
        /// <summary>
        /// Debug Output.  Prints calculated I/O distances for all contained FPGAs to System.Diagnostics.Debug.
        /// </summary>
        private void PrintFPGAIODistances()
        {
            try
            {
                foreach (string FPGAID in FPGAs.Keys)
                {
                    FalconMapping_FPGA FPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    System.Diagnostics.Debug.WriteLine("FPGA " + FPGAID + ": I/O Distance: " + FPGA.DistanceFromInput);
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in PrintFPGAIODistances()");
            }
        }
        /// <summary>
        /// Creates a Two-Dimensional adjacency matrix of the FPGAs in the system and outputs its contents to System.Diagnostics.Debug.
        /// </summary>
        private void PrintAdjacencyMatrix()
        {
            Object[,] adjMatrix = CreateFPGAAdjacencyMatrix();
            System.Diagnostics.Debug.WriteLine("-------------------------------");
            for (int row = 0; row < adjMatrix.GetLength(0); row++)
            {
                for (int col = 0; col < adjMatrix.GetLength(1); col++)
                {
                    System.Diagnostics.Debug.Write(String.Format("\t{0}", adjMatrix[row, col]));
                }
                System.Diagnostics.Debug.WriteLine("");
            }
        }
        
        /// <summary>
        /// Parse the connection set and determine, which components are "input components", based on whether they 
        /// have connections into them or not.  Then, given these components as source nodes,
        /// calculate the minimum "hop" distance of each other component from the closest input component.
        /// </summary>
        private void CalculateComponentIODistances()
        {
            int ComponentCount = Components.Count;
            int ProcessedComponentCount = 0;

            // Initialize distances
            foreach (FalconMapping_Component Comp in Components.Values)
            {
                Comp.DistanceFromInput = -1;
            }

            // First find each "input" component
            foreach (string ComponentID in Components.Keys)
            {
                bool bIsInput = false;
                // Test whether the component has ANY component core that does not receive input from the design
                FalconMapping_Component fmc = (FalconMapping_Component)Components[ComponentID];
                foreach(ComponentCore CompCore in fmc.ComponentCores.Values)
                {
                    if (CompCore.InterfaceType == VortexInterfaces.VortexCommon.VortexAttachmentType.None)
                        continue;

                    bIsInput = true;
                    foreach (string ConnectionID in Connections.Keys)
                    {
                        FalconMapping_Connection fmconn = (FalconMapping_Connection)(Connections[ConnectionID]);
                        if ((fmconn.SinkComponent == fmc.ID) && 
                            (fmconn.SinkComponentInstance == CompCore.NativeInstance))
                        {
                            // This component draws from an input source OTHER than INPUT
                            bIsInput = false;
                            break;
                        }
                    }
                    if (bIsInput)
                        break;
                }
                if (bIsInput)
                {
                    fmc.DistanceFromInput = 0;
                    ProcessedComponentCount++;
                }
                else
                {
                    fmc.DistanceFromInput = -1;
                }
            }

            if (ProcessedComponentCount == 0)
            {
                // There are no Components that can be tagged as Input
                // Set each one to 0 distance from I/O
                foreach (string ComponentID in Components.Keys)
                {
                    FalconMapping_Component thisComponent = (FalconMapping_Component)Components[ComponentID];
                    thisComponent.DistanceFromInput = 0;
                    ProcessedComponentCount++;
                }
            }

            // Handle case where a component has no explicit connections to other components
            // Identify those components and count them as processed
            foreach (FalconMapping_Component Comp in Components.Values)
            {
                bool bConnected = false;
                foreach (FalconMapping_Connection Conn in Connections.Values)
                {
                    if ((String.Compare(Comp.ID, Conn.SourceComponent) == 0) ||
                        (String.Compare(Comp.ID, Conn.SinkComponent) == 0))
                    {
                        bConnected = true;
                        break;
                    }
                }
                if ((!bConnected) && (Comp.DistanceFromInput < 0))
                {
                    Comp.DistanceFromInput = 0;
                    ProcessedComponentCount++;
                }
            }
            while (ProcessedComponentCount < ComponentCount)
            {
                foreach (string ComponentID in Components.Keys)
                {
                    FalconMapping_Component thisComponent = (FalconMapping_Component)Components[ComponentID];
                    foreach (string ConnectionID in Connections.Keys)
                    {
                        FalconMapping_Connection conn = (FalconMapping_Connection)(Connections[ConnectionID]);
                        if (conn.SinkComponent == ComponentID)      // Will need to update this to (SourceComponent != INPUT && SinkComponent == ComponentID)
                        {
                            if (Components.ContainsKey(conn.SourceComponent))
                            {
                                FalconMapping_Component srcComponent = (FalconMapping_Component)Components[conn.SourceComponent];
                                if (srcComponent.DistanceFromInput >= 0)
                                    if ((thisComponent.DistanceFromInput < 0) || (thisComponent.DistanceFromInput > (srcComponent.DistanceFromInput + 1)))
                                    {
                                        thisComponent.DistanceFromInput = srcComponent.DistanceFromInput + 1;
                                        ProcessedComponentCount++;
                                    }
                            }
                            else
                            {
                                // Unknown Source Component???
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Parse the link set and determine, which FPGAs are "input FGPA", based on whether they 
        /// have links into them or not.  Then, given these FPGAs as source nodes,
        /// calculate the minimum "hop" distance of each other FPGA from the closest input FPGA.
        /// </summary>
        private void CalculateFPGAIODistances()
        {
            int FPGACount = FPGAs.Count;
            int ProcessedFPGACount = 0;

            // First find each "input" FPGA
            foreach (string FPGAID in FPGAs.Keys)
            {
                bool bIsInput = true;
                foreach (string ConnectionID in Links.Keys)
                {
                    FalconMapping_Link fmlink = (FalconMapping_Link)(Links[ConnectionID]);
                    // Maybe implement FPGA Property IsInput()???
                    if (fmlink.SinkFPGA == FPGAID)      // Will need to update this to (SourceFPGA != INPUT && SinkFPGA == FPGAID)
                    {
                        // This FPGA draws from an input source OTHER than INPUT
                        bIsInput = false;
                        break;
                    }
                }
                FalconMapping_FPGA fmf = (FalconMapping_FPGA)FPGAs[FPGAID];
                if (bIsInput)
                {
                    fmf.DistanceFromInput = 0;
                    ProcessedFPGACount++;
                    //System.Diagnostics.Debug.WriteLine("FPGA " + fmf.ID + " is " + fmf.DistanceFromInput + " hops from I/O");
                }
                else
                {
                    fmf.DistanceFromInput = -1;
                }
            }
            if (ProcessedFPGACount == 0)
            {
                // There are no FPGAs that are tagged as Input
                // Set each one to 0 distance from I/O
                foreach (string FPGAID in FPGAs.Keys)
                {
                    FalconMapping_FPGA thisFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    thisFPGA.DistanceFromInput = 0;
                    ProcessedFPGACount++;
                }
            }
            while (ProcessedFPGACount < FPGACount)
            {
                foreach (string FPGAID in FPGAs.Keys)
                {
                    FalconMapping_FPGA thisFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    foreach (string ConnectionID in Links.Keys)
                    {
                        FalconMapping_Link link = (FalconMapping_Link)(Links[ConnectionID]);
                        if (link.SinkFPGA == FPGAID)      // Will need to update this to (SourceFPGA != INPUT && SinkFPGA == FPGAID)
                        {
                            if (FPGAs.ContainsKey(link.SourceFPGA))
                            {
                                FalconMapping_FPGA srcFPGA = (FalconMapping_FPGA)FPGAs[link.SourceFPGA];
                                if (srcFPGA.DistanceFromInput >= 0)
                                    if ((thisFPGA.DistanceFromInput < 0) || (thisFPGA.DistanceFromInput > (srcFPGA.DistanceFromInput + 1)))
                                    {
                                        thisFPGA.DistanceFromInput = srcFPGA.DistanceFromInput + 1;
                                        ProcessedFPGACount++;
                                        //System.Diagnostics.Debug.WriteLine("FPGA " + thisFPGA.ID + " is " + thisFPGA.DistanceFromInput + " hops from I/O");
                                    }
                            }
                            else
                            {
                                // Unknown Source FPGA???
                            }
                        }
                    }
                }
            }
        }        

        /// <summary>
        /// Normalizes all logical inter-component connection data densities to the least dense connection
        /// in the system.
        /// </summary>
        private void NormalizeConnectionDensities()
        {            
            double minDensity = 65536.0F;
            foreach (string connID in Connections.Keys)
            {
                FalconMapping_Connection conn = (FalconMapping_Connection)Connections[connID];
                if (conn.DataDensity < minDensity)
                    minDensity = conn.DataDensity;
            }
            foreach (string connID in Connections.Keys)
            {
                FalconMapping_Connection conn = (FalconMapping_Connection)Connections[connID];
                conn.NormalizeDataDensityTo(minDensity);
            }
        }
        /// <summary>
        /// Normalizes all physical inter-FPGA link speeds to the fastest speed link in the system.
        /// </summary>
        private void NormalizeLinkSpeeds()
        {
            double maxSpeed = -65536.0F;
            foreach (string linkID in Links.Keys)
            {
                FalconMapping_Link link = (FalconMapping_Link)Links[linkID];
                if (link.LinkSpeed > maxSpeed)
                    maxSpeed = link.LinkSpeed;
            }
            foreach (string linkID in Links.Keys)
            {
                FalconMapping_Link link = (FalconMapping_Link)Links[linkID];
                link.NormalizeLinkSpeedTo(maxSpeed);
            }
        }

        /// <summary>
        /// Creates a Two-Dimensional Array representing the hop-count between FPGAs in the system.  The first column of the array
        /// lists the FPGA IDs as the source FPGAs.  The first row lists the same FPGA IDs, in the same order, as the Sink FPGAs.   
        /// The first element, the pivot corner, contains a null value.   The remainder of the array cell[row, col] (row > 0, col > 0) 
        /// contains the number of hops required to reach the FPGA with ID cell[0, col] from the FPGA with ID cell[row, 0]
        /// </summary>
        /// <returns>Returns a Two-Dimensional adjacency matrix array with header row and column.</returns>
        private Object [,] CreateFPGAAdjacencyMatrix()
        {
            //            Sink... 
            // | Source         
            // | ...

            #region Matrix Initialization
            int rcCount = (FPGAs.Count + 1);
            int row, col;
            #endregion

            Object[,] adjMatrix = new Object[rcCount, rcCount];

            #region Header Row
            // Create the headerRow
            col = 1;
            adjMatrix[0, 0] = " ";
            foreach (string FPGAID in FPGAs.Keys)
            {
                adjMatrix[0, col] = FPGAID;
                col++;
            }
            #endregion 

            #region Leading Column + Empty Matrix 
            // Create the other Rows
            // Create Empty Adjacency Matrix
            for (row = 1; row < rcCount; row++)
            {
                for (col = 0; col < rcCount; col++)
                {
                    if (col == 0)
                        adjMatrix[row, col] = adjMatrix[0, row];
                    else if (row == col)
                        adjMatrix[row, col] = 0;
                    else
                        adjMatrix[row, col] = int.MaxValue;
                }
            }
            #endregion

            #region Fill in Base (Single-Hop) Links
            // Add Links
            for (row = 1; row < rcCount; row++)
            {
                for (col = 1; col < rcCount; col++)
                {
                    string srcID = adjMatrix[row, 0].ToString();
                    string sinkID = adjMatrix[0, col].ToString();

                    foreach (string LinkID in Links.Keys)
                    {
                        FalconMapping_Link link = (FalconMapping_Link)Links[LinkID];
                        if (((link.SourceFPGA == srcID) && (link.SinkFPGA == sinkID)) ||
                              ((link.SourceFPGA == sinkID) && (link.SinkFPGA == srcID) && (link.Bidirectional)))
                        {
                            adjMatrix[row, col] = 1;
                        }
                    }

                }
            }
            #endregion

            #region Use Floyd-Warshall Algorithm to complete the adjacency Matrix
            // Calculate FPGA Hop-Distance Using Floyd-Warshall's Algorithm
            for (int k = 1; k < rcCount; k++)
            {
                for (int i = 1; i < rcCount; i++)
                {
                    for (int j = 1; j < rcCount; j++)
                    {
                        int sum = (int)adjMatrix[i, k] + (int)adjMatrix[k, j];
                        if (sum >= 0)
                        {
                            adjMatrix[i, j] = Math.Min((int)adjMatrix[i, j], sum);
                        }
                    }
                }
            }
            #endregion

            // Set all MaxValue distances to -1
            for (int k = 1; k < rcCount; k++)
            {
                for (int i = 1; i < rcCount; i++)
                {
                    if (((int)adjMatrix[k, i]) == int.MaxValue)
                    {
                        adjMatrix[k, i] = -1;
                    }
                }
            }
            return adjMatrix;
        }

        /// <summary>
        /// Determines whether a path exists between SourceID and SinkID, given the adjacency matrix aMatrix.
        /// </summary>
        /// <param name="aMatrix">A two-dimensional matrix, with column and row headers, generated by CreateFPGAAdjacencyMatrix().</param>
        /// <param name="SourceID">The ID of the FPGA that is acting as the source for this comparison.</param>
        /// <param name="SinkID">The ID of the FPGA that is acting as the sink for this comparison.</param>
        /// <returns>True if a path exists, based on the matrix(i.e. hop count is greater than or equal to 0 and less than int.MaxValue).  False otherwise.</returns>
        private bool LinkPathExists(Object[,] aMatrix, string SourceID, string SinkID)
        {
            int row = -1;
            int col = -1;

            // Locate the Sink Column by traversing the top row...
            for (int i = 0; i < aMatrix.GetLength(0); i++)
            {
                if (FalconMapping_XML.StringsEqual((string)(aMatrix[0, i]), SinkID))
                {
                    col = i;
                    break;
                }
            }

            // Locate the Source Row by traversing the first column...
            for (int j = 0; j < aMatrix.GetLength(1); j++)
            {
                if (FalconMapping_XML.StringsEqual((string)(aMatrix[j, 0]), SourceID))
                {
                    row = j;
                    break;
                }
            }

            // Index the Matrix for the hop count...
            int hopCt = -1;
            if ((row >= 0) && (col >= 0))
            {
                hopCt = (int)aMatrix[row, col];
            }

            // If the hop count is greater than or equal to 0 and less than int.MaxValue, then a path exists.
            return ((hopCt >= 0) && (hopCt < int.MaxValue));
        }

        /// <summary>
        /// Determines if FPGA F1 is able to communicate with FPGA F2, given the adjacency matrix, Matrix.
        /// </summary>
        /// <param name="Matrix">The adjacency matrix calculated from the platform topology.</param>
        /// <param name="F1">The source FPGA.</param>
        /// <param name="F2">The target FPGA.</param>
        /// <returns>True if F1 can communicate with F2.</returns>
        private bool CanXCommunicateWithY(Object[,] Matrix, FalconMapping_FPGA F1, FalconMapping_FPGA F2)
        {
            return LinkPathExists(Matrix, F1.ID, F2.ID);
        }

        /// <summary>
        /// Determines whether the components of a group would be visible/accessible to all of their connected neighbors if the group were to be mapped to the specified FPGA.
        /// </summary>
        /// <param name="Matrix">The adjacency matrix generated based on the FPGA interconnects/links.</param>
        /// <param name="grp">The group that is being tested for placement.</param>
        /// <param name="fpga">The FPGA that is being tested for placement.</param>
        /// <returns>True if all component-members of the group would be able to communicate with their neighbors, if the entire group were placed on the specified FPGA.</returns>
        private bool GroupMembersVisibleToNeighbors(Object[,] Matrix, FalconMapping_Group grp, FalconMapping_FPGA fpga)
        {
            //For each component in 'grp'...
            foreach (FalconMapping_Component cmp in grp.GetGroupedComponents())
            {
                // If this component is placed on 'fpga', can it still talk to its neighbors
                // Determined by checking each connection
                foreach (FalconMapping_Connection conn in Connections.Values)
                {
                    bool bCheck = false;
                    string srcID = string.Empty;
                    string sinkID = string.Empty;

                    FalconMapping_Component otherCmp = null;
                    if (conn.SourceComponent == cmp.ID)
                    {
                        // This component outputs to another component (the sink)
                        otherCmp = Components[conn.SinkComponent];
                    }
                    else if (conn.SinkComponent == cmp.ID)
                    {
                        // This component outputs receives input from another component (the source)
                        otherCmp = Components[conn.SourceComponent];
                    }
                    else
                    {
                        // This component is not involed in this connection, skip it.
                        continue;
                    }

                    // Is the other component grouped?
                    if (otherCmp.IsGrouped)
                    {
                        // Other Component is in the same group, so it's good.
                        if (otherCmp.GroupID == grp.ID)
                            continue;
                        // Other Component is in another group, is that group mapped?
                        FalconMapping_Group otherGrp = Groups[otherCmp.GroupID];
                        if (otherGrp.IsMapped)
                        {
                            // It is mapped, set the source/sink IDs for checking                            
                            if (conn.SourceComponent == cmp.ID)
                            {
                                srcID = fpga.ID;                // The FPGA we're sending from (the one we're checking)
                                sinkID = otherGrp.TargetFPGA;   // The FPGA we're sending to (the one that's fixed)
                                bCheck = true;
                            }
                            else if (conn.SinkComponent == cmp.ID)
                            {
                                srcID = otherGrp.TargetFPGA;    // The FPGA we're sending from (the one we're checking)
                                sinkID = fpga.ID;               // The FPGA we're sending to (the one that's fixed)
                                bCheck = true;
                            }
                        }
                        else
                        {
                            // The other group isn't mapped yet, so we don't need to check it.   
                            // It will be checked against this group later when it's time to map it.
                            continue;
                        }
                    }
                    else
                    {
                        // Should never happen -- all components must be grouped
                        return false;
                    }

                    if (bCheck)
                    {
                        // We need to check this one, 
                        if (!LinkPathExists(Matrix, srcID, sinkID))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Calculates the average link usage across all links in the system.
        /// </summary>
        /// <returns></returns>
        private double AverageLinkUsage()
        {
            double total = 0.0F;
            int linkCt = 0;
            foreach (string linkID in Links.Keys)
            {
                FalconMapping_Link link = (FalconMapping_Link)Links[linkID];
                total = total + link.LinkUsage;
                linkCt++;
            }
            double avg = 0.0F;
            if (linkCt > 0)
            {
                avg = total / linkCt;
            }
            return avg;
        }

        /// <summary>
        /// Applies any connections corresponding to members of the specified group to the relevant links using Djikstra's algorithm as a basis for calculating paths.
        /// </summary>
        /// <param name="Matrix">The adjacency matrix generated based on the FPGA interconnects/links.</param>
        /// <param name="grp">The group whose component-connections are to be applied to the syst.</param>
        /// <param name="fpga">The FPGA on which the group was mapped.</param>
        private void ApplyGroupConnections(Object[,] Matrix, FalconMapping_Group grp, FalconMapping_FPGA fpga)
        {
            ArrayList AppliedConnections;
            ArrayList AppliedLinkPaths;
            ApplyGrpConnections(Matrix, grp, fpga, out AppliedConnections, out AppliedLinkPaths);
        }
        /// <summary>
        /// Applies any connections corresponding to members of the specified group to the relevant links using Djikstra's algorithm as a basis for calculating paths.
        /// </summary>
        /// <param name="Matrix">The adjacency matrix generated based on the FPGA interconnects/links.</param>
        /// <param name="grp">The group that is being tested for impact on the system.</param>
        /// <param name="fpga">The FPGA that is being tested for impact on the system.</param>
        /// <param name="AppliedConnections">(out) The set of connections that were applied to the link paths.  This list is ordered, with each index corresponding to the same index in AppliedLinkPaths.</param>
        /// <param name="AppliedLinkPaths">(out) The set of lists-of-links that were applied to with a corresponding connection.  This list is ordered, with each index corresponding to the same index in AppliedConnections.</param>
        private void ApplyGrpConnections(Object[,] Matrix, FalconMapping_Group grp, FalconMapping_FPGA fpga, out ArrayList AppliedConnections, out ArrayList AppliedLinkPaths)
        {
            AppliedConnections = new ArrayList();
            AppliedLinkPaths = new ArrayList();

            //For each component in 'grp'...
            foreach (object o in grp.GetGroupedComponents())
            {
                FalconMapping_Component cmp = (FalconMapping_Component)o;

                // If this component is placed on 'fpga', can it still talk to its neighbors
                // Determined by checking each connection
                foreach (string ConnID in Connections.Keys)
                {
                    bool bApply = false;
                    string srcID = string.Empty;
                    string sinkID = string.Empty;

                    FalconMapping_Connection conn = (FalconMapping_Connection)Connections[ConnID];
                    if (AppliedConnections.Contains(conn))
                        continue;

                    FalconMapping_Component otherCmp = null;
                    if (conn.SourceComponent == cmp.ID)
                    {
                        // This component outputs to another component (the sink)
                        otherCmp = (FalconMapping_Component)Components[conn.SinkComponent];
                    }
                    else if (conn.SinkComponent == cmp.ID)
                    {
                        // This component outputs receives input from another component (the source)
                        otherCmp = (FalconMapping_Component)Components[conn.SourceComponent];
                    }
                    else
                    {
                        // This component is not involed in this connection, skip it.
                        continue;
                    }

                    // Is the other component grouped?
                    if (otherCmp.IsGrouped)
                    {
                        // Other Component is in the same group, so it's good.
                        if (otherCmp.GroupID == grp.ID)
                            continue;
                        // Other Component is in another group, is that group mapped?
                        FalconMapping_Group otherGrp = (FalconMapping_Group)Groups[otherCmp.GroupID];
                        if (otherGrp.IsMapped)
                        {
                            // It is mapped, set the source/sink IDs for checking                            
                            if (conn.SourceComponent == cmp.ID)
                            {
                                srcID = fpga.ID;                // The FPGA we're sending from
                                sinkID = otherGrp.TargetFPGA;   // The FPGA we're sending to
                                bApply = true;
                            }
                            else if (conn.SinkComponent == cmp.ID)
                            {
                                srcID = otherGrp.TargetFPGA;    // The FPGA we're sending from
                                sinkID = fpga.ID;               // The FPGA we're sending to
                                bApply = true;
                            }
                        }
                        else
                        {
                            // The other group isn't mapped yet, so we don't need to check it.   
                            // It will be checked against this group later when it's time to map it.
                            continue;
                        }
                    }
                    else
                    {
                        // Should never happen -- all components must be grouped
                        throw new Exceptions.ComponentNotGroupedException(
                            String.Format("Component {0} is not grouped.  Unable to plot connection links for usage calculations.",
                            otherCmp.ID));
                    }

                    if (bApply)
                    {
                        if (LinkPathExists(Matrix, srcID, sinkID))
                        {
                            List<FalconMapping_Link> path = ApplyConnectionOnPath(conn, srcID, sinkID);
                            AppliedConnections.Add(conn);
                            AppliedLinkPaths.Add(path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the projected average link usage if the specified group were mapped to the specified FPGA.
        /// </summary>
        /// <param name="Matrix">The adjacency matrix generated based on the FPGA interconnects/links.</param>
        /// <param name="grp">The group that is being tested for impact on the system.</param>
        /// <param name="fpga">The FPGA that is being tested for impact on the system.</param>
        /// <returns>The projected average usage of all links in the system, given that the specified group is mapped to the specified FPGA.</returns>
        private double ProjectedAverageLinkUsage(Object[,] Matrix, FalconMapping_Group grp, FalconMapping_FPGA fpga)
        {
            ArrayList AppliedConnections;
            ArrayList AppliedLinkPaths;

            ApplyGrpConnections(Matrix, grp, fpga, out AppliedConnections, out AppliedLinkPaths);

            // Calculate the average
            double avg = AverageLinkUsage();

            // Now remove the connections that have been applied
            if (AppliedConnections.Count != AppliedLinkPaths.Count)
                throw new Exceptions.LinkUsageCalculationException(
                    String.Format("Unable to plot connection links for usage calculations."));

            for (int i = 0; i < AppliedConnections.Count; i++)
            {
                RemoveConnectionOnPath((FalconMapping_Connection)AppliedConnections[i], (List<FalconMapping_Link>)AppliedLinkPaths[i]);
            }
            return avg;
        }

        /// <summary>
        /// Applies the specified connection on a path between FPGA IDs sourceID and sinkID.   The path chosen is determined, ignorant of other connections,
        /// but uses a naive approach with Djikstra's Algorithm.
        /// </summary>
        /// <param name="conn">The connection to be applied.</param>
        /// <param name="sourceID">The ID of the FPGA with the component from which the connection originates.</param>
        /// <param name="sinkID">The ID of the FPGA with the component from which the connection terminates.</param>
        /// <returns>A list of FalconMapping_Link objects that are traversed in order to go from sourceID to sinkID, and which have had the connection applied 
        /// their usage.</returns>
        private List<FalconMapping_Link> ApplyConnectionOnPath(FalconMapping_Connection conn, string sourceID, string sinkID)
        {
            FalconMapping_FPGA srcFPGA = (FalconMapping_FPGA)FPGAs[sourceID];
            FalconMapping_FPGA sinkFPGA = (FalconMapping_FPGA)FPGAs[sinkID];

            List<FalconMapping_Link> links = new List<FalconMapping_Link>();
            links = LinksOnPath(sourceID, sinkID, links);
            foreach (FalconMapping_Link link in links)
            {
                link.AddConnection(conn);
            }
            return links;

        }
        /// <summary>
        /// Removes a connection from the usage of links on the specified path.
        /// </summary>
        /// <param name="conn">The connection to be removed.</param>
        /// <param name="links">The list of links from which the connection is to be removed.  This list is generated by ApplyConnectionOnPath().</param>
        private void RemoveConnectionOnPath(FalconMapping_Connection conn, List<FalconMapping_Link> links)
        {
            foreach (FalconMapping_Link link in links)
            {
                link.RemoveConnection(conn);
            }
        }
        /// <summary>
        /// Recursive implementation of Djikstra that approximates the path that will be taken by a connection, given the links available.
        /// </summary>
        /// <param name="SourceID">The ID of the FPGA with the component from which the connection originates.</param>
        /// <param name="SinkID">The ID of the FPGA with the component from which the connection terminates.</param>
        /// <param name="links">The set of links that have successfully been added to the path.   If a dead end is reached, this list is discarded at a previous 
        /// call in the recursive stack.</param>
        /// <returns>A list of FalconMapping_Links that represent the shortest (by hop count) path from SourceID to SinkID, if one exists.</returns>
        private List<FalconMapping_Link> LinksOnPath(string SourceID, string SinkID, List<FalconMapping_Link> links)
        {
            if (SourceID == SinkID)
            {
                return links;
            }
            
            List<FalconMapping_Link> tempLinks = new List<FalconMapping_Link>();
            List<FalconMapping_Link> savedLinks = null;
            foreach (string linkID in Links.Keys)
            {
                FalconMapping_Link link = (FalconMapping_Link)Links[linkID];
                if (links.Contains(link))
                    continue;

                if (((link.SourceFPGA == SourceID) && (link.SinkFPGA == SinkID)) ||                       // Forward path
                    ((link.SourceFPGA == SinkID) && (link.SinkFPGA == SourceID) && (link.Bidirectional))) // Reverse path
                {
                    // Last link in the chain
                    links.Add(link);
                    return links;
                }
                else if ((link.SourceFPGA == SourceID) ||                     // Forward path
                         (link.SinkFPGA == SourceID) && (link.Bidirectional)) // Reverse path
                {
                    // Not the last link, but maybe a link?
                    links.Add(link);
                    tempLinks = LinksOnPath(((link.SourceFPGA == SourceID) ? link.SinkFPGA : link.SourceFPGA), SinkID, links);
                    if (tempLinks.Count == 0)
                    {
                        links.Remove(link);
                        return tempLinks;
                    }
                    if ((savedLinks == null) || (tempLinks.Count < savedLinks.Count))
                    {
                        savedLinks = tempLinks;
                    }

                }
            }
            if (savedLinks == null)
                savedLinks = new List<FalconMapping_Link>();
            return savedLinks;
        }

        #endregion

        #region Mapping Algorithm Input, Output, and Reporting Methods

        /// <summary>
        /// Loads an XML file specifying the design to be synthesized.
        /// </summary>
        /// <param name="PathFile">The path to the XML file specifying the project paths.</param>
        /// <returns>True if the loading was successful, False otherwise.</returns>
        public bool LoadPathsFile(string PathFile)
        {
            try
            {
                PathMan = new PathManager(PathFile);
                RaiseMessageEvent("Loaded Paths from: {0}", PathFile);
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "LoadPathsFile()");
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Generates a component routing table output specification
        /// </summary>
        /// <param name="OutputPath">The path to the routing table output.</param>
        public void GenerateRouting(string OutputPath)
        {
            //DetectFlows();
            string baseFile = "maptool_routing_temp_file";
            string tempFile = baseFile + ".xml";
            while (File.Exists(tempFile))
                tempFile = baseFile + new Random().Next().ToString() + ".xml";
            this.WriteRoutingFile(tempFile);

            // Create Routing graph builder
            FalconGraphBuilder<string> fgb = new FalconGraphBuilder<string>(tempFile);
            fgb.GenerateTables(OutputPath);
            Console.WriteLine("Saved Routing Table Configuration to: {0}", OutputPath);
            try
            {
                File.Delete(tempFile);
            }
            catch
            { }
        }

        /// <summary>
        /// Reads information from the Platform, Design, and Communications Specifications files provided as properties.   
        /// If any of the files does not exist, the function returns false indicating it was unsuccessful.  
        /// </summary>
        /// <returns>Returns true if all three files exist, were read and successfully combined as mapping input.  False otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Cerebrum Project Files)</seealso>
        public bool ReadProjectFiles(string DesignFile, string CommunicationsFile)
        {
            try
            {
                string PlatformFile = String.Format(@"{0}\{1}\{1}.xml", PathMan["Platforms"], PathMan["ProjectPlatform"]);
                if ((!File.Exists(DesignFile)) && (!DesignFile.Contains("\\")))
                    DesignFile = PathMan.GetPath("LocalProjectRoot") + "\\" + DesignFile;
                if ((!File.Exists(CommunicationsFile)) && (!CommunicationsFile.Contains("\\")))
                    CommunicationsFile = PathMan.GetPath("LocalProjectRoot") + "\\" + CommunicationsFile;

                if (!File.Exists(PlatformFile))
                    return false;
                if (!File.Exists(DesignFile))
                    return false;
                if (!File.Exists(CommunicationsFile))
                    return false;

                bool result = FalconMapping_XML.ParseProjectFiles(PlatformFile, DesignFile, CommunicationsFile, this);

                this.RaiseMessageEvent("Finished processing project files.");
                //ValidateClockAssignments();
                return result;
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in ReadPDCFiles()");
                return false;
            }
        }

        /// <summary>
        /// Reads in the system specification from the specified XML file.
        /// </summary>
        /// <param name="InputFile">The path to the XML file.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Component Mapping, System)</seealso>
        public bool ReadSystemFile(string InputFile)
        {
            try
            {
                FalconMapping_XML.ReadSystemFile(InputFile, this);
                CalculateComponentIODistances();
                CalculateFPGAIODistances();
                return true;
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in ReadSystemFile(" + InputFile + ")");
            }
            return false;
        }

        /// <summary>
        /// Writes out the system specification to the specified XML file.
        /// </summary>
        /// <param name="OutputFile">The path to the XML file.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Component Mapping, System)</seealso>
        public bool WriteSystemFile(string OutputFile)
        {            
            try
            {
                FalconMapping_XML.WriteSystemFile(OutputFile, this, true);
                return true;
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in WriteSystemFile(" + OutputFile + ")");
            }
            return false;
        }


        /// <summary>
        /// Reads in the Pre- and Post-Mapping specification from the specified XML file.
        /// </summary>
        /// <param name="InputFile">The path to the XML file.</param>
        /// <param name="ReadPreMapping">Indicates whether to look for the PreMapping tag (true) or the PostMapping tag (false)</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Component Mapping, Mapping)</seealso>
        public bool ReadMappingFile(string InputFile,
                                bool ReadPreMapping)
        {
            try
            {
                FalconMapping_XML.ReadMappingFile(InputFile, this, ReadPreMapping);
                return true;
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in ReadMappingFile( " + InputFile + ", " + ReadPreMapping.ToString() + ")");
            }
            return false;
        }

        /// <summary>
        /// Writes out the Pre- and Post-Mapping specification to the specified XML file.
        /// </summary>
        /// <param name="OutputFile">The path to the XML file.</param>
        /// <param name="WritePreMapping">Indicates whether to write the PreMapping tag (true) or the PostMapping tag (false)</param>
        /// <param name="AppendIfFileExists">If true, the mapping specification will be appended to the file, if it exists.  If false, 
        /// the file will be overwritten.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Component Mapping, Mapping)</seealso>
        public bool WriteMappingFile(string OutputFile,
                                    bool WritePreMapping, 
                                    bool AppendIfFileExists)
        {
            try
            {
                FalconMapping_XML.WriteMappingFile(OutputFile, this, WritePreMapping, AppendIfFileExists);
                return true;
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in WriteMappingFile( " + OutputFile + ", " + WritePreMapping.ToString() + ")");
            }
            return false;
        }
        
        /// <summary>
        /// Generate XML file for routing table generation.  The output file includes the following information for each connection:
        /// Source Component, Sink Component, Source FPGA, Sink FPGA, and the normalized connection weight as specified in the mapping file.
        /// The file will also contain a list of FPGAs in the system, along with the set of links connecting them.
        /// </summary>
        /// <param name="OutputFile">The path to the XML file.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Component Mapping, Routing Table)</seealso>
        public bool WriteRoutingFile(string OutputFile)
        {
            try
            {
                if (!AllComponentsMapped())
                    return false;

                XmlDocument XDoc = new XmlDocument();
                XDoc.AppendChild(XDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement XERoot = XDoc.CreateElement("SystemRouting");
                FalconFileRoutines.WriteCerebrumDisclaimerXml(XERoot);    // Added by Matthew Cotter 8/18/2010
                XDoc.AppendChild(XERoot);

                XmlElement XEFPGAs = XDoc.CreateElement("Nodes");
                #region Extracted from FalconMapping_XML.WriteFPGAs()
                foreach (string fID in FPGAs.Keys)
                {
                    FalconMapping_FPGA fmf = (FalconMapping_FPGA)FPGAs[fID];
                    XmlElement XFPGA = XDoc.CreateElement("Node");
                    XFPGA.SetAttribute("Name", fmf.Name);
                    XFPGA.SetAttribute("ID", fmf.ID);
                    XEFPGAs.AppendChild(XFPGA);
                }
                #endregion
                XERoot.AppendChild(XEFPGAs);


                XmlElement XECC = XDoc.CreateElement("ComponentConnections");
                #region Write Connections (Source/Sink Component, Source/Sink FPGA, Weight)
                foreach (string ConnID in Connections.Keys)
                {
                    FalconMapping_Connection conn = (FalconMapping_Connection)Connections[ConnID];
                    string srcCompID = conn.SourceComponent;
                    string sinkCompID = conn.SinkComponent;
                    if (Components.ContainsKey(srcCompID) && Components.ContainsKey(sinkCompID))
                    {
                        FalconMapping_Component srcComp = (FalconMapping_Component)Components[srcCompID];
                        FalconMapping_Component sinkComp = (FalconMapping_Component)Components[sinkCompID];
                        if (srcComp.IsGrouped && sinkComp.IsGrouped)
                        {
                            string srcGroupID = srcComp.GroupID;
                            string sinkGroupID = sinkComp.GroupID;
                            if (Groups.ContainsKey(srcGroupID) && Groups.ContainsKey(sinkGroupID))
                            {
                                FalconMapping_Group srcGroup = (FalconMapping_Group)Groups[srcGroupID];
                                FalconMapping_Group sinkGroup = (FalconMapping_Group)Groups[sinkGroupID];
                                if (srcGroup.IsMapped && sinkGroup.IsMapped)
                                {
                                    string srcFPGAID = srcGroup.TargetFPGA;
                                    string sinkFPGAID = sinkGroup.TargetFPGA;
                                    if (FPGAs.ContainsKey(srcFPGAID) && FPGAs.ContainsKey(sinkFPGAID))
                                    {
                                        FalconMapping_FPGA srcFPGA = (FalconMapping_FPGA)FPGAs[srcFPGAID];
                                        FalconMapping_FPGA sinkFPGA = (FalconMapping_FPGA)FPGAs[sinkFPGAID];

                                        // srcCompID of srcGroupID, mapped to srcFPGAID, is connected to sinkCompID of sinkGroupID, mapped to sinkFPGAID
                                        XmlElement XENode = XDoc.CreateElement("ConnectionInfo");
                                        XENode.SetAttribute("SourceComponent", srcCompID);
                                        XENode.SetAttribute("SinkComponent", sinkCompID);
                                        XENode.SetAttribute("SourceNode", srcFPGAID);
                                        XENode.SetAttribute("SinkNode", sinkFPGAID);
                                        XENode.SetAttribute("Weight", conn.NormalizedDataDensity.ToString("##0.000"));
                                        XECC.AppendChild(XENode);
                                    }
                                    else // An FPGA doesn't exist
                                    {
                                        return false;
                                    }
                                }
                                else // A Group isn't mapped
                                {
                                    return false;
                                }
                            }
                            else // A group doesn't exist
                            {
                                return false;
                            }
                        }
                        else // A component isn't grouped
                        {
                            return false;
                        }
                    }
                    else // Either source or sink component doesn't exist
                    {
                        return false;
                    }
                }
                #endregion
                XERoot.AppendChild(XECC);

                XmlElement XELinks = XDoc.CreateElement("NodeLinks");
                #region Extracted from FalconMapping_XML.WriteLinks()
                foreach (string cID in Links.Keys)
                {
                    FalconMapping_Link fml = (FalconMapping_Link)Links[cID];
                    XmlElement XLink = XDoc.CreateElement("LinkInfo");
                    XLink.SetAttribute("SourceNode", fml.SourceFPGA);
                    XLink.SetAttribute("SinkNode", fml.SinkFPGA);
                    XLink.SetAttribute("LinkSpeed", fml.LinkSpeed.ToString("#0.000"));
                    XLink.SetAttribute("Bidirectional", fml.Bidirectional.ToString());
                    XELinks.AppendChild(XLink);
                }
                #endregion
                XERoot.AppendChild(XELinks);
                
                XDoc.Save(OutputFile);
                //RaiseMessageEvent("Saved Hardware Routing Component-Map to: {0}", OutputFile);
                return true;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in WriteRoutingFile(" + OutputFile + ")");
            }
            return false;
        }

        /// <summary>
        /// Generate XML file for XPS Project generation.  The output file contains the set of all components, along with the FPGA 
        /// that each is mapped to.
        /// </summary>
        /// <param name="OutputFile">The path to the XML file.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Component Mapping, Component Map {Components})</seealso>
        public bool WriteComponentMapFile(string OutputFile)
        {
            try
            {
                if (!AllComponentsMapped())
                    return false;

                StreamWriter sWriter = new StreamWriter(OutputFile);

                XmlDocument XDoc = new XmlDocument();
                XDoc.AppendChild(XDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement XERoot = XDoc.CreateElement("SystemMap");
                FalconFileRoutines.WriteCerebrumDisclaimerXml(XERoot);    // Added by Matthew Cotter 8/18/2010
                XDoc.AppendChild(XERoot);

                XmlElement XECM = XDoc.CreateElement("ComponentMap");
                #region Write Components
                foreach (string CompID in Components.Keys)
                {
                    string mappedFPGA = GetComponentFPGAID(CompID);
                    if (FPGAs.ContainsKey(mappedFPGA))
                    {
                        FalconMapping_FPGA fpga = (FalconMapping_FPGA)FPGAs[mappedFPGA];
                        XmlElement XComp = XDoc.CreateElement("Component");
                        XComp.SetAttribute("ID", CompID);
                        XComp.SetAttribute("FPGA", fpga.ID);
                        XECM.AppendChild(XComp);
                    }
                    else // FPGA does not exist
                    {
                        return false;
                    }
                }
                #endregion
                XERoot.AppendChild(XECM);

                XmlElement XEFPGAs = XDoc.CreateElement("FPGAs");
                #region Extracted from FalconMapping_XML.WriteFPGAs()
                foreach (string cID in FPGAs.Keys)
                {
                    FalconMapping_FPGA fmf = (FalconMapping_FPGA)FPGAs[cID];
                    XmlElement XFPGA = XDoc.CreateElement("FPGA");
                    XFPGA.SetAttribute("ID", fmf.ID);
                    XEFPGAs.AppendChild(XFPGA);
                }
                #endregion
                XERoot.AppendChild(XEFPGAs);

                XDoc.Save(sWriter);
                sWriter.Close();
                RaiseMessageEvent("Saved XPS Component-Map to: {0}", OutputFile);
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in WriteComponentMapFile(" + OutputFile + ")");
            }
            return false;
        }

        /// <summary>
        /// Validates clock signal assingments, saves component/core configurations, and writes XPS Map file XPS Project Builder.
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Component Mapping, FPGA Map)</seealso>
        public bool WriteFinalOutputs()
        {
            try
            {
                string XPSMapFile = PathMan["XPSMap"];
                bool bReturn = true;
                ValidateClockAssignments();
                InsertEdgeComponents();
                bReturn = bReturn && WriteVortexConfigs();
                bReturn = bReturn && WriteComponentConfigs();
                bReturn = bReturn && WriteClockGenerators();
                bReturn = bReturn && (WriteFPGAMapFile(XPSMapFile));
                if (bReturn)
                {
                    DirectoryInfo ProjectRoot = new DirectoryInfo(PathMan["LocalProjectRoot"]);
                    GenerateRouting(String.Format("{0}\\routing_table.xml", ProjectRoot.FullName));
                    GenerateMappingReport(new DirectoryInfo(String.Format("{0}\\output_files", PathMan["LocalProjectRoot"])).FullName, ProjectRoot.Name);
                    ProjectEventRecorder Events = new ProjectEventRecorder();
                    Events.Open(PathMan);
                    Events.LogProjectEvent("LastMappingCompleted");
                    Events.Close();
                }
                return bReturn;
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in FinalizeMappingOutputs()");
                return false;
            }
        }

        /// <summary>
        /// Update the design and communications files to reflect component mappings.
        /// </summary>
        /// <param name="DesignFile">The path to the Input/Output Design file.</param>
        /// <param name="CommunicationsFile">The path to the Input/Output Communications file.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Design File, Processors / Communications File, Interfaces)</seealso>
        public bool UpdateInstanceMappings(string DesignFile, string CommunicationsFile)
        {
            try
            {
                return (FalconMapping_XML.UpdateDesignProcessors(this, DesignFile)) && (FalconMapping_XML.UpdateCommunicationInterfaces(this, CommunicationsFile));
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in UpdateDesignProcessors()");
                return false;
            }
        }

        /// <summary>
        /// Generate XML file for XPS Project generation.  The output file contains the set of all FPGAs, along with all components that are
        /// mapped to each one.
        /// </summary>
        /// <param name="OutputFile">The path to the XML file.</param>
        /// <returns>True if successful, false otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Component Mapping, FPGA Map)</seealso>
        private bool WriteFPGAMapFile(string OutputFile)
        {
            try
            {
                if (!AllComponentsMapped())
                    return false;
                XmlDocument XDoc = new XmlDocument();
                XDoc.AppendChild(XDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement XERoot = XDoc.CreateElement("SystemMap");
                FalconFileRoutines.WriteCerebrumDisclaimerXml(XERoot);    // Added by Matthew Cotter 8/18/2010
                XDoc.AppendChild(XERoot);

                foreach (string FPGAID in FPGAs.Keys)
                {
                    FalconMapping_FPGA fpga = (FalconMapping_FPGA)FPGAs[FPGAID];
                    XmlElement XEFPGA = XDoc.CreateElement("FPGA");
                    XEFPGA.SetAttribute("ID", FPGAID);

                    List<FalconMapping_Group> groups = fpga.GetMappedGroups();
                    foreach (FalconMapping_Group group in groups)
                    {
                        List<FalconMapping_Component> components = group.GetGroupedComponents();
                        foreach (FalconMapping_Component comp in components)
                        {
                            string CompID = comp.ID;
                            XmlElement XEComp = XDoc.CreateElement("Component");
                            XEComp.SetAttribute("ID", CompID);
                            XEComp.SetAttribute("Location", comp.Source);
                            XEComp.SetAttribute("Name", comp.Name);
                            foreach (ComponentCore CompCore in comp.ComponentCores.Values)
                            {
                                if (Conditions.EvaluateAsBoolean(comp.TranslateString(CompCore.ValidCondition)))
                                {
                                    XmlElement XEPcore = XDoc.CreateElement("PCore");
                                    XEPcore.SetAttribute("Type", CompCore.CoreType);
                                    XEPcore.SetAttribute("Location", CompCore.CoreSource);
                                    XEPcore.SetAttribute("Instance", CompCore.CoreInstance);
                                    XEPcore.SetAttribute("Version", CompCore.CoreVersion);
                                    XEPcore.SetAttribute("Native", CompCore.NativeInstance);
                                    XEPcore.SetAttribute("UCF", CompCore.CoreUCF);
                                    XEComp.AppendChild(XEPcore);
                                }
                            }
                            XEFPGA.AppendChild(XEComp);
                        }
                    }

                    if (fpga.ClockGenerator.NumClocks > 0)
                    {
                        XmlElement XEClockGen = XDoc.CreateElement("Component");
                        XEClockGen.SetAttribute("ID", "clock_generator");
                        XmlElement XEClock = XDoc.CreateElement("PCore");
                        XEClock.SetAttribute("Type", fpga.ClockGenerator.Type);
                        XEClock.SetAttribute("Location", string.Empty);
                        XEClock.SetAttribute("Instance", fpga.ClockGenerator.Instance);
                        XEClock.SetAttribute("Version", fpga.ClockGenerator.Version);
                        XEClock.SetAttribute("Native", fpga.ClockGenerator.Instance);
                        XEClockGen.AppendChild(XEClock);
                        XEFPGA.AppendChild(XEClockGen);
                    }

                    XmlElement XERouter = XDoc.CreateElement("Component");
                    XERouter.SetAttribute("ID", "vortex_routers");
                    fpga.WriteVortexToXPSMap(XDoc, XERouter);
                    XEFPGA.AppendChild(XERouter);

                    XERoot.AppendChild(XEFPGA);
                }
                XDoc.Save(OutputFile);
                RaiseMessageEvent("Saved XPS FPGA-Map to: {0}", OutputFile);
                return true;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in WriteFPGAMapFile(" + OutputFile + ")");
            }
            return false;
        }

        /// <summary>
        /// Writes core-configuration files for clock generators
        /// </summary>
        /// <returns>True if successful, False otherwise</returns>
        public bool WriteClockGenerators()
        {
            try
            {
                if (!AllComponentsMapped())
                    return false;
                if (!Directory.Exists(PathMan["LocalProjectRoot"] + "\\core_config"))
                    Directory.CreateDirectory(PathMan["LocalProjectRoot"] + "\\core_config");
                foreach (FalconMapping_FPGA fpga in FPGAs.Values)
                {
                    if (fpga.ClockGenerator.NumClocks > 0)
                    {
                        fpga.ClockGenerator.SaveClockConfig(PathMan["LocalProjectRoot"]);
                    }
                }
                RaiseMessageEvent("Saved Clock Generator Configurations.");
                return true;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in WriteClockGenerators()");
            }
            return false;
        }

        /// <summary>
        /// Writes core-configuration files for Vortex routers
        /// </summary>
        /// <returns>True if successful, False otherwise</returns>
        public bool WriteVortexConfigs()
        {
            try
            {
                if (!AllComponentsMapped())
                    return false;
                string ConfigPath = String.Format("{0}\\core_config", PathMan["LocalProjectRoot"]);
                if (!Directory.Exists(ConfigPath))
                    Directory.CreateDirectory(ConfigPath);
                foreach (FalconMapping_FPGA fpga in FPGAs.Values)
                {
                    fpga.WriteVortexConfigurations(PathMan);
                }
                RaiseMessageEvent("Saved Vortex Configurations.");
                return true;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in WriteVortexConfigs()");
            }
            return false;
        }

        /// <summary>
        /// Loads all custom core configurations into the associated subcomponents
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public bool LoadComponentConfigs()
        {
            try
            {
                foreach (FalconMapping_Component Comp in Components.Values)
                {
                    Comp.LoadComponentConfig(PathMan);
                }
                RaiseMessageEvent("Loaded Component Configurations.");
                return true;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in LoadComponentConfigs()");
            }
            return false;
        }
        /// <summary>
        /// Saves all custom core configurations from the associated subcomponents
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public bool WriteComponentConfigs()
        {
            try
            {
                foreach (FalconMapping_FPGA FPGA in FPGAs.Values)
                {
                    foreach (FalconMapping_Component Comp in FPGA.RequiredComponents)
                    {
                        if (!Components.ContainsKey(Comp.ID))
                        {
                            Comp.SaveComponentConfig(PathMan);
                        }
                    }
                }       
                foreach (FalconMapping_Component Comp in Components.Values)
                {
                    Comp.SaveComponentConfig(PathMan);
                }         
                RaiseMessageEvent("Saved Component Configurations.");
                return true;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in WriteComponentConfigs()");
            }
            return false;
        }
        /// <summary>
        /// Generate XML file detailing the FPGA to which each component is assigned, the FPGA's IP Address if any, and an application port assigned
        /// to the component
        /// </summary>
        /// <returns>True if successful, false otherwise.</returns>
        /// <seealso href="https://www.cse.psu.edu/svn/mdl/falcon_repository/trunk/Software/Cerebrum/Documentation/ProjectXML.pdf">
        /// Project XML File Documentation (Component Mapping, Address Map)</seealso>
        public bool WriteComponentAddressMap(string OutputFile)
        {
            try
            {
                if (!AllComponentsMapped())
                    return false;

                XmlDocument XDoc = new XmlDocument();
                XDoc.AppendChild(XDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement XERoot = XDoc.CreateElement("AddressMap");
                FalconFileRoutines.WriteCerebrumDisclaimerXml(XERoot);    // Added by Matthew Cotter 8/18/2010
                XDoc.AppendChild(XERoot);

                #region Write Components
                foreach (string CompID in Components.Keys)
                {
                    string mappedFPGA = GetComponentFPGAID(CompID);
                    if (FPGAs.ContainsKey(mappedFPGA))
                    {
                        FalconMapping_FPGA fpga = (FalconMapping_FPGA)FPGAs[mappedFPGA];
                        FalconMapping_Component comp = (FalconMapping_Component)Components[CompID];
                        if (comp.Port > 1200)
                        {
                            // Lower ports may be assigned to system processes, so this is a reasonable lower bound, 
                            // In actuality the value should be -1 if this is not required
                            // Only write the core to the address map if the server is required?
                            XmlElement XComp = XDoc.CreateElement("Component");
                            XComp.SetAttribute("ID", comp.ID);
                            XComp.SetAttribute("Name", comp.Name);
                            XComp.SetAttribute("FPGA", fpga.ID);
                            XComp.SetAttribute("IP", fpga.IPAddress);
                            XComp.SetAttribute("MAC", fpga.MACAddress);
                            XComp.SetAttribute("Port", comp.Port.ToString());
                            XERoot.AppendChild(XComp);
                        }
                    }
                    else // FPGA does not exist
                    {
                        return false;
                    }
                }
                #endregion

                XDoc.Save(OutputFile);
                RaiseMessageEvent("Saved Component AddressMap to: {0}", OutputFile);
                return true;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in WriteComponentAddressMap(" + OutputFile + ")");
            }
            return false;
        }
        
        /// <summary>
        /// Generates a text file report of the componing mapping, including a report for the assignment of TDAs to the Vortex router(s)
        /// </summary>
        /// <param name="DirectoryPath">The directory in which the report is to be generated.</param>
        /// <param name="FileBase">The base name of the file to be generated, excluding any date, time, or other formatting.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool GenerateMappingReport(string DirectoryPath, string FileBase)
        {
            StreamWriter writer = null;

            try
            {
                string DateTimeString = String.Format("{0} at {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString());
                string DateTimeSuffix = String.Format("{0}_{1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                while (DateTimeSuffix.Contains("-"))
                    DateTimeSuffix = DateTimeSuffix.Replace("-", "");
                while (DateTimeSuffix.Contains(":"))
                    DateTimeSuffix = DateTimeSuffix.Replace(":", "");
                while (DateTimeSuffix.Contains("-"))
                    DateTimeSuffix = DateTimeSuffix.Replace("/", "");

                FileInfo fiReport = new FileInfo(String.Format("{0}\\{1}_mapping_report.txt", DirectoryPath, FileBase, string.Empty));
                if (!Directory.Exists(DirectoryPath))
                    Directory.CreateDirectory(DirectoryPath);

                if (fiReport.Exists)
                    fiReport.Delete();

                writer = new StreamWriter(fiReport.FullName);

                writer.WriteLine(String.Format("Falcon Component Mapping Report - Generated: {0}", DateTimeString));
                writer.WriteLine();
                writer.WriteLine("Component Mapping Results");
                foreach (FalconMapping_FPGA FPGA in this.FPGAs.Values)
                {
                    writer.WriteLine("\tComponents mapped to FPGA {0}", FPGA.ID);
                    foreach (FalconMapping_Group Group in FPGA.GetMappedGroups())
                    {
                        writer.WriteLine("\t\tComponent Group: {0}", Group.ID);
                        foreach (FalconMapping_Component Component in Group.GetGroupedComponents())
                        {
                            writer.WriteLine("\t\t\tComponent: {0}", Component.ID);
                            foreach (ComponentCore SubComp in Component.ComponentCores.Values)
                            {
                                writer.WriteLine("\t\t\t\tPCore: {0}", SubComp.NativeInstance, SubComp.CoreInstance);
                            }
                        }
                    }
                    writer.WriteLine();
                }
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine("Vortex TDA Information");
                foreach (FalconMapping_FPGA FPGA in this.FPGAs.Values)
                {
                    writer.WriteLine("\tRouters instantiated on FPGA {0}", FPGA.ID);
                    foreach (IVortex Vortex in FPGA.GetVortexRouters())
                    {
                        writer.WriteLine("\t\tVortex Router: Bus {0}, Switch {1}", Vortex.BusID, Vortex.SwitchID);
                        if (Vortex.ConfigurationAttachment != null)
                        {
                            writer.WriteLine("\t\t\tConfiguration Device: Port {1} (TDA = 0x{2}) : {0}",
                                    Vortex.ConfigurationAttachment.Instance,
                                    Vortex.ConfigurationAttachment.Port,
                                    Vortex.ConfigurationAttachment.TDA.ToString("x8"));
                        }
                        else
                        {
                            writer.WriteLine("\t\t\tConfiguration Device: NONE");
                        }
                        writer.WriteLine();
                        foreach (IVortexAttachment VortexAtt in Vortex.AttachedDevices)
                        {
                            FalconMapping_Component Component = null;
                            ComponentCore CompCore = null;

                            foreach (FalconMapping_Component SearchComp in this.Components.Values)
                            {
                                if (VortexAtt.Instance.StartsWith(SearchComp.ID))
                                {
                                    foreach (ComponentCore SearchCore in SearchComp.ComponentCores.Values)
                                    {
                                        if (String.Compare(VortexAtt.CoreInstance, SearchCore.NativeInstance, true) == 0)
                                        {
                                            Component = SearchComp;
                                            CompCore = SearchCore;
                                            break;
                                        }
                                    }
                                }
                                if (Component != null)
                                    break;
                            }
                            if (VortexAtt is IVortexBridgeAttachment)
                            {
                                writer.WriteLine("\t\t\tPort {2} (TDA = 0x{3}): {0} => {1} [{4} {5}]",
                                    "BRIDGE",
                                    VortexAtt.Instance,
                                    VortexAtt.Port,
                                    VortexAtt.TDA.ToString("x8"),
                                    VortexAtt.Type,
                                    VortexAtt.Version);
                            }
                            else if (VortexAtt is IVortexEdgeAttachment)
                            {
                                writer.WriteLine("\t\t\tPort {2} (TDA = 0x{3}): EDGE {0} => {1} [{4} {5}]",
                                    "BRIDGE",
                                    VortexAtt.Instance,
                                    VortexAtt.Port,
                                    VortexAtt.TDA.ToString("x8"),
                                    VortexAtt.Type,
                                    VortexAtt.Version);
                            }
                            else
                            {
                                writer.WriteLine("\t\t\tPort {2} (TDA = 0x{3}): {0} => {1} [{4} {5}]",
                                    Component.ID,
                                    CompCore.NativeInstance,
                                    VortexAtt.Port,
                                    VortexAtt.TDA.ToString("x8"),
                                    CompCore.CoreType,
                                    CompCore.CoreVersion);
                            }
                        }
                        writer.WriteLine();
                    }
                    writer.WriteLine();
                    writer.WriteLine();
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
            finally            
            {
                if (writer != null)
                    writer.Close();
            }
            return false;
        }

        /// <summary>
        /// Loads a pre-saved, intermediate state of groups and mappings into the library.
        /// </summary>
        /// <param name="OutputFile">The target file to contain the intermediate state information.</param>
        public void SaveIntermediateState(string OutputFile)
        {
            FalconMapping_XML.WriteIntermediateState(OutputFile, this);
        }

        /// <summary>
        /// Loads a pre-saved, intermediate state of groups and mappings into the library.
        /// </summary>
        /// <param name="InputFile">The source file containing the intermediate state information.</param>
        public void LoadIntermediateState(string InputFile)
        {
            FalconMapping_XML.ReadIntermediateState(InputFile, this);
        }
        #endregion

        #region Vortex Flow Identification

        /// <summary>
        /// Contructs a list of Identified data flows using the connections and components defined in the system
        /// </summary>
        public void IdentifyFlows()
        {
            List<DataFlow> _IdentifiedFlows = new List<DataFlow>();

            #region Identify SAPs that initiate a connection
            foreach (FalconMapping_Connection Connection in this.Connections.Values)
            {
                // Iterate through connections
                foreach (FalconMapping_Component Component in this.Components.Values)
                {
                    // Find the source component
                    if (String.Compare(Connection.SourceComponent, Component.ID) == 0)
                    {
                        // This component sources a connection, check its attachments
                        foreach (IVortexAttachment SAP in Component.VortexDevices)
                        {
                            // Find a matching SAP attachment
                            if (SAP.GetType() == typeof(VortexInterfaces.IVortexSAP))
                            {
                                // This subcomponent is a SAP
                                string SAPInst = string.Empty;
                                if (SAP.Instance.StartsWith(Component.ID))
                                    SAPInst = SAP.Instance.Substring(Component.ID.Length + 1);
                                //SAP.Instance.Replace(String.Format("{0}_", Component.ID), string.Empty);
                                if (String.Compare(SAPInst, Connection.SourceComponentInstance) == 0)
                                {
                                    // This SAP is the source for this connection
                                    foreach (ComponentCore CompCore in Component.ComponentCores.Values)
                                    {
                                        // Find the corresponding subcomponent
                                        if (String.Compare(CompCore.CoreInstance, SAP.Instance) == 0)
                                        {
                                            // This SAP subcomponent is the source for this connection
                                            DataFlow flow = new DataFlow();
                                            flow.Add(CompCore, SAP);
                                            _IdentifiedFlows.Add(flow);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

        }
        
        /// <summary>
        /// Once component mapping has been completed, this method is called to 'break' connections that span multiple FPGAs in order to insert the components that will connect them.
        /// </summary>
        public void InsertEdgeComponents()
        {
            try
            {
                MappedConnections = new Dictionary<string,FalconMapping_Connection>();
                FileInfo fiRoutingTemp = new FileInfo(String.Format("{0}\\routing_temp.xml", PathMan["ProjectTemp"]));
                WriteRoutingFile(fiRoutingTemp.FullName);
                FalconGraphBuilder<string> Routing = new FalconGraphBuilder<string>(fiRoutingTemp.FullName);
                fiRoutingTemp.Delete();

                // Iterate through all connections
                foreach (FalconMapping_Connection Connection in Connections.Values)
                {
                    // Get the components at both ends of the connection
                    FalconMapping_Component Source = Components[Connection.SourceComponent];
                    FalconMapping_Component Sink = Components[Connection.SinkComponent];

                    // Get the FPGA to which each is mapped
                    string SourceFPGA_ID = GetComponentFPGAID(Source.ID);
                    string SinkFPGA_ID = GetComponentFPGAID(Sink.ID);

                    // Check if they're the same FPGA
                    if (String.Compare(SourceFPGA_ID, SinkFPGA_ID) != 0)
                    {
                        // They're on different FPGAs, we need to split the connections
                        FalconMapping_Link lastLink = null;
                        List<FalconMapping_FPGA> ShortestPath = GetShortestPath(Routing, SourceFPGA_ID, SinkFPGA_ID);
                        for (int hop = 0; hop < (ShortestPath.Count - 1); hop++)
                        {
                            FalconMapping_Link nextLink = null;
                            foreach (FalconMapping_Link link in Links.Values)
                            {
                                if (((String.Compare(ShortestPath[hop].ID, link.SourceFPGA, true) == 0) &&
                                    (String.Compare(ShortestPath[hop + 1].ID, link.SinkFPGA, true) == 0)) ||
                                    ((String.Compare(ShortestPath[hop].ID, link.SinkFPGA, true) == 0) &&
                                    (String.Compare(ShortestPath[hop + 1].ID, link.SourceFPGA, true) == 0) &&
                                    link.Bidirectional))
                                {
                                    nextLink = link;
                                    break;
                                }
                            }
                            // Now we know where we need to go next
                            if (hop == 0)
                            {
                                // First hop -- create the connection from the source component to the edge component specified in the next link
                                FalconMapping_Connection newConn = new FalconMapping_Connection(String.Format("{0}_hop{1}", Connection.ID, hop.ToString()),
                                                                                                Connection.SourceComponent, nextLink.SourceComponent);
                                newConn.SourceComponentInstance = Connection.SourceComponentInstance;
                                newConn.SinkComponentInstance = nextLink.SourceEgressCore;
                                MappedConnections.Add(newConn.ID, newConn);
                            }
                            else
                            {
                                // Verify that the sink of the last link, and the source of the next link are on the same FPGA
                                if ((String.Compare(lastLink.SinkFPGA, nextLink.SourceFPGA) != 0) &&
                                    (String.Compare(lastLink.SourceFPGA, nextLink.SinkFPGA) != 0))
                                {
                                    String LinkSplicingError = String.Format("Attempt to splice ({0} => {1} / {2} => {3}) failed.",
                                        lastLink.SinkFPGA, nextLink.SourceFPGA,
                                        lastLink.SourceFPGA, nextLink.SinkFPGA);
                                    throw new Exception(String.Format("Error while inserting edge components for flow analysis: {0}", LinkSplicingError));
                                }

                                // Next (but not last) hop -- create the connection from link edge to link edge
                                FalconMapping_Connection newConn = new FalconMapping_Connection(String.Format("{0}_hop{1}", Connection.ID, hop.ToString()),
                                                                                                lastLink.SinkComponent, nextLink.SourceComponent);
                                newConn.SourceComponentInstance = lastLink.SinkIngressCore;
                                newConn.SinkComponentInstance = nextLink.SourceEgressCore;
                                MappedConnections.Add(newConn.ID, newConn);
                            }
                            lastLink = nextLink;
                        }

                        // Now, we should have connections from the source component to the sink FPGA
                        // Create the connection from the last edge to the sink component
                        FalconMapping_Connection lastConn = new FalconMapping_Connection(String.Format("{0}_hop{1}", Connection.ID, (ShortestPath.Count - 1).ToString()),
                                                                                        lastLink.SinkComponent, Connection.SinkComponent);
                        lastConn.SourceComponentInstance = lastLink.SinkIngressCore;
                        lastConn.SinkComponentInstance = Connection.SinkComponentInstance;
                        MappedConnections.Add(lastConn.ID, lastConn);

                    }
                    else
                    {
                        // Add the connection As-Is
                        FalconMapping_Connection newConn = new FalconMapping_Connection(Connection.ID, Connection.SinkComponent, Connection.SinkComponent);
                        newConn.SourceComponentInstance = Connection.SourceComponentInstance;
                        newConn.SinkComponentInstance = Connection.SinkComponentInstance;
                        MappedConnections.Add(newConn.ID, newConn);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
        }

        /// <summary>
        /// Gets the shortest path between two FPGAs, given the calculated routing.
        /// </summary>
        /// <param name="Routing">The FalconGraphBuilder routing table generated.</param>
        /// <param name="SourceFPGA_ID">The source FPGA ID</param>
        /// <param name="SinkFPGA_ID">The sink FPGA ID</param>
        /// <returns>A list of FPGA objects, in order, connecting the source to the sink.</returns>
        private List<FalconMapping_FPGA> GetShortestPath(FalconGraphBuilder<string> Routing, string SourceFPGA_ID, string SinkFPGA_ID)
        {
            List<FalconMapping_FPGA> path = new List<FalconMapping_FPGA>();
            path.Clear();
            Stack<string> sPath = Routing.ComputeShortestPath(SourceFPGA_ID, SinkFPGA_ID);
            while (sPath.Count > 0)
                path.Add(this.FPGAs[sPath.Pop()]);
            
            return path;
        }
        #endregion

        #region Subsystem Identification
        /// <summary>
        /// Identify FPGA and Component subsystems
        /// </summary>
        private void IdentifySubsystems(ref List<FPGA_Subsystem> FPGA_Systems, ref List<Component_Subsystem> Component_Systems)
        {
            FPGA_Systems = new List<FPGA_Subsystem>();
            Component_Systems = new List<Component_Subsystem>();

            #region Construct FPGA Subsystems from Links
            List<FalconMapping_Link> ProcessedLinks = new List<FalconMapping_Link>();
            while (ProcessedLinks.Count < this.Links.Count)
            {
                bool bUpdated = false;
                foreach (FalconMapping_Link link in this.Links.Values)
                {
                    if (ProcessedLinks.Contains(link))
                        continue;

                    FalconMapping_FPGA src = this.FPGAs[link.SourceFPGA];
                    FalconMapping_FPGA snk = this.FPGAs[link.SinkFPGA];

                    foreach (FPGA_Subsystem fpga_sub in FPGA_Systems)
                    {
                        if ((fpga_sub.Members.Contains(src)) && (!fpga_sub.Members.Contains(snk)))
                        {
                            fpga_sub.Add(snk);
                            ProcessedLinks.Add(link);
                            bUpdated = true;
                            break;
                        }
                        if ((fpga_sub.Members.Contains(snk)) && (!fpga_sub.Members.Contains(src)))
                        {
                            fpga_sub.Add(src);
                            ProcessedLinks.Add(link);
                            bUpdated = true;
                            break;
                        }
                    }
                    if (!bUpdated)
                    {
                        FPGA_Subsystem fpga_sub = new FPGA_Subsystem();
                        fpga_sub.Add(src);
                        fpga_sub.Add(snk);
                        ProcessedLinks.Add(link);
                        FPGA_Systems.Add(fpga_sub);
                        bUpdated = true;
                    }
                }
            }
            // Once all linkections have been processed, merge
            List<FPGA_Subsystem> FSToBeRemoved = new List<FPGA_Subsystem>();
            foreach (FPGA_Subsystem fpga_sub in FPGA_Systems)
            {
                foreach (FPGA_Subsystem fpga_sub2 in FPGA_Systems)
                {
                    if (fpga_sub == fpga_sub2)
                        continue;

                    if (fpga_sub.SharesMemberWith(fpga_sub2))
                    {
                        fpga_sub.MergeWith(fpga_sub2);
                        FSToBeRemoved.Add(fpga_sub2);
                    }
                }
            }
            // Remove empty subsystems
            foreach (FPGA_Subsystem fpga_sub in FSToBeRemoved)
            {
                FPGA_Systems.Remove(fpga_sub);
            }
            // Add missing FPGAs
            foreach (FalconMapping_FPGA fpga in this.FPGAs.Values)
            {
                bool bFound = false;
                foreach (FPGA_Subsystem fpga_sub in FPGA_Systems)
                {
                    if (fpga_sub.Members.Contains(fpga))
                    {
                        bFound = true;
                        break;
                    }
                }
                if (!bFound)
                {
                    FPGA_Subsystem fpga_sub = new FPGA_Subsystem();
                    fpga_sub.Add(fpga);
                    FPGA_Systems.Add(fpga_sub);
                }
            }
            #endregion

            #region Construct Component Subsystems from Connections
            List<FalconMapping_Connection> ProcessedConnections = new List<FalconMapping_Connection>();
            while (ProcessedConnections.Count < this.Connections.Count)
            {
                bool bUpdated = false;
                foreach (FalconMapping_Connection conn in this.Connections.Values)
                {
                    if (ProcessedConnections.Contains(conn))
                        continue;

                    FalconMapping_Component src = this.Components[conn.SourceComponent];
                    FalconMapping_Component snk = this.Components[conn.SinkComponent];

                    foreach (Component_Subsystem comp_sub in Component_Systems)
                    {
                        if ((comp_sub.Members.Contains(src)) && (!comp_sub.Members.Contains(snk)))
                        {
                            comp_sub.Add(snk);
                            ProcessedConnections.Add(conn);
                            bUpdated = true;
                            break;
                        }
                        if ((comp_sub.Members.Contains(snk)) && (!comp_sub.Members.Contains(src)))
                        {
                            comp_sub.Add(src);
                            ProcessedConnections.Add(conn);
                            bUpdated = true;
                            break;
                        }
                    }
                    if (!bUpdated)
                    {
                        Component_Subsystem comp_sub = new Component_Subsystem();
                        comp_sub.Add(src);
                        comp_sub.Add(snk);
                        ProcessedConnections.Add(conn);
                        Component_Systems.Add(comp_sub);
                        bUpdated = true;
                    }
                }
            }
            // Once all connections have been processed, merge
            List<Component_Subsystem> CSToBeRemoved = new List<Component_Subsystem>();
            foreach (Component_Subsystem comp_sub in Component_Systems)
            {
                foreach (Component_Subsystem comp_sub2 in Component_Systems)
                {
                    if (comp_sub == comp_sub2)
                        continue;

                    if (comp_sub.SharesMemberWith(comp_sub2))
                    {
                        comp_sub.MergeWith(comp_sub2);
                        CSToBeRemoved.Add(comp_sub2);
                    }
                }
            }
            // Remove empty subsystems
            foreach (Component_Subsystem comp_sub in CSToBeRemoved)
            {
                Component_Systems.Remove(comp_sub);
            }
            // Add missing components
            foreach (FalconMapping_Component comp in this.Components.Values)
            {
                bool bFound = false;
                foreach (Component_Subsystem comp_sub in Component_Systems)
                {
                    if (comp_sub.Members.Contains(comp))
                    {
                        bFound = true;
                        break;
                    }
                }
                if (!bFound)
                {
                    Component_Subsystem comp_sub = new Component_Subsystem();
                    comp_sub.Add(comp);
                    Component_Systems.Add(comp_sub);
                }
            }
            #endregion

            #region Subsystem Debug
            int i;
            i = 0;
            foreach (FPGA_Subsystem fpga_sub in FPGA_Systems)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("FPGA Subsystem #{0}", i.ToString()));
                foreach (FalconMapping_FPGA fpga in fpga_sub.Members)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("\tFPGA: {0}", fpga.ID));
                }
                i++;
            }
            i = 0;
            foreach (Component_Subsystem comp_sub in Component_Systems)
            {
                System.Diagnostics.Debug.WriteLine(String.Format("Component Subsystem #{0}", i.ToString()));
                foreach (FalconMapping_Component comp in comp_sub.Members)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("\tComponent: {0}", comp.ID));
                }
                i++;
            }
            #endregion

        }
        #endregion

        #region Clock Handling

        /// <summary>
        /// Automatically assigns and validates clocks and clock generator assignments.
        /// </summary>
        private void ValidateClockAssignments()
        {
            // Autoassign clocks on that FPGA
            // Then, finalize the clock generator on that FPGA
            foreach (FalconMapping_FPGA FPGA in FPGAs.Values)
            {
                FPGA.AutoAssignClocks();
                FPGA.FinalizeClockGenerator();
            }
        }

        #endregion

        #region Set/Collection Accessors (For Use With External Iterators)

        /// <summary>
        /// Get The Hashtable collection of FalconMapping_Components contained in the mapping system.  To be used for
        /// iterators or enumeration.
        /// </summary>
        /// <returns>The Hashtable collection of components.</returns>
        public Dictionary<string, FalconMapping_Component> GetComponents()
        {
            try
            {
                if (Components == null)
                    return new Dictionary<string, FalconMapping_Component>();
                return Components;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in GetComponents()");
            }
            return null;
        }
        
        /// <summary>
        /// Get The Hashtable collection of FalconMapping_FPGAs contained in the Mapping system.  To be used for
        /// iterators or enumeration.
        /// </summary>
        /// <returns>The Hashtable collection of FPGAs.</returns>
        public Dictionary<string, FalconMapping_FPGA> GetFPGAs()
        {
            try
            {
                if (FPGAs == null)
                    return new Dictionary<string, FalconMapping_FPGA>();
                return FPGAs;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in GetFPGAs()");
            }
            return null;
        }
        
        /// <summary>
        /// Get The Hashtable collection of FalconMapping_Groups contained in the Mapping system.  To be used for
        /// iterators or enumeration.
        /// </summary>
        /// <returns>The Hashtable collection of groups.</returns>
        public Dictionary<string, FalconMapping_Group> GetGroups()
        {
            try
            {
                if (Groups == null)
                    return new Dictionary<string, FalconMapping_Group>();
                return Groups;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in GetGroups()");
            }
            return null;
        }
        
        /// <summary>
        /// Get The Hashtable collection of FalconMapping_Clusters contained in the Mapping system.  To be used for
        /// iterators or enumeration.
        /// </summary>
        /// <returns>The Hashtable collection of clusters.</returns>
        public Dictionary<string, FalconMapping_Cluster> GetClusters()
        {
            try
            {
                if (Clusters == null)
                    return new Dictionary<string, FalconMapping_Cluster>();
                return Clusters;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in GetClusters()");
            }
            return null;
        }
        
        /// <summary>
        /// Get The Hashtable collection of logical component FalconMapping_Connections contained in the Mapping system.  To be used for
        /// iterators or enumeration.
        /// </summary>
        /// <returns>The Hashtable collection of logical component connections.</returns>
        public Dictionary<string, FalconMapping_Connection> GetConnections()
        {
            try
            {
                if (Connections == null)
                    return new Dictionary<string, FalconMapping_Connection>();
                return Connections;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in GetConnections()");
            }
            return null;
        }
        
        /// <summary>
        /// Get The Hashtable collection of physical FalconMapping_Links contained in the Mapping system.  To be used for
        /// iterators or enumeration.
        /// </summary>
        /// <returns>The Hashtable collection of physical FalconMapping_Links.</returns>
        public Dictionary<string, FalconMapping_Link> GetLinks()
        {
            try
            {
                if (Links == null)
                    return new Dictionary<string, FalconMapping_Link>();
                return Links;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in GetLinks()");
            }
            return null;
        }
        #endregion

        #region Manipulate Components Collection (Add/Remove/Modify/Get)
        /// <summary>
        /// Adds a new component to the components collection of mapping objects with the specified ID, Name, 
        /// and Required Resources.  If the required resources are unknown or to be determined later,
        /// null should be passed for NewComponentResources.  If the desired ID is already assigned to another
        /// component in the system, an IDAlreadyExistsException will be thrown.
        /// </summary>
        /// <param name="NewComponentID">The desired ID for the new component.  If this value is equal to string.Empty, 
        /// a pseudo-random number will be generated for the component ID.</param>
        /// <param name="NewComponentName">The desired name for the new component.  If this value is equal to string.Empty,
        /// the name will be "group" concatenated with the component ID.</param>
        /// <param name="NewSupportedArches">The architectures supported by this component.</param>
        /// <param name="NewComponentResources">If known, a ResourceInfo class containing the list of the resources
        /// required by this component.  If not known, this argument should be null, and required resources
        /// will be assumed to be none.</param>
        /// <returns>The ID of the newly added component if successful, string.Empty if unsuccessful.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        public string AddComponent(string NewComponentID, string NewComponentName, string NewSupportedArches, ResourceInfo NewComponentResources)
        {
            try
            {
                if (Components.ContainsKey(NewComponentID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", "Component", NewComponentID));
                else
                {
                    if (NewComponentID == string.Empty)
                    {
                        // Passed in a null string, generate a pseudo-random group ID
                        Random r = new Random();
                        NewComponentID = r.Next((2 * Components.Count) + 1).ToString();
                        while (Components.ContainsKey(NewComponentID))
                            NewComponentID = r.Next((2 * Components.Count) + 1).ToString();
                    }
                    if (NewComponentName == string.Empty)
                    {
                        NewComponentName = "comp" + NewComponentID;
                    }
                    FalconMapping_Component newComp = new FalconMapping_Component(NewComponentID, NewComponentName, NewSupportedArches);
                    if (NewComponentResources != null)
                    {
                        foreach (string resName in NewComponentResources.GetResources().Keys)
                        {
                            newComp.SetRequiredResource(resName, NewComponentResources.GetResource(resName));
                        }
                    }
                    Components.Add(NewComponentID, newComp);
                    return NewComponentID;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "AddComponent()");
                return string.Empty;
            }
        }

        /// <summary>
        /// Removes the component with the specified ID from the components collection of mapping objects.
        /// If no component exists with the specified ID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="ExComponentID">The ID of the component to be removed</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool RemoveComponent(string ExComponentID)
        {
            try
            {
                if (!Components.ContainsKey(ExComponentID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "Component", ExComponentID));
                else
                {
                    RemoveComponentFromGroup(ExComponentID);
                    Components.Remove(ExComponentID);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "RemoveComponent()");
            }
            return false;
        }

        /// <summary>
        /// Attempts to change the ID assigned to a component, given its current ID.   If no component exists that
        /// is assigned the ID specified by CurrentComponentID, an IDDoesNotExistException will be thrown.  If a component
        /// already is assigned the ID specified by NewComponentID, an IDAlreadyExistsException will be thrown.  
        /// In either case, the ID change will not occur.
        /// </summary>
        /// <param name="CurrentComponentID">The current ID of the component whose ID is to be changed.</param>
        /// <param name="NewComponentID">The new desired ID of the component.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyComponentID(string CurrentComponentID, string NewComponentID)
        {
            try
            {
                if (!Components.ContainsKey(CurrentComponentID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "Component", CurrentComponentID));
                else if (Components.ContainsKey(NewComponentID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", "Component", NewComponentID));
                else
                {
                    FalconMapping_Component currentComp = (FalconMapping_Component)Components[CurrentComponentID];
                    Components.Remove(CurrentComponentID);
                    currentComp.ID = NewComponentID;
                    Components.Add(NewComponentID, currentComp);
                    foreach (string connID in Connections.Keys)
                    {
                        FalconMapping_Connection conn = (FalconMapping_Connection)Connections[connID];
                        if (conn.SourceComponent == CurrentComponentID)
                            conn.SourceComponent = NewComponentID;
                        if (conn.SinkComponent == CurrentComponentID)
                            conn.SinkComponent = NewComponentID;
                    }
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyComponentID()");
            }
            return false;
        }

        /// <summary>
        /// Changes the name assigned to a component, given its ID.   If no component exists with the ID specified by
        /// ComponentID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="ComponentID">The ID of the component whose name is to be changed.</param>
        /// <param name="NewComponentName">The new desired name of the component.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyComponentName(string ComponentID, string NewComponentName)
        {
            try
            {
                if (!Components.ContainsKey(ComponentID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "Component", ComponentID));
                else
                {
                    FalconMapping_Component currentComp = (FalconMapping_Component)Components[ComponentID];
                    currentComp.Name = NewComponentName;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyComponentName()");
            }
            return false;
        }

        /// <summary>
        /// Sets the amount of particular resource required by the component specified by ComponentID. If no 
        /// component exists with the ID specified by ComponentID, an IDDoesNotExistException will be thrown. 
        /// </summary>
        /// <param name="ComponentID">The ID of the component whose resource set is to be changed.</param>
        /// <param name="ComponentResource">The name of the resource to be changed.</param>
        /// <param name="NewAmount">The new amount of the resource that is required.  This is NOT relative to the previous amount.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyComponentResource(string ComponentID, string ComponentResource, long NewAmount)
        {
            try
            {
                if (!Components.ContainsKey(ComponentID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "Component", ComponentID));
                else
                {
                    FalconMapping_Component currentComp = (FalconMapping_Component)Components[ComponentID];
                    currentComp.SetRequiredResource(ComponentResource, NewAmount);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyComponentResource()");
            }
            return false;
        }

        /// <summary>
        /// Attempts to change the port number assigned to a component, given its current ID.   If no component exists that
        /// is assigned the ID specified by ComponentID, an IDDoesNotExistException will be thrown.  
        /// </summary>
        /// <param name="ComponentID">The ID of the component whose Port is to be changed.</param>
        /// <param name="NewPort">The new desired port of the component.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyComponentPort(string ComponentID, int NewPort)
        {
            try
            {
                if (!Components.ContainsKey(ComponentID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "Component", ComponentID));
                else
                {
                    FalconMapping_Component currentComp = (FalconMapping_Component)Components[ComponentID];
                    currentComp.Port = NewPort;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyComponentPort()");
            }
            return false;
        }

        /// <summary>
        /// Returns the amount of a given resource that the specified component requires.  If no 
        /// component exists with the ID specified by ComponentID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="ComponentID">The ID of the component whose resource set is to be queried.</param>
        /// <param name="ComponentResource">The name of the resource to be examined.</param>
        /// <returns>The integer amount of the specified resource required by the component if successful, -1 otherwise.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public long GetComponentResource(string ComponentID, string ComponentResource)
        {
            try
            {
                if (!Components.ContainsKey(ComponentID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "Component", ComponentID));
                else
                {
                    FalconMapping_Component currentComp = (FalconMapping_Component)Components[ComponentID];
                    return currentComp.GetRequiredResource(ComponentResource);
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetComponentResource()");
            }
            return -1;
        }

        /// <summary>
        /// Returns the current name assigned to specified component.  If no component exists with the ID 
        /// specified by ComponentID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="ComponentID">The ID of the component whose name is to be returned.</param>
        /// <returns>The name of the component on success, string.Empty otherwise.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public string GetComponentName(string ComponentID)
        {
            try
            {
                if (!Components.ContainsKey(ComponentID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "Component", ComponentID));
                else
                {
                    FalconMapping_Component currentComp = (FalconMapping_Component)Components[ComponentID];
                    return currentComp.Name;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetComponentName()");
            }
            return string.Empty;
        }
        /// <summary>
        /// Returns the current port assigned to specified component.  If no component exists with the ID 
        /// specified by ComponentID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="ComponentID">The ID of the component whose port is to be returned.</param>
        /// <returns>The assigned to the component.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public int GetComponentPort(string ComponentID)
        {
            try
            {
                if (!Components.ContainsKey(ComponentID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "Component", ComponentID));
                else
                {
                    FalconMapping_Component currentComp = (FalconMapping_Component)Components[ComponentID];
                    return currentComp.Port;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetComponentPort()");
            }
            return -1;
        }

        /// <summary>
        /// Returns whether or not the component is grouped.  If no component exists with the ID 
        /// specified by ComponentID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="ComponentID">The ID of the component to determine grouped status for</param>
        /// <returns>True if the component exists and is grouped, false otherwise</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool IsComponentGrouped(string ComponentID)
        {
            try
            {
                if (!Components.ContainsKey(ComponentID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "Component", ComponentID));
                else
                {
                    FalconMapping_Component currentComp = (FalconMapping_Component)Components[ComponentID];
                    return currentComp.IsGrouped;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "IsComponentGrouped()");
            }
            return false;
        }

        /// <summary>
        /// Returns the ID of the group to which the specified component is grouped, if any.  If no component 
        /// exists with the ID specified by ComponentID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="ComponentID">The ID of the component to determine the group ID for, if any</param>
        /// <returns>The group ID of the group the component is in, if the component exists and is grouped, string.Empty otherwise</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public string GetComponentGroupID(string ComponentID)
        {
            try
            {
                if (!Components.ContainsKey(ComponentID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "Component", ComponentID));
                else
                {
                    FalconMapping_Component currentComp = (FalconMapping_Component)Components[ComponentID];
                    if (currentComp.IsGrouped)
                        return currentComp.GroupID;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetComponentGroupID()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the ID of the FPGA on which the group containing the specified Component is mapped.   If the Component ID does not
        /// exist, an IDDoesNotExistException is thrown.  If the Component is grouped, and its GroupID does not exist, an 
        /// IDDoesNotExistException is thrown.  If the Component is grouped, its group is mapped, but the Target FPGA ID does not exist,
        /// an IDDoesNotExistException is thrown.   If the Component is not grouped, or the group is not mapped, String.Empty is returned.
        /// </summary>
        /// <param name="ComponentID">The ID of the component to locate on an FPGA</param>
        /// <returns>The FPGA ID containing the group that the component is a member of, string.Empty otherwise.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public string GetComponentFPGAID(string ComponentID)
        {
            try
            {
                if (!Components.ContainsKey(ComponentID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", 
                        "Component", ComponentID));
                else
                {
                    if (IsComponentGrouped(ComponentID))
                    {
                        string GroupID = GetComponentGroupID(ComponentID);
                        if (!Groups.ContainsKey(GroupID))
                            throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist (from Component.GroupID)",
                                "Group", GroupID));
                        else
                        {
                            if (IsGroupMapped(GroupID))
                            {
                                string FPGAID = GetGroupTargetFPGAID(GroupID);
                                if (!FPGAs.ContainsKey(FPGAID))
                                    throw new Exceptions.IDDoesNotExistException(
                                        String.Format("{0} ID {1} does not exist (from Component.GroupID, Group.TargetFPGA)",
                                        "FPGA", FPGAID));
                                else
                                    return FPGAID;
                            }
                            else
                                return string.Empty;
                        }
                    }
                    else
                        return string.Empty;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetComponentFPGAID()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Calculates the average number of FPGA hops between components, based on the connections, 
        /// and mapped location of components.
        /// </summary>
        /// <returns>Returns the average number of hops if there are no errors during calculation, -1.0 otherwise.</returns>
        public double GetAverageFPGAHops()
        {
            try
            {
                Object[,] aMatrix = CreateFPGAAdjacencyMatrix();
                double totalHops = 0.0F;
                int validConnections = 0;
                foreach (string ConnID in Connections.Keys)
                {
                    FalconMapping_Connection conn = (FalconMapping_Connection)Connections[ConnID];
                    if ((Components.ContainsKey(conn.SourceComponent)) && (Components.ContainsKey(conn.SinkComponent)))
                    {
                        string srcID = GetComponentFPGAID(conn.SourceComponent);
                        string sinkID = GetComponentFPGAID(conn.SinkComponent);
                        if ((srcID == string.Empty) || (sinkID == string.Empty))
                        {
                            //System.Diagnostics.Debug.WriteLine("Error locating a component on an FPGA");
                            return -1.0F;
                        }

                        int srcIdx = -1;
                        int sinkIdx = -1;
                        for (int c = 1; c < FPGAs.Count + 1; c++)
                        {
                            if (aMatrix[0, c].ToString() == srcID)
                                srcIdx = c;
                            if (aMatrix[0, c].ToString() == sinkID)
                                sinkIdx = c;
                        }
                        if ((srcIdx < 0) || (sinkIdx < 0))
                        {
                            //System.Diagnostics.Debug.WriteLine("Error locating an FPGA in the adjacency matrix");
                            return -1.0F;
                        }
                        int hops = (int)aMatrix[srcIdx, sinkIdx];
                        totalHops = totalHops + hops;
                        validConnections += 1;
                    }
                }
                totalHops = totalHops / (double)validConnections;
                return totalHops;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetAverageFPGAHops()");
            }
            return -1.0F;
        }

        /// <summary>
        /// Iterates through the entire set of Components, and for any are not grouped, automatically generates a group for them.
        /// </summary>
        /// <returns>True on success, False on failure.</returns>
        internal bool GroupUngroupedComponents()
        {
            try
            {
                foreach (string ComponentID in Components.Keys)
                {
                    if (!IsComponentGrouped(ComponentID))
                        CreateNewGroupForComponent(ComponentID);
                }
                return true;
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in CreateNewGroupForComponent()");
            }
            return false;

        }

        /// <summary>
        /// Removes all component groupings in the system, resulting in all components being unmapped.
        /// </summary>
        public void UnGroupAll()
        {
            try
            {
                foreach (string ComponentID in Components.Keys)
                {
                    RemoveComponentFromGroup(ComponentID);
                }
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in UnGroupAll()");
            }
        }

        /// <summary>
        /// Determines whether or not the specified component has been mapped, by determining whether its associated
        /// group has been mapped.   If any ID in the search (Component, Group, FPGA) does not exist, an IDDoesNotExistException 
        /// will be thrown.
        /// </summary>
        /// <param name="ComponentID">The ID of the component to search for.</param>
        /// <returns>True if the component is grouped, and that group is mapped to an FPGA.  False if the component is not grouped, 
        /// the group is not mapped, or an error occurs.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool IsComponentMapped(string ComponentID)
        {
            try
            {
                if (!Components.ContainsKey(ComponentID))
                {
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist.", 
                        "Component", ComponentID));
                }
                else
                {
                    FalconMapping_Component comp = (FalconMapping_Component)Components[ComponentID];
                    if (!comp.IsGrouped)
                    {
                        return false;
                    }
                    else
                    {
                        if (!Groups.ContainsKey(comp.GroupID))
                        {
                            throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist (in FalconMapping_Component.GroupID).",
                                "Group", comp.GroupID));
                        }
                        else
                        {
                            FalconMapping_Group group = (FalconMapping_Group)Groups[comp.GroupID];
                            if (!group.IsMapped)
                            {
                                return false;
                            }
                            else
                            {
                                if (!FPGAs.ContainsKey(group.TargetFPGA))
                                {
                                    throw new Exceptions.IDDoesNotExistException(
                                        String.Format("{0} ID {1} does not exist  (in FalconMapping_Group.TargetFPGA).",
                                        "FPGA", group.TargetFPGA));
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in IsComponentMapped()");
            }
            return false;
        }

        /// <summary>
        /// Determines whether or not ALL components in the system have been mapped, by determining whether each component's associated
        /// group has been mapped.   If any ID in the search (Component, Group, FPGA) does not exist, an IDDoesNotExistException 
        /// will be thrown.
        /// </summary>
        /// <returns>True if ALL components are grouped, and each of those groups is mapped to an FPGA.  False if any component is not grouped, 
        /// any non-empty group is not mapped, or an error occurs.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool AllComponentsMapped()
        {
            bool success = true;
            try
            {
                foreach (string componentID in Components.Keys)
                {
                    success = success && IsComponentMapped(componentID);
                    if (!success)
                        break;
                }
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in AllComponentsMapped()");
            }
            return success;
        }

        /// <summary>
        /// Loads the subcomponents and corresponding configuration data for the specified component.
        /// </summary>
        /// <param name="ComponentID">The ID of the component for which to load data.</param>
        public void LoadComponentData(string ComponentID)
        {
            try
            {
                if (Components.ContainsKey(ComponentID))
                {
                    FalconMapping_Component fmc = Components[ComponentID];
                    fmc.LoadComponentSource(this.PathMan);
                    fmc.LoadComponentConfig(this.PathMan);
                }
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in LoadSubComponents(string ComponentID)");
            }
        }
        /// <summary>
        /// Sets the name of the directory to be used as the source of data (subcomponents and clocks) that define the internals of the component.
        /// </summary>
        /// <param name="ComponentID">The ID of the component for which to load data.</param>
        /// <param name="Source">The name of the directory (in a search path) that contains the core data.</param>
        public void SetComponentSource(string ComponentID, string Source)
        {
            try
            {
                if (Components.ContainsKey(ComponentID))
                {
                    FalconMapping_Component fmc = Components[ComponentID];
                    fmc.Source = Source;
                }
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in SetComponentSource(string ComponentID, string Source)");
            }
        }
        #endregion

        #region Manipulate FPGAs Collection (Add/Remove/Modify/Get)
        /// <summary>
        /// Adds a new FPGA to the FPGAs collection of mapping objects with the specified ID, Name, 
        /// and Required Resources.  If the required resources are unknown or to be determined later,
        /// null should be passed for NewFPGAResources.  If the desired ID is already assigned to another
        /// FPGA in the system, an IDAlreadyExistsException will be thrown.
        /// </summary>
        /// <param name="NewFPGAID">The desired ID for the new FPGA.  If this value is equal to string.Empty, 
        /// a pseudo-random number will be generated for the FPGA ID.</param>
        /// <param name="NewFPGAName">The desired name for the new FPGA.  If this value is equal to string.Empty,
        /// the name will be "group" concatenated with the FPGA ID.</param>
        /// <param name="NewFPGAArch">The architecture family of this FPGA.</param>
        /// <param name="NewFPGAResources">If known, a ResourceInfo class containing the list of the resources
        /// required by this FPGA.  If not known, this argument should be null, and required resources
        /// will be assumed to be none.</param>
        /// <returns>The ID of the newly added FPGA.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        public string AddFPGA(string NewFPGAID, string NewFPGAName, string NewFPGAArch, ResourceInfo NewFPGAResources)
        {
            try
            {
                if (FPGAs.ContainsKey(NewFPGAID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", "FPGA", NewFPGAID));
                else
                {
                    if (NewFPGAID == string.Empty)
                    {
                        // Passed in a null string, generate a pseudo-random group ID
                        Random r = new Random();
                        NewFPGAID = r.Next((2 * FPGAs.Count) + 1).ToString();
                        while (FPGAs.ContainsKey(NewFPGAID))
                            NewFPGAID = r.Next((2 * FPGAs.Count) + 1).ToString();
                    }
                    if (NewFPGAName == string.Empty)
                    {
                        NewFPGAName = "fpga" + NewFPGAID;
                    }
                    FalconMapping_FPGA newFPGA = new FalconMapping_FPGA(NewFPGAID, NewFPGAName, NewFPGAArch);
                    newFPGA.PathManager = this.PathMan;
                    newFPGA.InitializeVortex(this.GetFPGAs().Count);
                    if (NewFPGAResources != null)
                    {
                        foreach (string resName in NewFPGAResources.GetResources().Keys)
                        {
                            newFPGA.SetTotalResource(resName, NewFPGAResources.GetResource(resName));

                        }
                    }
                    FPGAs.Add(NewFPGAID, newFPGA);
                    return NewFPGAID;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "AddFPGA()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Removes the FPGA with the specified ID from the FPGAs collection of mapping objects.
        /// If no FPGA exists with the specified ID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="ExFPGAID">The ID of the FPGA to be removed</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool RemoveFPGA(string ExFPGAID)
        {
            try
            {
                if (!FPGAs.ContainsKey(ExFPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", ExFPGAID));
                else
                {
                    RemoveFPGAFromCluster(ExFPGAID);
                    FalconMapping_FPGA fpga = (FalconMapping_FPGA)FPGAs[ExFPGAID];
                    List<FalconMapping_Group> grps = fpga.GetMappedGroups();
                    foreach (FalconMapping_Group grp in grps)
                    {
                        UnMapGroup(grp.ID);
                    }
                    FPGAs.Remove(ExFPGAID);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "RemoveFPGA()");
            }
            return false;
        }

        /// <summary>
        /// Attempts to change the ID assigned to a FPGA, given its current ID.   If no FPGA exists that
        /// is assigned the ID specified by CurrentFPGAID, an IDDoesNotExistException will be thrown.  If an FPGA
        /// already is assigned the ID specified by NewFPGAID, an IDAlreadyExistsException will be thrown.  
        /// In either case, the ID change will not occur.
        /// </summary>
        /// <param name="CurrentFPGAID">The current ID of the FPGA whose ID is to be changed.</param>
        /// <param name="NewFPGAID">The new desired ID of the FPGA.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyFPGAID(string CurrentFPGAID, string NewFPGAID)
        {
            try
            {
                if (!FPGAs.ContainsKey(CurrentFPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", CurrentFPGAID));
                else if (FPGAs.ContainsKey(NewFPGAID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", "FPGA", NewFPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[CurrentFPGAID];
                    FPGAs.Remove(CurrentFPGAID);
                    currentFPGA.ID = NewFPGAID;
                    FPGAs.Add(NewFPGAID, currentFPGA);
                    foreach (string linkID in Links.Keys)
                    {
                        FalconMapping_Link link = (FalconMapping_Link)Links[linkID];
                        if (link.SourceFPGA == CurrentFPGAID)
                            link.SourceFPGA = NewFPGAID;
                        if (link.SinkFPGA == CurrentFPGAID)
                            link.SinkFPGA = NewFPGAID;
                    }
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyFPGAID()");
            }
            return false;
        }

        /// <summary>
        /// Changes the name assigned to a FPGA, given its ID.   If no FPGA exists with the ID specified by
        /// FPGAID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="FPGAID">The ID of the FPGA whose name is to be changed.</param>
        /// <param name="NewFPGAName">The new desired name of the FPGA.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyFPGAName(string FPGAID, string NewFPGAName)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    currentFPGA.Name = NewFPGAName;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyFPGAName()");
            }
            return false;
        }

        /// <summary>
        /// Sets the total amount of particular resource on the FPGA specified by FPGAID. If no 
        /// FPGA exists with the ID specified by FPGAID, an IDDoesNotExistException will be thrown.  If changing the
        /// total resource amount causes already allocated resources on the FPGA to exceed the new total, an 
        /// InsufficientResourcesException will be thrown.
        /// </summary>
        /// <param name="FPGAID">The ID of the FPGA whose resource set is to be changed.</param>
        /// <param name="FPGAResource">The name of the resource to be changed.</param>
        /// <param name="NewAmount">The new total amount of the resource that on the FPGA.  This is NOT relative to the previous amount.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        /// <exception cref="Exceptions.InsufficientResourcesException">Thrown if the resource modification causes the allocated resources to exceed the total.</exception>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyFPGAResource(string FPGAID, string FPGAResource, long NewAmount)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    currentFPGA.SetTotalResource(FPGAResource, NewAmount);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyFPGAResource()");
            }
            return false;
        }

        /// <summary>
        /// Attempts to change the IP Address assigned to a FPGA, given its current ID.   If no FPGA exists that
        /// is assigned the ID specified by CurrentFPGAID, an IDDoesNotExistException will be thrown.  
        /// </summary>
        /// <param name="FPGAID">The current ID of the FPGA whose ID is to be changed.</param>
        /// <param name="NewAddress">The new desired ID of the FPGA.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyFPGAIPAddress(string FPGAID, string NewAddress)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    currentFPGA.IPAddress = NewAddress;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyFPGAIPAddress()");
            }
            return false;
        }/// <summary>
        /// Attempts to change the MAC Address assigned to a FPGA, given its current ID.   If no FPGA exists that
        /// is assigned the ID specified by CurrentFPGAID, an IDDoesNotExistException will be thrown.  
        /// </summary>
        /// <param name="FPGAID">The current ID of the FPGA whose ID is to be changed.</param>
        /// <param name="NewAddress">The new desired ID of the FPGA.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyFPGAMACAddress(string FPGAID, string NewAddress)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    currentFPGA.MACAddress = NewAddress;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyFPGAMACAddress()");
            }
            return false;
        }

        
        /// <summary>
        /// Returns the amount of a given resource that the specified FPGA has in total.  If no 
        /// FPGA exists with the ID specified by FPGAID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="FPGAID">The ID of the FPGA whose resource set is to be queried.</param>
        /// <param name="FPGAResource">The name of the resource to be examined.</param>
        /// <returns>The total integer amount of the specified resource on the FPGA on success, -1 otherwise.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public long GetFPGAResource(string FPGAID, string FPGAResource)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    return currentFPGA.GetTotalResource(FPGAResource);
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetFPGAResource()");
            }
            return 0;
        }

        /// <summary>
        /// Returns the current name assigned to specified FPGA.  If no FPGA exists with the ID 
        /// specified by FPGAID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="FPGAID">The ID of the FPGA whose name is to be returned.</param>
        /// <returns>The name of the FPGA on success, string.Empty otherwise.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public string GetFPGAName(string FPGAID)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    return currentFPGA.Name;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetFPGAName()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns the current IPaddress assigned to specified FPGA.  If no FPGA exists with the ID 
        /// specified by FPGAID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="FPGAID">The ID of the FPGA whose IP is to be returned.</param>
        /// <returns>The address of the FPGA on success, string.Empty otherwise.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public string GetFPGAIPAddress(string FPGAID)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    return currentFPGA.IPAddress;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetFPGAAddress()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns the current MAC address assigned to specified FPGA.  If no FPGA exists with the ID 
        /// specified by FPGAID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="FPGAID">The ID of the FPGA whose MAC is to be returned.</param>
        /// <returns>The address of the FPGA on success, string.Empty otherwise.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public string GetFPGAMACAddress(string FPGAID)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    return currentFPGA.MACAddress;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetFPGAMACAddress()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns the hardware architecture of the specified FPGA.  If no FPGA exists with the ID 
        /// specified by FPGAID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="FPGAID">The ID of the FPGA whose architecture is to be returned.</param>
        /// <returns>The architecture of the FPGA on success, string.Empty otherwise.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public string GetFPGAArchitecture(string FPGAID)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    return currentFPGA.Architecture;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetFPGAArchitecture()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Removes all clustering from FPGAs in the system, resulting in ALL FPGAs being unclustered.
        /// </summary>
        public void UnClusterAll()
        {
            try
            {
                foreach (string FPGAID in FPGAs.Keys)
                {
                    RemoveFPGAFromCluster(FPGAID);
                }
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in UnClusterAll()");
            }
        }

        /// <summary>
        /// Returns whether or not the FPGA is clustered.  If no FPGA exists with the ID 
        /// specified by FPGAID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="FPGAID">The ID of the FPGA to determine clustered status for</param>
        /// <returns>True if the FPGA exists and is clustered, false otherwise</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool IsFPGAClustered(string FPGAID)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    return currentFPGA.IsClustered;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "IsFPGAClustered()");
            }
            return false;
        }

        /// <summary>
        /// Returns the ID of the cluster to which the specified FPGA is clustered, if any.  If no FPGA 
        /// exists with the ID specified by FPGAID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="FPGAID">The ID of the FPGA to determine the cluster ID for, if any</param>
        /// <returns>The cluster ID of the cluster the FPGA is in, if the FPGA exists and is clustered, string.Empty otherwise</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public string GetFPGAClusterID(string FPGAID)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    if (currentFPGA.IsClustered)
                        return currentFPGA.ClusterID;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetFPGAClusterID()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the percentage of a given resource that is currently used on the specified FPGA.
        /// </summary>
        /// <param name="FPGAID">The ID of the FPGA whose utilization is to be calculated.</param>
        /// <param name="FPGAResource">The name of the resource whose utilization is to be calculated</param>
        /// <returns>A non-negative value indicating the fraction (0 to 1) of the specified resource that's currently used, if that resource
        /// exists in any amount on the FPGA.   If the specified FPGA ID is not found, the specified resource is not present on the FPGA, 
        /// or an error occurs, -1.0 is returned.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public double GetFPGAResourceUtilization(string FPGAID, string FPGAResource)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    double total;
                    double used;
                    total = currentFPGA.GetTotalResource(FPGAResource);
                    if (total > 0)
                    {
                        used = currentFPGA.GetUsedResource(FPGAResource);
                        return (used / total);
                    }
                    else
                    {
                        return -1.0F;
                    }                    
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetFPGAResourceUtilization()");
            }
            return -1.0F;
        }

        /// <summary>
        /// Gets the average percentage of all resources that are currently used on the specified FPGA.  This function ignores non-uniformity of resources
        /// across FPGAs in the platform. For a particular resource, the utilization on a single FPGA is based solely on what is available/used on THAT FPGA.
        /// If, for any reason, the FPGA is found to have no resources, it will not be counted.
        /// However, if it has resources, but none are used, it will count as 0% utilization.
        /// </summary>
        /// <param name="FPGAID">The name of the resource whose utilization is to be calculated</param>
        /// <returns>A non-negative value indicating the fraction (0 to 1) of all resources that are currently used on the specified FPGA. If the specified FPGA
        /// ID is not found, has no resources, or an error occurs, -1.0 is returned.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public double GetAverageFPGAResourceUtilization(string FPGAID)
        {
            try
            {
                if (!FPGAs.ContainsKey(FPGAID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", "FPGA", FPGAID));
                else
                {
                    FalconMapping_FPGA currentFPGA = (FalconMapping_FPGA)FPGAs[FPGAID];
                    double total;
                    double used;
                    double totalUtil = 0.0F;
                    int resCount = 0;
                    Dictionary<string, long> fpgaResources = currentFPGA.GetTotalResources();
                    foreach (string resName in fpgaResources.Keys)
                    {
                        total = (long)fpgaResources[resName];
                        if (total > 0)
                        {
                            used = currentFPGA.GetUsedResource(resName);
                            totalUtil = totalUtil + (used / total);
                        }
                        resCount++;         
                    }
                    if (resCount > 0)
                    {
                        return totalUtil / (double)resCount;
                    }
                    else
                    {
                        return -1.0F;
                    }
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetAverageFPGAResourceUtilization()");
            }
            return -1.0F;
        }

        /// <summary>
        /// Gets the average resource utilization of all FPGAs in the system.  If, for any reason, an FPGA is found to have no resources, it will not be counted.
        /// However, if it has resources, but none are used, it will count as 0% utilization.
        /// </summary>
        /// <returns>A non-negative value indicating the fraction (0 to 1) of all resources that are currently used in the system. If an error occurs, 
        /// -1.0 is returned.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public double GetAverageFPGAResourceUtilization()
        {
            try
            {
               double totalUtil = 0.0F;
               int fpgaCount = 0;
               foreach(string FPGAID in FPGAs.Keys)
               {
                   double thisUtil;
                   thisUtil = GetAverageFPGAResourceUtilization(FPGAID);
                   if (thisUtil > 0)
                   {
                       totalUtil = totalUtil + thisUtil;
                   }
                   fpgaCount++;
                }
               if (fpgaCount > 0)
               {
                   return totalUtil / (double)fpgaCount;
               }
               else
               {
                   return -1.0F;
               }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetAverageFPGAResourceUtilization()");
            }
            return -1.0F;
        }

        #endregion

        #region Manipulate Groups Collection (Add/Remove Components, Map/Unmap to/from FPGAs, Add/Remove/Modify/Get)

        /// <summary>
        /// Creates a new group for an ungrouped component.  If the component is already grouped, it is removed from its existing group to be placed in the new group.
        /// </summary>
        /// <param name="ComponentID">The ID of the component to create a group for.</param>
        /// <returns>The ID of the group to which the component is currently assigned.</returns>
        public string CreateNewGroupForComponent(string ComponentID)
        {
            try
            {
                if (IsComponentGrouped(ComponentID))
                {
                    RemoveComponentFromGroup(ComponentID);
                }
                int i = 0;
                string groupID = String.Format("group_{0}", i);
                while (Groups.ContainsKey(groupID))
                {
                    i++;
                    groupID = String.Format("group_{0}", i);
                }
                AddGroup(groupID, String.Format("Auto-generated group #{0}", i));
                //System.Diagnostics.Debug.WriteLine(String.Format("Auto-generated group {0} for ungrouped component {1}", groupID, ComponentID));
                AddComponentToGroup(ComponentID, groupID);
                return GetComponentGroupID(ComponentID);
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in CreateNewGroupForComponent()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Adds the component specified by NewComponentMemberID to the group specified by GroupID.  If either of the
        /// component or group IDs does not exist in their respective collections, an IDDoesNotExistException is thrown.
        /// If the component is already a member of another group, a ComponentAlreadyGroupedException is thrown.
        /// </summary>
        /// <param name="NewComponentMemberID">The ID of the component to be added to the group</param>
        /// <param name="GroupID">The ID of the group to which the component is to be added</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        /// <exception cref="Exceptions.ComponentAlreadyGroupedException">Thrown if the component being grouped is already grouped.</exception>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool AddComponentToGroup(string NewComponentMemberID, string GroupID)
        {
            FalconMapping_Group workingGroup = null;
            FalconMapping_Component workingComponent = null;
            try
            {
                if (!Groups.ContainsKey(GroupID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", 
                        "Group", GroupID));
                else
                {
                    workingGroup = (FalconMapping_Group)Groups[GroupID];
                    if (!Components.ContainsKey(NewComponentMemberID))
                    {
                        throw new Exceptions.IDDoesNotExistException(
                            String.Format("{0} ID {1} does not exist", 
                            "Component", NewComponentMemberID));
                    }
                    else
                    {
                        workingComponent = (FalconMapping_Component)Components[NewComponentMemberID];
                        if ((workingComponent.IsGrouped) && (workingComponent.GroupID != GroupID))
                            throw new Exceptions.ComponentAlreadyGroupedException(
                                String.Format("Unable to add component {0} to group {1}.  Component {0} is already a member of group {2}",
                                workingComponent.ID, GroupID, workingComponent.GroupID));

                        if (workingGroup.IsMapped)
                        {
                            FalconMapping_FPGA fpga = (FalconMapping_FPGA)FPGAs[workingGroup.TargetFPGA];
                            if (fpga.HasSufficientResources(workingComponent))
                            {
                                fpga.UnMapGroup(workingGroup);
                                workingGroup.AddComponent(workingComponent);
                                fpga.MapGroup(workingGroup);
                                return true;
                            }
                            else
                            {
                                StringBuilder ResourceMessage = new StringBuilder();
                                ResourceInfo Available = fpga.GetAvailableResourceInfo();
                                ResourceInfo Required = workingComponent.Resources;
                                ResourceInfo Infra = fpga.InfrastructureCostToSupport(workingComponent);
                                Infra.Add(Required);
                                ResourceMessage.AppendFormat("{0} : {1} / {2}\n", "Resource", "Needed", "Available");
                                foreach (string ResKey in Infra.GetResources().Keys)
                                {
                                    ResourceMessage.AppendFormat("'{0}' : {1} / {2}\n", ResKey, Infra.GetResource(ResKey), Available.GetResource(ResKey));
                                }
                                throw new Exceptions.InsufficientResourcesException(
                                        String.Format("Unable to add component {0} to group {1} due to insufficient physical resources on target FPGA.\n{2}",
                                        workingComponent.ID, workingGroup.ID, ResourceMessage.ToString()));
                            }
                        }
                        else
                        {
                            workingGroup.AddComponent(workingComponent);
                            return true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (workingGroup == null)
                    InternalExceptionPreProcessor(ex, "AddComponentToGroup() - Invalid Group ID");
                else if (workingComponent == null)
                    InternalExceptionPreProcessor(ex, "AddComponentToGroup() - Invalid Component ID");
                else
                    InternalExceptionPreProcessor(ex, "AddComponentToGroup()");
            }
            return false;
        }

        /// <summary>
        /// Removes the component specified by ExComponentMemberID from the group to which it is associated.  If the
        /// specified component ID, or the ID of the group to which it is associated does not exist, an IDDoesNotExist
        /// exception is thrown.   If the component is not already grouped, nothing is changed.
        /// </summary>
        /// <param name="ExComponentMemberID">The ID of the component to be removed from it's group.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool RemoveComponentFromGroup(string ExComponentMemberID)
        {
            FalconMapping_Group workingGroup = null;
            FalconMapping_Component workingComponent = null;
            try
            {
                if (!Components.ContainsKey(ExComponentMemberID))
                    throw new Exceptions.IDDoesNotExistException(
                        String.Format("{0} ID {1} does not exist", 
                        "Component", ExComponentMemberID));
                else
                {
                    workingComponent = (FalconMapping_Component)Components[ExComponentMemberID];
                    if (!workingComponent.IsGrouped)
                        return true; // throw new Exceptions.ComponentNotGroupedException();
                    else
                    {
                        if (!Groups.ContainsKey(workingComponent.GroupID))
                        {
                            throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist (from Component.GroupID)", 
                                "Group", workingComponent.GroupID));
                        }
                        else
                        {
                            workingGroup = (FalconMapping_Group)Groups[workingComponent.GroupID];
                            if (workingGroup.IsMapped)
                            {
                                FalconMapping_FPGA fpga = (FalconMapping_FPGA)FPGAs[workingGroup.TargetFPGA];
                                try
                                {
                                    fpga.UnMapGroup(workingGroup);
                                    workingGroup.RemoveComponent(workingComponent);
                                    fpga.MapGroup(workingGroup);
                                }
                                catch (System.Exception ex)
                                {
                                    InternalExceptionPreProcessor(ex, "RemoveComponentFromGroup() - An error occured while attemping to update FPGA resources during removal.  The mapping state may be unstable.");
                                }
                                return true;
                            }
                            else
                            {
                                workingGroup.RemoveComponent(workingComponent);
                                return true;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (workingComponent == null)
                    InternalExceptionPreProcessor(ex, "RemoveComponentFromGroup() - Invalid Component ID");
                else if ((workingGroup == null) && (workingComponent.IsGrouped))
                    InternalExceptionPreProcessor(ex, "RemoveComponentFromGroup() - Invalid Group ID");
                else
                    InternalExceptionPreProcessor(ex, "RemoveComponentFromGroup()");
            }
            return false;
        }

        /// <summary>
        /// Maps the group specified by GroupID to the FPGA specified by TargetFPGAID.  If either of the
        /// target FPGA or group IDs does not exist in their respective collections, an IDDoesNotExistException is thrown.
        /// If the group is already mapped to another FPGA, a GroupAlreadyMappedException is thrown.  If there are 
        /// insufficient resources available on the FPGA, an InsufficientResourcesException is thrown.
        /// </summary>
        /// <param name="GroupID">The ID of the group which is to be mapped to the target FPGA.</param>
        /// <param name="TargetFPGAID">The ID of the FPGA to which the group is to be mapped.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.InsufficientResourcesException">Thrown if the resource modification causes the allocated resources to exceed the total.</exception>
        /// <exception cref="Exceptions.GroupAlreadyMappedException">Thrown if the component being grouped is already grouped.</exception>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool MapGroupToFPGA(string GroupID, string TargetFPGAID)
        {
            FalconMapping_Group workingGroup = null;
            FalconMapping_FPGA workingFPGA = null;
            try
            {
                if (!Groups.ContainsKey(GroupID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist", 
                                "Group", GroupID));
                else
                {
                    workingGroup = (FalconMapping_Group)Groups[GroupID];
                    if ((workingGroup.IsMapped) && (workingGroup.TargetFPGA != TargetFPGAID))
                        throw new Exceptions.GroupAlreadyMappedException(
                                String.Format("Unable to map group {0} to FPGA {1}.  Group {0} is already mapped to FPGA {2}",
                                workingGroup.ID, TargetFPGAID, workingGroup.TargetFPGA));
                    if (!FPGAs.ContainsKey(TargetFPGAID))
                    {
                        throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "FPGA", TargetFPGAID));
                    }
                    else
                    {
                        workingFPGA = (FalconMapping_FPGA)FPGAs[TargetFPGAID];
                        workingFPGA.MapGroup(workingGroup);
                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (workingGroup == null)
                    InternalExceptionPreProcessor(ex, "MapGroupToFPGA() - Invalid Group ID");
                else if ((workingFPGA == null) && (!workingGroup.IsMapped))
                    InternalExceptionPreProcessor(ex, "MapGroupToFPGA() - Invalid FPGA ID");
                else
                    InternalExceptionPreProcessor(ex, "MapGroupToFPGA()");
            }
            return false;
        }

        /// <summary>
        /// Removes the Group specified by GroupID from the FPGA to which it is mapped.  If the
        /// specified Group ID, or the ID of the FPGA to which it is mapped does not exist, an IDDoesNotExist
        /// exception is thrown.   If the group is not already mapped, nothing is changed.
        /// </summary>
        /// <param name="GroupID">The ID of the group to be unmapped from it's FPGA.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool UnMapGroup(string GroupID)
        {

            FalconMapping_Group workingGroup = null;
            FalconMapping_FPGA workingFPGA = null;
            try
            {
                if (!Groups.ContainsKey(GroupID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Group", GroupID));
                else
                {
                    workingGroup = (FalconMapping_Group)Groups[GroupID];
                    if (!workingGroup.IsMapped)
                        return true; // throw new Exceptions.GroupNotMappedException();
                    else
                    {
                        if (!FPGAs.ContainsKey(workingGroup.TargetFPGA))
                        {
                            throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist (from Group.TargetFPGA)",
                                "Group", workingGroup.TargetFPGA));
                        }
                        else
                        {
                            workingFPGA = (FalconMapping_FPGA)FPGAs[workingGroup.TargetFPGA];
                            workingFPGA.UnMapGroup(workingGroup);
                            return true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (workingGroup == null)
                    InternalExceptionPreProcessor(ex, "UnMapGroup() - Invalid Group ID");
                else if ((workingFPGA == null) && (!workingGroup.IsMapped))
                    InternalExceptionPreProcessor(ex, "UnMapGroup() - Invalid FPGA ID");
                else
                    InternalExceptionPreProcessor(ex, "UnMapGroup()");
            }
            return false;
        }




        /// <summary>
        /// Adds a new empty group to the Groups collection of mapping objects with the specified ID and Name. 
        /// If the desired ID is already assigned to another Group in the system, an IDAlreadyExistsException will be thrown.
        /// </summary>
        /// <param name="NewGroupID">The desired ID for the new group.  If this value is equal to string.Empty, 
        /// a pseudo-random number will be generated for the group ID.</param>
        /// <param name="NewGroupName">The desired name for the new group.  If this value is equal to string.Empty,
        /// the name will be "group" concatenated with the group ID.</param>
        /// <returns>The ID of the newly added Group if successful, string.Empty otherwise.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the ID already exists.</exception>
        public string AddGroup(string NewGroupID, string NewGroupName)
        {
            try
            {
                if (Groups.ContainsKey(NewGroupID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", "Group", NewGroupID));
                else
                {
                    if (NewGroupID == string.Empty)
                    {
                        // Passed in a null string, generate a pseudo-random group ID
                        Random r = new Random();
                        NewGroupID = r.Next((2 * Groups.Count) + 1).ToString();
                        while (Groups.ContainsKey(NewGroupID))
                            NewGroupID = r.Next((2 * Groups.Count) + 1).ToString();
                    }
                    if (NewGroupName == string.Empty)
                    {
                        NewGroupName = "group" + NewGroupID;
                    }

                    FalconMapping_Group newGroup = new FalconMapping_Group(NewGroupID, NewGroupName);
                    Groups.Add(NewGroupID, newGroup);
                    return NewGroupID;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "AddGroup()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Removes the FPGA with the specified ID from the FPGAs collection of mapping objects.
        /// If no FPGA exists with the specified ID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="ExGroupID">The ID of the FPGA to be removed</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool RemoveGroup(string ExGroupID)
        {
            try
            {
                if (!Groups.ContainsKey(ExGroupID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Group", ExGroupID));
                else
                {
                    FalconMapping_Group workingGroup = (FalconMapping_Group)Groups[ExGroupID];
                    if (workingGroup.IsMapped)
                        UnMapGroup(workingGroup.ID);

                    workingGroup.RemoveAll();
                    Groups.Remove(ExGroupID);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "RemoveGroup()");
            }
            return false;
        }


        /// <summary>
        /// Attempts to change the ID assigned to a group, given its current ID.   If no group exists that
        /// is assigned the ID specified by CurrentGroupID, an IDDoesNotExistException will be thrown.  If a group is 
        /// already is assigned the ID specified by CurrentGroupID, an IDAlreadyExistsException will be thrown.  
        /// In either case, the ID change will not occur.
        /// </summary>
        /// <param name="CurrentGroupID">The current ID of the group whose ID is to be changed.</param>
        /// <param name="NewGroupID">The new desired ID of the group.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyGroupID(string CurrentGroupID, string NewGroupID)
        {
            try
            {
                if (!Groups.ContainsKey(CurrentGroupID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Group", CurrentGroupID));
                else if (Groups.ContainsKey(NewGroupID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", "Group", NewGroupID));
                else
                {
                    FalconMapping_Group currentGroup = (FalconMapping_Group)Groups[CurrentGroupID];
                    Groups.Remove(CurrentGroupID);
                    currentGroup.ID = NewGroupID;
                    Groups.Add(NewGroupID, currentGroup);
                    foreach (string compID in Components.Keys)
                    {
                        FalconMapping_Component comp = (FalconMapping_Component)Components[compID];
                        if (comp.GroupID == CurrentGroupID)
                            comp.GroupID = NewGroupID;
                    }
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyGroupID()");
            }
            return false;
        }

        /// <summary>
        /// Changes the name assigned to a group, given its ID.   If no group exists with the ID specified by
        /// GroupID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="GroupID">The ID of the group whose name is to be changed.</param>
        /// <param name="NewGroupName">The new desired name of the group.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyGroupName(string GroupID, string NewGroupName)
        {
            try
            {
                if (!Groups.ContainsKey(GroupID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Group", GroupID));
                else
                {
                    FalconMapping_Group currentGroup = (FalconMapping_Group)Groups[GroupID];
                    currentGroup.Name = NewGroupName;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyGroupName()");
            }
            return false;
        }

        /// <summary>
        /// Returns the current name assigned to specified group.  If no group exists with the ID 
        /// specified by GroupID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="GroupID">The ID of the group whose name is to be returned.</param>
        /// <returns>The name of the group if successful, string.Empty otherwise.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public string GetGroupName(string GroupID)
        {
            try
            {
                if (!Groups.ContainsKey(GroupID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Group", GroupID));
                else
                {
                    FalconMapping_Group currentGroup = (FalconMapping_Group)Groups[GroupID];
                    return currentGroup.Name;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetGroupName()");
            }
            return string.Empty;
        }


        /// <summary>
        /// Removes all mappings in the system, resulting in all groups being unmapped.
        /// </summary>
        public void UnMapAll()
        {
            try
            {
                foreach (string GroupID in Groups.Keys)
                {
                    UnMapGroup(GroupID);
                }
            }
            catch (Exception ex)
            {
                InternalExceptionPreProcessor(ex, "Caught in UnMapAll()");
            }
        }

        /// <summary>
        /// Returns whether or not the Group is mapped.  If no group exists with the ID 
        /// specified by GroupID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="GroupID">The ID of the group to determine mapped status for</param>
        /// <returns>True if the group exists and is mapped, false otherwise</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool IsGroupMapped(string GroupID)
        {
            try
            {
                if (!Groups.ContainsKey(GroupID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Group", GroupID));
                else
                {
                    FalconMapping_Group currentGroup = (FalconMapping_Group)Groups[GroupID];
                    return currentGroup.IsMapped;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "IsGroupMapped()");
            }
            return false;
        }

        /// <summary>
        /// Returns the ID of the FPGA to which the specified group is mapped, if any.  If no group 
        /// exists with the ID specified by GroupID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="GroupID">The ID of the group to determine the mapped FPGA ID for, if any</param>
        /// <returns>The FPGA ID of the FPGA the group is mapped to, if the group exists and is mapped, string.Empty otherwise</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public string GetGroupTargetFPGAID(string GroupID)
        {
            try
            {
                if (!Groups.ContainsKey(GroupID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Group", GroupID));
                else
                {
                    FalconMapping_Group currentGroup = (FalconMapping_Group)Groups[GroupID];
                    if (currentGroup.IsMapped)
                        return currentGroup.TargetFPGA;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetGroupTargetFPGAID()");
            }
            return string.Empty;
        }

        #endregion

        #region Manipulate Clusters Collection (Add/Remove FPGAs, Add/Remove/Modify/Get)

        /// <summary>
        /// Adds the FPGA specified by NewFPGAMemberID to the cluster specified by ClusterID.  If either of the
        /// FPGA or cluster IDs does not exist in their respective collections, an IDDoesNotExistException is thrown.
        /// If the FPGA is already a member of another cluster, an FPGAAlreadyClusteredException is thrown.
        /// </summary>
        /// <param name="NewFPGAMemberID">The ID of the FPGA to be added to the cluster</param>
        /// <param name="ClusterID">The ID of the cluster to which the FPGA is to be added</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.FPGAAlreadyClusteredException">Thrown if the FPGA being clustered is already clustered.</exception>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool AddFPGAToCluster(string NewFPGAMemberID, string ClusterID)
        {
            FalconMapping_Cluster workingCluster = null;
            FalconMapping_FPGA workingFPGA = null;
            try
            {
                if (!Clusters.ContainsKey(ClusterID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Cluster", ClusterID));
                else
                {
                    workingCluster = (FalconMapping_Cluster)Clusters[ClusterID];
                    if (!FPGAs.ContainsKey(NewFPGAMemberID))
                    {
                        throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "FPGA", NewFPGAMemberID));
                    }
                    else
                    {
                        workingFPGA = (FalconMapping_FPGA)FPGAs[NewFPGAMemberID];
                        if ((workingFPGA.IsClustered) && (workingFPGA.ClusterID != ClusterID))
                            throw new Exceptions.FPGAAlreadyClusteredException(
                                String.Format("Unable to add FPGA {0} to cluster {1}.  FPGA {0} is already a member of cluster {2}",
                                workingFPGA.ID, ClusterID, workingFPGA.ClusterID));
                        workingCluster.AddFPGA(workingFPGA);
                        return true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (workingCluster == null)
                    InternalExceptionPreProcessor(ex, "AddFPGAToCluster() - Invalid Cluster ID");
                else if (workingFPGA == null)
                    InternalExceptionPreProcessor(ex, "AddFPGAToCluster() - Invalid FPGA ID");
                else
                    InternalExceptionPreProcessor(ex, "AddFPGAToCluster()");
            }
            return false;
        }

        /// <summary>
        /// Removes the FPGA specified by ExFPGAMemberID from the cluster to which it is associated.  If the
        /// specified FPGA ID, or the ID of the cluster to which it is associated does not exist, an IDDoesNotExist
        /// exception is thrown.   If the FPGA is not already clustered, nothing is changed.
        /// </summary>
        /// <param name="ExFPGAMemberID">The ID of the component to be removed from it's group.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool RemoveFPGAFromCluster(string ExFPGAMemberID)
        {
            FalconMapping_Cluster workingCluster = null;
            FalconMapping_FPGA workingFPGA = null;
            try
            {
                if (!FPGAs.ContainsKey(ExFPGAMemberID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "FPGA", ExFPGAMemberID));
                else
                {
                    workingFPGA = (FalconMapping_FPGA)FPGAs[ExFPGAMemberID];
                    if (!workingFPGA.IsClustered)
                        return true; // throw new Exceptions.ComponentNotGroupedException();
                    else
                    {
                        // This is wrong, but leave it to figure out why I'm getting problems with exception handling
                        // if (!Clusters.ContainsKey(workingFPGA.ClusterID))
                        if (!Clusters.ContainsKey(workingFPGA.ClusterID))
                        {
                            throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist (from FPGA.ClusterID)",
                                "Cluster", workingFPGA.ClusterID));
                        }
                        else
                        {
                            workingCluster = (FalconMapping_Cluster)Clusters[workingFPGA.ClusterID];
                            workingCluster.RemoveFPGA(workingFPGA);
                            return true;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                if (workingCluster == null)
                    InternalExceptionPreProcessor(ex, "RemoveFPGAFromCluster() - Invalid Cluster ID");
                else if (workingFPGA == null)
                    InternalExceptionPreProcessor(ex, "RemoveFPGAFromCluster() - Invalid FPGA ID");
                else
                    InternalExceptionPreProcessor(ex, "RemoveFPGAFromCluster()");
            }
            return false;
        }


        /// <summary>
        /// Adds a new empty cluster to the Clusters collection of mapping objects with the specified ID and Name. 
        /// If the desired ID is already assigned to another cluster in the system, an IDAlreadyExistsException will be thrown.
        /// </summary>
        /// <param name="NewClusterID">The desired ID for the new cluster.  If this value is equal to string.Empty, 
        /// a pseudo-random number will be generated for the cluster ID.</param>
        /// <param name="NewClusterName">The desired name for the new Cluster.  If this value is equal to string.Empty,
        /// the name will be "cluster" concatenated with the cluster ID.</param>
        /// <returns>The ID of the newly added Cluster if successful, string.Empty otherwise.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        public string AddCluster(string NewClusterID, string NewClusterName)
        {
            try
            {
                if (Clusters.ContainsKey(NewClusterID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", "Cluster", NewClusterID));
                else
                {
                    if (NewClusterID == string.Empty)
                    {
                        // Passed in a null string, generate a pseudo-random Cluster ID
                        Random r = new Random();
                        NewClusterID = r.Next((2 * Clusters.Count) + 1).ToString();
                        while (Clusters.ContainsKey(NewClusterID))
                            NewClusterID = r.Next((2 * Clusters.Count) + 1).ToString();
                    }
                    if (NewClusterName == string.Empty)
                    {
                        NewClusterName = "Cluster" + NewClusterID;
                    }

                    FalconMapping_Cluster newCluster = new FalconMapping_Cluster(NewClusterID, NewClusterName);
                    Clusters.Add(NewClusterID, newCluster);
                    return NewClusterID;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "AddCluster()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Removes the cluster with the specified ID from the Clusters collection of mapping objects.
        /// If no cluster exists with the specified ID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="ExClusterID">The ID of the Cluster to be removed</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool RemoveCluster(string ExClusterID)
        {
            try
            {
                if (!Clusters.ContainsKey(ExClusterID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Cluster", ExClusterID));
                else
                {
                    FalconMapping_Cluster workingCluster = (FalconMapping_Cluster)Clusters[ExClusterID];
                    workingCluster.RemoveAll();
                    Clusters.Remove(ExClusterID);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "RemoveCluster()");
            }
            return false;
        }


        /// <summary>
        /// Attempts to change the ID assigned to a cluster, given its current ID.   If no cluster exists that
        /// is assigned the ID specified by CurrentClusterID, an IDDoesNotExistException will be thrown.  If a cluster is 
        /// already is assigned the ID specified by CurrentClusterID, an IDAlreadyExistsException will be thrown.  
        /// In either case, the ID change will not occur.
        /// </summary>
        /// <param name="CurrentClusterID">The current ID of the cluster whose ID is to be changed.</param>
        /// <param name="NewClusterID">The new desired ID of the cluster.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyClusterID(string CurrentClusterID, string NewClusterID)
        {
            try
            {
                if (!Clusters.ContainsKey(CurrentClusterID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Cluster", CurrentClusterID));
                else if (Clusters.ContainsKey(NewClusterID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", "Cluster", NewClusterID));
                else
                {
                    FalconMapping_Cluster currentCluster = (FalconMapping_Cluster)Clusters[CurrentClusterID];
                    Clusters.Remove(CurrentClusterID);
                    currentCluster.ID = NewClusterID;
                    Clusters.Add(NewClusterID, currentCluster);
                    foreach (string fpgaID in FPGAs.Keys)
                    {
                        FalconMapping_FPGA fpga = (FalconMapping_FPGA)FPGAs[fpgaID];
                        if (fpga.ClusterID == CurrentClusterID)
                            fpga.ClusterID = NewClusterID;
                    }
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyClusterID()");
            }
            return false;
        }

        /// <summary>
        /// Changes the name assigned to a cluster, given its ID.   If no cluster exists with the ID specified by
        /// ClusterID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="ClusterID">The ID of the cluster whose name is to be changed.</param>
        /// <param name="NewClusterName">The new desired name of the cluster.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyClusterName(string ClusterID, string NewClusterName)
        {
            try
            {
                if (!Clusters.ContainsKey(ClusterID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Cluster", ClusterID));
                else
                {
                    FalconMapping_Cluster currentCluster = (FalconMapping_Cluster)Clusters[ClusterID];
                    currentCluster.Name = NewClusterName;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyClusterName()");
            }
            return false;
        }

        /// <summary>
        /// Returns the current name assigned to specified cluster.  If no cluster exists with the ID 
        /// specified by ClusterID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="ClusterID">The ID of the cluster whose name is to be returned.</param>
        /// <returns>The name of the cluster if successful, string.Empty otherwise.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public string GetClusterName(string ClusterID)
        {
            try
            {
                if (!Clusters.ContainsKey(ClusterID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Cluster", ClusterID));
                else
                {
                    FalconMapping_Cluster currentcluster = (FalconMapping_Cluster)Clusters[ClusterID];
                    return currentcluster.Name;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "GetClusterName()");
            }
            return string.Empty;
        }

        #endregion

        #region Manipulate Connections Collection (Add/Remove/Modify/Get)

        /// <summary>
        /// Adds a new connections to the Connections collection of mapping objects with the specified ID, Name, 
        /// Data Density, and Source/Sink Components.  If the desired ID is already assigned to another
        /// connection in the system, an IDAlreadyExistsException will be thrown.
        /// </summary>
        /// <param name="NewConnectionID">The desired ID for the new connection.  If this value is equal to string.Empty, 
        /// a pseudo-random number will be generated for the connection ID</param>
        /// <param name="NewConnectionName">The desired ID for the new connection.  If this value is equal to string.Empty, 
        /// the name will be the names of the source and sink components joined with a dash.</param>
        /// <param name="NewDataDensity">The density of data sent across the new connection.</param>
        /// <param name="SourceComponentID">The ID identifying the Source component for this connection.</param>
        /// <param name="SinkComponentID">The ID identifying the Sink component for this connection.</param>
        /// <returns>The ID of the newly added Connection if successful, string.Empty otherwise.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        public string AddConnection(string NewConnectionID, string NewConnectionName, double NewDataDensity, string SourceComponentID, string SinkComponentID)
        {
            try
            {
                if (NewDataDensity <= 0.0F)
                    throw new Exceptions.InvalidConnectionDensityException(
                        String.Format("Data Density specified for new connection must be non-zero and non-negative."));

                if (Connections.ContainsKey(NewConnectionID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", 
                        "Connection", NewConnectionID));
                else
                {
                    if (!Components.ContainsKey(SourceComponentID))
                        throw new Exceptions.ConnectionSourceDoesNotExistException(
                                String.Format("Component Source (ID {0}) for new Connection does not exist.",
                                SourceComponentID));
                    if (!Components.ContainsKey(SinkComponentID))
                        throw new Exceptions.ConnectionSinkDoesNotExistException(
                                String.Format("Component Sink (ID {0}) for Connection does not exist.",
                                SinkComponentID));

                    if (NewConnectionID == string.Empty)
                    {
                        // Passed in a null string, generate a pseudo-random Connection ID
                        Random r = new Random();
                        NewConnectionID = r.Next((2 * Connections.Count) + 1).ToString();
                        while (Connections.ContainsKey(NewConnectionID))
                            NewConnectionID = r.Next((2 * Connections.Count) + 1).ToString();
                    }
                    if (NewConnectionName == string.Empty)
                    {
                        FalconMapping_Component src = (FalconMapping_Component)Components[SourceComponentID];
                        FalconMapping_Component snk = (FalconMapping_Component)Components[SinkComponentID];
                        NewConnectionName = src.Name + "-" + snk.Name;
                    }

                    FalconMapping_Connection newConnection = new FalconMapping_Connection(NewConnectionID, SourceComponentID, SinkComponentID);
                    newConnection.DataDensity = NewDataDensity;
                    newConnection.Name = NewConnectionName;
                    Connections.Add(NewConnectionID, newConnection);
                    NormalizeConnectionDensities();
                    return NewConnectionID;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "AddConnection()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Removes the connection specified by ExConnectionID from the system.  If no connection exists with the ID specified by
        /// ConnectionID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="ExConnectionID">The ID of the connection to remove.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool RemoveConnection(string ExConnectionID)
        {
            try
            {
                if (!Connections.ContainsKey(ExConnectionID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Connection", ExConnectionID));
                else
                {
                    Connections.Remove(ExConnectionID);
                    NormalizeConnectionDensities();
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "RemoveConnection()");
            }
            return false;
        }

        /// <summary>
        /// Attempts to change the ID assigned to a connection, given its current ID.   If no connection exists that
        /// is assigned the ID specified by CurrentConnectionID, an IDDoesNotExistException will be thrown.  If a connection
        /// already is assigned the ID specified by NewConnectionID, an IDAlreadyExistsException will be thrown.  
        /// In either case, the ID change will not occur.
        /// </summary>
        /// <param name="CurrentConnectionID">The current ID of the connection whose ID is to be changed.</param>
        /// <param name="NewConnectionID">The new desired ID of the connection.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyConnectionID(string CurrentConnectionID, string NewConnectionID)
        {
            try
            {
                if (!Connections.ContainsKey(CurrentConnectionID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Connection", CurrentConnectionID));
                else if (Connections.ContainsKey(NewConnectionID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", "Connection", NewConnectionID));
                else
                {
                    FalconMapping_Connection currentConnection = (FalconMapping_Connection)Connections[CurrentConnectionID];
                    Connections.Remove(CurrentConnectionID);
                    currentConnection.ID = NewConnectionID;
                    Connections.Add(NewConnectionID, currentConnection);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyConnectionID()");
            }
            return false;
        }

        /// <summary>
        /// Changes the name assigned to a connection, given its ID.   If no connection exists with the ID specified by
        /// ConnectionID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="ConnectionID">The ID of the connection whose name is to be changed.</param>
        /// <param name="NewConnectionName">The new desired name of the connection.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyConnectionName(string ConnectionID, string NewConnectionName)
        {
            try
            {
                if (!Connections.ContainsKey(ConnectionID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Connection", ConnectionID));
                else
                {
                    FalconMapping_Connection currentConnection = (FalconMapping_Connection)Connections[ConnectionID];
                    currentConnection.Name = NewConnectionName;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyConnectionName()");
            }
            return false;
        }

        /// <summary>
        /// Changes the data density of a connection, given its ID.   If no connection exists with the ID specified by
        /// ConnectionID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="ConnectionID">The ID of the connection whose data density is to be changed.</param>
        /// <param name="NewDensity">The new data density of the connection.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyConnectionDensity(string ConnectionID, double NewDensity)
        {
            try
            {
                if (!Connections.ContainsKey(ConnectionID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Connection", ConnectionID));
                else
                {
                    FalconMapping_Connection currentConnection = (FalconMapping_Connection)Connections[ConnectionID];
                    currentConnection.DataDensity = NewDensity;
                    NormalizeConnectionDensities();
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyConnectionDensity()");
            }
            return false;
        }

        /// <summary>
        /// Changes the source component of a connection, given its ID.   If no connection exists with the ID specified by
        /// ConnectionID, an IDDoesNotExistException will be thrown.  If no component exists with the ID specified by 
        /// NewSourceComponentID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="ConnectionID">The ID of the connection whose source component is to be changed.</param>
        /// <param name="NewSourceComponentID">The ID of the new source component for the connection.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyConnectionSource(string ConnectionID, string NewSourceComponentID)
        {
            try
            {
                if (!Connections.ContainsKey(ConnectionID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Connection", ConnectionID));
                else
                {
                    if (!Components.ContainsKey(NewSourceComponentID))
                        throw new Exceptions.ConnectionSourceDoesNotExistException(
                                String.Format("New Component Source (ID {0}) for Connection ID {1} does not exist.",
                                NewSourceComponentID, ConnectionID));

                    FalconMapping_Connection currentConnection = (FalconMapping_Connection)Connections[ConnectionID];
                    currentConnection.SourceComponent = NewSourceComponentID;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyConnectionSource()");
            }
            return false;
        }

        /// <summary>
        /// Changes the sink component of a connection, given its ID.   If no connection exists with the ID specified by
        /// ConnectionID, an IDDoesNotExistException will be thrown.  If no component exists with the ID specified by 
        /// NewSinkComponentID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="ConnectionID">The ID of the connection whose sink component is to be changed.</param>
        /// <param name="NewSinkComponentID">The ID of the new sink component for the connection.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyConnectionSink(string ConnectionID, string NewSinkComponentID)
        {
            try
            {
                if (!Connections.ContainsKey(ConnectionID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Connection", ConnectionID));
                else
                {
                    if (!Components.ContainsKey(NewSinkComponentID))
                        throw new Exceptions.ConnectionSinkDoesNotExistException(
                                String.Format("New Component Sink (ID {0}) for Connection ID {1} does not exist.",
                                NewSinkComponentID, ConnectionID));

                    FalconMapping_Connection currentConnection = (FalconMapping_Connection)Connections[ConnectionID];
                    currentConnection.SinkComponent = NewSinkComponentID;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyConnectionSink()");
            }
            return false;
        }

        #endregion

        #region Manipulate Links Collection (Add/Remove/Modify/Get)
        /// <summary>
        /// Adds a new link to the Links collection of mapping objects with the specified ID, Name, 
        /// Link Speed, Source/Sink FPGAs and directionality.  If the desired ID is already assigned to another
        /// link in the system, an IDAlreadyExistsException will be thrown.
        /// </summary>
        /// <param name="NewLinkID">The desired ID for the new link.  If this value is equal to string.Empty, 
        /// a pseudo-random number will be generated for the link ID</param>
        /// <param name="NewLinkName">The desired ID for the new link.  If this value is equal to string.Empty, 
        /// the name will be the names of the source and sink FPGAs joined with a dash.</param>
        /// <param name="NewSpeed">The speed of the new link.</param>
        /// <param name="SourceFPGAID">The ID identifying the Source FPGA for this link.</param>
        /// <param name="SinkFPGAID">The ID identifying the Sink FPGA for this link.</param>
        /// <param name="Bidirectional">Boolean indicating whether the new link is bidirectional.</param>
        /// <returns>The ID of the newly added Link.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        public string AddLink(string NewLinkID, string NewLinkName, double NewSpeed, string SourceFPGAID, string SinkFPGAID, bool Bidirectional)
        {
            try
            {
                if (NewSpeed <= 0.0F)
                    throw new Exceptions.InvalidLinkSpeedException(
                        String.Format("Link Speed specified for new link must be non-zero and non-negative."));

                if (Links.ContainsKey(NewLinkID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", 
                        "Link", NewLinkID));
                else
                {
                    if (!FPGAs.ContainsKey(SourceFPGAID))
                        throw new Exceptions.LinkSourceDoesNotExistException(
                                String.Format("FPGA Source (ID {0}) for new Link does not exist.",
                                SourceFPGAID));
                    if (!FPGAs.ContainsKey(SinkFPGAID))
                        throw new Exceptions.LinkSinkDoesNotExistException(
                                String.Format("FPGA Sink (ID {0}) for Link does not exist.",
                                SinkFPGAID));

                    if (NewLinkID == string.Empty)
                    {
                        // Passed in a null string, generate a pseudo-random Link ID
                        Random r = new Random();
                        NewLinkID = r.Next((2 * Links.Count) + 1).ToString();
                        while (Links.ContainsKey(NewLinkID))
                            NewLinkID = r.Next((2 * Links.Count) + 1).ToString();
                    }
                    if (NewLinkName == string.Empty)
                    {
                        FalconMapping_FPGA src = (FalconMapping_FPGA)FPGAs[SourceFPGAID];
                        FalconMapping_FPGA snk = (FalconMapping_FPGA)FPGAs[SinkFPGAID];
                        NewLinkName = src.Name + "-" + snk.Name;
                    }

                    FalconMapping_Link newLink = new FalconMapping_Link(NewLinkID, SourceFPGAID, SinkFPGAID);
                    newLink.LinkSpeed = NewSpeed;
                    newLink.Name = NewLinkName;
                    newLink.Bidirectional = Bidirectional;
                    Links.Add(NewLinkID, newLink);
                    NormalizeLinkSpeeds();
                    return NewLinkID;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "AddLink()");
            }
            return string.Empty;
        }

        /// <summary>
        /// Removes the link specified by ExLinkID from the system.
        /// If no link exists with the ID specified by LinkID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="ExLinkID">The ID of the link to remove.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool RemoveLink(string ExLinkID)
        {
            try
            {
                if (!Links.ContainsKey(ExLinkID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Link", ExLinkID));
                else
                {
                    Links.Remove(ExLinkID);
                    NormalizeLinkSpeeds();
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "RemoveLink()");
            }
            return false;
        }

        /// <summary>
        /// Attempts to change the ID assigned to a link, given its current ID.   If no link exists that
        /// is assigned the ID specified by CurrentLinkID, an IDDoesNotExistException will be thrown.  If a link
        /// already is assigned the ID specified by NewLinkID, an IDAlreadyExistsException will be thrown.  
        /// In either case, the ID change will not occur.
        /// </summary>
        /// <param name="CurrentLinkID">The current ID of the link whose ID is to be changed.</param>
        /// <param name="NewLinkID">The new desired ID of the link.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDAlreadyExistsException">Thrown if the new ID already exists.</exception>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyLinkID(string CurrentLinkID, string NewLinkID)
        {
            try
            {
                if (!Links.ContainsKey(CurrentLinkID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Link", CurrentLinkID));
                else if (Links.ContainsKey(NewLinkID))
                    throw new Exceptions.IDAlreadyExistsException(
                        String.Format("{0} ID {1} already exists", 
                        "Link", NewLinkID));
                else
                {
                    FalconMapping_Link currentLink = (FalconMapping_Link)Links[CurrentLinkID];
                    Links.Remove(CurrentLinkID);
                    currentLink.ID = NewLinkID;
                    Links.Add(NewLinkID, currentLink);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyLinkID()");
            }
            return false;
        }

        /// <summary>
        /// Changes the name assigned to a link, given its ID.   If no link exists with the ID specified by
        /// LinkID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="LinkID">The ID of the link whose name is to be changed.</param>
        /// <param name="NewLinkName">The new desired name of the link.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyLinkName(string LinkID, string NewLinkName)
        {
            try
            {
                if (!Links.ContainsKey(LinkID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Link", LinkID));
                else
                {
                    FalconMapping_Link currentLink = (FalconMapping_Link)Links[LinkID];
                    currentLink.Name = NewLinkName;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyLinkName()");
            }
            return false;
        }

        /// <summary>
        /// Changes the speed of a link, given its ID.   If no link exists with the ID specified by
        /// LinkID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="LinkID">The ID of the link whose speed is to be changed.</param>
        /// <param name="NewSpeed">The new speed of the link.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyLinkSpeed(string LinkID, double NewSpeed)
        {
            try
            {
                if (!Links.ContainsKey(LinkID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Link", LinkID));
                else
                {
                    FalconMapping_Link currentLink = (FalconMapping_Link)Links[LinkID];
                    currentLink.LinkSpeed = NewSpeed;
                    NormalizeLinkSpeeds();
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyLinkDensity()");
            }
            return false;
        }

        /// <summary>
        /// Changes the source FPGA of a link, given its ID.   If no link exists with the ID specified by
        /// LinkID, an IDDoesNotExistException will be thrown.  If no FPGA exists with the ID specified by 
        /// NewSourceFPGAID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="LinkID">The ID of the link whose source FPGA is to be changed.</param>
        /// <param name="NewSourceFPGAID">The ID of the new source FPGA for the link.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyLinkSource(string LinkID, string NewSourceFPGAID)
        {
            try
            {
                if (!Links.ContainsKey(LinkID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Link", LinkID));
                else
                {
                    if (!FPGAs.ContainsKey(NewSourceFPGAID))
                        throw new Exceptions.LinkSourceDoesNotExistException(
                                String.Format("New FPGA Source (ID {0}) for Link ID {1} does not exist.",
                                NewSourceFPGAID, LinkID));

                    FalconMapping_Link currentLink = (FalconMapping_Link)Links[LinkID];
                    currentLink.SourceFPGA = NewSourceFPGAID;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyLinkSource()");
            }
            return false;
        }

        /// <summary>
        /// Changes the sink FPGA of a link, given its ID.   If no link exists with the ID specified by
        /// LinkID, an IDDoesNotExistException will be thrown.  If no FPGA exists with the ID specified by 
        /// NewSinkFPGAID, an IDDoesNotExistException will be thrown
        /// </summary>
        /// <param name="LinkID">The ID of the link whose sink FPGA is to be changed.</param>
        /// <param name="NewSinkFPGAID">The ID of the new sink FPGA for the link.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyLinkSink(string LinkID, string NewSinkFPGAID)
        {
            try
            {
                if (!Links.ContainsKey(LinkID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Link", LinkID));
                else
                {
                    if (!FPGAs.ContainsKey(NewSinkFPGAID))
                        throw new Exceptions.LinkSinkDoesNotExistException(
                                String.Format("New FPGA Sink (ID {0}) for Link ID {1} does not exist.",
                                NewSinkFPGAID, LinkID));

                    FalconMapping_Link currentLink = (FalconMapping_Link)Links[LinkID];
                    currentLink.SinkFPGA = NewSinkFPGAID;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyLinkSink()");
            }
            return false;
        }

        /// <summary>
        /// Changes the bidirectionality of a link, given its ID.   If no link exists with the ID specified by
        /// LinkID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="LinkID">The ID of the link whose bidirectionality is to be changed.</param>
        /// <param name="Bidirectional">True if the link should be set to bidirectional, false otherwise.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyLinkBidirectionality(string LinkID, bool Bidirectional)
        {
            try
            {
                if (!Links.ContainsKey(LinkID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Link", LinkID));
                else
                {
                    FalconMapping_Link currentLink = (FalconMapping_Link)Links[LinkID];
                    currentLink.Bidirectional = Bidirectional;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyLinkDirectionality()");
            }
            return false;
        }

        /// <summary>
        /// Reverses the direction of a link, given its ID, by swapping its Source and Sink IDs.   
        /// If no link exists with the ID specified by LinkID, an IDDoesNotExistException will be thrown.
        /// </summary>
        /// <param name="LinkID">The ID of the link whose directionality is to be reversed.</param>
        /// <returns>True on success, False on failure.</returns>
        /// <exception cref="Exceptions.IDDoesNotExistException">Thrown if the ID does not exist.</exception>
        public bool ModifyLinkReverse(string LinkID)
        {
            try
            {
                if (!Links.ContainsKey(LinkID))
                    throw new Exceptions.IDDoesNotExistException(
                                String.Format("{0} ID {1} does not exist",
                                "Link", LinkID));
                else
                {
                    FalconMapping_Link currentLink = (FalconMapping_Link)Links[LinkID];
                    string temp = currentLink.SourceFPGA;
                    currentLink.SourceFPGA = currentLink.SinkFPGA;
                    currentLink.SinkFPGA = temp;
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                InternalExceptionPreProcessor(ex, "ModifyLinkReverse()");
            }
            return false;
        }


        #endregion

        #region FalconGlobal.IFalconLibrary Implementation

        /// <summary>
        /// Returns the name of this library component.  Implementation of FalconGlobal.IFalconLibrary.FalconComponentName.
        /// </summary>
        public string FalconComponentName
        {
            get
            {
                return "Falcon Component Mapping System";
            }
        }

        /// <summary>
        /// Returns the version of this library component.  Implementation of FalconGlobal.IFalconLibrary.FalconComponentVersion.
        /// </summary>
        public string FalconComponentVersion
        {
            get
            {
                return "1.0.0";
            }
        }

        /// <summary>
        /// Returns the name/version/copyright information of this library component.  Implementation of 
        /// FalconGlobal.IFalconLibrary.GetFalconComponentVersion().
        /// </summary>
        public string GetFalconComponentVersion()
        {
            return String.Format("{0} {1} Copyright (c) 2010 PennState", FalconComponentName, FalconComponentVersion);
        }

        #endregion
    }
}
