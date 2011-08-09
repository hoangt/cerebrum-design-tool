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
 * CerebrumConnection.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: This is a Netron-based object representing a Cerebrum Connection.
 * History: 
 * >> (13 Sep 2010) Matthew Cotter: Implemented basics of Netron-inherited Connection class, to allow for customization of connection properties.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Netron;
using Netron.Diagramming;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Drawing.Imaging;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using CerebrumSharedClasses;
using System.Xml;
using FalconClockManager;

namespace CerebrumNetronObjects
{
    /// <summary>
    /// Defines a Connection object, inherited from the Netron Connection class, that is extended to include properties that Cerebrum may require.
    /// The object itself represents a logical connection between two components in the Cerebrum design environment.
    /// </summary>
    public class CerebrumConnection : Connection
    {
        /// <summary>
        /// Creates a connection between the specified points.
        /// </summary>
        /// <param name="ConnFrom">The starting point for the connection.</param>
        /// <param name="ConnTo">The ending point for the connection.</param>
        public CerebrumConnection(Point ConnFrom, Point ConnTo)
            : base(ConnFrom, ConnTo)
        {
        }
        
        /// <summary>
        /// Creates a connection between the specified points on the specified Model.
        /// </summary>
        /// <param name="ConnFrom">The starting point for the connection.</param>
        /// <param name="ConnTo">The ending point for the connection.</param>
        /// <param name="ConnModel">The Netron model on which the connection is created.</param>
        public CerebrumConnection(Point ConnFrom, Point ConnTo, IModel ConnModel)
            : base(ConnFrom, ConnTo, ConnModel)
		{
		}

        /// <summary>
        /// Default constructor.  Creates a connection from (10, 10) to (20, 20) to be later manipulated.
        /// </summary>
        public CerebrumConnection()
            : base(new Point(10,10), new Point(20,20)) { 
        
        }
    }
}
