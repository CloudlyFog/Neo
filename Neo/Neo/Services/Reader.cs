using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IronOcr;
using IronSoftware.Drawing;
using Color = System.Drawing.Color;

namespace Neo.Services;

internal sealed class Reader
{
    public static IronTesseract Ocr { get; } = new()
    {
        Language = OcrLanguage.EnglishBest,
        Configuration = new TesseractConfiguration
        {
            WhiteListCharacters = "0123456789 ",
            PageSegmentationMode = TesseractPageSegmentationMode.Auto,
            ReadBarCodes = false,
            RenderSearchablePdfsAndHocr = false,
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
        var output = Ocr.Read(input).Text.Replace("\r", "").RemoveSplitSymbol();
        return Regex.Replace(output, "[^0-9 _]", ";").RemoveSplitSymbol();
    }
        
    /// <summary>
    /// read matrix from image by Tesseract OCR
    /// </summary>
    /// <param name="image">image for read</param>
    /// <param name="dpi">set dpi of image</param>
    /// <param name="deviation">percentage of image' smoothing</param>
    /// <returns></returns>
    public static async Task<string> ReadAsync(Image image, int dpi = 300, double deviation = 1.7d)
    {
        var read = await Ocr.ReadAsync(ConfigureOcrInput((Bitmap)image, dpi, deviation));
        var output = read.Text.Replace("\r", "").RemoveSplitSymbol();
        return Regex.Replace(output, "[^0-9 _]", ";").RemoveSplitSymbol();
    }
        
    /// <summary>
    /// read matrix from image by Tesseract OCR
    /// </summary>
    /// <param name="anyBitmap">bitmap for read</param>
    /// <param name="dpi">set dpi of image</param>
    /// <param name="deviation">percentage of image' smoothing</param>
    /// <returns></returns>
    public static async Task<string> ReadAsync(AnyBitmap anyBitmap, int dpi = 300, double deviation = 1.7d)
    {
        var read = await Ocr.ReadAsync(ConfigureOcrInput(anyBitmap, dpi, deviation));
        var output = read.Text.Replace("\r", "").RemoveSplitSymbol();
        return Regex.Replace(output, "[^0-9 _]", ";").RemoveSplitSymbol();
    }
        
    /// <summary>
    /// read matrix from image by Tesseract OCR
    /// </summary>
    /// <param name="anyBitmap">bitmap for read</param>
    /// <param name="dpi">set dpi of image</param>
    /// <param name="deviation">percentage of image' smoothing</param>
    /// <returns></returns>
    public static string Read(AnyBitmap anyBitmap, int dpi = 300, double deviation = 1.7d)
    {
        var output = Ocr.Read(ConfigureOcrInput(anyBitmap, dpi, deviation)).Text.Replace("\r", "").RemoveSplitSymbol();
        return Regex.Replace(output, "[^0-9 _]", ";").RemoveSplitSymbol();
    }

    private static OcrInput ConfigureOcrInput(Bitmap bitmap, int dpi, double deviation)
    {
        return new OcrInput(bitmap.ImageGaussianSmooth(deviation).Sharpen())
            {
                TargetDPI = dpi
            }
            .ToGrayScale()
            .Invert();
    }

    private static OcrInput ConfigureOcrInput(AnyBitmap anyBitmap, int dpi, double deviation)
    {
        return new OcrInput(anyBitmap)
            {
                TargetDPI = dpi
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
        for (var i = 0; i < str.Length; i++)
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
        var sharpenImage = new Bitmap(image.Width, image.Height);

        const int filterWidth = 3;
        const int filterHeight = 3;
        var w = image.Width;
        var h = image.Height;

        var filter = new double[filterWidth, filterHeight];

        filter[0, 0] = filter[0, 1] = filter[0, 2] = filter[1, 0] = filter[1, 2] = filter[2, 0] = filter[2, 1] = filter[2, 2] = -1;
        filter[1, 1] = 9;

        const double factor = 1.0;
        const double bias = 0.0;

        var result = new Color[image.Width, image.Height];

        for (var x = 0; x < w; ++x)
        {
            for (var y = 0; y < h; ++y)
            {
                double red = 0.0, green = 0.0, blue = 0.0;


                for (var filterX = 0; filterX < filterWidth; filterX++)
                {
                    for (var filterY = 0; filterY < filterHeight; filterY++)
                    {
                        var imageX = (x - filterWidth / 2 + filterX + w) % w;
                        var imageY = (y - filterHeight / 2 + filterY + h) % h;

                        //=====[INSERT LINES]========================================================
                        // Get the color here - once per filter entry and image pixel.
                        var imageColor = image.GetPixel(imageX, imageY);
                        //===========================================================================

                        red += imageColor.R * filter[filterX, filterY];
                        green += imageColor.G * filter[filterX, filterY];
                        blue += imageColor.B * filter[filterX, filterY];
                    }
                    var r = Math.Min(Math.Max((int)(factor * red + bias), 0), 255);
                    var g = Math.Min(Math.Max((int)(factor * green + bias), 0), 255);
                    var b = Math.Min(Math.Max((int)(factor * blue + bias), 0), 255);

                    result[x, y] = Color.FromArgb(r, g, b);
                }
            }
        }
        for (var i = 0; i < w; ++i)
        {
            for (var j = 0; j < h; ++j)
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
        var destBitmapData = destBitmap.LockBits(rect with { X = 0, Y = 0 }, ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

        var pixels = new int[rect.Width * rect.Height];
        Marshal.Copy(sourceBitmapData.Scan0, pixels, 0, pixels.Length);
        Marshal.Copy(pixels, 0, destBitmapData.Scan0, pixels.Length);

        sourceBitmap.UnlockBits(sourceBitmapData);
        destBitmap.UnlockBits(destBitmapData);

        return destBitmap;
    }
}