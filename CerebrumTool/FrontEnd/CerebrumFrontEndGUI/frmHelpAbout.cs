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
 * frmHelpAbout.cs
 * Name: Matthew Cotter
 * Date: 10 Oct 2010 
 * Description: This form is the Help->About screen for the Cerebrum GUI.
 * History: 
 * >> (10 Oct 2010) Matthew Cotter: Simple support for current GUI version and backend library versions.
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

namespace CerebrumFrontEndGUI
{
    /// <summary>
    /// Simple form with brief details about the Cerebrum Design tool and loaded libraries.
    /// </summary>
    public partial class frmHelpAbout : Form
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public frmHelpAbout()
        {
            InitializeComponent();
            ListReferencedLibraries();
        }

        private void ListReferencedLibraries()
        {
            listLibraries.Items.Clear();

            // Mapping Algorithm
            FalconMappingAlgorithm.FalconMapping_Objects MappingAlgorithm = new FalconMappingAlgorithm.FalconMapping_Objects();
            listLibraries.Items.Add(String.Format("{0}; Version {1}", MappingAlgorithm.FalconComponentName, MappingAlgorithm.FalconComponentVersion));
            
            // XPS Project Builder
            FalconXpsBuilder.XpsBuilder XPSProjectBuilder = new FalconXpsBuilder.XpsBuilder();
            listLibraries.Items.Add(String.Format("{0}; Version {1}", XPSProjectBuilder.FalconComponentName, MappingAlgorithm.FalconComponentVersion));
            
            // System Synthesis
            FalconPlatformSynthesis.FalconSystemSynthesizer Synthesizer = new FalconPlatformSynthesis.FalconSystemSynthesizer();
            listLibraries.Items.Add(String.Format("{0}; Version {1}", Synthesizer.FalconComponentName, MappingAlgorithm.FalconComponentVersion));
            
            // JTAG Programmer
            FalconJTAG_Programmer.JProgrammer JTAGProgrammer = new FalconJTAG_Programmer.JProgrammer();
            listLibraries.Items.Add(String.Format("{0}; Version {1}", JTAGProgrammer.FalconComponentName, MappingAlgorithm.FalconComponentVersion));
            
        }
    }
}
