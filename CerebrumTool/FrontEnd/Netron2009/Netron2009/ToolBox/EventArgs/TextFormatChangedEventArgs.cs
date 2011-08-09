using System;
using System.Collections.Generic;
using System.Text;
using ToolBox.Formatting;

namespace ToolBox
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// Provides information about a TextFormat changed event.
    /// </summary>
    // ----------------------------------------------------------------------
    public class TextFormatChangedEventArgs : EventArgs
    {
        TextFormat myFormat;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the new text format.
        /// </summary>
        // ------------------------------------------------------------------
        public TextFormat TextFormat
        {
            get
            {
                return myFormat;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="format">TextFormat: The new format.</param>
        // ------------------------------------------------------------------
        public TextFormatChangedEventArgs(TextFormat format)
        {
            myFormat = format;
        }
    }
}
