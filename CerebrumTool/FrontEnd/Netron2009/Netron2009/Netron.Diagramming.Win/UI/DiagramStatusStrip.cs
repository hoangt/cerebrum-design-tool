using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Netron.Diagramming.Core;
using System.Drawing;

namespace Netron.Diagramming.Win
{
    public class DiagramStatusStrip : StatusStrip
    {
        ToolStripLabel entityNameLabel = new ToolStripLabel();
        ToolStripLabel entityWidthLabel = new ToolStripLabel();
        ToolStripLabel entityHeightLabel = new ToolStripLabel();
        ToolStripLabel entityPositionLabel = new ToolStripLabel();
        ToolStripLabel currentPageLabel = new ToolStripLabel();
        
        DiagramControl myDiagram;

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the diagram control to display the status for.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramControl Diagram
        {
            get
            {
                return this.myDiagram;
            }
            set
            {
                if (value != null)
                {
                    this.myDiagram = value;
                    RegisterEvents();
                    UpdatePageLabel();
                    this.myDiagram.OnNewDiagram +=
                        new EventHandler(OnNewDiagram);
                }
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public DiagramStatusStrip()
            : base()
        {
            this.LayoutStyle = ToolStripLayoutStyle.StackWithOverflow;

            this.entityNameLabel.AutoSize = true;
            this.entityHeightLabel.AutoSize = true;
            this.entityPositionLabel.AutoSize = true;
            this.entityWidthLabel.AutoSize = true;
            this.currentPageLabel.AutoSize = true;
            this.currentPageLabel.Alignment = ToolStripItemAlignment.Right;

            this.Items.Add(this.entityNameLabel);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(this.entityWidthLabel);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(this.entityHeightLabel);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(this.entityPositionLabel);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(this.currentPageLabel);
            this.currentPageLabel.TextAlign = ContentAlignment.MiddleRight;

            ResetStatusLabels();

            Selection.OnNewSelection +=
                new EventHandler(HandleOnNewSelection);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called when a new diagram is created.  All events are registered
        /// for by calling "RegisterEvents()".
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void OnNewDiagram(
            object sender,
            EventArgs e)
        {
            RegisterEvents();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Hooks-up the required events to update this status strip when
        /// a new document is created in the diagram.
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void RegisterEvents()
        {
            this.myDiagram.Controller.Model.OnCurrentPageChanged +=
                new CurrentPageChangedEventHandler(OnCurrentPageChanged);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Called when the current page in the diagram is changed.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">PageEventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void OnCurrentPageChanged(
            object sender, 
            PageEventArgs e)
        {
            UpdatePageLabel();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Sets the text of the "currentPageLabel" to be:
        /// Page "Index Of Current Page" / "Total Page Count".  For example, if
        /// the current page index is 2, and there are 3 pages, then the label
        /// will read: "Page 2/3".
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void UpdatePageLabel()
        {
            int currentPageIndex = this.Diagram.Controller.Model.Pages.IndexOf(
                   this.Diagram.Controller.Model.CurrentPage) + 1;

            int pageCount = Diagram.Controller.Model.Pages.Count;

            this.currentPageLabel.Text = "Page " +
                currentPageIndex.ToString() + "/" +
                pageCount.ToString();
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Display's the first entities (index 0) name, width, height, and
        /// position.  If there are no entities, then all panel's are cleared.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        protected virtual void HandleOnNewSelection(
            object sender, 
            EventArgs e)
        {
            if (Selection.SelectedItems.Count > 0)
            {
                IDiagramEntity entity = Selection.SelectedItems[0];
                if (entity != null)
                {
                    ShowEntityStatus(entity);
                    //entity.OnEntityChange +=
                    //    new EventHandler<EntityEventArgs>(HandleOnEntityChange);
                }
            }
            else
            {
                ResetStatusLabels();
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Display's the name, width, height, and position of the given
        /// entity.
        /// </summary>
        /// <param name="entity">IDiagramEntity</param>
        // ------------------------------------------------------------------
        protected virtual void ShowEntityStatus(IDiagramEntity entity)
        {
            if (entity == null)
            {
                ResetStatusLabels();
                return;
            }

            PointF location = new PointF(
                (float) entity.Rectangle.X,
                (float) entity.Rectangle.Y);
            location = Diagram.View.WorldToView(location);

            this.entityHeightLabel.Text = "Height: " +
                entity.Rectangle.Height.ToString();

            this.entityWidthLabel.Text = "Width: " +
                entity.Rectangle.Width.ToString();

            this.entityNameLabel.Text = "Name: " + entity.EntityName;

            this.entityPositionLabel.Text = "Position: " +
                location.ToString();
        }

        void HandleOnEntityChange(object sender, EntityEventArgs e)
        {
            ShowEntityStatus(e.Entity);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Clears the contents of all label's by setting their text to "".
        /// </summary>
        // ------------------------------------------------------------------
        protected virtual void ResetStatusLabels()
        {
            this.entityHeightLabel.Text = "Height:";
            this.entityWidthLabel.Text = "Width:";
            this.entityNameLabel.Text = "Name:";
            this.entityPositionLabel.Text = "Position:";
        }
    }
}
