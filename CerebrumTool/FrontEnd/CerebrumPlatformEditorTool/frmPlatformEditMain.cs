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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CerebrumSharedClasses;
using CerebrumSharedClasses.Platform_Details;

namespace CerebrumPlatformEditorTool
{
    public partial class frmPlatformEditMain : Form
    {
        public frmPlatformEditMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FPGA myFPGA = XBDImporter.Import(@"C:\FalconSVN\Software\Cerebrum\Install\Platforms\ML510_v1_00a\ML510\virtex5\Xilinx_ML510_v2_2_0.xbd");
            
            myFPGA.Resources.Add("Slice Registers", 81920);
            myFPGA.Resources.Add("Slice LUTs", 81920);
            myFPGA.Resources.Add("DSPs", 320);
            myFPGA.Resources.Add("BRAMs", 298);
            myFPGA.Resources.Add("PPC_Blocks", 2);
            myFPGA.Resources.Add("Ethernet_MAC", 6);
            myFPGA.Resources.Add("DDR2_DIMM", 2);
	
            myFPGA.WritePlatformFPGA(        @"C:\FalconSVN\Software\Cerebrum\Install\Platforms\ML510_v1_00a\ML510\virtex5\Xilinx_ML510_v2_2_0.xml");
        }
    }
}
