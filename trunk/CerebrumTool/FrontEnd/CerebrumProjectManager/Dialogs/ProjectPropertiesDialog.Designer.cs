namespace CerebrumProjectManager.Dialogs
{
    partial class ProjectPropertiesDialog
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblSystemFrequency = new System.Windows.Forms.Label();
            this.txtSystemFrequency = new System.Windows.Forms.TextBox();
            this.tabProperties = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.lblFreqWarn = new System.Windows.Forms.Label();
            this.tabProperties.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(343, 253);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(83, 34);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(432, 253);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(85, 34);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblSystemFrequency
            // 
            this.lblSystemFrequency.AutoSize = true;
            this.lblSystemFrequency.Location = new System.Drawing.Point(7, 18);
            this.lblSystemFrequency.Name = "lblSystemFrequency";
            this.lblSystemFrequency.Size = new System.Drawing.Size(98, 13);
            this.lblSystemFrequency.TabIndex = 2;
            this.lblSystemFrequency.Text = "System Frequency*";
            // 
            // txtSystemFrequency
            // 
            this.txtSystemFrequency.Location = new System.Drawing.Point(130, 15);
            this.txtSystemFrequency.Name = "txtSystemFrequency";
            this.txtSystemFrequency.Size = new System.Drawing.Size(100, 20);
            this.txtSystemFrequency.TabIndex = 3;
            this.txtSystemFrequency.Tag = "SystemFrequency";
            // 
            // tabProperties
            // 
            this.tabProperties.Controls.Add(this.tabGeneral);
            this.tabProperties.Location = new System.Drawing.Point(6, 12);
            this.tabProperties.Name = "tabProperties";
            this.tabProperties.SelectedIndex = 0;
            this.tabProperties.Size = new System.Drawing.Size(515, 220);
            this.tabProperties.TabIndex = 4;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.lblFreqWarn);
            this.tabGeneral.Controls.Add(this.txtSystemFrequency);
            this.tabGeneral.Controls.Add(this.lblSystemFrequency);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(507, 194);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // lblFreqWarn
            // 
            this.lblFreqWarn.AutoSize = true;
            this.lblFreqWarn.Location = new System.Drawing.Point(236, 18);
            this.lblFreqWarn.Name = "lblFreqWarn";
            this.lblFreqWarn.Size = new System.Drawing.Size(202, 13);
            this.lblFreqWarn.TabIndex = 4;
            this.lblFreqWarn.Text = "* - Requires re-map and re-build of project";
            // 
            // ProjectPropertiesDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(527, 298);
            this.ControlBox = false;
            this.Controls.Add(this.tabProperties);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ProjectPropertiesDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project Properties";
            this.tabProperties.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblSystemFrequency;
        private System.Windows.Forms.TextBox txtSystemFrequency;
        private System.Windows.Forms.TabControl tabProperties;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.Label lblFreqWarn;
    }
}