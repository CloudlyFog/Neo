using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Neo.Views
{
    public class CameraPreview : Frame
    {
        public static readonly BindableProperty CameraProperty = BindableProperty.Create (
            propertyName: "Camera",
            returnType: typeof(CameraOptions),
            declaringType: typeof(CameraPreview),
            defaultValue: CameraOptions.Back);

        public CameraOptions Options
        {
            get => (CameraOptions)GetValue (CameraProperty);
            set => SetValue (CameraProperty, value);
        }
    }
}