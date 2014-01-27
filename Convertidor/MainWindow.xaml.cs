using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

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
	      	
			this.ImagesListBox.Items.Clear();
	      	this.directory = strArray[0];
	      	foreach (FileSystemInfo fileSystemInfo in new DirectoryInfo(strArray[0]).GetFiles("*.jpg"))
	        	this.ImagesListBox.Items.Add((object) fileSystemInfo.Name);
	      	this.ConvertidorProgress.Maximum = this.ImagesListBox.Items.Count;
	      	this.CompressButton.IsEnabled = this.ImagesListBox.Items.Count > 0;
		}
		
		void Compress_Click(object sender, RoutedEventArgs e)
		{
			string str = Path.Combine(this.directory, "compressed");
			DirectoryInfo directory = Directory.CreateDirectory(str);
			
			var cursor = this.Cursor;
			this.Cursor = Cursors.Wait;
			
			var progress = 1;
			
			foreach (string path in this.ImagesListBox.Items)
			{
				string newFile = Path.Combine(directory.FullName, path);
				this.ResizeImage(Path.Combine(this.directory, path), newFile, 800, 600, true);

				UiInvoke(() => this.ConvertidorProgress.Value = progress++);
			}
			MessageBox.Show(this, "Conversion Completed", "Process Completed", MessageBoxButton.OK, MessageBoxImage.Information);
			this.ConvertidorProgress.Value = 0;
			this.Cursor = cursor;
			
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
		
		public static void UiInvoke(Action a)
		{
			Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, a);
		}

	}
}