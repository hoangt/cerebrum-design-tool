using System;
using System.Collections.Generic;
using System.Text;

using Netron.Diagramming.Core;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;

namespace Netron.DataVisualization
{
    public class ChartShape : SimpleShapeBase
    {
        Chart chart;
        Image image;

        public override SizeF Magnification
        {
            get
            {
                return base.Magnification;
            }
            set
            {
                base.Magnification = value;
                Invalidate();
            }
        }

        public override string EntityName
        {
            get 
            {
                return "Chart";
            }
        }

        public ChartShape() : base()
        {
        }

        public ChartShape(IModel model)
            : base(model)
        {
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Initializes the shape.
        /// </summary>
        // ------------------------------------------------------------------
        protected override void Initialize()
        {
            base.Initialize();

            chart = new Chart();
            chart.ChartAreas.Add("default");
            chart.ChartAreas[0].Area3DStyle.Enable3D = true;
            chart.ChartAreas[0].BackColor = Color.AliceBlue;
            Series s = chart.Series.Add("Series1");
            s.ChartType = SeriesChartType.Column;
            int count = 10;
            Random r = new Random();
            for (int i = 0; i < count; i++)
            {
                s.Points.AddY(r.Next(20));
            }
            s.Color = Color.DarkOrange;
            s.ChartArea = chart.ChartAreas[0].Name;

            mPenStyle = new LinePenStyle();
            mPenStyle.Color = Color.Black;
            mPenStyle.DashStyle = DashStyle.Solid;

            mPaintStyle = new GradientPaintStyle(
                Color.White, // Start color
                Color.Silver, // End color
                90F); // Gradient angle

            //Connector top = new Connector(Model);
            //top.Parent = this;
            //top.Point = TopCenter;
            //mConnectors.Add(top);

            //Connector bottomLeft = new Connector(Model);
            //bottomLeft.Parent = this;
            //bottomLeft.Point = BottomLeftCorner;
            //mConnectors.Add(bottomLeft);

            //Connector bottomRight = new Connector(Model);
            //bottomRight.Parent = this;
            //bottomRight.Point = BottomRightCorner;
            //mConnectors.Add(bottomRight);

            Transform(50, 50, 500, 350);
            chart.Location = new Point(50, 50);
            chart.Width = 150;
            chart.Height = 150;
            //chart.PostPaint += new EventHandler<ChartPaintEventArgs>(chart_PostPaint);
        }

        public override void Paint(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.HighQuality;
            base.Paint(g);

            if (mVisible == false)
            {
                return;
            }

            Pen pen = mPenStyle.DrawingPen();

            if (mHovered)
            {
                pen = ArtPalette.HighlightPen;
            }

            g.DrawRectangle(pen, this.Rectangle);

            Rectangle chartArea = Rectangle;
            chartArea.Inflate(-2, -2);
            chart.Width = Width;
            chart.Height = Height;

            Bitmap bmp = new Bitmap(chartArea.Width, chartArea.Height);
            chart.DrawToBitmap(bmp, new Rectangle(0, 0, chartArea.Width, chartArea.Height));
            //Trace.WriteLine("Chart: width = " + Width.ToString() + 
            if (bmp != null)
            {
                g.DrawImage(bmp, chartArea);
            }
        }
    }
}
