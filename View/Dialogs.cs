using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace GroupNStegafy.View
{
    public static class Dialogs
    {
        public static async Task ShowMessageFileTooLargeDialog()
        {
            var messageFileTooLargeDialog = new ContentDialog
            {
                Title = "ERROR",
                Content = "Message file exceeds the dimensions of the source image"
                          + Environment.NewLine + "Embedding will not occur"
                          + Environment.NewLine + "Choose another source or message image and try again.",
                CloseButtonText = "Ok"
            };

            await messageFileTooLargeDialog.ShowAsync();
        }

        public static async Task ShowFileSelectionCancelledDialog(string imageType)
        {
            var loadMessageCancelledDialog = new ContentDialog
            {
                Title = "CANCELLED",
                Content = "Cancelled loading " + imageType + " image",
                CloseButtonText = "Ok"
            };

            await loadMessageCancelledDialog.ShowAsync();
        }
    }
}
