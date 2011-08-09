namespace CerebrumFrontEndGUI
{
    /// <summary>
    /// Primary form for the Cerebrum Front End User Interface
    /// </summary>
    partial class frmCerebrumGUIMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator menuItemFileSeparator3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCerebrumGUIMain));
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileNewWizard = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileNewEmpty = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileSaveCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemFileRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileRecentClearList = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileRecentSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemFileSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemFilePrint = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemEditPreferences = new System.Windows.Forms.ToolStripMenuItem();
            this.menuView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewToolbars = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewToolbarsProject = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewToolbarsDesign = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewToolbarsMapping = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewToolbarsSynthesis = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemViewToolbarsHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuProject = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProjectPathSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProjectEditServerLists = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProjectProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDesign = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDesignConfigureProcessors = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDesignConfigureCommunications = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDesignConfigureProgramming = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMapping = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMappingReset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMappingComplete = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMappingConfirm = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMappingSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemMappingSavePreset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMappingLoadPreset = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMappingSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemMappingMode = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMappingModeManual = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMappingModeQuery = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSynthesis = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSynthesisBuildXPS = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSynthesisSynthesize = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSynthesisProgram = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSynthesisStartConfigServer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.menuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemHelpHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.designTabs = new System.Windows.Forms.TabControl();
            this.tabDesign = new System.Windows.Forms.TabPage();
            this.projectPanel = new CerebrumNetronObjects.CerebrumProjectPanel();
            this.tabMapping = new System.Windows.Forms.TabPage();
            this.splitMappingGUIInfo = new System.Windows.Forms.SplitContainer();
            this.mapTabLeftHorizontalSplitter = new System.Windows.Forms.SplitContainer();
            this.mappingHost = new System.Windows.Forms.Integration.ElementHost();
            this.mappingCanvas = new CerebrumMappingControls.MappingCanvasControl();
            this.tabsMapping = new System.Windows.Forms.TabControl();
            this.tabAll = new System.Windows.Forms.TabPage();
            this.lvExAll = new CerebrumSharedClasses.ListViewEx();
            this.colAllID = new System.Windows.Forms.ColumnHeader();
            this.colAllName = new System.Windows.Forms.ColumnHeader();
            this.colAllGroup = new System.Windows.Forms.ColumnHeader();
            this.colAllFPGA = new System.Windows.Forms.ColumnHeader();
            this.splitMapOptionsInfo = new System.Windows.Forms.SplitContainer();
            this.grpMapOptions = new System.Windows.Forms.GroupBox();
            this.lblMapIOWeight = new System.Windows.Forms.Label();
            this.trackIOWeight = new System.Windows.Forms.TrackBar();
            this.chkMapIOEnable = new System.Windows.Forms.CheckBox();
            this.grpMappingSelectedInfo = new System.Windows.Forms.GroupBox();
            this.lblMappingSelectedInfo = new System.Windows.Forms.Label();
            this.tabSynthesis = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnApplyFilter = new System.Windows.Forms.Button();
            this.grpFilesContainer = new System.Windows.Forms.GroupBox();
            this.treeProjectFiles = new System.Windows.Forms.TreeView();
            this.lblFilter = new System.Windows.Forms.Label();
            this.txtFileFilter = new System.Windows.Forms.TextBox();
            this.btnOpenSelected = new System.Windows.Forms.Button();
            this.btnRefreshFiles = new System.Windows.Forms.Button();
            this.grpLiveLog = new System.Windows.Forms.GroupBox();
            this.tabToolOutput = new System.Windows.Forms.TabControl();
            this.tabToolOutputPage = new System.Windows.Forms.TabPage();
            this.tbToolLiveLog = new System.Windows.Forms.TextBox();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.loadPathDialog = new System.Windows.Forms.OpenFileDialog();
            this.splitMainDesignReport = new System.Windows.Forms.SplitContainer();
            this.tabsReport = new System.Windows.Forms.TabControl();
            this.tabMessages = new System.Windows.Forms.TabPage();
            this.lvAllMessages = new System.Windows.Forms.ListView();
            this.colMessageType = new System.Windows.Forms.ColumnHeader();
            this.colMessageID = new System.Windows.Forms.ColumnHeader();
            this.colMessageString = new System.Windows.Forms.ColumnHeader();
            this.colMessageSource = new System.Windows.Forms.ColumnHeader();
            this.tabInfo = new System.Windows.Forms.TabPage();
            this.lvInfo = new System.Windows.Forms.ListView();
            this.colInfoType = new System.Windows.Forms.ColumnHeader();
            this.colInfoID = new System.Windows.Forms.ColumnHeader();
            this.colInfoMessage = new System.Windows.Forms.ColumnHeader();
            this.colInfoSource = new System.Windows.Forms.ColumnHeader();
            this.tabWarnings = new System.Windows.Forms.TabPage();
            this.lvWarnings = new System.Windows.Forms.ListView();
            this.colWarningType = new System.Windows.Forms.ColumnHeader();
            this.colWarningID = new System.Windows.Forms.ColumnHeader();
            this.colWarningMessage = new System.Windows.Forms.ColumnHeader();
            this.colWarningSource = new System.Windows.Forms.ColumnHeader();
            this.tabErrors = new System.Windows.Forms.TabPage();
            this.lvErrors = new System.Windows.Forms.ListView();
            this.colErrorType = new System.Windows.Forms.ColumnHeader();
            this.colErrorID = new System.Windows.Forms.ColumnHeader();
            this.colErrorMessage = new System.Windows.Forms.ColumnHeader();
            this.colErrorSource = new System.Windows.Forms.ColumnHeader();
            this.tabConsole = new System.Windows.Forms.TabPage();
            this.tbConsole = new System.Windows.Forms.TextBox();
            this.messageImages = new System.Windows.Forms.ImageList(this.components);
            this.status = new System.Windows.Forms.StatusStrip();
            this.tips = new System.Windows.Forms.ToolTip(this.components);
            this.container = new System.Windows.Forms.ToolStripContainer();
            this.toolStripHelp = new System.Windows.Forms.ToolStrip();
            this.toolBtnHelpHelp = new System.Windows.Forms.ToolStripButton();
            this.toolBtnHelpAbout = new System.Windows.Forms.ToolStripButton();
            this.toolStripMapping = new System.Windows.Forms.ToolStrip();
            this.toolBtnMappingReset = new System.Windows.Forms.ToolStripButton();
            this.toolBtnMappingComplete = new System.Windows.Forms.ToolStripButton();
            this.toolBtnMappingConfirm = new System.Windows.Forms.ToolStripButton();
            this.toolStripMappingSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBtnMappingSavePreset = new System.Windows.Forms.ToolStripButton();
            this.toolBtnMappingLoadPreset = new System.Windows.Forms.ToolStripButton();
            this.toolStripMappingSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBtnMappingModeManual = new System.Windows.Forms.ToolStripButton();
            this.toolBtnMappingModeQuery = new System.Windows.Forms.ToolStripButton();
            this.toolStripDesign = new System.Windows.Forms.ToolStrip();
            this.toolBtnDesignConfigureProcessors = new System.Windows.Forms.ToolStripButton();
            this.toolBtnDesignConfigureCommunications = new System.Windows.Forms.ToolStripButton();
            this.toolBtnDesignConfigureProgramming = new System.Windows.Forms.ToolStripButton();
            this.toolStripProject = new System.Windows.Forms.ToolStrip();
            this.toolBtnFileNewWizard = new System.Windows.Forms.ToolStripButton();
            this.toolBtnFileOpen = new System.Windows.Forms.ToolStripButton();
            this.toolBtnFileSave = new System.Windows.Forms.ToolStripButton();
            this.toolBtnFileSaveCopy = new System.Windows.Forms.ToolStripButton();
            this.projectToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBtnProjectPathSettings = new System.Windows.Forms.ToolStripButton();
            this.toolBtnProjectEditServerLists = new System.Windows.Forms.ToolStripButton();
            this.toolBtnProjectProperties = new System.Windows.Forms.ToolStripButton();
            this.projectToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBtnEditPreferences = new System.Windows.Forms.ToolStripButton();
            this.toolStripSynthesis = new System.Windows.Forms.ToolStrip();
            this.toolBtnSynthesisBuildXPS = new System.Windows.Forms.ToolStripButton();
            this.toolBtnSynthesisSynthesize = new System.Windows.Forms.ToolStripButton();
            this.toolBtnSynthesisProgram = new System.Windows.Forms.ToolStripButton();
            this.toolBtnSynthesisStartConfigServer = new System.Windows.Forms.ToolStripButton();
            this.button1 = new System.Windows.Forms.Button();
            menuItemFileSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuStripMain.SuspendLayout();
            this.designTabs.SuspendLayout();
            this.tabDesign.SuspendLayout();
            this.tabMapping.SuspendLayout();
            this.splitMappingGUIInfo.Panel1.SuspendLayout();
            this.splitMappingGUIInfo.Panel2.SuspendLayout();
            this.splitMappingGUIInfo.SuspendLayout();
            this.mapTabLeftHorizontalSplitter.Panel1.SuspendLayout();
            this.mapTabLeftHorizontalSplitter.Panel2.SuspendLayout();
            this.mapTabLeftHorizontalSplitter.SuspendLayout();
            this.tabsMapping.SuspendLayout();
            this.tabAll.SuspendLayout();
            this.splitMapOptionsInfo.Panel1.SuspendLayout();
            this.splitMapOptionsInfo.Panel2.SuspendLayout();
            this.splitMapOptionsInfo.SuspendLayout();
            this.grpMapOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackIOWeight)).BeginInit();
            this.grpMappingSelectedInfo.SuspendLayout();
            this.tabSynthesis.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.grpFilesContainer.SuspendLayout();
            this.grpLiveLog.SuspendLayout();
            this.tabToolOutput.SuspendLayout();
            this.tabToolOutputPage.SuspendLayout();
            this.tabSummary.SuspendLayout();
            this.splitMainDesignReport.Panel1.SuspendLayout();
            this.splitMainDesignReport.Panel2.SuspendLayout();
            this.splitMainDesignReport.SuspendLayout();
            this.tabsReport.SuspendLayout();
            this.tabMessages.SuspendLayout();
            this.tabInfo.SuspendLayout();
            this.tabWarnings.SuspendLayout();
            this.tabErrors.SuspendLayout();
            this.tabConsole.SuspendLayout();
            this.container.ContentPanel.SuspendLayout();
            this.container.TopToolStripPanel.SuspendLayout();
            this.container.SuspendLayout();
            this.toolStripHelp.SuspendLayout();
            this.toolStripMapping.SuspendLayout();
            this.toolStripDesign.SuspendLayout();
            this.toolStripProject.SuspendLayout();
            this.toolStripSynthesis.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuItemFileSeparator3
            // 
            menuItemFileSeparator3.Name = "menuItemFileSeparator3";
            menuItemFileSeparator3.Size = new System.Drawing.Size(227, 6);
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuEdit,
            this.menuView,
            this.menuProject,
            this.menuDesign,
            this.menuMapping,
            this.menuSynthesis,
            this.menuWindows,
            this.menuHelp});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(1044, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFileNew,
            this.menuItemFileOpen,
            this.menuItemFileSave,
            this.menuItemFileSaveCopy,
            this.menuItemFileClose,
            this.menuItemFileSeparator1,
            this.menuItemFileRecent,
            this.menuItemFileSeparator2,
            this.menuItemFilePrint,
            menuItemFileSeparator3,
            this.menuItemFileExit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(37, 20);
            this.menuFile.Text = "&File";
            // 
            // menuItemFileNew
            // 
            this.menuItemFileNew.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFileNewWizard,
            this.menuItemFileNewEmpty});
            this.menuItemFileNew.Name = "menuItemFileNew";
            this.menuItemFileNew.Size = new System.Drawing.Size(230, 22);
            this.menuItemFileNew.Text = "&New Project";
            // 
            // menuItemFileNewWizard
            // 
            this.menuItemFileNewWizard.Name = "menuItemFileNewWizard";
            this.menuItemFileNewWizard.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.menuItemFileNewWizard.Size = new System.Drawing.Size(220, 22);
            this.menuItemFileNewWizard.Text = "New Project Wizard";
            // 
            // menuItemFileNewEmpty
            // 
            this.menuItemFileNewEmpty.Enabled = false;
            this.menuItemFileNewEmpty.Name = "menuItemFileNewEmpty";
            this.menuItemFileNewEmpty.Size = new System.Drawing.Size(220, 22);
            this.menuItemFileNewEmpty.Text = "Empty Project";
            this.menuItemFileNewEmpty.Visible = false;
            // 
            // menuItemFileOpen
            // 
            this.menuItemFileOpen.Name = "menuItemFileOpen";
            this.menuItemFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.menuItemFileOpen.Size = new System.Drawing.Size(230, 22);
            this.menuItemFileOpen.Text = "&Open Project";
            // 
            // menuItemFileSave
            // 
            this.menuItemFileSave.Name = "menuItemFileSave";
            this.menuItemFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuItemFileSave.Size = new System.Drawing.Size(230, 22);
            this.menuItemFileSave.Text = "Sa&ve Project";
            // 
            // menuItemFileSaveCopy
            // 
            this.menuItemFileSaveCopy.Name = "menuItemFileSaveCopy";
            this.menuItemFileSaveCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F12)));
            this.menuItemFileSaveCopy.Size = new System.Drawing.Size(230, 22);
            this.menuItemFileSaveCopy.Text = "Save Project Cop&y...";
            this.menuItemFileSaveCopy.Visible = false;
            // 
            // menuItemFileClose
            // 
            this.menuItemFileClose.Name = "menuItemFileClose";
            this.menuItemFileClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.menuItemFileClose.Size = new System.Drawing.Size(230, 22);
            this.menuItemFileClose.Text = "&Close Project";
            // 
            // menuItemFileSeparator1
            // 
            this.menuItemFileSeparator1.Name = "menuItemFileSeparator1";
            this.menuItemFileSeparator1.Size = new System.Drawing.Size(227, 6);
            // 
            // menuItemFileRecent
            // 
            this.menuItemFileRecent.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFileRecentClearList,
            this.menuItemFileRecentSeparator1});
            this.menuItemFileRecent.Name = "menuItemFileRecent";
            this.menuItemFileRecent.Size = new System.Drawing.Size(230, 22);
            this.menuItemFileRecent.Text = "Recent Projects";
            // 
            // menuItemFileRecentClearList
            // 
            this.menuItemFileRecentClearList.Name = "menuItemFileRecentClearList";
            this.menuItemFileRecentClearList.Size = new System.Drawing.Size(122, 22);
            this.menuItemFileRecentClearList.Text = "Clear List";
            // 
            // menuItemFileRecentSeparator1
            // 
            this.menuItemFileRecentSeparator1.Name = "menuItemFileRecentSeparator1";
            this.menuItemFileRecentSeparator1.Size = new System.Drawing.Size(119, 6);
            // 
            // menuItemFileSeparator2
            // 
            this.menuItemFileSeparator2.Name = "menuItemFileSeparator2";
            this.menuItemFileSeparator2.Size = new System.Drawing.Size(227, 6);
            // 
            // menuItemFilePrint
            // 
            this.menuItemFilePrint.Name = "menuItemFilePrint";
            this.menuItemFilePrint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.menuItemFilePrint.Size = new System.Drawing.Size(230, 22);
            this.menuItemFilePrint.Text = "Print";
            // 
            // menuItemFileExit
            // 
            this.menuItemFileExit.Name = "menuItemFileExit";
            this.menuItemFileExit.Size = new System.Drawing.Size(230, 22);
            this.menuItemFileExit.Text = "E&xit";
            // 
            // menuEdit
            // 
            this.menuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemEditCut,
            this.menuItemEditCopy,
            this.menuItemEditPaste,
            this.menuItemEditSeparator1,
            this.menuItemEditPreferences});
            this.menuEdit.Name = "menuEdit";
            this.menuEdit.Size = new System.Drawing.Size(39, 20);
            this.menuEdit.Text = "&Edit";
            // 
            // menuItemEditCut
            // 
            this.menuItemEditCut.Name = "menuItemEditCut";
            this.menuItemEditCut.Size = new System.Drawing.Size(152, 22);
            this.menuItemEditCut.Text = "Cu&t";
            // 
            // menuItemEditCopy
            // 
            this.menuItemEditCopy.Name = "menuItemEditCopy";
            this.menuItemEditCopy.Size = new System.Drawing.Size(152, 22);
            this.menuItemEditCopy.Text = "&Copy";
            // 
            // menuItemEditPaste
            // 
            this.menuItemEditPaste.Name = "menuItemEditPaste";
            this.menuItemEditPaste.Size = new System.Drawing.Size(152, 22);
            this.menuItemEditPaste.Text = "&Paste";
            // 
            // menuItemEditSeparator1
            // 
            this.menuItemEditSeparator1.Name = "menuItemEditSeparator1";
            this.menuItemEditSeparator1.Size = new System.Drawing.Size(149, 6);
            // 
            // menuItemEditPreferences
            // 
            this.menuItemEditPreferences.Name = "menuItemEditPreferences";
            this.menuItemEditPreferences.Size = new System.Drawing.Size(152, 22);
            this.menuItemEditPreferences.Text = "P&references...";
            // 
            // menuView
            // 
            this.menuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemViewToolbars});
            this.menuView.Name = "menuView";
            this.menuView.Size = new System.Drawing.Size(44, 20);
            this.menuView.Text = "V&iew";
            // 
            // menuItemViewToolbars
            // 
            this.menuItemViewToolbars.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemViewToolbarsProject,
            this.menuItemViewToolbarsDesign,
            this.menuItemViewToolbarsMapping,
            this.menuItemViewToolbarsSynthesis,
            this.menuItemViewToolbarsHelp});
            this.menuItemViewToolbars.Name = "menuItemViewToolbars";
            this.menuItemViewToolbars.Size = new System.Drawing.Size(120, 22);
            this.menuItemViewToolbars.Text = "Toolbars";
            // 
            // menuItemViewToolbarsProject
            // 
            this.menuItemViewToolbarsProject.CheckOnClick = true;
            this.menuItemViewToolbarsProject.Name = "menuItemViewToolbarsProject";
            this.menuItemViewToolbarsProject.Size = new System.Drawing.Size(123, 22);
            this.menuItemViewToolbarsProject.Text = "Project";
            // 
            // menuItemViewToolbarsDesign
            // 
            this.menuItemViewToolbarsDesign.CheckOnClick = true;
            this.menuItemViewToolbarsDesign.Name = "menuItemViewToolbarsDesign";
            this.menuItemViewToolbarsDesign.Size = new System.Drawing.Size(123, 22);
            this.menuItemViewToolbarsDesign.Text = "Design";
            // 
            // menuItemViewToolbarsMapping
            // 
            this.menuItemViewToolbarsMapping.CheckOnClick = true;
            this.menuItemViewToolbarsMapping.Name = "menuItemViewToolbarsMapping";
            this.menuItemViewToolbarsMapping.Size = new System.Drawing.Size(123, 22);
            this.menuItemViewToolbarsMapping.Text = "Mapping";
            // 
            // menuItemViewToolbarsSynthesis
            // 
            this.menuItemViewToolbarsSynthesis.CheckOnClick = true;
            this.menuItemViewToolbarsSynthesis.Name = "menuItemViewToolbarsSynthesis";
            this.menuItemViewToolbarsSynthesis.Size = new System.Drawing.Size(123, 22);
            this.menuItemViewToolbarsSynthesis.Text = "Synthesis";
            // 
            // menuItemViewToolbarsHelp
            // 
            this.menuItemViewToolbarsHelp.CheckOnClick = true;
            this.menuItemViewToolbarsHelp.Name = "menuItemViewToolbarsHelp";
            this.menuItemViewToolbarsHelp.Size = new System.Drawing.Size(123, 22);
            this.menuItemViewToolbarsHelp.Text = "Help";
            // 
            // menuProject
            // 
            this.menuProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemProjectPathSettings,
            this.menuItemProjectEditServerLists,
            this.menuItemProjectProperties});
            this.menuProject.Name = "menuProject";
            this.menuProject.Size = new System.Drawing.Size(56, 20);
            this.menuProject.Text = "&Project";
            // 
            // menuItemProjectPathSettings
            // 
            this.menuItemProjectPathSettings.Name = "menuItemProjectPathSettings";
            this.menuItemProjectPathSettings.Size = new System.Drawing.Size(164, 22);
            this.menuItemProjectPathSettings.Text = "Path Settings...";
            // 
            // menuItemProjectEditServerLists
            // 
            this.menuItemProjectEditServerLists.Name = "menuItemProjectEditServerLists";
            this.menuItemProjectEditServerLists.Size = new System.Drawing.Size(164, 22);
            this.menuItemProjectEditServerLists.Text = "Edit Server Lists...";
            // 
            // menuItemProjectProperties
            // 
            this.menuItemProjectProperties.Name = "menuItemProjectProperties";
            this.menuItemProjectProperties.Size = new System.Drawing.Size(164, 22);
            this.menuItemProjectProperties.Text = "&Properties";
            // 
            // menuDesign
            // 
            this.menuDesign.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemDesignConfigureProcessors,
            this.menuItemDesignConfigureCommunications,
            this.menuItemDesignConfigureProgramming});
            this.menuDesign.Name = "menuDesign";
            this.menuDesign.Size = new System.Drawing.Size(55, 20);
            this.menuDesign.Text = "&Design";
            // 
            // menuItemDesignConfigureProcessors
            // 
            this.menuItemDesignConfigureProcessors.Name = "menuItemDesignConfigureProcessors";
            this.menuItemDesignConfigureProcessors.Size = new System.Drawing.Size(222, 22);
            this.menuItemDesignConfigureProcessors.Text = "Configure Processors";
            // 
            // menuItemDesignConfigureCommunications
            // 
            this.menuItemDesignConfigureCommunications.Name = "menuItemDesignConfigureCommunications";
            this.menuItemDesignConfigureCommunications.Size = new System.Drawing.Size(222, 22);
            this.menuItemDesignConfigureCommunications.Text = "Configure Communications";
            // 
            // menuItemDesignConfigureProgramming
            // 
            this.menuItemDesignConfigureProgramming.Name = "menuItemDesignConfigureProgramming";
            this.menuItemDesignConfigureProgramming.Size = new System.Drawing.Size(222, 22);
            this.menuItemDesignConfigureProgramming.Text = "Configure Programming";
            // 
            // menuMapping
            // 
            this.menuMapping.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemMappingReset,
            this.menuItemMappingComplete,
            this.menuItemMappingConfirm,
            this.menuItemMappingSeparator1,
            this.menuItemMappingSavePreset,
            this.menuItemMappingLoadPreset,
            this.menuItemMappingSeparator2,
            this.menuItemMappingMode});
            this.menuMapping.Name = "menuMapping";
            this.menuMapping.Size = new System.Drawing.Size(67, 20);
            this.menuMapping.Text = "&Mapping";
            // 
            // menuItemMappingReset
            // 
            this.menuItemMappingReset.Name = "menuItemMappingReset";
            this.menuItemMappingReset.Size = new System.Drawing.Size(216, 22);
            this.menuItemMappingReset.Text = "Reset Mapping";
            // 
            // menuItemMappingComplete
            // 
            this.menuItemMappingComplete.Name = "menuItemMappingComplete";
            this.menuItemMappingComplete.Size = new System.Drawing.Size(216, 22);
            this.menuItemMappingComplete.Text = "Auto-Complete Mapping";
            // 
            // menuItemMappingConfirm
            // 
            this.menuItemMappingConfirm.Name = "menuItemMappingConfirm";
            this.menuItemMappingConfirm.Size = new System.Drawing.Size(216, 22);
            this.menuItemMappingConfirm.Text = "Confirm Current Mapping";
            // 
            // menuItemMappingSeparator1
            // 
            this.menuItemMappingSeparator1.Name = "menuItemMappingSeparator1";
            this.menuItemMappingSeparator1.Size = new System.Drawing.Size(213, 6);
            // 
            // menuItemMappingSavePreset
            // 
            this.menuItemMappingSavePreset.Name = "menuItemMappingSavePreset";
            this.menuItemMappingSavePreset.Size = new System.Drawing.Size(216, 22);
            this.menuItemMappingSavePreset.Text = "Save Mapping Preset...";
            // 
            // menuItemMappingLoadPreset
            // 
            this.menuItemMappingLoadPreset.Name = "menuItemMappingLoadPreset";
            this.menuItemMappingLoadPreset.Size = new System.Drawing.Size(216, 22);
            this.menuItemMappingLoadPreset.Text = "Load/Set Mapping Preset...";
            // 
            // menuItemMappingSeparator2
            // 
            this.menuItemMappingSeparator2.Name = "menuItemMappingSeparator2";
            this.menuItemMappingSeparator2.Size = new System.Drawing.Size(213, 6);
            // 
            // menuItemMappingMode
            // 
            this.menuItemMappingMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemMappingModeManual,
            this.menuItemMappingModeQuery});
            this.menuItemMappingMode.Name = "menuItemMappingMode";
            this.menuItemMappingMode.Size = new System.Drawing.Size(216, 22);
            this.menuItemMappingMode.Text = "Interface Mode";
            // 
            // menuItemMappingModeManual
            // 
            this.menuItemMappingModeManual.Name = "menuItemMappingModeManual";
            this.menuItemMappingModeManual.Size = new System.Drawing.Size(172, 22);
            this.menuItemMappingModeManual.Text = "Manual Mapping";
            // 
            // menuItemMappingModeQuery
            // 
            this.menuItemMappingModeQuery.Name = "menuItemMappingModeQuery";
            this.menuItemMappingModeQuery.Size = new System.Drawing.Size(172, 22);
            this.menuItemMappingModeQuery.Text = "Query Information";
            // 
            // menuSynthesis
            // 
            this.menuSynthesis.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemSynthesisBuildXPS,
            this.menuItemSynthesisSynthesize,
            this.menuItemSynthesisProgram,
            this.menuItemSynthesisStartConfigServer});
            this.menuSynthesis.Name = "menuSynthesis";
            this.menuSynthesis.Size = new System.Drawing.Size(68, 20);
            this.menuSynthesis.Text = "&Synthesis";
            // 
            // menuItemSynthesisBuildXPS
            // 
            this.menuItemSynthesisBuildXPS.Name = "menuItemSynthesisBuildXPS";
            this.menuItemSynthesisBuildXPS.Size = new System.Drawing.Size(219, 22);
            this.menuItemSynthesisBuildXPS.Text = "Build XPS Projects...";
            // 
            // menuItemSynthesisSynthesize
            // 
            this.menuItemSynthesisSynthesize.Name = "menuItemSynthesisSynthesize";
            this.menuItemSynthesisSynthesize.Size = new System.Drawing.Size(219, 22);
            this.menuItemSynthesisSynthesize.Text = "Synthesize XPS Projects...";
            // 
            // menuItemSynthesisProgram
            // 
            this.menuItemSynthesisProgram.Name = "menuItemSynthesisProgram";
            this.menuItemSynthesisProgram.Size = new System.Drawing.Size(219, 22);
            this.menuItemSynthesisProgram.Text = "Program FPGA(s)...";
            // 
            // menuItemSynthesisStartConfigServer
            // 
            this.menuItemSynthesisStartConfigServer.Name = "menuItemSynthesisStartConfigServer";
            this.menuItemSynthesisStartConfigServer.Size = new System.Drawing.Size(219, 22);
            this.menuItemSynthesisStartConfigServer.Text = "Start Configuration Server...";
            // 
            // menuWindows
            // 
            this.menuWindows.Name = "menuWindows";
            this.menuWindows.Size = new System.Drawing.Size(68, 20);
            this.menuWindows.Text = "&Windows";
            this.menuWindows.Visible = false;
            // 
            // menuHelp
            // 
            this.menuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemHelpHelp,
            this.menuItemHelpAbout});
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new System.Drawing.Size(44, 20);
            this.menuHelp.Text = "&Help";
            // 
            // menuItemHelpHelp
            // 
            this.menuItemHelpHelp.Name = "menuItemHelpHelp";
            this.menuItemHelpHelp.Size = new System.Drawing.Size(262, 22);
            this.menuItemHelpHelp.Text = "Cerebrum System Designer &Help";
            // 
            // menuItemHelpAbout
            // 
            this.menuItemHelpAbout.Name = "menuItemHelpAbout";
            this.menuItemHelpAbout.Size = new System.Drawing.Size(262, 22);
            this.menuItemHelpAbout.Text = "&About Cerebrum System Designer...";
            // 
            // designTabs
            // 
            this.designTabs.Controls.Add(this.tabDesign);
            this.designTabs.Controls.Add(this.tabMapping);
            this.designTabs.Controls.Add(this.tabSynthesis);
            this.designTabs.Controls.Add(this.tabSummary);
            this.designTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.designTabs.Location = new System.Drawing.Point(0, 0);
            this.designTabs.Name = "designTabs";
            this.designTabs.SelectedIndex = 0;
            this.designTabs.Size = new System.Drawing.Size(1044, 458);
            this.designTabs.TabIndex = 2;
            // 
            // tabDesign
            // 
            this.tabDesign.Controls.Add(this.projectPanel);
            this.tabDesign.Location = new System.Drawing.Point(4, 22);
            this.tabDesign.Name = "tabDesign";
            this.tabDesign.Padding = new System.Windows.Forms.Padding(3);
            this.tabDesign.Size = new System.Drawing.Size(1036, 432);
            this.tabDesign.TabIndex = 0;
            this.tabDesign.Tag = "design";
            this.tabDesign.Text = "Design";
            this.tabDesign.UseVisualStyleBackColor = true;
            // 
            // projectPanel
            // 
            this.projectPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectPanel.Location = new System.Drawing.Point(3, 3);
            this.projectPanel.Name = "projectPanel";
            this.projectPanel.Size = new System.Drawing.Size(1030, 426);
            this.projectPanel.TabIndex = 0;
            // 
            // tabMapping
            // 
            this.tabMapping.Controls.Add(this.splitMappingGUIInfo);
            this.tabMapping.Location = new System.Drawing.Point(4, 22);
            this.tabMapping.Name = "tabMapping";
            this.tabMapping.Padding = new System.Windows.Forms.Padding(3);
            this.tabMapping.Size = new System.Drawing.Size(1036, 432);
            this.tabMapping.TabIndex = 1;
            this.tabMapping.Tag = "mapping";
            this.tabMapping.Text = "Mapping";
            this.tabMapping.UseVisualStyleBackColor = true;
            // 
            // splitMappingGUIInfo
            // 
            this.splitMappingGUIInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMappingGUIInfo.Location = new System.Drawing.Point(3, 3);
            this.splitMappingGUIInfo.Name = "splitMappingGUIInfo";
            // 
            // splitMappingGUIInfo.Panel1
            // 
            this.splitMappingGUIInfo.Panel1.Controls.Add(this.mapTabLeftHorizontalSplitter);
            // 
            // splitMappingGUIInfo.Panel2
            // 
            this.splitMappingGUIInfo.Panel2.Controls.Add(this.splitMapOptionsInfo);
            this.splitMappingGUIInfo.Size = new System.Drawing.Size(1030, 426);
            this.splitMappingGUIInfo.SplitterDistance = 751;
            this.splitMappingGUIInfo.TabIndex = 2;
            // 
            // mapTabLeftHorizontalSplitter
            // 
            this.mapTabLeftHorizontalSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapTabLeftHorizontalSplitter.Location = new System.Drawing.Point(0, 0);
            this.mapTabLeftHorizontalSplitter.Name = "mapTabLeftHorizontalSplitter";
            this.mapTabLeftHorizontalSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mapTabLeftHorizontalSplitter.Panel1
            // 
            this.mapTabLeftHorizontalSplitter.Panel1.Controls.Add(this.mappingHost);
            // 
            // mapTabLeftHorizontalSplitter.Panel2
            // 
            this.mapTabLeftHorizontalSplitter.Panel2.Controls.Add(this.tabsMapping);
            this.mapTabLeftHorizontalSplitter.Size = new System.Drawing.Size(751, 426);
            this.mapTabLeftHorizontalSplitter.SplitterDistance = 275;
            this.mapTabLeftHorizontalSplitter.TabIndex = 4;
            // 
            // mappingHost
            // 
            this.mappingHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mappingHost.Location = new System.Drawing.Point(0, 0);
            this.mappingHost.Name = "mappingHost";
            this.mappingHost.Size = new System.Drawing.Size(751, 275);
            this.mappingHost.TabIndex = 0;
            this.mappingHost.Child = this.mappingCanvas;
            // 
            // tabsMapping
            // 
            this.tabsMapping.Controls.Add(this.tabAll);
            this.tabsMapping.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsMapping.Location = new System.Drawing.Point(0, 0);
            this.tabsMapping.Name = "tabsMapping";
            this.tabsMapping.SelectedIndex = 0;
            this.tabsMapping.Size = new System.Drawing.Size(751, 147);
            this.tabsMapping.TabIndex = 2;
            this.tabsMapping.Tag = "";
            // 
            // tabAll
            // 
            this.tabAll.Controls.Add(this.lvExAll);
            this.tabAll.Location = new System.Drawing.Point(4, 22);
            this.tabAll.Name = "tabAll";
            this.tabAll.Padding = new System.Windows.Forms.Padding(3);
            this.tabAll.Size = new System.Drawing.Size(743, 121);
            this.tabAll.TabIndex = 0;
            this.tabAll.Text = "All Components";
            this.tabAll.UseVisualStyleBackColor = true;
            // 
            // lvExAll
            // 
            this.lvExAll.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAllID,
            this.colAllName,
            this.colAllGroup,
            this.colAllFPGA});
            this.lvExAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvExAll.FullRowSelect = true;
            this.lvExAll.Location = new System.Drawing.Point(3, 3);
            this.lvExAll.Name = "lvExAll";
            this.lvExAll.Size = new System.Drawing.Size(737, 115);
            this.lvExAll.TabIndex = 2;
            this.lvExAll.UseCompatibleStateImageBehavior = false;
            this.lvExAll.View = System.Windows.Forms.View.Details;
            // 
            // colAllID
            // 
            this.colAllID.Text = "ID";
            this.colAllID.Width = 137;
            // 
            // colAllName
            // 
            this.colAllName.Text = "Component";
            this.colAllName.Width = 158;
            // 
            // colAllGroup
            // 
            this.colAllGroup.Text = "Group";
            this.colAllGroup.Width = 147;
            // 
            // colAllFPGA
            // 
            this.colAllFPGA.Text = "FPGA";
            this.colAllFPGA.Width = 118;
            // 
            // splitMapOptionsInfo
            // 
            this.splitMapOptionsInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMapOptionsInfo.Location = new System.Drawing.Point(0, 0);
            this.splitMapOptionsInfo.Name = "splitMapOptionsInfo";
            this.splitMapOptionsInfo.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMapOptionsInfo.Panel1
            // 
            this.splitMapOptionsInfo.Panel1.Controls.Add(this.grpMapOptions);
            // 
            // splitMapOptionsInfo.Panel2
            // 
            this.splitMapOptionsInfo.Panel2.Controls.Add(this.grpMappingSelectedInfo);
            this.splitMapOptionsInfo.Size = new System.Drawing.Size(275, 426);
            this.splitMapOptionsInfo.SplitterDistance = 87;
            this.splitMapOptionsInfo.TabIndex = 11;
            // 
            // grpMapOptions
            // 
            this.grpMapOptions.Controls.Add(this.lblMapIOWeight);
            this.grpMapOptions.Controls.Add(this.trackIOWeight);
            this.grpMapOptions.Controls.Add(this.chkMapIOEnable);
            this.grpMapOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpMapOptions.Location = new System.Drawing.Point(0, 0);
            this.grpMapOptions.Name = "grpMapOptions";
            this.grpMapOptions.Size = new System.Drawing.Size(275, 84);
            this.grpMapOptions.TabIndex = 10;
            this.grpMapOptions.TabStop = false;
            this.grpMapOptions.Text = "Mapping Options";
            // 
            // lblMapIOWeight
            // 
            this.lblMapIOWeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMapIOWeight.Location = new System.Drawing.Point(171, 13);
            this.lblMapIOWeight.Name = "lblMapIOWeight";
            this.lblMapIOWeight.Size = new System.Drawing.Size(99, 21);
            this.lblMapIOWeight.TabIndex = 2;
            this.lblMapIOWeight.Text = "50%";
            this.lblMapIOWeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // trackIOWeight
            // 
            this.trackIOWeight.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.trackIOWeight.LargeChange = 10;
            this.trackIOWeight.Location = new System.Drawing.Point(3, 36);
            this.trackIOWeight.Maximum = 100;
            this.trackIOWeight.Name = "trackIOWeight";
            this.trackIOWeight.Size = new System.Drawing.Size(269, 45);
            this.trackIOWeight.TabIndex = 1;
            this.trackIOWeight.Value = 50;
            // 
            // chkMapIOEnable
            // 
            this.chkMapIOEnable.AutoSize = true;
            this.chkMapIOEnable.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkMapIOEnable.Location = new System.Drawing.Point(3, 16);
            this.chkMapIOEnable.Name = "chkMapIOEnable";
            this.chkMapIOEnable.Size = new System.Drawing.Size(269, 17);
            this.chkMapIOEnable.TabIndex = 0;
            this.chkMapIOEnable.Text = "Enable I/O Weighting";
            this.chkMapIOEnable.UseVisualStyleBackColor = true;
            // 
            // grpMappingSelectedInfo
            // 
            this.grpMappingSelectedInfo.Controls.Add(this.lblMappingSelectedInfo);
            this.grpMappingSelectedInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMappingSelectedInfo.Location = new System.Drawing.Point(0, 0);
            this.grpMappingSelectedInfo.Name = "grpMappingSelectedInfo";
            this.grpMappingSelectedInfo.Size = new System.Drawing.Size(275, 335);
            this.grpMappingSelectedInfo.TabIndex = 11;
            this.grpMappingSelectedInfo.TabStop = false;
            this.grpMappingSelectedInfo.Text = "Selected Item Information";
            // 
            // lblMappingSelectedInfo
            // 
            this.lblMappingSelectedInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMappingSelectedInfo.Location = new System.Drawing.Point(3, 16);
            this.lblMappingSelectedInfo.Name = "lblMappingSelectedInfo";
            this.lblMappingSelectedInfo.Size = new System.Drawing.Size(269, 316);
            this.lblMappingSelectedInfo.TabIndex = 0;
            // 
            // tabSynthesis
            // 
            this.tabSynthesis.Controls.Add(this.splitContainer1);
            this.tabSynthesis.Location = new System.Drawing.Point(4, 22);
            this.tabSynthesis.Name = "tabSynthesis";
            this.tabSynthesis.Size = new System.Drawing.Size(1036, 432);
            this.tabSynthesis.TabIndex = 2;
            this.tabSynthesis.Tag = "synthesis";
            this.tabSynthesis.Text = "Synthesis";
            this.tabSynthesis.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnApplyFilter);
            this.splitContainer1.Panel1.Controls.Add(this.grpFilesContainer);
            this.splitContainer1.Panel1.Controls.Add(this.lblFilter);
            this.splitContainer1.Panel1.Controls.Add(this.txtFileFilter);
            this.splitContainer1.Panel1.Controls.Add(this.btnOpenSelected);
            this.splitContainer1.Panel1.Controls.Add(this.btnRefreshFiles);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.grpLiveLog);
            this.splitContainer1.Size = new System.Drawing.Size(1036, 432);
            this.splitContainer1.SplitterDistance = 345;
            this.splitContainer1.TabIndex = 2;
            // 
            // btnApplyFilter
            // 
            this.btnApplyFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyFilter.Location = new System.Drawing.Point(233, 406);
            this.btnApplyFilter.Name = "btnApplyFilter";
            this.btnApplyFilter.Size = new System.Drawing.Size(100, 23);
            this.btnApplyFilter.TabIndex = 7;
            this.btnApplyFilter.Text = "Apply Filter";
            this.btnApplyFilter.UseVisualStyleBackColor = true;
            // 
            // grpFilesContainer
            // 
            this.grpFilesContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFilesContainer.Controls.Add(this.treeProjectFiles);
            this.grpFilesContainer.Location = new System.Drawing.Point(8, 4);
            this.grpFilesContainer.Name = "grpFilesContainer";
            this.grpFilesContainer.Size = new System.Drawing.Size(325, 341);
            this.grpFilesContainer.TabIndex = 6;
            this.grpFilesContainer.TabStop = false;
            this.grpFilesContainer.Text = "Tool Output Files";
            // 
            // treeProjectFiles
            // 
            this.treeProjectFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeProjectFiles.Location = new System.Drawing.Point(3, 16);
            this.treeProjectFiles.Name = "treeProjectFiles";
            this.treeProjectFiles.Size = new System.Drawing.Size(319, 322);
            this.treeProjectFiles.TabIndex = 1;
            // 
            // lblFilter
            // 
            this.lblFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(5, 383);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(29, 13);
            this.lblFilter.TabIndex = 5;
            this.lblFilter.Text = "Filter";
            // 
            // txtFileFilter
            // 
            this.txtFileFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileFilter.Location = new System.Drawing.Point(43, 380);
            this.txtFileFilter.Name = "txtFileFilter";
            this.txtFileFilter.Size = new System.Drawing.Size(290, 20);
            this.txtFileFilter.TabIndex = 4;
            // 
            // btnOpenSelected
            // 
            this.btnOpenSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenSelected.Location = new System.Drawing.Point(233, 351);
            this.btnOpenSelected.Name = "btnOpenSelected";
            this.btnOpenSelected.Size = new System.Drawing.Size(100, 23);
            this.btnOpenSelected.TabIndex = 3;
            this.btnOpenSelected.Text = "Open Selected";
            this.btnOpenSelected.UseVisualStyleBackColor = true;
            // 
            // btnRefreshFiles
            // 
            this.btnRefreshFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefreshFiles.Location = new System.Drawing.Point(8, 351);
            this.btnRefreshFiles.Name = "btnRefreshFiles";
            this.btnRefreshFiles.Size = new System.Drawing.Size(101, 23);
            this.btnRefreshFiles.TabIndex = 1;
            this.btnRefreshFiles.Text = "Refresh Files";
            this.btnRefreshFiles.UseVisualStyleBackColor = true;
            // 
            // grpLiveLog
            // 
            this.grpLiveLog.Controls.Add(this.tabToolOutput);
            this.grpLiveLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLiveLog.Location = new System.Drawing.Point(0, 0);
            this.grpLiveLog.Name = "grpLiveLog";
            this.grpLiveLog.Size = new System.Drawing.Size(687, 432);
            this.grpLiveLog.TabIndex = 1;
            this.grpLiveLog.TabStop = false;
            this.grpLiveLog.Text = "Tool Output";
            // 
            // tabToolOutput
            // 
            this.tabToolOutput.Controls.Add(this.tabToolOutputPage);
            this.tabToolOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabToolOutput.Location = new System.Drawing.Point(3, 16);
            this.tabToolOutput.Name = "tabToolOutput";
            this.tabToolOutput.SelectedIndex = 0;
            this.tabToolOutput.Size = new System.Drawing.Size(681, 413);
            this.tabToolOutput.TabIndex = 1;
            // 
            // tabToolOutputPage
            // 
            this.tabToolOutputPage.Controls.Add(this.tbToolLiveLog);
            this.tabToolOutputPage.Location = new System.Drawing.Point(4, 22);
            this.tabToolOutputPage.Name = "tabToolOutputPage";
            this.tabToolOutputPage.Padding = new System.Windows.Forms.Padding(3);
            this.tabToolOutputPage.Size = new System.Drawing.Size(673, 387);
            this.tabToolOutputPage.TabIndex = 0;
            this.tabToolOutputPage.Text = "Combined";
            this.tabToolOutputPage.UseVisualStyleBackColor = true;
            // 
            // tbToolLiveLog
            // 
            this.tbToolLiveLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbToolLiveLog.Location = new System.Drawing.Point(3, 3);
            this.tbToolLiveLog.Multiline = true;
            this.tbToolLiveLog.Name = "tbToolLiveLog";
            this.tbToolLiveLog.ReadOnly = true;
            this.tbToolLiveLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbToolLiveLog.Size = new System.Drawing.Size(667, 381);
            this.tbToolLiveLog.TabIndex = 0;
            // 
            // tabSummary
            // 
            this.tabSummary.Controls.Add(this.button1);
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Size = new System.Drawing.Size(1036, 432);
            this.tabSummary.TabIndex = 3;
            this.tabSummary.Text = "Summary";
            this.tabSummary.UseVisualStyleBackColor = true;
            // 
            // loadPathDialog
            // 
            this.loadPathDialog.DefaultExt = "xml";
            this.loadPathDialog.FileName = "paths.xml";
            this.loadPathDialog.Filter = "Cerebrum Project Paths Files|paths.xml";
            this.loadPathDialog.Title = "Load Existing Cerebrum Project...";
            // 
            // splitMainDesignReport
            // 
            this.splitMainDesignReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMainDesignReport.Location = new System.Drawing.Point(0, 0);
            this.splitMainDesignReport.Name = "splitMainDesignReport";
            this.splitMainDesignReport.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMainDesignReport.Panel1
            // 
            this.splitMainDesignReport.Panel1.Controls.Add(this.designTabs);
            // 
            // splitMainDesignReport.Panel2
            // 
            this.splitMainDesignReport.Panel2.Controls.Add(this.tabsReport);
            this.splitMainDesignReport.Size = new System.Drawing.Size(1044, 624);
            this.splitMainDesignReport.SplitterDistance = 458;
            this.splitMainDesignReport.TabIndex = 3;
            // 
            // tabsReport
            // 
            this.tabsReport.Controls.Add(this.tabMessages);
            this.tabsReport.Controls.Add(this.tabInfo);
            this.tabsReport.Controls.Add(this.tabWarnings);
            this.tabsReport.Controls.Add(this.tabErrors);
            this.tabsReport.Controls.Add(this.tabConsole);
            this.tabsReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsReport.ImageList = this.messageImages;
            this.tabsReport.Location = new System.Drawing.Point(0, 0);
            this.tabsReport.Name = "tabsReport";
            this.tabsReport.SelectedIndex = 0;
            this.tabsReport.Size = new System.Drawing.Size(1044, 162);
            this.tabsReport.TabIndex = 3;
            // 
            // tabMessages
            // 
            this.tabMessages.Controls.Add(this.lvAllMessages);
            this.tabMessages.ImageKey = "icoMessages";
            this.tabMessages.Location = new System.Drawing.Point(4, 23);
            this.tabMessages.Name = "tabMessages";
            this.tabMessages.Padding = new System.Windows.Forms.Padding(3);
            this.tabMessages.Size = new System.Drawing.Size(1036, 135);
            this.tabMessages.TabIndex = 0;
            this.tabMessages.Tag = "";
            this.tabMessages.Text = "All Messages";
            this.tabMessages.UseVisualStyleBackColor = true;
            // 
            // lvAllMessages
            // 
            this.lvAllMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colMessageType,
            this.colMessageID,
            this.colMessageString,
            this.colMessageSource});
            this.lvAllMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvAllMessages.FullRowSelect = true;
            this.lvAllMessages.GridLines = true;
            this.lvAllMessages.Location = new System.Drawing.Point(3, 3);
            this.lvAllMessages.Name = "lvAllMessages";
            this.lvAllMessages.ShowGroups = false;
            this.lvAllMessages.Size = new System.Drawing.Size(1030, 129);
            this.lvAllMessages.TabIndex = 0;
            this.lvAllMessages.UseCompatibleStateImageBehavior = false;
            this.lvAllMessages.View = System.Windows.Forms.View.Details;
            // 
            // colMessageType
            // 
            this.colMessageType.Text = "Type";
            this.colMessageType.Width = 66;
            // 
            // colMessageID
            // 
            this.colMessageID.Text = "ID";
            this.colMessageID.Width = 103;
            // 
            // colMessageString
            // 
            this.colMessageString.Text = "Message";
            this.colMessageString.Width = 409;
            // 
            // colMessageSource
            // 
            this.colMessageSource.Text = "Source";
            this.colMessageSource.Width = 195;
            // 
            // tabInfo
            // 
            this.tabInfo.Controls.Add(this.lvInfo);
            this.tabInfo.ImageKey = "icoInfo";
            this.tabInfo.Location = new System.Drawing.Point(4, 23);
            this.tabInfo.Name = "tabInfo";
            this.tabInfo.Size = new System.Drawing.Size(1036, 135);
            this.tabInfo.TabIndex = 4;
            this.tabInfo.Text = "Information";
            this.tabInfo.UseVisualStyleBackColor = true;
            // 
            // lvInfo
            // 
            this.lvInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colInfoType,
            this.colInfoID,
            this.colInfoMessage,
            this.colInfoSource});
            this.lvInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvInfo.FullRowSelect = true;
            this.lvInfo.GridLines = true;
            this.lvInfo.Location = new System.Drawing.Point(0, 0);
            this.lvInfo.Name = "lvInfo";
            this.lvInfo.ShowGroups = false;
            this.lvInfo.Size = new System.Drawing.Size(1036, 135);
            this.lvInfo.TabIndex = 1;
            this.lvInfo.UseCompatibleStateImageBehavior = false;
            this.lvInfo.View = System.Windows.Forms.View.Details;
            // 
            // colInfoType
            // 
            this.colInfoType.Text = "Type";
            this.colInfoType.Width = 66;
            // 
            // colInfoID
            // 
            this.colInfoID.Text = "ID";
            this.colInfoID.Width = 107;
            // 
            // colInfoMessage
            // 
            this.colInfoMessage.Text = "Message";
            this.colInfoMessage.Width = 409;
            // 
            // colInfoSource
            // 
            this.colInfoSource.Text = "Source";
            this.colInfoSource.Width = 195;
            // 
            // tabWarnings
            // 
            this.tabWarnings.Controls.Add(this.lvWarnings);
            this.tabWarnings.ImageKey = "icoWarning";
            this.tabWarnings.Location = new System.Drawing.Point(4, 23);
            this.tabWarnings.Name = "tabWarnings";
            this.tabWarnings.Padding = new System.Windows.Forms.Padding(3);
            this.tabWarnings.Size = new System.Drawing.Size(1036, 135);
            this.tabWarnings.TabIndex = 1;
            this.tabWarnings.Tag = "";
            this.tabWarnings.Text = "Warnings";
            this.tabWarnings.UseVisualStyleBackColor = true;
            // 
            // lvWarnings
            // 
            this.lvWarnings.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colWarningType,
            this.colWarningID,
            this.colWarningMessage,
            this.colWarningSource});
            this.lvWarnings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvWarnings.FullRowSelect = true;
            this.lvWarnings.GridLines = true;
            this.lvWarnings.Location = new System.Drawing.Point(3, 3);
            this.lvWarnings.Name = "lvWarnings";
            this.lvWarnings.ShowGroups = false;
            this.lvWarnings.Size = new System.Drawing.Size(1030, 129);
            this.lvWarnings.TabIndex = 1;
            this.lvWarnings.UseCompatibleStateImageBehavior = false;
            this.lvWarnings.View = System.Windows.Forms.View.Details;
            // 
            // colWarningType
            // 
            this.colWarningType.Text = "Type";
            this.colWarningType.Width = 66;
            // 
            // colWarningID
            // 
            this.colWarningID.Text = "ID";
            this.colWarningID.Width = 104;
            // 
            // colWarningMessage
            // 
            this.colWarningMessage.Text = "Message";
            this.colWarningMessage.Width = 409;
            // 
            // colWarningSource
            // 
            this.colWarningSource.Text = "Source";
            this.colWarningSource.Width = 195;
            // 
            // tabErrors
            // 
            this.tabErrors.Controls.Add(this.lvErrors);
            this.tabErrors.ImageKey = "icoError";
            this.tabErrors.Location = new System.Drawing.Point(4, 23);
            this.tabErrors.Name = "tabErrors";
            this.tabErrors.Size = new System.Drawing.Size(1036, 135);
            this.tabErrors.TabIndex = 2;
            this.tabErrors.Tag = "";
            this.tabErrors.Text = "Errors";
            this.tabErrors.UseVisualStyleBackColor = true;
            // 
            // lvErrors
            // 
            this.lvErrors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colErrorType,
            this.colErrorID,
            this.colErrorMessage,
            this.colErrorSource});
            this.lvErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvErrors.FullRowSelect = true;
            this.lvErrors.GridLines = true;
            this.lvErrors.Location = new System.Drawing.Point(0, 0);
            this.lvErrors.Name = "lvErrors";
            this.lvErrors.ShowGroups = false;
            this.lvErrors.Size = new System.Drawing.Size(1036, 135);
            this.lvErrors.TabIndex = 1;
            this.lvErrors.UseCompatibleStateImageBehavior = false;
            this.lvErrors.View = System.Windows.Forms.View.Details;
            // 
            // colErrorType
            // 
            this.colErrorType.Text = "Type";
            this.colErrorType.Width = 66;
            // 
            // colErrorID
            // 
            this.colErrorID.Text = "ID";
            this.colErrorID.Width = 113;
            // 
            // colErrorMessage
            // 
            this.colErrorMessage.Text = "Message";
            this.colErrorMessage.Width = 409;
            // 
            // colErrorSource
            // 
            this.colErrorSource.Text = "Source";
            this.colErrorSource.Width = 195;
            // 
            // tabConsole
            // 
            this.tabConsole.Controls.Add(this.tbConsole);
            this.tabConsole.ImageKey = "icoConsole";
            this.tabConsole.Location = new System.Drawing.Point(4, 23);
            this.tabConsole.Name = "tabConsole";
            this.tabConsole.Size = new System.Drawing.Size(1036, 135);
            this.tabConsole.TabIndex = 3;
            this.tabConsole.Text = "Console";
            this.tabConsole.UseVisualStyleBackColor = true;
            // 
            // tbConsole
            // 
            this.tbConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbConsole.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbConsole.Location = new System.Drawing.Point(0, 0);
            this.tbConsole.Multiline = true;
            this.tbConsole.Name = "tbConsole";
            this.tbConsole.ReadOnly = true;
            this.tbConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbConsole.Size = new System.Drawing.Size(1036, 135);
            this.tbConsole.TabIndex = 0;
            // 
            // messageImages
            // 
            this.messageImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("messageImages.ImageStream")));
            this.messageImages.TransparentColor = System.Drawing.Color.Transparent;
            this.messageImages.Images.SetKeyName(0, "icoError");
            this.messageImages.Images.SetKeyName(1, "icoWarning");
            this.messageImages.Images.SetKeyName(2, "icoInfo");
            this.messageImages.Images.SetKeyName(3, "icoConsole");
            this.messageImages.Images.SetKeyName(4, "icoMessages");
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(0, 748);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(1044, 22);
            this.status.TabIndex = 4;
            this.status.Text = "statusStrip1";
            // 
            // container
            // 
            // 
            // container.ContentPanel
            // 
            this.container.ContentPanel.Controls.Add(this.splitMainDesignReport);
            this.container.ContentPanel.Size = new System.Drawing.Size(1044, 624);
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container.Location = new System.Drawing.Point(0, 24);
            this.container.Name = "container";
            this.container.Size = new System.Drawing.Size(1044, 724);
            this.container.TabIndex = 5;
            this.container.Text = "toolStripContainer1";
            // 
            // container.TopToolStripPanel
            // 
            this.container.TopToolStripPanel.Controls.Add(this.toolStripHelp);
            this.container.TopToolStripPanel.Controls.Add(this.toolStripMapping);
            this.container.TopToolStripPanel.Controls.Add(this.toolStripProject);
            this.container.TopToolStripPanel.Controls.Add(this.toolStripDesign);
            this.container.TopToolStripPanel.Controls.Add(this.toolStripSynthesis);
            // 
            // toolStripHelp
            // 
            this.toolStripHelp.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripHelp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBtnHelpHelp,
            this.toolBtnHelpAbout});
            this.toolStripHelp.Location = new System.Drawing.Point(92, 0);
            this.toolStripHelp.Name = "toolStripHelp";
            this.toolStripHelp.Size = new System.Drawing.Size(58, 25);
            this.toolStripHelp.TabIndex = 3;
            // 
            // toolBtnHelpHelp
            // 
            this.toolBtnHelpHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnHelpHelp.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnHelpHelp.Image")));
            this.toolBtnHelpHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnHelpHelp.Name = "toolBtnHelpHelp";
            this.toolBtnHelpHelp.Size = new System.Drawing.Size(23, 22);
            this.toolBtnHelpHelp.Text = "Cerebrum Help";
            // 
            // toolBtnHelpAbout
            // 
            this.toolBtnHelpAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnHelpAbout.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnHelpAbout.Image")));
            this.toolBtnHelpAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnHelpAbout.Name = "toolBtnHelpAbout";
            this.toolBtnHelpAbout.Size = new System.Drawing.Size(23, 22);
            this.toolBtnHelpAbout.Text = "About Cerebrum";
            // 
            // toolStripMapping
            // 
            this.toolStripMapping.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripMapping.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBtnMappingReset,
            this.toolBtnMappingComplete,
            this.toolBtnMappingConfirm,
            this.toolStripMappingSeparator1,
            this.toolBtnMappingSavePreset,
            this.toolBtnMappingLoadPreset,
            this.toolStripMappingSeparator2,
            this.toolBtnMappingModeManual,
            this.toolBtnMappingModeQuery});
            this.toolStripMapping.Location = new System.Drawing.Point(3, 25);
            this.toolStripMapping.Name = "toolStripMapping";
            this.toolStripMapping.Size = new System.Drawing.Size(185, 25);
            this.toolStripMapping.TabIndex = 2;
            // 
            // toolBtnMappingReset
            // 
            this.toolBtnMappingReset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnMappingReset.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnMappingReset.Image")));
            this.toolBtnMappingReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnMappingReset.Name = "toolBtnMappingReset";
            this.toolBtnMappingReset.Size = new System.Drawing.Size(23, 22);
            this.toolBtnMappingReset.Text = "Reset Mapping";
            // 
            // toolBtnMappingComplete
            // 
            this.toolBtnMappingComplete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnMappingComplete.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnMappingComplete.Image")));
            this.toolBtnMappingComplete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnMappingComplete.Name = "toolBtnMappingComplete";
            this.toolBtnMappingComplete.Size = new System.Drawing.Size(23, 22);
            this.toolBtnMappingComplete.Text = "Auto-Complete Mapping";
            // 
            // toolBtnMappingConfirm
            // 
            this.toolBtnMappingConfirm.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnMappingConfirm.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnMappingConfirm.Image")));
            this.toolBtnMappingConfirm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnMappingConfirm.Name = "toolBtnMappingConfirm";
            this.toolBtnMappingConfirm.Size = new System.Drawing.Size(23, 22);
            this.toolBtnMappingConfirm.Text = "Confirm Current Mapping";
            // 
            // toolStripMappingSeparator1
            // 
            this.toolStripMappingSeparator1.Name = "toolStripMappingSeparator1";
            this.toolStripMappingSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolBtnMappingSavePreset
            // 
            this.toolBtnMappingSavePreset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnMappingSavePreset.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnMappingSavePreset.Image")));
            this.toolBtnMappingSavePreset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnMappingSavePreset.Name = "toolBtnMappingSavePreset";
            this.toolBtnMappingSavePreset.Size = new System.Drawing.Size(23, 22);
            this.toolBtnMappingSavePreset.Text = "Save Mapping Preset";
            // 
            // toolBtnMappingLoadPreset
            // 
            this.toolBtnMappingLoadPreset.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnMappingLoadPreset.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnMappingLoadPreset.Image")));
            this.toolBtnMappingLoadPreset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnMappingLoadPreset.Name = "toolBtnMappingLoadPreset";
            this.toolBtnMappingLoadPreset.Size = new System.Drawing.Size(23, 22);
            this.toolBtnMappingLoadPreset.Text = "Load Mapping Preset";
            // 
            // toolStripMappingSeparator2
            // 
            this.toolStripMappingSeparator2.Name = "toolStripMappingSeparator2";
            this.toolStripMappingSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolBtnMappingModeManual
            // 
            this.toolBtnMappingModeManual.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnMappingModeManual.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnMappingModeManual.Image")));
            this.toolBtnMappingModeManual.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnMappingModeManual.Name = "toolBtnMappingModeManual";
            this.toolBtnMappingModeManual.Size = new System.Drawing.Size(23, 22);
            this.toolBtnMappingModeManual.Text = "Toggle Mapping Mode: Manual Mapping";
            // 
            // toolBtnMappingModeQuery
            // 
            this.toolBtnMappingModeQuery.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnMappingModeQuery.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnMappingModeQuery.Image")));
            this.toolBtnMappingModeQuery.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnMappingModeQuery.Name = "toolBtnMappingModeQuery";
            this.toolBtnMappingModeQuery.Size = new System.Drawing.Size(23, 22);
            this.toolBtnMappingModeQuery.Text = "Toggle Mapping Mode: Query Information";
            // 
            // toolStripDesign
            // 
            this.toolStripDesign.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripDesign.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBtnDesignConfigureProcessors,
            this.toolBtnDesignConfigureCommunications,
            this.toolBtnDesignConfigureProgramming});
            this.toolStripDesign.Location = new System.Drawing.Point(107, 75);
            this.toolStripDesign.Name = "toolStripDesign";
            this.toolStripDesign.Size = new System.Drawing.Size(81, 25);
            this.toolStripDesign.TabIndex = 1;
            // 
            // toolBtnDesignConfigureProcessors
            // 
            this.toolBtnDesignConfigureProcessors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnDesignConfigureProcessors.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnDesignConfigureProcessors.Image")));
            this.toolBtnDesignConfigureProcessors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnDesignConfigureProcessors.Name = "toolBtnDesignConfigureProcessors";
            this.toolBtnDesignConfigureProcessors.Size = new System.Drawing.Size(23, 22);
            this.toolBtnDesignConfigureProcessors.Text = "Configure Processor(s)";
            // 
            // toolBtnDesignConfigureCommunications
            // 
            this.toolBtnDesignConfigureCommunications.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnDesignConfigureCommunications.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnDesignConfigureCommunications.Image")));
            this.toolBtnDesignConfigureCommunications.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnDesignConfigureCommunications.Name = "toolBtnDesignConfigureCommunications";
            this.toolBtnDesignConfigureCommunications.Size = new System.Drawing.Size(23, 22);
            this.toolBtnDesignConfigureCommunications.Text = "Configure Communications";
            // 
            // toolBtnDesignConfigureProgramming
            // 
            this.toolBtnDesignConfigureProgramming.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnDesignConfigureProgramming.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnDesignConfigureProgramming.Image")));
            this.toolBtnDesignConfigureProgramming.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnDesignConfigureProgramming.Name = "toolBtnDesignConfigureProgramming";
            this.toolBtnDesignConfigureProgramming.Size = new System.Drawing.Size(23, 22);
            this.toolBtnDesignConfigureProgramming.Text = "Configure Programming";
            // 
            // toolStripProject
            // 
            this.toolStripProject.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripProject.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBtnFileNewWizard,
            this.toolBtnFileOpen,
            this.toolBtnFileSave,
            this.toolBtnFileSaveCopy,
            this.projectToolStripSeparator1,
            this.toolBtnProjectPathSettings,
            this.toolBtnProjectEditServerLists,
            this.toolBtnProjectProperties,
            this.projectToolStripSeparator2,
            this.toolBtnEditPreferences});
            this.toolStripProject.Location = new System.Drawing.Point(3, 50);
            this.toolStripProject.Name = "toolStripProject";
            this.toolStripProject.Size = new System.Drawing.Size(208, 25);
            this.toolStripProject.TabIndex = 0;
            // 
            // toolBtnFileNewWizard
            // 
            this.toolBtnFileNewWizard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnFileNewWizard.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnFileNewWizard.Image")));
            this.toolBtnFileNewWizard.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnFileNewWizard.Name = "toolBtnFileNewWizard";
            this.toolBtnFileNewWizard.Size = new System.Drawing.Size(23, 22);
            this.toolBtnFileNewWizard.Text = "New Project";
            // 
            // toolBtnFileOpen
            // 
            this.toolBtnFileOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnFileOpen.Image")));
            this.toolBtnFileOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnFileOpen.Name = "toolBtnFileOpen";
            this.toolBtnFileOpen.Size = new System.Drawing.Size(23, 22);
            this.toolBtnFileOpen.Text = "Open Project";
            // 
            // toolBtnFileSave
            // 
            this.toolBtnFileSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnFileSave.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnFileSave.Image")));
            this.toolBtnFileSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnFileSave.Name = "toolBtnFileSave";
            this.toolBtnFileSave.Size = new System.Drawing.Size(23, 22);
            this.toolBtnFileSave.Text = "Save Project";
            // 
            // toolBtnFileSaveCopy
            // 
            this.toolBtnFileSaveCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnFileSaveCopy.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnFileSaveCopy.Image")));
            this.toolBtnFileSaveCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnFileSaveCopy.Name = "toolBtnFileSaveCopy";
            this.toolBtnFileSaveCopy.Size = new System.Drawing.Size(23, 22);
            this.toolBtnFileSaveCopy.Text = "Save Project Copy";
            // 
            // projectToolStripSeparator1
            // 
            this.projectToolStripSeparator1.Name = "projectToolStripSeparator1";
            this.projectToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolBtnProjectPathSettings
            // 
            this.toolBtnProjectPathSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnProjectPathSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnProjectPathSettings.Image")));
            this.toolBtnProjectPathSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnProjectPathSettings.Name = "toolBtnProjectPathSettings";
            this.toolBtnProjectPathSettings.Size = new System.Drawing.Size(23, 22);
            this.toolBtnProjectPathSettings.Text = "toolStripButton1";
            this.toolBtnProjectPathSettings.ToolTipText = "Edit Project Paths";
            // 
            // toolBtnProjectEditServerLists
            // 
            this.toolBtnProjectEditServerLists.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnProjectEditServerLists.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnProjectEditServerLists.Image")));
            this.toolBtnProjectEditServerLists.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnProjectEditServerLists.Name = "toolBtnProjectEditServerLists";
            this.toolBtnProjectEditServerLists.Size = new System.Drawing.Size(23, 22);
            this.toolBtnProjectEditServerLists.Text = "Edit Server Lists";
            // 
            // toolBtnProjectProperties
            // 
            this.toolBtnProjectProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnProjectProperties.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnProjectProperties.Image")));
            this.toolBtnProjectProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnProjectProperties.Name = "toolBtnProjectProperties";
            this.toolBtnProjectProperties.Size = new System.Drawing.Size(23, 22);
            this.toolBtnProjectProperties.Text = "Properties";
            // 
            // projectToolStripSeparator2
            // 
            this.projectToolStripSeparator2.Name = "projectToolStripSeparator2";
            this.projectToolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolBtnEditPreferences
            // 
            this.toolBtnEditPreferences.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnEditPreferences.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnEditPreferences.Image")));
            this.toolBtnEditPreferences.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnEditPreferences.Name = "toolBtnEditPreferences";
            this.toolBtnEditPreferences.Size = new System.Drawing.Size(23, 22);
            this.toolBtnEditPreferences.Text = "Cerebrum Preferences";
            // 
            // toolStripSynthesis
            // 
            this.toolStripSynthesis.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripSynthesis.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolBtnSynthesisBuildXPS,
            this.toolBtnSynthesisSynthesize,
            this.toolBtnSynthesisProgram,
            this.toolBtnSynthesisStartConfigServer});
            this.toolStripSynthesis.Location = new System.Drawing.Point(3, 75);
            this.toolStripSynthesis.Name = "toolStripSynthesis";
            this.toolStripSynthesis.Size = new System.Drawing.Size(104, 25);
            this.toolStripSynthesis.TabIndex = 4;
            // 
            // toolBtnSynthesisBuildXPS
            // 
            this.toolBtnSynthesisBuildXPS.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnSynthesisBuildXPS.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnSynthesisBuildXPS.Image")));
            this.toolBtnSynthesisBuildXPS.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnSynthesisBuildXPS.Name = "toolBtnSynthesisBuildXPS";
            this.toolBtnSynthesisBuildXPS.Size = new System.Drawing.Size(23, 22);
            this.toolBtnSynthesisBuildXPS.Text = "Build XPS Projects";
            // 
            // toolBtnSynthesisSynthesize
            // 
            this.toolBtnSynthesisSynthesize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnSynthesisSynthesize.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnSynthesisSynthesize.Image")));
            this.toolBtnSynthesisSynthesize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnSynthesisSynthesize.Name = "toolBtnSynthesisSynthesize";
            this.toolBtnSynthesisSynthesize.Size = new System.Drawing.Size(23, 22);
            this.toolBtnSynthesisSynthesize.Text = "Synthesize XPS Projects";
            // 
            // toolBtnSynthesisProgram
            // 
            this.toolBtnSynthesisProgram.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnSynthesisProgram.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnSynthesisProgram.Image")));
            this.toolBtnSynthesisProgram.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnSynthesisProgram.Name = "toolBtnSynthesisProgram";
            this.toolBtnSynthesisProgram.Size = new System.Drawing.Size(23, 22);
            this.toolBtnSynthesisProgram.Text = "Program FPGA(s)...";
            // 
            // toolBtnSynthesisStartConfigServer
            // 
            this.toolBtnSynthesisStartConfigServer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolBtnSynthesisStartConfigServer.Image = ((System.Drawing.Image)(resources.GetObject("toolBtnSynthesisStartConfigServer.Image")));
            this.toolBtnSynthesisStartConfigServer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnSynthesisStartConfigServer.Name = "toolBtnSynthesisStartConfigServer";
            this.toolBtnSynthesisStartConfigServer.Size = new System.Drawing.Size(23, 22);
            this.toolBtnSynthesisStartConfigServer.Text = "Start Runtime Configuration Server";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(126, 81);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "To Be Removed";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmCerebrumGUIMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1044, 770);
            this.Controls.Add(this.container);
            this.Controls.Add(this.status);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "frmCerebrumGUIMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cerebrum System Designer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.designTabs.ResumeLayout(false);
            this.tabDesign.ResumeLayout(false);
            this.tabMapping.ResumeLayout(false);
            this.splitMappingGUIInfo.Panel1.ResumeLayout(false);
            this.splitMappingGUIInfo.Panel2.ResumeLayout(false);
            this.splitMappingGUIInfo.ResumeLayout(false);
            this.mapTabLeftHorizontalSplitter.Panel1.ResumeLayout(false);
            this.mapTabLeftHorizontalSplitter.Panel2.ResumeLayout(false);
            this.mapTabLeftHorizontalSplitter.ResumeLayout(false);
            this.tabsMapping.ResumeLayout(false);
            this.tabAll.ResumeLayout(false);
            this.splitMapOptionsInfo.Panel1.ResumeLayout(false);
            this.splitMapOptionsInfo.Panel2.ResumeLayout(false);
            this.splitMapOptionsInfo.ResumeLayout(false);
            this.grpMapOptions.ResumeLayout(false);
            this.grpMapOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackIOWeight)).EndInit();
            this.grpMappingSelectedInfo.ResumeLayout(false);
            this.tabSynthesis.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.grpFilesContainer.ResumeLayout(false);
            this.grpLiveLog.ResumeLayout(false);
            this.tabToolOutput.ResumeLayout(false);
            this.tabToolOutputPage.ResumeLayout(false);
            this.tabToolOutputPage.PerformLayout();
            this.tabSummary.ResumeLayout(false);
            this.splitMainDesignReport.Panel1.ResumeLayout(false);
            this.splitMainDesignReport.Panel2.ResumeLayout(false);
            this.splitMainDesignReport.ResumeLayout(false);
            this.tabsReport.ResumeLayout(false);
            this.tabMessages.ResumeLayout(false);
            this.tabInfo.ResumeLayout(false);
            this.tabWarnings.ResumeLayout(false);
            this.tabErrors.ResumeLayout(false);
            this.tabConsole.ResumeLayout(false);
            this.tabConsole.PerformLayout();
            this.container.ContentPanel.ResumeLayout(false);
            this.container.TopToolStripPanel.ResumeLayout(false);
            this.container.TopToolStripPanel.PerformLayout();
            this.container.ResumeLayout(false);
            this.container.PerformLayout();
            this.toolStripHelp.ResumeLayout(false);
            this.toolStripHelp.PerformLayout();
            this.toolStripMapping.ResumeLayout(false);
            this.toolStripMapping.PerformLayout();
            this.toolStripDesign.ResumeLayout(false);
            this.toolStripDesign.PerformLayout();
            this.toolStripProject.ResumeLayout(false);
            this.toolStripProject.PerformLayout();
            this.toolStripSynthesis.ResumeLayout(false);
            this.toolStripSynthesis.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileNew;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileOpen;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileSaveCopy;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileSave;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileClose;
        private System.Windows.Forms.ToolStripSeparator menuItemFileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileRecent;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileRecentClearList;
        private System.Windows.Forms.ToolStripSeparator menuItemFileRecentSeparator1;
        private System.Windows.Forms.ToolStripSeparator menuItemFileSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuItemFilePrint;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileExit;
        private System.Windows.Forms.ToolStripMenuItem menuEdit;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditCut;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditCopy;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditPaste;
        private System.Windows.Forms.ToolStripSeparator menuItemEditSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditPreferences;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuProject;
        private System.Windows.Forms.ToolStripMenuItem menuItemProjectProperties;
        private System.Windows.Forms.ToolStripMenuItem menuDesign;
        private System.Windows.Forms.ToolStripMenuItem menuMapping;
        private System.Windows.Forms.ToolStripMenuItem menuSynthesis;
        private System.Windows.Forms.ToolStripMenuItem menuWindows;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;
        private System.Windows.Forms.ToolStripMenuItem menuItemHelpHelp;
        private System.Windows.Forms.ToolStripMenuItem menuItemHelpAbout;
        private System.Windows.Forms.TabControl designTabs;
        private System.Windows.Forms.TabPage tabDesign;
        private System.Windows.Forms.TabPage tabMapping;
        private System.Windows.Forms.TabPage tabSynthesis;
        private System.Windows.Forms.TabPage tabSummary;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileNewWizard;
        private System.Windows.Forms.ToolStripMenuItem menuItemFileNewEmpty;
        private System.Windows.Forms.OpenFileDialog loadPathDialog;
        private System.Windows.Forms.SplitContainer splitMainDesignReport;
        private System.Windows.Forms.TabControl tabsReport;
        private System.Windows.Forms.TabPage tabMessages;
        private System.Windows.Forms.TabPage tabWarnings;
        private System.Windows.Forms.TabPage tabErrors;
        private System.Windows.Forms.TabPage tabConsole;
        private System.Windows.Forms.ToolStripMenuItem menuItemDesignConfigureProcessors;
        private System.Windows.Forms.ToolStripMenuItem menuItemDesignConfigureCommunications;
        private System.Windows.Forms.ToolStripMenuItem menuItemDesignConfigureProgramming;
        private System.Windows.Forms.ToolStripMenuItem menuItemProjectPathSettings;
        private System.Windows.Forms.ListView lvAllMessages;
        private System.Windows.Forms.StatusStrip status;
        private System.Windows.Forms.ColumnHeader colMessageType;
        private System.Windows.Forms.ColumnHeader colMessageID;
        private System.Windows.Forms.ColumnHeader colMessageString;
        private System.Windows.Forms.ColumnHeader colMessageSource;
        private System.Windows.Forms.ListView lvErrors;
        private System.Windows.Forms.ColumnHeader colErrorType;
        private System.Windows.Forms.ColumnHeader colErrorID;
        private System.Windows.Forms.ColumnHeader colErrorMessage;
        private System.Windows.Forms.ColumnHeader colErrorSource;
        private System.Windows.Forms.ListView lvWarnings;
        private System.Windows.Forms.ColumnHeader colWarningType;
        private System.Windows.Forms.ColumnHeader colWarningID;
        private System.Windows.Forms.ColumnHeader colWarningMessage;
        private System.Windows.Forms.ColumnHeader colWarningSource;
        private System.Windows.Forms.TextBox tbConsole;
        private System.Windows.Forms.TabPage tabInfo;
        private System.Windows.Forms.ListView lvInfo;
        private System.Windows.Forms.ColumnHeader colInfoType;
        private System.Windows.Forms.ColumnHeader colInfoID;
        private System.Windows.Forms.ColumnHeader colInfoMessage;
        private System.Windows.Forms.ColumnHeader colInfoSource;
        private System.Windows.Forms.ToolTip tips;
        private System.Windows.Forms.ToolStripMenuItem menuItemSynthesisBuildXPS;
        private System.Windows.Forms.ToolStripMenuItem menuItemSynthesisSynthesize;
        private System.Windows.Forms.ToolStripMenuItem menuItemSynthesisProgram;
        private System.Windows.Forms.ToolStripMenuItem menuItemSynthesisStartConfigServer;
        private System.Windows.Forms.ImageList messageImages;
        private System.Windows.Forms.ToolStripContainer container;
        private System.Windows.Forms.ToolStrip toolStripProject;
        private System.Windows.Forms.ToolStripButton toolBtnFileNewWizard;
        private System.Windows.Forms.ToolStripButton toolBtnFileOpen;
        private System.Windows.Forms.ToolStripButton toolBtnFileSave;
        private System.Windows.Forms.ToolStripButton toolBtnFileSaveCopy;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewToolbars;
        private System.Windows.Forms.ToolStripButton toolBtnProjectPathSettings;
        private System.Windows.Forms.ToolStripButton toolBtnProjectProperties;
        private System.Windows.Forms.ToolStripSeparator projectToolStripSeparator1;
        private System.Windows.Forms.ToolStrip toolStripDesign;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewToolbarsProject;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewToolbarsDesign;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewToolbarsMapping;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewToolbarsSynthesis;
        private System.Windows.Forms.ToolStripButton toolBtnDesignConfigureProcessors;
        private System.Windows.Forms.ToolStripButton toolBtnDesignConfigureCommunications;
        private System.Windows.Forms.ToolStripButton toolBtnDesignConfigureProgramming;
        private System.Windows.Forms.ToolStripMenuItem menuItemViewToolbarsHelp;
        private System.Windows.Forms.ToolStripMenuItem menuItemMappingReset;
        private System.Windows.Forms.ToolStripMenuItem menuItemMappingComplete;
        private System.Windows.Forms.ToolStripMenuItem menuItemMappingConfirm;
        private System.Windows.Forms.ToolStripMenuItem menuItemMappingLoadPreset;
        private System.Windows.Forms.ToolStripSeparator menuItemMappingSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemMappingMode;
        private System.Windows.Forms.ToolStripMenuItem menuItemMappingModeManual;
        private System.Windows.Forms.ToolStripMenuItem menuItemMappingModeQuery;
        private System.Windows.Forms.ToolStrip toolStripMapping;
        private System.Windows.Forms.ToolStripButton toolBtnMappingReset;
        private System.Windows.Forms.ToolStripButton toolBtnMappingComplete;
        private System.Windows.Forms.ToolStripButton toolBtnMappingConfirm;
        private System.Windows.Forms.ToolStripSeparator toolStripMappingSeparator1;
        private System.Windows.Forms.ToolStripButton toolBtnMappingModeManual;
        private System.Windows.Forms.ToolStripButton toolBtnMappingModeQuery;
        private System.Windows.Forms.SplitContainer splitMappingGUIInfo;
        private System.Windows.Forms.SplitContainer mapTabLeftHorizontalSplitter;
        private System.Windows.Forms.Integration.ElementHost mappingHost;
        private CerebrumMappingControls.MappingCanvasControl mappingCanvas;
        private System.Windows.Forms.TabControl tabsMapping;
        private System.Windows.Forms.TabPage tabAll;
        private CerebrumSharedClasses.ListViewEx lvExAll;
        private System.Windows.Forms.ColumnHeader colAllID;
        private System.Windows.Forms.ColumnHeader colAllName;
        private System.Windows.Forms.ColumnHeader colAllGroup;
        private System.Windows.Forms.ColumnHeader colAllFPGA;
        private System.Windows.Forms.ToolStripSeparator projectToolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolBtnEditPreferences;
        private System.Windows.Forms.ToolStripMenuItem menuItemMappingSavePreset;
        private System.Windows.Forms.ToolStripSeparator menuItemMappingSeparator2;
        private System.Windows.Forms.ToolStripButton toolBtnMappingSavePreset;
        private System.Windows.Forms.ToolStripButton toolBtnMappingLoadPreset;
        private System.Windows.Forms.ToolStripSeparator toolStripMappingSeparator2;
        private System.Windows.Forms.ToolStrip toolStripHelp;
        private System.Windows.Forms.ToolStrip toolStripSynthesis;
        private System.Windows.Forms.ToolStripButton toolBtnHelpHelp;
        private System.Windows.Forms.ToolStripButton toolBtnHelpAbout;
        private System.Windows.Forms.ToolStripButton toolBtnSynthesisBuildXPS;
        private System.Windows.Forms.ToolStripButton toolBtnSynthesisSynthesize;
        private System.Windows.Forms.ToolStripButton toolBtnSynthesisProgram;
        private System.Windows.Forms.ToolStripButton toolBtnSynthesisStartConfigServer;
        private CerebrumNetronObjects.CerebrumProjectPanel projectPanel;
        private System.Windows.Forms.ToolStripMenuItem menuItemProjectEditServerLists;
        private System.Windows.Forms.ToolStripButton toolBtnProjectEditServerLists;
        private System.Windows.Forms.SplitContainer splitMapOptionsInfo;
        private System.Windows.Forms.GroupBox grpMapOptions;
        private System.Windows.Forms.Label lblMapIOWeight;
        private System.Windows.Forms.TrackBar trackIOWeight;
        private System.Windows.Forms.CheckBox chkMapIOEnable;
        private System.Windows.Forms.GroupBox grpMappingSelectedInfo;
        private System.Windows.Forms.Label lblMappingSelectedInfo;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox grpLiveLog;
        private System.Windows.Forms.TextBox tbToolLiveLog;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.TextBox txtFileFilter;
        private System.Windows.Forms.Button btnOpenSelected;
        private System.Windows.Forms.Button btnRefreshFiles;
        private System.Windows.Forms.GroupBox grpFilesContainer;
        private System.Windows.Forms.TreeView treeProjectFiles;
        private System.Windows.Forms.Button btnApplyFilter;
        private System.Windows.Forms.TabControl tabToolOutput;
        private System.Windows.Forms.TabPage tabToolOutputPage;
        private System.Windows.Forms.Button button1;
    }
}

