using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Netron.Diagramming.Core;

namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A TreeNode that represents an ILayer in an IPage of a Document.
    /// </summary>
    // ----------------------------------------------------------------------
    public class LayerTreeNode : DiagramTreeNodeBase
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// The layer we're representing.
        /// </summary>
        // ------------------------------------------------------------------
        protected ILayer myLayer;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the ILayer this TreeNode represents.
        /// </summary>
        // ------------------------------------------------------------------
        public ILayer Layer
        {
            get
            {
                return myLayer;
            }
            set
            {
                myLayer = value;
                Name = myLayer.Name;
                Text = myLayer.Name;
                this.Checked = myLayer.IsVisible;                
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Default constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public LayerTreeNode()
            : base()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor that receives the layer we're attached to.
        /// </summary>
        /// <param name="layer">ILayer</param>
        // ------------------------------------------------------------------
        public LayerTreeNode(ILayer layer)
        {
            // Use the 'Layer' property so this node gets initialized properly.
            Layer = layer;
        }        
    }
}
