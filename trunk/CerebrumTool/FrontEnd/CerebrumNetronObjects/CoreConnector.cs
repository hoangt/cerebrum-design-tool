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
 * CoreConnector.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: This is a Netron-based control used to display library cores and design layout and tools.
 * History: 
 * >> ( 1 Feb 2011) Matthew Cotter: Corrected the way connectors are highlighted when the mouse moves over them.
 * >> (26 Jan 2011) Matthew Cotter: Corrected a bug in which the connector ports would not rotate along with the core, as it was rotated.
 *                                      Note: As of this comment, the only asymmetric shape that was affected was the triangle, and it was corrected.
 * >> (16 Dec 2010) Matthew Cotter: Added CoreInstance property indicating the internal core instance to which this connector corresponds.
 *                                  Corrected GUI drawing of connector to facilitate proper display on rotated cores.
 *                                  Changed default appearance (shape/color) of SOPInput PortType.
 * >> (10 Oct 2010) Matthew Cotter: Added support for Vortex-based SAP/SOP core ports.
 *                                  Implemented basic functionality necessary to support port compatibility.
 * >> (13 Sep 2010) Matthew Cotter: Created Netron-inspired connector object representing a port on a design core.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Netron;
using Netron.Diagramming;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Drawing.Imaging;
using System.IO;
using System.ComponentModel;

namespace CerebrumNetronObjects
{
    /// <summary>
    /// Enumeration of port types
    /// </summary>
    public enum PortType
    {
        /// <summary>
        /// Invalid port type specification
        /// </summary>
        Invalid,
        /// <summary>
        /// I/O Interface port (Ethernet, PCI-E, Serial, Parallel, etc)
        /// </summary>
        IOInterface,
        /// <summary>
        /// SAP Initiator port
        /// </summary>
        SAPInitiator,
        /// <summary>
        /// SAP Target port
        /// </summary>
        SAPTarget,
        /// <summary>
        /// SOP Input port
        /// </summary>
        SOPInput,
        /// <summary>
        /// SOP Output port
        /// </summary>
        SOPOutput
    }
    
    /// <summary>
    /// Static class of functions and constants related to port types, styles and colors
    /// </summary>
    public static class Ports
    {
        /// <summary>
        /// Returns a list of compatible port types, given a known port type.
        /// </summary>
        /// <param name="SourceType">The known port type</param>
        /// <returns>A List of valid port types that would result in a valid pairing with the known port.</returns>
        public static List<PortType> GetCompatibleTargetTypes(PortType SourceType)
        {
            List<PortType> ValidTargets = new List<PortType>();
            switch (SourceType)
            {
                case PortType.IOInterface:
                    // Must match another IOInterface
                    ValidTargets.Add(PortType.IOInterface);
                    break;

                case PortType.SAPInitiator:
                    // Must connect to SAPTarget or SOPInput
                    ValidTargets.Add(PortType.SAPTarget);
                    ValidTargets.Add(PortType.SOPInput);
                    break;
                case PortType.SAPTarget:
                    // Must connect to SAPInitiator or SOPOutput
                    ValidTargets.Add(PortType.SAPInitiator);
                    ValidTargets.Add(PortType.SOPOutput);
                    break;

                case PortType.SOPInput:
                    // Must connect to SAPInitiator or SOPInput
                    ValidTargets.Add(PortType.SAPInitiator);
                    ValidTargets.Add(PortType.SOPOutput);
                    break;
                case PortType.SOPOutput:
                    // Must connect to SAPTarget or SOPInput
                    ValidTargets.Add(PortType.SAPTarget);
                    ValidTargets.Add(PortType.SOPInput);
                    break;
            }
            return ValidTargets;
        }

        /// <summary>
        /// Static class defining constants related to the display styles of port types
        /// </summary>
        public static class Styles
        {
            /// <summary>
            /// IO Interface port style
            /// </summary>
            readonly public static ConnectorStyle IOInterface = ConnectorStyle.Square;
            /// <summary>
            /// SAP Initiator port style
            /// </summary>
            readonly public static ConnectorStyle SAPInitiator = ConnectorStyle.Round;
            /// <summary>
            /// SAP Target port style
            /// </summary>
            readonly public static ConnectorStyle SAPTarget = ConnectorStyle.Diamond;
            /// <summary>
            /// SOP Input port style
            /// </summary>
            readonly public static ConnectorStyle SOPInput = ConnectorStyle.Square;
            /// <summary>
            /// SOP Output port style
            /// </summary>
            readonly public static ConnectorStyle SOPOutput = ConnectorStyle.RightTriangle;
        }

        /// <summary>
        /// Static class defining constants related to the display colors of port types
        /// </summary>
        public static class Colors
        {
            /// <summary>
            /// IO Interface port color
            /// </summary>
            readonly public static Color IOInterface = Color.Navy;
            /// <summary>
            /// SAP Initiator port color
            /// </summary>
            readonly public static Color SAPInitiator = Color.Red;
            /// <summary>
            /// SAP Target port color
            /// </summary>
            readonly public static Color SAPTarget = Color.Green;
            /// <summary>
            /// SOP Input port color
            /// </summary>
            readonly public static Color SOPInput = Color.Brown;
            /// <summary>
            /// SOP Output port color
            /// </summary>
            readonly public static Color SOPOutput = Color.Orange;
        }
    }

    /// <summary>
    /// Object representing a connector port on a core object
    /// </summary>
    [Serializable]
    public class CoreConnector : Connector
    {
        private int iConnSize = 6;
        /// <summary>
        /// Default Netron constructor.  Initializes Connector port properties
        /// </summary>
        /// <param name="site"></param>
        public CoreConnector(IModel site)
            : base(site)
        {
            this.PortName = string.Empty;
            this.PortType = PortType.Invalid;
            this.PortInterface = string.Empty;
        }
        /// <summary>
        /// Defines the bounding rectangle of the connector
        /// </summary>
        public override Rectangle Rectangle
        {
            get
            {
                return new Rectangle(Point.X - iConnSize, Point.Y - iConnSize, (2 * iConnSize), (2 * iConnSize));
            }
        }

        /// <summary>
        /// Method used to determine whether the specified point is a 'hit' on the object.
        /// </summary>
        /// <param name="p">The point to perform hit-testing on</param>
        /// <returns>True if the specified point falls on or in this connector; False otherwise.</returns>
        public override bool Hit(Point p)
        {
            return this.Rectangle.Contains(p);
        }

        /// <summary>
        /// Get or set the default size of the connector port
        /// </summary>
        public int ConnectorSize
        {
            get
            {
                return iConnSize;
            }
            set
            {
                if (value < 2)
                    value = 2;
                if (value > (0.2 * ((CerebrumCore)(this.Parent)).Width))
                    value = (int)(0.2 * ((CerebrumCore)(this.Parent)).Width);
                if (value > (0.2 * ((CerebrumCore)(this.Parent)).Height))
                    value = (int)(0.2 * ((CerebrumCore)(this.Parent)).Height);
                iConnSize = value;
            }
        }
        /// <summary>
        /// Get or set the name of the port
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// Get or set the type of the port
        /// </summary>
        public PortType PortType { get; set; }
        /// <summary>
        /// Get or set the interface type of the port (applicable if PortType == IOInterface)
        /// </summary>
        public string PortInterface { get; set; }
        /// <summary>
        /// Get or set the instance name of the internal core this port is associated with
        /// </summary>
        public string CoreInstance { get; set; }
        
        /// <summary>
        /// Get or set the scaled X-Offset location of the port
        /// </summary>
        public float XOffset { get; set; }
        /// <summary>
        /// Get or set the scaled Y-Offset location of the port
        /// </summary>
        public float YOffset { get; set; }
        /// <summary>
        /// Get or set the sizing scale factor of the connector port
        /// </summary>
        public float ScaleFactor { get; set; }

        /// <summary>
        /// Get or set the CerebrumCore to which this connector/port is attached
        /// </summary>
        public CerebrumCore Core { get; set; }
        /// <summary>
        /// Get or set the color of the connector
        /// </summary>
        public Color ConnectorColor { get; set; }
        /// <summary>
        /// Override of the paint method used to draw the connector on the specified Graphics object
        /// </summary>
        /// <param name="g">The graphics object on which the connector is drawn</param>
        public override void Paint(Graphics g)
        {
            if (g == null)
            {
                throw new ArgumentNullException(
                    "The Graphics object is 'null'");
            }

            if (Visible)
            {
                Color portColor = this.ConnectorColor;

                if (Hovered || IsSelected)
                {
                    portColor = Color.FromArgb((byte)~portColor.R, (byte)~portColor.G, (byte)~portColor.B);
                }
                Point TopLeft = new Point();
                Point TopRight = new Point();
                Point BottomLeft = new Point();
                if (this.Core.RotationAngle == 0)
                {
                    TopLeft = (TopLeftCorner);
                    TopRight = (TopRightCorner);
                    BottomLeft = (BottomLeftCorner);
                }
                else if (this.Core.RotationAngle == 90)
                {
                    TopLeft = (TopRightCorner);
                    TopRight = (BottomRightCorner);
                    BottomLeft = (TopLeftCorner);
                }
                else if (this.Core.RotationAngle == 180)
                {
                    TopLeft = (BottomRightCorner);
                    TopRight = (BottomLeftCorner);
                    BottomLeft = (TopRightCorner);
                }
                else if (this.Core.RotationAngle == 270)
                {
                    TopLeft = (BottomLeftCorner);
                    TopRight = (TopLeftCorner);
                    BottomLeft = (BottomRightCorner);
                }
                switch (this.myStyle)
                {
                    case ConnectorStyle.Simple:
                        DrawSimpleConnector(g);
                        break;

                    case ConnectorStyle.Round:
                        DrawRoundConnector(g, portColor);
                        break;

                    case ConnectorStyle.Square:
                        DrawSquareConnector(g, portColor);
                        break;

                    case ConnectorStyle.LeftTriangle:
                        DrawTriangleConnector(g, portColor);
                        break;

                    case ConnectorStyle.RightTriangle:
                        DrawTriangleConnector(g, portColor);
                        break;

                    case ConnectorStyle.UpTriangle:
                        DrawTriangleConnector(g, portColor);
                        break;

                    case ConnectorStyle.DownTriangle:
                        DrawTriangleConnector(g, portColor);
                        break;

                    case ConnectorStyle.Diamond:
                        DrawDiamondConnector(g, portColor);
                        break;
                }
            }
        }

        /// <summary>
        /// Draws the connector as a circle
        /// </summary>
        /// <param name="g">The graphics object on which the connector is drawn</param>
        /// <param name="portColor">The color with which the connector is drawn</param>
        protected virtual void DrawRoundConnector(Graphics g, Color portColor)
        {
            Pen pen = new Pen(this.ConnectorColor, 1);
            Size sz = this.Rectangle.Size;

            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(
                this.Rectangle.Left,
                this.Rectangle.Top,
                sz.Width,
                sz.Height);
            g.FillPath(new SolidBrush(portColor), path);
        }
        /// <summary>
        /// Draws the connector as a Square
        /// </summary>
        /// <param name="g">The graphics object on which the connector is drawn</param>
        /// <param name="portColor">The color with which the connector is drawn</param>
        protected virtual void DrawSquareConnector(Graphics g, Color portColor)
        {
            Pen pen = new Pen(this.ConnectorColor, 1);

            GraphicsPath path = new GraphicsPath();
            path.AddLine(new Point(this.Rectangle.Left, this.Rectangle.Top), new Point(this.Rectangle.Right, this.Rectangle.Top));
            path.AddLine(new Point(this.Rectangle.Right, this.Rectangle.Top), new Point(this.Rectangle.Right, this.Rectangle.Bottom));
            path.AddLine(new Point(this.Rectangle.Right, this.Rectangle.Bottom), new Point(this.Rectangle.Left, this.Rectangle.Bottom));
            path.AddLine(new Point(this.Rectangle.Left, this.Rectangle.Bottom), new Point(this.Rectangle.Left, this.Rectangle.Top));
            g.FillPath(new SolidBrush(portColor), path);
        }
        /// <summary>
        /// Draws the connector as a triangle
        /// </summary>
        /// <param name="g">The graphics object on which the connector is drawn</param>
        /// <param name="portColor">The color with which the connector is drawn</param>
        protected virtual void DrawTriangleConnector(Graphics g, Color portColor)
        {
            Pen pen = new Pen(this.ConnectorColor, 1);            

            Point A = new Point(0, 0);
            Point B = new Point(0, 0); 
            Point C = new Point(0, 0);
            Point Center = new Point(
                ((this.Rectangle.Left + this.Rectangle.Right) / 2),
                ((this.Rectangle.Top + this.Rectangle.Bottom) / 2)                
                );

            ConnectorStyle DrawnStyle = this.ConnectorStyle;

            if (this.Core.RotationAngle == 0)
            {
            }
            else if (this.Core.RotationAngle == 90)
            {
                switch (this.ConnectorStyle)
                {
                    case ConnectorStyle.LeftTriangle:   // ^ Triangle
                        DrawnStyle = ConnectorStyle.UpTriangle;
                        break;
                    case ConnectorStyle.RightTriangle:  // V Triangle
                        DrawnStyle = ConnectorStyle.DownTriangle;
                        break;
                    case ConnectorStyle.UpTriangle:     // > Triangle
                        DrawnStyle = ConnectorStyle.RightTriangle;
                        break;
                    case ConnectorStyle.DownTriangle:   // < Triangle
                        DrawnStyle = ConnectorStyle.LeftTriangle;
                        break;
                }
            }
            else if (this.Core.RotationAngle == 180)
            {
                switch (this.ConnectorStyle)
                {
                    case ConnectorStyle.LeftTriangle:   // > Triangle
                        DrawnStyle = ConnectorStyle.RightTriangle;
                        break;
                    case ConnectorStyle.RightTriangle:  // < Triangle
                        DrawnStyle = ConnectorStyle.LeftTriangle;
                        break;
                    case ConnectorStyle.UpTriangle:     // V Triangle
                        DrawnStyle = ConnectorStyle.DownTriangle;
                        break;
                    case ConnectorStyle.DownTriangle:   // ^ Triangle
                        DrawnStyle = ConnectorStyle.UpTriangle;
                        break;
                }
            }
            else if (this.Core.RotationAngle == 270)
            {
                switch (this.ConnectorStyle)
                {
                    case ConnectorStyle.LeftTriangle:   // V Triangle
                        DrawnStyle = ConnectorStyle.DownTriangle;
                        break;
                    case ConnectorStyle.RightTriangle:  // ^ Triangle
                        DrawnStyle = ConnectorStyle.UpTriangle;
                        break;
                    case ConnectorStyle.UpTriangle:     // < Triangle
                        DrawnStyle = ConnectorStyle.LeftTriangle;
                        break;
                    case ConnectorStyle.DownTriangle:   // > Triangle
                        DrawnStyle = ConnectorStyle.RightTriangle;
                        break;
                }
            }

            switch (DrawnStyle)
            {
                case ConnectorStyle.LeftTriangle:   // < Triangle
                    A = this.TopRightCorner;
                    B = this.BottomRightCorner;
                    C = new Point(this.TopLeftCorner.X, Center.Y);
                    break;
                case ConnectorStyle.RightTriangle:  // > Triangle
                    A = this.TopLeftCorner;
                    B = this.BottomLeftCorner;
                    C = new Point(this.TopRightCorner.X, Center.Y);
                    break;
                case ConnectorStyle.UpTriangle:     // ^ Triangle
                    A = this.BottomLeftCorner;
                    B = this.BottomRightCorner;
                    C = new Point(Center.X, this.TopRightCorner.Y);
                    break;
                case ConnectorStyle.DownTriangle:   // V Triangle
                    A = this.TopLeftCorner;
                    B = this.TopRightCorner;
                    C = new Point(Center.X, this.BottomRightCorner.Y);
                    break;
            }
            GraphicsPath path = new GraphicsPath();
            path.AddLine(A, B);
            path.AddLine(B, C);
            path.AddLine(C, A);
            g.FillPath(new SolidBrush(portColor), path);
        }
        /// <summary>
        /// Draws the connector as a diamond
        /// </summary>
        /// <param name="g">The graphics object on which the connector is drawn</param>
        /// <param name="portColor">The color with which the connector is drawn</param>
        protected virtual void DrawDiamondConnector(Graphics g, Color portColor)
        {
            Pen pen = new Pen(this.ConnectorColor, 1);
            Point Center = new Point(
                ((this.Rectangle.Left + this.Rectangle.Right) / 2),
                ((this.Rectangle.Top + this.Rectangle.Bottom) / 2)
                );

            Point A = new Point(Center.X, this.TopLeftCorner.Y);
            Point B = new Point(this.BottomRightCorner.X, Center.Y);
            Point C = new Point(Center.X, this.BottomRightCorner.Y);
            Point D = new Point(this.TopLeftCorner.X, Center.Y);

            GraphicsPath path = new GraphicsPath();
            path.AddLine(A, B);
            path.AddLine(B, C);
            path.AddLine(C, D);
            path.AddLine(D, A);
            g.FillPath(new SolidBrush(portColor), path);
        }

        /// <summary>
        /// Gets the location of the center-point of the connector port
        /// </summary>
        public new Point Center
        {
            get
            {
                return new Point(this.Rectangle.Left + (this.Rectangle.Width / 2),
                                 this.Rectangle.Top + (this.Rectangle.Height / 2));
            }
        }
    }
}
