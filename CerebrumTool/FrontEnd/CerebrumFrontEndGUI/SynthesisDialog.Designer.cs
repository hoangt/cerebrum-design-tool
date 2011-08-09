namespace CerebrumFrontEndGUI
{
    partial class SynthesisDialog
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
            this.chkCompileLinux = new System.Windows.Forms.CheckBox();
            this.chkSynthesizeHardware = new System.Windows.Forms.CheckBox();
            this.chkForceClean = new System.Windows.Forms.CheckBox();
            this.chkSelectiveClean = new System.Windows.Forms.CheckBox();
            this.chkFPGASelect = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(10, 137);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "Start";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(137, 137);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkCompileLinux
            // 
            this.chkCompileLinux.AutoSize = true;
            this.chkCompileLinux.Location = new System.Drawing.Point(10, 102);
            this.chkCompileLinux.Name = "chkCompileLinux";
            this.chkCompileLinux.Size = new System.Drawing.Size(136, 17);
            this.chkCompileLinux.TabIndex = 4;
            this.chkCompileLinux.Text = "Compile Linux Software";
            this.chkCompileLinux.UseVisualStyleBackColor = true;
            // 
            // chkSynthesizeHardware
            // 
            this.chkSynthesizeHardware.AutoSize = true;
            this.chkSynthesizeHardware.Location = new System.Drawing.Point(12, 10);
            this.chkSynthesizeHardware.Name = "chkSynthesizeHardware";
            this.chkSynthesizeHardware.Size = new System.Drawing.Size(126, 17);
            this.chkSynthesizeHardware.TabIndex = 0;
            this.chkSynthesizeHardware.Text = "Synthesize Hardware";
            this.chkSynthesizeHardware.UseVisualStyleBackColor = true;
            // 
            // chkForceClean
            // 
            this.chkForceClean.AutoSize = true;
            this.chkForceClean.Location = new System.Drawing.Point(23, 56);
            this.chkForceClean.Name = "chkForceClean";
            this.chkForceClean.Size = new System.Drawing.Size(183, 17);
            this.chkForceClean.TabIndex = 2;
            this.chkForceClean.Text = "Force clean of previous synthesis";
            this.chkForceClean.UseVisualStyleBackColor = true;
            // 
            // chkSelectiveClean
            // 
            this.chkSelectiveClean.AutoSize = true;
            this.chkSelectiveClean.Location = new System.Drawing.Point(40, 79);
            this.chkSelectiveClean.Name = "chkSelectiveClean";
            this.chkSelectiveClean.Size = new System.Drawing.Size(157, 17);
            this.chkSelectiveClean.TabIndex = 3;
            this.chkSelectiveClean.Text = "Select which cores to clean";
            this.chkSelectiveClean.UseVisualStyleBackColor = true;
            // 
            // chkFPGASelect
            // 
            this.chkFPGASelect.AutoSize = true;
            this.chkFPGASelect.Location = new System.Drawing.Point(23, 33);
            this.chkFPGASelect.Name = "chkFPGASelect";
            this.chkFPGASelect.Size = new System.Drawing.Size(187, 17);
            this.chkFPGASelect.TabIndex = 1;
            this.chkFPGASelect.Text = "Select which FPGAs to synthesize";
            this.chkFPGASelect.UseVisualStyleBackColor = true;
            // 
            // SynthesisDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(222, 167);
            this.ControlBox = false;
            this.Controls.Add(this.chkFPGASelect);
            this.Controls.Add(this.chkSelectiveClean);
            this.Controls.Add(this.chkCompileLinux);
            this.Controls.Add(this.chkSynthesizeHardware);
            this.Controls.Add(this.chkForceClean);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SynthesisDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Synthesis Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkCompileLinux;
        private System.Windows.Forms.CheckBox chkSynthesizeHardware;
        private System.Windows.Forms.CheckBox chkForceClean;
        private System.Windows.Forms.CheckBox chkSelectiveClean;
        private System.Windows.Forms.CheckBox chkFPGASelect;
    }
}