namespace CerebrumProjectManager.Dialogs
{
    partial class SynthesisSelectivePurgeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SynthesisSelectivePurgeDialog));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.treeCores = new SmartSolutions.Controls.TriStateTreeView();
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblInstruction
            // 
            this.lblInstruction.Location = new System.Drawing.Point(13, 13);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(489, 49);
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.Text = resources.GetString("lblInstruction.Text");
            // 
            // treeCores
            // 
            this.treeCores.CheckBoxes = true;
            this.treeCores.CheckedImageIndex = -1;
            this.treeCores.IndeterminateImageIndex = -1;
            this.treeCores.Location = new System.Drawing.Point(12, 65);
            this.treeCores.Name = "treeCores";
            this.treeCores.Size = new System.Drawing.Size(490, 379);
            this.treeCores.TabIndex = 0;
            this.treeCores.UncheckedImageIndex = -1;
            // 
            // btnContinue
            // 
            this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnContinue.Location = new System.Drawing.Point(265, 451);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(75, 38);
            this.btnContinue.TabIndex = 2;
            this.btnContinue.Text = "Continue";
            this.btnContinue.UseVisualStyleBackColor = true;
            // 
            // btnAbort
            // 
            this.btnAbort.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.btnAbort.Location = new System.Drawing.Point(427, 451);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 38);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(346, 450);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 38);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // SynthesisSelectivePurgeDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 501);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnContinue);
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.treeCores);
            this.Name = "SynthesisSelectivePurgeDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Selectively Clean Synthesized Cores";
            this.Load += new System.EventHandler(this.SynthesisSelectivePurgeDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private SmartSolutions.Controls.TriStateTreeView treeCores;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.Button btnCancel;

    }
}