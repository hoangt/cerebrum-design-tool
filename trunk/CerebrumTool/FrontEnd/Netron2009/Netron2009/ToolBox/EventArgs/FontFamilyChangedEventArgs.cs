using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ToolBox
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// Provides information about a FontFamilyChanged event.
    /// </summary>
    // ----------------------------------------------------------------------
    public class FontFamilyChangedEventArgs : EventArgs
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// The old value (changed from).
        /// </summary>
        // ------------------------------------------------------------------
        string oldName;

        // ------------------------------------------------------------------
        /// <summary>
        /// The new value (changed to).
        /// </summary>
        // ------------------------------------------------------------------
        string newName;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the old FontFamily name (changed from).
        /// </summary>
        // ------------------------------------------------------------------
        public string OldValue
        {
            get
            {
                return this.oldName;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the new FontFamily name (changed to).
        /// </summary>
        // ------------------------------------------------------------------
        public string NewValue
        {
            get
            {
                return this.newName;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oldValue">FontFamily: The old value.</param>
        /// <param name="newValue">FontFamily: The new value.</param>
        // ------------------------------------------------------------------
        public FontFamilyChangedEventArgs(
            string oldValue,
            string newValue)
        {
            this.oldName = oldValue;
            this.newName = newValue;
        }
    }
}
