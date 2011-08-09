using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Netron.Diagramming.Core;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace BasicShapes
{
    [Serializable()]
    [Shape(
        "Triangle", // Shape Name
        "5B148C27-31F4-456f-8D1F-C7B8C8E86283",  // Shape Key
        "Basic Shapes",  // Category
        "A triangular shape.",  // Description
        false)]  // Is for internal use only?  NO.
    public class TriangleShape : SimpleShapeBase
    {
        #region Fields

        // ------------------------------------------------------------------
        /// <summary>
        /// The current version - used when deserializing the shape.
        /// </summary>
        // ------------------------------------------------------------------
        protected double triangleShapeVersion = 1.0;

        #endregion

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the graphics path that defines this triangle.
        /// </summary>
        // ------------------------------------------------------------------
        public GraphicsPath Path
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                path.AddLine(TopCenter,
                    BottomRightCorner);

                path.AddLine(BottomRightCorner,
                    BottomLeftCorner);

                path.AddLine(BottomLeftCorner,
                    TopCenter);
                return path;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the friendly name of this shape.
        /// </summary>
        // ------------------------------------------------------------------
        public override string EntityName
        {
            get
            {
                return "Triangle";
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns the version of this shape.
        /// </summary>
        // ------------------------------------------------------------------
        public override double Version
        {
            get
            {
                return triangleShapeVersion;
            }
        }

        #endregion

        #region Constructors

        // ------------------------------------------------------------------
        /// <summary>
        /// Default constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public TriangleShape()
            : base()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor that receives the model.
        /// </summary>
        /// <param name="model">IModel</param>
        // ------------------------------------------------------------------
        public TriangleShape(IModel model)
            : base(model)
        {
        }        

        // -------------------------------------------------------------------
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        // -------------------------------------------------------------------
        protected TriangleShape(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            double version = info.GetDouble("TriangleShapeVersion");
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Initializes the shape.
        /// </summary>
        // ------------------------------------------------------------------
        protected override void Initialize()
        {
            base.Initialize();
            mPenStyle = new LinePenStyle();
            mPenStyle.Color = Color.Black;
            mPenStyle.DashStyle = DashStyle.Solid;

            mPaintStyle = new GradientPaintStyle(
                Color.White, // Start color
                Color.Silver, // End color
                90F); // Gradient angle

            Connector top = new Connector(Model);
            top.Parent = this;
            top.Point = TopCenter;
            mConnectors.Add(top);

            Connector bottomLeft = new Connector(Model);
            bottomLeft.Parent = this;
            bottomLeft.Point = BottomLeftCorner;
            mConnectors.Add(bottomLeft);

            Connector bottomRight = new Connector(Model);
            bottomRight.Parent = this;
            bottomRight.Point = BottomRightCorner;
            mConnectors.Add(bottomRight);

            Transform(50, 50, 150, 150);
        }

        #region Serialization

        // -------------------------------------------------------------------
        /// <summary>
        /// Populates a <see cref="
        /// T:System.Runtime.Serialization.SerializationInfo"></see> with 
        /// the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The 
        /// <see 
        /// cref="T:System.Runtime.Serialization.SerializationInfo">
        /// </see> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="
        /// T:System.Runtime.Serialization.StreamingContext"></see>) 
        /// for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The 
        /// caller does not have the required permission. </exception>
        // -------------------------------------------------------------------
        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("TriangleShapeVersion", triangleShapeVersion);
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns if the point specified falls within the graphics
        /// path that defines this triangle (i.e. not in our rectangular
        /// bounds!).
        /// </summary>
        /// <param name="p">Point</param>
        /// <returns>bool</returns>
        // ------------------------------------------------------------------
        public override bool Hit(Point p)
        {
            return Path.IsVisible(p);
        }

        #region Painting

        // ------------------------------------------------------------------
        /// <summary>
        /// Paints the shape.
        /// </summary>
        /// <param name="g"></param>
        // ------------------------------------------------------------------
        public override void Paint(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            base.Paint(g);

            if (mVisible == false)
            {
                return;
            }

            Pen pen = mPenStyle.DrawingPen();

            if (mHovered)
            {
                pen = ArtPalette.HighlightPen;
            }

            GraphicsPath path = Path;

            // Draw the shadow first so it's in the background.
            DrawShadow(g);

            g.DrawPath(pen, path);
            g.FillPath(mPaintStyle.GetBrush(mRectangle), path);

            foreach (IConnector con in mConnectors)
            {
                con.Paint(g);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Draws the shape's shadow.
        /// </summary>
        /// <param name="g">Graphics</param>
        // ------------------------------------------------------------------
        void DrawShadow(Graphics g)
        {
            Point top = TopCenter;
            top.Offset(5, 5);

            Point bottomRight = BottomRightCorner;
            bottomRight.Offset(5, 5);

            Point bottomLeft = BottomLeftCorner;
            bottomLeft.Offset(5, 5);

            GraphicsPath shadow = new GraphicsPath();
            shadow.AddLine(top, bottomRight);
            shadow.AddLine(bottomRight, bottomLeft);
            shadow.AddLine(bottomLeft, top);

            g.FillPath(ArtPalette.ShadowBrush, shadow);
        }

        #endregion
    }
}
