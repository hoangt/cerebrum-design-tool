using System;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Collections;

namespace ToolBox.Imaging
{
	// ----------------------------------------------------------------------
	/// <summary>
	/// An collection of images suitable for serialization.
	/// </summary>
	// ----------------------------------------------------------------------
	[Serializable]
	public class ImageCollection : CollectionBase
	{
		// ------------------------------------------------------------------
		/// <summary>
		/// Constructor.
		/// </summary>
		// ------------------------------------------------------------------
		public ImageCollection()
		{				
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Adds the specified Image to the collection.
		/// </summary>
		/// <param name="image">Image: Any image object.</param>
		/// <returns>int: The number of images in the collection.</returns>
		// ------------------------------------------------------------------
		public int Add(Image image)
		{
			this.List.Add(image);

			return this.List.Count;
		}

		// ------------------------------------------------------------------
		/// <summary>
		/// Gets the Image at the specified index.
		/// </summary>
		// ------------------------------------------------------------------
		public Image this[int index]
		{
			get
			{
				return (Image) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		#region Serialization

		// ------------------------------------------------------------------
		/// <summary>
		/// Serializes the data to an XML file.
		/// </summary>
		/// <param name="fileName">(string) The name of the file</param>
		// ------------------------------------------------------------------
		public void SerializeToXMLFile(string fileName)
		{
			FileStream fs = new FileStream(fileName, FileMode.Create);	
			XmlSerializer x = new XmlSerializer(this.GetType());
			x.Serialize(fs, this);
			fs.Close();
		}

		// ------------------------------------------------------------------
		// ------------------------------------------------------------------
		public void SerializeToBinaryFile(string fileName)
		{
			FileStream fs = new FileStream(fileName, FileMode.Create);
			BinaryFormatter bf = new BinaryFormatter();
			bf.Serialize(fs, this);
			fs.Close();
		}

		// ------------------------------------------------------------------
		// ------------------------------------------------------------------
		public static ImageCollection DeserializeFromXml(string fileName)
		{		
			ImageCollection newImageCollection = new ImageCollection();
			FileStream fs = new FileStream(fileName, FileMode.Open);
			XmlSerializer x = new XmlSerializer(typeof(ImageCollection));
			newImageCollection = (ImageCollection) x.Deserialize(fs);
			fs.Close();
			return newImageCollection;
		}

		// ------------------------------------------------------------------
		// ------------------------------------------------------------------
		public static ImageCollection DeserializeFromBinary(string fileName)
		{
			ImageCollection newImageCollection = new ImageCollection();
			FileStream fs = new FileStream(fileName, FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();
			newImageCollection = (ImageCollection) bf.Deserialize(fs);	
			fs.Close();
			return newImageCollection;
		}

		#endregion
	}
}
