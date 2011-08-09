using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Netron.Diagramming.Core;

namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A MenuStrip that contains ToolStripMenuItems for peforming actions
    /// against a diagram.
    /// </summary>
    // ----------------------------------------------------------------------
    public class DiagramMenuStrip : MenuStrip
    {
        #region Fields

        // ------------------------------------------------------------------
        /// <summary>
        /// The diagram to perform actions on.
        /// </summary>
        // ------------------------------------------------------------------
        IDiagramControl myDiagram;

        // ------------------------------------------------------------------
        /// <summary>
        /// The "File" ToolStripMenuItem.
        /// </summary>
        // ------------------------------------------------------------------
        FileMenu myFileMenu = new FileMenu();

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the diagram to perform actions on.
        /// </summary>
        // ------------------------------------------------------------------
        public IDiagramControl Diagram
        {
            get
            {
                return this.myDiagram;
            }
            set
            {
                if (value != null)
                {
                    this.myDiagram = value;
                    this.OnNewDiagram();
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the 'File' menu item.
        /// </summary>
        // ------------------------------------------------------------------
        public FileMenu FileMenu
        {
            get
            {
                return myFileMenu;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramMenuStrip()
            : base()
        {
            AddMenuItems();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds all menu items.  The default items shipped with this control
        /// are:
        ///     * File menu
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void AddMenuItems()
        {
            this.Items.Add(this.myFileMenu);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called when a new diagram is attached.  The default here is
        /// the diagram is passed on to all DiagramMenuItemBase's.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void OnNewDiagram()
        {
            // Do nothing if the attached diagram isn't valid.
            if (this.myDiagram == null)
            {
                return;
            }

            foreach (ToolStripMenuItem item in this.Items)
            {
                if (item is DiagramMenuItemBase)
                {
                    (item as DiagramMenuItemBase).Diagram = this.myDiagram;
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Disables all ToolStripItems that belong to this MenuStrip.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual void EnableAllItems()
        {
            foreach (ToolStripItem item in this.Items)
            {
                item.Enabled = true;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Disables all ToolStripItems that belong to this MenuStrip.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual void DisableAllItems()
        {
            foreach (ToolStripItem item in this.Items)
            {
                item.Enabled = false;
            }
        }
    }
}
