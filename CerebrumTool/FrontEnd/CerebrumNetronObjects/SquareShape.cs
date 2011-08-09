/******************************************************************** 
 * Cerebrum Embedded System Design Automation Framework
 * Copyright (C) 2010  The Pennsylvania State University
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 ********************************************************************/
/******************************************************************** 
 * SquareShape.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: This is a Netron-based object used to represent a square-shaped design object.
 * History: 
 * >> (13 Sep 2010) Matthew Cotter: Implemented subclass of Netron ImageShape class to be used by CerebrumCore for representation of design objects.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Netron.Diagramming.Core;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace CerebrumNetronObjects
{
    /// <summary>
    /// Square-shaped Netron object with the capabilty to display an Image on its canvas
    /// </summary>
    public class SquareShape : ImageShape
    {
        #region Fields
        /// <summary>
        /// Generic entity name of the shape object.  Overriden in child classes
        /// </summary>
        protected string mEntityName = "Square";

        // ------------------------------------------------------------------
        /// <summary>
        /// The current version - used when deserializing the shape.
        /// </summary>
        // ------------------------------------------------------------------
        protected double squareShapeVersion = 1.0;

        #endregion

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the graphics path that defines this square.
        /// </summary>
        // ------------------------------------------------------------------
        
        public GraphicsPath Path
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                path.AddLine(TopLeftCorner,
                    TopRightCorner);
                path.AddLine(TopRightCorner,
                    BottomRightCorner);
                path.AddLine(BottomRightCorner,
                    BottomLeftCorner);
                path.AddLine(BottomLeftCorner,
                    TopLeftCorner);
                return path;
            }
        }
        private float BorderWidth = 3.0F;
        /// <summary>
        /// Defines the Graphics path the defines the border of the object, for drawing purposes only
        /// </summary>
        public GraphicsPath BorderPath
        {
            get
            {
                GraphicsPath path = new GraphicsPath();
                path.AddLine(TopLeftCorner.X - BorderWidth, TopLeftCorner.Y - BorderWidth,
                    TopRightCorner.X + BorderWidth, TopRightCorner.Y - BorderWidth);
                path.AddLine(TopRightCorner.X + BorderWidth, TopRightCorner.Y - BorderWidth,
                    BottomRightCorner.X + BorderWidth, BottomRightCorner.Y + BorderWidth);
                path.AddLine(BottomRightCorner.X + BorderWidth, BottomRightCorner.Y + BorderWidth,
                    BottomLeftCorner.X - BorderWidth, BottomLeftCorner.Y + BorderWidth);
                path.AddLine(BottomLeftCorner.X - BorderWidth, BottomLeftCorner.Y + BorderWidth,
                    TopLeftCorner.X - BorderWidth, TopLeftCorner.Y - BorderWidth);
                return path;
            }
        }
        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the friendly name of this shape.
        /// </summary>
        // ------------------------------------------------------------------
        public override string EntityName
        {
            get
            {
                return mEntityName;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns the version of this shape.
        /// </summary>
        // ------------------------------------------------------------------
        public override double Version
        {
            get
            {
                return squareShapeVersion;
            }
        }

        #endregion

        #region Constructors

        // ------------------------------------------------------------------
        /// <summary>
        /// Default constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public SquareShape()
            : base()
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor that receives the model.
        /// </summary>
        /// <param name="model">IModel</param>
        // ------------------------------------------------------------------
        public SquareShape(IModel model)
            : base(model)
        {
        }        

        // -------------------------------------------------------------------
        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        // -------------------------------------------------------------------
        protected SquareShape(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            double version = info.GetDouble("SquareShapeVersion");
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Initializes the shape.
        /// </summary>
        // ------------------------------------------------------------------
        protected override void Initialize()
        {
            base.Initialize();
            mPenStyle = new LinePenStyle();
            mPenStyle.Color = Color.Black;
            mPenStyle.DashStyle = DashStyle.Solid;
            mPenStyle.Width = 5;

            //mPaintStyle = new GradientPaintStyle(
            //    Color.White, // Start color
            //    Color.Silver, // End color
            //    90F); // Gradient angle

            Transform(this.Width, this.Height, 150, 150);
            mConnectors.Clear();

            Connector top = new Connector(Model);
            top.Parent = this;
            top.Point = new Point(TopCenter.X, TopCenter.Y + 10);
            mConnectors.Add(top);

            Connector bottom = new Connector(Model);
            bottom.Parent = this;
            bottom.Point = new Point(BottomCenter.X, BottomCenter.Y - 10);
            mConnectors.Add(bottom);

            Connector left = new Connector(Model);
            left.Parent = this;
            left.Point = new Point(    // LeftCenter
                TopLeftCorner.X + 10,
                TopLeftCorner.Y + (mRectangle.Height / 2));
            mConnectors.Add(left);

            Connector right = new Connector(Model);
            right.Parent = this;
            right.Point = new Point(    // RightCenter
                TopLeftCorner.X + (mRectangle.Width) - 10,
                TopLeftCorner.Y + (mRectangle.Height / 2));
            mConnectors.Add(right);

            Connector center = new Connector(Model);
            center.Parent = this;
            center.Point = Center;
            mConnectors.Add(center);
        }

        #region Serialization

        // -------------------------------------------------------------------
        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        // -------------------------------------------------------------------
        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("SquareShapeVersion", squareShapeVersion);
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns if the point specified falls within the graphics
        /// path that defines this triangle (i.e. not in our rectangular
        /// bounds!).
        /// </summary>
        /// <param name="p">Point</param>
        /// <returns>bool</returns>
        // ------------------------------------------------------------------
        public override bool Hit(Point p)
        {
            return Path.IsVisible(p);
        }

        #region Painting

        // ------------------------------------------------------------------
        /// <summary>
        /// Paints the shape.
        /// </summary>
        /// <param name="g"></param>
        // ------------------------------------------------------------------
        public override void Paint(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            base.Paint(g);

            if (mVisible == false)
            {
                return;
            }

            Pen pen = mPenStyle.DrawingPen();

            if (mHovered)
            {
                pen = ArtPalette.HighlightPen;
            }

            GraphicsPath path = Path;
            GraphicsPath border = BorderPath;

            // Draw the shadow first so it's in the background.
            DrawShadow(g);
            SolidBrush backBrush = new SolidBrush(pen.Color);
            g.FillPath(backBrush, border);
            g.FillPath(mPaintStyle.GetBrush(mRectangle), path);

            foreach (IConnector con in mConnectors)
            {
                con.Paint(g);
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Draws the shape's shadow.
        /// </summary>
        /// <param name="g">Graphics</param>
        // ------------------------------------------------------------------
        public void DrawShadow(Graphics g)
        {
            Point topRight = TopRightCorner;
            topRight.Offset(5, 5);

            Point bottomRight = BottomRightCorner;
            bottomRight.Offset(5, 5);

            Point bottomLeft = BottomLeftCorner;
            bottomLeft.Offset(5, 5);

            GraphicsPath shadow = new GraphicsPath();
            shadow.AddLine(topRight, bottomRight);
            shadow.AddLine(bottomRight, bottomLeft);
            //shadow.AddLine(bottomLeft, bottomLeft.Offset(1, 1));
            //shadow.AddLine(bottomLeft.Offset(1, 1), bottomRight.Offset(1, 1));
            //shadow.AddLine(bottomRight.Offset(1, 1), topRight.Offset(1, 1));

            g.FillPath(ArtPalette.ShadowBrush, shadow);
        }

        #endregion

    }
}
