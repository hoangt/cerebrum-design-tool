namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of server lists
    /// </summary>
    partial class ServerManagerDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerManagerDialog));
            this.lvAllServers = new System.Windows.Forms.ListView();
            this.lvProgramServers = new System.Windows.Forms.ListView();
            this.lvCompileServers = new System.Windows.Forms.ListView();
            this.lvSynthServers = new System.Windows.Forms.ListView();
            this.imgListServers = new System.Windows.Forms.ImageList(this.components);
            this.lblSynthServers = new System.Windows.Forms.Label();
            this.lblProgServers = new System.Windows.Forms.Label();
            this.lblCompileServers = new System.Windows.Forms.Label();
            this.lblAllServers = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpSelected = new System.Windows.Forms.GroupBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblInfoLabel1 = new System.Windows.Forms.Label();
            this.lblInfoLabel2 = new System.Windows.Forms.Label();
            this.btnAddServer = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.grpSelected.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvAllServers
            // 
            this.lvAllServers.Location = new System.Drawing.Point(12, 140);
            this.lvAllServers.Name = "lvAllServers";
            this.lvAllServers.Size = new System.Drawing.Size(235, 200);
            this.lvAllServers.TabIndex = 0;
            this.lvAllServers.UseCompatibleStateImageBehavior = false;
            // 
            // lvProgramServers
            // 
            this.lvProgramServers.Location = new System.Drawing.Point(294, 205);
            this.lvProgramServers.Name = "lvProgramServers";
            this.lvProgramServers.Size = new System.Drawing.Size(235, 135);
            this.lvProgramServers.TabIndex = 1;
            this.lvProgramServers.UseCompatibleStateImageBehavior = false;
            // 
            // lvCompileServers
            // 
            this.lvCompileServers.Location = new System.Drawing.Point(294, 374);
            this.lvCompileServers.Name = "lvCompileServers";
            this.lvCompileServers.Size = new System.Drawing.Size(235, 135);
            this.lvCompileServers.TabIndex = 3;
            this.lvCompileServers.UseCompatibleStateImageBehavior = false;
            // 
            // lvSynthServers
            // 
            this.lvSynthServers.Location = new System.Drawing.Point(295, 33);
            this.lvSynthServers.Name = "lvSynthServers";
            this.lvSynthServers.Size = new System.Drawing.Size(235, 135);
            this.lvSynthServers.TabIndex = 2;
            this.lvSynthServers.UseCompatibleStateImageBehavior = false;
            // 
            // imgListServers
            // 
            this.imgListServers.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListServers.ImageStream")));
            this.imgListServers.TransparentColor = System.Drawing.Color.Transparent;
            this.imgListServers.Images.SetKeyName(0, "server");
            // 
            // lblSynthServers
            // 
            this.lblSynthServers.AutoSize = true;
            this.lblSynthServers.Location = new System.Drawing.Point(292, 17);
            this.lblSynthServers.Name = "lblSynthServers";
            this.lblSynthServers.Size = new System.Drawing.Size(137, 13);
            this.lblSynthServers.TabIndex = 4;
            this.lblSynthServers.Text = "Available Synthesis Servers";
            // 
            // lblProgServers
            // 
            this.lblProgServers.AutoSize = true;
            this.lblProgServers.Location = new System.Drawing.Point(291, 189);
            this.lblProgServers.Name = "lblProgServers";
            this.lblProgServers.Size = new System.Drawing.Size(153, 13);
            this.lblProgServers.TabIndex = 5;
            this.lblProgServers.Text = "Available Programming Servers";
            // 
            // lblCompileServers
            // 
            this.lblCompileServers.AutoSize = true;
            this.lblCompileServers.Location = new System.Drawing.Point(291, 358);
            this.lblCompileServers.Name = "lblCompileServers";
            this.lblCompileServers.Size = new System.Drawing.Size(146, 13);
            this.lblCompileServers.TabIndex = 6;
            this.lblCompileServers.Text = "Available Compilation Servers";
            // 
            // lblAllServers
            // 
            this.lblAllServers.AutoSize = true;
            this.lblAllServers.Location = new System.Drawing.Point(12, 124);
            this.lblAllServers.Name = "lblAllServers";
            this.lblAllServers.Size = new System.Drawing.Size(103, 13);
            this.lblAllServers.TabIndex = 7;
            this.lblAllServers.Text = "All Available Servers";
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(294, 527);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 36);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "&Save Changes";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(427, 527);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(102, 36);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // grpSelected
            // 
            this.grpSelected.Controls.Add(this.btnUpdate);
            this.grpSelected.Controls.Add(this.txtPort);
            this.grpSelected.Controls.Add(this.txtUserName);
            this.grpSelected.Controls.Add(this.txtAddress);
            this.grpSelected.Controls.Add(this.lblPort);
            this.grpSelected.Controls.Add(this.lblUser);
            this.grpSelected.Controls.Add(this.lblAddress);
            this.grpSelected.Location = new System.Drawing.Point(12, 419);
            this.grpSelected.Name = "grpSelected";
            this.grpSelected.Size = new System.Drawing.Size(235, 144);
            this.grpSelected.TabIndex = 10;
            this.grpSelected.TabStop = false;
            this.grpSelected.Text = "Selected Server";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(107, 108);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(122, 27);
            this.btnUpdate.TabIndex = 11;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(76, 72);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(153, 20);
            this.txtPort.TabIndex = 5;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(76, 46);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(153, 20);
            this.txtUserName.TabIndex = 4;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(76, 20);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(153, 20);
            this.txtAddress.TabIndex = 3;
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(7, 75);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 2;
            this.lblPort.Text = "Port";
            // 
            // lblUser
            // 
            this.lblUser.AutoSize = true;
            this.lblUser.Location = new System.Drawing.Point(7, 49);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(60, 13);
            this.lblUser.TabIndex = 1;
            this.lblUser.Text = "User Name";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(7, 23);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(45, 13);
            this.lblAddress.TabIndex = 0;
            this.lblAddress.Text = "Address";
            // 
            // lblInfoLabel1
            // 
            this.lblInfoLabel1.Location = new System.Drawing.Point(12, 13);
            this.lblInfoLabel1.Name = "lblInfoLabel1";
            this.lblInfoLabel1.Size = new System.Drawing.Size(235, 51);
            this.lblInfoLabel1.TabIndex = 11;
            this.lblInfoLabel1.Text = "Manage the lists of available servers to be used for Synthesis, Programming and C" +
                "ompilation";
            // 
            // lblInfoLabel2
            // 
            this.lblInfoLabel2.Location = new System.Drawing.Point(12, 47);
            this.lblInfoLabel2.Name = "lblInfoLabel2";
            this.lblInfoLabel2.Size = new System.Drawing.Size(235, 68);
            this.lblInfoLabel2.TabIndex = 12;
            this.lblInfoLabel2.Text = resources.GetString("lblInfoLabel2.Text");
            // 
            // btnAddServer
            // 
            this.btnAddServer.Location = new System.Drawing.Point(12, 346);
            this.btnAddServer.Name = "btnAddServer";
            this.btnAddServer.Size = new System.Drawing.Size(88, 25);
            this.btnAddServer.TabIndex = 13;
            this.btnAddServer.Text = "Add";
            this.btnAddServer.UseVisualStyleBackColor = true;
            this.btnAddServer.Click += new System.EventHandler(this.btnAddServer_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(159, 346);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(88, 25);
            this.btnRemove.TabIndex = 14;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // ServerManagerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 578);
            this.ControlBox = false;
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAddServer);
            this.Controls.Add(this.lblInfoLabel2);
            this.Controls.Add(this.lblInfoLabel1);
            this.Controls.Add(this.grpSelected);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblAllServers);
            this.Controls.Add(this.lblCompileServers);
            this.Controls.Add(this.lblProgServers);
            this.Controls.Add(this.lblSynthServers);
            this.Controls.Add(this.lvCompileServers);
            this.Controls.Add(this.lvSynthServers);
            this.Controls.Add(this.lvProgramServers);
            this.Controls.Add(this.lvAllServers);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerManagerDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cerebrum Server Manager";
            this.grpSelected.ResumeLayout(false);
            this.grpSelected.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvAllServers;
        private System.Windows.Forms.ListView lvProgramServers;
        private System.Windows.Forms.ListView lvCompileServers;
        private System.Windows.Forms.ListView lvSynthServers;
        private System.Windows.Forms.ImageList imgListServers;
        private System.Windows.Forms.Label lblSynthServers;
        private System.Windows.Forms.Label lblProgServers;
        private System.Windows.Forms.Label lblCompileServers;
        private System.Windows.Forms.Label lblAllServers;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpSelected;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.Label lblInfoLabel1;
        private System.Windows.Forms.Label lblInfoLabel2;
        private System.Windows.Forms.Button btnAddServer;
        private System.Windows.Forms.Button btnRemove;
    }
}