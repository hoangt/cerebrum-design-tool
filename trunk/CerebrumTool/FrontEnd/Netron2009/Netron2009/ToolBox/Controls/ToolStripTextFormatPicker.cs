using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ToolBox.Formatting;

namespace ToolBox.Controls
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A ToolStripComboBox that displays the different string formatting
    /// values in the enum TextFormat and allows the user to select a
    /// format to use.  Register for the 'TextFormatChanged' event to be
    /// notified when a new format is selected.
    /// </summary>
    // ----------------------------------------------------------------------
    public class ToolStripTextFormatPicker : ToolStripComboBox
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Event raised when the TextFormat is changed.
        /// </summary>
        // ------------------------------------------------------------------
        public event TextFormatChangedEventHandler TextFormatChanged;

        // ------------------------------------------------------------------
        /// <summary>
        /// Our current format.
        /// </summary>
        // ------------------------------------------------------------------
        TextFormat myFormat = TextFormat.String;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the TextFormat being displayed.
        /// </summary>
        // ------------------------------------------------------------------
        public TextFormat TextFormat
        {
            get
            {
                return myFormat;
            }
            set
            {
                myFormat = value;
                GoToFormat(myFormat);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public ToolStripTextFormatPicker()
            : base()
        {
            // Get the possible enum values like this so we don't have to
            // update this when new values are added/removed.
            string[] names = Enum.GetNames(typeof(TextFormat));
            foreach (string name in names)
            {
                Items.Add(name);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Makes the specified TextFormat the one being viewed in the
        /// ComboBox.
        /// </summary>
        /// <param name="format">TextFormat</param>
        // ------------------------------------------------------------------
        protected void GoToFormat(TextFormat format)
        {
            int index = 0;
            foreach (object item in Items)
            {
                if (item.ToString() == format.ToString())
                {
                    SelectedIndex = index;
                    return;
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns if the specified text is a valid TextFormat.
        /// </summary>
        /// <param name="text">string</param>
        /// <returns>bool</returns>
        // ------------------------------------------------------------------
        protected bool IsValidTextFormat(string text)
        {
            string[] names = Enum.GetNames(typeof(TextFormat));
            foreach (string name in names)
            {
                if (text == name)
                {
                    return true;
                }
            }

            return false;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Get the Text and converts it to a TextFormat value.
        /// </summary>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            TextFormat format;

            // Make sure the current text is a legal value?

            try
            {
                format = (TextFormat) Enum.Parse(typeof(TextFormat), Text);
                myFormat = format;
                RaiseTextFormatChangedEvent(myFormat);
            }
            catch
            {
                // Should we do something with the error?
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Raises the TextFormatChanged event.  This DOES NOT set the local
        /// reference to the supplied TextFormat.
        /// </summary>
        /// <param name="format">TextFormat: The new TextFormat.</param>
        // ------------------------------------------------------------------
        protected virtual void RaiseTextFormatChangedEvent(
            TextFormat format)
        {
            if (this.TextFormatChanged != null)
            {
                // Raise the event
                this.TextFormatChanged(
                    this,
                    new TextFormatChangedEventArgs(format));
            }
        }
    }
}
