using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace ToolBox.Imaging
{
	// ----------------------------------------------------------------------
	/// <summary>
	/// ImageViewer is a UserControl that contains a PictureBox with Image
	/// auto-scaling, zooming, rotating, and flipping capapbilities.
	/// </summary>
	// ----------------------------------------------------------------------
	public class ImageViewer : System.Windows.Forms.UserControl
	{
		#region Fields

		// ------------------------------------------------------------------
		/// <summary>
		/// The percentage to scale the image by when zooming.
		/// </summary>
		// ------------------------------------------------------------------
		int scalePercent = 5;

		// ------------------------------------------------------------------
		/// <summary>
		/// The index of the Image currently being displayed.
		/// </summary>
		// ------------------------------------------------------------------
		int currentIndex = 0;

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies if the collection of Images should wrap around when
		/// navigating through the collection.
		/// </summary>
		// ------------------------------------------------------------------
		bool wrap = true;

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies if the Image displayed is sized to fit in the bounds of
		/// this Panel.
		/// </summary>
		// ------------------------------------------------------------------
		bool autoScaleImage =	true;

		// ------------------------------------------------------------------
		/// <summary>
		/// The collection of Images to display.
		/// </summary>
		// ------------------------------------------------------------------
		ImageCollection myImages = new ImageCollection();		

		// ------------------------------------------------------------------
		/// <summary>
		/// Required designer variable.
		/// </summary>
		// ------------------------------------------------------------------
		private System.ComponentModel.Container components = null;

		// ------------------------------------------------------------------
		/// <summary>
		/// The picture box to show an image in.
		/// </summary>
		// ------------------------------------------------------------------
		PictureBox myPictureBox = new PictureBox();

		// ------------------------------------------------------------------
		/// <summary>
		/// The original version of the current picture being viewed.
		/// </summary>
		// ------------------------------------------------------------------
		Image originalImage = null;

		// ------------------------------------------------------------------
		/// <summary>
		/// The "divisor" for calculating the new image size during a zoom
		/// operation.
		/// </summary>
		// ------------------------------------------------------------------
		int zoomFactor = 10;

		#endregion

		#region Properties

		// ------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the percentage to scale the image by when zooming.
		/// </summary>
		// ------------------------------------------------------------------
		public int ScalePercent
		{
			get
			{
				return this.scalePercent;
			}
			set
			{
				this.scalePercent = value;
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the horizontal resolution, in dots per inch, for the
		/// current Image.
		/// </summary>
		// ------------------------------------------------------------------
		public float HorizontalResolution
		{
			get
			{				
				return ( (Bitmap) this.Image ).HorizontalResolution;
			}
			set
			{
				try
				{
					Bitmap bmp = (Bitmap) this.Image;
					bmp.SetResolution(value, bmp.VerticalResolution);	
				}
				catch(Exception e)
				{
					MessageBox.Show(e.Message);
				}
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the vertical resolution, in dots per inch, for the
		/// current Image.
		/// </summary>
		// ------------------------------------------------------------------
		public float VerticalResolution
		{
			get
			{
				return ( (Bitmap) this.Image ).VerticalResolution;
			}
			set
			{
				try
				{
					Bitmap bmp = (Bitmap) this.Image;
					bmp.SetResolution(bmp.HorizontalResolution, value);	
				}
				catch(Exception e)
				{
					MessageBox.Show(e.Message);
				}
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the amount to zoom in or out by.
		/// </summary>
		// ------------------------------------------------------------------
		public int ZoomFactor
		{
			get
			{
				return this.zoomFactor;
			}
			set
			{
				this.zoomFactor = value;
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Gets or sets if the Image should be / is sized to fit in the bounds
		/// of this PictureBox.
		/// </summary>
		// ------------------------------------------------------------------
		public bool AutoScaleImage
		{
			get
			{
				return this.autoScaleImage;
			}
			set
			{
				this.autoScaleImage = value;
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Gets or sets if the collection of Images wraps around during 
		/// navigation.  If set to true, then the navigation mode is 
		/// "continuous".
		/// </summary>
		// ------------------------------------------------------------------
		public bool Wrap
		{
			get
			{
				return this.wrap;
			}
			set
			{
				this.wrap = value;
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the current Image displayed without adding it to
		/// the collection of Images.
		/// </summary>
		// ------------------------------------------------------------------
		public Image Image
		{
			get
			{
				return this.myPictureBox.Image;
			}
			set
			{
				if (value != null)
				{
					this.myPictureBox.Image = value;
					this.originalImage = (Image) value.Clone();

					if (this.autoScaleImage)
					{
						if (!this.FitCheck(this.originalImage))
						{
							this.AutoFit();
						}
					}
				}
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Gets or sets the border of this control.
		/// </summary>
		// ------------------------------------------------------------------
		new public BorderStyle BorderStyle
		{
			get
			{
				return this.myPictureBox.BorderStyle;
			}
			set
			{
				this.myPictureBox.BorderStyle = value;
			}
		}

		#endregion

		#region Constructors

		// ------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		// ------------------------------------------------------------------
		public ImageViewer() : base()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Add the PictureBox and make it fill this control.
			this.Controls.Add(this.myPictureBox);
			this.myPictureBox.Dock = DockStyle.Fill;

			// Center the Image.
			this.myPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		// ------------------------------------------------------------------
		public ImageViewer(ImageCollection images)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// Add the PictureBox and make it fill this control.
			this.Controls.Add(this.myPictureBox);
			this.myPictureBox.Dock = DockStyle.Fill;

			// Center the Image.
			this.myPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;

			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

			// If there are images in the specified collection,
			if ( (images != null) && (images.Count > 0) )
			{
				// Assign local reference to the collection of images.
				this.myImages = images;

				// Show the first image.
				this.Image = this.myImages[0];
			}
		}

		#endregion

		// ------------------------------------------------------------------
		/// <summary>
		/// Adds the specified Image to the collection and displays it.
		/// </summary>
		/// <param name="image">Image: The Image to add and show.</param>
		// ------------------------------------------------------------------
		public void AddImage(Image image)
		{
			// Add the Image to the collection.
			this.myImages.Add(image);

			// Set the current Image to be displayed.
			this.Image = image;
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Performs a "fit check" on the specified image.  If the image's
		/// width and height are greater than this region, then false is
		/// returned.
		/// </summary>
		/// <param name="image">Image: Any image type.</param>
		/// <returns>bool: True if the image will fit, false 
		/// otherwise.</returns>
		// ------------------------------------------------------------------
		public bool FitCheck(Image image)
		{
			if ( (image.Width > this.Width) ||
				(image.Height > this.Height) )
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Fits the current image to the region of this viewer.
		/// </summary>
		// ------------------------------------------------------------------
		public void AutoFit()
		{
			this.CreateNewImage(this.Size);
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Rotates the image by 90 degrees in the clockwise or 
		/// counter-clockwise direction.
		/// </summary>
		/// <param name="clockwise">bool: The rotation is performed in the
		/// clockwise direction if true, counter-clockwise if false.</param>
		// ------------------------------------------------------------------
		public void Rotate90(bool clockwise)
		{
			RotateFlipType flipType = RotateFlipType.Rotate270FlipNone;
			if (clockwise)
			{
				flipType = RotateFlipType.Rotate90FlipNone;
			}

			this.myPictureBox.Image.RotateFlip(flipType);
			this.myPictureBox.Refresh();
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Performs a horizontal flip on the image.
		/// </summary>
		// ------------------------------------------------------------------
		public void FlipX()
		{
			this.myPictureBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
			this.myPictureBox.Refresh();
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Performs a vertical flip on the image.
		/// </summary>
		// ------------------------------------------------------------------
		public void FlipY()
		{
			this.myPictureBox.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
			this.myPictureBox.Refresh();
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Zooms in on the current image using the "ZoomFactor" property.
		/// </summary>
		// ------------------------------------------------------------------
		public void ZoomIn()
		{
			// If there is an image loaded,
			if (this.myPictureBox.Image != null)
			{
				// get the current width and height of it.
				int currWidth = this.myPictureBox.Image.Width;
				int currHeight = this.myPictureBox.Image.Height;

				// Calculate the new width and heidth.
				int newWidth = currWidth + (currWidth / this.zoomFactor);
				int newHeight = currHeight + (currHeight / this.zoomFactor);

				// Get the scaled image.
//				this.Image = ImageHandler.ScaleToSize(
//					this.Image,
//					newWidth,
//					newHeight);
				this.myPictureBox.Image = ImageHandler.ScaleByPercent(
					this.Image, 100 + this.scalePercent);
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Zooms out on the current image using the "ZoomFactor" property.
		/// </summary>
		// ------------------------------------------------------------------
		public void ZoomOut()
		{			
			// If there is an image loaded,
			if (this.myPictureBox.Image != null)
			{
				// get the current width and height of it.
				int currWidth = this.myPictureBox.Image.Width;
				int currHeight = this.myPictureBox.Image.Height;

				// Calculate the new width and heidth.
				int newWidth = currWidth - (currWidth / this.zoomFactor);
				int newHeight = currHeight - (currHeight / this.zoomFactor);

				// Get the scaled image.
//				this.Image = ImageHandler.ScaleToSize(
//					this.Image,
//					newWidth,
//					newHeight);
				this.myPictureBox.Image = ImageHandler.ScaleByPercent(
					this.Image, 100 - this.scalePercent);
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Creates a new Image with the same type as the current image using
		/// the specified size.
		/// </summary>
		/// <param name="size">Size: The size of the new Image.</param>
		// ------------------------------------------------------------------
		private void CreateNewImage(Size size)
		{
			// Create a new Bitmap with size specified.
			Bitmap bmp = new Bitmap(this.Image, size);

			// Set the picture box's image to the new BitMap.
			this.myPictureBox.Image = bmp;
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Resets the image to it's original state.
		/// </summary>
		// ------------------------------------------------------------------
		public void Reset()
		{
			this.myPictureBox.Image = (Image) this.originalImage.Clone();
		}

		#region File IO

		// ------------------------------------------------------------------
		/// <summary>
		/// Saves the current image to disk.  A SaveFileDialog is displayed
		/// for the user to specify where to save the image.
		/// </summary>
		// ------------------------------------------------------------------
		public void SaveCurrentImage()
		{
			if (this.Image == null)
			{
				return;
			}
			else
			{
				SaveFileDialog dlg = new SaveFileDialog();

				dlg.Filter = "All Image Files |*.jpg;*.bmp;*.gif;*.png";
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					try
					{
						this.Image.Save(dlg.FileName);
					}
					catch (Exception ex)
					{
						MessageBox.Show(
							"An error ocurred while saving the file.\n\n" +
							ex.Message + "\n\n" + ex.StackTrace);
					}
				}
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Displays an OpenFileDialog box, opens the specified image 
		/// file(s), adds the files to the collection, and displays the last
		/// file opened.
		/// </summary>
		/// <param name="multiSelect">bool: Option to allow more than one
		/// image file to be opened at a time.</param>
		/// <returns>bool: True if successful, false otherwise.</returns>
		// ------------------------------------------------------------------
		public Image[] OpenImage(bool multiSelect)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Multiselect = multiSelect;

			dlg.Filter = "All Image Files |*.jpg;*.bmp;*.gif;*.png";

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				ArrayList allImagesOpened = new ArrayList();
				foreach (string fileName in dlg.FileNames)
				{
					StreamReader sr = new StreamReader(fileName);
					Image i = Image.FromStream(sr.BaseStream);
					this.AddImage(i);
					allImagesOpened.Add(i);
				}

				return (Image[]) allImagesOpened.ToArray(typeof(Image));;
			}
			else
			{
				return null;
			}
		}

		#endregion

		#region Navigation

		// ------------------------------------------------------------------
		/// <summary>
		/// Displays the next Image in the collection.
		/// </summary>
		// ------------------------------------------------------------------
		public void ShowNextImage(bool keepCurrentPictureSettings)
		{
			// If there are Images in the collection,
			if (this.myImages.Count > 0)
			{
				// If the current index is less than the number of Images - 2
				// (i.e. we can go forward one)
				if (this.currentIndex < (this.myImages.Count - 2) )
				{
					// Increment the current index and show the Image.
					this.currentIndex++;
					this.Image = this.myImages[this.currentIndex];
				}
				else
				{
					// otherwise, we are at the end of the list.  If wrapping
					// is enabled, then reset the current index to zero and
					// display the first Image.
					if (this.wrap)
					{
						this.currentIndex = 0;
						this.Image = this.myImages[0];
					}
				}
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Displays the previous Image in the collection.
		/// </summary>
		// ------------------------------------------------------------------
		public void ShowPreviousImage()
		{
			// If there are Images in the collection,
			if (this.myImages.Count > 0)
			{
				// If the current index is greater than zero (i.e. we can move
				// back one)
				if (this.currentIndex > 0)
				{
					// Decrement the current index and show the Image.
					this.currentIndex--;
					this.Image = this.myImages[this.currentIndex];
				}
				else
				{
					// otherwise, we are at the start of the list.  If wrapping
					// is enabled, then set the current index to the last Image
					// in the list and display it.
					if (this.wrap)
					{
						this.currentIndex = this.myImages.Count - 1;
						this.Image = this.myImages[this.currentIndex];
					}
				}
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Displays the next Image in the collection.
		/// </summary>
		// ------------------------------------------------------------------
		public void ShowFirstImage()
		{
			// If there are Images in the collection,
			if (this.myImages.Count > 0)
			{
				// Reset the current index.
				this.currentIndex = 0;

				// Show the Image.
				this.Image = this.myImages[0];
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Displays the last Image in the collection.
		/// </summary>
		// ------------------------------------------------------------------
		public void ShowLastImage()
		{
			// If there are Images in the collection,
			if (this.myImages.Count > 0)
			{
				// Set the current index to the last index in the array.
				this.currentIndex = this.myImages.Count - 1;

				// Show the Image.
				this.Image = this.myImages[this.currentIndex];
			}
		}

		#endregion

		#region Cleanup

		// ------------------------------------------------------------------
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		// ------------------------------------------------------------------
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion

		
	}
}
