using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Detector.EpisodeStorage.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Detector.EpisodeStorage.ScreenShotDB
{
    public class Lightener
    {
        private readonly ILogger<Lightener> _logger;
        private readonly IOptions<Config> _config;

        public Lightener(ILogger<Lightener> logger, IOptions<Config> config)
        {
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// HSL (hue, saturation, lightness) scheme
        /// https://docs.microsoft.com/en-us/dotnet/api/system.drawing.imaging.colormatrix?view=netcore-3.0
        /// https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/matrix-representation-of-transformations
        /// </summary>
        /// <param name="image">image to transform</param>
        /// <param name="value">lightness value</param>
        public Bitmap ProcessImage(Bitmap image, float value)
        {
            //create a blank bitmap the same size as original
            var lightImage = new Bitmap(image.Width, image.Height);

            // create the ColorMatrix for lightness.
            var colorMatrix = new ColorMatrix(
                new float[][]
                {
                    new float[] {1, 0, 0, 0, 0},
                    new float[] {0, 1, 0, 0, 0},
                    new float[] {0, 0, 1, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {value, value, value, 0, 1}
                });

            //create some image attributes
            var attributes = new ImageAttributes();
            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);
            //draw original image on the new image using the color matrix
            Graphics.FromImage(lightImage)
                .DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

            return lightImage;
        }

        public static Bitmap HistEq( Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            BitmapData sd = img.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int bytes = sd.Stride * sd.Height;
            byte[] buffer = new byte[bytes];
            byte[] result = new byte[bytes];
            Marshal.Copy(sd.Scan0, buffer, 0, bytes);
            img.UnlockBits(sd);
            int current = 0;
            double[] pn = new double[256];
            for (int p = 0; p < bytes; p += 4)
            {
                pn[buffer[p]]++;
            }
            for (int prob = 0; prob < pn.Length; prob++)
            {
                pn[prob] /= (w * h);
            }
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    current = y * sd.Stride + x * 4;
                    double sum = 0;
                    for (int i = 0; i < buffer[current]; i++)
                    {
                        sum += pn[i];
                    }
                    for (int c = 0; c < 3; c++)
                    {
                        result[current + c] = (byte)Math.Floor(255 * sum);
                    }
                    result[current + 3] = 255;
                }
            }
            Bitmap res = new Bitmap(w, h);
            BitmapData rd = res.LockBits(new Rectangle(0, 0, w, h),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(result, 0, rd.Scan0, bytes);
            res.UnlockBits(rd);
            return res;
        }
    }
}
