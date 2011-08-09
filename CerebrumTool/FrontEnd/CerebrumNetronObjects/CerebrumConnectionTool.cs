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
using System;
using System.Diagnostics;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;

namespace CerebrumNetronObjects
{
    /// <summary>
    /// Copy of the Netron.Diagramming.Core.ConnectionTool class, required to create CerebrumConnections objects rather than Netron Connection class objects in the design.
    /// </summary>
    public class CerebrumConnectionTool : AbstractTool, IMouseListener
    {
        #region Fields
        /// <summary>
        /// the location of the mouse when the motion starts
        /// </summary>
        private Point initialPoint;
        private bool doDraw;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ConnectionTool"/> class.
        /// </summary>
        /// <param name="name">The name of the tool.</param>
        public CerebrumConnectionTool(string name)
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
            Controller.View.CurrentCursor = CursorPalette.Grip;
            this.SuspendOtherTools();
            doDraw = false;
        }

        /// <summary>
        /// Handles the mouse down event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        /// <returns>Returns 'true' if the event was handled, otherwise 'false'.</returns>
        public bool MouseDown(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            if (e.Button == MouseButtons.Left && Enabled && !IsSuspended)
            {

                initialPoint = e.Location;
                doDraw = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles the mouse move event
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseMove(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("The argument object is 'null'");
            Point point = e.Location;
            if (IsActive)
            {
                if (foundConnector != null)
                    foundConnector.Hovered = false;
                foundConnector = Selection.FindConnectorAt(e.Location);
                if (foundConnector != null)
                    foundConnector.Hovered = true;
            }
            if (IsActive && doDraw)
            {
                Controller.View.PaintGhostLine(initialPoint, point);
                Controller.View.Invalidate(System.Drawing.Rectangle.Inflate(Controller.View.Ghost.Rectangle, 20, 20));


            }
        }
        private IConnector foundConnector;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        public void MouseUp(MouseEventArgs e)
        {
            if (IsActive)
            {
                DeactivateTool();

                // First, make sure the initial point is far enough away from 
                // the final point to make a connection.
                int maxX = Math.Abs(Math.Max(initialPoint.X, e.Location.X));
                int maxY = Math.Abs(Math.Max(initialPoint.Y, e.Location.Y));

                if (!(maxX > ConnectionBase.MinLength) ||
                    !(maxY > ConnectionBase.MinLength))
                {
                    return;
                }

                //whatever comes hereafter, a compund command is the most economic approach
                CompoundCommand package = new CompoundCommand(this.Controller);

                //let's see if the connection endpoints hit other connectors
                //note that the following can be done because the actual connection has not been created yet
                //otherwise the search would find the endpoints of the newly created connection, which
                //would create a loop and a stack overflow!
                IConnector startConnector = Selection.FindConnectorAt(initialPoint);
                IConnector endConnector = Selection.FindConnectorAt(e.Location);

                #region Create the new connection
                CerebrumConnection cn = new CerebrumConnection(this.initialPoint, e.Location, this.Controller.Model);
                AddConnectionCommand newcon = new AddConnectionCommand(this.Controller, cn);
                package.Commands.Add(newcon);
                #endregion

                //#region Initial attachment?
                //if(startConnector != null)
                //{
                //    BindConnectorsCommand bindStart = new BindConnectorsCommand(this.Controller, startConnector, cn.From);
                //    package.Commands.Add(bindStart);
                //}
                //#endregion 

                //#region Final attachment?
                //if(endConnector != null)
                //{
                //    BindConnectorsCommand bindEnd = new BindConnectorsCommand(this.Controller, endConnector, cn.To);
                //    package.Commands.Add(bindEnd);
                //}
                //#endregion 

                package.Text = "New connection";
                this.Controller.UndoManager.AddUndoCommand(package);

                //do it all
                package.Redo();

                //reset highlight of the found connector
                if (foundConnector != null)
                    foundConnector.Hovered = false;
                //drop the painted ghost
                Controller.View.ResetGhost();
                //release other tools
                this.UnsuspendTools();

                ActivateTool();
            }
        }
        #endregion
    }
}
