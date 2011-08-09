using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Collections;

namespace ToolBox
{
    public class DataGridViewColorColumn : DataGridViewComboBoxColumn
    {
        ArrayList colorNames;

        // ------------------------------------------------------------------
        /// <summary>
        /// Returns the names of all colors that are used to populate
        /// the columns combo box.
        /// </summary>
        // ------------------------------------------------------------------
        public string[] ColorNames
        {
            get
            {
                return (string[]) this.colorNames.ToArray(typeof(string));
            }
        }

        // ------------------------------------------------------------------
        /// <summary>
        /// Constructor.
        /// </summary>
        // ------------------------------------------------------------------
        public DataGridViewColorColumn()
            : base()
        {
            // Get all colors that exist in System.Drawing.Color.  This beats
            // manually adding each color to the array!
            colorNames = new ArrayList();
            Type colorType = typeof(Color);
            PropertyInfo[] propInfoList =
                 colorType.GetProperties(
                 BindingFlags.Static | 
                 BindingFlags.DeclaredOnly | 
                 BindingFlags.Public);

            int numOfProperties = propInfoList.Length;
            for (int i = 0; i < numOfProperties; i++)
            {
                PropertyInfo propInfo = (PropertyInfo)propInfoList[i];
                Color color = (Color)propInfo.GetValue(null, null);
                colorNames.Add(color.Name);
            }

            foreach (string color in colorNames)
            {
                this.Items.Add(color);
            }
        }
    }
}
