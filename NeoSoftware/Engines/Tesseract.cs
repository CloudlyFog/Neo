using Android.App;
using Tesseract.Droid;

namespace NeoSoftware.Engines
{
    public class Tesseract
    {
        public static TesseractApi Api { get; set; } =
            new TesseractApi(Application.Context, AssetsDeployment.OncePerVersion);
    }
}