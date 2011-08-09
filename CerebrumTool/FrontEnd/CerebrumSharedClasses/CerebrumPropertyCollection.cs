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
 * CerebrumPropertyCollection.cs
 * Name: Matthew Cotter
 * Date: 10 Feb 2010 
 * Description: Encapsulates a collection of properties of various types.
 * History: 
 * >> ( 9 May 2010) Matthew Cotter: Another attempt to weed out the disconnect between Clock ports and Clock signals being connected to components.
 * >> ( 7 Apr 2010) Matthew Cotter: Corrected bug in refreshing property collections that caused existing property values to not be 
 *                                      updated properly from the file.
 * >> (15 Feb 2010) Matthew Cotter: Completed implementation of load, save, set, and store functionality for property collections.
 * >> (10 Feb 2010) Matthew Cotter: Created basic definition of a core property collection.
 * >> (10 Feb 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using FalconPathManager;

namespace CerebrumSharedClasses
{
    /// <summary>
    /// Enumeration of property types for Cerebrum cores and components
    /// </summary>
    public enum CerebrumPropertyTypes
    {
        /// <summary>
        /// Invalid property type
        /// </summary>
        None,
        /// <summary>
        /// Indicates a property used specifically by the Cerebrum Framework
        /// </summary>
        CEREBRUMPROPERTY,
        /// <summary>
        /// Indicates a PCore Parameter
        /// </summary>
        PARAMETER,
        /// <summary>
        /// Indicates a PCore Bus Interface connection
        /// </summary>
        BUS_INTERFACE,
        /// <summary>
        /// Indicates a PCore Port connection
        /// </summary>
        PORT,
        /// <summary>
        /// Defines an input clock signal for the component
        /// </summary>
        INPUT_CLOCK,
        /// <summary>
        /// Defines an output clock signal for the component
        /// </summary>
        OUTPUT_CLOCK
    }
    /// <summary>
    /// Defines fields of an INPUT_CLOCK property value
    /// </summary>
    public class ClockInfoStruct
    {
        /// <summary>
        /// Default constructor.  Populates all fields of the clock information object.
        /// </summary>
        /// <param name="InPort"></param>
        /// <param name="SrcComp"></param>
        /// <param name="SrcCore"></param>
        /// <param name="SrcPort"></param>
        public ClockInfoStruct(string InPort, string SrcComp, string SrcCore, string SrcPort)
        {
            this.InputPort = InPort;
            this.SourceComponent = SrcComp;
            this.SourceCore = SrcCore;
            this.SourcePort = SrcPort;
        }
        /// <summary>
        /// The name of the port on the core that requires the input clock.
        /// </summary>
        public string InputPort { get; set; }
        /// <summary>
        /// The instance of the component that will provide the input clock signal.
        /// </summary>
        public string SourceComponent { get; set; }
        /// <summary>
        /// The instance of the core within the component that will provide the input clock signal.
        /// </summary>
        public string SourceCore { get; set; }
        /// <summary>
        /// The port of the core within the component that will provide the input clock signal.
        /// </summary>
        public string SourcePort { get; set; }
    }

    /// <summary>
    /// Defines a class which manages access to all types of properties, referenced by name and type.
    /// </summary>
    public class CerebrumPropertyCollection
    {
        #region Private Members
        private string _PropertyCollectionInstance;
        private string _NativeInstance;
        private string _PropertyCollectionType;
        private List<CerebrumPropertyEntry> _CerebrumProperties;
        private List<CerebrumPropertyEntry> _Parameters;
        private List<CerebrumPropertyEntry> _BusInterfaces;
        private List<CerebrumPropertyEntry> _Ports;
        private List<CerebrumPropertyEntry> _InputClocks;
        private List<CerebrumPropertyEntry> _OutputClocks;
        #endregion

        #region Constructor and Initialization
        /// <summary>
        /// Basic constructor.  Initializes an empty property collection for a core/component of the specified Instance and Type
        /// </summary>
        /// <param name="TrueInstance">The true hardware instance of core/component associated with this collection of properties.</param>
        /// <param name="NativeInstance">The native (internal) instance of core/component associated with this collection of properties.</param>
        /// <param name="Type">The type of core/component associated with this collection of properties.</param>
        public CerebrumPropertyCollection(string TrueInstance, string NativeInstance, string Type)
        {
            this._PropertyCollectionInstance = TrueInstance;
            this._NativeInstance = NativeInstance;
            this._PropertyCollectionType = Type;
            Reset();
        }
        /// <summary>
        /// Reinitializes all internal lists.
        /// </summary>
        private void Reset()
        {
            _CerebrumProperties = new List<CerebrumPropertyEntry>();
            _Parameters = new List<CerebrumPropertyEntry>();
            _BusInterfaces = new List<CerebrumPropertyEntry>();
            _Ports = new List<CerebrumPropertyEntry>();
            _InputClocks = new List<CerebrumPropertyEntry>();
            _OutputClocks = new List<CerebrumPropertyEntry>();
        }

        /// <summary>
        /// Copies all parameters, properties and their values from the specified collection into this one.
        /// </summary>
        /// <param name="CloneSource">The collection from which properties are to be copied.</param>
        public void CloneFrom(CerebrumPropertyCollection CloneSource)
        {
            this.Reset();
            foreach(CerebrumPropertyEntry CPE in CloneSource.GetEntries(CerebrumPropertyTypes.PARAMETER))
            {
                CerebrumPropertyEntry newCPE = new CerebrumPropertyEntry(CPE);
                newCPE.AssociatedCore = this._NativeInstance;
                AddEntry(newCPE);
            }
            foreach(CerebrumPropertyEntry CPE in CloneSource.GetEntries(CerebrumPropertyTypes.PORT))
            {
                CerebrumPropertyEntry newCPE = new CerebrumPropertyEntry(CPE);
                newCPE.AssociatedCore = this._NativeInstance;
                AddEntry(newCPE);
            }
            foreach(CerebrumPropertyEntry CPE in CloneSource.GetEntries(CerebrumPropertyTypes.CEREBRUMPROPERTY))
            {
                CerebrumPropertyEntry newCPE = new CerebrumPropertyEntry(CPE);
                newCPE.AssociatedCore = this._NativeInstance;
                AddEntry(newCPE);
            }
            foreach(CerebrumPropertyEntry CPE in CloneSource.GetEntries(CerebrumPropertyTypes.BUS_INTERFACE))
            {
                CerebrumPropertyEntry newCPE = new CerebrumPropertyEntry(CPE);
                newCPE.AssociatedCore = this._NativeInstance;
                AddEntry(newCPE);
            }
            foreach (CerebrumPropertyEntry CPE in CloneSource.GetEntries(CerebrumPropertyTypes.INPUT_CLOCK))
            {
                CerebrumPropertyEntry newCPE = new CerebrumPropertyEntry(CPE);
                newCPE.AssociatedCore = this._NativeInstance;
                AddEntry(newCPE);
            }
        }
        #endregion

        #region Public Accessor Methods
        /// <summary>
        /// Get or set the Instance name associated with this property collection.
        /// </summary>
        public string PropertyCollectionInstance
        {
            get
            {
                return _PropertyCollectionInstance;
            }
            set
            {
                _PropertyCollectionInstance = value;
            }
        }
        /// <summary>
        /// Get or set the Instance type associated with this property collection.
        /// </summary>
        public string PropertyCollectionType
        {
            get
            {
                return _PropertyCollectionType;
            }
            set
            {
                _PropertyCollectionType = value;
            }
        }

        /// <summary>
        /// Gets the value of the property of the specified Type and Name.  Returns null if the property was not found.
        /// </summary>
        /// <param name="PropType">The type of the property value to get.</param>
        /// <param name="PropName">The name of the property value to get.</param>
        /// <returns>Returns a string value of the property value.  Returns null if the property was not found.</returns>
        public object GetValue(CerebrumPropertyTypes PropType, string PropName)
        {
            CerebrumPropertyEntry Entry = GetEntry(PropType, PropName);
            if (Entry != null)
            {
                if (PropType == CerebrumPropertyTypes.INPUT_CLOCK)
                    return new ClockInfoStruct(Entry.ClockPort, Entry.ClockInputComponent, Entry.ClockInputCore, Entry.ClockInputCorePort);
                else
                    return Entry.PropertyValue;
            }
            return null;
        }
        /// <summary>
        /// Sets the value of the property of the specified Type and Name.  If PropType is INPUT_CLOCK, PropValue must be a InputClockInfoStruct object.
        /// </summary>
        /// <param name="PropType">The type of the property value to set.</param>
        /// <param name="PropName">The name of the property value to set.</param>
        /// <param name="PropValue">The new value of the property to set.  This object is turned into a string and saved.</param>
        /// <param name="CreateIfNotExists">If the specified property does not exist, this indicates whether or not the property should be created with the specified name and value.</param>
        public void SetValue(CerebrumPropertyTypes PropType, string PropName, object PropValue, bool CreateIfNotExists)
        {
            CerebrumPropertyEntry Entry = GetEntry(PropType, PropName);
            if (Entry != null)
            {
                if (PropType == CerebrumPropertyTypes.INPUT_CLOCK)
                {
                    if (PropValue.GetType() == typeof(ClockInfoStruct))
                    {
                        ClockInfoStruct ClockInfo = (ClockInfoStruct)PropValue;
                        Entry.ClockPort = ClockInfo.InputPort;
                        Entry.ClockInputComponent = ClockInfo.SourceComponent;
                        Entry.ClockInputCore = ClockInfo.SourceCore;
                        Entry.ClockInputCorePort = ClockInfo.SourcePort;

                        // Delete the corresponding port -- it will be set when the XPS project is rebuilt
                        List<CerebrumPropertyEntry> Ports = GetEntries(CerebrumPropertyTypes.PORT);
                        CerebrumPropertyEntry ClockPortEntry = GetEntryByName(Ports, Entry.ClockPort);
                        if (ClockPortEntry != null)
                        {
                            Ports.Remove(ClockPortEntry);
                        }
                    }
                    else
                    {
                        throw new Exception(String.Format("Unable to set value for INPUT_CLOCK property.  Invald object specified"));
                    }
                }
                else
                {
                    Entry.PropertyValue = PropValue.ToString();
                }
            }
            else if (CreateIfNotExists)
            {
                CerebrumPropertyEntry CPE = new CerebrumPropertyEntry();
                CPE.PropertyType = PropType;
                CPE.PropertyName = PropName;
                if (PropType == CerebrumPropertyTypes.INPUT_CLOCK)
                {
                    if (PropValue.GetType() == typeof(ClockInfoStruct))
                    {
                        ClockInfoStruct ClockInfo = (ClockInfoStruct)PropValue;
                        CPE.PropertyName = ClockInfo.InputPort;
                        CPE.ClockPort = ClockInfo.InputPort;
                        CPE.ClockInputComponent = ClockInfo.SourceComponent;
                        CPE.ClockInputCore = ClockInfo.SourceCore;
                        CPE.ClockInputCorePort = ClockInfo.SourcePort;

                        // Delete the corresponding port -- it will be set when the XPS project is rebuilt
                        List<CerebrumPropertyEntry> Ports = GetEntries(CerebrumPropertyTypes.PORT);
                        CerebrumPropertyEntry ClockPortEntry = GetEntryByName(Ports, CPE.ClockPort);
                        if (ClockPortEntry != null)
                        {
                            Ports.Remove(ClockPortEntry);
                        }
                    }
                    else
                    {
                        throw new Exception(String.Format("Unable to set value for INPUT_CLOCK property.  Invald object specified"));
                    }
                }
                else
                {
                    CPE.PropertyValue = (PropValue == null ? string.Empty : PropValue.ToString());
                }
                AddEntry(CPE);
            }
        }
        /// <summary>
        /// Sets the value of the property based on the supplied entry object.
        /// </summary>
        /// <param name="SetEntry">The CerebrumPropertyEntry to set or add.</param>
        /// <param name="CreateIfNotExists">If the specified property does not exist, this indicates whether or not the property should be created as specified.</param>
        public void SetValue(CerebrumPropertyEntry SetEntry, bool CreateIfNotExists)
        {
            CerebrumPropertyEntry Entry = GetEntry(SetEntry.PropertyType, SetEntry.PropertyName);
            if (Entry != null)
            {
                if (SetEntry.PropertyType == CerebrumPropertyTypes.INPUT_CLOCK)
                {
                    Entry.ClockPort = SetEntry.ClockPort;
                    Entry.ClockInputComponent = SetEntry.ClockInputComponent;
                    Entry.ClockInputCore = SetEntry.ClockInputCore;
                    Entry.ClockInputCorePort = SetEntry.ClockInputCorePort;

                    // Delete the corresponding port -- it will be set when the XPS project is rebuilt
                    List<CerebrumPropertyEntry> Ports = GetEntries(CerebrumPropertyTypes.PORT);
                    CerebrumPropertyEntry ClockPortEntry = GetEntryByName(Ports, Entry.ClockPort);
                    if (ClockPortEntry != null)
                    {
                        Ports.Remove(ClockPortEntry);
                    }
                }
                else
                {
                    Entry.PropertyValue = SetEntry.PropertyValue;
                }
            }
            else if (CreateIfNotExists)
            {
                AddEntry(SetEntry);
            }
        }
        /// <summary>
        /// Gets the CerebrumPropertyEntry object with the specified name and type from the appropriate collection, if it exists.
        /// </summary>
        /// <param name="PropType">The type of the property entry to get.</param>
        /// <param name="PropName">The name of the property entry to get.</param>
        /// <returns>Returns the CerebrumPropertyEntry object with the specified name and type from the appropriate collection, if it exists. Null otherwise.</returns>
        public CerebrumPropertyEntry GetEntry(CerebrumPropertyTypes PropType, string PropName)
        {
            List<CerebrumPropertyEntry> EntryList = GetEntries(PropType);
            if (EntryList != null)
            {
                return GetEntryByName(EntryList, PropName);
            }
            return null;
        }
        /// <summary>
        /// Gets a the list of CerebrumPropertyEntry objects stored of the specified type.
        /// </summary>
        /// <param name="PropType">The type of properties to get.</param>
        /// <returns>Returns the list of property entries of the corresponding type.</returns>
        public List<CerebrumPropertyEntry> GetEntries(CerebrumPropertyTypes PropType)
        {
            switch (PropType)
            {
                case CerebrumPropertyTypes.CEREBRUMPROPERTY:
                    return _CerebrumProperties;
                case CerebrumPropertyTypes.PORT:
                    return _Ports;
                case CerebrumPropertyTypes.PARAMETER:
                    return _Parameters;
                case CerebrumPropertyTypes.BUS_INTERFACE:
                    return _BusInterfaces;
                case CerebrumPropertyTypes.INPUT_CLOCK:
                    return _InputClocks;
                case CerebrumPropertyTypes.OUTPUT_CLOCK:
                    return _OutputClocks;
                default:
                    return null;
            }
        }
        /// <summary>
        /// Deletes a property value from the collection.
        /// </summary>
        /// <param name="PropType">The type of the property value to delete.</param>
        /// <param name="PropName">The name of the property value to delete.</param>
        public void DeleteValue(CerebrumPropertyTypes PropType, string PropName)
        {
            List<CerebrumPropertyEntry> Entries = GetEntries(PropType);
            CerebrumPropertyEntry Entry = GetEntryByName(Entries, PropName);
            if (Entry != null)
            {
                if (PropType == CerebrumPropertyTypes.INPUT_CLOCK)
                {
                    // Delete the corresponding port -- it will be set when the XPS project is rebuilt
                    List<CerebrumPropertyEntry> Ports = GetEntries(CerebrumPropertyTypes.PORT);
                    CerebrumPropertyEntry ClockPortEntry = GetEntryByName(Ports, Entry.ClockPort);
                    if (ClockPortEntry != null)
                    {
                        Ports.Remove(ClockPortEntry);
                    }
                }
                Entries.Remove(Entry);
            }
        }

        #endregion

        #region Private Add/Get Entry Methods
        /// <summary>
        /// Adds the specified property entry to the appropriate property list.
        /// </summary>
        /// <param name="CPE">The CerebrumPropertyEntry to be added to this collection.</param>
        private void AddEntry(CerebrumPropertyEntry CPE)
        {
            if ((CPE.PropertyName == string.Empty) || (CPE.PropertyName == null))
                return;
            List<CerebrumPropertyEntry> TargetList = GetEntries(CPE.PropertyType);
            if (TargetList != null)
            {
                TargetList.Add(CPE);
            }            
            //if (CPE.PropertyType == CerebrumPropertyTypes.INPUT_CLOCK)
            //{
            //    // Delete the corresponding port -- it will be set when the XPS project is rebuilt
            //    List<CerebrumPropertyEntry> Ports = GetEntries(CerebrumPropertyTypes.PORT);
            //    CerebrumPropertyEntry ClockPortEntry = GetEntryByName(Ports, CPE.ClockPort);
            //    if (ClockPortEntry != null)
            //    {
            //        Ports.Remove(ClockPortEntry);
            //    }
            //}
        }
        /// <summary>
        /// Gets the CerebrumPropertyEntry object from the provided list with the specified name.
        /// </summary>
        /// <param name="EntryList">The list of objects to search for the property with the specified name.</param>
        /// <param name="PropName">The name of the property entry to get.</param>
        /// <returns>Returns the CerebrumPropertyEntry object with the specified name from the provided list, if it exists. Null otherwise.</returns>
        private CerebrumPropertyEntry GetEntryByName(List<CerebrumPropertyEntry> EntryList, string PropName)
        {
            foreach (CerebrumPropertyEntry Entry in EntryList)
            {
                if (String.Compare(Entry.PropertyName, PropName, true) == 0)
                {
                    return Entry;
                }
            }
            return null;
        }
        #endregion

        #region Save/Load Collection Methods
        /// <summary>
        /// Loads this collection with Property entries defined in the Cerebrum CoreConfig format from the specified project directory.
        /// </summary>
        /// <param name="ProjectPath">The path to the project root directory.</param>
        public void LoadPropertyCollection(string ProjectPath)
        {
            #region Verify that the Directory/File Exists
            if (!Directory.Exists(ProjectPath))
                return;
            if ((this._PropertyCollectionInstance == null) || (this._PropertyCollectionInstance == string.Empty))
                return;
            if ((this._PropertyCollectionType == null) || (this._PropertyCollectionType == string.Empty))
                return;
            if (!Directory.Exists(ProjectPath + "\\core_config"))
                Directory.CreateDirectory(ProjectPath + "\\core_config");

            string ConfigFile = String.Format("{0}\\core_config\\{1}.xml", ProjectPath, this._PropertyCollectionInstance);
            if (!File.Exists(ConfigFile))
                return;
            #endregion

            #region Verify that the file matches this instance
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(ConfigFile);
            XmlNode xConfigNode = CerebrumXmlInterface.GetXmlNode(xDoc, "CoreConfig.MPD");

            if (xConfigNode == null)
                return;

            XmlAttribute xInst = null;
            XmlAttribute xType = null;
            foreach (XmlAttribute xAttr in xConfigNode.Attributes)
            {
                if (String.Compare(xAttr.Name, "Core", true) == 0)
                    xType = xAttr;
                else if (String.Compare(xAttr.Name, "Instance", true) == 0)
                    xInst = xAttr;
                if ((xInst != null) && (xType != null))
                    break;
            }
            if ((xInst == null) || (xType == null))
                return;

            if ((xType.Value != this._PropertyCollectionType) || (xInst.Value != this._PropertyCollectionInstance))
                return;
            #endregion

            #region Parse the Properties
            foreach (XmlNode xNode in xConfigNode.ChildNodes)
            {
                if (xNode.NodeType != XmlNodeType.Element)
                    continue;
                CerebrumPropertyEntry cpe = null;
                CerebrumPropertyTypes PropType = CerebrumPropertyTypes.None;
                try
                {
                    PropType = (CerebrumPropertyTypes)Enum.Parse(typeof(CerebrumPropertyTypes), xNode.Name.ToUpper());
                }
                catch (Exception ex)
                {
                    ErrorReporting.DebugException(ex);
                    throw new Exception(String.Format("Unknown CoreConfig type: {0} in {1}", xNode.Name, ConfigFile));
                }
                if (PropType == CerebrumPropertyTypes.None)
                    continue;


                string PropName = string.Empty;
                string PropClockPort = string.Empty;
                string PropValue = string.Empty;
                foreach (XmlAttribute xAttr in xNode.Attributes)
                {
                    if (String.Compare(xAttr.Name, "Name") == 0)
                    {
                        PropName = xAttr.Value;
                    }
                    else if (String.Compare(xAttr.Name, "Value") == 0)
                    {
                        PropValue = xAttr.Value;
                    }
                    else if (String.Compare(xAttr.Name, "Port") == 0)
                    {
                        PropClockPort = xAttr.Value;
                    }
                }
                // Instance and HW_VER properties are not to be loaded from XML
                if (String.Compare(PropName, "Instance", true) == 0)
                    continue;
                if (String.Compare(PropName, "HW_VER", true) == 0)
                    continue;

                if (PropType == CerebrumPropertyTypes.INPUT_CLOCK)
                {
                    cpe = GetEntry(PropType, PropClockPort);
                    if (cpe == null)
                    {
                        SetValue(PropType, PropClockPort, new ClockInfoStruct(PropClockPort, string.Empty, string.Empty, string.Empty), true);
                    }
                    cpe = GetEntry(PropType, PropClockPort);
                }
                else
                {
                    SetValue(PropType, PropName, PropValue, true);
                    cpe = GetEntry(PropType, PropName);
                }
                cpe.AssociatedCore = this._NativeInstance;

                foreach (XmlAttribute xAttr in xNode.Attributes)
                {
                    if (String.Compare(xAttr.Name, "Type", true) == 0)
                    {
                        cpe.PropertyValueType = xAttr.Value;
                    }
                    else if (String.Compare(xAttr.Name, "Default", true) == 0)
                    {
                        cpe.PropertyDefault = xAttr.Value;
                    }
                    else if (String.Compare(xAttr.Name, "Range", true) == 0)
                    {
                        cpe.ParseRange(xAttr.Value);
                    }
                    else if (String.Compare(xAttr.Name, "Values", true) == 0)
                    {
                        cpe.ParseValues(xAttr.Value);
                    }
                    else if (String.Compare(xAttr.Name, "Description", true) == 0)
                    {
                        cpe.PropertyDescription = xAttr.Value;
                    }
                    else if (String.Compare(xAttr.Name, "Port", true) == 0)
                    {
                        cpe.ClockPort = xAttr.Value;
                    }
                    else if (String.Compare(xAttr.Name, "InputComponent", true) == 0)
                    {
                        cpe.ClockInputComponent = xAttr.Value;
                    }
                    else if (String.Compare(xAttr.Name, "InputCore", true) == 0)
                    {
                        cpe.ClockInputCore = xAttr.Value;
                    }
                    else if (String.Compare(xAttr.Name, "InputCorePort", true) == 0)
                    {
                        cpe.ClockInputCorePort = xAttr.Value;
                    }
                }
                if (cpe.PropertyType == CerebrumPropertyTypes.INPUT_CLOCK)
                {
                    CerebrumPropertyEntry PortProp = GetEntry(CerebrumPropertyTypes.PORT, cpe.ClockPort);
                    if ((String.Compare(cpe.ClockInputComponent, string.Empty) == 0) ||
                        (String.Compare(cpe.ClockInputCore, string.Empty) == 0) ||
                        (String.Compare(cpe.ClockInputCorePort, string.Empty) == 0))
                    {
                        string PortSignal = String.Format("{0}_{1}_{2}", cpe.ClockInputComponent, cpe.ClockInputCore, cpe.ClockInputCorePort);
                        SetValue(CerebrumPropertyTypes.PORT, cpe.ClockPort, PortSignal, true);
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Saves this collection of Property entries in the Cerebrum CoreConfig format in the specified project directory.
        /// </summary>
        /// <param name="ProjectPath">The path to the project root directory.</param>
        public void SavePropertyCollection(string ProjectPath)
        {
            #region Verify that the Directory Exists
            if (!Directory.Exists(ProjectPath))
                return;
            if ((this._PropertyCollectionInstance == null) || (this._PropertyCollectionInstance == string.Empty))
                return;
            if ((this._PropertyCollectionType == null) || (this._PropertyCollectionType == string.Empty))
                return;
            if (!Directory.Exists(ProjectPath + "\\core_config"))
                Directory.CreateDirectory(ProjectPath + "\\core_config");

            string ConfigFile = String.Format("{0}\\core_config\\{1}.xml", ProjectPath, this._PropertyCollectionInstance);
            #endregion

            #region Set the attributes so that the file matches this instance
            XmlDocument xDoc = new XmlDocument();

            xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlNode xRoot = xDoc.CreateElement("CoreConfig");
            XmlElement xConfigNode = xDoc.CreateElement("MPD");
            xConfigNode.SetAttribute("Instance", this._PropertyCollectionInstance);
            xConfigNode.SetAttribute("Core", this._PropertyCollectionType);
            #endregion

            #region Save the CerebrumProperties
            foreach (CerebrumPropertyEntry cpe in this.GetEntries(CerebrumPropertyTypes.CEREBRUMPROPERTY))
            {
                if ((cpe.PropertyValue == null) && (cpe.PropertyValue == string.Empty))
                    continue;
                XmlElement xeNode = null;
                xeNode = xDoc.CreateElement(cpe.PropertyType.ToString());
                xeNode.SetAttribute("Name", cpe.PropertyName);
                xeNode.SetAttribute("Value", cpe.PropertyValue);
                xConfigNode.AppendChild(xeNode);
            }
            #endregion
            
            #region Save the Parameters
            foreach (CerebrumPropertyEntry cpe in this.GetEntries(CerebrumPropertyTypes.PARAMETER))
            {
                if ((cpe.PropertyValue == null) && (cpe.PropertyValue == string.Empty))
                    continue;
                XmlElement xeNode = null;
                xeNode = xDoc.CreateElement(cpe.PropertyType.ToString());
                xeNode.SetAttribute("Name", cpe.PropertyName);
                xeNode.SetAttribute("Value", cpe.PropertyValue);
                xConfigNode.AppendChild(xeNode);
            }
            #endregion
            
            #region Save the Ports
            foreach (CerebrumPropertyEntry cpe in this.GetEntries(CerebrumPropertyTypes.PORT))
            {
                if ((cpe.PropertyValue == null) && (cpe.PropertyValue == string.Empty))
                    continue;
                XmlElement xeNode = null;
                xeNode = xDoc.CreateElement(cpe.PropertyType.ToString());
                xeNode.SetAttribute("Name", cpe.PropertyName);
                xeNode.SetAttribute("Value", cpe.PropertyValue);
                xConfigNode.AppendChild(xeNode);
            }
            #endregion
            
            #region Save the Bus Interfaces
            foreach (CerebrumPropertyEntry cpe in this.GetEntries(CerebrumPropertyTypes.BUS_INTERFACE))
            {
                if ((cpe.PropertyValue == null) && (cpe.PropertyValue == string.Empty))
                    continue;
                XmlElement xeNode = null;
                xeNode = xDoc.CreateElement(cpe.PropertyType.ToString());
                xeNode.SetAttribute("Name", cpe.PropertyName);
                xeNode.SetAttribute("Value", cpe.PropertyValue);
                xConfigNode.AppendChild(xeNode);
            }
            #endregion

            #region Save the Input Clocks
            foreach (CerebrumPropertyEntry cpe in this.GetEntries(CerebrumPropertyTypes.INPUT_CLOCK))
            {
                XmlElement xeNode = null;
                xeNode = xDoc.CreateElement(cpe.PropertyType.ToString());
                xeNode.SetAttribute("Port", cpe.ClockPort);
                xeNode.SetAttribute("InputComponent", cpe.ClockInputComponent);
                xeNode.SetAttribute("InputCore", cpe.ClockInputCore);
                xeNode.SetAttribute("InputCorePort", cpe.ClockInputCorePort);
                xConfigNode.AppendChild(xeNode);

                // Generate the input port signal
                XmlElement xeNodePort = null;
                xeNodePort = xDoc.CreateElement("PORT");
                xeNodePort.SetAttribute("Name", cpe.ClockPort);
                xeNodePort.SetAttribute("Value", String.Format("{0}_{1}_{2}", cpe.ClockInputComponent, cpe.ClockInputCore, cpe.ClockInputCorePort));
                xConfigNode.AppendChild(xeNodePort);
            }
            #endregion

            #region Save the Output Clocks
            foreach (CerebrumPropertyEntry cpe in this.GetEntries(CerebrumPropertyTypes.OUTPUT_CLOCK))
            {
                XmlElement xeNode = null;
                xeNode = xDoc.CreateElement(cpe.PropertyType.ToString());
                xeNode.SetAttribute("Port", cpe.ClockPort);
                xeNode.SetAttribute("Component", cpe.ClockInputComponent);
                xeNode.SetAttribute("Core", cpe.ClockInputCore);
                xeNode.SetAttribute("CorePort", cpe.ClockInputCorePort);
                xConfigNode.AppendChild(xeNode);

                // Generate the output port signal
                XmlElement xeNodePort = null;
                xeNodePort = xDoc.CreateElement("PORT");
                xeNodePort.SetAttribute("Name", cpe.ClockPort);
                xeNodePort.SetAttribute("Value", String.Format("{0}_{1}_{2}", cpe.ClockInputComponent, cpe.ClockInputCore, cpe.ClockInputCorePort));
                xConfigNode.AppendChild(xeNodePort);
            }
            #endregion

            xRoot.AppendChild(xConfigNode);
            xDoc.AppendChild(xRoot);
            xDoc.Save(ConfigFile);
        }
        #endregion
    }
}
