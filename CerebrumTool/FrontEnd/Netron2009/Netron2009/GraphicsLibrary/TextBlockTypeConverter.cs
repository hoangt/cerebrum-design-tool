using System;
using System.ComponentModel;
using System.Globalization;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// TextBlockTypeConverter is a special type converter which will be  
    /// associated with the TextBlock class.  It converts a TextBlock object   
    /// to string representation for use in a property grid.
    /// </summary>
    // ----------------------------------------------------------------------
    public class TextBlockTypeConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(
            ITypeDescriptorContext context,
            CultureInfo culture,
            object value,
            Type destType)
        {
            if (destType == typeof(string) && value is TextBlock)
            {
                // Cast the value to a TextBlock type
                TextBlock tb = (TextBlock)value;

                // Return the current text.
                return tb.Text;
            }
            return base.ConvertTo(context, culture, value, destType);
        }
    }
}
