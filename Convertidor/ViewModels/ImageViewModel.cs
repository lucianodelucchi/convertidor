using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
		
		private IEnumerable<Image> images = new List<Image>();
		private ImageConverterService converterService;
		private RelayCommand convertImagesCommand;
		
		
		public ImageViewModel()
		{
			this.converterService = new ImageConverterService();
		}
		
		public IEnumerable<Image> Images
		{
			get { return this.images; }
			set 
			{
				this.images = value;
				this.NotifyPropertyChanged();
			}
		}
	
		public void LoadImages(string[] draggedFolders)
		{
			var imgs = new List<Image>();
			
			foreach (var directory in draggedFolders) 
				foreach (FileSystemInfo fileSystemInfo in new DirectoryInfo(directory).GetFiles("*.jpg"))
					imgs.Add(Image.CreateImage(
												fileSystemInfo.Name,
												fileSystemInfo.FullName,
			                            	 	Path.Combine(directory, "compressed", fileSystemInfo.Name)
					                            ));//compressed could be configurable
			
			this.Images = imgs;
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
                        param => this.ConvertImages(),
                        param => this.CanConvertImages
                        );
                }
                return convertImagesCommand;
            }
        }
		
        private void ConvertImages()
        {
        	this.converterService.ConvertImages(this.Images);
        }
        
        /// <summary>
        /// Returns true if the images can be converted.
        /// </summary>
        bool CanConvertImages
        {
        	get { return this.Images.Count() > 0; }
        }
	}
}
