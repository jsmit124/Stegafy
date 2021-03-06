﻿using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using GroupNStegafy.Constants;
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

                openPicker.FileTypeFilter.Add(FileTypeConstants.PortableNetworkImage);
                openPicker.FileTypeFilter.Add(FileTypeConstants.BitmapFileType);

                file = await openPicker.PickSingleFileAsync();

                if (file.FileType != FileTypeConstants.BitmapFileType &&
                    file.FileType != FileTypeConstants.PortableNetworkImage)
                {
                    throw new ArgumentOutOfRangeException(file.DisplayName, "File must be .bmp or .jpg file type");
                }
            }
            catch (NullReferenceException)
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

                openPicker.FileTypeFilter.Add(FileTypeConstants.PortableNetworkImage);
                openPicker.FileTypeFilter.Add(FileTypeConstants.BitmapFileType);
                openPicker.FileTypeFilter.Add(FileTypeConstants.TextFileType);

                file = await openPicker.PickSingleFileAsync();

                if (file.FileType != FileTypeConstants.BitmapFileType && file.FileType !=
                                                                      FileTypeConstants.PortableNetworkImage
                                                                      && file.FileType !=
                                                                      FileTypeConstants.TextFileType)
                {
                    throw new ArgumentOutOfRangeException(file.DisplayName,
                        "File must be .bmp, .jpg, or .txt file type");
                }
            }
            catch (NullReferenceException)
            {
                await Dialogs.ShowFileSelectionCancelledDialog("message");
            }

            return file;
        }

        /// <summary>
        ///     Reads the text from file.
        /// </summary>
        /// <param name="textFile">The text file.</param>
        /// <returns>The text stored in the storage file</returns>
        public async Task<string> ReadTextFromFile(StorageFile textFile)
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