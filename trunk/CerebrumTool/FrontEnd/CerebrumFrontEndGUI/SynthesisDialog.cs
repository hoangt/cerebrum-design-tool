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
 * SynthesisDialog.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: This form is the Synthesis-options dialog displayed before invoking the Synthesis back end tool.
 * History: 
 * >> ( 9 May 2010) Matthew Cotter: Added options for synthesis dialog to allow synthesis of a subset of FPGAs from the platform during synthesis.
 * >> (22 Oct 2010) Matthew Cotter: Simple options interface for the Synthesis tool.
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

namespace CerebrumFrontEndGUI
{
    /// <summary>
    /// Simple form with Synthesis tool options displayed prior to execution of synthesis tool.
    /// </summary>
    public partial class SynthesisDialog : Form
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SynthesisDialog()
        {
            InitializeComponent();
            chkSynthesizeHardware.CheckedChanged += new EventHandler(chkSynthesizeHardware_CheckedChanged);
            chkFPGASelect.CheckedChanged += new EventHandler(chkFPGASelect_CheckedChanged);
            chkForceClean.CheckedChanged += new EventHandler(chkForceClean_CheckedChanged);
            chkSelectiveClean.CheckedChanged += new EventHandler(chkSelectiveClean_CheckedChanged);
            chkCompileLinux.CheckedChanged += new EventHandler(chkCompileLinux_CheckedChanged);
        }
        private void ValidateCheckboxState()
        {
            chkForceClean.Enabled = chkSynthesizeHardware.Checked;
            chkFPGASelect.Enabled = chkSynthesizeHardware.Checked;
            chkSelectiveClean.Enabled = chkForceClean.Checked && chkSynthesizeHardware.Checked;
        }
        void chkSynthesizeHardware_CheckedChanged(object sender, EventArgs e)
        {
            ValidateCheckboxState();
        }
        void chkFPGASelect_CheckedChanged(object sender, EventArgs e)
        {
            ValidateCheckboxState();
        }
        void chkForceClean_CheckedChanged(object sender, EventArgs e)
        {
            ValidateCheckboxState();
        }
        void chkSelectiveClean_CheckedChanged(object sender, EventArgs e)
        {
            ValidateCheckboxState();
        }
        void chkCompileLinux_CheckedChanged(object sender, EventArgs e)
        {
            ValidateCheckboxState();
        }


        /// <summary>
        /// Indicates whether the 'Synthesize Hardware' checkbox is checked on the dialog form
        /// </summary>
        public bool SynthesizeHardware
        {
            get
            {
                return chkSynthesizeHardware.Checked;
            }
            set
            {
                chkSynthesizeHardware.Checked = value;
            }
        }
        /// <summary>
        /// Indicates whether the 'Force clean of previous synthesis' checkbox is checked on the dialog form
        /// </summary>
        public bool ForceClean
        {
            get
            {
                return chkForceClean.Checked && SynthesizeHardware;
            }
            set
            {
                chkForceClean.Checked = value && SynthesizeHardware;
            }
        }
        /// <summary>
        /// Indicates whether the 'Select FPGAs to synthesize' checkbox is checked on the dialog form
        /// </summary>
        public bool SelectiveSynthesis
        {
            get
            {
                return chkFPGASelect.Checked && SynthesizeHardware;
            }
            set
            {
                chkFPGASelect.Checked = value && SynthesizeHardware;
            }
        }
        /// <summary>
        /// Indicates whether the 'Select which cores to clean' checkbox is checked on the dialog form
        /// </summary>
        public bool SelectiveClean
        {
            get
            {
                return chkSelectiveClean.Checked && ForceClean;
            }
            set
            {
                chkSelectiveClean.Checked = value && ForceClean;
            }
        }
        /// <summary>
        /// Indicates whether the 'Compile Linux Software' checkbox is checked on the dialog form
        /// </summary>
        public bool CompileSoftware
        {
            get
            {
                return chkCompileLinux.Checked;
            }
            set
            {
                chkCompileLinux.Checked = value;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

    }
}
