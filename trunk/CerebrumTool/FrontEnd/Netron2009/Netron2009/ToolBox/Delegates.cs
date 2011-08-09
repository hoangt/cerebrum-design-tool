using System;
using System.Collections.Generic;
using System.Text;

namespace ToolBox
{
    // ------------------------------------------------------------------
    /// <summary>
    /// Delegate for the FontSizeChanged event.
    /// </summary>
    // ------------------------------------------------------------------
    public delegate void FontSizeChangedEventHandler(object sender,
        FontSizeChangedEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Delegate for the StringAlignmentChanged event.
    /// </summary>
    // ------------------------------------------------------------------
    public delegate void StringAlignmentChangedEventHandler(object sender,
        StringAlignmentChangedEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Delegate for the FontFamilyChanged event.
    /// </summary>
    // ------------------------------------------------------------------
    public delegate void FontFamilyChangedEventHandler(object sender,
        FontFamilyChangedEventArgs e);

    // ------------------------------------------------------------------
    /// <summary>
    /// Delegate for the TextFormatChanged event.
    /// </summary>
    // ------------------------------------------------------------------
    public delegate void TextFormatChangedEventHandler(object sender,
        TextFormatChangedEventArgs e);
}
