using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Xamarin.Essentials;
using Camera = Android.Hardware.Camera;

namespace Neo.Droid.View
{
    public sealed class NativeCameraPreview : ViewGroup, ISurfaceHolderCallback
    {
        private Camera _camera;
        private SurfaceView _surfaceView;
        private ISurfaceHolder _surfaceHolder;
        private IList<Camera.Size> _supportedPreviewSizes;
        private Camera.Size _previewSize;
        private IWindowManager _windowManager;

        public bool IsPreviewing { get; set; }

        public Camera Preview
        {
            get => _camera;
            set
            {
                _camera = value;
                if (_camera == null) return;
                _supportedPreviewSizes = Preview.GetParameters().SupportedPictureSizes;
                RequestLayout();
            }
        }
        
        public NativeCameraPreview(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
        public NativeCameraPreview(Context context) : base(context)
        {
            _surfaceView = new SurfaceView(context);
            AddView(_surfaceView);

            _windowManager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

            IsPreviewing = false;
            _surfaceHolder = _surfaceView.Holder;
            _surfaceHolder?.AddCallback(this);
        }

        public NativeCameraPreview(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public NativeCameraPreview(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
        }

        public NativeCameraPreview(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base(context, attrs, defStyleAttr, defStyleRes)
        {
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            var msw = MeasureSpec.MakeMeasureSpec(r - l, MeasureSpecMode.Exactly);
            var msh = MeasureSpec.MakeMeasureSpec(b - t, MeasureSpecMode.Exactly);

            _surfaceView.Measure(msw, msh);
            _surfaceView.Layout(0, 0, r - l, b - t);
        }

        public void SurfaceChanged(ISurfaceHolder holder, Format format, int width, int height)
        {
            if (Preview == null) return;
            try
            {
                var parameters = Preview.GetParameters();
                parameters?.SetPreviewSize(_previewSize.Width, _previewSize.Height);
                RequestLayout();

                switch (_windowManager.DefaultDisplay!.Rotation)
                {
                    case SurfaceOrientation.Rotation0:
                        _camera.SetDisplayOrientation(90);
                        break;
                    case SurfaceOrientation.Rotation90:
                        _camera.SetDisplayOrientation(0);
                        break;
                    case SurfaceOrientation.Rotation270:
                        _camera.SetDisplayOrientation(180);
                        break;
                }

                Preview.SetParameters(parameters);
                Preview.StartPreview();
                IsPreviewing = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            try
            {
                Preview?.SetPreviewDisplay(holder);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"          ERROR: ", ex.Message);
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            holder.RemoveCallback(this);
        }
    }
}