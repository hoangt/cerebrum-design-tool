namespace Netron.Diagramming.Win
{
    partial class PageSetupDialog
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
            this.myPageNameLabel = new System.Windows.Forms.Label();
            this.myPageColorLabel = new System.Windows.Forms.Label();
            this.myPageNameTextBox = new System.Windows.Forms.TextBox();
            this.myPageColorPicker = new ToolBox.OfficePickers.ComboBoxColorPicker();
            this.myOkButton = new System.Windows.Forms.Button();
            this.myCancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // myPageNameLabel
            // 
            this.myPageNameLabel.AutoSize = true;
            this.myPageNameLabel.Location = new System.Drawing.Point(12, 24);
            this.myPageNameLabel.Name = "myPageNameLabel";
            this.myPageNameLabel.Size = new System.Drawing.Size(38, 13);
            this.myPageNameLabel.TabIndex = 0;
            this.myPageNameLabel.Text = "Name:";
            // 
            // myPageColorLabel
            // 
            this.myPageColorLabel.AutoSize = true;
            this.myPageColorLabel.Location = new System.Drawing.Point(17, 68);
            this.myPageColorLabel.Name = "myPageColorLabel";
            this.myPageColorLabel.Size = new System.Drawing.Size(34, 13);
            this.myPageColorLabel.TabIndex = 1;
            this.myPageColorLabel.Text = "Color:";
            // 
            // myPageNameTextBox
            // 
            this.myPageNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.myPageNameTextBox.Location = new System.Drawing.Point(56, 21);
            this.myPageNameTextBox.Name = "myPageNameTextBox";
            this.myPageNameTextBox.Size = new System.Drawing.Size(277, 20);
            this.myPageNameTextBox.TabIndex = 2;
            // 
            // myPageColorPicker
            // 
            this.myPageColorPicker.Color = System.Drawing.Color.Black;
            this.myPageColorPicker.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.myPageColorPicker.DropDownHeight = 1;
            this.myPageColorPicker.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.myPageColorPicker.DropDownWidth = 1;
            this.myPageColorPicker.FormattingEnabled = true;
            this.myPageColorPicker.IntegralHeight = false;
            this.myPageColorPicker.ItemHeight = 16;
            this.myPageColorPicker.Items.AddRange(new object[] {
            "Color",
            "Color"});
            this.myPageColorPicker.Location = new System.Drawing.Point(57, 65);
            this.myPageColorPicker.Name = "myPageColorPicker";
            this.myPageColorPicker.Size = new System.Drawing.Size(90, 22);
            this.myPageColorPicker.TabIndex = 3;
            // 
            // myOkButton
            // 
            this.myOkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.myOkButton.Location = new System.Drawing.Point(171, 99);
            this.myOkButton.Name = "myOkButton";
            this.myOkButton.Size = new System.Drawing.Size(67, 25);
            this.myOkButton.TabIndex = 4;
            this.myOkButton.Text = "OK";
            this.myOkButton.UseVisualStyleBackColor = true;
            // 
            // myCancelButton
            // 
            this.myCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.myCancelButton.Location = new System.Drawing.Point(266, 99);
            this.myCancelButton.Name = "myCancelButton";
            this.myCancelButton.Size = new System.Drawing.Size(67, 25);
            this.myCancelButton.TabIndex = 5;
            this.myCancelButton.Text = "Cancel";
            this.myCancelButton.UseVisualStyleBackColor = true;
            // 
            // PageSetupDialog
            // 
            this.AcceptButton = this.myOkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 136);
            this.Controls.Add(this.myCancelButton);
            this.Controls.Add(this.myOkButton);
            this.Controls.Add(this.myPageColorPicker);
            this.Controls.Add(this.myPageNameTextBox);
            this.Controls.Add(this.myPageColorLabel);
            this.Controls.Add(this.myPageNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PageSetupDialog";
            this.Text = "Page Setup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label myPageNameLabel;
        private System.Windows.Forms.Label myPageColorLabel;
        private System.Windows.Forms.TextBox myPageNameTextBox;
        private ToolBox.OfficePickers.ComboBoxColorPicker myPageColorPicker;
        private System.Windows.Forms.Button myOkButton;
        private System.Windows.Forms.Button myCancelButton;
    }
}