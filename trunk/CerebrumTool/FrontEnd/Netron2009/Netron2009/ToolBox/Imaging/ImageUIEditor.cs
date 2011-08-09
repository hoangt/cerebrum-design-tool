using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Drawing;

namespace ToolBox.Imaging
{
	// ----------------------------------------------------------------------
	/// <summary>
	/// ImageUIEditor is a custom editor that allows the properties of an
	/// Image to be edited from a PropertyGrid.
	/// </summary>
	// ----------------------------------------------------------------------
	public class ImageUIEditor : System.Drawing.Design.UITypeEditor
	{
		// ------------------------------------------------------------------
		/// <summary>
		/// Gets the edit style
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		// ------------------------------------------------------------------
		public override UITypeEditorEditStyle GetEditStyle(
			ITypeDescriptorContext context)
		{
			if (context != null)
			{
				return UITypeEditorEditStyle.Modal;
			}
			else
			{
				return UITypeEditorEditStyle.None;
			}
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Edits tyhe value
		/// </summary>
		/// <param name="context">ITypeDescriptorContext</param>
		/// <param name="provider">IServiceProvider</param>
		/// <param name="value">object</param>
		/// <returns></returns>
		// ------------------------------------------------------------------
		[RefreshProperties(RefreshProperties.All)]    
		public override object EditValue(ITypeDescriptorContext context, 
			IServiceProvider provider, object value)
		{   
			ImageEditorDialog editor = null;
       
			if ( (context == null) || 
				(provider == null) || 
				(context.Instance == null) )
			{
				return base.EditValue(provider, value);
			}
			else
			{
				if (value is Image)
				{
					Image image = (Image) value;
					editor = new ImageEditorDialog( (Image) image.Clone());
					if (editor.ShowDialog() == DialogResult.OK)
					{
						value = editor.Image;
					}
				}
          
				return value;
			}
		}
	}
}
