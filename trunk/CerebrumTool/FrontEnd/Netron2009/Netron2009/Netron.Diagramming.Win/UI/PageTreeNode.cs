using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Netron.Diagramming.Core;

namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A TreeNode that displays the ILayers and IShapes in an IPage.  This
    /// is the root node and it's Text property is set to the IPage's Name.
    /// The IPage that belongs to this TreeNode can be accessed by the 
    /// property 'Page'.  The structure is as follows:
    ///     * Page Node (this TreeNode)
    ///             * Shapes Node
    ///                     * Shape1
    ///                     * Shape2
    ///                         .
    ///                         .
    ///                         .
    ///                     * ShapeN
    ///             * Layers Node
    ///                     * Layer1
    ///                     * Layer2
    ///                         .
    ///                         .
    ///                         .
    ///                     * LayerN
    /// </summary>
    // ----------------------------------------------------------------------
    public class PageTreeNode : DiagramTreeNodeBase
    {
        #region Fields

        // ------------------------------------------------------------------
        /// <summary>
        /// The IPage to display all layers and shapes for.
        /// </summary>
        // ------------------------------------------------------------------
        IPage myPage;

        // ------------------------------------------------------------------
        /// <summary>
        /// The folder that has all layers listed under it.
        /// </summary>
        // ------------------------------------------------------------------
        TreeNode myLayersFolder;

        #endregion

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the IPage to display the ILayers and IShapes for.
        /// The nodes for the layers and shapes are cleared and re-loaded
        /// when used as a setter.
        /// </summary>
        // ------------------------------------------------------------------
        public IPage Page
        {
            get
            {
                return this.myPage;
            }
            set
            {
                if (value != null)
                {
                    this.myPage = value;
                    this.Text = myPage.Name;        
            
                    // Add a folder for the layers.
                    myLayersFolder = new TreeNode("Layers");
                    Nodes.Add(myLayersFolder);
                    foreach (ILayer layer in myPage.Layers)
                    {
                        LayerTreeNode tn = new LayerTreeNode(layer);
                        myLayersFolder.Nodes.Add(tn);
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the diagram attached to this tree node.  
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
                myDiagram.Controller.Model.OnCurrentPageChanged +=
                    new CurrentPageChangedEventHandler(
                    HandleCurrentPageChanged);
            }
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Default constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public PageTreeNode()
            : base()
        {
            BuildMenu();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor that receives the IPage.
        /// </summary>
        /// <param name="page">IPage</param>
        // ------------------------------------------------------------------
        public PageTreeNode(IPage page)
            : base()
        {
            // Use the property so the layers and shapes get loaded.
            Page = page;            
            BuildMenu();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Builds the ContextMenuStrip that allows the user to edit our
        /// page's properties.
        /// </summary>
        // ------------------------------------------------------------------
        protected void BuildMenu()
        {
            // Build our custom ContextMenuStrip that get's displayed when
            // the user right-clicks on this node.
            ContextMenuStrip = new ContextMenuStrip();

            // Allows the user to change the settings of this page (such as
            // page name, color, etc.).
            ToolStripMenuItem pageSetupMenu = new ToolStripMenuItem();
            pageSetupMenu.Text = "Page Setup...";
            pageSetupMenu.Click += new EventHandler(HandlePageSetupMenuClick);
            ContextMenuStrip.Items.Add(pageSetupMenu);

            // Allows the user to change the settings of this page (such as
            // page name, color, etc.).
            ToolStripMenuItem deleteMenu = new ToolStripMenuItem();
            deleteMenu.Text = "Delete Page";
            deleteMenu.Image = Images.Delete;
            deleteMenu.Click += new EventHandler(HandleDeleteMenuClick);
            ContextMenuStrip.Items.Add(deleteMenu);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// When the current page has changed in the diagram, if the new 
        /// current page is our page, then set this node as the selected
        /// node in the TreeView.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">PageEventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleCurrentPageChanged(
            object sender, 
            PageEventArgs e)
        {
            if (e.Page == myPage)
            {
                this.TreeView.SelectedNode = this;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the ToolStripMenuItem that allows the user to
        /// delete this page is clicked.  This calls method 'Delete()'.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleDeleteMenuClick(
            object sender, 
            EventArgs e)
        {
            Delete();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the ToolStripMenuItem that allows the user to
        /// edit this page's setup is clicked.  This method calls
        /// 'ShowPageSetup()'.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandlePageSetupMenuClick(
            object sender, 
            EventArgs e)
        {
            ShowPageSetup();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Removes our page from the Model IF this page is not the default 
        /// page.  The default page cannot be deleted.  We don't have to
        /// remove this node from the PageListTreeNode - it monitors the
        /// model's collection of pages and adds/removes nodes when the
        /// collection is changed.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void Delete()
        {
            IModel model = Page.Model;
            model.RemovePage(Page, true);  // Allow warnings.
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Displays a PageSetupDialog so the user can edit the page name
        /// and color.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void ShowPageSetup()
        {
            if (myPage == null)
            {
                return;
            }

            PageSetupDialog dialog = new PageSetupDialog();
            dialog.PageColor = myPage.Ambience.BackgroundColor;
            dialog.PageName = myPage.Name;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                myPage.Name = dialog.PageName;
                myPage.Ambience.BackgroundColor = dialog.PageColor;

                // Update our text to reflect the changed name.
                Text = myPage.Name;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Occurs when a mouse button is pressed on this node, if the left
        /// mouse button was pressed once, then set the current page being 
        /// viewed in the diagram to this page (if it's not already).
        /// </summary>
        /// <param name="e"></param>
        // ------------------------------------------------------------------
        public override void OnMouseDown(TreeNodeMouseClickEventArgs e)
        {
            base.OnMouseDown(e);

            // Do nothing if we aren't attached to a diagram.
            if (Diagram == null)
            {
                return;
            }

            if ((e.Button == MouseButtons.Left) &&
                (e.Clicks == 1))
            {
                //PageTreeNode node = e.Node as PageTreeNode;
                //IPage page = node.Page;
                if (Page != Diagram.Controller.Model.CurrentPage)
                {
                    Diagram.Controller.DeactivateAllTools();
                    Diagram.Controller.View.HideTracker();  // Just in case
                    // there are selected items.
                    Diagram.Controller.Model.SetCurrentPage(Page);
                    Diagram.Controller.View.Invalidate();
                }
            }
        }
    }
}
