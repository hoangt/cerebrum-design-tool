using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A static class that generates different geometric GraphicsPath's, 
    /// such as a RoundedRectangle, Triangle, etc.
    /// </summary>
    // ----------------------------------------------------------------------
    public static class Geometry
    {
        #region Rounded Rectangle

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates a graphics path for drawing a rectangle that has rounded
        /// corners.
        /// </summary>
        /// <param name="bounds">Rectangle: The bounds for the GraphicsPath
        /// created.</param>
        /// <param name="cornerRadius">int: The radius of the corners.  This
        /// must be less than or equal to the supplied bounds width or
        /// height, whichever is less.</param>
        /// <returns>GraphicsPath</returns>
        // ------------------------------------------------------------------
        public static GraphicsPath RoundedRectangle(
            Rectangle bounds,
            int cornerRadius)
        {
            GraphicsPath graphicsPath = new GraphicsPath();

            // If the radius is zero, just add a rectangle.
            if (cornerRadius == 0)
            {
                graphicsPath.AddRectangle(bounds);
                return graphicsPath;
            }

            // Make sure the corner radius isn't greater than the width or
            // height of this rectangle, which ever is less.
            int maxRadius = Math.Min(bounds.Width, bounds.Height);
            if (cornerRadius > maxRadius)
            {
                cornerRadius = maxRadius;
            }

            // The *real* corners of our bounding rectangle.
            Point upperLeftCorner = bounds.Location;
            Point upperRightCorner = new Point(bounds.Right,
                bounds.Top);
            Point lowerLeftCorner = new Point(bounds.Left,
                bounds.Bottom);
            Point lowerRightCorner = new Point(bounds.Right,
                bounds.Bottom);

            // When adding the lines individually, it's important to
            // CONTINUOUSLY create the rectangle.  That is, draw a line,
            // then where that line ended is where the next line starts.
            // Otherwise you get mixed results.  For example, if you drew
            // the top line then the bottom line, there would be a diagnal
            // line connecting the two.

            //                    
            //                     (Drawn 1st)
            //  Top Side Start  ___________________ Top Side End
            //                 /                   \
            // Left Side End  /                     \ Right Side Start
            //                |                     |
            //                |                     |
            //  (Drawn 4th)   |                     |  (Drawn 2nd)
            //                |                     |        
            // Left Side Start \                    / Right Side End
            //                  \__________________/
            //    Bottom Side End  (Drawn 3rd)     Bottom Side Start

            // The top side
            Point topSideStartPoint = new Point(
                upperLeftCorner.X + cornerRadius,
                upperLeftCorner.Y);
            Point topSideEndPoint = new Point(
                upperRightCorner.X - cornerRadius,
                upperRightCorner.Y);

            // The right side.
            Point rightSideStartPoint = new Point(
                upperRightCorner.X,
                upperRightCorner.Y + cornerRadius);
            Point rightSideEndPoint = new Point(
                lowerRightCorner.X,
                lowerRightCorner.Y - cornerRadius);

            // The bottom side.
            Point bottomSideStartPoint = new Point(
                lowerRightCorner.X - cornerRadius,
                lowerRightCorner.Y);
            Point bottomSideEndPoint = new Point(
                lowerLeftCorner.X + cornerRadius,
                lowerLeftCorner.Y);

            // The left side.
            Point leftSideStartPoint = new Point(
                lowerLeftCorner.X,
                lowerLeftCorner.Y - cornerRadius);
            Point leftSideEndPoint = new Point(
                upperLeftCorner.X,
                upperLeftCorner.Y + cornerRadius);

            // The upper right arc
            Rectangle upperRightArcArea = new Rectangle(
                topSideEndPoint.X,
                topSideEndPoint.Y,
                cornerRadius,
                cornerRadius);

            // The lower right arc
            Rectangle lowerRightArcArea = new Rectangle(
                bottomSideStartPoint.X,
                rightSideEndPoint.Y,
                cornerRadius,
                cornerRadius);

            // The lower left arc
            Rectangle lowerLeftArcArea = new Rectangle(
                leftSideStartPoint.X,
                leftSideStartPoint.Y,
                cornerRadius,
                cornerRadius);

            // The upper left arc
            Rectangle upperLeftArcArea = new Rectangle(
                upperLeftCorner.X,
                upperLeftCorner.Y,
                cornerRadius,
                cornerRadius);

            // Now draw it.  Start with the top side.
            if (topSideStartPoint.X < topSideEndPoint.X)
            {
                graphicsPath.AddLine(topSideStartPoint, topSideEndPoint);
            }

            // Draw the arc connecting the top side with the right side.
            graphicsPath.AddArc(upperRightArcArea, 270, 90);

            // Draw the right side.
            if (rightSideStartPoint.Y < rightSideEndPoint.Y)
            {
                graphicsPath.AddLine(rightSideStartPoint, rightSideEndPoint);
            }

            // Draw the arc connecting the right side with the bottome side.
            graphicsPath.AddArc(lowerRightArcArea, 0, 90);

            // Draw the bottom side.
            if (bottomSideStartPoint.X > bottomSideEndPoint.X)
            {
                graphicsPath.AddLine(bottomSideStartPoint, bottomSideEndPoint);
            }

            // Draw the arc connecting the bottom side with the left side.
            graphicsPath.AddArc(lowerLeftArcArea, 90, 90);

            // Draw the left side.
            if (leftSideStartPoint.Y > leftSideEndPoint.Y)
            {
                graphicsPath.AddLine(leftSideStartPoint, leftSideEndPoint);
            }

            // Draw the arc connecting the left side with the top side.
            graphicsPath.AddArc(upperLeftArcArea, -180, 90);

            // Make sure the path is closed.
            graphicsPath.CloseFigure();

            return graphicsPath;
        }

        #endregion
    }
}
