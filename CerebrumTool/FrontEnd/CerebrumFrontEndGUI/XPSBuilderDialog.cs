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
 * XPSBuilderDialog.cs
 * Name: Matthew Cotter
 * Date: 22 Oct 2010 
 * Description: This form is the XPS Builder-options dialog displayed before invoking the XPS Builder back end tool.
 * History: 
 * >> (22 Oct 2010) Matthew Cotter: Simple options interface for the XPS Builder tool.
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
    /// Simple form with XPS Builder tool options displayed prior to execution of synthesis tool.
    /// </summary>
    public partial class XPSBuilderDialog : Form
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public XPSBuilderDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Indicates whether the 'Force Clean' checkbox is checked on the dialog form
        /// </summary>
        public bool ForceClean
        {
            get
            {
                return chkBox1.Checked;
            }
            set
            {
                chkBox1.Checked = value;
            }
        }
        /// <summary>
        /// Indicates whether the 'Allow Empty Synthesis' checkbox is checked on the dialog form
        /// </summary>
        public bool AllowEmptySynth
        {
            get
            {
                return chkBox2.Checked;
            }
            set
            {
                chkBox2.Checked = value;
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
