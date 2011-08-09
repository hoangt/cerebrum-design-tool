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
 * ProjectPropertiesDialog.cs
 * Name: Matthew Cotter
 * Date: 18 Feb 2011 
 * Description: Dialog to display, manage and edit project properties.
 * History: 
 * >> (18 Feb 2011) Matthew Cotter: Simple implementation to dialog and edit project properties.
 * >> (18 Feb 2011) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FalconPathManager;
using CerebrumSharedClasses;
using CerebrumNetronObjects;


namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of project properties.
    /// </summary>
    public partial class ProjectPropertiesDialog : Form
    {
        private PathManager _PathMan;
        /// <summary>
        /// Initializes the form and assigns the associated path manager.
        /// </summary>
        /// <param name="PathMan">The pre-loaded project path manager from and to which the project properties should be loaded and saved.</param>
        public ProjectPropertiesDialog(PathManager PathMan)
        {
            _PathMan = PathMan;
            InitializeComponent();
        }

        /// <summary>
        /// Loads the form-indicates properties from the project path manager.
        /// </summary>
        public void LoadProjectProperties()
        {
            foreach (TabPage page in tabProperties.TabPages)
            {
                foreach (Control c in page.Controls)
                {
                    if (c.Tag != null)
                    {
                        string Property = (string)c.Tag;
                        if (_PathMan.HasPath(Property))
                        {
                            if (c is TextBox)
                            {
                                TextBox PropText = (TextBox)c;
                                PropText.Text = _PathMan[Property];
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Saves the form-indicates properties into the project path manager.
        /// </summary>
        public void SaveProjectProperties()
        {
            foreach (TabPage page in tabProperties.TabPages)
            {
                foreach (Control c in page.Controls)
                {
                    if (c.Tag != null)
                    {
                        string Property = (string)c.Tag;
                        if (c is TextBox)
                        {
                            TextBox PropText = (TextBox)c;
                            _PathMan.SetPath(Property, PropText.Text);
                        }
                    }
                }
            }
        }

    }
}
