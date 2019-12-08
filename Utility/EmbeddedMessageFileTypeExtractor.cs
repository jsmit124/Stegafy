using GroupNStegafy.Enumerables;

namespace GroupNStegafy.Utility
{
    /// <summary>
    ///     Stores information for the EmbeddedMessageFileTypeExtractor class
    /// </summary>
    public static class EmbeddedMessageFileTypeExtractor
    {
        #region Methods

        /// <summary>
        ///     Determines the file type to extract.
        /// </summary>
        /// Precondition: none
        /// Postcondition: none
        /// <param name="pixelInformation">The pixel information.</param>
        /// <param name="imageWidth">Width of the image.</param>
        /// <returns>The file type of the embedded image</returns>
        public static FileTypes DetermineFileTypeToExtract(byte[] pixelInformation, uint imageWidth)
        {
            var pixelColor = PixelColorInfo.GetPixelBgra8(pixelInformation, 0, 1, imageWidth);

            if ((pixelColor.B & 1) != 0)
            {
                return FileTypes.Text;
            }

            return FileTypes.Bitmap;
        }

        #endregion
    }
}