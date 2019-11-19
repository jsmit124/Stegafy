using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace GroupNStegafy.IO
{
    /// <summary>
    ///     Stores information for the file reader class
    /// </summary>
    public class FileReader
    {
        #region Methods

        /// <summary>
        ///     Selects the source image file.
        /// </summary>
        /// <returns>The source image file</returns>
        public async Task<StorageFile> SelectSourceImageFile()
        {
            var openPicker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            var file = await openPicker.PickSingleFileAsync();

            if (file.FileType != ".bmp" && file.FileType != ".png")
            {
                throw new ArgumentOutOfRangeException(file.DisplayName, "File must be .bmp or .jpg file type");
            }

            return file;
        }

        #endregion
    }
}