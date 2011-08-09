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
 * CerebrumCoreCreateEditDialog.cs
 * Name: Matthew Cotter
 * Date: 20 Jun 2011
 * Description: Dialog to display, manage and edit CerebrumCore definition files.
 * History: 
 * >> (20 Jun 2011) Matthew Cotter: Created dialog to display, manage and edit CerebrumCore definition files.
 * >> (20 Jun 2011) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CerebrumNetronObjects;

namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of CerebrumCore definition files
    /// </summary>
    public partial class CerebrumCoreCreateEditDialog : Form
    {
        private CerebrumCore CoreUnderEdit;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CerebrumCoreCreateEditDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FileInfo CoreFile = new FileInfo(@"C:\FalconSVN\Software\Cerebrum\Install\Cores\sap_proc_unit_v1_00a\sap_proc_unit_v1_00a.xml");
            CoreUnderEdit = CoreLibrary.LoadCoreDefinition(CoreFile, true, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CoreLibrary.SaveCoreDefinition(new DirectoryInfo(@"C:\FalconSVN\Software\Cerebrum\Install\Cores\sap_proc_unit_v1_00b"), CoreUnderEdit);                
        }
    }
}
