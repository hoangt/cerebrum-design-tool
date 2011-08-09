using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace GraphicsLibrary
{
    public interface IRenderer : ISerializable
    {
        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns a name that describes what this painter is used for (such
        /// as 'Background Painter', 'Rectangle Painter', etc.).
        /// </summary>
        // ------------------------------------------------------------------
        string Name
        {
            get;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the region (area) that describes the interior of a graphics
        /// shape compsed of rectangles and paths.
        /// </summary>
        // ------------------------------------------------------------------
        Region Region
        {
            get;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the GraphicsPath that defines the area to be drawn.
        /// </summary>
        // ------------------------------------------------------------------
        GraphicsPath GraphicsPath
        {
            get;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the overall quality when rendering GDI+ objects.
        /// </summary>
        // ------------------------------------------------------------------
        QualityMode Quality
        {
            get;
            set;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the smoothing mode (such as antialiasing) used.
        /// </summary>
        // ------------------------------------------------------------------
        SmoothingMode SmoothingMode
        {
            get;
            set;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the style of the line.
        /// </summary>
        // ------------------------------------------------------------------
        LineStyle LineStyle
        {
            get;
            set;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the line color.
        /// </summary>
        // ------------------------------------------------------------------
        Color LineColor
        {
            get;
            set;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the width of the line.
        /// </summary>
        // ------------------------------------------------------------------
        float LineWidth
        {
            get;
            set;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets if a shadow is drawn.
        /// </summary>
        // ------------------------------------------------------------------
        bool ShowShadow
        {
            get;
            set;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the offset (in x and y direction) of the shadow.
        /// </summary>
        // ------------------------------------------------------------------
        Point ShadowOffset
        {
            get;
            set;
        }

        #endregion

        #region Methods

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns if the point specified falls within the bounds of this
        /// painter.
        /// </summary>
        /// <param name="point"></param>
        /// <returns>bool: True if the point lies within the bounds.</returns>
        // ------------------------------------------------------------------
        bool Hit(Point point);

        // ------------------------------------------------------------------
        /// <summary>
        /// Paints the current GraphicsPath with the current Fill and 
        /// Outline settings.
        /// </summary>
        /// <param name="area">Rectangle</param>
        /// <param name="g">Graphics</param>
        // ------------------------------------------------------------------
        void Paint(Graphics g);

        // ------------------------------------------------------------------
        /// <summary>
        /// Draws only the border.
        /// </summary>
        /// <param name="g">Graphics</param>
        // ------------------------------------------------------------------
        void DrawBorder(Graphics g);

        // ------------------------------------------------------------------
        /// <summary>
        /// Moves the upper left-hand corner for the bounds of the painter 
        /// to the specified location.
        /// </summary>
        /// <param name="point">Point</param>
        // ------------------------------------------------------------------
        void MoveTo(Point point);

        // ------------------------------------------------------------------
        /// <summary>
        /// Moves the location of the upper left-hand corner of the bounds
        /// by the amount specified.
        /// </summary>
        /// <param name="dx">int: The amount to move in the x direction.
        /// </param>
        /// <param name="dy">int: The amount to move in the y direction.
        /// </param>
        // ------------------------------------------------------------------
        void MoveBy(int dx, int dy);

        // ------------------------------------------------------------------
        /// <summary>
        /// Generates the GraphicsPath for this shape.
        /// </summary>
        /// <returns>GraphicsPath</returns>
        // ------------------------------------------------------------------
        void CreateGraphicsPath();

        #endregion
    }
}
