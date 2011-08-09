using System;
using System.ComponentModel;
using System.Globalization;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// GradientFillTypeConverter is a special type converter which will be  
    /// associated with the GradientFill class.  It converts a Fill object to  
    /// string representation for use in a property grid.
    /// </summary>
    // ----------------------------------------------------------------------
    public class GradientFillTypeConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destType)
        {
            if (destType == typeof(string) && value is GradientFill)
            {
                // Cast the value to a GradientFill type
                GradientFill f = (GradientFill)value;

                // Return a brief description.
                return "Gradient Fill";
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
