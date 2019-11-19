using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupNStegafy.IO
{
    public class FileWriter
    {
        public async void SaveWritableBitmap(WriteableBitmap image, double dpiX, double dpiY)
        {
            var fileSavePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };
            fileSavePicker.FileTypeChoices.Add("PNG files", new List<string> { ".png" });
            var saveFile = await fileSavePicker.PickSaveFileAsync();

            if (saveFile != null)
            {
                var stream = await saveFile.OpenAsync(FileAccessMode.ReadWrite);
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                var pixelStream = image.PixelBuffer.AsStream();
                var pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);

                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                    (uint)image.PixelWidth,
                    (uint)image.PixelHeight, dpiX, dpiY, pixels);
                await encoder.FlushAsync();

                stream.Dispose();
            }
        }
    }
}
