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
 * CoreLibraryTab.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: This is a Netron-based object used to manage library tabs of core libraries.
 * History: 
 * >> (22 Oct 2010) Matthew Cotter: Created functions to create core instances with pre-defined instance names rather than defaults.
 * >> (10 Oct 2010) Matthew Cotter: Added support for collapsing and expanding all library tabs and propagating Core Error messages to top-level panel.
 * >> (13 Sep 2010) Matthew Cotter: Implemented basics of Netron-inspired LibraryTab class, along with Cerebrum/Core specific methods to load Core package/library definitions.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using NetronProject;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;

using WeifenLuo.WinFormsUI.Docking;
using CerebrumSharedClasses;

namespace CerebrumNetronObjects
{
    /// <summary>
    /// Object representing a library toolbox tab in the Netron design UI
    /// </summary>
    public class CoreLibraryTab : DockContent, IDiagramHandlerTab
    {
        /// <summary>
        /// The library view object utilized by the library tab
        /// </summary>
        protected CoreLibraryViewer myLibraryViewer;

        /// <summary>
        /// Initializes LibraryTab  (events, layout, labels, etc)
        /// </summary>
        protected void InitializeComponent()
        {
            // Initialize the library viewer.
            myLibraryViewer = new CoreLibraryViewer();
            myLibraryViewer.CoreError += new CoreErrorMessage(OnCoreError);
            Controls.Add(myLibraryViewer);
            myLibraryViewer.Dock = DockStyle.Fill;

            this.SuspendLayout();
            // 
            // LibraryTab
            // 
            this.ClientSize = new Size(292, 574);
            this.DockAreas = ((DockAreas)((((DockAreas.Float |
                DockAreas.DockLeft) |
                DockAreas.DockRight) |
                DockAreas.Document)));

            this.HideOnClose = true;
            this.Name = "Library";
            this.ShowHint = DockState.DockLeft;
            this.TabText = "Library";
            this.Text = "Library";
            this.BackColor = Color.WhiteSmoke;
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Loads the specified XML definition file into the specified Library
        /// </summary>
        /// <param name="LibraryName">The name of the library to load the core definition into</param>
        /// <param name="path">The path to the core definition file</param>
        public virtual void LoadCoreDefinition(string LibraryName, string path)
        {
            try
            {
                myLibraryViewer.LoadCoreDefinition(LibraryName, path);
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
        }

        /// <summary>
        /// The Netron diagram object associated with this LibraryViewer
        /// </summary>
        public DiagramControl Diagram
        {
            get
            {
                return (DiagramControl) myLibraryViewer.Diagram;
            }
            set
            {
                if (value != null)
                {
                    myLibraryViewer.Diagram = value;
                }
            }
        }

        /// <summary>
        /// Creates a new CerebrumCore object of a given type and version, with the specified instance, if it exists in any loaded libraries
        /// </summary>
        /// <param name="Instance">The instance name of the new CerebrumCore</param>
        /// <param name="Type">The type name of the CerebrumCore to create</param>
        /// <param name="Version">The version of the CerebrumCore to create</param>
        /// <returns>A new CerebrumCore object, if a matching type/version was found; null otherwise</returns>
        public CerebrumCore CreateCoreInstance(string Instance, string Type, string Version)
        {
            return myLibraryViewer.CreateCoreInstance(Instance, Type, Version);
        }

        /// <summary>
        /// Clears all libraries and tools from the library tab
        /// </summary>
        public void Clear()
        {
            myLibraryViewer.Clear();
        }
        /// <summary>
        /// Expands all collapsible library tabs
        /// </summary>
        public void ExpandAll()
        {
            myLibraryViewer.ExpandAll();
        }
        /// <summary>
        /// Collapses all collapsible library tabs
        /// </summary>
        public void CollapseAll()
        {
            myLibraryViewer.CollapseAll();
        }


        /// <summary>
        /// Event fired when this core generates an error message
        /// </summary>
        public event CoreErrorMessage CoreError;
        void OnCoreError(CerebrumCore Core, string Message)
        {
            if (CoreError != null)
            {
                if ((Core.CoreInstance == string.Empty) || (Core.CoreInstance == null))
                    CoreError(Core, Message);
                else
                    CoreError(Core, Message);
            }
        }

        /// <summary>
        /// Default constructor.  Initializes an empty LibraryTab
        /// </summary>
        public CoreLibraryTab()
            : base()
        {
            InitializeComponent();
        }

    }
}
