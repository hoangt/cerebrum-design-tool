using System;
using System.Collections.Generic;
using System.Text;
using Netron.Diagramming.Core;
using System.Windows.Forms;
using System.Drawing;
namespace Netron.Diagramming.Win
{
    /// <summary>
    /// WinForm implementation of the <see cref="IController"/> interface.
    /// </summary>
    public class Controller : ControllerBase
    {
        #region Tool Names

        public const string TextToolName = "Text Tool";
        public const string TextEditorToolName = "Text Editor Tool";

        #endregion

        #region Constructor
        ///<summary>
        ///Default constructor
        ///</summary>
        public Controller(IDiagramControl surface) : base(surface)
        {
            this.AddTool(new TextTool(TextToolName));           
        }
        
        #endregion

        public override bool ActivateTextEditor(ITextProvider textProvider)
        {
            TextEditor.GetEditor(textProvider);
            TextEditor.Show();
            return true;
        }  
    }
}
