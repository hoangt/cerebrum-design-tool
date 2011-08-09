using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace Netron.Diagramming.Core
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// This tool implements the action of moving shapes on the canvas. 
    /// <para>Note that this tool is slightly different than other tools 
    /// since it activates itself unless it has been suspended by another 
    /// tool. </para>
    /// </summary>
    public class DragDropTool : 
        AbstractTool, 
        IMouseListener, 
        IDragDropListener
    {

        #region Fields
        Cursor feedbackCursor;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:DragDropTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public DragDropTool(string name) : base(name)
        { 
        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected override void OnActivateTool()
        {
            Controller.View.CurrentCursor = Cursors.SizeAll;
        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public bool MouseDown(MouseEventArgs e)
        {
            return false; //continue spreading the events
        }

        /// <summary>
        /// Handles the mouse move event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseMove(MouseEventArgs e)
        {
           
        }
        /// <summary>
        /// Handles the mouse up event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseUp(MouseEventArgs e)
        {
            
        }
        #endregion

        /// <summary>
        /// On dragdrop.
        /// </summary>
        /// <param name="e">The <see cref="T:KeyEventArgs"/> instance containing the event data.</param>
        public bool OnDragDrop(DragEventArgs e)
        {
            Control control = (Control)this.Controller.ParentControl;
            Point p = control.PointToClient(new Point(e.X, e.Y));
            IShape shape = null;
            IDataObject iDataObject = e.Data;
            string text;

            if (iDataObject.GetDataPresent("IShape"))
            {
                shape = (IShape)iDataObject.GetData("IShape");
                shape.Model = Controller.Model;
            }
            else if (iDataObject.GetDataPresent(typeof(string)))
            {
                text = iDataObject.GetData(
                        typeof(string)).ToString();

                //foreach (string shapeType in Enum.GetNames(typeof(ShapeTypes)))
                //{
                //    if (shapeType.ToString().ToLower() == text.ToLower())
                //    {
                //        shape = ShapeFactory.GetShape(shapeType);
                //        break;
                //    }
                //}

                // If our shape is still null, then the text being dragged
                // onto the canvas is not the name of a shape, so add a
                // TextOnly shape with the text.
                //if (shape == null)
                //{
                //    shape = new TextOnly(Controller.Model);
                //    (shape as TextOnly).Text = text;
                //}

                shape = new TextOnly(Controller.Model);
                (shape as TextOnly).Text = text;
            }
            else if (iDataObject.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    string[] files = (string[])iDataObject.GetData(
                        DataFormats.FileDrop);

                    foreach (string fileName in files)
                    {
                        FileInfo info = new FileInfo(fileName);

                        if (info.Exists)
                        {
                            FileShape fileShape = new FileShape(fileName);
                            AddShapeCommand cmd = new AddShapeCommand(
                                this.Controller,
                                fileShape,
                                p);
                            this.Controller.UndoManager.AddUndoCommand(cmd);
                            cmd.Redo();
                            p.Offset(20, 20);
                        }
                    }

                    feedbackCursor = null;
                }
                catch
                {                    
                }
            }
            else if (iDataObject.GetDataPresent(typeof(Bitmap)))
            {                
                // Doesn't support dropping images yet - having problems
                // getting images off the clipboard.
                shape = new ImageShape(Controller.Model);
                (shape as ImageShape).Image = 
                    (Image)iDataObject.GetData(typeof(Bitmap));
                return true;
            }

            if(shape != null)
            {
                // Matthew Cotter: THIS IS WHERE THE DROPPED OBJECT IS PLACED
                Point CursorLoc = System.Windows.Forms.Cursor.Position;
                Point DnDLoc = new Point(e.X, e.Y);
                p = control.PointToClient(DnDLoc);

                // Factor in magnification offset
                SizeF mag = new SizeF(this.Controller.Model.CurrentPage.Magnification.Width / 100, this.Controller.Model.CurrentPage.Magnification.Height / 100);
                SizeF mult = new SizeF((1 / mag.Width) / 2, (1 / mag.Height) / 2);
                p = new Point((int)((p.X) * mult.Width), (int)((p.Y) * mult.Height));



                //ComplexRectangle shape = new ComplexRectangle();
                //shape.Rectangle = new Rectangle(p.X, p.Y, 150, 70);
                //shape.Text = "Just an example, work in progress.";

                //TextLabel shape = new TextLabel();
                //shape.Rectangle = new Rectangle(p.X, p.Y, 150, 70);
                //shape.Text = "Just an example, work in progress.";

                AddShapeCommand cmd = new AddShapeCommand(
                    this.Controller, 
                    shape, 
                    p);
                this.Controller.UndoManager.AddUndoCommand(cmd);
                cmd.Redo();
                feedbackCursor = null;

                // Once added, shift by the appropriate amount
                shape.MoveBy(p);
                // Then shift again by 50 to move shape under the mouse
                shape.MoveBy(new Point(50, 50));
                return true;
            }

            // The drop event wasn't handled here.
            return false;
        }

        /// <summary>
        /// On drag leave.
        /// </summary>
        /// <param name="e">The <see cref="T:KeyEventArgs"/> instance containing the event data.</param>
        public bool OnDragLeave(EventArgs e)
        {
            return false;
        }

        /// <summary>
        /// On drag over.
        /// </summary>
        /// <param name="e">The <see cref="T:KeyPressEventArgs"/> instance containing the event data.</param>
        public bool OnDragOver(DragEventArgs e)
        {
            return AnalyzeData(e);
        }


        /// <summary>
        /// On drag enter
        /// </summary>
        /// <param name="e"></param>
        public bool OnDragEnter(DragEventArgs e)
        {
            return AnalyzeData(e);
        }

        private bool AnalyzeData(DragEventArgs e)
        {
            IDataObject iDataObject = e.Data;

            if (iDataObject.GetDataPresent("IShape"))
            {
                feedbackCursor = CursorPalette.DropShape;
                e.Effect = DragDropEffects.Copy;
                return true;
            }
            
            if(iDataObject.GetDataPresent(typeof(string)))
            {
                // Need to determine if the text on the clipboard is the
                // name of a shape or if it's just plain old text to add.
                //foreach(string shapeType in Enum.GetNames(typeof(ShapeTypes)))
                //{
                //    if(shapeType.ToString().ToLower() == 
                //        iDataObject.GetData(
                //        typeof(string)).ToString().ToLower())
                //    {
                //        feedbackCursor = CursorPalette.DropShape;
                //        e.Effect = DragDropEffects.Copy;
                //        return true;
                //    }
                //}
                feedbackCursor = CursorPalette.DropText;
                e.Effect = DragDropEffects.Copy;
                return true;
            }

            if (iDataObject.GetDataPresent(DataFormats.FileDrop))
            {
                feedbackCursor = CursorPalette.Add;
                e.Effect = DragDropEffects.Copy;
                return true;
            }
            
            // Having problems getting images off the clipboard.
            //if(iDataObject.GetDataPresent(typeof(Bitmap)))
            //{
            //    feedbackCursor = CursorPallet.DropImage;
            //    e.Effect = DragDropEffects.Copy;
            //    return;

            //}
            return false;
        }


        public void GiveFeedback(GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            Cursor.Current = feedbackCursor;
        }
    }

}
