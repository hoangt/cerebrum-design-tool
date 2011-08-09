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
 * CoreLibrary.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: This is a Netron-based object representing a Cerebrum Core library.
 * History: 
 * >> (20 Jun 2011) Matthew Cotter: Added parsing of CoreLibraryCategory into CerebrumCore objects in LoadCoreDefinition.
 *                                  Added method SaveCoreDefinition to save a CerebrumCore definition to a directory.
 * >> ( 9 May 2011) Matthew Cotter: Added support for Vortex Types Edge and Bridge to the Vortex "interfaces" supported by the Cerebrum Core.
 * >> (20 Apr 2011) Matthew Cotter: Corrected bug in LoadXPSMap() that caused all components to ALWAYS be imported into all projects in a multi FPGA platform.
 * >> (18 Feb 2011) Matthew Cotter: Made LoadXPSMap a static method here, called by the method of the same name in XPS Builder XpsProjectOptions class.  This allows 
 *                                      the Platform Synthesis tool to invoke this function to load the list of mapped components as well.
 * >> (15 Feb 2011) Matthew Cotter: Overhaul as part of code reorganization to facilitate uniform access to/from Component/Core objects.
 *                                      Added static methods for locating and loading component definition files.
 *                                      Added static method for loading and creating CerebrumCores from required core definitions from Platform XML.
 * >> (18 Jan 2011) Matthew Cotter: Added support for GUI-added platform components to be visible within the design GUI.
 * >> (22 Dec 2010) Matthew Cotter: Added additional support for customizable clock management.
 * >> (16 Dec 2010) Matthew Cotter: Implemented support of <Visible> tag in Core definition to prevent cores with it set to false from appearing in GUI toolbox.
 *                                  Corrected instantiation of cores by type/version rather than Netron GUID.
 * >> (24 Oct 2010) Matthew Cotter: Added support for translating and evaluating conditions and properties within the context of previously set properties.
 * >> (23 Oct 2010) Matthew Cotter: Added support for differentiated port location (Input and Output) located differently based on corresponding type.
 *                                  Corrected minor bug in constructor that improperly initialized Owner property.
 * >> (22 Oct 2010) Matthew Cotter: Corrected crash bug that results from incorrect location of core port connectors
 *                                  Added support for sub-component property load/save within core packages.
 * >> (11 Oct 2010) Matthew Cotter: Moved core package integration into CoreLibrary where package definition is loaded.
 * >> (10 Oct 2010) Matthew Cotter: Initial support for core packages (multiple pcores per CCore) and integration with ClockGenerator library.
 *                                  Corrected core port location and association in concert with Vortex integration for GUI support
 * >> (24 Sep 2010) Matthew Cotter: Continued work on right-click support for properties.
 * >> (23 Sep 2010) Matthew Cotter: Added support for loading, saving, and accessing properties associated with a CCore.
 *                                  Started event support for click events (left and right)
 * >> (21 Sep 2010) Matthew Cotter: Added support for accessing custom properties associated with a CCore.
 * >> (17 Sep 2010) Matthew Cotter: Added ImagePath property for displaying cores in GUI.
 * >> (13 Sep 2010) Matthew Cotter: Implemented basics of Netron-inspired Library class, along with Cerebrum/Core specific methods to load Core package definitions.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Drawing.Imaging;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using Netron;
using Netron.Diagramming;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;
using System.Xml;
using CerebrumSharedClasses;
using FalconClockManager;
using VortexInterfaces.VortexCommon;
using FalconPathManager;

namespace CerebrumNetronObjects
{
    /// <summary>
    /// Object defining a collection of associated cores
    /// </summary>
    public class CoreLibrary
    {
        /// <summary>
        /// List of Netron 'shapes' defined in the library.
        /// </summary>
        protected CollectionBase<IShape> myShapes;

        /// <summary>
        /// List of Invisible Netron 'shapes' defined in the library.
        /// </summary>
        protected CollectionBase<IShape> hiddenShapes;

        /// <summary>
        /// Constructor. Initializes an empty CoreLibrary with the specified name
        /// </summary>
        /// <param name="Name"></param>
        public CoreLibrary(string Name)
        {
            this.Name = Name;
            myShapes = new CollectionBase<IShape>();
            hiddenShapes = new CollectionBase<IShape>();
        }

        /// <summary>
        /// Loads a core definition from the specified file into this library
        /// </summary>
        /// <param name="path">The path to the Core Definition file</param>
        public void LoadCoreDefinition(string path)
        {
            try
            {
                FileInfo CoreDef = new FileInfo(path);
                CerebrumCore cCore = null;
                try
                {
                    cCore = ParseOutCoreParameters(CoreDef);
                }
                catch (Exception ex)
                {
                    CerebrumSharedClasses.ErrorReporting.TraceException(ex);
                    return;
                }
                if (cCore != null)
                {
                    if (cCore.VisibleInLibrary)
                    {
                        Trace.WriteLine(String.Format("Loaded Core {0} from {1} into Library {2}.", cCore.CoreName, path, this.Name));
                        myShapes.Add(cCore);
                    }
                    else
                    {
                        Trace.WriteLine(String.Format("Loaded Core {0} from {1}, but it is flagged as non-visible.", cCore.CoreName, path, this.Name));
                        hiddenShapes.Add(cCore);
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message, "An error occurred while " +
                    "loading library from:" + path);
                CerebrumSharedClasses.ErrorReporting.TraceException(e);
                return;
            }
        }

        /// <summary>
        /// The name of this Core Library
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The list of shapes (cores) defined in this library
        /// </summary>
        public CollectionBase<IShape> Shapes
        {
            get
            {
                return myShapes;
            }
        }

        /// <summary>
        /// Netron method.  Determines whether an object with the specified GUID exists within the library.
        /// </summary>
        /// <param name="guid">The GUID to search for</param>
        /// <returns>True if an object with the specified GUID is found, false otherwise.</returns>
        public bool ContainsShape(string guid)
        {
            foreach (IShape shape in myShapes)
            {
                if (shape.Uid.ToString() == guid)
                {
                    return true;
                }
            }
            foreach (IShape shape in hiddenShapes)
            {
                if (shape.Uid.ToString() == guid)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Netron method.  Creates an object with the specified GUID if it exists within the library.
        /// </summary>
        /// <param name="guid">The GUID to search for</param>
        /// <returns>An IShape object if the GUID was found, null otherwise</returns>
        public IShape CreateNewInstance(string guid)
        {
            foreach (IShape shape in myShapes)
            {
                if (shape.Uid.ToString() == guid)
                {
                    CerebrumCore CC = (CerebrumCore)shape;
                    IShape newInstance = (IShape)(new CerebrumCore(string.Empty, ((CerebrumCore)shape)));
                    return newInstance;
                }
            }
            foreach (IShape shape in hiddenShapes)
            {
                if (shape.Uid.ToString() == guid)
                {
                    CerebrumCore CC = (CerebrumCore)shape;
                    IShape newInstance = (IShape)(new CerebrumCore(string.Empty, ((CerebrumCore)shape)));
                    return newInstance;
                }
            }
            return null;
        }
        /// <summary>
        /// Netron method.  Creates an object with the specified GUID if it exists within the library.
        /// </summary>
        /// <param name="Instance">The Instance name to assign to the new instance</param>
        /// <param name="guid">The GUID to search for</param>
        /// <returns>An IShape object if the GUID was found, null otherwise</returns>
        public IShape CreateNewInstance(string Instance, string guid)
        {
            foreach (IShape shape in myShapes)
            {
                if (shape.Uid.ToString() == guid)
                {
                    CerebrumCore CC = (CerebrumCore)shape;
                    IShape newInstance = (IShape)(new CerebrumCore(Instance, ((CerebrumCore)shape)));
                    return newInstance;
                }
            }
            foreach (IShape shape in hiddenShapes)
            {
                if (shape.Uid.ToString() == guid)
                {
                    CerebrumCore CC = (CerebrumCore)shape;
                    IShape newInstance = (IShape)(new CerebrumCore(Instance, ((CerebrumCore)shape)));
                    return newInstance;
                }
            }
            return null;
        }
        /// <summary>
        /// Determines whether a core of the specified type and version exists within the library
        /// </summary>
        /// <param name="Type">The type name of the core to search for</param>
        /// <param name="Version">The version the core to search for</param>
        /// <returns>True if a matching core was found, false otherwise</returns>
        public bool ContainsCore(string Type, string Version)
        {
            foreach (IShape shape in myShapes)
            {
                if (shape.GetType() == typeof(CerebrumCore))
                {
                    CerebrumCore CC = (CerebrumCore)shape;
                    if ((CC.CoreType == Type) && (CC.CoreVersion == Version))
                        return true;
                }
            }
            foreach (IShape shape in hiddenShapes)
            {
                if (shape.GetType() == typeof(CerebrumCore))
                {
                    CerebrumCore CC = (CerebrumCore)shape;
                    if ((CC.CoreType == Type) && (CC.CoreVersion == Version))
                        return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Creates a core of the specified type and version if it exists within the library
        /// </summary>
        /// <param name="Instance">The instance name of the new core to be created</param>
        /// <param name="Type">The type name of the core to search for</param>
        /// <param name="Version">The version the core to search for</param>
        /// <returns>A CerebrumCore if a match was found, null otherwise</returns>
        public CerebrumCore CreateCoreInstance(string Instance, string Type, string Version)
        {
            foreach (IShape shape in myShapes)
            {
                if (shape.GetType() == typeof(CerebrumCore))
                {
                    CerebrumCore CC = (CerebrumCore)shape;
                    if ((CC.CoreType == Type) && (CC.CoreVersion == Version))
                    {
                        CC = (CerebrumCore)((IShape)CreateNewInstance(Instance, CC.Uid.ToString()));
                        CC.CoreInstance = Instance;
                        CC.CoreError += new CoreErrorMessage(OnCoreError);
                        return CC;
                    }
                }
            }
            foreach (IShape shape in hiddenShapes)
            {
                if (shape.GetType() == typeof(CerebrumCore))
                {
                    CerebrumCore CC = (CerebrumCore)shape;
                    if ((CC.CoreType == Type) && (CC.CoreVersion == Version))
                    {
                        CC = (CerebrumCore)((IShape)CreateNewInstance(Instance, CC.Uid.ToString()));
                        CC.CoreInstance = Instance;
                        CC.CoreError += new CoreErrorMessage(OnCoreError);
                        return CC;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Event fired when this core generates an error message
        /// </summary>
        public event CoreErrorMessage CoreError;
        /// <summary>
        /// Fires the CoreError event if it is defined.
        /// </summary>
        /// <param name="Core">A reference to the CerebrumCore generating the error.</param>
        /// <param name="Message">A message describing the error that was generated.</param>
        public void OnCoreError(CerebrumCore Core, string Message)
        {
            if (CoreError != null)
                CoreError(Core, Message);
        }

        /// <summary>
        /// Loads specified XML document into a CerebrumCore Object.  If the XML specification includes the Visible tag and LoadInivisble argument are both set to false, 
        /// this method will return null.
        /// </summary>
        /// <param name="DocumentPath">The path to the XML document.</param>
        /// <param name="LoadInvisible">Indicates whether the core should be completely loaded if the Visible flag is set to false in the XML specification.</param>
        /// <returns>A CerebrumCore object, if successful and either Visible tag or LoadInvisible argument are true.   Null otherwise.</returns>
        public CerebrumCore ParseOutCoreParameters(FileInfo DocumentPath, bool LoadInvisible)
        {
            return CoreLibrary.LoadCoreDefinition(DocumentPath, LoadInvisible, this);
        }

        /// <summary>
        /// Loads specified XML document into a CerebrumCore Object.  If the XML specification includes the Visible tag set to false, 
        /// this method will return null.
        /// </summary>
        /// <param name="DocumentPath">The path to the XML document.</param>
        /// <returns>A CerebrumCore object, if successful and the document allows the core to be visible.   Null otherwise.</returns>
        public CerebrumCore ParseOutCoreParameters(FileInfo DocumentPath)
        {
            return CoreLibrary.LoadCoreDefinition(DocumentPath, false, this);
        }
        
        #region Static Methods

        /// <summary>
        /// Searches the core search paths defined in the specified project path manager for the core directory specified.
        /// </summary>
        /// <param name="PathMan">The project path manager with paths loaded.</param>
        /// <param name="SourceDirectory">The name of the core directory/definition to search for.</param>
        /// <returns>A FileInfo object representing the core definition file, if it was located.  Null otherwise.</returns>
        public static FileInfo LocateCoreDefinition(PathManager PathMan, string SourceDirectory)
        {
            // Scan core repositories from the Paths file
            string SearchPaths = PathMan["CerebrumCores"];
            if (PathMan.HasPath("CoreSearchPaths"))
            {
                SearchPaths = String.Format("{0};{1}", SearchPaths, PathMan["CoreSearchPaths"]);
            }
            string[] SearchList = SearchPaths.Split(';');

            for (int i = 0; i < SearchList.Length; i++)
            {
                string SearchPath = SearchList[i].Trim();
                if (SearchPath == string.Empty)
                    continue;
                if (Directory.Exists(SearchPath))
                {
                    // Get a list of directories in the Repo Path
                    foreach (string Dir in Directory.GetDirectories(SearchPath))
                    {
                        DirectoryInfo di = new DirectoryInfo(Dir);
                        if (String.Compare(di.Name, SourceDirectory) == 0)
                        {
                            // Get all *.XML files in the directory
                            foreach (FileInfo fi in di.GetFiles("*.xml", SearchOption.TopDirectoryOnly))
                            {
                                // Look for one that corresponds to the directory name
                                if (String.Compare(fi.Name, String.Format("{0}.xml", di.Name)) == 0)
                                {
                                    return fi;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Loads specified XML document into a CerebrumCore Object.  If the XML specification includes the Visible tag and LoadInivisble argument are both set to false, 
        /// this method will return null.
        /// </summary>
        /// <param name="DocumentPath">The path to the XML document.</param>
        /// <param name="LoadInvisible">Indicates whether the core should be completely loaded if the Visible flag is set to false in the XML specification.</param>
        /// <param name="Lib">A CoreLibrary object used to generate Error Messages to the GUI, if one is available.  If null, no CoreError events are fired.</param>
        /// <returns>A CerebrumCore object, if successful and either Visible tag or LoadInvisible argument are true.   Null otherwise.</returns>
        public static CerebrumCore LoadCoreDefinition(FileInfo DocumentPath, bool LoadInvisible, CoreLibrary Lib)
        {
            return LoadCoreDefinition(DocumentPath, LoadInvisible, Lib, null);
        }
        /// <summary>
        /// Loads specified XML document into a CerebrumCore Object.  If the XML specification includes the Visible tag and LoadInivisble argument are both set to false, 
        /// this method will return null.
        /// </summary>
        /// <param name="DocumentPath">The path to the XML document.</param>
        /// <param name="LoadInvisible">Indicates whether the core should be completely loaded if the Visible flag is set to false in the XML specification.</param>
        /// <param name="Lib">A CoreLibrary object used to generate Error Messages to the GUI, if one is available.  If null, no CoreError events are fired.</param>
        /// <param name="FixedInstanceName">Assigns this fixed instance name to the new component created from the definition.  If this parameter is an empty string or null, it is ignored.</param>
        /// <returns>A CerebrumCore object, if successful and either Visible tag or LoadInvisible argument are true.   Null otherwise.</returns>
        public static CerebrumCore LoadCoreDefinition(FileInfo DocumentPath, bool LoadInvisible, CoreLibrary Lib, string FixedInstanceName)
        {
            // Parse everything regardless of whether the core is visible in the library toolbox... it may still show up on the design field.
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(DocumentPath.FullName);

            CerebrumCore cCore = new CerebrumCore();
            try
            {
                if ((FixedInstanceName != null) && (FixedInstanceName != string.Empty))
                {
                    cCore.CoreInstance = FixedInstanceName;
                }

                cCore.CoreLocation = DocumentPath.Directory.Name;

                XmlNode xCore = CerebrumXmlInterface.GetXmlNode(xDoc, "CerebrumCore.General");
                XmlNode xDesign = CerebrumXmlInterface.GetXmlNode(xDoc, "CerebrumCore.Software.DesignDisplay");
                XmlNode xHardware = CerebrumXmlInterface.GetXmlNode(xDoc, "CerebrumCore.Hardware");

                #region Parse General Core Properties
                cCore.CoreTrueName = CerebrumXmlInterface.GetXmlInnerText(xCore, "Name", true);
                cCore.CoreType = CerebrumXmlInterface.GetXmlInnerText(xCore, "Type", true);
                cCore.CoreVersion = CerebrumXmlInterface.GetXmlInnerText(xCore, "Version", true);
                cCore.CoreOwner = CerebrumXmlInterface.GetXmlInnerText(xCore, "Owner", true);
                cCore.CoreDescription = CerebrumXmlInterface.GetXmlInnerText(xCore, "Description", true);
                cCore.CoreInstancePrefix = CerebrumXmlInterface.GetXmlInnerText(xCore, "InstancePrefix", true);
                cCore.CoreName = String.Format("{0} (v{1})", cCore.CoreTrueName, cCore.CoreVersion);
                cCore.CoreLibraryCategory = CerebrumXmlInterface.GetXmlAttribute(xDoc, "CerebrumCore.Software.DesignDisplay.Category", "Name", true);
                if ((cCore.CoreLibraryCategory == null) || (cCore.CoreLibraryCategory == string.Empty))
                {
                    cCore.CoreLibraryCategory = "General / Unsorted";
                }

                XmlNode VisibleNode = CerebrumXmlInterface.GetXmlNode(xDoc, "CerebrumCore.General.Visible");
                cCore.VisibleInLibrary = true;
                if (VisibleNode != null)
                {
                    bool val = true;
                    bool.TryParse(VisibleNode.InnerText, out val);
                    cCore.VisibleInLibrary = val;
                }

                string Keywords = CerebrumXmlInterface.GetXmlInnerText(xCore, "Keywords", true);
                cCore.CoreKeywords.Clear();
                cCore.CoreKeywords.AddRange(Keywords.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
                string UseServer = CerebrumXmlInterface.GetXmlInnerText(xCore, "CoreServer", true);
                bool bUseServer = false;
                if (!bool.TryParse(UseServer, out bUseServer))
                {
                    Trace.WriteLine(String.Format("Error parsing boolean string -- CerebrumCore.General.CoreServer in ",
                        DocumentPath.FullName));
                }
                cCore.CoreServer = bUseServer;
                #endregion

                #region Parse Hardware Resources And Architectures
                XmlNode xResources = CerebrumXmlInterface.GetXmlNode(xHardware, "Resources");
                if (xResources != null)
                {
                    foreach (XmlNode xn in xResources.ChildNodes)
                    {
                        if (xn.NodeType != XmlNodeType.Element)
                            continue;
                        if (String.Compare(xn.Name, "Resource", true) == 0)
                        {
                            string RName = string.Empty;
                            long RAmt = 0;
                            foreach (XmlAttribute xAttr in xn.Attributes)
                            {
                                if (String.Compare(xAttr.Name, "Name", true) == 0)
                                {
                                    RName = xAttr.Value;
                                }
                                else if (String.Compare(xAttr.Name, "Amount", true) == 0)
                                {
                                    long val;
                                    if (long.TryParse(xAttr.Value, out val))
                                    {
                                        RAmt = val;
                                    }
                                    else
                                    {
                                        Trace.WriteLine(String.Format("Error parsing Resource string -- CerebrumCore.Hardware.Resources.Resource in ",
                                            DocumentPath.FullName));
                                    }
                                }
                            }
                            if ((RName != string.Empty) && (RAmt > 0))
                                cCore.Resources.Add(RName.ToLower(), RAmt);
                        }
                    }
                }

                XmlNode xArches = CerebrumXmlInterface.GetXmlNode(xHardware, "SupportedArchitectures");
                cCore.SupportedArchitectures.Clear();
                if (xArches != null)
                {
                    foreach (XmlNode xn in xArches.ChildNodes)
                    {
                        if (xn.NodeType != XmlNodeType.Element)
                            continue;
                        if (String.Compare(xn.Name, "Arch", true) == 0)
                        {
                            cCore.SupportedArchitectures.Add(xn.InnerText);
                        }
                    }
                }
                //cCore.CoreHWLocation = CerebrumXmlInterface.GetXmlInnerText(xHardware, "Sources.PCore");
                #endregion

                #region Parse Hardware SubComponent Cores
                XmlNode xComponentCores = CerebrumXmlInterface.GetXmlNode(xDoc, "CerebrumCore.Hardware.PCores");
                if (xComponentCores != null)
                {
                    foreach (XmlNode xn in xComponentCores.ChildNodes)
                    {
                        if (xn.NodeType != XmlNodeType.Element)
                            continue;
                        if (String.Compare(xn.Name, "PCore", true) == 0)
                        {
                            string xPCType = CerebrumXmlInterface.GetXmlAttribute(xn, "", "Type");
                            string xPCInst = CerebrumXmlInterface.GetXmlAttribute(xn, "", "Instance");
                            string xPCValid = CerebrumXmlInterface.GetXmlAttribute(xn, "", "Valid");
                            string xPCSource = CerebrumXmlInterface.GetXmlAttribute(xn, "", "Source");
                            string xPCUCF = CerebrumXmlInterface.GetXmlAttribute(xn, "", "UCF");
                            string xPCVersion = CerebrumXmlInterface.GetXmlAttribute(xn, "", "Version");

                            cCore.AddSubComponent(xPCInst, xPCType, xPCValid);
                            ComponentCore CompCore = cCore.GetComponentCore(xPCInst);
                            CompCore.CoreVersion = xPCVersion;
                            CompCore.CoreUCF = xPCUCF;
                            CompCore.CoreSource = xPCSource;
                            CompCore.OwnerComponent = cCore;
                        }
                    }
                }
                #endregion

                #region Parse Software-Hardware Properties/Parameters (for Properties Dialog/Core Properties)
                XmlNode xProperties = CerebrumXmlInterface.GetXmlNode(xDoc, "CerebrumCore.Software.Properties");
                if (xProperties != null)
                {
                    foreach (XmlNode xPropNode in xProperties.ChildNodes)
                    {
                        if (xPropNode.NodeType != XmlNodeType.Element)
                            continue;
                        string PropCategory = CerebrumXmlInterface.GetXmlAttribute(xPropNode, string.Empty, "Category");
                        string PropName = CerebrumXmlInterface.GetXmlAttribute(xPropNode, string.Empty, "Name");
                        string PropDescription = CerebrumXmlInterface.GetXmlAttribute(xPropNode, string.Empty, "Description");
                        string PropDefault = CerebrumXmlInterface.GetXmlAttribute(xPropNode, string.Empty, "Default");
                        string PropCurrent = PropDefault;
                        string PropValueType = CerebrumXmlInterface.GetXmlAttribute(xPropNode, string.Empty, "Type");
                        string PropParameter = CerebrumXmlInterface.GetXmlAttribute(xPropNode, string.Empty, "Parameter");
                        string PropRange = CerebrumXmlInterface.GetXmlAttribute(xPropNode, string.Empty, "Range");
                        string PropValues = CerebrumXmlInterface.GetXmlAttribute(xPropNode, string.Empty, "Values");
                        string PropCore = CerebrumXmlInterface.GetXmlAttribute(xPropNode, string.Empty, "Core");

                        CerebrumPropertyEntry newPropEntry = new CerebrumPropertyEntry();
                        newPropEntry.AssociatedCore = PropCore;
                        newPropEntry.DialogVisible = true;
                        newPropEntry.PropertyCategory = PropCategory;
                        newPropEntry.PropertyDefault = PropDefault;
                        newPropEntry.PropertyDescription = PropDescription;
                        newPropEntry.PropertyName = PropName;
                        newPropEntry.PropertyParameter = PropParameter;
                        newPropEntry.PropertyValueType = PropValueType;
                        if (PropRange != string.Empty)
                            newPropEntry.ParseRange(PropRange);
                        if (PropValues != string.Empty)
                            newPropEntry.ParseValues(PropValues);
                        newPropEntry.SetValue(PropCurrent);

                        if ((newPropEntry.PropertyParameter == string.Empty) || (newPropEntry.AssociatedCore == string.Empty))
                        {
                            newPropEntry.PropertyType = CerebrumPropertyTypes.CEREBRUMPROPERTY;
                        }
                        if (PropCore == string.Empty)
                        {
                            cCore.Properties.SetValue(newPropEntry, true);
                        }
                        else
                        {
                            cCore.GetComponentCore(PropCore).Properties.SetValue(newPropEntry, true);
                        }
                    }
                }
                #endregion

                #region Parse Design Tool Properties
                string ImgPath = CerebrumXmlInterface.GetXmlAttribute(xDesign, "Image", "Path");
                cCore.RelativeImagePath = ImgPath;
                cCore.CoreImagePath = DocumentPath.Directory.FullName + "\\" + ImgPath;
                try
                {
                    if (!File.Exists(cCore.CoreImagePath))
                    {
                        cCore.CoreBitmap = null;
                    }
                    else
                    {
                        cCore.CoreBitmap = new Bitmap(cCore.CoreImagePath);
                    }
                }
                catch (Exception ex)
                {
                    ErrorReporting.DebugException(ex);
                    Trace.WriteLine(String.Format("Error loading image -- CerebrumCore.Software.DesignDisplay.Image:Path in {0}",
                        DocumentPath.FullName));
                    if (Lib != null)
                    {
                        Lib.OnCoreError(cCore, String.Format("Error loading image -- CerebrumCore.Software.DesignDisplay.Image:Path in {0}",
                            DocumentPath.FullName));
                    }
                }
                string szWidth = CerebrumXmlInterface.GetXmlAttribute(xDesign, "DefaultSize", "Width");
                string szHeight = CerebrumXmlInterface.GetXmlAttribute(xDesign, "DefaultSize", "Height");
                int szW = 150;
                int szH = 150;
                int.TryParse(szWidth, out szW);
                int.TryParse(szHeight, out szH);

                cCore.MaxSize = new Size(szW, szH);
                cCore.MinSize = new Size(szW, szH);
                cCore.Width = szW;
                cCore.Height = szH;
                #endregion

                #region Parse Interfaces and Interface Cores
                try
                {
                    if (xHardware != null)
                    {
                        foreach (XmlNode xInterface in xHardware.ChildNodes)
                        {
                            if (String.Compare(xInterface.Name, "Interface", true) == 0)
                            {
                                string CoreInst = string.Empty;
                                VortexAttachmentType CoreType = VortexAttachmentType.SAP;
                                bool CorePE = false;
                                foreach (XmlAttribute xAttr in xInterface.Attributes)
                                {
                                    if (String.Compare(xAttr.Name, "Type", true) == 0)
                                    {
                                        if (String.Compare(xAttr.Value, "SAP", true) == 0)
                                        {
                                            CoreType = VortexAttachmentType.SAP;
                                        }
                                        else if (String.Compare(xAttr.Value, "SOP", true) == 0)
                                        {
                                            CoreType = VortexAttachmentType.SOP;
                                        }
                                        else if (String.Compare(xAttr.Value, "Bridge", true) == 0)
                                        {
                                            CoreType = VortexAttachmentType.VortexBridge;
                                        }
                                        else if (String.Compare(xAttr.Value, "Edge", true) == 0)
                                        {
                                            CoreType = VortexAttachmentType.VortexEdge;
                                        }
                                        else
                                        {
                                            Trace.WriteLine(String.Format("Error parsing hardware interface core, Unknown Interface Value [{1}={2}] -- CerebrumCore.Hardware.Interface in {0}",
                                                DocumentPath.FullName, xAttr.Name, xAttr.Value));
                                            if (Lib != null)
                                            {
                                                Lib.OnCoreError(cCore, String.Format("Error parsing hardware interface core, Unknown Interface Value [{1}={2}] -- CerebrumCore.Hardware.Interface in {0}",
                                                    DocumentPath.FullName, xAttr.Name, xAttr.Value));
                                            }
                                            throw new Exception(String.Format("Error parsing hardware interface core, Unknown Interface Value [{1}={2}] -- CerebrumCore.Hardware.Interface in {0}",
                                                DocumentPath.FullName, xAttr.Name, xAttr.Value));
                                        }
                                    }
                                    else if ((String.Compare(xAttr.Name, "PE", true) == 0) ||
                                             (String.Compare(xAttr.Name, "ProcElement", true) == 0) ||
                                             (String.Compare(xAttr.Name, "ProcessingElement", true) == 0))
                                    {
                                        bool val = false;
                                        if (bool.TryParse(xAttr.Value, out val))
                                        {
                                            CorePE = val;
                                        }
                                        else
                                        {
                                            Trace.WriteLine(String.Format("Error parsing hardware interface core, Unknown Attribute Value [{1}={2}] -- CerebrumCore.Hardware.Interface in {0}",
                                                DocumentPath.FullName, xAttr.Name, xAttr.Value));
                                            if (Lib != null)
                                            {
                                                Lib.OnCoreError(cCore, String.Format("Error parsing hardware interface core, Unknown Attribute Value [{1}={2}] -- CerebrumCore.Hardware.Interface in {0}",
                                                    DocumentPath.FullName, xAttr.Name, xAttr.Value));
                                            }
                                        }
                                    }
                                }
                                CoreInst = xInterface.InnerText;
                                cCore.AddVortexAttachmentInstance(CoreInst, CoreType);

                                if (cCore.ComponentCores.ContainsKey(CoreInst))
                                {
                                    ComponentCore C = cCore.ComponentCores[CoreInst];
                                    C.IsPE = CorePE;
                                    C.InterfaceType = CoreType;
                                }
                                else
                                {
                                    Trace.WriteLine(String.Format("Error parsing hardware interface core, Unable to identify PCore [{1}] -- CerebrumCore.Hardware.Interface in {0}",
                                        DocumentPath.FullName, CoreInst));
                                    if (Lib != null)
                                    {
                                        Lib.OnCoreError(cCore, String.Format("Error parsing hardware interface core, Unable to identify PCore [{1}] -- CerebrumCore.Hardware.Interface in {0}",
                                            DocumentPath.FullName, CoreInst));
                                    }
                                    throw new Exception(String.Format("Error parsing hardware interface core, Unable to identify PCore [{1}] -- CerebrumCore.Hardware.Interface in {0}",
                                        DocumentPath.FullName, CoreInst));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorReporting.DebugException(ex);
                    Trace.WriteLine(String.Format("Error parsing hardware interface cores -- CerebrumCore.Hardware.Interface in {0}",
                        DocumentPath.FullName));
                    if (Lib != null)
                    {
                        Lib.OnCoreError(cCore, String.Format("Error parsing hardware interface cores -- CerebrumCore.Hardware.Interface in {0}",
                            DocumentPath.FullName));
                    }
                }
                #endregion

                #region Parse MHSImports
                XmlNode xImports = CerebrumSharedClasses.CerebrumXmlInterface.GetXmlNode(xDoc, "CerebrumCore.Hardware.MHSImports");
                if (xImports != null)
                {
                    foreach (XmlNode xImportNode in xImports.ChildNodes)
                    {
                        if (String.Compare(xImportNode.Name, "MHSImport", true) == 0)
                        {
                            foreach (XmlAttribute xImportSource in xImportNode.Attributes)
                            {
                                if (String.Compare(xImportSource.Name, "Source", true) == 0)
                                {
                                    string xmlImport = String.Format("{0}\\{1}", DocumentPath.Directory.FullName, xImportSource.Value);
                                    if (!cCore.MHSImports.Contains(xmlImport))
                                        cCore.MHSImports.Add(xmlImport);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Parse Core I/O Ports
                try
                {
                    string DefaultInstance = CerebrumXmlInterface.GetXmlInnerText(xDoc, "CerebrumCore.Hardware.Interface");     // Default to first instance defined in the file

                    XmlNode Ports = CerebrumXmlInterface.GetXmlNode(xDesign, "Ports");
                    if (Ports != null)
                    {
                        foreach (XmlNode xPort in Ports.ChildNodes)
                        {
                            if (xPort.NodeType != XmlNodeType.Element)
                                continue;
                            string xPortType = CerebrumXmlInterface.GetXmlAttribute(xPort, string.Empty, "Type");
                            string xPortName = CerebrumXmlInterface.GetXmlAttribute(xPort, string.Empty, "Name");
                            string xPortIF = CerebrumXmlInterface.GetXmlAttribute(xPort, string.Empty, "Interface");
                            string xPortInstance = CerebrumXmlInterface.GetXmlAttribute(xPort, string.Empty, "Instance");
                            if ((xPortInstance == null) || (xPortInstance == string.Empty))
                                xPortInstance = DefaultInstance;
                            string XPos = CerebrumXmlInterface.GetXmlAttribute(xPort, string.Empty, "X");
                            string YPos = CerebrumXmlInterface.GetXmlAttribute(xPort, string.Empty, "Y");
                            string Scale = CerebrumXmlInterface.GetXmlAttribute(xPort, string.Empty, "Scale");
                            if ((Scale == string.Empty) || (Scale == null))
                                Scale = "1.0";
                            float x = 0.0F;
                            float y = 0.0F;
                            float s = 1.0F;
                            if (float.TryParse(XPos, out x) && float.TryParse(YPos, out y) && float.TryParse(Scale, out s))
                            {
                                x = (Math.Abs(x) / 100);
                                y = (Math.Abs(y) / 100);
                                cCore.AddPort(xPortName, xPortType, xPortIF, xPortInstance, x, y, s);
                            }
                            else
                            {
                                Trace.WriteLine(String.Format("Error parsing design port specification -- CerebrumCore.Software.DesignDisplay.Ports.Port in {0}",
                                    DocumentPath.FullName));
                                if (Lib != null)
                                {
                                    Lib.OnCoreError(cCore, String.Format("Error parsing design port specification -- CerebrumCore.Software.DesignDisplay.Ports.Port in {0}",
                                        DocumentPath.FullName));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorReporting.DebugException(ex);
                    Trace.WriteLine(String.Format("Error parsing design port specification -- CerebrumCore.Software.DesignDisplay.Ports.Port in {0}",
                        DocumentPath.FullName));
                    if (Lib != null)
                    {
                        Lib.OnCoreError(cCore, String.Format("Error parsing design port specification -- CerebrumCore.Software.DesignDisplay.Ports.Port in {0}",
                            DocumentPath.FullName));
                    }
                }
                #endregion

                #region Parse Core Clocks
                try
                {
                    XmlNode xClocks = CerebrumXmlInterface.GetXmlNode(xHardware, "Clocks");
                    if (xClocks != null)
                    {
                        foreach (XmlNode xClock in xClocks.ChildNodes)
                        {
                            if (xClock.NodeType != XmlNodeType.Element)
                                continue;
                            if (String.Compare(xClock.Name, "Clock", true) == 0)
                            {
                                string xCLKMatch = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Match");
                                if ((xCLKMatch == null) || (xCLKMatch == string.Empty))
                                {
                                    string xCLKDesc = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Name");
                                    string xCLKDir = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Direction");
                                    string xCLKCore = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Core");
                                    string xCLKPort = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Port");
                                    string xCLKFreq = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Frequency");
                                    string xCLKPhase = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Phase");
                                    string xCLKGroup = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Group");
                                    string xCLKBuff = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Buffered");
                                    string xCLKLock = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "LockedOn");

                                    bool IsOutputClock = false;

                                    xCLKDir = xCLKDir.ToUpper();
                                    if (xCLKDir.StartsWith("O"))
                                        IsOutputClock = true;
                                    else
                                        IsOutputClock = false;

                                    // Set defaults for missing values
                                    if ((xCLKPhase == null) || (xCLKPhase == string.Empty))
                                        xCLKPhase = "0";
                                    if ((xCLKGroup == null) || (xCLKGroup == string.Empty))
                                        xCLKGroup = "NONE";
                                    if ((xCLKBuff == null) || (xCLKBuff == string.Empty))
                                        xCLKBuff = "true";

                                    string frequency = xCLKFreq;

                                    // Parse phase
                                    int phase = 0;
                                    if (!int.TryParse(xCLKPhase, out phase))
                                    {
                                        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                                            DocumentPath.FullName));
                                        if (Lib != null)
                                        {
                                            Lib.OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                                                            DocumentPath.FullName));
                                        }
                                    }

                                    // Parse Clock Group
                                    ClockGroup group = ClockGroup.NONE;
                                    try
                                    {
                                        group = (ClockGroup)Enum.Parse(typeof(ClockGroup), xCLKGroup);
                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorReporting.DebugException(ex);
                                        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                                            DocumentPath.FullName));
                                        if (Lib != null)
                                        {
                                            Lib.OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                                                            DocumentPath.FullName));
                                        }
                                    }

                                    // Parse buffer state
                                    bool buffered = true;
                                    if (!bool.TryParse(xCLKBuff, out buffered))
                                    {
                                        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                                            DocumentPath.FullName));
                                        if (Lib != null)
                                        {
                                            Lib.OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                                                            DocumentPath.FullName));
                                        }
                                    }

                                    ClockSignal CS = new ClockSignal();
                                    CS.ComponentInstance = cCore.CoreInstance;
                                    CS.Frequency = frequency;
                                    CS.Phase = phase;
                                    CS.Group = group;
                                    CS.Buffered = buffered;
                                    CS.SignalDirection = (IsOutputClock ? ClockDirection.OUTPUT : ClockDirection.INPUT);
                                    CS.Name = xCLKDesc;
                                    CS.CoreInstance = xCLKCore;
                                    CS.Port = xCLKPort;
                                    CS.LockedPort = xCLKLock;

                                    if (CS.SignalDirection == ClockDirection.OUTPUT)
                                        cCore.OutputClocks.Add(CS);
                                    else
                                        cCore.InputClocks.Add(CS);
                                }
                            }
                        }
                        // Process dependent clocks last
                        foreach (XmlNode xClock in xClocks.ChildNodes)
                        {
                            if (xClock.NodeType != XmlNodeType.Element)
                                continue;
                            if (String.Compare(xClock.Name, "Clock", true) == 0)
                            {
                                string xCLKMatch = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Match");
                                if ((xCLKMatch != null) && (xCLKMatch != string.Empty))
                                {
                                    string xCLKDesc = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Name");
                                    string xCLKDir = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Direction");
                                    string xCLKCore = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Core");
                                    string xCLKPort = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Port");
                                    string xCLKPhase = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Phase");
                                    string xCLKRatio = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "Ratio");
                                    string xCLKLock = CerebrumXmlInterface.GetXmlAttribute(xClock, "", "LockedOn");

                                    bool IsOutputClock = false;

                                    xCLKDir = xCLKDir.ToLower();
                                    if (xCLKDir.StartsWith("o"))
                                        IsOutputClock = true;
                                    else
                                        IsOutputClock = false;

                                    // Set defaults for missing values
                                    if ((xCLKPhase == null) || (xCLKPhase == string.Empty))
                                        xCLKPhase = "0";
                                    if ((xCLKRatio == null) || (xCLKRatio == string.Empty))
                                        xCLKRatio = "1.0";

                                    double ratio = 0;
                                    if (!double.TryParse(xCLKRatio, out ratio))
                                    {
                                        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                                            DocumentPath.FullName));
                                        if (Lib != null)
                                        {
                                            Lib.OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                                                            DocumentPath.FullName));
                                        }
                                    }
                                    // Parse phase
                                    int phase = 0;
                                    if (!int.TryParse(xCLKPhase, out phase))
                                    {
                                        Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                                            DocumentPath.FullName));
                                        if (Lib != null)
                                        {
                                            Lib.OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                                                            DocumentPath.FullName));
                                        }
                                    }

                                    ClockSignal CS = null;

                                    if (IsOutputClock)
                                    {
                                        foreach (ClockSignal ClkSig in cCore.OutputClocks)
                                        {
                                            if ((String.Compare(ClkSig.CoreInstance, xCLKCore, true) == 0) &&
                                                (String.Compare(ClkSig.Port, xCLKPort, true) == 0))
                                            {
                                                CS = new ClockSignal(ClkSig);
                                                CS.Frequency = CS.Frequency;
                                                CS.FrequencyRatio = ratio;
                                                CS.Phase = (CS.Phase + phase);
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (ClockSignal ClkSig in cCore.InputClocks)
                                        {
                                            if ((String.Compare(ClkSig.CoreInstance, xCLKCore, true) == 0) &&
                                                (String.Compare(ClkSig.Port, xCLKMatch, true) == 0))
                                            {
                                                CS = new ClockSignal(ClkSig);
                                                CS.Frequency = CS.Frequency;
                                                CS.FrequencyRatio = ratio;
                                                CS.Phase = (CS.Phase + phase);
                                                break;
                                            }
                                        }
                                    }
                                    if (CS != null)
                                    {
                                        CS.ComponentInstance = cCore.CoreInstance;
                                        CS.SignalDirection = (IsOutputClock ? ClockDirection.OUTPUT : ClockDirection.INPUT);
                                        CS.Name = xCLKDesc;
                                        CS.CoreInstance = xCLKCore;
                                        CS.LockedPort = xCLKLock;
                                        if (CS.SignalDirection == ClockDirection.OUTPUT)
                                            cCore.OutputClocks.Add(CS);
                                        else
                                            cCore.InputClocks.Add(CS);
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorReporting.DebugException(ex);
                    Trace.WriteLine(String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                        DocumentPath.FullName));
                    if (Lib != null)
                    {
                        Lib.OnCoreError(cCore, String.Format("Error parsing core clocks specification -- CerebrumCore.Hardware.Clocks in {0}",
                        DocumentPath.FullName));
                    }
                }
                #endregion

                #region Parse Core Resets
                try
                {

                }
                catch (Exception ex)
                {
                    ErrorReporting.DebugException(ex);
                    Trace.WriteLine(String.Format("Error parsing core resets specification -- CerebrumCore.Hardware.Resets in {0}",
                        DocumentPath.FullName));
                    if (Lib != null)
                    {
                        Lib.OnCoreError(cCore, String.Format("Error parsing core resets specification -- CerebrumCore.Hardware.Resets in {0}",
                        DocumentPath.FullName));
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                CerebrumSharedClasses.ErrorReporting.TraceException(ex);
                if (Lib != null)
                {
                    Lib.OnCoreError(cCore, String.Format("Unspecified error loading core specification: {0}", DocumentPath.FullName));
                }
                cCore = null;
            }
            return cCore;
        }

        /// <summary>
        /// Saves the specified CerebrumCore definition in the location indicated by the document path.
        /// </summary>
        /// <param name="CorePath">The DirectoryInfo representing the location where the core definition is to be saved.</param>
        /// <param name="CC">The CerebrumCore whose definition is to be saved.</param>
        public static void SaveCoreDefinition(DirectoryInfo CorePath, CerebrumCore CC)
        {
            try
            {
                #region Create XML Document
                XmlDocument XDoc = new XmlDocument();
                XDoc.AppendChild(XDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement xCoreRoot = XDoc.CreateElement("CerebrumCore");
                #endregion

                #region General Properties Section
                XmlElement xGeneral = XDoc.CreateElement("General");
                xGeneral.AppendChild(CreateNodeWithInnerText(XDoc, "Visible", CC.VisibleInLibrary));
                xGeneral.AppendChild(CreateNodeWithInnerText(XDoc, "Name", CC.CoreTrueName));
                xGeneral.AppendChild(CreateNodeWithInnerText(XDoc, "Type", CC.CoreType));
                xGeneral.AppendChild(CreateNodeWithInnerText(XDoc, "Version", CC.CoreVersion));
                xGeneral.AppendChild(CreateNodeWithInnerText(XDoc, "Description", CC.CoreDescription));
                xGeneral.AppendChild(CreateNodeWithInnerText(XDoc, "Owner", CC.CoreOwner));
                xGeneral.AppendChild(CreateNodeWithInnerText(XDoc, "CoreServer", false));
                xGeneral.AppendChild(CreateNodeWithInnerText(XDoc, "InstancePrefix", CC.CoreInstancePrefix));
                StringBuilder Keywords = new StringBuilder();
                for (int k = 0; k < CC.CoreKeywords.Count; k++)
                    Keywords.AppendFormat("{0};", CC.CoreKeywords[k]);
                xGeneral.AppendChild(CreateNodeWithInnerText(XDoc, "Keywords", Keywords.ToString().Trim(';')));
                xCoreRoot.AppendChild(xGeneral);
                #endregion

                #region Software/Tool UI Section
                XmlElement xSoftware = XDoc.CreateElement("Software");

                #region Design & Display
                XmlElement xDesignDisplay = XDoc.CreateElement("DesignDisplay");
                XmlElement xCategory = XDoc.CreateElement("Category");
                xCategory.SetAttribute("Name", CC.CoreLibraryCategory);
                xDesignDisplay.AppendChild(xCategory);

                XmlElement xImage = XDoc.CreateElement("Image");
                xImage.SetAttribute("Path", CC.CoreImagePath.Replace(String.Format("{0}\\", CorePath.FullName), string.Empty));
                xDesignDisplay.AppendChild(xImage);

                XmlElement xDefaultSize = XDoc.CreateElement("DefaultSize");
                xDefaultSize.SetAttribute("Width", CC.MinSize.Width.ToString());
                xDefaultSize.SetAttribute("Height", CC.MinSize.Height.ToString());
                xDesignDisplay.AppendChild(xDefaultSize);

                #region Ports
                XmlElement xPorts = XDoc.CreateElement("Ports");
                foreach (CoreConnector CorePort in CC.CorePorts)
                {
                    XmlElement xPort = XDoc.CreateElement("Port");
                    string PortTypeString = "Invalid";
                    switch (CorePort.PortType)
                    {
                        case PortType.IOInterface:
                            PortTypeString = "IOINTERFACE";
                            break;
                        case PortType.SAPInitiator:
                            PortTypeString = "INITIATOR";
                            break;
                        case PortType.SAPTarget:
                            PortTypeString = "TARGET";
                            break;
                        case PortType.SOPInput:
                            PortTypeString = "INPUT";
                            break;
                        case PortType.SOPOutput:
                            PortTypeString = "OUTPUT";
                            break;
                        default:
                            PortTypeString = "Invalid";
                            break;
                    }
                    xPort.SetAttribute("Type", PortTypeString);
                    xPort.SetAttribute("Name", CorePort.PortName);
                    xPort.SetAttribute("X", ((int)Math.Round(CorePort.XOffset * 100, 0)).ToString());
                    xPort.SetAttribute("Y", ((int)Math.Round(CorePort.YOffset * 100, 0)).ToString());
                    xPort.SetAttribute("Interface", CorePort.PortInterface);
                    xPort.SetAttribute("Instance", CorePort.CoreInstance);
                    xPorts.AppendChild(xPort);
                }
                xDesignDisplay.AppendChild(xPorts);
                #endregion

                xSoftware.AppendChild(xDesignDisplay);
                #endregion

                #region Properties
                XmlElement xProperties = XDoc.CreateElement("Properties");
                foreach (CerebrumPropertyEntry CPE in CC.CoreProperties)
                {
                    XmlElement xProperty = XDoc.CreateElement("Property");
                    xProperty.SetAttribute("Name", CPE.PropertyName);
                    xProperty.SetAttribute("Default", CPE.PropertyDefault);
                    xProperty.SetAttribute("Category", CPE.PropertyCategory);
                    xProperty.SetAttribute("Description", CPE.PropertyDescription);
                    xProperty.SetAttribute("Type", CPE.PropertyType.ToString());
                    if (CPE.PropertyValues.Count > 0)
                    {
                        StringBuilder Values = new StringBuilder();
                        foreach (KeyValuePair<string, string> ValuePair in CPE.PropertyValues)
                        {
                            Values.AppendFormat("{0}={1}, ", ValuePair.Key, ValuePair.Value);
                        }
                        xProperty.SetAttribute("Values", Values.ToString().Trim().Trim(','));
                    }
                    if ((CPE.AssociatedCore != null) && (CPE.AssociatedCore != string.Empty))
                    {
                        xProperty.SetAttribute("Core", CPE.AssociatedCore);
                    }
                    xProperties.AppendChild(xProperty);
                }
                xSoftware.AppendChild(xProperties);
                #endregion

                xCoreRoot.AppendChild(xSoftware);
                #endregion

                #region Hardware Section
                XmlElement xHardware = XDoc.CreateElement("Hardware");

                #region Interfaces
                foreach (VortexInterfaces.IVortexAttachment IVA in CC.VortexDevices)
                {
                    XmlElement xInterface = XDoc.CreateElement("Interface");
                    string InterfaceType = string.Empty;
                    if (IVA is VortexInterfaces.IVortexSAP)
                        InterfaceType = "SAP";
                    else if (IVA is VortexInterfaces.IVortexSOP)
                        InterfaceType = "SOP";
                    else if (IVA is VortexInterfaces.IVortexEdgeAttachment)
                        InterfaceType = "Edge";
                    else if (IVA is VortexInterfaces.IVortexBridgeAttachment)
                        InterfaceType = "Bridge";
                    xInterface.SetAttribute("Type", InterfaceType);
                    if (CC.ComponentCores.ContainsKey(IVA.CoreInstance))
                    {
                        if (CC.ProcessingElementCores.Contains(CC.ComponentCores[IVA.CoreInstance]))
                            xInterface.SetAttribute("PE", "True");
                    }
                    xInterface.InnerText = IVA.CoreInstance;
                    xHardware.AppendChild(xInterface);
                }
                #endregion

                #region PCores
                XmlElement xPCores = XDoc.CreateElement("PCores");
                foreach (ComponentCore CompCore in CC.ComponentCores.Values)
                {
                    XmlElement xPCore = XDoc.CreateElement("PCore");
                    xPCore.SetAttribute("Type", CompCore.CoreType);
                    xPCore.SetAttribute("Version", CompCore.CoreVersion);
                    xPCore.SetAttribute("Instance", CompCore.NativeInstance);
                    if ((CompCore.CoreSource != null) && (CompCore.CoreSource != string.Empty))
                    {
                        xPCore.SetAttribute("Source", CompCore.CoreSource.Replace(String.Format("{0}\\", CorePath.FullName), string.Empty));
                    }
                    if ((CompCore.CoreUCF != null) && (CompCore.CoreUCF != string.Empty))
                    {
                        xPCore.SetAttribute("UCF", CompCore.CoreUCF.Replace(String.Format("{0}\\", CorePath.FullName), string.Empty));
                    }
                    if ((CompCore.ValidCondition != null) && (CompCore.ValidCondition != string.Empty))
                    {
                        xPCore.SetAttribute("Valid", CompCore.ValidCondition);
                    }
                    xPCores.AppendChild(xPCore);
                }
                xHardware.AppendChild(xPCores);
                #endregion

                #region Supported Architectures
                XmlElement xSupportedArchitectures = XDoc.CreateElement("SupportedArchitectures");
                foreach (string SupArch in CC.SupportedArchitectures)
                {
                    xSupportedArchitectures.AppendChild(CreateNodeWithInnerText(XDoc, "Arch", SupArch));
                }
                xHardware.AppendChild(xSupportedArchitectures);
                #endregion

                #region Resources
                XmlElement xResources = XDoc.CreateElement("Resources");
                foreach (KeyValuePair<string, long> CoreRes in CC.Resources)
                {
                    XmlElement xRes = XDoc.CreateElement("Resource");
                    xRes.SetAttribute("Name", CoreRes.Key);
                    xRes.SetAttribute("Amount", CoreRes.Value.ToString());
                    xResources.AppendChild(xRes);
                }
                xHardware.AppendChild(xResources);
                #endregion

                #region MHS Imports
                XmlElement xMHSImports = XDoc.CreateElement("MHSImports");
                foreach (string MHS in CC.MHSImports)
                {
                    XmlElement xMHS = XDoc.CreateElement("MHSImport");
                    xMHS.SetAttribute("Source", MHS.Replace(String.Format("{0}\\", CorePath.FullName), string.Empty));
                    xMHSImports.AppendChild(xMHS);
                }
                xHardware.AppendChild(xMHSImports);
                #endregion

                #region Clocks
                XmlElement xClocks = XDoc.CreateElement("Clocks");
                foreach (ClockSignal CS in CC.OutputClocks)
                {
                    XmlElement xClock = XDoc.CreateElement("Clock");
                    if ((CS.Name != null) && (CS.Name != string.Empty))
                    {
                        xClock.SetAttribute("Name", CS.Name);
                    }
                    xClock.SetAttribute("Direction", (CS.SignalDirection == ClockDirection.INPUT ? "IN" : "OUT"));
                    xClock.SetAttribute("Core", CS.CoreInstance);
                    xClock.SetAttribute("Port", CS.Port);
                    xClock.SetAttribute("Frequency", CS.Frequency.ToString());
                    xClock.SetAttribute("Phase", CS.Phase.ToString());
                    xClock.SetAttribute("Group", CS.Group.ToString());
                    xClock.SetAttribute("Buffered", CS.Buffered.ToString());
                    xClocks.AppendChild(xClock);
                }
                foreach (ClockSignal CS in CC.InputClocks)
                {
                    XmlElement xClock = XDoc.CreateElement("Clock");
                    if ((CS.Name != null) && (CS.Name != string.Empty))
                    {
                        xClock.SetAttribute("Name", CS.Name);
                    }
                    //xClock.SetAttribute("Direction", (CS.SignalDirection == ClockDirection.INPUT ? "IN" : "OUT"));
                    xClock.SetAttribute("Core", CS.CoreInstance);
                    xClock.SetAttribute("Port", CS.Port);
                    xClock.SetAttribute("Frequency", CS.Frequency.ToString());
                    xClock.SetAttribute("Phase", CS.Phase.ToString());
                    xClock.SetAttribute("Group", CS.Group.ToString());
                    xClock.SetAttribute("Buffered", CS.Buffered.ToString());
                    xClocks.AppendChild(xClock);
                }
                xHardware.AppendChild(xClocks);
                #endregion

                #region Resets
                XmlElement xResets = XDoc.CreateElement("Resets");
                xHardware.AppendChild(xResets);
                #endregion

                xCoreRoot.AppendChild(xHardware);
                #endregion

                #region Save Core Definition
                XDoc.AppendChild(xCoreRoot);
                FileInfo TargetFile = new FileInfo(String.Format("{0}\\{1}.xml", CorePath.FullName, CorePath.Name));
                if (!CorePath.Exists)
                {
                    try { CorePath.Create(); }
                    catch { }
                }
                if (CorePath.Exists)
                {
                    if (TargetFile.Exists)
                    {
                        try { TargetFile.Delete(); }
                        catch { }
                    }
                    XDoc.Save(TargetFile.FullName);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
        }

        /// <summary>
        /// Creates a new XML Node Element with the specified Name and InnerText based on the schema of the target document.
        /// </summary>
        /// <param name="Owner">The target document on which the new node is to be based.</param>
        /// <param name="NodeName">The name of the new Node element.</param>
        /// <param name="InnerText">The InnerText of the new Node element.</param>
        /// <returns>The new XmlElement object created with the specified parameters.</returns>
        private static XmlElement CreateNodeWithInnerText(XmlDocument Owner, string NodeName, object InnerText)
        {
            XmlElement xNewNode = Owner.CreateElement(NodeName);
            xNewNode.InnerText = InnerText.ToString();
            return xNewNode;
        }

        /// <summary>
        /// Reads a list of Required CerebrumCores for an FPGA from the FPGA Node of the Board XML file.
        /// </summary>
        /// <param name="PathMan">The project path manager used to locate the Platform and CerebrumCore repositories.</param>
        /// <param name="FPGANode">The XmlNode object from the Board XML file defining the FPGA on which components are required.</param>
        /// <param name="MappingID">The mapping ID assigned to the FPGA on which the components are required.</param>
        /// <returns>A List of CerebrumCore objects, parsed from the XML FPGA Node.</returns>
        public static List<CerebrumCore> ReadRequiredComponentsFromXML(PathManager PathMan, XmlNode FPGANode, string MappingID)
        {
            List<CerebrumCore> RequiredCerebrumCores = new List<CerebrumCore>();
            if (!PathMan.HasPath("ProjectPlatform"))
                return new List<CerebrumCore>();

            foreach (XmlNode RequiredComponentNode in FPGANode.ChildNodes)
            {
                if (String.Compare(RequiredComponentNode.Name, "RequiredCore", true) == 0)
                {
                    CerebrumCore newComponent = ReadRequiredComponentFromXML(PathMan, MappingID, RequiredComponentNode);
                    if (newComponent != null)
                    {
                        RequiredCerebrumCores.Add(newComponent);
                    }
                }
            }
            return RequiredCerebrumCores;
        }

        /// <summary>
        /// Reads a Required CerebrumCore for an FPGA from the specified XML Node.
        /// </summary>
        /// <param name="PathMan">The project path manager used to locate the Platform and CerebrumCore repositories.</param>
        /// <param name="MappingID">The mapping ID assigned to the FPGA on which the components are required.</param>
        /// <param name="RequiredComponentNode">The XmlNode object from which the component information is to be parsed.</param>
        /// <returns>A CerebrumCore object parsed from the XML Node.</returns>
        public static CerebrumCore ReadRequiredComponentFromXML(PathManager PathMan, string MappingID, XmlNode RequiredComponentNode)
        {
            CerebrumCore newComponent = null;
            string SourceFolder = string.Empty;
            string SourceType = string.Empty;
            string SourceVer = string.Empty;
            string SourcePath = string.Empty;
            bool DesignVisible = false;
            string TrueInstance = string.Empty;
            foreach (XmlAttribute xAttr in RequiredComponentNode.Attributes)
            {
                if (String.Compare(xAttr.Name, "Source", true) == 0)
                {
                    SourceFolder = xAttr.Value;
                }
                else if (String.Compare(xAttr.Name, "Type", true) == 0)
                {
                    SourceType = xAttr.Value;
                }
                else if (String.Compare(xAttr.Name, "Version", true) == 0)
                {
                    SourceVer = xAttr.Value;
                }
                else if (String.Compare(xAttr.Name, "DesignVisible", true) == 0)
                {
                    bool.TryParse(xAttr.Value, out DesignVisible);
                }
            }
            // Scan core repositories from the Paths file
            List<string> SearchPaths = new List<string>();
            if (PathMan.HasPath("ProjectPlatform"))
            {
                SearchPaths.Add(String.Format("{0}\\{1}", PathMan["Platforms"], PathMan["ProjectPlatform"])); // Look in the Platform Directory
                SearchPaths.Add(PathMan["CerebrumCores"]); // Look in the CerebrumCores Directory
            }
            if (PathMan.HasPath("CoreSearchPaths"))
            {
                string[] OtherSearchPaths = PathMan["CoreSearchPaths"].Split(';');
                SearchPaths.AddRange(OtherSearchPaths);
            }
            foreach (string SearchPath in SearchPaths)
            {
                if (Directory.Exists(String.Format("{0}", SearchPath)))
                {
                    if (Directory.Exists(String.Format("{0}\\{1}", SearchPath, SourceFolder)))
                    {
                        if (File.Exists(String.Format("{0}\\{1}\\{1}.xml", SearchPath, SourceFolder)))
                        {
                            SourcePath = String.Format("{0}\\{1}\\{1}.xml", SearchPath, SourceFolder);
                            break;
                        }
                    }
                }
            }
            if (!File.Exists(SourcePath))
            {
                throw new Exception("Unable to load source for required Platform core");
            }

            foreach (XmlNode xProperty in RequiredComponentNode.ChildNodes)
            {
                if (String.Compare(xProperty.Name, "Parameter", true) == 0)
                {
                    foreach (XmlAttribute xAttr in xProperty.Attributes)
                    {
                        if (String.Compare(xAttr.Name, "Instance", true) == 0)
                        {
                            TrueInstance = String.Format("{0}_{1}", MappingID.Replace(".", "_"), xAttr.Value);
                            break;
                        }
                    }
                    if (TrueInstance != string.Empty)
                        break;
                }
            }

            if (TrueInstance != string.Empty)
            {
                newComponent = CoreLibrary.LoadCoreDefinition(new FileInfo(SourcePath), true, null, TrueInstance);
                newComponent.VisibleInDesign = DesignVisible;
                newComponent.LoadComponentPropertiesFromPlatformNode(SourcePath, RequiredComponentNode);
                newComponent.CoreInstance = TrueInstance;
            }
            return newComponent;
        }
        
        /// <summary>
        /// Parses the output of the Component Mapping Algorithm and assigns each core in the design to its target FPGA for integration into the corresponding XPS project.
        /// </summary>
        /// <param name="XPSMapFile">The path to the XPS Map file used to build the XPS projects.</param>
        /// <param name="ComponentList">(ref) The list of CerebrumCores that were read from the XPS Map.</param>
        /// <param name="CompCoresList">(ref) The list of ComponentCores that were read from the XPS Map.</param>
        /// <param name="PathMan">The project path manager used to locate the Platform and CerebrumCore repositories.</param>
        /// <param name="FPGAID">The ID of the FPGA whose XPS map is to be loaded.  If this parameter is null, ALL components are loaded regardless of FPGA.</param>
        /// <returns>Returns true if the parsing was successful, False otherwise.</returns>
        public static bool LoadXPSMap(string XPSMapFile, ref List<CerebrumCore> ComponentList, ref List<ComponentCore> CompCoresList, PathManager PathMan, string FPGAID)
        {
            if (XPSMapFile == "")
            {
                return false;
            }
            if (!System.IO.File.Exists(XPSMapFile))
            {
                return false;
            }

            #region Load Components and PCores from XPSMap
            ComponentList = new List<CerebrumCore>();
            CompCoresList = new List<ComponentCore>();
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(XPSMapFile);
                foreach (XmlNode xRoot in xDoc.ChildNodes)
                {
                    if (String.Compare(xRoot.Name, "SystemMap", true) == 0)
                    {
                        foreach (XmlNode xFPGANode in xRoot.ChildNodes)
                        {
                            if (String.Compare(xFPGANode.Name, "FPGA", true) == 0)
                            {
                                string FPGA_ID = string.Empty;
                                foreach (XmlAttribute xFPGAAttr in xFPGANode.Attributes)
                                {
                                    if (String.Compare(xFPGAAttr.Name, "ID", true) == 0)
                                    {
                                        FPGA_ID = xFPGAAttr.Value;
                                        break;
                                    }
                                }
                                if ((FPGAID == null) ||
                                    (String.Compare(FPGA_ID, FPGAID, true) == 0))
                                {
                                    foreach (XmlNode xComponentNode in xFPGANode.ChildNodes)
                                    {
                                        if (String.Compare(xComponentNode.Name, "Component", true) == 0)
                                        {
                                            string Component_ID = string.Empty;
                                            string Component_Type = string.Empty;
                                            string Component_Source = string.Empty;
                                            foreach (XmlAttribute xComponentAttr in xComponentNode.Attributes)
                                            {
                                                if (String.Compare(xComponentAttr.Name, "ID", true) == 0)
                                                {
                                                    Component_ID = xComponentAttr.Value;
                                                }
                                                else if (String.Compare(xComponentAttr.Name, "Location", true) == 0)
                                                {
                                                    Component_Source = xComponentAttr.Value;
                                                }
                                                else if (String.Compare(xComponentAttr.Name, "Name", true) == 0)
                                                {
                                                    Component_Type = xComponentAttr.Value;
                                                }
                                            }
                                            if (Component_ID != string.Empty)
                                            {
                                                FileInfo ComponentDef = CoreLibrary.LocateCoreDefinition(PathMan, Component_Source);
                                                if ((ComponentDef != null) && (ComponentDef.Exists))
                                                {
                                                    CerebrumCore CC = CoreLibrary.LoadCoreDefinition(ComponentDef, true, null, Component_ID);
                                                    ComponentList.Add(CC);
                                                    CC.MappedFPGA = FPGA_ID;
                                                    CompCoresList.AddRange(CC.ComponentCores.Values);
                                                    CC.LoadComponentConfig(PathMan["LocalProjectRoot"]);
                                                    CC.LoadCoreConfigs(PathMan["LocalProjectRoot"]);
                                                }
                                                else
                                                {
                                                    // Non-Component "Component"  Read each pcore individually
                                                    foreach (XmlNode xPCoreNode in xComponentNode.ChildNodes)
                                                    {
                                                        if (String.Compare(xPCoreNode.Name, "PCore", true) == 0)
                                                        {
                                                            // This is the PCore Node.   Get the info needed
                                                            string pcType = string.Empty;
                                                            string pcLoc = string.Empty;
                                                            string pcInst = string.Empty;
                                                            string pcVer = string.Empty;
                                                            string pcNative = string.Empty;
                                                            string pcUCF = string.Empty;

                                                            foreach (XmlAttribute xPCoreAttr in xPCoreNode.Attributes)
                                                            {
                                                                if (String.Compare(xPCoreAttr.Name, "Type", true) == 0)
                                                                {
                                                                    pcType = xPCoreAttr.Value;
                                                                }
                                                                else if (String.Compare(xPCoreAttr.Name, "Location", true) == 0)
                                                                {
                                                                    pcLoc = xPCoreAttr.Value;
                                                                }
                                                                else if (String.Compare(xPCoreAttr.Name, "Instance", true) == 0)
                                                                {
                                                                    pcInst = xPCoreAttr.Value;
                                                                }
                                                                else if (String.Compare(xPCoreAttr.Name, "Version", true) == 0)
                                                                {
                                                                    pcVer = xPCoreAttr.Value;
                                                                }
                                                                else if (String.Compare(xPCoreAttr.Name, "Native", true) == 0)
                                                                {
                                                                    pcNative = xPCoreAttr.Value;
                                                                }
                                                                else if (String.Compare(xPCoreAttr.Name, "UCF", true) == 0)
                                                                {
                                                                    pcUCF = xPCoreAttr.Value;
                                                                }
                                                            }

                                                            ComponentCore pcore = new ComponentCore(pcNative, pcType);
                                                            pcore.CoreInstance = pcInst;
                                                            pcore.CoreType = pcType;
                                                            pcore.CoreSource = pcLoc;
                                                            pcore.CoreVersion = pcVer;
                                                            pcore.NativeInstance = pcNative;
                                                            pcore.MappedFPGA = FPGA_ID;
                                                            pcore.OwnerComponent = null;
                                                            pcore.CoreUCF = pcUCF;
                                                            CompCoresList.Add(pcore);
                                                            pcore.LoadCoreConfig(PathMan["LocalProjectRoot"]);
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
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                return false;
            }
            #endregion
            return true;
        }

        #endregion
    }
}