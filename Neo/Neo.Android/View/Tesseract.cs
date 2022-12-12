using Android.App;
using Tesseract.Droid;

namespace Neo.Droid.View
{
    public class Tesseract
    {
        public static void Main()
        {
            var api = new TesseractApi(Application.Context, AssetsDeployment.OncePerVersion);
            
            
        }
    }
}