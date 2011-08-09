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
 * FalconMapping_XML.cs
 * Name: Matthew Cotter
 * Date: 16 Apr 2010 
 * Description: This static class presents a set of Library-style functions for reading
 * and writing the XML-format files as input and output to/from the Component-to-FPGA
 * mapping algorithm.
 * History: 
 * >> ( 9 May 2010) Matthew Cotter: Implemented support for configuring and supporting edge/bridge components read from the platform.
 * >> (28 Jan 2011) Matthew Cotter: Began work on supporting/defining Links that are backed by Vortex-based hardware cores in the platform.
 * >> (25 Jan 2011) Matthew Cotter: Implemented initial support for auto-saving intermediate mapping state.
 * >> (18 Jan 2011) Matthew Cotter: Added support for GUI-added platform components overriding those instantiated by the platform.
 * >> (16 Dec 2010) Matthew Cotter: Added parsing support to load/save internal attachments on sub-cores for connections.
 *                                  Added parsing support for FPGA-required cores and corresponding properties/parameters associated with each core.
 * >> (11 Oct 2010) Matthew Cotter: Added handling of subcomponent PCores for integration into Vortex-based SAP/SOP communication infrastructure
 * >> ( 7 Oct 2010) Matthew Cotter: Changes required due to migration of ResourceInfo & ResourceSet structures to separate library.
 *                                  Converted generic Hashtables to TypedHashtables<> for readability and verifiability.
 * >> (13 Aug 2010) Matthew Cotter: Updated code that handles loading of Platform files to use new hierarchical location and format of platforms 
 *                                    (paths.ProjectPlatform -> paths.Platforms\<Platform>\<Platform.xml> -> 
 *                                      paths.Platforms\<Platform>\<Board>\<Board>.xml -> paths.Platforms\<Platform>\<Board>\<fpga>\<fpga>.xml.
 * >> (10 Aug 2010) Matthew Cotter: Changed all string comparisons to use the StringsEqual function, which performs a case-insensitive comparison without modifying either string.
 *                                  Removed all calls to <string>.ToLower() -- IDs, Names, etc are case-sensitive.   Too many problems arose due to case conversions.
 * >> (22 Jul 2010) Matthew Cotter: Corrected a bug that arose from conversion of resource values to long ints, that caused an OverflowException to occur when reading in FPGAs from a system specification file.
 * >> ( 7 Jul 2010) Matthew Cotter: Updated ProcessDesignCoresConnections() to support enable/disable of core server generation in core address map file.
 * >> ( 6 Jul 2010) Matthew Cotter: Updated ProcessPlatforms() and ProcessBoard() to handle hierarchical format of system platform.
 * >> (30 Jun 2010) Matthew Cotter: Added Method CreateMappingInput() to facilitate input from Cerebrum design system.
 * >> (15 Jun 2010) Matthew Cotter: Removed automatic generation of groups at Load-time.  Auto-generation is now only done at 'execution' time.
 *                                    Modified code to allow system and mapping files to be written to the same file in either order.  Previously, the 
 *                                       system information would overwrite and erase the file, while the mapping would append.  Now, both functions 
 *                                       only modify their respective XML sections, copying others in-tact.
 *                                    Updated Mapping output to omit empty groups in the mapping section.
 * >> (10 Jun 2010) Matthew Cotter: Corrected reading/appending of Mapping XML files.
 * >> ( 8 Jun 2010) Matthew Cotter: Corrected bug in Link parsing that later caused a divide-by-zero if LinkSpeed is not specified in XML.
 * >> ( 6 May 2010) Matthew Cotter: Added functions to utilize singular collective object
 * >> (25 Apr 2010) Matthew Cotter: Added code to Read/Write Pre- and Post-Mapping sections of XML
 * >> (17 Apr 2010) Matthew Cotter: Completed work on writing all XML (sub-) sections of <Objects>
 * >> (16 Apr 2010) Matthew Cotter: Started work on writing all XML (sub-) sections of <Objects>
 *                                    Completed work on reading all XML (sub-) sections of <Objects>
 * >> (15 Apr 2010) Matthew Cotter: Completed work on reading <Objects>, <Components>, and <FPGAs> sections
 *                                    Started and completed work on reading <Groups> and <Clusters>
 *                                    Initial work on reading <Connectivity>, <Connections>, and <Links> sections
 * >> (14 Apr 2010) Matthew Cotter: Initial work on reading <Objects>, <Components>, and <FPGAs> sections
 * >> ( 9 Apr 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using FalconGlobal;
using CerebrumSharedClasses;
using CerebrumNetronObjects;

namespace FalconMappingAlgorithm
{
    /// <summary>
    /// Static library-class to implement I/O to read/write system specification and mapping from/to a file.
    /// </summary>
    public static class FalconMapping_XML
    {
        private const string TOP_LEVEL_NODE = "MappingSystem";
        private const string OBJECTS_NODE = "Objects";
        private const string COMPONENTS_NODE = "Components";
        private const string COMPONENT_NODE = "Component";
        private const string FPGAS_NODE = "FPGAs";
        private const string FPGA_NODE = "FPGA";

        private const string RESOURCE_NODE = "Resource";

        private const string GROUPS_NODE = "Groups";
        private const string GROUP_NODE = "Group";
        private const string CMEMBER_NODE = "ComponentMember";
        private const string CLUSTERS_NODE = "Clusters";
        private const string CLUSTER_NODE = "Cluster";
        private const string FMEMBER_NODE = "FPGAMember";

        private const string CONNECTIVITY_NODE = "Connectivity";
        private const string CONNECTIONS_NODE = "Connections";
        private const string CONNECTION_NODE = "Connection";
        private const string LINKS_NODE = "Links";
        private const string LINK_NODE = "Link";

        private const string PREMAPPING_NODE = "PreMapping";
        private const string POSTMAPPING_NODE = "PostMapping";

        private const string MAPPEDGROUP_NODE = "MappedGroup";
       
        /// <summary>
        /// Performs a case-insensitive test on two strings to determine whether they are equivalent.
        /// </summary>
        /// <param name="S1">The first string to be compared</param>
        /// <param name="S2">The second string to be compared</param>
        /// <returns>True if S1 == S2 (case-insensitive), false otherwise.</returns>
        public static bool StringsEqual(string S1, string S2)
        {
            return (string.Compare(S1, S2, true) == 0);
        }

        #region Internally-Accessible Read/Write Functions

        /// <summary>
        /// Reads in the system specification from the specified XML file.
        /// </summary>
        /// <param name="InputFile">The path to the XML file.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        internal static void ReadSystemFile(string InputFile, 
                                FalconMapping_Objects MapObjects)
        {
            if (!File.Exists(InputFile))
                throw new FileNotFoundException("Unable to open Mapping File for reading", InputFile);

            StreamReader sReader = new StreamReader(InputFile);
            XmlTextReader XTReader = new XmlTextReader(sReader);
            XmlDocument XDoc = new XmlDocument();

            XmlNode XNSystem = null;

            XDoc.Load(XTReader);
            MapObjects.Reset();

            // Start parsing the file
            for (int iNode = 0; iNode < XDoc.ChildNodes.Count; iNode++)
            {
                if (StringsEqual(XDoc.ChildNodes[iNode].Name, TOP_LEVEL_NODE))
                {
                    // <SystemMapping> Node
                    XNSystem = XDoc.ChildNodes[iNode];
                    break;
                }
            }
            try
            {
                if (XNSystem != null)
                {
                    // Read in all Objects and Connectivity prior to reading 
                    // in Pre-Mapping or PostMapping Information
                    for (int iSubNode = 0; iSubNode < XNSystem.ChildNodes.Count; iSubNode++)
                    {
                        if (StringsEqual(XNSystem.ChildNodes[iSubNode].Name, OBJECTS_NODE))
                        {
                            // <Objects>
                            ReadObjects(XNSystem.ChildNodes[iSubNode], MapObjects);
                            if (MapObjects.Components.Count == 0)
                                throw new Exceptions.NoComponentsReadException(
                                    String.Format("No Components were read from XML file: {0}", InputFile));
                            if (MapObjects.FPGAs.Count == 0)
                                throw new Exceptions.NoFPGAsReadException(
                                    String.Format("No FPGAs were read from XML file: {0}", InputFile));
                            //if (Groups.Count == 0)
                            //    throw new Exceptions.NoGroupsReadException(
                            //        String.Format("No Groups were read from XML file: {0}", InputFile));
                            //if (Clusters.Count == 0)
                            //    throw new Exceptions.NoClustersReadException(
                            //        String.Format("No Clusters were read from XML file: {0}", InputFile));

                        }
                        else if (StringsEqual(XNSystem.ChildNodes[iSubNode].Name, CONNECTIVITY_NODE))
                        {
                            // <Connectivity>
                            ReadConnectivity(XNSystem.ChildNodes[iSubNode], MapObjects);
                            if (MapObjects.Connections.Count == 0)
                                throw new Exceptions.NoConnectionsReadException("No Connections read from XML File: " + InputFile);
                            if (MapObjects.Links.Count == 0)
                                throw new Exceptions.NoLinksReadException("No Links read from XML File: " + InputFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            XTReader.Close();
            sReader.Close();

        }

        /// <summary>
        /// Reads in the Pre- and Post-Mapping specification from the specified XML file.
        /// </summary>
        /// <param name="InputFile">The path to the XML file.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        /// <param name="ReadPreMapping">Indicates whether to look for the PreMapping tag (true) or the PostMapping tag (false)</param>
        internal static void ReadMappingFile(string InputFile,
                                FalconMapping_Objects MapObjects,
                                bool ReadPreMapping)
        {
            if (!File.Exists(InputFile))
                throw new FileNotFoundException("Unable to open Mapping File for reading", InputFile);

            StreamReader sReader = new StreamReader(InputFile);
            XmlTextReader XTReader = new XmlTextReader(sReader);
            XmlDocument XDoc = new XmlDocument();

            XmlNode XNSystem = null;

            XDoc.Load(XTReader);

            // Start parsing the file
            for (int iNode = 0; iNode < XDoc.ChildNodes.Count; iNode++)
            {
                if (StringsEqual(XDoc.ChildNodes[iNode].Name, TOP_LEVEL_NODE))
                {
                    // <SystemMapping> Node
                    XNSystem = XDoc.ChildNodes[iNode];
                    break;
                }
            }
            if (XNSystem != null)
            {
                // All Objects and Connectivity Should have been read in prior to reading 
                // in Pre-Mapping or PostMapping Information
                for (int iSubNode = 0; iSubNode < XNSystem.ChildNodes.Count; iSubNode++)
                {
                    if (StringsEqual(XNSystem.ChildNodes[iSubNode].Name, PREMAPPING_NODE) && ReadPreMapping)
                    {
                        // <PreMapping>
                        ReadMapping(XNSystem.ChildNodes[iSubNode], MapObjects, ReadPreMapping);
                        break;
                    }
                    else if (StringsEqual(XNSystem.ChildNodes[iSubNode].Name, POSTMAPPING_NODE) && !ReadPreMapping)
                    {
                        // <PostMapping>
                        ReadMapping(XNSystem.ChildNodes[iSubNode], MapObjects, ReadPreMapping);
                        break;
                    }
                }
            }

            XTReader.Close();
            sReader.Close();
        }

        /// <summary>
        /// Writes out the system specification to the specified XML file.
        /// </summary>
        /// <param name="OutputFile">The path to the XML file.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        /// <param name="AppendIfFileExists">Indicates whether the output should be appeneded to the file, if it already exists.</param>
        internal static void WriteSystemFile(string OutputFile,
                                FalconMapping_Objects MapObjects,
                                bool AppendIfFileExists)
        {
            XmlDocument XDoc = new XmlDocument();
            XmlElement XERoot = null;
            XmlElement XReadRoot = null;
            XDoc.AppendChild(XDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            XERoot = XDoc.CreateElement(TOP_LEVEL_NODE);
            FalconFileRoutines.WriteCerebrumDisclaimerXml(XERoot);    // Added by Matthew Cotter 8/18/2010

            bool bWrittenObj = false;
            bool bWrittenCon = false;
            if ((System.IO.File.Exists(OutputFile)) && AppendIfFileExists)
            {
                XmlDocument XReadDoc = new XmlDocument();
                XReadDoc.Load(OutputFile);
                foreach (XmlNode xn in XReadDoc.ChildNodes)
                {
                    if (StringsEqual(xn.Name, TOP_LEVEL_NODE))
                    {
                        XReadRoot = (XmlElement)XDoc.ImportNode(xn, true);
                        break;
                    }
                }
                if (XReadRoot != null)
                {
                    foreach (XmlNode xn in XReadRoot.ChildNodes)
                    {
                        if (StringsEqual(xn.Name, OBJECTS_NODE))
                        {
                            if (!bWrittenObj)
                            {
                                WriteObjects(XDoc, XERoot, MapObjects);
                            }
                            bWrittenObj = true;
                        }
                        else if (StringsEqual(xn.Name, CONNECTIVITY_NODE))
                        {
                            if (!bWrittenCon)
                            {
                                WriteConnectivity(XDoc, XERoot, MapObjects);
                            }
                            bWrittenCon = true;
                        }
                        else
                        {
                            if (xn.NodeType != XmlNodeType.Comment)
                            {
                                XmlNode xnn = XDoc.ImportNode(xn, true);
                                XERoot.AppendChild(xnn);
                            }
                        }
                    }
                }
            }

            if (!bWrittenObj)
            {
                WriteObjects(XDoc, XERoot, MapObjects);
            }
            if (!bWrittenCon)
            {
                WriteConnectivity(XDoc, XERoot, MapObjects);
            }

            XDoc.AppendChild(XERoot);
            XDoc.Save(OutputFile);
            MapObjects.RaiseMessageEvent("Saved System Design State to: {0}", OutputFile);
        }

        /// <summary>
        /// Writes out the Pre- or Post-Mapping specification to the specified XML file.
        /// </summary>
        /// <param name="OutputFile">The path to the XML file.</param>
        /// <param name="MapObjects">The Mapping object collection to be saved.</param>
        /// <param name="WritePreMapping">Indicates whether to write the PreMapping tag (true) or the PostMapping tag (false)</param>
        /// <param name="AppendIfFileExists">Indicates whether the output should be appeneded to the file, if it already exists.</param>
        internal static void WriteMappingFile(string OutputFile,
                                    FalconMapping_Objects MapObjects,
                                    bool WritePreMapping,
                                    bool AppendIfFileExists)
        {
            XmlDocument XDoc = new XmlDocument();
            XmlElement XERoot = null;
            XmlElement XReadRoot = null;
            XDoc.AppendChild(XDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            XERoot = XDoc.CreateElement(TOP_LEVEL_NODE);
            FalconFileRoutines.WriteCerebrumDisclaimerXml(XERoot);    // Added by Matthew Cotter 8/18/2010
            XDoc.AppendChild(XERoot);

            bool bWritten = false;
            if ((System.IO.File.Exists(OutputFile)) && AppendIfFileExists)
            {
                XmlDocument XReadDoc = new XmlDocument();
                XReadDoc.Load(OutputFile);
                foreach (XmlNode xn in XReadDoc.ChildNodes)
                {
                    if (StringsEqual(xn.Name, TOP_LEVEL_NODE))
                    {
                        XReadRoot = (XmlElement)XDoc.ImportNode(xn, true);
                        break;
                    }
                }
                if (XReadRoot != null)
                {
                    foreach (XmlNode xn in XReadRoot.ChildNodes)
                    {
                        if (((StringsEqual(xn.Name, PREMAPPING_NODE) && (WritePreMapping))) ||
                            ((StringsEqual(xn.Name, POSTMAPPING_NODE) && (!WritePreMapping))))
                        {
                            if (!bWritten)
                            {
                                WriteMapping(XDoc, XERoot, MapObjects, WritePreMapping);
                            }
                            bWritten = true;
                        }
                        else
                        {
                            if (xn.NodeType != XmlNodeType.Comment)
                            {
                                XmlNode xnn = XDoc.ImportNode(xn, true);
                                XERoot.AppendChild(xnn);
                            }
                        }
                    }
                }
            }

            if (!bWritten)
            {
                WriteMapping(XDoc, XERoot, MapObjects, WritePreMapping);
            }

            XDoc.AppendChild(XERoot);
            XDoc.Save(OutputFile);
            MapObjects.RaiseMessageEvent("Saved Component Mapping State to: {0}", OutputFile);
        }

        /// <summary>
        /// Reads temporary, intermedate mapping state from a file.  The contents of this file will only include groups and group-mappings.
        /// Ungrouped components cannot be mapped, so do are not included in this file.
        /// </summary>
        /// <param name="InputFile">The file to read the intermediate state from.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        internal static void ReadIntermediateState(string InputFile,
                                    FalconMapping_Objects MapObjects)
        {
            try
            {
                if (!File.Exists(InputFile))
                    throw new FileNotFoundException("Unable to open Mapping File for reading", InputFile);

                XmlDocument XDoc = new XmlDocument();
                XDoc.Load(InputFile);

                // Start parsing the file
                foreach (XmlNode XRoot in XDoc.ChildNodes)
                {
                    if (String.Compare(XRoot.Name, TOP_LEVEL_NODE, true) == 0)
                    {
                        foreach (XmlNode xObjectsNode in XRoot.ChildNodes)
                        {
                            if (String.Compare(xObjectsNode.Name, GROUPS_NODE, true) == 0)
                            {
                                ReadGroups(xObjectsNode, MapObjects);
                            }
                            else if (String.Compare(xObjectsNode.Name, PREMAPPING_NODE, true) == 0)
                            {
                                ReadMapping(xObjectsNode, MapObjects, true);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// Writes temporary, intermedate mapping state to a file.  The contents of this file will only include groups and group-mappings.
        /// Ungrouped components cannot be mapped, so do are not included in this file.
        /// </summary>
        /// <param name="OutputFile">The file to write the intermediate state to.</param>
        /// <param name="MapObjects">The Mapping object collection to be saved.</param>
        internal static void WriteIntermediateState(string OutputFile,
                                    FalconMapping_Objects MapObjects)
        {
            try
            {
                XmlDocument XDoc = new XmlDocument();
                XmlNode XRoot = XDoc.CreateElement(TOP_LEVEL_NODE);
                XmlNode XGroups = XDoc.CreateElement(GROUPS_NODE);
                WriteGroups(XDoc, (XmlElement)XGroups, MapObjects);
                XRoot.AppendChild(XGroups);
                WriteMapping(XDoc, (XmlElement)XRoot, MapObjects, true);
                XDoc.AppendChild(XRoot);

                XDoc.Save(OutputFile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }


        #endregion

        #region Private Read "Helper" Functions

        /// <summary>
        /// Reads in the system "Objects" specification from an XMLDocument XmlNode.
        /// </summary>
        /// <param name="ObjectsNode">The XmlNode that "contains" the Objects specification from the XML File.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        static void ReadObjects(XmlNode ObjectsNode,
                                FalconMapping_Objects MapObjects)
        {
            // Must read Components and FPGAs before Groups and Clusters
            for (int iNode = 0; iNode < ObjectsNode.ChildNodes.Count; iNode++)
            {
                if (StringsEqual(ObjectsNode.ChildNodes[iNode].Name, COMPONENTS_NODE))
                {
                    MapObjects.Components = ReadComponents(ObjectsNode.ChildNodes[iNode], MapObjects);
                }
                else if (StringsEqual(ObjectsNode.ChildNodes[iNode].Name, FPGAS_NODE))
                {
                    MapObjects.FPGAs = ReadFPGAs(ObjectsNode.ChildNodes[iNode], MapObjects);
                }                
            }
            for (int iNode = 0; iNode < ObjectsNode.ChildNodes.Count; iNode++)
            {
                if (StringsEqual(ObjectsNode.ChildNodes[iNode].Name, GROUPS_NODE))
                {
                    MapObjects.Groups = ReadGroups(ObjectsNode.ChildNodes[iNode], MapObjects);
                }
                else if (StringsEqual(ObjectsNode.ChildNodes[iNode].Name, CLUSTERS_NODE))
                {
                    MapObjects.Clusters = ReadClusters(ObjectsNode.ChildNodes[iNode], MapObjects);
                }
            }
        }

        /// <summary>
        /// Reads in the system "Connectivity" specification from an XMLDocument XmlNode.
        /// </summary>
        /// <param name="ConnectivityNode">The XmlNode that "contains" the Connectivity specification from the XML File.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        static void ReadConnectivity(XmlNode ConnectivityNode,
                                FalconMapping_Objects MapObjects)
        {
            for (int iNode = 0; iNode < ConnectivityNode.ChildNodes.Count; iNode++)
            {
                if (StringsEqual(ConnectivityNode.ChildNodes[iNode].Name, CONNECTIONS_NODE))
                {
                    ReadConnections(ConnectivityNode.ChildNodes[iNode], MapObjects);
                }
                else if (StringsEqual(ConnectivityNode.ChildNodes[iNode].Name, LINKS_NODE))
                {
                    ReadLinks(ConnectivityNode.ChildNodes[iNode], MapObjects);
                }
            }
        }

        /// <summary>
        /// Reads in the set of system mapping components from the specified XMLDocument XMLNode.
        /// </summary>
        /// <param name="ComponentsNode">The XmlNode that "contains" the Components specification from the XML File.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        /// <returns>Returns a hashtable that contains all of the Components from the XML file.</returns>
        static Dictionary<string, FalconMapping_Component> ReadComponents(XmlNode ComponentsNode,
                                FalconMapping_Objects MapObjects)
        {
            Dictionary<string, FalconMapping_Component> htComponents = new Dictionary<string, FalconMapping_Component>();
            if (ComponentsNode.ChildNodes.Count == 0)
                return htComponents;
            for (int iComponent = 0; iComponent < ComponentsNode.ChildNodes.Count; iComponent++)
            {
                if (StringsEqual(ComponentsNode.ChildNodes[iComponent].Name, COMPONENT_NODE))
                {
                    // <Component>
                    XmlNode XNComponent = ComponentsNode.ChildNodes[iComponent];
                    string componentName = string.Empty;
                    string componentID = string.Empty;
                    string componentSource = string.Empty;
                    string componentArches = string.Empty;
                    foreach (XmlAttribute XA in XNComponent.Attributes)
                    {
                        if (StringsEqual(XA.Name, "Name"))
                            componentName = XA.Value;
                        else if (StringsEqual(XA.Name, "ID"))
                            componentID = XA.Value;
                        else if (StringsEqual(XA.Name, "Location"))
                            componentSource = XA.Value;
                        else if (StringsEqual(XA.Name, "SupportedArch"))
                            componentArches = XA.Value;
                    }
                    FalconMapping_Component fmc = new FalconMapping_Component(componentID, componentName, componentArches);
                    fmc.Source = componentSource;
                    for (int iResource = 0; iResource < XNComponent.ChildNodes.Count; iResource++)
                    {
                        if (StringsEqual(XNComponent.ChildNodes[iResource].Name, RESOURCE_NODE))
                        {
                            XmlNode XNResource = XNComponent.ChildNodes[iResource];
                            string resName = string.Empty;
                            int resValue = 0;
                            foreach (XmlAttribute XA in XNResource.Attributes)
                            {
                                if (StringsEqual(XA.Name, "Name"))
                                    resName = XA.Value.ToLower();
                                else if (StringsEqual(XA.Name, "Value"))
                                    resValue = (int.Parse(XA.Value));
                            }
                            fmc.SetRequiredResource(resName, resValue);
                        }
                    }
                    htComponents.Add(fmc.ID, fmc);
                }
            }
            return htComponents;
        }

        /// <summary>
        /// Reads in the set of system mapping FPGAs from the specified XMLDocument XMLNode.
        /// </summary>
        /// <param name="FPGAsNode">The XmlNode that "contains" the FPGAs specification from the XML File.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        /// <returns>Returns a hashtable that contains all of the FPGAs from the XML file.</returns>
        static Dictionary<string, FalconMapping_FPGA> ReadFPGAs(XmlNode FPGAsNode,
                                FalconMapping_Objects MapObjects)
        {
            Dictionary<string, FalconMapping_FPGA> htFPGAs = new Dictionary<string, FalconMapping_FPGA>();
            if (FPGAsNode.ChildNodes.Count == 0)
                return htFPGAs;
            for (int iFPGA = 0; iFPGA < FPGAsNode.ChildNodes.Count; iFPGA++)
            {
                if (StringsEqual(FPGAsNode.ChildNodes[iFPGA].Name, FPGA_NODE))
                {
                    // <FPGA>
                    XmlNode XNFPGA = FPGAsNode.ChildNodes[iFPGA];
                    string fpgaName = string.Empty;
                    string fpgaID = string.Empty;
                    string fpgaFamily = string.Empty;
                    foreach (XmlAttribute XA in XNFPGA.Attributes)
                    {
                        if (StringsEqual(XA.Name, "Name"))
                            fpgaName = XA.Value;
                        else if (StringsEqual(XA.Name, "ID"))
                            fpgaID = XA.Value;
                        else if (StringsEqual(XA.Name, "Family"))
                            fpgaFamily = XA.Value;
                    }
                    FalconMapping_FPGA fmp = new FalconMapping_FPGA(fpgaID, fpgaName, fpgaFamily);
                    fmp.PathManager = MapObjects.PathMan;
                    fmp.InitializeVortex(MapObjects.GetFPGAs().Count + 1);
                    for (int iResource = 0; iResource < XNFPGA.ChildNodes.Count; iResource++)
                    {
                        if (StringsEqual(XNFPGA.ChildNodes[iResource].Name, RESOURCE_NODE))
                        {
                            XmlNode XNResource = XNFPGA.ChildNodes[iResource];
                            string resName = string.Empty;
                            long resValue = 0;
                            foreach (XmlAttribute XA in XNResource.Attributes)
                            {
                                if (StringsEqual(XA.Name, "Name"))
                                    resName = XA.Value.ToLower();
                                else if (StringsEqual(XA.Name, "Value"))
                                    resValue = (long.Parse(XA.Value));
                            }
                            resName = resName.ToLower();
                            fmp.SetTotalResource(resName, resValue);
                        }
                    }
                    htFPGAs.Add(fmp.ID, fmp);
                }
            }
            return htFPGAs;
        }

        /// <summary>
        /// Reads in the set of system mapping Groups from the specified XMLDocument XMLNode.  These groups
        /// specify pre-set groupings of logical components in the design.
        /// </summary>
        /// <param name="GroupsNode">The XmlNode that "contains" the group specification from the XML File.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        /// <returns>Returns a hashtable that contains all of the groups from the XML file.</returns>
        static Dictionary<string, FalconMapping_Group> ReadGroups(XmlNode GroupsNode,
                                                                    FalconMapping_Objects MapObjects)
        {
            Dictionary<string, FalconMapping_Group> htGroups = new Dictionary<string, FalconMapping_Group>();
            if (GroupsNode.ChildNodes.Count == 0)
                return htGroups;
            for (int iGroup = 0; iGroup < GroupsNode.ChildNodes.Count; iGroup++)
            {
                if (StringsEqual(GroupsNode.ChildNodes[iGroup].Name, GROUP_NODE))
                {
                    // <Group>
                    XmlNode XNGroup = GroupsNode.ChildNodes[iGroup];
                    string GroupName = string.Empty;
                    string GroupID = string.Empty;
                    foreach (XmlAttribute XA in XNGroup.Attributes)
                    {
                        if (StringsEqual(XA.Name, "Name"))
                            GroupName = XA.Value;
                        else if (StringsEqual(XA.Name, "ID"))
                            GroupID = XA.Value;
                    }
                    FalconMapping_Group fmg = null;
                    if (!MapObjects.Groups.ContainsKey(GroupID))
                    {
                        MapObjects.AddGroup(GroupID, GroupName);
                    }
                    fmg = MapObjects.Groups[GroupID];
                    for (int iComponent = 0; iComponent < XNGroup.ChildNodes.Count; iComponent++)
                    {
                        if (StringsEqual(XNGroup.ChildNodes[iComponent].Name, CMEMBER_NODE))
                        {
                            XmlNode XNCompMember = XNGroup.ChildNodes[iComponent];
                            string compID = string.Empty;
                            foreach (XmlAttribute XA in XNCompMember.Attributes)
                            {
                                if (StringsEqual(XA.Name, "ID"))
                                    compID = XA.Value;
                            }
                            if (MapObjects.Components.ContainsKey(compID))
                            {
                                // Component exists
                                fmg.AddComponent(MapObjects.Components[compID]);
                            }
                            else
                            {
                                // Group Specifies a Component ID that does not exist
                            }
                        }
                    }
                    htGroups.Add(fmg.ID, fmg);
                }
            }
            return htGroups;
        }

        /// <summary>
        /// Reads in the set of system mapping clusters from the specified XMLDocument XMLNode.  These groups
        /// specify pre-set logical clusters of physical FPGAs in the system.
        /// </summary>
        /// <param name="ClustersNode">The XmlNode that "contains" the cluster specification from the XML File.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        /// <returns>Returns a hashtable that contains all of the clusters from the XML file.</returns>
        static Dictionary<string, FalconMapping_Cluster> ReadClusters(XmlNode ClustersNode,
                                                                        FalconMapping_Objects MapObjects)
        {
            Dictionary<string, FalconMapping_Cluster> htClusters = new Dictionary<string, FalconMapping_Cluster>();
            if (ClustersNode.ChildNodes.Count == 0)
                return htClusters;
            for (int iCluster = 0; iCluster < ClustersNode.ChildNodes.Count; iCluster++)
            {
                if (StringsEqual(ClustersNode.ChildNodes[iCluster].Name, CLUSTER_NODE))
                {
                    // <Cluster>
                    XmlNode XNCluster = ClustersNode.ChildNodes[iCluster];
                    string ClusterName = string.Empty;
                    string ClusterID = string.Empty;
                    foreach (XmlAttribute XA in XNCluster.Attributes)
                    {
                        if (StringsEqual(XA.Name, "Name"))
                            ClusterName = XA.Value;
                        else if (StringsEqual(XA.Name, "ID"))
                            ClusterID = XA.Value;
                    }
                    FalconMapping_Cluster fmc = new FalconMapping_Cluster(ClusterID, ClusterName);
                    for (int iFPGA = 0; iFPGA < XNCluster.ChildNodes.Count; iFPGA++)
                    {
                        if (StringsEqual(XNCluster.ChildNodes[iFPGA].Name, FMEMBER_NODE))
                        {
                            XmlNode XNFPGAMember = XNCluster.ChildNodes[iFPGA];
                            string fpgaID = string.Empty;
                            foreach (XmlAttribute XA in XNFPGAMember.Attributes)
                            {
                                if (StringsEqual(XA.Name, "ID"))
                                    fpgaID = XA.Value;
                            }
                            if (MapObjects.FPGAs.ContainsKey(fpgaID))
                            {
                                // FPGA exists
                                fmc.AddFPGA(MapObjects.FPGAs[fpgaID]);
                            }
                            else
                            {
                                // Cluster Specifies an FPGA ID that does not exist
                            }
                        }
                    }
                    htClusters.Add(fmc.ID, fmc);
                }
            }
            return htClusters;
        }

        /// <summary>
        /// Reads in the set of system mapping connections from the specified XMLDocument XMLNode.  The connections
        /// specify the logical connections between components in the system.
        /// </summary>
        /// <param name="ConnectionsNode">The XmlNode that "contains" the connections specification from the XML File.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        /// <returns>Returns a hashtable that contains all of the connections from the XML file.</returns>
        static Dictionary<string, FalconMapping_Connection> ReadConnections(XmlNode ConnectionsNode,
                                                                            FalconMapping_Objects MapObjects)
        {
            Dictionary<string, FalconMapping_Connection> htConnections = new Dictionary<string, FalconMapping_Connection>();
            if (ConnectionsNode.ChildNodes.Count == 0)
                return htConnections;
            for (int iConnection = 0; iConnection < ConnectionsNode.ChildNodes.Count; iConnection++)
            {
                if (StringsEqual(ConnectionsNode.ChildNodes[iConnection].Name, CONNECTION_NODE))
                {
                    // <Connection>
                    XmlNode XNConnection = ConnectionsNode.ChildNodes[iConnection];
                    string ConnectionID = string.Empty;
                    string ConnectionSource = string.Empty;
                    string ConnectionSourceInstance = string.Empty;
                    string ConnectionSink = string.Empty;
                    string ConnectionSinkInstance = string.Empty;
                    double ConnectionWt = 1.0F;
                    foreach (XmlAttribute XA in XNConnection.Attributes)
                    {
                        if (StringsEqual(XA.Name, "ID"))
                            ConnectionID = XA.Value;
                        else if (StringsEqual(XA.Name, "Input") || StringsEqual(XA.Name, "Source"))
                            ConnectionSource = XA.Value;
                        else if (StringsEqual(XA.Name, "InputInstance") || StringsEqual(XA.Name, "SourceInstance"))
                            ConnectionSourceInstance = XA.Value;
                        else if (StringsEqual(XA.Name, "Output") || StringsEqual(XA.Name, "Sink"))
                            ConnectionSink = XA.Value;
                        else if (StringsEqual(XA.Name, "OutputInstance") || StringsEqual(XA.Name, "SinkInstance"))
                            ConnectionSinkInstance = XA.Value;
                        else if (StringsEqual(XA.Name, "DataWeight") || StringsEqual(XA.Name, "Weight")
                              || StringsEqual(XA.Name, "DataDensity") || StringsEqual(XA.Name, "Density"))
                            ConnectionWt = float.Parse(XA.Value);
                    }
                    FalconMapping_Connection fmc = null;
                    if (!MapObjects.Connections.ContainsKey(ConnectionID))
                    {
                        MapObjects.AddConnection(ConnectionID, ConnectionID, ConnectionWt, ConnectionSource, ConnectionSink);
                    }
                    fmc = MapObjects.Connections[ConnectionID];
                    fmc.DataDensity = ConnectionWt;
                    fmc.SourceComponentInstance = ConnectionSourceInstance;
                    fmc.SinkComponentInstance = ConnectionSinkInstance;
                    htConnections.Add(fmc.ID, fmc);
                }
            }
           
            return htConnections;
        }

        /// <summary>
        /// Reads in the set of system mapping link from the specified XMLDocument XMLNode.  The links
        /// specify the physical connections between FPGAs in the system.
        /// </summary>
        /// <param name="LinksNode">The XmlNode that "contains" the links specification from the XML File.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        /// <returns>Returns a hashtable that contains all of the links from the XML file.</returns>
        static Dictionary<string, FalconMapping_Link> ReadLinks(XmlNode LinksNode,
                                                                FalconMapping_Objects MapObjects)
        {
            Dictionary<string, FalconMapping_Link> htLinks = new Dictionary<string, FalconMapping_Link>();
            if (LinksNode.ChildNodes.Count == 0)
                return htLinks;
            for (int iLink = 0; iLink < LinksNode.ChildNodes.Count; iLink++)
            {
                if (StringsEqual(LinksNode.ChildNodes[iLink].Name, LINK_NODE))
                {
                    // <Link>
                    XmlNode XNLink = LinksNode.ChildNodes[iLink];
                    string LinkID = string.Empty;
                    string LinkSource = string.Empty;
                    string LinkSink = string.Empty;
                    double LinkWt = 1.0F;
                    bool LinkBidir = true;
                    foreach (XmlAttribute XA in XNLink.Attributes)
                    {
                        if (StringsEqual(XA.Name, "ID"))
                            LinkID = XA.Value;
                        else if (StringsEqual(XA.Name, "Input") || StringsEqual(XA.Name, "Source") || StringsEqual(XA.Name, "SourceFPGA"))
                            LinkSource = XA.Value;
                        else if (StringsEqual(XA.Name, "Output") || StringsEqual(XA.Name, "Sink") || StringsEqual(XA.Name, "SinkFPGA"))
                            LinkSink = XA.Value;
                        else if (StringsEqual(XA.Name, "LinkSpeed") || StringsEqual(XA.Name, "Speed"))
                            LinkWt = float.Parse(XA.Value);
                        else if (StringsEqual(XA.Name, "Bidirectional"))
                            LinkBidir = bool.Parse(XA.Value);
                    }
                    FalconMapping_Link fml = null;
                    if (!MapObjects.Links.ContainsKey(LinkID))
                    {
                        MapObjects.AddLink(LinkID, LinkID, LinkWt, LinkSource, LinkSink, LinkBidir);
                    }
                    fml = MapObjects.Links[LinkID];
                    htLinks.Add(fml.ID, fml);
                    foreach (XmlAttribute XA in XNLink.Attributes)
                    {
                        if (String.Compare(XA.Name, "SourceComponent", true) == 0)
                        {
                            fml.SourceComponent = XA.Value;
                        }
                        else if (String.Compare(XA.Name, "SourceIngressCore", true) == 0)
                        {
                            fml.SourceIngressCore = XA.Value;
                        }
                        else if (String.Compare(XA.Name, "SourceEgressCore", true) == 0)
                        {
                            fml.SourceEgressCore = XA.Value;
                        }
                        else if (String.Compare(XA.Name, "SinkComponent", true) == 0)
                        {
                            fml.SinkComponent = XA.Value;
                        }
                        else if (String.Compare(XA.Name, "SinkIngressCore", true) == 0)
                        {
                            fml.SinkIngressCore = XA.Value;
                        }
                        else if (String.Compare(XA.Name, "SinkEgressCore", true) == 0)
                        {
                            fml.SinkEgressCore = XA.Value;
                        }
                    }
                    foreach (XmlNode xCoreNode in XNLink.ChildNodes)
                    {
                        if (String.Compare(xCoreNode.Name, "RequiredCore", true) == 0)
                        {
                            string OnFPGA = string.Empty;
                            string CoreSource = string.Empty;
                            foreach (XmlAttribute xAttr in xCoreNode.Attributes)
                            {
                                if (String.Compare(xAttr.Name, "OnFPGA", true) == 0)
                                {
                                    OnFPGA = xAttr.Value;
                                }
                                else if (String.Compare(xAttr.Name, "Source", true) == 0)
                                {
                                    CoreSource = xAttr.Value;
                                }
                            }
                            if ((OnFPGA != string.Empty) && (CoreSource != string.Empty))
                            {
                                CerebrumCore LinkCore = CoreLibrary.ReadRequiredComponentFromXML(MapObjects.PathMan, OnFPGA, xCoreNode);
                                if (LinkCore != null)
                                {
                                    foreach (FalconMapping_FPGA FPGA in MapObjects.GetFPGAs().Values)
                                    {
                                        if (String.Compare(FPGA.ID, OnFPGA, true) == 0)
                                        {
                                            string SupportedArchitectures = string.Empty;
                                            foreach (string Arch in LinkCore.SupportedArchitectures)
                                            {
                                                SupportedArchitectures = String.Format("{0};{1}", Arch, SupportedArchitectures);
                                            }
                                            SupportedArchitectures = SupportedArchitectures.Trim(';');
                                            FalconMapping_Component LinkComponent = new FalconMapping_Component(LinkCore.CoreInstance, LinkCore.CoreInstance, SupportedArchitectures);
                                            LinkComponent.Source = CoreSource;
                                            LinkComponent.LoadComponentSource(MapObjects.PathMan);
                                            string FilePath = new Uri(XNLink.OwnerDocument.BaseURI).AbsolutePath;
                                            LinkComponent.InternalComponentObject.LoadComponentPropertiesFromPlatformNode(FilePath, xCoreNode);
                                            FPGA.RequiredComponents.Add(LinkComponent);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return htLinks;
        }

        /// <summary>
        /// Reads in the Pre- or Post- Mapping and updates/modified the associated Groups and FPGAs.
        /// </summary>
        /// <param name="MappingNode">The XmlNode that "contains" the mapping specification from the XML File.</param>
        /// <param name="MapObjects">The Mapping object collection to be loaded.</param>
        /// <param name="ReadPreMapping">Indicates whether to read the PreMapping tag (true) or the PostMapping tag (false)</param>
        static void ReadMapping(XmlNode MappingNode,
                                FalconMapping_Objects MapObjects, 
                                bool ReadPreMapping)
        {
            for (int iMapGrp = 0; iMapGrp < MappingNode.ChildNodes.Count; iMapGrp++)
            {
                if (StringsEqual(MappingNode.ChildNodes[iMapGrp].Name, MAPPEDGROUP_NODE))
                {
                    // <MappedGroup>
                    XmlNode XMGNode = MappingNode.ChildNodes[iMapGrp];
                    string groupID = string.Empty;
                    string fpgaID = string.Empty;
                    foreach (XmlAttribute XA in XMGNode.Attributes)
                    {
                        if (StringsEqual(XA.Name, "ID"))
                            groupID = XA.Value;
                        else if (StringsEqual(XA.Name, "TargetFPGA"))
                            fpgaID = XA.Value;
                    }
                    if ((MapObjects.Groups.ContainsKey(groupID)) && (MapObjects.FPGAs.ContainsKey(fpgaID)))
                    {
                        FalconMapping_Group fmg = MapObjects.Groups[groupID];
                        FalconMapping_FPGA fmf = MapObjects.FPGAs[fpgaID];
                        fmf.MapGroup(fmg);
                        System.Diagnostics.Debug.WriteLine("Fixed-Mapping Group " + groupID + " to FPGA " + fpgaID);
                    }
                }
            }
        }

        #endregion

        #region Private Write "Helper" Functions

        /// <summary>
        /// Writes out the system "Objects" specification to an XMLDocument under the specified XMLElement Node.
        /// </summary>
        /// <param name="XDoc">The XmlDocument object that defines the schema on which the entire 
        /// document is to be defined.</param>
        /// <param name="XRoot">The XmlElement under which the Objects Element is to be populated within the document</param>
        /// <param name="MapObjects">The Mapping object collection to be saved.</param>
        static void WriteObjects(XmlDocument XDoc,
                                 XmlElement XRoot,
                                 FalconMapping_Objects MapObjects)
        {
            XmlElement XEObjects = XDoc.CreateElement(OBJECTS_NODE);
            XRoot.AppendChild(XEObjects);

            XmlElement XEComponents = XDoc.CreateElement(COMPONENTS_NODE);
            XEObjects.AppendChild(XEComponents);
            WriteComponents(XDoc, XEComponents, MapObjects);

            XmlElement XEFPGAs = XDoc.CreateElement(FPGAS_NODE);
            XEObjects.AppendChild(XEFPGAs);
            WriteFPGAs(XDoc, XEFPGAs, MapObjects);

            XmlElement XEGroups = XDoc.CreateElement(GROUPS_NODE);
            XEObjects.AppendChild(XEGroups);
            WriteGroups(XDoc, XEGroups, MapObjects);

            XmlElement XEClusters = XDoc.CreateElement(CLUSTERS_NODE);
            XEObjects.AppendChild(XEClusters);
            WriteClusters(XDoc, XEClusters, MapObjects);

        }

        /// <summary>
        /// Writes out the system "Components" specification to an XMLDocument under the specified XMLElement Node.
        /// </summary>
        /// <param name="XDoc">The XmlDocument object that defines the schema on which the entire 
        /// document is to be defined.</param>
        /// <param name="XComponents">The XmlElement under which the Components Element is to be populated within the document</param>
        /// <param name="MapObjects">The Mapping object collection to be saved.</param>
        static void WriteComponents(XmlDocument XDoc,
                                    XmlElement XComponents,
                                    FalconMapping_Objects MapObjects)
        {

            foreach (string cID in MapObjects.Components.Keys)
            {
                FalconMapping_Component fmc = MapObjects.Components[cID];
                XmlElement XComponent = XDoc.CreateElement(COMPONENT_NODE);
                XComponent.SetAttribute("Name", fmc.Name);
                XComponent.SetAttribute("ID", fmc.ID);
                XComponents.AppendChild(XComponent);

                Dictionary<string, long> Resources = fmc.Resources.GetResources();
                foreach (string resName in Resources.Keys)
                {
                    XmlElement XRes = XDoc.CreateElement(RESOURCE_NODE);
                    XRes.SetAttribute("Name", resName);
                    XRes.SetAttribute("Value", Resources[resName].ToString());
                    XComponent.AppendChild(XRes);
                }
            }
        }

        /// <summary>
        /// Writes out the system "FPGAs" specification to an XMLDocument under the specified XMLElement Node.
        /// </summary>
        /// <param name="XDoc">The XmlDocument object that defines the schema on which the entire 
        /// document is to be defined.</param>
        /// <param name="XFPGAs">The XmlElement under which the FPGAs Element is to be populated within the document</param>
        /// <param name="MapObjects">The Mapping object collection to be saved.</param>
        static void WriteFPGAs(XmlDocument XDoc,
                               XmlElement XFPGAs,
                               FalconMapping_Objects MapObjects)
        {
            foreach (string cID in MapObjects.FPGAs.Keys)
            {
                FalconMapping_FPGA fmf = MapObjects.FPGAs[cID];
                XmlElement XFPGA = XDoc.CreateElement(FPGA_NODE);
                XFPGA.SetAttribute("Name", fmf.Name);
                XFPGA.SetAttribute("ID", fmf.ID);
                XFPGAs.AppendChild(XFPGA);

                Dictionary<string, long> Resources = fmf.GetTotalResources();
                foreach (string resName in Resources.Keys)
                {
                    XmlElement XRes = XDoc.CreateElement(RESOURCE_NODE);
                    XRes.SetAttribute("Name", resName);
                    XRes.SetAttribute("Value", Resources[resName].ToString());
                    XFPGA.AppendChild(XRes);
                }
            }
        }

        /// <summary>
        /// Writes out the system "Groups" specification to an XMLDocument under the specified XMLElement Node.
        /// </summary>
        /// <param name="XDoc">The XmlDocument object that defines the schema on which the entire 
        /// document is to be defined.</param>
        /// <param name="XGroups">The XmlElement under which the Groups Element is to be populated within the document</param>
        /// <param name="MapObjects">The Mapping object collection to be saved.</param>
        static void WriteGroups(XmlDocument XDoc,
                                XmlElement XGroups,
                                FalconMapping_Objects MapObjects)
        {

            foreach (string cID in MapObjects.Groups.Keys)
            {
                FalconMapping_Group fmg = MapObjects.Groups[cID];
                XmlElement XGroup = XDoc.CreateElement(GROUP_NODE);
                XGroup.SetAttribute("Name", fmg.Name);
                XGroup.SetAttribute("ID", fmg.ID);
                XGroups.AppendChild(XGroup);

                List<FalconMapping_Component> Members = fmg.GetGroupedComponents();
                foreach (object member in Members)
                {
                    XmlElement XMember = XDoc.CreateElement(CMEMBER_NODE);
                    FalconMapping_Component fmComp = (FalconMapping_Component)member;
                    XMember.SetAttribute("ID", fmComp.ID);
                    XGroup.AppendChild(XMember);
                }
            }
        }

        /// <summary>
        /// Writes out the system "Clusters" specification to an XMLDocument under the specified XMLElement Node.
        /// </summary>
        /// <param name="XDoc">The XmlDocument object that defines the schema on which the entire 
        /// document is to be defined.</param>
        /// <param name="XClusters">The XmlElement under which the Clusters Element is to be populated within the document</param>
        /// <param name="MapObjects">The Mapping object collection to be saved.</param>
        static void WriteClusters(XmlDocument XDoc,
                                  XmlElement XClusters,
                                  FalconMapping_Objects MapObjects)
        {

            foreach (string cID in MapObjects.Clusters.Keys)
            {
                FalconMapping_Cluster fmg = MapObjects.Clusters[cID];
                XmlElement XCluster = XDoc.CreateElement(CLUSTER_NODE);
                XCluster.SetAttribute("Name", fmg.Name);
                XCluster.SetAttribute("ID", fmg.ID);
                XClusters.AppendChild(XCluster);

                ArrayList Members = fmg.GetMembers();
                foreach (object member in Members)
                {
                    XmlElement XMember = XDoc.CreateElement(FMEMBER_NODE);
                    FalconMapping_FPGA fmFPGA = (FalconMapping_FPGA)member;
                    XMember.SetAttribute("ID", fmFPGA.ID);
                    XCluster.AppendChild(XMember);
                }
            }
        }

        /// <summary>
        /// Writes out the system "Connectivity" specification to an XMLDocument under the specified XMLElement Node.
        /// </summary>
        /// <param name="XDoc">The XmlDocument object that defines the schema on which the entire 
        /// document is to be defined.</param>
        /// <param name="XRoot">The XmlElement under which the Connectivity Element is to be populated within the document</param>
        /// <param name="MapObjects">The Mapping object collection to be saved.</param>
        static void WriteConnectivity(XmlDocument XDoc,
                                    XmlElement XRoot,
                                    FalconMapping_Objects MapObjects)
        {
            XmlElement XEConnectivity = XDoc.CreateElement(CONNECTIVITY_NODE);
            XRoot.AppendChild(XEConnectivity);

            XmlElement XEConnections = XDoc.CreateElement(CONNECTIONS_NODE);
            XEConnectivity.AppendChild(XEConnections);
            WriteConnections(XDoc, XEConnections, MapObjects);

            XmlElement XELinks = XDoc.CreateElement(LINKS_NODE);
            XEConnectivity.AppendChild(XELinks);
            WriteLinks(XDoc, XELinks, MapObjects);
        }

        /// <summary>
        /// Writes out the system "Connections" specification to an XMLDocument under the specified XMLElement Node.
        /// </summary>
        /// <param name="XDoc">The XmlDocument object that defines the schema on which the entire 
        /// document is to be defined.</param>
        /// <param name="XConnections">The XmlElement under which the Connections Element is to be populated within the document</param>
        /// <param name="MapObjects">The Mapping object collection to be saved.</param>
        static void WriteConnections(XmlDocument XDoc,
                                    XmlElement XConnections,
                                    FalconMapping_Objects MapObjects)
        {
            foreach (string cID in MapObjects.Connections.Keys)
            {
                FalconMapping_Connection fmc = MapObjects.Connections[cID];
                XmlElement XConnection = XDoc.CreateElement(CONNECTION_NODE);
                //XConnections.SetAttribute("Name", fmc.Name);
                XConnection.SetAttribute("ID", fmc.ID);
                XConnection.SetAttribute("Input", fmc.SourceComponent);
                XConnection.SetAttribute("InputInstance", fmc.SourceComponentInstance);
                XConnection.SetAttribute("Output", fmc.SinkComponent);
                XConnection.SetAttribute("OutputInstance", fmc.SinkComponentInstance);
                XConnection.SetAttribute("DataWeight", fmc.DataDensity.ToString("#.000"));
                //XConnection.SetAttribute("Bidirectional", fmc.ID);
                XConnections.AppendChild(XConnection);
            }
        }

        /// <summary>
        /// Writes out the system "Links" specification to an XMLDocument under the specified XMLElement Node.
        /// </summary>
        /// <param name="XDoc">The XmlDocument object that defines the schema on which the entire 
        /// document is to be defined.</param>
        /// <param name="XLinks">The XmlElement under which the Links Element is to be populated within the document</param>
        /// <param name="MapObjects">The Mapping object collection to be saved.</param>
        static void WriteLinks(XmlDocument XDoc,
                               XmlElement XLinks,
                               FalconMapping_Objects MapObjects)
        {
            foreach (string cID in MapObjects.Links.Keys)
            {
                FalconMapping_Link fml = MapObjects.Links[cID];
                XmlElement XLink = XDoc.CreateElement(LINK_NODE);
                XLink.SetAttribute("ID", fml.ID);
                XLink.SetAttribute("Input", fml.SourceFPGA);
                XLink.SetAttribute("Output", fml.SinkFPGA);
                XLink.SetAttribute("LinkSpeed", fml.LinkSpeed.ToString("#.000"));
                XLink.SetAttribute("Bidirectional", fml.Bidirectional.ToString());
                XLinks.AppendChild(XLink);
            }
        }

        /// <summary>
        /// Writes out the Pre- or Post- Mapping specification to an XMLDocument under the specified XMLElement Node.
        /// </summary>
        /// <param name="XDoc">The XmlDocument object that defines the schema on which the entire 
        /// document is to be defined.</param>
        /// <param name="XRoot">The XmlElement under which the MappedGroup Elements are to be populated within the document</param>
        /// <param name="MapObjects">The Mapping object collection to be saved.</param>
        /// <param name="WritePreMapping">Indicates whether to write the PreMapping tag (true) or the PostMapping tag (false)</param>
        static void WriteMapping(XmlDocument XDoc,
                                XmlElement XRoot,
                                FalconMapping_Objects MapObjects, 
                                bool WritePreMapping)
        {
            XmlElement XMap;
            if (WritePreMapping)
                XMap = XDoc.CreateElement(PREMAPPING_NODE);
            else
                XMap = XDoc.CreateElement(POSTMAPPING_NODE);

            XMap.SetAttribute("Date", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());
            XRoot.AppendChild(XMap);

            foreach (string gID in MapObjects.Groups.Keys)
            {
                FalconMapping_Group fmg = MapObjects.Groups[gID];
                if (fmg.GetGroupedComponents().Count > 0)
                {
                    XmlElement XMGNode = XDoc.CreateElement(MAPPEDGROUP_NODE);
                    XMGNode.SetAttribute("ID", fmg.ID);
                    XMGNode.SetAttribute("TargetFPGA", fmg.TargetFPGA);
                    XMap.AppendChild(XMGNode);
                }
            }
        }

        #endregion

        /// <summary>
        /// Parses the three specified input files into the mapping system rather than utilizing a single input file.
        /// </summary>
        /// <param name="Platform">The path to the platform specification file.</param>
        /// <param name="Design">The path to the platform specification file.</param>
        /// <param name="Communications">The path to the platform specification file.</param>
        /// <param name="MapObjects">The FalconMapping_Objects object to be populated from the three files provided.</param>
        internal static bool ParseProjectFiles(string Platform, 
                                                string Design, 
                                                string Communications, 
                                                FalconMapping_Objects MapObjects)
        {
            try
            {
                string tempMapFile = string.Empty;
                XmlDocument DesignDoc = new XmlDocument();
                DesignDoc.Load(Design);
                MapObjects.RaiseMessageEvent("Loaded Design from: {0}", Design);
                XmlDocument CommDoc = new XmlDocument();
                CommDoc.Load(Communications);
                MapObjects.RaiseMessageEvent("Loaded Communications from: {0}", Communications);
                MapObjects.Reset();

                // Read Design doc first to get components and connections
                foreach(XmlNode xNode in DesignDoc.ChildNodes)
                {
                    if (StringsEqual(xNode.Name, "design"))
                    {
                        ProcessDesignCoresConnections(MapObjects, xNode);
                    }
                }
                LoadPlatformFile(MapObjects);
                // Then read communications document for FPGA Communication assignments
                foreach (XmlNode xNode in CommDoc.ChildNodes)
                {
                    if (StringsEqual(xNode.Name, "interfaces"))
                    {
                        foreach (XmlNode xIFaceNode in xNode.ChildNodes)
                        {
                            if (StringsEqual(xIFaceNode.Name, "interface"))
                            {
                                string BoardIPAddress = string.Empty;
                                string BoardMACAddress = string.Empty;
                                string BoardFPGAID = string.Empty;
                                foreach (XmlNode xAttr in xIFaceNode.Attributes)
                                {
                                    if (StringsEqual(xAttr.Name, "device"))
                                    {
                                        BoardFPGAID = xAttr.Value;
                                    }
                                }
                                foreach (XmlNode xIfaceProp in xIFaceNode.ChildNodes)
                                {
                                    if (StringsEqual(xIfaceProp.Name, "ipaddress"))
                                    {
                                        BoardIPAddress = xIfaceProp.InnerText;
                                    }
                                    if (StringsEqual(xIfaceProp.Name, "ethernetmac"))
                                    {
                                        BoardMACAddress = xIfaceProp.InnerText;
                                    }
                                }
                                if (BoardFPGAID != string.Empty)
                                {
                                    MapObjects.ModifyFPGAIPAddress(BoardFPGAID, BoardIPAddress);
                                    MapObjects.ModifyFPGAMACAddress(BoardFPGAID, BoardMACAddress);
                                }
                            }
                            //else if (StringsEqual(xIFaceNode.Name, "communications"))
                            //{
                            //    ProcessPlatformLinks(MapObjects, xIFaceNode);
                            //}
                        }
                    }
                }
                // Once everything has been loaded.   Map FPGA-required components to their respective FPGAs
                #region Map FPGA-Required Static Components
                foreach (FalconMapping_FPGA FPGA in MapObjects.FPGAs.Values)
                {
                    string ReqGroupID = String.Format("required_{0}", FPGA.ID);
                    if (!MapObjects.Groups.ContainsKey(ReqGroupID))
                    {
                        MapObjects.AddGroup(ReqGroupID, ReqGroupID);
                    }
                    FalconMapping_Group RequiredGroup = MapObjects.Groups[ReqGroupID];

                    foreach (FalconMapping_Component Component in FPGA.RequiredComponents)
                    {
                        if (MapObjects.Components.ContainsKey(Component.ID))
                        {
                            // This component has already been created elsewhere
                            FalconMapping_Component ExistingComponent = MapObjects.Components[Component.ID];
                            if (ExistingComponent.IsGrouped)
                            {
                                FalconMapping_Group ExistingGroup = MapObjects.Groups[ExistingComponent.GroupID];
                                if (ExistingGroup.IsMapped)
                                {
                                    // This should NOT happen, if it does we have a problem
                                }
                                else
                                {
                                    // We need to map the group to this FPGA
                                    FPGA.MapGroup(ExistingGroup);
                                }
                            }
                            else
                            {
                                RequiredGroup.AddComponent(ExistingComponent);
                            }
                        }
                        else
                        {
                            MapObjects.Components.Add(Component.ID, Component); // Add it to the collection
                            RequiredGroup.AddComponent(Component);
                        }
                    }
                    FPGA.MapGroup(RequiredGroup);
                }
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                MapObjects.ExProcessor(ex, "Caught in ParseProjectFiles()");
                return false;
            }
        }

        /// <summary>
        /// Process the XML section of the Platform that specifies the Links in the platform.
        /// </summary>
        /// <param name="MapObjects">The Mapping Algorithm objects library.</param>
        /// <param name="xNode">The XML node from which to parse.</param>
        private static void ProcessPlatformLinks(FalconMapping_Objects MapObjects, XmlNode xNode)
        {
            foreach (XmlNode xCommNode in xNode.ChildNodes)
            {
                if (StringsEqual(xCommNode.Name, "link"))
                {
                    string fSource = string.Empty;
                    string fSink = string.Empty;
                    bool fBidir = true;
                    foreach (XmlAttribute xAttr in xCommNode.Attributes)
                    {
                        if (StringsEqual(xAttr.Name, "source"))
                        {
                            fSource = xAttr.Value;
                        }
                        else if (StringsEqual(xAttr.Name, "sink"))
                        {
                            fSink = xAttr.Value;
                        }
                        else if (StringsEqual(xAttr.Name, "bidirectional"))
                        {
                            bool.TryParse(xAttr.Value, out fBidir);
                        }
                    }
                    string lnNameID = String.Format("{0}-{1}", fSource, fSink);
                    MapObjects.AddLink(lnNameID, lnNameID, 1.0, fSource, fSink, fBidir);
                }
            }
        }
        /// <summary>
        /// Loads the platform file identified in the PathManager loaded in the Mapping algorithm objects library.
        /// </summary>
        /// <param name="MapObjects">The Mapping Algorithm objects library.</param>
        /// <returns>True if successful, False otherwise.</returns>
        private static bool LoadPlatformFile(FalconMapping_Objects MapObjects)
        {
            try
            {
                FileInfo PlatformFile = new FileInfo(String.Format(@"{0}\{1}\{1}.xml", MapObjects.PathMan["Platforms"], MapObjects.PathMan["ProjectPlatform"]));
                if (!PlatformFile.Exists)
                    return false;
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(PlatformFile.FullName);

                foreach (XmlNode xElem in xDoc.ChildNodes)
                {
                    if (xElem.Name.ToLower() == "platform")
                    {
                        foreach (XmlNode xBoardNode in xElem.ChildNodes)
                        {
                            if (xBoardNode.Name.ToLower() == "board")
                            {
                                string boardFile = string.Empty;
                                string boardID = string.Empty;
                                foreach (XmlAttribute xAttr in xBoardNode.Attributes)
                                {
                                    if (xAttr.Name.ToLower() == "file")
                                    {
                                        boardFile = xAttr.Value;
                                    }
                                    else if (xAttr.Name.ToLower() == "id")
                                    {
                                        boardID = xAttr.Value;
                                    }
                                }
                                if ((boardID != string.Empty) && (boardFile != string.Empty))
                                {
                                    MapObjects.AddCluster(boardID, boardID);
                                    ProcessBoard(MapObjects, boardID, boardFile);
                                }
                            }
                        }
                        foreach (XmlNode xLinksNode in xElem.ChildNodes)
                        {
                            if (String.Compare(xLinksNode.Name, "Links", true) == 0)
                            {
                                ReadLinks(xLinksNode, MapObjects);
                            }
                        }
                    }
                }
                MapObjects.RaiseMessageEvent("Loaded Platform from: {0}", PlatformFile);
                return true;
            }
            catch (Exception ex)
            {
                MapObjects.RaiseMessageEvent(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Parses the Platform Board definition identified in the PathManager loaded in the Mapping algorithm objects library.
        /// </summary>
        /// <param name="MapObjects">The Mapping Algorithm objects library.</param>
        /// <param name="boardID">The ID of the board to be parsed.</param>
        /// <param name="BoardFileName">The name of the board definition file to be parsed.</param>
        private static void ProcessBoard(FalconMapping_Objects MapObjects, 
                                        string boardID, 
                                        string BoardFileName)
        {
            string BoardShortName = BoardFileName.Substring(0, BoardFileName.IndexOf("."));
            FileInfo BoardFile = new FileInfo(String.Format(@"{0}\{1}\{2}\{2}.xml", MapObjects.PathMan["Platforms"], MapObjects.PathMan["ProjectPlatform"], BoardShortName));
            if (!BoardFile.Exists)
                return;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(BoardFile.FullName);

            foreach (XmlNode xBoard in xDoc.ChildNodes)
            {
                if (xBoard.Name.ToLower() == "board")
                {
                    foreach (XmlNode xFPGA in xBoard.ChildNodes)
                    {
                        if (xFPGA.Name.ToLower() == "fpga")
                        {
                            string fpgaFile = string.Empty;
                            string fpgaID = string.Empty;
                            foreach (XmlAttribute xAttr in xFPGA.Attributes)
                            {
                                if (xAttr.Name.ToLower() == "file")
                                {
                                    fpgaFile = xAttr.Value;
                                }
                                else if (xAttr.Name.ToLower() == "id")
                                {
                                    fpgaID = xAttr.Value;
                                }
                            }
                            if ((fpgaID != string.Empty) && (fpgaFile != string.Empty))
                            {
                                ProcessFPGA(MapObjects, boardID, BoardShortName, fpgaID, fpgaFile);
                            }
                            string mappingID = String.Format("{0}.{1}", boardID, fpgaID);
                            FalconMapping_FPGA currentFPGA = null;
                            if (MapObjects.FPGAs.ContainsKey(mappingID))
                            {
                                currentFPGA = MapObjects.FPGAs[mappingID];
                                List<CerebrumCore> ReqCores = CoreLibrary.ReadRequiredComponentsFromXML(MapObjects.PathMan, xFPGA, currentFPGA.ID);
                                foreach (CerebrumCore ReqCore in ReqCores)
                                {
                                    FalconMapping_Component ReqComp = new FalconMapping_Component(ReqCore.CoreInstance, ReqCore.CoreType, string.Empty);
                                    ReqComp.SupportedArchitectures.AddRange(ReqComp.SupportedArchitectures);
                                    ReqComp.InternalComponentObject = ReqCore;
                                    if ((ReqComp.Source == null) || (ReqComp.Source == string.Empty))
                                        ReqComp.Source = ReqCore.CoreLocation;
                                    currentFPGA.RequiredComponents.Add(ReqComp);
                                }
                            }
                            else 
                            {
                                throw new Exceptions.IDDoesNotExistException(String.Format("Unable to find FPGA with ID {0} during platform load", String.Format("{0}.{1}", boardID, fpgaID)));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses the Platform FPGA definition identified in the PathManager loaded in the Mapping algorithm objects library.
        /// </summary>
        /// <param name="MapObjects">The Mapping Algorithm objects library.</param>
        /// <param name="boardID">The ID of the Board that the FPGA to be parsed is part of.</param>
        /// <param name="BoardShortName">The name of the Board that the FPGA to be parsed is part of.</param>
        /// <param name="FPGAID">The ID of the FPGA to be parsed.</param>
        /// <param name="FPGAFileName">The name of the FPGA definition file to be parsed.</param>
        private static void ProcessFPGA(FalconMapping_Objects MapObjects, 
                                        string boardID, 
                                        string BoardShortName, 
                                        string FPGAID, 
                                        string FPGAFileName)
        {
            string FPGAShortName = FPGAFileName.Substring(0, FPGAFileName.IndexOf("."));
            FileInfo FPGAFile = new FileInfo(String.Format(@"{0}\{1}\{2}\{3}\{3}.xml", MapObjects.PathMan["Platforms"], MapObjects.PathMan["ProjectPlatform"], BoardShortName, FPGAShortName));
            if (!FPGAFile.Exists)
                return;

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(FPGAFile.FullName);
            foreach (XmlNode xNode in xDoc.ChildNodes)
            {
                if (xNode.Name.ToLower() != "xml")
                {
                    // Find the global memory attribute
                    long globalMem = 0;
                    foreach (XmlNode xBoardItem in xNode.ChildNodes)
                    {
                        if (StringsEqual(xBoardItem.Name, "global_attributes"))
                        {
                            foreach (XmlNode xAttrNode in xBoardItem.ChildNodes)
                            {
                                if (StringsEqual(xAttrNode.Name, "resource"))
                                {
                                    long sticks = -1;
                                    long per_stick = -1;
                                    foreach (XmlAttribute xAttr in xAttrNode.Attributes)
                                    {
                                        if (StringsEqual(xAttr.Name, "memory_sticks"))
                                        {
                                            long val;
                                            if (long.TryParse(xAttr.Value, out val))
                                                sticks = val;
                                        }
                                        else if (StringsEqual(xAttr.Name,  "mb_per_stick"))
                                        {
                                            long val;
                                            if (long.TryParse(xAttr.Value, out val))
                                                per_stick = val;
                                        }
                                    }
                                    if ((sticks >= 0) && (per_stick >= 0))
                                    {
                                        globalMem = sticks * (per_stick * 1024 * 1024);                                        
                                    }
                                }
                            }
                        }
                    }
                    foreach (XmlNode xBoardItem in xNode.ChildNodes)
                    {
                        if (StringsEqual(xBoardItem.Name, "fpga"))
                        {
                            string fpgaFamily = string.Empty;
                            // Get Key information
                            foreach (XmlAttribute xAttr in xBoardItem.Attributes)
                            {
                                if (StringsEqual(xAttr.Name, "family"))
                                {
                                    fpgaFamily = xAttr.Value;
                                }
                            }
                            FPGAID = boardID + "." + FPGAID;
                            MapObjects.AddFPGA(FPGAID, FPGAID, fpgaFamily, null);
                            MapObjects.ModifyFPGAResource(FPGAID, "memory", globalMem);
                            MapObjects.AddFPGAToCluster(FPGAID, boardID);
                            foreach (XmlNode xFPGAProp in xBoardItem.ChildNodes)
                            {
                                if (String.Compare(xFPGAProp.Name, "resource", true) == 0)
                                {
                                    ParseResourceNode(xFPGAProp, MapObjects, FPGAID);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Processes the design file Cores and Connections into the mapping algorithm object library.
        /// </summary>
        /// <param name="MapObjects">The Mapping Algorithm objects library.</param>
        /// <param name="xNode">The XML node to be parsed.</param>
        private static void ProcessDesignCoresConnections(FalconMapping_Objects MapObjects, 
                                                        XmlNode xNode)
        {
            foreach(XmlNode xDesNode in xNode.ChildNodes)
            {
                if (StringsEqual(xDesNode.Name, "logic"))
                {
                    // Process Cores First
                    int CorePort = 1234;    // Base port assignment
                    foreach(XmlNode xCompNode in xDesNode.ChildNodes)
                    {
                        if (StringsEqual(xCompNode.Name, "core"))
                        {
                            string cID = string.Empty;
                            string cName = string.Empty;
                            string cSource = string.Empty;
                            string cArches = string.Empty;
                            bool bRequiresServer = false;
                            foreach(XmlAttribute xAttr in xCompNode.Attributes)
                            {
                                if (StringsEqual(xAttr.Name, "id"))
                                {
                                    cID = xAttr.Value;
                                }
                                else if (StringsEqual(xAttr.Name, "name"))
                                {
                                    cName = xAttr.Value;
                                }
                                else if (StringsEqual(xAttr.Name, "location"))
                                {
                                    cSource = xAttr.Value;
                                }
                                else if (StringsEqual(xAttr.Name, "server"))
                                {
                                    bool.TryParse(xAttr.Value, out bRequiresServer);
                                }
                                else if (StringsEqual(xAttr.Name, "supportedarch"))
                                {
                                    cArches = xAttr.Value;
                                }
                            }
                            MapObjects.AddComponent(cID, cName, cArches, null);
                            if (bRequiresServer)
                                MapObjects.ModifyComponentPort(cID, CorePort++);
                            else
                                MapObjects.ModifyComponentPort(cID, -1);
                            MapObjects.SetComponentSource(cID, cSource);
                            FalconMapping_Component NewComponent = MapObjects.Components[cID];
                            MapObjects.LoadComponentData(cID);

                            foreach(XmlNode xResNode in xCompNode.ChildNodes)
                            {
                                string resName = string.Empty;
                                string resValue = string.Empty;
                                foreach (XmlAttribute xAttr in xResNode.Attributes)
                                {
                                    if (StringsEqual(xAttr.Name, "name"))
                                    {
                                        resName = xAttr.Value;
                                    }
                                    else if (StringsEqual(xAttr.Name, "value"))
                                    {
                                        resValue = xAttr.Value;
                                    }
                                }
                                int val = 0;
                                if (int.TryParse(resValue, out val))
                                {
                                    MapObjects.ModifyComponentResource(cID, resName, int.Parse(resValue));
                                }
                                else
                                {
                                    throw new InvalidCastException(String.Format("Resource amount for resource {0} in Core {1} is invalid.", resName, cID));
                                }
                            }
                        }
                    }
                }
            }
            MapObjects.LoadComponentConfigs();

            foreach (XmlNode xDesNode in xNode.ChildNodes)
            {
                if (StringsEqual(xDesNode.Name, "connections"))
                {
                    MapObjects.Connections = ReadConnections(xDesNode, MapObjects);
                }
                else if (StringsEqual(xDesNode.Name, "groups"))
                {
                    MapObjects.Groups = ReadGroups(xDesNode, MapObjects);
                }
            }
        }

        
        /// <summary>
        /// Parses a resource value node from the specified XML node.
        /// </summary>
        /// <param name="xResNode">The XML node to be parsed.</param>
        /// <param name="MapObjects">The Mapping Algorithm objects library.</param>
        /// <param name="fpgaID">The ID of the FPGA to which the resources are to be attributed.</param>
        private static void ParseResourceNode(XmlNode xResNode, 
                                              FalconMapping_Objects MapObjects, 
                                                string fpgaID)
        {
            string ResName = string.Empty;
            long ResAmount = 0;
            foreach (XmlAttribute xAttr in xResNode.Attributes)
            {
                if (String.Compare(xAttr.Name, "Name", true) == 0)
                {
                    ResName = xAttr.Value;
                }
                else if (String.Compare(xAttr.Name, "Amount", true) == 0)
                {
                    long.TryParse(xAttr.Value, out ResAmount);
                }
            }

            if ((ResAmount > 0) && (ResName != string.Empty))
            {
                MapObjects.ModifyFPGAResource(fpgaID, ResName.ToLower(), ResAmount);
            }
        }

        internal static bool UpdateCommunicationInterfaces(FalconMapping_Objects MapObjects,
                                                            string CommsFile)
        {
            try
            {
                XmlDocument CommsDoc = new XmlDocument();
                CommsDoc.Load(CommsFile);
                foreach (XmlNode xIFSNode in CommsDoc.ChildNodes)
                {
                    if (StringsEqual(xIFSNode.Name, "interfaces"))
                    {
                        foreach (XmlNode xIFNode in xIFSNode.ChildNodes)
                        {
                            if (StringsEqual(xIFNode.Name, "interface"))
                            {
                                string IFInst = string.Empty;
                                foreach (XmlAttribute xAttr in xIFNode.Attributes)
                                {
                                    if (string.Compare(xAttr.Name, "Instance", true) == 0)
                                    {
                                        IFInst = xAttr.Value;
                                        break;
                                    }
                                }
                                if (IFInst != string.Empty)
                                {
                                    foreach (FalconMapping_Component Comp in MapObjects.GetComponents().Values)
                                    {
                                        foreach (ComponentCore CompCore in Comp.ComponentCores.Values)
                                        {
                                            if (String.Compare(CompCore.CoreInstance, IFInst, true) == 0)
                                            {
                                                // Found the processor
                                                string TargetFPGA = MapObjects.GetComponentFPGAID(Comp.ID);
                                                bool bFoundFPGATag = false;
                                                foreach (XmlNode xIFProp in xIFNode.ChildNodes)
                                                {
                                                    if (String.Compare(xIFProp.Name, "FPGA", true) == 0)
                                                    {
                                                        bFoundFPGATag = true;
                                                        if (TargetFPGA != string.Empty)
                                                        {
                                                            xIFProp.InnerText = TargetFPGA;
                                                        }
                                                        else
                                                        {
                                                            xIFNode.RemoveChild(xIFProp);
                                                        }
                                                    }
                                                }
                                                if ((!bFoundFPGATag) && (TargetFPGA != string.Empty))
                                                {
                                                    XmlNode xIFFPGA = CommsDoc.CreateElement("FPGA");
                                                    xIFFPGA.InnerText = TargetFPGA;
                                                    xIFNode.AppendChild(xIFFPGA);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                CommsDoc.Save(CommsFile);
                return true;
            }
            catch (Exception ex)
            {
                MapObjects.RaiseMessageEvent(ErrorReporting.ExceptionDetails(ex));
            }
            return false;
        }
        internal static bool UpdateDesignProcessors(FalconMapping_Objects MapObjects,
                                                    string DesignFile)
        {
            try
            {
                XmlDocument DesignDoc = new XmlDocument();
                DesignDoc.Load(DesignFile);
                foreach (XmlNode xNode in DesignDoc.ChildNodes)
                {
                    if (StringsEqual(xNode.Name, "design"))
                    {
                        foreach (XmlNode xProcessorsNode in xNode.ChildNodes)
                        {
                            if (StringsEqual(xProcessorsNode.Name, "processors"))
                            {
                                foreach (XmlNode xProcNode in xProcessorsNode)
                                {
                                    string ProcInst = string.Empty;
                                    foreach (XmlAttribute xAttr in xProcNode.Attributes)
                                    {
                                        if (string.Compare(xAttr.Name, "Instance", true) == 0)
                                        {
                                            ProcInst = xAttr.Value;
                                            break;
                                        }
                                    }
                                    if (ProcInst != string.Empty)
                                    {
                                        foreach (FalconMapping_Component Comp in MapObjects.GetComponents().Values)
                                        {
                                            foreach (ComponentCore CompCore in Comp.ComponentCores.Values)
                                            {
                                                if (String.Compare(CompCore.CoreInstance, ProcInst, true) == 0)
                                                {
                                                    // Found the processor
                                                    string TargetFPGA = MapObjects.GetComponentFPGAID(Comp.ID);
                                                    bool bFoundFPGATag = false;
                                                    foreach (XmlNode xProcProp in xProcNode.ChildNodes)
                                                    {
                                                        if (String.Compare(xProcProp.Name, "FPGA", true) == 0)
                                                        {
                                                            bFoundFPGATag = true;
                                                            if (TargetFPGA != string.Empty)
                                                            {
                                                                xProcProp.InnerText = TargetFPGA;
                                                            }
                                                            else
                                                            {
                                                                xProcNode.RemoveChild(xProcProp);
                                                            }
                                                        }
                                                    }
                                                    if ((!bFoundFPGATag) && (TargetFPGA != string.Empty))
                                                    {
                                                        XmlNode xProcFPGA = DesignDoc.CreateElement("FPGA");
                                                        xProcFPGA.InnerText = TargetFPGA;
                                                        xProcNode.AppendChild(xProcFPGA);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                DesignDoc.Save(DesignFile);
                return true;
            }
            catch (Exception ex)
            {
                MapObjects.RaiseMessageEvent(ErrorReporting.ExceptionDetails(ex));
            }
            return false;
        }                                                   
    }
}
