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
 * MappingLinkControl.xaml.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: Represents a physical link between FPGAs within the mapping algorithm.
 * History: 
 * >> (17 Sep 2010) Matthew Cotter: Finished implementation of IMappingControl and display of link-connection lines.
 * >> (13 Sep 2010) Matthew Cotter: Created basic definition of Link control.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace CerebrumMappingControls
{
    /// <summary>
    /// Interaction logic for MappingLinkControl.xaml
    /// </summary>
    public partial class MappingLinkControl : UserControl, IMappingControl
    {
        /// <summary>
        /// Default constructor.  Initializes the control with default values.
        /// </summary>
        public MappingLinkControl()
        {
            InitializeComponent();
            this.Source = null;
            this.Sink = null;
            this.Weight = 1.0F;
            this.Alignable = false;
        }

        /// <summary>
        /// Get or set the FPGA that sources the link
        /// </summary>
        public MappingFPGAControl Source { get; set; }
        /// <summary>
        /// Get or set the FPGA that sinks the link
        /// </summary>
        public MappingFPGAControl Sink { get; set; }
        /// <summary>
        /// Get or set the location of the source port in the UI
        /// </summary>
        public Point SourcePort { get; set; }
        /// <summary>
        /// Get or set the location of the sink port in the UI
        /// </summary>
        public Point SinkPort { get; set; }
        /// <summary>
        /// Get or set the weight of the connection in the UI
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Sets the source link and relative locaton of the source port
        /// </summary>
        /// <param name="FPGA">The source link FPGA</param>
        /// <param name="RelativePort">The relative location of the source port</param>
        public void SetSource(MappingFPGAControl FPGA, Point RelativePort)
        {
            this.Source = FPGA;
            this.SourcePort = RelativePort;
        }
        /// <summary>
        /// Sets the sink link and relative locaton of the source port
        /// </summary>
        /// <param name="FPGA">The sink link FPGA</param>
        /// <param name="RelativePort">The relative location of the sink port</param>
        public void SetSink(MappingFPGAControl FPGA, Point RelativePort)
        {
            this.Sink = FPGA;
            this.SinkPort = RelativePort;
        }

        /// <summary>
        /// Draws the link on the GUI
        /// </summary>
        /// <param name="drawOn">The canvas on which the link should be drawn</param>
        public void Draw(Canvas drawOn)
        {
            if ((this.Source == null) || (this.Sink == null))
                return;

            Point TopLeft = Source.GetGlobalTopLeft();
            Point BottomRight = Sink.GetGlobalTopLeft();

            Point Start = new Point(TopLeft.X + (Source.Width * SourcePort.X), TopLeft.Y + (Source.Height * SourcePort.Y));
            Point End = new Point(BottomRight.X + (Sink.Width * SinkPort.X), BottomRight.Y + (Sink.Height * SinkPort.Y));

            line.Visibility = Visibility.Hidden;

            line.Stroke = this.BackgroundBrush;
            line.ClipToBounds = true;
            line.Margin = new Thickness(0, 0, 0, 0);
            line.StrokeThickness = (this.Weight * 6);

            this.Top = 0;
            this.Left = 0;
            this.Width = (double)drawOn.GetValue(Canvas.WidthProperty);
            this.Height = (double)drawOn.GetValue(Canvas.HeightProperty);

            Start = Mapping.GetGlobalCenter(Source);
            End = Mapping.GetGlobalCenter(Sink);

            //this.Background = Brushes.Black;

            line.Width = this.Width;
            line.Height = this.Height;
            line.X1 = Start.X;
            line.Y1 = Start.Y;

            line.X2 = End.X;
            line.Y2 = End.Y;
            //Trace.WriteLine(String.Format("Drawing Connection from {0} to {1}", Start, End));
            line.Visibility = Visibility.Visible;
            if (!drawOn.Children.Contains(this))
                drawOn.Children.Add(this);
        }
        /// <summary>
        /// Erases the link on the GUI
        /// </summary>
        /// <param name="eraseFrom">The canvas from which the link should be erased</param>
        public void Erase(Canvas eraseFrom)
        {
            line.Visibility = Visibility.Hidden;
            if (!eraseFrom.Children.Contains(this))
                eraseFrom.Children.Remove(this);
        }

        #region IMappingControl Implementation

        // Generic Properties
        /// <summary>
        /// Indicates whether the control can be aligned as a child
        /// </summary>
        public bool Alignable { get; set; }
        /// <summary>
        /// Indicates how many columns the control's children are aligned into
        /// </summary>
        public int AlignmentColumns { get; set; }
        /// <summary>
        /// Indicates minimum spacing between aligned children
        /// </summary>
        public double AlignmentSpacing { get; set; }
        /// <summary>
        /// Defines the mapping name of the control
        /// </summary>
        public string MappingName { get; set; }
        /// <summary>
        /// Defines the mapping ID of the control
        /// </summary>
        public string MappingID { get; set; }
        /// <summary>
        /// Top-level Mapping canvas control each control is associated with
        /// </summary>
        public MappingCanvasControl ParentCanvas { get; set; }
        /// <summary>
        /// Generic canvas exposed by each control
        /// </summary>
        public Canvas ObjectCanvas { get { return null; } }
        /// <summary>
        /// Specifies the brush used to paint the background canvas
        /// </summary>
        public Brush BackgroundBrush { get; set; }

        // Size and Location
        /// <summary>
        /// Defines the top coordinate of the control
        /// </summary>
        public double Top { get { return Mapping.GetTop(this); } set { Mapping.SetTop(this, value); } }
        /// <summary>
        /// Defines the left coordinate of the control
        /// </summary>
        public double Left { get { return Mapping.GetLeft(this); } set { Mapping.SetLeft(this, value); } }
        /// <summary>
        /// Defines the right coordinate of the control
        /// </summary>
        public double Right { get { return Mapping.GetRight(this); } set { Mapping.SetRight(this, value); } }
        /// <summary>
        /// Defines the bottom coordinate of the control
        /// </summary>
        public double Bottom { get { return Mapping.GetBottom(this); } set { Mapping.SetBottom(this, value); } }
        /// <summary>
        /// Get or set the width of the control
        /// </summary>
        public new double Width { get { return Mapping.GetWidth(this); } set { Mapping.SetWidth(this, value); } }
        /// <summary>
        /// Get or set the height of the control
        /// </summary>
        public new double Height { get { return Mapping.GetHeight(this); } set { Mapping.SetHeight(this, value); } }
        /// <summary>
        /// Gets the top-left coordinate of the control with respect to the global coordinate space
        /// </summary>
        /// <returns></returns>
        public Point GetGlobalTopLeft() { return Mapping.GetGlobalTopLeft(this); }

        // Child Object Management
        /// <summary>
        /// Indicates whether this control has any child controls
        /// </summary>
        /// <returns>True if this control has children; false otherwise</returns>
        public bool HasChildren() { return false; }
        /// <summary>
        /// Gets a list of this control's child controls 
        /// </summary>
        /// <returns>A list of IMappingControls that are children of this control</returns>
        public List<IMappingControl> GetChildren() { return Mapping.GetChildren(null); }
        /// <summary>
        /// Aligns all children within this control
        /// </summary>
        public void AlignChildren() { }
        /// <summary>
        /// Removes all children from this control
        /// </summary>
        public void ClearChildren() { }
        /// <summary>
        /// Function used to determine if another mapping control can be placed within this one
        /// </summary>
        /// <param name="o">A control to be tested for compatibility</param>
        /// <returns>True if this control can contain the other; false otherwise</returns>
        public bool CanAccept(Object o) { return Mapping.CanXContainY(this, o); }
        /// <summary>
        /// Gets the child of the specified type and ID
        /// </summary>
        /// <param name="ID">The ID of the child to retrieve</param>
        /// <param name="ChildType">The type of the child to retrieve</param>
        /// <returns>The child if it was found; null otherwise</returns>
        public IMappingControl GetChild(string ID, Type ChildType)
        {
            if (ChildType != this.GetType())
                return null;
            if (this.MappingID != ID)
                return null;

            return this;
        }
        /// <summary>
        /// Function used to locate a child object at a mouse location
        /// </summary>
        /// <param name="mouse">The mouse device used to locate the mouse</param>
        /// <returns>A object, if one was found, at the location of the mouse</returns>
        public IMappingControl GetObjectAtMouse(MouseDevice mouse)
        {
            Point p = mouse.GetPosition(this);
            double x = p.X;
            double y = p.Y;

            if (line.IsMouseOver)
                return this;
            return null;
        }

        // Parent Object Management 
        /// <summary>
        /// Gets the parent control to which this is currently attached
        /// </summary>
        public IMappingControl AttachedParent { get; set; }
        /// <summary>
        /// Gets the parent control to which this was most recently attached
        /// </summary>
        public IMappingControl RecentParent { get; set; }
        /// <summary>
        /// Attaches the control to the specified parent
        /// </summary>
        /// <param name="newParent"></param>
        public void AttachToParent(IMappingControl newParent) { if (newParent == AttachedParent) return; Mapping.AttachAToB(this, newParent); }
        /// <summary>
        /// Detaches the control from its current parent
        /// </summary>
        public void DetachFromParent() { Mapping.DetachFromParent(this); }
        /// <summary>
        /// Attaches the control to its most recent parent
        /// </summary>
        public void ReAttachToParent() { Mapping.ReAttachToParent(this); }

        // Visibility
        /// <summary>
        /// Shows the control
        /// </summary>
        public void Show()
        {
            this.Visibility = Visibility.Visible;
            Draw(this.ParentCanvas.drawingBoard);
        }
        /// <summary>
        /// Hides the control
        /// </summary>
        public void Hide()
        {
            this.Visibility = Visibility.Hidden;
            Erase(this.ParentCanvas.drawingBoard);
        }
        #endregion
    }
}
