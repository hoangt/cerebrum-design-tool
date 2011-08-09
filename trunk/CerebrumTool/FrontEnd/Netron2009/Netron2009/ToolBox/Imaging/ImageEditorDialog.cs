using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using XPExplorerBar;

namespace ToolBox.Imaging
{
	// ----------------------------------------------------------------------
	/// <summary>
	/// Contains an ImageViewer and ImageController for allowing the user
	/// to load and edit images.
	/// </summary>
	// ----------------------------------------------------------------------
	public class ImageEditorDialog : System.Windows.Forms.Form
	{
		#region Fields

		// ------------------------------------------------------------------
		/// <summary>
		/// The ImageViewer to show an image.
		/// </summary>
		// ------------------------------------------------------------------
		ImageViewer imageViewer = new ImageViewer();

		private XPExplorerBar.TaskPane taskPane;
		private System.Windows.Forms.Splitter splitter1;
		private XPExplorerBar.Expando fileExpando;
		private System.Windows.Forms.LinkLabel openFileLinkLabel;
		private System.Windows.Forms.LinkLabel saveLinkLabel;
		private XPExplorerBar.Expando rotationExpando;
		private System.Windows.Forms.LinkLabel rotateLeftLinkLabel;
		private System.Windows.Forms.LinkLabel rotateRightLinkLabel;
		private System.Windows.Forms.LinkLabel flipHorizontalLinkLabel;
		private System.Windows.Forms.LinkLabel flipVerticalLinkLabel;
		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button resetButton;
		private XPExplorerBar.Expando resolutionExpando;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown horizResolutionNumericUpDown;
		private System.Windows.Forms.NumericUpDown vertResolutionNumericUpDown;
		private XPExplorerBar.Expando zoomExpando;
		private System.Windows.Forms.Button zoomInButton;
		private System.Windows.Forms.Button zoomOutButton;

		// ------------------------------------------------------------------
		/// <summary>
		/// Required designer variable.
		/// </summary>
		// ------------------------------------------------------------------
		private System.ComponentModel.Container components = null;

		#endregion

		#region Properties

		// ------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the Image.
		/// </summary>
		// ------------------------------------------------------------------
		public Image Image
		{
			get
			{
				return this.imageViewer.Image;
			}
			set
			{
				this.imageViewer.AddImage(value);
			}
		}		

		#endregion

		// ------------------------------------------------------------------
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="image">Image: The image to edit.</param>
		// ------------------------------------------------------------------
		public ImageEditorDialog(Image image) : base()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Controls.Add(this.imageViewer);
			this.imageViewer.Dock = DockStyle.Fill;
			this.imageViewer.BringToFront();

			if (image != null)
			{
				this.Image = image;
				this.horizResolutionNumericUpDown.Value =
					(decimal) this.imageViewer.HorizontalResolution;
				this.vertResolutionNumericUpDown.Value =
					(decimal) this.imageViewer.VerticalResolution;
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Opens an image from file.
		/// </summary>
		/// <param name="sender">object</param>
		/// <param name="e">LinkLabelLinkClickedEventArgs</param>
		// ------------------------------------------------------------------
		private void openFileLinkLabel_LinkClicked(object sender, 
			System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			this.imageViewer.OpenImage(false);

			if (this.Image != null)
			{
				this.horizResolutionNumericUpDown.Value =
					(decimal) this.imageViewer.HorizontalResolution;
				this.vertResolutionNumericUpDown.Value =
					(decimal) this.imageViewer.VerticalResolution;
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Saves the current image to a file.
		/// </summary>
		/// <param name="sender">object</param>
		/// <param name="e">LinkLabelLinkClickedEventArgs</param>
		// ------------------------------------------------------------------
		private void saveLinkLabel_LinkClicked(object sender, 
			System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			this.imageViewer.SaveCurrentImage();
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Rotates the image 90 degrees counter clockwise.
		/// </summary>
		/// <param name="sender">object</param>
		/// <param name="e">LinkLabelLinkClickedEventArgs</param>
		// ------------------------------------------------------------------
		private void rotateLeftLinkLabel_LinkClicked(object sender, 
			System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			this.imageViewer.Rotate90(false);
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Rotates the image 90 degrees clockwise.
		/// </summary>
		/// <param name="sender">object</param>
		/// <param name="e">LinkLabelLinkClickedEventArgs</param>
		// ------------------------------------------------------------------
		private void rotateRightLinkLabel_LinkClicked(object sender, 
			System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			this.imageViewer.Rotate90(true);
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Flips the image about the Y axis.
		/// </summary>
		/// <param name="sender">object</param>
		/// <param name="e">LinkLabelLinkClickedEventArgs</param>
		// ------------------------------------------------------------------
		private void flipHorizontalLinkLabel_LinkClicked(object sender, 
			System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			this.imageViewer.FlipX();
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Flips the image about the X axis.
		/// </summary>
		/// <param name="sender">object</param>
		/// <param name="e">LinkLabelLinkClickedEventArgs</param>
		// ------------------------------------------------------------------
		private void flipVerticalLinkLabel_LinkClicked(object sender, 
			System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			this.imageViewer.FlipY();
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Close this Dialog when the cancel button is clicked.
		/// </summary>
		/// <param name="sender">object</param>
		/// <param name="e">System.EventArgs</param>
		// ------------------------------------------------------------------
		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Close this Dialog when the cancel button is clicked.
		/// </summary>
		/// <param name="sender">object</param>
		/// <param name="e">System.EventArgs</param>
		// ------------------------------------------------------------------
		private void okButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Resets the image to it's original state.
		/// </summary>
		/// <param name="sender">object</param>
		/// <param name="e">System.EventArgs</param>
		// ------------------------------------------------------------------
		private void resetButton_Click(object sender, System.EventArgs e)
		{
			this.imageViewer.Reset();
		}

		#region Overrides

		// ------------------------------------------------------------------
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		// ------------------------------------------------------------------
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.taskPane = new XPExplorerBar.TaskPane();
			this.fileExpando = new XPExplorerBar.Expando();
			this.openFileLinkLabel = new System.Windows.Forms.LinkLabel();
			this.saveLinkLabel = new System.Windows.Forms.LinkLabel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.rotationExpando = new XPExplorerBar.Expando();
			this.rotateLeftLinkLabel = new System.Windows.Forms.LinkLabel();
			this.rotateRightLinkLabel = new System.Windows.Forms.LinkLabel();
			this.flipHorizontalLinkLabel = new System.Windows.Forms.LinkLabel();
			this.flipVerticalLinkLabel = new System.Windows.Forms.LinkLabel();
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.resetButton = new System.Windows.Forms.Button();
			this.resolutionExpando = new XPExplorerBar.Expando();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.horizResolutionNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.vertResolutionNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.zoomExpando = new XPExplorerBar.Expando();
			this.zoomInButton = new System.Windows.Forms.Button();
			this.zoomOutButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.taskPane)).BeginInit();
			this.taskPane.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.fileExpando)).BeginInit();
			this.fileExpando.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rotationExpando)).BeginInit();
			this.rotationExpando.SuspendLayout();
			this.bottomPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.resolutionExpando)).BeginInit();
			this.resolutionExpando.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horizResolutionNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.vertResolutionNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomExpando)).BeginInit();
			this.zoomExpando.SuspendLayout();
			this.SuspendLayout();
			// 
			// taskPane
			// 
			this.taskPane.AllowExpandoDragging = true;
			this.taskPane.AutoScrollMargin = new System.Drawing.Size(12, 12);
			this.taskPane.Dock = System.Windows.Forms.DockStyle.Right;
			this.taskPane.Expandos.AddRange(new XPExplorerBar.Expando[] {
																			this.fileExpando,
																			this.rotationExpando,
																			this.resolutionExpando,
																			this.zoomExpando});
			this.taskPane.Location = new System.Drawing.Point(472, 0);
			this.taskPane.Name = "taskPane";
			this.taskPane.Size = new System.Drawing.Size(192, 510);
			this.taskPane.TabIndex = 0;
			this.taskPane.SizeChanged += new System.EventHandler(this.taskPane_SizeChanged);
			// 
			// fileExpando
			// 
			this.fileExpando.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.fileExpando.Animate = true;
			this.fileExpando.AutoLayout = true;
			this.fileExpando.ExpandedHeight = 98;
			this.fileExpando.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.fileExpando.Items.AddRange(new System.Windows.Forms.Control[] {
																				   this.openFileLinkLabel,
																				   this.saveLinkLabel});
			this.fileExpando.Location = new System.Drawing.Point(12, 12);
			this.fileExpando.Name = "fileExpando";
			this.fileExpando.Size = new System.Drawing.Size(168, 98);
			this.fileExpando.TabIndex = 0;
			this.fileExpando.Text = "File";
			// 
			// openFileLinkLabel
			// 
			this.openFileLinkLabel.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.openFileLinkLabel.Location = new System.Drawing.Point(12, 37);
			this.openFileLinkLabel.Name = "openFileLinkLabel";
			this.openFileLinkLabel.TabIndex = 0;
			this.openFileLinkLabel.TabStop = true;
			this.openFileLinkLabel.Text = "Open...";
			this.openFileLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.openFileLinkLabel_LinkClicked);
			// 
			// saveLinkLabel
			// 
			this.saveLinkLabel.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.saveLinkLabel.Location = new System.Drawing.Point(12, 64);
			this.saveLinkLabel.Name = "saveLinkLabel";
			this.saveLinkLabel.TabIndex = 1;
			this.saveLinkLabel.TabStop = true;
			this.saveLinkLabel.Text = "Save...";
			this.saveLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.saveLinkLabel_LinkClicked);
			// 
			// splitter1
			// 
			this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter1.Location = new System.Drawing.Point(469, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 510);
			this.splitter1.TabIndex = 1;
			this.splitter1.TabStop = false;
			// 
			// rotationExpando
			// 
			this.rotationExpando.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.rotationExpando.Animate = true;
			this.rotationExpando.AutoLayout = true;
			this.rotationExpando.ExpandedHeight = 152;
			this.rotationExpando.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.rotationExpando.Items.AddRange(new System.Windows.Forms.Control[] {
																					   this.rotateLeftLinkLabel,
																					   this.rotateRightLinkLabel,
																					   this.flipHorizontalLinkLabel,
																					   this.flipVerticalLinkLabel});
			this.rotationExpando.Location = new System.Drawing.Point(12, 122);
			this.rotationExpando.Name = "rotationExpando";
			this.rotationExpando.Size = new System.Drawing.Size(168, 152);
			this.rotationExpando.TabIndex = 3;
			this.rotationExpando.Text = "Rotate and Flip";
			// 
			// rotateLeftLinkLabel
			// 
			this.rotateLeftLinkLabel.Location = new System.Drawing.Point(12, 37);
			this.rotateLeftLinkLabel.Name = "rotateLeftLinkLabel";
			this.rotateLeftLinkLabel.Size = new System.Drawing.Size(132, 23);
			this.rotateLeftLinkLabel.TabIndex = 0;
			this.rotateLeftLinkLabel.TabStop = true;
			this.rotateLeftLinkLabel.Text = "Rotate left 90 degrees";
			this.rotateLeftLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.rotateLeftLinkLabel_LinkClicked);
			// 
			// rotateRightLinkLabel
			// 
			this.rotateRightLinkLabel.Location = new System.Drawing.Point(12, 64);
			this.rotateRightLinkLabel.Name = "rotateRightLinkLabel";
			this.rotateRightLinkLabel.Size = new System.Drawing.Size(124, 23);
			this.rotateRightLinkLabel.TabIndex = 1;
			this.rotateRightLinkLabel.TabStop = true;
			this.rotateRightLinkLabel.Text = "Rotate right 90 degrees";
			this.rotateRightLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.rotateRightLinkLabel_LinkClicked);
			// 
			// flipHorizontalLinkLabel
			// 
			this.flipHorizontalLinkLabel.Location = new System.Drawing.Point(12, 91);
			this.flipHorizontalLinkLabel.Name = "flipHorizontalLinkLabel";
			this.flipHorizontalLinkLabel.TabIndex = 2;
			this.flipHorizontalLinkLabel.TabStop = true;
			this.flipHorizontalLinkLabel.Text = "Flip horizontal";
			this.flipHorizontalLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.flipHorizontalLinkLabel_LinkClicked);
			// 
			// flipVerticalLinkLabel
			// 
			this.flipVerticalLinkLabel.Location = new System.Drawing.Point(12, 118);
			this.flipVerticalLinkLabel.Name = "flipVerticalLinkLabel";
			this.flipVerticalLinkLabel.TabIndex = 3;
			this.flipVerticalLinkLabel.TabStop = true;
			this.flipVerticalLinkLabel.Text = "Flip vertical";
			this.flipVerticalLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.flipVerticalLinkLabel_LinkClicked);
			// 
			// bottomPanel
			// 
			this.bottomPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.bottomPanel.Controls.Add(this.resetButton);
			this.bottomPanel.Controls.Add(this.cancelButton);
			this.bottomPanel.Controls.Add(this.okButton);
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomPanel.Location = new System.Drawing.Point(0, 510);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(664, 56);
			this.bottomPanel.TabIndex = 2;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.okButton.Location = new System.Drawing.Point(454, 16);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 0;
			this.okButton.Text = "&OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.cancelButton.Location = new System.Drawing.Point(574, 16);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// resetButton
			// 
			this.resetButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.resetButton.Location = new System.Drawing.Point(40, 16);
			this.resetButton.Name = "resetButton";
			this.resetButton.TabIndex = 2;
			this.resetButton.Text = "&Reset";
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// resolutionExpando
			// 
			this.resolutionExpando.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.resolutionExpando.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.resolutionExpando.Items.AddRange(new System.Windows.Forms.Control[] {
																						 this.label1,
																						 this.label2,
																						 this.horizResolutionNumericUpDown,
																						 this.vertResolutionNumericUpDown});
			this.resolutionExpando.Location = new System.Drawing.Point(12, 286);
			this.resolutionExpando.Name = "resolutionExpando";
			this.resolutionExpando.Size = new System.Drawing.Size(168, 100);
			this.resolutionExpando.TabIndex = 4;
			this.resolutionExpando.Text = "Resolution";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Horizontal";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 74);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Vertical";
			// 
			// horizResolutionNumericUpDown
			// 
			this.horizResolutionNumericUpDown.Location = new System.Drawing.Point(88, 38);
			this.horizResolutionNumericUpDown.Maximum = new System.Decimal(new int[] {
																						 1000,
																						 0,
																						 0,
																						 0});
			this.horizResolutionNumericUpDown.Name = "horizResolutionNumericUpDown";
			this.horizResolutionNumericUpDown.Size = new System.Drawing.Size(72, 21);
			this.horizResolutionNumericUpDown.TabIndex = 2;
			this.horizResolutionNumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.horizResolutionNumericUpDown_KeyDown);
			this.horizResolutionNumericUpDown.ValueChanged += new System.EventHandler(this.horizResolutionNumericUpDown_ValueChanged);
			// 
			// vertResolutionNumericUpDown
			// 
			this.vertResolutionNumericUpDown.Location = new System.Drawing.Point(88, 72);
			this.vertResolutionNumericUpDown.Maximum = new System.Decimal(new int[] {
																						1000,
																						0,
																						0,
																						0});
			this.vertResolutionNumericUpDown.Name = "vertResolutionNumericUpDown";
			this.vertResolutionNumericUpDown.Size = new System.Drawing.Size(72, 21);
			this.vertResolutionNumericUpDown.TabIndex = 3;
			this.vertResolutionNumericUpDown.ValueChanged += new System.EventHandler(this.vertResolutionNumericUpDown_ValueChanged);
			// 
			// zoomExpando
			// 
			this.zoomExpando.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.zoomExpando.Animate = true;
			this.zoomExpando.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.zoomExpando.Items.AddRange(new System.Windows.Forms.Control[] {
																				   this.zoomInButton,
																				   this.zoomOutButton});
			this.zoomExpando.Location = new System.Drawing.Point(12, 398);
			this.zoomExpando.Name = "zoomExpando";
			this.zoomExpando.Size = new System.Drawing.Size(168, 100);
			this.zoomExpando.TabIndex = 5;
			this.zoomExpando.Text = "Zoom";
			// 
			// zoomInButton
			// 
			this.zoomInButton.BackColor = System.Drawing.Color.LightGray;
			this.zoomInButton.Location = new System.Drawing.Point(8, 48);
			this.zoomInButton.Name = "zoomInButton";
			this.zoomInButton.Size = new System.Drawing.Size(24, 23);
			this.zoomInButton.TabIndex = 0;
			this.zoomInButton.Text = "+";
			this.zoomInButton.Click += new System.EventHandler(this.zoomInButton_Click);
			// 
			// zoomOutButton
			// 
			this.zoomOutButton.BackColor = System.Drawing.Color.LightGray;
			this.zoomOutButton.Location = new System.Drawing.Point(136, 48);
			this.zoomOutButton.Name = "zoomOutButton";
			this.zoomOutButton.Size = new System.Drawing.Size(24, 23);
			this.zoomOutButton.TabIndex = 1;
			this.zoomOutButton.Text = "-";
			this.zoomOutButton.Click += new System.EventHandler(this.zoomOutButton_Click);
			// 
			// ImageEditorDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(664, 566);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.taskPane);
			this.Controls.Add(this.bottomPanel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "ImageEditorDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Image Editor";
			((System.ComponentModel.ISupportInitialize)(this.taskPane)).EndInit();
			this.taskPane.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.fileExpando)).EndInit();
			this.fileExpando.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rotationExpando)).EndInit();
			this.rotationExpando.ResumeLayout(false);
			this.bottomPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.resolutionExpando)).EndInit();
			this.resolutionExpando.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horizResolutionNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.vertResolutionNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.zoomExpando)).EndInit();
			this.zoomExpando.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void horizResolutionNumericUpDown_ValueChanged(object sender, 
			System.EventArgs e)
		{
			this.imageViewer.HorizontalResolution =
				(float) this.horizResolutionNumericUpDown.Value;
		}

		private void vertResolutionNumericUpDown_ValueChanged(object sender, 
			System.EventArgs e)
		{
			this.imageViewer.VerticalResolution =
				(float) this.vertResolutionNumericUpDown.Value;
		}

		private void horizResolutionNumericUpDown_KeyDown(object sender, 
			System.Windows.Forms.KeyEventArgs e)
		{
			this.horizResolutionNumericUpDown.Update();
		}

		private void taskPane_SizeChanged(object sender, System.EventArgs e)
		{
			foreach (Expando expando in this.taskPane.Expandos)
			{
				expando.AutoLayout = true;
			}
		}

		private void zoomInButton_Click(object sender, System.EventArgs e)
		{
			this.imageViewer.ZoomIn();
		}

		private void zoomOutButton_Click(object sender, System.EventArgs e)
		{
			this.imageViewer.ZoomOut();
		}

	}
}
