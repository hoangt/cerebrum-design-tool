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
 * ProjectPathsDialog.cs
 * Name: Matthew Cotter
 * Date: 21 Sep 2010 
 * Description: Dialog to display, manage and edit project paths.
 * History: 
 * >> ( 3 Mar 2011) Matthew Cotter: Added project path 'PECompileDir' for SAP PE Codelet compilation.
 * >> (21 Sep 2010) Matthew Cotter: Simple implementation to dialog and edit project paths.
 * >> (21 Sep 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FalconPathManager;

namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of project path settings.
    /// </summary>
    public partial class ProjectPathsDialog : Form
    {
        private PathManager _PMan;

        /// <summary>
        /// Constructor.  Creates an empty paths dialog form with associated event handlers and tooltips
        /// </summary>
        /// <param name="PathMan"></param>
        public ProjectPathsDialog(PathManager PathMan)
        {
            InitializeComponent();
            _PMan = PathMan;
            RegisterEventHandlers();
            SetToolTips();
        }
        /// <summary>
        /// Populates textbox fields with their corresponding path values from the path manager
        /// </summary>
        public void PopulateFromPathManager()
        {
            Hashtable h = _PMan.GetAllPaths();
            foreach (Control c in this.Controls)
            {
                PopulatePaths(c, h);
            }
        }
        /// <summary>
        /// Copies values from the form into the corresponding entries of the path manager
        /// </summary>
        public void UpdatePathManager()
        {
            ExtractChildren(this);
        }
        private void PopulatePaths(Control c, Hashtable h)
        {
            foreach (Control ctrl in c.Controls)
            {
                if (ctrl.Tag != null)
                {
                    if (ctrl.GetType() != typeof(Label))
                    {
                        string tag = (string)ctrl.Tag;
                        ctrl.Text = (string)h[tag.ToLower()];
                    }
                }
                PopulatePaths(ctrl, h);
            }
        }
        private void ExtractChildren(Control c)
        {
            foreach (Control ctrl in c.Controls)
            {
                if (ctrl.Tag != null)
                {
                    if (ctrl.GetType() != typeof(Label))
                    {
                        string key = (string)ctrl.Tag;
                        string value = ctrl.Text;
                        _PMan.SetPath(key, value);
                    }
                }
                ExtractChildren(ctrl);
            }
        }

        private void btnLocateCoreSearchPaths_Click(object sender, EventArgs e)
        {
            folderBrowser.Description = "Locate Local F-Core Search Path";
            DialogResult result = folderBrowser.ShowDialog();
            this.Focus();
            if (result == DialogResult.OK)
            {
                txtLocalCores.Text = String.Format("{0};{1}", txtLocalCores.Text, folderBrowser.SelectedPath).Trim(';');
            }
        }
        private void btnLocateLocalEDK_Click(object sender, EventArgs e)
        {
            folderBrowser.Description = "Locate Local Xilinx EDK Directory";
            DialogResult result = folderBrowser.ShowDialog();
            this.Focus();
            if (result == DialogResult.OK)
            {
                txtLocalEDK.Text = folderBrowser.SelectedPath;
            }
        }

        private TextBox _LastFocusedTextBox;
        private void RegisterEventHandlers()
        {
            foreach (Control c in tabCerebrum.Controls)
            {
                if (c.GetType() == typeof(Label))
                {
                    Label l = (Label)c;
                    l.Click += new EventHandler(PathLabel_Clicked);
                }
                else if (c.GetType() == typeof(TextBox))
                {
                    TextBox t = (TextBox)c;
                    t.GotFocus+=new EventHandler(PathText_GotFocus);
                }
            }
        }
        private void PathText_GotFocus(object sender, EventArgs e)
        {
            _LastFocusedTextBox = (TextBox)sender;
        }
        private void PathLabel_Clicked(object sender, EventArgs e)
        {
            if (_LastFocusedTextBox != null)
            {
                _LastFocusedTextBox.SelectedText = "${" + ((string)((Label)sender).Tag) + "}";
            }
        }
        private void SetToolTips()
        {
            toolTips.SetToolTip(lblCerebrumRoot, "The path to the folder where the Cerebrum Framework is located.");
            toolTips.SetToolTip(lblCerebrumBin, "The path to the folder where the Cerebrum Framework executables are located.");
            toolTips.SetToolTip(lblCerebrumPlatforms, "The path to the folder where the Cerebrum Hardware Platform Specifications are located.");
            toolTips.SetToolTip(lblCerebrumCores, "The path to the folder where the Cerebrum F-Cores are located.");

            toolTips.SetToolTip(lblLocalCores, "The list of additional paths that are to be searched for F-Cores when the project is loaded. Each path in this list should be separated by semicolons.");
            toolTips.SetToolTip(lblRemoteSynthesis, "The full path, on the remote synthesis server, where the XPS Projects should be located and synthesis performed.");
            toolTips.SetToolTip(lblRemotePCores, "The full path(s), on the remote synthesis server, where any additional pcore references are located.   If this entry contains multiple paths, each should be delimited by a semicolon.");
            toolTips.SetToolTip(lblRemoteProgramming, "The full path, on the remote programming server, where the FPGAs bitstream/ELF files should be stored during programming.");
            
            toolTips.SetToolTip(lblLocalEDK, "(Optional)  The path to the location of the Xilinx EDK tool, if installed locally.");
            toolTips.SetToolTip(lblRemoteEDK, "The full path, on the remote synthesis server, where the Xilinx EDK tool is installed.");

            toolTips.SetToolTip(lblLinuxKernelSource, "The path to the default Linux kernel source to be compiled for ther FPGA processor(s).   If a processor specifies a different path, that path will override this one.");
            toolTips.SetToolTip(lblDeviceTree, "The path to the Linux device tree, on the remote synthesis server, to be used for the Linux kernel compilation.");
            toolTips.SetToolTip(lblELDKPath, "The path to the ELDK Cross-compilation tools to be used for compiling the Linux kernel as well as Core Server applications and drivers.");
            toolTips.SetToolTip(lblMicroblazeGNU, "The path to the ELDK Cross-compilation tools to be used for compiling the Linux kernel as well as Core Server applications and drivers. (Microblaze Only)");
            
            toolTips.SetToolTip(lblCoreServerSource, "The path to the Cerebrum Core Server Source tree, on the remote compilation server.");
            toolTips.SetToolTip(lblCoreServerNFS, "The path where the compiled core server applications and drivers should be placed.  This path should be the path that is mounted by the Linux booting on the FPGA(s).");
            toolTips.SetToolTip(lblFPGANFS, "This is the path that the FPGA mounts the 'CoreServer NFS Mount' path to locally, at boot up.  This path is used to allow the Linux running on the FPGA to automatically load the appropriate core drivers and applications at start up.");

            //foreach (Control c in tabCerebrum.Controls)
            //{
            //    if (c.GetType() == typeof(Label))
            //    {
            //        Label l = (Label)c;
            //        toolTips.SetToolTip(l, _PMan.GetDescription((string)l.Tag);
            //    }
            //}
        }
    }
}
