using System;
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
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Java.Lang;
using Java.Interop;
using Neo.Services;
using StringBuilder = System.Text.StringBuilder;
using static Android.Gms.Vision.Detector;
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;
using Exception = Java.Lang.Exception;
using View = Android.Views.View;

namespace NeoSoftware
{
    [Activity(Label = "Recognize", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ISurfaceHolderCallback, IProcessor
    {
        private SurfaceView _cameraView;
        private TextView _txtView;
        private CameraSource _cameraSource;
        private TextRecognizer _textRecognizer;
        private TextView _output;
        private View _confirmDataInput;
        private AlertDialog _confirmationAlertDialog;


        private
            const int RequestCameraPermissionID = 1001;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource  
            SetContentView(Resource.Layout.activity_main);

            ConfigureRecognizer();
            ConfigureConfirmation();
        }

        private void ConfigureRecognizer()
        {
            _cameraView = FindViewById<SurfaceView>(Resource.Id.surface_view);
            _txtView = FindViewById<TextView>(Resource.Id.txtview);
            _output = FindViewById<TextView>(Resource.Id.output);
            _textRecognizer = new TextRecognizer.Builder(ApplicationContext).Build();


            if (!_textRecognizer.IsOperational)
            {
                Log.Error("Main Activity", "Detector dependencies are not yet available");
                throw new Exception($"{nameof(_textRecognizer.IsOperational)} is {_textRecognizer.IsOperational}");
            }

            try
            {
                _cameraSource = new CameraSource.Builder(ApplicationContext, _textRecognizer)
                    .SetFacing(CameraFacing.Back)
                    .SetRequestedPreviewSize(1555, 1080).SetRequestedFps(25f).SetAutoFocusEnabled(true).Build();
                _cameraView.Holder.AddCallback(this);
                _textRecognizer.SetProcessor(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ConfigureConfirmation()
        {
            _confirmDataInput = LayoutInflater.Inflate(Resource.Layout.confirmation, null);
            _confirmationAlertDialog = new AlertDialog.Builder(this).Create();
            _confirmationAlertDialog.SetTitle("Confirm input");
            _confirmationAlertDialog.SetCancelable(true);
            _confirmationAlertDialog.SetView(_confirmDataInput);
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
                ActivityCompat.RequestPermissions(this, new[]
                {
                    Manifest.Permission.Camera
                }, RequestCameraPermissionID);
                return;
            }

            _cameraSource.Start(_cameraView.Holder);
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            _cameraSource.Stop();
        }

        public void ReceiveDetections(Detections detections)
        {
            var items = detections.DetectedItems;
            if (items.Size() == 0)
                return;
            _txtView.Post(() =>
            {
                var strBuilder = new StringBuilder();
                for (var i = 0; i < items.Size(); ++i)
                {
                    strBuilder.Append(((TextBlock)items.ValueAt(i)).Value);
                    strBuilder.Append("\n");
                }

                _txtView.Text = strBuilder.ToString();
                Thread.Sleep(700);
            });
        }

        public void Release()
        {
        }

        [Export("Solve")]
        public void OpenConfirmationDialog(View view)
        {
            _confirmationAlertDialog.Show();
        }

        private void Solve()
        {
            try
            {
                _output.Text = new Solver(_txtView.Text);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [Export("Confirm")]
        public void ConfirmDataInput(View view)
        {
            _confirmationAlertDialog.Cancel();
            Solve();
        }
    }
}