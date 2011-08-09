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
 * MappingComponentControl.xaml.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: Represents a component within the mapping algorithm.
 * History: 
 * >> (30 Sep 2010) Matthew Cotter: Added customizable background color and text support.
 * >> (17 Sep 2010) Matthew Cotter: Completed implementaton of IMappingControl.
 * >> (15 Sep 2010) Matthew Cotter: Added support for Image Display on component control.
 *                                  Corrected implementation of alignment and attachment methods.
 * >> (13 Sep 2010) Matthew Cotter: Basic definition of a generic component representation.
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
using System.IO;

namespace CerebrumMappingControls
{
    /// <summary>
    /// Interaction logic for MappingComponentControl.xaml
    /// </summary>
    public partial class MappingComponentControl : UserControl, IMappingControl
    {
        /// <summary>
        /// Default constructor.  Initializes background and alignment properties.
        /// </summary>
        public MappingComponentControl()
        {
            InitializeComponent();
            this.Background = new SolidColorBrush(Colors.Azure);
            this.Inputs = new List<MappingConnectionControl>();
            this.Outputs = new List<MappingConnectionControl>();
            this.Alignable = true;
        }

        /// <summary>
        /// Get or set the path to the image used for displaying the component
        /// </summary>
        public string ImageFile { get; set; }
        /// <summary>
        /// Shows the display image
        /// </summary>
        public void ShowImage()
        {
            this.Background = Brushes.Transparent;
            ImageBrush bgBrush;
            if ((ImageFile == null) || (ImageFile == string.Empty) || !(File.Exists(ImageFile)))
            {
                Mapping.RenderTextToCanvas(this.ObjectCanvas, BackgroundBrush, Brushes.Black, this.MappingID);
            }
            else
            {
                BitmapImage bi = new BitmapImage();
                // BitmapImage.UriSource must be in a BeginInit/EndInit block.
                bi.BeginInit();
                bi.UriSource = new Uri(this.ImageFile, UriKind.RelativeOrAbsolute);
                bi.EndInit();
                bgBrush = new ImageBrush(bi);
                bgBrush.Stretch = Stretch.Uniform;
                this.ObjectCanvas.Background = bgBrush;               
            }           
        }
        /// <summary>
        /// Hides the display image
        /// </summary>
        public void HideImage()
        {
            this.Background = BackgroundBrush;
            this.ObjectCanvas.Background = BackgroundBrush;
        }

        /// <summary>
        /// List of Connections that source the component as inputs
        /// </summary>
        public List<MappingConnectionControl> Inputs { get; set; }
        /// <summary>
        /// List of Connections that source the component as outputs
        /// </summary>
        public List<MappingConnectionControl> Outputs { get; set; }

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
        public void AlignChildren() { }
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
            ShowImage();
        }
        /// <summary>
        /// Hides the control
        /// </summary>
        public void Hide()
        {
            this.Visibility = Visibility.Hidden;
            HideImage();
        }
        #endregion
    }
}
