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

            var applicationWidth = 500;
            var applicationHeight = 200;

            ApplicationView.PreferredLaunchViewSize = new Size
                { Width = applicationWidth, Height = applicationHeight };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                .SetPreferredMinSize(new Size(applicationWidth, applicationHeight));
        }

        #endregion

        private async void EmbedButton_Click(object sender, RoutedEventArgs e)
        {
            var viewId = 0;
            var newView = CoreApplication.CreateNewView();
            await newView.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    var frame = new Frame();
                    frame.Navigate(typeof(EmbedMessagePage), null);
                    Window.Current.Content = frame;

                    viewId = ApplicationView.GetForCurrentView().Id;

                    Window.Current.Activate();
                });

            var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(viewId);
        }

        private void ExtractButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO
            this.Frame.Navigate(typeof(ExtractMessagePage));
        }
    }
}