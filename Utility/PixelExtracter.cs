using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using GroupNStegafy.Converter;

namespace GroupNStegafy.Utility
{
    /// <summary>
    ///     Stores methods for extracting pixels from objects
    /// </summary>
    public static class PixelExtracter
    {
        #region Methods

        /// <summary>
        ///     Extracts the pixel data from file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>The byte array of pixel information from the file</returns>
        public static async Task<byte[]> ExtractPixelDataFromFile(StorageFile file)
        {
            var copyBitmapImage = await FileBitmapConverter.ConvertFileToBitmap(file);

            using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                var transform = new BitmapTransform {
                    ScaledWidth = Convert.ToUInt32(copyBitmapImage.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(copyBitmapImage.PixelHeight)
                };

                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage
                );

                return pixelData.DetachPixelData();
            }
        }

        #endregion
    }
}