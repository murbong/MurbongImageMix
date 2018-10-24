using System;
using System.Drawing;
using System.IO;

namespace Palc.Imaging
{

	/// <summary>
	/// Represents an image of any format
	/// </summary>
	public class RawImage : ICloneable
	{
		protected byte[] RawImageData { get; set; }

		/// <summary>
		/// Create a new raw image out of a Image-Object
		/// </summary>
		/// <param name="imagesource"></param>
		public RawImage(Image imagesource)
		{
			if (imagesource == null)
			{
				throw new ArgumentNullException("imagesource", "No valid Image");
			}

			LoadFromImage(imagesource);
		}

		/// <summary>
		/// Reads image data from an image stream
		/// </summary>
		/// <param name="imagesource"></param>
		public void LoadFromImage(Image imagesource)
		{
			if (imagesource == null)
			{
				throw new ArgumentNullException("imagesource", "No valid Image");
			}

			MemoryStream imagestream = new MemoryStream();
			imagesource.Save(imagestream, imagesource.RawFormat);
			RawImageData = imagestream.ToArray();

			checkImageData();
		}

		/// <summary>
		/// Create a new raw image out of a file
		/// </summary>
		/// <param name="sourcefilename"></param>
		public RawImage(string sourcefilename)
		{
			LoadFromFile(sourcefilename);
		}

		/// <summary>
		/// Reads image dara from a file
		/// </summary>
		/// <param name="filename"></param>
		public void LoadFromFile(string filename)
		{
			if (File.Exists(filename))
			{
				RawImageData = File.ReadAllBytes(filename);
			}

			checkImageData();
		}

		/// <summary>
		/// Check if loaded data is present
		/// </summary>
		private void checkImageData()
		{
			if (RawImageData == null)
			{
				throw new System.ArgumentException("The image data was invalid or could not be opened");
			}
		}

		/// <summary>
		/// Save image data to a file
		/// </summary>
		/// <param name="filename"></param>
		public void Save(string filename)
		{
			File.WriteAllBytes(filename, RawImageData);
		}


		#region ICloneable Member

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion



	}
}
