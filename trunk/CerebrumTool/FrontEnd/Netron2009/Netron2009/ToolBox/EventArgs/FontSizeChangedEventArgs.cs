using System;
using System.Collections.Generic;
using System.Text;

namespace ToolBox
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// Provides information about a font size changed event.
    /// </summary>
    // ----------------------------------------------------------------------
    public class FontSizeChangedEventArgs : EventArgs
    {
        float myNewSize;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the new font size.
        /// </summary>
        // ------------------------------------------------------------------
        public float Size
        {
            get
            {
                return myNewSize;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="newSize">float</param>
        // ------------------------------------------------------------------
        public FontSizeChangedEventArgs(float newSize)
        {
            myNewSize = newSize;
        }
    }
}
