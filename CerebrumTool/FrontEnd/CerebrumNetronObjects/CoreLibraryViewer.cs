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
 * CoreLibraryViewer.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: This is a Netron-based object used to view library tabs of core libraries.
 * History: 
 * >> (14 Feb 2010) Matthew Cotter: Corrected bug that was causing the library viewer to sometimes become disabled and inaccessible.
 * >> (16 Dec 2010) Matthew Cotter: Added code to prevent display of toolbox libraries with no entries.
 * >> (22 Oct 2010) Matthew Cotter: Created functions to create core instances with pre-defined instance names rather than defaults.
 * >> (10 Oct 2010) Matthew Cotter: Added support for collapsing and expanding all library tabs and propagating Core Error messages to top-level panel.
 * >> (13 Sep 2010) Matthew Cotter: Implemented basics of Netron-inspired LibraryTab class, along with Cerebrum/Core specific methods to load Core package/library definitions.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using IP.Components;

using Netron.Diagramming.Core;
using System.Diagnostics;
using System.IO;
using Netron.Diagramming.Win;
using CerebrumSharedClasses;

namespace CerebrumNetronObjects
{
    /// <summary>
    /// Creates a LibraryViewer toolbox associated with a Netron-based design diagram
    /// </summary>
    public class CoreLibraryViewer : Toolbox
    {
        private CoreLibraryManager myManager;
        /// <summary>
        /// The diagram control associated with this LibraryViewer
        /// </summary>
        protected IDiagramControl myDiagram;
        /// <summary>
        /// The diagram control associated with this LibraryViewer
        /// </summary>
        public IDiagramControl Diagram
        {
            get
            {
                return myDiagram;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                myDiagram = value;
                OnNewDiagram();
            }
        }

        /// <summary>
        /// Default constructor.  Creates an empty LibraryViewer
        /// </summary>
        public CoreLibraryViewer()
            : base(false)
        {
            myManager = new CoreLibraryManager();
            myManager.CoreError += new CoreErrorMessage(OnCoreError);
            
        }

        /// <summary>
        /// Loads the specified XML definition file into the specified Library
        /// </summary>
        /// <param name="LibraryName">The name of the library to load the core definition into</param>
        /// <param name="path">The path to the core definition file</param>
        public void LoadCoreDefinition(string LibraryName, string path)
        {
            try
            {
                CoreLibrary lib = null;
                if (this.myManager.ContainsLibrary(LibraryName))
                    lib = myManager.GetLibrary(LibraryName);
                else
                {
                    lib = myManager.CreateLibrary(LibraryName);
                }
                lib.LoadCoreDefinition(path);
                if (lib.Shapes.Count > 0)
                {
                    AddLibrary(lib);
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
        }

        /// <summary>
        /// Clears all libraries and tools from the library tab
        /// </summary>
        public void Clear()
        {
            this.Categories.Clear();
            this.myDiagram = null;
            myManager = new CoreLibraryManager();
        }
        /// <summary>
        /// Adds a new library to the library manager and toolbox
        /// </summary>
        /// <param name="library">The CoreLibrary to be added</param>
        public void AddLibrary(CoreLibrary library)
        {
            if (this.Diagram == null)
            {
                throw new Exception("Cannot add a library to LibraryViewer " +
                    "before assigning a diagram.");
            }

            if (!myManager.ContainsLibrary(library.Name))
                myManager.Libraries.Add(library);

            Toolbox.Tab tab = this.Categories[library.Name];

            if (tab == null)
            {
                tab = new Tab(library.Name);
            
                this.Categories.Add(tab);
                tab.SelectedChanged +=
                    new EventHandler<TabEventArgs>(tab_SelectedChanged);
            }

            foreach (IShape shape in library.Shapes)
            {
                // Important!  You have to add the item to the tab items
                // BEFORE assigning the shape so the Text and ToolTip
                // properties take.  For some reason, if you don't then
                // the default values ("item") are used.
                bool bAlreadyInTab = false;
                foreach (ShapeToolBoxItem stbItem in tab.Items)
                {
                    if (stbItem.Shape == shape)
                    {
                        bAlreadyInTab = true;
                        break;
                    }
                }
                if (!bAlreadyInTab)
                {
                    ShapeToolBoxItem item = new ShapeToolBoxItem();
                    tab.Items.Add(item);
                    item.Shape = shape;
                }
            }
        }


        /// <summary>
        /// Event fired when this core generates an error message
        /// </summary>
        public event CoreErrorMessage CoreError;
        void OnCoreError(CerebrumCore Core, string Message)
        {
            if (CoreError != null)
                CoreError(Core, Message);
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
            foreach (CoreLibrary lib in myManager.Libraries)
            {
                if (lib.ContainsCore(Type, Version))
                    return lib.CreateCoreInstance(Instance, Type, Version);
            }
            return null;
        }

        /// <summary>
        /// Method invoked when a new diagram is attached to the toolbox
        /// </summary>
        protected virtual void OnNewDiagram()
        {
            this.Enabled = true;
            this.AllowDrop = true;
        }

        /// <summary>
        /// Expands all collapsible library tabs
        /// </summary>
        public void ExpandAll()
        {
            foreach (Tab t in this.Categories)
            {
                if (!t.Opened)
                    t.Opened = true;
            }
        }
        /// <summary>
        /// Collapses all collapsible library tabs
        /// </summary>
        public void CollapseAll()
        {
            foreach (Tab t in this.Categories)
            {
                if (t.Opened)
                    t.Opened = false;
            }
        }
        // ------------------------------------------------------------------
        /// <summary>
        /// Deactivate all tools when the non-removable "Pointer" item is
        /// selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        // ------------------------------------------------------------------
        void tab_SelectedChanged(object sender, TabEventArgs e)
        {
            // Each Tab has a non-removable pointer item.  Deactivate all
            // tools when this is clicked.
            if (this.SelectedItem != null)
            {
                if (this.SelectedItem == e.Tab.PointerItem)
                {
                    this.myDiagram.Controller.DeactivateAllTools();
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Starts a new drag-drop action if the selected item is a
        /// ShapeToolBoxItem.
        /// </summary>
        /// <param name="e">MouseEventArgs</param>
        // ------------------------------------------------------------------
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (this.SelectedItem != null)
            {
                if (this.SelectedItem is ShapeToolBoxItem)
                {
                    ShapeToolBoxItem item =
                        (ShapeToolBoxItem)this.SelectedItem;
                    this.StartDragDrop(item);
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Begins a new drag-drop action.  For the shape that belongs to the
        /// specified ShapeToolBoxItem, a new instance is created from the
        /// LibraryManager.  We have to create a new instance of the shape
        /// so multiple shapes can be added independent of one another.
        /// </summary>
        /// <param name="shapeToolBoxItem">ShapeToolBoxItem</param>
        // ------------------------------------------------------------------
        void StartDragDrop(ShapeToolBoxItem shapeToolBoxItem)
        {
            string guid = "";
            try
            {
                guid = shapeToolBoxItem.Shape.Uid.ToString();
                IDataObject dataObject = new DataObject();
                IShape shape = this.myManager.CreateNewInstance(guid);
                if (shape != null)
                {
                    dataObject.SetData(
                        "IShape",
                        shape);

                    this.DoDragDrop(dataObject, DragDropEffects.Copy);
                    Trace.WriteLine("LibraryViewer set the data " +
                        "to be dragged to shape " + shape.EntityName);
                }
            }
            catch
            {
                Trace.WriteLine("LibraryViewer, could not create " +
                    "instance for shape with GUID\n" +
                    guid);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Enables drag-drop if the item on the clipboard is an IShape.
        /// </summary>
        /// <param name="drgevent"></param>
        // ------------------------------------------------------------------
        protected override void OnDragOver(DragEventArgs drgevent)
        {
            base.OnDragOver(drgevent);

            if (drgevent.Data.GetDataPresent("IShape"))
            {
                drgevent.Effect = DragDropEffects.Copy;
            }
            else
            {
                drgevent.Effect = DragDropEffects.None;
            }
        }
    }
}
