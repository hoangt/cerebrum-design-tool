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
    public abstract class RendererBase : IRenderer
    {
        #region Fields

        protected string name = "Graphics Painter";
        protected GraphicsPath graphicsPath = null;
        protected QualityMode quality = QualityMode.Default;
        protected SmoothingMode smoothingMode = SmoothingMode.HighQuality;
        protected bool showShadow = true;
        protected Point shadowOffset = new Point(5, 5);
        protected LineStyle lineStyle = LineStyle.Solid;
        protected float lineWidth = 1F;
        protected Color lineColor = Color.Black;
        protected Color shadowColor = Color.FromArgb(30, Color.Black);

        #endregion

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the style of the line.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual LineStyle LineStyle
        {
            get
            {
                return this.lineStyle;
            }
            set
            {
                this.lineStyle = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the line color.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual Color LineColor
        {
            get
            {
                return this.lineColor;
            }
            set
            {
                this.lineColor = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the width of the line.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual float LineWidth
        {
            get
            {
                return this.lineWidth;
            }
            set
            {
                this.lineWidth = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the GraphicsPath that defines this shape.  If the rotation
        /// angle is not zero, then a trasformation matrix is added to the
        /// graphics path.
        /// </summary>
        // ------------------------------------------------------------------
        [Browsable(false)]
        public virtual GraphicsPath GraphicsPath
        {
            get
            {
                if (this.graphicsPath == null)
                {
                    this.CreateGraphicsPath();
                }
                return this.graphicsPath;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the overall quality when rendering GDI+ objects.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual QualityMode Quality
        {
            get
            {
                return this.quality;
            }
            set
            {
                this.quality = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the region (area) that describes the interior of a graphics
        /// shape compsed of rectangles and paths.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual Region Region
        {
            get
            {
                if (this.graphicsPath == null)
                {
                    this.CreateGraphicsPath();
                }
                return new Region(this.graphicsPath);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the smoothing mode (antialiasing) used.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual SmoothingMode SmoothingMode
        {
            get
            {
                return this.smoothingMode;
            }
            set
            {
                this.smoothingMode = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets if a shadow is drawn.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual bool ShowShadow
        {
            get
            {
                return this.showShadow;
            }
            set
            {
                this.showShadow = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the offset (in x and y direction) of the shadow.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual Point ShadowOffset
        {
            get
            {
                return this.shadowOffset;
            }
            set
            {
                this.shadowOffset = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a name that describes what this painter is used for 
        /// (such as 'Background Painter', 'Rectangle Painter', etc.).
        /// </summary>
        // ------------------------------------------------------------------
        public virtual string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        #endregion

        #region Constructors

        // ------------------------------------------------------------------
        /// <summary>
        /// Default constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public RendererBase()
        {
            this.InitializePainter();
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
        protected RendererBase(
            SerializationInfo info,
            StreamingContext context)
        {
            this.name = (string)info.GetValue("Name", typeof(string));

            this.quality = (QualityMode)info.GetValue("QualityMode", 
                typeof(QualityMode));

            this.smoothingMode = (SmoothingMode)info.GetValue("SmoothingMode", 
                typeof(SmoothingMode));

            this.lineStyle = (LineStyle)info.GetValue("LineStyle", 
                typeof(LineStyle));

            this.lineColor = (Color)info.GetValue("LineColor", 
                typeof(Color));

            this.lineWidth = (float)info.GetValue("LineWidth", typeof(float));

        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the default settings of this painter.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void InitializePainter()
        {
        }

        #region IRenderer Members

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns if the point specified falls within the region of this
        /// painter.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>bool: True if the point lies within the region.</returns>
        // ------------------------------------------------------------------
        public virtual bool Hit(Point point)
        {
            return this.Region.IsVisible(point);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Generates the shadow, fills the shadow, then generates and draws
        /// the 'real' graphics path.
        /// </summary>
        /// <param name="g">Graphics</param>
        // ------------------------------------------------------------------
        public virtual void Paint(Graphics g)
        {
            g.SmoothingMode = smoothingMode;
            string dashStyleString = this.lineStyle.ToString();
            GraphicsPath shadow;
            SolidBrush shadowBrush = new SolidBrush(this.shadowColor);

            if (this.showShadow == true)
            {
                // First generate the graphics path for the shadow.
                this.GenerateShadowPath();
                shadow = this.graphicsPath;

                // Fill the shadow first.  The 'real' painted area will be drawn
                // on top of the shadow.
                g.FillPath(shadowBrush, shadow);
            }

            // Now generate the 'real' graphics path to be painted.
            this.CreateGraphicsPath();

            // Lastly, draw the outline if needed.
            if (this.lineStyle != LineStyle.None)
            {
                DashStyle dashStyle =
                    (DashStyle)Enum.Parse(typeof(DashStyle), dashStyleString);

                Pen pen = new Pen(this.lineColor, this.lineWidth);
                pen.DashStyle = dashStyle;
                g.DrawPath(pen, this.graphicsPath);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Generates the shadow, fills the shadow, then generates and draws
        /// the 'real' graphics path.
        /// </summary>
        /// <param name="g">Graphics</param>
        // ------------------------------------------------------------------
        public virtual void DrawBorder(Graphics g)
        {
            g.SmoothingMode = smoothingMode;
            string dashStyleString = this.lineStyle.ToString();            

            // Draw the outline if needed.
            if (this.lineStyle != LineStyle.None)
            {
                // Generate the graphics path to be drawn.
                this.CreateGraphicsPath();

                DashStyle dashStyle =
                    (DashStyle)Enum.Parse(typeof(DashStyle), dashStyleString);

                Pen pen = new Pen(this.lineColor, this.lineWidth);
                pen.DashStyle = dashStyle;
                g.DrawPath(pen, this.graphicsPath);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Abstract method to be implemented by all painters: moves the upper 
        /// left-hand corner for the bounds of the painter to the specified 
        /// location.
        /// </summary>
        /// <param name="point">Point</param>
        // ------------------------------------------------------------------
        public abstract void MoveTo(Point point);

        // ------------------------------------------------------------------
        /// <summary>
        /// Abstract method to be implemented by all painters: moves the 
        /// location of the upper left-hand corner of the bounds by the amount 
        /// specified.
        /// </summary>
        /// <param name="dx">int: The amount to move in the x direction.
        /// </param>
        /// <param name="dy">int: The amount to move in the y direction.
        /// </param>
        // ------------------------------------------------------------------
        public abstract void MoveBy(int dx, int dy);

        // ------------------------------------------------------------------
        /// <summary>
        /// Abstract method to be implemented by all painters: generates the 
        /// GraphicsPath to be drawn.
        /// </summary>
        /// <returns>GraphicsPath</returns>
        // ------------------------------------------------------------------
        public abstract void CreateGraphicsPath();        

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Abstract method to be implemented by all painters: generates the 
        /// graphics path for the shadow.
        /// </summary>
        // ------------------------------------------------------------------
        protected abstract GraphicsPath GenerateShadowPath();

        // ------------------------------------------------------------------
        /// <summary>
        /// Converts this Painter object to a string representation.  A
        /// Painter can also be created from this string by calling 
        /// FromString.
        /// </summary>
        /// <returns>string</returns>
        // ------------------------------------------------------------------
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("LineColor: " + this.lineColor.ToString() + ";");
            sb.Append("LineStyle: " + this.lineStyle.ToString() + ";");
            sb.Append("LineWidth: " + this.lineWidth.ToString() + ";");
            sb.Append("QualityMode: " + this.quality.ToString() + ";");
            sb.Append("SmoothingMode: " + this.smoothingMode.ToString() + ";");
            return sb.ToString();
        }

        #region ISerializable Members

        // ------------------------------------------------------------------
        /// <summary>
        /// ISerializable implementation.  Adds all data required to serialize
        /// the painter to disk.
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
            info.AddValue("Name", this.name, typeof(string));
            info.AddValue("QualityMode", this.quality, typeof(QualityMode));

            info.AddValue("SmoothingMode", this.smoothingMode,
                typeof(SmoothingMode));

            info.AddValue("LineStyle", this.lineStyle, typeof(LineStyle));
            info.AddValue("LineColor", this.lineColor, typeof(Color));
            info.AddValue("LineWidth", this.lineWidth);
        }

        #endregion
    }
}
