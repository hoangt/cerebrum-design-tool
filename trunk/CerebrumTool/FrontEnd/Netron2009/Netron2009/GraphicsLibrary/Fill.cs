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
    [TypeConverter(typeof(FillTypeConverter))]
    public class Fill : ISerializable
    {
        protected FillStyle fillStyle = FillStyle.Solid;
        protected GradientFill gradientFill = new GradientFill();
        protected Color solidColor = Color.WhiteSmoke;

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the fill style (solid, gradient, or hatch).
        /// </summary>
        // ------------------------------------------------------------------
        public FillStyle FillStyle
        {
            get
            {
                return this.fillStyle;
            }
            set
            {
                this.fillStyle = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the gradient fill.
        /// </summary>
        // ------------------------------------------------------------------
        public GradientFill Gradient
        {
            get
            {
                return this.gradientFill;
            }
            set
            {
                this.gradientFill = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the color to use when the FillStyle is Solid.
        /// </summary>
        // ------------------------------------------------------------------
        public Color SolidColor
        {
            get
            {
                return this.solidColor;
            }
            set
            {
                this.solidColor = value;
            }
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public Fill()
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
        protected Fill(
            SerializationInfo info,
            StreamingContext context)
        {
            this.fillStyle = 
                (FillStyle)info.GetValue("FillStyle", typeof(FillStyle));

            this.gradientFill =
                (GradientFill)info.GetValue("GradientFill", 
                typeof(GradientFill));

            this.solidColor =
                (Color)info.GetValue("SolidColor", typeof(Color));
            
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates and returns the brush for the current settings.
        /// </summary>
        /// <param name="area">Rectangle</param>
        /// <returns>Brush</returns>
        // ------------------------------------------------------------------
        public virtual Brush GetBrush(Rectangle area)
        {
            Brush brush = new SolidBrush(this.solidColor);

            switch (this.fillStyle)
            {
                case FillStyle.Gradient:
                    brush = this.gradientFill.GetBrush(area);
                    break;
            }

            return brush;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Fills the specified GraphicsPath with the current FillStyle.
        /// </summary>
        /// <param name="area">Rectangle</param>
        /// <param name="path">GraphicsPath</param>
        /// <param name="g">Graphics</param>
        // ------------------------------------------------------------------
        public virtual void FillGraphicsPath(
            Rectangle area,
            GraphicsPath path, 
            Graphics g)
        {
            Brush brush = this.GetBrush(area);
            g.FillPath(brush, path);
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
        public virtual void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            info.AddValue("FillStyle", this.fillStyle, typeof(FillStyle));
            info.AddValue("GradientFill", this.gradientFill, 
                typeof(GradientFill));

            info.AddValue("SolidColor", this.solidColor, typeof(Color));
        }

        #endregion
    }
}
