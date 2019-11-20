using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GroupNStegafy.View
{
    /// <summary>
    ///     The main page used as an entry point for the program
    /// </summary>
    public sealed partial class MainPage
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
        }

        #endregion

        private void EmbedButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EmbedMessagePage));
        }

        private void ExtractButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ExtractMessagePage));
        }
    }

}