using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace GroupNStegafy.View
{
    /// <summary>
    ///     Stores all content dialogs shown within the program
    /// </summary>
    public static class Dialogs
    {
        #region Methods

        /// <summary>
        ///     Shows the message file too large dialog.
        /// </summary>
        public static async Task ShowMessageFileTooLargeDialog()
        {
            var messageFileTooLargeDialog = new ContentDialog {
                Title = "ERROR",
                Content = "Message file exceeds the dimensions of the source image"
                          + Environment.NewLine + "Embedding will not occur"
                          + Environment.NewLine + "Choose another source or message image and try again.",
                CloseButtonText = "Ok"
            };

            await messageFileTooLargeDialog.ShowAsync();
        }

        /// <summary>
        ///     Shows the file selection cancelled dialog.
        /// </summary>
        /// <param name="imageType">Type of the image.</param>
        public static async Task ShowFileSelectionCancelledDialog(string imageType)
        {
            var loadMessageCancelledDialog = new ContentDialog {
                Title = "CANCELLED",
                Content = "Cancelled loading " + imageType + " image",
                CloseButtonText = "Ok"
            };

            await loadMessageCancelledDialog.ShowAsync();
        }

        #endregion
    }
}