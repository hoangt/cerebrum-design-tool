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
 * MappingFPGAControl.xaml.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: Represents a FPGA within the mapping algorithm.
 * History: 
 * >> (28 Sep 2010) Matthew Cotter: Added customizable background color and text support.
 * >> (17 Sep 2010) Matthew Cotter: Added full implementations of IMappingControl and ICollapsible interfaces.
 * >> (15 Sep 2010) Matthew Cotter: Updated attachment methods and child object location method.
 * >> (13 Sep 2010) Matthew Cotter: Created basic definition of FPGA object.
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

namespace CerebrumMappingControls
{
    /// <summary>
    /// Interaction logic for MappingFPGAControl.xaml
    /// </summary>
    public partial class MappingFPGAControl : UserControl, IMappingControl, ICollapsible
    {
        /// <summary>
        /// Default constructor.  Initializes background and alignment properties.
        /// </summary>
        public MappingFPGAControl()
        {
            InitializeComponent();
            this.Background = new SolidColorBrush(Colors.Teal);
            this.Inputs = new List<MappingLinkControl>();
            this.Outputs = new List<MappingLinkControl>();
            this.Alignable = true;
        }

        /// <summary>
        /// Image to be used as background for the control
        /// </summary>
        public Image DisplayImage { get; set; }

        /// <summary>
        /// List of Links that source the FPGA as inputs
        /// </summary>
        public List<MappingLinkControl> Inputs { get; set; }
        /// <summary>
        /// List of Links that sink the FPGA as outputs
        /// </summary>
        public List<MappingLinkControl> Outputs { get; set; }


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
        public Canvas ObjectCanvas { get { return root; } }
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
        public List<IMappingControl> GetChildren() { return Mapping.GetChildren(root); }
        /// <summary>
        /// Aligns all children within this control
        /// </summary>
        public void AlignChildren() { Mapping.AlignChildren(this, this.AlignmentColumns, this.AlignmentSpacing); }
        /// <summary>
        /// Removes all children from this control
        /// </summary>
        public void ClearChildren() { foreach (IMappingControl imc in GetChildren()) imc.ClearChildren(); root.Children.Clear(); }
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
            if ((this.GetType() == ChildType) &&
                (this.MappingID == ID))
                return this;
            IMappingControl i = null;
            foreach (Object o in GetChildren())
            {
                i = ((IMappingControl)o).GetChild(ID, ChildType);
                if (i != null)
                    break;
            }
            return i;
        }
        /// <summary>
        /// Function used to locate a child object at a mouse location
        /// </summary>
        /// <param name="mouse">The mouse device used to locate the mouse</param>
        /// <returns>A object, if one was found, at the location of the mouse</returns>
        public IMappingControl GetObjectAtMouse(MouseDevice mouse) { return Mapping.GetObjectAtMouse(this, mouse); }

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
        private void RenderBackgroundText()
        {
            this.Background = BackgroundBrush;
            Mapping.RenderTextToCanvas(this.ObjectCanvas, BackgroundBrush, Brushes.Black, this.MappingID);
        }
        /// <summary>
        /// Shows the control
        /// </summary>
        public void Show()
        {
            this.Visibility = Visibility.Visible;
            RenderBackgroundText();
            if (!this.Collapsed)
            {
                foreach (UIElement uie in root.Children)
                {
                    if (uie is IMappingControl)
                    {
                        ((IMappingControl)uie).Show();
                        uie.Visibility = Visibility.Visible;
                    }
                }
            }
            this.AlignChildren();
        }
        /// <summary>
        /// Hides the control
        /// </summary>
        public void Hide()
        {
            this.Visibility = Visibility.Hidden;
            this.Background = BackgroundBrush;
            this.ObjectCanvas.Background = BackgroundBrush;
            foreach (UIElement uie in root.Children)
            {
                if (uie is IMappingControl)
                {
                    ((IMappingControl)uie).Hide();
                    uie.Visibility = Visibility.Hidden;
                }
            }
        }
        
        #endregion

        #region ICollapsible Implementation
        private bool bCollapsed;
        /// <summary>
        /// Indicates whether the control is collapsed
        /// </summary>
        public bool Collapsed
        {
            get
            {
                return bCollapsed;
            }
        }

        private double collapseScaleX;
        private double collapseScaleY;
        /// <summary>
        /// Get or set the x-dimension collapsing scale
        /// </summary>
        public double CollapsedXScale
        {
            get
            {
                return collapseScaleX;
            }
            set
            {
                if (this.Collapsed)
                    return;
                if (value > 1.0)
                    value = 1.0;
                if (value < 0.0)
                    value = 0.0;
                collapseScaleX = value;
            }
        }
        /// <summary>
        /// Get or set the y-dimension collapsing scale
        /// </summary>
        public double CollapsedYScale
        {
            get
            {
                return collapseScaleY;
            }
            set
            {
                if (this.Collapsed)
                    return;
                if (value > 1.0)
                    value = 1.0;
                if (value < 0.0)
                    value = 0.0;
                collapseScaleY = value;
            }
        }

        private double currentScaleX;
        private double currentScaleY;
        /// <summary>
        /// Get the current x-dimension scale
        /// </summary>
        public double CurrentXScale
        {
            get
            {
                return currentScaleX;
            }
        }
        /// <summary>
        /// Get the current y-dimension scale
        /// </summary>
        public double CurrentYScale
        {
            get
            {
                return currentScaleY;
            }
        }

        /// <summary>
        /// Initialzes the collapsible interface
        /// </summary>
        /// <param name="xScale">The initial x-dimension collapsing scale</param>
        /// <param name="yScale">The initial y-dimension collapsing scale</param>
        public void InitCollapsible(double xScale, double yScale)
        {
            CollapsedXScale = xScale;
            CollapsedYScale = yScale;
            bCollapsed = false;
            currentScaleX = 1.0;
            currentScaleY = 1.0;
        }
        private double FullWidth;
        private double FullHeight;
        /// <summary>
        /// Toggles the current state from Collapsed to Expanded, or vice versa.
        /// </summary>
        public void Toggle()
        {
            if (this.Collapsed)
                Expand();
            else
                Collapse();
        }
        /// <summary>
        /// Expands the control if it is collapsed
        /// </summary>
        public void Expand()
        {
            if (!this.Collapsed)
                return;

            Scale(1 / currentScaleX, 1 / currentScaleY);
            foreach (IMappingControl imap in GetChildren())
                ((UIElement)imap).Visibility = Visibility.Visible;
            bCollapsed = false;
            this.Alignable = !bCollapsed;
            this.Show();
        }
        /// <summary>
        /// Collapses the control if it is not collapsed
        /// </summary>
        public void Collapse()
        {
            if (this.Collapsed)
                return;
            bCollapsed = true;
            this.Alignable = !bCollapsed;
            FullWidth = this.Width;
            FullHeight = this.Height;
            Scale(collapseScaleX, collapseScaleY);
            foreach (IMappingControl imap in GetChildren())
                ((UIElement)imap).Visibility = Visibility.Hidden;
        }
        private void Scale(double ScaleXFactor, double ScaleYFactor)
        {
            Point myCenter = Mapping.GetCenter(this);
            double myW = this.Width;
            double myH = this.Height;

            double newW = myW * ScaleXFactor;
            double newH = myH * ScaleYFactor;

            Mapping.SetCenter(this, myCenter, newW, newH);
            currentScaleX = ScaleXFactor;
            currentScaleY = ScaleYFactor;
        }
        #endregion
    }
}
