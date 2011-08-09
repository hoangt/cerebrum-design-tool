namespace CerebrumFrontEndGUI
{
    /// <summary>
    /// Simple form with brief details about the Cerebrum Design tool and loaded libraries.
    /// </summary>
    partial class frmHelpAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHelpAbout));
            this.groupDialogBorder = new System.Windows.Forms.GroupBox();
            this.lblLibraries = new System.Windows.Forms.Label();
            this.listLibraries = new System.Windows.Forms.ListBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblCopyright1 = new System.Windows.Forms.Label();
            this.lblCerebrumLabel = new System.Windows.Forms.Label();
            this.txtGPL = new System.Windows.Forms.TextBox();
            this.groupDialogBorder.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupDialogBorder
            // 
            this.groupDialogBorder.Controls.Add(this.txtGPL);
            this.groupDialogBorder.Controls.Add(this.lblLibraries);
            this.groupDialogBorder.Controls.Add(this.listLibraries);
            this.groupDialogBorder.Controls.Add(this.btnOK);
            this.groupDialogBorder.Controls.Add(this.lblCopyright1);
            this.groupDialogBorder.Controls.Add(this.lblCerebrumLabel);
            this.groupDialogBorder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupDialogBorder.Location = new System.Drawing.Point(0, 0);
            this.groupDialogBorder.Name = "groupDialogBorder";
            this.groupDialogBorder.Size = new System.Drawing.Size(418, 376);
            this.groupDialogBorder.TabIndex = 0;
            this.groupDialogBorder.TabStop = false;
            // 
            // lblLibraries
            // 
            this.lblLibraries.AutoSize = true;
            this.lblLibraries.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLibraries.Location = new System.Drawing.Point(9, 209);
            this.lblLibraries.Name = "lblLibraries";
            this.lblLibraries.Size = new System.Drawing.Size(115, 17);
            this.lblLibraries.TabIndex = 4;
            this.lblLibraries.Text = "Loaded Libraries";
            // 
            // listLibraries
            // 
            this.listLibraries.FormattingEnabled = true;
            this.listLibraries.Location = new System.Drawing.Point(12, 229);
            this.listLibraries.Name = "listLibraries";
            this.listLibraries.ScrollAlwaysVisible = true;
            this.listLibraries.Size = new System.Drawing.Size(391, 82);
            this.listLibraries.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(169, 328);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 36);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // lblCopyright1
            // 
            this.lblCopyright1.AutoSize = true;
            this.lblCopyright1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCopyright1.Location = new System.Drawing.Point(23, 65);
            this.lblCopyright1.Name = "lblCopyright1";
            this.lblCopyright1.Size = new System.Drawing.Size(0, 17);
            this.lblCopyright1.TabIndex = 1;
            // 
            // lblCerebrumLabel
            // 
            this.lblCerebrumLabel.AutoSize = true;
            this.lblCerebrumLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCerebrumLabel.Location = new System.Drawing.Point(12, 16);
            this.lblCerebrumLabel.Name = "lblCerebrumLabel";
            this.lblCerebrumLabel.Size = new System.Drawing.Size(369, 31);
            this.lblCerebrumLabel.TabIndex = 0;
            this.lblCerebrumLabel.Text = "Cerebrum Design Framework";
            // 
            // txtGPL
            // 
            this.txtGPL.Location = new System.Drawing.Point(12, 50);
            this.txtGPL.Multiline = true;
            this.txtGPL.Name = "txtGPL";
            this.txtGPL.ReadOnly = true;
            this.txtGPL.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGPL.Size = new System.Drawing.Size(391, 155);
            this.txtGPL.TabIndex = 5;
            this.txtGPL.Text = resources.GetString("txtGPL.Text");
            // 
            // frmHelpAbout
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(418, 376);
            this.ControlBox = false;
            this.Controls.Add(this.groupDialogBorder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmHelpAbout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Help About...";
            this.groupDialogBorder.ResumeLayout(false);
            this.groupDialogBorder.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupDialogBorder;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblCopyright1;
        private System.Windows.Forms.Label lblCerebrumLabel;
        private System.Windows.Forms.Label lblLibraries;
        private System.Windows.Forms.ListBox listLibraries;
        private System.Windows.Forms.TextBox txtGPL;
    }
}