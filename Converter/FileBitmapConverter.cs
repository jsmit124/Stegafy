using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupNStegafy.Converter
{
    /// <summary>
    ///     Stores methods for converting between files and bitmap images
    /// </summary>
    public static class FileBitmapConverter
    {
        #region Methods

        /// <summary>
        ///     Converts the file to bitmap.
        /// </summary>
        /// <param name="imageFile">The image file.</param>
        /// <returns>The bitmap image stored in the storage file</returns>
        public static async Task<BitmapImage> ConvertFileToBitmap(StorageFile imageFile)
        {
            IRandomAccessStream inputStream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputStream);

            return newImage;
        }

        #endregion
    }
}