using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Tesseract;
using Tesseract.Droid;
using PageIteratorLevel = Tesseract.PageIteratorLevel;
using Result = Tesseract.Result;

namespace Neo.Droid.Services;

public sealed class Tesseract : ITesseractApi
{
    private static TesseractApi _tesseractApi = new(Application.Context, AssetsDeployment.OncePerVersion);

    public Tesseract()
    {
        // _tesseractApi = new TesseractApi(Application.Context, AssetsDeployment.OncePerVersion);
    }

    public void Dispose()
    {
        _tesseractApi.Dispose();
    }

    public Task<bool> Init(string lang, OcrEngineMode? mode = null)
    {
        return _tesseractApi.Init(lang);
    }

    public Task<bool> SetImage(string path)
    {
        return _tesseractApi.SetImage(path);
    }

    public Task<bool> SetImage(byte[] data)
    {
        return _tesseractApi.SetImage(data);
    }

    public Task<bool> SetImage(Stream stream)
    {
        return _tesseractApi.SetImage(stream);
    }

    public IEnumerable<Result> Results(PageIteratorLevel level)
    {
        return _tesseractApi.Results(level);
    }

    public void SetPageSegmentationMode(PageSegmentationMode mode)
    {
        _tesseractApi.SetPageSegmentationMode(mode);
    }

    public void SetWhitelist(string whitelist)
    {
        _tesseractApi.SetWhitelist(whitelist);
    }

    public void SetBlacklist(string blacklist)
    {
        _tesseractApi.SetBlacklist(blacklist);
    }

    public void Clear()
    {
        _tesseractApi.Clear();
    }

    public void SetRectangle(Rectangle? rect)
    {
        _tesseractApi.SetRectangle(rect);
    }

    public void SetVariable(string key, string value)
    {
        _tesseractApi.SetVariable(key, value);
    }

    public string Text { get; } = _tesseractApi.Text;
    public bool Initialized { get; } = _tesseractApi.Initialized;
    public event EventHandler<ProgressEventArgs>? Progress;
}