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
 * CorePropertiesDialog.cs
 * Name: Matthew Cotter
 * Date: 21 Sep 2010 
 * Description: Dialog to display, manage and edit core properties.
 * History: 
 * >> (24 Oct 2010) Matthew Cotter: Corrected support for named-value properties.
 * >> (22 Oct 2010) Matthew Cotter: Added support for 'General' unspecified propertie category.
 * >> (24 Sep 2010) Matthew Cotter: Corrected display and access of core properties and tabs on dialog.
 * >> (22 Sep 2010) Matthew Cotter: Implemented dialog to manage and save ethernet communication properties.
 * >> (21 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CerebrumNetronObjects;
using CerebrumSharedClasses;

namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of properties associated with a core.
    /// </summary>
    public partial class CorePropertiesDialog : Form
    {
        private CerebrumCore cCore;

        /// <summary>
        /// Default constructor.  Initializes an empty form
        /// </summary>
        public CorePropertiesDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads properties from the specified Core object and organizes them into controls for editing.
        /// </summary>
        /// <param name="CC">The CerebrumCore whose properties are to be edited.</param>
        public void LoadPropertiesFromCoreObject(CerebrumCore CC)
        {
            this.Text = String.Format("Properties - '{0}' Instance: '{1}'", CC.CoreType, CC.CoreInstance);

            cCore = CC;
            tips.RemoveAll();
            tabProperties.TabPages.Clear();
            tabProperties.SuspendLayout();

            foreach (CerebrumPropertyEntry cpe in cCore.CoreProperties)
            {
                if (!cpe.DialogVisible)
                    continue;

                TabPage pg = null;
                // Get or create the tab page
                string PropertyCat = cpe.PropertyCategory;
                if ((PropertyCat == null) || (PropertyCat == string.Empty))
                    PropertyCat = "General";

                if (!tabProperties.TabPages.ContainsKey(PropertyCat))
                    tabProperties.TabPages.Add(PropertyCat, PropertyCat);
                pg = tabProperties.TabPages[PropertyCat];
                if (pg.Tag == null)
                    pg.Tag = 0;
                int tabPropCount = (int)pg.Tag;

                // Create the label and Input Object
                Label PropLabel = new Label();
                PropLabel.Text = cpe.PropertyName;
                tips.SetToolTip(PropLabel, cpe.PropertyDescription);

                Control EntryControl = null;
                if (cpe.PropertyValues.Count > 0)
                {
                    // Use a combo-box
                    ComboBox entryBox = new ComboBox();
                    entryBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    entryBox.Items.Clear();
                    entryBox.DisplayMember = "Key";
                    entryBox.ValueMember = "Value";
                    foreach (KeyValuePair<string, string> pair in cpe.PropertyValues)
                    {
                        entryBox.Items.Add(pair);
                        if (pair.Value == cpe.PropertyValue)
                            entryBox.SelectedIndex = entryBox.Items.Count - 1;
                    }
                    EntryControl = entryBox;
                }
                else
                {
                    // Use a text box
                    TextBox entryBox = new TextBox();
                    entryBox.Text = cpe.PropertyValue;
                    EntryControl = entryBox;
                }
                pg.Controls.Add(PropLabel);
                pg.Controls.Add(EntryControl);
                PropLabel.Top = 10 + (tabPropCount * 30);
                PropLabel.Left = 10;
                PropLabel.Width = (pg.ClientRectangle.Width - 40) / 2;
                PropLabel.Height = 30;

                EntryControl.Top = PropLabel.Top;
                EntryControl.Left = 10 + PropLabel.Width + 10;
                EntryControl.Width = PropLabel.Width;
                EntryControl.Height = 30;

                EntryControl.Tag = cpe;

                pg.Tag = tabPropCount + 1;
            }
            tabProperties.ResumeLayout();
        }

        /// <summary>
        /// Saves the values specified in the form into the corresponding properties from which they were originally loaded.
        /// </summary>
        public void SaveProperties()
        {
            if (cCore == null)
                return;
            foreach (TabPage pg in tabProperties.TabPages)
            {
                foreach (Control c in pg.Controls)
                {
                    CerebrumPropertyEntry AssociatedProperty = null;
                    AssociatedProperty = (CerebrumPropertyEntry)c.Tag;
                    if (AssociatedProperty != null)
                    {
                        if (c.GetType() == typeof(TextBox))
                        {
                            TextBox tb = (TextBox)c;
                            AssociatedProperty.SetValue(tb.Text);
                        }
                        else if (c.GetType() == typeof(ComboBox))
                        {
                            ComboBox cb = (ComboBox)c;
                            if (cb.SelectedIndex == -1)
                                continue;
                            AssociatedProperty.SetValue(((KeyValuePair<string, string>)cb.SelectedItem).Value);
                        }
                    }
                }
            }
        }
    }
}
