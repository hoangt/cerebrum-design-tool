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
 * FalconNodeList.cs
 * Author: S. Desmond Nathanson (with code inherited from Ahmed Al Maashri)
 * Date: June 4, 2010
 * Description: This is a structure used the support the FalconGraph
 *              it takes the NodeList structure and uses the FalconNode as its
 *              nodes.
 *              
 * History:
 * >> S. Desmond Nathanson (June 4, 2010): Comments added, unfortunately,
 *                                         this is the first time so no 
 *                                         prior history exists at this
 *                                         point.
 *************************************************************************/
#region Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using SkmDataStructures2;

#endregion

namespace FalconGraph
{
    /// <summary>
    /// A collection of Falcon Nodes. Mostly to store all of falcon graphs nodes.
    /// </summary>
    /// <typeparam name="T">The type used to identify each Falcon Node.</typeparam>
    public class FalconNodeList<T> : Collection<FalconNode<T>>
    {
        #region Constructors
        /// <summary>
        /// Default constructor, makes an empty list.
        /// </summary>
        public FalconNodeList() : base() { }
        
        /// <summary>
        /// Constructor which specifies the intial size. Makes empty nodes to fill the size.
        /// </summary>
        /// <param name="initialSize">Initial size of the list.</param>
        public FalconNodeList(int initialSize)
        {
            // Add the specified number of items
            for (int i = 0; i < initialSize; i++)
                base.Items.Add(default(FalconNode<T>));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Searches the NodeList for a Node containing a particular value.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns>The Node in the NodeList, if it exists; null otherwise.</returns>
        public FalconNode<T> FindByValue(T value)
        {
            // search the list for the value
            foreach (FalconNode<T> node in Items)
                if (node.isEqual(value))
                    return node;
            // if we reached here, we didn't find a matching node
            return null;
        }
        #endregion
    }
}
