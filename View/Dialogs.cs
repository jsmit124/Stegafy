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
        /// Precondition: none
        /// Postcondition: none
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
        /// Precondition: none
        /// Postcondition: none
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

        /// <summary>
        ///     Shows the no message dialog.
        /// </summary>
        /// Precondition: none
        /// Postcondition: none
        public static async Task ShowNoMessageDialog()
        {
            var noMessageDialog = new ContentDialog {
                Title = "No Message Found",
                Content = "There was no embedded message found in this image",
                CloseButtonText = "Ok"
            };

            await noMessageDialog.ShowAsync();
        }

        /// <summary>
        ///     Shows the not possible to embed text into source image dialog.
        /// </summary>
        /// Precondition: none
        /// Postcondition: none
        public static async Task ShowNotPossibleToEmbedTextDialog()
        {
            var notPossibletoEmbedTextDialog = new ContentDialog {
                Title = "ERROR",
                Content = "Text cannot be embedded into this image." + Environment.NewLine +
                          "Choose another source image and try again.",
                CloseButtonText = "Ok"
            };

            await notPossibletoEmbedTextDialog.ShowAsync();
        }

        /// <summary>
        ///     Shows the required bpcc to embed text into source image dialog.
        /// </summary>
        /// Precondition: none
        /// Postcondition: none
        public static async Task ShowRequiredBpccToEmbedTextDialog(int requiredBpcc)
        {
            var requiredBpccDialog = new ContentDialog {
                Title = "ERROR",
                Content = "Text cannot be embedded into this image with currently selected BPCC count."
                          + Environment.NewLine + "To embed this message, it will require a BPCC selection of " +
                          requiredBpcc + " or higher.",
                CloseButtonText = "Ok"
            };

            await requiredBpccDialog.ShowAsync();
        }

        /// <summary>
        ///     Shows the error no encryption key dialog.
        /// </summary>
        /// Precondition: none
        /// Postcondition: none
        public static async Task ShowNoEncryptionKeyInput()
        {
            var noKeyDialog = new ContentDialog {
                Title = "ERROR",
                Content = "No encryption key was input. Please input a key to use for encryption.",
                CloseButtonText = "Ok"
            };

            await noKeyDialog.ShowAsync();
        }

        #endregion
    }
}