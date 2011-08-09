using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace GraphicsLibrary.TwoDimensional
{
    [Serializable()]
    public class EllipseRenderer : Renderer2D
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public EllipseRenderer() : base()
        {
            this.name = "Ellipse";
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
        protected EllipseRenderer(
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
            this.graphicsPath = new GraphicsPath();
            this.graphicsPath.AddEllipse(base.bounds);
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
