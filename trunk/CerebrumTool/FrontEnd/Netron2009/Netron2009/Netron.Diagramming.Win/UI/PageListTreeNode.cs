using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Netron.Diagramming.Core;
using System.Drawing;

namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A TreeNode that contains a sub-node for each page in the collection
    /// supplied.
    /// </summary>
    // ----------------------------------------------------------------------
    public class PageListTreeNode : DiagramTreeNodeBase
    {
        #region Fields

        // ------------------------------------------------------------------
        /// <summary>
        /// The index of the image to use for a PageTreeNode (used for the
        /// selected and not selected states).
        /// </summary>
        // ------------------------------------------------------------------
        int myPageNodeImageIndex = 2;

        #endregion

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the index of the image to use for a PageTreeNode 
        /// (used for the selected and not selected states).
        /// </summary>
        // ------------------------------------------------------------------
        public int PageNodeImageIndex
        {
            get
            {
                return myPageNodeImageIndex;
            }
            set
            {
                myPageNodeImageIndex = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the current diagram.  A current list is cleared and
        /// a PageTreeNode is added for each page in the diagram.
        /// </summary>
        // ------------------------------------------------------------------
        public override IDiagramControl Diagram
        {
            get
            {
                return base.Diagram;
            }
            set
            {
                base.Diagram = value;

                // Clear all nodes in this node and add a PageTreeNode for
                // each IPage in the collection.
                Nodes.Clear();
                foreach (IPage page in Diagram.Document.Model.Pages)
                {
                    PageTreeNode node = new PageTreeNode(page);
                    node.Diagram = Diagram;
                    node.SelectedImageIndex = PageNodeImageIndex;
                    node.ImageIndex = PageNodeImageIndex;
                    Nodes.Add(node);
                }

                // Monitor the pages collection so we can add/remove page nodes
                // when the collection is changed.
                this.Diagram.Controller.Model.Pages.OnItemAdded +=
                    new EventHandler<CollectionEventArgs<IPage>>(Pages_OnItemAdded);

                this.Diagram.Controller.Model.Pages.OnItemRemoved +=
                    new EventHandler<CollectionEventArgs<IPage>>(Pages_OnItemRemoved);
            }
        }

        #endregion

        #region Constructors

        // ------------------------------------------------------------------
        /// <summary>
        /// Default constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public PageListTreeNode()
            : base()
        {
            Text = "Pages";          

            // Build the ContextMenuStip that gets displayed when this node
            // is right-clicked on.
            this.ContextMenuStrip = new ContextMenuStrip();

            // Allows the user to add a new page.
            ToolStripMenuItem mnuAddPage = new ToolStripMenuItem("Add Page");
            mnuAddPage.Click += new EventHandler(HandleAddPageMenuClick);
            this.ContextMenuStrip.Items.Add(mnuAddPage);
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Removes the PageTreeNode the removed page item was attached to.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">CollectionEventArgs<IPage></param>
        // ------------------------------------------------------------------
        void Pages_OnItemRemoved(object sender, CollectionEventArgs<IPage> e)
        {
            foreach (PageTreeNode node in this.Nodes)
            {
                if (node.Page == e.Item)
                {
                    node.Remove();
                    this.TreeView.Invalidate();
                    break;
                }
            }

            // Now select the node for the page that's the (new) current
            // page.
            foreach (PageTreeNode node in this.Nodes)
            {
                if (node.Page == Diagram.Controller.Model.CurrentPage)
                {
                    this.TreeView.SelectedNode = node;
                    break;
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a PageTreeNode to this node when a page is added to the
        /// diagram.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">CollectionEventArgs<IPage></param>
        // ------------------------------------------------------------------
        void Pages_OnItemAdded(object sender, CollectionEventArgs<IPage> e)
        {
            // I don't see it happening, but just in case a null item
            // is added do nothing.
            if (e.Item == null)
            {
                return;
            }

            PageTreeNode node = new PageTreeNode(e.Item);
            node.Diagram = Diagram;
            node.ImageIndex = PageNodeImageIndex;
            node.SelectedImageIndex = PageNodeImageIndex;
            Nodes.Add(node);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called when the ToolStripMenuItem for adding an IPage to the
        /// diagram is clicked.  Method 'AddPage()' is called.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleAddPageMenuClick(
            object sender,
            EventArgs e)
        {
            AddPage();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a new IPage to the diagram AND adds a node to this node for
        /// the new IPage.  The PageSetupDialog is shown so the user can
        /// specify the new page name, color, and to cancel if desired.
        /// </summary>
        /// <returns>IPage: The page added.  If the page was not added
        /// successfully, then 'null' is returned.</returns>
        // ------------------------------------------------------------------
        public virtual IPage AddPage()
        {
            string pageName = "Page" + 
                (Diagram.Controller.Model.Pages.Count + 1).ToString();
            Color pageColor = Color.White;
            Page page = null;

            // Show the PageSetupDialog so the user can specify the name and
            // color of the new page.
            PageSetupDialog dialog = new PageSetupDialog();
            dialog.PageName = pageName;
            dialog.PageColor = pageColor;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                page = new Page(dialog.PageName, Diagram.Controller.Model);
                page.Ambience.PageColor = dialog.PageColor;
                page.Ambience.PageBackgroundType = CanvasBackgroundTypes.FlatColor;
                AddPage(page);
            }

            return page;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds the specified page to the Model.  A new page node is not
        /// added here - that's done by monitoring the pages collection
        /// so a new page node gets added even if the new page isn't added
        /// from here.
        /// </summary>
        /// <param name="page">IPage</param>
        // ------------------------------------------------------------------
        protected virtual void AddPage(IPage page)
        {
            Diagram.Controller.Model.AddPage(page);
            Diagram.Controller.Model.SetCurrentPage(page);
        }
    }
}
