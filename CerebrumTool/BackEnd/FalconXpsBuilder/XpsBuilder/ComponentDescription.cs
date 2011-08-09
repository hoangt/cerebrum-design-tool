using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CerebrumSharedClasses;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace FalconXpsBuilder
{
    /// <summary>
    /// Describes generic attributes of a component--enough to identify and locate it--as well as sources of static/custom MHS information used by the core.
    /// </summary>
    public class ComponentDescription
    {
        private List<string> _CustomXMLs;
        private List<string> _NativeCoreInstances;
        /// <summary>
        /// Constructor initializes Component Description with ID, Source Location, and Owning FPGA.
        /// </summary>
        /// <param name="ID">The Component ID</param>
        /// <param name="Location">Source of Component Information</param>
        /// <param name="OwnerFPGA">The ID of the FPGA to which this component is mapped</param>
        /// <param name="Type">The type name of the component</param>
        public ComponentDescription(string ID, string Location, string OwnerFPGA, string Type)
        {
            this.ID = ID;
            this.Location = Location;
            this.OwnerFPGA = OwnerFPGA;
            this.Type = Type;
            this._CustomXMLs = new List<string>();
            this._NativeCoreInstances = new List<string>();
            this._Properties = new List<CerebrumPropertyEntry>();
        }
        /// <summary>
        /// Get or set the ID of the component
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Get or set the location of component information
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// Get the list of custom XML sources defined by the component.
        /// </summary>
        public List<string> CustomXMLSources
        {
            get
            {
                return _CustomXMLs;
            }
        }
        /// <summary>
        /// FPGA Name where is the Component should be loaded or mapped.
        /// </summary>
        public string OwnerFPGA { get; set; }
        /// <summary>
        /// The Type Identifier of the component
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// List of core instance, as native to the core definition
        /// </summary>
        public List<string> NativeCoreInstances
        {
            get
            {
                return _NativeCoreInstances;
            }
        }


        #region Top-Level Component Property Handling
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
            foreach (CerebrumPropertyEntry CPE in this._Properties)
            {
                if ((CPE.PropertyName == null) || (CPE.PropertyName == string.Empty))
                    continue;

                if (Output.Contains(CPE.PropertyName))
                    Output = Output.Replace(CPE.PropertyName, CPE.PropertyValue);
            }
            return Output;
        }

        private List<CerebrumPropertyEntry> _Properties;
        /// <summary>
        /// Get the list of global properties and parameters set for this component
        /// </summary>
        public List<CerebrumPropertyEntry> Properties
        {
            get
            {
                return _Properties;
            }
        }
        /// <summary>
        /// Sets the value of a specified property to the specified value
        /// </summary>
        /// <param name="PropertyName">The name of the property</param>
        /// <param name="PropertyValue">The new value of the property</param>
        public void SetPropertyValue(string PropertyName, string PropertyValue)
        {
            CerebrumPropertyEntry cpe = GetProperty(PropertyName);
            if (cpe != null)
                cpe.SetValue(PropertyValue);
        }
        /// <summary>
        /// Gets the current value of the specified property
        /// </summary>
        /// <param name="PropertyName">The name of the property</param>
        /// <returns>The value of the property</returns>
        public string GetPropertyValue(string PropertyName)
        {
            CerebrumPropertyEntry cpe = GetProperty(PropertyName);
            if (cpe != null)
                return cpe.PropertyValue;
            return null;
        }
        private CerebrumPropertyEntry GetProperty(string PropertyName)
        {
            foreach (CerebrumPropertyEntry cpe in _Properties)
                if (String.Compare(cpe.PropertyName, PropertyName) == 0)
                    return cpe;
            return null;
        }
        /// <summary>
        /// Saves the top-level parameters of the component.
        /// </summary>
        /// <param name="ProjectPath">The path to the project in which to save the parameters</param>
        public void SaveCoreConfig(string ProjectPath)
        {
            #region Verify that the Directory Exists
            if (!Directory.Exists(ProjectPath))
                return;
            if ((this.ID == null) || (this.ID == string.Empty))
                return;
            if (!Directory.Exists(ProjectPath + "\\core_config"))
                Directory.CreateDirectory(ProjectPath + "\\core_config");

            string ConfigFile = String.Format("{0}\\core_config\\{1}.xml", ProjectPath, this.ID);
            #endregion

            #region Set the attributes so that the file matches this instance
            XmlDocument xDoc = new XmlDocument();

            xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlNode xRoot = xDoc.CreateElement("CoreConfig");
            XmlElement xConfigNode = xDoc.CreateElement("MPD");
            xConfigNode.SetAttribute("Instance", this.ID);
            xConfigNode.SetAttribute("Core", this.Type);
            #endregion

            #region Save the Parameters
            foreach (CerebrumPropertyEntry cpe in _Properties)
            {
                XmlElement xeNode = null;
                xeNode = xDoc.CreateElement(cpe.PropertyType.ToString());
                xeNode.SetAttribute("Name", cpe.PropertyName);
                xeNode.SetAttribute("Value", cpe.PropertyValue);
                xConfigNode.AppendChild(xeNode);
            }
            xRoot.AppendChild(xConfigNode);
            xDoc.AppendChild(xRoot);
            xDoc.Save(ConfigFile);
            #endregion
        }
        /// <summary>
        /// Loads the top-level parameters of the component.
        /// </summary>
        /// <param name="ProjectPath">The path to the project from which to load the parameters</param>
        public void LoadCoreConfig(string ProjectPath)
        {
            #region Verify that the Directory/File Exists
            if (!Directory.Exists(ProjectPath))
                return;
            if ((this.ID == null) || (this.ID == string.Empty))
                return;
            if (!Directory.Exists(ProjectPath + "\\core_config"))
                Directory.CreateDirectory(ProjectPath + "\\core_config");

            string ConfigFile = String.Format("{0}\\core_config\\{1}.xml", ProjectPath, this.ID);
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

            if ((xType.Value != this.Type) || (xInst.Value != this.ID))
                return;
            #endregion

            #region Parse the Parameters
            foreach (XmlNode xNode in xConfigNode.ChildNodes)
            {
                CerebrumPropertyEntry cpe = null;
                string PropName = string.Empty;
                foreach (XmlAttribute xAttr in xNode.Attributes)
                {
                    if (String.Compare(xAttr.Name, "Name") == 0)
                    {
                        PropName = xAttr.Value;
                        break;
                    }
                }
                cpe = GetProperty(PropName);
                if (cpe == null)
                {
                    cpe = new CerebrumPropertyEntry();
                    cpe.PropertyName = PropName;
                    cpe.PropertyType = (CerebrumPropertyTypes)Enum.Parse(typeof(CerebrumPropertyTypes), xNode.Name);
                    cpe.DialogVisible = false;
                }

                foreach (XmlAttribute xAttr in xNode.Attributes)
                {
                    if (String.Compare(xAttr.Name, "Value") == 0)
                    {
                        cpe.SetValue(xAttr.Value);
                    }
                    else if (String.Compare(xAttr.Name, "Type") == 0)
                    {
                        cpe.PropertyValueType = xAttr.Value;
                    }
                    else if (String.Compare(xAttr.Name, "Default") == 0)
                    {
                        cpe.PropertyDefault = xAttr.Value;
                    }
                    else if (String.Compare(xAttr.Name, "Range") == 0)
                    {
                        cpe.ParseRange(xAttr.Value);
                    }
                    else if (String.Compare(xAttr.Name, "Values") == 0)
                    {
                        cpe.ParseValues(xAttr.Value);
                    }
                    else if (String.Compare(xAttr.Name, "Description") == 0)
                    {
                        cpe.PropertyDescription = xAttr.Value;
                    }
                }
                bool bAdd = true;
                foreach (CerebrumPropertyEntry CPE in _Properties)
                {
                    if (String.Compare(cpe.PropertyName, CPE.PropertyName, true) == 0)
                    {
                        bAdd = false;
                        break;
                    }
                }
                if (bAdd)
                    _Properties.Add(cpe);
            }
            #endregion
        }
        #endregion

    }
}
