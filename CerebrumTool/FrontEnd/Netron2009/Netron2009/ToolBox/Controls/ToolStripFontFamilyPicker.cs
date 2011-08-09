using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ToolBox.Controls
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A ToolStripComboBox that contains all FontFamily names in
    /// System.Drawing.FontFamily.Families.
    /// </summary>
    // ----------------------------------------------------------------------
    public class ToolStripFontFamilyPicker : ToolStripComboBox
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Event raised when the FontFamily is changed.
        /// </summary>
        // ------------------------------------------------------------------
        public event FontFamilyChangedEventHandler FontFamilyChanged;

        // ------------------------------------------------------------------
        /// <summary>
        /// The name of the current FontFamily selected.
        /// </summary>
        // ------------------------------------------------------------------
        string familyName = "Arial";

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the name of the selected FontFamily.
        /// </summary>
        // ------------------------------------------------------------------
        public string FamilyName
        {
            get
            {
                return this.familyName;
            }
            set
            {
                this.familyName = value;
                this.SelectedItem = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public ToolStripFontFamilyPicker()
            : base()
        {
            foreach (FontFamily fontFamily in FontFamily.Families)
            {
                this.Items.Add(fontFamily.Name);
            }

            // Arial is the default font.
            this.SelectedItem = "Arial";
            this.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the new value and raises the FontFamilyChangedEvent.
        /// </summary>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            // When the selected value is changed, get the current (old) 
            // value, tell the base ComboBox Control to handle the 
            // SelectedValueChanged, get the new name, and finally raise the
            // FontFamilyChanged event if it's not null.
            string oldValue = this.familyName;
            base.OnSelectionChangeCommitted(e);
            this.familyName = this.SelectedItem.ToString();
            this.RaiseFontFamilyChangedEvent(oldValue, this.familyName);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Raises the FontFamilyChanged event.  This DOES NOT set the local
        /// reference to the current familyName.
        /// </summary>
        /// <param name="oldValue">string: The name of the old 
        /// FontFamily.</param>
        /// <param name="newValue">string: The name of the new 
        /// FontFamily.</param>
        // ------------------------------------------------------------------
        protected virtual void RaiseFontFamilyChangedEvent(
            string oldValue,
            string newValue)
        {
            if (this.FontFamilyChanged != null)
            {
                // Raise the event
                this.FontFamilyChanged(
                    this,
                    new FontFamilyChangedEventArgs(oldValue, newValue));
            }
        }
    }
}
