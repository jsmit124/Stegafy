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
    public sealed partial class EmbedMessagePage
    {
        #region Data members

        private readonly double applicationHeight = (double) Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double) Application.Current.Resources["AppWidth"];

        private readonly EmbedManager embedManager;

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

            this.embedManager = new EmbedManager();
        }

        #endregion

        #region Methods

        private async void loadSourceButton_Click(object sender, RoutedEventArgs e)
        {
            this.progressRing.IsActive = true;

            await this.embedManager.LoadSourceImage();
            if (!this.embedManager.SourceImageLoaded())
            {
                this.progressRing.IsActive = false;
                return;
            }

            this.sourceImageDisplay.Source = this.embedManager.SourceImage;

            this.checkIfSourceLoadedToEnableSettings();
            this.progressRing.IsActive = false;
        }

        private async void loadMessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.progressRing.IsActive = true;
            this.hideMessageDisplays();

            await this.embedManager.LoadMessage();

            if (!this.embedManager.MessageLoaded())
            {
                this.progressRing.IsActive = false;
                return;
            }

            if (this.embedManager.MessageFileType == FileTypeConstants.TextFileType)
            {
                this.textFileDisplay.Text = this.embedManager.TextFromFile;
                this.textFileDisplay.Visibility = Visibility.Visible;
                this.textFileScroller.Visibility = Visibility.Visible;
            }
            else
            {
                this.monochromeImageDisplay.Source = this.embedManager.MessageImage;
                this.monochromeImageDisplay.Visibility = Visibility.Visible;
            }

            this.checkIfMessageLoadedToEnableLoadSourceButton();
            this.checkIfSourceLoadedToEnableSettings();
            this.progressRing.IsActive = false;
        }

        private async void embedButton_Click(object sender, RoutedEventArgs e)
        {
            this.progressRing.IsActive = true;

            var isChecked = this.encryptionSelectionCheckBox.IsChecked;
            var encryptionIsChecked = isChecked != null && isChecked.Value;
            var bpccSelection = (ComboBoxItem) this.BPCCSelectionComboBox.SelectedItem;
            if (bpccSelection?.Content != null)
            {
                var bpcc = int.Parse(bpccSelection.Content.ToString());
                var encryptionKey = this.encryptionKeyTextBox.Text;
                if (string.IsNullOrEmpty(encryptionKey))
                {
                    encryptionKey = "CS";
                }

                if (encryptionIsChecked && encryptionKey.Equals(string.Empty) &&
                    this.embedManager.MessageFileType.Equals(FileTypeConstants.TextFileType))
                {
                    await Dialogs.ShowNoEncryptionKeyInput();
                    this.progressRing.IsActive = false;
                    return;
                }

                await this.embedManager.EmbedMessage(encryptionIsChecked, bpcc, encryptionKey);
            }

            if (this.embedManager.MessageTooLarge)
            {
                this.progressRing.IsActive = false;
                return;
            }

            this.embeddedImageDisplay.Source = this.embedManager.EmbeddedImage;
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
            this.embedManager.SaveEmbeddedImage();
        }

        private void checkIfSourceLoadedToEnableSettings()
        {
            if (this.embedManager.SourceImageLoaded())
            {
                this.enableUniversalEmbedSettings();

                if (this.embedManager.MessageFileType.Equals(FileTypeConstants.TextFileType))
                {
                    this.enableTextSettingsOptions();
                }
                else
                {
                    this.resetSettings();
                    this.disableTextSettingsOptions();
                    this.embedButton.IsEnabled = true;
                    this.encryptionSelectionCheckBox.IsEnabled = true;
                }
            }
        }

        private void disableTextSettingsOptions()
        {
            this.BPCCSelectionComboBox.IsEnabled = false;
            this.encryptionKeyTextBox.IsEnabled = false;
        }

        private void checkIfMessageLoadedToEnableLoadSourceButton()
        {
            if (this.embedManager.MessageLoaded())
            {
                this.loadSourceButton.IsEnabled = true;
            }
        }

        private void enableTextSettingsOptions()
        {
            this.BPCCSelectionComboBox.IsEnabled = true;
            this.encryptionKeyTextBox.IsEnabled = true;
        }

        private void enableUniversalEmbedSettings()
        {
            this.embedButton.IsEnabled = true;
            this.encryptionSelectionCheckBox.IsEnabled = true;
        }

        private void hideMessageDisplays()
        {
            this.monochromeImageDisplay.Visibility = Visibility.Collapsed;
            this.textFileDisplay.Visibility = Visibility.Collapsed;
            this.textFileScroller.Visibility = Visibility.Collapsed;
        }

        private void resetSettings()
        {
            this.encryptionKeyTextBox.Text = "";
            this.BPCCSelectionComboBox.SelectedIndex = 0;
        }

        #endregion
    }
}