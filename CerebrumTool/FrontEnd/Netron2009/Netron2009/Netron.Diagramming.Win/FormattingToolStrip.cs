using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OfficePickers.ColorPicker;
using System.Drawing;
using System.Collections;
using System.Diagnostics;

namespace Netron.NetronLight.UI.Win
{
    public class FormattingToolStrip : DiagramBaseToolStrip
    {
        #region Fields

        ArrayList selectedShapes = new ArrayList();

        /// <summary>
        /// Specifies if changes to the selected shapes is allowed.  This
        /// is set to false when we're updating the colors to show in the
        /// color picker buttons so the shapes aren't changed.
        /// </summary>
        protected bool allowChanges = true;

        // ------------------------------------------------------------------
        /// <summary>
        /// The ToolStrip button that allows the shape's color to be changed.
        /// </summary>
        // ------------------------------------------------------------------
        ToolStripColorPicker solidFillColorButton = 
            new ToolStripColorPicker(Color.White);

        // ------------------------------------------------------------------
        /// <summary>
        /// The ToolStrip button that allows the shape's color to be changed.
        /// </summary>
        // ------------------------------------------------------------------
        ToolStripColorPicker lineColorButton =
            new ToolStripColorPicker(Color.Black);

        // ------------------------------------------------------------------
        /// <summary>
        /// The ToolStrip button that allows the shape's color to be changed.
        /// </summary>
        // ------------------------------------------------------------------
        ToolStripColorPicker textColorButton =
            new ToolStripColorPicker(Color.Black);

        // ------------------------------------------------------------------
        /// <summary>
        /// The ToolStrip button that sets the horizontal alignment of a
        /// TextBlock to 'Near' for left alignment.
        /// </summary>
        // ------------------------------------------------------------------
        ToolStripButton textLeftAlignmentButton = new ToolStripButton();

        // ------------------------------------------------------------------
        /// <summary>
        /// The ToolStrip button that sets the horizontal alignment of a
        /// TextBlock to 'Center'.
        /// </summary>
        // ------------------------------------------------------------------
        ToolStripButton textCenterAlignmentButton = new ToolStripButton();

        // ------------------------------------------------------------------
        /// <summary>
        /// The ToolStrip button that sets the horizontal alignment of a
        /// TextBlock to 'Far' for right alignment.
        /// </summary>
        // ------------------------------------------------------------------
        ToolStripButton textRightAlignmentButton = new ToolStripButton();

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public FormattingToolStrip()
            : base()
        {
            this.solidFillColorButton.Image = ImagePallet.BucketFill;
            this.solidFillColorButton.DisplayStyle = ToolStripItemDisplayStyle.None;
            this.solidFillColorButton.ButtonDisplayStyle = 
                ToolStripColorPickerDisplayType.UnderLineAndImage;
            //this.solidFillColorButton.ImageTransparentColor = Color.Magenta; ;
            this.solidFillColorButton.Text = "Fill";
            this.solidFillColorButton.ToolTipText = "Fill";

            this.lineColorButton.Image = ImagePallet.Outline;
            this.lineColorButton.DisplayStyle = ToolStripItemDisplayStyle.None;
            this.lineColorButton.ButtonDisplayStyle =
                ToolStripColorPickerDisplayType.UnderLineAndImage;
            //this.lineColorButton.ImageTransparentColor = Color.Magenta; ;
            this.lineColorButton.Text = "Line Color";
            this.lineColorButton.ToolTipText = "Line Color";

            this.textColorButton.Image = ImagePallet.Font;
            this.textColorButton.DisplayStyle = ToolStripItemDisplayStyle.None;
            this.textColorButton.ButtonDisplayStyle =
                ToolStripColorPickerDisplayType.UnderLineAndImage;
            //this.textColorButton.ImageTransparentColor = Color.Magenta; ;
            this.textColorButton.Text = "Text Color";
            this.textColorButton.ToolTipText = "Text Color";

            this.textLeftAlignmentButton.Image = ImagePallet.LeftAlignment;
            this.textLeftAlignmentButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //this.textLeftAlignmentButton.ImageTransparentColor = Color.Silver;
            this.textLeftAlignmentButton.Text = "Align Left";
            this.textLeftAlignmentButton.ToolTipText = "Align Left";

            this.textCenterAlignmentButton.Image = ImagePallet.CenterAlignment;
            this.textCenterAlignmentButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //this.textCenterAlignmentButton.ImageTransparentColor = Color.Silver;
            this.textCenterAlignmentButton.Text = "Align Center";
            this.textCenterAlignmentButton.ToolTipText = "Align Center";
            // By default, show the center alignment button as being selected.
            
            this.textRightAlignmentButton.Image = ImagePallet.RightAlignment;
            this.textRightAlignmentButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            //this.textRightAlignmentButton.ImageTransparentColor = Color.Silver;
            this.textRightAlignmentButton.Text = "Align Right";
            this.textRightAlignmentButton.ToolTipText = "Align Right";

            this.Items.Add(this.textLeftAlignmentButton);
            this.Items.Add(this.textCenterAlignmentButton);
            this.Items.Add(this.textRightAlignmentButton);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(this.textColorButton);
            this.Items.Add(this.lineColorButton); 
            this.Items.Add(this.solidFillColorButton);

            this.textLeftAlignmentButton.Click += 
                new EventHandler(textLeftAlignmentButton_Click);

            this.textCenterAlignmentButton.Click += 
                new EventHandler(textCenterAlignmentButton_Click);

            this.textRightAlignmentButton.Click += 
                new EventHandler(textRightAlignmentButton_Click);

            this.textColorButton.SelectedColorChanged += 
                new EventHandler(textColorButton_SelectedColorChanged);

            Selection.OnNewSelection += new EventHandler(Selection_OnNewSelection);
            this.solidFillColorButton.SelectedColorChanged +=
                new EventHandler(SelectedFillColorChanged);

            this.lineColorButton.SelectedColorChanged +=
                new EventHandler(SelectedLineColorChanged);

            this.DisableAllItems();
        }

        void textRightAlignmentButton_Click(object sender, EventArgs e)
        {
            if (allowChanges == false)
            {
                return;
            }

            foreach (IShape shape in this.selectedShapes)
            {
                if (shape.TextBlock != null)
                {
                    shape.TextBlock.HorizontalAlignment = StringAlignment.Far;
                    this.diagramControl.Invalidate(shape.Rectangle);
                }
            }
        }

        void textCenterAlignmentButton_Click(object sender, EventArgs e)
        {
            if (allowChanges == false)
            {
                return;
            }

            foreach (IShape shape in this.selectedShapes)
            {
                if (shape.TextBlock != null)
                {
                    shape.TextBlock.HorizontalAlignment = StringAlignment.Center;
                    this.diagramControl.Invalidate(shape.Rectangle);
                }
            }
        }

        void textLeftAlignmentButton_Click(object sender, EventArgs e)
        {
            if (allowChanges == false)
            {
                return;
            }

            foreach (IShape shape in this.selectedShapes)
            {
                if (shape.TextBlock != null)
                {
                    shape.TextBlock.HorizontalAlignment = StringAlignment.Near;
                    this.diagramControl.Invalidate(shape.Rectangle);
                }
            }
        }

        void textColorButton_SelectedColorChanged(object sender, EventArgs e)
        {
            if (allowChanges == false)
            {
                return;
            }

            foreach (IShape shape in this.selectedShapes)
            {
                if (shape.TextBlock != null)
                {
                    shape.TextBlock.TextFill.SolidColor =
                        this.textColorButton.Color;
                    this.diagramControl.Invalidate(shape.Rectangle);
                }
            }
            this.Select();
        }

        void SelectedLineColorChanged(object sender, EventArgs e)
        {
            if (allowChanges == false)
            {
                return;
            }

            foreach (IShape shape in this.selectedShapes)
            {
                if (shape.Background != null)
                {
                    shape.Background.LineColor = lineColorButton.Color;
                    this.diagramControl.Invalidate(shape.Rectangle);
                }
            }
            this.Select();
        }

        void SelectedFillColorChanged(object sender, EventArgs e)
        {
            if (allowChanges == false)
            {
                return;
            }

            foreach (IShape shape in this.selectedShapes)
            {
                if (shape.Background != null)
                {
                    shape.Background.Fill.SolidColor = 
                        solidFillColorButton.Color;

                    this.diagramControl.Invalidate(shape.Rectangle);
                }
            }
            this.Select();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Disables all ToolStripItems and all ToolStripColorPicker's.
        /// </summary>
        // ------------------------------------------------------------------
        public override void DisableAllItems()
        {
            base.DisableAllItems();
            this.solidFillColorButton.Enabled = false;
            this.lineColorButton.Enabled = false;
            this.textColorButton.Enabled = false;
        }
        // ------------------------------------------------------------------
        /// <summary>
        /// Disables all ToolStripItems and all ToolStripColorPicker's.
        /// </summary>
        // ------------------------------------------------------------------
        public override void EnableAllItems()
        {
            base.EnableAllItems();
            this.solidFillColorButton.Enabled = true;
            this.lineColorButton.Enabled = true;
            this.textColorButton.Enabled = true;
        }


        void Selection_OnNewSelection(object sender, EventArgs e)
        {
            this.selectedShapes.Clear();
            
            foreach (IDiagramEntity entity in Selection.SelectedItems.ToArray())
            {
                if (entity is IShape)
                {
                    IShape shape = entity as IShape;
                    if ( (shape.Background != null) || 
                        (shape.TextBlock != null) )
                    {
                        this.selectedShapes.Add(shape);
                    }
                }
            }

            if (this.selectedShapes.Count > 0)
            {
                // Don't allow changes while we're updating the tool strip.
                this.allowChanges = false;
                this.EnableAllItems();
                IShape shape = (IShape)this.selectedShapes[0];
                if (shape.Background != null)
                {
                    this.solidFillColorButton.Color =
                        shape.Background.Fill.SolidColor;
                    this.lineColorButton.Color =
                        shape.Background.LineColor;
                }

                if (shape.TextBlock != null)
                {
                    this.textColorButton.Color =
                        shape.TextBlock.TextFill.SolidColor;
                }

                // Now we can allow changes.
                this.allowChanges = true;
            }
            else
            {
                this.DisableAllItems();
            }
        }
    }
}
