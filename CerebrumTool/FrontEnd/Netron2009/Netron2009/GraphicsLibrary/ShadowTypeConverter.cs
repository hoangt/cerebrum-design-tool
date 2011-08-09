using System;
using System.ComponentModel;
using System.Globalization;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// ShadowTypeConverter is a special type converter which will be  
    /// associated with the Shadow class.  It converts a Shadow object to  
    /// string representation for use in a property grid.
    /// </summary>
    // ----------------------------------------------------------------------
    public class ShadowTypeConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destType)
        {
            if (destType == typeof(string) && value is Shadow)
            {
                // Cast the value to a Fill type
                Shadow shadow = (Shadow)value;

                // Return the current fill style.
                return "Shadow";
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
