namespace CerebrumProjectManager.Wizards
{
    /// <summary>
    /// Defines a form implementation of the New Project Wizard
    /// </summary>
    partial class frmNewProjectWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNewProjectWizard));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.grpPFSummary = new System.Windows.Forms.GroupBox();
            this.txtPFSummary = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comboAvailablePF = new System.Windows.Forms.ComboBox();
            this.lblAvailablePFLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panelProgress = new System.Windows.Forms.Panel();
            this.step4 = new System.Windows.Forms.CheckBox();
            this.step3 = new System.Windows.Forms.CheckBox();
            this.step2 = new System.Windows.Forms.CheckBox();
            this.step1 = new System.Windows.Forms.CheckBox();
            this.step0 = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnMvDown = new System.Windows.Forms.Button();
            this.btnMvUp = new System.Windows.Forms.Button();
            this.btnRemoveServer = new System.Windows.Forms.Button();
            this.btnAddServer = new System.Windows.Forms.Button();
            this.lvServers = new CerebrumSharedClasses.ListViewEx();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnClearPaths = new System.Windows.Forms.Button();
            this.btnLoadPaths = new System.Windows.Forms.Button();
            this.btnOpenPathsDialog = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.loadPathDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveProjectFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnSaveLocation = new System.Windows.Forms.Button();
            this.txtSaveLocation = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.grpPFSummary.SuspendLayout();
            this.panelProgress.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.lblWelcome);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 380);
            this.panel1.TabIndex = 0;
            this.panel1.Tag = "0";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(205, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(382, 31);
            this.label6.TabIndex = 1;
            this.label6.Text = "New Project Wizard";
            // 
            // lblWelcome
            // 
            this.lblWelcome.BackColor = System.Drawing.Color.Transparent;
            this.lblWelcome.Location = new System.Drawing.Point(205, 54);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(385, 314);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = resources.GetString("lblWelcome.Text");
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(515, 386);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 36);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(337, 386);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 36);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "<- &Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNext.Location = new System.Drawing.Point(418, 386);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 36);
            this.btnNext.TabIndex = 0;
            this.btnNext.Text = "&Next ->";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.grpPFSummary);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.comboAvailablePF);
            this.panel2.Controls.Add(this.lblAvailablePFLabel);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Location = new System.Drawing.Point(606, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(600, 380);
            this.panel2.TabIndex = 7;
            this.panel2.Tag = "1";
            // 
            // grpPFSummary
            // 
            this.grpPFSummary.Controls.Add(this.txtPFSummary);
            this.grpPFSummary.Location = new System.Drawing.Point(210, 160);
            this.grpPFSummary.Name = "grpPFSummary";
            this.grpPFSummary.Size = new System.Drawing.Size(380, 208);
            this.grpPFSummary.TabIndex = 8;
            this.grpPFSummary.TabStop = false;
            this.grpPFSummary.Text = "Selected Platform Summary";
            // 
            // txtPFSummary
            // 
            this.txtPFSummary.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPFSummary.Location = new System.Drawing.Point(7, 20);
            this.txtPFSummary.Multiline = true;
            this.txtPFSummary.Name = "txtPFSummary";
            this.txtPFSummary.ReadOnly = true;
            this.txtPFSummary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPFSummary.Size = new System.Drawing.Size(367, 182);
            this.txtPFSummary.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(205, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(379, 31);
            this.label7.TabIndex = 6;
            this.label7.Text = "Selecting the Hardware Platform";
            // 
            // comboAvailablePF
            // 
            this.comboAvailablePF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAvailablePF.FormattingEnabled = true;
            this.comboAvailablePF.Location = new System.Drawing.Point(335, 111);
            this.comboAvailablePF.Name = "comboAvailablePF";
            this.comboAvailablePF.Size = new System.Drawing.Size(255, 21);
            this.comboAvailablePF.TabIndex = 3;
            // 
            // lblAvailablePFLabel
            // 
            this.lblAvailablePFLabel.Location = new System.Drawing.Point(207, 114);
            this.lblAvailablePFLabel.Name = "lblAvailablePFLabel";
            this.lblAvailablePFLabel.Size = new System.Drawing.Size(102, 19);
            this.lblAvailablePFLabel.TabIndex = 2;
            this.lblAvailablePFLabel.Text = "Available Platforms:";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(205, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(385, 61);
            this.label5.TabIndex = 1;
            this.label5.Text = "First select a hardware platform for the system from the list of available platfo" +
                "rms below.  Selecting a platform from the list will display some information abo" +
                "ut it in the panel below.";
            // 
            // panelProgress
            // 
            this.panelProgress.Controls.Add(this.step4);
            this.panelProgress.Controls.Add(this.step3);
            this.panelProgress.Controls.Add(this.step2);
            this.panelProgress.Controls.Add(this.step1);
            this.panelProgress.Controls.Add(this.step0);
            this.panelProgress.Location = new System.Drawing.Point(57, 396);
            this.panelProgress.Name = "panelProgress";
            this.panelProgress.Size = new System.Drawing.Size(203, 355);
            this.panelProgress.TabIndex = 12;
            this.panelProgress.Tag = "global";
            // 
            // step4
            // 
            this.step4.AutoCheck = false;
            this.step4.AutoSize = true;
            this.step4.Location = new System.Drawing.Point(12, 122);
            this.step4.Name = "step4";
            this.step4.Size = new System.Drawing.Size(134, 17);
            this.step4.TabIndex = 4;
            this.step4.TabStop = false;
            this.step4.Text = "Confirm && Save Project";
            this.step4.UseVisualStyleBackColor = true;
            // 
            // step3
            // 
            this.step3.AutoCheck = false;
            this.step3.AutoSize = true;
            this.step3.Location = new System.Drawing.Point(12, 99);
            this.step3.Name = "step3";
            this.step3.Size = new System.Drawing.Size(145, 17);
            this.step3.TabIndex = 3;
            this.step3.TabStop = false;
            this.step3.Text = "Configuring Project Paths";
            this.step3.UseVisualStyleBackColor = true;
            // 
            // step2
            // 
            this.step2.AutoCheck = false;
            this.step2.AutoSize = true;
            this.step2.Location = new System.Drawing.Point(12, 76);
            this.step2.Name = "step2";
            this.step2.Size = new System.Drawing.Size(110, 17);
            this.step2.TabIndex = 2;
            this.step2.TabStop = false;
            this.step2.Text = "Configure Servers";
            this.step2.UseVisualStyleBackColor = true;
            // 
            // step1
            // 
            this.step1.AutoCheck = false;
            this.step1.AutoSize = true;
            this.step1.Location = new System.Drawing.Point(12, 53);
            this.step1.Name = "step1";
            this.step1.Size = new System.Drawing.Size(164, 17);
            this.step1.TabIndex = 1;
            this.step1.TabStop = false;
            this.step1.Text = "Select the Hardware Platform";
            this.step1.UseVisualStyleBackColor = true;
            // 
            // step0
            // 
            this.step0.AutoCheck = false;
            this.step0.AutoSize = true;
            this.step0.Location = new System.Drawing.Point(12, 30);
            this.step0.Name = "step0";
            this.step0.Size = new System.Drawing.Size(48, 17);
            this.step0.TabIndex = 0;
            this.step0.TabStop = false;
            this.step0.Text = "Start";
            this.step0.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnMvDown);
            this.panel3.Controls.Add(this.btnMvUp);
            this.panel3.Controls.Add(this.btnRemoveServer);
            this.panel3.Controls.Add(this.btnAddServer);
            this.panel3.Controls.Add(this.lvServers);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Location = new System.Drawing.Point(12, 454);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(600, 380);
            this.panel3.TabIndex = 13;
            this.panel3.Tag = "2";
            // 
            // btnMvDown
            // 
            this.btnMvDown.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnMvDown.Location = new System.Drawing.Point(552, 315);
            this.btnMvDown.Name = "btnMvDown";
            this.btnMvDown.Size = new System.Drawing.Size(36, 36);
            this.btnMvDown.TabIndex = 8;
            this.btnMvDown.Text = "Dn";
            this.btnMvDown.UseVisualStyleBackColor = true;
            this.btnMvDown.Visible = false;
            // 
            // btnMvUp
            // 
            this.btnMvUp.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnMvUp.Location = new System.Drawing.Point(510, 315);
            this.btnMvUp.Name = "btnMvUp";
            this.btnMvUp.Size = new System.Drawing.Size(36, 36);
            this.btnMvUp.TabIndex = 7;
            this.btnMvUp.Text = "Up";
            this.btnMvUp.UseVisualStyleBackColor = true;
            this.btnMvUp.Visible = false;
            // 
            // btnRemoveServer
            // 
            this.btnRemoveServer.Location = new System.Drawing.Point(309, 315);
            this.btnRemoveServer.Name = "btnRemoveServer";
            this.btnRemoveServer.Size = new System.Drawing.Size(96, 36);
            this.btnRemoveServer.TabIndex = 5;
            this.btnRemoveServer.Text = "Remove";
            this.btnRemoveServer.UseVisualStyleBackColor = true;
            this.btnRemoveServer.Click += new System.EventHandler(this.btnRemoveServer_Click);
            // 
            // btnAddServer
            // 
            this.btnAddServer.Location = new System.Drawing.Point(205, 315);
            this.btnAddServer.Name = "btnAddServer";
            this.btnAddServer.Size = new System.Drawing.Size(98, 36);
            this.btnAddServer.TabIndex = 4;
            this.btnAddServer.Text = "Add Server";
            this.btnAddServer.UseVisualStyleBackColor = true;
            this.btnAddServer.Click += new System.EventHandler(this.btnAddServer_Click);
            // 
            // lvServers
            // 
            this.lvServers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.lvServers.FullRowSelect = true;
            this.lvServers.Location = new System.Drawing.Point(205, 95);
            this.lvServers.Name = "lvServers";
            this.lvServers.Size = new System.Drawing.Size(383, 202);
            this.lvServers.TabIndex = 3;
            this.lvServers.UseCompatibleStateImageBehavior = false;
            this.lvServers.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            this.columnHeader1.Width = 28;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Host";
            this.columnHeader2.Width = 92;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "User";
            this.columnHeader3.Width = 64;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Synthesis?";
            this.columnHeader4.Width = 64;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Program?";
            this.columnHeader5.Width = 59;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Compile?";
            this.columnHeader6.Width = 56;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(205, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(383, 61);
            this.label1.TabIndex = 2;
            this.label1.Text = "Next, you must configure the list(s) of servers that Cerebrum will use to perform" +
                " hardware synthesis, software compilation, and board/FPGA programming.";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label2.Location = new System.Drawing.Point(205, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(383, 31);
            this.label2.TabIndex = 0;
            this.label2.Text = "Configure Servers";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btnClearPaths);
            this.panel5.Controls.Add(this.btnLoadPaths);
            this.panel5.Controls.Add(this.btnOpenPathsDialog);
            this.panel5.Controls.Add(this.label8);
            this.panel5.Controls.Add(this.label3);
            this.panel5.Location = new System.Drawing.Point(859, 485);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(600, 380);
            this.panel5.TabIndex = 14;
            this.panel5.Tag = "3";
            // 
            // btnClearPaths
            // 
            this.btnClearPaths.Location = new System.Drawing.Point(296, 208);
            this.btnClearPaths.Name = "btnClearPaths";
            this.btnClearPaths.Size = new System.Drawing.Size(178, 39);
            this.btnClearPaths.TabIndex = 4;
            this.btnClearPaths.Text = "Clear Current Paths";
            this.btnClearPaths.UseVisualStyleBackColor = true;
            this.btnClearPaths.Click += new System.EventHandler(this.btnClearPaths_Click);
            // 
            // btnLoadPaths
            // 
            this.btnLoadPaths.Location = new System.Drawing.Point(296, 163);
            this.btnLoadPaths.Name = "btnLoadPaths";
            this.btnLoadPaths.Size = new System.Drawing.Size(178, 39);
            this.btnLoadPaths.TabIndex = 3;
            this.btnLoadPaths.Text = "Load From an Existing File...";
            this.btnLoadPaths.UseVisualStyleBackColor = true;
            this.btnLoadPaths.Click += new System.EventHandler(this.btnLoadPaths_Click);
            // 
            // btnOpenPathsDialog
            // 
            this.btnOpenPathsDialog.Location = new System.Drawing.Point(296, 253);
            this.btnOpenPathsDialog.Name = "btnOpenPathsDialog";
            this.btnOpenPathsDialog.Size = new System.Drawing.Size(178, 38);
            this.btnOpenPathsDialog.TabIndex = 2;
            this.btnOpenPathsDialog.Text = "Configure Project Paths...";
            this.btnOpenPathsDialog.UseVisualStyleBackColor = true;
            this.btnOpenPathsDialog.Click += new System.EventHandler(this.btnOpenPathsDialog_Click);
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(207, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(385, 58);
            this.label8.TabIndex = 1;
            this.label8.Text = resources.GetString("label8.Text");
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label3.Location = new System.Drawing.Point(205, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(364, 34);
            this.label3.TabIndex = 0;
            this.label3.Text = "Configure Required Project Paths";
            // 
            // loadPathDialog
            // 
            this.loadPathDialog.DefaultExt = "xml";
            this.loadPathDialog.FileName = "paths.xml";
            this.loadPathDialog.Filter = "Cerebrum Project Paths Files|paths.xml";
            this.loadPathDialog.Title = "Import from Existing Cerebrum Project Paths File";
            // 
            // saveProjectFolderDialog
            // 
            this.saveProjectFolderDialog.Description = "Select Location for New Project...";
            this.saveProjectFolderDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnSaveLocation);
            this.panel4.Controls.Add(this.txtSaveLocation);
            this.panel4.Controls.Add(this.label9);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Location = new System.Drawing.Point(828, 400);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(600, 380);
            this.panel4.TabIndex = 16;
            this.panel4.Tag = "4";
            // 
            // btnSaveLocation
            // 
            this.btnSaveLocation.Location = new System.Drawing.Point(564, 112);
            this.btnSaveLocation.Name = "btnSaveLocation";
            this.btnSaveLocation.Size = new System.Drawing.Size(28, 23);
            this.btnSaveLocation.TabIndex = 3;
            this.btnSaveLocation.Text = "...";
            this.btnSaveLocation.UseVisualStyleBackColor = true;
            this.btnSaveLocation.Click += new System.EventHandler(this.btnSaveLocation_Click);
            // 
            // txtSaveLocation
            // 
            this.txtSaveLocation.Location = new System.Drawing.Point(210, 112);
            this.txtSaveLocation.Name = "txtSaveLocation";
            this.txtSaveLocation.Size = new System.Drawing.Size(348, 20);
            this.txtSaveLocation.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(207, 54);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(385, 51);
            this.label9.TabIndex = 1;
            this.label9.Text = "Select the Location where you want to save your new Cerebrum Project.  All requir" +
                "ed project files will be created and saved in the folder specified below.";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label4.Location = new System.Drawing.Point(205, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(364, 32);
            this.label4.TabIndex = 0;
            this.label4.Text = "Confirm && Save Project";
            // 
            // frmNewProjectWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(600, 427);
            this.ControlBox = false;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panelProgress);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNewProjectWizard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Cerebrum Project Wizard";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.grpPFSummary.ResumeLayout(false);
            this.grpPFSummary.PerformLayout();
            this.panelProgress.ResumeLayout(false);
            this.panelProgress.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox comboAvailablePF;
        private System.Windows.Forms.Label lblAvailablePFLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grpPFSummary;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtPFSummary;
        private System.Windows.Forms.Panel panelProgress;
        private System.Windows.Forms.CheckBox step4;
        private System.Windows.Forms.CheckBox step3;
        private System.Windows.Forms.CheckBox step2;
        private System.Windows.Forms.CheckBox step1;
        private System.Windows.Forms.CheckBox step0;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnRemoveServer;
        private System.Windows.Forms.Button btnAddServer;
        private CerebrumSharedClasses.ListViewEx lvServers;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnMvDown;
        private System.Windows.Forms.Button btnMvUp;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnOpenPathsDialog;
        private System.Windows.Forms.Button btnLoadPaths;
        private System.Windows.Forms.OpenFileDialog loadPathDialog;
        private System.Windows.Forms.FolderBrowserDialog saveProjectFolderDialog;
        private System.Windows.Forms.Button btnClearPaths;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnSaveLocation;
        private System.Windows.Forms.TextBox txtSaveLocation;
    }
}