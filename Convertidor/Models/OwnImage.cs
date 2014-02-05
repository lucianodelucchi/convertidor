using System;

namespace Convertidor.Models
{
	/// <summary>
	/// Description of Image.
	/// </summary>
	public class OwnImage
	{
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

        protected OwnImage()
        {
        }

        #endregion // Creation
	
     	#region Properties

     	/// <summary>
        /// Gets/sets the filename of the image.
        /// </summary>
        public string FileName { get; set; }	
     	
        /// <summary>
        /// Gets/sets the full path of the image.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets/sets the full path of where to save the converted image.
        /// </summary>
        public string SaveAs { get; set; }
        
        #endregion // Properties

	}
}
