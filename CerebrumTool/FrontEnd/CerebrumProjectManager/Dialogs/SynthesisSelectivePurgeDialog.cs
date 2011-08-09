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
 * SynthesisSelectivePurgeDialog.cs
 * Name: Matthew Cotter
 * Date: 22 Feb 2011 
 * Description: Dialog to allow user to selectively choose which cores' previous synthesis work will be cleaned prior to beginning synthesis.
 * History: 
 * >> (20 May 2011) Matthew Cotter: Corrected bug in loading and display of cores for selection.
 * >> (30 Mar 2011) Matthew Cotter: Completed initial version of selective core clean dialog.
 * >> (22 Feb 2011) Matthew Cotter: Simple checked Tree View style hierarchical list of FPGAs, Components, and Cores to be synthesized.
 * >> (22 Feb 2011) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SmartSolutions.Controls;
using CerebrumSharedClasses;
using CerebrumNetronObjects;

namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog allowing user to selectively choose which Components and Cores are purged prior to synthesis.
    /// </summary>
    public partial class SynthesisSelectivePurgeDialog : Form
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SynthesisSelectivePurgeDialog()
        {
            InitializeComponent();
        }

        private void SynthesisSelectivePurgeDialog_Load(object sender, EventArgs e)
        {
        }

        private void PrintRecursive(TreeNode treeNode)
        {
            // Print the node.
            System.Diagnostics.Debug.WriteLine(treeNode.Text);
            // Print each node recursively.
            foreach (TreeNode tn in treeNode.Nodes)
            {
                PrintRecursive(tn);
            }
        }

        /// <summary>
        /// Clears the TriStateTreeView Control and populates it with the provided list of Components and Cores.
        /// </summary>
        /// <param name="FPGAs">The list of FPGAs on which the components/cores will be synthesized..</param>
        /// <param name="Components">The list of CerebrumCore components to be synthesized.</param>
        /// <param name="Cores">The list of ComponentCore cores to be synthesized.</param>
        public void PopulateTreeView(List<string> FPGAs, ref List<CerebrumCore> Components, ref List<ComponentCore> Cores)
        {
            TriStateTreeNode SystemNode = new TriStateTreeNode("Hardware System");
            SystemNode.CheckboxVisible = true;
            SystemNode.IsContainer = true;

            TriStateTreeNode unknownNode = new TriStateTreeNode("Unknown");
            SystemNode.Nodes.Add(unknownNode);
            unknownNode.CheckboxVisible = true;
            unknownNode.IsContainer = true;

            Dictionary<string, TriStateTreeNode> FPGANodes = new Dictionary<string, TriStateTreeNode>();
            Dictionary<string, TriStateTreeNode> ComponentNodes = new Dictionary<string, TriStateTreeNode>();

            // Add FPGAs
            foreach (string FID in FPGAs)
            {
                TriStateTreeNode othersNode = new TriStateTreeNode("Required / Infrastructure Cores");
                othersNode.CheckboxVisible = true;
                othersNode.IsContainer = true;

                TriStateTreeNode FPGANode = new TriStateTreeNode(FID);
                SystemNode.Nodes.Add(FPGANode);
                FPGANode.Nodes.Add(othersNode);

                FPGANode.CheckboxVisible = true;
                FPGANode.IsContainer = true;
                FPGANodes.Add(FID, FPGANode);
                ComponentNodes.Add(String.Format("{0}_other", FID), othersNode);
            }

            // Add CerebrumCores
            foreach (CerebrumCore CC in Components)
            {
                TriStateTreeNode ComponentNode = new TriStateTreeNode(CC.CoreInstance);
                if (FPGANodes.ContainsKey(CC.MappedFPGA))
                {
                    FPGANodes[CC.MappedFPGA].Nodes.Add(ComponentNode);
                }
                else
                {
                    unknownNode.Nodes.Add(ComponentNode);
                }

                ComponentNode.Tag = CC;
                ComponentNode.CheckboxVisible = true;
                ComponentNode.IsContainer = true;
                ComponentNodes.Add(CC.CoreInstance, ComponentNode);
            }

            // Add Component Cores
            foreach (ComponentCore CompCore in Cores)
            {
                TriStateTreeNode CoreNode = new TriStateTreeNode(CompCore.CoreInstance);
                CoreNode.Tag = CompCore;
                CoreNode.CheckboxVisible = true;
                CoreNode.IsContainer = false;
                CoreNode.Checked = CompCore.PurgeBeforeSynthesis;
                
                if (CompCore.OwnerComponent != null)
                {
                    if (ComponentNodes.ContainsKey(CompCore.OwnerComponent.CoreInstance))
                    {
                        ComponentNodes[CompCore.OwnerComponent.CoreInstance].Nodes.Add(CoreNode);
                    }
                    else 
                    {
                        unknownNode.Nodes.Add(CoreNode);
                    }
                }
                else 
                {
                    string OthersNodeID = String.Format("{0}_other", CompCore.MappedFPGA);
                    if (ComponentNodes.ContainsKey(OthersNodeID))
                    {
                        ComponentNodes[OthersNodeID].Nodes.Add(CoreNode);
                    }
                    else 
                    {
                        unknownNode.Nodes.Add(CoreNode);
                    }
                }
            }
            // Remove the "Unknown" Node, if nothing has fallen into it
            if (unknownNode.Nodes.Count == 0)
            {
                SystemNode.Nodes.Remove(unknownNode);
            }

            treeCores.BeginUpdate();
            treeCores.Nodes.Clear();
            treeCores.Nodes.Add(SystemNode);
            treeCores.ExpandAll();
            treeCores.EndUpdate();
            //PrintRecursive(SystemNode);
        }

        /// <summary>
        /// Updates the Purge State of call Components and Cores in the TreeView Node.
        /// </summary>
        public void UpdateStatus()
        {
            UpdateTree(treeCores.Nodes[0]);
        }
        /// <summary>
        /// Updates the Purge State of call Components and Cores in and under the specified TreeView Node.
        /// </summary>
        /// <param name="TNRoot">The TreeNode who's corresponding component's state and children are to be updated.</param>
        private void UpdateTree(TreeNode TNRoot)
        {
            TriStateTreeNode Root = TNRoot as TriStateTreeNode;
            if (Root != null)
            {
                if (Root.Tag != null)
                {
                    CerebrumCore CC = Root.Tag as CerebrumCore;
                    if (CC != null)
                    {
                        CC.PurgeBeforeSynthesis = Root.CheckState;
                    }
                    ComponentCore CompCore = Root.Tag as ComponentCore;
                    if (CompCore != null)
                    {
                        CompCore.PurgeBeforeSynthesis = Root.Checked;
                    }
                }
                foreach (TreeNode TSTN in Root.Nodes)
                {
                    UpdateTree(TSTN);
                }
            }
        }
    }
}
