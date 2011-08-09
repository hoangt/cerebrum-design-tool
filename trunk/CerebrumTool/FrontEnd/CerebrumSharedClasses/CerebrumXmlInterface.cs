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
 * BackEndToolDelegates.cs
 * Name: Matthew Cotter
 * Date: 11 Oct 2010 
 * Description: Defines several static methods to locate XML nodes and attributes as children of an XML node.
 * History: 
 * >> (22 Oct 2010) Matthew Cotter: Modified return values to return an empty string if a value is not specified or node is not found.
 * >> (19 Oct 2010) Matthew Cotter: Added additional error handling support.
 * >> (11 Oct 2010) Matthew Cotter: Defined methods to read and access XML Nodes and Attributes by 'dotted-hierarchical-addressing'
 * >> (11 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace CerebrumSharedClasses
{
    /// <summary>
    /// Static class to access and locate XML nodes in a document or node structure through hierarchical addressing of nodes
    /// </summary>
    public static class CerebrumXmlInterface
    {
        /// <summary>
        /// Returns the value of a named attribute of the addressed node, if it exists.  Location of the node and attribute are case-insensitive.  For a case-sensitive search, use
        /// the GetXmlAttribute(XmlNode, string, string, bool) overload.
        /// </summary>
        /// <param name="xNode">The node underwhich to search, and relative to which the NodePath is addressed.</param>
        /// <param name="NodePath">The path to the desired node, relative to the node in the xNode parameter</param>
        /// <param name="AttributeName">The name of the attribute whose value is to be retrieved</param>
        /// <returns>A string containing the attribute's value if found, an empty string if either the attribute or node was not found, or null if an error occurs.</returns>
        public static string GetXmlAttribute(XmlNode xNode, string NodePath, string AttributeName)
        {
            if (xNode == null)
                return null;
            return GetXmlAttribute(xNode, NodePath, AttributeName, true);
        }
        /// <summary>
        /// Returns the value of a named attribute of the addressed node, if it exists
        /// </summary>
        /// <param name="xNode">The node underwhich to search, and relative to which the NodePath is addressed.</param>
        /// <param name="NodePath">The path to the desired node, relative to the node in the xNode parameter</param>
        /// <param name="AttributeName">The name of the attribute whose value is to be retrieved</param>
        /// <param name="IgnoreCase">Indicates whether the search should be case-insensitive(true) or case-sensitive.</param>
        /// <returns>A string containing the attribute's value if found, an empty string if either the attribute or node was not found, or null if an error occurs.</returns>
        public static string GetXmlAttribute(XmlNode xNode, string NodePath, string AttributeName, bool IgnoreCase)
        {
            if (xNode == null)
                return null;
            try
            {
                if (NodePath == string.Empty)
                {
                    foreach (XmlAttribute xAttr in xNode.Attributes)
                    {
                        if (String.Compare(xAttr.Name, AttributeName, IgnoreCase) == 0)
                            return xAttr.Value;
                    }
                }
                else
                {
                    string[] Nodes = NodePath.Split('.');
                    foreach (XmlNode xChild in xNode.ChildNodes)
                    {
                        if (xChild.NodeType != XmlNodeType.Element)
                            continue;
                        if (String.Compare(xChild.Name, Nodes[0], IgnoreCase) == 0)
                        {
                            string SubPath = String.Join(".", Nodes, 1, Nodes.Length - 1);
                            return GetXmlAttribute(xChild, SubPath, AttributeName, IgnoreCase);
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
            return null;
        }

        /// <summary>
        /// Returns the value of the addressed node's InnerText, if it exists.  Location of the node is case-insensitive.  For a case-sensitive search, use
        /// the GetXmlInnerText(XmlNode, string, bool) overload.
        /// </summary>
        /// <param name="xNode">The node underwhich to search, and relative to which the NodePath is addressed.</param>
        /// <param name="NodePath">The path to the desired node, relative to the node in the xNode parameter</param>
        /// <returns>A string containing the node's InnerText if found, an empty string if the node was not found, or null if an error occurs.</returns>
        public static string GetXmlInnerText(XmlNode xNode, string NodePath)
        {
            if (xNode == null)
                return null;
            return GetXmlInnerText(xNode, NodePath, true);
        }
        /// <summary>
        /// Returns the value of the addressed node's InnerText, if it exists.  Location of the node is case-insensitive.  For a case-sensitive search, use
        /// the GetXmlInnerText(XmlNode, string, bool) overload.
        /// </summary>
        /// <param name="xNode">The node underwhich to search, and relative to which the NodePath is addressed.</param>
        /// <param name="NodePath">The path to the desired node, relative to the node in the xNode parameter</param>
        /// <param name="IgnoreCase">Indicates whether the search should be case-insensitive(true) or case-sensitive.</param>
        /// <returns>A string containing the node's InnerText if found, an empty string if the node was not found, or null if an error occurs.</returns>
        public static string GetXmlInnerText(XmlNode xNode, string NodePath, bool IgnoreCase)
        {
            if (xNode == null)
                return null;
            try
            {
                if (NodePath == string.Empty)
                {
                    return xNode.InnerText;
                }
                else
                {
                    string[] Nodes = NodePath.Split('.');
                    foreach (XmlNode xChild in xNode.ChildNodes)
                    {
                        if (xChild.NodeType != XmlNodeType.Element)
                            continue;
                        if (String.Compare(xChild.Name, Nodes[0], IgnoreCase) == 0)
                        {
                            string SubPath = String.Join(".", Nodes, 1, Nodes.Length - 1);
                            return GetXmlInnerText(xChild, SubPath, IgnoreCase);
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
            return null;
        }

        /// <summary>
        /// Returns the addressed node, if it exists.  Location of the node is case-insensitive.  For a case-sensitive search, use
        /// the GetXmlNode(XmlNode, string, bool) overload.
        /// </summary>
        /// <param name="xNode">The node underwhich to search, and relative to which the NodePath is addressed.</param>
        /// <param name="NodePath">The path to the desired node, relative to the node in the xNode parameter</param>
        /// <returns>The XmlNode referencing the node if found, or null if an error occurs or the node was not found.</returns>
        public static XmlNode GetXmlNode(XmlNode xNode, string NodePath)
        {
            if (xNode == null)
                return null;
            return GetXmlNode(xNode, NodePath, true);
        }
        /// <summary>
        /// Returns the addressed node, if it exists.  Location of the node is case-insensitive.  For a case-sensitive search, use
        /// the GetXmlNode(XmlNode, string, bool) overload.
        /// </summary>
        /// <param name="xNode">The node underwhich to search, and relative to which the NodePath is addressed.</param>
        /// <param name="NodePath">The path to the desired node, relative to the node in the xNode parameter</param>
        /// <param name="IgnoreCase">Indicates whether the search should be case-insensitive(true) or case-sensitive.</param>
        /// <returns>The XmlNode referencing the node if found, or null if an error occurs or the node was not found.</returns>
        public static XmlNode GetXmlNode(XmlNode xNode, string NodePath, bool IgnoreCase)
        {
            if (xNode == null)
                return null;
            try
            {
                if (NodePath == string.Empty)
                {
                    return xNode;
                }
                else
                {
                    string[] Nodes = NodePath.Split('.');
                    foreach (XmlNode xChild in xNode.ChildNodes)
                    {
                        if (xChild.NodeType != XmlNodeType.Element)
                            continue;
                        if (String.Compare(xChild.Name, Nodes[0], IgnoreCase) == 0)
                        {
                            string SubPath = String.Join(".", Nodes, 1, Nodes.Length - 1);
                            return GetXmlNode(xChild, SubPath, IgnoreCase);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
            return null;
        }
    }
}
