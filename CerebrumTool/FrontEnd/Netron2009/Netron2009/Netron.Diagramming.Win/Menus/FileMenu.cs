using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A ToolStripMenuItem that contains menu items for each of the following
    /// actions:
    ///     * New Document
    ///     * Open
    ///     * Save
    /// </summary>
    // ----------------------------------------------------------------------
    public class FileMenu : DiagramMenuItemBase
    {
        protected ToolStripMenuItem myNewDocumentMenuItem = 
            new ToolStripMenuItem();

        protected ToolStripMenuItem myOpenDiagramMenuItem = 
            new ToolStripMenuItem();

        protected ToolStripMenuItem mySaveMenuItem = new ToolStripMenuItem();

        protected ToolStripMenuItem mySaveAsMenuItem = new ToolStripMenuItem();

        protected ToolStripMenuItem myPageSetupMenuItem = new ToolStripMenuItem();

        protected ToolStripMenuItem myPrintPreviewMenuItem = new ToolStripMenuItem();

        protected ToolStripMenuItem myPrintMenuItem = new ToolStripMenuItem();

        protected ToolStripMenuItem myExitMenuItem = new ToolStripMenuItem();

        protected ToolStripMenuItem myRecentFilesMenuItem =
            new ToolStripMenuItem();

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public FileMenu()
            : base()
        {
            this.ShowShortcutKeys = true;
            this.ShortcutKeys = Keys.Control & Keys.F;
            this.ShortcutKeyDisplayString = "CTRL+F";
            this.Text = "File";
            SetMenuItemsText();
            SetMenuItemsImage();
            SetMenuItemsDisplayStyle();
            AddDropDownItems();
            RegisterForEvents();

            // Initially all items should be disabled until a diagram is
            // assigned.
            this.DisableAllDropDownItems();
        }

        #region Initialization

        // ------------------------------------------------------------------
        /// <summary>
        /// Assigns the shortcut keys to all menu items.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetShortcutKeys()
        {
            this.myNewDocumentMenuItem.ShortcutKeys = Keys.Control & Keys.N;
            this.myNewDocumentMenuItem.ShortcutKeyDisplayString = "CTRL+N";

            this.myOpenDiagramMenuItem.ShortcutKeys = Keys.Control & Keys.O;
            this.myOpenDiagramMenuItem.ShortcutKeyDisplayString = "CTRL+O";

            this.myPrintMenuItem.ShortcutKeys = Keys.Control & Keys.P;
            this.myPrintMenuItem.ShortcutKeyDisplayString = "CTRL+P";

            this.mySaveMenuItem.ShortcutKeys = Keys.Control & Keys.S;
            this.mySaveMenuItem.ShortcutKeyDisplayString = "CTRL+S";
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the text for each ToolStripMenuItem.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetMenuItemsText()
        {
            this.myNewDocumentMenuItem.Text = "New Document";
            this.myOpenDiagramMenuItem.Text = "Open...";
            this.myPageSetupMenuItem.Text = "Page Setup...";
            this.myPrintMenuItem.Text = "Print...";
            this.myPrintPreviewMenuItem.Text = "Print Preview...";
            this.mySaveMenuItem.Text = "Save";
            this.mySaveAsMenuItem.Text = "Save As";
            this.myExitMenuItem.Text = "Exit";
            this.myRecentFilesMenuItem.Text = "Recent Files";
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the image for each ToolStripMenuItem.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetMenuItemsImage()
        {
            this.myNewDocumentMenuItem.Image = Images.NewDocument;
            this.myOpenDiagramMenuItem.Image = Images.OpenFolder;
            this.myPageSetupMenuItem.Image = Images.PageSetup;
            this.myPrintMenuItem.Image = Images.Print;
            this.myPrintPreviewMenuItem.Image = Images.PrintPreview;
            this.mySaveMenuItem.Image = Images.Save;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the display style for each ToolStripMenuItem.  The value
        /// set here is ImageAndText.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetMenuItemsDisplayStyle()
        {
            this.myNewDocumentMenuItem.DisplayStyle =
                ToolStripItemDisplayStyle.ImageAndText;

            this.myOpenDiagramMenuItem.DisplayStyle =
                ToolStripItemDisplayStyle.ImageAndText;

            this.myPageSetupMenuItem.DisplayStyle =
                ToolStripItemDisplayStyle.ImageAndText;

            this.myPrintMenuItem.DisplayStyle = 
                ToolStripItemDisplayStyle.ImageAndText;

            this.myPrintPreviewMenuItem.DisplayStyle = 
                ToolStripItemDisplayStyle.ImageAndText;

            this.mySaveMenuItem.DisplayStyle =
                ToolStripItemDisplayStyle.ImageAndText;

            this.mySaveAsMenuItem.DisplayStyle = 
                ToolStripItemDisplayStyle.Text;

            this.myExitMenuItem.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.myRecentFilesMenuItem.DisplayStyle = 
                ToolStripItemDisplayStyle.Text;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds all ToolStripMenuItem's.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void AddDropDownItems()
        {
            this.DropDownItems.Add(this.myNewDocumentMenuItem);
            this.DropDownItems.Add(this.myOpenDiagramMenuItem);
            this.DropDownItems.Add(new ToolStripSeparator());

            this.DropDownItems.Add(this.mySaveMenuItem);
            this.DropDownItems.Add(this.mySaveAsMenuItem);
            this.DropDownItems.Add(new ToolStripSeparator());

            this.DropDownItems.Add(this.myPageSetupMenuItem);
            this.DropDownItems.Add(this.myPrintPreviewMenuItem);
            this.DropDownItems.Add(this.myPrintMenuItem);
            this.DropDownItems.Add(new ToolStripSeparator());

            this.DropDownItems.Add(this.myExitMenuItem);

            this.DropDownItems.Add(this.myRecentFilesMenuItem);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Registers for the Click event for each ToolStripMenuItem.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void RegisterForEvents()
        {
            this.myNewDocumentMenuItem.Click +=
                new EventHandler(HandleNewDocumentClick);

            this.myOpenDiagramMenuItem.Click +=
                new EventHandler(HandleOpenDiagramMenuItemClick);

            this.mySaveMenuItem.Click +=
                new EventHandler(HandleSaveMenuItemClick);

            this.mySaveAsMenuItem.Click +=
                new EventHandler(HandleSaveAsMenuItemClick);

            this.myPageSetupMenuItem.Click +=
                new EventHandler(HandlePageSetupMenuItemClick);

            this.myPrintMenuItem.Click +=
                new EventHandler(HandlePrintMenuItemClick);

            this.myPrintPreviewMenuItem.Click +=
                new EventHandler(HandlePrintPreviewMenuItemClick);

            this.myExitMenuItem.Click +=
                new EventHandler(HandleExitMenuItemClick);

            this.myRecentFilesMenuItem.DropDownItemClicked += 
                new ToolStripItemClickedEventHandler(
                myRecentFilesMenuItem_DropDownItemClicked);
        }

        #endregion
        
        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the list of filenames to show in the "Recent Files" menu.
        /// </summary>
        /// <param name="filenames">string[]</param>
        /// <param name="append">bool: Specifies if they should be appended
        /// to already existing files.  If set to 'false', then the current
        /// list is cleared.</param>
        // ------------------------------------------------------------------
        public void SetRecentlyOpenedFiles(string[] filenames, bool append)
        {
            if (append == false)
            {
                myRecentFilesMenuItem.DropDownItems.Clear();
            }
            foreach (string file in filenames)
            {
                if (File.Exists(file))
                {
                    ToolStripMenuItem fileMenu = new ToolStripMenuItem();
                    fileMenu.Text = file;
                    this.myRecentFilesMenuItem.DropDownItems.Add(fileMenu);
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Opens a recently opened file.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">ToolStripItemClickedEventArgs</param>
        // ------------------------------------------------------------------
        void myRecentFilesMenuItem_DropDownItemClicked(
            object sender,
            ToolStripItemClickedEventArgs e)
        {
            if (File.Exists(e.ClickedItem.Text))
            {
                this.Diagram.Open(e.ClickedItem.Text);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the ToolStripMenuItem for page setup is 
        /// clicked.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        void HandlePageSetupMenuItemClick(object sender, EventArgs e)
        {
            this.myDiagram.PageSetup();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the ToolStripMenuItem for print previewing the 
        /// diagram is clicked.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandlePrintPreviewMenuItemClick(
            object sender, 
            EventArgs e)
        {
            this.myDiagram.PrintPreview();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the ToolStripMenuItem for printing the 
        /// diagram is clicked.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandlePrintMenuItemClick(
            object sender, 
            EventArgs e)
        {
            this.myDiagram.Print();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the ToolStripMenuItem for closing the 
        /// application is clicked.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleExitMenuItemClick(
            object sender,
            EventArgs e)
        {
            Form owner = this.Parent.FindForm();

            if (owner != null)
            {
                owner.Close();
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the ToolStripMenuItem for saving the diagram
        /// to a new loation (forces 'SaveAs') is clicked.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleSaveAsMenuItemClick(
            object sender,
            EventArgs e)
        {
            this.myDiagram.SaveAs();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the ToolStripMenuItem for saving the diagram
        /// is clicked.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleSaveMenuItemClick(
            object sender, 
            EventArgs e)
        {
            this.myDiagram.Save();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the ToolStripMenuItem for opening a diagram 
        /// is clicked.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleOpenDiagramMenuItemClick(
            object sender, 
            EventArgs e)
        {
            this.myDiagram.Open();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method called when the ToolStripMenuItem for creating a new 
        /// document is clicked.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleNewDocumentClick(
            object sender, 
            EventArgs e)
        {
            this.myDiagram.NewDocument();
        }
    }
}
