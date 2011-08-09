using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ToolBox.Controls
{
    public class ToolStripFontSizePicker : ToolStripComboBox
    {   
        // ------------------------------------------------------------------
        /// <summary>
        /// Event raised when the Font size is changed by the user.  Setting
        /// our font size using the property 'FontSize' does NOT raise this
        /// event!
        /// </summary>
        // ------------------------------------------------------------------
        public event FontSizeChangedEventHandler FontSizeChanged;

        // ------------------------------------------------------------------
        /// <summary>
        /// The font size.
        /// </summary>
        // ------------------------------------------------------------------
        protected float myFontSize = 10;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the font size being displayed.  When used as a 
        /// setter, if the specified font size does not currently exist, then
        /// the specified size is added with the suffix "pt" added to the end.
        /// This new font size is set the current font size.
        /// </summary>
        // ------------------------------------------------------------------
        public float FontSize
        {
            get
            {
                return myFontSize;
            }
            set
            {
                myFontSize = value;

                foreach (object item in this.Items)
                {
                    if (ParseFontSize(item.ToString()) == myFontSize)
                    {
                        return;
                    }
                }

                // If we made it this far, then the specified size doesn't
                // exist so add it.
                this.AddSize(myFontSize);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public ToolStripFontSizePicker()
            : base()
        {
            // Populate the ComboBox with default selections.
            this.AddSize(6F);
            this.AddSize(8F);
            this.AddSize(9F);
            this.AddSize(10F);
            this.AddSize(12F);
            this.AddSize(14F);
            this.AddSize(18F);
            this.AddSize(24F);
            this.AddSize(30F);
            this.AddSize(36F);
            this.AddSize(48F);
            this.AddSize(60F);

            // 10 point size is the default.
            this.SelectedIndex = 3;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds the specified font size to the list of Items that will be
        /// viewed by the user.  The suffix " pt." is added to the end and is
        /// added to the list as a string.
        /// </summary>
        /// <param name="fontSize"></param>
        // ------------------------------------------------------------------
        protected void AddSize(float fontSize)
        {
            Items.Add(fontSize.ToString() + " pt.");
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the new value and raises the FontSizeChangedEvent.
        /// </summary>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        //protected override void OnSelectionChangeCommitted(EventArgs e)
        //{
        //    base.OnSelectionChangeCommitted(e);

        //    float parsedSize = ParseFontSize(this.Text);
        //    if (parsedSize != float.NaN)
        //    {
        //        myFontSize = parsedSize;
        //        RaiseFontSizeChangedEvent(myFontSize);
        //    }
        //}

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the new value and raises the FontSizeChangedEvent.
        /// </summary>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            float parsedSize = ParseFontSize(this.Text);
            if (parsedSize != float.NaN)
            {
                myFontSize = parsedSize;
                RaiseFontSizeChangedEvent(myFontSize);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// If the 'Enter' key is pressed, the text in the ComboBox is parsed
        /// to get the user-entered value and raises the 
        /// FontSizeChangedEvent.  This allows users to specify a font size
        /// other than what is provided in the default list.  The new value
        /// is added to the list.
        /// </summary>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Enter)
            {
                float parsedSize = ParseFontSize(this.Text);
                if (parsedSize != float.NaN)
                {
                    // Use the property to the new value gets added to the
                    // list if needed.
                    FontSize = parsedSize;
                    RaiseFontSizeChangedEvent(myFontSize);
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Removes the trailing "pt" from the supplied text and parses the
        /// text to a float.
        /// </summary>
        /// <param name="text">string</param>
        /// <returns>float: The parsed size.  If the text provided could not
        /// be parsed, then 'float.NaN' is returned.</returns>
        // ------------------------------------------------------------------
        protected float ParseFontSize(string text)
        {
            float parsedSize = float.NaN;

            // Remove the "pt." suffix and all leading and trailing white
            // spaces.
            string trimmed = text.Replace("pt.", "");
            trimmed = trimmed.Trim();
            try
            {
                // Now try to parse the text to a float.
                parsedSize = float.Parse(trimmed);
            }
            catch
            {
                // Should we do something with the error?
            }
            return parsedSize;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Raises the FontSizeChanged event.  This DOES NOT set the local
        /// reference to the current font size.
        /// </summary>
        /// <param name="newValue">float: The new font size.</param>
        // ------------------------------------------------------------------
        protected virtual void RaiseFontSizeChangedEvent(float newValue)
        {
            if (this.FontSizeChanged != null)
            {
                // Raise the event
                this.FontSizeChanged(
                    this,
                    new FontSizeChangedEventArgs(newValue));
            }
        }
    }
}
