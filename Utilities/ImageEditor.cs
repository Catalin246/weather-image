// using System.Drawing;
// using System.Drawing.Imaging;
// using System.IO;
// using WeatherImage.Models;

// namespace WeatherImage.Utilities
// {
//     public static class ImageEditor
//     {
//         public static Stream CreateWeatherImage(string backgroundImageUrl, StationMeasurement stationData)
//         {
//             using (var client = new HttpClient())
//             using (var backgroundImageStream = client.GetStreamAsync(backgroundImageUrl).Result)
//             using (var bitmap = new Bitmap(backgroundImageStream))
//             {
//                 using (var graphics = Graphics.FromImage(bitmap))
//                 {
//                     var font = new Font("Arial", 24, FontStyle.Bold);
//                     var brush = new SolidBrush(Color.White);
//                     var point = new PointF(10, 10);

//                     string weatherText = $"Station: {stationData.Name}\n" +
//                                          $"Temperature: {stationData.Temperature} °C";

//                     graphics.DrawString(weatherText, font, brush, point);
//                 }

//                 var output = new MemoryStream();
//                 bitmap.Save(output, ImageFormat.Png);
//                 output.Position = 0; // Reset stream position for reading
//                 return output;
//             }
//         }
//     }
// }
