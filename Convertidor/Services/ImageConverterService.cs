using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Convertidor.Services
{
	/// <summary>
	/// Description of ImageConverterService.
	/// </summary>
	public class ImageConverterService
	{
		
		private static ImageCodecInfo jpegCodecInfo;
		private static EncoderParameters myEncoderParameters;
		
		public ImageConverterService()
		{
			jpegCodecInfo = GetEncoder(ImageFormat.Jpeg);
			
			// Create an Encoder object based on the GUID
			// for the Quality parameter category.
			System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

			// Create an EncoderParameters object.
			// An EncoderParameters object has an array of EncoderParameter
			// objects. In this case, there is only one
			// EncoderParameter object in the array.
			myEncoderParameters = new EncoderParameters(1);

			EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 75L);
			myEncoderParameters.Param[0] = myEncoderParameter;
		}
		
		private ImageCodecInfo GetEncoder(ImageFormat format)
		{

			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
				{
					return codec;
				}
			}
			return null;
		}
		
		public void ConvertImages(IEnumerable<Convertidor.Models.Image> images)
		{
			foreach (var image in images)
				this.ConvertImage(image);
		}
		
		private void ConvertImage(Convertidor.Models.Image image)
		{
			var onlyResizeIfWider = true;
			var newWidth = 800;
			var maxHeight = 600;
			
			var destinationImageFile = new FileInfo(image.SaveAs);
			Directory.CreateDirectory(destinationImageFile.DirectoryName);
			
			var fullsizeImage = System.Drawing.Image.FromFile(image.Path);
			
			fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
			fullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
			
			if (onlyResizeIfWider && fullsizeImage.Width <= newWidth)
				newWidth = fullsizeImage.Width;
			
			int thumbHeight = fullsizeImage.Height * newWidth / fullsizeImage.Width;
			if (thumbHeight > maxHeight)
			{
				newWidth = fullsizeImage.Width * maxHeight / fullsizeImage.Height;
				thumbHeight = maxHeight;
			}
			
			fullsizeImage.GetThumbnailImage(newWidth, thumbHeight, (System.Drawing.Image.GetThumbnailImageAbort) null, IntPtr.Zero)
						.Save(image.SaveAs, jpegCodecInfo, myEncoderParameters);
			
			fullsizeImage.Dispose();

		}
	}
}
