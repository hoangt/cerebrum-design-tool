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
 * frmCerebrumGUIMain.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: This is the primary form for the Cerebrum GUI.  All toolbars, menus, and controls will be hosted on this form.
 * History: 
 * >> (29 May 2010) Matthew Cotter: Added exception handling to Backend File Browser interface.
 * >> (26 May 2010) Matthew Cotter: Corrected mapping toolbar to correctly enable the confirm mapping button if loaded while all components are mapped.
 * >> (25 May 2010) Matthew Cotter: In Project Netron.Diagramming.Core: Corrected placement of new components that are dragged and dropped onto the design layout.
 * >> (18 May 2010) Matthew Cotter: Added support for redirecting FPGA-specific formatted output to individual tabs from the Synthesis output.
 * >> (17 May 2010) Matthew Cotter: Implemented initial support for back end GUI interface for output and report nagivation.
 * >> ( 7 Apr 2010) Matthew Cotter: Added access to Mapping I/O Weighting functionality.
 * >> (22 Mar 2011) Matthew Cotter: Implemented Most Recently Used (MRU) Projects List.
 * >> (18 Feb 2011) Matthew Cotter: Implemented invocation of simple project properties dialog.
 * >> (25 Jan 2011) Matthew Cotter: Implemented initial support for auto-saving intermediate mapping state.
 * >> (24 Oct 2010) Matthew Cotter: Implemented programming tool interface to GUI.
 * >> (23 Oct 2010) Matthew Cotter: Migrated Back End tool support to multithreaded system to prevent GUI freeze during execution.
 * >> (22 Oct 2010) Matthew Cotter: Added Error Log support to GUI.
 *                                  Reorganized toolbar management handler(s).
 * >> ( 1 Oct 2010) Matthew Cotter: Initial support for Back End integration at a single-threaded level.
 * >> (30 Sep 2010) Matthew Cotter: Reorganized and streamlined menu/toolbar event handlers.
 *                                  Added missing try/catch blocks to fortify code against crashing exceptions.
 *                                  Corrected bugs in messaging tabs interface.
 * >> (27 Sep 2010) Matthew Cotter: Added mapping toolbar and tooltips.
 * >> (24 Sep 2010) Matthew Cotter: Implemented MessageEventController to facilitate inter-library messaging for assorted message types.
 *                                  Added support for Project Manager Open/Close events, and mapping via listview on Mapping GUI.
 * >> (23 Sep 2010) Matthew Cotter: Added Project Paths dialog to main GUI menus
 * >> (21 Sep 2010) Matthew Cotter: Implemented support for Mapping GUI Events, Messaging and Error notifications.
 * >>                               Reorganized menu and toolbar enable/disable functions
 * >> (17 Sep 2010) Matthew Cotter: Added limited support of listview control to mapping GUI
 * >> (15 Sep 2010) Matthew Cotter: Added initial support for mapping GUI and Design-Stage specific menu states and toolbars.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
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
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;

using CerebrumSharedClasses;
using CerebrumProjectManager;
using CerebrumNetronObjects;
using CerebrumMappingControls;
using System.Threading;

namespace CerebrumFrontEndGUI
{
    /// <summary>
    /// Primary form for the Cerebrum Front End User Interface
    /// </summary>
    public partial class frmCerebrumGUIMain : Form
    {
        #region MessageEventController
        private MessageEventController Messages = null;

        private void RegisterMessageEventController()
        {
            try
            {
                Messages = new MessageEventController();
                Messages.OnClearMessages += new MessageEventController.MessageHandler(Messages_OnClearMessages);
                Messages.OnInfoMessage += new MessageEventController.MessageHandler(Messages_OnInfoMessage);
                Messages.OnWarningMessage += new MessageEventController.MessageHandler(Messages_OnWarningMessage);
                Messages.OnErrorMessage += new MessageEventController.MessageHandler(Messages_OnErrorMessage);
                Messages.OnConsoleMessage += new MessageEventController.MessageHandler(Messages_OnConsoleMessage);
                Messages.OnStatusMessage += new MessageEventController.MessageHandler(Messages_OnStatusMessage);

                projectPanel.AttachMessageController(Messages);
                mappingCanvas.AttachMessageController(Messages);
                ProjMan.AttachMessageController(Messages);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }

        private delegate void StreamWriterWriter(StreamWriter sw, string Message);
        private delegate void TextBoxClearer(TextBox tb);
        private delegate void TextBoxWriter(TextBox tb, string Message);
        private delegate void StatusBarWriter(StatusStrip ss, string Message);
        private delegate void ListViewClearer(ListView lv);
        private delegate void ListViewWriter(ListView lv, string MsgID, string Message, string MsgSource, string IconText);
        private delegate void TabTitleChanger(TabPage tab, string Title);

        void WriteStreamWriter(StreamWriter sw, string Message)
        {
            if (this.InvokeRequired)
            {
                if (sw != null)
                {
                    sw.WriteLine(Message);
                }
            }
        }
        void ClearTextBox(TextBox tb)
        {
            if (tb != null)
            {
                if (tb.InvokeRequired)
                {
                    tb.Invoke(new TextBoxClearer(ClearTextBox), tb);
                }
                else
                {
                    tb.Clear();
                }
            }
        }
        void WriteToTextBox(TextBox tb, string Message)
        {
            if (tb != null)
            {
                if (tb.InvokeRequired)
                {
                    tb.Invoke(new TextBoxWriter(WriteToTextBox), tb, Message);
                }
                else
                {
                    tb.AppendText(Message.Trim(new char[] { '\n', '\r' }) + Environment.NewLine);
                }
            }
        }
        void ClearListView(ListView lv)
        {
            if (lv != null)
            {
                if (lv.InvokeRequired)
                {
                    lv.Invoke(new ListViewClearer(ClearListView), lv);
                }
                else
                {
                    lv.Items.Clear();
                }
            }
        }
        void WriteListView(ListView lv, string MsgID, string Message, string MsgSource, string IconText)
        {
            if (lv != null)
            {
                if (lv.InvokeRequired)
                {
                    lv.Invoke(new ListViewWriter(WriteListView), lv, MsgID, Message, MsgSource);
                }
                else
                {
                    if (String.Compare(IconText, string.Empty) == 0)
                    {
                        lv.Items.Add(new ListViewItem(new string[] { string.Empty, MsgID, Message, MsgSource }, IconText));
                    }
                    else
                    {
                        lv.Items.Add(new ListViewItem(new string[] { string.Empty, MsgID, Message, MsgSource }));
                    }
                }
            }
        }
        void ChangeTitleTab(TabPage tab, string Title)
        {
            if (tab != null)
            {
                if (tab.InvokeRequired)
                {
                    tab.Invoke(new TabTitleChanger(ChangeTitleTab), tab, Title);
                }
                else
                {
                    tab.Text = Title;
                }
            }
        }
        void WriteStatusBar(StatusStrip ss, string Message)
        {
            if (ss != null)
            {
                if (ss.InvokeRequired)
                {
                    ss.Invoke(new StatusBarWriter(WriteStatusBar), ss, Message);
                }
                else
                {
                    ss.Text = Message;
                }
            }
        }

        void Messages_OnClearMessages(string MsgID, string Message, string MsgSource)
        {
            try
            {
                ClearTextBox(tbConsole);

                ClearListView(lvAllMessages);
                ChangeTitleTab(tabMessages, "All Messages (0)");

                ClearListView(lvInfo);
                ChangeTitleTab(tabInfo, "Information (0)");

                ClearListView(lvWarnings);
                ChangeTitleTab(tabWarnings, "Warnings (0)");

                ClearListView(lvErrors);
                ChangeTitleTab(tabErrors, "Errors (0)");
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void Messages_OnConsoleMessage(string MsgID, string Message, string MsgSource)
        {
            try
            {
                WriteToTextBox(tbConsole, String.Format("{0} -- {1}: {2} [{3}]{4}",
                    DateTime.Now.ToShortTimeString(),
                    MsgID,
                    Message,
                    MsgSource,
                    Environment.NewLine));

                WriteLog(String.Format("Console Message: {0}", Message));
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void Messages_OnInfoMessage(string MsgID, string Message, string MsgSource)
        {
            try
            {
                WriteListView(lvAllMessages, MsgID, Message, MsgSource, "icoInfo");
                WriteListView(lvInfo, MsgID, Message, MsgSource, "icoInfo");
                ChangeTitleTab(tabMessages, String.Format("{0} ({1})", "All Messages", lvAllMessages.Items.Count));
                ChangeTitleTab(tabInfo, String.Format("{0} ({1})", "Information", lvInfo.Items.Count));

                WriteLog(String.Format("Information Message: {0}", Message));
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void Messages_OnWarningMessage(string MsgID, string Message, string MsgSource)
        {
            try
            {
                WriteListView(lvAllMessages, MsgID, Message, MsgSource, "icoWarning");
                WriteListView(lvWarnings, MsgID, Message, MsgSource, "icoWarning");
                ChangeTitleTab(tabMessages, String.Format("{0} ({1})", "All Messages", lvAllMessages.Items.Count));
                ChangeTitleTab(tabWarnings, String.Format("{0} ({1})", "Warnings", lvWarnings.Items.Count));
                WriteLog(String.Format("Warning Message: {0}", Message));
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void Messages_OnErrorMessage(string MsgID, string Message, string MsgSource)
        {
            try
            {
                WriteListView(lvAllMessages, MsgID, Message, MsgSource, "icoError");
                WriteListView(lvErrors, MsgID, Message, MsgSource, "icoError");
                ChangeTitleTab(tabMessages, String.Format("{0} ({1})", "All Messages", lvAllMessages.Items.Count));
                ChangeTitleTab(tabErrors, String.Format("{0} ({1})", "Error", lvErrors.Items.Count));

                WriteLog(String.Format("ERROR Message: {0}", Message));
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void Messages_OnStatusMessage(string MsgID, string Message, string MsgSource)
        {
            WriteStatusBar(this.status, Message);
            WriteLog(String.Format("Status Message: {0}", Message));
        }
        #endregion

        private const string MAIN_FORM_TITLE = "Cerebrum Design Framework";

        #region Form Initialization

        /// <summary>
        /// Default constructor.  Initializes the form, menu/toolbar handlers, project manager, tooltips, mapping interface, and back end tool interfaces
        /// </summary>
        public frmCerebrumGUIMain()
        {
            InitializeComponent();
            InitializeForm();
        }
        private void InitializeForm()
        {
            try
            {
                // Create the project manager
                ProjMan = new ProjectManager(projectPanel);
                ProjMan.ProjectOpened += new ProjectManager.ProjectStatusChangedHandler(ProjMan_ProjectOpened);
                ProjMan.ProjectClosed += new ProjectManager.ProjectStatusChangedHandler(ProjMan_ProjectClosed);

                // Create custom event handlers
                designTabs.SelectedIndexChanged += new EventHandler(tabs_SelectedIndexChanged);
                RegisterMenuHandlers();
                RegisterToolbarHandlers();
                RegisterBackEndHandlers();

                // Mapping Canvas Handlers
                RegisterMappingCanvasEventHandlers();

                // Create tooltips on required controls
                RegisterToolTips();

                // Place ToolStrips
                InitializeToolStripLocations();

                // Create and attach the Message Center/Controller to objects that use it
                RegisterMessageEventController();

                UpdateMRUMenu();

                XPSCompleteDelegate = new ProjectManager.ToolCompleteDelegate(XPSComplete);
                SynthCompleteDelegate = new ProjectManager.ToolCompleteDelegate(SynthesisComplete);
                JProgCompleteDelegate = new ProjectManager.ToolCompleteDelegate(JProgrammerComplete);

                XPSMessageDelegate = new ProjectManager.ToolMessageDelegate(WriteXPSBuilderLog);
                SynthMessageDelegate = new ProjectManager.ToolMessageDelegate(WriteSynthesisLog);
                JProgMessageDelegate = new ProjectManager.ToolMessageDelegate(WriteJProgrammerLog);

                OpenLog();
                this.Text = String.Format("{0}", MAIN_FORM_TITLE);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void UpdateMRUMenu()
        {
            List<string> MRU = ProjMan.GetMRUList();
            menuItemFileRecent.DropDownItems.Clear();

            if (ProjMan.MRUAvailable)
            {
                for (int i = 0; i < MRU.Count; i++)
                {
                    FileInfo ProjectFile = new FileInfo(MRU[i]);
                    ToolStripMenuItem menuRecentItem = new ToolStripMenuItem();
                    string itemText = ProjectFile.Directory.Name;
                    menuRecentItem.Text = String.Format("&{0} - {1}", i + 1, itemText);
                    menuRecentItem.Tag = ProjectFile.FullName;
                    menuRecentItem.Click += new EventHandler(FileRecentItemOpen);
                    menuItemFileRecent.DropDownItems.Add(menuRecentItem);
                }
            }
            else
            {
                ToolStripMenuItem menuMRUItem = new ToolStripMenuItem();
                menuMRUItem.Text = "Most Recently Used Projects Unavailable";
                menuMRUItem.ToolTipText = "Please run Cerebrum as Administrator to enable access to the MRU list.";
                menuMRUItem.Enabled = false;
                menuItemFileRecent.DropDownItems.Add(menuMRUItem);
            }
            menuItemFileRecent.DropDownItems.Add(menuItemFileRecentSeparator1);
            menuItemFileRecent.DropDownItems.Add(menuItemFileRecentClearList);
            menuItemFileRecentClearList.Enabled = ProjMan.MRUAvailable;
        }

        /// <summary>
        /// Override that performs validation of menu and toolbar states when the form is redrawn.
        /// </summary>
        /// <param name="e">Generic event arguments</param>
        protected override void OnShown(EventArgs e)
        {
            try
            {
                base.OnShown(e);

                // Validate Menu and ToolBar States
                ValidateMenuAndToolbarState();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        /// <summary>
        /// Override that confirms project saving and exiting when something attempts to close the form.
        /// </summary>
        /// <param name="e">Generic event arguments</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = !ExitCerebrum();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
            base.OnClosing(e);
        }

        private void RegisterToolTips()
        {
            try
            {
                //tips.SetToolTip(toolBtnMappingReset, "Clear ALL currently set component mappings.");
                //tips.SetToolTip(toolBtnMappingComplete, "Finishes component mapping automatically, optimizing for resource utilization and communication");
                //tips.SetToolTip(toolBtnMappingConfirm, "Accept and save the component mapping, as-shown.");
                //tips.SetToolTip(toolBtnMappingModeQuery, "Switches the Mapping Interface to query information on selected objects.");
                //tips.SetToolTip(toolBtnMappingModeManual, "Switches the Mapping Interface to allow for manual mapping/placement of selected objects.");
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        
        #endregion

        #region Project Manager & Events

        void ProjMan_ProjectOpened()
        {
            ToggleInterface(true);
            ShowHideMenus();
            ValidateMenuAndToolbarState();
            if (ProjMan != null)
            {
                if (ProjMan.ProjectLoaded)
                {
                    this.Text = String.Format("[{0}] - ({1})", ProjMan.ProjectDirectory.Name, MAIN_FORM_TITLE);
                }
            }
            RemoveFPGATabs();
        }
        void ProjMan_ProjectClosed()
        {
            KillXPS();
            KillSynthesis();
            KillProgramming();
            ToggleInterface(false);
            ShowHideMenus();
            ValidateMenuAndToolbarState();
            if (mappingCanvas.MapObjects != null)
            {
                mappingCanvas.MapObjects.Reset();
            }
            this.Text = String.Format("{0}", MAIN_FORM_TITLE);
            tbToolLiveLog.Clear();
            txtFileFilter.Clear();
            RemoveFPGATabs();
        }
        private ProjectManager ProjMan;
        #endregion

        #region Menu and Toolbar Management (Handles Enabled and/or Visible Properties on State Changes)
        void InitializeToolStripLocations()
        {
            this.SuspendLayout();
            try
            {
                toolStripProject.Top = 0;
                toolStripDesign.Top = 500;
                toolStripMapping.Top = 1000;
                toolStripSynthesis.Top = 1500;
                toolStripHelp.Top = 2000;

                toolStripProject.Left = 1000;
                toolStripDesign.Left = 2000;
                toolStripMapping.Left = 3000;
                toolStripSynthesis.Left = 4000;
                toolStripHelp.Left = 5000;

                int TSDelta = 10;
                toolStripProject.Left = 1;
                toolStripDesign.Left = toolStripProject.Left + toolStripProject.Width + TSDelta;
                toolStripMapping.Left = toolStripDesign.Left + toolStripDesign.Width + TSDelta;
                toolStripSynthesis.Left = toolStripMapping.Left + toolStripMapping.Width + TSDelta;
                toolStripHelp.Left = toolStripSynthesis.Left + toolStripSynthesis.Width + TSDelta;

                toolStripProject.Top = 1;
                toolStripDesign.Top = 1;
                toolStripMapping.Top = 1;
                toolStripSynthesis.Top = 1;
                toolStripHelp.Top = 1;
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
            this.ResumeLayout();
        }

        void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ProjMan.RefreshCoreConfigs();
                ShowHideMenus();
                ValidateMenuAndToolbarState();
                RefreshProjectFilesView(txtFileFilter.Text);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }

        private void ValidateMenuState()
        {
            try
            {
                #region File Menu
                menuFile.Enabled = true;
                menuItemFileNew.Enabled = menuFile.Enabled;
                menuItemFileNewWizard.Enabled = menuFile.Enabled;
                menuItemFileNewEmpty.Enabled = false;
                menuItemFileOpen.Enabled = menuFile.Enabled;
                menuItemFileSaveCopy.Visible = false && ProjMan.ProjectLoaded;
                menuItemFileSaveCopy.Enabled = false && ProjMan.ProjectLoaded;
                menuItemFileSave.Enabled = ProjMan.ProjectLoaded;
                menuItemFileRecent.Enabled = true;
                menuItemFileRecentClearList.Enabled = true;
                menuItemFilePrint.Visible = false && ProjMan.ProjectLoaded;
                menuItemFilePrint.Enabled = false && ProjMan.ProjectLoaded;
                menuItemFileExit.Enabled = menuFile.Enabled;
                #endregion

                #region Edit Menu
                menuEdit.Enabled = true;
                menuItemEditCopy.Enabled = ProjMan.ProjectLoaded;
                menuItemEditCut.Enabled = ProjMan.ProjectLoaded;
                menuItemEditPaste.Enabled = ProjMan.ProjectLoaded;
                menuItemEditPreferences.Enabled = false && menuEdit.Enabled;
                #endregion

                #region View Menu
                menuView.Enabled = true;
                menuItemViewToolbars.Enabled = menuView.Enabled;
                menuItemViewToolbarsProject.Enabled = menuView.Enabled;
                menuItemViewToolbarsDesign.Enabled = menuView.Enabled;
                menuItemViewToolbarsMapping.Enabled = menuView.Enabled;
                menuItemViewToolbarsSynthesis.Enabled = menuView.Enabled;
                menuItemViewToolbarsHelp.Enabled = menuView.Enabled;
                #endregion

                #region Project Menu
                menuProject.Enabled = ProjMan.ProjectLoaded;
                menuItemProjectPathSettings.Enabled = ProjMan.ProjectLoaded;
                menuItemProjectEditServerLists.Enabled = ProjMan.ProjectLoaded;
                menuItemProjectProperties.Enabled = ProjMan.ProjectLoaded;
                #endregion

                #region Design Menu
                menuDesign.Enabled = designTabs.Visible && (String.Compare(designTabs.SelectedTab.Text, "Design", true) == 0) && ProjMan.ProjectLoaded;
                menuItemDesignConfigureProcessors.Enabled = ProjMan.ProjectLoaded;
                menuItemDesignConfigureCommunications.Enabled = ProjMan.ProjectLoaded;
                menuItemDesignConfigureProgramming.Enabled = ProjMan.ProjectLoaded;
                #endregion

                #region Mapping Menu
                menuMapping.Enabled = designTabs.Visible && (String.Compare(designTabs.SelectedTab.Text, "Mapping", true) == 0) && ProjMan.ProjectLoaded;
                menuItemMappingReset.Enabled = menuMapping.Enabled;
                menuItemMappingComplete.Enabled = menuMapping.Enabled;
                menuItemMappingConfirm.Enabled = menuMapping.Enabled && mappingCanvas.AllComponentsMapped();
                menuItemMappingSavePreset.Enabled = menuMapping.Enabled;
                menuItemMappingLoadPreset.Enabled = menuMapping.Enabled;
                menuItemMappingMode.Enabled = menuMapping.Enabled;
                menuItemMappingModeManual.Enabled = menuMapping.Enabled && (mappingCanvas.UIMode == MappingCanvasControl.InterfaceMode.GetInformation);
                menuItemMappingModeQuery.Enabled = menuMapping.Enabled && (mappingCanvas.UIMode == MappingCanvasControl.InterfaceMode.ManualMapping);
                #endregion

                #region Synthesis Menu
                menuSynthesis.Enabled = designTabs.Visible && (String.Compare(designTabs.SelectedTab.Text, "Synthesis", true) == 0) && ProjMan.ProjectLoaded;
                menuItemSynthesisBuildXPS.Enabled = menuSynthesis.Enabled && ProjMan.ReadyForXPSBuild();
                menuItemSynthesisSynthesize.Enabled = menuSynthesis.Enabled && ProjMan.ReadyForSynthesis();
                menuItemSynthesisProgram.Enabled = menuSynthesis.Enabled && ProjMan.ReadyForProgramming();
                menuItemSynthesisStartConfigServer.Enabled = menuSynthesis.Enabled && false;
                #endregion

                #region Windows Menu
                #endregion

                #region Help Menu
                menuHelp.Enabled = true;
                menuItemHelpHelp.Enabled = true;
                menuItemHelpAbout.Enabled = true;
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void ValidateToolbarState()
        {
            // Toolbar buttons should be disabled if the corresponding menu item (if one exists) is disabled
            try
            {
                #region Project Toolbar
                toolStripProject.Enabled = true;
                toolBtnFileNewWizard.Enabled = menuItemFileNewWizard.Enabled;
                //toolBtnFileNewEmpty.Enabled = menuItemFileNewEmpty.Enabled;
                toolBtnFileOpen.Enabled = menuItemFileOpen.Enabled;
                toolBtnFileSaveCopy.Enabled = menuItemFileSaveCopy.Enabled;
                toolBtnFileSave.Enabled = menuItemFileSave.Enabled;
                //toolBtnFileRecentClearList.Enabled = menuItemFileRecentClearList.Enabled;
                //toolBtnFilePrint.Enabled = menuItemFilePrint.Enabled;
                //toolBtnFileExit.Enabled = menuItemFileExit.Enabled;
                toolBtnProjectPathSettings.Enabled = menuItemProjectPathSettings.Enabled;
                toolBtnProjectEditServerLists.Enabled = menuItemProjectEditServerLists.Enabled;
                toolBtnProjectProperties.Enabled = menuItemProjectProperties.Enabled;
                toolBtnEditPreferences.Enabled = menuItemEditPreferences.Enabled;
                #endregion

                #region Edit Toolbar
                //toolStripEdit.Enabled = true;
                //toolBtnEditCopy.Enabled = menuItemEditCopy.Enabled;
                //toolBtnEditCut.Enabled = menuItemEditCut.Enabled;
                //toolBtnEditPaste.Enabled = menuItemEditPaste.Enabled;
                #endregion

                #region View Toolbar
                //toolStripView.Enabled = menuItemViewToolbars.Enabled;
                //toolBtnViewToolbarsProject.Enabled = menuItemViewToolbarsProject.Enabled;
                //toolBtnViewToolbarsDesign.Enabled = menuItemViewToolbarsDesign.Enabled;
                //toolBtnViewToolbarsMapping.Enabled = menuItemViewToolbarsMapping.Enabled;
                //toolBtnViewToolbarsSynthesis.Enabled = menuItemViewToolbarsSynthesis.Enabled;
                //toolBtnViewToolbarsHelp.Enabled = menuItemViewToolbarsHelp.Enabled;

                // Correct Checked State
                menuItemViewToolbarsProject.Checked = toolStripProject.Visible;
                menuItemViewToolbarsDesign.Checked = toolStripDesign.Visible;
                menuItemViewToolbarsMapping.Checked = toolStripMapping.Visible;
                menuItemViewToolbarsSynthesis.Checked = toolStripSynthesis.Visible;
                menuItemViewToolbarsHelp.Checked = toolStripHelp.Visible;
                #endregion

                #region Design Toolbar
                toolStripDesign.Enabled = menuDesign.Visible && (String.Compare(designTabs.SelectedTab.Text, "Design", true) == 0) && ProjMan.ProjectLoaded;
                toolBtnDesignConfigureProcessors.Enabled = menuItemDesignConfigureProcessors.Enabled;
                toolBtnDesignConfigureCommunications.Enabled = menuItemDesignConfigureCommunications.Enabled;
                toolBtnDesignConfigureProgramming.Enabled = menuItemDesignConfigureProgramming.Enabled;
                #endregion

                #region Mapping Toolbar
                toolStripMapping.Enabled = menuMapping.Visible && (String.Compare(designTabs.SelectedTab.Text, "Mapping", true) == 0) && ProjMan.ProjectLoaded;
                toolBtnMappingReset.Enabled = menuItemMappingReset.Enabled;
                toolBtnMappingComplete.Enabled = menuItemMappingComplete.Enabled;
                toolBtnMappingConfirm.Enabled = menuItemMappingConfirm.Enabled;
                toolBtnMappingSavePreset.Enabled = menuItemMappingSavePreset.Enabled;
                toolBtnMappingLoadPreset.Enabled = menuItemMappingLoadPreset.Enabled;
                toolBtnMappingModeManual.Enabled = menuItemMappingModeManual.Enabled && (mappingCanvas.UIMode == MappingCanvasControl.InterfaceMode.GetInformation);
                toolBtnMappingModeQuery.Enabled = menuItemMappingModeQuery.Enabled && (mappingCanvas.UIMode == MappingCanvasControl.InterfaceMode.ManualMapping);
                #endregion

                #region Synthesis Toolbar
                toolStripSynthesis.Enabled = menuSynthesis.Visible && (String.Compare(designTabs.SelectedTab.Text, "Synthesis", true) == 0) && ProjMan.ProjectLoaded;
                toolBtnSynthesisBuildXPS.Enabled = menuItemSynthesisBuildXPS.Enabled;
                toolBtnSynthesisSynthesize.Enabled = menuItemSynthesisSynthesize.Enabled;
                toolBtnSynthesisProgram.Enabled = menuItemSynthesisProgram.Enabled;
                toolBtnSynthesisStartConfigServer.Enabled = menuItemSynthesisStartConfigServer.Enabled;
                #endregion

                #region Help Toolbar
                toolStripHelp.Visible = true;
                toolBtnHelpHelp.Enabled = true;
                toolBtnHelpAbout.Enabled = true;
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void ValidateMenuAndToolbarState()
        {
            try
            {
                ShowHideMenus();
                ValidateMenuState();
                ValidateToolbarState();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }

        private void ToggleInterface(bool bEnabled)
        {
            try
            {
                designTabs.Visible = bEnabled;
                designTabs.Enabled = bEnabled;
                designTabs.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void ToggleDesignInterface(bool bEnabled)
        {
            try
            {
                menuDesign.Visible = bEnabled;
                menuDesign.Enabled = bEnabled;
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void ToggleMappingInterface(bool bEnabled)
        {
            try
            {
                if (!bEnabled)
                {
                    if (ProjMan != null)
                        if (ProjMan.ProjectLoaded)
                        {
                            mappingCanvas.SaveState();
                            ProjMan.RefreshCoreConfigs();
                        }
                }
                menuMapping.Visible = bEnabled;
                menuMapping.Enabled = bEnabled;
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void ToggleSynthesisInterface(bool bEnabled)
        {
            try
            {
                menuSynthesis.Visible = bEnabled;
                menuSynthesis.Enabled = bEnabled;
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void ShowHideMenus()
        {
            try
            {
                designTabs.Visible = ProjMan.ProjectLoaded;

                if (!ProjMan.ProjectLoaded)
                {
                    ToggleInterface(false);
                    ToggleDesignInterface(false);
                    ToggleMappingInterface(false);
                    ToggleSynthesisInterface(false);
                }
                else
                {
                    TabPage pg = designTabs.SelectedTab;
                    if ((string)pg.Tag == "design")
                    {
                        ToggleDesignInterface(true);
                        ToggleMappingInterface(false);
                        ToggleSynthesisInterface(false);
                    }
                    else if ((string)pg.Tag == "mapping")
                    {
                        ToggleDesignInterface(false);
                        ToggleMappingInterface(true);
                        ToggleSynthesisInterface(false);

                        if (ProjMan.ProjectLoaded)
                        {
                            ActivateMappingUI();
                        }
                    }
                    else if ((string)pg.Tag == "synthesis")
                    {
                        ToggleDesignInterface(false);
                        ToggleMappingInterface(false);
                        ToggleSynthesisInterface(true);
                    }
                    else
                    {
                        ToggleDesignInterface(false);
                        ToggleMappingInterface(false);
                        ToggleSynthesisInterface(false);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }

        #endregion

        #region Menu/Toolbar Event Handlers

        private void RegisterMenuHandlers()
        {
            try
            {
                // File Menu
                // File->New Project Menu
                menuItemFileNewWizard.Click += new EventHandler(FileNewProjectWizard);
                //menuItemFileNewEmpty.Click += new EventHandler(FileNewProjectWizard);
                menuItemFileOpen.Click += new EventHandler(FileOpenProject);
                menuItemFileSave.Click += new EventHandler(FileSaveProject);
                menuItemFileSaveCopy.Click += new EventHandler(FileSaveProjectCopy);
                menuItemFileClose.Click += new EventHandler(FileCloseProject);
                // ------
                menuItemFileRecent.MouseEnter += new EventHandler(FileRecentMouseEnter);
                menuItemFileRecentClearList.Click += new EventHandler(FileRecentClearList);
                menuItemFilePrint.Click += new EventHandler(FilePrint);
                menuItemFileExit.Click += new EventHandler(FileExit);

                // Edit Menu
                menuItemEditCopy.Click += new EventHandler(EditCopy);
                menuItemEditCut.Click += new EventHandler(EditCut);
                menuItemEditPaste.Click += new EventHandler(EditPaste);
                // ------
                menuItemEditPreferences.Click += new EventHandler(EditPreferences);

                // View Menu
                // View->Toolbars Menu
                menuItemViewToolbarsProject.Click += new EventHandler(ViewToolbarsProject);
                menuItemViewToolbarsDesign.Click += new EventHandler(ViewToolbarsDesign);
                menuItemViewToolbarsMapping.Click += new EventHandler(ViewToolbarsMapping);
                menuItemViewToolbars.Click += new EventHandler(ViewToolbarsSynthesis);
                menuItemViewToolbarsHelp.Click += new EventHandler(ViewToolbarsHelp);

                // Project Menu
                menuItemProjectPathSettings.Click += new EventHandler(ProjectPathSettings);
                menuItemProjectEditServerLists.Click += new EventHandler(ProjectEditServerLists);
                menuItemProjectProperties.Click += new EventHandler(ProjectProperies);

                // Design Menu
                menuItemDesignConfigureProcessors.Click += new EventHandler(DesignConfigureProcessors);
                menuItemDesignConfigureCommunications.Click += new EventHandler(DesignConfigureCommunications);
                menuItemDesignConfigureProgramming.Click += new EventHandler(DesignConfigureProgramming);

                // Mapping Menu
                menuItemMappingReset.Click += new EventHandler(MappingReset);
                menuItemMappingComplete.Click += new EventHandler(MappingComplete);
                menuItemMappingConfirm.Click += new EventHandler(MappingConfirm);
                menuItemMappingSavePreset.Click += new EventHandler(MappingSavePreset);
                menuItemMappingLoadPreset.Click += new EventHandler(MappingLoadPreset);
                menuItemMappingModeQuery.Click += new EventHandler(MappingModeQuery);
                menuItemMappingModeManual.Click += new EventHandler(MappingModeManual);

                // Synthesis Menu
                menuItemSynthesisBuildXPS.Click += new EventHandler(SynthesisBuildXPS);
                menuItemSynthesisSynthesize.Click += new EventHandler(SynthesisSynthesize);
                menuItemSynthesisProgram.Click += new EventHandler(SynthesisProgram);
                menuItemSynthesisStartConfigServer.Click += new EventHandler(SynthesisStartConfigServer);

                // Help Menu
                menuItemHelpHelp.Click += new EventHandler(HelpHelp);
                menuItemHelpAbout.Click += new EventHandler(HelpAbout);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void RegisterToolbarHandlers()
        {
            try
            {
                // Project Toolbar (File & Project Menus)
                toolBtnFileNewWizard.Click += new EventHandler(FileNewProjectWizard);
                //toolBtnFileNewEmpty.Click += new EventHandler(FileNewProjectWizard);
                toolBtnFileOpen.Click += new EventHandler(FileOpenProject);
                toolBtnFileSave.Click += new EventHandler(FileSaveProject);
                toolBtnFileSaveCopy.Click += new EventHandler(FileSaveProjectCopy);
                //toolBtnFileClose.Click += new EventHandler(FileCloseProject);
                //toolBtnFileRecentClearList.Click += new EventHandler(FileRecentClearList);
                //toolBtnFilePrint.Click += new EventHandler(FilePrint);
                //toolBtnFileExit.Click += new EventHandler(FileExit);
                // ------
                toolBtnProjectPathSettings.Click += new EventHandler(ProjectPathSettings);
                toolBtnProjectEditServerLists.Click += new EventHandler(ProjectEditServerLists);
                toolBtnProjectProperties.Click += new EventHandler(ProjectProperies);
                // ------
                toolBtnEditPreferences.Click += new EventHandler(EditPreferences);

                // Edit Toolbar
                //toolBtnEditCopy.Click += new EventHandler(EditCopy);
                //toolBtnEditCut.Click += new EventHandler(EditCut);
                //toolBtnEditPaste.Click += new EventHandler(EditPaste);

                // View Toolbar
                //toolBtnViewToolbarsProject.Click += new EventHandler(ViewToolbarsProject);
                //toolBtnViewToolbarsDesign.Click += new EventHandler(ViewToolbarsDesign);
                //toolBtnViewToolbarsMapping.Click += new EventHandler(ViewToolbarsMapping);
                //toolBtnViewToolbars.Click += new EventHandler(ViewToolbarsSynthesis);
                //toolBtnViewToolbarsHelp.Click += new EventHandler(ViewToolbarsHelp);


                // Design Toolbar
                toolBtnDesignConfigureProcessors.Click += new EventHandler(DesignConfigureProcessors);
                toolBtnDesignConfigureCommunications.Click += new EventHandler(DesignConfigureCommunications);
                toolBtnDesignConfigureProgramming.Click += new EventHandler(DesignConfigureProgramming);

                // Mapping Toolbar
                toolBtnMappingReset.Click += new EventHandler(MappingReset);
                toolBtnMappingComplete.Click += new EventHandler(MappingComplete);
                toolBtnMappingConfirm.Click += new EventHandler(MappingConfirm);
                toolBtnMappingSavePreset.Click += new EventHandler(MappingSavePreset);
                toolBtnMappingLoadPreset.Click += new EventHandler(MappingLoadPreset);
                toolBtnMappingModeQuery.Click += new EventHandler(MappingModeQuery);
                toolBtnMappingModeManual.Click += new EventHandler(MappingModeManual);

                // Synthesis Toolbar
                toolBtnSynthesisBuildXPS.Click += new EventHandler(SynthesisBuildXPS);
                toolBtnSynthesisSynthesize.Click += new EventHandler(SynthesisSynthesize);
                toolBtnSynthesisProgram.Click += new EventHandler(SynthesisProgram);
                toolBtnSynthesisStartConfigServer.Click += new EventHandler(SynthesisStartConfigServer);

                // Help Toolbar
                toolBtnHelpHelp.Click += new EventHandler(HelpHelp);
                toolBtnHelpAbout.Click += new EventHandler(HelpAbout);

            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }

        #region File Menu

        private bool ExitCerebrum()
        {
            bool bExit = true;
            try
            {
                DialogResult confirmExit = MessageBox.Show("Are you sure you want to exit the Cerebrum Framework?",
                                                           "Confirm Exit",
                                                           MessageBoxButtons.YesNo,
                                                           MessageBoxIcon.Question);
                if (confirmExit == DialogResult.No)
                    return false;

                if (ProjMan.ProjectLoaded)
                {
                    bExit = ProjMan.CloseProject();
                }
                if (bExit)
                {
                    CloseLog();
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
            return bExit;
        }

        private void FileNewProjectWizard(object sender, EventArgs e)
        {
            try
            {
                ProjMan.NewProject(true);
                ShowHideMenus();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void FileOpenProject(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = loadPathDialog.ShowDialog(this);
                this.Focus();
                if (dr == DialogResult.OK)
                {
                    ProjMan.OpenProject(new FileInfo(loadPathDialog.FileName).Directory.FullName);
                    loadPathDialog.InitialDirectory = new FileInfo(loadPathDialog.FileName).Directory.FullName;
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
            ShowHideMenus();
        }
        private void FileSaveProject(object sender, EventArgs e)
        {
            try
            {
                ProjMan.SaveProject();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
            ShowHideMenus();
        }
        private void FileSaveProjectCopy(object sender, EventArgs e)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void FileCloseProject(object sender, EventArgs e)
        {
            try
            {
                ProjMan.CloseProject();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
            ShowHideMenus();
        }
        private void FileRecentMouseEnter(object sender, EventArgs e)
        {
            try
            {
                UpdateMRUMenu();
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
        }
        private void FileRecentItemOpen(object sender, EventArgs e)
        {
            ToolStripMenuItem menuRecentItem = sender as ToolStripMenuItem;
            if (menuRecentItem != null)
            {
                FileInfo ProjectFile = new FileInfo((string)menuRecentItem.Tag);
                if (ProjectFile.Exists)
                {
                    if (ProjMan.ProjectLoaded)
                    {
                        if (!ProjMan.CloseProject())
                        {
                            return;
                        }
                    }
                    ProjMan.OpenProject(ProjectFile.Directory.FullName);
                }
                else
                {
                    DialogResult res;
                    string Title = "Project No Longer Exists";
                    string Message = String.Format("Unable to locate project at {0}.  Remove it from the Recently used list?", ProjectFile.FullName);
                    res = MessageBox.Show(Message, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (res == DialogResult.Yes)
                    {
                        ProjMan.RemoveMRU(ProjectFile.FullName);
                    }
                }
            }
        }
        private void FileRecentClearList(object sender, EventArgs e)
        {
            try
            {
                ProjMan.ClearMRU();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void FilePrint(object sender, EventArgs e)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void FileExit(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }


        
        #endregion

        #region Edit Menu

        void EditCopy(object sender, EventArgs e)
        {
            try
            {
                projectPanel.Copy();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void EditCut(object sender, EventArgs e)
        {
            try
            {
                projectPanel.Cut();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void EditPaste(object sender, EventArgs e)
        {
            try
            {
                projectPanel.Paste();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void EditPreferences(object sender, EventArgs e)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        #endregion

        #region View Menu
        void ViewToolbarsProject(object sender, EventArgs e)
        {
            try
            {
                toolStripProject.Visible = (menuItemViewToolbarsProject.Checked);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void ViewToolbarsDesign(object sender, EventArgs e)
        {
            try
            {
                toolStripDesign.Visible = (menuItemViewToolbarsDesign.Checked);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void ViewToolbarsMapping(object sender, EventArgs e)
        {
            try
            {
                toolStripMapping.Visible = (menuItemViewToolbarsMapping.Checked);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void ViewToolbarsSynthesis(object sender, EventArgs e)
        {
            try
            {
                toolStripSynthesis.Visible = (menuItemViewToolbarsSynthesis.Checked);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void ViewToolbarsHelp(object sender, EventArgs e)
        {
            try
            {
                toolStripHelp.Visible = (menuItemViewToolbarsHelp.Checked);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        #endregion

        #region Project Menu
        void ProjectPathSettings(object sender, EventArgs e)
        {
            try
            {
                ProjMan.ShowProjectPathsDialog(this);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void ProjectEditServerLists(object sender, EventArgs e)
        {
            try
            {
                ProjMan.ShowServerManagerDialog(this);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void ProjectProperies(object sender, EventArgs e)
        {
            try
            {
                ProjMan.ShowProjectPropertiesDialog(this);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        #endregion

        #region Design Menu
        void DesignConfigureProcessors(object sender, EventArgs e)
        {
            try
            {
                ProjMan.ShowProcessorManagerDialog(this);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void DesignConfigureCommunications(object sender, EventArgs e)
        {
            try
            {
                ProjMan.ShowCommunicationsManagerDialog(this);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void DesignConfigureProgramming(object sender, EventArgs e)
        {
            try
            {
                ProjMan.ShowProgrammingManagerDialog(this, false, null);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        #endregion

        #region Mapping Menu

        private void MappingModeManual(object sender, EventArgs e)
        {
            try
            {
                toolBtnMappingModeManual.Enabled = false;
                toolBtnMappingModeQuery.Enabled = true;
                mappingCanvas.UIMode = MappingCanvasControl.InterfaceMode.ManualMapping;
                mappingCanvas.Focus();

                grpMappingSelectedInfo.Text = "Selected Item Information";
                lblMappingSelectedInfo.Text = string.Empty;
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void MappingModeQuery(object sender, EventArgs e)
        {
            try
            {
                toolBtnMappingModeManual.Enabled = true;
                toolBtnMappingModeQuery.Enabled = false;
                mappingCanvas.UIMode = MappingCanvasControl.InterfaceMode.GetInformation;
                mappingCanvas.Focus();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void MappingComplete(object sender, EventArgs e)
        {
            try
            {
                this.SuspendLayout();
                mappingCanvas.SuspendMappingStateEvents = true;
                if (mappingCanvas.CompleteMapping())
                {
                    LoadMapState(tabsMapping.SelectedTab);
                    toolBtnMappingConfirm.Enabled = true;
                }
                else
                {
                    ActivateMappingUI();                    
                    toolBtnMappingConfirm.Enabled = false;
                }
                mappingCanvas.SuspendMappingStateEvents = false;
                this.ResumeLayout();
                mappingCanvas.Focus();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void MappingConfirm(object sender, EventArgs e)
        {
            try
            {
                mappingCanvas.SaveMapping();
                ProjMan.RefreshProcessorMappings();
                ProjMan.RefreshCoreConfigs();
                ProjMan.LoadCommunications();
                mappingCanvas.Focus();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void MappingReset(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you SURE you want to reset ALL component mappings?\nAny unsaved mapping state will be lost.", "Confirm", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    mappingCanvas.LoadProject(ProjMan, true);
                    InitializeMappingTabs();
                }
                mappingCanvas.Focus();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void MappingSavePreset(object sender, EventArgs e)
        {
            try
            {
                if (ProjMan.ProjectLoaded)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.AddExtension = true;
                    sfd.Filter = "XML File (*.xml)|*.xml";
                    sfd.FilterIndex = 0;
                    sfd.DefaultExt = ".xml";
                    sfd.InitialDirectory = ProjMan.ProjectDirectory.FullName;
                    sfd.CheckPathExists = true;
                    sfd.OverwritePrompt = true;
                    sfd.DereferenceLinks = true;
                    sfd.Title = "Save Current Mapping State";
                    sfd.ValidateNames = true;
                    DialogResult res = sfd.ShowDialog(this);
                    this.Focus();
                    if (res == DialogResult.OK)
                    {
                        mappingCanvas.SaveState(sfd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void MappingLoadPreset(object sender, EventArgs e)
        {
            try
            {
                if (ProjMan.ProjectLoaded)
                {
                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.AddExtension = true;
                    ofd.Filter = "XML File (*.xml)|*.xml";
                    ofd.FilterIndex = 0;
                    ofd.DefaultExt = ".xml";
                    ofd.InitialDirectory = ProjMan.ProjectDirectory.FullName;
                    ofd.CheckPathExists = true;
                    ofd.CheckFileExists = true;
                    ofd.DereferenceLinks = true;
                    ofd.Title = "Load Previous Mapping State";
                    ofd.ValidateNames = true;
                    DialogResult res = ofd.ShowDialog(this);
                    this.Focus();
                    if (res == DialogResult.OK)
                    {
                        mappingCanvas.LoadState(ofd.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        #endregion

        #region Synthesis Menu
        void SynthesisBuildXPS(object sender, EventArgs e)
        {
            try
            {
                ProjMan.RefreshCoreConfigs();
                if (XPSThread == null)
                {
                    if (ProjMan.ReadyForXPSBuild())
                    {
                        XPSBuilderDialog dialog = new XPSBuilderDialog();
                        dialog.ForceClean = true;
                        dialog.AllowEmptySynth = true;
                        if (dialog.ShowDialog(this) == DialogResult.OK)
                        {
                            ProjectManager.ToolStartArgs ToolArgs = new ProjectManager.ToolStartArgs();
                            ToolArgs.AllowEmptySynth = dialog.AllowEmptySynth;
                            ToolArgs.ForceClean = dialog.ForceClean;
                            this.Focus();
                            RunXPSBuilder(ToolArgs, tbToolLiveLog);
                        }
                    }
                }
                else
                {
                    KillXPS();
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void SynthesisSynthesize(object sender, EventArgs e)
        {
            try
            {
                ProjMan.RefreshCoreConfigs();
                if (SynthesisThread == null)
                {
                    if (ProjMan.ReadyForSynthesis())
                    {
                        SynthesisDialog dialog = new SynthesisDialog();
                        dialog.ForceClean = true;
                        dialog.SynthesizeHardware = true;
                        dialog.SelectiveSynthesis= false;
                        dialog.CompileSoftware = false;
                        dialog.SelectiveClean = false;
                        if (dialog.ShowDialog(this) == DialogResult.OK)
                        {
                            ProjectManager.ToolStartArgs ToolArgs = new ProjectManager.ToolStartArgs();
                            ToolArgs.CompileSoftware = dialog.CompileSoftware;
                            ToolArgs.SynthesizeHardware = dialog.SynthesizeHardware;
                            ToolArgs.ForceClean = dialog.ForceClean;
                            ToolArgs.SelectiveClean = dialog.SelectiveClean;
                            ToolArgs.SelectiveSynthesis = dialog.SelectiveSynthesis;
                            this.Focus();
                            RunSynthesis(ToolArgs, tbToolLiveLog);
                        }
                    }
                }
                else
                {
                    KillSynthesis();
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void SynthesisProgram(object sender, EventArgs e)
        {
            try
            {
                ProjMan.RefreshCoreConfigs();
                ProjMan.ShowProgrammingManagerDialog(this, true, new StartProgrammerCallback(StartJProgrammer));
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        void SynthesisStartConfigServer(object sender, EventArgs e)
        {
            try
            {
                ProjMan.RefreshCoreConfigs();
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        #endregion

        #region Windows Menu

        #endregion

        #region Help Menu
        void HelpHelp(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Coming soon! HTML Help File.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }

        void HelpAbout(object sender, EventArgs e)
        {
            try
            {
                frmHelpAbout HelpAbout = new frmHelpAbout();
                HelpAbout.ShowDialog(this);
                this.Focus();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        #endregion

        #endregion
        
        #region Component Mapping Management Display, Tabs, Events, and Management

        private void ActivateMappingUI()
        {
            try
            {
                mappingCanvas.Left = 0;
                mappingCanvas.Top = 0;
                mappingCanvas.Width = mappingHost.Width;
                mappingCanvas.Height = mappingHost.Height;
                mappingCanvas.UnmappedPct = 0.3;

                mappingCanvas.ColumnsPerCanvas = 1;
                mappingCanvas.ColumnsPerCluster = 1;
                mappingCanvas.ColumnsPerFPGA = 1;
                mappingCanvas.ColumnsPerGroup = 2;
                ProjMan.SaveProject();
                mappingCanvas.LoadProject(ProjMan, false);

                InitializeMappingTabs();
                if (mappingCanvas.MapObjects != null)
                {
                    chkMapIOEnable.Checked = mappingCanvas.MapObjects.UseIOWeighting;
                    trackIOWeight.Value = (int)(mappingCanvas.MapObjects.IOWeight * 100);
                }
                else
                {
                    chkMapIOEnable.Checked = false;
                }
                ResetMappingToolbar();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }

        private void ResetMappingToolbar()
        {
            try
            {
                toolBtnMappingModeManual.Enabled = false;
                toolBtnMappingModeQuery.Enabled = true;
                toolBtnMappingConfirm.Enabled = mappingCanvas.AllComponentsMapped();
                toolBtnMappingComplete.Enabled = true;
                toolBtnMappingReset.Enabled = true;

                mappingCanvas.UIMode = MappingCanvasControl.InterfaceMode.ManualMapping;
                if (mappingCanvas.MapObjects != null)
                {
                    chkMapIOEnable.Checked = mappingCanvas.MapObjects.UseIOWeighting;
                }
                else
                {
                    chkMapIOEnable.Checked = false;
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        
        #region Mapping Tab Management

        private enum CompareTypes
        {
            None = -1,
            Equal = 0,
            Contains = 1,
            StartsWith = 2,
            EndsWith = 3
        }
        private const string GROUP_NONE = "--None--";
        private const string GROUP_NEW = "New Group...";
        private const string FPGA_NONE = "--Unmapped--";
        private const int COLUMN_ID = 0;
        private const int COLUMN_COMPONENT = 1;
        private const int COLUMN_GROUP = 2;
        private const int COLUMN_FPGA = 3;
        private StringCollection Groups = new StringCollection();
        private StringCollection FPGAs = new StringCollection();

        /// <summary>
        /// Initializes listview tabs used in the mapping interface
        /// </summary>
        public void InitializeMappingTabs()
        {
            try
            {
                tabsMapping.SelectedIndexChanged += new EventHandler(mapTabs_SelectedIndexChanged);

                lvExAll.AddSubItem = true;
                lvExAll.HideComboAfterSelChange = true;
                lvExAll.FullRowSelect = true;
                lvExAll.View = View.Details;
                lvExAll.Items.Clear();
                lvExAll.ComboBoxChanged += new ListViewEx.ComboBoxChangedHandler(lvExMapping_ComboBoxChanged);

                //lvExUnmapped.AddSubItem = true;
                //lvExUnmapped.HideComboAfterSelChange = true;
                //lvExUnmapped.FullRowSelect = true;
                //lvExUnmapped.View = View.Details;
                //lvExUnmapped.Items.Clear();
                //lvExUnmapped.ComboBoxChanged += new ListViewEx.ComboBoxChangedHandler(lvExMapping_ComboBoxChanged);

                LoadMapState(tabsMapping.SelectedTab);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }

        private void mapTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                LoadMapState(tabsMapping.SelectedTab);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void LoadMapState(TabPage pg)
        {
            ListViewEx lvEx = null;
            this.SuspendLayout();
            try
            {
                #region Locate and Initialize ListViewEx
                if (pg == null)
                    return;

                foreach (Control c in pg.Controls)
                    if (c.GetType() == typeof(ListViewEx))
                    {
                        lvEx = (ListViewEx)c;
                        break;
                    }

                lvEx.SuspendLayout();
                lvEx.BeginUpdate();
                if (lvEx == null)
                    return;
                lvEx.ComboBoxChanged -= new ListViewEx.ComboBoxChangedHandler(lvExMapping_ComboBoxChanged);

                lvEx.AddSubItem = true;
                lvEx.HideComboAfterSelChange = true;
                lvEx.FullRowSelect = true;
                lvEx.View = View.Details;
                lvEx.ClearData();
                #endregion

                #region Load Filter
                string Filter = (string)pg.Tag;
                string[] Filters = ((Filter == null) ? new string[] { } : Filter.Split(';'));
                #endregion

                #region Load Groups
                Groups = new StringCollection();
                Groups.Add(GROUP_NEW);
                Groups.Add(GROUP_NONE);
                foreach (string key in mappingCanvas.MapObjects.GetGroups().Keys)
                {
                    Groups.Add(key);
                }
                #endregion

                #region Load FPGAs
                FPGAs = new StringCollection();
                FPGAs.Add(FPGA_NONE);
                foreach (string key in mappingCanvas.MapObjects.GetFPGAs().Keys)
                {
                    FPGAs.Add(key);
                }
                #endregion

                #region Load Components
                int i = 0;
                foreach (string key in mappingCanvas.MapObjects.GetComponents().Keys)
                {
                    string name = mappingCanvas.MapObjects.GetComponentName(key);
                    string group = mappingCanvas.MapObjects.GetComponentGroupID(key);
                    string fpga = mappingCanvas.MapObjects.GetComponentFPGAID(key);
                    if (group == string.Empty)
                        group = GROUP_NONE;
                    if (fpga == string.Empty)
                        fpga = FPGA_NONE;
                    bool bAdd = true;
                    foreach (string F in Filters)
                    {
                        string[] FStrings = F.Split(',');
                        if (FStrings.Length != 3)
                            continue;

                        string ColSrc = FStrings[0].ToUpper();
                        string TypeSrc = FStrings[1];
                        string CompareVal = FStrings[2];

                        string TestVal = string.Empty;
                        CompareTypes CompareType = CompareTypes.None;

                        if (ColSrc == "ID")
                            TestVal = key;
                        else if (ColSrc == "NAME")
                            TestVal = name;
                        else if (ColSrc == "GROUP")
                            TestVal = group;
                        else if (ColSrc == "FPGA")
                            TestVal = fpga;

                        if (TypeSrc == "=")
                            CompareType = CompareTypes.Equal;
                        else if (TypeSrc == "<")
                            CompareType = CompareTypes.StartsWith;
                        else if (TypeSrc == ">")
                            CompareType = CompareTypes.EndsWith;
                        else if (TypeSrc == ".")
                            CompareType = CompareTypes.Contains;

                        if (CompareType == CompareTypes.None)
                            break;

                        switch (CompareType)
                        {
                            case CompareTypes.Equal:
                                bAdd = bAdd && (TestVal.Equals(CompareVal));
                                break;
                            case CompareTypes.StartsWith:
                                bAdd = bAdd && (TestVal.StartsWith(CompareVal));
                                break;
                            case CompareTypes.EndsWith:
                                bAdd = bAdd && (TestVal.EndsWith(CompareVal));
                                break;
                            case CompareTypes.Contains:
                                bAdd = bAdd && (TestVal.Contains(CompareVal));
                                break;
                        }
                    }
                    if (bAdd)
                    {
                        lvEx.Items.Add(key, key);
                        lvEx.Items[i].SubItems.Add(name);
                        lvEx.AddComboBoxCell(i, COLUMN_GROUP, Groups);
                        lvEx.SetCellText(i, COLUMN_GROUP, group);
                        lvEx.AddComboBoxCell(i, COLUMN_FPGA, FPGAs);
                        lvEx.SetCellText(i, COLUMN_FPGA, fpga);
                        i++;
                    }
                }
                #endregion

                lvEx.ComboBoxChanged += new ListViewEx.ComboBoxChangedHandler(lvExMapping_ComboBoxChanged);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
            finally
            {
                if (lvEx != null)
                {
                    lvEx.ResumeLayout();
                    lvEx.EndUpdate();
                }
            }
            this.ResumeLayout();
        }
        private void lvExMapping_ComboBoxChanged(ListViewEx lvEx, int row, int col, object oldValue, object newValue)
        {
            try
            {
                string CmpID = lvEx.Items[row].SubItems[COLUMN_ID].Text;
                string GrpID = lvEx.Items[row].SubItems[COLUMN_GROUP].Text;
                string FpgaID = lvEx.Items[row].SubItems[COLUMN_FPGA].Text;
                switch (col)
                {
                    case COLUMN_FPGA:
                        string targetFPGA = (string)newValue;
                        if (targetFPGA == FPGA_NONE)
                        {
                            // Unmapping
                            if (GrpID != GROUP_NONE)
                            {
                                // Map GrpID to FPGA NONE
                                mappingCanvas.UnMapGroup(GrpID);
                            }
                            else
                            {
                                // Map CmpID to FPGA NONE
                                mappingCanvas.UnMapComponent(CmpID);
                            }
                        }
                        else
                        {
                            // Mapping
                            if (GrpID != GROUP_NONE)
                            {
                                // Map GrpID to FPGA targetFPGA
                                mappingCanvas.MapGroupToFPGA(GrpID, targetFPGA);
                            }
                            else
                            {
                                // Map CmpID to FPGA targetFPGA
                                mappingCanvas.MapComponentToFPGA(CmpID, targetFPGA);
                            }
                        }
                        break;
                    case COLUMN_GROUP:
                        string targetGroup = (string)newValue;
                        if (targetGroup == GROUP_NONE)
                        {
                            // Group CmpID in Group NONE
                            mappingCanvas.UnGroupComponent(CmpID);
                        }
                        else
                        {
                            // Group CmpID in Group targetGroup
                            mappingCanvas.AddComponentToGroup(CmpID, targetGroup);
                        }
                        break;
                }
                // LoadMapState(mapTabs.SelectedTab);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }

        private void chkMapIOEnable_CheckedChanged(object sender, EventArgs e)
        {
            mappingCanvas.MapObjects.UseIOWeighting = chkMapIOEnable.Checked;
            trackIOWeight.Enabled = mappingCanvas.MapObjects.UseIOWeighting;
            lblMapIOWeight.Text = String.Format("{0}%", (int)(mappingCanvas.MapObjects.IOWeight * 100));
            trackIOWeight.Value = (int)(mappingCanvas.MapObjects.IOWeight * 100);
        }
        private void trackIOWeight_Scroll(object sender, EventArgs e)
        {
            mappingCanvas.MapObjects.IOWeight = (double)trackIOWeight.Value / 100.0;
            trackIOWeight.Enabled = mappingCanvas.MapObjects.UseIOWeighting;
            lblMapIOWeight.Text = String.Format("{0}%", (int)(mappingCanvas.MapObjects.IOWeight * 100));
        }
        private void trackIOWeight_ValueChanged(object sender, EventArgs e)
        {
            mappingCanvas.MapObjects.IOWeight = (double)trackIOWeight.Value / 100.0;
            trackIOWeight.Enabled = mappingCanvas.MapObjects.UseIOWeighting;
            lblMapIOWeight.Text = String.Format("{0}%", (int)(mappingCanvas.MapObjects.IOWeight * 100));
        }

        #endregion
        
        #region Mapping Canvas Event Handlers

        private void RegisterMappingCanvasEventHandlers()
        {
            try
            {
                mappingCanvas.MappingStateChanged += new MappingCanvasControl.MappingStateChangedHandler(mappingCanvas_MappingStateChanged);
                mappingCanvas.MappingError += new MappingCanvasControl.MappingErrorHandler(mappingCanvas_MappingError);
                mappingCanvas.MappingException += new MappingCanvasControl.MappingExceptionHandler(mappingCanvas_MappingException);
                mappingCanvas.ItemInformationRecevied += new MappingCanvasControl.ItemInformationHandler(mappingCanvas_ItemInformationRecevied);

                chkMapIOEnable.CheckedChanged +=new EventHandler(chkMapIOEnable_CheckedChanged);
                trackIOWeight.Scroll +=new EventHandler(trackIOWeight_Scroll);
                trackIOWeight.ValueChanged += new EventHandler(trackIOWeight_ValueChanged);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }

        private void mappingCanvas_ItemInformationRecevied(string Type, string ID, string Name, string Information)
        {
            try
            {
                grpMappingSelectedInfo.Text = Type;
                lblMappingSelectedInfo.Text = Information;
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void mappingCanvas_MappingStateChanged()
        {
            try
            {
                LoadMapState(tabsMapping.SelectedTab);
                toolBtnMappingConfirm.Enabled = false;
                menuItemMappingConfirm.Enabled = menuMapping.Enabled && mappingCanvas.AllComponentsMapped();
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void mappingCanvas_MappingException(string ExceptionTrace)
        {
            try
            {
                MessageBox.Show(ExceptionTrace, "Exception!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }
        private void mappingCanvas_MappingError(string Message)
        {
            try
            {
                MessageBox.Show(Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                WriteLog(ex);
                ErrorReporting.MessageBoxException(ex);
            }
        }

        #endregion

        #endregion

        #region Error/Exception Logging
        StreamWriter logWriter;

        /// <summary>
        /// Opens the StreamWriter used for the Cerebrum Logfile
        /// </summary>
        public void OpenLog()
        {
            DateTime N = DateTime.Now;
            string TimeNow = String.Format("{0}{1}_{2}", N.Hour.ToString("00"), N.Minute.ToString("00"), N.Second.ToString("00"));
            string DateNow = String.Format("{0}{1}{2}", N.Year.ToString("0000"), N.Month.ToString("00"), N.Day.ToString("00"));
            string AppDir = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Directory.FullName;
            logWriter = new StreamWriter(String.Format("{0}\\cerebrum_{1}_{2}.log", AppDir, DateNow, TimeNow), true);
            logWriter.WriteLine(String.Format("******************************************************************"));
            logWriter.WriteLine(String.Format("***  Started Cerebrum Log {0} {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString()));
            logWriter.WriteLine(String.Format("******************************************************************"));
            logWriter.WriteLine();
        }
        /// <summary>
        /// Closes the StreamWriter used for the Cerebrum Logfile
        /// </summary>
        public void CloseLog()
        {
            if (logWriter != null)
            {
                logWriter.WriteLine();
                logWriter.WriteLine(String.Format("******************************************************************"));
                logWriter.WriteLine(String.Format("***  Closed Cerebrum Log {0} {1}", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString()));
                logWriter.WriteLine(String.Format("******************************************************************"));
                logWriter.Close();
                logWriter = null;
            }
        }
        /// <summary>
        /// Writes a string message to the Cerebrum Logfile
        /// </summary>
        public void WriteLog(string Message)
        {
            if (logWriter != null)
            {
                logWriter.WriteLine(String.Format("[{0} {1}]: {2}",
                    DateTime.Now.ToShortDateString(),
                    DateTime.Now.ToShortTimeString(),
                    Message));
                logWriter.Flush();
            }
        }
        /// <summary>
        /// Writes an exception to the Cerebrum Logfile
        /// </summary>
        public void WriteLog(Exception ex)
        {
            if (logWriter != null)
            {
                WriteLog(String.Format("EXCEPTION: {0}", ErrorReporting.ExceptionDetails(ex)));
                logWriter.Flush();
            }
        }
        #endregion

        #region BackEnd Flow

        #region Shared Functions

        private delegate void LogWriterCallback(string Message);
        private delegate bool ReadPasswordCallback(string Prompt, out string Password);
        private void KillThread(ref Thread t)
        {
            try
            {
                if (t != null)
                {
                    t.Abort();
                }
            }
            catch
            {
            }
            t = null;
        }

        #endregion

        #region XPS Builder
        StreamWriter XPSBuilderLog;
        TextBox tbXPSBuilderLog;
        Thread XPSThread;
        ProjectManager.ToolCompleteDelegate XPSCompleteDelegate;
        ProjectManager.ToolMessageDelegate XPSMessageDelegate;
        private void KillXPS()
        {
            KillThread(ref XPSThread);
            XPSComplete();
        }
        private void RunXPSBuilder(ProjectManager.ToolStartArgs StartArgs, TextBox XPSBuilderLiveLog)
        {
            if (!Directory.Exists(ProjMan.PathManager["LocalOutput"]))
                Directory.CreateDirectory(ProjMan.PathManager["LocalOutput"]);
            XPSBuilderLog = new StreamWriter(String.Format("{0}\\xpsbuild.log", ProjMan.PathManager["LocalOutput"]));
            tbXPSBuilderLog = XPSBuilderLiveLog;
            tbXPSBuilderLog.Clear();
            ProjMan.XPSBuilderToolMessage += XPSMessageDelegate;

            //ProjMan.RunXPSBuilder(bClean, bForceSynth);
            //ProjMan.XPSBuilderToolMessage -= new ProjectManager.ToolMessageDelegate(WriteXPSBuilderLog);
            //WriteXPSBuilderLog("XPS Builder Tool Execution Completed");
            //tbXPSBuilderLog = null;
            //XPSBuilderLog.Close();
            //XPSBuilderLog = null;

            ProjMan.XPSBuilderComplete += XPSCompleteDelegate;
            ParameterizedThreadStart pts = new ParameterizedThreadStart(ProjMan.RunXPSBuilder);
            XPSThread = new Thread(pts);
            XPSThread.Start(StartArgs);
            RemoveFPGATabs();

        }
        private void WriteXPSBuilderLog(string Message)
        {
            string LogLine = String.Format("[{0} {1}] - {2}",
                DateTime.Now.ToShortDateString(),
                DateTime.Now.ToShortTimeString(),
                Message);
            if (XPSBuilderLog != null)
            {
                XPSBuilderLog.WriteLine(LogLine);
            }
            WriteToTextBox(tbXPSBuilderLog, LogLine);
        }
        private void XPSComplete()
        {
            ProjMan.XPSBuilderToolMessage -= XPSMessageDelegate;
            ProjMan.XPSBuilderComplete -= XPSCompleteDelegate;
            WriteXPSBuilderLog("XPS Builder Tool Execution Completed");
            if (tbXPSBuilderLog != null)
            {
                tbXPSBuilderLog = null;
            }
            if (XPSBuilderLog != null)
            {
                XPSBuilderLog.Close();
                XPSBuilderLog = null;
            }
            XPSThread = null;
        }
        #endregion

        #region System Synthesis
        private delegate void RegisterFPGAMessageDelegate(string Tab, string Message);
        private delegate void FPGATabManageDelegate();

        StreamWriter SynthesisLog;
        TextBox tbSynthesisLog;
        Thread SynthesisThread;
        ProjectManager.ToolCompleteDelegate SynthCompleteDelegate;
        ProjectManager.ToolMessageDelegate SynthMessageDelegate;
        private void KillSynthesis()
        {
            KillThread(ref SynthesisThread);
            SynthesisComplete();
        }
        private void RunSynthesis(ProjectManager.ToolStartArgs StartArgs, TextBox SynthesisLiveLog)
        {
            if (!Directory.Exists(ProjMan.PathManager["LocalOutput"]))
                Directory.CreateDirectory(ProjMan.PathManager["LocalOutput"]);
            SynthesisLog = new StreamWriter(String.Format("{0}\\synth.log", ProjMan.PathManager["LocalOutput"]));
            tbSynthesisLog = SynthesisLiveLog;
            //ClearFPGATabs(); 
            RemoveFPGATabs();
            tbSynthesisLog.Clear();
            ProjMan.SynthesisToolMessage += SynthMessageDelegate;

            //ProjMan.RunSynthesis(bSWOnly, bHWOnly, bClean);
            //ProjMan.SynthesisToolMessage -= new ProjectManager.ToolMessageDelegate(WriteSynthesisLog);
            //WriteSynthesisLog("Synthesis Tool Execution Completed");
            //tbSynthesisLog = null;
            //SynthesisLog.Close();
            //SynthesisLog = null;

            ProjMan.SynthesisComplete += SynthCompleteDelegate;
            ParameterizedThreadStart pts = new ParameterizedThreadStart(ProjMan.RunSynthesis);
            SynthesisThread = new Thread(pts);
            SynthesisThread.Start(StartArgs);
        }
        private void WriteSynthesisLog(string Message)
        {
            string LogLine = String.Format("[{0} {1}] - {2}",
                DateTime.Now.ToShortDateString(),
                DateTime.Now.ToShortTimeString(),
                Message);
            if (SynthesisLog != null)
            {
                SynthesisLog.WriteLine(LogLine);
            }
            WriteToTextBox(tbSynthesisLog, LogLine);
            if (Message.StartsWith("["))
            {
                int start = Message.IndexOf('[') + 1;
                int end = Message.IndexOf(']');
                int len = end - start;
                string Tab = Message.Substring(start, len);
                RegisterFPGAMessage(Tab, LogLine);
            }
        }
        private void SynthesisComplete()
        {
            ProjMan.SynthesisToolMessage -= SynthMessageDelegate;
            ProjMan.SynthesisComplete -= SynthCompleteDelegate;
            WriteSynthesisLog("Synthesis Tool Execution Completed");
            if (tbSynthesisLog != null)
            {
                tbSynthesisLog = null;
            }
            if (SynthesisLog != null)
            {
                SynthesisLog.Close();
                SynthesisLog = null;
            }
            SynthesisThread = null;
        }
        
        private void RegisterFPGAMessage(string TabText, string LogLine)
        {
            if (tabToolOutput.InvokeRequired)
            {
                tabToolOutput.Invoke(new RegisterFPGAMessageDelegate(RegisterFPGAMessage), TabText, LogLine);
            }
            else
            {
                TabPage FPGAPage = null;
                TextBox FPGAText = null;
                foreach (TabPage TP in tabToolOutput.TabPages)
                {
                    if (String.Compare(TP.Text, TabText, true) == 0)
                    {
                        FPGAPage = TP;
                        FPGAText = (TextBox)FPGAPage.Tag;
                        break;
                    }
                }
                if (FPGAPage == null)
                {
                    // Create the Textbox and the TabPage
                    FPGAText = new TextBox();
                    FPGAText.Dock = System.Windows.Forms.DockStyle.Fill;
                    FPGAText.Location = new System.Drawing.Point(3, 3);
                    FPGAText.Multiline = true;
                    FPGAText.ReadOnly = true;
                    FPGAText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
                    FPGAText.TabIndex = 0;

                    // Create the TabPage
                    FPGAPage = new System.Windows.Forms.TabPage();
                    FPGAPage.SuspendLayout();
                    FPGAPage.Location = new System.Drawing.Point(4, 22);
                    FPGAPage.Padding = new System.Windows.Forms.Padding(3);
                    FPGAPage.TabIndex = 0;
                    FPGAPage.Text = TabText;
                    FPGAPage.UseVisualStyleBackColor = true;
                    FPGAPage.Tag = FPGAText;

                    // Add them to the approprate controls
                    FPGAPage.Controls.Add(FPGAText);
                    tabToolOutput.TabPages.Add(FPGAPage);
                    FPGAPage.ResumeLayout();
                }
                if (FPGAText != null)
                {
                    WriteToTextBox(FPGAText, LogLine);
                }
            }
        }
        private void ClearFPGATabs()
        {
            if (tabToolOutput.InvokeRequired)
            {
                tabToolOutput.Invoke(new FPGATabManageDelegate(ClearFPGATabs));
            }
            else
            {
                TextBox FPGAText = null;
                foreach (TabPage TP in tabToolOutput.TabPages)
                {
                    if (String.Compare(TP.Text, "Combined", true) != 0)
                    {
                        FPGAText = (TextBox)TP.Tag;
                        if (FPGAText != null)
                        {
                            FPGAText.Clear();
                        }
                    }
                }
            }
        }

        private void RemoveFPGATabs()
        {
            if (tabToolOutput.InvokeRequired)
            {
                tabToolOutput.Invoke(new FPGATabManageDelegate(ClearFPGATabs));
            }
            else
            {
                while (tabToolOutput.TabPages.Count > 1)
                {
                    foreach (TabPage TP in tabToolOutput.TabPages)
                    {
                        if (String.Compare(TP.Text, "Combined", true) != 0)
                        {
                            tabToolOutput.TabPages.Remove(TP);
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region JTAG Programmer
        StreamWriter JProgrammerLog;
        TextBox tbJProgrammerLog;
        Thread ProgrammingThread;
        ProjectManager.ToolCompleteDelegate JProgCompleteDelegate;
        ProjectManager.ToolMessageDelegate JProgMessageDelegate;
        private void KillProgramming()
        {
            KillThread(ref ProgrammingThread);
            JProgrammerComplete();
        }
        private void StartJProgrammer()
        {
            try
            {
                if (ProgrammingThread == null)
                {
                    if (ProjMan.ReadyForProgramming())
                    {
                        ProjectManager.ToolStartArgs StartArgs = new ProjectManager.ToolStartArgs();
                        RunJProgrammer(StartArgs, tbToolLiveLog);
                    }
                }
                else
                {
                    KillProgramming();
                }
            }
            catch
            {}
        }
        private void RunJProgrammer(ProjectManager.ToolStartArgs StartArgs, TextBox JProgrammerLiveLog)
        {
            if (!Directory.Exists(ProjMan.PathManager["LocalOutput"]))
                Directory.CreateDirectory(ProjMan.PathManager["LocalOutput"]);
            JProgrammerLog = new StreamWriter(String.Format("{0}\\jprogrammer.log", ProjMan.PathManager["LocalOutput"]));
            tbJProgrammerLog = JProgrammerLiveLog;
            tbJProgrammerLog.Clear();
            ProjMan.JProgrammerToolMessage += JProgMessageDelegate;

            //ProjMan.RunJProgrammer();
            //ProjMan.JProgrammerToolMessage -= new ProjectManager.ToolMessageDelegate(WriteJProgrammerLog);
            //WriteSynthesisLog("JTAG Programmer Tool Execution Completed");
            //tbJProgrammerLog = null;
            //JProgrammerLog.Close();
            //JProgrammerLog = null;

            ProjMan.JProgrammerComplete += JProgCompleteDelegate;
            ParameterizedThreadStart pts = new ParameterizedThreadStart(ProjMan.RunJProgrammer);
            ProgrammingThread = new Thread(pts);
            ProgrammingThread.Start(StartArgs);
        }
        private void WriteJProgrammerLog(string Message)
        {
            string LogLine = String.Format("[{0} {1}] - {2}",
                DateTime.Now.ToShortDateString(),
                DateTime.Now.ToShortTimeString(),
                Message);
            if (JProgrammerLog != null)
            {
                JProgrammerLog.WriteLine(LogLine);
            }
            WriteToTextBox(tbJProgrammerLog, LogLine);
        }
        private void JProgrammerComplete()
        {
            ProjMan.JProgrammerComplete -= JProgCompleteDelegate;
            ProjMan.JProgrammerToolMessage -= JProgMessageDelegate;
            WriteJProgrammerLog("JTAG Programmer Tool Execution Completed");
            if (tbJProgrammerLog != null)
            {
                tbJProgrammerLog = null;
            }
            if (JProgrammerLog != null)
            {
                JProgrammerLog.Close();
                JProgrammerLog = null;
            }
            ProgrammingThread = null;
        }
        #endregion

        #region Runtime Configuration
        StreamWriter RuntimeConfigLog;
        TextBox tbRuntimeConfigLog;

        private void DoRuntimeConfig(TextBox RuntimeConfigLiveLog)
        {
            if (!Directory.Exists(ProjMan.PathManager["LocalOutput"]))
                Directory.CreateDirectory(ProjMan.PathManager["LocalOutput"]);
            RuntimeConfigLog = new StreamWriter(String.Format("{0}\\runtime_config.log", ProjMan.PathManager["LocalOutput"]));
            tbRuntimeConfigLog = RuntimeConfigLiveLog;
            tbRuntimeConfigLog.Clear();
            //ProjMan.RuntimeConfigToolMessage += new ProjectManager.ToolMessageDelegate(WriteRuntimeConfigLog);
            //ProjMan.RunSynthesis();
            //ProjMan.RuntimeConfigToolMessage -= new ProjectManager.ToolMessageDelegate(WriteRuntimeConfigLog);

            //WriteSynthesisLog("Runtime Configuration Tool Execution Completed");
            tbRuntimeConfigLog = null;
            RuntimeConfigLog.Close();
            RuntimeConfigLog = null;
        }
        private void WriteRuntimeConfigLog(string Message)
        {
            string LogLine = String.Format("[{0} {1}] - {2}",
                DateTime.Now.ToShortDateString(),
                DateTime.Now.ToShortTimeString(),
                Message);
            if (RuntimeConfigLog != null)
            {
                RuntimeConfigLog.WriteLine(LogLine);
            }
            WriteToTextBox(tbRuntimeConfigLog, LogLine);
        }

        #endregion

        #region BackEnd Flow UI Management

        private void RegisterBackEndHandlers()
        {
            btnRefreshFiles.Click +=new EventHandler(btnRefreshFiles_Click);
            btnOpenSelected.Click += new EventHandler(btnOpenSelected_Click);
            btnApplyFilter.Click += new EventHandler(btnApplyFilter_Click);
            txtFileFilter.GotFocus += new EventHandler(txtFileFilter_GotFocus);
            txtFileFilter.KeyUp += new KeyEventHandler(txtFileFilter_KeyUp);
            treeProjectFiles.DoubleClick += new EventHandler(treeProjectFiles_DoubleClick);
        }

        private void LaunchNodeFile(TreeNode Node)
        {
            try
            {
                string FilePath = (string)Node.Tag;
                if (File.Exists(FilePath))
                {
                    Process FileProcess = new Process();
                    FileProcess.StartInfo.FileName = FilePath;
                    FileProcess.Start();
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                Messages_OnErrorMessage("FileOpenError", "An error occured trying to open the selected file.  Please verify the file is associated with an editor in Windows and try again.", "Project Files View");
            }
        }

        private void btnRefreshFiles_Click(object sender, EventArgs e)
        {
            RefreshProjectFilesView(txtFileFilter.Text);
        }
        private void treeProjectFiles_DoubleClick(object sender, EventArgs e)
        {
            if (treeProjectFiles.SelectedNode != null)
            {
                TreeNode Node = treeProjectFiles.SelectedNode;
                LaunchNodeFile(Node);
            }
        }
        private void btnOpenSelected_Click(object sender, EventArgs e)
        {
            if (treeProjectFiles.SelectedNode != null)
            {
                TreeNode Node = treeProjectFiles.SelectedNode; 
                LaunchNodeFile(Node);
            }
        }
        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            RefreshProjectFilesView(txtFileFilter.Text);
        }
        private void txtFileFilter_GotFocus(object sender, EventArgs e)
        {
            txtFileFilter.SelectAll();
        }
        void txtFileFilter_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                RefreshProjectFilesView(txtFileFilter.Text);
            }
        }


        private void RefreshProjectFilesView(string FilterText)
        {
            try
            {
                if (ProjMan == null) return;
                if (!ProjMan.ProjectLoaded) return;
                List<TreeNode> OutNodes = new List<TreeNode>();
                treeProjectFiles.BeginUpdate();
                treeProjectFiles.Nodes.Clear();
                TreeNode ProjectRoot = new TreeNode(String.Format("Cerebrum Project: {0}", ProjMan.ProjectDirectory.Name));
                TreeNode ProjectOutput = new TreeNode("./output_files");
                TreeNode ProjectConfig = new TreeNode("./core_config");
                TreeNode ProjectFPGAs = new TreeNode("Platform FPGAs");

                #region Add Files in Project "Output Files" Directory
                if (String.Compare(FilterText, string.Empty) == 0)
                {
                    foreach (FileInfo FI in ProjMan.ProjectOutputDirectory.GetFiles("*.log", SearchOption.TopDirectoryOnly))
                    {
                        TreeNode FileNode = new TreeNode(String.Format("{0}", FI.Name));
                        FileNode.Tag = FI.FullName;
                        ProjectOutput.Nodes.Add(FileNode);
                    }
                    foreach (FileInfo FI in ProjMan.ProjectOutputDirectory.GetFiles("*.txt", SearchOption.TopDirectoryOnly))
                    {
                        TreeNode FileNode = new TreeNode(String.Format("{0}", FI.Name));
                        FileNode.Tag = FI.FullName;
                        ProjectOutput.Nodes.Add(FileNode);
                    }
                }
                else
                {
                    foreach (FileInfo FI in ProjMan.ProjectOutputDirectory.GetFiles(FilterText, SearchOption.TopDirectoryOnly))
                    {
                        TreeNode FileNode = new TreeNode(String.Format("{0}", FI.Name));
                        FileNode.Tag = FI.FullName;
                        ProjectOutput.Nodes.Add(FileNode);
                    }
                }
                #endregion

                #region Add Files in Project "Core Config" Directory
                if (String.Compare(FilterText, string.Empty) == 0)
                {
                    foreach (FileInfo FI in ProjMan.ProjectCoreConfigDirectory.GetFiles())
                    {
                        TreeNode FileNode = new TreeNode(String.Format("{0}", FI.Name));
                        FileNode.Tag = FI.FullName;
                        ProjectConfig.Nodes.Add(FileNode);
                    }
                }
                else
                {
                    foreach (FileInfo FI in ProjMan.ProjectCoreConfigDirectory.GetFiles(FilterText, SearchOption.TopDirectoryOnly))
                    {
                        TreeNode FileNode = new TreeNode(String.Format("{0}", FI.Name));
                        FileNode.Tag = FI.FullName;
                        ProjectConfig.Nodes.Add(FileNode);
                    }
                }
                #endregion

                #region Add Files in XPS Projects Directories
                DirectoryInfo XPSDir = new DirectoryInfo(String.Format("{0}\\xps_projects", ProjMan.ProjectDirectory));
                foreach (DirectoryInfo FPGADir in XPSDir.GetDirectories())
                {
                    TreeNode FPGANode = new TreeNode(FPGADir.Name);

                    #region Add MHS & UCF Files -- Always
                    FileInfo MHSFile = new FileInfo(String.Format("{0}\\system.mhs", FPGADir.FullName));
                    if (MHSFile.Exists)
                    {
                        TreeNode MHSNode = new TreeNode(String.Format("./{0}", MHSFile.Name));
                        MHSNode.Tag = MHSFile.FullName;
                        FPGANode.Nodes.Add(MHSNode);
                    }
                    FileInfo UCFFile = new FileInfo(String.Format("{0}\\data\\system.ucf", FPGADir.FullName));
                    if (UCFFile.Exists)
                    {
                        TreeNode UCFNode = new TreeNode(String.Format("./data/{0}", UCFFile.Name));
                        UCFNode.Tag = UCFFile.FullName;
                        FPGANode.Nodes.Add(UCFNode);
                    }
                    #endregion

                    #region Add Files in FPGA "Output Files" Directory
                    DirectoryInfo OutDir = new DirectoryInfo(String.Format("{0}\\output", FPGADir.FullName));
                    if (OutDir.Exists)
                    {
                        TreeNode OutNode = new TreeNode(OutDir.Name);
                        if (String.Compare(FilterText, string.Empty) == 0)
                        {
                            foreach (FileInfo OutFile in OutDir.GetFiles())
                            {
                                TreeNode FileNode = new TreeNode(OutFile.Name);
                                FileNode.Tag = OutFile.FullName;
                                OutNode.Nodes.Add(FileNode);
                            }
                        }
                        else
                        {
                            foreach (FileInfo OutFile in OutDir.GetFiles(FilterText))
                            {
                                TreeNode FileNode = new TreeNode(OutFile.Name);
                                FileNode.Tag = OutFile.FullName;
                                OutNode.Nodes.Add(FileNode);
                            }
                        }
                        FPGANode.Nodes.Add(OutNode);
                        OutNodes.Add(OutNode);
                    }
                    #endregion

                    ProjectFPGAs.Nodes.Add(FPGANode);
                }
                #endregion

                ProjectRoot.Nodes.Add(ProjectOutput);
                ProjectRoot.Nodes.Add(ProjectConfig);
                ProjectRoot.Nodes.Add(ProjectFPGAs);
                treeProjectFiles.Nodes.Add(ProjectRoot);
                treeProjectFiles.ExpandAll();
                ProjectConfig.Collapse();
                foreach (TreeNode OutNode in OutNodes)
                {
                    OutNode.Collapse();
                }
                treeProjectFiles.EndUpdate();
            }
            catch(Exception ex)
            {
                treeProjectFiles.Nodes.Clear();
                treeProjectFiles.EndUpdate();
                if (FilterText != string.Empty)
                {
                    RefreshProjectFilesView(string.Empty);
                }
                else
                {
                    ErrorReporting.DebugException(ex);
                }
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            CerebrumProjectManager.Dialogs.CerebrumCoreCreateEditDialog CoreEditDialog = new CerebrumProjectManager.Dialogs.CerebrumCoreCreateEditDialog();
            CoreEditDialog.Show();
        }

        #endregion

    }
}
