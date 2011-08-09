namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of project path settings.
    /// </summary>
    partial class ProjectPathsDialog
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
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabCerebrum = new System.Windows.Forms.TabPage();
            this.txtRemoteProgramming = new System.Windows.Forms.TextBox();
            this.lblRemoteProgramming = new System.Windows.Forms.Label();
            this.btnLocateLocalEDK = new System.Windows.Forms.Button();
            this.btnLocateCoreSearchPaths = new System.Windows.Forms.Button();
            this.txtFPGANFS = new System.Windows.Forms.TextBox();
            this.lblFPGANFS = new System.Windows.Forms.Label();
            this.txtCoreServerNFS = new System.Windows.Forms.TextBox();
            this.lblCoreServerNFS = new System.Windows.Forms.Label();
            this.txtCoreServerSource = new System.Windows.Forms.TextBox();
            this.lblCoreServerSource = new System.Windows.Forms.Label();
            this.txtMicroblazeGNUPath = new System.Windows.Forms.TextBox();
            this.lblMicroblazeGNU = new System.Windows.Forms.Label();
            this.txtEDLKPath = new System.Windows.Forms.TextBox();
            this.lblELDKPath = new System.Windows.Forms.Label();
            this.txtLinuxKernelSource = new System.Windows.Forms.TextBox();
            this.lblLinuxKernelSource = new System.Windows.Forms.Label();
            this.txtDeviceTree = new System.Windows.Forms.TextBox();
            this.lblDeviceTree = new System.Windows.Forms.Label();
            this.txtRemoteEDK = new System.Windows.Forms.TextBox();
            this.lblRemoteEDK = new System.Windows.Forms.Label();
            this.txtLocalEDK = new System.Windows.Forms.TextBox();
            this.lblLocalEDK = new System.Windows.Forms.Label();
            this.txtRemoteCores = new System.Windows.Forms.TextBox();
            this.lblRemotePCores = new System.Windows.Forms.Label();
            this.txtLocalCores = new System.Windows.Forms.TextBox();
            this.lblLocalCores = new System.Windows.Forms.Label();
            this.txtRemoteSynthesis = new System.Windows.Forms.TextBox();
            this.lblRemoteSynthesis = new System.Windows.Forms.Label();
            this.txtCerebrumCores = new System.Windows.Forms.TextBox();
            this.lblCerebrumCores = new System.Windows.Forms.Label();
            this.txtCerebrumPlatforms = new System.Windows.Forms.TextBox();
            this.lblCerebrumPlatforms = new System.Windows.Forms.Label();
            this.txtCerebrumBin = new System.Windows.Forms.TextBox();
            this.lblCerebrumBin = new System.Windows.Forms.Label();
            this.txtCerebrumRoot = new System.Windows.Forms.TextBox();
            this.lblCerebrumRoot = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTips = new System.Windows.Forms.ToolTip(this.components);
            this.txtPECompilationD = new System.Windows.Forms.TextBox();
            this.lblPECompilationDir = new System.Windows.Forms.Label();
            this.tabs.SuspendLayout();
            this.tabCerebrum.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabCerebrum);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(677, 604);
            this.tabs.TabIndex = 0;
            // 
            // tabCerebrum
            // 
            this.tabCerebrum.BackColor = System.Drawing.Color.Gainsboro;
            this.tabCerebrum.Controls.Add(this.txtPECompilationD);
            this.tabCerebrum.Controls.Add(this.lblPECompilationDir);
            this.tabCerebrum.Controls.Add(this.txtRemoteProgramming);
            this.tabCerebrum.Controls.Add(this.lblRemoteProgramming);
            this.tabCerebrum.Controls.Add(this.btnLocateLocalEDK);
            this.tabCerebrum.Controls.Add(this.btnLocateCoreSearchPaths);
            this.tabCerebrum.Controls.Add(this.txtFPGANFS);
            this.tabCerebrum.Controls.Add(this.lblFPGANFS);
            this.tabCerebrum.Controls.Add(this.txtCoreServerNFS);
            this.tabCerebrum.Controls.Add(this.lblCoreServerNFS);
            this.tabCerebrum.Controls.Add(this.txtCoreServerSource);
            this.tabCerebrum.Controls.Add(this.lblCoreServerSource);
            this.tabCerebrum.Controls.Add(this.txtMicroblazeGNUPath);
            this.tabCerebrum.Controls.Add(this.lblMicroblazeGNU);
            this.tabCerebrum.Controls.Add(this.txtEDLKPath);
            this.tabCerebrum.Controls.Add(this.lblELDKPath);
            this.tabCerebrum.Controls.Add(this.txtLinuxKernelSource);
            this.tabCerebrum.Controls.Add(this.lblLinuxKernelSource);
            this.tabCerebrum.Controls.Add(this.txtDeviceTree);
            this.tabCerebrum.Controls.Add(this.lblDeviceTree);
            this.tabCerebrum.Controls.Add(this.txtRemoteEDK);
            this.tabCerebrum.Controls.Add(this.lblRemoteEDK);
            this.tabCerebrum.Controls.Add(this.txtLocalEDK);
            this.tabCerebrum.Controls.Add(this.lblLocalEDK);
            this.tabCerebrum.Controls.Add(this.txtRemoteCores);
            this.tabCerebrum.Controls.Add(this.lblRemotePCores);
            this.tabCerebrum.Controls.Add(this.txtLocalCores);
            this.tabCerebrum.Controls.Add(this.lblLocalCores);
            this.tabCerebrum.Controls.Add(this.txtRemoteSynthesis);
            this.tabCerebrum.Controls.Add(this.lblRemoteSynthesis);
            this.tabCerebrum.Controls.Add(this.txtCerebrumCores);
            this.tabCerebrum.Controls.Add(this.lblCerebrumCores);
            this.tabCerebrum.Controls.Add(this.txtCerebrumPlatforms);
            this.tabCerebrum.Controls.Add(this.lblCerebrumPlatforms);
            this.tabCerebrum.Controls.Add(this.txtCerebrumBin);
            this.tabCerebrum.Controls.Add(this.lblCerebrumBin);
            this.tabCerebrum.Controls.Add(this.txtCerebrumRoot);
            this.tabCerebrum.Controls.Add(this.lblCerebrumRoot);
            this.tabCerebrum.Location = new System.Drawing.Point(4, 22);
            this.tabCerebrum.Name = "tabCerebrum";
            this.tabCerebrum.Padding = new System.Windows.Forms.Padding(3);
            this.tabCerebrum.Size = new System.Drawing.Size(669, 578);
            this.tabCerebrum.TabIndex = 0;
            this.tabCerebrum.Text = "Cerebrum";
            // 
            // txtRemoteProgramming
            // 
            this.txtRemoteProgramming.Location = new System.Drawing.Point(177, 222);
            this.txtRemoteProgramming.Name = "txtRemoteProgramming";
            this.txtRemoteProgramming.Size = new System.Drawing.Size(453, 20);
            this.txtRemoteProgramming.TabIndex = 43;
            this.txtRemoteProgramming.Tag = "ProgrammingPath";
            // 
            // lblRemoteProgramming
            // 
            this.lblRemoteProgramming.AutoSize = true;
            this.lblRemoteProgramming.BackColor = System.Drawing.Color.Transparent;
            this.lblRemoteProgramming.Location = new System.Drawing.Point(7, 225);
            this.lblRemoteProgramming.Name = "lblRemoteProgramming";
            this.lblRemoteProgramming.Size = new System.Drawing.Size(133, 13);
            this.lblRemoteProgramming.TabIndex = 42;
            this.lblRemoteProgramming.Tag = "ProgrammingPath";
            this.lblRemoteProgramming.Text = "Remote Programming Path";
            // 
            // btnLocateLocalEDK
            // 
            this.btnLocateLocalEDK.Location = new System.Drawing.Point(637, 264);
            this.btnLocateLocalEDK.Name = "btnLocateLocalEDK";
            this.btnLocateLocalEDK.Size = new System.Drawing.Size(26, 23);
            this.btnLocateLocalEDK.TabIndex = 41;
            this.btnLocateLocalEDK.Text = "...";
            this.btnLocateLocalEDK.UseVisualStyleBackColor = true;
            this.btnLocateLocalEDK.Click += new System.EventHandler(this.btnLocateLocalEDK_Click);
            // 
            // btnLocateCoreSearchPaths
            // 
            this.btnLocateCoreSearchPaths.Location = new System.Drawing.Point(637, 143);
            this.btnLocateCoreSearchPaths.Name = "btnLocateCoreSearchPaths";
            this.btnLocateCoreSearchPaths.Size = new System.Drawing.Size(26, 23);
            this.btnLocateCoreSearchPaths.TabIndex = 40;
            this.btnLocateCoreSearchPaths.Text = "...";
            this.btnLocateCoreSearchPaths.UseVisualStyleBackColor = true;
            this.btnLocateCoreSearchPaths.Click += new System.EventHandler(this.btnLocateCoreSearchPaths_Click);
            // 
            // txtFPGANFS
            // 
            this.txtFPGANFS.Location = new System.Drawing.Point(177, 542);
            this.txtFPGANFS.Name = "txtFPGANFS";
            this.txtFPGANFS.Size = new System.Drawing.Size(453, 20);
            this.txtFPGANFS.TabIndex = 39;
            this.txtFPGANFS.Tag = "OnBoardMount";
            // 
            // lblFPGANFS
            // 
            this.lblFPGANFS.AutoSize = true;
            this.lblFPGANFS.BackColor = System.Drawing.Color.Transparent;
            this.lblFPGANFS.Location = new System.Drawing.Point(7, 545);
            this.lblFPGANFS.Name = "lblFPGANFS";
            this.lblFPGANFS.Size = new System.Drawing.Size(136, 13);
            this.lblFPGANFS.TabIndex = 38;
            this.lblFPGANFS.Tag = "OnBoardMount";
            this.lblFPGANFS.Text = "On-FPGA NFS Mount Point";
            // 
            // txtCoreServerNFS
            // 
            this.txtCoreServerNFS.Location = new System.Drawing.Point(177, 516);
            this.txtCoreServerNFS.Name = "txtCoreServerNFS";
            this.txtCoreServerNFS.Size = new System.Drawing.Size(453, 20);
            this.txtCoreServerNFS.TabIndex = 37;
            this.txtCoreServerNFS.Tag = "LinuxBootMount";
            // 
            // lblCoreServerNFS
            // 
            this.lblCoreServerNFS.AutoSize = true;
            this.lblCoreServerNFS.BackColor = System.Drawing.Color.Transparent;
            this.lblCoreServerNFS.Location = new System.Drawing.Point(7, 519);
            this.lblCoreServerNFS.Name = "lblCoreServerNFS";
            this.lblCoreServerNFS.Size = new System.Drawing.Size(117, 13);
            this.lblCoreServerNFS.TabIndex = 36;
            this.lblCoreServerNFS.Tag = "LinuxBootMount";
            this.lblCoreServerNFS.Text = "CoreServer NFS Mount";
            // 
            // txtCoreServerSource
            // 
            this.txtCoreServerSource.Location = new System.Drawing.Point(177, 490);
            this.txtCoreServerSource.Name = "txtCoreServerSource";
            this.txtCoreServerSource.Size = new System.Drawing.Size(453, 20);
            this.txtCoreServerSource.TabIndex = 35;
            this.txtCoreServerSource.Tag = "CoreServerSource";
            // 
            // lblCoreServerSource
            // 
            this.lblCoreServerSource.AutoSize = true;
            this.lblCoreServerSource.BackColor = System.Drawing.Color.Transparent;
            this.lblCoreServerSource.Location = new System.Drawing.Point(7, 493);
            this.lblCoreServerSource.Name = "lblCoreServerSource";
            this.lblCoreServerSource.Size = new System.Drawing.Size(145, 13);
            this.lblCoreServerSource.TabIndex = 34;
            this.lblCoreServerSource.Tag = "CoreServerSource";
            this.lblCoreServerSource.Text = "Cerebrum CoreServer Source";
            // 
            // txtMicroblazeGNUPath
            // 
            this.txtMicroblazeGNUPath.Location = new System.Drawing.Point(177, 418);
            this.txtMicroblazeGNUPath.Name = "txtMicroblazeGNUPath";
            this.txtMicroblazeGNUPath.Size = new System.Drawing.Size(453, 20);
            this.txtMicroblazeGNUPath.TabIndex = 33;
            this.txtMicroblazeGNUPath.Tag = "MicroblazeGNUTools";
            // 
            // lblMicroblazeGNU
            // 
            this.lblMicroblazeGNU.AutoSize = true;
            this.lblMicroblazeGNU.BackColor = System.Drawing.Color.Transparent;
            this.lblMicroblazeGNU.Location = new System.Drawing.Point(7, 421);
            this.lblMicroblazeGNU.Name = "lblMicroblazeGNU";
            this.lblMicroblazeGNU.Size = new System.Drawing.Size(139, 13);
            this.lblMicroblazeGNU.TabIndex = 32;
            this.lblMicroblazeGNU.Tag = "MicroblazeGNUTools";
            this.lblMicroblazeGNU.Text = "Microblaze GNU Tools Path";
            // 
            // txtEDLKPath
            // 
            this.txtEDLKPath.Location = new System.Drawing.Point(177, 392);
            this.txtEDLKPath.Name = "txtEDLKPath";
            this.txtEDLKPath.Size = new System.Drawing.Size(453, 20);
            this.txtEDLKPath.TabIndex = 31;
            this.txtEDLKPath.Tag = "ELDKLocation";
            // 
            // lblELDKPath
            // 
            this.lblELDKPath.AutoSize = true;
            this.lblELDKPath.BackColor = System.Drawing.Color.Transparent;
            this.lblELDKPath.Location = new System.Drawing.Point(7, 395);
            this.lblELDKPath.Name = "lblELDKPath";
            this.lblELDKPath.Size = new System.Drawing.Size(60, 13);
            this.lblELDKPath.TabIndex = 30;
            this.lblELDKPath.Tag = "EDLKLocation";
            this.lblELDKPath.Text = "ELDK Path";
            // 
            // txtLinuxKernelSource
            // 
            this.txtLinuxKernelSource.Location = new System.Drawing.Point(177, 340);
            this.txtLinuxKernelSource.Name = "txtLinuxKernelSource";
            this.txtLinuxKernelSource.Size = new System.Drawing.Size(453, 20);
            this.txtLinuxKernelSource.TabIndex = 29;
            this.txtLinuxKernelSource.Tag = "LinuxKernelLocation";
            // 
            // lblLinuxKernelSource
            // 
            this.lblLinuxKernelSource.AutoSize = true;
            this.lblLinuxKernelSource.BackColor = System.Drawing.Color.Transparent;
            this.lblLinuxKernelSource.Location = new System.Drawing.Point(7, 343);
            this.lblLinuxKernelSource.Name = "lblLinuxKernelSource";
            this.lblLinuxKernelSource.Size = new System.Drawing.Size(170, 13);
            this.lblLinuxKernelSource.TabIndex = 28;
            this.lblLinuxKernelSource.Tag = "LinuxKernelLocation";
            this.lblLinuxKernelSource.Text = "Linux Kernel Source Path (Default)";
            // 
            // txtDeviceTree
            // 
            this.txtDeviceTree.Location = new System.Drawing.Point(177, 366);
            this.txtDeviceTree.Name = "txtDeviceTree";
            this.txtDeviceTree.Size = new System.Drawing.Size(453, 20);
            this.txtDeviceTree.TabIndex = 27;
            this.txtDeviceTree.Tag = "DeviceTreeLocation";
            // 
            // lblDeviceTree
            // 
            this.lblDeviceTree.AutoSize = true;
            this.lblDeviceTree.BackColor = System.Drawing.Color.Transparent;
            this.lblDeviceTree.Location = new System.Drawing.Point(7, 369);
            this.lblDeviceTree.Name = "lblDeviceTree";
            this.lblDeviceTree.Size = new System.Drawing.Size(91, 13);
            this.lblDeviceTree.TabIndex = 26;
            this.lblDeviceTree.Tag = "DeviceTreeLocation";
            this.lblDeviceTree.Text = "Device Tree Path";
            // 
            // txtRemoteEDK
            // 
            this.txtRemoteEDK.Location = new System.Drawing.Point(177, 292);
            this.txtRemoteEDK.Name = "txtRemoteEDK";
            this.txtRemoteEDK.Size = new System.Drawing.Size(453, 20);
            this.txtRemoteEDK.TabIndex = 25;
            this.txtRemoteEDK.Tag = "XilinxEDKDirectory";
            // 
            // lblRemoteEDK
            // 
            this.lblRemoteEDK.AutoSize = true;
            this.lblRemoteEDK.BackColor = System.Drawing.Color.Transparent;
            this.lblRemoteEDK.Location = new System.Drawing.Point(7, 295);
            this.lblRemoteEDK.Name = "lblRemoteEDK";
            this.lblRemoteEDK.Size = new System.Drawing.Size(141, 13);
            this.lblRemoteEDK.TabIndex = 24;
            this.lblRemoteEDK.Tag = "XilinxEDKDirectory";
            this.lblRemoteEDK.Text = "Remote Xilinx EDK Directory";
            // 
            // txtLocalEDK
            // 
            this.txtLocalEDK.Location = new System.Drawing.Point(177, 266);
            this.txtLocalEDK.Name = "txtLocalEDK";
            this.txtLocalEDK.Size = new System.Drawing.Size(453, 20);
            this.txtLocalEDK.TabIndex = 23;
            this.txtLocalEDK.Tag = "LocalXilinxEDKDirectory";
            // 
            // lblLocalEDK
            // 
            this.lblLocalEDK.AutoSize = true;
            this.lblLocalEDK.BackColor = System.Drawing.Color.Transparent;
            this.lblLocalEDK.Location = new System.Drawing.Point(7, 269);
            this.lblLocalEDK.Name = "lblLocalEDK";
            this.lblLocalEDK.Size = new System.Drawing.Size(130, 13);
            this.lblLocalEDK.TabIndex = 22;
            this.lblLocalEDK.Tag = "LocalXilinxEDKDirectory";
            this.lblLocalEDK.Text = "Local Xilinx EDK Directory";
            // 
            // txtRemoteCores
            // 
            this.txtRemoteCores.Location = new System.Drawing.Point(177, 196);
            this.txtRemoteCores.Name = "txtRemoteCores";
            this.txtRemoteCores.Size = new System.Drawing.Size(453, 20);
            this.txtRemoteCores.TabIndex = 21;
            this.txtRemoteCores.Tag = "GlobalSynthPCores";
            // 
            // lblRemotePCores
            // 
            this.lblRemotePCores.AutoSize = true;
            this.lblRemotePCores.BackColor = System.Drawing.Color.Transparent;
            this.lblRemotePCores.Location = new System.Drawing.Point(7, 199);
            this.lblRemotePCores.Name = "lblRemotePCores";
            this.lblRemotePCores.Size = new System.Drawing.Size(136, 13);
            this.lblRemotePCores.TabIndex = 20;
            this.lblRemotePCores.Tag = "GlobalSynthPCores";
            this.lblRemotePCores.Text = "Remote Core Search Paths";
            // 
            // txtLocalCores
            // 
            this.txtLocalCores.Location = new System.Drawing.Point(177, 145);
            this.txtLocalCores.Name = "txtLocalCores";
            this.txtLocalCores.Size = new System.Drawing.Size(453, 20);
            this.txtLocalCores.TabIndex = 19;
            this.txtLocalCores.Tag = "CoreSearchPaths";
            // 
            // lblLocalCores
            // 
            this.lblLocalCores.AutoSize = true;
            this.lblLocalCores.BackColor = System.Drawing.Color.Transparent;
            this.lblLocalCores.Location = new System.Drawing.Point(7, 148);
            this.lblLocalCores.Name = "lblLocalCores";
            this.lblLocalCores.Size = new System.Drawing.Size(125, 13);
            this.lblLocalCores.TabIndex = 18;
            this.lblLocalCores.Tag = "CoreSearchPaths";
            this.lblLocalCores.Text = "Local Core Search Paths";
            // 
            // txtRemoteSynthesis
            // 
            this.txtRemoteSynthesis.Location = new System.Drawing.Point(177, 171);
            this.txtRemoteSynthesis.Name = "txtRemoteSynthesis";
            this.txtRemoteSynthesis.Size = new System.Drawing.Size(453, 20);
            this.txtRemoteSynthesis.TabIndex = 17;
            this.txtRemoteSynthesis.Tag = "RemoteProject";
            // 
            // lblRemoteSynthesis
            // 
            this.lblRemoteSynthesis.AutoSize = true;
            this.lblRemoteSynthesis.BackColor = System.Drawing.Color.Transparent;
            this.lblRemoteSynthesis.Location = new System.Drawing.Point(7, 174);
            this.lblRemoteSynthesis.Name = "lblRemoteSynthesis";
            this.lblRemoteSynthesis.Size = new System.Drawing.Size(117, 13);
            this.lblRemoteSynthesis.TabIndex = 16;
            this.lblRemoteSynthesis.Tag = "RemoteProject";
            this.lblRemoteSynthesis.Text = "Remote Synthesis Path";
            // 
            // txtCerebrumCores
            // 
            this.txtCerebrumCores.Enabled = false;
            this.txtCerebrumCores.Location = new System.Drawing.Point(177, 100);
            this.txtCerebrumCores.Name = "txtCerebrumCores";
            this.txtCerebrumCores.ReadOnly = true;
            this.txtCerebrumCores.Size = new System.Drawing.Size(453, 20);
            this.txtCerebrumCores.TabIndex = 7;
            this.txtCerebrumCores.Tag = "CerebrumCores";
            this.txtCerebrumCores.Text = "This path is fixed at Install-Time of the Cerebrum Tool.";
            // 
            // lblCerebrumCores
            // 
            this.lblCerebrumCores.AutoSize = true;
            this.lblCerebrumCores.BackColor = System.Drawing.Color.Transparent;
            this.lblCerebrumCores.Location = new System.Drawing.Point(7, 103);
            this.lblCerebrumCores.Name = "lblCerebrumCores";
            this.lblCerebrumCores.Size = new System.Drawing.Size(127, 13);
            this.lblCerebrumCores.TabIndex = 6;
            this.lblCerebrumCores.Tag = "CerebrumCores";
            this.lblCerebrumCores.Text = "Cerebrum Cores Directory";
            // 
            // txtCerebrumPlatforms
            // 
            this.txtCerebrumPlatforms.Enabled = false;
            this.txtCerebrumPlatforms.Location = new System.Drawing.Point(177, 74);
            this.txtCerebrumPlatforms.Name = "txtCerebrumPlatforms";
            this.txtCerebrumPlatforms.ReadOnly = true;
            this.txtCerebrumPlatforms.Size = new System.Drawing.Size(453, 20);
            this.txtCerebrumPlatforms.TabIndex = 5;
            this.txtCerebrumPlatforms.Tag = "Platforms";
            this.txtCerebrumPlatforms.Text = "This path is fixed at Install-Time of the Cerebrum Tool.";
            // 
            // lblCerebrumPlatforms
            // 
            this.lblCerebrumPlatforms.AutoSize = true;
            this.lblCerebrumPlatforms.BackColor = System.Drawing.Color.Transparent;
            this.lblCerebrumPlatforms.Location = new System.Drawing.Point(7, 77);
            this.lblCerebrumPlatforms.Name = "lblCerebrumPlatforms";
            this.lblCerebrumPlatforms.Size = new System.Drawing.Size(143, 13);
            this.lblCerebrumPlatforms.TabIndex = 4;
            this.lblCerebrumPlatforms.Tag = "Platforms";
            this.lblCerebrumPlatforms.Text = "Cerebrum Platforms Directory";
            // 
            // txtCerebrumBin
            // 
            this.txtCerebrumBin.Enabled = false;
            this.txtCerebrumBin.Location = new System.Drawing.Point(177, 48);
            this.txtCerebrumBin.Name = "txtCerebrumBin";
            this.txtCerebrumBin.ReadOnly = true;
            this.txtCerebrumBin.Size = new System.Drawing.Size(453, 20);
            this.txtCerebrumBin.TabIndex = 3;
            this.txtCerebrumBin.Tag = "BinDirectory";
            this.txtCerebrumBin.Text = "This path is fixed at Install-Time of the Cerebrum Tool.";
            // 
            // lblCerebrumBin
            // 
            this.lblCerebrumBin.AutoSize = true;
            this.lblCerebrumBin.BackColor = System.Drawing.Color.Transparent;
            this.lblCerebrumBin.Location = new System.Drawing.Point(7, 51);
            this.lblCerebrumBin.Name = "lblCerebrumBin";
            this.lblCerebrumBin.Size = new System.Drawing.Size(115, 13);
            this.lblCerebrumBin.TabIndex = 2;
            this.lblCerebrumBin.Tag = "BinDirectory";
            this.lblCerebrumBin.Text = "Cerebrum Bin Directory";
            // 
            // txtCerebrumRoot
            // 
            this.txtCerebrumRoot.Enabled = false;
            this.txtCerebrumRoot.Location = new System.Drawing.Point(177, 22);
            this.txtCerebrumRoot.Name = "txtCerebrumRoot";
            this.txtCerebrumRoot.ReadOnly = true;
            this.txtCerebrumRoot.Size = new System.Drawing.Size(453, 20);
            this.txtCerebrumRoot.TabIndex = 1;
            this.txtCerebrumRoot.Tag = "CerebrumRoot";
            this.txtCerebrumRoot.Text = "This path is fixed at Install-Time of the Cerebrum Tool.";
            // 
            // lblCerebrumRoot
            // 
            this.lblCerebrumRoot.AutoSize = true;
            this.lblCerebrumRoot.BackColor = System.Drawing.Color.Transparent;
            this.lblCerebrumRoot.Location = new System.Drawing.Point(7, 25);
            this.lblCerebrumRoot.Name = "lblCerebrumRoot";
            this.lblCerebrumRoot.Size = new System.Drawing.Size(126, 13);
            this.lblCerebrumRoot.TabIndex = 0;
            this.lblCerebrumRoot.Tag = "CerebrumRoot";
            this.lblCerebrumRoot.Text = "Cerebrum Install Location";
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(528, 622);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 36);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(609, 622);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 36);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // folderBrowser
            // 
            this.folderBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;
            this.folderBrowser.ShowNewFolderButton = false;
            // 
            // txtPECompilationD
            // 
            this.txtPECompilationD.Location = new System.Drawing.Point(177, 444);
            this.txtPECompilationD.Name = "txtPECompilationD";
            this.txtPECompilationD.Size = new System.Drawing.Size(453, 20);
            this.txtPECompilationD.TabIndex = 45;
            this.txtPECompilationD.Tag = "PECompileDir";
            // 
            // lblPECompilationDir
            // 
            this.lblPECompilationDir.AutoSize = true;
            this.lblPECompilationDir.BackColor = System.Drawing.Color.Transparent;
            this.lblPECompilationDir.Location = new System.Drawing.Point(7, 447);
            this.lblPECompilationDir.Name = "lblPECompilationDir";
            this.lblPECompilationDir.Size = new System.Drawing.Size(123, 13);
            this.lblPECompilationDir.TabIndex = 44;
            this.lblPECompilationDir.Tag = "MicroblazeGNUTools";
            this.lblPECompilationDir.Text = "PE Compilation Directory";
            // 
            // ProjectPathsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 669);
            this.ControlBox = false;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tabs);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProjectPathsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Project Paths";
            this.tabs.ResumeLayout(false);
            this.tabCerebrum.ResumeLayout(false);
            this.tabCerebrum.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabCerebrum;
        private System.Windows.Forms.TextBox txtCerebrumRoot;
        private System.Windows.Forms.Label lblCerebrumRoot;
        private System.Windows.Forms.TextBox txtCerebrumCores;
        private System.Windows.Forms.Label lblCerebrumCores;
        private System.Windows.Forms.TextBox txtCerebrumPlatforms;
        private System.Windows.Forms.Label lblCerebrumPlatforms;
        private System.Windows.Forms.TextBox txtCerebrumBin;
        private System.Windows.Forms.Label lblCerebrumBin;
        private System.Windows.Forms.TextBox txtCoreServerSource;
        private System.Windows.Forms.Label lblCoreServerSource;
        private System.Windows.Forms.TextBox txtMicroblazeGNUPath;
        private System.Windows.Forms.Label lblMicroblazeGNU;
        private System.Windows.Forms.TextBox txtEDLKPath;
        private System.Windows.Forms.Label lblELDKPath;
        private System.Windows.Forms.TextBox txtLinuxKernelSource;
        private System.Windows.Forms.Label lblLinuxKernelSource;
        private System.Windows.Forms.TextBox txtDeviceTree;
        private System.Windows.Forms.Label lblDeviceTree;
        private System.Windows.Forms.TextBox txtRemoteEDK;
        private System.Windows.Forms.Label lblRemoteEDK;
        private System.Windows.Forms.TextBox txtLocalEDK;
        private System.Windows.Forms.Label lblLocalEDK;
        private System.Windows.Forms.TextBox txtRemoteCores;
        private System.Windows.Forms.Label lblRemotePCores;
        private System.Windows.Forms.TextBox txtLocalCores;
        private System.Windows.Forms.Label lblLocalCores;
        private System.Windows.Forms.TextBox txtRemoteSynthesis;
        private System.Windows.Forms.Label lblRemoteSynthesis;
        private System.Windows.Forms.TextBox txtFPGANFS;
        private System.Windows.Forms.Label lblFPGANFS;
        private System.Windows.Forms.TextBox txtCoreServerNFS;
        private System.Windows.Forms.Label lblCoreServerNFS;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnLocateLocalEDK;
        private System.Windows.Forms.Button btnLocateCoreSearchPaths;
        private System.Windows.Forms.FolderBrowserDialog folderBrowser;
        private System.Windows.Forms.ToolTip toolTips;
        private System.Windows.Forms.TextBox txtRemoteProgramming;
        private System.Windows.Forms.Label lblRemoteProgramming;
        private System.Windows.Forms.TextBox txtPECompilationD;
        private System.Windows.Forms.Label lblPECompilationDir;
    }
}