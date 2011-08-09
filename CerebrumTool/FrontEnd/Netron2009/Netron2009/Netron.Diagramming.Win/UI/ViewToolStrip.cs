using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A ToolStrip that controls different aspects about the view of a
    /// diagram (i.e. show connectors, show grid, etc.).
    /// </summary>
    // ----------------------------------------------------------------------
    public class ViewToolStrip : DiagramBaseToolStrip
    {
        ToolStripButton myShowConnectorsButton = new ToolStripButton();
        ToolStripButton myShowGridButton = new ToolStripButton();

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public ViewToolStrip()
            : base()
        {
            this.myShowConnectorsButton.CheckOnClick = true;
            this.myShowGridButton.CheckOnClick = true;
            SetText();
            SetDisplayStyle();
            SetImage();
            AddItems();
            RegisterEvents();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the Text property for each button.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetText()
        {
            this.myShowConnectorsButton.Text = "Show Connection Points";
            this.myShowGridButton.Text = "Show Grid";
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the DisplayStyle property for each button to 'Image'.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetDisplayStyle()
        {
            this.myShowConnectorsButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;
            this.myShowGridButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image from the ImagePalette for each button.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetImage()
        {
            this.myShowConnectorsButton.Image = Images.ShowConnectors;
            myShowGridButton.Image = Images.ShowGrid;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds all buttons to the tool strip.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void AddItems()
        {
            Items.Add(myShowConnectorsButton);
            Items.Add(myShowGridButton);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Register's for a Click event for each button.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void RegisterEvents()
        {
            this.myShowConnectorsButton.Click +=
                new EventHandler(HandleShowConnectorsButtonClick);

            myShowGridButton.Click += 
                new EventHandler(HandleShowGridButtonClick);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Instructs the diagram whether all shape's connector's should be
        /// shown.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleShowGridButtonClick(
            object sender, 
            EventArgs e)
        {
            this.diagramControl.View.ShowGrid = myShowGridButton.Checked;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Instructs the diagram whether all shape's connector's should be
        /// shown.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleShowConnectorsButtonClick(
            object sender, 
            EventArgs e)
        {
            this.diagramControl.ShowConnectors = 
                this.myShowConnectorsButton.Checked;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Initializes the ToolStrip when a new diagram is attached.
        /// </summary>
        // ------------------------------------------------------------------
        protected override void OnNewDiagram()
        {
            base.OnNewDiagram();
            this.EnableAllItems();
            this.myShowConnectorsButton.Checked = 
                this.diagramControl.ShowConnectors;

            this.myShowGridButton.Checked = this.diagramControl.View.ShowGrid;
        }
    }
}
