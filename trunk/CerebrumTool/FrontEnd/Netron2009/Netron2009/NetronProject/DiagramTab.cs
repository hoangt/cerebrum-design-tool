using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using WeifenLuo.WinFormsUI.Docking;

using Netron.Diagramming.Core;
using Netron.Diagramming.Win;

namespace NetronProject
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A dockable Form that hosts a DiagramControl.
    /// </summary>
    // ----------------------------------------------------------------------
    public class DiagramTab : DockContent, IDiagramTab
    {
        protected DiagramControl myDiagram;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the diagram this dockable form hosts.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramControl Diagram
        {
            get
            {
                return myDiagram;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Default constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramTab()
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiagramTab));
            this.myDiagram = new Netron.Diagramming.Win.DiagramControl();
            ((System.ComponentModel.ISupportInitialize)(this.myDiagram)).BeginInit();
            this.SuspendLayout();
            // 
            // myDiagram
            // 
            this.myDiagram.AllowDrop = true;
            this.myDiagram.AutoScroll = true;
            this.myDiagram.BackColor = System.Drawing.Color.DarkGray;
            this.myDiagram.BackgroundType = Netron.Diagramming.Core.CanvasBackgroundTypes.FlatColor;
            this.myDiagram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myDiagram.Document = ((Netron.Diagramming.Core.Document)(resources.GetObject("myDiagram.Document")));
            this.myDiagram.EnableAddConnection = true;
            this.myDiagram.FileName = "";
            this.myDiagram.Location = new System.Drawing.Point(0, 0);
            this.myDiagram.Magnification = new System.Drawing.SizeF(69F, 69F);
            this.myDiagram.Name = "myDiagram";
            this.myDiagram.Origin = new System.Drawing.Point(0, 0);
            this.myDiagram.PageSettings = ((System.Drawing.Printing.PageSettings)(resources.GetObject("myDiagram.PageSettings")));
            this.myDiagram.ShowConnectors = true;
            this.myDiagram.ShowGrid = false;
            this.myDiagram.ShowRulers = false;
            this.myDiagram.Size = new System.Drawing.Size(865, 673);
            this.myDiagram.TabIndex = 0;
            this.myDiagram.Text = "diagramControl1";
            // 
            // DiagramTab
            //             
            this.ClientSize = new System.Drawing.Size(865, 673);
            this.Controls.Add(this.myDiagram);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Name = "DiagramTab";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            ((System.ComponentModel.ISupportInitialize)(this.myDiagram)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
