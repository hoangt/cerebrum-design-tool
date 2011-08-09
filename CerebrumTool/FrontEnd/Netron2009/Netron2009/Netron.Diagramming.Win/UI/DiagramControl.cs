using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Collections;

using Netron.Diagramming.Core;

using ToolBox.Forms;

namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// The toolbox diagram control (aka surface control).
    /// </summary>
    // ----------------------------------------------------------------------
    [
    ToolboxBitmap(typeof(DiagramControl), "DiagramControl.bmp"),
    ToolboxItem(true),
    Description("Generic diagramming control for .Net"),
    Designer(typeof(Netron.Diagramming.Win.DiagramControlDesigner)),
    DefaultProperty("Name"),
    DefaultEvent("OnMouseDown")]
    public class DiagramControl : DiagramControlBase
    {
        #region Constants
        private const int WM_VSCROLL = 0x0115;
        private const int WM_HSCROLL = 0x0114;

        #endregion

        #region Events
       
        #endregion

        #region Fields

        // ------------------------------------------------------------------
        /// <summary>
        /// the tooltip control
        /// </summary>
        // ------------------------------------------------------------------
        public ToolTip toolTip;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DiagramControl"/> class.
        /// </summary>
        public DiagramControl() : base()
        {           

            #region double-buffering
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            #endregion
            if(!DesignMode)
            {

                // Init the MVC, see the Visio diagram for an overview of the 
                // instantiation proces.
                Controller = new Controller(this);

                // Create the view. This is not done in the base class 
                // because the view and the controller depend on the 
                // medium (web/win...)
                View = new View(this);

                // The diagram document is the total serializable package and 
                // contains in particular the model (which will be 
                // instantiated in the following line).
                Document = new Document();
                AttachToDocument(Document);
                Controller.View = View;
                TextEditor.Init(this);

                View.OnCursorChange += 
                    new EventHandler<CursorEventArgs>(mView_OnCursorChange);

                View.OnBackColorChange += 
                    new EventHandler<ColorEventArgs>(View_OnBackColorChange);

                //Controller.OnShowContextMenu +=
                //    new EventHandler<EntityMenuEventArgs>(
                //    Controller_OnShowContextMenu);

                this.AllowDrop = true;               
                
                this.toolTip = new ToolTip();
                toolTip.IsBalloon = true;
                toolTip.UseAnimation = true;
                toolTip.UseFading = true;
                toolTip.ToolTipIcon = ToolTipIcon.Info;
                toolTip.ToolTipTitle = "Info";
                toolTip.Active = false;
                toolTip.BackColor = Color.OrangeRed;
            }
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Raises the 
        /// <see cref="E:System.Windows.Forms.ScrollableControl.Scroll">
        /// </see> event.
        /// </summary>
        /// <param name="se">A 
        /// <see cref="T:System.Windows.Forms.ScrollEventArgs"></see> that 
        /// contains the event data.</param>
        // ------------------------------------------------------------------
        protected override void OnScroll(ScrollEventArgs se)
        {
            //base.OnScroll(se);
            if (se.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                Origin = new Point(se.NewValue, Origin.Y);
                //System.Diagnostics.Trace.WriteLine(se.NewValue);
            }
            else
            {
                Origin = new Point(Origin.X, se.NewValue);
                //System.Diagnostics.Trace.WriteLine(se.NewValue);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Handles the OnShowContextMenu event of the Controller control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The 
        /// <see cref="T:Netron.Diagramming.Core.EntityMenuEventArgs"/> 
        /// instance containing the event data.</param>
        // ------------------------------------------------------------------
        void Controller_OnShowContextMenu(object sender, EntityMenuEventArgs e)
        {
            // Get a point adjusted by the current scroll position and 
            // zoom factor.
            Point location = Point.Round(this.View.WorldToView(
                e.MouseEventArgs.Location));

            BuildMenu(location);
        }

        #region Methods
       
        void mView_OnCursorChange(object sender, CursorEventArgs e)
        {
           this.Cursor = e.Cursor;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Builds the context menu
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void BuildMenu(Point location)
        {
            ContextMenuStrip = new ContextMenuStrip();
            ContextMenuStrip.Items.Clear();

            ToolStripMenuItem mnuDelete = new ToolStripMenuItem("Delete");
            mnuDelete.Image = Images.Delete;
            mnuDelete.Click += new EventHandler(OnDelete);
            ContextMenuStrip.Items.Add(mnuDelete);
            mnuDelete.Enabled = false;

            ToolStripMenuItem mnuCut = new ToolStripMenuItem("Cut");
            mnuCut.Image = Images.Cut;
            mnuCut.Click += new EventHandler(OnCut);
            ContextMenuStrip.Items.Add(mnuCut);
            mnuCut.Enabled = false;

            ToolStripMenuItem mnuCopy = new ToolStripMenuItem("Copy");
            mnuCopy.Image = Images.Copy;
            mnuCopy.Click += new EventHandler(OnCopy);
            ContextMenuStrip.Items.Add(mnuCopy);
            mnuCopy.Enabled = false;

            ToolStripMenuItem mnuPaste = new ToolStripMenuItem("Paste");
            mnuPaste.Image = Images.Paste;
            mnuPaste.Click += new EventHandler(OnPaste);
            ContextMenuStrip.Items.Add(mnuPaste);
            mnuPaste.Enabled = true;

            if (Selection.SelectedItems.Count > 0)
            {
                mnuDelete.Enabled = true;
                mnuCut.Enabled = true;
                mnuCopy.Enabled = true;
            }

            //ContextMenuStrip.Items.Add(new ToolStripSeparator());

            //#region Selected Shape Menu

            //ToolStripMenuItem shapeMenu = new ToolStripMenuItem("Shape");
            //IShape shape = Controller.Model.GetShapeAt(location);

            //if ((shape != null) && (shape.Menu() != null))
            //{
            //    foreach (ToolStripItem item in shape.Menu())
            //    {
            //        shapeMenu.DropDownItems.Add(item);
            //    }
            //    ContextMenuStrip.Items.Add(shapeMenu);
            //    ContextMenuStrip.Items.Add(new ToolStripSeparator());
            //}

            //#endregion

            //#region Default shapes that can be added

            //ToolStripMenuItem mnuShapes = new ToolStripMenuItem("Add Shape");
            //ContextMenuStrip.Items.Add(mnuShapes);

            //ToolStripMenuItem mnuRecShape = new ToolStripMenuItem(
            //    "Rectangular");
            //mnuRecShape.Image = Images.DrawRectangle;
            //mnuRecShape.Click += new EventHandler(OnRecShape);
            //mnuShapes.DropDownItems.Add(mnuRecShape);

            //ToolStripMenuItem mnuEllipseShape = new ToolStripMenuItem(
            //    "Ellipse");
            //mnuEllipseShape.Image = Images.DrawEllipse;
            //mnuEllipseShape.Click += new EventHandler(OnEllipseShape);
            //mnuShapes.DropDownItems.Add(mnuEllipseShape);

            //ToolStripMenuItem mnuClassShape = new ToolStripMenuItem(
            //    "Class Shape");
            //mnuClassShape.Image = Images.ClassShape;
            //mnuClassShape.Click += new EventHandler(OnClassShape);
            //mnuShapes.DropDownItems.Add(mnuClassShape);

            //#endregion

            //#region Page Menus

            //ContextMenuStrip.Items.Add(new ToolStripSeparator());

            //ToolStripMenuItem mnuAddPage = new ToolStripMenuItem("Add Page");
            //mnuAddPage.Click += new EventHandler(OnAddPage);
            //ContextMenuStrip.Items.Add(mnuAddPage);

            //ToolStripMenuItem mnuDeleteCurrentPage =
            //    new ToolStripMenuItem("Delete Page");
            //mnuDeleteCurrentPage.Click +=
            //    new EventHandler(OnDeleteCurrentPage);
            //ContextMenuStrip.Items.Add(mnuDeleteCurrentPage);

            //#endregion

            //ContextMenuStrip.Items.Add(new ToolStripSeparator());

            //ToolStripMenuItem mnuProps = new ToolStripMenuItem("Properties");
            //mnuProps.Image = Images.Properties;
            //mnuProps.Click += new EventHandler(OnProperties);
            //ContextMenuStrip.Items.Add(mnuProps);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called when the menu item for deleting the current page in the 
        /// context menu strip is clicked.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        void OnDeleteCurrentPage(object sender, EventArgs e)
        {
            RemovePage(Controller.Model.CurrentPage, true);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called when the menu item for adding a page in the context
        /// menu strip is clicked.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        void OnAddPage(object sender, EventArgs e)
        {
            this.AddPage(true, true);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called on delete.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance 
        /// containing the event data.</param>
        // ------------------------------------------------------------------
        private void OnDelete(object sender, EventArgs e)
        {
            ActivateTool(Netron.Diagramming.Win.Controller.DeleteToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called on paste.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance 
        /// containing the event data.</param>
        // ------------------------------------------------------------------
        private void OnPaste(object sender, EventArgs e)
        {
            ActivateTool(ControllerBase.PasteToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called on cut.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance 
        /// containing the event data.</param>
        // ------------------------------------------------------------------
        private void OnCut(object sender, EventArgs e)
        {
            ActivateTool(ControllerBase.CutToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called on copy.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance 
        /// containing the event data.</param>
        // ------------------------------------------------------------------
        private void OnCopy(object sender, EventArgs e)
        {
            ActivateTool(Netron.Diagramming.Win.Controller.CopyToolName);
        }

        private void OnProperties(object sender, EventArgs e)
        {
            //this.RaiseOnShowCanvasProperties(
            //    new SelectionEventArgs(new object[] { this }));
            object[] itemsToShow;
            if (Selection.SelectedItems.Count > 0)
            {
                itemsToShow = Selection.SelectedItems.ToArray();
            }
            else
            {
                itemsToShow = new object[] {Document};
            }

            PropertiesForm.ShowPropertiesForm(itemsToShow);
        }

        private void OnRecShape(object sender, EventArgs e)
        {
            SimpleRectangle shape = new SimpleRectangle(); ;
            Point center = new Point(
                this.ClientRectangle.Left + (ClientRectangle.Width / 2),
                ClientRectangle.Top + (ClientRectangle.Height / 2));
            shape.Location = center;
            this.AddShape(shape);
            shape.Invalidate();
        }

        private void OnEllipseShape(object sender, EventArgs e)
        {
            SimpleEllipse shape = new SimpleEllipse();
            Point center = new Point(
                this.ClientRectangle.Left + (ClientRectangle.Width / 2),
                ClientRectangle.Top + (ClientRectangle.Height / 2));
            shape.Location = center;
            this.AddShape(shape);
            shape.Invalidate();
        }

        private void OnClassShape(object sender, EventArgs e)
        {
            ClassShape shape = new ClassShape();
            shape.BodyType = BodyType.List;
            Point center = new Point(
                this.ClientRectangle.Left + (ClientRectangle.Width / 2),
                ClientRectangle.Top + (ClientRectangle.Height / 2));
            shape.Location = center;
            this.AddShape(shape);
            shape.Invalidate();
        }

        private void OnNewConnection(object sender, EventArgs e)
        {

        }       
        
        #endregion

        #region API visible members

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a shape to the diagram.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns>IShape: The shape that was added.</returns>
        // ------------------------------------------------------------------
        public IShape AddShape(IShape shape)
        {
            this.Controller.Model.AddShape(shape);
            return shape;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a connection to the diagram.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns></returns>
        // ------------------------------------------------------------------
        public IConnection AddConnection(IConnection connection)
        {
            this.Controller.Model.AddConnection(connection);
            return connection;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a connection to the diagram.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        // ------------------------------------------------------------------
        public IConnection AddConnection(IConnector from, IConnector to)
        {

            Connection cn = new Connection(Point.Empty, Point.Empty);
            this.AddConnection(cn);
            from.AttachConnector(cn.From);
            to.AttachConnector(cn.To);
            return cn;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a new page to the diagram.
        /// </summary>
        /// <param name="makeCurrentPage">bool: Specifies if the diagram's
        /// active page is set to the new page.
        /// <param name="showPageSetupDialog">bool: Option to display
        /// the PageSetupDialog so the user can change the default page
        /// setup.</param>
        /// <returns>IPage</returns>
        // ------------------------------------------------------------------
        public IPage AddPage(
            bool makeCurrentPage,
            bool showPageSetupDialog)
        {
            IPage page = Controller.Model.AddPage();

            // Show the page setup dialog?
            if (showPageSetupDialog)
            {
                this.ShowPageSetupDialog(page);
            }

            if (makeCurrentPage)
            {
                this.Controller.Model.SetCurrentPage(page);
            }

            return page;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Deletes the page specified if it is not the default page.
        /// </summary>
        /// <param name="page">IPage: The page to remove.</param>
        /// <param name="allowWarnings">bool: Specifies if the user should
        /// be given the option to cancel the action if the current page
        /// has entities.  Also, when set to true, if the current page is
        /// the default page, then a message box is shown informing the
        /// user that the default page cannot be deleted.</param>
        /// <returns>bool: If the delete was successful.  True is returned
        /// if the current page was removed.</returns>
        // ------------------------------------------------------------------
        public bool RemovePage(IPage page, bool allowWarnings)
        {
            return Controller.Model.RemovePage(page, allowWarnings);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Displays the PageSetupDialog so the user can change the specified
        /// page's settings.
        /// </summary>
        /// <param name="page">IPage</param>
        // ------------------------------------------------------------------
        public void ShowPageSetupDialog(IPage page)
        {
            // Show the PageSetupDialog so the user can specify the name and
            // color of the new page.
            PageSetupDialog dialog = new PageSetupDialog();
            dialog.PageName = page.Name;
            dialog.PageColor = page.Ambience.PageColor;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                page.Ambience.PageColor = dialog.PageColor;
                page.Ambience.PageBackgroundType = CanvasBackgroundTypes.FlatColor;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        ///  Changes the paint style of the selected entities.
        /// </summary>
        /// <param name="paintStyle">The paint style.</param>
        // ------------------------------------------------------------------
        public void ChangeStyle(IPaintStyle paintStyle)
        {
            this.Controller.ChangeStyle(paintStyle);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the pen style of the selected entities.
        /// </summary>
        /// <param name="penStyle"></param>
        // ------------------------------------------------------------------
        public void ChangeStyle(IPenStyle penStyle)
        {
            this.Controller.ChangeStyle(penStyle);
        }

        public new void Layout(LayoutType type)
        {
            if (this.Controller.Model.CurrentPage.Shapes.Count == 0)
                throw new InconsistencyException("There are no shapes on the canvas; there's nothing to lay out.");

            switch (type)
            {
                case LayoutType.ForceDirected:
                    RunActivity("ForceDirected Layout");
                    break;
                case LayoutType.FruchtermanRheingold:
                    RunActivity("FruchtermanReingold Layout");
                    break;
                case LayoutType.RadialTree:
                    SetLayoutRoot();
                    RunActivity("Radial TreeLayout");
                    break;
                case LayoutType.Balloon:
                    SetLayoutRoot();
                    RunActivity("Balloon TreeLayout");
                    break;
                case LayoutType.ClassicTree:
                    SetLayoutRoot();
                    RunActivity("Standard TreeLayout");
                    break;
                default:
                    break;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the layout root.
        /// </summary>
        // ------------------------------------------------------------------
        private void SetLayoutRoot()
        {
            //this layout needs a root, you should assign one before calling this method
            if (this.Controller.Model.LayoutRoot == null)
            {
                //if a shape is selected we'll take that one as the root for the layout
                if (this.SelectedItems.Count > 0)
                    this.Controller.Model.LayoutRoot = this.SelectedItems[0] as IShape;
                else //use the zero-th shape
                    this.Controller.Model.LayoutRoot = this.Controller.Model.CurrentPage.Shapes[0];
            }
        }

        public void Unwrap(IBundle bundle)
        {
            if (bundle != null)
            {
                #region Unwrap the bundle
                Anchors.Clear();
                this.Controller.Model.Unwrap(bundle.Entities);
                Rectangle rec = Utils.BoundingRectangle(bundle.Entities);
                rec.Inflate(30, 30);
                this.Controller.View.Invalidate(rec);
                #endregion
            }
        }
        #endregion
    }
}
