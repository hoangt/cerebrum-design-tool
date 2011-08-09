using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ToolBox;
using ToolBox.OfficePickers;
using ToolBox.Controls;

using Netron.Diagramming.Core;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A ToolStrip that allows the user to change the formatting (color, 
    /// line style, text alignment, etc.) of selected entities.
    /// </summary>
    // ----------------------------------------------------------------------
    public class FormatToolStrip : DiagramBaseToolStrip
    {
        #region Buttons

        // Color buttons.
        ToolStripColorPicker myFillButton = new ToolStripColorPicker();
        ToolStripColorPicker myLineColorButton = new ToolStripColorPicker();
        ToolStripColorPicker myTextColorButton = new ToolStripColorPicker();

        // Line style buttons.
        ToolStripLineStylePicker myLineStyleButton = 
            new ToolStripLineStylePicker();

        ToolStripLineWidthPicker myLineWeightPicker = 
            new ToolStripLineWidthPicker();

        ToolStripLineCapPicker myLineCapPicker = new ToolStripLineCapPicker();

        // Text style items.
        ToolStripFontFamilyPicker myFontFamilyPicker = 
            new ToolStripFontFamilyPicker();

        ToolStripFontSizePicker myFontSizePicker =
            new ToolStripFontSizePicker();

        ToolStripButton myBoldFontButton = new ToolStripButton();
        ToolStripButton myItalicsFontButton = new ToolStripButton();
        ToolStripButton myUnderlineFontButton = new ToolStripButton();

        ToolStripButton myLeftTextAlignButton = new ToolStripButton();
        ToolStripButton myCenterTextAlignButton = new ToolStripButton();
        ToolStripButton myRightTextAlignButton = new ToolStripButton();

        // Text formatting
        ToolStripTextFormatPicker myTextFormatPicker = 
            new ToolStripTextFormatPicker();

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public FormatToolStrip()
            : base()
        {
            SetText();
            SetDisplayStyle();
            SetImage();
            AddItems();

            // The default text alignment is centered.  I know this because I
            // hard coded it!  If it's changed, it'll have to be changed
            // here.
            SetDefaultTextStyleButtons();

            RegisterEvents();

            // Watch when the Selection of entities in the canvas changes so
            // we can update the state of the buttons (enable/disable).
            Selection.OnNewSelection +=
                new EventHandler(Selection_OnNewSelection);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the 'Checked' property for the center align button to
        /// true and all others to false.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetDefaultTextStyleButtons()
        {
            myUnderlineFontButton.Checked = false;
            myItalicsFontButton.Checked = false;
            myBoldFontButton.Checked = false;

            myLeftTextAlignButton.Checked = false;
            myCenterTextAlignButton.Checked = true;
            myRightTextAlignButton.Checked = false;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Disables all buttons if there are no entities selected, enables
        /// all if there are.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        void Selection_OnNewSelection(object sender, EventArgs e)
        {
            if (Selection.SelectedItems == null)
            {
                return;
            }
            if (Selection.SelectedItems.Count > 0)
            {
                this.EnableAllItems();

                // Update the text styld buttons for the first entity in the
                // selection that implements ITextProvider.
                ITextStyle textStyle = null;
                foreach (IDiagramEntity entity in Selection.SelectedItems)
                {
                    if (entity is ITextProvider)
                    {
                        textStyle = (entity as ITextProvider).TextStyle;
                        myUnderlineFontButton.Checked = textStyle.IsUnderlined;
                        myItalicsFontButton.Checked = textStyle.IsItalic;
                        myBoldFontButton.Checked = textStyle.IsBold;

                        switch (textStyle.VerticalAlignment)
                        {
                            case StringAlignment.Near :
                                myLeftTextAlignButton.Checked = true;
                                myCenterTextAlignButton.Checked = false;
                                myRightTextAlignButton.Checked = false;
                                break;

                            case StringAlignment.Center :
                                myLeftTextAlignButton.Checked = false;
                                myCenterTextAlignButton.Checked = true;
                                myRightTextAlignButton.Checked = false;
                                break;

                            case StringAlignment.Far :
                                myLeftTextAlignButton.Checked = false;
                                myCenterTextAlignButton.Checked = false;
                                myRightTextAlignButton.Checked = true;
                                break;
                        }

                        break;
                    }
                }

                // Use our default settings if an ITextProvider is not selected.
                if (textStyle == null)
                {
                    SetDefaultTextStyleButtons();
                }
            }
            else
            {
                this.DisableAllItems();
                SetDefaultTextStyleButtons();
            }

            // The fill button is always enabled because if there aren't
            // any selected items then we change the background color of
            // the current page.
            this.myFillButton.Enabled = true;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the Text property for each button.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetText()
        {
            this.myFillButton.ToolTipText = "Fill Color";
            this.myLineColorButton.ToolTipText = "Line Color";
            this.myTextColorButton.ToolTipText = "Text Color";

            this.myLineStyleButton.Text = "Line Style";
            this.myLineWeightPicker.ToolTipText = "Line Weight";
            this.myLineCapPicker.ToolTipText = "Line Ends";

            this.myFontFamilyPicker.ToolTipText = "Font";
            this.myFontSizePicker.ToolTipText = "Font Size";
            this.myBoldFontButton.Text = "Bold";
            this.myItalicsFontButton.Text = "Italic";
            this.myUnderlineFontButton.Text = "U";
            this.myUnderlineFontButton.ToolTipText = "Underline";

            FontStyle style = FontStyle.Underline | FontStyle.Bold;
            this.myUnderlineFontButton.Font = new Font(
                "Bookman Old Style",
                9,
                style);

            this.myLeftTextAlignButton.Text = "Align Left";
            this.myCenterTextAlignButton.Text = "Align Center";
            this.myRightTextAlignButton.Text = "Align Right";

            this.myTextFormatPicker.ToolTipText = "Text Format";
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the DisplayStyle property for each button to 'Image'.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetDisplayStyle()
        {
            this.myFillButton.ButtonDisplayStyle = 
                ToolStripPickerDisplayType.UnderLineAndImage;

            myTextColorButton.ButtonDisplayStyle = 
                ToolStripPickerDisplayType.UnderLineAndImage;

            this.myLineColorButton.ButtonDisplayStyle =
                ToolStripPickerDisplayType.UnderLineAndImage;

            this.myLineStyleButton.ButtonDisplayStyle =
                ToolStripPickerDisplayType.NormalImage;
            this.myLineStyleButton.Size = new Size(30, 23);

            this.myLineWeightPicker.ButtonDisplayStyle =
                ToolStripPickerDisplayType.NormalImage;
            this.myLineWeightPicker.Size = new Size(30, 23);

            this.myLineCapPicker.ButtonDisplayStyle =
                ToolStripPickerDisplayType.NormalImage;
            this.myLineCapPicker.Size = new Size(30, 23);

            this.myLeftTextAlignButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myCenterTextAlignButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myRightTextAlignButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myBoldFontButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myItalicsFontButton.DisplayStyle =
                ToolStripItemDisplayStyle.Image;

            this.myUnderlineFontButton.DisplayStyle =
                ToolStripItemDisplayStyle.Text;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image from the ImagePalette for each button.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void SetImage()
        {
            this.myFillButton.Image = Images.BucketFill;

            this.myLineColorButton.Image = Images.Outline;

            this.myTextColorButton.Image = Images.Font;

            this.myLineStyleButton.Image = Images.DashStyles;
            this.myLineWeightPicker.Image = Images.LineWeights;
            this.myLineCapPicker.Image = Images.LineCaps;

            this.myBoldFontButton.Image = Images.Bold;

            this.myItalicsFontButton.Image = Images.Italic;

            this.myLeftTextAlignButton.Image = Images.LeftAlignment;
            this.myCenterTextAlignButton.Image = Images.CenterAlignment;
            this.myRightTextAlignButton.Image = Images.RightAlignment;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds all buttons to the tool strip.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void AddItems()
        {
            // Adding items so this toolbar is similar to the formatting
            // toolbar in Microsoft Visio.

            // First add our text format picker.  This is the only thing that
            // Visio doesn't have in their formatting toolbar.
            Items.Add(myTextFormatPicker);

            // Separate the text format picker from the next group.
            this.Items.Add(new ToolStripSeparator());

            // Next the font editors.
            this.Items.Add(this.myFontFamilyPicker);
            this.Items.Add(this.myFontSizePicker);

            // Separate the font editors from the next group.
            this.Items.Add(new ToolStripSeparator());

            // Next the font style editors (bold, italics, underline).
            Items.Add(myBoldFontButton);
            Items.Add(myItalicsFontButton);
            Items.Add(myUnderlineFontButton);

            // Separate the font style editors from the next group.            
            this.Items.Add(new ToolStripSeparator());

            // Next are the string alignment editors (left, center, right text
            // alignment).
            this.Items.Add(this.myLeftTextAlignButton);
            this.Items.Add(this.myCenterTextAlignButton);
            this.Items.Add(this.myRightTextAlignButton);

            // Separate the string alignment editors from the next group.            
            this.Items.Add(new ToolStripSeparator());

            // Next are the color editors (text color, fill, line).
            this.Items.Add(this.myTextColorButton);
            this.Items.Add(this.myFillButton);
            this.Items.Add(this.myLineColorButton);

            // Separate the color editors from the next group.
            this.Items.Add(new ToolStripSeparator());

            // Lastly are the line editors (line weight, style, and line
            // caps).
            this.Items.Add(this.myLineWeightPicker);            
            this.Items.Add(this.myLineStyleButton);
            this.Items.Add(this.myLineCapPicker);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Register's for a Click event for each button.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void RegisterEvents()
        {
            this.myTextFormatPicker.TextFormatChanged += 
                new TextFormatChangedEventHandler(HandleTextFormatChanged);

            this.myTextColorButton.SelectedColorChanged +=
                new EventHandler(HandleSelectedTextColorChanged);

            this.myFillButton.SelectedColorChanged +=
                new EventHandler(SelectedFillColorChanged);

            this.myLineColorButton.SelectedColorChanged +=
                new EventHandler(SelectedLineColorChanged);

            this.myLineStyleButton.SelectedStyleChanged +=
                new EventHandler(SelectedLineStyleChanged);

            this.myLineWeightPicker.SelectedWidthChanged +=
                new EventHandler(HandleLineWeightChanged);

            this.myLineCapPicker.SelectedCapChanged +=
                new EventHandler(HandleLineEndsChanged);

            this.myFontFamilyPicker.FontFamilyChanged += 
                new FontFamilyChangedEventHandler(
                HandleFontFamilyChanged);

            this.myFontSizePicker.FontSizeChanged += 
                new FontSizeChangedEventHandler(
                HandleFontSizeChanged);

            this.myLeftTextAlignButton.Click +=
                new EventHandler(HandleLeftTextAlignButtonClick);

            this.myCenterTextAlignButton.Click +=
                new EventHandler(HandleCenterTextAlignButtonClick);

            this.myRightTextAlignButton.Click +=
                new EventHandler(HandleRightTextAlignButtonClick);

            this.myBoldFontButton.Click +=
                new EventHandler(HandleBoldFontButtonClick);

            this.myItalicsFontButton.Click +=
                new EventHandler(HandleItalicsFontButtonClick);

            this.myUnderlineFontButton.Click +=
                new EventHandler(HandleUnderlineFontButtonClick);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the TextFormat of all selected entities that implement
        /// ITextProvider.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">TextFormatChangedEventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleTextFormatChanged(
            object sender, 
            TextFormatChangedEventArgs e)
        {
            ITextProvider textProvider;
            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity is ITextProvider)
                {
                    textProvider = entity as ITextProvider;

                    try
                    {
                        textProvider.TextStyle.TextFormat = e.TextFormat;
                    }
                    catch
                    {
                        // What to do with the error?
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the text color all selected entities that implement 
        /// ITextProvider to the selected color.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleSelectedTextColorChanged(
            object sender, 
            EventArgs e)
        {
            ITextProvider textProvider;
            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity is ITextProvider)
                {
                    textProvider = entity as ITextProvider;

                    try
                    {
                        textProvider.TextStyle.FontColor = 
                            myTextColorButton.Color;
                    }
                    catch
                    {
                        MessageBox.Show("The text color cannot be " +
                            "changed to the value specified.",
                            "Error Changing Text Color",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the font style of all selected entities that implement 
        /// ITextProvider to have underlined font.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleUnderlineFontButtonClick(
            object sender, 
            EventArgs e)
        {
            // Toggle the checked state.  The new state will be used to
            // specify if underlined font is used.
            myUnderlineFontButton.Checked = !myUnderlineFontButton.Checked;
            ITextProvider textProvider;

            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity is ITextProvider)
                {
                    textProvider = entity as ITextProvider;

                    try
                    {
                        textProvider.TextStyle.IsUnderlined =
                            myUnderlineFontButton.Checked;
                    }
                    catch
                    {
                        MessageBox.Show(
                            "The current font cannot be underlined.",
                            "Error Changing Font Style",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the font style of all selected entities that implement 
        /// ITextProvider to have italic font.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleItalicsFontButtonClick(
            object sender, 
            EventArgs e)
        {
            // Toggle the checked state.  The new state will be used to
            // spcify if italic font is used.
            myItalicsFontButton.Checked = !myItalicsFontButton.Checked;
            ITextProvider textProvider;

            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity is ITextProvider)
                {
                    textProvider = entity as ITextProvider;

                    try
                    {
                        textProvider.TextStyle.IsItalic =
                            myItalicsFontButton.Checked;
                    }
                    catch
                    {
                        MessageBox.Show(
                            "The current font cannot be italic.",
                            "Error Changing Font Style",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the font style of all selected entities that implement 
        /// ITextProvider to have bold font.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleBoldFontButtonClick(
            object sender, 
            EventArgs e)
        {
            // Toggle the checked state.  The new state will be used to
            // spcify if bold font is used.
            myBoldFontButton.Checked = !myBoldFontButton.Checked;
            ITextProvider textProvider;

            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity is ITextProvider)
                {
                    textProvider = entity as ITextProvider;

                    try
                    {
                        // Create a new font but use the old font size and
                        // style (we only want to change the font family).
                        textProvider.TextStyle.IsBold = 
                            myBoldFontButton.Checked;
                    }
                    catch
                    {
                        MessageBox.Show(
                            "The current font cannot be bold.",
                            "Error Changing Font Style",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }        

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the horizontal alignment of all selected entities that
        /// implement ITextProvider to be right (far) aligned.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleRightTextAlignButtonClick(
            object sender,
            EventArgs e)
        {
            // First un-check the left and center align buttons and check
            // this one to give visual feedback it's selected.
            this.myCenterTextAlignButton.Checked = false;
            this.myRightTextAlignButton.Checked = true;
            this.myLeftTextAlignButton.Checked = false;
            this.SetTextHorizontalAlignment(StringAlignment.Far);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the horizontal alignment of all selected entities that
        /// implement ITextProvider to be center aligned.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleCenterTextAlignButtonClick(
            object sender,
            EventArgs e)
        {
            // First un-check the left and right align buttons and check
            // this one to give visual feedback it's selected.
            this.myCenterTextAlignButton.Checked = true;
            this.myRightTextAlignButton.Checked = false;
            this.myLeftTextAlignButton.Checked = false;
            this.SetTextHorizontalAlignment(StringAlignment.Center);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the horizontal alignment of all selected entities that
        /// implement ITextProvider to be left (near) aligned.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleLeftTextAlignButtonClick(
            object sender, 
            EventArgs e)
        {
            // First un-check the center and right align buttons and check
            // this one to give visual feedback it's selected.
            this.myCenterTextAlignButton.Checked = false;
            this.myRightTextAlignButton.Checked = false;
            this.myLeftTextAlignButton.Checked = true;
            this.SetTextHorizontalAlignment(StringAlignment.Near);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the font family of the selected entities if they implement
        /// ITextProvider.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">FontFamilyChangedEventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleFontFamilyChanged(
            object sender, 
            FontFamilyChangedEventArgs e)
        {
            ITextProvider textProvider;
            string fontFamilyName = e.NewValue;

            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity is ITextProvider)
                {
                    textProvider = entity as ITextProvider;

                    try
                    {
                        // Create a new font but use the old font size and
                        // style (we only want to change the font family).
                        textProvider.TextStyle.Font = new Font(
                            fontFamilyName,
                            textProvider.TextStyle.Font.Size,
                            textProvider.TextStyle.Font.Style);
                    }
                    catch
                    {
                        MessageBox.Show("Font " + fontFamilyName +
                            " is not a supported value.",
                            "Error Setting Font",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the font size of all selected entities that implement
        /// ITextProvider.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">FontSizeChangedEventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleFontSizeChanged(
            object sender,
            FontSizeChangedEventArgs e)
        {
            ITextProvider textProvider;

            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity is ITextProvider)
                {
                    textProvider = entity as ITextProvider;
                    textProvider.TextStyle.FontSize = e.Size;
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the line color for all selected entities to the color
        /// currently selected.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void SelectedLineColorChanged(
            object sender, 
            EventArgs e)
        {
            DashStyle lineStyle = this.myLineStyleButton.LineStyle;
            Color lineColor = this.myLineColorButton.Color;
            float width = this.myLineWeightPicker.LineWidth;

            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity.PenStyle != null)
                {
                    if (entity.PenStyle is PenStyle)
                    {
                        PenStyle penStyle = new PenStyle(
                            lineColor,
                            entity.PenStyle.DashStyle,
                            entity.PenStyle.Width);
                        entity.PenStyle = penStyle;
                    }
                    else if (entity.PenStyle is LinePenStyle)
                    {
                        LinePenStyle oldStyle =
                            entity.PenStyle as LinePenStyle;

                        LinePenStyle linePenStyle = new LinePenStyle();
                        linePenStyle.StartCap = oldStyle.StartCap;
                        linePenStyle.EndCap = oldStyle.EndCap;
                        linePenStyle.CustomEndCap = oldStyle.CustomEndCap;
                        linePenStyle.CustomStartCap = oldStyle.CustomStartCap;
                        linePenStyle.Color = lineColor;
                        linePenStyle.Width = oldStyle.Width;
                        linePenStyle.DashStyle = oldStyle.DashStyle;

                        entity.PenStyle = linePenStyle;
                    }
                }
                else
                {
                    entity.PenStyle = new PenStyle(
                        lineColor,
                        lineStyle,
                        width);
                }
                entity.Invalidate();
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the line style for all selected entities to the style
        /// specified.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void SelectedLineStyleChanged(
            object sender, 
            EventArgs e)
        {
            DashStyle lineStyle = this.myLineStyleButton.LineStyle;
            Color lineColor = this.myLineColorButton.Color;
            float width = this.myLineWeightPicker.LineWidth;

            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity.PenStyle != null)
                {
                    if (entity.PenStyle is PenStyle)
                    {
                        PenStyle penStyle = new PenStyle(
                            entity.PenStyle.Color,
                            lineStyle,
                            entity.PenStyle.Width);
                        entity.PenStyle = penStyle;
                    }
                    else if (entity.PenStyle is LinePenStyle)
                    {
                        LinePenStyle oldStyle =
                            entity.PenStyle as LinePenStyle;

                        LinePenStyle linePenStyle = new LinePenStyle();
                        linePenStyle.StartCap = oldStyle.StartCap;
                        linePenStyle.EndCap = oldStyle.EndCap;
                        linePenStyle.CustomEndCap = oldStyle.CustomEndCap;
                        linePenStyle.CustomStartCap = oldStyle.CustomStartCap;
                        linePenStyle.Color = oldStyle.Color;
                        linePenStyle.Width = oldStyle.Width;
                        linePenStyle.DashStyle = lineStyle;

                        entity.PenStyle = linePenStyle;
                    }
                }
                else
                {
                    entity.PenStyle = new PenStyle(
                        lineColor,
                        lineStyle,
                        width);
                }
                entity.Invalidate();
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the start and endcaps of each selected entity to
        /// the new selected value.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        void HandleLineEndsChanged(object sender, EventArgs e)
        {
            LinePenStyle penStyle;
            LineCap startCap = this.myLineCapPicker.GetLeftCap();
            LineCap endCap = this.myLineCapPicker.GetEndCap();

            DashStyle lineStyle = this.myLineStyleButton.LineStyle;            
            Color lineColor = this.myLineColorButton.Color;
            float width = this.myLineWeightPicker.LineWidth;

            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity.PenStyle != null)
                {
                    lineColor = entity.PenStyle.Color;
                    width = entity.PenStyle.Width;
                    lineStyle = entity.PenStyle.DashStyle;
                }
                penStyle = new LinePenStyle();
                penStyle.Color = lineColor;
                penStyle.DashStyle = lineStyle;
                penStyle.Width = width;

                if (startCap == LineCap.Custom)
                {
                    penStyle.StartCap = LineCap.Custom;
                    penStyle.CustomStartCap = LinePenStyle.GenerallizationCap;
                }
                else
                {
                    penStyle.StartCap = startCap;
                }

                if (endCap == LineCap.Custom)
                {
                    penStyle.EndCap = LineCap.Custom;
                    penStyle.CustomEndCap = LinePenStyle.GenerallizationCap;
                }
                else
                {
                    penStyle.EndCap = endCap;
                }

                entity.PenStyle = penStyle;
                entity.Invalidate();
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the line weight of each selected entity to the new
        /// selected value.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        void HandleLineWeightChanged(object sender, EventArgs e)
        {
            DashStyle lineStyle = this.myLineStyleButton.LineStyle;
            Color lineColor = this.myLineColorButton.Color;
            float width = this.myLineWeightPicker.LineWidth;

            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity.PenStyle != null)
                {
                    if (entity.PenStyle is PenStyle)
                    {
                        PenStyle penStyle = new PenStyle(
                            entity.PenStyle.Color,
                            entity.PenStyle.DashStyle,
                            width);
                        entity.PenStyle = penStyle;
                    }
                    else if (entity.PenStyle is LinePenStyle)
                    {
                        LinePenStyle oldStyle = 
                            entity.PenStyle as LinePenStyle;

                        LinePenStyle linePenStyle = new LinePenStyle();
                        linePenStyle.StartCap = oldStyle.StartCap;
                        linePenStyle.EndCap = oldStyle.EndCap;
                        linePenStyle.CustomEndCap = oldStyle.CustomEndCap;
                        linePenStyle.CustomStartCap = oldStyle.CustomStartCap;
                        linePenStyle.Color = oldStyle.Color;
                        linePenStyle.DashStyle = oldStyle.DashStyle;
                        linePenStyle.Width = width;

                        entity.PenStyle = linePenStyle;
                    }
                    else
                    {
                        entity.PenStyle.Width = width;
                    }
                }
                else
                {
                    entity.PenStyle = new PenStyle(
                        lineColor,
                        lineStyle,
                        width);
                }
                entity.Invalidate();
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Changes the paint style for all selected entities to the color
        /// specified.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void SelectedFillColorChanged(
            object sender, 
            EventArgs e)
        {
            IShape shape;
            Color fillColor = this.myFillButton.Color;

            if (Selection.SelectedItems.Count > 0)
            {
                foreach (IDiagramEntity entity in Selection.SelectedItems)
                {
                    // Should we change the color of all entities or just
                    // the shapes?  What about connections?  I wouldn't want
                    // a connection with a fill, but somebody might.
                    if (entity is IShape)
                    {
                        shape = entity as IShape;
                        shape.PaintStyle = new SolidPaintStyle(fillColor);
                        shape.Invalidate();
                    }
                }
            }
            else
            {
                IPage page = diagramControl.Controller.Model.CurrentPage;
                page.Ambience.PageBackgroundType = CanvasBackgroundTypes.FlatColor;
                page.Ambience.PageColor = fillColor;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the horizontal alignment of all selected entities that
        /// implement ITextProvider to the alignment specified.
        /// </summary>
        /// <param name="alignment">StringAlignment</param>
        // ------------------------------------------------------------------
        protected virtual void SetTextHorizontalAlignment(
            StringAlignment alignment)
        {
            ITextProvider textProvider;

            foreach (IDiagramEntity entity in Selection.SelectedItems)
            {
                if (entity is ITextProvider)
                {
                    textProvider = entity as ITextProvider;
                    textProvider.TextStyle.VerticalAlignment = alignment;
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Initializes the ToolStrip when a new diagram is attached.
        /// </summary>
        // ------------------------------------------------------------------
        protected override void OnNewDiagram()
        {
            base.OnNewDiagram();

            this.DisableAllItems();
        }

    }
}
