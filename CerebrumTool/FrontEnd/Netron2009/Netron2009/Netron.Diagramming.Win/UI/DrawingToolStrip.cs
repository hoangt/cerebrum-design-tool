using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Netron.Diagramming.Core;

namespace Netron.Diagramming.Win
{
    public class DrawingToolStrip : DiagramBaseToolStrip
    {

        #region Fields

        protected ToolStripButton mySelectionToolButton = 
            new ToolStripButton();

        protected ToolStripButton myDrawElipseToolButton = 
            new ToolStripButton();

        protected ToolStripButton myDrawRectangleToolButton =
            new ToolStripButton();

        protected ToolStripButton myTextToolButton =
           new ToolStripButton();

        protected ToolStripButton myMultiLineToolButton =
            new ToolStripButton();

        protected ToolStripButton myPolygonToolButton =
            new ToolStripButton();

        protected ToolStripButton myScribbleToolButton =
            new ToolStripButton();

        protected ToolStripButton myConnectionToolButton = 
            new ToolStripButton();

        protected ToolStripButton myConnectorMoverToolButton =
            new ToolStripButton();

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public DrawingToolStrip()
            : base()
        {
            this.Name = "Drawing Toolstrip";

            #region Setting Tool Button's Text

            // Set the text of each button for the tool tip.
            this.mySelectionToolButton.Text = "Selection Tool";
            this.myDrawElipseToolButton.Text = "Ellipse Tool";
            this.myDrawRectangleToolButton.Text = "Rectangle Tool";
            this.myMultiLineToolButton.Text = "Multi Lines Tool";
            this.myPolygonToolButton.Text = "Polygon Tool";
            this.myScribbleToolButton.Text = "Scribble Tool";
            this.myConnectionToolButton.Text = "Connection Tool";
            this.myConnectorMoverToolButton.Text = "Move Connector Tool";
            this.myTextToolButton.Text = "Text Tool";

            #endregion

            #region Setting Tool Button's Display Style

            // Only show the image on the button.
            this.mySelectionToolButton.DisplayStyle = 
                ToolStripItemDisplayStyle.Image;

            this.myDrawElipseToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myDrawRectangleToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myMultiLineToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myPolygonToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myScribbleToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myConnectionToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myConnectorMoverToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myTextToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            #endregion

            #region Setting Tool Button's Images

            // Get the images from the resource.
            this.mySelectionToolButton.Image = Images.Arrow;
            this.myDrawElipseToolButton.Image = Images.DrawEllipse;
            this.myDrawRectangleToolButton.Image = Images.DrawRectangle;
            this.myMultiLineToolButton.Image = Images.MultiLines;
            this.myPolygonToolButton.Image = Images.Polygon;
            this.myScribbleToolButton.Image = Images.Scribble;
            this.myConnectionToolButton.Image = Images.Connection;
            this.myConnectorMoverToolButton.Image = Images.MoveConnector;
            this.myTextToolButton.Image = Images.TextBox;

            #endregion

            #region Adding Tool Button's To ToolStrip

            // Add the buttons to this tool strip.
            this.Items.Add(this.mySelectionToolButton);
            this.Items.Add(this.myConnectionToolButton);
            //this.Items.Add(this.myDrawElipseToolButton);
            //this.Items.Add(this.myDrawRectangleToolButton);
            //this.Items.Add(this.myMultiLineToolButton);
            //this.Items.Add(this.myPolygonToolButton);
            //this.Items.Add(this.myScribbleToolButton);
            //this.Items.Add(new ToolStripSeparator()); 
            //this.Items.Add(this.myTextToolButton);
            //this.Items.Add(new ToolStripSeparator());  
            //this.Items.Add(this.myConnectorMoverToolButton);

            #endregion

            #region Registering Button-Click Events

            // Hook-up click-events to start the drawing.
            this.mySelectionToolButton.Click += 
                new EventHandler(HandleSelectionToolButtonClick);

            this.myDrawElipseToolButton.Click += 
                new EventHandler(HandleDrawElipseToolButtonClick);

            this.myDrawRectangleToolButton.Click += 
                new EventHandler(HandleDrawRectangleToolButtonClick);

            this.myMultiLineToolButton.Click += 
                new EventHandler(HandleMultiLineToolButtonClick);

            this.myPolygonToolButton.Click += 
                new EventHandler(HandlePolygonToolButtonClick);

            this.myScribbleToolButton.Click += 
                new EventHandler(HandleScribbleToolButtonClick);

            this.myConnectionToolButton.Click += 
                new EventHandler(HandleConnectionToolButtonClick);

            this.myConnectorMoverToolButton.Click += 
                new EventHandler(HandleConnectorMoverToolButtonClick);

            this.myTextToolButton.Click += 
                new EventHandler(HandleTextToolButtonClick);

            #endregion
        }

        #region Button Click Methods

        // ------------------------------------------------------------------
        /// <summary>
        /// Activates the Controller's ScribbleTool.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleScribbleToolButtonClick(
            object sender, 
            EventArgs e)
        {
            StartTool(Controller.ScribbleToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Activates the Controller's PolygonTool.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandlePolygonToolButtonClick(
            object sender, 
            EventArgs e)
        {
            StartTool(Controller.PolygonToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Activates the Controller's MultiLineTool.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleMultiLineToolButtonClick(
            object sender, 
            EventArgs e)
        {
            StartTool(Controller.MultiLineToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Activates the Controller's TextTool.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleTextToolButtonClick(
            object sender, 
            EventArgs e)
        {
            StartTool(Controller.TextToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Activates the Controller's ConnectorMoverTool.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleConnectorMoverToolButtonClick(
            object sender, 
            EventArgs e)
        {
            StartTool(Controller.ConnectorMoverToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Activates the Controller's ConnectionTool.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleConnectionToolButtonClick(
            object sender, 
            EventArgs e)
        {
            StartTool(Controller.ConnectionToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Activates the Controller's RectangleTool.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleDrawRectangleToolButtonClick(
            object sender, 
            EventArgs e)
        {
            StartTool(Controller.RectangleToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Activates the Controller's EllipseTool.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleDrawElipseToolButtonClick(
            object sender, 
            EventArgs e)
        {
            StartTool(Controller.EllipseToolName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Deactivates all tools.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleSelectionToolButtonClick(
            object sender, 
            EventArgs e)
        {
            this.diagramControl.Controller.DeactivateAllTools();
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Initializes the ToolStrip when a new diagram is attached.
        /// </summary>
        // ------------------------------------------------------------------
        protected override void OnNewDiagram()
        {
            base.OnNewDiagram();
            this.EnableAllItems();
            this.UnCheckAll();
            this.mySelectionToolButton.Checked = true;

            this.diagramControl.Controller.OnToolActivate +=
                new EventHandler<ToolEventArgs>(HandleToolActivated);

            this.diagramControl.Controller.OnToolDeactivate +=
                new EventHandler<ToolEventArgs>(HandleToolDeactivated);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Unchecks all buttons and checks the "Selection" button to
        /// indicate no drawing tool is active.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">ToolEventArgs</param>
        // ------------------------------------------------------------------
        void HandleToolActivated(object sender, ToolEventArgs e)
        {
            this.UnCheckAll();
            string toolName = e.Properties.Name;

            if (toolName == ControllerBase.RectangleToolName)
            {
                this.myDrawRectangleToolButton.Checked = true;
            }
            else if (toolName == ControllerBase.EllipseToolName)
            {
                this.myDrawElipseToolButton.Checked = true;
            }
            else if (toolName == Controller.TextToolName)
            {
                this.myTextToolButton.Checked = true;
            }
            else if (toolName == ControllerBase.MultiLineToolName)
            {
                this.myMultiLineToolButton.Checked = true;
            }
            else if (toolName == ControllerBase.PolygonToolName)
            {
                this.myPolygonToolButton.Checked = true;
            }
            else if (toolName == ControllerBase.ScribbleToolName)
            {
                this.myScribbleToolButton.Checked = true;
            }
            else if (toolName == ControllerBase.ConnectionToolName)
            {
                this.myConnectionToolButton.Checked = true;
            }
            else if (toolName == ControllerBase.ConnectorMoverToolName)
            {
                this.myConnectorMoverToolButton.Checked = true;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Unchecks all buttons and checks the "Selection" button to
        /// indicate no drawing tool is active.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">ToolEventArgs</param>
        // ------------------------------------------------------------------
        void HandleToolDeactivated(object sender, ToolEventArgs e)
        {
            this.UnCheckAll();
            this.mySelectionToolButton.Checked = true;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Starts the tool with the specified name.
        /// </summary>
        /// <param name="toolName">string: The name of the tool, such as
        /// "Rectangle Tool", "Ellipse Tool", "Connection Tool", 
        /// "Connector Mover Tool", etc.  Use the const tool names provided
        /// from the Controller if using the tools that come shipped with
        /// this framework.</param>
        // ------------------------------------------------------------------
        protected virtual void StartTool(string toolName)
        {            
            if (this.diagramControl != null)
            {
                try
                {
                    this.diagramControl.ActivateTool(toolName);
                }
                catch (Exception e)
                {
                    string errorMessage = 
                        "An error occured while activating " +
                        "the tool named " + toolName + ".\n\n" +
                        "Error Message: " + e.Message;
                    MessageBox.Show(errorMessage);
                }
            }
        }
    }
}
