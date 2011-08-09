using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace GraphicsLibrary
{
    [Serializable()]
    [TypeConverter(typeof(CornerStyleTypeConverter))]
    public enum CornerStyle
    {
        Sharp,
        Rounded
    }
}
