using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Netron.Diagramming.Core;

namespace Netron.Diagramming.Win
{
    public abstract class DiagramTreeNodeBase : TreeNode
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// The current diagram.
        /// </summary>
        // ------------------------------------------------------------------
        protected IDiagramControl myDiagram;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the current diagram.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual IDiagramControl Diagram
        {
            get
            {
                return myDiagram;
            }
            set
            {
                myDiagram = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Default constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramTreeNodeBase()
            : base()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Occurs when a mouse button is pressed on this node.
        /// </summary>
        /// <param name="e">TreeNodeMouseClickEventArgs</param>
        // ------------------------------------------------------------------
        public virtual void OnMouseDown(TreeNodeMouseClickEventArgs e)
        {
        }
    }
}
