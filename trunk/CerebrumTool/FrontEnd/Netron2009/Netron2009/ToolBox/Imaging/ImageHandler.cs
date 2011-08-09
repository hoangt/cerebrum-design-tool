using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ToolBox.Imaging
{
	// ----------------------------------------------------------------------
	/// <summary>
	/// Contains static methods for sizing and cropping images.
	/// </summary>
	// ----------------------------------------------------------------------
	public class ImageHandler
	{
		// ------------------------------------------------------------------
		/// <summary>
		/// Enumeration of the dimensions available for an image.
		/// </summary>
		// ------------------------------------------------------------------
		public enum Dimensions 
		{
			Width,
			Height
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Enumeration of the anchor positions available for an image.
		/// </summary>
		// ------------------------------------------------------------------
		public enum AnchorPosition
		{
			Top,
			Center,
			Bottom,
			Left,
			Right
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Scales an image by the percentage specified.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="percent"></param>
		/// <returns></returns>
		// ------------------------------------------------------------------
		public static Image ScaleByPercent(Image image, int percent)
		{
			float nPercent = ((float)percent/100);

			int sourceWidth = image.Width;
			int sourceHeight = image.Height;
			int sourceX = 0;
			int sourceY = 0;

			int destX = 0;
			int destY = 0; 
			int destWidth  = (int)(sourceWidth * nPercent);
			int destHeight = (int)(sourceHeight * nPercent);

			Bitmap bmPhoto = new Bitmap(destWidth, destHeight, 
				PixelFormat.Format24bppRgb);
			bmPhoto.SetResolution(image.HorizontalResolution, 
				image.VerticalResolution);

			Graphics grPhoto = Graphics.FromImage(bmPhoto);
			grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

			grPhoto.DrawImage(image, 
				new Rectangle(destX,destY,destWidth,destHeight),
				new Rectangle(sourceX,sourceY,sourceWidth,sourceHeight),
				GraphicsUnit.Pixel);

			grPhoto.Dispose();
			return bmPhoto;
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Scales an image to the size specified.
		/// </summary>
		/// <param name="imgPhoto"></param>
		/// <param name="Width"></param>
		/// <param name="Height"></param>
		/// <returns></returns>
		// ------------------------------------------------------------------
		public static Image ScaleToSize(
			Image image, 
			int width, 
			int height)
		{
			int sourceWidth = image.Width;
			int sourceHeight = image.Height;
			int sourceX = 0;
			int sourceY = 0;
			int destX = 0;
			int destY = 0; 

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = ((float)width/(float)sourceWidth);
			nPercentH = ((float)height/(float)sourceHeight);
			if(nPercentH < nPercentW)
			{
				nPercent = nPercentH;
				destX = System.Convert.ToInt16((width - 
					(sourceWidth * nPercent))/2);
			}
			else
			{
				nPercent = nPercentW;
				destY = System.Convert.ToInt16((height - 
					(sourceHeight * nPercent))/2);
			}

			int destWidth  = (int)(sourceWidth * nPercent);
			int destHeight = (int)(sourceHeight * nPercent);

			Bitmap bmPhoto = new Bitmap(width, height, 
				PixelFormat.Format24bppRgb);
			bmPhoto.SetResolution(image.HorizontalResolution, 
				image.VerticalResolution);

			Graphics grPhoto = Graphics.FromImage(bmPhoto);
			grPhoto.Clear(Color.Red);
			grPhoto.InterpolationMode = 
				InterpolationMode.HighQualityBicubic;

			grPhoto.DrawImage(image, 
				new Rectangle(destX,destY,destWidth,destHeight),
				new Rectangle(sourceX,sourceY,sourceWidth,sourceHeight),
				GraphicsUnit.Pixel);

			grPhoto.Dispose();
			return bmPhoto;
		}

		// ------------------------------------------------------------------
		// ------------------------------------------------------------------
		public static Image Crop(Image imgPhoto, int Width, 
			int Height, AnchorPosition Anchor)
		{
			int sourceWidth = imgPhoto.Width;
			int sourceHeight = imgPhoto.Height;
			int sourceX = 0;
			int sourceY = 0;
			int destX = 0;
			int destY = 0;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = ((float)Width/(float)sourceWidth);
			nPercentH = ((float)Height/(float)sourceHeight);

			if(nPercentH < nPercentW)
			{
				nPercent = nPercentW;
				switch(Anchor)
				{
					case AnchorPosition.Top:
						destY = 0;
						break;
					case AnchorPosition.Bottom:
						destY = (int)
							(Height - (sourceHeight * nPercent));
						break;
					default:
						destY = (int)
							((Height - (sourceHeight * nPercent))/2);
						break;
				}
			}
			else
			{
				nPercent = nPercentH;
				switch(Anchor)
				{
					case AnchorPosition.Left:
						destX = 0;
						break;
					case AnchorPosition.Right:
						destX = (int)
							(Width - (sourceWidth * nPercent));
						break;
					default:
						destX = (int)
							((Width - (sourceWidth * nPercent))/2);
						break;
				} 
			}

			int destWidth  = (int)(sourceWidth * nPercent);
			int destHeight = (int)(sourceHeight * nPercent);

			Bitmap bmPhoto = new Bitmap(Width, 
				Height, PixelFormat.Format24bppRgb);
			bmPhoto.SetResolution(imgPhoto.HorizontalResolution, 
				imgPhoto.VerticalResolution);

			Graphics grPhoto = Graphics.FromImage(bmPhoto);
			grPhoto.InterpolationMode = 
				InterpolationMode.HighQualityBicubic;

			grPhoto.DrawImage(imgPhoto, 
				new Rectangle(destX,destY,destWidth,destHeight),
				new Rectangle(sourceX,sourceY,sourceWidth,sourceHeight),
				GraphicsUnit.Pixel);

			grPhoto.Dispose();
			return bmPhoto;
		}
	}
}
