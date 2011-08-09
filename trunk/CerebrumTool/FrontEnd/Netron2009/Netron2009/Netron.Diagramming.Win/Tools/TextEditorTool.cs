using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Netron.Diagramming.Core;
using System.Diagnostics;

namespace Netron.Diagramming.Win
{
    public class TextEditorTool : Netron.Diagramming.Core.HitTool
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="T:TextEditorTool"/> 
        /// class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        // ------------------------------------------------------------------
        public TextEditorTool(string toolName)
            : base(toolName)
        {
        }

        public override bool MouseDown(MouseEventArgs e)
        {
            bool handled = base.MouseDown(e);
            ITextProvider textProvider;
            IDiagramEntity entity;
            Trace.WriteLine("TextEditorTool - mouse down.\n");

            if (Selection.SelectedItems.Count > 0)
            {
                entity = Selection.SelectedItems[0];

                if (entity is ITextProvider)
                {
                    textProvider = entity as ITextProvider;
                    if ((e.Button == MouseButtons.Left) &&
                        (e.Clicks == textProvider.EditTextClicks) &&
                        (textProvider.AllowTextEditing) )
                    {
                        ActivateTool();
                        TextEditor.GetEditor(textProvider);
                        TextEditor.Show();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
