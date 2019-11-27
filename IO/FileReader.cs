using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using GroupNStegafy.View;

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
            StorageFile file = null;
            try
            {
                var openPicker = new FileOpenPicker {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };

                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".bmp");

                file = await openPicker.PickSingleFileAsync();

                if (file.FileType != ".bmp" && file.FileType != ".png")
                {
                    throw new ArgumentOutOfRangeException(file.DisplayName, "File must be .bmp or .jpg file type");
                }
            }
            catch (Exception)
            {
                await Dialogs.ShowFileSelectionCancelledDialog("source");
            }

            return file;
        }

        /// <summary>
        ///     Selects the source image file.
        /// </summary>
        /// <returns>The source image file</returns>
        public async Task<StorageFile> SelectMessageFile()
        {
            StorageFile file = null;
            try
            {
                var openPicker = new FileOpenPicker {
                    ViewMode = PickerViewMode.Thumbnail,
                    SuggestedStartLocation = PickerLocationId.DocumentsLibrary
                };

                openPicker.FileTypeFilter.Add(".png");
                openPicker.FileTypeFilter.Add(".bmp");
                openPicker.FileTypeFilter.Add(".txt");

                file = await openPicker.PickSingleFileAsync();

                if (file.FileType != ".bmp" && file.FileType != ".png" && file.FileType != ".txt")
                {
                    throw new ArgumentOutOfRangeException(file.DisplayName,
                        "File must be .bmp, .jpg, or .txt file type");
                }
            }
            catch (Exception)
            {
                await Dialogs.ShowFileSelectionCancelledDialog("message");
            }

            return file;
        }

        public async Task<String> ReadTextFromFile(StorageFile textFile)
        {
            var inputStream = await textFile.OpenSequentialReadAsync();

            string fileContents;
            using (var streamReader = new StreamReader(inputStream.AsStreamForRead()))
            {
                fileContents = await streamReader.ReadToEndAsync();
            }

            return fileContents;
        }

        #endregion
    }
}