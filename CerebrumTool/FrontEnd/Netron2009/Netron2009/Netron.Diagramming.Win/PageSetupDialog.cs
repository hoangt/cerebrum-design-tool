using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Netron.Diagramming.Win
{
    public partial class PageSetupDialog : Form
    {
        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the text from the TextBox that displays the
        /// name of a page.
        /// </summary>
        // ------------------------------------------------------------------
        public string PageName
        {
            get
            {
                return myPageNameTextBox.Text;
            }
            set
            {
                myPageNameTextBox.Text = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the color from the ComboBoxColorPicker that displays 
        /// the page's color.
        /// </summary>
        // ------------------------------------------------------------------
        public Color PageColor
        {
            get
            {
                return myPageColorPicker.Color;
            }
            set
            {
                myPageColorPicker.Color = value;
            }
        }

        #endregion

        #region Constructors

        // ------------------------------------------------------------------
        /// <summary>
        /// Default constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public PageSetupDialog()
        {
            InitializeComponent();
        }

        #endregion
    }
}