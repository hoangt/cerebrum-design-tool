using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;

namespace GraphicsLibrary.TwoDimensional
{
    [Serializable()]
    public class RectangleRenderer : Renderer2D
    {
        int cornerRadius = 0;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the radius of the corners in the rectangle.  When
        /// set to zero, the corners are 90 deg angles.  The radius can not
        /// be greater than the width or height of this rectangle, whichever
        /// is less.
        /// </summary>
        // ------------------------------------------------------------------
        public int CornerRadius
        {
            get
            {
                return this.cornerRadius;
            }
            set
            {
                int maxRadius = Math.Min(Bounds.Width, Bounds.Height);
                if (value > maxRadius)
                {
                    this.cornerRadius = maxRadius;
                }
                else
                {
                    this.cornerRadius = value;
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public RectangleRenderer()
            : base()
        {
            this.name = "Rectangle";
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
        protected RectangleRenderer(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Generates the GraphicsPath for an ellipse.
        /// </summary>
        /// <returns>GraphicsPath</returns>
        // ------------------------------------------------------------------
        public override void CreateGraphicsPath()
        {
            base.graphicsPath = new GraphicsPath();
            
            // If the radius is zero, just add a rectangle.
            if (cornerRadius == 0)
            {
                graphicsPath.AddRectangle(base.bounds);
                base.CreateGraphicsPath();
                return;
            }

            // Otherwise, use our Geometry class to build our rounded
            // rectangle.
            base.graphicsPath = Geometry.RoundedRectangle(
                base.bounds,
                this.cornerRadius);

            base.CreateGraphicsPath();
        }

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
        }
    }
}
