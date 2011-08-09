using System;
using System.ComponentModel;
using System.Globalization;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// FillStyleTypeConverter is a special type converter which will be  
    /// associated with the FillStyle enum.  It converts a FillStyle to  
    /// string representation for use in a property grid.
    /// </summary>
    // ----------------------------------------------------------------------
    public class FillStyleTypeConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destType)
        {
            if (destType == typeof(string) && value is FillStyle)
            {
                // Cast the value to a Fill type
                FillStyle f = (FillStyle)value;

                // Return the current fill style.
                return f.ToString();
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
