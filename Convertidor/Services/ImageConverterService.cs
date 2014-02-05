using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Convertidor.Models;

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
		
		public void ConvertImages(IEnumerable<OwnImage> images)
		{
			foreach (var image in images)
				this.ConvertImage(image);
		}
		
		public async Task<bool> ConvertImagesAsync(IEnumerable<OwnImage> images, IProgress<OwnImage> progress, CancellationToken ct)
		{
			var tasks = new List<Task<OwnImage>>();
			
			images.ToList().ForEach(image => tasks.Add(this.ConvertImageAsync(image, ct)));
			
			foreach(var bucket in Interleaved(tasks))
			{
				var t = await bucket;
				try
				{
					var image = await t.ConfigureAwait(false);
					
					if (progress != null) {
						progress.Report(image);
					}
					
					ct.ThrowIfCancellationRequested();
				}
				catch(OperationCanceledException) { return false; }
				catch(Exception exc) { throw exc; }
			}
			
			return true;
		}
		
		private Task<OwnImage> ConvertImageAsync(OwnImage image, CancellationToken ct)
		{
			return Task.Run(() => this.ConvertImage(image), ct);
		}
		
		private OwnImage ConvertImage(OwnImage image)
		{
			var onlyResizeIfWider = true;
			var newWidth = 800;
			var maxHeight = 600;
			
			var destinationImageFile = new FileInfo(image.SaveAs);
			Directory.CreateDirectory(destinationImageFile.DirectoryName);
			
			using (var fullsizeImage = System.Drawing.Image.FromFile(image.Path)) 
			{
				
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
			}
		
			return image;

		}
		
		public static Task<Task<T>> [] Interleaved<T>(IEnumerable<Task<T>> tasks)
		{
			var inputTasks = tasks.ToList();
			
			var buckets = new TaskCompletionSource<Task<T>>[inputTasks.Count];
			var results = new Task<Task<T>>[buckets.Length];
			for (int i = 0; i < buckets.Length; i++)
			{
				buckets[i] = new TaskCompletionSource<Task<T>>();
				results[i] = buckets[i].Task;
			}
			
			int nextTaskIndex = -1;
			Action<Task<T>> continuation = completed =>
			{
				var bucket = buckets[Interlocked.Increment(ref nextTaskIndex)];
				bucket.TrySetResult(completed);
			};
			
			foreach (var inputTask in inputTasks)
				inputTask.ContinueWith(continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
			
			return results;
		}
	}
}
