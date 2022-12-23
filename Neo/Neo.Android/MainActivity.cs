using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Neo.Droid.Services;
using Neo.Services;
using SoftwareDeployment.Deployment;
using Tesseract;
using Tesseract.Droid;
using TinyIoC;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XLabs.Ioc;
using XLabs.Ioc.TinyIOC;
using XLabs.Platform.Device;
using IDevice = Carbon.Kit.Xamarin.Abstractions.Device.Bluetooth.Contracts.IDevice;

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
            DependencyService.Register<ITesseractApi>();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);

            var container = TinyIoCContainer.Current;
            container.Register<ITesseractApi>((cont, parameters)
                => new TesseractApi(ApplicationContext, AssetsDeployment.OncePerInitialization));
            Resolver.SetResolver(new TinyResolver(container));
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