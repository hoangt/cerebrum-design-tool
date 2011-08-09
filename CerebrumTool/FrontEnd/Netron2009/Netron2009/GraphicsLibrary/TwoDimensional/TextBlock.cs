using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace GraphicsLibrary
{
    [Serializable()]
    public class TextBlock : Renderer2D, ISerializable
    {
        protected Brush textBrush;
        protected string text = "";
        protected Font font = new Font("Arial", 10);
        protected Fill textFill = new Fill();
        protected StringAlignment horizontalAlignment = StringAlignment.Center;
        protected StringAlignment verticalAlignment = StringAlignment.Center;
        protected StringTrimming stringTrimming = StringTrimming.EllipsisWord;
        protected TextDirection textDirection = TextDirection.Horizontal;
        protected StringFormat stringFormat = new StringFormat();

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the string or text.
        /// </summary>
        // ------------------------------------------------------------------
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the Font type.
        /// </summary>
        // ------------------------------------------------------------------
        public Font Font
        {
            get
            {
                return this.font;
            }
            set
            {
                this.font = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the text fill style.
        /// </summary>
        // ------------------------------------------------------------------
        [TypeConverter(typeof(FillTypeConverter))]
        public Fill TextFill
        {
            get
            {
                return this.textFill;
            }
            set
            {
                this.textFill = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the horizontal alignment to be near, center, or far.
        /// </summary>
        // ------------------------------------------------------------------
        public StringAlignment HorizontalAlignment
        {
            get
            {
                return this.horizontalAlignment;
            }
            set
            {
                this.horizontalAlignment = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the vertical alignment to be near, center, or far.
        /// </summary>
        // ------------------------------------------------------------------
        public StringAlignment VerticalAlignment
        {
            get
            {
                return this.verticalAlignment;
            }
            set
            {
                this.verticalAlignment = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets how to trim characters from a string that does not
        /// completely fit in the bounds of this block.
        /// </summary>
        // ------------------------------------------------------------------
        public StringTrimming StringTrimming
        {
            get
            {
                return this.stringTrimming;
            }
            set
            {
                this.stringTrimming = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the direction (horizontal or vertical) of the text.
        /// </summary>
        // ------------------------------------------------------------------
        public TextDirection TextDirection
        {
            get
            {
                return this.textDirection;
            }
            set
            {
                this.textDirection = value;
            }
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public TextBlock() : base()
        {
            this.name = "Text Block";
            this.textFill.FillStyle = FillStyle.Solid;
            this.textFill.SolidColor = Color.Black;
            this.Fill.FillStyle = FillStyle.Solid;
            this.Fill.SolidColor = Color.Transparent;
            this.LineStyle = LineStyle.None;
            this.showShadow = false;
            
            this.bounds = new Rectangle(
                0,
                0,
                10,
                10);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        /// <param name="info">SerializationInfo: Stores all the data needed
        /// to serialize or deserialize an object.</param>
        /// <param name="context">StreamingContext: Describes the source and
        /// destination of a given serialized stream, and provides an 
        /// additional caller-defined context.</param>
        // ------------------------------------------------------------------
        protected TextBlock(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            this.text = (string)info.GetValue("Text", typeof(string));

            this.font = (Font)info.GetValue("Font", typeof(Font));
            this.textFill = (Fill)info.GetValue("TextFill", typeof(Fill));

            this.horizontalAlignment = 
                (StringAlignment)info.GetValue("HorizontalAlignment", 
                typeof(StringAlignment));

            this.verticalAlignment = 
                (StringAlignment)info.GetValue("VerticalAlignment", 
                typeof(StringAlignment));

            this.textDirection = 
                (TextDirection)info.GetValue("TextDirection", 
                typeof(TextDirection));

            this.stringTrimming =
                (StringTrimming)info.GetValue("StringTrimming",
                typeof(StringTrimming));
        }

        /// <summary>
        /// The graphics path is a simple rectangle.
        /// </summary>
        public override void CreateGraphicsPath()
        {
            this.graphicsPath = new GraphicsPath();
            this.graphicsPath.AddRectangle(this.bounds);          
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates the brush for the text.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void CreateBrush()
        {
            Rectangle rect = new Rectangle();
            rect.X = (int)this.bounds.X;
            rect.Y = (int)this.bounds.Y;
            rect.Height = (int)this.bounds.Height;
            rect.Width = (int)this.bounds.Width;
            this.textBrush = this.textFill.GetBrush(rect);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Creates the StringFormat based on verticalAlignment, 
        /// horizontalAlignment, textDirection, and fitTextToArea.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void CreateStringFormat()
        {
            this.stringFormat = new StringFormat();
            if (this.textDirection == TextDirection.Vertical)
            {
                this.stringFormat.FormatFlags =
                    this.stringFormat.FormatFlags | 
                    StringFormatFlags.DirectionVertical;
            }

            this.stringFormat.Trimming = this.stringTrimming;

            this.stringFormat.LineAlignment = this.verticalAlignment;
            this.stringFormat.Alignment = this.horizontalAlignment;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Paints the text block to the current Rectangle.  Set the Rectangle
        /// property before calling this.
        /// </summary>
        /// <param name="g">Graphics</param>
        // ------------------------------------------------------------------
        public override void Paint(Graphics g)
        {
            // Have the base Painter paint the background and outline first.
            // We want to paint the text OVER the background, not behind it.
            base.Paint(g);

            this.CreateBrush();
            this.CreateStringFormat();

            Rectangle rect = new Rectangle(
                (int)this.bounds.X,
                (int)this.bounds.Y,
                (int)this.bounds.Width,
                (int)this.bounds.Height);

            SizeF textSize = g.MeasureString(
                this.text,
                this.font,
                this.bounds.Size,
                this.stringFormat);

            rect.Size = textSize.ToSize();

            g.DrawString(
                this.text,
                this.font,
                this.textBrush,
                this.bounds,
                this.stringFormat);
        }

        #region ISerializable Members

        // ------------------------------------------------------------------
        /// <summary>
        /// ISerializable implementation.  Adds all data required to serialize
        /// the Fill to disk.
        /// </summary>
        /// <param name="info">SerializationInfo: Stores all the data needed
        /// to serialize or deserialize an object.</param>
        /// <param name="context">StreamingContext: Describes the source and
        /// destination of a given serialized stream, and provides an 
        /// additional caller-defined context.</param>
        // ------------------------------------------------------------------
        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Text", this.text);
            info.AddValue("Font", this.font, typeof(Font));
            info.AddValue("TextFill", this.textFill, typeof(Fill));

            info.AddValue("HorizontalAlignment", this.horizontalAlignment, 
                typeof(StringAlignment));

            info.AddValue("VerticalAlignment", this.verticalAlignment, 
                typeof(StringAlignment));

            info.AddValue("TextDirection", this.textDirection, 
                typeof(TextDirection));

            info.AddValue("StringTrimming", this.stringTrimming,
                typeof(StringTrimming));
        }

        #endregion
    }
}
