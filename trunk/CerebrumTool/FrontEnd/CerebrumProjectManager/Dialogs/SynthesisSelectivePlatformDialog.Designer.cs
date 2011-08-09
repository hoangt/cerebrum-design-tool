namespace CerebrumProjectManager.Dialogs
{
    partial class SynthesisSelectivePlatformDialog
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
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.treeFPGAs = new SmartSolutions.Controls.TriStateTreeView();
            this.SuspendLayout();
            // 
            // lblInstruction
            // 
            this.lblInstruction.Location = new System.Drawing.Point(13, 13);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(327, 49);
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.Text = "Check the box next to each FPGA that is to be synthesized.  Any unchecked FPGAs w" +
                "ill be skipped during synthesis.";
            // 
            // btnContinue
            // 
            this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnContinue.Location = new System.Drawing.Point(103, 367);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(75, 38);
            this.btnContinue.TabIndex = 2;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = true;
            // 
            // btnAbort
            // 
            this.btnAbort.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnAbort.Location = new System.Drawing.Point(265, 367);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 38);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(184, 366);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 38);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // treeFPGAs
            // 
            this.treeFPGAs.CheckBoxes = true;
            this.treeFPGAs.CheckedImageIndex = -1;
            this.treeFPGAs.IndeterminateImageIndex = -1;
            this.treeFPGAs.Location = new System.Drawing.Point(16, 65);
            this.treeFPGAs.Name = "treeFPGAs";
            this.treeFPGAs.Size = new System.Drawing.Size(324, 295);
            this.treeFPGAs.TabIndex = 5;
            this.treeFPGAs.UncheckedImageIndex = -1;
            // 
            // SynthesisSelectivePlatformDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 411);
            this.Controls.Add(this.treeFPGAs);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.lblInstruction);
            this.Name = "SynthesisSelectivePlatformDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Selectively Synthesize FPGAs";
            this.Load += new System.EventHandler(this.SynthesisSelectivePlatformDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnCancel;
        private SmartSolutions.Controls.TriStateTreeView treeFPGAs;

    }
}