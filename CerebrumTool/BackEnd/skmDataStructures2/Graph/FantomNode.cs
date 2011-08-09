#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace SkmDataStructures2
{
    
    /// <summary>
    /// Extends the graph node and adds N number of ports to each node
    /// </summary>
    /// <typeparam name="T">Type of node stored in fantom node</typeparam>
    
    public class FantomNode<T> : GraphNode<T>
    {
        #region Private Member Variables
        //private int[] ports;        // N number of ports associated to each node
        private List<int> ports;        // N number of ports associated to each node
        private int DefNumOfPorts = 8; // Default number of ports
        private object fpga_board;
        #endregion


        #region Constructors
        public FantomNode() : base() { }
        public FantomNode(T value) : base(value) { }
        public FantomNode(T value, NodeList<T> neighbors) : base(value, neighbors) { }
        public FantomNode(T value, int NumPorts) : base(value)
        {
            ports = new List<int>(NumPorts);
        }
        #endregion

        #region Properties
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
        #endregion
    }
}
