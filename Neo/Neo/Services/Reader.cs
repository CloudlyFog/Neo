using IronOcr;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Neo.Services
{
    internal class Reader
    {
        public static IronTesseract OCR { get; } = new IronTesseract()
        {
            Language = OcrLanguage.EnglishBest,
            Configuration = new TesseractConfiguration()
            {
                WhiteListCharacters = "0123456789 ",
                PageSegmentationMode = TesseractPageSegmentationMode.Auto,
                ReadBarCodes = false,
                RenderSearchablePdfsAndHocr = false
            }
        };

        /// <summary>
        /// read matrix from image by Tesseract OCR
        /// </summary>
        /// <param name="path">path to image</param>
        /// <param name="dpi">set dpi of image</param>
        /// <param name="deviation">percentage of image' smoothing</param>
        /// <returns></returns>
        public static string Read(string path, int dpi = 300, double deviation = 1.7d)
        {
            var input = ConfigureOcrInput((Bitmap)Image.FromFile(path), dpi, deviation);
            var output = OCR.Read(input).Text.Replace("\r", "").RemoveSplitSymbol();
            return Regex.Replace(output, "[^0-9 _]", ";").RemoveSplitSymbol();
        }
        
        /// <summary>
        /// read matrix from image by Tesseract OCR
        /// </summary>
        /// <param name="image">image for reading</param>
        /// <returns></returns>
        public static string Read(Image image)
        {
            var output = OCR.Read(image).Text.Replace("\r", "").RemoveSplitSymbol();
            return Regex.Replace(output, "[^0-9 _]", ";").RemoveSplitSymbol();
        }

        private static OcrInput ConfigureOcrInput(Bitmap bitmap, int DPI, double deviation)
        {
            return new OcrInput(bitmap.ImageGaussianSmooth(deviation).Sharpen())
            {
                TargetDPI = DPI
            }
            .ToGrayScale()
            .Invert();
        }
    }

    public static class StringExtension
    {
        public static string RemoveSplitSymbol(this string str)
        {
            var list = str.ToList();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == Parser.SplitSymbol && str[i] == str[++i])
                    list.RemoveAt(--i);
            }
            return string.Join("", list);
        }
    }

    public static class BitmapExtension
    {
        public static Bitmap Sharpen(this Bitmap image)
        {
            Bitmap sharpenImage = new Bitmap(image.Width, image.Height);

            int filterWidth = 3;
            int filterHeight = 3;
            int w = image.Width;
            int h = image.Height;

            double[,] filter = new double[filterWidth, filterHeight];

            filter[0, 0] = filter[0, 1] = filter[0, 2] = filter[1, 0] = filter[1, 2] = filter[2, 0] = filter[2, 1] = filter[2, 2] = -1;
            filter[1, 1] = 9;

            double factor = 1.0;
            double bias = 0.0;

            Color[,] result = new Color[image.Width, image.Height];

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    double red = 0.0, green = 0.0, blue = 0.0;


                    for (int filterX = 0; filterX < filterWidth; filterX++)
                    {
                        for (int filterY = 0; filterY < filterHeight; filterY++)
                        {
                            int imageX = (x - filterWidth / 2 + filterX + w) % w;
                            int imageY = (y - filterHeight / 2 + filterY + h) % h;

                            //=====[INSERT LINES]========================================================
                            // Get the color here - once per fiter entry and image pixel.
                            var imageColor = image.GetPixel(imageX, imageY);
                            //===========================================================================

                            red += imageColor.R * filter[filterX, filterY];
                            green += imageColor.G * filter[filterX, filterY];
                            blue += imageColor.B * filter[filterX, filterY];
                        }
                        int r = Math.Min(Math.Max((int)(factor * red + bias), 0), 255);
                        int g = Math.Min(Math.Max((int)(factor * green + bias), 0), 255);
                        int b = Math.Min(Math.Max((int)(factor * blue + bias), 0), 255);

                        result[x, y] = Color.FromArgb(r, g, b);
                    }
                }
            }
            for (int i = 0; i < w; ++i)
            {
                for (int j = 0; j < h; ++j)
                {
                    sharpenImage.SetPixel(i, j, result[i, j]);
                }
            }
            return sharpenImage;
        }

        public static Bitmap ImageGaussianSmooth(this Bitmap image, double deviation = 1)
            => GaussianBlur.FilterProcessImage(image, deviation);

        public static Bitmap Crop(this Bitmap sourceBitmap, Rectangle rect)
        {
            // Add code to check and adjust rect to be inside sourceBitmap

            var sourceBitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

            var destBitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
            var destBitmapData = destBitmap.LockBits(new Rectangle(0, 0, rect.Width, rect.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            var pixels = new int[rect.Width * rect.Height];
            Marshal.Copy(sourceBitmapData.Scan0, pixels, 0, pixels.Length);
            Marshal.Copy(pixels, 0, destBitmapData.Scan0, pixels.Length);

            sourceBitmap.UnlockBits(sourceBitmapData);
            destBitmap.UnlockBits(destBitmapData);

            return destBitmap;
        }
    }
}