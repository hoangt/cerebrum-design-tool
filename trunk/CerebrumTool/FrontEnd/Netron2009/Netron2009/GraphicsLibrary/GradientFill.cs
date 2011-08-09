using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace GraphicsLibrary
{
    [Serializable()]
    [TypeConverter(typeof(GradientFillTypeConverter))]
    public class GradientFill : ISerializable
    {
        Color startingColor = Color.White;
        Color endingColor = Color.WhiteSmoke;
        LinearGradientMode gradientMode = LinearGradientMode.Horizontal;
        LinearGradientBrush brush;

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the starting color of the gradient.
        /// </summary>
        // ------------------------------------------------------------------
        public Color StartingColor
        {
            get
            {
                return this.startingColor;
            }
            set
            {
                this.startingColor = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the ending color of the gradient.
        /// </summary>
        // ------------------------------------------------------------------
        public Color EndingColor
        {
            get
            {
                return this.endingColor;
            }
            set
            {
                this.endingColor = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the gradient direction.
        /// </summary>
        // ------------------------------------------------------------------
        public LinearGradientMode GradientMode
        {
            get
            {
                return this.gradientMode;
            }
            set
            {
                this.gradientMode = value;
            }
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public GradientFill()
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
        protected GradientFill(
            SerializationInfo info,
            StreamingContext context)
        {
            this.startingColor = 
                (Color)info.GetValue("StartingColor", typeof(Color));

            this.endingColor =
                (Color)info.GetValue("EndingColor", typeof(Color));

            this.gradientMode =
                (LinearGradientMode)info.GetValue("LinearGradientMode", 
                typeof(LinearGradientMode));            
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates a new LinearGradientBrush 
        /// </summary>
        // ------------------------------------------------------------------
        public Brush GetBrush(Rectangle area)
        {
            this.brush = new LinearGradientBrush(
                area,
                startingColor,
                endingColor,
                gradientMode);
            return this.brush;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Fills the Rectangle specified with the current linear gradient 
        /// mode, starting color, and ending color.
        /// </summary>
        /// <param name="area">Rectangle</param>
        /// <param name="g">Graphics</param>
        // ------------------------------------------------------------------
        public void FillRectangle(
            Rectangle area,
            Graphics g)
        {
            this.GetBrush(area);
            g.FillRectangle(this.brush, area);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Fills the specified GraphicsPath with the current FillStyle.
        /// </summary>
        /// <param name="area">Rectangle</param>
        /// <param name="path">GraphicsPath</param>
        /// <param name="g">Graphics</param>
        // ------------------------------------------------------------------
        public void FillGraphicsPath(
            Rectangle area,
            GraphicsPath path, 
            Graphics g)
        {
            this.GetBrush(area);
            g.FillPath(this.brush, path);
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
            info.AddValue("StartingColor", this.startingColor, typeof(Color));
            info.AddValue("EndingColor", this.endingColor, typeof(Color));
            info.AddValue("LinearGradientMode", this.gradientMode, 
                typeof(LinearGradientMode));
        }

        #endregion
    }
}
