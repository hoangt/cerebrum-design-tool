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
 * SynthesisSelectivePlatformDialog.cs
 * Name: Matthew Cotter
 * Date: 13 May 2011 
 * Description: Dialog to allow user to selectively choose which FPGAs from the platform will be synthesized.
 * History: 
 * >> (13 May 2011) Matthew Cotter: Simple checked Tree View style hierarchical list of FPGAs.
 * >> (13 May 2011) Matthew Cotter: Source file created -- Initial version.
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
    public partial class SynthesisSelectivePlatformDialog : Form
    {

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SynthesisSelectivePlatformDialog()
        {
            InitializeComponent();
        }

        private void SynthesisSelectivePlatformDialog_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Clears the TreeView Control and populates it with the provided list of FPGAs.
        /// </summary>
        /// <param name="FPGAs">The list of FPGAs available for synthesis.</param>
        public void PopulateTreeView(ref Dictionary<string, bool> FPGAs)
        {
            TriStateTreeNode SystemNode = new TriStateTreeNode("Hardware System");
            SystemNode.CheckboxVisible = true;
            SystemNode.IsContainer = true;
            SystemNode.Checked = true;

            foreach (KeyValuePair<string, bool> FPGAPair in FPGAs)
            {
                TriStateTreeNode FPGANode = new TriStateTreeNode(FPGAPair.Key);
                FPGANode.CheckboxVisible = true;
                FPGANode.IsContainer = false;
                SystemNode.Nodes.Add(FPGANode);
                
                FPGANode.Checked = FPGAPair.Value;
            }

            treeFPGAs.BeginUpdate();
            treeFPGAs.Nodes.Clear();
            treeFPGAs.Nodes.Add(SystemNode);
            treeFPGAs.ExpandAll();
            treeFPGAs.EndUpdate();
        }

        /// <summary>
        /// Updates the Synthesis State of all FPGAs in the TreeView Node.
        /// </summary>
        /// <param name="FPGAs">The list of FPGAs available for synthesis.</param>
        public void UpdateStatus(ref Dictionary<string, bool> FPGAs)
        {
            foreach (TreeNode TN in treeFPGAs.Nodes[0].Nodes)
            {
                TriStateTreeNode TSTN = TN as TriStateTreeNode;
                if (TSTN != null)
                {
                    if (FPGAs.ContainsKey(TSTN.Text))
                    {
                        FPGAs[TSTN.Text] = TSTN.Checked;
                    }
                }
            }
        }
    }
}
