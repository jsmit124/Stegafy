using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using GroupNStegafy.Constants;
using GroupNStegafy.Controller;

namespace GroupNStegafy.View
{
    /// <summary>
    ///     The embedding page used for embedding images
    /// </summary>
    public sealed partial class EmbedMessagePage : Page
    {
        #region Data members

        private readonly double applicationHeight = (double) Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double) Application.Current.Resources["AppWidth"];

        private readonly StegafyManager stegafyManager;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmbedMessagePage" /> class.
        /// </summary>
        public EmbedMessagePage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size
                {Width = this.applicationWidth, Height = this.applicationHeight};
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                           .SetPreferredMinSize(new Size(this.applicationWidth, this.applicationHeight));

            this.stegafyManager = new StegafyManager();
        }

        #endregion

        #region Methods

        private async void loadSourceButton_Click(object sender, RoutedEventArgs e)
        {
            this.progressRing.IsActive = true;

            await this.stegafyManager.LoadSourceImage();
            if (!this.stegafyManager.SourceImageLoaded())
            {
                this.progressRing.IsActive = false;
                return;
            }

            this.sourceImageDisplay.Source = this.stegafyManager.SourceImage;

            this.checkIfSourceLoadedToEnableSettings();
            this.progressRing.IsActive = false;
        }

        private async void loadMessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.progressRing.IsActive = true;
            this.hideMessageDisplays();

            await this.stegafyManager.LoadMessage();

            if (!this.stegafyManager.MessageLoaded())
            {
                this.progressRing.IsActive = false;
                return;
            }

            if (this.stegafyManager.MessageFileType == FileTypeConstants.TextFileType)
            {
                this.textFileDisplay.Text = this.stegafyManager.TextFromFile;
                this.textFileDisplay.Visibility = Visibility.Visible;
                this.textFileScroller.Visibility = Visibility.Visible;
            }
            else
            {
                this.monochromeImageDisplay.Source = this.stegafyManager.MessageImage;
                this.monochromeImageDisplay.Visibility = Visibility.Visible;
            }

            this.checkIfMessageLoadedToEnableLoadSourceButton();
            this.progressRing.IsActive = false;
        }

        private async void embedButton_Click(object sender, RoutedEventArgs e)
        {
            this.progressRing.IsActive = true;

            var encryptionIsChecked = this.encryptionSelectionCheckBox.IsChecked.Value;
            var bpccSelection = (ComboBoxItem) this.BPCCSelectionComboBox.SelectedItem;
            var bpcc = int.Parse(bpccSelection.Content.ToString());

            await this.stegafyManager.EmbedMessage(encryptionIsChecked, bpcc);

            if (this.stegafyManager.MessageTooLarge)
            {
                this.progressRing.IsActive = false;
                return;
            }

            this.embeddedImageDisplay.Source = this.stegafyManager.EmbeddedImage;
            this.saveButton.IsEnabled = true;
            this.progressRing.IsActive = false;
        }

        private void homeButton_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null,
                new SlideNavigationTransitionInfo {Effect = SlideNavigationTransitionEffect.FromRight});
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            this.stegafyManager.SaveEmbeddedImage();
        }

        private void checkIfSourceLoadedToEnableSettings()
        {
            if (this.stegafyManager.SourceImageLoaded())
            {
                this.enableSettingsOptions();
            }
        }

        private void checkIfMessageLoadedToEnableLoadSourceButton()
        {
            if (this.stegafyManager.MessageLoaded())
            {
                this.loadSourceButton.IsEnabled = true;
            }
        }

        private void enableSettingsOptions()
        {
            this.embedButton.IsEnabled = true;
            this.encryptionSelectionCheckBox.IsEnabled = true;
            this.BPCCSelectionComboBox.IsEnabled = true;
        }

        private void hideMessageDisplays()
        {
            this.monochromeImageDisplay.Visibility = Visibility.Collapsed;
            this.textFileDisplay.Visibility = Visibility.Collapsed;
            this.textFileScroller.Visibility = Visibility.Collapsed;
        }

        #endregion
    }
}