using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace ToolBox.Controls
{
    // ----------------------------------------------------------------------
    /// <summary>
    /// A System.Windows.Forms TextBox that displays the name of a Font
    /// family and allows the user to manually enter a new Font Family.
    /// If the user-entered name is invalid then the forecolor of the TextBox
    /// is changed to InValidFontFamilyColor to indicate it is an invalid 
    /// font name.  The forecolor is set to ValidFontFamilyColor if a valid
    /// Font Family name is entered.
    /// </summary>
    // ----------------------------------------------------------------------
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(TextBox))]
    public class FontFamilyTextBox : TextBox
    {
        #region Fields

        // ------------------------------------------------------------------
        /// <summary>
        /// The FontFamily to edit/display.
        /// </summary>
        // ------------------------------------------------------------------
        FontFamily fontFamily = new FontFamily("Arial");

        // ------------------------------------------------------------------
        /// <summary>
        /// The color to set the ForeColor to when a valid FontFamily name is
        /// entered.
        /// </summary>
        // ------------------------------------------------------------------
        Color validFontFamilyColor = Color.Green;

        // ------------------------------------------------------------------
        /// <summary>
        /// The color to set the ForeColor to when an invalid FontFamily name 
        /// is entered.
        /// </summary>
        // ------------------------------------------------------------------
        Color invalidFontFamilyColor = Color.Red;

        #endregion

        #region Properties

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the FontFamily to display/edit the name of.
        /// </summary>
        // ------------------------------------------------------------------
        public FontFamily FontFamily
        {
            get
            {
                return this.fontFamily;
            }
            set
            {
                this.fontFamily = value;
                this.Text = this.fontFamily.Name;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the color to set the ForeColor to when a valid 
        /// FontFamily name is entered.
        /// </summary>
        // ------------------------------------------------------------------
        public Color ValidFontFamilyColor
        {
            get
            {
                return this.validFontFamilyColor;
            }
            set
            {
                this.validFontFamilyColor = value;
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the color to set the ForeColor to when an invalid 
        /// FontFamily name is entered.
        /// </summary>
        // ------------------------------------------------------------------
        public Color InValidFontFamilyColor
        {
            get
            {
                return this.invalidFontFamilyColor;
            }
            set
            {
                this.invalidFontFamilyColor = value;
            }
        }

        #endregion

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public FontFamilyTextBox() : base()
        {
            this.FontFamily = new FontFamily("Arial");
            this.TextChanged += 
                new EventHandler(FontFamilyTextBox_TextChanged);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor - receives the FontFamily to display/edit.
        /// </summary>
        /// <param name="fontFamily">FontFamily</param>
        // ------------------------------------------------------------------
        public FontFamilyTextBox(FontFamily fontFamily)
        {
            // Use the property to set the supplied font family so the Text
            // property in this TextBox gets set.
            this.FontFamily = fontFamily;
            this.TextChanged += 
                new EventHandler(FontFamilyTextBox_TextChanged);
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Searches through all FontFamilies and returns if a match is found.
        /// </summary>
        /// <param name="name">string: The name of the FontFamily.</param>
        /// <returns>bool: True if successful, false otherwise.</returns>
        // ------------------------------------------------------------------
        public bool IsFontFamilyNameValid(string name)
        {
            bool answer = false;
            foreach (FontFamily family in FontFamily.Families)
            {
                if (name == family.Name)
                {
                    answer = true;
                    break;
                }
            }
            return answer;
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Verify the font family name being entered is a valid name.  If it
        /// is, set the ForeColor to green and create a new FontFamily based
        /// on the new name.  Otherwise set the ForeColor to red.
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        // ------------------------------------------------------------------
        void FontFamilyTextBox_TextChanged(object sender, EventArgs e)
        {
            if (this.IsFontFamilyNameValid(this.Text) == true)
            {
                this.ForeColor = this.validFontFamilyColor;
                this.fontFamily = new FontFamily(this.Text);
            }
            else
            {
                this.ForeColor = this.invalidFontFamilyColor;
            }
        }
    }
}
