using System;
using System.ComponentModel;
using System.Globalization;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// PainterCollectionTypeConverter is a special type converter which will 
    /// be associated with the PainterCollection class.  It converts a 
    /// PainterCollection object to string representation for use in a 
    /// property grid.
    /// </summary>
    // ----------------------------------------------------------------------
    public class RendererCollectionTypeConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destType)
        {
            if (destType == typeof(string) && value is RendererCollection)
            {
                // Cast the value to a Painter type
                RendererCollection pc = (RendererCollection)value;

                // Return a brief description.
                return "List of graphics painters.";
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
