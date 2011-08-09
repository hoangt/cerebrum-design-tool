namespace CerebrumProjectManager.Dialogs
{
    /// <summary>
    /// Dialog used to allow for simple editing of project path settings.
    /// </summary>
    partial class PECodeEditorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PECodeEditorDialog));
            this.toolBar = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnOpenExisting = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.sepSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnClear = new System.Windows.Forms.ToolStripButton();
            this.sepSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnReload = new System.Windows.Forms.ToolStripButton();
            this.btnClose = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.sepSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnExternalEditor = new System.Windows.Forms.ToolStripButton();
            this.toolBar.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolBar
            // 
            this.toolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpenExisting,
            this.btnSave,
            this.sepSeparator1,
            this.btnClear,
            this.sepSeparator2,
            this.btnReload,
            this.btnClose,
            this.sepSeparator3,
            this.btnExternalEditor});
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.Size = new System.Drawing.Size(701, 25);
            this.toolBar.TabIndex = 5;
            this.toolBar.Text = "toolStrip1";
            // 
            // btnNew
            // 
            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNew.Image = ((System.Drawing.Image)(resources.GetObject("btnNew.Image")));
            this.btnNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(23, 22);
            this.btnNew.Text = "New";
            this.btnNew.ToolTipText = "Create New Code File";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnOpenExisting
            // 
            this.btnOpenExisting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpenExisting.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenExisting.Image")));
            this.btnOpenExisting.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenExisting.Name = "btnOpenExisting";
            this.btnOpenExisting.Size = new System.Drawing.Size(23, 22);
            this.btnOpenExisting.Text = "Replace With Existing";
            this.btnOpenExisting.ToolTipText = "Replace With Existing Code File";
            this.btnOpenExisting.Click += new System.EventHandler(this.btnOpenExisting_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "Save Code";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // sepSeparator1
            // 
            this.sepSeparator1.Name = "sepSeparator1";
            this.sepSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnClear
            // 
            this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(23, 22);
            this.btnClear.Text = "Unassign code file";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // sepSeparator2
            // 
            this.sepSeparator2.Name = "sepSeparator2";
            this.sepSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnReload
            // 
            this.btnReload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnReload.Image = ((System.Drawing.Image)(resources.GetObject("btnReload.Image")));
            this.btnReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(23, 22);
            this.btnReload.Text = "Reload Code File";
            this.btnReload.ToolTipText = "Reload Code File from Disk";
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // btnClose
            // 
            this.btnClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(23, 22);
            this.btnClose.Text = "Close";
            this.btnClose.ToolTipText = "Close Code Editor";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status});
            this.statusStrip.Location = new System.Drawing.Point(0, 633);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(701, 22);
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "Test";
            // 
            // status
            // 
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(0, 17);
            // 
            // txtSource
            // 
            this.txtSource.AcceptsReturn = true;
            this.txtSource.AcceptsTab = true;
            this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSource.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSource.Location = new System.Drawing.Point(0, 25);
            this.txtSource.MaxLength = 2147483647;
            this.txtSource.Multiline = true;
            this.txtSource.Name = "txtSource";
            this.txtSource.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSource.Size = new System.Drawing.Size(701, 608);
            this.txtSource.TabIndex = 7;
            this.txtSource.WordWrap = false;
            this.txtSource.TextChanged += new System.EventHandler(this.txtSource_TextChanged);
            // 
            // sepSeparator3
            // 
            this.sepSeparator3.Name = "sepSeparator3";
            this.sepSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnExternalEditor
            // 
            this.btnExternalEditor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExternalEditor.Image = ((System.Drawing.Image)(resources.GetObject("btnExternalEditor.Image")));
            this.btnExternalEditor.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExternalEditor.Name = "btnExternalEditor";
            this.btnExternalEditor.Size = new System.Drawing.Size(23, 22);
            this.btnExternalEditor.Text = "Edit in External Editor";
            this.btnExternalEditor.Click += new System.EventHandler(this.btnExternalEditor_Click);
            // 
            // PECodeEditorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 655);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolBar);
            this.MinimizeBox = false;
            this.Name = "PECodeEditorDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Code Editor";
            this.toolBar.ResumeLayout(false);
            this.toolBar.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolBar;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnOpenExisting;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripButton btnReload;
        private System.Windows.Forms.ToolStripButton btnClose;
        private System.Windows.Forms.ToolStripSeparator sepSeparator1;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripSeparator sepSeparator2;
        private System.Windows.Forms.ToolStripSeparator sepSeparator3;
        private System.Windows.Forms.ToolStripButton btnExternalEditor;
    }
}