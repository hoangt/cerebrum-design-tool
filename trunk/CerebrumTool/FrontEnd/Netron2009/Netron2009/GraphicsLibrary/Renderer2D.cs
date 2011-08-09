using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;

namespace GraphicsLibrary
{
    [Serializable()]
    [TypeConverter(typeof(RendererTypeConverter))]
    public abstract class Renderer2D : RendererBase, IRenderer2D
    {
        protected float angle = 0;
        protected Rectangle bounds = new Rectangle();
        protected Fill fill = new Fill();
        protected Rectangle shadowArea = new Rectangle();

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the rotation angle.
        /// </summary>
        // ------------------------------------------------------------------
        public float Angle
        {
            get
            {
                return this.angle;
            }
            set
            {
                this.angle = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns the point that the shape is rotated about.  The default
        /// is the Center of the shape.  Override this to provide your custom
        /// rotation point.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual Point RotationPoint
        {
            get
            {
                return this.Center;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the bounds of this Shape.
        /// </summary>
        // ------------------------------------------------------------------
        [Browsable(false)]
        public Rectangle Bounds
        {
            get
            {
                return this.bounds;
            }
            set
            {
                this.bounds = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns the center point of the bounding rectangle.  This does not
        /// take rotation into acount.
        /// </summary>
        // ------------------------------------------------------------------
        public virtual Point Center
        {
            get
            {
                return new Point(
                    this.bounds.X + (this.bounds.Width / 2),
                    this.bounds.Y + (this.bounds.Height / 2));
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the size of the bounds to be painted.
        /// </summary>
        // ------------------------------------------------------------------
        public Size Size
        {
            get
            {
                return this.bounds.Size;
            }
            set
            {
                this.bounds.Size = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the location of the upper left-hand corner of the
        /// bounds to be painted.
        /// </summary>
        // ------------------------------------------------------------------
        public Point Location
        {
            get
            {
                return this.bounds.Location;
            }
            set
            {
                if (value != Point.Empty)
                {
                    this.bounds.Location = value;
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the fill style (solid, gradient, or hatch).
        /// </summary>
        // ------------------------------------------------------------------
        [TypeConverter(typeof(FillTypeConverter))]
        public Fill Fill
        {
            get
            {
                return this.fill;
            }
            set
            {
                this.fill = value;
            }
        }

        #endregion

        #region Constructors

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public Renderer2D() : base()
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
        protected Renderer2D(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            this.fill = (Fill)info.GetValue("Fill", typeof(Fill));
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the default settings of this painter.
        /// </summary>
        // ------------------------------------------------------------------
        protected override void InitializePainter()
        {
            // Create the default location and size so an error won't occur
            // if the user forgets to set the bounds before calling Paint.
            this.bounds.Location = new Point(150, 100);
            this.bounds.Height = 50;
            this.bounds.Width = 50;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates a Matrix for rotation.  The center point of the bounding
        /// rectangle is used as the rotation point.
        /// </summary>
        /// <returns>Matrix</returns>
        // ------------------------------------------------------------------
        public virtual Matrix CreateRotationMatrix()
        {
            Matrix m = new Matrix();
            m.RotateAt(this.angle, this.Center);
            return m;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Transforms the bounds of this painter by the amount specified.
        /// </summary>
        /// <param name="pointOffset">Point: The offset to apply to the
        /// location of the upper left-hand corner of the bounding 
        /// rectangle.</param>
        /// <param name="sizeOffset">Size: The amount to offset the current
        /// size of the bounding rectangle by.</param>
        // ------------------------------------------------------------------
        public virtual void Transform(Point pointOffset, Size sizeOffset)
        {
            int xCurrent = this.bounds.Location.X;
            int yCurrent = this.bounds.Location.Y;
            int xOffset = pointOffset.X;
            int yOffset = pointOffset.Y;
            this.bounds.Location = new Point(
                xCurrent + xOffset,
                yCurrent + yOffset);

            this.bounds.Size += sizeOffset;                
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Moves the upper left-hand corner for the bounds of the painter 
        /// to the specified location.
        /// </summary>
        /// <param name="point">Point</param>
        // ------------------------------------------------------------------
        public override void MoveTo(Point point)
        {
            this.bounds.Location = point;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Moves the location of the upper left-hand corner of the bounds
        /// by the amount specified.
        /// </summary>
        /// <param name="dx">int: The amount to move in the x direction.
        /// </param>
        /// <param name="dy">int: The amount to move in the y direction.
        /// </param>
        // ------------------------------------------------------------------
        public override void MoveBy(int dx, int dy)
        {
            int newX = dx + this.bounds.Location.X;
            int newY = dy + this.bounds.Location.Y;
            this.bounds.Location = new Point(newX, newY);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Tells the base Painter1D to generate and draw the graphics path
        /// and shadow, then fills the path.
        /// </summary>
        /// <param name="g">Graphics</param>
        // ------------------------------------------------------------------
        public override void Paint(Graphics g)
        {
            base.Paint(g);

            // Fill the area.
            this.fill.FillGraphicsPath(
                this.bounds, 
                this.graphicsPath, 
                g);            
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Method to be overriden by all shapes.  The rotaion matrix is 
        /// applied here (method 'ApplyRotationMatrix()' is called), so after
        /// all derived classes finish creating the path, they should include
        /// a call to this base class 'base.CreateGraphicsPath()'.
        /// </summary>
        // ------------------------------------------------------------------
        public override void CreateGraphicsPath()
        {
            this.ApplyRotaionMatrix();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates a new matrix for rotating the shape and applies it to the
        /// current graphics path.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void ApplyRotaionMatrix()
        {
            Matrix m = this.CreateRotationMatrix();
            this.graphicsPath.Transform(m);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Generates the graphics path for the shadow.  The default here is
        /// the 'Bounds' are temporarily shifted by 'ShadowOffset' and
        /// 'CreateGraphicsPath()' is called.
        /// </summary>
        // ------------------------------------------------------------------
        protected override GraphicsPath GenerateShadowPath()
        {
            // We need to restore the original bounds.
            Rectangle originalBounds = this.bounds;
            GraphicsPath shadow;

            // Offset the current bounds by the shadow offset.
            this.bounds = new Rectangle(
                this.bounds.X + shadowOffset.X,
                this.bounds.Y + shadowOffset.Y,
                this.bounds.Width,
                this.bounds.Height);

            // Keep a copy of the shadow area.
            this.shadowArea = this.bounds;

            // Now generate graphics path using these new bounds for the
            // shadow.
            this.CreateGraphicsPath();
            shadow = this.graphicsPath;

            // Restore the original bounds.
            this.bounds = originalBounds;

            return shadow;
        }

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
            StringBuilder sb = new StringBuilder(base.ToString());
            sb.Append("Bounds: " + this.bounds.ToString() + ";");
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
        public override void GetObjectData(
            SerializationInfo info, 
            StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Fill", this.fill, typeof(Fill));
        }

        #endregion
    }
}
