using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A ToolStripContainer that has all default diagramming ToolStrips.
    /// </summary>
    // ----------------------------------------------------------------------
    public class DiagramToolStripContainer : ToolStripContainer
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// The diagram to perform actions on.
        /// </summary>
        // ------------------------------------------------------------------
        protected DiagramControl myDiagram;

        protected DiagramMainToolStrip myMainToolStip;
        protected ActionToolStrip myActionToolStrip;
        protected DiagramMenuStrip myDiagramMenuStrip;
        protected DiagramStatusStrip myDiagramStatusStrip;
        protected ViewToolStrip myViewToolStrip;
        protected FormatToolStrip myFormatToolStrip;
        protected DrawingToolStrip myDrawingToolStrip;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the DiagramControl to perform actions on.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramControl Diagram
        {
            get
            {
                return myDiagram;
            }
            set
            {
                myDiagram = value;

                this.myMainToolStip.Diagram = this.myDiagram;
                this.myActionToolStrip.Diagram = this.myDiagram;
                this.myDiagramMenuStrip.Diagram = this.myDiagram;
                this.myDiagramStatusStrip.Diagram = this.myDiagram;
                this.myViewToolStrip.Diagram = this.myDiagram;
                this.myFormatToolStrip.Diagram = this.myDiagram;
                this.myDrawingToolStrip.Diagram = this.myDiagram;

            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramToolStripContainer()
            : base()
        {
            Initialize();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Intializes all tool strips and adds them to this container.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void Initialize()
        {
            // Create all tool strips and attach our diagram to them.
            myMainToolStip = new DiagramMainToolStrip();
            myActionToolStrip = new ActionToolStrip();
            myDiagramMenuStrip = new DiagramMenuStrip();
            myDiagramStatusStrip = new DiagramStatusStrip();
            myViewToolStrip = new ViewToolStrip();
            myFormatToolStrip = new FormatToolStrip();
            myDrawingToolStrip = new DrawingToolStrip();
            
            TopToolStripPanel.Controls.Clear();
            TopToolStripPanel.Controls.Add(myDrawingToolStrip);     // Selection and Connector Tools
            TopToolStripPanel.Controls.Add(myMainToolStip);         // Undo/Redo, Zoom & View Buttons
            TopToolStripPanel.Controls.Add(myActionToolStrip);      // Alignment Tools
            //TopToolStripPanel.Controls.Add(myViewToolStrip);
            //TopToolStripPanel.Controls.Add(myFormatToolStrip);
            //TopToolStripPanel.Controls.Add(myDiagramMenuStrip);

            //BottomToolStripPanel.Controls.Add(myDiagramStatusStrip);
        }

        // Added by Matthew Cotter 1/27/2011 to facilitate adding buttons to the Action toolbar with event handlers outside this project.
        public void AddActionButton(string Text, System.Drawing.Image ButtonImage, EventHandler OnClickHandler)
        {
            myActionToolStrip.AddCustomButton(Text, ButtonImage, OnClickHandler);
        }
    }
}
