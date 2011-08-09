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
 * frmNewProjectWizard.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: Step-by-Step wizard for creating a new Cerebrum Project.
 * History: 
 * >> (23 Oct 2010) Matthew Cotter: Added missing try/catch blocks for error protection.
 *                                  Added reference to Project Manager to wizard to utilize Proj. Mgr. functions for project files
 *                                      and propagation of messages to GUI.
 * >> (22 Oct 2010) Matthew Cotter: Added check to ensure that save path is populated and valid before 'Finish' button is enabled.
 * >> ( 7 Oct 2010) Matthew Cotter: Added disposal of project platforms after changing to ensure that floating boards, FPGAs, and promptly freed.
 * >> ( 1 Oct 2010) Matthew Cotter: Reorganized saving of server lists from wizard.
 *                                  Corrected bug that wrote the wrong path name for Project Temporary folder.
 * >> (30 Sep 2010) Matthew Cotter: Changed default folder for save folder browser dialog to Cerebrum Root/Install folder.
 * >> (21 Sep 2010) Matthew Cotter: Changed invocation of Project Paths dialog to use DialogResult for verification.
 * >> (15 Sep 2010) Matthew Cotter: Corrected bug in listing Platforms, and saving Platform to Project file.
 *                                  Added override of default tab-key handling for Servers Listview Dialog.
 * >> (13 Sep 2010) Matthew Cotter: Created basic outline of Wizard process based on ICerebrumInterface.
 * >> (13 Sep 2010) Matthew Cotter: Source file created -- Initial version.
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
using System.Xml;
using System.IO;
using CerebrumSharedClasses;
using CerebrumProjectManager;
using FalconPathManager;

namespace CerebrumProjectManager.Wizards
{
    /// <summary>
    /// Defines a form implementation of the New Project Wizard
    /// </summary>
    public partial class frmNewProjectWizard : Form, ICerebrumWizard
    {
        /// <summary>
        /// Get or set the step number indicating the first step of the wizard
        /// </summary>
        public int FirstStep { get; set; }
        /// <summary>
        /// Get or set the step number indicating the last step of the wizard
        /// </summary>
        public int LastStep { get; set; }

        private Panel currentPanel;
        private List<Panel> wizPanels;
        private Panel globalPanel;
        private CerebrumPlatform _SelectedPlatform;
        private class GenericServer
        {
            public FalconGlobal.FalconServer FServer;
            public bool Synthesis { get; set; }
            public bool Programming { get; set; }
            public bool Compilation { get; set; }
        }

        private List<GenericServer> _AvailableServers;       
        private PathManager _Paths;        
        private int currentStep;
        private Form ownerForm;
        private string SaveProjectLocation;
        private ProjectManager _ProjMan;

        /// <summary>
        /// Get or set the project location set by the wizard
        /// </summary>
        public string ProjectLocation
        {
            get
            {
                return SaveProjectLocation;
            }
        }

        private enum WizardSteps : int
        {
            InitialStep = 0,
            PlatformStep = 1,
            ServersStep = 2,
            PathsStep = 3,
            SaveStep = 4,
            EndStep = 5,
        }

        /// <summary>
        /// Constructor.   Initializes the wizard as owned by the specified form and associated with the specified Project Manager
        /// </summary>
        /// <param name="owner">The form which will own this wizard dialog</param>
        /// <param name="ProjMan">The project manager controlling the associated project</param>
        public frmNewProjectWizard(Form owner, ProjectManager ProjMan)
        {
            try
            {
                InitializeComponent();
                ownerForm = owner;
                _ProjMan = ProjMan;
                _Paths = _ProjMan.DefaultPaths(); ;

                this.Cancelled = false;
                this.Error = false;
                this.ErrorMessage = string.Empty;
                wizPanels = new List<Panel>();

                currentPanel = null;
                btnBack.Enabled = false;
                btnNext.Enabled = true;
                btnCancel.Enabled = true;
                panelProgress.VisibleChanged += new EventHandler(panelProgress_VisibleChanged);

                this.FirstStep = (int)WizardSteps.InitialStep;
                this.LastStep = (int)WizardSteps.EndStep;
                LoadTaggedPanels();

                // Hardware Platform Configuration
                comboAvailablePF.SelectedIndexChanged += new EventHandler(comboAvailablePF_SelectedIndexChanged);

                // Server List Configuration
                InitListViewEx();
                _AvailableServers = new List<GenericServer>();
                _AvailableServers.Clear();
                lvServers.ComboBoxChanged += new ListViewEx.ComboBoxChangedHandler(lvServers_ComboBoxChanged);
                lvServers.EditCellChanged += new ListViewEx.EditCellChangedHandler(lvServers_EditCellChanged);

                // Final Step
                txtSaveLocation.TextChanged += new EventHandler(txtSaveLocation_TextChanged);
            }
            catch (Exception ex)
            {
                ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard (Init)");
                ErrorReporting.DebugException(ex);
            }
        }

        #region Step - Configure Hardware Platform
        void comboAvailablePF_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if ((comboAvailablePF.Items.Count == 0) || (comboAvailablePF.SelectedIndex < 0))
                {
                    btnNext.Enabled = false;
                    return;
                }
                if (_SelectedPlatform == null)
                {
                    _SelectedPlatform = new CerebrumPlatform();
                    _SelectedPlatform.ProjectManager = _ProjMan;
                }
                else
                    _SelectedPlatform.Dispose();

                _SelectedPlatform.LoadPlatformFromFile(Paths, comboAvailablePF.SelectedItem.ToString());

                StringBuilder Summary = new StringBuilder();
                Summary.AppendLine(String.Format("Name: {0}", _SelectedPlatform.Name));
                Summary.AppendLine(String.Format("Board Count: {0}", _SelectedPlatform.BoardCount));
                Summary.AppendLine(String.Format("FPGA Count: {0}", _SelectedPlatform.FPGACount));
                Summary.AppendLine(String.Format("-------------------------------"));
                Summary.AppendLine(String.Format("Hierarchy"));
                foreach (CerebrumPlatformBoard b in _SelectedPlatform.Boards)
                {
                    Summary.AppendLine(String.Format(" | - Board: {0}", b.ID));
                    foreach (CerebrumPlatformFPGA f in b.FPGAs)
                    {
                        Summary.AppendLine(String.Format(" | -- FPGA: {0} ({1} {2} {3} {4})", f.ID, f.Family, f.Part, f.Package, f.SpeedGrade));
                    }
                }
                txtPFSummary.Text = Summary.ToString();
                btnNext.Enabled = (_SelectedPlatform.FPGACount > 0);
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard (Platform)");
                ErrorReporting.DebugException(ex);
            }
        }
        void LoadHardwarePlatformList()
        {
            try
            {
                int previousIndex = -1;
                comboAvailablePF.Items.Clear();
                foreach (string dir in Directory.GetDirectories(Paths["Platforms"]))
                {
                    DirectoryInfo di = new DirectoryInfo(dir);
                    string ListName = di.Name;
                    string PlatformDefFile = String.Format("{0}\\{1}\\{1}.xml", Paths["Platforms"], di.Name);
                    if (File.Exists(PlatformDefFile))
                    {
                        if ((_SelectedPlatform != null) && (_SelectedPlatform.Name == ListName))
                        {
                            previousIndex = comboAvailablePF.Items.Count;
                        }
                        comboAvailablePF.Items.Add(ListName);
                    }
                }
                if ((previousIndex < 0) && (comboAvailablePF.Items.Count > 0))
                    previousIndex = 0;
                comboAvailablePF.SelectedIndex = previousIndex;
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard (Platform)");
                ErrorReporting.DebugException(ex);
            }
        }
        #endregion

        #region Step - Configure the Servers List
        void InitListViewEx()
        {
            lvServers.ClearData();
            lvServers.Items.Clear();
            lvServers.AddSubItem = true;
            lvServers.HideComboAfterSelChange = true;
            lvServers.FullRowSelect = true;
            lvServers.View = View.Details;
        }
        void AddToListView(int row, GenericServer gs)
        {
            lvServers.Items.Add(gs.FServer.ID, gs.FServer.ID);
            lvServers.AddEditableCell(row, 1);
            lvServers.SetCellText(row, 1, gs.FServer.Address);
            lvServers.AddEditableCell(row, 2);
            lvServers.SetCellText(row, 2, gs.FServer.UserName);
            lvServers.AddComboBoxCell(row, 3, new string[] { "Yes", "No" });
            lvServers.SetCellText(row, 3, (gs.Synthesis ? "Yes" : "No"));
            lvServers.AddComboBoxCell(row, 4, new string[] { "Yes", "No" });
            lvServers.SetCellText(row, 4, (gs.Synthesis ? "Yes" : "No"));
            lvServers.AddComboBoxCell(row, 5, new string[] { "Yes", "No" });
            lvServers.SetCellText(row, 5, (gs.Synthesis ? "Yes" : "No"));
        }
        void lvServers_EditCellChanged(ListViewEx lvEx, int row, int col, object oldValue, object newValue)
        {
            ListViewItem lvi = lvEx.Items[row];
            string ID = lvi.Text;
            UpdateServerInfo(ID, col, (string)newValue);
        }
        void lvServers_ComboBoxChanged(ListViewEx lvEx, int row, int col, object oldValue, object newValue)
        {
            ListViewItem lvi = lvEx.Items[row];
            string ID = lvi.Text;
            UpdateServerInfo(ID, col, (string)newValue);
        }
        void UpdateServerInfo( string ID, int col, string newValue)
        {
            try
            {
                GenericServer gs = null;
                foreach (GenericServer srv in _AvailableServers)
                {
                    if (srv.FServer.ID == ID)
                    {
                        gs = srv;
                        break;
                    }
                }
                if (gs != null)
                {
                    switch (col)
                    {
                        case 0: // Changed ID -- can't happen
                            break;
                        case 1: // Changed Address
                            gs.FServer.Address = newValue;
                            break;
                        case 2: // Changed User name
                            gs.FServer.UserName = newValue;
                            break;
                        case 3: // Changed Synthesis flag
                            gs.Synthesis = (String.Compare(newValue, "yes", true) == 0);
                            break;
                        case 4: // Changed Programming flag
                            gs.Programming = (String.Compare(newValue, "yes", true) == 0);
                            break;
                        case 5: // Changed Compile flag
                            gs.Compilation = (String.Compare(newValue, "yes", true) == 0);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard (Servers)");
                ErrorReporting.DebugException(ex);
            }
        }

        void PopulateServerList()
        {
            InitListViewEx();
            int row = 0;
            foreach (GenericServer srv in _AvailableServers)
            {
                AddToListView(row, srv);
                row++;
            }
        }
        void btnAddServer_Click(object sender, EventArgs e)
        {
            int newID = 0;
            foreach (ListViewItem lvi in lvServers.Items)
            {
                int val = 0;
                if (int.TryParse(lvi.Text, out val))
                {
                    if (val >= newID)
                        newID = val + 1;
                }
            }
            GenericServer ls = new GenericServer();
            ls.FServer = new FalconGlobal.FalconServer();
            ls.Synthesis = false;
            ls.Programming = false;
            ls.Compilation = false;
            ls.FServer.ID = newID.ToString();
            ls.FServer.UserName = "new_user";
            _AvailableServers.Add(ls);
            AddToListView(lvServers.Items.Count, ls);
        }
        void btnRemoveServer_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (ListViewItem lvi in lvServers.SelectedItems)
                {
                    GenericServer ls = null;
                    foreach (GenericServer srv in _AvailableServers)
                    {
                        if (srv.FServer.ID == lvi.Text)
                        {
                            ls = srv;
                            break;
                        }
                    }
                    if (ls != null)
                        _AvailableServers.Remove(ls);
                }
                PopulateServerList();
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard (Servers)");
                ErrorReporting.DebugException(ex);
            }
        }
        #endregion

        #region Step - Configure the Project Paths
        void btnLoadPaths_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = loadPathDialog.ShowDialog();
                this.Focus();
                if (dr == DialogResult.OK)
                {
                    _Paths = _ProjMan.DefaultPaths();
                    _Paths.LoadPaths(loadPathDialog.FileName, false);
                }
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard (Paths)");
                ErrorReporting.DebugException(ex);
            }
        }
        void btnOpenPathsDialog_Click(object sender, EventArgs e)
        {
            try
            {
                Dialogs.ProjectPathsDialog ppd = new Dialogs.ProjectPathsDialog(Paths);
                ppd.PopulateFromPathManager();
                DialogResult dr = ppd.ShowDialog(this);
                this.Focus();
                if (dr == DialogResult.OK)
                {
                    ppd.UpdatePathManager();
                }
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard (Paths)");
                ErrorReporting.DebugException(ex);
            }
        }
        void btnClearPaths_Click(object sender, EventArgs e)
        {
            Paths = _ProjMan.DefaultPaths();
        }
        #endregion

        #region Step - Save the New "Empty" Project
        /// <summary>
        /// Gets the list of servers that have been classified as Synthesis servers
        /// </summary>
        /// <returns>A list of servers that have been classified as Synthesis servers</returns>
        public List<FalconGlobal.FalconServer> GetSynthesisServers()
        {
            List<FalconGlobal.FalconServer> servers = new List<FalconGlobal.FalconServer>();
            foreach (GenericServer gs in _AvailableServers)
            {
                if (gs.Synthesis)
                    servers.Add(gs.FServer);
            }
            return servers;
        }
        /// <summary>
        /// Gets the list of servers that have been classified as Programming servers
        /// </summary>
        /// <returns>A list of servers that have been classified as Programming servers</returns>
        public List<FalconGlobal.FalconServer> GetProgrammingServers()
        {
            List<FalconGlobal.FalconServer> servers = new List<FalconGlobal.FalconServer>();
            foreach (GenericServer gs in _AvailableServers)
            {
                if (gs.Programming)
                    servers.Add(gs.FServer);
            }
            return servers;
        }
        /// <summary>
        /// Gets the list of servers that have been classified as Compilation servers
        /// </summary>
        /// <returns>A list of servers that have been classified as Compilation servers</returns>
        public List<FalconGlobal.FalconServer> GetCompilationServers()
        {
            List<FalconGlobal.FalconServer> servers = new List<FalconGlobal.FalconServer>();
            foreach (GenericServer gs in _AvailableServers)
            {
                if (gs.Compilation)
                    servers.Add(gs.FServer);
            }
            return servers;
        }
        void btnSaveLocation_Click(object sender, EventArgs e)
        {
            saveProjectFolderDialog.SelectedPath = _ProjMan.GetCerebrumProjectsPath();
            DialogResult dr = saveProjectFolderDialog.ShowDialog(this);
            this.Focus();
            if (dr == DialogResult.OK)
            {
                txtSaveLocation.Text = saveProjectFolderDialog.SelectedPath;
            }
        }
        void txtSaveLocation_TextChanged(object sender, EventArgs e)
        {
            if (currentStep == (int)WizardSteps.SaveStep)
            {
                btnNext.Enabled = (txtSaveLocation.Text.Trim() != string.Empty);
            }
        }        
        
        /// <summary>
        /// Saves the empty project in the wizard-specified project location
        /// </summary>
        /// <returns>True if the project was successfully saved; false otherwise</returns>
        public bool SaveProject()
        {
            try
            {
                if (Directory.Exists(SaveProjectLocation))
                {
                    int subItemCount = (Directory.GetFiles(SaveProjectLocation).Length + Directory.GetDirectories(SaveProjectLocation).Length);
                    if (subItemCount > 0)
                    {
                        DialogResult dr = MessageBox.Show(
                            String.Format("The directory '{0}' already exists.   If you save the new project in this location, all files/folders within it will be ERASED.  Are you sure you want to save the project here?", SaveProjectLocation),
                            "Confirm overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        this.Focus();
                        if (dr == DialogResult.Yes)
                        {
                            foreach (DirectoryInfo DI in new DirectoryInfo(SaveProjectLocation).GetDirectories())
                            {
                                DI.Delete(true);
                            }
                            foreach (FileInfo FI in new DirectoryInfo(SaveProjectLocation).GetFiles())
                            {
                                FI.Delete();
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                if (!Directory.Exists(SaveProjectLocation))
                    Directory.CreateDirectory(SaveProjectLocation);

                string PathsFile = String.Format("{0}\\paths.xml", SaveProjectLocation);
                string DesignFile = String.Format("{0}\\design.xml", SaveProjectLocation);
                string CommunicationsFile = String.Format("{0}\\comms.xml", SaveProjectLocation);
                string SynthServersFile = String.Format("{0}\\sservers.xml", SaveProjectLocation);
                string ProgServersFile = String.Format("{0}\\pservers.xml", SaveProjectLocation);
                string CompServersFile = String.Format("{0}\\cservers.xml", SaveProjectLocation);

                #region Saving paths.xml...
                // Defined in Global Install Paths File
                // Paths.SetPath("CerebrumRoot", @"C:\Program Files\Cerebrum");
                // Paths.SetPath("BinDirectory", @"${LocalProjectRoot}\bin");
                // Paths.SetPath("Platforms", @"${LocalProjectRoot}\Platforms");
                // Paths.SetPath("CerebrumCores", @"${LocalProjectRoot}\Cores");

                // Defined by selected platform
                Paths.SetPath("ProjectPlatform", _SelectedPlatform.Name);

                // Defined relative to project folder
                Paths.SetPath("LocalProjectRoot", SaveProjectLocation);
                Paths.SetPath("LocalProject", "${LocalProjectRoot}\\xps_projects");
                Paths.SetPath("LocalOutput", "${LocalProjectRoot}\\output_files");
                Paths.SetPath("ProjectTemp", "${LocalProjectRoot}\\temp_files");
                Paths.SetPath("CoreConfigs", "${LocalProjectRoot}\\core_config");


                // Inter-stage communication files used by back end flow
                Paths.SetPath("XPSMap", "${LocalProjectRoot}\\xpsmap.xml");
                Paths.SetPath("AddressMap", "${LocalProjectRoot}\\addressmap.xml");
                Paths.SetPath("RoutingTable", "${LocalProjectRoot}\\routing.xml");

                // Paths defined and/or specified by the user
                Hashtable h = Paths.GetAllPaths();
                foreach (string Key in h.Keys)
                {
                    Paths.SetPath(Key, (string)(h[Key]));
                }

                Paths.SavePaths(PathsFile);
                _ProjMan.PathManager = Paths;
                #endregion

                #region Saving sservers.xml, pservers.xml, cservers.xml ....

                XmlDocument xSynthServerDoc = new XmlDocument();
                xSynthServerDoc.AppendChild(xSynthServerDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement xSynthServerRoot = xSynthServerDoc.CreateElement("Servers");
                xSynthServerDoc.AppendChild(xSynthServerRoot);

                XmlDocument xProgServerDoc = new XmlDocument();
                xProgServerDoc.AppendChild(xProgServerDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement xProgServerRoot = xProgServerDoc.CreateElement("Servers");
                xProgServerDoc.AppendChild(xProgServerRoot);

                XmlDocument xCompileServerDoc = new XmlDocument();
                xCompileServerDoc.AppendChild(xCompileServerDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement xCompileServerRoot = xCompileServerDoc.CreateElement("Servers");
                xCompileServerDoc.AppendChild(xCompileServerRoot);

                foreach (GenericServer server in _AvailableServers)
                {
                    if (server.Synthesis)
                    {
                        server.FServer.WriteServerNode(xSynthServerRoot);
                    }
                    if (server.Compilation)
                    {
                        server.FServer.WriteServerNode(xProgServerRoot);
                    }
                    if (server.Programming)
                    {
                        server.FServer.WriteServerNode(xCompileServerRoot);
                    }
                }
                xSynthServerDoc.Save(SynthServersFile);
                xProgServerDoc.Save(ProgServersFile);
                xCompileServerDoc.Save(CompServersFile);
                #endregion

                #region Saving design.xml...
                // Skeleton design file
                XmlDocument xdDoc = new XmlDocument();
                xdDoc.AppendChild(xdDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement xdRoot = xdDoc.CreateElement("Design");
                xdDoc.AppendChild(xdRoot);
                xdRoot.AppendChild(xdDoc.CreateElement("Logic"));
                xdRoot.AppendChild(xdDoc.CreateElement("Connections"));
                xdRoot.AppendChild(xdDoc.CreateElement("Groups"));
                xdRoot.AppendChild(xdDoc.CreateElement("Processors"));
                xdRoot.AppendChild(xdDoc.CreateElement("Programming"));
                xdDoc.Save(DesignFile);
                #endregion

                #region Saving comms.xml...
                // Skeleton communications file
                XmlDocument xcmDoc = new XmlDocument();
                xcmDoc.AppendChild(xcmDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement xcmRoot = xcmDoc.CreateElement("Interfaces");
                xcmDoc.AppendChild(xcmRoot);
                xcmDoc.Save(CommunicationsFile);
                #endregion

                return true;
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard (Save)");
                ErrorReporting.DebugException(ex);
            }
            return false;
        }

        #endregion

        #region Panel Management
        void panelProgress_VisibleChanged(object sender, EventArgs e)
        {
            try
            {
                if ((panelProgress.Visible) && (currentPanel != null))
                {
                    int tag;
                    if (int.TryParse(currentPanel.Tag.ToString(), out tag))
                    {
                        foreach (CheckBox cb in panelProgress.Controls)
                        {
                            int checkNum = int.Parse(cb.Name.Replace("step", string.Empty));
                            if (checkNum < tag)
                            {
                                cb.ForeColor = Color.Green;
                                cb.Font = new Font(cb.Font, FontStyle.Regular);
                                cb.Checked = true;
                            }
                            else
                            {
                                if (checkNum == tag)
                                {
                                    cb.ForeColor = Color.Black;
                                    cb.Font = new Font(cb.Font, FontStyle.Bold);
                                }
                                else
                                {
                                    cb.ForeColor = Color.Maroon;
                                    cb.Font = new Font(cb.Font, FontStyle.Regular);
                                }
                                cb.Checked = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard");
                ErrorReporting.DebugException(ex);
            }
        }
        void LoadTaggedPanels()
        {
            try
            {
                int i = 0;
                int lastCount = -1;
                while (wizPanels.Count < (LastStep - FirstStep))
                {
                    foreach (Object o in this.Controls)
                    {
                        if (o.GetType() == typeof(Panel))
                        {
                            Panel p = (Panel)o;
                            int tag;
                            if (int.TryParse(p.Tag.ToString(), out tag))
                            {
                                if (tag == i)
                                {
                                    wizPanels.Add(p);
                                    p.Visible = false;
                                    i++;
                                }
                            }
                            else
                            {
                                if (p.Tag.ToString() == "global")
                                {
                                    globalPanel = p;
                                    p.Visible = false;
                                }
                                this.Controls.Remove(p);
                            }
                        }
                    }
                    if (lastCount == wizPanels.Count)
                    {
                        // Nothing changed
                        return;
                    }
                    else
                    {
                        // Something changed
                        lastCount = wizPanels.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard");
                ErrorReporting.DebugException(ex);
            }
        }
        void DisplayPanel(int step)
        {
            this.SuspendLayout();
            try
            {
                if (currentPanel != null)
                    currentPanel.Visible = false;

                currentPanel = wizPanels[step];


                currentPanel.Controls.Add(globalPanel);
                if (currentPanel != null)
                {
                    LayoutForm();
                    currentPanel.Visible = true;
                }
                PopulatePanel(step);
                globalPanel.Visible = true;
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard");
                ErrorReporting.DebugException(ex);
            }
            this.ResumeLayout();
        }
        void PopulatePanel(int step)
        {
            try
            {
                switch (step)
                {
                    case 0: // Welcome
                        return;
                    case 1: // Selecting Hardware Platform
                        LoadHardwarePlatformList();
                        return;
                    case 2: // Configure Servers
                        PopulateServerList();
                        return;
                    case 3: // Configure Paths
                        return;
                    case 4: // Save Project
                        btnNext.Enabled = (txtSaveLocation.Text.Trim() != string.Empty);
                        return;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard");
                ErrorReporting.DebugException(ex);
            }
        }
        void LayoutForm()
        {
            try
            {
                if (globalPanel != null)
                {
                    globalPanel.Location = new Point(0, 0);
                }
                foreach (Object o in this.Controls)
                {
                    if (o.GetType() == typeof(Panel))
                    {
                        Panel p = (Panel)o;
                        if (p != currentPanel)
                            p.Visible = false;
                        else
                            p.Visible = true;
                        p.Location = new Point(0, 0);
                        p.Size = new Size(this.ClientRectangle.Width, this.ClientRectangle.Height - (30 + btnCancel.Height));
                    }
                }

                int btnTop = this.ClientRectangle.Height - 15 - btnCancel.Height;
                btnCancel.Location = new Point(this.ClientRectangle.Width - 15 - btnCancel.Width, btnTop);
                btnNext.Location = new Point(btnCancel.Left - btnNext.Width - 30, btnTop);
                btnBack.Location = new Point(btnNext.Left - btnBack.Width - 15, btnTop);
            }
            catch (Exception ex)
            {
                _ProjMan.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "New Project Wizard");
                ErrorReporting.DebugException(ex);
            }
        }
        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(this, "Are you SURE you want to cancel?", "Cancel New Project Wizard", MessageBoxButtons.YesNo);
            this.Focus();
            if (dr == DialogResult.Yes)
            {
                Cancelled = true;
                End();
            }
        }
        void btnBack_Click(object sender, EventArgs e)
        {
            btnNext.Enabled = true;
            GoToPreviousStep();
        }
        void btnNext_Click(object sender, EventArgs e)
        {
            btnBack.Enabled = true;
            GoToNextStep();
        }
        #endregion

        /// <summary>
        /// Overrides tab-key handling to support 'tabbing-through' Server ListView
        /// </summary>
        /// <param name="forward">Indicates whether the tab is to move forward or backward</param>
        /// <returns>True if the tab key has been processed(consumed), false if not</returns>
        protected override bool ProcessTabKey(bool forward)
        {
            if (this.currentStep == (int)WizardSteps.ServersStep)
            {
                if (lvServers.Focused)
                {
                    bool result = false;
                    if (forward)
                    {
                        result = lvServers.TabForward();
                    }
                    else
                    {
                        result = lvServers.TabBackward();
                    }
                    if (result)
                    {
                        return true;
                    }
                }
            }
            return base.ProcessTabKey(forward);
        }

        #region ICerebrumWizard Interface Implementation
        /// <summary>
        /// Event fired when the wizard completes
        /// </summary>
        public event WizardCompleteHandlerDelegate WizardCompleteEvent;

        /// <summary>
        /// Property indicating whether the wizard was cancelled
        /// </summary>
        public bool Cancelled { get; set; }
        /// <summary>
        /// Property indicating whether the wizard resulted in an error
        /// </summary>
        public bool Error { get; set; }
        /// <summary>
        /// String indicating the error message, if any, generated by the wizard
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Method to start the wizard
        /// </summary>
        public void Start()
        {
            this.Show(ownerForm);
            currentStep = this.FirstStep;
            DisplayWizardStep();
        }
        /// <summary>
        /// Method to end the wizard
        /// </summary>
        public void End()
        {
            if ((!Cancelled) && (!Error))
            {
                if (!SaveProject())
                    return;
            }
            if (WizardCompleteEvent != null)
            {
                WizardCompleteEvent(this);
            }
            btnNext.Enabled = false;
            btnBack.Enabled = false;
            btnCancel.Enabled = false;
            this.Hide();
        }

        /// <summary>
        /// Reverts the wizard to the previous step
        /// </summary>
        public void GoToPreviousStep()
        {
            if (currentStep > (int)WizardSteps.InitialStep)
            {
                currentStep--;
            }
            if (currentStep == (int)WizardSteps.InitialStep)
            {
                btnBack.Enabled = false;
            }
            DisplayWizardStep();
        }
        /// <summary>
        /// Advances the wizard to the next step
        /// </summary>
        public void GoToNextStep()
        {
            if (currentStep < (int)WizardSteps.EndStep)
            {
                currentStep++;
            }
            DisplayWizardStep();
        }
        /// <summary>
        /// Forces the wizard to display the current step
        /// </summary>
        public void DisplayWizardStep()
        {
            if (currentStep > LastStep)
            {
                Error = true;
                ErrorMessage = String.Format("Unknown wizard step reached; Step {0}", currentStep);
                End();
                return;
            }
            else if (currentStep == LastStep)
            {
                SaveProjectLocation = txtSaveLocation.Text;
                End();
                return;
            }
            if (currentStep == (wizPanels.Count - 1))
            {
                btnNext.Text = "Finish";
            }
            else
            {
                btnNext.Text = "Next ->";
            }
            DisplayPanel(currentStep);
        }

        /// <summary>
        /// The PathManager object defining the project within(or for) which the wizard is running.
        /// </summary>
        public PathManager Paths
        {
            get
            {
                return _Paths;
            }
            set
            {
                _Paths = value;
            }
        }
        #endregion

    }
}
