using System;
using System.Drawing;

namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Tool to draw complex rectangles on the canvas
    /// </summary>
    public class ComplexRectangleTool : AbstractDrawingTool
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructor
        public ComplexRectangleTool() : base("ComplexRectangle Tool")
        {
        }
        public ComplexRectangleTool(string name): base(name)
        {

        }
        #endregion

        #region Methods
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsActive && started)
            {
                Point point = new Point(e.X, e.Y);               

                Controller.View.PaintGhostRectangle(startingPoint, point);

                Controller.View.Invalidate(System.Drawing.Rectangle.Inflate(Controller.View.Ghost.Rectangle,20,20));
            }
        }


        /// <summary>
        /// This method will be called when the user has finished drawing a ghost rectangle or bundle
        /// and initiates the actual creation of a bundle and the addition to the model via the appropriate command.
        /// </summary>
        protected override void GhostDrawingComplete()
        {

            try
            {
                ComplexRectangle shape = new ComplexRectangle(this.Controller.Model);
                shape.Width = (int) Rectangle.Width;
                shape.Height = (int) Rectangle.Height;
                AddShapeCommand cmd = new AddShapeCommand(this.Controller, shape, new Point((int) Rectangle.X, (int)Rectangle.Y));
                this.Controller.UndoManager.AddUndoCommand(cmd);
                cmd.Redo();
            }
            catch
            {
                base.Controller.DeactivateTool(this);
                Controller.View.Invalidate();
                throw;
            }

            //base.Controller.DeactivateTool(this);
        }


        #endregion
    }

}
