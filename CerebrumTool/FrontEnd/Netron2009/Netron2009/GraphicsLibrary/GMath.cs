using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace GraphicsLibrary
{
    public class GMath
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Adds the x and y values of the two points specified.
        /// </summary>
        /// <param name="p1">Point</param>
        /// <param name="p2">Point</param>
        /// <returns>Point: A new point who's the sum of the two points
        /// provided.</returns>
        // ------------------------------------------------------------------
        public static Point Add(Point p1, Point p2)
        {
            return new Point(
                p1.X + p2.X,
                p1.Y + p2.Y);
        }

        //public static bool operator + (Point p1, Point p2)
        //{
        //    if ((p1.X == p2.X) && (p1.Y == p2.Y))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static bool operator > (Point p1, Point p2)
        //{
        //    if ((p1.X > p2.X) && (p1.Y > p2.Y))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public static bool operator < (Point p1, Point p2)
        //{
        //    if ((p1.X < p2.X) && (p1.Y < p2.Y))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
    }
}
