using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Android.App;
using IronOcr;
using Neo.Services;
using Tesseract;
using Tesseract.Droid;
using XLabs.Ioc;

namespace Neo.Droid.Services;

internal sealed class Reader : IDisposable
{
    private readonly Stream _stream;
    private IronTesseract _ocr;
    private readonly ITesseractApi _tesseractApi;

    public Reader(Stream stream)
    {
        _stream = stream;
        _tesseractApi = Resolver.Resolve<ITesseractApi>();
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
            using var api = new TesseractApi(Application.Context, AssetsDeployment.OncePerVersion);
            await api.Init("eng");
            await api.SetImage(_stream);
            var output = api.Text
                .Replace("\r", "").RemoveSplitSymbol();
            _output = Regex
                .Replace(output, "[^0-9 _]", ";").RemoveSplitSymbol();
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
        return new OcrInput(prevInput.Pages.GetEnumerator().Current.ToBitmap()
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