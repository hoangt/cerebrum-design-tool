#region Using directives

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

#endregion

namespace SkmDataStructures2
{
    
    public class FantomGraph<T> : Graph<T> , IEnumerable<T>
    {
        #region Private Member Variables
        
        #endregion

        #region Constructors
        public FantomGraph() : base() {}
        public FantomGraph(NodeList<T> nodeSet) : base(nodeSet) { }        
        #endregion

        #region Methods
        #region Add
        #region AddNode
        /// <summary>
        /// Adds a new FantomNode instance to the Graph
        /// </summary>
        /// <param name="node">The FantomNode instance to add.</param>
        public void AddNode(FantomNode<T> node)
        {
            // adds a node to the graph
            nodeSet.Add(node);
        }

        /// <summary>
        /// Adds a new value to the graph.
        /// </summary>
        /// <param name="value">The value to add to the graph</param>
        public void AddNode(T value, int no_ports)
        {
            nodeSet.Add(new FantomNode<T>(value, no_ports));
        }
        #endregion

        #region Add*Edge Methods
        /// <summary>
        /// Adds a directed edge from one FantomNode (from) to another (to).
        /// </summary>
        /// <param name="from">The FantomNode from which the directed edge eminates.</param>
        /// <param name="to">The FantomNode to which the edge leads.</param>
        public void AddDirectedEdge(FantomNode<T> from, FantomNode<T> to)
        {
            AddDirectedEdge(from, to, 0);
        }

        /// <summary>
        /// Adds a directed edge from one FantomNode (from) to another (to) with an associated cost.
        /// </summary>
        /// <param name="from">The FantomNode from which the directed edge eminates.</param>
        /// <param name="to">The FatnomNode to which the edge leads.</param>
        /// <param name="cost">The cost of the edge from "from" to "to".</param>
        public void AddDirectedEdge(FantomNode<T> from, FantomNode<T> to, float cost)
        {
            from.Neighbors.Add(to);
            from.Costs.Add(cost);
        }

        /// <summary>
        /// Adds a directed edge from a FantomNode with one value (from) to a FantomNode 
        /// with another value (to) with an associated cost.
        /// </summary>
        /// <param name="from">The value of the FantomNode from which the directed edge eminates.</param>
        /// <param name="to">The value of the FantomNode to which the edge leads.</param>
        /// <param name="cost">The cost of the edge from "from" to "to".</param>
        /// <param name="outport">The "from" node out port associated with the edge.</param>
        public void AddDirectedEdge(T from, T to, float cost, int outport)
        {
            ((FantomNode<T>) nodeSet.FindByValue(from)).Neighbors.Add(nodeSet.FindByValue(to));
            ((FantomNode<T>) nodeSet.FindByValue(from)).Costs.Add(cost);
            ((FantomNode<T>)nodeSet.FindByValue(from)).Ports.Add(outport);
        }

        /// <summary>
        /// Adds an undirected edge from one FantomNode to another.
        /// </summary>
        /// <param name="from">One of the FantomNodes that is joined by the edge.</param>
        /// <param name="to">One of the FantomNodes that is joined by the edge.</param>
        public void AddUndirectedEdge(FantomNode<T> from, FantomNode<T> to)
        {
            AddUndirectedEdge(from, to, 0);
        }

        /// <summary>
        /// Adds an undirected edge from one FantomNode to another with an associated cost.
        /// </summary>
        /// <param name="from">One of the FantomNodes that is joined by the edge.</param>
        /// <param name="to">One of the FantomNodes that is joined by the edge.</param>
        /// <param name="cost">The cost of the undirected edge.</param>
        public void AddUndirectedEdge(FantomNode<T> from, FantomNode<T> to, float cost)
        {
            from.Neighbors.Add(to);
            from.Costs.Add(cost);

            to.Neighbors.Add(from);
            to.Costs.Add(cost);
        }

        /// <summary>
        /// Adds an undirected edge from a FantomNode with one value (from) to a FantomNode 
        /// with another value (to) with an associated cost.
        /// </summary>
        /// <param name="from">The value of one of the FantomNode that is joined by the edge.</param>
        /// <param name="to">The value of one of the FantomNode that is joined by the edge.</param>
        /// <param name="cost">The cost of the undirected edge.</param>
        public new void AddUndirectedEdge(T from, T to, float cost)
        {
            ((FantomNode<T>)nodeSet.FindByValue(from)).Neighbors.Add(nodeSet.FindByValue(to));
            ((FantomNode<T>)nodeSet.FindByValue(from)).Costs.Add(cost);

            ((FantomNode<T>)nodeSet.FindByValue(to)).Neighbors.Add(nodeSet.FindByValue(from));
            ((FantomNode<T>)nodeSet.FindByValue(to)).Costs.Add(cost);
        }
        #endregion
        #endregion

        #region Clear
        
        #endregion

        #region Contains
        
        #endregion

        #region Remove
        /// <summary>
        /// Attempts to remove a node from a graph.
        /// </summary>
        /// <param name="value">The value that is to be removed from the graph.</param>
        /// <returns>True if the corresponding node was found, and removed; false if the value was not
        /// present in the graph.</returns>
        /// <remarks>This method removes the FantomNode instance, and all edges leading to or from the
        /// FantomNode.</remarks>
        public new bool Remove(T value)
        {
            // first remove the node from the nodeset
            FantomNode<T> nodeToRemove = (FantomNode<T>)nodeSet.FindByValue(value);
            if (nodeToRemove == null)
                // node wasn't found
                return false;

            // otherwise, the node was found
            nodeSet.Remove(nodeToRemove);

            // enumerate through each node in the nodeSet, removing edges to this node
            foreach (FantomNode<T> gnode in nodeSet)
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
        #endregion

        #region IEnumerable<T> Members
        /// <summary>
        /// Returns an enumerator that allows for iterating through the contents of the graph.
        public new IEnumerator<T> GetEnumerator()
        {
            foreach (FantomNode<T> gnode in nodeSet)
                yield return gnode.Value;            
        }
        // This is added to satisfy C# IEnumerable interface implementation requirement
        IEnumerator IEnumerable.GetEnumerator()
        {  // reuse typed enumerator
            return GetEnumerator();
        }
        #endregion

        #region FantomNodeAt
        /// <summary>
        /// Returns a Fantom Node
        /// </summary>
        public FantomNode<T> FantomNodeAt(int index)
        {
            return ((FantomNode<T>)Nodes[index]);
        }       
        #endregion
       
        #region GetFantomNodeByVal
        /// <summary>
        /// Returns a Fantom Node whose value is Val
        /// </summary>
        /// <param name="Val">Value of Fantom Nofr</param>
        /// <returns>Fantom Node whose value is Val</returns>
        public FantomNode<T> GetFantomNodeByVal(int Val)
        {
            foreach (Node<T> node in Nodes)
            {
                if (((FantomNode<T>)node).Value.Equals(Val))
                    return ((FantomNode<T>)node);
            }
            return null;
        }
        #endregion

        #region UpdateValues
        /// <summary>
        /// Updates the values of an edge, used in creating routing tables
        /// </summary>
        /// <param name="from">The first node in the edge pair</param>
        /// <param name="to">The second node in the edge pair</param>
        /// <remarks>
        /// This function is simply a placeholder for now. It will be fleshed 
        /// out in greater detail in later builds
        /// </remarks>
        
        //!!!!This function is going to change.!!!!
        public void UpdateCost(T from, T to, float amount)
        {
            //TODO Find a way to update the edge weight
            //int EdgeIndex = StartNode.(FantomNode<T>)nodeSet;
            //(FantomNode<T>)nodeSet.FindByValue(from).Costs(EdgeIndex) = (FantomNode<T>)nodeSet.FindByValue(from).Costs(EdgeIndex) + amount;
        }
        #endregion

        #endregion

        #region Public Properties
        /// <summary>
        /// Returns the set of nodes in the graph.
        /// </summary>
        public new NodeList<T> Nodes
        {
            get
            {
                NodeList<T> ReturnNodeSet = new NodeList<T>();
                foreach (Node<T> cur_node in nodeSet)
                    ReturnNodeSet.Add(cur_node);
                
                return ReturnNodeSet;
            }
        }
        
        /// <summary>
        /// Returns the number of vertices in the graph.
        /// </summary>
        public new int Count
        {
            get { return nodeSet.Count; }
        }
        #endregion

        
    }
}
