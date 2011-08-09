using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using WeifenLuo.WinFormsUI.Docking;

using Netron.Diagramming.Core;
using Netron.Diagramming.Win;
using System.Diagnostics;

namespace NetronProject
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A dockable Form that hosts a "DiagramExplorer".  You have to attach
    /// this to a DiagramControl by using property "Diagram"!
    /// </summary>
    // ----------------------------------------------------------------------
    public class ExplorerTab : DockContent, IDiagramHandlerTab
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// The TreeView that shows a diagram's pages and layers.
        /// </summary>
        // ------------------------------------------------------------------
        protected DiagramExplorer myExplorer;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the DiagramControl the explorer is attached to.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramControl Diagram
        {
            get
            {
                return myExplorer.Diagram;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                myExplorer.Diagram = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public ExplorerTab()
            : base()
        {
            InitializeComponent();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Initializes the Form.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void InitializeComponent()
        {
            // The explorer.
            myExplorer = new DiagramExplorer();
            Controls.Add(myExplorer);
            myExplorer.Dock = DockStyle.Fill;

            this.SuspendLayout();
            // 
            // ExplorerTab
            // 
            this.ClientSize = new System.Drawing.Size(292, 574);
            this.DockAreas = ((DockAreas)((((DockAreas.Float | 
                DockAreas.DockLeft) | 
                DockAreas.DockRight) | 
                DockAreas.Document)));

            this.HideOnClose = true;
            this.Name = "ExplorerTab";
            this.ShowHint = DockState.DockLeft;
            this.TabText = "Explorer";
            this.Text = "Explorer";
            this.BackColor = Color.WhiteSmoke;
            this.ResumeLayout(false);
        }
    }
}
