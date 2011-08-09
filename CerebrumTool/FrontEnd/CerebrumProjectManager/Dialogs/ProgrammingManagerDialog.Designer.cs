namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of programming information associated with FPGAs and Boards.
    /// </summary>
    partial class ProgrammingManagerDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgrammingManagerDialog));
            this.btnClose = new System.Windows.Forms.Button();
            this.grpSelected = new System.Windows.Forms.GroupBox();
            this.comboCableType = new System.Windows.Forms.ComboBox();
            this.lblID = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtCablePort = new System.Windows.Forms.TextBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lblCableType = new System.Windows.Forms.Label();
            this.lblCablePort = new System.Windows.Forms.Label();
            this.treeBoards = new System.Windows.Forms.TreeView();
            this.btnProgram = new System.Windows.Forms.Button();
            this.grpSelected.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(499, 191);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(118, 42);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // grpSelected
            // 
            this.grpSelected.Controls.Add(this.comboCableType);
            this.grpSelected.Controls.Add(this.lblID);
            this.grpSelected.Controls.Add(this.txtID);
            this.grpSelected.Controls.Add(this.txtCablePort);
            this.grpSelected.Controls.Add(this.btnUpdate);
            this.grpSelected.Controls.Add(this.lblCableType);
            this.grpSelected.Controls.Add(this.lblCablePort);
            this.grpSelected.Location = new System.Drawing.Point(230, 12);
            this.grpSelected.Name = "grpSelected";
            this.grpSelected.Size = new System.Drawing.Size(387, 161);
            this.grpSelected.TabIndex = 10;
            this.grpSelected.TabStop = false;
            this.grpSelected.Text = "Selected Item";
            // 
            // comboCableType
            // 
            this.comboCableType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCableType.FormattingEnabled = true;
            this.comboCableType.Location = new System.Drawing.Point(100, 45);
            this.comboCableType.Name = "comboCableType";
            this.comboCableType.Size = new System.Drawing.Size(281, 21);
            this.comboCableType.TabIndex = 31;
            // 
            // lblID
            // 
            this.lblID.AutoSize = true;
            this.lblID.Location = new System.Drawing.Point(11, 22);
            this.lblID.Name = "lblID";
            this.lblID.Size = new System.Drawing.Size(35, 13);
            this.lblID.TabIndex = 20;
            this.lblID.Text = "Board";
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(99, 19);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(282, 20);
            this.txtID.TabIndex = 23;
            // 
            // txtCablePort
            // 
            this.txtCablePort.Location = new System.Drawing.Point(100, 72);
            this.txtCablePort.Name = "txtCablePort";
            this.txtCablePort.Size = new System.Drawing.Size(282, 20);
            this.txtCablePort.TabIndex = 30;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(100, 109);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(195, 32);
            this.btnUpdate.TabIndex = 26;
            this.btnUpdate.Text = "Update Programming Info";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // lblCableType
            // 
            this.lblCableType.AutoSize = true;
            this.lblCableType.Location = new System.Drawing.Point(11, 45);
            this.lblCableType.Name = "lblCableType";
            this.lblCableType.Size = new System.Drawing.Size(61, 13);
            this.lblCableType.TabIndex = 27;
            this.lblCableType.Text = "Cable Type";
            // 
            // lblCablePort
            // 
            this.lblCablePort.AutoSize = true;
            this.lblCablePort.Location = new System.Drawing.Point(11, 71);
            this.lblCablePort.Name = "lblCablePort";
            this.lblCablePort.Size = new System.Drawing.Size(56, 13);
            this.lblCablePort.TabIndex = 28;
            this.lblCablePort.Text = "Cable Port";
            // 
            // treeBoards
            // 
            this.treeBoards.Location = new System.Drawing.Point(12, 12);
            this.treeBoards.Name = "treeBoards";
            this.treeBoards.Size = new System.Drawing.Size(212, 237);
            this.treeBoards.TabIndex = 11;
            // 
            // btnProgram
            // 
            this.btnProgram.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnProgram.Location = new System.Drawing.Point(230, 191);
            this.btnProgram.Name = "btnProgram";
            this.btnProgram.Size = new System.Drawing.Size(118, 42);
            this.btnProgram.TabIndex = 12;
            this.btnProgram.Text = "Program Board(s)";
            this.btnProgram.UseVisualStyleBackColor = true;
            this.btnProgram.Click += new System.EventHandler(this.btnProgram_Click);
            // 
            // ProgrammingManagerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 263);
            this.ControlBox = false;
            this.Controls.Add(this.btnProgram);
            this.Controls.Add(this.treeBoards);
            this.Controls.Add(this.grpSelected);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgrammingManagerDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cerebrum Board Programming Manager";
            this.grpSelected.ResumeLayout(false);
            this.grpSelected.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox grpSelected;
        private System.Windows.Forms.TreeView treeBoards;
        private System.Windows.Forms.Label lblID;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtCablePort;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label lblCableType;
        private System.Windows.Forms.Label lblCablePort;
        private System.Windows.Forms.Button btnProgram;
        private System.Windows.Forms.ComboBox comboCableType;
    }
}