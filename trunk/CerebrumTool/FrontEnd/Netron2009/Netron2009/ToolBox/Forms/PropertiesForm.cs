using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ToolBox.Forms
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A simple Form that contains a PropertyGrid for displaying an object's
    /// or an array of object's properties.
    /// </summary>
    // ----------------------------------------------------------------------
    public partial class PropertiesForm : Form
    {
        // ------------------------------------------------------------------
        /// <summary>
        /// Gets the PropertyGrid in the Form.
        /// </summary>
        // ------------------------------------------------------------------
        public PropertyGrid PropertyGrid
        {
            get
            {
                return myPropertyGrid;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public PropertiesForm()
        {
            InitializeComponent();

            this.Icon = Images.PropertiesIcon;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Displays the PropertiesForm and shows the objects specified in the
        /// PropertyGrid.  When using this method, the Form is shown in its
        /// normal state (i.e. not as a modal dialog).
        /// </summary>
        /// <param name="selectedObjects">object[]</param>
        // ------------------------------------------------------------------
        public static void ShowPropertiesForm(object[] selectedObjects)
        {
            PropertiesForm frm = new PropertiesForm();
            frm.myPropertyGrid.SelectedObjects = selectedObjects;
            frm.Show();
        }
    }
}