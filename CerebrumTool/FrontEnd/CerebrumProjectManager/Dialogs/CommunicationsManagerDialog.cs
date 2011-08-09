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
 * CommunicationsManagerDialog.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: Dialog to display, manage and edit ethernet communication interface properties.
 * History: 
 * >> (22 Oct 2010) Matthew Cotter: Created dialog to manage and save ethernet communication properties.
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
    /// Dialog used to allow for simple editing of communication interfaces
    /// </summary>
    public partial class CommunicationsManagerDialog : Form
    {
        private ProjectManager ProjMan;

        /// <summary>
        /// Default constructor.  Initializes treeview for interface management.
        /// </summary>
        /// <param name="ProjMan">The Project Manager associated with the loaded project</param>
        public CommunicationsManagerDialog(ProjectManager ProjMan)
        {
            this.ProjMan = ProjMan;
            InitializeComponent();

            grpSelected.SuspendLayout();
            ClearInfoBox();
            grpSelected.ResumeLayout();

            txtMAC.GotFocus += new EventHandler(textBox_GotFocus);

            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            btnClose.Click += new EventHandler(btnCancel_Click);
            
            treeInterfaces.AfterSelect += new TreeViewEventHandler(treeView_SelectedNodeChanged);
        }
        /// <summary>
        /// Load communications interfaces from the project manager into the treeview.
        /// </summary>
        public void LoadInterfaces()
        {
            PopulateTreeView(ProjMan.CommunicationInterfaces);
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
            if (tree.SelectedNode.Tag.GetType() == typeof(CommunicationConfiguration))
            {
                PopulateCommunicationInfo((CommunicationConfiguration)tree.SelectedNode.Tag);
            }
            else
            {
                ClearInfoBox();
            }
        }
        private void PopulateTreeView(List<CommunicationConfiguration> CommCfgs)
        {
            treeInterfaces.Nodes.Clear();
            foreach (CommunicationConfiguration CommCfg in CommCfgs)
            {
                TreeNode ConfigNode = new TreeNode(CommCfg.HardwareInstance);
                ConfigNode.Tag = CommCfg;
                treeInterfaces.Nodes.Add(ConfigNode);
            }
            treeInterfaces.ExpandAll();
        }

        #region InfoBox Management
        private void PopulateCommunicationInfo(CommunicationConfiguration CommCfg)
        {
            grpSelected.SuspendLayout();
            ClearInfoBox();
            PopulateInfoBox(CommCfg);
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
                if (grpSelected.Tag.GetType() == typeof(CommunicationConfiguration))
                {
                    SaveInfoBox((CommunicationConfiguration)grpSelected.Tag);
                }
            }
        }
        private void ClearInfoBox()
        {
            txtInstance.Clear();
            txtFPGA.Clear();
            txtMAC.Clear();
            txtIP.Clear();
            chkDHCP.Checked = false;

            grpSelected.Tag = null;

            btnUpdate.Enabled = false;
            btnUpdate.Text = "Update";
        }

        private void PopulateInfoBox(CommunicationConfiguration CommCfg)
        {
            grpSelected.Tag = CommCfg;

            txtInstance.Text = CommCfg.HardwareInstance;
            txtFPGA.Text = CommCfg.MappedFPGA;
            chkDHCP.Checked = CommCfg.UseDHCP;
            txtMAC.Text = CommCfg.MACAddress;
            txtIP.Text = CommCfg.IPAddress;

            btnUpdate.Enabled = true;
        }
        private void SaveInfoBox(CommunicationConfiguration CommCfg)
        {
            //CommCfg.HardwareInstance = txtInstance.Text;
            //CommCfg.MappedFPGA = txtInstance.Text;
            CommCfg.UseDHCP = chkDHCP.Checked;
            CommCfg.MACAddress = txtMAC.Text;
            CommCfg.IPAddress = txtIP.Text;
        }
        #endregion

    }
}
