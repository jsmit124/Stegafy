using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using GroupNStegafy.Controller;

namespace GroupNStegafy.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtractMessagePage
    {
        #region Data members

        private readonly double applicationHeight = (double) Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double) Application.Current.Resources["AppWidth"];

        private readonly ExtractManager extractManager;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtractMessagePage" /> class.
        /// </summary>
        public ExtractMessagePage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size
                {Width = this.applicationWidth, Height = this.applicationHeight};
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                           .SetPreferredMinSize(new Size(this.applicationWidth, this.applicationHeight));

            this.extractManager = new ExtractManager();
        }

        #endregion

        #region Methods

        private void homeButton_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null,
                new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromLeft});
        }

        private async void loadEmbeddedImageButton_Click(object sender, RoutedEventArgs e)
        {
            await this.extractManager.LoadEmbeddedImage();

            this.embeddedImageDisplay.Source = this.extractManager.EmbeddedImage;

            if (this.extractManager.EmbeddedImageFile != null)
            {
                this.extractButton.IsEnabled = true;
            }
        }

        private async void extractButton_Click(object sender, RoutedEventArgs e)
        {
            await this.extractManager.ExtractMessage();

            if (this.extractManager.ExtractedImage != null)
            {
                if (this.extractManager.EncryptionUsed)
                {
                    //TODO handle displaying decrypted image
                }

                this.decryptedImageDisplay.Source = this.extractManager.ExtractedImage;
                this.saveDecryptedMessageButton.IsEnabled = true;
            }
            else if (this.extractManager.ExtractedText != null)
            {
                if (this.extractManager.EncryptionUsed)
                {
                    this.decryptedMessageTextBlock.Text = this.extractManager.DecryptedText;
                }

                this.encryptedMessageTextBlock.Text = this.extractManager.ExtractedText;
            }
        }

        private void saveDecryptedMessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.extractManager.SaveExtractedMessage();
        }

        #endregion
    }
}