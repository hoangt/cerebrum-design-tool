using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Netron.Diagramming.Core;
namespace Netron.Diagramming.Win
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// This tool implements the action of scaling and/or rotating shapes on 
    /// the canvas. 
    /// </summary>
    // ----------------------------------------------------------------------
    public class HitTool : AbstractTool, IMouseListener
    {

        #region Fields
       

     

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:HitTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public HitTool(string name)
            : base(name)
        {
        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when the tool is activated.
        /// </summary>
        protected override void OnActivateTool()
        {
            

        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public bool MouseDown(MouseEventArgs e)
        {
            if(e == null)
                throw new ArgumentNullException("The argument object is 'null'");

            ITextProvider textProvider;
            IDiagramEntity entity;

            if(e.Button == MouseButtons.Left  && Enabled && !IsSuspended)
            {
               
                //if(e.Clicks == 1)
                {
                    TextEditor.Hide();
                    Selection.CollectEntitiesAt(e.Location);
                    if(Selection.SelectedItems.Count > 0)
                    {
                        IMouseListener listener = 
                            Selection.SelectedItems[0].GetService(
                            typeof(IMouseListener)) as IMouseListener;

                        if(listener != null)
                        {
                            if (listener.MouseDown(e))
                                return true;
                        }
                    }
                }

                if (Selection.SelectedItems.Count > 0)
                {
                    entity = Selection.SelectedItems[0];

                    if (entity is ITextProvider)
                    {
                        textProvider = entity as ITextProvider;
                        if ((e.Button == MouseButtons.Left) &&
                            (e.Clicks == textProvider.EditTextClicks) &&
                            (textProvider.AllowTextEditing))
                        {
                            //ActivateTool();
                            TextEditor.GetEditor(textProvider);
                            TextEditor.Show();
                            return true;
                        }
                    }
                }
               
            }
             return false;
        }

        /// <summary>
        /// Handles the mouse move event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseMove(MouseEventArgs e)
        {
      
        }
      
        public void MouseUp(MouseEventArgs e)
        {
            if(IsActive)
            {
                DeactivateTool();
            }
        }
        #endregion
    }

}
