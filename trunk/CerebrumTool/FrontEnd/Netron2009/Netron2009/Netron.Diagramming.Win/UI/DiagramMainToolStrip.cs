using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Netron.Diagramming.Core;

namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A ToolStrip that controls the main diagram actions:
    ///     * New Documnet
    ///     * Open Diagram
    ///     * Save Diagram
    ///     * Undo
    ///     * Redo
    ///     * Go forward (navigate to the next page in the document)
    ///     * Go back (navigate to the previous page in the document)
    /// </summary>
    // ----------------------------------------------------------------------
    public class DiagramMainToolStrip : DiagramBaseToolStrip
    {
        #region Fields

        protected ToolStripButton myNewDiagramButton = new ToolStripButton();
        protected ToolStripButton myOpenButton = new ToolStripButton();
        protected ToolStripButton mySaveDiagramButton = new ToolStripButton();
        protected ToolStripButton myUndoButton = new ToolStripButton();
        protected ToolStripButton myRedoButton = new ToolStripButton();

        // ------------------------------------------------------------------
        /// <summary>
        /// Allows the user to zoom in on an area.
        /// </summary>
        // ------------------------------------------------------------------
        protected ToolStripButton myZoomAreaButton = new ToolStripButton();

        // ------------------------------------------------------------------
        /// <summary>
        /// Allows the user to incrementally adjust the magnification.
        /// </summary>
        // ------------------------------------------------------------------
        protected ToolStripButton myZoomInButton = new ToolStripButton();

        // ------------------------------------------------------------------
        /// <summary>
        /// Allows the user to incrementally adjust the magnification.
        /// </summary>
        // ------------------------------------------------------------------
        protected ToolStripButton myZoomOutButton = new ToolStripButton();

        // ------------------------------------------------------------------
        /// <summary>
        /// Set the zoom of the diagram and origin such that all entities in
        /// the current page are in view when this button is clicked.
        /// </summary>
        // ------------------------------------------------------------------
        protected ToolStripButton myViewAllButton = new ToolStripButton();

        // ------------------------------------------------------------------
        /// <summary>
        /// Activates the PanTool when clicked.
        /// </summary>
        // ------------------------------------------------------------------
        protected ToolStripButton myPanButton = new ToolStripButton();

        // ------------------------------------------------------------------
        /// <summary>
        /// Activates the ImageExportTool when clicked.
        /// </summary>
        // ------------------------------------------------------------------
        protected ToolStripButton myImageExportButton = new ToolStripButton();

        // ------------------------------------------------------------------
        /// <summary>
        /// Navigates to the next page in the document when clicked.
        /// </summary>
        // ------------------------------------------------------------------
        protected ToolStripButton myGoForwardButton = new ToolStripButton();

        // ------------------------------------------------------------------
        /// <summary>
        /// Navigates to the previous page in the document when clicked.
        /// </summary>
        // ------------------------------------------------------------------
        protected ToolStripButton myGoBackButton = new ToolStripButton();

        #endregion

        #region Constructors

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramMainToolStrip()
            : base()
        {
            // Set the text of each button for the tooltip.
            SetText();

            // Get the image for each button from the ImagePallet.
            SetImage();

            // Only show the image on the button.
            SetDisplayStyle();

            // Add the buttons to this tool strip.
            AddItems();

            // Disable all items until the diagram control is set.
            this.DisableAllItems();

            // Hook-up click-events to start the action.
            RegisterEvents();
        }

        #endregion

        #region Initialization

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the Text property for each button.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetText()
        {
            this.myNewDiagramButton.Text = "New Diagram";
            this.myOpenButton.Text = "Open...";
            this.mySaveDiagramButton.Text = "Save Diagram...";
            this.myUndoButton.Text = "Undo";
            this.myRedoButton.Text = "Redo";
            this.myZoomAreaButton.Text = "Zoom Area";
            this.myZoomInButton.Text = "Zoom In";
            this.myZoomOutButton.Text = "Zoom Out";
            this.myViewAllButton.Text = "View All";
            this.myPanButton.Text = "Pan Tool";
            this.myImageExportButton.Text = 
                "Copy Selection As Image To Clipboard";

            this.myGoBackButton.Text = "Go Back";
            this.myGoBackButton.ToolTipText = "Selects the previous page " +
                "in the document.";

            this.myGoForwardButton.Text = "Go Forward";
            this.myGoForwardButton.ToolTipText = "Selects the next page " +
                "in the document.";
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the DisplayStyle property for each button to 'Image'.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetDisplayStyle()
        {
            this.myNewDiagramButton.DisplayStyle =
               ToolStripItemDisplayStyle.Image;

            this.myOpenButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.mySaveDiagramButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myUndoButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myRedoButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myZoomAreaButton.DisplayStyle = 
                ToolStripItemDisplayStyle.Image;

            this.myZoomInButton.DisplayStyle = 
                ToolStripItemDisplayStyle.Image;

            this.myZoomOutButton.DisplayStyle = 
                ToolStripItemDisplayStyle.Image;

            this.myViewAllButton.DisplayStyle = 
                ToolStripItemDisplayStyle.Text;

            this.myPanButton.DisplayStyle = ToolStripItemDisplayStyle.Image;

            this.myImageExportButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myGoForwardButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myGoBackButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image from the ImagePalette for each button.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetImage()
        {
            this.myNewDiagramButton.Image = Images.NewDocument;
            this.myOpenButton.Image = Images.OpenFolder;
            this.mySaveDiagramButton.Image = Images.Save;
            this.myUndoButton.Image = Images.Undo;
            this.myRedoButton.Image = Images.Redo;
            this.myZoomAreaButton.Image = Images.ZoomMarquee;
            this.myZoomInButton.Image = Images.ZoomIn;
            this.myZoomOutButton.Image = Images.ZoomOut;
            this.myPanButton.Image = Images.Hand;
            this.myImageExportButton.Image = Images.Picture;
            this.myGoBackButton.Image = Images.NavigateBack;
            this.myGoForwardButton.Image = Images.NavigateForward;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds all buttons to the tool strip.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void AddItems()
        {
            //this.Items.Add(this.myNewDiagramButton);
            //this.Items.Add(this.myOpenButton);
            //this.Items.Add(this.mySaveDiagramButton);
            //this.Items.Add(new ToolStripSeparator());
            //this.Items.Add(this.myImageExportButton);
            //this.Items.Add(new ToolStripSeparator());
            this.Items.Add(this.myUndoButton);
            this.Items.Add(this.myRedoButton);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(myViewAllButton);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(this.myZoomAreaButton);
            this.Items.Add(this.myZoomInButton);
            this.Items.Add(this.myZoomOutButton);
            this.Items.Add(this.myPanButton);
            //this.Items.Add(new ToolStripSeparator());
            //this.Items.Add(this.myGoBackButton);
            //this.Items.Add(this.myGoForwardButton);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Register's for a Click event for each button.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void RegisterEvents()
        {
            this.myNewDiagramButton.Click +=
               new EventHandler(HandleNewDiagramButtonClick);

            this.myOpenButton.Click +=
                new EventHandler(HandleOpenButtonClick);

            this.mySaveDiagramButton.Click +=
                new EventHandler(HandleSaveDiagramButtonClick);

            this.myUndoButton.Click += 
                new EventHandler(HandleUndoButtonClick);

            this.myRedoButton.Click += 
                new EventHandler(HandleRedoButtonClick);

            this.myZoomAreaButton.Click += 
                new EventHandler(HandleZoomButtonClick);

            this.myZoomInButton.Click +=
                new EventHandler(HandleZoomInButtonClick);

            this.myZoomOutButton.Click +=
                new EventHandler(HandleZoomOutButtonClick);

            myViewAllButton.Click += 
                new EventHandler(HandleViewAllButtonClick);

            this.myPanButton.Click += 
                new EventHandler(HandlePanButtonClick);

            this.myImageExportButton.Click +=
                new EventHandler(HandleImageExportButtonClick);

            this.myGoBackButton.Click +=
                new EventHandler(HandleGoBackButtonClick);

            this.myGoForwardButton.Click +=
                new EventHandler(HandleGoForwardButtonClick);
        }

        #endregion

        #region Button Click Methods

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myGoForwardButton' is clicked - navigates
        /// the diagram to the next page.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleGoForwardButtonClick(
            object sender, 
            EventArgs e)
        {
            this.diagramControl.Controller.GoForward(true);  // Wrap pages.
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myGoBackButton' is clicked - navigates
        /// the diagram to the previous page.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleGoBackButtonClick(
            object sender, 
            EventArgs e)
        {
            this.diagramControl.Controller.GoBack(true);  // Wrap pages.
        }

        #region Zoom and Pan

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myViewAllButton' is clicked - starts  
        /// the 'ZoomAreaTool' to allow the user to zoom in on a user-drawn
        /// rectangular area.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleViewAllButtonClick(
            object sender, 
            EventArgs e)
        {
            diagramControl.View.ZoomFit();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myZoomOutButton' is clicked - starts  
        /// the 'ZoomOutTool' to allow the user to incrementally zoom out.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleZoomOutButtonClick(
            object sender, 
            EventArgs e)
        {
            this.diagramControl.ActivateTool(Controller.ZoomOutToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myZoomInButton' is clicked - starts  
        /// the 'ZoomInTool' to allow the user to incrementally zoom in.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleZoomInButtonClick(
            object sender, 
            EventArgs e)
        {
            this.diagramControl.ActivateTool(Controller.ZoomInToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myZoomButton' is clicked - starts the 
        /// 'ZoomTool' to allow the user to draw a rectangle area to zoom
        /// in on.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleZoomButtonClick(
            object sender,
            EventArgs e)
        {
            this.diagramControl.ActivateTool(Controller.ZoomAreaToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myPanButton' is clicked - starts the 
        /// 'PanTool' to allow the user to change the origin of the diagram.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandlePanButtonClick(
            object sender,
            EventArgs e)
        {
            this.diagramControl.ActivateTool(Controller.PanToolName);
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myImageExportButton' is clicked - starts  
        /// the 'ImageExportTool' to copy the selected shapes to the clipboard
        /// as an image.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleImageExportButtonClick(
            object sender,
            EventArgs e)
        {
            this.diagramControl.ActivateTool(Controller.ImageExportToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myOpenButton' is clicked - opens a
        /// previously saved diagram from disk.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleOpenButtonClick(
            object sender, 
            EventArgs e)
        {
            this.diagramControl.Open();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'mySaveDiagramButton' is clicked - saves 
        /// the current diagram to disk.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleSaveDiagramButtonClick(
            object sender, 
            EventArgs e)
        {
            this.diagramControl.Save();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myRedoButton' is clicked - undoes the
        /// last command.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleRedoButtonClick(
            object sender, 
            EventArgs e)
        {
            if (this.diagramControl != null)
            {
                this.diagramControl.Redo();
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myUndoButton' is clicked - redoes the
        /// last undo.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleUndoButtonClick(
            object sender, 
            EventArgs e)
        {
            if (this.diagramControl != null)
            {
                this.diagramControl.Undo();
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the 'myNewDiagramButton' is clicked - creates a 
        /// new, blank diagram.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleNewDiagramButtonClick(
            object sender, 
            EventArgs e)
        {
            if (this.diagramControl != null)
            {
                this.diagramControl.NewDocument();
            }
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Update the Undo/Redo buttons and attach an OnHistoryChanged event
        /// to the new diagram.
        /// </summary>
        // ------------------------------------------------------------------
        protected override void OnNewDiagram()
        {
            this.EnableAllItems();
            UpdateUndoRedoButtons();
            UpdateNavigationButtons();

            // Watch when the diagram's history changes so we can
            // update the undo and redo buttons.
            this.diagramControl.OnHistoryChange +=
                new EventHandler<HistoryChangeEventArgs>(
                HandleDiagramControlOnHistoryChange);

            // Disable the navigate forward and backward buttons if there's
            // only one page in the diagram.
            this.diagramControl.Controller.Model.Pages.OnItemAdded += 
                new EventHandler<CollectionEventArgs<IPage>>(Pages_OnItemAdded);

            this.diagramControl.Controller.Model.Pages.OnItemRemoved += 
                new EventHandler<CollectionEventArgs<IPage>>(Pages_OnItemRemoved);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Enables the navigate forward ('myGoForwardButton') and the
        /// navigate back ('myGoBackButton') if there's more than one 
        /// page in the diagram.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">CollectionEventArgs<IPage></param>
        // ------------------------------------------------------------------
        protected virtual void Pages_OnItemRemoved(
            object sender, 
            CollectionEventArgs<IPage> e)
        {
            this.UpdateNavigationButtons();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Enables the navigate forward ('myGoForwardButton') and the
        /// navigate back ('myGoBackButton') if there's more than one 
        /// page in the diagram.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">CollectionEventArgs<IPage></param>
        // ------------------------------------------------------------------
        protected virtual void Pages_OnItemAdded(
            object sender, 
            CollectionEventArgs<IPage> e)
        {
            this.UpdateNavigationButtons();            
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the diagrams (undo/redo) history changed.  The
        /// 'myRedoButton' and 'myUndoButton' buttons are enabled/disabled
        /// depending on the history state.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleDiagramControlOnHistoryChange(
            object sender,
            HistoryChangeEventArgs e)
        {
            UpdateUndoRedoButtons();

        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Enables the navigate forward ('myGoForwardButton') and the
        /// navigate back ('myGoBackButton') if there's more than one 
        /// page in the diagram.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void UpdateNavigationButtons()
        {
            if (this.diagramControl.Controller.Model.Pages.Count > 1)
            {
                this.myGoForwardButton.Enabled = true;
                this.myGoBackButton.Enabled = true;
            }
            else
            {
                this.myGoForwardButton.Enabled = false;
                this.myGoBackButton.Enabled = false;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Enables/disables the undo and redo buttons depending on the state
        /// of the UndoManager.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void UpdateUndoRedoButtons()
        {
            UndoManager undoManager =
                this.diagramControl.Controller.UndoManager;

            if (undoManager.RedoText.Length == 0)
            {
                this.myRedoButton.Enabled = false;
                this.myRedoButton.Text = "Can't Redo";
            }
            else
            {
                this.myRedoButton.Enabled = true;
                this.myRedoButton.Text = "Redo " + undoManager.RedoText;
            }

            if (undoManager.UndoText.Length == 0)
            {
                this.myUndoButton.Enabled = false;
                this.myUndoButton.Text = "Can't Undo";
            }
            else
            {
                this.myUndoButton.Enabled = true;
                this.myUndoButton.Text = "Undo " + undoManager.UndoText;
            }
        }
    }
}
