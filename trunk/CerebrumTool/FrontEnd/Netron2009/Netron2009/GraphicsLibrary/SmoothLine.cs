using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A LinePainter in which all points are connected using arcs to generate
    /// a "smooth line".
    /// </summary>
    // ----------------------------------------------------------------------
    [Serializable()]
    public class SmoothLine : LineRenderer
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public SmoothLine()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates a new graphics path and adds an arc that connects all
        /// points together.
        /// </summary>
        // ------------------------------------------------------------------
        public override void CreateGraphicsPath()
        {
            this.graphicsPath = new GraphicsPath();
            this.graphicsPath.AddCurve(this.points.ToPointArray());           
        }
    }
}
