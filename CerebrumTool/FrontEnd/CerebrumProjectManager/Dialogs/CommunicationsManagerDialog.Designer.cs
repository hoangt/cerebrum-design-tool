namespace CerebrumProjectManager.Dialogs
{
    partial class CommunicationsManagerDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommunicationsManagerDialog));
            this.btnClose = new System.Windows.Forms.Button();
            this.grpSelected = new System.Windows.Forms.GroupBox();
            this.chkDHCP = new System.Windows.Forms.CheckBox();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.lblIPAddress = new System.Windows.Forms.Label();
            this.lblFPGA = new System.Windows.Forms.Label();
            this.txtFPGA = new System.Windows.Forms.TextBox();
            this.lblInstance = new System.Windows.Forms.Label();
            this.txtInstance = new System.Windows.Forms.TextBox();
            this.txtMAC = new System.Windows.Forms.TextBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.lblDHCPEnabled = new System.Windows.Forms.Label();
            this.lblCablePort = new System.Windows.Forms.Label();
            this.treeInterfaces = new System.Windows.Forms.TreeView();
            this.grpSelected.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(501, 235);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(118, 42);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // grpSelected
            // 
            this.grpSelected.Controls.Add(this.chkDHCP);
            this.grpSelected.Controls.Add(this.txtIP);
            this.grpSelected.Controls.Add(this.lblIPAddress);
            this.grpSelected.Controls.Add(this.lblFPGA);
            this.grpSelected.Controls.Add(this.txtFPGA);
            this.grpSelected.Controls.Add(this.lblInstance);
            this.grpSelected.Controls.Add(this.txtInstance);
            this.grpSelected.Controls.Add(this.txtMAC);
            this.grpSelected.Controls.Add(this.btnUpdate);
            this.grpSelected.Controls.Add(this.lblDHCPEnabled);
            this.grpSelected.Controls.Add(this.lblCablePort);
            this.grpSelected.Location = new System.Drawing.Point(230, 12);
            this.grpSelected.Name = "grpSelected";
            this.grpSelected.Size = new System.Drawing.Size(387, 217);
            this.grpSelected.TabIndex = 10;
            this.grpSelected.TabStop = false;
            this.grpSelected.Text = "Selected Item";
            // 
            // chkDHCP
            // 
            this.chkDHCP.AutoSize = true;
            this.chkDHCP.Location = new System.Drawing.Point(367, 74);
            this.chkDHCP.Name = "chkDHCP";
            this.chkDHCP.Size = new System.Drawing.Size(15, 14);
            this.chkDHCP.TabIndex = 36;
            this.chkDHCP.UseVisualStyleBackColor = true;
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(99, 127);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(282, 20);
            this.txtIP.TabIndex = 35;
            // 
            // lblIPAddress
            // 
            this.lblIPAddress.AutoSize = true;
            this.lblIPAddress.Location = new System.Drawing.Point(10, 126);
            this.lblIPAddress.Name = "lblIPAddress";
            this.lblIPAddress.Size = new System.Drawing.Size(58, 13);
            this.lblIPAddress.TabIndex = 34;
            this.lblIPAddress.Text = "IP Address";
            // 
            // lblFPGA
            // 
            this.lblFPGA.AutoSize = true;
            this.lblFPGA.Location = new System.Drawing.Point(11, 48);
            this.lblFPGA.Name = "lblFPGA";
            this.lblFPGA.Size = new System.Drawing.Size(35, 13);
            this.lblFPGA.TabIndex = 32;
            this.lblFPGA.Text = "FPGA";
            // 
            // txtFPGA
            // 
            this.txtFPGA.Location = new System.Drawing.Point(99, 45);
            this.txtFPGA.Name = "txtFPGA";
            this.txtFPGA.ReadOnly = true;
            this.txtFPGA.Size = new System.Drawing.Size(282, 20);
            this.txtFPGA.TabIndex = 33;
            // 
            // lblInstance
            // 
            this.lblInstance.AutoSize = true;
            this.lblInstance.Location = new System.Drawing.Point(11, 22);
            this.lblInstance.Name = "lblInstance";
            this.lblInstance.Size = new System.Drawing.Size(48, 13);
            this.lblInstance.TabIndex = 20;
            this.lblInstance.Text = "Instance";
            // 
            // txtInstance
            // 
            this.txtInstance.Location = new System.Drawing.Point(99, 19);
            this.txtInstance.Name = "txtInstance";
            this.txtInstance.ReadOnly = true;
            this.txtInstance.Size = new System.Drawing.Size(282, 20);
            this.txtInstance.TabIndex = 23;
            // 
            // txtMAC
            // 
            this.txtMAC.Location = new System.Drawing.Point(100, 101);
            this.txtMAC.Name = "txtMAC";
            this.txtMAC.Size = new System.Drawing.Size(282, 20);
            this.txtMAC.TabIndex = 30;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(100, 179);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(195, 32);
            this.btnUpdate.TabIndex = 26;
            this.btnUpdate.Text = "Update Interface";
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // lblDHCPEnabled
            // 
            this.lblDHCPEnabled.AutoSize = true;
            this.lblDHCPEnabled.Location = new System.Drawing.Point(11, 74);
            this.lblDHCPEnabled.Name = "lblDHCPEnabled";
            this.lblDHCPEnabled.Size = new System.Drawing.Size(65, 13);
            this.lblDHCPEnabled.TabIndex = 27;
            this.lblDHCPEnabled.Text = "Use DHCP?";
            // 
            // lblCablePort
            // 
            this.lblCablePort.AutoSize = true;
            this.lblCablePort.Location = new System.Drawing.Point(11, 100);
            this.lblCablePort.Name = "lblCablePort";
            this.lblCablePort.Size = new System.Drawing.Size(71, 13);
            this.lblCablePort.TabIndex = 28;
            this.lblCablePort.Text = "MAC Address";
            // 
            // treeInterfaces
            // 
            this.treeInterfaces.Location = new System.Drawing.Point(12, 12);
            this.treeInterfaces.Name = "treeInterfaces";
            this.treeInterfaces.Size = new System.Drawing.Size(212, 265);
            this.treeInterfaces.TabIndex = 11;
            // 
            // CommunicationsManagerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(631, 289);
            this.ControlBox = false;
            this.Controls.Add(this.treeInterfaces);
            this.Controls.Add(this.grpSelected);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommunicationsManagerDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cerebrum Communication Interface Manager";
            this.grpSelected.ResumeLayout(false);
            this.grpSelected.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox grpSelected;
        private System.Windows.Forms.TreeView treeInterfaces;
        private System.Windows.Forms.Label lblInstance;
        private System.Windows.Forms.TextBox txtInstance;
        private System.Windows.Forms.TextBox txtMAC;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label lblDHCPEnabled;
        private System.Windows.Forms.Label lblCablePort;
        private System.Windows.Forms.Label lblFPGA;
        private System.Windows.Forms.TextBox txtFPGA;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label lblIPAddress;
        private System.Windows.Forms.CheckBox chkDHCP;
    }
}