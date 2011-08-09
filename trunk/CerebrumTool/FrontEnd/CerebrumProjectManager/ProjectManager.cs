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
 * ProjectManager.cs
 * Name: Matthew Cotter
 * Date: 13 Sep 2010 
 * Description: Overarching point-of-contact for the Cerebrum project.  This class manages design objects, saving and loading of project files, population and inter-class communication between
 * the Netron-based design UI, the Front End primary form, various design and project wizards, and the back end tool flows.
 * History: 
 * >> (29 May 2010) Matthew Cotter: Corrected implementation of OK/Cancel result on password dialog
 *                                      to allow entry to be properly cancelled without a failed login attempt
 * >> (28 May 2010) Matthew Cotter: Corrected bug that was causing inter-component clocking assignments produced by mapping to be invalidating prior to XPS Build.
 * >> (22 Mar 2011) Matthew Cotter: Modified implementation of install-time paths to reflect Registry-based location rather than a defaults text file.
 *                                  Implemented registry-based Most Recently Used (MRU) Projects List.
 * >> (18 Feb 2011) Matthew Cotter: Implemented simple project properties dialog.
 * >> ( 1 Feb 2011) Matthew Cotter: Corrected a bug that was causing an unhandled exception to be thrown when attempting to create a connection without an identified source or sink component.
 * >> (28 Jan 2011) Matthew Cotter: Added additional DirectoryInfo properties for other common local project paths (XPS Projects, Temp, Output, Core Config)
 * >> (27 Jan 2011) Matthew Cotter: Implemented Cut.
 *                                  Corrected bug in implementations of Copy/Paste (and Cut).
 *                                  Implemented hotkeys for rotation of Cores (Ctrl+[Shift+]R).
 *                                  Implemented hover-over tooltip displays for components (with enhanced tooltips by holding Shift when hovering.)
 * >> (26 Jan 2011) Matthew Cotter: Changed Core and Connection collections to Dictionaries to facilitate easier methods of checking whether an ID or Connection already exists
 *                                  Corrected bug that would allow Netron to attach a connection to an illegal port if the user manually hit the connector object.
 *                                  Added code to disallow duplicate connections (same source to same sink) from being created.  Any future designs with these types of redundant connections
 *                                      should have them removed when they are loaded.
 * >> (18 Jan 2011) Matthew Cotter: Added support for GUI-added platform components to be loaded, saved and visible within the design GUI.
 * >> (22 Dec 2010) Matthew Cotter: Added additional support for customizable clock management.
 * >> (16 Dec 2010) Matthew Cotter: Modified Save and Load of Layout to support saving internal port mappings for multiple-SAP/SOP port components.
 *                                  Corrected load/save layout to correctly load the size of cores as project is reloaded.
 *                                  Added support for context menu allowing Cut/Copy/Paste, Rotation and property modification of core(s).
 * >> ( 3 Nov 2010) Matthew Cotter: Added additional multi-threading support and messaging for access to Back End tools.
 * >> (24 Oct 2010) Matthew Cotter: Added initial multi-threading support and messaging for access to Back End tools.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Netron;
using Netron.Diagramming;
using Netron.Diagramming.Core;
using Netron.Diagramming.Win;
using FalconPathManager;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Threading;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Collections;
using System.Reflection;
using Microsoft.Win32;

using CerebrumNetronObjects;
using CerebrumProjectManager.Wizards;
using CerebrumSharedClasses;

using FalconXpsBuilder;
using FalconPlatformSynthesis;
using FalconJTAG_Programmer;
using FalconClockManager;

namespace CerebrumProjectManager
{
    /// <summary>
    /// Delegate that allows the ProgrammingManagerDialog to invoke the Programming Tool outside the ProjectManager
    /// </summary>
    public delegate void StartProgrammerCallback();

    /// <summary>
    /// Class used to manage and identify all aspects of the Cerebrum Project
    /// </summary>
    public class ProjectManager
    {
        #region Private Members
        private ProjectEventRecorder _Events;
        private PathManager _PathMan;
        private CerebrumPlatform _ProjectPlatform;

        private Dictionary<string, CerebrumCore> _Cores;
        private Dictionary<string, CerebrumConnection> _Connections;
        private List<IDiagramEntity> _OtherEntities;
        private List<CerebrumProcessor> _Processors;
        private List<CommunicationConfiguration> _CommConfigs;

        private System.Windows.Forms.ToolTip toolTips;

        //private Hashtable _InstanceCountTracker;

        private Form _CerebrumMainForm;
        private delegate void VoidDelegate();        
        private void FocusCerebrumForm()
        {
	        if (_CerebrumMainForm != null)
	        {
		        if (_CerebrumMainForm.InvokeRequired)
		        {
			        _CerebrumMainForm.Invoke(new VoidDelegate(FocusCerebrumForm));
		        }
		        else
		        {
			        _CerebrumMainForm.Focus();
		        }
	        }
        }
        /// <summary>
        /// Cerebrum Project Design Panel - responsible for loading and creating CerebrumCore objects
        /// </summary>
        internal CerebrumProjectPanel ProjectPanel;
        #endregion

        /// <summary>
        /// Creates a new Project Manager, associating it with the specified design panel control, and its owner form.
        /// </summary>
        /// <param name="CerebrumPanel">The design panel used to display and modify the design specified within the project.</param>
        public ProjectManager(CerebrumProjectPanel CerebrumPanel)
        {
            ProjectPanel = CerebrumPanel;
            _CerebrumMainForm = ProjectPanel.FindForm();

            PasswordRequest = new PasswordRequestDelegate(GetPassword);
            XPSRelay = new MessageEventDelegate(RelayXPSMessage);
            SynthesisRelay = new MessageEventDelegate(RelaySynthMessage);
            JProgrammerRelay = new MessageEventDelegate(RelayJProgMessage);

            _Events = new ProjectEventRecorder();

            toolTips = new System.Windows.Forms.ToolTip();

            AttachCerebrumPanelEventHandlers();
        }

        #region Properties
        private bool bProjectLoaded;
        /// <summary>
        /// Indicates whether a project is currently loaded
        /// </summary>
        public bool ProjectLoaded
        {
            get
            {
                return bProjectLoaded;
            }
        }
        private bool bProjectSaved;
        /// <summary>
        /// Indicates whether the currently loaded project has been saved
        /// </summary>
        public bool ProjectSaved
        {
            get
            {
                if (bProjectLoaded)
                    return bProjectSaved;
                else
                    return true;
            }
        }

        private string _LoadedProjectDir;
        /// <summary>
        /// Get the DirectoryInfo specifying the location of the loaded project's directory
        /// </summary>
        public DirectoryInfo ProjectDirectory
        {
            get
            {
                if (Directory.Exists(_LoadedProjectDir))
                    return new DirectoryInfo(_LoadedProjectDir);
                else 
                    return null;
            }
        }
        /// <summary>
        /// Get the DirectoryInfo specifying the location of the loaded project's temporary files directory
        /// </summary>
        public DirectoryInfo ProjectTempDirectory
        {
            get
            {
                if (this.ProjectDirectory != null)
                {
                    string Dir = _PathMan["ProjectTemp"];
                    return new DirectoryInfo(Dir);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Get the DirectoryInfo specifying the location of the loaded project's output files directory
        /// </summary>
        public DirectoryInfo ProjectOutputDirectory
        {
            get
            {
                if (this.ProjectDirectory != null)
                {
                    string Dir = _PathMan["LocalOutput"];
                    return new DirectoryInfo(Dir);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Get the DirectoryInfo specifying the location of the loaded project's XPS projects directory
        /// </summary>
        public DirectoryInfo ProjectXPSDirectory
        {
            get
            {
                if (this.ProjectDirectory != null)
                {
                    string Dir = _PathMan["LocalProject"];
                    return new DirectoryInfo(Dir);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Get the DirectoryInfo specifying the location of the loaded project's Core Configuration directory
        /// </summary>
        public DirectoryInfo ProjectCoreConfigDirectory
        {
            get
            {
                if (this.ProjectDirectory != null)
                {
                    string Dir = _PathMan["CoreConfigs"];
                    return new DirectoryInfo(Dir);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Get the DirectoryInfo specifying the location of the loaded project's PE Source directory
        /// </summary>
        public DirectoryInfo ProjectPESourceDirectory
        {
            get
            {
                if (this.ProjectDirectory != null)
                {
                    string Dir = String.Format("{0}\\pe_source", this.ProjectDirectory.FullName);
                    return new DirectoryInfo(Dir);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the FileInfo specifying the location of the loaded project's Paths file
        /// </summary>
        public FileInfo ProjectPathsFile
        {
            get
            {
                return new FileInfo(String.Format("{0}\\paths.xml", _LoadedProjectDir));
            }
        }
        /// <summary>
        /// Get the FileInfo specifying the location of the loaded project's Design file
        /// </summary>
        public FileInfo DesignFile
        {
            get
            {
                return new FileInfo(String.Format("{0}\\design.xml", _LoadedProjectDir));
            }
        }
        /// <summary>
        /// Get the FileInfo specifying the location of the loaded project's Communications file
        /// </summary>
        public FileInfo CommunicationsFile
        {
            get
            {
                return new FileInfo(String.Format("{0}\\comms.xml", _LoadedProjectDir));
            }
        }
        /// <summary>
        /// Get the FileInfo specifying the location of the loaded project's Synthesis Servers file
        /// </summary>
        public FileInfo SynthesisServersFile
        {
            get
            {
                return new FileInfo(String.Format("{0}\\sservers.xml", this.ProjectDirectory.FullName));
            }
        }
        /// <summary>
        /// Get the FileInfo specifying the location of the loaded project's Programming Servers file
        /// </summary>
        public FileInfo ProgrammingServersFile
        {
            get
            {
                return new FileInfo(String.Format("{0}\\pservers.xml", _LoadedProjectDir));
            }
        }
        /// <summary>
        /// Get the FileInfo specifying the location of the loaded project's Compilation Servers file
        /// </summary>
        public FileInfo CompilationServersFile
        {
            get
            {
                return new FileInfo(String.Format("{0}\\cservers.xml", _LoadedProjectDir));
            }
        }
        /// <summary>
        /// Get the FileInfo specifying the location of the loaded project's Cerebrum GUI Layout file
        /// </summary>
        public FileInfo CerebrumGUILayoutFile
        {
            get
            {
                return new FileInfo(String.Format("{0}\\cerebrum_gui.xml", _LoadedProjectDir));
            }
        }
        /// <summary>
        /// Get the PathManager specifying the project paths of the loaded project
        /// </summary>
        public PathManager PathManager
        {
            get
            {
                return _PathMan;
            }
            set
            {
                _PathMan = value;
            }
        }

        /// <summary>
        /// Static method used to locate, load, and return a PathManager object with only the default Cerebrum paths loaded.
        /// </summary>
        /// <returns></returns>
        public PathManager DefaultPaths()
        {
            PathManager PM = new PathManager();
            PM.Clear();
            //this.SetInstallTimePaths(PM);
            return PM;
        }
        /// <summary>
        /// Sets the Install-time paths for this instance of Cerebrum in the specified PathManager
        /// </summary>
        /// <param name="PM">The path manager to be modified.</param>
        private void SetInstallTimePaths(PathManager PM)
        {
            PM.SetPath("CerebrumRoot", this.GetCerebrumInstallPath());
            PM.SetPath("BinDirectory", this.GetCerebrumBinPath());
            PM.SetPath("Platforms", this.GetCerebrumPlatformsPath());
            PM.SetPath("CerebrumCores", this.GetCerebrumCoresPath());
        }

        /// <summary>
        /// Get the Cerebrum Platform associated with the loaded project
        /// </summary>
        public CerebrumPlatform ProjectPlatform
        {
            get
            {
                return _ProjectPlatform;
            }
        }
        internal List<CerebrumProcessor> Processors
        {
            get
            {
                return _Processors;
            }
        }
        internal List<CommunicationConfiguration> CommunicationInterfaces
        {
            get
            {
                return _CommConfigs;
            }
        }

        /// <summary>
        /// Gets the CerebrumCore with the specified instance, if it exists in the project
        /// </summary>
        /// <param name="InstanceName">The name of the instance of the core to get</param>
        /// <returns>The CerebrumCore object representing the component with the specified instance name</returns>
        public CerebrumCore GetCoreInstance(string InstanceName)
        {
            foreach (CerebrumCore CC in _Cores.Values)
            {
                if (String.Compare(CC.CoreInstance, InstanceName, true) == 0)
                {
                    return CC;
                }
            }
            foreach (CerebrumCore CC in this.ProjectPlatform.RequiredCores)
            {
                if (String.Compare(CC.CoreInstance, InstanceName, true) == 0)
                {
                    return CC;
                }
            }
            return null;
        }
        #endregion

        #region Project Events
        /// <summary>
        /// Delegate that defines a simple state change in the project
        /// </summary>
        public delegate void ProjectStatusChangedHandler();

        /// <summary>
        /// Event that is fired whenever a project is opened
        /// </summary>
        public event ProjectStatusChangedHandler ProjectOpened;
        /// <summary>
        /// Event that is fired whenever a project is closed
        /// </summary>
        public event ProjectStatusChangedHandler ProjectClosed;
        #endregion

        #region Save/Load Projects
        /// <summary>
        /// Creates a new Cerebrum Project, either entirely empty or by invoking the new project wizard. (Currently only the wizard is supported)
        /// </summary>
        /// <param name="StartWizard">Indicates whether the project wizard should be invoked to create the new project. (Currently only the wizard is supported)</param>
        public void NewProject(bool StartWizard)
        {
            try
            {
                myMessages.RaiseMessageEvent(MessageEventType.Clear, string.Empty, string.Empty, string.Empty);
                if (this.ProjectLoaded)
                {
                    if (!CloseProject())
                        return;
                }
                ResetProject();

                _PathMan = this.DefaultPaths();
                //if (StartWizard)
                //{
                    myMessages.RaiseMessageEvent(MessageEventType.Console, "New Project", String.Format("Started New Project Wizard..."), "Project Manager");
                    frmNewProjectWizard newWiz = new frmNewProjectWizard(_CerebrumMainForm, this);
                    newWiz.WizardCompleteEvent += new WizardCompleteHandlerDelegate(newWiz_WizardCompleteEvent);
                    newWiz.Start();
                //}
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.NewProject");
                ErrorReporting.DebugException(ex);
            }
        }
        void newWiz_WizardCompleteEvent(ICerebrumWizard wiz)
        {
            try
            {
                FocusCerebrumForm();
                frmNewProjectWizard newWiz = (frmNewProjectWizard)wiz;
                if (newWiz.Error || newWiz.Cancelled)
                {
                    if (newWiz.Error)
                    {
                        myMessages.RaiseMessageEvent(MessageEventType.Console, "New Project", String.Format("Cancelled New Project Wizard..."), "Project Manager");
                    }
                    _PathMan.Clear();
                    return;
                }
                myMessages.RaiseMessageEvent(MessageEventType.Console, "New Project", String.Format("Successfully created Project {0}", newWiz.ProjectLocation), "Project Manager");

                _Cores = new Dictionary<string, CerebrumCore>();
                _Connections = new Dictionary<string, CerebrumConnection>();
                _OtherEntities = new List<IDiagramEntity>();
                //_InstanceCountTracker = new Hashtable();

                _Cores.Clear();
                _Connections.Clear();
                _OtherEntities.Clear();
                //_InstanceCountTracker.Clear();
                _LoadedProjectDir = newWiz.ProjectLocation;

                SaveProject();
                OpenProject(newWiz.ProjectLocation);
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.NewProjectWizard_Complete");
                ErrorReporting.DebugException(ex);
            }
        }
        /// <summary>
        /// Opens an existing project, located in the specified folder
        /// </summary>
        /// <param name="ProjectFolder">The folder containing the project to be opened.</param>
        public void OpenProject(string ProjectFolder)
        {
            try
            {
                myMessages.RaiseMessageEvent(MessageEventType.Clear, string.Empty, string.Empty, string.Empty);
                ResetProject();
                if (LoadPaths(ProjectFolder))
                {

                    // Load the diagram
                    _LoadedProjectDir = ProjectFolder;
                    ProjectPanel.NewDiagram(ProjectFolder);
                    AttachDiagramEventHandlers();
                    CreateProjectDirectories();

                    // Override the built-in Connection Tool
                    ProjectPanel.Diagram.Controller.AddOverrideTool(new CerebrumConnectionTool(Netron.Diagramming.Core.ControllerBase.ConnectionToolName));

                    // Scan core repositories from the Paths file
                    string CerebrumCoresPath = _PathMan["CerebrumCores"];
                    ProjectPanel.ScanRepository(CerebrumCoresPath);
                    if (_PathMan.HasPath("CoreSearchPaths"))
                    {
                        string[] OtherSearchPaths = _PathMan["CoreSearchPaths"].Split(';');
                        for (int i = 0; i < OtherSearchPaths.Length; i++)
                        {
                            string OtherSearchPath = OtherSearchPaths[i].Trim();
                            if (OtherSearchPath == string.Empty)
                                continue;
                            if (Directory.Exists(OtherSearchPath))
                            {
                                ProjectPanel.ScanRepository(OtherSearchPath);
                            }
                        }
                    }

                    // Load the Platform
                    _ProjectPlatform.LoadProjectPlatform(_PathMan);

                    // Load the Design
                    if (!LoadDesignAndLayout())
                    {
                        myMessages.RaiseMessageEvent(MessageEventType.Error, "Open Design/Layout", String.Format("Failed to open design/layout in {0}", ProjectFolder), "Project Manager");
                        ResetProject();
                        return;
                    }
                    foreach (CerebrumCore cCore in _Cores.Values)
                    {
                        cCore.LoadCoreConfigs(this.ProjectDirectory.FullName);
                    }

                    ValidateClockAssignments();

                    // Servers and Communication can be deferred on an as-needed basis

                    ProjectPanel.ShowLibrary();
                    ProjectPanel.ExpandAll();

                    LoadCommunications();

                    CreateProjectDirectories();
                    _Events.Open(_PathMan);
                    bProjectLoaded = true;
                    bProjectSaved = true;
                    if (ProjectOpened != null)
                        ProjectOpened();
                    myMessages.RaiseMessageEvent(MessageEventType.Console, "Open Project", String.Format("Successfully opened Project {0}", this.ProjectDirectory), "Project Manager");
                    this.SetMRU(this.ProjectPathsFile.FullName);
                }
                else
                {
                    myMessages.RaiseMessageEvent(MessageEventType.Console, "Open Project Paths", String.Format("Failed to open Project {0}", ProjectFolder), "Project Manager");
                }
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.OpenProject");
                ErrorReporting.DebugException(ex);
            }
        }

        /// <summary>
        /// Creates the project Temp, Output, XPS and CoreConfig directories, if they don't already exist
        /// </summary>
        private void CreateProjectDirectories()
        {
            if (this.ProjectDirectory != null)
            {
                if (!this.ProjectTempDirectory.Exists)
                    this.ProjectTempDirectory.Create();
                if (!this.ProjectOutputDirectory.Exists)
                    this.ProjectOutputDirectory.Create();
                if (!this.ProjectTempDirectory.Exists)
                    this.ProjectTempDirectory.Create();
                if (!this.ProjectXPSDirectory.Exists)
                    this.ProjectXPSDirectory.Create();
                if (!this.ProjectCoreConfigDirectory.Exists)
                    this.ProjectCoreConfigDirectory.Create();
                if (!this.ProjectPESourceDirectory.Exists)
                    this.ProjectPESourceDirectory.Create();
            }
        }
        /// <summary>
        /// Saves the currently loaded project, including design, layout, servers, paths, and communication files.
        /// </summary>
        public void SaveProject()
        {
            try
            {
                CreateProjectDirectories();
                SavePaths();
                SaveDesignAndLayout();
                SaveComponentConfigs();
                SaveCommunications();
                bProjectSaved = true;
                myMessages.RaiseMessageEvent(MessageEventType.Console, "Save Project", String.Format("Successfully saved Project {0}", this.ProjectDirectory), "Project Manager");
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.SaveProject");
            }
        }
        /// <summary>
        /// Loads the relevant project paths from the project file, setting local-machine-specific paths to the appropriate values.
        /// </summary>
        /// <param name="ProjectFolder">The folder from which the project files are to be loaded.</param>
        /// <returns>True if the paths were successfully loaded and set, false otherwise.</returns>
        private bool LoadPaths(string ProjectFolder)
        {
            try
            {
                // Load the Paths
                _PathMan.LoadPaths(String.Format("{0}\\paths.xml", ProjectFolder));
                return true;
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                myMessages.RaiseMessageEvent(MessageEventType.Error, "LoadProject", String.Format("Error loading project file from '{0}'", ProjectFolder), "ProjectManager");
                _PathMan.Clear();
            }
            return false;
        }

        /// <summary>
        /// Saves the currently loaded project paths only
        /// </summary>
        public void SavePaths()
        {
            try
            {
                PathManager.SavePaths(this.ProjectDirectory.FullName + "\\paths.xml");
                myMessages.RaiseMessageEvent(MessageEventType.Console, "Save Paths", String.Format("Saved Paths for Project {0}", this.ProjectDirectory), "Project Manager");
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.SavePaths");
                ErrorReporting.DebugException(ex);
            }
        }
        /// <summary>
        /// Closes the currently loaded project and resets any references or pointers to files or objects
        /// </summary>
        /// <returns></returns>
        public bool CloseProject()
        {
            try
            {
                // Check if saved
                if (!this.ProjectSaved)
                {
                    DialogResult saveProject = MessageBox.Show("The current project has unsaved changes, do you want to save them before closing?",
                                                       "Confirm Save Before Close",
                                                       MessageBoxButtons.YesNoCancel,
                                                       MessageBoxIcon.Question);
                    FocusCerebrumForm();
                    if (saveProject == DialogResult.Cancel)
                        return false;
                    if (saveProject == DialogResult.Yes)
                    {
                        SaveProject();
                    }
                }
                ResetProject();
                _Events.Close();
                if (ProjectClosed != null)
                    ProjectClosed();
                myMessages.RaiseMessageEvent(MessageEventType.Console, "Close Project", String.Format("Closed Project Project {0}", this.ProjectDirectory), "Project Manager");
                myMessages.RaiseMessageEvent(MessageEventType.Clear, string.Empty, string.Empty, string.Empty);
                return true;
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.CloseProject");
                ErrorReporting.DebugException(ex);
            }
            return false;
        }

        private void ResetProject()
        {
            ProjectPanel.RemoveDiagram();

            _PathMan = new PathManager();
            if (_ProjectPlatform != null)
                _ProjectPlatform.Dispose();
            _ProjectPlatform = new CerebrumPlatform();
            _ProjectPlatform.ProjectManager = this;
            _Cores = new Dictionary<string, CerebrumCore>();
            _Processors = new List<CerebrumProcessor>();
            _Connections = new Dictionary<string, CerebrumConnection>();
            _OtherEntities = new List<IDiagramEntity>();
            _CommConfigs = new List<CommunicationConfiguration>();
            //_InstanceCountTracker = new Hashtable();

            _PathMan.Clear();
            _LoadedProjectDir = string.Empty;

            bProjectLoaded = false;
            bProjectSaved = true;
        }

        #region Save/Load Project Files
        private bool LoadDesignAndLayout()
        {
            try
            {
                if (ProjectPanel.Diagram == null)
                    return false;
                if (!CerebrumGUILayoutFile.Exists)
                    return false;

                XmlDocument xGuiDoc = new XmlDocument();
                xGuiDoc.Load(CerebrumGUILayoutFile.FullName);
                XmlNode xGUICores = xGUICores = CerebrumXmlInterface.GetXmlNode(xGuiDoc, "CerebrumGUI.Cores");

                XmlDocument xDesDoc = new XmlDocument();
                xDesDoc.Load(DesignFile.FullName);
                XmlNode xLogic = CerebrumXmlInterface.GetXmlNode(xDesDoc, "Design.Logic");

                List<CerebrumCore> LoadedCores = new List<CerebrumCore>();
                foreach (XmlNode xCore in xLogic.ChildNodes)
                {
                    string CoreType = CerebrumXmlInterface.GetXmlAttribute(xCore, "", "Name");
                    string CoreVer = CerebrumXmlInterface.GetXmlAttribute(xCore, "", "Version");
                    string CoreInstance = CerebrumXmlInterface.GetXmlAttribute(xCore, "", "ID");
                    CerebrumCore loadedCore = null;

                    // Check to see if it is a Platform Core
                    // If it is, clone the core from the platform specification
                    bool bPlatformCore = false;
                    foreach (CerebrumCore CC in this.ProjectPlatform.RequiredCores)
                    {
                        if (CC.VisibleInDesign)
                        {
                            if (String.Compare(CC.CoreInstance, CoreInstance, true) == 0)
                            {
                                bPlatformCore = true;
                                loadedCore = new CerebrumCore(CC.CoreInstance, CC);
                                break;
                            }
                        }
                    }
                    if (!bPlatformCore)
                    {
                        // If it is not a platform core, clone it from the library
                        loadedCore = ProjectPanel.LibraryTab.CreateCoreInstance(CoreInstance, CoreType, CoreVer);
                    }
                    if (loadedCore == null)
                    {
                        // Specified core was not in the platform, and could not be loaded from the library!!!  This is a major ERROR!
                        string ErrMsg = String.Format("Error loading design core: {0} v{1}.", CoreType, CoreVer);
                        myMessages.RaiseMessageEvent(MessageEventType.Error, "CoreError", ErrMsg, "Load Design");
                        MessageBox.Show(ErrMsg);
                        FocusCerebrumForm();
                        return false;
                    }
                    else
                    {
                        string InfoMsg = String.Format("Loaded design core: {0} v{1} from {2}.", CoreType, CoreVer, (bPlatformCore ? "Platform" : "Design"));
                        myMessages.RaiseMessageEvent(MessageEventType.Info, "CoreLoaded", InfoMsg, "Load Design");
                    }
                    // Load the locations from the GUI File
                    XmlNode xCoreLoc = CerebrumXmlInterface.GetXmlNode(xGUICores, loadedCore.CoreInstance);
                    if (xCoreLoc != null)
                    {
                        foreach (XmlAttribute xAttr in xCoreLoc.Attributes)
                        {
                            if (String.Compare(xAttr.Name, "X", true) == 0)
                            {
                                loadedCore.Location = new Point(int.Parse(xAttr.Value), loadedCore.Location.Y);
                            }
                            else if (String.Compare(xAttr.Name, "Y", true) == 0)
                            {
                                loadedCore.Location = new Point(loadedCore.Location.X, int.Parse(xAttr.Value));
                            }
                            else if (String.Compare(xAttr.Name, "Width", true) == 0)
                            {
                                int w = int.Parse(xAttr.Value);
                                loadedCore.MinSize = new Size(w, loadedCore.MinSize.Height);
                                loadedCore.MaxSize = loadedCore.MinSize;
                                loadedCore.Width = w;
                            }
                            else if (String.Compare(xAttr.Name, "Height", true) == 0)
                            {
                                int h = int.Parse(xAttr.Value);
                                loadedCore.MinSize = new Size(loadedCore.MinSize.Width, h);
                                loadedCore.MaxSize = loadedCore.MinSize;
                                loadedCore.Height = h;
                            }
                            else if (String.Compare(xAttr.Name, "Rotation", true) == 0)
                            {
                                int r = int.Parse(xAttr.Value);
                                loadedCore.RotationAngle = r;
                            }
                        }
                    }
                    else
                    {
                        string ErrMsg = String.Format("Unable to locate design document coordinates for component '{0}' in {1}", loadedCore.CoreInstance, CerebrumGUILayoutFile.Name);
                        myMessages.RaiseMessageEvent(MessageEventType.Warning, "Design Warning", ErrMsg, "Load Design");
                    }
                    LoadedCores.Add(loadedCore);
                    ProjectPanel.Diagram.AddShape(loadedCore);
                }

                // Load Platform-specific cores that have not been loaded from the design already
                foreach (CerebrumCore CC in this.ProjectPlatform.RequiredCores)
                {
                    if (CC.VisibleInDesign)
                    {
                        bool bFoundMatch = false;
                        foreach (CerebrumCore loadedCore in LoadedCores)
                        {
                            if (String.Compare(CC.CoreInstance, loadedCore.CoreInstance, true) == 0)
                            {
                                // This core, having already been loaded, should have been matched while the design was loaded.
                                bFoundMatch = true;
                                break;
                            }
                        }
                        if (!bFoundMatch)
                        {
                            CerebrumCore loadedCore = new CerebrumCore(CC.CoreInstance, CC);
                            LoadedCores.Add(loadedCore);
                            ProjectPanel.Diagram.AddShape(loadedCore);
                        }
                    }
                }

                XmlNode xConnections = CerebrumXmlInterface.GetXmlNode(xDesDoc, "Design.Connections");
                XmlNode xGUIConnections = CerebrumXmlInterface.GetXmlNode(xGuiDoc, "CerebrumGUI.Connections");

                foreach (XmlNode xConn in xConnections.ChildNodes)
                {
                    string ConnID = CerebrumXmlInterface.GetXmlAttribute(xConn, "", "ID");
                    string ConnName = CerebrumXmlInterface.GetXmlAttribute(xConn, "", "Name");
                    string CoreSourceID = CerebrumXmlInterface.GetXmlAttribute(xConn, "", "Source");
                    string CoreSinkID = CerebrumXmlInterface.GetXmlAttribute(xConn, "", "Sink");

                    CerebrumCore SourceCore = null;
                    CerebrumCore SinkCore = null;
                    string SourcePortName = string.Empty;
                    string SinkPortName = string.Empty;
                    string SourcePortCore = string.Empty;
                    string SinkPortCore = string.Empty;
                    CoreConnector SourcePort = null;
                    CoreConnector SinkPort = null;

                    foreach (CerebrumCore CC in LoadedCores)
                    {
                        if (CC.CoreInstance == CoreSourceID)
                            SourceCore = CC;
                        if (CC.CoreInstance == CoreSinkID)
                            SinkCore = CC;
                        if ((SourceCore != null) && (SinkCore != null))
                            break;
                    }
                    if ((SourceCore == null) || (SinkCore == null))
                        continue;

                    // Load the ports from the GUI File
                    XmlNode xCorePorts = CerebrumXmlInterface.GetXmlNode(xGUIConnections, ConnID);
                    if (xCorePorts != null)
                    {
                        foreach (XmlAttribute xAttr in xCorePorts.Attributes)
                        {
                            if (String.Compare(xAttr.Name, "SourcePort", true) == 0)
                            {
                                SourcePortName = xAttr.Value;
                            }
                            else if (String.Compare(xAttr.Name, "SinkPort", true) == 0)
                            {
                                SinkPortName = xAttr.Value;
                            }
                            if (String.Compare(xAttr.Name, "SourceCore", true) == 0)
                            {
                                SourcePortCore = xAttr.Value;
                            }
                            else if (String.Compare(xAttr.Name, "SinkCore", true) == 0)
                            {
                                SinkPortCore = xAttr.Value;
                            }
                        }
                        if ((SourcePortName == null) || (SinkPortName == null))
                            continue;

                        SourcePort = SourceCore.GetPortByName(SourcePortName, SourcePortCore);
                        SinkPort = SinkCore.GetPortByName(SinkPortName, SinkPortCore);
                        if ((SourcePort == null) || (SinkPort == null))
                            continue;
                        ProjectPanel.CreateConnection(SourcePort, SinkPort);
                    }
                    else
                    {
                        string ErrMsg = String.Format("Unable to locate design document coordinates for connection '{0}' in {1}", ConnName, CerebrumGUILayoutFile.Name);
                        myMessages.RaiseMessageEvent(MessageEventType.Warning, "Design Warning", ErrMsg, "Load Design");
                    }
                }

                XmlNode xProcs = CerebrumXmlInterface.GetXmlNode(xDesDoc, "Design.Processors");
                if (xProcs != null)
                    LoadProcessors(xProcs);

                XmlNode xProg = CerebrumXmlInterface.GetXmlNode(xDesDoc, "Design.Programming");
                if (xProg != null)
                    LoadProgramming(xProg);

                ProjectPanel.ShowLibrary();
                return true;
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                string ErrMsg = String.Format("Error Loading Design '{0}', Layout '{1}'", DesignFile.Name, CerebrumGUILayoutFile.Name);
                myMessages.RaiseMessageEvent(MessageEventType.Warning, "CoreError", ErrMsg, "Load Design");
            }
            return false;
        }
        private void SaveDesignAndLayout()
        {
            SaveDesign();
            SaveLayout();
        }

        private void SaveDesign()
        {
            // Prepare the Design.XML File
            XmlDocument xDesignDoc = new XmlDocument();
            xDesignDoc.AppendChild(xDesignDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement xDesignRoot = xDesignDoc.CreateElement("Design");
            xDesignDoc.AppendChild(xDesignRoot);
            XmlElement xLogic = xDesignDoc.CreateElement("Logic");
            xDesignRoot.AppendChild(xLogic);
            // Iterate through the Cores
            foreach (CerebrumCore CC in _Cores.Values)
            {
                // Populate the corresponding Design Doc Element
                {
                    StringBuilder Arches = new StringBuilder();
                    foreach (string Arch in CC.SupportedArchitectures)
                    {
                        Arches.Append(String.Format("{0}{1}", Arch, (CC.SupportedArchitectures.Count > 1 ? "," : string.Empty)));
                    }

                    XmlElement xLogicCore = xDesignDoc.CreateElement("Core");
                    xLogicCore.SetAttribute("ID", CC.CoreInstance);
                    xLogicCore.SetAttribute("Name", CC.CoreType);
                    xLogicCore.SetAttribute("Location", CC.CoreLocation);
                    xLogicCore.SetAttribute("OwnerName", CC.CoreOwner);
                    xLogicCore.SetAttribute("Version", CC.CoreVersion);
                    xLogicCore.SetAttribute("Server", CC.CoreServer.ToString());
                    xLogicCore.SetAttribute("SupportedArch", Arches.ToString());

                    //foreach (CoreResource CR in CC.Resources)
                    //{
                    //    XmlElement xResource = xDesignDoc.CreateElement("Resource");
                    //    XmlAttribute xAttrResName = xDesignDoc.CreateAttribute("Name");
                    //    XmlAttribute xAttrResAmount = xDesignDoc.CreateAttribute("Value");

                    //    xAttrResName.Value = CR.Name;
                    //    xAttrResAmount.Value = CR.Amount.ToString();

                    //    xResource.Attributes.Append(xAttrResName);
                    //    xResource.Attributes.Append(xAttrResAmount);
                    //    xLogicCore.AppendChild(xResource);
                    //}

                    xLogic.AppendChild(xLogicCore);
                }
            }

            XmlElement xConnections = xDesignDoc.CreateElement("Connections");
            xDesignRoot.AppendChild(xConnections);

            foreach (CerebrumConnection c in _Connections.Values)
            {
                CoreConnector SourcePort = ((CoreConnector)(c.From.AttachedTo));
                CoreConnector SinkPort = ((CoreConnector)(c.To.AttachedTo));
                CerebrumCore SourceCore = SourcePort.Core;
                CerebrumCore SinkCore = SinkPort.Core;

                string ConnID = c.Name;
                // Populate the corresponding Design Doc Element
                {
                    XmlElement xConnection = xDesignDoc.CreateElement("Connection");
                    XmlAttribute xAttrID = xDesignDoc.CreateAttribute("ID");
                    XmlAttribute xAttrName = xDesignDoc.CreateAttribute("Name");
                    XmlAttribute xAttrSource = xDesignDoc.CreateAttribute("Source");
                    XmlAttribute xAttrSourceInstance = xDesignDoc.CreateAttribute("SourceInstance");
                    XmlAttribute xAttrSink = xDesignDoc.CreateAttribute("Sink");
                    XmlAttribute xAttrSinkInstance = xDesignDoc.CreateAttribute("SinkInstance");

                    xAttrID.Value = ConnID;
                    xAttrName.Value = ConnID;
                    xAttrSource.Value = SourceCore.CoreInstance;
                    xAttrSourceInstance.Value = SourcePort.CoreInstance;
                    xAttrSink.Value = SinkCore.CoreInstance;
                    xAttrSinkInstance.Value = SinkPort.CoreInstance;

                    xConnection.Attributes.Append(xAttrID);
                    xConnection.Attributes.Append(xAttrName);
                    xConnection.Attributes.Append(xAttrSource);
                    xConnection.Attributes.Append(xAttrSourceInstance);
                    xConnection.Attributes.Append(xAttrSink);
                    xConnection.Attributes.Append(xAttrSinkInstance);

                    xConnections.AppendChild(xConnection);
                }
            }

            xDesignRoot.AppendChild(xDesignDoc.CreateElement("Groups"));
            XmlElement xProcessors = xDesignDoc.CreateElement("Processors");
            SaveProcessors(xProcessors);
            xDesignRoot.AppendChild(xProcessors);
            XmlElement xProgramming = xDesignDoc.CreateElement("Programming");
            SaveProgramming(xProgramming);
            xDesignRoot.AppendChild(xProgramming);

            xDesignDoc.Save(DesignFile.FullName);
        }
        private void SaveLayout()
        {
            // Prepare the Cerebrum.GUI File
            XmlDocument xGUIDoc = new XmlDocument();
            xGUIDoc.AppendChild(xGUIDoc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement xGUIRoot = xGUIDoc.CreateElement("CerebrumGUI");
            xGUIDoc.AppendChild(xGUIRoot);
            XmlElement xGUICores = xGUIDoc.CreateElement("Cores");
            xGUIRoot.AppendChild(xGUICores);

            // Iterate through the Cores
            foreach (CerebrumCore CC in _Cores.Values)
            {
                // Populate the GUI Layout Element
                {
                    XmlElement xCoreLoc = xGUIDoc.CreateElement(CC.CoreInstance);
                    XmlAttribute xLocX = xGUIDoc.CreateAttribute("X");
                    XmlAttribute xLocY = xGUIDoc.CreateAttribute("Y");
                    XmlAttribute xLocW = xGUIDoc.CreateAttribute("Width");
                    XmlAttribute xLocH = xGUIDoc.CreateAttribute("Height");
                    XmlAttribute xLocR = xGUIDoc.CreateAttribute("Rotation");

                    xLocX.Value = CC.Rectangle.Left.ToString();
                    xLocY.Value = CC.Rectangle.Top.ToString();
                    xLocW.Value = CC.Rectangle.Width.ToString();
                    xLocH.Value = CC.Rectangle.Height.ToString();
                    xLocR.Value = CC.RotationAngle.ToString();

                    xCoreLoc.Attributes.Append(xLocX);
                    xCoreLoc.Attributes.Append(xLocY);
                    xCoreLoc.Attributes.Append(xLocW);
                    xCoreLoc.Attributes.Append(xLocH);
                    xCoreLoc.Attributes.Append(xLocR);
                    xGUICores.AppendChild(xCoreLoc);
                }
            }

            XmlElement xGUIConnections = xGUIDoc.CreateElement("Connections");
            xGUIRoot.AppendChild(xGUIConnections);

            foreach (CerebrumConnection c in _Connections.Values)
            {
                CerebrumCore SourceCore = ((CoreConnector)(c.From.AttachedTo)).Core;
                CerebrumCore SinkCore = ((CoreConnector)(c.To.AttachedTo)).Core;

                string ConnID = c.Name;
                // Populate the GUI Layout Element
                {
                    XmlElement xConnLoc = xGUIDoc.CreateElement(ConnID);
                    XmlAttribute xLocSourcePort = xGUIDoc.CreateAttribute("SourcePort");
                    XmlAttribute xLocSinkPort = xGUIDoc.CreateAttribute("SinkPort");
                    XmlAttribute xLocSourcePortCore = xGUIDoc.CreateAttribute("SourceCore");
                    XmlAttribute xLocSinkPortCore = xGUIDoc.CreateAttribute("SinkCore");

                    xLocSourcePort.Value = ((CoreConnector)(c.From.AttachedTo)).PortName;
                    xLocSinkPort.Value = ((CoreConnector)(c.To.AttachedTo)).PortName;
                    xLocSourcePortCore.Value = ((CoreConnector)(c.From.AttachedTo)).CoreInstance;
                    xLocSinkPortCore.Value = ((CoreConnector)(c.To.AttachedTo)).CoreInstance;


                    xConnLoc.Attributes.Append(xLocSourcePort);
                    xConnLoc.Attributes.Append(xLocSourcePortCore);
                    xConnLoc.Attributes.Append(xLocSinkPort);
                    xConnLoc.Attributes.Append(xLocSinkPortCore);
                    xGUIConnections.AppendChild(xConnLoc);
                }
            }
            xGUIDoc.Save(CerebrumGUILayoutFile.FullName);
        }

        private void SaveComponentConfigs()
        {
            try
            {
                if (!this.ProjectCoreConfigDirectory.Exists)
                {
                    this.ProjectCoreConfigDirectory.Create();
                }
                //else
                //{
                //    foreach (FileInfo fi in this.ProjectCoreConfigDirectory.GetFiles())
                //    {
                //        fi.Delete();
                //    }
                //}
                foreach (CerebrumCore cCore in _Cores.Values)
                {
                    cCore.SaveCoreConfigs(this.ProjectDirectory.FullName);
                }
            }
            catch (Exception ex)
            {
                this.RaiseMessageEvent(MessageEventType.Error, "SaveComponentConfigs", ex.Message, "Project Manager");
            }
        }
        private void AddSubComponentProcessors(CerebrumCore CC)
        {
            foreach (ComponentCore CompCore in CC.ComponentCores.Values)
            {
                if ((CompCore.CoreType.ToLower().StartsWith("ppc440_")) ||
                    (CompCore.CoreType.ToLower().StartsWith("ppc405_")) ||
                    (String.Compare(CompCore.CoreType.ToLower(), "microblaze") == 0))
                {
                    CerebrumProcessor CP = new CerebrumProcessor(CompCore.CoreType);
                    CP.Instance = CompCore.CoreInstance;
                    _Processors.Add(CP);
                }
            }
        }
        private void RemoveSubComponentProcessors(CerebrumCore CC)
        {
            foreach (ComponentCore CompCore in CC.ComponentCores.Values)
            {
                foreach (CerebrumProcessor CP in _Processors)
                {
                    if (CP.Instance == CompCore.CoreInstance)
                    {
                        _Processors.Remove(CP);
                        break;
                    }
                }
            }
        }
        private void AddSubComponentCommunications(CerebrumCore CC)
        {
            foreach (ComponentCore CompCore in CC.ComponentCores.Values)
            {
                if ((String.Compare(CompCore.CoreType.ToLower(), "xps_ll_temac") == 0))
                {
                    CommunicationConfiguration CommCfg = new CommunicationConfiguration();
                    CommCfg.HardwareInstance = CompCore.CoreInstance;
                    _CommConfigs.Add(CommCfg);
                }
            }
        }
        private void RemoveSubComponentCommunications(CerebrumCore CC)
        {
            foreach (ComponentCore CompCore in CC.ComponentCores.Values)
            {
                foreach (CommunicationConfiguration CommCfg in _CommConfigs)
                {
                    if (CommCfg.HardwareInstance == CompCore.CoreInstance)
                    {
                        _CommConfigs.Remove(CommCfg);
                        break;
                    }
                }
            }
        }
        private void LoadProcessors(XmlNode ProcessorsNode)
        {
            foreach (XmlNode xProc in ProcessorsNode)
            {
                string ProcInstance = string.Empty;
                foreach (XmlAttribute xProcAttr in xProc.Attributes)
                {
                    if (String.Compare(xProcAttr.Name, "Instance", true) == 0)
                    {
                        ProcInstance = xProcAttr.Value;
                        break;
                    }
                }
                if (ProcInstance != string.Empty)
                {
                    foreach (CerebrumProcessor CP in _Processors)
                    {
                        if (String.Compare(CP.Instance, ProcInstance, true) == 0)
                        {
                            CP.LoadFromXml(xProc);
                        }
                    }
                }
            }
        }
        private void SaveProcessors(XmlNode ProcessorsNode)
        {
            if (_Processors != null)
            {
                foreach (CerebrumProcessor CP in _Processors)
                {
                    ProcessorsNode.AppendChild(CP.SaveToXml(ProcessorsNode.OwnerDocument));
                }
            }
        }

        /// <summary>
        /// Refreshes processor mappings based on the current state of the mapping system
        /// </summary>
        public void RefreshProcessorMappings()
        {
            XmlDocument xDesDoc = new XmlDocument();
            xDesDoc.Load(DesignFile.FullName);
            XmlNode xProcessorsNode = CerebrumXmlInterface.GetXmlNode(xDesDoc, "Design.Processors");

            if (xProcessorsNode != null)
            {
                foreach (XmlNode xProc in xProcessorsNode)
                {
                    string ProcInstance = string.Empty;
                    foreach (XmlAttribute xProcAttr in xProc.Attributes)
                    {
                        if (String.Compare(xProcAttr.Name, "Instance", true) == 0)
                        {
                            ProcInstance = xProcAttr.Value;
                            break;
                        }
                    }
                    if (ProcInstance != string.Empty)
                    {
                        foreach (CerebrumProcessor CP in _Processors)
                        {
                            if (String.Compare(CP.Instance, ProcInstance, true) == 0)
                            {
                                CP.LoadFromXml(xProc);
                            }
                        }
                    }
                }
            }
        }
        private void LoadProgramming(XmlNode ProgrammingNode)
        {
            foreach (XmlNode xProg in ProgrammingNode)
            {
                string FPGAInstance = string.Empty;
                foreach (XmlAttribute xProcAttr in xProg.Attributes)
                {
                    if (String.Compare(xProcAttr.Name, "Device", true) == 0)
                    {
                        FPGAInstance = xProcAttr.Value;
                        break;
                    }
                }
                if (FPGAInstance != string.Empty)
                {
                    foreach (CerebrumPlatformFPGA FPGA in _ProjectPlatform.FPGAs)
                    {
                        if (String.Compare(FPGA.MappingID, FPGAInstance, true) == 0)
                        {
                            FPGA.ProgramConfig.LoadFromXml(xProg);
                        }
                    }
                }
            }
        }
        private void SaveProgramming(XmlNode ProgrammingNode)
        {
            foreach (CerebrumPlatformBoard Board in _ProjectPlatform.Boards)
            {
                Board.SaveProgrammingConfig(ProgrammingNode);
            }
        }

        /// <summary>
        /// Loads communcation configurations and refreshes mappings based on the current state of the mapping system
        /// </summary>
        public bool LoadCommunications()
        {
            try
            {
                XmlDocument xCommDoc = new XmlDocument();
                xCommDoc.Load(CommunicationsFile.FullName);
                XmlNode xIFs = CerebrumXmlInterface.GetXmlNode(xCommDoc, "Interfaces");
                foreach (XmlNode xIFNode in xIFs.ChildNodes)
                {
                    string IFInstance = string.Empty;
                    foreach (XmlAttribute xIFAttr in xIFNode.Attributes)
                    {
                        if (String.Compare(xIFAttr.Name, "Instance", true) == 0)
                        {
                            IFInstance = xIFAttr.Value;
                            break;
                        }
                    }
                    if (IFInstance != string.Empty)
                    {
                        foreach (CommunicationConfiguration CommCfg in _CommConfigs)
                        {
                            if (String.Compare(CommCfg.HardwareInstance, IFInstance, true) == 0)
                            {
                                CommCfg.LoadFromXml(xIFNode);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ex.Message, "Load Communications");
                ErrorReporting.DebugException(ex);
            }
            return false;
        }
        private bool SaveCommunications()
        {
            try
            {
                XmlDocument xCommDoc = new XmlDocument();
                xCommDoc.AppendChild(xCommDoc.CreateXmlDeclaration("1.0", "utf-8", null));
                XmlElement xCommRoot = xCommDoc.CreateElement("Interfaces");
                xCommDoc.AppendChild(xCommRoot);
                if (_CommConfigs != null)
                {
                    foreach (CommunicationConfiguration ccfg in _CommConfigs)
                    {
                        xCommRoot.AppendChild(ccfg.SaveToXml(xCommDoc));
                    }
                }
                xCommDoc.Save(CommunicationsFile.FullName);
                return true;
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ex.Message, "Save Communications");
                ErrorReporting.DebugException(ex);
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Reloads core configuration parameters for all cores loaded in the design.
        /// </summary>
        public void RefreshCoreConfigs()
        {
            foreach (CerebrumCore CC in _Cores.Values)
            {
                CC.LoadCoreConfigs(ProjectDirectory.FullName);
                foreach (ComponentCore CompCore in CC.ComponentCores.Values)
                {
                    CompCore.LoadCoreConfig(ProjectDirectory.FullName);
                }
            }
        }
        #endregion

        #region Core/SubComponent Location
        /// <summary>
        /// Gets the path to the image file supplied by the specified core instance
        /// </summary>
        /// <param name="CoreInstance">The instance name of the core whose image is to be located</param>
        /// <returns>The full path to the core instance's image file</returns>
        public string GetCoreInstanceImage(string CoreInstance)
        {
            foreach (CerebrumCore CC in _Cores.Values)
            {
                if (CC.CoreInstance == CoreInstance)
                {
                    return CC.CoreImagePath;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// Gets a CerebrumCore that contains the specified subcore instance
        /// </summary>
        /// <param name="SubInstance">The instance name of the subcore, whose parent is to be located</param>
        /// <returns>A CerebrumCore, if it exists, that contains the specified subcore instance</returns>
        public CerebrumCore GetCerebrumCoreContaining(string SubInstance)
        {
            foreach (CerebrumCore CC in _Cores.Values)
            {
                foreach (ComponentCore CompCore in CC.ComponentCores.Values)
                {
                    if (String.Compare(CompCore.CoreInstance, SubInstance, true) == 0)
                    {
                        return CC;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Clock / Reset Enumeration
        /// <summary>
        /// Enumerates a list of cores exposing clock signals that are compatible with the required input clock.  The list of components returned
        /// will exclude SourceComponent.  If this component is to be included in the list, this parameter should be null.
        /// </summary>
        /// <param name="SourceComponent">Specifies the component that requires InputClock.   If this parameter is not null, this component will not be 
        /// included in the list.  If it is null, it will be included, if it exposes a compatible output clock.</param>
        /// <param name="InputClock">The input clock signal for which to enumerate components exposing compatible clocks.</param>
        /// <returns>A list of CerebrumCore objects each of which contains at least one compatible output clock.  An empty list indicates that no compatible clocks were found.</returns>
        public List<CerebrumCore> EnumerateCoresWithCompatibleOutputClock(CerebrumCore SourceComponent, ClockSignal InputClock)
        {
            InputClock.GetFrequencyFromParameter = GetParameterMethod;
            List<CerebrumCore> Results = new List<CerebrumCore>();
            foreach (CerebrumCore CC in _Cores.Values)
            {
                foreach (ClockSignal CS in CC.OutputClocks)
                {
                    CS.GetFrequencyFromParameter = GetParameterMethod;
                    if (CS.SignalDirection == ClockDirection.OUTPUT)
                    {
                        if (CS.IsCompatibleWith(InputClock))
                        {
                            Results.Add(CC); 
                            break;
                        }
                    }
                }
            }
            //foreach (CerebrumCore CC in this.ProjectPlatform.RequiredCores)
            //{
            //    foreach (ClockSignal CS in CC.OutputClocks)
            //    {
            //        if (CS.IsOutput)
            //        {
            //            if (CS.IsCompatibleWith(InputClock))
            //            {
            //                Results.Add(CC);
            //                break;
            //            }
            //        }
            //    }
            //}
            return Results;
        }
        /// <summary>
        /// Enumerates a list of output clocks exposed by the TestComponent that are compatible with input clock.
        /// </summary>
        /// <param name="TestComponent">Specifies the component whose clocks are to be enumerated.</param>
        /// <param name="InputClock">The input clock signal for which to enumerate compatible clocks.</param>
        /// <returns>A list of ClockSignal objects each of which is a compatible output clock exposed by TestComponent.  An empty list indicates that no compatible clocks were found.</returns>
        public List<ClockSignal> EnumerateCompatibleOutputClocks(CerebrumCore TestComponent, ClockSignal InputClock)
        {
            List<ClockSignal> Results = new List<ClockSignal>();
            foreach (ClockSignal CS in TestComponent.OutputClocks)
            {
                CS.GetFrequencyFromParameter = GetParameterMethod;
                if (CS.SignalDirection == ClockDirection.OUTPUT)
                {
                    if (CS.IsCompatibleWith(InputClock))
                    {
                        Results.Add(CS);
                    }
                }
            }
            return Results;
        }

        private void ValidateClockAssignments()
        {
            foreach (CerebrumCore CC in _Cores.Values)
            {
                foreach (ClockSignal InCS in CC.InputClocks)
                {
                    bool bFound = false;
                    if (InCS.Connected)
                    {
                        if (_Cores.ContainsKey(InCS.SourceComponentInstance))
                        {
                            CerebrumCore SourceCC = _Cores[InCS.SourceComponentInstance];
                            if (SourceCC.ComponentCores.ContainsKey(InCS.SourceCoreInstance))
                            {
                                ComponentCore CompCore = SourceCC.ComponentCores[InCS.SourceCoreInstance];
                                foreach (ClockSignal OutCS in SourceCC.OutputClocks)
                                {
                                    if ((String.Compare(OutCS.ComponentInstance, SourceCC.CoreInstance) == 0) &&
                                        (String.Compare(OutCS.CoreInstance, CompCore.NativeInstance) == 0) &&
                                        (String.Compare(OutCS.Port, InCS.SourcePort) == 0) &&
                                        (String.Compare(OutCS.ComponentInstance, "clock_generator") == 0))
                                    {
                                        bFound = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (!bFound)
                    {
                        InCS.DisconnectFromSource();
                    }
                }
            }
        }
        #endregion

        #region Dialog Management
        /// <summary>
        /// Displays the project properties editor dialog
        /// </summary>
        /// <param name="owner">The form which will own the dialog</param>
        public void ShowProjectPropertiesDialog(Form owner)
        {
            try
            {
                Dialogs.ProjectPropertiesDialog ppd = new Dialogs.ProjectPropertiesDialog(_PathMan);
                ppd.LoadProjectProperties();
                DialogResult dr = ppd.ShowDialog(owner);
                FocusCerebrumForm();
                if (dr == DialogResult.OK)
                {
                    ppd.SaveProjectProperties();
                    SavePaths();
                }
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.ShowProjectPropertiesDialog");
                ErrorReporting.DebugException(ex);
            }
        }
        /// <summary>
        /// Displays the project paths editor dialog
        /// </summary>
        /// <param name="owner">The form which will own the dialog</param>
        public void ShowProjectPathsDialog(Form owner)
        {
            try
            {
                Dialogs.ProjectPathsDialog ppd = new Dialogs.ProjectPathsDialog(_PathMan);
                ppd.PopulateFromPathManager();
                DialogResult dr = ppd.ShowDialog(owner);
                FocusCerebrumForm();
                if (dr == DialogResult.OK)
                {
                    ppd.UpdatePathManager();
                    SavePaths();
                }
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.ShowProjectPathsDialog");
                ErrorReporting.DebugException(ex);
            }
        }
        /// <summary>
        /// Displays the server management/editor dialog
        /// </summary>
        /// <param name="owner">The form which will own the dialog</param>
        public void ShowServerManagerDialog(Form owner)
        {
            try
            {
                Dialogs.ServerManagerDialog smd = new Dialogs.ServerManagerDialog(this);
                smd.LoadServers();
                smd.Show(owner);
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.ShowProjectPathsDialog");
            }
        }
        /// <summary>
        /// Displays the processor management/editor dialog
        /// </summary>
        /// <param name="owner">The form which will own the dialog</param>
        public void ShowProcessorManagerDialog(Form owner)
        {
            try
            {
                Dialogs.ProcessorManagerDialog pmd = new Dialogs.ProcessorManagerDialog(this);
                pmd.LoadProcessors();
                pmd.Show(owner);
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.ShowProjectPathsDialog");
                ErrorReporting.DebugException(ex);
            }
        }
        /// <summary>
        /// Displays the communications management/editor dialog
        /// </summary>
        /// <param name="owner">The form which will own the dialog</param>
        public void ShowCommunicationsManagerDialog(Form owner)
        {
            try
            {
                Dialogs.CommunicationsManagerDialog cmd = new Dialogs.CommunicationsManagerDialog(this);
                cmd.LoadInterfaces();
                cmd.Show(owner);
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.ShowProjectPathsDialog");
                ErrorReporting.DebugException(ex);
            }
        }
        /// <summary>
        /// Displays the programming management/editor dialog
        /// </summary>
        /// <param name="owner">The form which will own the dialog</param>
        /// <param name="bEnableProgram">Flag used to indicate whether the button to begin programming should be enabled.</param>
        /// <param name="progCB">A function callback to be invoked by the programming dialog to begin programming.</param>
        public void ShowProgrammingManagerDialog(Form owner, bool bEnableProgram, StartProgrammerCallback progCB)
        {
            try
            {
                Dialogs.ProgrammingManagerDialog pmd = new Dialogs.ProgrammingManagerDialog(this, bEnableProgram);
                pmd.LoadBoards();
                pmd.ProgrammerLauncher = progCB;
                pmd.Show(owner);
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.ShowProjectPathsDialog");
                ErrorReporting.DebugException(ex);
            }
        }
        /// <summary>
        /// Displays the PE Code Editor editor dialog
        /// </summary>
        /// <param name="owner">The form which will own the dialog,</param>
        /// <param name="CompCore">The ComponentCore whose PE code is to be edited.</param>
        public void ShowPECodeEditorDialog(Form owner, ComponentCore CompCore)
        {
            try
            {
                Dialogs.PECodeEditorDialog peced = new CerebrumProjectManager.Dialogs.PECodeEditorDialog(this, CompCore);
                DialogResult result = peced.ShowDialog();
                FocusCerebrumForm();
                CompCore.SaveCoreConfig(this.ProjectDirectory.FullName);
            }
            catch (Exception ex)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Exception", ErrorReporting.ExceptionDetails(ex), "ProjectManager.ShowPECodeEditorDialog");
                ErrorReporting.DebugException(ex);
            }
        }
        #endregion

        #region Inter-Object Parameter Communication
        private object GetParameterMethod(ParameterSourceTypes Source, string ComponentInstance, string CoreInstance, string ParameterName)
        {
            switch (Source)
            {
                case ParameterSourceTypes.PARAMETER_PROJECT:
                    // Get PARAMETER from project (PathManager)
                    return this.PathManager[ParameterName];
                case ParameterSourceTypes.PARAMETER_COMPONENT:
                    // Get PARAMETER from CerebrumCore
                    foreach(CerebrumCore CC in this._Cores.Values)
                    {
                        if (String.Compare(CC.CoreInstance, ComponentInstance, true) == 0)
                        {
                            return CC.Properties.GetValue(CerebrumPropertyTypes.CEREBRUMPROPERTY, ParameterName);
                        }
                    }
                    break;
                case ParameterSourceTypes.PARAMETER_CORE:
                    // Get PARAMETER from ComponentCore within CerebrumCore
                    foreach (CerebrumCore CC in this._Cores.Values)
                    {
                        if (String.Compare(CC.CoreInstance, ComponentInstance, true) == 0)
                        {
                            foreach (ComponentCore CompCore in CC.ComponentCores.Values)
                            {
                                if (String.Compare(CompCore.NativeInstance, CoreInstance, true) == 0)
                                {
                                    return CompCore.Properties.GetValue(CerebrumPropertyTypes.PARAMETER, ParameterName);
                                }
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return string.Empty;
        }
        #endregion


        #region CerebrumNetronObjects Interaction

        #region CerebrumPanel/ProjectPanel Event Handlers/Management

        private void AttachCerebrumPanelEventHandlers()
        {
            ProjectPanel.CutEvent += new ParameterFreeEventDelegate(ProjectPanel_Cut);
            ProjectPanel.CopyEvent += new ParameterFreeEventDelegate(ProjectPanel_Copy);
            ProjectPanel.PasteEvent += new ParameterFreeEventDelegate(ProjectPanel_Paste);
            ProjectPanel.PasteVirtualEvent += new ParameterFreeEventDelegate(ProjectPanel_PasteVirtual);
            ProjectPanel.Rotate90CCWEvent += new ParameterFreeEventDelegate(ProjectPanel_Rotate90CCW);
            ProjectPanel.Rotate90CWEvent += new ParameterFreeEventDelegate(ProjectPanel_Rotate90CW);
        }
        void ProjectPanel_Paste()
        {
            if ((CoreClipBoard != null) && (CoreClipBoard.Count > 0))
            {
                ProjectPanel.Diagram.SelectedItems.Clear();
                foreach (CerebrumCore CC in CoreClipBoard)
                {
                    CerebrumCore newCC = new CerebrumCore(CC.CoreInstance, CC);
                    ProjectPanel.Diagram.AddShape(newCC);
                    newCC.Location = new Point(CC.X + 50, CC.Y + 50);
                    ProjectPanel.Diagram.SelectedItems.Add(newCC);
                }
            }
        }
        void ProjectPanel_PasteVirtual()
        {
            if ((CoreClipBoard != null) && (CoreClipBoard.Count > 0))
            {
                ProjectPanel.Diagram.SelectedItems.Clear();
                foreach (CerebrumCore CC in CoreClipBoard)
                {
                    CerebrumCore newCC = CC.Virtualize();
                    ProjectPanel.Diagram.AddShape(newCC);
                    newCC.Location = new Point(CC.X + 50, CC.Y + 50);
                    ProjectPanel.Diagram.SelectedItems.Add(newCC);
                }
            }
        }
        void ProjectPanel_Copy()
        {
            if (CoreClipBoard == null)
                CoreClipBoard = new List<CerebrumCore>();
            else
                CoreClipBoard.Clear();

            foreach (IDiagramEntity IDE in ProjectPanel.Diagram.SelectedItems)
            {
                if (IDE.GetType() == typeof(CerebrumCore))
                {
                    CoreClipBoard.Add(new CerebrumCore(string.Empty, (CerebrumCore)IDE));
                }
            }
            ProjectPanel.Diagram.SelectedItems.Clear();
        }
        void ProjectPanel_Cut()
        {
            if (CoreClipBoard == null)
                CoreClipBoard = new List<CerebrumCore>();
            else
                CoreClipBoard.Clear();

            // Remove all selected cores from the diagram and save them in the clipboard
            CollectionBase<IDiagramEntity> ToRemove = new CollectionBase<IDiagramEntity>();
            foreach (IDiagramEntity IDE in ProjectPanel.Diagram.SelectedItems)
            {
                if (IDE.GetType() == typeof(CerebrumCore))
                {
                    CoreClipBoard.Add(new CerebrumCore(string.Empty, (CerebrumCore)IDE));
                    ToRemove.Add(IDE);
                }
            }
            // Delete the selected items from the project panel
            DeleteCommand del = new DeleteCommand(ProjectPanel.Diagram.Controller, ToRemove);
            del.Redo();
            ProjectPanel.Diagram.SelectedItems.Clear();
        }
        void ProjectPanel_Rotate90CCW()
        {
            foreach (IDiagramEntity IDE in ProjectPanel.Diagram.SelectedItems)
            {
                if (IDE.GetType() == typeof(CerebrumCore))
                {
                    CerebrumCore CC = (CerebrumCore)IDE;
                    CC.RotationAngle -= 90;
                }
            }
        }
        void ProjectPanel_Rotate90CW()
        {
            foreach (IDiagramEntity IDE in ProjectPanel.Diagram.SelectedItems)
            {
                if (IDE.GetType() == typeof(CerebrumCore))
                {
                    CerebrumCore CC = (CerebrumCore)IDE;
                    CC.RotationAngle += 90;
                }
            }
        }
        #endregion

        private List<CerebrumCore> CoreClipBoard = null;

        private void AttachDiagramEventHandlers()
        {
            ProjectPanel.Diagram.OnEntityAdded += new EventHandler<Netron.Diagramming.Core.EntityEventArgs>(HandleDiagramEntityAdded);
            ProjectPanel.Diagram.OnEntityRemoved += new EventHandler<Netron.Diagramming.Core.EntityEventArgs>(HandleDiagramEntityRemoved);
            ProjectPanel.Diagram.MouseMove += new MouseEventHandler(HandleDiagramMouseMoved);
        }
        private void HandleDiagramEntityAdded(object sender, Netron.Diagramming.Core.EntityEventArgs e)
        {
            try
            {
                #region Connection Added
                if (e.Entity.GetType() == typeof(CerebrumConnection))
                {
                    CerebrumConnection c = (CerebrumConnection)e.Entity;
                    if (c.Name == null)
                        c.Name = string.Empty;
                    CerebrumCore SourceCore = null;
                    CerebrumCore SinkCore = null;
                    CoreConnector SourcePort = null;
                    CoreConnector SinkPort = null;

                    foreach (CerebrumCore CC in _Cores.Values)
                    {
                        if (CC.Hit(c.From.Point))
                        {
                            CoreConnector cConn = CC.GetOutPortClosestToXY(c.From.Point.X, c.From.Point.Y);
                            if (cConn == null)
                            {
                                // Ensure that the connection is reverted somehow.
                                ProjectPanel.Diagram.Undo();
                                return; // Not a valid connection
                            }
                            SourcePort = cConn;
                            SourceCore = CC;
                            break;
                        }
                    }
                    foreach (CerebrumCore CC in _Cores.Values)
                    {
                        if (CC.Hit(c.To.Point))
                        {
                            CoreConnector cConn = CC.GetCompatiblePortClosestToXY(c.To.Point.X, c.To.Point.Y, SourcePort);
                            if (cConn == null)
                            {
                                // Ensure that the connection is reverted somehow.
                                ProjectPanel.Diagram.Undo();
                                return; // Not a valid connection
                            }
                            SinkPort = cConn;
                            SinkCore = CC;
                            break;
                        }
                    }
                    if ((SourcePort != null) && (SinkPort != null) && (SourceCore != null) && (SinkPort != null) && (SourceCore != SinkCore))
                    {
                        if (Ports.GetCompatibleTargetTypes(SourcePort.PortType).Contains(SinkPort.PortType))
                        {
                            c.Name = String.Format("{0}_{1}-to-{2}_{3}", SourceCore.CoreInstance, SourcePort.CoreInstance, SinkCore.CoreInstance, SinkPort.CoreInstance);
                            if (_Connections.ContainsKey(c.Name))
                            {
                                // Connection already exists, undo the new one
                                ProjectPanel.Diagram.Undo();
                            }
                            else
                            {
                                Trace.WriteLine(String.Format("Added connection: {0}", c.Name));
                                ProjectPanel.AttachConnectors(c, SourcePort, SinkPort);
                                bProjectSaved = false;
                                _Connections.Add(c.Name, c);
                                c.OnEntityChange += new EventHandler<EntityEventArgs>(Handle_OnEntityChange);
                            }
                        }
                    }
                    else
                    {
                        // Ensure that the connection is reverted somehow.
                        ProjectPanel.Diagram.Undo();
                        return; // Not a valid connection
                    }
                }
                #endregion

                #region Core Added
                else if (e.Entity.GetType() == typeof(CerebrumCore))
                {
                    CerebrumCore CC = (CerebrumCore)(e.Entity);
                    if (!_Cores.ContainsKey(CC.CoreInstance))
                    {
                        CC.OnMouseEnter += new EventHandler<EntityMouseEventArgs>(HandleEntityMouseEnter);
                        CC.OnMouseLeave += new EventHandler<EntityMouseEventArgs>(HandleEntityMouseLeave);

                        if ((CC.CoreInstance == null) || (CC.CoreInstance == string.Empty))
                        {
                            int InstNum = 0;
                            string ProposedInstance = CC.CoreInstancePrefix + "_" + InstNum.ToString();
                            while (_Cores.ContainsKey(ProposedInstance))
                            {
                                InstNum++;
                                ProposedInstance = CC.CoreInstancePrefix + "_" + InstNum.ToString();
                            }
                            CC.CoreInstance = ProposedInstance;
                        }
                        Point Loc = ProjectPanel.Diagram.PointToClient(System.Windows.Forms.Cursor.Position);
                        //Loc.Offset(ProjectPanel.LibraryTab.Width, 0);
                        Debug.WriteLine(String.Format("Mouse Loc: {0}", Loc));
                        Debug.WriteLine(String.Format("Core  Loc: {0}", CC.Location));

                        bProjectSaved = false;

                        _Cores.Add(CC.CoreInstance, CC);
                        AddSubComponentProcessors(CC);
                        AddSubComponentCommunications(CC);

                        CC.OnEntityChange += new EventHandler<EntityEventArgs>(Handle_OnEntityChange);
                        CC.OnMouseDown += new EventHandler<EntityMouseEventArgs>(CC_OnMouseDown);
                        //CC.OnMouseUp += new EventHandler<EntityMouseEventArgs>(CC_OnMouseUp);

                        //CC.OnClick += new EventHandler<EntityEventArgs>(CC_OnClick);
                        //CC.OnDoubleClick += new EventHandler<EntityEventArgs>(CC_OnDoubleClick);
                        CC.OnRightClick += new EventHandler<EntityEventArgs>(CC_OnRightClick);
                        CC.CorePropertiesRequested += new CerebrumCore.CorePropertiesRequestedHandler(CC_CorePropertiesRequested);
                    }
                }
                #endregion

                #region Other Entity Added
                else
                {
                    _OtherEntities.Add(e.Entity);
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
                RaiseMessageEvent(MessageEventType.Error, "Error adding object to design!", ex.Message, "Project Manager");
                ProjectPanel.Diagram.Undo();
            }
        }
        private void HandleDiagramEntityRemoved(object sender, Netron.Diagramming.Core.EntityEventArgs e)
        {
            if (e.Entity.GetType() == typeof(CerebrumConnection))
            {
                CerebrumConnection c = (CerebrumConnection)e.Entity;
                if (_Connections.ContainsKey(c.Name))
                {
                    _Connections.Remove(c.Name);
                    bProjectSaved = false;
                }
            }
            else if (e.Entity.GetType() == typeof(CerebrumCore))
            {
                CerebrumCore CC = (CerebrumCore)(e.Entity);
                // Remove the core
                if (_Cores.ContainsKey(CC.CoreInstance))
                {
                    if (!CC.VisibleInLibrary)
                    {
                        string InfoMsg = String.Format("Unable to remove the specified Component {0} from the design.", CC.CoreInstance, CerebrumGUILayoutFile.Name);
                        myMessages.RaiseMessageEvent(MessageEventType.Warning, "Design Warning", InfoMsg, "Remove Component");
                        ProjectPanel.Diagram.AddShape(CC);
                        return;
                    }
                    _Cores.Remove(CC.CoreInstance);
                    RemoveSubComponentProcessors(CC);
                    RemoveSubComponentCommunications(CC);
                    bProjectSaved = false;
                }

                // Remove any connections attached to it
                CollectionBase<IDiagramEntity> ToRemove = new CollectionBase<IDiagramEntity>();
                foreach (CerebrumConnection c in _Connections.Values)
                {
                    CoreConnector fcc = (CoreConnector)(c.From.AttachedTo);
                    CoreConnector tcc = (CoreConnector)(c.To.AttachedTo);
                    if (((fcc != null) && (fcc.Core == CC)) ||
                        ((tcc != null) && (tcc.Core == CC)))
                    {
                        ToRemove.Add(c);
                    }
                }
                DeleteCommand del = new DeleteCommand(ProjectPanel.Diagram.Controller, ToRemove);
                del.Redo();
                foreach (IDiagramEntity ide in ToRemove)
                {
                    _Connections.Remove(((CerebrumConnection)ide).Name);
                    bProjectSaved = false;
                }
            }
            else
            {
                _OtherEntities.Remove(e.Entity);
            }
        }
        void Handle_OnEntityChange(object sender, EntityEventArgs e)
        {
            bProjectSaved = false;
        }

        //void CC_OnClick(object sender, EntityEventArgs e)
        //{
        //    // Currently nothing done on left-click
        //    // Nothing extravagant in OnClick, because a double click always triggers a single click first
        //}        
        void CC_OnMouseUp(object sender, EntityMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Clicks > 1)
                {
                    ((CerebrumCore)sender).RaiseOnDoubleClick(new EntityEventArgs(e.Entity));
                }
                else
                {
                    ((CerebrumCore)sender).RaiseOnClick(new EntityEventArgs(e.Entity));
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ((CerebrumCore)sender).RaiseOnRightClick(new EntityEventArgs(e.Entity));
            }
        }
        void CC_OnDoubleClick(object sender, EntityEventArgs e)
        {
            // Display properties dialog on double-click
            ((CerebrumCore)sender).RaisePropertiesRequested();
        }
        void CC_CorePropertiesRequested(CerebrumCore core)
        {
            Dialogs.CorePropertiesDialog cpd = new CerebrumProjectManager.Dialogs.CorePropertiesDialog();
            cpd.LoadPropertiesFromCoreObject(core);
            DialogResult result = cpd.ShowDialog();
            FocusCerebrumForm();
            if (result == DialogResult.OK)
            {
                cpd.SaveProperties();
            }
        }
        void CC_OnMouseDown(object sender, EntityMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Clicks > 1)
                {
                    ((CerebrumCore)sender).RaiseOnDoubleClick(new EntityEventArgs(e.Entity));
                }
                else
                {
                    ((CerebrumCore)sender).RaiseOnClick(new EntityEventArgs(e.Entity));
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ProjectPanel.Diagram.Capture = false;
                ((CerebrumCore)sender).RaiseOnRightClick(new EntityEventArgs(e.Entity));
            }
        }
        void CC_OnRightClick(object sender, EntityEventArgs e)
        {
            CerebrumCore CC = ((CerebrumCore)sender);
            ContextMenu context = new ContextMenu();
            MenuItem mnuCopy = new MenuItem("&Copy (Ctrl+C)", new EventHandler(context_CopyClick));
            MenuItem mnuCut = new MenuItem("Cu&t (Ctrl+X)", new EventHandler(context_CutClick));
            MenuItem mnuPaste = new MenuItem("Paste (Ctrl+V)", new EventHandler(context_PasteClick));
            MenuItem mnuRot90CW = new MenuItem("&Rotate 90 deg. Clockwise (Ctrl+R)", new EventHandler(context_RotCW_Click));
            MenuItem mnuRot90CCW = new MenuItem("Rotate 90 deg. C-Clock&wise (Ctrl+Shift+R)", new EventHandler(context_RotCCW_Click));
            MenuItem mnuProperties = new MenuItem("Properties", new EventHandler(context_PropertiesClick));
            MenuItem mnuClocks = new MenuItem("Manage Clock Signals", new EventHandler(context_ManageClocks_Click));
            MenuItem mnuCodeletSources = new MenuItem("Assign/Edit PE Code");

            if (CC != null)
            {
                if (ProjectPanel.Diagram.SelectedItems.Count > 0)
                {
                    mnuRot90CCW.Enabled = true;
                    mnuRot90CW.Enabled = true;
                    mnuCopy.Enabled = true;
                    mnuCut.Enabled = true;
                    if (ProjectPanel.Diagram.SelectedItems.Count == 1)
                    {
                        if (CC.InputClocks.Count > 0)
                            mnuClocks.Enabled = true;
                        else
                            mnuClocks.Enabled = false;
                        if (CC.ProcessingElementCores.Count > 0)
                        {
                            mnuCodeletSources.Enabled = true;
                            foreach (ComponentCore CompCore in CC.ProcessingElementCores)
                            {
                                MenuItem mnuAssignEditCodelet = new MenuItem(CompCore.NativeInstance, new EventHandler(context_AssignEditPECode));
                                mnuAssignEditCodelet.Tag = CompCore;
                                mnuCodeletSources.MenuItems.Add(mnuAssignEditCodelet);
                            }
                        }
                        else
                        {
                            mnuCodeletSources.Enabled = false;
                        }
                        mnuProperties.Enabled = true;
                        context.Tag = CC;
                    }
                    else
                    {
                        mnuCodeletSources.Enabled = false;
                        mnuProperties.Enabled = false;
                        mnuClocks.Enabled = false;
                    }
                }
                else
                {
                    mnuCodeletSources.Enabled = false;
                    mnuCopy.Enabled = false;
                    mnuCut.Enabled = false;
                    mnuProperties.Enabled = false;
                    mnuClocks.Enabled = false;
                }
                mnuPaste.Enabled = (CoreClipBoard != null) && (CoreClipBoard.Count > 0);                        
            }
            context.MenuItems.Add(mnuCopy);
            context.MenuItems.Add(mnuCut);
            context.MenuItems.Add(mnuPaste);
            context.MenuItems.Add(mnuRot90CW);
            context.MenuItems.Add(mnuRot90CCW);
            context.MenuItems.Add(new MenuItem("-"));
            context.MenuItems.Add(mnuCodeletSources);
            context.MenuItems.Add(mnuClocks);
            context.MenuItems.Add(new MenuItem("-"));
            context.MenuItems.Add(mnuProperties);
            context.Show(ProjectPanel, ProjectPanel.PointToClient(System.Windows.Forms.Control.MousePosition));
            //((CerebrumCore)sender).RaisePropertiesRequested();            
        }

        void context_PropertiesClick(object sender, EventArgs e)
        {
            MenuItem clickedItem = (MenuItem)sender;
            Object o = clickedItem.Parent.Tag;
            if (o != null)
            {
                if (o.GetType() == typeof(CerebrumCore))
                {
                    CerebrumCore CC = (CerebrumCore)o;
                    CC.RaisePropertiesRequested();
                }
            }
        }
        void context_ManageClocks_Click(object sender, EventArgs e)
        {
            MenuItem clickedItem = (MenuItem)sender;
            Object o = clickedItem.Parent.Tag;
            if (o != null)
            {
                if (o.GetType() == typeof(CerebrumCore))
                {
                    CerebrumCore CC = (CerebrumCore)o;
                    Dialogs.CoreClockManagerDialog ccmd = new CerebrumProjectManager.Dialogs.CoreClockManagerDialog();
                    ccmd.LoadClocksFromCoreObject(CC, this);
                    DialogResult result = ccmd.ShowDialog();
                    FocusCerebrumForm();
                    if (result == DialogResult.OK)
                    {
                        ccmd.SaveClockAssignments();
                    }
                }
            }
        }
        void context_RotCCW_Click(object sender, EventArgs e)
        {
            ProjectPanel_Rotate90CCW();
        }
        void context_RotCW_Click(object sender, EventArgs e)
        {
            ProjectPanel_Rotate90CW();
        }
        void context_CopyClick(object sender, EventArgs e)
        {
            ProjectPanel_Copy();
        }
        void context_CutClick(object sender, EventArgs e)
        {
            ProjectPanel_Cut();
        }
        void context_PasteClick(object sender, EventArgs e)
        {
            ProjectPanel_Paste();
        }
        void context_AssignEditPECode(object sender, EventArgs e)
        {
            MenuItem clickedItem = (MenuItem)sender;
            ComponentCore AssociatedCore = (ComponentCore)clickedItem.Tag;
            clickedItem.Tag = null;
            ShowPECodeEditorDialog(_CerebrumMainForm, AssociatedCore);
        }
        
        #endregion

        #region Tooltip Timing & Management
        // Tooltip constants
        private const int TOOLTIP_DELAY = 2000;
        private const int MAX_MOUSE_MOVE_DELTA = 5;

        // Tooltip Timer and State
        private System.Windows.Forms.Timer tooltipTimer;
        private string tooltipMessage = string.Empty;
        private int tooltipDuration = 0;
        private Point tooltipLastMousePos;

        // Tooltip management methods
        private void StartTooltipTimer(int TipTime, string Tip, Point CurrentMouse)
        {
            lock(this)
            {
                if (tooltipTimer == null)
                {
                    tooltipTimer = new System.Windows.Forms.Timer();
                    tooltipTimer.Tick += new EventHandler(tooltipTimer_Tick);
                }
            }
            lock (tooltipTimer)
            {
                tooltipMessage = Tip;
                tooltipDuration = TipTime;
                tooltipLastMousePos = CurrentMouse;

                tooltipTimer.Interval = TOOLTIP_DELAY;
                tooltipTimer.Start();
            }
        }
        private void ResetTooltipTimer()
        {
            if (tooltipTimer == null)
                return;
            lock (tooltipTimer)
            {
                if (tooltipTimer.Enabled)
                {
                    tooltipTimer.Stop();
                    tooltipTimer.Interval = TOOLTIP_DELAY;
                    tooltipTimer.Start();
                }
            }
        }
        private void ActivateTooltip()
        {
            string TooltipString = string.Empty;
            int Duration = 2500;        // 2.5s default
            StopTooltipTimer(ref TooltipString, ref Duration);
            if ((TooltipString == null) || (TooltipString == string.Empty))
                return;
            lock (tooltipTimer)
            {
                string TabString = "\t";

                // Change default tab spacing in tooltips
                TabString = "   ";

                if (TabString != "\t")
                {
                    TooltipString = TooltipString.Replace("\t", TabString);
                }
                Point mouseLoc = _CerebrumMainForm.PointToClient(System.Windows.Forms.Cursor.Position);
                int X = mouseLoc.X;
                int Y = mouseLoc.Y + 50;
                if (Duration == 0)
                {
                    toolTips.Show(TooltipString, _CerebrumMainForm, X, Y);
                }
                else
                {
                    toolTips.Show(TooltipString, _CerebrumMainForm, X, Y, Duration);
                }
            }
        }
        private void StopTooltipTimer(ref string Tip, ref int Duration)
        {
            lock (tooltipTimer)
            {
                Tip = tooltipMessage;
                Duration = tooltipDuration;
                tooltipTimer.Stop();
            }
        }
        private void StopTooltipTimer()
        {
            lock (tooltipTimer)
            {
                tooltipTimer.Stop();
            }
        }
        private void HideTooltips()
        {
            toolTips.Hide(_CerebrumMainForm);
        }
        // Tooltip timer Tick() event
        private void tooltipTimer_Tick(object sender, EventArgs e)
        {
            ActivateTooltip();
        } 

        // Netron object events to trigger tooltips
        void HandleEntityMouseEnter(object sender, EntityMouseEventArgs e)
        {
            string TooltipString = string.Empty;
            bool ShiftDown = (Keys.None != (Control.ModifierKeys & Keys.Shift));
            bool ControlDown = (Keys.None != (Control.ModifierKeys & Keys.Control));
            bool AltDown = (Keys.None != (Control.ModifierKeys & Keys.Alt));
            if (e.Entity.GetType() == typeof(CerebrumConnection))
            {
                #region CerebrumConnection Tooltips
                CerebrumConnection C = (CerebrumConnection)e.Entity;
                
                return;
                #endregion
            }
            else if (e.Entity.GetType() == typeof(CerebrumCore))
            {
                #region Component/Core Tooltips
                HideTooltips();
                CerebrumCore CC = (CerebrumCore)e.Entity;
                string InterfaceList = string.Empty;
                foreach (Connector Port in CC.Connectors)
                {
                    if (Port.GetType() == typeof(CoreConnector))
                    {
                        CoreConnector CorePort = (CoreConnector)Port;
                        InterfaceList += String.Format("\n\t{0} ({1})", CorePort.PortName, CorePort.PortType.ToString());
                    }
                }
                TooltipString = String.Format("Component: {5}\nInstance: {0}\nType: {1}\nVersion: {2}{3}{4}",
                    CC.CoreInstance,
                    CC.CoreType,
                    CC.CoreVersion,
                    (InterfaceList == string.Empty ? string.Empty : "\nInterfaces:"),
                    (InterfaceList == string.Empty ? string.Empty : InterfaceList),
                    CC.CoreName);
                if (ShiftDown)
                {
                    // Enhanced tooltip
                    string ETooltipString = string.Empty;
                    string PCoreList = string.Empty;
                    foreach (ComponentCore CmpCore in CC.ComponentCores.Values)
                    {
                        PCoreList += String.Format("\n\t{0} ({1})", CmpCore.NativeInstance, CmpCore.CoreType);
                    }
                    ETooltipString = String.Format("Internal Cores:{0}\nDescription: {1}", PCoreList, CC.CoreDescription);
                    TooltipString = String.Format("{0}\n{1}", TooltipString, ETooltipString);
                }
                int Timeout = 2500;     // Default: 2.5s timeout
                if (ControlDown)
                {
                    Timeout = 0;  // Infinite timeout
                }
                else if (ShiftDown)
                {
                    Timeout = 6000;   // 6s timeout
                }
                StartTooltipTimer(Timeout, TooltipString, System.Windows.Forms.Cursor.Position);
                #endregion
            }
            else
            {
                return;
            }
        }
        void HandleDiagramMouseMoved(object sender, MouseEventArgs e)
        {
            Point currentPos = System.Windows.Forms.Cursor.Position;
            Point deltaPos = new Point(Math.Abs(currentPos.X - tooltipLastMousePos.X), Math.Abs(currentPos.Y - tooltipLastMousePos.Y));
            if ((deltaPos.X > MAX_MOUSE_MOVE_DELTA) || (deltaPos.Y > MAX_MOUSE_MOVE_DELTA))
                ResetTooltipTimer();
            tooltipLastMousePos = currentPos;
        }
        void HandleEntityMouseLeave(object sender, EntityMouseEventArgs e)
        {
            HideTooltips();
        }
        #endregion

        #region BackEnd Project Flow

        #region BackEnd Tool Events
        /// <summary>
        /// Delegate that is used for relaying tool messages to the GUI
        /// </summary>
        /// <param name="Message">The message to be relayed</param>
        public delegate void ToolMessageDelegate(string Message);
        /// <summary>
        /// Delegate that fires when a tool has completed execution for GUI notification
        /// </summary>
        public delegate void ToolCompleteDelegate();

        /// <summary>
        /// Event that fires when the XPS Builder tool needs to relay a message
        /// </summary>
        public event ToolMessageDelegate XPSBuilderToolMessage;
        /// <summary>
        /// Event that fires when the XPS Builder tool has completed execution
        /// </summary>
        public event ToolCompleteDelegate XPSBuilderComplete;

        /// <summary>
        /// Event that fires when the Synthesis tool needs to relay a message
        /// </summary>
        public event ToolMessageDelegate SynthesisToolMessage;
        /// <summary>
        /// Event that fires when the Synthesis tool has completed execution
        /// </summary>
        public event ToolCompleteDelegate SynthesisComplete;

        /// <summary>
        /// Event that fires when the JTAG Programmer tool needs to relay a message
        /// </summary>
        public event ToolMessageDelegate JProgrammerToolMessage;
        /// <summary>
        /// Event that fires when the JTAG Programmer tool has completed execution
        /// </summary>
        public event ToolCompleteDelegate JProgrammerComplete;
        #endregion

        /// <summary>
        /// Parameterized Thread argument containing properties used by Back End tools
        /// </summary>
        public struct ToolStartArgs
        {
            // Shared Members
            /// <summary>
            /// Indicates whether projects and/or synthesis files should be purged prior to tool start
            /// </summary>
            public bool ForceClean { get; set; }

            // Synthesis Members
            /// <summary>
            /// Indicates whether only hardware should be synthesized
            /// </summary>
            public bool SynthesizeHardware { get; set; }
            /// <summary>
            /// Indicates whether only software should be compiled
            /// </summary>
            public bool CompileSoftware { get; set; }
            /// <summary>
            /// Indicates whether only selected components/pcores should be purged prior to synthesis.
            /// </summary>
            public bool SelectiveClean { get; set; }
            /// <summary>
            /// Indicates whether only selected FPGAs should be synthesized.
            /// </summary>
            public bool SelectiveSynthesis { get; set; }

            // XPS Builder Members
            /// <summary>
            /// Indicates whether the XPS builder should pre-empted for empty projects
            /// </summary>
            public bool AllowEmptySynth { get; set; }
        }

        private string GetPassword(string UserName, string ServerName)
        {
            string PW = string.Empty;
            if (PasswordPrompt(String.Format("{0}@{1}'s Password", UserName, ServerName), out PW))
            {
                return PW;
            }
            else
            {
                return string.Empty;
            }
        }
        private bool PasswordPrompt(string Prompt, out string Password)
        {
            Password = string.Empty;
            Form ownerForm = _CerebrumMainForm;
            if (_CerebrumMainForm.InvokeRequired)
            {
                ownerForm = null;
                FocusCerebrumForm();
            }
            string FirstEntry = string.Empty;
            string SecondEntry = string.Empty;

            Dialogs.PasswordPromptDialog ppd = new Dialogs.PasswordPromptDialog();

            // First Prompt
            DialogResult dr = DialogResult.OK;
            while (dr != DialogResult.Cancel)
            {
                ppd.Clear();
                ppd.Title = Prompt;
                if (ownerForm != null)
                {
                    FocusCerebrumForm();
                    dr = ppd.ShowDialog(ownerForm);
                }
                else
                {
                    dr = ppd.ShowDialog();
                }
                FocusCerebrumForm();
                dr = ppd.PasswordResult;
                if (dr == DialogResult.OK)
                    FirstEntry = ppd.Password;
                else
                    return false;

                // Second Prompt
                ppd.Clear();
                ppd.Title = String.Format("{0} Confirm", Prompt);
                dr = DialogResult.Cancel;
                if (ownerForm != null)
                {
                    FocusCerebrumForm();
                    dr = ppd.ShowDialog();
                }
                else
                {
                    dr = ppd.ShowDialog();
                }
                FocusCerebrumForm();
                dr = ppd.PasswordResult;
                if (dr == DialogResult.OK)
                    SecondEntry = ppd.Password;
                else
                    return false;
                if (String.Compare(FirstEntry, SecondEntry) == 0)
                {
                    Password = FirstEntry;
                    return true;
                }
                else
                {
                    MessageBox.Show("Passwords did not match.  Please try again.", "Password mismatch", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    FocusCerebrumForm();
                }
            }
            return false;
        }

        private PasswordRequestDelegate PasswordRequest;

        #region XPS Builder
        private MessageEventDelegate XPSRelay;
        /// <summary>
        /// Method to test whether the project is ready for XPS project generation
        /// </summary>
        /// <returns>True if the project is ready; false otherwise</returns>
        public bool ReadyForXPSBuild()
        {
            if (!this.ProjectLoaded)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Design Flow", "No Project Loaded", "Project Manager");
                return false;
            }
            if (!this.ProjectDirectory.Exists)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Critical", "Loaded Project Directory Does Not Exist.", "Project Manager");
                return false;
            }
            if (!File.Exists(PathManager["XPSMap"]))
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Design Flow", "XPS-Map file could not be found.", "Project Manager");
                return false;
            }
            return true;
        }
        /// <summary>
        /// Method to invoke the XPS Builder tool via a new thread call
        /// </summary>
        /// <param name="ThreadArgs">A ToolStartArgs object containing required parameters for the tool</param>
        public void RunXPSBuilder(object ThreadArgs)
        {
            ToolStartArgs args = (ToolStartArgs)ThreadArgs;
            RunXPSBuilder(args);
        }
        private void RunXPSBuilder(ToolStartArgs args)
        {
            FalconXPSBuilderControl XPSControl = null;
            try
            {
                RefreshCoreConfigs();
                SaveProject();
                XPSControl = new FalconXPSBuilderControl();
                XPSControl.ForceClean = args.ForceClean;
                XPSControl.AllowEmptySynth = args.AllowEmptySynth;
                XPSControl.OnRequirePassword += PasswordRequest;
                XPSControl.MessageEvent += XPSRelay;
                XPSControl.LoadPaths(this.ProjectPathsFile.FullName);
                XPSControl.ConfigureAndBuild(this.SynthesisServersFile.FullName, this.DesignFile.FullName);
            }
            catch (ThreadAbortException TAEx)
            {
                ErrorReporting.ExceptionDetails(TAEx);
            }
            catch (Exception ex)
            {
                RelayXPSMessage(ErrorReporting.ExceptionDetails(ex));
            }
            finally
            {
                if (XPSControl != null)
                {
                    XPSControl.OnRequirePassword -= PasswordRequest;
                    XPSControl.MessageEvent -= XPSRelay;
                }
            }
            if (XPSBuilderComplete != null)
                XPSBuilderComplete();
        }
        private void RelayXPSMessage(string Message)
        {
            if (XPSBuilderToolMessage != null)
                XPSBuilderToolMessage(Message);
        }
        #endregion

        #region Synthesis
        private MessageEventDelegate SynthesisRelay;
        /// <summary>
        /// Method to test whether the project is ready for synthesis
        /// </summary>
        /// <returns>True if the project is ready; false otherwise</returns>
        public bool ReadyForSynthesis()
        {
            if (!this.ProjectLoaded)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Design Flow", "No Project Loaded", "Project Manager");
                return false;
            }
            if (!this.ProjectDirectory.Exists)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Critical", "Loaded Project Directory Does Not Exist.", "Project Manager");
                return false;
            }
            if (!File.Exists(PathManager["XPSMap"]))
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Design Flow", "XPS-Map file could not be found. Run Mapping and XPS Builder before Synthesis.", "Project Manager");
                return false;
            }
            // Check that XPS Build has completed -- somehow?
            //ProjectEventRecorder PER = new ProjectEventRecorder();
            //PER.Open(_PathMan);
            //PER.GetProjectEvent("XPSComplete");
            //PER.Close();
            return true;
        }
        /// <summary>
        /// Method to invoke the synthesis tool via a new thread call
        /// </summary>
        /// <param name="ThreadArgs">A ToolStartArgs object containing required parameters for the tool</param>
        public void RunSynthesis(object ThreadArgs)
        {
            ToolStartArgs args = (ToolStartArgs)ThreadArgs;
            RunSynthesis(args);
        }
        private void RunSynthesis(ToolStartArgs args)
        {
            FalconSystemSynthesizer Synth = null;
            try
            {
                SaveProject();
                Synth = new FalconSystemSynthesizer();
                Synth.SoftwareOnly = args.CompileSoftware;
                Synth.HardwareOnly = true;
                Synth.PerformFullSynthesis = args.SynthesizeHardware;
                Synth.ForceClean = args.ForceClean;
                Synth.SelectiveClean = args.SelectiveClean;
                Synth.SelectiveSynthesis = args.SelectiveSynthesis;
                Synth.OnRequirePassword += PasswordRequest;
                Synth.OnRequestSelectiveSynthesis += SynthSelectSynthCallback;
                Synth.OnRequestSelectivePurge += SynthSelectPurgeCallback;
                Synth.MessageEvent += SynthesisRelay;

                if ((Synth.LoadPathsFile(this.ProjectPathsFile.FullName)) &&
                    (Synth.LoadServersFile(this.SynthesisServersFile.FullName)) &&
                    (Synth.LoadPlatformFile()) &&
                    (Synth.LoadDesignFile(this.DesignFile.FullName)) &&
                    (Synth.LoadCommsFile(this.CommunicationsFile.FullName)) &&
                    (Synth.Ready()))
                {
                    SynthesisErrorCodes code = Synth.SynthesizeDesign();
                    if ((code != SynthesisErrorCodes.SYNTHESIS_OK) && (code != SynthesisErrorCodes.SYNTHESIS_SKIPPED))
                    {
                        RelaySynthMessage(String.Format("Synthesis failed with error code {0}", code.ToString()));
                    }
                    else
                    {
                        RelaySynthMessage("Synthesis Complete!");
                    }
                }
                else
                {
                    RelaySynthMessage("System/Design not ready for synthesis.  Verify all information has been loaded and is correct.");
                }
            }
            catch (ThreadAbortException TAEx)
            {
                ErrorReporting.ExceptionDetails(TAEx);
            }
            catch (Exception ex)
            {
                RelaySynthMessage(ErrorReporting.ExceptionDetails(ex));
            }
            finally
            {
                if (Synth != null)
                {
                    Synth.OnRequirePassword -= PasswordRequest;
                    Synth.MessageEvent -= SynthesisRelay;
                }
            }
            if (SynthesisComplete != null)
                SynthesisComplete();
        }
        private void RelaySynthMessage(string Message)
        {
            if (SynthesisToolMessage != null)
                SynthesisToolMessage(Message);
        }

        private bool SynthSelectPurgeCallback(List<string> FPGAs, ref List<CerebrumCore> Components, ref List<ComponentCore> Cores)
        {
            Dialogs.SynthesisSelectivePurgeDialog SSPD = new Dialogs.SynthesisSelectivePurgeDialog();
            SSPD.PopulateTreeView(FPGAs, ref Components, ref Cores);
            DialogResult res;
            res = SSPD.ShowDialog();
            FocusCerebrumForm();
            if (res == DialogResult.Abort)
            {
                return false;
            }
            if (res == DialogResult.OK)
            {
                SSPD.UpdateStatus();
            }
            return true;
        }
        private bool SynthSelectSynthCallback(ref Dictionary<string, bool> FPGAs)
        {
            Dialogs.SynthesisSelectivePlatformDialog SSPD = new Dialogs.SynthesisSelectivePlatformDialog();
            SSPD.PopulateTreeView(ref FPGAs);
            DialogResult res;
            res = SSPD.ShowDialog();
            FocusCerebrumForm();
            if (res == DialogResult.Abort)
            {
                return false;
            }
            if (res == DialogResult.OK)
            {
                SSPD.UpdateStatus(ref FPGAs);
            }
            return true;
        }

        /// <summary>
        /// Compile Resource Reports for each FPGA in the Platform.
        /// </summary>
        public void CompileResourceReports()
        {
            FalconSystemSynthesizer Synth = new FalconSystemSynthesizer();
            Synth.SoftwareOnly = false;
            Synth.HardwareOnly = false;
            Synth.PerformFullSynthesis = false;
            Synth.ForceClean = false;
            Synth.SelectiveClean = false;
            Synth.SelectiveSynthesis = false;
            
            Synth.LoadPathsFile(this.ProjectPathsFile.FullName);
            Synth.LoadServersFile(this.SynthesisServersFile.FullName);
            Synth.LoadPlatformFile();
            Synth.LoadDesignFile(this.DesignFile.FullName);
            Synth.LoadCommsFile(this.CommunicationsFile.FullName);

            Synth.CompileResourceReports();
        }
        #endregion

        #region JProgrammer
        private MessageEventDelegate JProgrammerRelay;
        /// <summary>
        /// Method to test whether the project is ready for programming
        /// </summary>
        /// <returns>True if the project is ready; false otherwise</returns>
        public bool ReadyForProgramming()
        {
            if (!this.ProjectLoaded)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Design Flow", "No Project Loaded", "Project Manager");
                return false;
            }
            if (!this.ProjectDirectory.Exists)
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Critical", "Loaded Project Directory Does Not Exist.", "Project Manager");
                return false;
            }
            if (!File.Exists(PathManager["XPSMap"]))
            {
                myMessages.RaiseMessageEvent(MessageEventType.Error, "Design Flow", "XPS-Map file could not be found. Run Mapping, XPS Builder, and Synthesis before Programming.", "Project Manager");
                return false;
            }
            // Check that XPS Build and Synthesis have completed -- somehow?
            return true;
        }
        /// <summary>
        /// Method to invoke the JTAG programmer tool via a new thread call
        /// </summary>
        /// <param name="ThreadArgs">A ToolStartArgs object containing required parameters for the tool</param>
        public void RunJProgrammer(object ThreadArgs)
        {
            ToolStartArgs args = (ToolStartArgs)ThreadArgs;
            RunJProgrammer(args);
        }
        private void RunJProgrammer(ToolStartArgs args)
        {
            MultiJProgrammer MJProgrammer = null;
            try
            {
                SaveProject();
                MJProgrammer = new MultiJProgrammer();
                MJProgrammer.OnRequirePassword += PasswordRequest;
                MJProgrammer.MessageEvent += JProgrammerRelay;

                MJProgrammer.LoadPaths(this.ProjectPathsFile.FullName);
                MJProgrammer.LoadServersFile(this.SynthesisServersFile.FullName);
                MJProgrammer.LoadPlatformFile();
                MJProgrammer.LoadDesignFile(this.DesignFile.FullName);
                MJProgrammer.ProgramSystem();
            }
            catch (ThreadAbortException TAEx)
            {
                ErrorReporting.ExceptionDetails(TAEx);
            }
            catch (Exception ex)
            {
                RelayJProgMessage(ErrorReporting.ExceptionDetails(ex));
            }
            finally
            {
                if (MJProgrammer != null)
                {
                    MJProgrammer.OnRequirePassword -= PasswordRequest;
                    MJProgrammer.MessageEvent -= JProgrammerRelay;
                }
            }
            if (JProgrammerComplete!= null)
                JProgrammerComplete();
        }
        private void RelayJProgMessage(string Message)
        {
            if (JProgrammerToolMessage != null)
                JProgrammerToolMessage(Message);
        }
        #endregion

        #endregion

        #region Inter-object Message Event Controller 
        MessageEventController myMessages;
        /// <summary>
        /// Attach the specified MessageEventController to this control, allowing it to propagate messages to the GUI
        /// </summary>
        /// <param name="EventController"></param>
        public void AttachMessageController(MessageEventController EventController)
        {
            myMessages = EventController;
        }

        /// <summary>
        /// Sends message event to the message controller for propagation to the GUI handler
        /// </summary>
        /// <param name="MsgType">The type of the message event to be fired</param>
        /// <param name="MsgID">A identifier indicating the message generated</param>
        /// <param name="Message">The message generated.</param>
        /// <param name="MsgSource">A string indicating the source of the message</param>
        public void RaiseMessageEvent(MessageEventType MsgType, string MsgID, string Message, string MsgSource)
        {
            myMessages.RaiseMessageEvent(MsgType, MsgID, Message, MsgSource);
        }
        #endregion

        #region Windows Registry Access
        private const string PRIMARY_REG_KEY = "SOFTWARE\\PennState\\Cerebrum";

        private string _CerebrumRootDirectory = string.Empty;
        private string _BinDirectory = string.Empty;
        private string _CerebrumCoresDirectory = string.Empty;
        private string _PlatformsDirectory = string.Empty;
        private string _ProjectsDirectory = string.Empty;

        /// <summary>
        /// Get the path to the location of the Cerebrum Install.  If it is not set in the registry, it is assumed to be the parent directory of the
        /// directory containing the Cerebrum (this) executable.
        /// </summary>
        /// <returns>The path to be used for CerebrumRoot for each project loaded.</returns>
        public string GetCerebrumInstallPath()
        {
            if (_CerebrumRootDirectory == string.Empty)
            {
                bool bRegSuccess = false;

                try
                {
                    RegistryKey InstallKey = Registry.LocalMachine.OpenSubKey(PRIMARY_REG_KEY, false);
                    _CerebrumRootDirectory = (string)InstallKey.GetValue("CerebrumRoot", string.Empty);
                    bRegSuccess = (_CerebrumRootDirectory != string.Empty);
                    if (!bRegSuccess)
                    {
                        _CerebrumRootDirectory = (string)InstallKey.GetValue("InstallPath", string.Empty);
                        bRegSuccess = (_CerebrumRootDirectory != string.Empty);
                    }
                }
                catch { }
                if (!bRegSuccess)
                {
                    // Path to Cerebrum Global (Install-Time) Paths
                    Assembly Entry = System.Reflection.Assembly.GetEntryAssembly();
                    _CerebrumRootDirectory = new FileInfo(Entry.FullName).Directory.Parent.FullName;
                }
            }
            return _CerebrumRootDirectory;
        }
        /// <summary>
        /// Get the path to the location of the Cerebrum binary executables.  If it is not set in the registry, it is assumed to be the 
        /// directory containing the Cerebrum (this) executable.
        /// </summary>
        /// <returns>The path to be used for BinDirectory for each project loaded.</returns>
        public string GetCerebrumBinPath()
        {
            if (_BinDirectory == string.Empty)
            {
                bool bRegSuccess = false;

                try
                {
                    RegistryKey InstallKey = Registry.LocalMachine.OpenSubKey(PRIMARY_REG_KEY, false);
                    _BinDirectory = (string)InstallKey.GetValue("BinDirectory", string.Empty);
                    bRegSuccess = (_BinDirectory != string.Empty);
                }
                catch { }
                if (!bRegSuccess)
                {
                    _BinDirectory = String.Format("{0}\\bin", GetCerebrumInstallPath());
                }
            }
            return _BinDirectory;
        }
        /// <summary>
        /// Get the path to the location of the Cerebrum Core packages.  If it is not set in the registry, it is assumed to be the 'Cores' subdirectory of the
        /// parent directory of the directory containing the Cerebrum (this) executable.
        /// </summary>
        /// <returns>The path to be used for CerebrumCores for each project loaded.</returns>
        public string GetCerebrumCoresPath()
        {
            if (_CerebrumCoresDirectory == string.Empty)
            {
                bool bRegSuccess = false;

                try
                {
                    RegistryKey InstallKey = Registry.LocalMachine.OpenSubKey(PRIMARY_REG_KEY, false);
                    _CerebrumCoresDirectory = (string)InstallKey.GetValue("CerebrumCores", string.Empty);
                    bRegSuccess = (_CerebrumCoresDirectory != string.Empty);
                }
                catch { }
                if (!bRegSuccess)
                {
                    _CerebrumCoresDirectory = String.Format("{0}\\Cores", GetCerebrumInstallPath());
                }
            }
            return _CerebrumCoresDirectory;
        }
        /// <summary>
        /// Get the path to the location of the Cerebrum Platforms.  If it is not set in the registry, it is assumed to be the 'Platforms' subdirectory of the
        /// parent directory of the directory containing the Cerebrum (this) executable.
        /// </summary>
        /// <returns>The path to be used for Platforms for each project loaded.</returns>
        public string GetCerebrumPlatformsPath()
        {
            if (_PlatformsDirectory == string.Empty)
            {
                bool bRegSuccess = false;

                try
                {
                    RegistryKey InstallKey = Registry.LocalMachine.OpenSubKey(PRIMARY_REG_KEY, false);
                    _PlatformsDirectory = (string)InstallKey.GetValue("Platforms", string.Empty);
                    bRegSuccess = (_PlatformsDirectory != string.Empty);
                }
                catch { }
                if (!bRegSuccess)
                {
                    _PlatformsDirectory = String.Format("{0}\\Platforms", GetCerebrumInstallPath());
                }
            }
            return _PlatformsDirectory;
        }
        /// <summary>
        /// Get the default path to be supplied for the creation of new projects.  If it not available, this function returns the result of
        /// GetCerebrumInstallPath().
        /// </summary>
        /// <returns>The DefaultProjectPath entry from the system registry.</returns>
        public string GetCerebrumProjectsPath()
        {
            if (_ProjectsDirectory == string.Empty)
            {
                bool bRegSuccess = false;

                try
                {
                    RegistryKey InstallKey = Registry.LocalMachine.OpenSubKey(PRIMARY_REG_KEY, false);
                    _ProjectsDirectory = (string)InstallKey.GetValue("DefaultProjectPath", string.Empty);
                    bRegSuccess = (_ProjectsDirectory != string.Empty);
                }
                catch { }
                if (!bRegSuccess)
                {
                    _ProjectsDirectory = GetCerebrumInstallPath();
                }
            }
            return _ProjectsDirectory;
        }


        private const string MRU_SUBKEY = "ProjectMRUList";
        private const int MAX_MAX_MRU_ENTRIES = 15;
        private int _MRUMaxEntries = 9;
        /// <summary>
        /// Get or set the maximum number of projects retained or returned when updating/requesting the MRU List.
        /// </summary>
        public int MaxMRUEntries
        {
            get
            {
                return _MRUMaxEntries;
            }
            set
            {
                if (value > MAX_MAX_MRU_ENTRIES)
                    value = MAX_MAX_MRU_ENTRIES;
                if (value < 0)
                    value = 0;
                _MRUMaxEntries = value;
            }
        }
        /// <summary>
        /// Gets the list of Most Recently Used (MRU) projects from the system registry.
        /// </summary>
        /// <returns>A list of project paths, in the order they were most recently used.</returns>
        public List<string> GetMRUList()
        {
            string projectPath = string.Empty;
            List<string> MRUList = new List<string>();
            try
            {
                string MRUKey = String.Format("{0}\\{1}", PRIMARY_REG_KEY, MRU_SUBKEY);
                RegistryKey MRUListKey = Registry.CurrentUser.OpenSubKey(MRUKey, false);
                for (int i = 0; i < MaxMRUEntries; i++)
                {
                    projectPath = (string)MRUListKey.GetValue(String.Format("Project{0}", i), string.Empty);
                    if (projectPath != string.Empty)
                    {
                        MRUList.Add(projectPath);
                    }
                }
            }
            catch { }
            return MRUList;
        }
        /// <summary>
        /// Sets the specified project path as the most recently used project path, shifting all other paths in the list down one position.
        /// </summary>
        /// <returns>A list of project paths, in the order they were most recently used.</returns>
        public List<string> SetMRU(string MRUEntry)
        {            
            List<string> MRUList = GetMRUList();
            if (MRUList.Contains(MRUEntry))
            {
                MRUList.Remove(MRUEntry);
            }
            MRUList.Insert(0, MRUEntry);
            try
            {
                string MRUKey = String.Format("{0}\\{1}", PRIMARY_REG_KEY, MRU_SUBKEY);
                RegistryKey MRUListKey = Registry.CurrentUser.OpenSubKey(MRUKey, RegistryKeyPermissionCheck.ReadWriteSubTree);

                int EntriesAdded = 0;
                int i = 0;
                while((EntriesAdded < MaxMRUEntries) && (i < MRUList.Count))
                {
                    string ListEntry = MRUList[i];
                    if ((ListEntry != null) && (ListEntry != string.Empty))
                    {
                        MRUListKey.SetValue(String.Format("Project{0}", i), ListEntry);
                        EntriesAdded++;
                    }
                    i++;
                }

                int k = EntriesAdded;
                while (k < MAX_MAX_MRU_ENTRIES)
                {
                    MRUListKey.DeleteValue(String.Format("Project{0}", k), false);
                    k++;
                }
            }
            catch (Exception ex)
            {
                ErrorReporting.DebugException(ex);
            }
            return GetMRUList();
        }
        /// <summary>
        /// Removes the specified project path from the most recently used project path, shifting all other paths in the list down one position.
        /// </summary>
        /// <returns>A list of project paths, in the order they were most recently used.</returns>
        public List<string> RemoveMRU(string MRUEntry)
        {
            List<string> MRUList = GetMRUList();
            if (MRUList.Contains(MRUEntry))
            {
                MRUList.Remove(MRUEntry);
            }
            try
            {
                string MRUKey = String.Format("{0}\\{1}", PRIMARY_REG_KEY, MRU_SUBKEY);
                RegistryKey MRUListKey = Registry.CurrentUser.OpenSubKey(MRUKey, true);

                int EntriesAdded = 0;
                int i = 0;
                while ((EntriesAdded < MaxMRUEntries) && (i < MRUList.Count))
                {
                    string ListEntry = MRUList[i];
                    if ((ListEntry != null) && (ListEntry != string.Empty))
                    {
                        MRUListKey.SetValue(String.Format("Project{0}", i), ListEntry);
                        EntriesAdded++;
                    }
                    i++;
                }

                int k = EntriesAdded;
                while (k < MAX_MAX_MRU_ENTRIES)
                {
                    MRUListKey.DeleteValue(String.Format("Project{0}", k), false);
                    k++;
                }
            }
            catch { }
            return GetMRUList();
        }
        /// <summary>
        /// Clears ALL projects in the most recently used list.
        /// </summary>
        public void ClearMRU()
        {
            try
            {
                string MRUKey = String.Format("{0}\\{1}", PRIMARY_REG_KEY, MRU_SUBKEY);
                RegistryKey MRUListKey = Registry.CurrentUser.OpenSubKey(MRUKey, true);

                int k = 0;
                while (k < MAX_MAX_MRU_ENTRIES)
                {
                    MRUListKey.DeleteValue(String.Format("Project{0}", k), false);
                    k++;
                }
            }
            catch { }
        }

        /// <summary>
        /// Gets boolean indicating whether the MRU List is available or not.
        /// </summary>
        public bool MRUAvailable
        {
            get
            {
                try
                {
                    string MRUKey = String.Format("{0}\\{1}", PRIMARY_REG_KEY, MRU_SUBKEY);
                    RegistryKey MRUListKey = Registry.CurrentUser.OpenSubKey(MRUKey, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorReporting.DebugException(ex);
                }
                return false;
            }
        }
        #endregion
    }
}
