using System;
using Android.Content;
using Android.Hardware;
using Carbon.Kit.Xamarin.Abstractions.Device.Permissions;
using Carbon.Kit.Xamarin.Plugins.Device.Permissions;
using Neo.Droid.Renders;
using Neo.Droid.View;
using Neo.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CameraPreview), typeof(CameraRenderer))]

namespace Neo.Droid.Renders
{
    public class CameraRenderer : ViewRenderer<CameraPreview, NativeCameraPreview>
    {
        private NativeCameraPreview _nativeCamera;
        public CameraRenderer(Context context) : base(context)
        {
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<CameraPreview> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                _nativeCamera = new NativeCameraPreview(Context);
                SetNativeControl(_nativeCamera);
            }

            if (e.OldElement != null)
            {
                // Unsubscribe
                _nativeCamera.Click -= OnNativeCameraClicked;
            }

            if (e.NewElement == null) return;
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                    {
                        // await DisplayAlert("Need location", "Gunna need that location", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Camera))
                        status = results[Permission.Camera];
                }

                if (status == PermissionStatus.Granted)
                {
                    Control.Preview = Camera.Open((int)e.NewElement.Options);
                }
                else if (status != PermissionStatus.Unknown)
                {
                    // await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
               

            // Subscribe
            _nativeCamera.Click += OnNativeCameraClicked;
        }

        public void OnNativeCameraClicked(object sender, EventArgs e)
        {
            if (_nativeCamera.IsPreviewing)
            {
                _nativeCamera.Preview.StopPreview();
                _nativeCamera.IsPreviewing = false;
            }
            else
            {
                _nativeCamera.Preview.StartPreview();
                _nativeCamera.IsPreviewing = true;
            }
        }
    }
}