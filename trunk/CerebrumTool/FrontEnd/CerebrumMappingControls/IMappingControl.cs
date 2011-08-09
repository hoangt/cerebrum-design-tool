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
 * IMappingControl.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: Defines the interface exposed by a mapping control and the static class library of functions used to implement common control methods.
 * History: 
 * >> (22 Oct 2010) Matthew Cotter: Added missing try/catch blocks to fortify code against unhandled exceptions.
 *                                  Added access to MessageEventController to allow mapping UI exceptions to be propagated to top-level GUI.
 * >> (30 Sep 2010) Matthew Cotter: Corrected display and rendering of text.
 * >> (28 Sep 2010) Matthew Cotter: Added support to draw text on a control rather than an image.
 * >> (17 Sep 2010) Matthew Cotter: Added methods to get/set points and locations of mapping controls.
 *                                  Added methods to support attaching, detaching, and re-attaching of controls to/from parents
 * >> (15 Sep 2010) Matthew Cotter: Added generic alignment procedure to Mapping static class
 * >> (13 Sep 2010) Matthew Cotter: Defined generic mapping control interface and static class (Mapping) to define common 'helper' function implementations.
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
using System.Collections;
using CerebrumSharedClasses;

namespace CerebrumMappingControls
{
    /// <summary>
    /// Interface defining the properties and methods common to generic mapping interface controls
    /// </summary>
    public interface IMappingControl
    {
        /// <summary>
        /// Defines the top coordinate of the control
        /// </summary>
        double Top { get; set; }
        /// <summary>
        /// Defines the left coordinate of the control
        /// </summary>
        double Left { get; set; }
        /// <summary>
        /// Defines the right coordinate of the control
        /// </summary>
        double Right { get; set; }
        /// <summary>
        /// Defines the bottom coordinate of the control
        /// </summary>
        double Bottom { get; set; }

        /// <summary>
        /// Defines the mapping name of the control
        /// </summary>
        string MappingName { get; set; }
        /// <summary>
        /// Defines the mapping ID of the control
        /// </summary>
        string MappingID { get; set; }

        /// <summary>
        /// Indicates whether the control can be aligned as a child
        /// </summary>
        bool Alignable { get; set; }
        /// <summary>
        /// Indicates how many columns the control's children are aligned into
        /// </summary>
        int AlignmentColumns { get; set; }
        /// <summary>
        /// Indicates minimum spacing between aligned children
        /// </summary>
        double AlignmentSpacing { get; set; }

        /// <summary>
        /// Specifies the brush used to paint the background canvas
        /// </summary>
        Brush BackgroundBrush { get; set; }
        /// <summary>
        /// Generic canvas exposed by each control
        /// </summary>
        Canvas ObjectCanvas { get; }

        /// <summary>
        /// Top-level Mapping canvas control each control is associated with
        /// </summary>
        MappingCanvasControl ParentCanvas { get; set; }

        /// <summary>
        /// Function used to locate a child object at a mouse location
        /// </summary>
        /// <param name="mouse">The mouse device used to locate the mouse</param>
        /// <returns>A object, if one was found, at the location of the mouse</returns>
        IMappingControl GetObjectAtMouse(MouseDevice mouse);

        /// <summary>
        /// Function used to determine if another mapping control can be placed within this one
        /// </summary>
        /// <param name="o">A control to be tested for compatibility</param>
        /// <returns>True if this control can contain the other; false otherwise</returns>
        bool CanAccept(Object o);

        /// <summary>
        /// Indicates whether this control has any child controls
        /// </summary>
        /// <returns>True if this control has children; false otherwise</returns>
        bool HasChildren();
        /// <summary>
        /// Gets a list of this control's child controls 
        /// </summary>
        /// <returns>A list of IMappingControls that are children of this control</returns>
        List<IMappingControl> GetChildren();
        /// <summary>
        /// Removes all children from this control
        /// </summary>
        void ClearChildren();
        /// <summary>
        /// Aligns all children within this control
        /// </summary>
        void AlignChildren();

        /// <summary>
        /// Gets the child of the specified type and ID
        /// </summary>
        /// <param name="ID">The ID of the child to retrieve</param>
        /// <param name="ChildType">The type of the child to retrieve</param>
        /// <returns>The child if it was found; null otherwise</returns>
        IMappingControl GetChild(string ID, Type ChildType);

        /// <summary>
        /// Gets the parent control to which this is currently attached
        /// </summary>
        IMappingControl AttachedParent { get; set; }
        /// <summary>
        /// Gets the parent control to which this was most recently attached
        /// </summary>
        IMappingControl RecentParent { get; set; }

        /// <summary>
        /// Detaches the control from its current parent
        /// </summary>
        void DetachFromParent();
        /// <summary>
        /// Attaches the control to its most recent parent
        /// </summary>
        void ReAttachToParent();
        /// <summary>
        /// Attaches the control to the specified parent
        /// </summary>
        /// <param name="newParent"></param>
        void AttachToParent(IMappingControl newParent);

        /// <summary>
        /// Shows the control
        /// </summary>
        void Show();
        /// <summary>
        /// Hides the control
        /// </summary>
        void Hide();

        /// <summary>
        /// Gets the top-left coordinate of the control with respect to the global coordinate space
        /// </summary>
        /// <returns></returns>
        Point GetGlobalTopLeft();


        #region Implemented by WPF User Controls
        /// <summary>
        /// Sets the value of a dependency property of the control
        /// </summary>
        /// <param name="dp">The dependency property to set</param>
        /// <param name="value">The new value of the property</param>
        void SetValue(DependencyProperty dp, object value);
        /// <summary>
        /// Gets the value of a dependency property of the control
        /// </summary>
        /// <param name="dp">The dependency property to get</param>
        /// <returns>The value of the property</returns>
        object GetValue(DependencyProperty dp);
        /// <summary>
        /// Get or set the width of the control
        /// </summary>
        double Width
        {
            get;
            set;
        }
        /// <summary>
        /// Get or set the height of the control
        /// </summary>
        double Height
        {
            get;
            set;
        }
        /// <summary>
        /// Get or set the background of the control
        /// </summary>
        Brush Background
        {
            get;
            set;
        }
        #endregion
    }

    /// <summary>
    /// Static class of 'helper' functions to implement common mapping functions
    /// </summary>
    public static class Mapping
    {
        /// <summary>
        /// MessageEventController used to send messages to the GUI from the mapping controls
        /// </summary>
        public static MessageEventController MappingMessenger { get; set; }

        /// <summary>
        /// Determines whether the specified object is a mapping control
        /// </summary>
        /// <param name="o">An object to test</param>
        /// <returns>True if the object is an IMappingControl</returns>
        public static bool IsControl(Object o)
        {
            try
            {
                System.Type t = o.GetType();
                if ((t == typeof(MappingCanvasControl)) ||
                    (t == typeof(MappingClusterControl)) ||
                    (t == typeof(MappingFPGAControl)) ||
                    (t == typeof(MappingConnectionControl)) ||
                    (t == typeof(MappingComponentControl)) ||
                    (t == typeof(MappingLinkControl)) ||
                    (t == typeof(MappingGroupControl)))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
            return false;
        }
        /// <summary>
        /// Determines whether a control is a mapping control capable of supporting children
        /// </summary>
        /// <param name="o">An object to test</param>
        /// <returns>True if the control is an IMappingControl capable of supporting children</returns>
        public static bool IsContainerControl(Object o)
        {
            try
            {
                System.Type t = o.GetType();
                if ((t == typeof(MappingCanvasControl)) ||
                    (t == typeof(MappingClusterControl)) ||
                    (t == typeof(MappingFPGAControl)) ||
                    (t == typeof(MappingGroupControl)))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
            return false;
        }
        /// <summary>
        /// Determines whether objects of type X is capable of containing children of type Y
        /// </summary>
        /// <param name="X">The container object to test</param>
        /// <param name="Y">The child object to test</param>
        /// <returns>True if X objects may contain Y objects; false otherwise</returns>
        public static bool CanXContainY(Object X, Object Y)
        {
            try
            {
                System.Type xType = X.GetType();
                System.Type yType = Y.GetType();
                if ((!IsControl(X)) || (!IsControl(Y)))
                    return false;

                System.Diagnostics.Debug.WriteLine(String.Format("Testing whether {0} can contain {1}", ((IMappingControl)X).MappingID, ((IMappingControl)Y).MappingID));
                // Connections can't contain anything
                if (xType == typeof(MappingConnectionControl))
                    return false;
                // Links can't contain anything
                if (xType == typeof(MappingLinkControl))
                    return false;
                // Components can't contain anything
                if (xType == typeof(MappingComponentControl))
                    return false;

                // FPGAs can only contain groups (and on the GUI, Components)
                if (xType == typeof(MappingFPGAControl))
                {
                    if ((yType != typeof(MappingGroupControl)) &&
                        (yType != typeof(MappingComponentControl)))
                        return false;
                }
                // Groups can only contain components
                if (xType == typeof(MappingGroupControl))
                {
                    if (yType != typeof(MappingComponentControl))
                        return false;
                }
                // Clusters can only contain FPGAs
                if (xType == typeof(MappingClusterControl))
                {
                    if (yType != typeof(MappingFPGAControl))
                        return false;
                }

                // Components on the canvas have to be handled depending on where they are dropped
                // if (xType == typeof(MappingCanvasControl))
                //  return true;

                // Other combinations are allowed
                return true;
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
            return false;
        }
        /// <summary>
        /// Aligns all children of the specified IMappingControl based on its alignment parameters
        /// </summary>
        /// <param name="c">The IMappingControl to align</param>
        /// <param name="columns">The number of columns to align children in</param>
        /// <param name="border">The internal border width for alignment</param>
        public static void AlignChildren(IMappingControl c, int columns, double border)
        {
            try
            {
                if (c.GetType() == typeof(MappingCanvasControl))
                {
                    MappingCanvasControl mcc = (MappingCanvasControl)c;
                    AlignMappingCanvas(mcc, 6, 10.0F);
                }
                else
                {
                    if (c.Alignable)
                        AlignCanvas(c, c.AlignmentColumns, c.AlignmentSpacing);
                }
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
        }
        private static void AlignCanvas(IMappingControl imapCtrl, int columns, double border)
        {
            try
            {
                if (!imapCtrl.Alignable)
                    return;

                Canvas canvas = imapCtrl.ObjectCanvas;
                int count = canvas.Children.Count;

                int rows = (int)Math.Ceiling((double)count / (double)columns);
                double h = ((imapCtrl.Bottom - imapCtrl.Top) - ((rows + 1) * border)) / rows;
                double w = ((imapCtrl.Right - imapCtrl.Left) - ((columns + 1) * border)) / columns;

                int row; int col;
                row = 0;
                col = 0;
                foreach (object o in canvas.Children)
                {
                    if (o is IMappingControl)
                    {
                        IMappingControl imap = (IMappingControl)o;
                        if (imap.Alignable)
                        {
                            imap.Top = border + (row * (border + h));
                            imap.Left = border + (col * (border + w));
                            if ((!(imap is ICollapsible)) || (((imap is ICollapsible) && (!((ICollapsible)imap).Collapsed))))
                            {
                                imap.Height = h;
                                imap.Width = w;
                            }
                            else
                            {
                                ICollapsible icol = (ICollapsible)imap;

                                // Center the collapsed box, so that it expands around its location
                                // Calculate where its center should be 
                                double ctrX;
                                double ctrY;
                                ctrX = imap.Left + (w / 2);
                                ctrY = imap.Top + (h / 2);
                                Point ctr = new Point(ctrX, ctrY);

                                // Calculate its collapsed size in the new alignment and set it
                                imap.Height = h * icol.CurrentYScale;
                                imap.Width = w * icol.CurrentXScale;

                                // Place it
                                SetCenter(imap, ctr);
                            }
                            imap.AlignChildren();
                        }
                        col++;
                        if (col == columns)
                        {
                            row++;
                            col = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
        }
        private static void AlignMappingCanvas(MappingCanvasControl canvas, int columns, double border)
        {
            try
            {
                int mapCount = canvas.mapped.Children.Count;
                int unmapCount = canvas.unmapped.Children.Count;

                int mapRows = (int)Math.Ceiling((double)mapCount / (double)columns);
                int unmapRows = (int)Math.Ceiling((double)unmapCount / (double)columns);

                double mapH = (canvas.mapped.Height - ((mapRows + 1) * border)) / mapRows;
                double mapW = (canvas.mapped.Width - ((columns + 1) * border)) / columns;
                double unmapH = (canvas.unmapped.Height - ((unmapRows + 1) * border)) / unmapRows;
                double unmapW = (canvas.unmapped.Width - ((columns + 1) * border)) / columns;

                int row; int col;
                row = 0;
                col = 0;
                foreach (object o in canvas.mapped.Children)
                {
                    if (o is IMappingControl)
                    {
                        IMappingControl imap = (IMappingControl)o;
                        if (imap.Alignable)
                        {
                            imap.Top = border + (row * (border + mapH));
                            imap.Left = border + (col * (border + mapW));
                            imap.Height = mapH;
                            imap.Width = mapW;
                            imap.AlignChildren();
                        }
                        col++;
                        if (col == columns)
                        {
                            row++;
                            col = 0;
                        }
                    }
                }

                row = 0;
                col = 0;
                foreach (object o in canvas.unmapped.Children)
                {
                    if (o is IMappingControl)
                    {
                        IMappingControl imap = (IMappingControl)o;
                        if (imap.Alignable)
                        {
                            imap.Top = border + (row * (border + unmapH));
                            imap.Left = border + (col * (border + unmapW));
                            imap.Height = unmapH;
                            imap.Width = unmapW;
                            imap.AlignChildren();
                        }
                        col++;
                        if (col == columns)
                        {
                            row++;
                            col = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
        }

        /// <summary>
        /// Gets the coordinates of the top-left corner of the specified control within the global coordinate space
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <returns>A Point indicating the top-left corner of the control</returns>
        public static Point GetGlobalTopLeft(IMappingControl ctrl)
        {
            try
            {
                IMappingControl relative = GetCollapsedAncestor(ctrl);
                double myTop = relative.Top;
                double myLeft = relative.Left;

                IMappingControl nextUp = relative.AttachedParent;
                IMappingControl lastUp = relative;
                while (nextUp != null)
                {
                    if (nextUp == relative.ParentCanvas)
                    {
                        MappingCanvasControl mcc = (MappingCanvasControl)(nextUp);
                        Canvas OwnerCanvas = null;
                        if (mcc.unmapped.Children.Contains((UIElement)lastUp))
                        {
                            OwnerCanvas = mcc.unmapped;
                        }
                        else if (mcc.mapped.Children.Contains((UIElement)lastUp))
                        {
                            OwnerCanvas = mcc.mapped;
                        }
                        else if (mcc.drawingBoard.Children.Contains((UIElement)lastUp))
                        {
                            OwnerCanvas = mcc.drawingBoard;
                        }
                        if (OwnerCanvas != null)
                        {
                            myTop += (double)OwnerCanvas.GetValue(Canvas.TopProperty);
                            myLeft += (double)OwnerCanvas.GetValue(Canvas.LeftProperty);
                        }
                        break;
                    }
                    else
                    {
                        myTop += nextUp.Top;
                        myLeft += nextUp.Left;
                        lastUp = nextUp;
                        nextUp = nextUp.AttachedParent;
                    }
                }
                return new Point(myLeft, myTop);
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
            return new Point(0, 0);
        }
        /// <summary>
        /// Gets the coordinates of the center of the specified control within the global coordinate space
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <returns>A Point indicating the center of the control</returns>
        public static Point GetGlobalCenter(IMappingControl ctrl)
        {
            try
            {
                IMappingControl relative = GetCollapsedAncestor(ctrl);
                Point TopLeft = GetGlobalTopLeft(relative);
                Point myCenter = new Point(TopLeft.X + (relative.Width / 2), TopLeft.Y + (relative.Height / 2));
                return myCenter;
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
            return new Point(0, 0);
        }

        /// <summary>
        /// Determines whether ancestor of the specified control is collapsed.
        /// </summary>
        /// <param name="imap">The control to test</param>
        /// <returns>True if any ancestor of the specified control is collapsed; false otherwise</returns>
        public static bool IsAncestorCollapsed(IMappingControl imap)
        {
            try
            {
                if (imap.AttachedParent == null)
                    return false;

                if (imap.GetType() == typeof(MappingCanvasControl))
                    return false;

                if (!(imap.AttachedParent is ICollapsible))
                    return IsAncestorCollapsed(imap.AttachedParent);

                if (((ICollapsible)imap.AttachedParent).Collapsed)
                    return true;
                else
                    return IsAncestorCollapsed(imap.AttachedParent);
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
            return false;
        }
        /// <summary>
        /// Gets an ancestor of the specified control that is collapsed, if one exists
        /// </summary>
        /// <param name="imap">The control to test</param>
        /// <returns>An ancestor of the specified control that is collapsed, if one exists</returns>
        public static IMappingControl GetCollapsedAncestor(IMappingControl imap)
        {
            try
            {
                if (!IsAncestorCollapsed(imap))
                    return imap;

                if (imap.GetType() == typeof(MappingCanvasControl))
                    return imap;

                if (!(imap.AttachedParent is ICollapsible))
                {
                    IMappingControl ancestor = GetCollapsedAncestor(imap.AttachedParent);
                    if (ancestor == imap.AttachedParent)
                        return imap;
                    else
                        return ancestor;
                }

                if (((ICollapsible)imap.AttachedParent).Collapsed)
                {
                    return imap.AttachedParent;
                }
                else
                {
                    IMappingControl ancestor = GetCollapsedAncestor(imap.AttachedParent);
                    if (ancestor == imap.AttachedParent)
                        return imap;
                    else
                        return ancestor;
                }
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
            return null;
        }
        /// <summary>
        /// Gets an ancestor of the specified control that is collapsible, if one exists
        /// </summary>
        /// <param name="imap">The control to test</param>
        /// <returns>An ancestor of the specified control that is collapsible, if one exists</returns>
        public static ICollapsible GetCollapsibleAncestor(IMappingControl imap)
        {
            try
            {
                if (imap.GetType() == typeof(MappingCanvasControl))
                    return null;

                if (imap is ICollapsible)
                    return (ICollapsible)imap;
                else
                    return GetCollapsibleAncestor(imap.AttachedParent);
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
            return null;
        }
        /// <summary>
        /// Gets the nearest ancestor of the specified control that is capable of supporting the source control
        /// </summary>
        /// <param name="source">The control to test</param>
        /// <param name="testContainer">The container to test</param>
        /// <returns>The nearest ancestor of the specified control that is capable of supporting the source control, if one exists</returns>
        public static IMappingControl GetEligibleContainer(IMappingControl source, IMappingControl testContainer)
        {
            try
            {
                if (testContainer.GetType() == typeof(MappingCanvasControl))
                    return testContainer;

                if (Mapping.CanXContainY(testContainer, source))
                    return testContainer;
                else
                    return GetEligibleContainer(source, testContainer.AttachedParent);
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
            if (source != null)
            {
                return source.ParentCanvas;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Renders text to the specified canvas in the colors indicated.
        /// </summary>
        /// <param name="c">The canvas on which to render the text</param>
        /// <param name="BGColor">The background color of the canvas</param>
        /// <param name="TextColor">The foreground color of the text</param>
        /// <param name="Text">The text string to render</param>
        public static void RenderTextToCanvas(Canvas c, Brush BGColor, Brush TextColor, string Text)
        {
            try
            {
                ImageBrush bgBrush;
                DrawingGroup dg = new DrawingGroup();
                DrawingContext dc = dg.Open();
                dc.DrawText(
                        new FormattedText(Text,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"),
                        48, TextColor),
                        new Point(10, 10)
                    );
                dc.Close();
                DrawingImage img = new DrawingImage(dg);
                bgBrush = new ImageBrush(img);
                bgBrush.Stretch = Stretch.Uniform;

                double w = img.Width + 15;
                double h = img.Height + 15;

                if (w < c.Width)
                    w = c.Width;
                if (h < c.Height)
                    h = c.Height;

                dg = new DrawingGroup();
                dc = dg.Open();
                dc.DrawRectangle(BGColor, new Pen(), new Rect(0, 0, w, h));
                dc.DrawText(
                        new FormattedText(Text,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Segoe UI"),
                        48, TextColor),
                        new Point(0, 0)
                    );
                dc.Close();
                img = new DrawingImage(dg);
                bgBrush = new ImageBrush(img);
                bgBrush.Stretch = Stretch.Uniform;
                c.Background = bgBrush;
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
        }

        #region IMappingControl Implementations

        /// <summary>
        /// Get or set a flag indicating whether the GUI elements are being populated from the mapping library.
        /// While set to true, GUI changes are not propagated to the library state.  This should ONLY
        /// be set to True while reloading the GUI from the library state.
        /// </summary>
        public static bool Populating { get; set; }

        #region Get Size and Location
        /// <summary>
        /// Gets the top-coordinate of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <returns>The top-coordinate of the control, relative to it's parent</returns>
        public static double GetTop(IMappingControl ctrl)
        {
            return (double)ctrl.GetValue(Canvas.TopProperty);
        }
        /// <summary>
        /// Gets the left-coordinate of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <returns>The left-coordinate of the control, relative to it's parent</returns>
        public static double GetLeft(IMappingControl ctrl)
        {
            return (double)ctrl.GetValue(Canvas.LeftProperty);
        }
        /// <summary>
        /// Gets the right-coordinate of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <returns>The right-coordinate of the control, relative to it's parent</returns>
        public static double GetRight(IMappingControl ctrl)
        {
            return (double)ctrl.GetValue(Canvas.RightProperty);
        }
        /// <summary>
        /// Gets the bottom-coordinate of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <returns>The bottom-coordinate of the control, relative to it's parent</returns>
        public static double GetBottom(IMappingControl ctrl)
        {
            return (double)ctrl.GetValue(Canvas.BottomProperty);
        }
        /// <summary>
        /// Gets the width of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <returns>The width of the control, relative to it's parent</returns>
        public static double GetWidth(IMappingControl ctrl)
        {
            return (double)ctrl.GetValue(Canvas.WidthProperty);
        }
        /// <summary>
        /// Gets the height of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <returns>The height of the control, relative to it's parent</returns>
        public static double GetHeight(IMappingControl ctrl)
        {
            return (double)ctrl.GetValue(Canvas.HeightProperty);
        }
        /// <summary>
        /// Gets the center coordinates of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <returns>The center coordinates of the control, relative to it's parent</returns>
        public static Point GetCenter(IMappingControl ctrl)
        {
            return new Point(
                GetLeft(ctrl) + (GetWidth(ctrl) / 2),
                GetTop(ctrl) + (GetHeight(ctrl) / 2));
        }
        #endregion

        #region Set Size and Location
        /// <summary>
        /// Sets the top-coordinate of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <param name="value">The new value of the coordinate</param>
        public static void SetTop(IMappingControl ctrl, double value)
        {
            if (double.IsNaN(value)) return;
            if (double.IsInfinity(value)) return;
            if (value < 0) return;
            ctrl.SetValue(Canvas.TopProperty, value);
            double newBottom = GetHeight(ctrl) + value;
            if (GetBottom(ctrl) != newBottom)
                SetBottom(ctrl, newBottom);
        }
        /// <summary>
        /// Sets the left-coordinate of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <param name="value">The new value of the coordinate</param>
        public static void SetLeft(IMappingControl ctrl, double value)
        {
            if (double.IsNaN(value)) return;
            if (double.IsInfinity(value)) return;
            if (value < 0) return;
            ctrl.SetValue(Canvas.LeftProperty, value);
            double newRight = GetWidth(ctrl) + value;
            if (GetRight(ctrl) != newRight)
                SetRight(ctrl, newRight);
        }
        /// <summary>
        /// Sets the right-coordinate of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <param name="value">The new value of the coordinate</param>
        public static void SetRight(IMappingControl ctrl, double value)
        {
            if (double.IsNaN(value)) return;
            if (double.IsInfinity(value)) return;
            if (value < 0) return;
            ctrl.SetValue(Canvas.RightProperty, value);
            double newWidth = value - GetLeft(ctrl);
            if (GetWidth(ctrl) != newWidth)
                SetWidth(ctrl, newWidth);
        }
        /// <summary>
        /// Sets the bottom-coordinate of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <param name="value">The new value of the coordinate</param>
        public static void SetBottom(IMappingControl ctrl, double value)
        {
            if (double.IsNaN(value)) return;
            if (double.IsInfinity(value)) return;
            if (value < 0) return;
            ctrl.SetValue(Canvas.BottomProperty, value);
            double newHeight = value - GetTop(ctrl);
            if (GetHeight(ctrl) != newHeight)
                SetHeight(ctrl, newHeight);
        }
        /// <summary>
        /// Sets the width of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <param name="value">The new value of the width</param>
        public static void SetWidth(IMappingControl ctrl, double value)
        {
            if (double.IsNaN(value)) return;
            if (double.IsInfinity(value)) return;
            if (value < 0) return;
            ctrl.SetValue(Canvas.WidthProperty, value);
            double newRight = value + GetLeft(ctrl);
            if (GetRight(ctrl) != newRight)
                SetRight(ctrl, newRight);
        }
        /// <summary>
        /// Sets the height of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <param name="value">The new value of the height</param>
        public static void SetHeight(IMappingControl ctrl, double value)
        {
            if (double.IsNaN(value)) return;
            if (double.IsInfinity(value)) return;
            if (value < 0) return;
            ctrl.SetValue(Canvas.HeightProperty, value);
            double newBottom = value + GetTop(ctrl);
            if (GetBottom(ctrl) != newBottom)
                SetBottom(ctrl, newBottom);
        }

        /// <summary>
        /// Sets the center coordinates of the control, relative to it's parent
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <param name="p">The new center point of the control</param>
        public static void SetCenter(IMappingControl ctrl, Point p)
        {
            SetCenter(ctrl, p, GetWidth(ctrl), GetHeight(ctrl));
        }
        /// <summary>
        /// Sets the center coordinates of the control, relative to it's parent, and concurrently resizes the control
        /// </summary>
        /// <param name="ctrl">The control to locate</param>
        /// <param name="p">The new center point of the control</param>
        /// <param name="NewWidth">The new width of the control</param>
        /// <param name="NewHeight">The new height of the control</param>
        public static void SetCenter(IMappingControl ctrl, Point p, double NewWidth, double NewHeight)
        {
            Point TL = new Point(
                p.X - (NewWidth / 2),
                p.Y - (NewHeight / 2));
            SetLeft(ctrl, TL.X);
            SetTop(ctrl, TL.Y);
            SetWidth(ctrl, NewWidth);
            SetHeight(ctrl, NewHeight);
        }
        #endregion

        /// <summary>
        /// Get a list of all child controls of the specified canvas
        /// </summary>
        /// <param name="canv">The canvas whose children to retrieve</param>
        /// <returns>A List of all child controls of the specified canvas</returns>
        public static List<IMappingControl> GetChildren(Canvas canv)
        {
            try
            {
                List<IMappingControl> childList = new List<IMappingControl>();
                if (canv != null)
                {
                    foreach (object o in canv.Children)
                    {
                        if (Mapping.IsControl(o))
                        {
                            childList.Add((IMappingControl)o);
                        }
                    }
                }
                return childList;
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
            return new List<IMappingControl>();
        }
        /// <summary>
        /// Gets the IMapping control located beneath the mouse, relative to the specified control
        /// </summary>
        /// <param name="ctrl">The control with which location is determined</param>
        /// <param name="mouse">The mouse device used to locate the control</param>
        /// <returns>An IMappingControl object located at the mouse, if one was found; null otherwise.</returns>
        public static IMappingControl GetObjectAtMouse(IMappingControl ctrl, MouseDevice mouse)
        {
            try 
            {
                Point p = mouse.GetPosition((UIElement)ctrl);
                double x = p.X;
                double y = p.Y;

                IMappingControl found = null;
                if (((x >= 0) && (x <= ctrl.Width)) &&
                   ((y >= 0) && (y <= ctrl.Height)))
                {
                    found = ctrl;
                    if (ctrl is ICollapsible)
                        if (((ICollapsible)ctrl).Collapsed)
                            return found;
                    foreach (object o in ctrl.ObjectCanvas.Children)
                    {
                        if (o is IMappingControl)
                        {
                            IMappingControl childFound = ((IMappingControl)o).GetObjectAtMouse(mouse);
                            if (childFound != null)
                            {
                                found = childFound;
                                break;
                            }
                        }
                    }
                }
                return found;
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
            if (ctrl != null)
            {
                return ctrl.ParentCanvas;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Attaches the first control to the second as a child
        /// </summary>
        /// <param name="A">The child control</param>
        /// <param name="B">The parent control</param>
        public static void AttachAToB(IMappingControl A, IMappingControl B)
        {
            try
            {
                if (A.GetType() == typeof(MappingComponentControl))
                {
                    #region Attach Component to Canvas / FPGA / Group
                    MappingComponentControl component = (MappingComponentControl)A;
                    if (B.GetType() == typeof(MappingCanvasControl))
                    {
                        MappingCanvasControl canvas = (MappingCanvasControl)B;

                        // Remove the component from its previous group
                        component.DetachFromParent();
                        if (!Populating)
                        {
                            component.ParentCanvas.MapObjects.RemoveComponentFromGroup(component.MappingID);
                        }

                        // Place it in the unmapped bin
                        canvas.unmapped.Children.Add((UIElement)component);
                        component.BackgroundBrush = canvas.UnmappedColor;
                        component.AttachedParent = canvas;
                        component.RecentParent = component.AttachedParent;
                    }
                    else if (B.GetType() == typeof(MappingFPGAControl))
                    {
                        MappingFPGAControl fpga = (MappingFPGAControl)B;

                        // Remove the component from its previous group
                        component.DetachFromParent();
                        component.ParentCanvas.MapObjects.RemoveComponentFromGroup(component.MappingID);

                        // Create a new group to house the component
                        string newID = component.ParentCanvas.MapObjects.CreateNewGroupForComponent(component.MappingID);
                        MappingGroupControl newGroup = component.ParentCanvas.CreateGroupControl(newID, newID, component.ParentCanvas);

                        // Add the component to the new group
                        AttachAToB(component, newGroup);

                        // Add the group to the FPGA
                        AttachAToB(newGroup, fpga);

                        // Was it successful?
                        if (!(newGroup.ParentCanvas.MapObjects.GetGroupTargetFPGAID(newGroup.MappingID) == fpga.MappingID))
                        {
                            AttachAToB(newGroup, component.ParentCanvas);
                        }
                    }
                    else if (B.GetType() == typeof(MappingGroupControl))
                    {
                        MappingGroupControl group = (MappingGroupControl)B;

                        // Remove the component from its previous group
                        component.DetachFromParent();
                        if (!Populating)
                        {
                            component.ParentCanvas.MapObjects.RemoveComponentFromGroup(component.MappingID);

                            // Add it to its new group
                            component.ParentCanvas.MapObjects.AddComponentToGroup(component.MappingID, group.MappingID);
                        }

                        // Was it successful?
                        if (component.ParentCanvas.MapObjects.GetComponentGroupID(component.MappingID) == group.MappingID)
                        {
                            group.ObjectCanvas.Children.Add((UIElement)component);
                            component.BackgroundBrush = group.BackgroundBrush;
                            component.AttachedParent = group;
                            component.RecentParent = component.AttachedParent;
                        }
                        else
                        {
                            component.ReAttachToParent();
                        }
                    }
                    else
                    {
                        return;
                    }
                    A.ParentCanvas.StateChanged();
                    if (IsAncestorCollapsed(component))
                        component.Hide();
                    else
                        component.Show();
                    #endregion
                }
                else if (A.GetType() == typeof(MappingGroupControl))
                {
                    #region Attach Group to Canvas / FPGA
                    MappingGroupControl group = (MappingGroupControl)A;
                    if (B.GetType() == typeof(MappingCanvasControl))
                    {
                        MappingCanvasControl canvas = (MappingCanvasControl)B;

                        // Remove group from its previous mapping, place it in the unmapped bin
                        group.DetachFromParent();

                        group.ParentCanvas.MapObjects.UnMapGroup(group.MappingID);
                        canvas.unmapped.Children.Add((UIElement)group);

                        group.AttachedParent = canvas;
                        group.RecentParent = group.AttachedParent;
                    }
                    else if (B.GetType() == typeof(MappingFPGAControl))
                    {
                        MappingFPGAControl fpga = (MappingFPGAControl)B;

                        // Remove group from its previous mapping
                        group.DetachFromParent();
                        if (!Populating)
                        {
                            group.ParentCanvas.MapObjects.UnMapGroup(group.MappingID);
                        }

                        // Map it to it's new FPGA
                        fpga.ObjectCanvas.Children.Add((UIElement)group);

                        if (!Populating)
                        {
                            group.ParentCanvas.MapObjects.MapGroupToFPGA(group.MappingID, fpga.MappingID);
                        }

                        // Was it successful?
                        if (group.ParentCanvas.MapObjects.GetGroupTargetFPGAID(group.MappingID) == fpga.MappingID)
                        {
                            group.AttachedParent = fpga;
                            group.RecentParent = group.AttachedParent;
                        }
                        else
                        {
                            group.ReAttachToParent();
                        }
                    }
                    else
                    {
                        return;
                    }
                    A.ParentCanvas.StateChanged();
                    if (IsAncestorCollapsed(group))
                        group.Hide();
                    else
                        group.Show();
                    #endregion
                }
                else if (A.GetType() == typeof(MappingFPGAControl))
                {
                    #region Attach FPGA to Canvas / Cluster
                    MappingFPGAControl fpga = (MappingFPGAControl)A;
                    if (B.GetType() == typeof(MappingCanvasControl))
                    {
                        MappingCanvasControl canvas = (MappingCanvasControl)B;

                        // Remove group from its previous cluster
                        fpga.DetachFromParent();
                        if (!Populating)
                        {
                            fpga.ParentCanvas.MapObjects.RemoveFPGAFromCluster(fpga.MappingID);
                        }

                        // Drop it in the mapped bin
                        canvas.mapped.Children.Add((UIElement)fpga);
                        fpga.AttachedParent = canvas;
                        fpga.RecentParent = fpga.AttachedParent;
                    }
                    else if (B.GetType() == typeof(MappingClusterControl))
                    {
                        MappingClusterControl cluster = (MappingClusterControl)B;

                        // Remove group from its previous cluster
                        fpga.DetachFromParent();
                        if (!Populating)
                        {
                            fpga.ParentCanvas.MapObjects.RemoveFPGAFromCluster(fpga.MappingID);
                        }
                        cluster.ObjectCanvas.Children.Add((UIElement)fpga);

                        // Put it in it's new cluster
                        fpga.AttachedParent = cluster;
                        if (!Populating)
                        {
                            fpga.ParentCanvas.MapObjects.AddFPGAToCluster(fpga.MappingID, cluster.MappingID);
                        }
                        fpga.RecentParent = fpga.AttachedParent;
                    }
                    else
                    {
                        return;
                    }
                    A.ParentCanvas.StateChanged();
                    if (IsAncestorCollapsed(fpga))
                        fpga.Hide();
                    else
                        fpga.Show();
                    #endregion
                }
                else if (A.GetType() == typeof(MappingClusterControl))
                {
                    #region Attach Cluster to Canvas
                    MappingClusterControl cluster = (MappingClusterControl)A;
                    if (B.GetType() == typeof(MappingCanvasControl))
                    {
                        // Clusters aren't added and removed like other containers.  They either exist, or they don't
                        // They have no direct impact on the mapping algorithm
                        MappingCanvasControl canvas = (MappingCanvasControl)B;

                        if (canvas == cluster.AttachedParent)  // This should always be the case, except when the cluster is first created in the GUI system
                            return;

                        cluster.DetachFromParent();
                        canvas.mapped.Children.Add((UIElement)cluster);
                        cluster.AttachedParent = canvas;
                        cluster.RecentParent = cluster.AttachedParent;
                    }
                    else
                    {
                        return;
                    }
                    A.ParentCanvas.StateChanged();
                    if (IsAncestorCollapsed(cluster))
                        cluster.Hide();
                    else
                        cluster.Show();
                    #endregion
                }
                else if (A.GetType() == typeof(MappingConnectionControl))
                {
                    #region Attach Connection to Canvas
                    MappingConnectionControl connection = (MappingConnectionControl)A;
                    if (B.GetType() == typeof(MappingCanvasControl))
                    {
                        MappingCanvasControl canvas = (MappingCanvasControl)B;

                        if (canvas == connection.AttachedParent)  // This should always be the case, except when the connection is first created in the GUI system
                            return;

                        connection.DetachFromParent();
                        canvas.drawingBoard.Children.Add((UIElement)connection);
                        connection.AttachedParent = canvas;
                        connection.RecentParent = connection.AttachedParent;
                    }
                    else
                    {
                        return;
                    }
                    A.ParentCanvas.StateChanged();
                    // Connections are shown by the Canvas when RedrawInterconnects is Called
                    #endregion
                }
                else if (A.GetType() == typeof(MappingLinkControl))
                {
                    #region Attach Link to Canvas
                    MappingLinkControl link = (MappingLinkControl)A;
                    if (B.GetType() == typeof(MappingCanvasControl))
                    {
                        MappingCanvasControl canvas = (MappingCanvasControl)B;
                        link.DetachFromParent();

                        if (canvas == link.AttachedParent)  // This should always be the case, except when the connection is first created in the GUI system
                            return;

                        canvas.drawingBoard.Children.Add((UIElement)link);
                        link.AttachedParent = canvas;
                        link.RecentParent = link.AttachedParent;
                    }
                    else
                    {
                        return;
                    }
                    A.ParentCanvas.StateChanged();
                    // Links are shown by the Canvas when RedrawInterconnects is Called
                    #endregion
                }
                else if (A.GetType() == typeof(MappingCanvasControl))
                {
                    #region Attach Canvas to itself
                    // MappingCanvasControl cannot be attached to anything but itself
                    A.AttachedParent = A;
                    A.RecentParent = A;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
        }
        /// <summary>
        /// Detatches the specified control from its current parent.
        /// </summary>
        /// <param name="orphan">The control to remove from its parent</param>
        public static void DetachFromParent(IMappingControl orphan)
        {
            try
            {
                if (orphan.AttachedParent != null)
                {
                    if (orphan.AttachedParent.GetType() == typeof(MappingCanvasControl))
                    {
                        MappingCanvasControl oldCanvas = (MappingCanvasControl)orphan.AttachedParent;
                        if (oldCanvas.mapped.Children.Contains((UIElement)orphan)) oldCanvas.mapped.Children.Remove((UIElement)orphan);
                        if (oldCanvas.unmapped.Children.Contains((UIElement)orphan)) oldCanvas.unmapped.Children.Remove((UIElement)orphan);
                        if (oldCanvas.drawingBoard.Children.Contains((UIElement)orphan)) oldCanvas.drawingBoard.Children.Remove((UIElement)orphan);
                        orphan.AttachedParent = null;
                    }
                    else if (orphan.AttachedParent is IMappingControl)
                    {
                        IMappingControl imap = (IMappingControl)orphan.AttachedParent;
                        imap.ObjectCanvas.Children.Remove((UIElement)orphan);
                        orphan.AttachedParent = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
        }
        /// <summary>
        /// Reattaches the specified control to its most recent parent
        /// </summary>
        /// <param name="orphan">The control to reattach</param>
        public static void ReAttachToParent(IMappingControl orphan)
        {
            try
            {
                orphan.AttachToParent(orphan.RecentParent);
            }
            catch (Exception ex)
            {
                MappingMessenger.RaiseMessageEvent(MessageEventType.Error, string.Empty, ErrorReporting.ExceptionDetails(ex), "Mapping GUI Subsystem");
            }
        }

        #endregion
    }
}
