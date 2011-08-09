#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
#endregion

namespace SkmDataStructures2
{
    /// <summary>
    /// The TreeNode class represents a node in a tree
    /// </summary>
    /// <typeparam name="T">The type of data stored in the tree node.</typeparam>
    public class TreeNode<T> : Node<T>
    {
        #region Constructors
        public TreeNode() : base() { }
        public TreeNode(T data) : base(data, null) { }        
        #endregion

        #region Public Properties
        public TreeNodeList<T> Children
        {
            get
            {
                if (base.Neighbors == null)
                    return null;
                else
                    return (TreeNodeList<T>)base.Neighbors;
            }            
        }
        #endregion

        #region Methods
        public void AddNode(TreeNode<T> Node)
        {
            if (base.Neighbors == null)
                base.Neighbors = new TreeNodeList<T>();
            base.Neighbors.Add(Node);
        }

        public void RemoveNode(T NodeValue)
        {
            Neighbors.Remove(Neighbors.FindByValue(NodeValue));
        }
        #endregion
    }

    public class TreeNodeList<T> : NodeList<T>
    {
        #region Constructors
        public TreeNodeList() : base() { }

        public TreeNodeList(int initialSize)
        {
            // Add the specified number of items
            for (int i = 0; i < initialSize; i++)
                base.Items.Add(default(TreeNode<T>));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Searches the TreeNodeList for a Node containing a particular value.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>The TreeNode in the TreeNodeList, if it exists; null otherwise.</returns>
        public TreeNode<T> FindByValue(T value)
        {
            // search the list for the value
            foreach (TreeNode<T> node in Items)
                if (node.Value.Equals(value))
                    return node;

            // if we reached here, we didn't find a matching node
            return null;
        }
        #endregion

    }

}



    