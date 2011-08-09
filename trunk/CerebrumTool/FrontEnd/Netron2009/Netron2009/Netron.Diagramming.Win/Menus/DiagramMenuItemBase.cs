using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Netron.Diagramming.Core;

namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// Abstract, base class for all diagram ToolStripMenuItems.
    /// </summary>
    // ----------------------------------------------------------------------
    public abstract class DiagramMenuItemBase : ToolStripMenuItem
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// The diagram to perform actions on.
        /// </summary>
        // ------------------------------------------------------------------
        protected IDiagramControl myDiagram;

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
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramMenuItemBase()
            : base()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when a new IDiagram is attached.  The default here
        /// is all drop-down items are enabled if the new diagram is not
        /// 'null'.  Otherwise all items are disabled.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void OnNewDiagram()
        {
            if (this.myDiagram != null)
            {
                this.EnableAllDropDownItems();
            }
            else
            {
                this.DisableAllDropDownItems();
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Enables all drop-down items in this MenuStrip.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual void EnableAllDropDownItems()
        {
            foreach (ToolStripItem item in this.DropDownItems)
            {
                item.Enabled = true;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Disables all drop-down items in this MenuStrip.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual void DisableAllDropDownItems()
        {
            foreach (ToolStripItem item in this.DropDownItems)
            {
                item.Enabled = false;
            }
        }
    }
}
