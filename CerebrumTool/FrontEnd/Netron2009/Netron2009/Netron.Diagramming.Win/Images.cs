using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace Netron.Diagramming.Win
{
    public class Images
    {
        public const string PropertiesFileName =
            "PropertiesHS.png";

        #region Fields

        const string nameSpace = "Netron.Diagramming.Win";

        static Image alignObjectsBottom =
            GetImage("AlignObjectsBottomHS.png");

        static Image alignObjectsCenteredHorizontal =
            GetImage("AlignObjectsCenteredHorizontalHS.png");

        static Image alignObjectsCenteredVertical =
            GetImage("AlignObjectsCenteredVerticalHS.png");

        static Image alignObjectsLeft = GetImage("AlignObjectsLeftHS.png");

        static Image alignObjectsRight = GetImage("AlignObjectsRightHS.png");

        static Image alignObjectsTop = GetImage("AlignObjectsTopHS.png");

        static Image arrow = GetImage("StandardArrow.png");

        static Image bold = GetImage("Bold.png");

        static Image bringForward = GetImage("BringForwardHS.png");

        static Image bringToFront = GetImage("BringToFrontHS.png");

        static Image bucketFill = GetImage("BucketFill.bmp", Color.Magenta);

        static Image centerAlignment = GetImage(
            "CenterAlignment.bmp",
            Color.Silver);

        static Image classShape = GetImage("ClassShape.png");

        static Image mClassShape2 = GetImage("ClassShape2.png");

        static Image closedFolder = GetImage("ClosedFolder.gif");

        static Image connection = GetImage("Connection.png");

        static Image copy = GetImage("Copy.png");

        static Image cut = GetImage("Cut.png");

        static Image dashStyles = GetImage("DashStyles.png");

        static Image delete = GetImage("Delete.png");

        static Image drawEllipse = GetImage("DrawEllipse.png");

        static Image drawing = GetImage("Drawing.png");

        static Image drawRectangle = GetImage("DrawRectangle.png");

        static Image font = GetImage("FontDialogHS.png");

        static Image group = GetImage("Group.png");

        static Image hand = GetImage("hand.gif");

        static Image italic = GetImage("Italic.png");

        static Image layoutShapes = GetImage("LayoutShapes.png");

        static Image leftAlignment = GetImage(
            "LeftAlignment.bmp",
            Color.Silver);

        static Image lineCaps = GetImage("LineCaps.png");

        static Image lineWeights = GetImage("LineWeights.png");

        static Image moveConnector = GetImage("ConnectorMover.png");

        static Image multiLines = GetImage("StraightLines.png");

        static Image navigateForward = GetImage("NavForward.png");

        static Image navigateBack = GetImage("NavBack.png");

        static Image newDocumnet = GetImage("NewDocumentHS.png");

        static Image openFolder = GetImage("OpenFolder.png");

        static Image openedFolder = GetImage("OpenedFolder.gif");

        static Image outline = GetImage(
            "PenDraw.bmp",
            Color.Magenta);

        static Image page = GetImage("Page.png");

        static Image paste = GetImage("Paste.png");

        static Image picture = GetImage("InsertPicture.png");

        static Image polygon = GetImage("Polygon.png");

        static Image print = GetImage("Print.png");

        static Image printPreview = GetImage("PrintPreview.png");

        static Image printSetup = GetImage("PrintSetup.png");

        static Image properties = GetImage("PropertiesHS.png");

        static Image redo = GetImage("Edit_RedoHS.png");

        static Image rightAlignment = GetImage(
            "RightAlignment.bmp",
            Color.Silver);

        static Image save = GetImage("Save.png");

        static Image schema = GetImageFromIcon("Schema.ico");

        static Image scribble = GetImage("Scribble.png");

        static Image sendBackwards = GetImage("SendBackwardHS.png");

        static Image sendToBack = GetImage("SendToBackHS.png");

        static Image showConnectors = GetImage(
            "ShowConnectors.PNG",
            Color.Magenta);

        static Image mshowGrid = GetImage(
            "ShowGridlines2HS.png");

        static Image textBox = GetImage("Textbox.png");         

        static Image undo = GetImage("Edit_UndoHS.png");

        static Image ungroup = GetImage("Ungroup.png");

        static Image viewZoomIn = GetImage("ViewZoomIn.png");

        static Image viewZoomOut = GetImage("ViewZoomOut.png");

        static Image zoomIn = GetImage("ZoomIn.png", Color.White);

        static Image zoomOut = GetImage("ZoomOut.png", Color.White);

        static Image zoomMarquee = GetImage("ZoomMarquee.png", Color.White);

        #endregion

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents aligning the bottom edges of
        /// objects.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image AlignObjectsBottom
        {
            get
            {
                return alignObjectsBottom;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents horizontally aligning the center
        /// of objects.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image AlignObjectsCenteredHorizontal
        {
            get
            {
                return alignObjectsCenteredHorizontal;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents vertically aligning the center
        /// of objects.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image AlignObjectsCenteredVertical
        {
            get
            {
                return alignObjectsCenteredVertical;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents aligning the left edge of objects.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image AlignObjectsLeft
        {
            get
            {
                return alignObjectsLeft;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents aligning the right edge of objects.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image AlignObjectsRight
        {
            get
            {
                return alignObjectsRight;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents aligning the top edge of objects.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image AlignObjectsTop
        {
            get
            {
                return alignObjectsTop;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a arrow (standard cursor arrow).
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Arrow
        {
            get
            {
                return arrow;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents bold font.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Bold
        {
            get
            {
                return bold;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents bringing an object all the way to
        /// to the front in the z-order.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image BringToFront
        {
            get
            {
                return bringToFront;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents bring an object forward in the 
        /// z-order.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image BringForward
        {
            get
            {
                return bringForward;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets an image of a bucket of color - used to illustrate fill 
        /// color.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image BucketFill
        {
            get
            {
                return bucketFill;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents text being centered.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image CenterAlignment
        {
            get
            {
                return centerAlignment;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a ClassShape.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image ClassShape
        {
            get
            {
                return classShape;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets another image that represents a ClassShape.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image ClassShape2
        {
            get
            {
                return mClassShape2;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a folder that's closed.  This is
        /// used by the DiagramExplorer to represent a TreeNode that is
        /// not expanded.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image ClosedFolder
        {
            get
            {
                return closedFolder;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a connection line.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Connection
        {
            get
            {
                return connection;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents copying an item.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Copy
        {
            get
            {
                return copy;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents cutting an item.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Cut
        {
            get
            {
                return cut;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents different dashstyles.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image DashStyles
        {
            get
            {
                return dashStyles;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that has an 'x' to represent deleting something.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Delete
        {
            get
            {
                return delete;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents an ellipse.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image DrawEllipse
        {
            get
            {
                return drawEllipse;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a drawing, such as a bitmap.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Drawing
        {
            get
            {
                return drawing;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a rectangle.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image DrawRectangle
        {
            get
            {
                return drawRectangle;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents font.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Font
        {
            get
            {
                return font;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents grouping objects together.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Group
        {
            get
            {
                return group;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets an image of a hand (used to indicate panning).
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Hand
        {
            get
            {
                return hand;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets an image of italic font.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Italic
        {
            get
            {
                return italic;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents different shape layouts.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image LayoutShapes
        {
            get
            {
                return layoutShapes;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents text being aligned to the left.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image LeftAlignment
        {
            get
            {
                return leftAlignment;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents different linecaps.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image LineCaps
        {
            get
            {
                return lineCaps;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents different line weights.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image LineWeights
        {
            get
            {
                return lineWeights;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a connection line.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image MoveConnector
        {
            get
            {
                return moveConnector;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents straight lines, used for the
        /// MultiLine tool.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image MultiLines
        {
            get
            {
                return multiLines;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents navigating to the next page.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image NavigateForward
        {
            get
            {
                return navigateForward;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents navigating to the previous page.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image NavigateBack
        {
            get
            {
                return navigateBack;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents creating a new diagram.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image NewDocument
        {
            get
            {
                return newDocumnet;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents opening a file.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image OpenFolder
        {
            get
            {
                return openFolder;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a folder that's opened.  This is
        /// used by the DiagramExplorer to represent a TreeNode that is
        /// expanded.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image OpenedFolder
        {
            get
            {
                return openedFolder;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a pen drawing a line with a
        /// color - used to illustrate line color.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Outline
        {
            get
            {
                return outline;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a page in a book.  This is used
        /// by the DiagramExplorer's PageNode.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Page
        {
            get
            {
                return page;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents pasting an item from the clipboard.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Paste
        {
            get
            {
                return paste;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a picture or image.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Picture
        {
            get
            {
                return picture;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a polygon.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Polygon
        {
            get
            {
                return polygon;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image of a printer.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Print
        {
            get
            {
                return print;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image for items that perform a print preview.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image PrintPreview
        {
            get
            {
                return printPreview;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image for items that perform a print setup.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image PageSetup
        {
            get
            {
                return printSetup;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents properties.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Properties
        {
            get
            {
                return properties;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents redoing a command.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Redo
        {
            get
            {
                return redo;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents text being aligned to the right.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image RightAlignment
        {
            get
            {
                return rightAlignment;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents saving to disk.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Save
        {
            get
            {
                return save;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents a database schema.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Schema
        {
            get
            {
                return schema;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents drawing arcs.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Scribble
        {
            get
            {
                return scribble;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents sending an object back in the 
        /// z-order.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image SendBackwards
        {
            get
            {
                return sendBackwards;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents sending an object all the way back
        /// in the z-order.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image SendToBack
        {
            get
            {
                return sendToBack;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image for showing/hiding connectors.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image ShowConnectors
        {
            get
            {
                return showConnectors;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image for showing/hiding grid lines.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image ShowGrid
        {
            get
            {
                return mshowGrid;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image of a text box.  This is used to represent the
        /// simple TextOnlyShape.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image TextBox
        {
            get
            {
                return textBox;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents undoing a command.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Undo
        {
            get
            {
                return undo;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents ungrouping objects.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image Ungroup
        {
            get
            {
                return ungroup;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents zooming in.  The image is a 
        /// magnifying glass with a plus sign.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image ViewZoomIn
        {
            get
            {
                return viewZoomIn;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents zooming out.  The image is a 
        /// magnifying glass with a minus sign.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image ViewZoomOut
        {
            get
            {
                return viewZoomOut;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents zooming in.  The image is a simple
        /// plus sign in a circle.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image ZoomIn
        {
            get
            {
                return zoomIn;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents zooming in/out on an area.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image ZoomMarquee
        {
            get
            {
                return zoomMarquee;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the image that represents zooming out.  The image is a simple
        /// minus sign in a circle.
        /// </summary>
        // ------------------------------------------------------------------
        public static Image ZoomOut
        {
            get
            {
                return zoomOut;
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
