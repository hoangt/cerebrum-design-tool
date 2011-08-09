using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Netron.Diagramming.Core;

namespace Netron.Diagramming.Win
{
    public class DiagramBaseToolStrip : ToolStrip
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// The diagram control to perform actions on.
        /// </summary>
        // ------------------------------------------------------------------
        protected DiagramControl diagramControl;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the diagram control to draw on.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual DiagramControl Diagram
        {
            get
            {
                return this.diagramControl;
            }
            set
            {
                if (value != null)
                {
                    this.diagramControl = value;

                    // Inform the others a new diagram was set.
                    this.OnNewDiagram();
                }
                else
                {
                    this.DisableAllItems();
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramBaseToolStrip()
            : base()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Enables all items in the tool strip.
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
        /// Disables all items in the tool strip.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual void DisableAllItems()
        {
            foreach (ToolStripItem item in this.Items)
            {
                item.Enabled = false;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called when a new diagram is attached to this ToolStrip.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void OnNewDiagram()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the 'Checked' property to false for all ToolStripButtons.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void UnCheckAll()
        {
            foreach (ToolStripItem item in this.Items)
            {
                if (item is ToolStripButton)
                {
                    (item as ToolStripButton).Checked = false;
                }
            }
        }
    }
}
