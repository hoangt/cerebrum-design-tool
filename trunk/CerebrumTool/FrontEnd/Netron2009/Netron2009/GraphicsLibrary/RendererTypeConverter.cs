using System;
using System.ComponentModel;
using System.Globalization;

namespace GraphicsLibrary
{
	// ----------------------------------------------------------------------
	/// <summary>
    /// PainterTypeConverter is a special type converter which will be  
    /// associated with the Painter class.  It converts a Painter object to  
    /// string representation for use in a property grid.
	/// </summary>
	// ----------------------------------------------------------------------
	public class RendererTypeConverter : ExpandableObjectConverter
	{
		public override object ConvertTo(
			ITypeDescriptorContext context, 
			CultureInfo culture, 
			object value, 
			Type destType )
		{
            if (destType == typeof(string) && value is IRenderer2D)
			{
				// Cast the value to a Painter type
                IRenderer2D p = (IRenderer2D)value;

				// Return a brief description.
				return p.Name;
			}
			return base.ConvertTo(context,culture,value,destType);
		}
	}
}
