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
 * Description: This is a structure used the support the FalconGraphBuilder
 *              it takes the graph structure and uses the FalconNode as its
 *              nodes.
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
using System.Collections;
using System.Text;
using SkmDataStructures2;

#endregion

namespace FalconGraph
{
    /// <summary>
    /// Stores the Falcon nodes in a graph structure. Meant to allow the nodes to have any identifying type.
    /// The nodes themselves keep track of edges.
    /// </summary>
    /// <typeparam name="T">Type to identify the nodes.</typeparam>
    public class FalconGraph<T> : IEnumerable<T>
    {
        #region Private Member Variables
        /// <summary>
        /// The nodes of the graph.
        /// </summary>
        protected FalconNodeList<T> nodeSet;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor, makes an empty graph.
        /// </summary>
        public FalconGraph() : this(null) { }
        /// <summary>
        /// Constructor which pre-populates its graph.
        /// </summary>
        /// <param name="nodeSet">The nodes of the graph</param>
        public FalconGraph(FalconNodeList<T> nodeSet)
        {
            if (nodeSet == null)
                this.nodeSet = new FalconNodeList<T>();
            else
                this.nodeSet = nodeSet;
        }
        #endregion

        #region Methods
        #region Add
        #region AddNode
        /// <summary>
        /// Adds a new FalconNode instance to the Graph
        /// </summary>
        /// <param name="node">The FalconNode instance to add.</param>
        public void AddNode(FalconNode<T> node)
        {
            // adds a node to the graph
            nodeSet.Add(node);
        }

        /// <summary>
        /// Adds a new FalconNode with value "value" to the graph
        /// </summary>
        /// <param name="value">The value of the new node</param>
        public void AddNode(T value)
        {
            nodeSet.Add(new FalconNode<T>(value));
        }

        /// <summary>
        /// Adds a new value to the graph.
        /// </summary>
        /// <param name="value">The value to add to the graph</param>
        /// <param name="no_ports">Number of ports on the node</param>
        public void AddNode(T value, int no_ports)
        {
            nodeSet.Add(new FalconNode<T>(value, no_ports));
        }
        #endregion

        #region Add*Edge Methods
        /// <summary>
        /// Adds a directed edge from a GraphNode with one value (from) to a GraphNode with another value (to).
        /// </summary>
        /// <param name="from">The value of the GraphNode from which the directed edge eminates.</param>
        /// <param name="to">The value of the GraphNode to which the edge leads.</param>
        public void AddDirectedEdge(T from, T to)
        {
            AddDirectedEdge(from, to, 0);
        }
        
        /// <summary>
        /// Adds a directed edge from one FalconNode (from) to another (to).
        /// </summary>
        /// <param name="from">The FalconNode from which the directed edge eminates.</param>
        /// <param name="to">The FalconNode to which the edge leads.</param>
        public void AddDirectedEdge(FalconNode<T> from, FalconNode<T> to)
        {
            AddDirectedEdge(from, to, 0);
        }

        /// <summary>
        /// Adds a directed edge from one FalconNode (from) to another (to) with an associated cost.
        /// </summary>
        /// <param name="from">The FalconNode from which the directed edge eminates.</param>
        /// <param name="to">The FatnomNode to which the edge leads.</param>
        /// <param name="cost">The cost of the edge from "from" to "to".</param>
        public void AddDirectedEdge(FalconNode<T> from, FalconNode<T> to, float cost)
        {
            from.Neighbors.Add(to);
            from.Costs.Add(cost);
        }

        /// <summary>
        /// Adds a directed edge from a GraphNode with one value (from) to a GraphNode with another value (to)
        /// with an associated cost.
        /// </summary>
        /// <param name="from">The value of the GraphNode from which the directed edge eminates.</param>
        /// <param name="to">The value of the GraphNode to which the edge leads.</param>
        /// <param name="cost">The cost of the edge from "from" to "to".</param>
        public void AddDirectedEdge(T from, T to, float cost)
        {
            ((FalconNode<T>)nodeSet.FindByValue(from)).Neighbors.Add(nodeSet.FindByValue(to));
            ((FalconNode<T>)nodeSet.FindByValue(from)).Costs.Add(cost);
        }

        /// <summary>
        /// Adds a directed edge from a FalconNode with one value (from) to a FalconNode 
        /// with another value (to) with an associated cost.
        /// </summary>
        /// <param name="from">The value of the FalconNode from which the directed edge eminates.</param>
        /// <param name="to">The value of the FalconNode to which the edge leads.</param>
        /// <param name="cost">The cost of the edge from "from" to "to".</param>
        /// <param name="outport">The "from" node out port associated with the edge.</param>
        public void AddDirectedEdge(T from, T to, float cost, int outport)
        {
            ((FalconNode<T>)nodeSet.FindByValue(from)).Neighbors.Add(nodeSet.FindByValue(to));
            ((FalconNode<T>)nodeSet.FindByValue(from)).Costs.Add(cost);
            ((FalconNode<T>)nodeSet.FindByValue(from)).Ports.Add(outport);
        }

        /// <summary>
        /// Adds an undirected edge from one FalconNode to another.
        /// </summary>
        /// <param name="from">One of the FalconNodes that is joined by the edge.</param>
        /// <param name="to">One of the FalconNodes that is joined by the edge.</param>
        public void AddUndirectedEdge(FalconNode<T> from, FalconNode<T> to)
        {
            AddUndirectedEdge(from, to, 0);
        }

        /// <summary>
        /// Adds an undirected edge from one FalconNode to another with an associated cost.
        /// </summary>
        /// <param name="from">One of the FalconNodes that is joined by the edge.</param>
        /// <param name="to">One of the FalconNodes that is joined by the edge.</param>
        /// <param name="cost">The cost of the undirected edge.</param>
        public void AddUndirectedEdge(FalconNode<T> from, FalconNode<T> to, float cost)
        {
            from.Neighbors.Add(to);
            from.Costs.Add(cost);

            to.Neighbors.Add(from);
            to.Costs.Add(cost);
        }

        /// <summary>
        /// Adds an undirected edge from a FalconNode with one value (from) to a FalconNode 
        /// with another value (to) with an associated cost.
        /// </summary>
        /// <param name="from">The value of one of the FalconNode that is joined by the edge.</param>
        /// <param name="to">The value of one of the FalconNode that is joined by the edge.</param>
        /// <param name="cost">The cost of the undirected edge.</param>
        public void AddUndirectedEdge(T from, T to, float cost)
        {
            ((FalconNode<T>)nodeSet.FindByValue(from)).Neighbors.Add(nodeSet.FindByValue(to));
            ((FalconNode<T>)nodeSet.FindByValue(from)).Costs.Add(cost);

            ((FalconNode<T>)nodeSet.FindByValue(to)).Neighbors.Add(nodeSet.FindByValue(from));
            ((FalconNode<T>)nodeSet.FindByValue(to)).Costs.Add(cost);
        }
        #endregion
        #endregion

        #region Clear

        #endregion

        #region Contains
        /// <summary>
        /// Returns a Boolean, indicating if a particular value exists within the graph.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>True if the value exist in the graph; false otherwise.</returns>
        public bool Contains(T value)
        {
            return nodeSet.FindByValue(value) != null;
        }
        #endregion

        #region Remove
        /// <summary>
        /// Attempts to remove a node from a graph.
        /// </summary>
        /// <param name="value">The value that is to be removed from the graph.</param>
        /// <returns>True if the corresponding node was found, and removed; false if the value was not
        /// present in the graph.</returns>
        /// <remarks>This method removes the FalconNode instance, and all edges leading to or from the
        /// FalconNode.</remarks>
        public bool Remove(T value)
        {
            // first remove the node from the nodeset
            FalconNode<T> nodeToRemove = (FalconNode<T>)nodeSet.FindByValue(value);
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            // otherwise, the node was found
            nodeSet.Remove(nodeToRemove);

            // enumerate through each node in the nodeSet, removing edges to this node
            foreach (FalconNode<T> gnode in nodeSet)
            {
                int index = gnode.Neighbors.IndexOf(nodeToRemove);
                if (index != -1)
                {
                    // remove the reference to the node and associated cost
                    gnode.Neighbors.RemoveAt(index);
                    gnode.Costs.RemoveAt(index);
                }
            }

            return true;
        }

        /// <summary>
        /// Edge removal
        /// </summary>
        /// <param name="from">First node in the pair</param>
        /// <param name="to">Second node in the pair</param>
        /// <returns>True iff removal was succesful</returns>
        public bool Remove(T from, T to)
        {
            FalconNode<T> SourceNode = (FalconNode<T>)nodeSet.FindByValue(from);
            FalconNode<T> SinkNode = (FalconNode<T>)nodeSet.FindByValue(to);
            if (SourceNode == null || SinkNode == null || !(SourceNode.Neighbors.Contains(SinkNode)))
                return false;
            else
            {
                SourceNode.Costs.RemoveAt(SourceNode.Neighbors.IndexOf(SinkNode));
                SourceNode.Neighbors.Remove(SinkNode);
                return true;
            }
        }
        #endregion

        #region IEnumerable<T> Members
        /// <summary>
        /// Returns an enumerator that allows for iterating through the contents of the graph.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (FalconNode<T> gnode in nodeSet)
                yield return gnode.Value;
        }
        // This is added to satisfy C# IEnumerable interface implementation requirement
        IEnumerator IEnumerable.GetEnumerator()
        {  // reuse typed enumerator
            return GetEnumerator();
        }
        #endregion

        #region FalconNodeAt
        /// <summary>
        /// Returns a Falcon Node according to the index (this does not find a node by value)
        /// </summary>
        public FalconNode<T> FalconNodeAt(int index)
        {
            return ((FalconNode<T>)Nodes[index]);
        }
        #endregion

        #region GetFalconNodeByVal
        /// <summary>
        /// Returns a Falcon Node whose value is Val
        /// </summary>
        /// <param name="Val">Value of Falcon Nofr</param>
        /// <returns>Falcon Node whose value is Val</returns>
        public FalconNode<T> GetFalconNodeByVal(T Val)
        {
            foreach (Node<T> node in Nodes)
            {
                if (((FalconNode<T>)node).Value.Equals(Val))
                    return ((FalconNode<T>)node);
            }
            return null;
        }
        #endregion

        #region UpdateValues
        /// <summary>
        /// Updates the values of an edge, by some positive or negative float
        /// </summary>
        /// <param name="from">The first node in the edge pair</param>
        /// <param name="to">The second node in the edge pair</param>
        /// <param name="amount">The float which is the value the edge will be changed by</param>
        public void UpdateWeight(T from, T to, float amount)
        {
            FalconNode<T> SourceNode = nodeSet.FindByValue(from);
            FalconNode<T> SinkNode = nodeSet.FindByValue(to);
            bool debugotron = SourceNode.Neighbors.Contains(SinkNode);
            int index_of_SinkNode = SourceNode.Neighbors.IndexOf(SinkNode);
            float new_weight = SourceNode.Costs[index_of_SinkNode] + amount;
            Remove(from, to);
            AddDirectedEdge(from, to, new_weight);
        }

        #endregion
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns the set of nodes in the graph.
        /// </summary>
        public FalconNodeList<T> Nodes
        {
            get
            {
                FalconNodeList<T> ReturnNodeSet = new FalconNodeList<T>();
                foreach (FalconNode<T> cur_node in nodeSet)
                    ReturnNodeSet.Add(cur_node);

                return ReturnNodeSet;
            }
        }

        /// <summary>
        /// Returns the number of vertices in the graph.
        /// </summary>
        public int Count
        {
            get { return nodeSet.Count; }
        }
        #endregion


    }
}
