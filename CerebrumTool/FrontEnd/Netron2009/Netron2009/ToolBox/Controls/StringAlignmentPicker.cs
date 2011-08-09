using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ToolBox.Controls
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A ComboBox that contains a list of the possible string alignments:
    /// Near, Center, and Far.
    /// </summary>
    // ----------------------------------------------------------------------
    public class StringAlignmentPicker : ComboBox
    {       
        // ------------------------------------------------------------------
        /// <summary>
        /// Event raised when a new StringAlignment is selected.
        /// </summary>
        // ------------------------------------------------------------------
        public event StringAlignmentChangedEventHandler StringAlignmentChanged;

        // ------------------------------------------------------------------
        /// <summary>
        /// The currently selected StringAlignment.
        /// </summary>
        // ------------------------------------------------------------------
        StringAlignment stringAlignment = StringAlignment.Near;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the selected StringAlignment.
        /// </summary>
        // ------------------------------------------------------------------
        public StringAlignment StringAlignment
        {
            get
            {
                return this.stringAlignment;
            }
            set
            {
                this.stringAlignment = value;
                this.SelectedItem = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public StringAlignmentPicker() : base()
        {
            this.DropDownStyle = ComboBoxStyle.DropDownList;
            this.Items.Add(StringAlignment.Near);
            this.Items.Add(StringAlignment.Center);
            this.Items.Add(StringAlignment.Far);
            this.SelectedIndex = 0;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the new StringAlignment value and raises the 
        /// StringAlignmentChanged event.
        /// </summary>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected override void OnSelectedValueChanged(EventArgs e)
        {
            try
            {
                StringAlignment oldValue = this.stringAlignment;
                base.OnSelectedItemChanged(e);
                this.stringAlignment = (StringAlignment)Enum.Parse(
                    typeof(StringAlignment),
                    this.Text);
                this.RaiseStringAlignmentChangedEvent(
                    oldValue,
                    this.stringAlignment);
            }
            catch
            {
                MessageBox.Show("String alignment " + this.Text +
                    " is not a valid value.  Your choices are:\n" +
                    "Near\n" +
                    "Center\n" +
                    "Far");
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Raises the StringAlignmentChanged event.  This DOES NOT change the
        /// local reference to the current StringAlignment (stringAlignment).
        /// </summary>
        /// <param name="oldValue">string: The name of the old 
        /// FontFamily.</param>
        /// <param name="newValue">string: The name of the new 
        /// FontFamily.</param>
        // ------------------------------------------------------------------
        protected virtual void RaiseStringAlignmentChangedEvent(
            StringAlignment oldValue,
            StringAlignment newValue)
        {
            if (this.StringAlignmentChanged != null)
            {
                // Raise the event
                this.StringAlignmentChanged(
                    this,
                    new StringAlignmentChangedEventArgs(oldValue, newValue));
            }
        }
    }
}
