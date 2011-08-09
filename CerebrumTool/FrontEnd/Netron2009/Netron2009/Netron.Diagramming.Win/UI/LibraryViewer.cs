using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using IP.Components;

using Netron.Diagramming.Core;
using System.Diagnostics;
using System.IO;

namespace Netron.Diagramming.Win
{
    public class LibraryViewer : Toolbox
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Manages all libraries.
        /// </summary>
        // ------------------------------------------------------------------
        LibraryManager myManager;

        // ------------------------------------------------------------------
        /// <summary>
        /// The diagram to add shapes to.
        /// </summary>
        // ------------------------------------------------------------------
        protected IDiagramControl myDiagram;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the diagram to add shapes to.
        /// </summary>
        // ------------------------------------------------------------------
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

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public LibraryViewer() : base(false) // Do not create a "General" 
                                             // category.
        {
            myManager = new LibraryManager();
            this.Enabled = false;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called when a new diagram is attached.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void OnNewDiagram()
        {
            this.Enabled = true;
            this.AllowDrop = true;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a library to the viewer.
        /// </summary>
        /// <param name="library">Library: The library to add.</param>
        // ------------------------------------------------------------------
        public virtual void AddLibrary(Library library)
        {
            if (this.Enabled == false)
            {
                throw new Exception("Cannot add a library to LibraryViewer " +
                    "before assigning a diagram.");
            }

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
                ShapeToolBoxItem item = new ShapeToolBoxItem();
                tab.Items.Add(item);
                item.Shape = shape;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Loads the specified shape library into the LibraryViewer.
        /// </summary>
        /// <param name="path">string: The full path to the library ("dll") to
        /// load.</param>
        // ------------------------------------------------------------------
        public virtual void LoadLibrary(string path)
        {
            try
            {
                Library lib = new Library();
                lib.Load(path);
                AddLibrary(lib);
            }
            catch (Exception e)
            {
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Loops through all ".dll's" in the specified folder and loads
        /// all shape libraries.  This is useful if you have a root directory
        /// that hosts all shape libraries that you want loaded on startup.
        /// </summary>
        /// <param name="folder">string: The full path to the root folder
        /// that has the shape libraries to load.</param>
        // ------------------------------------------------------------------
        public virtual void LoadAllLibraries(string folder)
        {
            // Iterate through all dll's in the folder specified (if it
            // exists) and load all shape libraries.
            if (Directory.Exists(folder))
            {
                string[] files = Directory.GetFiles(folder);

                foreach (string dll in files)
                {
                    FileInfo info = new FileInfo(dll);
                    if (info.Extension == ".dll")
                    {
                        LoadLibrary(dll);
                    }
                }
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
 
        /// <summary>
        /// Returns an IShape form of the entity 
        /// </summary>
        /// <param name="LibraryName">Name of Library</param>
        /// <param name="ShapeName">Name of shape</param>
        /// <returns>IShape of entity</returns>
        public IShape GenerateCustomEntity(string LibraryName, string ShapeName)
        {
            try
            {
                foreach (Library lib in myManager.Libraries)
                {
                    if (lib.Name == LibraryName)
                    {
                        foreach (IShape shape in lib.Shapes)
                        {
                            ImageShape ImSh = (ImageShape)shape;

                            if (ImSh.EntityName == ShapeName)
                                return lib.CreateNewInstance(shape.Uid.ToString());
                        }                        
                    }                    
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return null;
        }
    }
}
