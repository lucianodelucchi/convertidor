using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using System.Drawing.Imaging;
using System.Drawing;

namespace Convertidor
{

	public partial class MainWindow : Window
	{
		private string directory;
	    private System.Drawing.Image FullsizeImage;
	    private System.Drawing.Image NewImage;
		private static ImageCodecInfo jpegCodecInfo;
		private static EncoderParameters myEncoderParameters;
		
		public MainWindow()
		{
			InitializeComponent();
			jpegCodecInfo = GetEncoder(ImageFormat.Jpeg);
			
			// Create an Encoder object based on the GUID
			// for the Quality parameter category.
			System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

			// Create an EncoderParameters object.
			// An EncoderParameters object has an array of EncoderParameter
			// objects. In this case, there is only one
			// EncoderParameter object in the array.
			EncoderParameters myEncoderParameters = new EncoderParameters(1);

			EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 75L);
			myEncoderParameters.Param[0] = myEncoderParameter;
		}
		
		void Droparea_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			    e.Effects = DragDropEffects.All;
		    else
		    	e.Effects = DragDropEffects.None;
			    	
		}
		
		void Droparea_Drop(object sender, DragEventArgs e)
		{
			string[] strArray = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
	      	
//			this.lstFiles.Items.Clear();
//	      	this.directory = strArray[0];
//	      	foreach (FileSystemInfo fileSystemInfo in new DirectoryInfo(strArray[0]).GetFiles("*.jpg"))
//	        	this.lstFiles.Items.Add((object) fileSystemInfo.Name);
//	      	this.pbStatus.Maximum = this.lstFiles.Items.Count;
//	      	this.btnCompress.Enabled = this.lstFiles.Items.Count > 0;
		}
		
		void Compress_Click(object sender, RoutedEventArgs e)
		{
//			string str = Path.Combine(this.directory, "comprimidas");
//			DirectoryInfo directory = Directory.CreateDirectory(str);
//			this.Cursor = Cursors.WaitCursor;
//			foreach (string path2 in this.lstFiles.Items)
//			{
//				string NewFile = Path.Combine(directory.FullName, path2);
//				this.ResizeImage(Path.Combine(this.directory, path2), NewFile, 800, 600, true);
//				this.pbStatus.PerformStep();
//				Application.DoEvents();
//			}
//			this.pbStatus.Value = 0;
//			this.Cursor = Cursors.Default;
			
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
		
		public void ResizeImage(string OriginalFile, string NewFile, int NewWidth, int MaxHeight, bool OnlyResizeIfWider)
		{
			this.FullsizeImage = System.Drawing.Image.FromFile(OriginalFile);
			this.FullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
			this.FullsizeImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
			if (OnlyResizeIfWider && this.FullsizeImage.Width <= NewWidth)
				NewWidth = this.FullsizeImage.Width;
			int thumbHeight = this.FullsizeImage.Height * NewWidth / this.FullsizeImage.Width;
			if (thumbHeight > MaxHeight)
			{
				NewWidth = this.FullsizeImage.Width * MaxHeight / this.FullsizeImage.Height;
				thumbHeight = MaxHeight;
			}
			this.NewImage = this.FullsizeImage.GetThumbnailImage(NewWidth, thumbHeight, (System.Drawing.Image.GetThumbnailImageAbort) null, IntPtr.Zero);
			this.FullsizeImage.Dispose();
			this.NewImage.Save(NewFile, jpegCodecInfo, myEncoderParameters);
		}
		

	}
}