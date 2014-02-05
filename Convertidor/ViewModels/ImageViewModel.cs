using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Convertidor.Infrastructure;
using Convertidor.Models;
using Convertidor.Services;

namespace Convertidor.ViewModels
{
	/// <summary>
	/// Description of ImageViewModel.
	/// </summary>
	public class ImageViewModel : ViewModel
	{
		
		private ObservableCollection<OwnImage> images = new ObservableCollection<OwnImage>();
		private ObservableCollection<OwnImage> processedImages = new ObservableCollection<OwnImage>();
		private ImageConverterService converterService;
		private RelayCommand convertImagesCommand;
		private RelayCommand cancelConvertImagesCommand;
		private CancellationTokenSource cts;
		
		public ImageViewModel()
		{
			this.converterService = new ImageConverterService();
			
			this.Images.CollectionChanged += (s, e) => this.NotifyPropertyChanged("ProgressBarVisibility");
		}
		
		public ObservableCollection<OwnImage>Images
		{
			get { return this.images; }
			set { this.images = value; }
		}
		
		public Visibility ProgressBarVisibility
		{
			get { return this.Images.Count() > 0 ? Visibility.Visible : Visibility.Collapsed; }
		}
		
		public ObservableCollection<OwnImage> ProcessedImages {
			get { return processedImages; }
			set { processedImages = value; }
		}
		
		public void LoadImages(string[] draggedFolders)
		{
		
			foreach (var directory in draggedFolders)
				foreach (FileSystemInfo fileSystemInfo in new DirectoryInfo(directory).GetFiles("*.jpg"))
					this.Images.Add(OwnImage.CreateOwnImage(
						fileSystemInfo.Name,
						fileSystemInfo.FullName,
						Path.Combine(directory, "compressed", fileSystemInfo.Name)
					));//compressed could be configurable
			
		}
		
		/// <summary>
		/// Returns a command that converts the images.
		/// </summary>
		public ICommand ConvertImagesCommand
		{
			get
			{
				if (convertImagesCommand == null)
				{
					convertImagesCommand = new RelayCommand(
						param => this.ConvertImagesAsync(),
						param => this.CanConvertImages
					);
				}
				return convertImagesCommand;
			}
		}
		
		/// <summary>
		/// Returns a command that cancel the conversion of the images.
		/// </summary>
		public ICommand CancelConvertImagesCommand
		{
			get
			{
				if (cancelConvertImagesCommand == null)
				{
					cancelConvertImagesCommand = new RelayCommand(
						param => this.CancelConvertImages(),
						param => this.CanCancelConvertImages
					);
				}
				return cancelConvertImagesCommand;
			}
		}
		
		private void ConvertImages()
		{
			this.converterService.ConvertImages(this.Images);
		}
		
		private void CancelConvertImages()
		{
			if (this.cts != null) {
				this.cts.Cancel();
			}
		}
		
		private void ConvertImagesAsync()
		{
			//construct Progress<T>, passing ReportProgress as the Action<T>
			var progressIndicator = new Progress<OwnImage>(ReportProgress);
			
			this.cts = new CancellationTokenSource();
			cts.Token.Register(() => this.ConversionCancelled());
			
			this.converterService.ConvertImagesAsync(this.Images, progressIndicator, cts.Token).ContinueWith(param => this.ConversionFinished(param.Result));
		}
		
		private void ConversionCancelled()
		{
			MessageBox.Show("Process Cancelled.");
		}
		
		
		private void ConversionFinished(bool successful)
		{
			if (successful) {
				MessageBox.Show("Process Completed");
				this.Images.Clear();
			}
			
		}
		
		/// <summary>
		/// Returns true if the images can be converted.
		/// </summary>
		bool CanConvertImages
		{
			get { return this.Images.Count() > 0; }
		}
		
		/// <summary>
		/// Returns true if the conversion process can be cancelled.
		/// </summary>
		bool CanCancelConvertImages
		{
			get 
			{
				return (this.ProcessedImages.Count > 0 && this.Images.Count > 0);
			}
		}
		
		void ReportProgress(OwnImage value)
		{
			this.ProcessedImages.Add(value);
			//this.Images.Remove(value);
		}
	}
}
