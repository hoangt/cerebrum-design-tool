using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

namespace ToolBox.Controls
{
	// ----------------------------------------------------------------------
	/// <summary>
	/// Handles different fill mode types for other UserControls to inherit
	/// from.  The fill mode types supported are Gradient, Solid, and Hatch.
	/// Double buffering and painting using WmPaint is utilized to reduce
	/// flickering.
	/// </summary>
	// ----------------------------------------------------------------------
	public class BaseControl : System.Windows.Forms.UserControl
	{
		#region Fields

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies if this control is being resized.
		/// </summary>
		// ------------------------------------------------------------------
		protected bool isResizing = false;

		// ------------------------------------------------------------------
		/// <summary>
		/// GraphicsPath for creating the shape of this Shape.
		/// </summary>
		// ------------------------------------------------------------------
		protected GraphicsPath graphicsPath = new GraphicsPath();

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the start gradient color.
		/// </summary>
		// ------------------------------------------------------------------
		Color gradientStart = Color.WhiteSmoke;
		
		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the end gradient color.
		/// </summary>
		// ------------------------------------------------------------------
		Color gradientEnd = Color.SlateGray;
		
		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the gradient mode.
		/// </summary>
		// ------------------------------------------------------------------
		LinearGradientMode gradientMode = LinearGradientMode.Horizontal;

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the fill type.
		/// </summary>
		// ------------------------------------------------------------------
		FillModeEnum fillMode = FillModeEnum.Gradient;

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the color of the border.
		/// </summary>
		// ------------------------------------------------------------------
		Color borderColor = Color.Black;

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the width of the border.
		/// </summary>
		// ------------------------------------------------------------------
		float borderWidth = 2;

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the color of the hatch lines.
		/// </summary>
		// ------------------------------------------------------------------
		Color hatchForeColor = Color.Black;

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the background color of the hatch.
		/// </summary>
		// ------------------------------------------------------------------
		Color hatchBackColor = Color.WhiteSmoke;

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the style of the hatch when the fill mode is set to
		/// hatch.
		/// </summary>
		// ------------------------------------------------------------------
		HatchStyle hatchStyle = HatchStyle.ForwardDiagonal;

		// ------------------------------------------------------------------
		// ------------------------------------------------------------------
		protected Brush brush = null;

		// ------------------------------------------------------------------
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		// ------------------------------------------------------------------
		private System.ComponentModel.Container components = null;

		#endregion

		// ------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		// ------------------------------------------------------------------
		public BaseControl() : base()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.fillMode = FillModeEnum.Gradient;
			Rectangle rect = this.ClientRectangle;
			this.brush = this.GetBrush(rect);

		}

		#region Properties

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the color of the border.
		/// </summary>
		// ------------------------------------------------------------------
		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.Invalidate();
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the width of the border.
		/// </summary>
		// ------------------------------------------------------------------
		public float BorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				this.borderWidth = value;
				this.Invalidate();
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the start gradient color.
		/// </summary>
		// ------------------------------------------------------------------
		public Color GradientStart
		{
			get
			{
				return this.gradientStart;
			}
			set
			{
				this.gradientStart = value;
				this.Invalidate();
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the background color of the hatch.
		/// </summary>
		// ------------------------------------------------------------------
		public Color HatchBackColor
		{
			get
			{
				return this.hatchBackColor;
			}
			set
			{
				this.hatchBackColor = value;
				this.Invalidate();
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the color of the hatch lines.
		/// </summary>
		// ------------------------------------------------------------------
		public Color HatchForeColor
		{
			get
			{
				return this.hatchForeColor;
			}
			set
			{
				this.hatchForeColor = value;
				this.Invalidate();
			}
		}
		
		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the end gradient color.
		/// </summary>
		// ------------------------------------------------------------------
		public Color GradientEnd
		{
			get
			{
				return this.gradientEnd;
			}
			set
			{
				this.gradientEnd = value;
				this.Invalidate();
			}
		}
		
		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the gradient mode.
		/// </summary>
		// ------------------------------------------------------------------
		public LinearGradientMode GradientMode
		{
			get
			{
				return this.gradientMode;
			}
			set
			{
				this.gradientMode = value;
				this.Invalidate();
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the fill type of this control.
		/// </summary>
		// ------------------------------------------------------------------
		public FillModeEnum FillMode
		{
			get
			{
				return this.fillMode;
			}
			set
			{
				this.fillMode = value;
				this.Invalidate();
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Specifies the hatch style when the fill mode is set to hatch.
		/// </summary>
		// ------------------------------------------------------------------
		public HatchStyle HatchStyle
		{
			get
			{
				return this.hatchStyle;
			}
			set
			{
				this.hatchStyle = value;
				this.Invalidate();
			}
		}

		#endregion

		#region Overrides

		// ------------------------------------------------------------------
		// ------------------------------------------------------------------
		protected override void OnPaint(PaintEventArgs e)
		{					
			base.OnPaintBackground (e);
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			this.graphicsPath = new GraphicsPath();
			Rectangle rect = this.ClientRectangle;
			this.graphicsPath.AddRectangle(rect);
			this.graphicsPath.CloseFigure();			
			this.brush = this.GetBrush(rect);
			e.Graphics.FillPath(this.brush, this.graphicsPath);
            if (this.BorderStyle != BorderStyle.None)
            {
                e.Graphics.DrawPath(this.GetPen(), this.graphicsPath);
            }
		}

		// ------------------------------------------------------------------
		// ------------------------------------------------------------------
		protected Pen GetPen()
		{
			Color c = System.Drawing.SystemColors.Control;
			if (this.borderColor != Color.Transparent)
			{
				c = this.borderColor;
			}
			return new Pen(c, this.borderWidth);
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Returns a new System.Drawing.Brush depending on the FillMode
		/// selected.
		/// </summary>
		/// <returns>Brush: A new Brush.</returns>
		// ------------------------------------------------------------------
		protected Brush GetBrush(Rectangle rect)
		{
			if (this.fillMode == FillModeEnum.Solid)
			{
				return new SolidBrush(this.BackColor);
			}
			else if (this.fillMode == FillModeEnum.Gradient)
			{
				return new LinearGradientBrush(
					rect,
					this.gradientStart,
					this.gradientEnd,
					this.gradientMode);
			}
			else if (this.fillMode == FillModeEnum.Hatch)
			{				
				return new HatchBrush(
					this.hatchStyle,
					this.hatchForeColor, 
					this.hatchBackColor);
			}
			else
			{
				return new SolidBrush(this.BackColor);
			}
		}


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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// GradientControl
			// 
			this.Name = "GradientControl";
			this.Size = new System.Drawing.Size(528, 150);

		}
		#endregion
	}
}
