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
/*************************************************************************
 * FalconGraph.cs
 * Author: S. Desmond Nathanson (with code inherited from Ahmed Al Maashri)
 * Date: June 4, 2010
 * Description: This is a structure used the support the FalconGraph and
 *              the FalconNodeList it takes the node structure and uses 
 *              the FalconNode as its nodes. Most notably, it introduces
 *              neighbors.
 *              
 * History:
 * >> S. Desmond Nathanson (June 4, 2010): Comments added, unfortunately,
 *                                         this is the first time so no 
 *                                         prior history exists at this
 *                                         point.
 *************************************************************************/
#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SkmDataStructures2;

#endregion

namespace FalconGraph
{

    /// <summary>
    /// Extends the graph node and adds N number of ports to each node
    /// </summary>
    /// <typeparam name="T">Type of node stored in Falcon node</typeparam>

    public class FalconNode<T> : GraphNode<T>
    {
        #region Private Member Variables
        //private int[] ports;        // N number of ports associated to each node
        private List<int> ports;        // N number of ports associated to each node
        private int DefNumOfPorts = 8; // Default number of ports
        private object fpga_board;
        private FalconNodeList<T> neighbors = null;
        //private T data;
        #endregion


        #region Constructors
        /// <summary>
        /// Basic constructor, makes a node with a null ID and no neighbors.
        /// </summary>
        public FalconNode() : base() { }
        /// <summary>
        /// Constructor, makes a node with the input value as its ID and no neighbors
        /// </summary>
        /// <param name="value">Node's ID</param>
        public FalconNode(T value) : this(value, null) { }
        /// <summary>
        /// Constructor, makes a node with the input value as its ID and the FalconNodeList as its neighbors
        /// </summary>
        /// <param name="value">Node's ID</param>
        /// <param name="neighbors">Nodes neighbors</param>
        public FalconNode(T value, FalconNodeList<T> neighbors)
        {
            this.Value = value;
            this.neighbors = neighbors;
        }
        /// <summary>
        /// Constructor, makes a node with the input value as its ID and a number of ports
        /// </summary>
        /// <param name="value">Node's ID</param>
        /// <param name="NumPorts">Number of ports</param>
        public FalconNode(T value, int NumPorts) : base(value)
        {
            ports = new List<int>(NumPorts);
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        new public FalconNodeList<T> Neighbors
        {
            get
            {
                if (neighbors == null)
                    neighbors = new FalconNodeList<T>();

                return neighbors;
            }
        }

        
        /// <summary>
        /// Returns the set of ports and destinations connected to.       
        /// </summary>        
        public List<int> Ports
        {
            get
            {
                if (ports == null)
                    ports = new List<int>(DefNumOfPorts);

                return ports;
            }
        }

        /// <summary>
        /// Gets/Sets FPGA Board Contents
        /// </summary>
        public object FPGA_BOARD
        {
            get { return this.fpga_board; }

            set { this.fpga_board = value; }
        }

        /// <summary>
        /// Checks to see if some value is equal to the node's identifier.
        /// </summary>
        /// <param name="compare">Node to be compared</param>
        /// <returns>True if equal, false otherwise</returns>
        public bool isEqual(T compare)
        {
            return EqualityComparer<T>.Default.Equals(Value, compare);
        }

        #endregion
    }
}
