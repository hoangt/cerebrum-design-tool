using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Netron.Diagramming.Core;
using System.Drawing;

namespace Netron.Diagramming.Win
{
    // ------------------------------------------------------------------
    /// <summary>
    /// A TreeView that show's a DiagramControl's pages and layers.
    /// </summary>
    // ------------------------------------------------------------------
    public class DiagramExplorer : TreeView
    {
        #region Fields

        DiagramControl diagram;
        TreeNode rootNode = new TreeNode("Diagram");
        PageListTreeNode pagesNode;
        ToolTip myToolTip = new ToolTip();
        int myClosedFolderImageIndex = 0;
        int myOpenFolderImageIndex = 1;
        int myPageImageIndex = 2;

        #endregion

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the index of the closed folder image in the ImageList.
        /// </summary>
        // ------------------------------------------------------------------
        public int ClosedFolderImageIndex
        {
            get
            {
                return myClosedFolderImageIndex;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the index of the open folder image in the ImageList.
        /// </summary>
        // ------------------------------------------------------------------
        public int OpenFolderImageIndex
        {
            get
            {
                return myOpenFolderImageIndex;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the index of the page image in the ImageList.
        /// </summary>
        // ------------------------------------------------------------------
        public int PageImageIndex
        {
            get
            {
                return myPageImageIndex;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the ToolTip associated with this TreeView.
        /// </summary>
        // ------------------------------------------------------------------
        public ToolTip ToolTip
        {
            get
            {
                return myToolTip;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the diagram.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual DiagramControl Diagram
        {
            get
            {
                return diagram;
            }
            set
            {
                if (value != null)
                {
                    diagram = value;
                    OnNewDiagram();
                    diagram.OnDiagramOpened +=
                        new EventHandler<FileEventArgs>(HandleOnDiagramOpened);
                    diagram.OnNewDiagram +=
                        new EventHandler(HandleOnNewDiagram);
                }
            }
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramExplorer()
            : base()
        {
            // Add the images used by the TreeNode's.  Make sure to change
            // the image index reference fields if the order below is
            // changed (the 'myClosedFolderImageIndex', 
            // 'myOpenFolderImageIndex', etc.).
            this.ImageList = new ImageList();
            this.ImageList.Images.Add(Images.ClosedFolder);

            this.ImageList.Images.Add(Images.OpenedFolder);

            this.ImageList.Images.Add(Images.Page);
            ImageList.Images.Add(Images.Schema);

            Nodes.Add(rootNode);
            rootNode.ImageIndex = 0;
            rootNode.SelectedImageIndex = 1;

            this.ShowNodeToolTips = true;
            this.ShowPlusMinus = true;
            this.ShowRootLines = true;
            this.HideSelection = false;
            
            this.NodeMouseClick +=
                new TreeNodeMouseClickEventHandler(HandleNodeMouseClick);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called when a new diagram is attached.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void OnNewDiagram()
        {
            if (pagesNode == null)
            {
                this.pagesNode =
                    new PageListTreeNode();
                this.pagesNode.ImageIndex = ClosedFolderImageIndex;
                this.pagesNode.SelectedImageIndex = OpenFolderImageIndex;
                this.pagesNode.PageNodeImageIndex = PageImageIndex;
                rootNode.Nodes.Add(pagesNode);
            }

            pagesNode.Diagram = diagram;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called when a new diagram is loaded from disk.  Method 
        /// 'Initialize()' is called to create a new tree based on the new 
        /// diagram.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">FileEventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleOnDiagramOpened(
            object sender, 
            FileEventArgs e)
        {
            OnNewDiagram();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called when a new diagram is created.  Method 'Initialize()' is
        /// called to create a new tree based on the new diagram.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleOnNewDiagram(
            object sender, 
            EventArgs e)
        {
            OnNewDiagram();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when a Node in the TreeView is clicked on.  The
        /// following actions are performed:
        ///     * If the left mouse button was clicked once then method
        ///       'HandleNodeLeftMouseSingleClick' is called.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">TreeNodeMouseClickEventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleNodeMouseClick(
            object sender, 
            TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is DiagramTreeNodeBase)
            {
                (e.Node as DiagramTreeNodeBase).OnMouseDown(e);
            }

            if ((e.Clicks == 1) && (e.Button == MouseButtons.Right))
            {
                HandleNodeRightMouseSingleClick(sender, e);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when a Node in the TreeView is clicked on once with
        /// the RIGHT mouse button - displays the context menu.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">TreeNodeMouseClickEventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleNodeRightMouseSingleClick(
            object sender,
            TreeNodeMouseClickEventArgs e)
        {
            // Show the nodes ContextMenuStrip if it has one.
            if (e.Node.ContextMenuStrip != null)
            {
                e.Node.ContextMenuStrip.Show(e.Location);
            }
        }
    }
}
