using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace GraphicsLibrary
{
    public interface IRenderer2D : IRenderer
    {
        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns the point of rotaion used for the roation matrix.
        /// </summary>
        // ------------------------------------------------------------------
        Point RotationPoint
        {
            get;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the rotation angle.
        /// </summary>
        // ------------------------------------------------------------------
        float Angle
        {
            get;
            set;
        }

        Point Center
        {
            get;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the location of the upper left-hand corner of the
        /// bounds to be painted.
        /// </summary>
        // ------------------------------------------------------------------
        Point Location
        {
            get;
            set;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the size of the bounds to be painted.
        /// </summary>
        // ------------------------------------------------------------------
        Size Size
        {
            get;
            set;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the bounds to paint.
        /// </summary>
        // ------------------------------------------------------------------
        Rectangle Bounds
        {
            get;
            set;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the fill style (solid, gradient, or hatch).
        /// </summary>
        // ------------------------------------------------------------------
        Fill Fill
        {
            get;
            set;
        }

        #endregion

        #region Methods

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates a Matrix for rotation.  The center point returned by
        /// property 'Center' is used as the rotation point.
        /// </summary>
        /// <returns>Matrix</returns>
        // ------------------------------------------------------------------
        Matrix CreateRotationMatrix();

        // ------------------------------------------------------------------
        /// <summary>
        /// Transforms the bounds of this painter by the amount specified.
        /// </summary>
        /// <param name="pointOffset">Point: The offset to apply to the
        /// location of the upper left-hand corner of the bounding 
        /// rectangle.</param>
        /// <param name="sizeOffset">Size: The amount to offset the current
        /// size of the bounding rectangle by.</param>
        // ------------------------------------------------------------------
        void Transform(Point pointOffset, Size sizeOffset);       

        #endregion
    }
}
