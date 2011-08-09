using System;
using System.ComponentModel;
using System.Globalization;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// FillTypeConverter is a special type converter which will be  
    /// associated with the Fill class.  It converts a Fill object to  
    /// string representation for use in a property grid.
    /// </summary>
    // ----------------------------------------------------------------------
    public class FillTypeConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destType)
        {
            if (destType == typeof(string) && value is Fill)
            {
                // Cast the value to a Fill type
                Fill f = (Fill)value;

                // Return the current fill style.
                return f.FillStyle.ToString();
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
