#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion;

namespace SkmDataStructures2
{

    /// <summary>
    /// Represents a tree.  This class provides access to the Root of the tree.  The developer
    /// must manually create the tree by adding descendents to the root.
    /// </summary>
    /// <typeparam name="T">The type of data stored in the binary tree's nodes.</typeparam>
    public class Tree<T>
    {
        #region Private Member Variables
        private TreeNode<T> root = null;
        #endregion

        #region Public Methods
        /// <summary>
        /// Clears out the contents of the tree.
        /// </summary>
        public void Clear()
        {
            root = null;
        }
        #endregion

        #region Public Properties
        public TreeNode<T> Root
        {
            get
            {
                return root;
            }
            set
            {
                root = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a tree node to toplevel tree
        /// </summary>
        /// <param name="Node">tree node to add</param>
        public void AddNode(TreeNode<T> Node)
        {
            if (Root == null)
                Root = new TreeNode<T>();
            Root.AddNode(Node);
        }

        /// <summary>
        /// Removes a tree node from toplevel tree
        /// </summary>
        /// <param name="Node">tree node to add</param>
        public void RemoveNode(T NodeValue)
        {
            if (Root != null)
                Root.RemoveNode(NodeValue);
        }
        #endregion
    }
}


