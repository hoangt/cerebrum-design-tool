using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A LinePainter in which all points are connected using straight lines.
    /// </summary>
    // ----------------------------------------------------------------------
    [Serializable()]
    public class StraightLine : LineRenderer
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public StraightLine()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates a new graphics path and adds straight lines connecting
        /// the points.
        /// </summary>
        // ------------------------------------------------------------------
        public override void CreateGraphicsPath()
        {
            this.graphicsPath = new GraphicsPath();
            this.graphicsPath.AddLines(this.points.ToPointArray());
        }
    }
}
