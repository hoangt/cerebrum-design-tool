using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GraphicsLibrary
{
    public class RectangleConverter
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Converts a Rectangle to a RectangleF.
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <returns>RectangleF</returns>
        // ------------------------------------------------------------------
        public static RectangleF ToRectangleF(Rectangle rect)
        {
            RectangleF rectangleF = new RectangleF(
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height);
            return rectangleF;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates a Rectangle from a RectangleF.
        /// </summary>
        /// <param name="rectF">RectangleF</param>
        /// <returns>Rectangle</returns>
        // ------------------------------------------------------------------
        public static Rectangle FromRectangleF(RectangleF rectF)
        {
            Rectangle rectangle = new Rectangle(
                (int)rectF.X,
                (int)rectF.Y,
                (int)rectF.Width,
                (int)rectF.Height);
            return rectangle;
        }
    }
}
