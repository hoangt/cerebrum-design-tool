using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Drawing;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A collection of System.Drawing.Point's.
    /// </summary>
    // ----------------------------------------------------------------------
    [Serializable()]
    public class PointList : CollectionBase, ISerializable
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public PointList()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds the specified point to the collection.
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>int</returns>
        // ------------------------------------------------------------------
        public int Add(Point point)
        {
            return this.List.Add(point);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds an array of points to this list.
        /// </summary>
        /// <param name="points">Point[]: An array of points.</param>
        /// <returns>int</returns>
        // ------------------------------------------------------------------
        public int Add(Point[] points)
        {
            int index = 0;
            foreach (Point p in points)
            {
                index = this.List.Add(p);
            }
            return index;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Adds a new point to the collection for the specified x and y 
        /// values.
        /// </summary>
        /// <param name="x">int: The horizontal position of the point.</param>
        /// <param name="y">int: The vertical position of the point.</param>
        /// <returns>int</returns>
        // ------------------------------------------------------------------
        public int Add(int x, int y)
        {
            return this.List.Add(new Point(x, y));
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the point at the specified index.  If the index is out of the
        /// list bounds, then Point.Empty is returned.
        /// </summary>
        /// <param name="index">int</param>
        /// <returns>Point</returns>
        // ------------------------------------------------------------------
        public Point Get(int index)
        {
            if ((index > 0) && (index < this.List.Count))
            {
                return (Point)this.List[index];
            }
            else
            {
                return Point.Empty;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Removes the point from the list.  An exception is NOT thrown if 
        /// not successful.
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>bool: True if successful.</returns>
        // ------------------------------------------------------------------
        public bool Remove(Point point)
        {
            try
            {
                this.List.Remove(point);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the Point at the specified index.  When used as a
        /// setter, the value of the point is set if the index is within the
        /// bounds of the inner list.  If the index is one more than the number
        /// of items, then the point is added to the list.  Otherwise, the
        /// action is ignored.
        /// </summary>
        /// <param name="index">int</param>
        /// <returns>Point</returns>
        // ------------------------------------------------------------------
        public Point this[int index]
        {
            get
            {
                return this.Get(index);
            }
            set
            {
                if (index < this.List.Count)
                {
                    this.List[index] = value;
                }

                if (index == this.List.Count)
                {
                    this.Add(value);
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Converts the inner list to a Point[].
        /// </summary>
        /// <returns>Point[]</returns>
        // ------------------------------------------------------------------
        public Point[] ToPointArray()
        {
            Point[] points = null;
            if (this.List.Count > 0)
            {
                points = new Point[this.List.Count];
                for (int i = 0; i < this.List.Count; i++)
                {
                    points[i] = this.Get(i);
                }
            }
            return points;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Offsets ALL points by the amount specified.
        /// </summary>
        /// <param name="amount">Point</param>
        // ------------------------------------------------------------------
        public void Offset(Point amount)
        {
            foreach (Point p in this.List)
            {
                p.Offset(amount);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Offsets ALL points by the amount specified.
        /// </summary>
        /// <param name="dx">int: The amount of the offset in the 
        /// x-coordinate.</param>
        /// <param name="dy">int: The amount of the offset in the 
        /// y-coordinate.</param>
        // ------------------------------------------------------------------
        public void Offset(int dx, int dy)
        {
            foreach (Point p in this.List)
            {
                p.Offset(dx, dy);
            }
        }

        #region ISerializable Members

        // ------------------------------------------------------------------
        /// <summary>
        /// ISerializable implementation.  Adds all data required to serialize
        /// the list of points to disk.
        /// </summary>
        /// <param name="info">SerializationInfo: Stores all the data needed
        /// to serialize or deserialize an object.</param>
        /// <param name="context">StreamingContext: Describes the source and
        /// destination of a given serialized stream, and provides an 
        /// additional caller-defined context.</param>
        // ------------------------------------------------------------------
        public void GetObjectData(
            SerializationInfo info, 
            StreamingContext context)
        {
            info.AddValue("Points", this.ToPointArray(), typeof(Point[]));
        }

        #endregion
    }
}
