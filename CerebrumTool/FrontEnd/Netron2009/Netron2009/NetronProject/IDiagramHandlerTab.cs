using System;
using System.Collections.Generic;
using System.Text;
using WeifenLuo.WinFormsUI.Docking;
using Netron.Diagramming.Win;

namespace NetronProject
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// Specifies the signature for all dockable forms that need to be
    /// attached to a DiagramControl.
    /// </summary>
    // ----------------------------------------------------------------------
    public interface IDiagramHandlerTab : IDockContent
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the DiagramControl for the tab.
        /// </summary>
        // ------------------------------------------------------------------
        DiagramControl Diagram
        {
            get;
            set;
        }
    }
}
