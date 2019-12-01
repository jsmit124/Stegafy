using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.Constants;

namespace GroupNStegafy.IO
{
    /// <summary>
    ///     Stores information for the file writer class
    /// </summary>
    public class FileWriter
    {
        #region Methods

        /// <summary>
        ///     Saves the writable bitmap.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="dpiX">The dpi x.</param>
        /// <param name="dpiY">The dpi y.</param>
        public async void SaveWritableBitmap(WriteableBitmap image, double dpiX, double dpiY)
        {
            var fileSavePicker = new FileSavePicker {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };
            fileSavePicker.FileTypeChoices.Add("PNG files", new List<string> {FileTypeConstants.PortableNetworkImage});
            var saveFile = await fileSavePicker.PickSaveFileAsync();

            if (saveFile != null)
            {
                var stream = await saveFile.OpenAsync(FileAccessMode.ReadWrite);
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                var pixelStream = image.PixelBuffer.AsStream();
                var pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                    (uint) image.PixelWidth,
                    (uint) image.PixelHeight, dpiX, dpiY, pixels);
                await encoder.FlushAsync();

                stream.Dispose();
            }
        }

        #endregion
    }
}