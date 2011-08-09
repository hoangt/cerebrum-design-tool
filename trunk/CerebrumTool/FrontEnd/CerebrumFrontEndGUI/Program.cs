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
using System.Linq;
using System.Windows.Forms;
using CerebrumSharedClasses;

namespace CerebrumFrontEndGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //bool FirstInstance;
                //string CerebrumMutexName = "Global\\PennStateFalconCerebrumDesignTool";
                //System.Threading.Mutex m = new System.Threading.Mutex(true, CerebrumMutexName, out FirstInstance);
                //if (FirstInstance)
                //{
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new frmCerebrumGUIMain());
                //}
                //else
                //{
                //    MessageBox.Show("Only one instance of the Cerebrum Design tool may be running at a time.");
                //}
            }
            catch (Exception ex)
            {
                ErrorReporting.MessageBoxException(ex);
            }
        }
    }
}
