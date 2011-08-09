using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace GraphicsLibrary
{
    [Serializable()]
    public abstract class LineRenderer : RendererBase
    {
        #region Fields

        // ------------------------------------------------------------------
        /// <summary>
        /// The list of points for this line.
        /// </summary>
        // ------------------------------------------------------------------
        protected PointList points = new PointList();

        #endregion

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the collection of points used to make up this line.
        /// </summary>
        // ------------------------------------------------------------------
        public PointList Points
        {
            get
            {
                return this.points;
            }
            set
            {
                this.points = value;
            }
        }

        #endregion

        #region Constructors

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public LineRenderer()
            : base()
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
        protected LineRenderer(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            this.points = (PointList) info.GetValue(
                "Points", 
                typeof(PointList));
        }

        #endregion

        protected override void InitializePainter()
        {
            base.InitializePainter();
            this.LineStyle = LineStyle.Solid;
            this.lineColor = Color.Black;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// The first point in the collection is moved to the location
        /// specified.  The offset (dx, dy) from the first point's old 
        /// location to this new location is calculated and applied to all
        /// remaining points.  For example, if the first point is at (0,0),
        /// and it's moved to (5,5), this is an offset of: dx = 5, dy = 5.
        /// This offset is applied to all other points.
        /// </summary>
        /// <param name="point">Point</param>
        // ------------------------------------------------------------------
        public override void MoveTo(Point point)
        {
            if (this.points.Count < 1)
            {
                return;
            }

            int dx = point.X - points[0].X;
            int dy = point.Y - points[0].Y;
            this.MoveBy(dx, dy);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Offsets all points by the amount specified.
        /// </summary>
        /// <param name="dx">int: The amount to move in the x direction.
        /// </param>
        /// <param name="dy">int: The amount to move in the y direction.
        /// </param>
        // ------------------------------------------------------------------
        public override void MoveBy(int dx, int dy)
        {
            this.points.Offset(dx, dy);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates the graphics path for the shadow.  All points are 
        /// temporarily offset by the amount specified in 'ShadowOffset' and
        /// method 'CreateGraphicsPath()' is called, thus producing an exact
        /// replica shifted.
        /// </summary>
        // ------------------------------------------------------------------
        protected override GraphicsPath GenerateShadowPath()
        {
            PointList original = this.points;
            GraphicsPath shadow;
            this.points.Offset(this.shadowOffset);
            this.CreateGraphicsPath();
            shadow = this.graphicsPath;

            // Restore the original points.
            this.points = original;
            return shadow;
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
            info.AddValue("Points", this.points, typeof(PointList));
        }

        #endregion
    }
}
