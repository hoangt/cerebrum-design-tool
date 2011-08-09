namespace CerebrumFrontEndGUI
{
    partial class XPSBuilderDialog
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
            this.chkBox2 = new System.Windows.Forms.CheckBox();
            this.chkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(12, 83);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "Start";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(157, 83);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkBox2
            // 
            this.chkBox2.AutoSize = true;
            this.chkBox2.Location = new System.Drawing.Point(12, 35);
            this.chkBox2.Name = "chkBox2";
            this.chkBox2.Size = new System.Drawing.Size(188, 17);
            this.chkBox2.TabIndex = 1;
            this.chkBox2.Text = "Allow Synthesis of \'Empty\' Projects";
            this.chkBox2.UseVisualStyleBackColor = true;
            // 
            // chkBox1
            // 
            this.chkBox1.AutoSize = true;
            this.chkBox1.Location = new System.Drawing.Point(12, 12);
            this.chkBox1.Name = "chkBox1";
            this.chkBox1.Size = new System.Drawing.Size(163, 17);
            this.chkBox1.TabIndex = 0;
            this.chkBox1.Text = "Force Clean Existing Projects";
            this.chkBox1.UseVisualStyleBackColor = true;
            // 
            // XPSBuilderDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(244, 120);
            this.ControlBox = false;
            this.Controls.Add(this.chkBox2);
            this.Controls.Add(this.chkBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "XPSBuilderDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XPS Builder Options";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkBox2;
        private System.Windows.Forms.CheckBox chkBox1;
    }
}