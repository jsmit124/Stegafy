using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace GroupNStegafy.View
{
    /// <summary>
    ///     The main page used as an entry point for the program
    /// </summary>
    public sealed partial class MainPage
    {

        private readonly double applicationHeight = (double)Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double)Application.Current.Resources["AppWidth"];

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size
                { Width = this.applicationWidth, Height = this.applicationHeight };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                           .SetPreferredMinSize(new Size(this.applicationWidth, this.applicationHeight));
        }

        #endregion

        private void EmbedButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EmbedMessagePage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
        }

        private void ExtractButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ExtractMessagePage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }
    }

}