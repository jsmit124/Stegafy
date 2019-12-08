using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace GroupNStegafy.View
{
    /// <summary>
    ///     The main page used as an entry point for the program
    /// </summary>
    public sealed partial class MainPage
    {
        #region Data members

        private readonly double applicationHeight = (double) Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double) Application.Current.Resources["AppWidth"];

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPage" /> class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size
                {Width = this.applicationWidth, Height = this.applicationHeight};
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                           .SetPreferredMinSize(new Size(this.applicationWidth, this.applicationHeight));
        }

        #endregion

        #region Methods

        private void EmbedButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EmbedMessagePage), null,
                new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromLeft});
        }

        private void ExtractButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ExtractMessagePage), null,
                new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromRight});
        }

        #endregion
    }
}