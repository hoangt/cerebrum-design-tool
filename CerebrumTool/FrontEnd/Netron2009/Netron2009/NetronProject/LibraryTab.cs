using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using Netron.Diagramming.Core;
using Netron.Diagramming.Win;

using WeifenLuo.WinFormsUI.Docking;

namespace NetronProject
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A dockable Form that hosts a "LibraryViewer".  The LibraryViewer is
    /// what shows all shape libraries loaded for the user to drag and drop
    /// onto the diagram.  This is like the "Toolbox" in Visual Studio.
    /// </summary>
    // ----------------------------------------------------------------------
    public class LibraryTab : DockContent, IDiagramHandlerTab
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Shows all shape libraries.
        /// </summary>
        // ------------------------------------------------------------------
        LibraryViewer myLibraryViewer;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the DiagramControl the library viewer is attached to.
        /// </summary>
        // ------------------------------------------------------------------
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

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public LibraryTab()
            : base()
        {
            InitializeComponent();
        }

        #region Initialization

        // ------------------------------------------------------------------
        /// <summary>
        /// Initializes the Form.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void InitializeComponent()
        {
            // Initialize the library viewer.
            myLibraryViewer = new LibraryViewer();
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

        #endregion

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
            myLibraryViewer.LoadAllLibraries(folder);
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
                myLibraryViewer.LoadLibrary(path);
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// Returns IShape of entity
        /// </summary>
        /// <param name="LibraryName">Name of Library</param>
        /// <param name="ShapeName">Name of shape</param>
        /// <returns>IShape of entity</returns>
        public IShape GenerateCustomEntity(string LibraryName, string ShapeName)
        {
            try
            {
                return (myLibraryViewer.GenerateCustomEntity(LibraryName, ShapeName));
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
