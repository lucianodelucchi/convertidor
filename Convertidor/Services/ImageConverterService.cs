// -----------------------------------------------------------------------------
//  <copyright file="ImageConverterService.cs" company="">
//      Copyright (c) 
//  </copyright>
// -----------------------------------------------------------------------------
namespace Convertidor.Services
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    
    using Convertidor.Models;
    using System.Reactive.Linq;

    /// <summary>
    /// Description of ImageConverterService.
    /// </summary>
    public class ImageConverterService
    {
        /// <summary>
        /// </summary>
        private static ImageCodecInfo jpegCodecInfo;
        
        /// <summary>
        /// </summary>
        private static EncoderParameters myEncoderParameters;
        
        /// <summary>
        /// </summary>
        public ImageConverterService()
        {
            jpegCodecInfo = this.GetEncoder(ImageFormat.Jpeg);
            
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
        
        /// <summary>
        /// </summary>
        /// <param name="images"></param>
        public void ConvertImages(List<OwnImage> images)
        {
            foreach (var image in images)
            {
                this.ConvertImage(image);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="images"></param>
        /// <returns></returns>
        public IObservable<OwnImage> ConvertImages(IEnumerable<OwnImage> images, CancellationToken ct)
        {
            return images.ToObservable().SelectMany(image => Observable.FromAsync(() => this.ConvertImageAsync(image, ct)));
        }

        /// <summary>
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid) return codec;
            }
            
            return null;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="image"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private Task<OwnImage> ConvertImageAsync(OwnImage image, CancellationToken ct)
        {
            return Task.Run(() => this.ConvertImage(image), ct);
        }
        
        /// <summary>
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
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
                {
                    newWidth = fullsizeImage.Width;
                }
                
                int thumbHeight = fullsizeImage.Height * newWidth / fullsizeImage.Width;
                if (thumbHeight > maxHeight) 
                {
                    newWidth = fullsizeImage.Width * maxHeight / fullsizeImage.Height;
                    thumbHeight = maxHeight;
                }
                
                fullsizeImage.GetThumbnailImage(newWidth, thumbHeight, null, IntPtr.Zero)
                    .Save(image.SaveAs, jpegCodecInfo, myEncoderParameters);
            }
        
            return image;
        }
    }
}
