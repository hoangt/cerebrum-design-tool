using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ToolBox
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// Provides information about a StringAlignmentChanged event.
    /// </summary>
    // ----------------------------------------------------------------------
    public class StringAlignmentChangedEventArgs : EventArgs
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// The old value (changed from).
        /// </summary>
        // ------------------------------------------------------------------
        StringAlignment oldValue;

        // ------------------------------------------------------------------
        /// <summary>
        /// The new value (changed to).
        /// </summary>
        // ------------------------------------------------------------------
        StringAlignment newValue;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the old StringAlignment (changed from).
        /// </summary>
        // ------------------------------------------------------------------
        public StringAlignment OldValue
        {
            get
            {
                return this.oldValue;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the new StringAlignment (changed to).
        /// </summary>
        // ------------------------------------------------------------------
        public StringAlignment NewValue
        {
            get
            {
                return this.newValue;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="oldValue">StringAlignment: The value changed 
        /// from.</param>
        /// <param name="newValue">StringAlignment: The value changed
        /// to.</param>
        // ------------------------------------------------------------------
        public StringAlignmentChangedEventArgs(
            StringAlignment oldValue, 
            StringAlignment newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }
    }
}
