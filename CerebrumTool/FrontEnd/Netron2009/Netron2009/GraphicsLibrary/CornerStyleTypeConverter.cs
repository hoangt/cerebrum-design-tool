using System;
using System.ComponentModel;
using System.Globalization;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// CornerStyleTypeConverter is a special type converter which will be  
    /// associated with the CornerStyle enum.  It converts a CornerStyle object   
    /// to string representation for use in a property grid.
    /// </summary>
    // ----------------------------------------------------------------------
    public class CornerStyleTypeConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destType)
        {
            if (destType == typeof(string) && value is CornerStyle)
            {
                // Cast the value to a CornerStyle type
                CornerStyle cs = (CornerStyle)value;

                // Return the current text.
                return cs.ToString();
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
