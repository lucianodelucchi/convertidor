// -----------------------------------------------------------------------------
//  <copyright file="ImageViewModel.cs" company="">
//      Copyright (c) 
//  </copyright>
// -----------------------------------------------------------------------------
namespace Convertidor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    
    using Convertidor.Infrastructure;
    using Convertidor.Models;
    using Convertidor.Services;
    
    /// <summary>
    /// Description of ImageViewModel.
    /// </summary>
    public class ImageViewModel : ViewModel
    {
        private ObservableCollection<OwnImage> images;
        private ObservableCollection<OwnImage> processedImages;
        private ImageConverterService converterService;
        private RelayCommand convertImagesCommand;
        private RelayCommand cancelConvertImagesCommand;
        private CancellationTokenSource cts;
        
        #region Constructor
        public ImageViewModel()
        {
            this.converterService = new ImageConverterService();
            
            this.images = new ObservableCollection<OwnImage>();
            this.processedImages = new ObservableCollection<OwnImage>();
        }
        #endregion
        
        #region Property Getters and Setters
        public ObservableCollection<OwnImage> Images
        {
            get { return this.images; }
            set
            {
                this.images = value;
                this.NotifyPropertyChanged();
                // Notify that IsProgressBarVisible so it's reevaluated
                this.NotifyPropertyChanged("IsProgressBarVisible");
            }
        }
        
        public bool IsProgressBarVisible
        {
            get { return this.Images.Count() > 0; }
        }
        
        public ObservableCollection<OwnImage> ProcessedImages 
        {
            get { return this.processedImages; }
            set 
            { 
                this.processedImages = value;
                // Notify that ProcessedImagesProgress so it's reevaluated
                this.NotifyPropertyChanged("ProcessedImagesProgress");
            }
        }

        public int ProcessedImagesProgress
        {
            get
            {
                float total = 0;
                
                if (this.Images.Count > 0)
                {
                    total = (float)this.ProcessedImages.Count * 100 / this.Images.Count;
                }
                
                return (int)Math.Round(total, MidpointRounding.AwayFromZero);
            }
        }
        
        /// <summary>
        /// Returns a command that converts the images.
        /// </summary>
        public ICommand ConvertImagesCommand
        {
            get
            {
                if (this.convertImagesCommand == null) 
                {
                    this.convertImagesCommand = new RelayCommand(
                                                        param => this.ConvertImagesAsync(),
                                                        param => this.CanConvertImages);
                }
                
                return this.convertImagesCommand;
            }
        }
                
        /// <summary>
        /// Returns a command that cancel the conversion of the images.
        /// </summary>
        public ICommand CancelConvertImagesCommand
        {
            get
            {
                if (this.cancelConvertImagesCommand == null) 
                {
                    this.cancelConvertImagesCommand = new RelayCommand(
                                                            param => this.CancelConvertImages(),
                                                            param => this.CanCancelConvertImages);
                }
                
                return this.cancelConvertImagesCommand;
            }
        }
        
        /// <summary>
        /// Returns true if the images can be converted.
        /// </summary>
        private bool CanConvertImages
        {
            get { return this.Images.Count > 0; }
        }
                           
        /// <summary>
        /// Returns true if the conversion process can be cancelled.
        /// </summary>
        private bool CanCancelConvertImages
        {
            get
            {
                return this.ProcessedImages.Count > 0 && this.Images.Count > 0;
            }
        }
       
        #endregion
          
        #region Methods
        public void LoadImages(string[] draggedFolders)
        {
            var currentImages = this.Images;
            
            this.Images = null;
            
            foreach (var directory in draggedFolders)
            {
                foreach (FileSystemInfo fileSystemInfo in new DirectoryInfo(directory).GetFiles("*.jpg"))
                {
                    currentImages.Add(
                        OwnImage.CreateOwnImage(
                            fileSystemInfo.Name,
                            fileSystemInfo.FullName,
                            Path.Combine(directory, "compressed", fileSystemInfo.Name))); // compressed could be configurable
                }
            }
            
            this.Images = currentImages;
        }
        
        private void CancelConvertImages()
        {
            if (this.cts != null) 
            {
                this.cts.Cancel();
            }
        }
        
        private void ConvertImages()
        {
            this.converterService.ConvertImages(this.Images);
        }
        
        private async void ConvertImagesAsync()
        {
            // construct Progress<T>, passing ReportProgress as the Action<T>
            var progressIndicator = new Progress<OwnImage>(this.ReportProgress);
            
            this.cts = new CancellationTokenSource();
            this.cts.Token.Register(() => this.ConversionCancelled());
            
            var result = await this.converterService.ConvertImagesAsync(this.Images, progressIndicator, this.cts.Token);

            this.ConversionFinished(result);
        }
        
        private void ConversionCancelled()
        {
            this.Clear();
            MessageBox.Show("Process Cancelled.");
        }
        
        private void ConversionFinished(bool successful)
        {
            if (successful) 
            {
                this.Clear();
                MessageBox.Show("Finished");
            }
        }
        
        /// <summary>
        /// UI clean up
        /// </summary>
        private void Clear()
        {
            this.Images = new ObservableCollection<OwnImage>(this.Images.Except(this.ProcessedImages));
            this.ProcessedImages = null;
            this.ProcessedImages = new ObservableCollection<OwnImage>();
        }
        
        private void ReportProgress(OwnImage value)
        {
            if (!this.cts.IsCancellationRequested) 
            {
                var processed = this.ProcessedImages;
                processed.Add(value);
                this.ProcessedImages = processed;
            }
        }

        #endregion
    }
}
