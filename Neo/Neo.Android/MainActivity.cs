using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Neo.Services;
using SoftwareDeployment.Deployment;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Neo.Droid
{
    [Activity(Label = "Neo", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                               ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : FormsAppCompatActivity
    {
        public MainActivity()
        {
            DependencyService.Register<IMediaService, MediaService>();
            DependencyService.Register<IEngineDeployment, EngineDeployment>();

            DependencyService.Get<IEngineDeployment>()
                .Deploy(IEngineDeployment.TessdataDirectory, IEngineDeployment.InstallationPath, true);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}