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
 * CerebrumProjectPanel.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: This is a Netron-based control used to display library cores and design layout and tools.
 * History: 
 * >> (16 Dec 2010) Matthew Cotter: Added support for Copy and Paste of cores.  Cut is defined, but not implemented.
 * >> (16 Dec 2010) Matthew Cotter: Added support for Copy and Paste of cores.  Cut is defined, but not implemented.
 *                                  Added overrides for standard hotkeys (Ctrl+C, Ctrl+V, Ctrl+X) for Copy, Paste, and Cut commands.
 *                                  --Enabled View/Align/Zoom/Pan toolbars in underlying Netron toolstrip panel(s).
 * >> (22 Oct 2010) Matthew Cotter: Moved Load and Save of Design/Layout to Project Manager class for organization purposes.
 * >> (15 Sep 2010) Matthew Cotter: Corrected bug that occured when a scanned respository path did not exist.
 * >> (10 Sep 2010) Matthew Cotter: Updated when and how the design frame is resized as the primary form resizes and the library toolbox moves.
 *                                  Added support for receiving and propagating core errors.
 *                                  Added ability to expand and collapse all library toolbox tabs.
 *                                  Corrected bug in locating and attaching connections and connectors.
 * >> (30 Sep 2010) Matthew Cotter: Added functionality to clear library toolbox when project is closed or loaded.
 * >> (23 Sep 2010) Matthew Cotter: Added MessageEventController to pass messages to main GUI form generated within project panel.
 * >> (10 Oct 2010) Matthew Cotter: Added support for collapsing and expanding all library tabs and propagating Core Error messages to top-level panel.
 * >> (13 Sep 2010) Matthew Cotter: Created Netron-inspired project design panel implementing basics required to load/save design and load core package definitions.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 * Notes:
 *  To add/modify the toolbars that are integrated into the Netron design environment, 
 *         see the Netron.Diagramming.Win project; File: ./UI/DiagramToolStripContainer.cs
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Drawing;

using WeifenLuo.WinFormsUI.Docking;

using NetronProject;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;
using System.Diagnostics;
using System.IO;
using CerebrumSharedClasses;

namespace CerebrumNetronObjects
{
    /// <summary>
    /// Delegate method to signal Cut, Copy and Paste events
    /// </summary>
    public delegate void ParameterFreeEventDelegate();

    /// <summary>
    /// Netron-based design interface including toolbar, library toolbox and diagramming environment
    /// </summary>
    public class CerebrumProjectPanel : Panel
    {
        #region Event Handling and Generation including handler of Diagram KeyUp event

        #region Diagram.KeyUp event handler
        void myDiagram_KeyUp(object sender, KeyEventArgs e)
        {
            bool Ctrl = e.Control;
            bool Shift = e.Shift;
            bool Alt = e.Alt;
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.C:
                    if (Ctrl)
                        Copy();
                    break;
                case System.Windows.Forms.Keys.X:
                    if (Ctrl)
                        Cut();
                    break;
                case System.Windows.Forms.Keys.V:
                    if (Ctrl)
                        if (Shift)
                            PasteVirtual();
                        else
                            Paste();
                    break;
                case System.Windows.Forms.Keys.R:
                    if (Ctrl)
                    {
                        if (Shift)
                            Rotate_90CCW();
                        else
                            Rotate_90CW();
                    }
                    break;
                default:
                    base.OnKeyUp(e);
                    break;
            }
        }
        #endregion

        #region Event Declarations
        /// <summary>
        /// Event that is fired when cores are to be copied
        /// </summary>
        public event ParameterFreeEventDelegate CopyEvent;
        /// <summary>
        /// Event that is fired when cores are to be cut
        /// </summary>
        public event ParameterFreeEventDelegate CutEvent;
        /// <summary>
        /// Event that is fired when cores are to be pasted
        /// </summary>
        public event ParameterFreeEventDelegate PasteEvent;
        /// <summary>
        /// Event that is fired when cores are to be pasted as virtual copies
        /// </summary>
        public event ParameterFreeEventDelegate PasteVirtualEvent;
        /// <summary>
        /// Event that is fired when cores are to be rotated 90 degrees Counter-Clockwise
        /// </summary>
        public event ParameterFreeEventDelegate Rotate90CCWEvent;
        /// <summary>
        /// Event that is fired when cores are to be rotated 90 degrees Clockwise
        /// </summary>
        public event ParameterFreeEventDelegate Rotate90CWEvent;
        #endregion

        #region Event-firing methods
        /// <summary>
        /// Method that invokes CopyEvent event for copying cores
        /// </summary>
        public void Copy()
        {
            if (CopyEvent != null) CopyEvent();
        }
        /// <summary>
        /// Method that invokes CutEvent event for cutting cores
        /// </summary>
        public void Cut()
        {
            if (CutEvent != null) CutEvent();
        }
        /// <summary>
        /// Method that invokes PasteEvent event for pasting cores
        /// </summary>
        public void Paste()
        {
            if (PasteEvent != null) PasteEvent();
        }
        /// <summary>
        /// Method that invokes PasteVirtualEvent event for pasting cores as virtual copies
        /// </summary>
        public void PasteVirtual()
        {
            if (PasteVirtualEvent != null) PasteVirtualEvent();
        }
        /// <summary>
        /// Method that invokes Rot90CCWEvent event for pasting cores
        /// </summary>
        public void Rotate_90CCW()
        {
            if (Rotate90CCWEvent != null) Rotate90CCWEvent();
        }
        /// <summary>
        /// Method that invokes Rot90CWEvent event for pasting cores
        /// </summary>
        public void Rotate_90CW()
        {
            if (Rotate90CWEvent != null) Rotate90CWEvent();
        }
        #endregion

        #region Netron-ToolStrip EventHandler events
        void myToolStripContainer_Rotate90CCW(object sender, EventArgs e)
        {
            Rotate_90CCW();
        }
        void myToolStripContainer_Rotate90CW(object sender, EventArgs e)
        {
            Rotate_90CW();
        }
        #endregion
        #endregion

        #region Netron-based Methods
        /// <summary>
        /// Docking panel associated with the design panel
        /// </summary>
        protected DockPanel myDockPanel;
        /// <summary>
        /// Core library tab associated with the design panel
        /// </summary>
        protected CoreLibraryTab myLibraryTab;
        /// <summary>
        /// Explorer (unused) tab associated with the design panel
        /// </summary>
        protected ExplorerTab myExplorerTab;
        /// <summary>
        /// Toolstrip container associated with the design panel
        /// </summary>
        protected DiagramToolStripContainer myToolStripContainer;
        private DiagramControl myDiagram;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the library tab that hosts a LibraryViewer.  A new LibraryTab
        /// is created if the current one is 'null'.
        /// </summary>
        // ------------------------------------------------------------------
        public CoreLibraryTab LibraryTab
        {
            get
            {
                if (myLibraryTab == null)
                {
                    myLibraryTab = new CoreLibraryTab();
                    myLibraryTab.CoreError += new CoreErrorMessage(OnCoreError);
                    //myLibraryTab.Dock = DockStyle.Fill;
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

        /// <summary>
        /// Design diagram associated with the design panel
        /// </summary>
        public DiagramControl Diagram
        {
            get
            {
                return myDiagram;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public CerebrumProjectPanel()
            : base()
        {
            Initialize();
            this.SizeChanged += new EventHandler(CerebrumProjectPanel_SizeChanged);
        }

        void CerebrumProjectPanel_SizeChanged(object sender, EventArgs e)
        {
            SizeDiagramWindow();
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
            myToolStripContainer.AddActionButton("Rotate 90 CCW", null, new EventHandler(myToolStripContainer_Rotate90CCW));
            myToolStripContainer.AddActionButton("Rotate 90 CW", null, new EventHandler(myToolStripContainer_Rotate90CW));

            myDockPanel = new DockPanel();
            myDockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            myToolStripContainer.ContentPanel.Controls.Add(myDockPanel);
            myDockPanel.Dock = DockStyle.Fill;


            myDockPanel.ActiveContentChanged +=
                new EventHandler(OnActiveContentChanged);

            myDockPanel.ActiveDocumentChanged +=
                new EventHandler(OnActiveDocumentChanged);

            myDockPanel.Paint += new PaintEventHandler(myDockPanel_Paint);
        }

        void myDockPanel_Paint(object sender, PaintEventArgs e)
        {
            SizeDiagramWindow();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Attaches each IDiagramHandlerTab to the currently active 
        /// IDiagramTab.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void AttachToActiveDiagram()
        {
            Trace.WriteLine("Attaching to active diagram");

            if (myDiagram != null)
            {
                if (myDiagram != myToolStripContainer.Diagram)
                {
                    Trace.WriteLine("Attaching diagram to tool strip container");
                    myToolStripContainer.Diagram = myDiagram;
                }

                // Now, loop through all IDiagramHandlerTab's and attach
                // them to the active diagram.
                IDiagramHandlerTab diagramHandler;
                foreach (IDockContent form in myDockPanel.Contents)
                {
                    if (form is IDiagramHandlerTab)
                    {
                        diagramHandler = form as IDiagramHandlerTab;
                        if (diagramHandler.Diagram != myDiagram)
                        {
                            Trace.WriteLine("Attaching diagram to tab " +
                                form.DockHandler.TabText);
                            diagramHandler.Diagram = myDiagram;
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
            SizeDiagramWindow();
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
        public int NewDiagram()
        {
            string name = "NewCerebrumDesign";
            return NewDiagram(name);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a new DiagramTab using the specified name.
        /// </summary>
        /// <param name="name">string</param>
        /// <returns>int: The number of IDiagramTabs.</returns>
        // ------------------------------------------------------------------
        public int NewDiagram(string name)
        {
            RemoveDiagram();

            myDiagram = new DiagramControl();
            ((System.ComponentModel.ISupportInitialize)(myDiagram)).BeginInit();
            this.SuspendLayout();
            int DiagramSize = 8000;

            myDiagram.AllowDrop = true;
            myDiagram.AutoScroll = true;
            myDiagram.BackColor = System.Drawing.Color.White;
            myDiagram.BackgroundType = Netron.Diagramming.Core.CanvasBackgroundTypes.FlatColor;
            myDiagram.Dock = System.Windows.Forms.DockStyle.Right;
            myDiagram.Document = new Document();
            myDiagram.EnableAddConnection = true;
            myDiagram.FileName = "";
            myDiagram.Magnification = new System.Drawing.SizeF(20F, 20F);
            myDiagram.Name = name;
            myDiagram.Origin = new System.Drawing.Point(100, 100);
            System.Drawing.Printing.PageSettings pg = new System.Drawing.Printing.PageSettings();
            pg.PaperSize = new System.Drawing.Printing.PaperSize("CerebrumProjectDiagram", DiagramSize, DiagramSize);

            myDiagram.PageSettings = pg;

            myDiagram.ShowConnectors = true;
            myDiagram.ShowGrid = false;
            myDiagram.ShowRulers = false;
            myDiagram.TabIndex = 0;
            myDiagram.Text = "diagramControl1";
            myDiagram.Visible = true;

            myDiagram.KeyUp += new KeyEventHandler(myDiagram_KeyUp);
            myDockPanel.Controls.Add(myDiagram);

            ((System.ComponentModel.ISupportInitialize)(myDiagram)).EndInit();
            this.ResumeLayout(false);

            LibraryTab.Diagram = myDiagram;
            ExplorerTab.Diagram = myDiagram;
            myToolStripContainer.Diagram = myDiagram;
            SizeDiagramWindow();
            return 1;
        }

        /// <summary>
        /// Detaches the diagram from internal tool windows
        /// </summary>
        public void RemoveDiagram()
        {
            ClearLibrary();
            HideLibrary();
            if (myDiagram != null)
            {
                if (myDockPanel.Controls.Contains(myDiagram))
                {
                    myDockPanel.Controls.Remove(myDiagram);
                }
            }
        }
        private void SizeDiagramWindow()
        {
            if (myDiagram == null)
                return;

            Point topLeft = new Point(0, 0);
            Size szDiagram = new Size(this.ClientRectangle.Width, this.ClientRectangle.Height);

            if ((LibraryTab.Visible) &&
                (!LibraryTab.VisibleState.ToString().Contains("AutoHide")) &&
                (!LibraryTab.VisibleState.ToString().Contains("Float")))
                topLeft.X = LibraryTab.Width;

            szDiagram.Width = szDiagram.Width - topLeft.X;
            szDiagram.Height = szDiagram.Height - topLeft.Y;

            myDiagram.Location = topLeft;
            myDiagram.Size = szDiagram;
        }
        /// <summary>
        /// Shows the LibraryTab.
        /// </summary>
        public void ShowLibrary()
        {
            LibraryTab.Show(myDockPanel);
        }
        /// <summary>
        /// Hides the LibraryTab.
        /// </summary>
        public void HideLibrary()
        {
            LibraryTab.Hide();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Shows the ExplorerTab.
        /// </summary>
        // ------------------------------------------------------------------
        public void ShowExplorer()
        {
            //ExplorerTab.Show(myDockPanel);
            ExplorerTab.Hide();
        }
        #endregion

        /// <summary>
        /// Loads the specified XML definition file into the Library toolbox
        /// </summary>
        /// <param name="path">The path to the core definition file</param>
        public void LoadCoreDefinition(string path)
        {
            string LibraryName = "Cerebrum Cores";
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(path);
                LibraryName = CerebrumXmlInterface.GetXmlAttribute(xDoc, "CerebrumCore.Software.DesignDisplay.Category", "Name", true);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception caught in CerebrumProjectPanel.LoadCoreDefinition()");
                Trace.WriteLine(ex.Message);
                return;
            }
            LibraryTab.LoadCoreDefinition(LibraryName, path);
        }
        /// <summary>
        /// Clears all libraries and tools from the library toolbox
        /// </summary>
        public void ClearLibrary()
        {
            LibraryTab.Clear();
        }
        /// <summary>
        /// Scans the specified path for core definitions to be loaded into the library toolbox.
        /// </summary>
        /// <param name="RepoPath">The path a repository within which to search for core definitions</param>
        public void ScanRepository(string RepoPath)
        {
            if (!Directory.Exists(RepoPath))
            {
                myMessages.RaiseMessageEvent(MessageEventType.Warning, "Core Library", String.Format("Unable to locate core path: {0}", RepoPath), "Core Library");
                return;
            }
            // Get a list of directories in the Repo Path
            foreach (string Dir in Directory.GetDirectories(RepoPath))
            {
                DirectoryInfo di = new DirectoryInfo(Dir);
                // Get all *.XML files in the directory
                foreach (FileInfo fi in di.GetFiles("*.xml", SearchOption.TopDirectoryOnly))
                {
                    // Look for one that corresponds to the directory name
                    if (fi.Name == String.Format("{0}.xml", di.Name))
                    {
                        // Try to load the core definition and break out
                        LoadCoreDefinition(fi.FullName);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Manually assigns edges of a connection to the specified connectors to ensure both ends are connected
        /// </summary>
        /// <param name="c">The connection to validate</param>
        /// <param name="FromCC">The input connector to attach to the connection</param>
        /// <param name="ToCC">The output connector to attach to the connection</param>
        public void AttachConnectors(CerebrumConnection c, CoreConnector FromCC, CoreConnector ToCC)
        {
            if (c.From.AttachedTo != null)
                c.From.DetachConnector(c.From);
            c.From.Point = new Point(FromCC.Center.X, FromCC.Center.Y);
            c.From.AttachTo(FromCC);

            if (c.To.AttachedTo != null)
                c.To.DetachConnector(c.To);
            c.To.Point = new Point(ToCC.Center.X, ToCC.Center.Y);
            c.To.AttachTo(ToCC);

            c.Invalidate();
        }
        /// <summary>
        /// Create a new connection between two connector ports
        /// </summary>
        /// <param name="SourcePort">The input connector to attach to the connection</param>
        /// <param name="SinkPort">The output connector to attach to the connection</param>
        public void CreateConnection(CoreConnector SourcePort, CoreConnector SinkPort)
        {
            CerebrumConnection c = new CerebrumConnection(SourcePort.Center, SinkPort.Center);
            myDiagram.AddConnection(c);
            AttachConnectors(c, SourcePort, SinkPort);
        }

        MessageEventController myMessages;
        /// <summary>
        /// Attach the specified MessageEventController to this control, allowing it to propagate messages to the GUI
        /// </summary>
        /// <param name="EventController"></param>
        public void AttachMessageController(MessageEventController EventController)
        {
            myMessages = EventController;
        }

        /// <summary>
        /// Expands all collapsible library tabs
        /// </summary>
        public void ExpandAll()
        {
            LibraryTab.ExpandAll();
        }
        /// <summary>
        /// Collapses all collapsible library tabs
        /// </summary>
        public void CollapseAll()
        {
            LibraryTab.CollapseAll();
        }

        void OnCoreError(CerebrumCore Core, string Message)
        {
            myMessages.RaiseMessageEvent(MessageEventType.Error, "Core Error", Message, String.Format("{0}:{1}", Core.CoreType, Core.CoreInstance));
        }
    }
}
