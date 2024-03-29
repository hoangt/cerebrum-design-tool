using System;
using System.Collections.Generic;
using System.Text;

namespace GraphicsLibrary
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// Specifies the direction of a triangle created by Geometry.
    /// </summary>
    // ----------------------------------------------------------------------
    public enum TriangleDirection
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// The apex is at the top.
        /// 
        ///             *
        ///            * *
        ///           *   *
        ///          *******
        /// </summary>
        // ------------------------------------------------------------------
        Up,

        // ------------------------------------------------------------------
        /// <summary>
        /// The apex is at the bottom.
        /// 
        ///          *******
        ///           *   *
        ///            * *
        ///             *
        /// </summary>
        // ------------------------------------------------------------------
        Down,

        // ------------------------------------------------------------------
        /// <summary>
        /// The apex is at the left.
        /// 
        ///              *
        ///            * * 
        ///          *   *
        ///            * *
        ///              *
        /// </summary>
        // ------------------------------------------------------------------
        Left,

        // ------------------------------------------------------------------
        /// <summary>
        /// The apex is at the right.
        /// 
        ///          *
        ///          * *
        ///          *  *
        ///          * *
        ///          *
        /// </summary>
        // ------------------------------------------------------------------
        Right
    }
}
