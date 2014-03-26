// -----------------------------------------------------------------------------
//  <copyright file="OwnImage.cs" company="">
//      Copyright (c) 
//  </copyright>
// -----------------------------------------------------------------------------
namespace Convertidor.Models
{
    using System;
    
    /// <summary>
    /// Description of Image.
    /// </summary>
    public class OwnImage
    {
        protected OwnImage()
        {
        }
        
        #region Properties

        /// <summary>
        /// Gets or sets the filename of the image.
        /// </summary>
        public string FileName { get; set; }    
         
        /// <summary>
        /// Gets or sets the full path of the image.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the full path of where to save the converted image.
        /// </summary>
        public string SaveAs { get; set; }
        
        #endregion // Properties
        
        #region Creation
        public static OwnImage CreateNewOwnImage()
        {
            return new OwnImage();
        }

        public static OwnImage CreateOwnImage(string filename, string path, string saveAs)
        {
            return new OwnImage
            {
                Path = path,
                SaveAs = saveAs,
                FileName = filename
            };
        }

        #endregion // Creation
    }
}
