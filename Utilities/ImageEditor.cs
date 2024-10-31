using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace WeatherImage.Utilities.ImageEditor
{
    public class ImageEditor
    {
        /// <summary>
        /// Adds text overlays to an image and returns a stream with the updated image.
        /// </summary>
        /// <param name="imageStream">Stream of the base image to modify.</param>
        /// <param name="texts">Array of tuples containing text, position, font size, and color.</param>
        /// <returns>A stream containing the modified image with text overlays.</returns>
        public static Stream AddTextToImage(Stream imageStream, params (string text, (float x, float y) position, int fontSize, string colorHex)[] texts)
        {
            var memoryStream = new MemoryStream();

            // Load the image from the provided stream
            var image = Image.Load(imageStream);

            // Clone and modify the image to add text overlays
            image.Clone(img =>
            {
                var textGraphicsOptions = new TextGraphicsOptions()
                {
                    TextOptions = {
                        WrapTextWidth = image.Width - 10 // Set a wrap width slightly less than the image width
                    }
                };

                foreach (var (text, (x, y), fontSize, colorHex) in texts)
                {
                    // Define font and color
                    var font = SystemFonts.CreateFont("Verdana", fontSize);
                    var color = Rgba32.ParseHex(colorHex);

                    // Draw the text onto the image at the specified location
                    img.DrawText(textGraphicsOptions, text, font, color, new PointF(x, y));
                }
            })
            .SaveAsPng(memoryStream); // Save the modified image to the memory stream

            memoryStream.Position = 0; // Reset the stream position to the beginning

            return memoryStream;
        }
    }
}
