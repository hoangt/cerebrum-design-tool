using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

using Netron.Diagramming.Core;
using Netron.Diagramming.Win;
using System.Diagnostics;

namespace NetronProject
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A Panel that hosts the ToolStripContainer for all ToolStrips and
    /// a DockPanel for all of the dockable Forms.
    /// </summary>
    // ----------------------------------------------------------------------
    public class ProjectPanel : Panel
    {
        protected DockPanel myDockPanel;
        protected LibraryTab myLibraryTab;
        protected ExplorerTab myExplorerTab;
        protected DiagramToolStripContainer myToolStripContainer;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the library tab that hosts a LibraryViewer.  A new LibraryTab
        /// is created if the current one is 'null'.
        /// </summary>
        // ------------------------------------------------------------------
        public LibraryTab LibraryTab
        {
            get
            {
                if (myLibraryTab == null)
                {
                    myLibraryTab = new LibraryTab();
                }
                return myLibraryTab;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the library tab that hosts a LibraryViewer.  A new LibraryTab
        /// is created if the current one is 'null'.
        /// </summary>
        // ------------------------------------------------------------------
        public ExplorerTab ExplorerTab
        {
            get
            {
                if (myExplorerTab == null)
                {
                    myExplorerTab = new ExplorerTab();
                }
                return myExplorerTab;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the active DiagramTab.  If there are no DiagramTab's
        /// currently being viewed, then 'null' is returned.
        /// </summary>
        // ------------------------------------------------------------------
        public IDiagramTab ActiveDiagramTab
        {
            get
            {
                IDiagramTab activeTab = null;
                if (myDockPanel.ActiveDocument != null)
                {
                    if (myDockPanel.ActiveDocument is IDiagramTab)
                    {
                        activeTab = myDockPanel.ActiveDocument as IDiagramTab;
                    }
                }
                return activeTab;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the number of IDiagramTabs that exist in the dock panel.
        /// </summary>
        // ------------------------------------------------------------------
        public int NumberOfDiagramTabs
        {
            get
            {
                int count = 0;
                IDockContent[] documents = myDockPanel.DocumentsToArray();
                foreach (IDockContent doc in documents)
                {
                    if (doc is IDiagramTab)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public ProjectPanel()
            : base()
        {
            Initialize();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Initializes the diagram tool strip container and dock panel.  The 
        /// library and explorer tabs are not shown.  Call "AddDiagram(name)"
        /// to add a DiagramTab, "ShowLibrary()" to show the LibraryTab, and
        /// "ShowExplorer()" to show the ExplorerTab.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void Initialize()
        {
            myToolStripContainer = new DiagramToolStripContainer();
            Controls.Add(myToolStripContainer);
            myToolStripContainer.Dock = DockStyle.Fill;

            myDockPanel = new DockPanel();
            myDockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            myToolStripContainer.ContentPanel.Controls.Add(myDockPanel);
            myDockPanel.Dock = DockStyle.Fill;

            myDockPanel.ActiveContentChanged +=
                new EventHandler(OnActiveContentChanged);

            myDockPanel.ActiveDocumentChanged +=
                new EventHandler(OnActiveDocumentChanged);
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
        public void LoadAllLibraries(string path)
        {
            LibraryTab.LoadAllLibraries(path);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Attaches each IDiagramHandlerTab to the currently active 
        /// IDiagramTab.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void AttachToActiveDiagram()
        {
            Trace.WriteLine("Attaching to active diagram");
            // First get the active diagram tab.
            IDiagramTab tab = ActiveDiagramTab;

            if (tab != null)
            {
                if (tab.Diagram != myToolStripContainer.Diagram)
                {
                    Trace.WriteLine("Attaching diagram to tool strip container");
                    myToolStripContainer.Diagram = tab.Diagram;
                }

                // Now, loop through all IDiagramHandlerTab's and attach
                // them to the active diagram.
                IDiagramHandlerTab diagramHandler;
                foreach (IDockContent form in myDockPanel.Contents)
                {
                    if (form is IDiagramHandlerTab)
                    {
                        diagramHandler = form as IDiagramHandlerTab;
                        if (diagramHandler.Diagram != tab.Diagram)
                        {
                            Trace.WriteLine("Attaching diagram to tab " +
                                form.DockHandler.TabText);
                            diagramHandler.Diagram = tab.Diagram;
                        }
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Attaches each IDiagramHandlerTab to the currently active 
        /// IDiagramTab.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void OnActiveDocumentChanged(object sender, EventArgs e)
        {
            AttachToActiveDiagram();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Attaches each IDiagramHandlerTab to the currently active 
        /// IDiagramTab.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void OnActiveContentChanged(
            object sender,
            EventArgs e)
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a new DiagramTab to the project.  The name used is
        /// "Diagram" + number of diagram tabs.  For example, if there are
        /// currently 2 diagram tabs and this method is called, then the
        /// name of the new diagram tab will be "Diagram3".
        /// </summary>
        /// <returns>int: The number of IDiagramTabs.</returns>
        // ------------------------------------------------------------------
        public int AddDiagram()
        {
            string name = "Diagram" + (NumberOfDiagramTabs + 1).ToString();
            return AddDiagram(name);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a new DiagramTab using the specified name.
        /// </summary>
        /// <param name="name">string</param>
        /// <returns>int: The number of IDiagramTabs.</returns>
        // ------------------------------------------------------------------
        public int AddDiagram(string name)
        {
            DiagramTab tab = new DiagramTab();
            tab.Name = name;
            tab.Text = name;
            tab.Show(myDockPanel);

            //LibraryTab.Diagram = tab.Diagram;
            //ExplorerTab.Diagram = tab.Diagram;
            //myToolStripContainer.Diagram = tab.Diagram;
            
            // Note, we don't have to attach the new Diagram to the explorer
            // and library - that's done when the active content in the dock
            // panel is changed.
            return NumberOfDiagramTabs;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Shows the LibraryTab.
        /// </summary>
        // ------------------------------------------------------------------
        public void ShowLibrary()
        {
            LibraryTab.Show(myDockPanel);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Shows the ExplorerTab.
        /// </summary>
        // ------------------------------------------------------------------
        public void ShowExplorer()
        {
            ExplorerTab.Show(myDockPanel);
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
                return (myLibraryTab.GenerateCustomEntity(LibraryName, ShapeName));
            }
            catch (Exception ex)
            {
                return null;
            }            
        }
    }
}
