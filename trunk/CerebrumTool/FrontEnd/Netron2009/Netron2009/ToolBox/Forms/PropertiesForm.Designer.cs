namespace ToolBox.Forms
{
    partial class PropertiesForm
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
            this.myPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // myPropertyGrid
            // 
            this.myPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.myPropertyGrid.Name = "myPropertyGrid";
            this.myPropertyGrid.Size = new System.Drawing.Size(391, 442);
            this.myPropertyGrid.TabIndex = 0;
            // 
            // PropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(391, 442);
            this.Controls.Add(this.myPropertyGrid);
            this.Name = "PropertiesForm";
            this.Text = "Properties";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid myPropertyGrid;
    }
}