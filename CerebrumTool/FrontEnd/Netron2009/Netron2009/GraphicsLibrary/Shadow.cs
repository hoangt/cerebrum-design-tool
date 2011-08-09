using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace GraphicsLibrary
{
    [Serializable()]
    [TypeConverter(typeof(ShadowTypeConverter))]
    public class Shadow
    {
        Color color = Color.FromArgb(30, Color.Black);

        // ------------------------------------------------------------------
        /// <summary>
        /// Specifies if the shadow is shown.
        /// </summary>
        // ------------------------------------------------------------------
        protected bool isVisible = true;

        // ------------------------------------------------------------------
        /// <summary>
        /// The amount in the x direction the shadow will be offset from it's
        /// parent.
        /// </summary>
        // ------------------------------------------------------------------
        protected int xOffset = 5;

        // ------------------------------------------------------------------
        /// <summary>
        /// The amount in the y direction the shadow will be offset from it's
        /// parent.
        /// </summary>
        // ------------------------------------------------------------------
        protected int yOffset = 5;

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public Shadow()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info">SerializationInfo: Stores all the data needed
        /// to serialize or deserialize an object.</param>
        /// <param name="context">StreamingContext: Describes the source and
        /// destination of a given serialized stream, and provides an 
        /// additional caller-defined context.</param>
        // ------------------------------------------------------------------
        protected Shadow(
            SerializationInfo info,
            StreamingContext context)
        {
        }

        /// <summary>
        /// Shifts the area to be painted by the current x and y offset and
        /// paints the shadow if 'IsVisible' is true.
        /// </summary>
        /// <param name="area">Rectangle</param>
        /// <param name="g">Graphics</param>
        public void Paint(
            Rectangle area, 
            GraphicsPath path,
            Graphics g)
        {            
            if (this.isVisible == true)
            {
                Rectangle shadowArea = new Rectangle(
                area.X + xOffset,
                area.Y + yOffset,
                area.Width,
                area.Height);
            }
        }

        #region ISerializable Members

        // ------------------------------------------------------------------
        /// <summary>
        /// ISerializable implementation.  Adds all data required to serialize
        /// the Fill to disk.
        /// </summary>
        /// <param name="info">SerializationInfo: Stores all the data needed
        /// to serialize or deserialize an object.</param>
        /// <param name="context">StreamingContext: Describes the source and
        /// destination of a given serialized stream, and provides an 
        /// additional caller-defined context.</param>
        // ------------------------------------------------------------------
        public void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {           
            //base.GetObjectData(info, context);
            info.AddValue("IsVisible", this.isVisible, typeof(bool));
            info.AddValue("XOffset", this.xOffset, typeof(int));

            info.AddValue("YOffset", this.yOffset, typeof(int));

        }

        #endregion
    }
}
