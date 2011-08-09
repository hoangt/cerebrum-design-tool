namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of processor information associated with the design.
    /// </summary>
    partial class ProcessorManagerDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessorManagerDialog));
            this.btnClose = new System.Windows.Forms.Button();
            this.grpSelected = new System.Windows.Forms.GroupBox();
            this.comboConsole = new System.Windows.Forms.ComboBox();
            this.txtCompilerArgs = new System.Windows.Forms.TextBox();
            this.lblCompilerArgs = new System.Windows.Forms.Label();
            this.lblTargetFPGA = new System.Windows.Forms.Label();
            this.txtTargetFPGA = new System.Windows.Forms.TextBox();
            this.lblInstance = new System.Windows.Forms.Label();
            this.lblOS = new System.Windows.Forms.Label();
            this.txtLinuxSource = new System.Windows.Forms.TextBox();
            this.lblType = new System.Windows.Forms.Label();
            this.txtMakeConfig = new System.Windows.Forms.TextBox();
            this.txtInstance = new System.Windows.Forms.TextBox();
            this.lblLinuxSource = new System.Windows.Forms.Label();
            this.txtOS = new System.Windows.Forms.TextBox();
            this.lblMakeConfig = new System.Windows.Forms.Label();
            this.txtType = new System.Windows.Forms.TextBox();
            this.txtDTS = new System.Windows.Forms.TextBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lblConsole = new System.Windows.Forms.Label();
            this.lblDTS = new System.Windows.Forms.Label();
            this.treeProcs = new System.Windows.Forms.TreeView();
            this.grpSelected.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(499, 325);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(118, 36);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // grpSelected
            // 
            this.grpSelected.Controls.Add(this.comboConsole);
            this.grpSelected.Controls.Add(this.txtCompilerArgs);
            this.grpSelected.Controls.Add(this.lblCompilerArgs);
            this.grpSelected.Controls.Add(this.lblTargetFPGA);
            this.grpSelected.Controls.Add(this.txtTargetFPGA);
            this.grpSelected.Controls.Add(this.lblInstance);
            this.grpSelected.Controls.Add(this.lblOS);
            this.grpSelected.Controls.Add(this.txtLinuxSource);
            this.grpSelected.Controls.Add(this.lblType);
            this.grpSelected.Controls.Add(this.txtMakeConfig);
            this.grpSelected.Controls.Add(this.txtInstance);
            this.grpSelected.Controls.Add(this.lblLinuxSource);
            this.grpSelected.Controls.Add(this.txtOS);
            this.grpSelected.Controls.Add(this.lblMakeConfig);
            this.grpSelected.Controls.Add(this.txtType);
            this.grpSelected.Controls.Add(this.txtDTS);
            this.grpSelected.Controls.Add(this.btnUpdate);
            this.grpSelected.Controls.Add(this.lblConsole);
            this.grpSelected.Controls.Add(this.lblDTS);
            this.grpSelected.Location = new System.Drawing.Point(230, 12);
            this.grpSelected.Name = "grpSelected";
            this.grpSelected.Size = new System.Drawing.Size(387, 307);
            this.grpSelected.TabIndex = 10;
            this.grpSelected.TabStop = false;
            this.grpSelected.Text = "Selected Item";
            // 
            // comboConsole
            // 
            this.comboConsole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboConsole.FormattingEnabled = true;
            this.comboConsole.Location = new System.Drawing.Point(98, 124);
            this.comboConsole.Name = "comboConsole";
            this.comboConsole.Size = new System.Drawing.Size(282, 21);
            this.comboConsole.TabIndex = 39;
            // 
            // txtCompilerArgs
            // 
            this.txtCompilerArgs.Location = new System.Drawing.Point(98, 227);
            this.txtCompilerArgs.Name = "txtCompilerArgs";
            this.txtCompilerArgs.Size = new System.Drawing.Size(282, 20);
            this.txtCompilerArgs.TabIndex = 38;
            // 
            // lblCompilerArgs
            // 
            this.lblCompilerArgs.AutoSize = true;
            this.lblCompilerArgs.Location = new System.Drawing.Point(11, 227);
            this.lblCompilerArgs.Name = "lblCompilerArgs";
            this.lblCompilerArgs.Size = new System.Drawing.Size(71, 13);
            this.lblCompilerArgs.TabIndex = 37;
            this.lblCompilerArgs.Text = "Compiler Args";
            // 
            // lblTargetFPGA
            // 
            this.lblTargetFPGA.AutoSize = true;
            this.lblTargetFPGA.Location = new System.Drawing.Point(12, 100);
            this.lblTargetFPGA.Name = "lblTargetFPGA";
            this.lblTargetFPGA.Size = new System.Drawing.Size(69, 13);
            this.lblTargetFPGA.TabIndex = 35;
            this.lblTargetFPGA.Text = "Target FPGA";
            // 
            // txtTargetFPGA
            // 
            this.txtTargetFPGA.Location = new System.Drawing.Point(99, 97);
            this.txtTargetFPGA.Name = "txtTargetFPGA";
            this.txtTargetFPGA.ReadOnly = true;
            this.txtTargetFPGA.Size = new System.Drawing.Size(282, 20);
            this.txtTargetFPGA.TabIndex = 36;
            // 
            // lblInstance
            // 
            this.lblInstance.AutoSize = true;
            this.lblInstance.Location = new System.Drawing.Point(12, 22);
            this.lblInstance.Name = "lblInstance";
            this.lblInstance.Size = new System.Drawing.Size(48, 13);
            this.lblInstance.TabIndex = 20;
            this.lblInstance.Text = "Instance";
            // 
            // lblOS
            // 
            this.lblOS.AutoSize = true;
            this.lblOS.Location = new System.Drawing.Point(12, 74);
            this.lblOS.Name = "lblOS";
            this.lblOS.Size = new System.Drawing.Size(22, 13);
            this.lblOS.TabIndex = 21;
            this.lblOS.Text = "OS";
            // 
            // txtLinuxSource
            // 
            this.txtLinuxSource.Location = new System.Drawing.Point(98, 201);
            this.txtLinuxSource.Name = "txtLinuxSource";
            this.txtLinuxSource.Size = new System.Drawing.Size(282, 20);
            this.txtLinuxSource.TabIndex = 34;
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(12, 48);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(31, 13);
            this.lblType.TabIndex = 22;
            this.lblType.Text = "Type";
            // 
            // txtMakeConfig
            // 
            this.txtMakeConfig.Location = new System.Drawing.Point(98, 175);
            this.txtMakeConfig.Name = "txtMakeConfig";
            this.txtMakeConfig.Size = new System.Drawing.Size(282, 20);
            this.txtMakeConfig.TabIndex = 33;
            // 
            // txtInstance
            // 
            this.txtInstance.Location = new System.Drawing.Point(99, 19);
            this.txtInstance.Name = "txtInstance";
            this.txtInstance.ReadOnly = true;
            this.txtInstance.Size = new System.Drawing.Size(282, 20);
            this.txtInstance.TabIndex = 23;
            // 
            // lblLinuxSource
            // 
            this.lblLinuxSource.AutoSize = true;
            this.lblLinuxSource.Location = new System.Drawing.Point(11, 201);
            this.lblLinuxSource.Name = "lblLinuxSource";
            this.lblLinuxSource.Size = new System.Drawing.Size(69, 13);
            this.lblLinuxSource.TabIndex = 32;
            this.lblLinuxSource.Text = "Linux Source";
            // 
            // txtOS
            // 
            this.txtOS.Location = new System.Drawing.Point(99, 71);
            this.txtOS.Name = "txtOS";
            this.txtOS.ReadOnly = true;
            this.txtOS.Size = new System.Drawing.Size(282, 20);
            this.txtOS.TabIndex = 24;
            // 
            // lblMakeConfig
            // 
            this.lblMakeConfig.AutoSize = true;
            this.lblMakeConfig.Location = new System.Drawing.Point(11, 175);
            this.lblMakeConfig.Name = "lblMakeConfig";
            this.lblMakeConfig.Size = new System.Drawing.Size(64, 13);
            this.lblMakeConfig.TabIndex = 31;
            this.lblMakeConfig.Text = "MakeConfig";
            // 
            // txtType
            // 
            this.txtType.Location = new System.Drawing.Point(99, 45);
            this.txtType.Name = "txtType";
            this.txtType.ReadOnly = true;
            this.txtType.Size = new System.Drawing.Size(282, 20);
            this.txtType.TabIndex = 25;
            // 
            // txtDTS
            // 
            this.txtDTS.Location = new System.Drawing.Point(99, 149);
            this.txtDTS.Name = "txtDTS";
            this.txtDTS.Size = new System.Drawing.Size(282, 20);
            this.txtDTS.TabIndex = 30;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(127, 265);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(122, 27);
            this.btnUpdate.TabIndex = 26;
            this.btnUpdate.Text = "Update Processor";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // lblConsole
            // 
            this.lblConsole.AutoSize = true;
            this.lblConsole.Location = new System.Drawing.Point(12, 123);
            this.lblConsole.Name = "lblConsole";
            this.lblConsole.Size = new System.Drawing.Size(45, 13);
            this.lblConsole.TabIndex = 27;
            this.lblConsole.Text = "Console";
            // 
            // lblDTS
            // 
            this.lblDTS.AutoSize = true;
            this.lblDTS.Location = new System.Drawing.Point(12, 149);
            this.lblDTS.Name = "lblDTS";
            this.lblDTS.Size = new System.Drawing.Size(29, 13);
            this.lblDTS.TabIndex = 28;
            this.lblDTS.Text = "DTS";
            // 
            // treeProcs
            // 
            this.treeProcs.Location = new System.Drawing.Point(12, 12);
            this.treeProcs.Name = "treeProcs";
            this.treeProcs.Size = new System.Drawing.Size(212, 346);
            this.treeProcs.TabIndex = 11;
            // 
            // ProcessorManagerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 370);
            this.ControlBox = false;
            this.Controls.Add(this.treeProcs);
            this.Controls.Add(this.grpSelected);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProcessorManagerDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cerebrum Processor Manager";
            this.grpSelected.ResumeLayout(false);
            this.grpSelected.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox grpSelected;
        private System.Windows.Forms.TreeView treeProcs;
        private System.Windows.Forms.Label lblInstance;
        private System.Windows.Forms.Label lblOS;
        private System.Windows.Forms.TextBox txtLinuxSource;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.TextBox txtMakeConfig;
        private System.Windows.Forms.TextBox txtInstance;
        private System.Windows.Forms.Label lblLinuxSource;
        private System.Windows.Forms.TextBox txtOS;
        private System.Windows.Forms.Label lblMakeConfig;
        private System.Windows.Forms.TextBox txtType;
        private System.Windows.Forms.TextBox txtDTS;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label lblConsole;
        private System.Windows.Forms.Label lblDTS;
        private System.Windows.Forms.Label lblTargetFPGA;
        private System.Windows.Forms.TextBox txtTargetFPGA;
        private System.Windows.Forms.TextBox txtCompilerArgs;
        private System.Windows.Forms.Label lblCompilerArgs;
        private System.Windows.Forms.ComboBox comboConsole;
    }
}