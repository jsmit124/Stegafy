using Windows.UI;

namespace GroupNStegafy.Utility
{
    /// <summary>
    ///     Stores methods for getting and setting pixel color data
    /// </summary>
    public static class PixelColorInfo
    {
        #region Methods

        /// <summary>
        ///     Gets the pixel bgra8 color from the current pixel.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <returns>The color of the current pixel</returns>
        public static Color GetPixelBgra8(byte[] pixels, int x, int y, uint width)
        {
            var offset = (x * (int) width + y) * 4;
            var r = pixels[offset + 2];
            var g = pixels[offset + 1];
            var b = pixels[offset + 0];
            return Color.FromArgb(0, r, g, b);
        }

        /// <summary>
        ///     Sets the pixel bgra8 color to the current pixel.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        public static void SetPixelBgra8(byte[] pixels, int x, int y, Color color, uint width)
        {
            var offset = (x * (int) width + y) * 4;
            pixels[offset + 2] = color.R;
            pixels[offset + 1] = color.G;
            pixels[offset + 0] = color.B;
        }

        #endregion
    }
}