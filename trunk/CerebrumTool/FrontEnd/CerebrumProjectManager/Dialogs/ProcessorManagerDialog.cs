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
 * ProcessorManagerDialog.cs
 * Name: Matthew Cotter
 * Date: 10 Oct 2010 
 * Description: Dialog to display, manage and edit processor properties.
 * History: 
 * >> (22 Oct 2010) Matthew Cotter: Completed and corrected dialog to manage, view and update processor properties.
 * >> (10 Oct 2010) Matthew Cotter: Basic implementation of processor management dialog.
 * >> (10 Oct 2010) Matthew Cotter: Source file created -- Initial version.
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
    /// Dialog used to allow for simple editing of processor information associated with the design.
    /// </summary>
    public partial class ProcessorManagerDialog : Form
    {
        private ProjectManager ProjMan;

        /// <summary>
        /// Constructor.  Initializes the base processor manager dialog.
        /// </summary>
        /// <param name="ProjMan">The Project Manager associated with the loaded project.</param>
        public ProcessorManagerDialog(ProjectManager ProjMan)
        {
            this.ProjMan = ProjMan;
            InitializeComponent();

            grpSelected.SuspendLayout();
            ClearInfoBox();
            grpSelected.ResumeLayout();

            txtDTS.GotFocus += new EventHandler(textBox_GotFocus);
            txtMakeConfig.GotFocus += new EventHandler(textBox_GotFocus);
            txtLinuxSource.GotFocus += new EventHandler(textBox_GotFocus);
            txtCompilerArgs.GotFocus += new EventHandler(textBox_GotFocus);

            txtType.KeyPress += new KeyPressEventHandler(txtPort_KeyPress);
            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            btnClose.Click += new EventHandler(btnCancel_Click);

            treeProcs.AfterSelect += new TreeViewEventHandler(treeView_SelectedNodeChanged);
        }

        /// <summary>
        /// Load design processors from the project manager into the treeview.
        /// </summary>
        public void LoadProcessors()
        {
            PopulateTreeView(ProjMan.Processors);
        }

        private void treeView_SelectedNodeChanged(object sender, TreeViewEventArgs e)
        {
            TreeView tree = (TreeView)sender;
            if (tree.SelectedNode.Tag.GetType() == typeof(CerebrumProcessor))
            {
                PopulateProcessorInfo((CerebrumProcessor)tree.SelectedNode.Tag);
            }
            else
            {
                ClearInfoBox();
            }
        }
        private void PopulateTreeView(List<CerebrumProcessor> Procs)
        {
            treeProcs.Nodes.Clear();
            foreach (CerebrumProcessor CP in Procs)
            {
                TreeNode ProcNode = new TreeNode(CP.Instance);
                ProcNode.Tag = CP;
                treeProcs.Nodes.Add(ProcNode);
            }
            treeProcs.ExpandAll();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        #region InfoBox Management        
        private void PopulateProcessorInfo(CerebrumProcessor Proc)
        {
            grpSelected.SuspendLayout();
            ClearInfoBox();
            PopulateInfoBox(Proc);
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
                if (grpSelected.Tag.GetType() == typeof(CerebrumProcessor))
                {
                    SaveInfoBox((CerebrumProcessor)grpSelected.Tag);
                }
            }
        }
        private void ClearInfoBox()
        {
            txtInstance.Clear();
            txtOS.Clear();
            txtType.Clear();
            comboConsole.Items.Clear();
            txtDTS.Clear();
            txtLinuxSource.Clear();
            txtMakeConfig.Clear();

            grpSelected.Tag = null;

            btnUpdate.Enabled = false;
        }
        private void PopulateInfoBox(CerebrumProcessor Proc)
        {
            grpSelected.Tag = Proc;
            txtInstance.Text = Proc.Instance;
            txtType.Text = Proc.Type.ToString();
            txtTargetFPGA.Text = Proc.FPGA;   
            txtOS.Text = "Linux";   // txtOS.Text = Proc.OS;

            List<string> AvailableConsoles = new List<string>();
            CerebrumCore CC = ProjMan.GetCerebrumCoreContaining(Proc.Instance);
            if (CC != null)
            {
                AvailableConsoles.AddRange(CC.GetComponentCoresOfType("xps_uart16550"));
            }
            comboConsole.Items.Clear();
            foreach (string dev in AvailableConsoles)
            {
                comboConsole.Items.Add(dev);
            }

            foreach (object o in comboConsole.Items)
            {
                if (String.Compare((string)o, Proc.ConsoleDevice, true) == 0)
                {
                    comboConsole.SelectedItem = o;
                    break;
                }
            }

            txtDTS.Text = Proc.DTSFile;
            txtLinuxSource.Text = Proc.LinuxSourcePath;
            txtCompilerArgs.Text = Proc.CompilerArguments;
            txtMakeConfig.Text = Proc.MakeConfig;

            btnUpdate.Enabled = true;
        }
        private void SaveInfoBox(CerebrumProcessor Proc)
        {
            //Proc.Instance = txtInstance.Text;     // Read-only, Instance cannot be changed
            //Proc.Type = Enum.Parse(PlatformProcessorType, txtType.Text);  // Read-Only fixed by instance
            //Proc.OS = "Linux";                    // Read-Only, currently only Linux supported
            //Proc.FPGA = txtTargetFPGA.Text;       // Read-Only, updated during mapping
            Proc.ConsoleDevice = (string)(comboConsole.SelectedItem);
            Proc.DTSFile = txtDTS.Text;
            Proc.LinuxSourcePath = txtLinuxSource.Text;
            Proc.CompilerArguments = txtCompilerArgs.Text;
            Proc.MakeConfig = txtMakeConfig.Text;
        }
        #endregion

    }
}
