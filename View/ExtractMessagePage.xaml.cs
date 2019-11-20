
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.IO;

namespace GroupNStegafy.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtractMessagePage
    {

        private double dpiX;
        private double dpiY;
        private WriteableBitmap modifiedImage;
        private readonly FileWriter fileWriter;
        private readonly FileReader fileReader;


        public ExtractMessagePage()
        {
            this.InitializeComponent();

            var applicationWidth = 1600;
            var applicationHeight = 900;

            ApplicationView.PreferredLaunchViewSize = new Size
                { Width = applicationWidth, Height = applicationHeight };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                           .SetPreferredMinSize(new Size(applicationWidth, applicationHeight));

            this.modifiedImage = null;
            this.dpiX = 0;
            this.dpiY = 0;

            this.fileWriter = new FileWriter();
            this.fileReader = new FileReader();
        }
    }
}
