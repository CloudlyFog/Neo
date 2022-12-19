using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IronOcr;
using Color = System.Drawing.Color;

namespace Neo.Services;

internal sealed class Reader : IDisposable
{
    private readonly Stream _stream;
    private IronTesseract _ocr;

    public Reader(Stream stream)
    {
        _stream = stream;
        _ocr = new IronTesseract
        {
            Language = OcrLanguage.EnglishBest,
            Configuration = new TesseractConfiguration
            {
                WhiteListCharacters = "0123456789 ",
                PageSegmentationMode = TesseractPageSegmentationMode.Auto,
                ReadBarCodes = false,
                RenderSearchablePdfsAndHocr = false,
                EngineMode = TesseractEngineMode.TesseractAndLstm,
                TesseractVersion = TesseractVersion.Tesseract5,
            }
        };
    }

    /// <summary>
    /// read matrix from image by ocr
    /// </summary>
    /// <param name="stream">stream for read</param>
    /// <param name="dpi">dpi of output image</param>
    /// <exception cref="ArgumentNullException">if ocr wasn't initialized</exception>
    /// <exception cref="InvalidOperationException">if executable path was wrong</exception>
    /// <returns></returns>
    public string Read(int dpi = 300, double deviation = 1.7d)
    {
        if (_ocr is null)
            throw new ArgumentNullException(_ocr.ToString());
        string _output;
        try
        {
            using var input = ConfigureOcrInput(_stream, dpi, deviation);

            // here we delete \r and \n from output
            var output = _ocr.Read(input)
                .Text.Replace("\r", "").RemoveSplitSymbol();

            _output =
                Regex.Replace(output, "[^0-9 _]", ";").RemoveSplitSymbol();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            Dispose();
            throw new InvalidOperationException(exception.Message, exception.InnerException);
        }

        return _output;
    }

    /// <summary>
    /// read matrix from image by ocr
    /// </summary>
    /// <param name="stream">stream for read</param>
    /// <param name="dpi">dpi of output image</param>
    /// <exception cref="ArgumentNullException">if ocr wasn't initialized</exception>
    /// <exception cref="InvalidOperationException">if executable path was wrong</exception>
    /// <returns></returns>
    public async Task<string> ReadAsync(int dpi = 300, double deviation = 1.7d)
    {
        if (_ocr is null)
            throw new ArgumentNullException(_ocr.ToString());
        string _output;
        try
        {
            using var input = ConfigureOcrInput(_stream, dpi, deviation);
            var ocrResult = await _ocr.ReadAsync(input);

            var output =
                ocrResult.Text.Replace("\r", "").RemoveSplitSymbol();
            _output =
                Regex.Replace(output, "[^0-9 _]", ";").RemoveSplitSymbol();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            Dispose();
            throw new InvalidOperationException(exception.Message, exception.InnerException);
        }

        return _output;
    }

    private static OcrInput ConfigureOcrInput(Stream stream, int dpi, double deviation)
    {
        // init for the next cast from stream to bitmap
        var prevInput = new OcrInput(stream)
            {
                TargetDPI = dpi
            }
            .ToGrayScale()
            .Invert();

        // pass bitmap to OcrInput's constructor instead of stream 
        // and here the bitmap 'll smoothed and increased sharpness for next reading
        return new OcrInput(prevInput.Pages[0].ToBitmap()
                .ImageGaussianSmooth(deviation).Sharpen())
            {
                TargetDPI = dpi
            }
            .ToGrayScale()
            .Invert();
    }

    private void ReleaseUnmanagedResources()
    {
        _ocr = null;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        ReleaseUnmanagedResources();
        if (disposing)
        {
            _stream.Dispose();
        }
    }

    ~Reader()
    {
        Dispose(false);
    }
}

public static class StringExtension
{
    /// <summary>
    /// Remove split symbols from string
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
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
    /// <summary>
    /// sharpen passed image
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    public static Bitmap Sharpen(this Bitmap image)
    {
        var sharpenImage = new Bitmap(image.Width, image.Height);

        const int filterWidth = 3;
        const int filterHeight = 3;
        var w = image.Width;
        var h = image.Height;

        var filter = new double[filterWidth, filterHeight];

        filter[0, 0] = filter[0, 1] =
            filter[0, 2] = filter[1, 0] = filter[1, 2] = filter[2, 0] = filter[2, 1] = filter[2, 2] = -1;
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
                sharpenImage.SetPixel(i, j, result[i, j]);
        }

        return sharpenImage;
    }

    /// <summary>
    /// smooth passed image by gaussian blur
    /// </summary>
    /// <param name="image"></param>
    /// <param name="deviation"></param>
    /// <returns></returns>
    public static Bitmap ImageGaussianSmooth(this Bitmap image, double deviation = 1.7d)
        => GaussianBlur.FilterProcessImage(image, deviation);

    public static Bitmap Crop(this Bitmap sourceBitmap, Rectangle rect)
    {
        // Add code to check and adjust rect to be inside sourceBitmap

        var sourceBitmapData = sourceBitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

        var destBitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format24bppRgb);
        var destBitmapData = destBitmap.LockBits(rect with { X = 0, Y = 0 }, ImageLockMode.WriteOnly,
            PixelFormat.Format32bppRgb);

        var pixels = new int[rect.Width * rect.Height];
        Marshal.Copy(sourceBitmapData.Scan0, pixels, 0, pixels.Length);
        Marshal.Copy(pixels, 0, destBitmapData.Scan0, pixels.Length);

        sourceBitmap.UnlockBits(sourceBitmapData);
        destBitmap.UnlockBits(destBitmapData);

        return destBitmap;
    }
}