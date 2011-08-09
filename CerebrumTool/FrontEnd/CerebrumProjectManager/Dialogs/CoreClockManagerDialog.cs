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
 * CoreClockManagerDialog.cs
 * Name: Matthew Cotter
 * Date: 21 Sep 2010 
 * Description: Dialog to display, manage and edit core clock connections within the design.
 * History: 
 * >> (21 Dec 2010) Matthew Cotter: Implemented dialog to manage and save core clock signal inputs.
 * >> (21 Dec 2010) Matthew Cotter: Source file created -- Initial version.
 ********************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CerebrumNetronObjects;
using CerebrumSharedClasses;
using FalconClockManager;

namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of properties associated with a core.
    /// </summary>
    public partial class CoreClockManagerDialog : Form
    {
        private CerebrumCore cCore;
        private ProjectManager ProjMan;

        private List<ComboBox> ComponentLists;


        /// <summary>
        /// Default constructor.  Initializes an empty form
        /// </summary>
        public CoreClockManagerDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads clocks from the specified Core object and organizes them into controls for editing.
        /// </summary>
        /// <param name="CC">The CerebrumCore whose clocks are to be edited.</param>
        /// <param name="ProjMan">The ProjectManager for the current project.</param>
        public void LoadClocksFromCoreObject(CerebrumCore CC, ProjectManager ProjMan)
        {
            this.ProjMan = ProjMan;
            this.cCore = CC;
            this.Text = String.Format("Clocks - '{0}' Instance: '{1}'", cCore.CoreType, cCore.CoreInstance);

            tips.RemoveAll();
            tabClocks.TabPages.Clear();
            tabClocks.TabPages.Add("Clocks");
            tabClocks.SuspendLayout();
            ComponentLists = new List<ComboBox>();
            ComponentLists.Clear();

            TabPage pg = tabClocks.TabPages[0];
            int clk_idx = 0;

            foreach (ClockSignal CS in cCore.InputClocks)
            {
                // Create the label and Input Object
                Label ClockLabel = new Label();
                ComboBox ComponentList = new ComboBox();
                ComponentList.DropDownStyle = ComboBoxStyle.DropDownList;
                ComboBox ClockList = new ComboBox();
                ClockList.DropDownStyle = ComboBoxStyle.DropDownList;
                ClockList.DisplayMember = "Description";

                ComponentLists.Add(ComponentList);

                ComponentList.Items.Add("* Auto-Select");
                ComponentList.Tag = ClockLabel;
                ClockLabel.Tag = ClockList;
                if (ComponentList.Items.Count > 0)
                    ComponentList.SelectedIndex = 0;

                if (CS.Name == string.Empty)
                    ClockLabel.Text = CS.Port;
                else
                    ClockLabel.Text = CS.Name;
                ComponentList.SelectedIndexChanged += new EventHandler(ComponentList_SelectedIndexChanged);
                List<CerebrumCore> CompatibleCores = ProjMan.EnumerateCoresWithCompatibleOutputClock(cCore, CS);
                foreach (CerebrumCore CompatCore in CompatibleCores)
                {
                    ComponentList.Items.Add(CompatCore.CoreInstance);
                    // Select the correct component
                    if (String.Compare(CS.SourceComponentInstance, CompatCore.CoreInstance, true) == 0)
                    {
                        ComponentList.SelectedIndex = ComponentList.Items.Count - 1;
                    }
                }
                if (ComponentList.SelectedIndex < 1)
                {
                    ClockList.Enabled = false;
                }
                Rectangle rect = pg.ClientRectangle;
                ClockLabel.Top = (clk_idx * 30) + 10;
                ComponentList.Top = ClockLabel.Top;
                ClockList.Top = ClockLabel.Top;

                ClockLabel.Left = 10;
                ComponentList.Left = 130;
                ClockList.Left = 390;

                ClockLabel.Width = 110;
                ComponentList.Width = 250;
                ClockList.Width = 200;

                ClockLabel.Height = 30;
                ComponentList.Height = 30;
                ClockList.Height = 30;

                pg.Controls.Add(ClockLabel);
                pg.Controls.Add(ComponentList);
                pg.Controls.Add(ClockList);

                clk_idx++;
            }
            tabClocks.ResumeLayout();
        }

        void ComponentList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            Label clkL = (Label) cb.Tag;
            ComboBox clkb = (ComboBox)clkL.Tag;

            clkb.Items.Clear();
            if (cb.SelectedIndex > 0)
            {
                CerebrumCore CC = ProjMan.GetCoreInstance(cb.SelectedItem.ToString());
                if (CC != null)
                {
                    ClockSignal CS = cCore.GetClockByName(clkL.Text, false);
                    if (CS != null)
                    {
                        List<ClockSignal> CompatibleClocks = ProjMan.EnumerateCompatibleOutputClocks(CC, CS);
                        foreach (ClockSignal CompatClock in CompatibleClocks)
                        {
                            clkb.Items.Add(CompatClock.Name);
                            // Select the correct clock
                            if ((String.Compare(CS.SourceCoreInstance, CompatClock.CoreInstance, true) == 0) &&
                                (String.Compare(CS.SourcePort, CompatClock.Port, true) == 0))
                            {
                                clkb.SelectedIndex = clkb.Items.Count - 1;
                            }
                        }
                    }
                }
            }
            if (clkb.Items.Count == 0)
            {
                clkb.Enabled = false;
            }
            else
            {
                clkb.Enabled = true;
                if ((clkb.Items.Count > 0) && (clkb.SelectedIndex < 0))
                    clkb.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Saves current clock assignments, as they appear on the dialog
        /// </summary>
        public void SaveClockAssignments()
        {
            foreach (ClockSignal CS in cCore.InputClocks)
            {
                foreach (ComboBox cb in ComponentLists)
                {
                    Label clkL = (Label)cb.Tag;
                    if (String.Compare(CS.Name, clkL.Text, true) == 0)
                    {
                        ComboBox clkb = (ComboBox)clkL.Tag;
                        if (cb.SelectedIndex > 0)
                        {
                            CerebrumCore sourceClockCore = ProjMan.GetCoreInstance(cb.SelectedItem.ToString());
                            if (sourceClockCore != null)
                            {
                                ClockSignal sourceClock = sourceClockCore.GetClockByName(clkb.SelectedItem.ToString(), true);
                                CS.ConnectToSource(sourceClock);
                            }
                            else
                            {
                                CS.DisconnectFromSource();
                            }
                        }
                        else
                        {
                            CS.DisconnectFromSource();
                        }
                    }
                }
            }
        }
    }
}
