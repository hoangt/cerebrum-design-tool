using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace ToolBox
{
    public static class Images
    {
        const string nameSpace = "ToolBox";

        static Bitmap myPropertiesImage = GetImage("Properties.png");

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents properties.
        /// </summary>
        // ------------------------------------------------------------------
        public static Bitmap Properties
        {
            get
            {
                return myPropertiesImage;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// The same as the Properties image but converted to an Icon.
        /// </summary>
        // ------------------------------------------------------------------
        public static Icon PropertiesIcon
        {
            get
            {
                return ConvertBitmapToIcon(myPropertiesImage);
            }
        }

        #endregion

        #region Helpers

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image from the embedded resources for the specified 
        /// filename and sets the image's transparent color to the one
        /// specified..
        /// </summary>
        /// <param name="filename">string: The filename from the embedded
        /// resources.</param>
        /// <param name="transparentColor">Color: The transparent color
        /// for the image.</param>
        /// <returns>Bitmap</returns>
        // ------------------------------------------------------------------
        static Bitmap GetImage(string filename, Color transparentColor)
        {
            Bitmap bmp = GetImage(filename);
            bmp.MakeTransparent(transparentColor);
            return bmp;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image from the embedded resources for the specified 
        /// filename.
        /// </summary>
        /// <param name="filename">string: The filename from the embedded
        /// resources.</param>
        /// <returns>Bitmap</returns>
        // ------------------------------------------------------------------
        static Bitmap GetImage(string filename)
        {
            return new Bitmap(GetStream(filename));
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the icon from the embedded resources for the specified 
        /// filename and converts it to an Image.
        /// </summary>
        /// <param name="filename">string: The filename from the embedded
        /// resources.</param>
        /// <returns>Bitmap</returns>
        // ------------------------------------------------------------------
        static Bitmap GetImageFromIcon(string filename)
        {
            Icon icon = new Icon(GetStream(filename));
            Bitmap image = Bitmap.FromHicon(icon.Handle);
            return image;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image from the embedded resources for the specified 
        /// filename and converts it to an Icon.
        /// </summary>
        /// <param name="filename">string: The filename from the embedded
        /// resources.</param>
        /// <returns>Icon</returns>
        // ------------------------------------------------------------------
        static Icon GetIconFromImage(string filename)
        {
            Bitmap image = GetImage(filename);
            return ConvertBitmapToIcon(image);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Converts the specified Bitmap to an Icon.
        /// </summary>
        /// <param name="image">Bitmap</param>
        /// <returns>Icon</returns>
        // ------------------------------------------------------------------
        static Icon ConvertBitmapToIcon(Bitmap image)
        {
            return Icon.FromHandle(image.GetHicon());
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns a Stream from the manifest resources for the specified 
        /// filename.
        /// </summary>
        /// <param name="filename">string</param>
        /// <returns>Stream</returns>
        // ------------------------------------------------------------------
        public static Stream GetStream(string filename)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(
                nameSpace +
                ".Resources." +
                filename);
        }

        #endregion
    }
}
