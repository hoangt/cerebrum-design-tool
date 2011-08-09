using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using Netron.Diagramming.Core;
using System.Windows.Forms;
using System.Diagnostics;

namespace Netron.Diagramming.Win
{
    public class ShapeToolBoxItem : IP.Components.Toolbox.Item
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// The shape this item is for.
        /// </summary>
        // ------------------------------------------------------------------
        IShape myShape;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the shape this item is for.
        /// </summary>
        // ------------------------------------------------------------------
        public IShape Shape
        {
            get
            {
                return myShape;
            }
            set
            {
                myShape = value;

                if ((myShape.EntityName != String.Empty) ||
                    (myShape.EntityName != ""))
                {
                    this.Text = myShape.EntityName;
                    this.Tooltip = myShape.EntityName;
                }

                if (myShape.LibraryImage != null)
                {
                    this.Image = myShape.LibraryImage;
                }
                else
                {
                    // We'll be nice and assign a default image.
                    this.Image = Images.ClassShape2;
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Default constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public ShapeToolBoxItem()
            : base()
        {
        }
    }
}
