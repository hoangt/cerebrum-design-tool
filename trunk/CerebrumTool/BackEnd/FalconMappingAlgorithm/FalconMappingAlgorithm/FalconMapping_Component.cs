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
 * FalconMapping_Component.cs
 * Name: Matthew Cotter
 * Date: 2 Apr 2010 
 * Description: This class implements the methods and properties required to represent
 * a logical component for use during the Component-to-FPGA mapping algorithm.
 * History: 
 * >> (16 Feb 2010) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Added CerebrumCore object as internal reference for loading properties, component cores, managing clocks, 
 *                                          and loading component definition from a single source.
 *                                      Modified code to use improved properties system for loading, saving, and managing core configurations.
 * >> ( 7 Feb 2011) Matthew Cotter: Corrected bug in VortexTypes property which lists types of Vortex interfaces exposed by the component.
 * >> (27 Jan 2011) Matthew Cotter: Corrected issue in which platform-required components were not properly reading and accounting for their required resources.
 * >> (18 Jan 2011) Matthew Cotter: Added support for GUI-added platform components overriding those instantiated by the platform.
 * >> (22 Dec 2010) Matthew Cotter: Added additional support for customizable clock management.
 * >> (16 Dec 2010) Matthew Cotter: Added and implemented multi-SAP support to load, identifiy and correctly attach all Vortex interfaces exposed by component.
 * >> ( 1 Dec 2010) Matthew Cotter: Integration of Multiple-SAP Components into mapping.
 * >> (18 Oct 2010) Matthew Cotter: Continued integration of Vortex-based SAP/SOP communication infrastructure
 *                                  Added SAP/SOP type signifiers to components.
 *                                  Added Vortex SAP/SOP property to components.
 * >> (11 Oct 2010) Matthew Cotter: Added handling of subcomponent PCores for integration into Vortex-based SAP/SOP communication infrastructure
 * >> ( 7 Oct 2010) Matthew Cotter: Changes required due to migration of ResourceInfo & ResourceSet structures to separate library.
 *                                  Converted generic Hashtables to TypedHashtables<> for readability and verifiability.
 * >> ( 7 Jul 2010) Matthew Cotter: Initialize Port property to -1, as an invalid (or not required) value.
 * >> ( 6 Jul 2010) Matthew Cotter: Added Port property to be used for external communication with the Component on the FPGA platform.
 * >> ( 6 May 2010) Matthew Cotter: Added functionality to store/calculate Distance from I/O
 *                                  Added property to return the group ID, if any, the component is mapped to
 * >> (18 Apr 2010) Matthew Cotter: Implemented Highest/Lowest Resource Functions
 * >> ( 2 Apr 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;
using System.Diagnostics;
using CerebrumNetronObjects;
using CerebrumSharedClasses;
using FalconResources;
using FalconClockManager;
using FalconPathManager;
using VortexInterfaces;
using VortexObjects;
using VortexInterfaces.VortexCommon;

namespace FalconMappingAlgorithm
{
    /// <summary>
    /// Represents a logical component in the system design.
    /// 
    /// </summary>
    public class FalconMapping_Component
    {
        private string strID;
        private string strName;
        private bool bIsGrouped;
        private string sGroupID;

        private int iDistanceFromInput;
        private List<string> _SupportedArchitectures;


        /// <summary>
        /// Creates a new FalconMapping_Component object representing a logical component in the design.
        /// </summary>
        /// <param name="sID">The ID of the new component.</param>
        /// <param name="sName">The Name of the new component.</param>
        /// <param name="sArches">A comma-delimited string listing all FPGA architectures this component supports; with an empty string indicating that all are supported.</param>
        public FalconMapping_Component(string sID, string sName, string sArches)
        {
            strID = sID;
            strName = sName;
            iDistanceFromInput = -1;
            Port = -1;
            ParseSupportedArchitectures(sArches);
        }

        #region Properties 

        /// <summary>
        /// Get or set the ID of this component.
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
                strID = value;
            }
        }
        
        /// <summary>
        /// Get or set the name of this component.
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
        /// Get or set a boolean flag indicating whether this component has grouped.
        /// Note: This property should only be written by the AddComponent() call of the FalconMapping_Group object this
        /// component is being added to.  Modifying this property in another situation could create an unpredictable state.
        /// </summary>
        public bool IsGrouped
        {
            get
            {
                return bIsGrouped;
            }
            set
            {
                bIsGrouped = value;
            }
        }
        
        /// <summary>
        /// Get or set the component's copy of the ID of the group of which it is a member.  
        /// If it's not grouped, this should return string.Empty.
        /// Note: This property should only be written by the AddComponent() call of the FalconMapping_Group object this
        /// component is being added to.  Modifying this property in another situation could create an unpredictable state.
        /// </summary>
        public string GroupID
        {
            get
            {
                return sGroupID;
            }
            set
            {
                sGroupID = value;
            }
        }

        /// <summary>
        /// Gets the pre-calculated minimum number of logical connections between this component and nearest input component.
        /// If this value is 0, it indicates that this component is an input component.
        /// If this value is less than 0, it indicates that distance calculation has not, been or could not be done for this component.
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
        /// Gets the ResourceInfo object containing required resources for this component, including any required NIFs
        /// </summary>
        public ResourceInfo Resources
        {
            get
            {
                ResourceInfo RIRequired = new ResourceInfo();
                foreach (KeyValuePair<string, long> Pair in InternalComponentObject.Resources)
                {
                    RIRequired.SetResource(Pair.Key, Pair.Value);
                }

                foreach (IVortexAttachment IVA in InternalComponentObject.VortexDevices)
                {
                    if (IVA.NIF != null)
                    {
                        RIRequired.Add(IVA.NIF.GetResources());
                    }
                    else if (IVA is IVortexBridgeAttachment)
                    {
                        VortexBridgeAttachment VBA = IVA as VortexBridgeAttachment;
                        RIRequired.Add(VBA.GetResources());
                    }
                }
                return RIRequired;
            }
        }

        /// <summary>
        /// Get or set the Port number assigned to this Component.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Get or set the list of architecture families supported by this component.  (i.e. virtex4, virtex5, virtex6, etc)
        /// If this list is empty (Count == 0) then the core is supported on all architectures.  
        /// </summary>
        public List<string> SupportedArchitectures
        {
            get
            {
                return _SupportedArchitectures;
            }
            set
            {
                _SupportedArchitectures = value;
            }
        }
        
        /// <summary>
        /// Parse the list of supported FPGA architectures from the specified string.
        /// </summary>
        /// <param name="ArchString">The comma-delimited string list of supported FPGA architectures.</param>
        private void ParseSupportedArchitectures(string ArchString)
        {
            ArchString = ArchString.Trim();
            ArchString = ArchString.Trim(',');
            List<string> SupArches = new List<string>();
            string[] Arches = ArchString.Split(',');
            for (int i = 0; i < Arches.Length; i++)
            {
                Arches[i] = Arches[i].Trim();
                if (!SupArches.Contains(Arches[i]))
                    SupArches.Add(Arches[i]);
            }
            _SupportedArchitectures = SupArches;
        }

        /// <summary>
        /// Gets or sets the name of the folder in which this core's source can be found
        /// </summary>
        public string Source { get; set; }
        #endregion

        /// <summary>
        /// Function to forcibly clear the associated group information from a component. 
        /// Note: This function is only intended to be used as cleanup in the event that this component cannot be properly ungrouped with
        /// a call to its associated group's RemoveComponent() function.  Calling this in another situation could create an unpredictable state.
        /// </summary>
        public void ClearGroup()
        {
            bIsGrouped = false;
            sGroupID = String.Empty;
        }

        #region Resource Management
        /// <summary>
        /// Sets the amount required of a particular resource for the component.
        /// </summary>
        /// <param name="ResourceName">Name of the resource.</param>
        /// <param name="ResourceAmount">Value indicating how much of the specified resource is required.</param>
        public void SetRequiredResource(string ResourceName, long ResourceAmount)
        {
        }

        /// <summary>
        /// Gets a Hashtable set of the amount required of a all resources for the component.
        /// </summary>
        /// <returns>The set of all resources required for the component.</returns>
        public Dictionary<string, long> GetRequiredResources()
        {
            return this.Resources.GetResources();
        }

        /// <summary>
        /// Gets the amount required of a particular resource for the component.
        /// </summary>
        /// <param name="ResourceName">Name of the resource.</param>
        /// <returns>The amount of the specified resource required for the component.</returns>
        public long GetRequiredResource(string ResourceName)
        {
            return this.Resources.GetResource(ResourceName);
        }

        /// <summary>
        /// Returns the name of the most demanding resource for this component.
        /// </summary>
        /// <returns>The name of the most demanding resource for this component.</returns>
        public string GetHighestResourceName()
        {
            string RName = string.Empty;
            long RValue = 0;
            this.Resources.GetHighestResource(ref RName, ref RValue);
            return RName;
        }
        
        /// <summary>
        /// Returns the amount of the most demanding resource for this component.
        /// </summary>
        /// <returns>The amount of the most demanding resource for this component.</returns>
        public long GetHighestResourceAmount()
        {
            string RName = string.Empty;
            long RValue = 0;
            this.Resources.GetHighestResource(ref RName, ref RValue);
            return RValue;
        }
        
        /// <summary>
        /// Returns the name of the least demanding resource for this component
        /// </summary>
        /// <returns>The name of the least demanding resource for this component</returns>
        public string GetLowestResourceName()
        {
            string RName = string.Empty;
            long RValue = 0;
            this.Resources.GetLowestResource(ref RName, ref RValue);
            return RName;
        }
        
        /// <summary>
        /// Returns the amount of the least demanding resource for this component.
        /// </summary>
        /// <returns>The amount of the least demanding resource within this group.</returns>
        public long GetLowestResourceAmount()
        {
            string RName = string.Empty;
            long RValue = 0;
            this.Resources.GetLowestResource(ref RName, ref RValue);
            return RValue;
        }
        #endregion

        #region Top-Level Component Property Handling

        private CerebrumCore _InternalComponentObject;
        /// <summary>
        /// The CerebrumCore object that this FalconMapping_Component represents.
        /// </summary>
        internal CerebrumCore InternalComponentObject
        {
            get
            {
                return _InternalComponentObject;
            }
            set
            {
                _InternalComponentObject = value;
            }
        }

        /// <summary>
        /// Exposes the collection containing the properties and parameters of the component.
        /// </summary>
        public CerebrumPropertyCollection Properties
        {
            get
            {
                return InternalComponentObject.Properties;
            }
        }

        /// <summary>
        /// Translates parameters within the specified condition string, in the context of the core's parameters and its defaults
        /// </summary>
        /// <param name="Input">Input condition string</param>
        /// <returns>Translated string to be evaluated.</returns>
        public string TranslateString(string Input)
        {
            string Output = Input;
            if (Output == null)
                return String.Empty;
            if (Output == string.Empty)
                return Output;
            foreach (CerebrumPropertyEntry CPE in this.InternalComponentObject.Properties.GetEntries(CerebrumPropertyTypes.CEREBRUMPROPERTY))
            {
                if ((CPE.PropertyName == null) || (CPE.PropertyValue == string.Empty))
                    continue;
                if (Output.Contains(CPE.PropertyName))
                    Output = Output.Replace(CPE.PropertyName, CPE.PropertyValue);
            }
            return Output;
        }
        /// <summary>
        /// Sets the value of a specified property to the specified value
        /// </summary>
        /// <param name="PropertyName">The name of the property</param>
        /// <param name="PropertyValue">The new value of the property</param>
        public void SetPropertyValue(string PropertyName, string PropertyValue)
        {
            InternalComponentObject.Properties.SetValue(CerebrumPropertyTypes.CEREBRUMPROPERTY, PropertyName, PropertyValue, true);
        }
        /// <summary>
        /// Gets the current value of the specified property
        /// </summary>
        /// <param name="PropertyName">The name of the property</param>
        /// <returns>The value of the property</returns>
        public string GetPropertyValue(string PropertyName)
        {
            return (string)InternalComponentObject.Properties.GetValue(CerebrumPropertyTypes.CEREBRUMPROPERTY, PropertyName);
        }

        /// <summary>
        /// Saves the properties and parameters for the top-level component and all subcomponents.
        /// </summary>
        /// <param name="PathMan">The project path manager used to locate the configuration files.</param>
        public void SaveComponentConfig(PathManager PathMan)
        {
            InternalComponentObject.SaveCoreConfigs(PathMan["LocalProjectRoot"]);
        }
        /// <summary>
        /// Loads the properties and parameters for the top-level component and all subcomponents.
        /// </summary>
        /// <param name="PathMan">The project path manager used to locate the configuration files.</param>
        public void LoadComponentConfig(PathManager PathMan)
        {
            InternalComponentObject.LoadCoreConfigs(PathMan["LocalProjectRoot"]);
        }
        /// <summary>
        /// Get a list of clocks signals that feed into this component
        /// </summary>
        public List<ClockSignal> InputClocks
        {
            get
            {
                return InternalComponentObject.InputClocks;
            }
        }
        /// <summary>
        /// Get a list of clocks signals that are generated (output) by this component
        /// </summary>
        public List<ClockSignal> OutputClocks
        {
            get
            {
                return InternalComponentObject.OutputClocks;
            }
        }

        internal Dictionary<string, ComponentCore> ComponentCores
        {
            get
            {
                return InternalComponentObject.ComponentCores;
            }
        }

        /// <summary>
        /// Loads the Component Source for this component from the paths specified in the path manager.
        /// </summary>
        /// <param name="PathMan">The project path manager from which the component should be located.</param>
        internal void LoadComponentSource(PathManager PathMan)
        {
            if ((this.Source == null) || (this.Source == string.Empty))
                return;
            FileInfo SourcePath = CoreLibrary.LocateCoreDefinition(PathMan, this.Source);
            if ((SourcePath != null) && (SourcePath.Exists))
            {
                InternalComponentObject = CoreLibrary.LoadCoreDefinition(SourcePath, true, null, this.ID);
            }
        }
        /// <summary>
        /// Locates the Component Source for this component from the paths specified in the path manager.
        /// </summary>
        /// <param name="PathMan">The project path manager from which the component should be located.</param>
        private string LocateComponentSource(PathManager PathMan)
        {
            string SourcePath = string.Empty;
            // Scan core repositories from the Paths file
            List<string> SearchPaths = new List<string>();
            SearchPaths.Add(String.Format("{0}\\{1}", PathMan["Platforms"], PathMan["ProjectPlatform"])); // Look in the Platform Directory
            SearchPaths.Add(PathMan["CerebrumCores"]); // Look in the CerebrumCores Directory

            if (PathMan.HasPath("CoreSearchPaths"))
            {
                string[] OtherSearchPaths = PathMan["CoreSearchPaths"].Split(';');
                SearchPaths.AddRange(OtherSearchPaths);
            }
            foreach (string SearchPath in SearchPaths)
            {
                if (Directory.Exists(String.Format("{0}", SearchPath)))
                {
                    if (Directory.Exists(String.Format("{0}\\{1}", SearchPath, this.Source)))
                    {
                        if (File.Exists(String.Format("{0}\\{1}\\{1}.xml", SearchPath, this.Source)))
                        {
                            SourcePath = String.Format("{0}\\{1}\\{1}.xml", SearchPath, this.Source);
                            break;
                        }
                    }
                }
            }
            if (!File.Exists(SourcePath))
            {
                return string.Empty;
            }
            else
            {
                return SourcePath;
            }
        }

        //private void ParseComponentSource(string SourceXMLPath)
        //{
        //    XmlDocument xDoc = new XmlDocument();
        //    FileInfo DocumentPath = new FileInfo(SourceXMLPath);

        //    xDoc.Load(SourceXMLPath);
        //    XmlNode xHardware = CerebrumXmlInterface.GetXmlNode(xDoc, "CerebrumCore.Hardware");

        //    #region Parse Hardware Resources And Architectures
        //    XmlNode xResources = CerebrumXmlInterface.GetXmlNode(xHardware, "Resources");
        //    if (xResources != null)
        //    {
        //        foreach (XmlNode xn in xResources.ChildNodes)
        //        {
        //            if (xn.NodeType != XmlNodeType.Element)
        //                continue;
        //            if (String.Compare(xn.Name, "Resource", true) == 0)
        //            {
        //                string RName = string.Empty;
        //                long RAmt = 0;
        //                foreach (XmlAttribute xAttr in xn.Attributes)
        //                {
        //                    if (String.Compare(xAttr.Name, "Name", true) == 0)
        //                    {
        //                        RName = xAttr.Value;
        //                    }
        //                    else if (String.Compare(xAttr.Name, "Amount", true) == 0)
        //                    {
        //                        long val;
        //                        if (long.TryParse(xAttr.Value, out val))
        //                        {
        //                            RAmt = val;
        //                        }
        //                        else
        //                        {
        //                            Trace.WriteLine(String.Format("Error parsing Resource string -- CerebrumCore.Hardware.Resources.Resource in ",
        //                                DocumentPath.FullName));
        //                        }
        //                    }
        //                }
        //                if ((RName != string.Empty) && (RAmt > 0))
        //                    this.Resources.SetResource(RName, RAmt);
        //            }
        //        }
        //    }

        //    XmlNode xArches = CerebrumXmlInterface.GetXmlNode(xHardware, "SupportedArchitectures");
        //    this.SupportedArchitectures.Clear();
        //    if (xArches != null)
        //    {
        //        foreach (XmlNode xn in xArches.ChildNodes)
        //        {
        //            if (xn.NodeType != XmlNodeType.Element)
        //                continue;
        //            if (String.Compare(xn.Name, "Arch", true) == 0)
        //            {
        //                this.SupportedArchitectures.Add(xn.InnerText);
        //            }
        //        }
        //    }
        //    //cCore.CoreHWLocation = CerebrumXmlInterface.GetXmlInnerText(xHardware, "Sources.PCore");
        //    #endregion

        //    #region Parse Component PCores

        //    XmlNode xHWInterface = CerebrumXmlInterface.GetXmlNode(xDoc, "CerebrumCore.Hardware");
        //    foreach(XmlNode xVortexInterface in xHWInterface.ChildNodes)
        //    {
        //        if (string.Compare(xVortexInterface.Name, "interface", true) == 0)
        //        {
        //            VortexInterfaces.VortexCommon.VortexAttachmentType vType = VortexInterfaces.VortexCommon.VortexAttachmentType.SAP;
        //            string InterfaceInstance = xVortexInterface.InnerText;

        //            foreach (XmlAttribute xAttr in xVortexInterface.Attributes)
        //            {
        //                if (String.Compare(xAttr.Name, "Type", true) == 0)
        //                {
        //                    if (String.Compare(xAttr.Value, "SOP", true) == 0)
        //                    {
        //                        vType = VortexInterfaces.VortexCommon.VortexAttachmentType.SOP;
        //                    }
        //                    else if (String.Compare(xAttr.Value, "SAP", true) == 0)
        //                    {
        //                        vType = VortexInterfaces.VortexCommon.VortexAttachmentType.SAP;
        //                    }
        //                }
        //            }
        //            AddVortexAttachmentInstance(String.Format("{0}_{1}", this.ID, InterfaceInstance), vType);
        //        }
        //    }
        //    XmlNode xPCores = CerebrumXmlInterface.GetXmlNode(xDoc, "CerebrumCore.Hardware.PCores");
        //    if (xPCores != null)
        //    {
        //        foreach (XmlNode xn in xPCores.ChildNodes)
        //        {
        //            if (xn.NodeType != XmlNodeType.Element)
        //                continue;
        //            if (String.Compare(xn.Name, "PCore", true) == 0)
        //            {
        //                string xPCType = CerebrumXmlInterface.GetXmlAttribute(xn, "", "Type");
        //                string xPCVersion = CerebrumXmlInterface.GetXmlAttribute(xn, "", "Version");
        //                string xPCInst = CerebrumXmlInterface.GetXmlAttribute(xn, "", "Instance");
        //                string xPCSource = CerebrumXmlInterface.GetXmlAttribute(xn, "", "Source");
        //                string xPCUCF = CerebrumXmlInterface.GetXmlAttribute(xn, "", "UCF");
        //                string xPCValid = CerebrumXmlInterface.GetXmlAttribute(xn, "", "Valid");

        //                if ((xPCSource != string.Empty) && (xPCSource != null))
        //                    xPCSource = (String.Format("{0}\\{1}", this.Source, xPCSource));
        //                else
        //                    xPCSource = string.Empty;

        //                if (xPCUCF == null)
        //                    xPCUCF = string.Empty;

        //                AddSubPCore(xPCType, xPCInst, xPCVersion, xPCSource, xPCUCF, xPCValid);
        //            }
        //        }
        //    }
        //    #endregion

        //    #region Parse Core Clocks
        //    try
        //    {
        //        XmlNode xClocks = CerebrumXmlInterface.GetXmlNode(xHardware, "Clocks");
        //        foreach (XmlNode xClock in xClocks.ChildNodes)
        //        {
        //            if (xClock.NodeType != XmlNodeType.Element)
        //                continue;
        //            if (String.Compare(xClock.Name, "Clock", true) == 0)
        //            {
        //                string xCLKMatch = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Match");
        //                if ((xCLKMatch == null) || (xCLKMatch == string.Empty))
        //                {
        //                    string xCLKDesc = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Name");
        //                    string xCLKDir = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Direction");
        //                    string xCLKCore = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Core");
        //                    string xCLKPort = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Port");
        //                    string xCLKFreq = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Frequency");
        //                    string xCLKPhase = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Phase");
        //                    string xCLKGroup = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Group");
        //                    string xCLKBuff = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Buffered");

        //                    bool IsOutputClock = false;

        //                    xCLKDir = xCLKDir.ToLower();
        //                    if (xCLKDir.StartsWith("o"))
        //                        IsOutputClock = true;
        //                    else
        //                        IsOutputClock = false;

        //                    // Set defaults for missing values
        //                    if ((xCLKPhase == null) || (xCLKPhase == string.Empty))
        //                        xCLKPhase = "0";
        //                    if ((xCLKGroup == null) || (xCLKGroup == string.Empty))
        //                        xCLKGroup = "NONE";
        //                    if ((xCLKBuff == null) || (xCLKBuff == string.Empty))
        //                        xCLKBuff = "true";

        //                    // Parse Frequency
        //                    xCLKFreq = xCLKFreq.ToLower();
        //                    xCLKFreq = xCLKFreq.Replace("ghz", "000mhz");
        //                    xCLKFreq = xCLKFreq.Replace("mhz", "000khz");
        //                    xCLKFreq = xCLKFreq.Replace("khz", "000");
        //                    long frequency = 0;
        //                    if (!long.TryParse(xCLKFreq, out frequency))
        //                    {
        //                        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                        //OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                    }
        //                    // Parse phase
        //                    int phase = 0;
        //                    if (!int.TryParse(xCLKPhase, out phase))
        //                    {
        //                        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                        //OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                    }

        //                    // Parse Clock Group
        //                    ClockGroup group = ClockGroup.NONE;
        //                    try
        //                    {
        //                        group = (ClockGroup)Enum.Parse(typeof(ClockGroup), xCLKGroup);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ErrorReporting.DebugException(ex);
        //                        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                        //OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                    }

        //                    // Parse buffer state
        //                    bool buffered = true;
        //                    if (!bool.TryParse(xCLKBuff, out buffered))
        //                    {
        //                        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                        //OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                    }

        //                    ClockSignal CS = new ClockSignal();
        //                    CS.Frequency = frequency;
        //                    CS.Phase = phase;
        //                    CS.Group = group;
        //                    CS.Buffered = buffered;
        //                    CS.IsOutput = IsOutputClock;
        //                    CS.Name = xCLKDesc;
        //                    CS.ComponentInstance = this.ID;
        //                    CS.CoreInstance = xCLKCore;
        //                    CS.PortName = xCLKPort;
        //                    if (CS.IsOutput)
        //                        this.OutputClocks.Add(CS);
        //                    else
        //                        this.InputClocks.Add(CS);
        //                }
        //            }
        //        }
        //        // Process dependent clocks last
        //        foreach (XmlNode xClock in xClocks.ChildNodes)
        //        {
        //            if (xClock.NodeType != XmlNodeType.Element)
        //                continue;
        //            if (String.Compare(xClock.Name, "Clock", true) == 0)
        //            {
        //                string xCLKMatch = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Match");
        //                if ((xCLKMatch != null) && (xCLKMatch != string.Empty))
        //                {
        //                    string xCLKDesc = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Name");
        //                    string xCLKDir = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Direction");
        //                    string xCLKCore = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Core");
        //                    string xCLKPort = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Port");
        //                    string xCLKPhase = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Phase");
        //                    string xCLKRatio = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Ratio");

        //                    bool IsOutputClock = false;

        //                    xCLKDir = xCLKDir.ToLower();
        //                    if (xCLKDir.StartsWith("o"))
        //                        IsOutputClock = true;
        //                    else
        //                        IsOutputClock = false;

        //                    // Set defaults for missing values
        //                    if ((xCLKPhase == null) || (xCLKPhase == string.Empty))
        //                        xCLKPhase = "0";
        //                    if ((xCLKRatio == null) || (xCLKRatio == string.Empty))
        //                        xCLKRatio = "1.0";

        //                    double ratio = 0;
        //                    if (!double.TryParse(xCLKRatio, out ratio))
        //                    {
        //                        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                        //OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                    }
        //                    // Parse phase
        //                    int phase = 0;
        //                    if (!int.TryParse(xCLKPhase, out phase))
        //                    {
        //                        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                        //OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //                    }

        //                    ClockSignal CS = null;

        //                    if (IsOutputClock)
        //                    {
        //                        foreach (ClockSignal ClkSig in this.OutputClocks)
        //                        {
        //                            if ((String.Compare(ClkSig.CoreInstance, xCLKCore, true) == 0) &&
        //                                (String.Compare(ClkSig.PortName, xCLKPort, true) == 0))
        //                            {
        //                                CS = new ClockSignal(ClkSig);
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        foreach (ClockSignal ClkSig in this.InputClocks)
        //                        {
        //                            if ((String.Compare(ClkSig.CoreInstance, xCLKCore, true) == 0) &&
        //                                (String.Compare(ClkSig.PortName, xCLKMatch, true) == 0))
        //                            {
        //                                CS = new ClockSignal(ClkSig);
        //                                CS.Frequency = (long)(CS.Frequency * ratio);
        //                                CS.Phase = (CS.Phase + phase);
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    if (CS != null)
        //                    {
        //                        CS.IsOutput = IsOutputClock;
        //                        CS.Name = xCLKDesc;
        //                        CS.ComponentInstance = this.ID;
        //                        CS.CoreInstance = xCLKCore;
        //                        CS.PortName = xCLKPort;
        //                        if (CS.IsOutput)
        //                            this.OutputClocks.Add(CS);
        //                        else
        //                            this.InputClocks.Add(CS);
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorReporting.DebugException(ex);
        //        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //        //OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}", DocumentPath.FullName));
        //    }
        //    #endregion

        //    #region Parse Core Resets
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorReporting.DebugException(ex);
        //        Trace.WriteLine(String.Format("Error parsing core resets specification -- CerebrumCore.Hardware.Resets in {0}", DocumentPath.FullName));
        //        //OnCoreError(cCore, String.Format("Error parsing core resets specification -- CerebrumCore.Hardware.Resets in {0}", DocumentPath.FullName));
        //    }
        //    #endregion

        //    #region Parse Core External Reset Signals
            
        //    XmlNode xResets = CerebrumXmlInterface.GetXmlNode(xDoc, "CerebrumCore.Hardware.Resets");
        //    if (xResets != null)
        //    {
        //        foreach (XmlNode xReset in xResets.ChildNodes)
        //        {
        //            if (xReset.NodeType != XmlNodeType.Element)
        //                continue;
        //            if (String.Compare(xReset.Name, "Reset", true) == 0)
        //            {
        //                string CoreInst = string.Empty;
        //                string PortName = string.Empty;
        //                foreach (XmlAttribute xAttr in xReset.Attributes)
        //                {
        //                    if (String.Compare(xAttr.Name, "Core", true) == 0)
        //                    {
        //                        CoreInst = xAttr.Value;
        //                    }
        //                    else if (String.Compare(xAttr.Name, "Port", true) == 0)
        //                    {
        //                        PortName = xAttr.Value;
        //                    }
        //                }
        //                if ((CoreInst != string.Empty) && (PortName != string.Empty))
        //                {
        //                    AddCoreResetSignal(CoreInst, PortName);
        //                }
        //            }
        //        }
        //    }
        //    #endregion
        //}
        //internal void AddCoreResetSignal(string CoreInstance, string PortName)
        //{
        //    string Inst = String.Format("{0}_{1}", this.ID, CoreInstance);
        //    if (this.ComponentCores.ContainsKey(Inst))
        //    {
        //        if (!this.ComponentCores[Inst].ResetPorts.Contains(PortName))
        //        {
        //            this.ComponentCores[Inst].ResetPorts.Add(PortName);
        //        }
        //    }
        //}
        //internal void AddSubPCore(string CoreType, string CoreInstance, string Version, string CoreSource, string CoreUCF, string ValidCond)
        //{
        //    ComponentCore newCore = new ComponentCore(this);
        //    newCore.NativeInstanceName = CoreInstance;
        //    newCore.HardwareInstanceName = String.Format("{0}_{1}", this.ID, CoreInstance);
        //    newCore.TypeName = CoreType;
        //    newCore.Version = Version;
        //    newCore.RelativeSource = CoreSource;
        //    newCore.CustomUCF = CoreUCF;
        //    newCore.ValidCondition = ValidCond;
        //    this.ComponentCores.Add(newCore.HardwareInstanceName, newCore);
        //}
        //internal void AddCoreClock(string CoreInstance, string ClockName, long Frequency, int Phase, ClockGroup Group, bool Buffered, bool OutputClock)
        //{
        //    if (!this.SubComponents.ContainsKey(CoreInstance))
        //    {
        //        throw new Exception(String.Format("Unable to add clock signal {0} -- sub-component {1} was not found.",
        //            ClockName,
        //            CoreInstance));
        //    }
        //    this.SubComponents[CoreInstance].ClockSet.AddClock(ClockName, Frequency, Phase, Group, Buffered);
        //}
        //internal bool AddCoreDependentClock(string CoreInstance, string ClockName, string DependentClockName, double Ratio, int Phase)
        //{
        //    if (!this.SubComponents.ContainsKey(CoreInstance))
        //    {
        //        throw new Exception(String.Format("Unable to add clock signal {0} -- sub-component {1} was not found.",
        //            ClockName,
        //            CoreInstance));
        //    }
        //    if (!this.SubComponents[CoreInstance].ClockSet.HasClock(DependentClockName))
        //    {
        //        throw new Exception(String.Format("Unable to add dependent clock signal {0} on sub-component {1} -- dependent-clock {2} was not found.",
        //            ClockName,
        //            CoreInstance,
        //            DependentClockName));
        //    }
        //    return this.SubComponents[CoreInstance].ClockSet.AddDependentClock(ClockName, DependentClockName, Ratio, Phase);
        //}
        //internal Dictionary<string, ClockSignal> GetSubComponentClocks(string CoreInstance)
        //{
        //    if (this.SubComponents.ContainsKey(CoreInstance))
        //    {
        //        return this.SubComponents[CoreInstance].ClockSet.GetClocks();
        //    }
        //    else
        //    {
        //        return new Dictionary<string, ClockSignal>();
        //    }
        //}

        internal void AddCoreProperty(string CoreInstance, CerebrumPropertyEntry PropEntry)
        {
            if (InternalComponentObject.ComponentCores.ContainsKey(CoreInstance))
            {
                InternalComponentObject.ComponentCores[CoreInstance].Properties.SetValue(PropEntry, true);
            }
            else
            {
                throw new Exception(String.Format("Unable to add property {0} -- sub-component {1} was not found.",
                    PropEntry.PropertyName,
                    CoreInstance));
            }
        }
        internal void AddNativeCoreProperty(string NativeCoreInstance, CerebrumPropertyEntry PropEntry)
        {
            if (InternalComponentObject.ComponentCores.ContainsKey(NativeCoreInstance))
            {
                InternalComponentObject.ComponentCores[NativeCoreInstance].Properties.SetValue(PropEntry, true);
            }
            else
            {
                throw new Exception(String.Format("Unable to add property {0} -- native sub-component {1} was not found.",
                    PropEntry.PropertyName,
                    NativeCoreInstance));
            }
        }
        internal void AddCoreCerebrumProperty(string CoreInstance, CerebrumPropertyEntry PropEntry)
        {
            if (InternalComponentObject.ComponentCores.ContainsKey(CoreInstance))
            {
                PropEntry.PropertyType = CerebrumPropertyTypes.CEREBRUMPROPERTY;
                InternalComponentObject.ComponentCores[CoreInstance].Properties.SetValue(PropEntry, true);
            }
            else
            {
                throw new Exception(String.Format("Unable to add property {0} -- sub-component {1} was not found.",
                    PropEntry.PropertyName,
                    CoreInstance));
            }
        }
        internal void AddNativeCoreCerebrumProperty(string NativeCoreInstance, CerebrumPropertyEntry PropEntry)
        {
            if (InternalComponentObject.ComponentCores.ContainsKey(NativeCoreInstance))
            {
                PropEntry.PropertyType = CerebrumPropertyTypes.CEREBRUMPROPERTY;
                InternalComponentObject.ComponentCores[NativeCoreInstance].Properties.SetValue(PropEntry, true);
            }
            else
            {
                throw new Exception(String.Format("Unable to add property {0} -- native sub-component {1} was not found.",
                    PropEntry.PropertyName,
                    NativeCoreInstance));
            }
        }
        
        #endregion
        
        #region Vortex Management
        /// <summary>
        /// Gets a list of Vortex interfaces (SAP or SOP) exposed by the component 
        /// </summary>
        public List<VortexAttachmentType> VortexTypes
        {
            get
            {
                List<VortexAttachmentType> _VortexTypes = new List<VortexInterfaces.VortexCommon.VortexAttachmentType>();
                foreach (IVortexAttachment VA in VortexDevices)
                {
                    if (VA is IVortexSAP)
                        _VortexTypes.Add(VortexInterfaces.VortexCommon.VortexAttachmentType.SAP);
                    else if (VA is IVortexSOP)
                        _VortexTypes.Add(VortexInterfaces.VortexCommon.VortexAttachmentType.SOP);
                    else if (VA is IVortexBridgeAttachment)
                        _VortexTypes.Add(VortexInterfaces.VortexCommon.VortexAttachmentType.VortexBridge);
                }
                return _VortexTypes;
            }
        }

        internal List<IVortexAttachment> VortexDevices
        {
            get
            {
                return _InternalComponentObject.VortexDevices;
            }
        }

        #endregion

    }
}
