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
 * CerebrumPropertyEntry.cs
 * Name: Matthew Cotter
 * Date: 11 Oct 2010 
 * Description: Defines a property, parameter, bus interface, or port that is used for setting the corresponding entry related to a component or core object.
 * History: 
 * >> (15 Feb 2010) Matthew Cotter: Added support for Clock signal properties.
 *                                  Added method to parse a parameter or property from required cores in the Platform XML.
 * >> (10 Feb 2011) Matthew Cotter: File and Class renamed to CerebrumPropertyEntry
 * >> (22 Oct 2010) Matthew Cotter: Corrected setting/loading of enumerated-value properties.
 * >> (22 Oct 2010) Matthew Cotter: Corrected initialization in constructor and added copy-constructor used when cloning a core, including its properties.
 * >> (11 Oct 2010) Matthew Cotter: Created basic definition of a core property.
 * >> (11 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CerebrumSharedClasses
{
    /// <summary>
    /// Defines a property associated with a core, including any information about the property (type, legal values, description, default, etc)
    /// </summary>
    public class CerebrumPropertyEntry
    {
        private string _Value;
        private Dictionary<string, string> _LegalValues;
        private string _RangeString;
        private string _ValueString;

        /// <summary>
        /// Default 'empty' constructor.
        /// </summary>
        public CerebrumPropertyEntry()
        {
            this._ValueString = string.Empty;
            this._RangeString = RangeString;
            this.PropertyType = CerebrumPropertyTypes.None;
            this.PropertyCategory = string.Empty;
            this.PropertyName = string.Empty;
            this.PropertyDescription = string.Empty;
            this.PropertyDefault = string.Empty;
            SetValue(this.PropertyDefault);
            this.PropertyValueType = string.Empty;
            this.PropertyParameter = string.Empty;
            this.AssociatedCore = string.Empty;
            _LegalValues = new Dictionary<string, string>();
            this._RangeString = string.Empty;
            this._ValueString = string.Empty;

            this.ClockInputComponent = string.Empty;
            this.ClockInputCore = string.Empty;
            this.ClockInputCorePort = string.Empty;
            this.ClockPort = string.Empty;
        }

        /// <summary>
        /// Copy-constructor.  Creates a new property entry object that is a copy of an existing one.
        /// </summary>
        /// <param name="CloneSource">The existing property of which to create a copy</param>
        public CerebrumPropertyEntry(CerebrumPropertyEntry CloneSource)
        {
            this.AssociatedCore = CloneSource.AssociatedCore;
            this.DialogVisible = CloneSource.DialogVisible;
            this.PropertyType = CloneSource.PropertyType;
            this.PropertyCategory = CloneSource.PropertyCategory;
            this.PropertyDefault = CloneSource.PropertyDefault;
            this.PropertyDescription = CloneSource.PropertyDescription;
            this.PropertyName = CloneSource.PropertyName;
            this.PropertyParameter = CloneSource.PropertyParameter;
            this.PropertyType = CloneSource.PropertyType;
            this.SetValue(CloneSource.PropertyValue);
            foreach (KeyValuePair<string, string> P in CloneSource.PropertyValues)
            {
                this.PropertyValues.Add(P.Key, P.Value);
            }
            this.PropertyValueType = CloneSource.PropertyValueType;
            this._RangeString = CloneSource.RangeString;
            this._ValueString = CloneSource.ValuesString;

            this.ClockInputComponent = CloneSource.ClockInputComponent;
            this.ClockInputCore = CloneSource.ClockInputCore;
            this.ClockInputCorePort = CloneSource.ClockInputCorePort;
            this.ClockPort = CloneSource.ClockPort;
        }

        #region PropertyEntry Properties
        /// <summary>
        /// Get or set the type of the property
        /// </summary>
        public CerebrumPropertyTypes PropertyType { get; set; }
        /// <summary>
        /// Get or set the category classification of the property for dialog/display purposes
        /// </summary>
        public string PropertyCategory { get; set; }
        /// <summary>
        /// Get or set the name of the property
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Get or set the default value of the property
        /// </summary>
        public string PropertyDefault { get; set; }
        /// <summary>
        /// Get or set the value of the property
        /// </summary>
        public string PropertyValue
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
            }
        }
        /// <summary>
        /// Get or set the instance name of the core to which this property applies
        /// </summary>
        public string AssociatedCore { get; set; }
        /// <summary>
        /// Get or set the name of the core parameter which corresponds to this property, if any.
        /// </summary>
        public string PropertyParameter { get; set; }
        /// <summary>
        /// Get or set a description of the property
        /// </summary>
        public string PropertyDescription { get; set; }
        /// <summary>
        /// Get or set the data type of the property (integer, string, etc)
        /// </summary>
        public string PropertyValueType { get; set; }
        /// <summary>
        /// Get or set a flag indicating whether this property is visible in dialog displays
        /// </summary>
        public bool DialogVisible { get; set; }
        /// <summary>
        /// Get a dictionary/hashtable of legal values for this property.
        /// </summary>
        public Dictionary<string, string> PropertyValues
        {
            get
            {
                if (_LegalValues == null)
                    _LegalValues = new Dictionary<string, string>();
                return _LegalValues;
            }
        }

        /// <summary>
        /// Get the raw string used to define the range of legal values
        /// </summary>
        public string RangeString 
        {
            get
            {
                return _RangeString;
            }
        }
        /// <summary>
        /// Get the raw string used to define the set of legal values
        /// </summary>
        public string ValuesString
        {
            get
            {
                return _ValueString;
            }
        }


        #region Input Clock Properties
        /// <summary>
        /// For properties of type INPUT_CLOCK only.  The name of the port on the core to which the input clock signal should be attached.
        /// </summary>
        public string ClockPort { get; set; }
        /// <summary>
        /// For properties of type INPUT_CLOCK only.  The name of the component from which this input clock signal receieves its input.
        /// </summary>
        public string ClockInputComponent { get; set; }
        /// <summary>
        /// For properties of type INPUT_CLOCK only.  The name of the core within the component from which this input clock signal receieves its input.
        /// </summary>
        public string ClockInputCore { get; set; }
        /// <summary>
        /// For properties of type INPUT_CLOCK only.  The name of the port on the core within the component from which this input clock signal receieves its input.
        /// </summary>
        public string ClockInputCorePort { get; set; }
        #endregion
        #endregion

        #region Accessor Methods
        /// <summary>
        /// Get the current value of the property
        /// </summary>
        /// <returns>The current value of the property</returns>
        public string GetValue()
        {
            return _Value;
        }
        /// <summary>
        /// Set the current value of the property.  If the property is illegal, the current value is unchanged.  If the value is null, the default value is used.
        /// </summary>
        /// <param name="NewValue">The new value to be set</param>
        public void SetValue(string NewValue)
        {
            if (this.PropertyValues.Count == 0)
                _Value = NewValue;
            else if (this.PropertyValues.Values.Contains(NewValue))
                _Value = NewValue;
            else if (NewValue == null)
                _Value = PropertyDefault;
        }
        #endregion

        /// <summary>
        /// Parses a raw string indicating the range of legal values.
        /// </summary>
        /// <param name="RangeString">The raw string of legal values</param>
        public void ParseRange(string RangeString)
        {
            this.PropertyValues.Clear();
            this._ValueString = string.Empty;
            this._RangeString = RangeString;

            if ((RangeString == string.Empty) || (RangeString == null))
                return;

            if (RangeString.Contains(':'))
            {
                // Continuous range
                string[] bounds = RangeString.Split(':');
                if (bounds.Length == 2)
                {
                    int highbound = -1, lowbound = -1;
                    if ((int.TryParse(bounds[0], out lowbound)) && (int.TryParse(bounds[1], out highbound)))
                    {
                        if (lowbound > highbound)
                        {
                            int t = lowbound;
                            lowbound = highbound;
                            highbound = t;
                        }
                        for (int i = lowbound; i <= highbound; i++)
                        {
                            this.PropertyValues.Add(i.ToString(), i.ToString());
                        }
                    }
                }
            }
            else if (RangeString.Contains(','))
            {
                // Discrete range
                string[] discreteValues = RangeString.Split(',');
                for (int i = 0; i < discreteValues.Length; i++)
                {
                    int discreteValue;
                    if (int.TryParse(discreteValues[i], out discreteValue))
                    {
                        this.PropertyValues.Add(discreteValue.ToString(), discreteValue.ToString());
                    }
                }
            }
        }
        /// <summary>
        /// Parses a raw string indicating the set of legal name/value pairs.
        /// </summary>
        /// <param name="ValueString"></param>
        public void ParseValues(string ValueString)
        {
            this.PropertyValues.Clear();
            this._ValueString = ValueString;
            this._RangeString = string.Empty;

            if ((ValueString == string.Empty) || (ValueString == null))
                return;

            string[] ValuePairs = ValueString.Split(',');
            for (int k = 0; k < ValuePairs.Length; k++)
            {
                string[] pair = ValuePairs[k].Trim().Split('=');
                if (pair.Length == 2)
                {
                    string key = pair[0].Trim();
                    string value = pair[1].Trim();
                    this.PropertyValues.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Populate the property entry from an XmlElement defining properties of a RequiredCore from a Platform.
        /// </summary>
        /// <param name="SourceFile">The file containing the XML being parsed.</param>
        /// <param name="xRequiredParameter">The XML node to be parsed.</param>
        public void ParseRequiredComponentProperty(string SourceFile, XmlNode xRequiredParameter)
        {
            string PropType = xRequiredParameter.Name.ToUpper();
            string PropCore = string.Empty;
            string PropName = string.Empty;
            string PropValue = string.Empty;

            foreach (XmlAttribute xAttr in xRequiredParameter.Attributes)
            {
                if (String.Compare(xAttr.Name, "Core", true) == 0)
                {
                    PropCore = xAttr.Value;
                }
                else if (String.Compare(xAttr.Name, "Name", true) == 0)
                {
                    PropName = xAttr.Value;
                }
                else if (String.Compare(xAttr.Name, "Value", true) == 0)
                {
                    PropValue = xAttr.Value;
                }
            }
            try
            {
                this.PropertyType = (CerebrumPropertyTypes)Enum.Parse(typeof(CerebrumPropertyTypes), xRequiredParameter.Name.ToUpper());
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                throw new Exception(String.Format("Unknown Property Type type: {0} in {1}", xRequiredParameter.Name, SourceFile));
            }
            this.PropertyName = PropName;
            this.AssociatedCore = PropCore;
            this.PropertyValue = PropValue;
        }
    }
}
