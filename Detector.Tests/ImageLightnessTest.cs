using System.Drawing;
using Detector.EpisodeStorage.ScreenShotDB;
using Xunit;

namespace Detector.Tests
{
    public class ImageLightnessTest
    {
        [Fact]
        public void ProcessImage()
        {
            var image = new Bitmap(@"D:\TEMP\cbimage.jpg");
            var lightener = new Lightener(null, null);
            lightener.ProcessImage(image, .45f).Save(@"D:\TEMP\cbimage_light.jpg");
            lightener.ProcessImage(image, -.45f).Save(@"D:\TEMP\cbimage_dark.jpg");
        }
    }
}
