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
 * ProgrammingManagerDialog.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: Dialog to display, manage and edit programming properties.
 * History: 
 * >> (23 Oct 2010) Matthew Cotter: Added support for callback used to invoke programming tool from this dialog.
 * >> (22 Oct 2010) Matthew Cotter: Simple implementation to dialog and edit programming properties.
 * >> (22 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;

using CerebrumSharedClasses;
using FalconGlobal;
using CerebrumNetronObjects;

namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of programming information associated with FPGAs and Boards.
    /// </summary>
    public partial class ProgrammingManagerDialog : Form
    {
        /// <summary>
        /// Delegate that allows the ProgrammingManagerDialog to invoke the Programming Tool outside the ProjectManager
        /// </summary>
        public StartProgrammerCallback ProgrammerLauncher;
        private ProjectManager ProjMan;

        /// <summary>
        /// Constructor.  Initializes the base programming manager dialog.
        /// </summary>
        /// <param name="ProjMan">The Project Manager associated with the loaded project.</param>
        /// <param name="bEnableProgramButton">Indicates whether the programming button should be enabled.  Edit-Only vs Edit-and-Program mode.</param>
        public ProgrammingManagerDialog(ProjectManager ProjMan, bool bEnableProgramButton)
        {
            this.ProjMan = ProjMan;
            InitializeComponent();

            grpSelected.SuspendLayout();
            ClearInfoBox();
            grpSelected.ResumeLayout();

            txtCablePort.GotFocus += new EventHandler(textBox_GotFocus);

            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            btnClose.Click += new EventHandler(btnCancel_Click);


            btnProgram.Enabled = bEnableProgramButton;
            treeBoards.AfterSelect += new TreeViewEventHandler(treeView_SelectedNodeChanged);
        }
        /// <summary>
        /// Load boards and FPGAs from the project manager into the treeview.
        /// </summary>
        public void LoadBoards()
        {
            PopulateTreeView(ProjMan.ProjectPlatform.Boards);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void treeView_SelectedNodeChanged(object sender, TreeViewEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            if (tree.SelectedNode.Tag.GetType() == typeof(CerebrumPlatformBoard))
            {
                PopulateProgrammingInfo((CerebrumPlatformBoard)tree.SelectedNode.Tag);
            }
            else if (tree.SelectedNode.Tag.GetType() == typeof(CerebrumPlatformFPGA))
            {
                PopulateProgrammingInfo((CerebrumPlatformFPGA)tree.SelectedNode.Tag);
            }
            else
            {
                ClearInfoBox();
            }
        }
        private void PopulateTreeView(List<CerebrumPlatformBoard> Boards)
        {
            treeBoards.Nodes.Clear();
            foreach (CerebrumPlatformBoard CPB in Boards)
            {
                TreeNode BoardNode = new TreeNode(CPB.Name);
                BoardNode.Tag = CPB;
                treeBoards.Nodes.Add(BoardNode);
                foreach (CerebrumPlatformFPGA CPF in CPB.FPGAs)
                {
                    TreeNode FPGANode = new TreeNode(CPF.ID);
                    FPGANode.Tag = CPF;
                    BoardNode.Nodes.Add(FPGANode);
                }
            }
            treeBoards.ExpandAll();
        }

        private void btnProgram_Click(object sender, EventArgs e)
        {
            if (ProgrammerLauncher != null)
            {
                ProgrammerLauncher();
                this.Hide();
            }
            else
            {
                btnProgram.Enabled = false;
            }
        }

        #region InfoBox Management
        private void PopulateProgrammingInfo(CerebrumPlatformBoard Board)
        {
            grpSelected.SuspendLayout();
            ClearInfoBox();
            PopulateInfoBox(Board);
            grpSelected.ResumeLayout();
        }
        private void PopulateProgrammingInfo(CerebrumPlatformFPGA FPGA)
        {
            grpSelected.SuspendLayout();
            ClearInfoBox();
            PopulateInfoBox(FPGA);
            grpSelected.ResumeLayout();
        }

        private void textBox_GotFocus(object sender, EventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
        private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
            {
                e.Handled = true;
            }
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (grpSelected.Tag != null)
            {
                if (grpSelected.Tag.GetType() == typeof(CerebrumPlatformFPGA))
                {
                    SaveInfoBox((CerebrumPlatformFPGA)grpSelected.Tag);
                }
                if (grpSelected.Tag.GetType() == typeof(CerebrumPlatformBoard))
                {
                    SaveInfoBox((CerebrumPlatformBoard)grpSelected.Tag);
                }
            }
        }
        private void ClearInfoBox()
        {
            txtID.Clear();
            lblID.Text = "ID";
            comboCableType.Items.Clear();
            txtCablePort.Clear();

            grpSelected.Tag = null;

            btnUpdate.Enabled = false;
            btnUpdate.Text = "Update";
        }

        private void PopulateInfoBox(CerebrumPlatformBoard Board)
        {
            grpSelected.Tag = Board;
            lblID.Text = "Board ID";
            txtID.Text = Board.ID;
            comboCableType.Items.Clear();
            comboCableType.Items.Add(JTAGCableType.Xilinx_PlatformUSB.ToString());

            #region Find a Common Programming Type
            JTAGCableType CommonType = JTAGCableType.Invalid;
            string CablePort = null;
            foreach (CerebrumPlatformFPGA FPGA in Board.FPGAs)
            {
                if (CablePort == null)
                {
                    CablePort = FPGA.ProgramConfig.CablePort;
                }
                else if (String.Compare(CablePort, FPGA.ProgramConfig.CablePort) != 0)
                {
                    CablePort = string.Empty;
                }
                if (CommonType == JTAGCableType.Invalid)
                {
                    CommonType = FPGA.ProgramConfig.CableType;
                }
                else if (CommonType != FPGA.ProgramConfig.CableType)
                {
                    CommonType = JTAGCableType.None;
                }
            }
            #endregion

            #region Clear Fields without a common entry
            if (CablePort == string.Empty)
            {
                txtCablePort.Clear();
            }
            else
            {
                txtCablePort.Text = CablePort;
            }
            if ((CommonType == JTAGCableType.None) || (CommonType == JTAGCableType.Invalid))
            {
                comboCableType.Items.Clear();
            }
            else
            {
                foreach (object o in comboCableType.Items)
                {
                    if (String.Compare((string)o, CommonType.ToString(), true) == 0)
                    {
                        comboCableType.SelectedItem = o;
                        break;
                    }
                }
            }
            #endregion

            btnUpdate.Enabled = true;
            btnUpdate.Text = "Update ALL FPGAs on this Board";
        }
        private void PopulateInfoBox(CerebrumPlatformFPGA FPGA)
        {
            grpSelected.Tag = FPGA;
            lblID.Text = "FPGA ID";
            txtID.Text = FPGA.ID;

            comboCableType.Items.Clear();
            comboCableType.Items.Add(JTAGCableType.Xilinx_PlatformUSB.ToString());

            foreach (object o in comboCableType.Items)
            {
                if (String.Compare((string)o, FPGA.ProgramConfig.CableType.ToString(), true) == 0)
                {
                    comboCableType.SelectedItem = o;
                    break;
                }
            }
            txtCablePort.Text = FPGA.ProgramConfig.CablePort;

            btnUpdate.Enabled = true;
            btnUpdate.Text = "Update this FPGA";
        }
        private void SaveInfoBox(CerebrumPlatformBoard Board)
        {
            foreach (CerebrumPlatformFPGA FPGA in Board.FPGAs)
            {
                SaveInfoBox(FPGA);
            }
        }
        private void SaveInfoBox(CerebrumPlatformFPGA FPGA)
        {
            if (comboCableType.SelectedItem is JTAGCableType)
            {
                FPGA.ProgramConfig.CableType = (JTAGCableType)(comboCableType.SelectedItem);
            }
            else
            {
                // Default
                FPGA.ProgramConfig.CableType = JTAGCableType.Xilinx_PlatformUSB;
            }
            FPGA.ProgramConfig.CablePort = txtCablePort.Text;
        }
        #endregion
    }
}
