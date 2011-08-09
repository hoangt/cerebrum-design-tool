using System;
using System.Collections.Generic;
using System.Text;
using Netron.Diagramming.Win;
using WeifenLuo.WinFormsUI.Docking;

namespace NetronProject
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// Specifies the signature for all dockable forms that host a 
    /// DiagramControl.
    /// </summary>
    // ----------------------------------------------------------------------
    public interface IDiagramTab : IDockContent
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the diagram that this tab hosts.
        /// </summary>
        // ------------------------------------------------------------------
        DiagramControl Diagram
        {
            get;
        }
    }
}
