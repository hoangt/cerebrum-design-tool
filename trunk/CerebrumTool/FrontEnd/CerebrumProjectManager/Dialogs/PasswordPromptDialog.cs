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
 * PasswordPromptDialog.cs
 * Name: Matthew Cotter
 * Date: 01 Oct 2010 
 * Description: Dialog to request login password for a specified username and server for remote logins.
 * History: 
 * >> (29 May 2010) Matthew Cotter: Corrected implementation of OK/Cancel result to allow entry to be properly cancelled without a failed login attempt
 * >> (01 Oct 2010) Matthew Cotter: Implemented dialog to manage and save ethernet communication properties.
 * >> (01 Oct 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple prompting of login credentials.
    /// </summary>
    public partial class PasswordPromptDialog : Form
    {
        private bool bOK = false;
        private bool bCancelled = false;
        private DialogResult _PasswordResult;

        /// <summary>
        /// Default constructor.  Initializes an empty input form.
        /// </summary>
        public PasswordPromptDialog()
        {
            _PasswordResult = DialogResult.Cancel;
            InitializeComponent();
        }

        /// <summary>
        /// The title caption of the password prompt dialog
        /// </summary>
        public string Title
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }
        /// <summary>
        /// Indicates whether the dialog was cancelled
        /// </summary>
        public bool Cancelled
        {
            get
            {
                return bCancelled;
            }
        }
        /// <summary>
        /// The password that was entered by the user
        /// </summary>
        public string Password
        {
            get
            {
                string s = txtPW.Text;
                txtPW.Clear();
                return s;
            }
        }
        /// <summary>
        /// Indicates whether the user confirmed the entry
        /// </summary>
        public bool OK
        {
            get
            {
                return bOK;
            }
        }

        /// <summary>
        /// Clears the password textbox.
        /// </summary>
        public void Clear()
        {
            txtPW.Clear();
        }

        /// <summary>
        /// Overrides OnShown method, forcing focus and selection of entire input textbox when the form is displayed.
        /// </summary>
        /// <param name="e">Generic event arguments</param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            txtPW.Focus();
            txtPW.SelectAll();
        }
        
        /// <summary>
        /// DialogResult indicating whether password entry was OK or Cancelled
        /// </summary>
        public DialogResult PasswordResult
        {
            get
            {
                return _PasswordResult;
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            _PasswordResult = DialogResult.OK;
            bOK = true;
            bCancelled = false;
            this.Hide();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            _PasswordResult = DialogResult.Cancel;
            bOK = false;
            bCancelled = true;
            this.Hide();
        }
    }
}
