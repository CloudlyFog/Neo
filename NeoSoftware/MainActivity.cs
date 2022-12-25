using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Gms.Vision;
using Android.Gms.Vision.Texts;
using Android.Util;
using Android.Graphics;
using Android.Runtime;
using Android;
using static Android.Gms.Vision.Detector;
using System.Text;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;

namespace NeoSoftware
{
    [Activity(Label = "Neo", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ISurfaceHolderCallback, IProcessor
    {
        private SurfaceView cameraView;
        private TextView txtView;
        private CameraSource cameraSource;

        private
            const int RequestCameraPermissionID = 1001;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource  
            SetContentView(Resource.Layout.activity_main);
            cameraView = FindViewById<SurfaceView>(Resource.Id.surface_view);
            txtView = FindViewById<TextView>(Resource.Id.txtview);
            var txtRecognizer = new TextRecognizer.Builder(ApplicationContext).Build();
            if (!txtRecognizer.IsOperational)
            {
                Log.Error("Main Activity", "Detector dependencies are not yet available");
            }
            else
            {
                cameraSource = new CameraSource.Builder(ApplicationContext, txtRecognizer).SetFacing(CameraFacing.Back)
                    .SetRequestedPreviewSize(2340, 1080).SetRequestedFps(0.5f).SetAutoFocusEnabled(true).Build();
                cameraView.Holder.AddCallback(this);
                txtRecognizer.SetProcessor(this);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) !=
                Android.Content.PM.Permission.Granted)
            {
                //Request permission  
                ActivityCompat.RequestPermissions(this, new string[]
                {
                    Android.Manifest.Permission.Camera
                }, RequestCameraPermissionID);
                return;
            }

            cameraSource.Start(cameraView.Holder);
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            cameraSource.Stop();
        }

        public void ReceiveDetections(Detections detections)
        {
            var items = detections.DetectedItems;
            if (items.Size() != 0)
            {
                txtView.Post(() =>
                {
                    var strBuilder = new StringBuilder();
                    for (var i = 0; i < items.Size(); ++i)
                    {
                        strBuilder.Append(((TextBlock)items.ValueAt(i)).Value);
                        strBuilder.Append("\n");
                    }

                    txtView.Text = strBuilder.ToString();
                });
            }
        }

        public void Release()
        {
        }
    }
}