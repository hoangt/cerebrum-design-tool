using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Netron.Diagramming.Core;
using Netron.Diagramming.Core.Analysis;

namespace Netron.Diagramming.Win
{
    public class ActionToolStrip : DiagramBaseToolStrip
    {
        private List<ToolStripButton> _CustomButtons;

        #region Fields

        protected ToolStripButton alignLeftEdgesToolButton =
           new ToolStripButton();

        protected ToolStripButton alignRightEdgesToolButton =
           new ToolStripButton();

        protected ToolStripButton alignTopEdgesToolButton =
           new ToolStripButton();

        protected ToolStripButton alignBottomEdgesToolButton =
           new ToolStripButton();

        protected ToolStripButton alignCentersHorizontallyToolButton =
           new ToolStripButton();

        protected ToolStripButton alignCentersVerticallyToolButton =
           new ToolStripButton();

        protected ToolStripButton sendToBackToolButton =
           new ToolStripButton();

        protected ToolStripButton sendBackwardsToolButton =
           new ToolStripButton();

        protected ToolStripButton bringToFrontToolButton =
           new ToolStripButton();

        protected ToolStripButton bringForwardsToolButton =
           new ToolStripButton();

        protected ToolStripButton groupToolButton =
           new ToolStripButton();

        protected ToolStripButton ungroupToolButton =
           new ToolStripButton();

        protected ToolStripDropDownButton layoutButton =
            new ToolStripDropDownButton();

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public ActionToolStrip()
            : base()
        {
            // Set the text for the tool tip's.
            this.alignLeftEdgesToolButton.Text = "Align Left Edges";
            this.alignRightEdgesToolButton.Text = "Align Right Edges";
            this.alignTopEdgesToolButton.Text = "Align Top Edges";
            this.alignBottomEdgesToolButton.Text = "Align Bottom Edges";
            this.alignCentersHorizontallyToolButton.Text =
                "Align Centers Horizontally";
            this.alignCentersVerticallyToolButton.Text =
                "Align Centers Vertically";
            this.sendToBackToolButton.Text = "Send to Back";
            this.sendBackwardsToolButton.Text = "Send Backwards";
            this.bringToFrontToolButton.Text = "Bring to Front";
            this.bringForwardsToolButton.Text = "Bring Forwards";
            this.groupToolButton.Text = "Group";
            this.ungroupToolButton.Text = "Ungroup";
            this.layoutButton.ToolTipText =
                "Layout selected shapes in different arrangements.";

            // Only show the button's image, not the text.
            this.alignLeftEdgesToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.alignRightEdgesToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.alignTopEdgesToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.alignBottomEdgesToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.alignCentersHorizontallyToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.alignCentersVerticallyToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.sendToBackToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.sendBackwardsToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.bringToFrontToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.bringForwardsToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.groupToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.ungroupToolButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.layoutButton.DisplayStyle = 
                ToolStripItemDisplayStyle.Image;

            // Get the button images from the ImagePallet.
            this.alignLeftEdgesToolButton.Image = Images.AlignObjectsLeft;
            this.alignRightEdgesToolButton.Image = Images.AlignObjectsRight;
            this.alignTopEdgesToolButton.Image = Images.AlignObjectsTop;
            this.alignBottomEdgesToolButton.Image = Images.AlignObjectsBottom;
            this.alignCentersHorizontallyToolButton.Image =
                Images.AlignObjectsCenteredHorizontal;
            this.alignCentersVerticallyToolButton.Image =
                Images.AlignObjectsCenteredVertical;
            this.sendToBackToolButton.Image = Images.SendToBack;
            this.sendBackwardsToolButton.Image = Images.SendBackwards;
            this.bringToFrontToolButton.Image = Images.BringToFront;
            this.bringForwardsToolButton.Image = Images.BringForward;
            this.groupToolButton.Image = Images.Group;
            this.ungroupToolButton.Image = Images.Ungroup;
            this.layoutButton.Image = Images.LayoutShapes;

            // Add the buttons.
            this.Items.Add(this.alignLeftEdgesToolButton);
            this.Items.Add(this.alignRightEdgesToolButton);
            this.Items.Add(this.alignTopEdgesToolButton);
            this.Items.Add(this.alignBottomEdgesToolButton);
            this.Items.Add(this.alignCentersHorizontallyToolButton);
            this.Items.Add(this.alignCentersVerticallyToolButton);
            //this.Items.Add(new ToolStripSeparator());
            //this.Items.Add(this.sendToBackToolButton);
            //this.Items.Add(this.sendBackwardsToolButton);
            //this.Items.Add(this.bringToFrontToolButton);
            //this.Items.Add(this.bringForwardsToolButton);
            //this.Items.Add(this.layoutButton);
            //this.Items.Add(new ToolStripSeparator());
            //this.Items.Add(this.groupToolButton);
            //this.Items.Add(this.ungroupToolButton);

            // Add the layout drop down menu items to the 'layoutButton'.
            string[] layoutNames = Enum.GetNames(typeof(LayoutType));
            foreach (string name in layoutNames)
            {
                this.layoutButton.DropDownItems.Add(name);
            }

            this.alignLeftEdgesToolButton.Click += 
                new EventHandler(alignLeftEdgesToolButton_Click);

            this.alignRightEdgesToolButton.Click += 
                new EventHandler(alignRightEdgesToolButton_Click);

            this.alignTopEdgesToolButton.Click += 
                new EventHandler(alignTopEdgesToolButton_Click);

            this.alignBottomEdgesToolButton.Click += 
                new EventHandler(alignBottomEdgesToolButton_Click);

            this.alignCentersHorizontallyToolButton.Click += 
                new EventHandler(alignCentersHorizontallyToolButton_Click);

            this.alignCentersVerticallyToolButton.Click += 
                new EventHandler(alignCentersVerticallyToolButton_Click);

            this.sendToBackToolButton.Click +=
                new EventHandler(sendToBackToolButton_Click);

            this.sendBackwardsToolButton.Click +=
                new EventHandler(sendBackwardsToolButton_Click);

            this.bringToFrontToolButton.Click
                += new EventHandler(bringToFrontToolButton_Click);

            this.bringForwardsToolButton.Click +=
                new EventHandler(bringForwardsToolButton_Click);

            this.groupToolButton.Click +=
                new EventHandler(groupToolButton_Click);

            this.ungroupToolButton.Click +=
                new EventHandler(ungroupToolButton_Click);

            this.layoutButton.DropDownItemClicked += 
                new ToolStripItemClickedEventHandler(layoutButton_DropDownItemClicked);

            // Disable all buttons.  There's no action to perform until
            // a selection is made.
            this.DisableAllItems();

            // Watch when the selected items changed so we can disable or
            // enable the buttons as needed.
            Selection.OnNewSelection += 
                new EventHandler(Selection_OnNewSelection);

            // Custom Buttons
            _CustomButtons = new List<ToolStripButton>();
        }
        
        // Added by Matthew Cotter 1/27/2011 to facilitate adding buttons to the Action toolbar with event handlers outside this project.
        public void AddCustomButton(string Text, System.Drawing.Image ButtonImage, EventHandler OnClickHandler)
        {
            ToolStripButton newItem = new ToolStripButton(Text, ButtonImage, OnClickHandler);
            this.Items.Add(newItem);
            _CustomButtons.Add(newItem);
        }
        void UpdateCustomButtons()
        {
            bool enabled = false;
            if (Selection.SelectedItems.Count >= 1)
            {
                enabled = true;
            }
            foreach (ToolStripButton tsb in _CustomButtons)
            {
                tsb.Enabled = enabled;
            }
        }
        void layoutButton_DropDownItemClicked(
            object sender, 
            ToolStripItemClickedEventArgs e)
        {
            // Each value of the LayoutType enum was set as the text,
            // so we can create the enum value from the text in the
            // dropdown menu.
            LayoutType layoutType = (LayoutType) Enum.Parse(
                typeof(LayoutType), 
                e.ClickedItem.Text);
            try
            {
                this.diagramControl.Layout(layoutType);
            }
            catch (Exception ex)
            {
                MessageBox.Show("The layout failed.  See below " +
                    "for the error message:\n\n" +
                    ex.Message);
            }
        }


        void Selection_OnNewSelection(object sender, EventArgs e)
        {
            // Enable or disable the appropriate buttons when a new
            // selection is made.
            this.UpdateAlignmentAndLayoutButtons();
            this.UpdateSendBackBringForwardButtons();
            this.UpdateGroupingButtons();
            this.UpdateCustomButtons();
        }

        protected override void OnNewDiagram()
        {
            base.OnNewDiagram();
            this.DisableAllItems();            
        }

        #region Button Click Events

        void alignBottomEdgesToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.AlignBottomEdgesToolName);
        }

        void alignTopEdgesToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.AlignTopEdgesToolName);
        }

        void alignRightEdgesToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.AlignRightEdgesToolName);
        }

        void alignCentersVerticallyToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.AlignCentersVertToolName);
        }

        void alignCentersHorizontallyToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.AlignCentersHorizToolName);
        }

        void alignLeftEdgesToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.AlignLeftEdgesToolName);
        }

        void ungroupToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.UngroupToolName);
        }

        void groupToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.GroupToolName);
        }

        void bringForwardsToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.SendForwardsToolName);
        }

        void bringToFrontToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.SendToFrontToolName);
        }

        void sendBackwardsToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.SendBackwardsToolName);
        }

        void sendToBackToolButton_Click(object sender, EventArgs e)
        {
            StartTool(ControllerBase.SendToBackToolName);
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Starts the tool with the specified name.
        /// </summary>
        /// <param name="toolName">string: The name of the tool, such as
        /// "Rectangle Tool", "Ellipse Tool", "Connection Tool", 
        /// "Connector Mover Tool", "Text Tool"</param>
        // ------------------------------------------------------------------
        void StartTool(string toolName)
        {
            if (this.diagramControl != null)
            {
                try
                {
                    this.diagramControl.ActivateTool(toolName);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the 'Enabled' property to all alignment buttons to the value
        /// specified.
        /// </summary>
        /// <param name="enable">bool: If true, the buttons are enabled.</param>
        // ------------------------------------------------------------------
        protected void UpdateAlignmentAndLayoutButtons()
        {
            bool enabled = false;

            if (Selection.SelectedItems.Count >= 2)
            {
                enabled = true;
            }

            this.alignBottomEdgesToolButton.Enabled = enabled;
            this.alignCentersHorizontallyToolButton.Enabled = enabled;
            this.alignCentersVerticallyToolButton.Enabled = enabled;
            this.alignLeftEdgesToolButton.Enabled = enabled;
            this.alignRightEdgesToolButton.Enabled = enabled;
            this.alignTopEdgesToolButton.Enabled = enabled;

            // For the layouts to be enbabled, at least one shape must be
            // selected and it has to be attached (via connectors) to
            // at least one other shape.
            bool layoutsEnabled = false;
            if (Selection.SelectedItems.Count >= 1)
            {
                // Use the first selected shape as the root node.
                IDiagramEntity entity = Selection.SelectedItems[0];

                if (entity is IShape)
                {
                    //IShape shape = entity as IShape;
                    //INode node = shape as INode;
                    //if (node.OutDegree > 0)
                    //{
                    //    layoutsEnabled = true;
                    //    this.diagramControl.SetLayoutRoot(shape);
                    //}
                }
            }
            this.layoutButton.Enabled = layoutsEnabled;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// If there are one or more items selected, the send back, bring
        /// forward buttons are enbable.
        /// specified.
        /// </summary>
        // ------------------------------------------------------------------
        protected void UpdateSendBackBringForwardButtons()
        {
            bool enabled = false;

            if (Selection.SelectedItems.Count > 0)
            {
                enabled = true;
            }
            this.sendBackwardsToolButton.Enabled = enabled;
            this.sendToBackToolButton.Enabled = enabled;
            this.bringForwardsToolButton.Enabled = enabled;
            this.bringToFrontToolButton.Enabled = enabled;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Enables the grouping button if there are two or more entities
        /// selected.  The ungrouping buttin is enabled only if there is
        /// at least one IGroup in the selection.
        /// </summary>
        // ------------------------------------------------------------------
        protected void UpdateGroupingButtons()
        {
            bool groupingEnabled = false;
            bool ungroupingEnabled = false;
           
            if (Selection.SelectedItems.Count >= 2)
            {
                groupingEnabled = true;
            }
            this.groupToolButton.Enabled = groupingEnabled;

            // If there are groups in the selection then enable
            // the ungroup button.
            foreach (IDiagramEntity entity in 
                Selection.SelectedItems.ToArray())
            {
                if (entity is IGroup)
                {
                    ungroupingEnabled = true;
                    break;
                }
            }
            this.ungroupToolButton.Enabled = ungroupingEnabled;
        }

        public override void DisableAllItems()
        {
            base.DisableAllItems();
        }
    }
}
