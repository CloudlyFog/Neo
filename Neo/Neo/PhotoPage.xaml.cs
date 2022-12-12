using System;
using System.Threading.Tasks;
using IronOcr;
using IronSoftware.Drawing;
using Neo.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Color = Xamarin.Forms.Color;
using Image = Xamarin.Forms.Image;

namespace Neo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoPage
    {
        public Button TakePhoto { get; set; }

        public static MediaFile Photo { get; set; }

        public Grid GridTakePhoto { get; set; }

        private const int TakePhotoSizeBtn = 80;
        
        public PhotoPage()
        {
            InitializeComponent();
            
            SetStylesTakePhotoButton();
            ConfigureTakePhotoButton();
        }
        
 
        private async void TakePhotoAsync(object sender, EventArgs e)
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("Exception",
                        $"{nameof(CrossMedia.Current.IsCameraAvailable)}: CrossMedia.Current.IsCameraAvailable" +
                        $"\n{nameof(CrossMedia.Current.IsTakePhotoSupported)}: {CrossMedia.Current.IsTakePhotoSupported}","OK");
                }

                if (!await SavePhotoAsync())
                    await DisplayAlert("Exception", $"didn't save photo.", "ok");
                
                Content = new Image
                {
                    Source = ImageSource.FromStream(() => Photo?.GetStream()),
                };
            }
            catch (Exception ex)
            {
                await DisplayAlert("Oh... something went wrong :(",
                    $"inner: {ex.InnerException?.Message}\nmessage: {ex.Message}", 
                    "OK");
            }
        }

        private static async Task<bool> SavePhotoAsync()
        {
            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                Name = "tessImage.png",
                DefaultCamera = CameraDevice.Rear,
                SaveToAlbum = false,
                SaveMetaData = false,
            });
                
            if (photo is null)
                return false;
            Photo = photo;
            DependencyService.Get<IMediaService>().SavePicture("tessImage.png", photo.GetStream(), "Pictures");
            
            return true;
        }

        private void SetStylesTakePhotoButton()
        {
            // Take photo
            TakePhoto = new Button
            {
                CornerRadius = TakePhotoSizeBtn,
                BorderColor = Color.DimGray,
                BorderWidth = 5,
                BackgroundColor = System.Drawing.Color.Bisque
            };
            
            GridTakePhoto = new Grid
            {
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    TakePhoto
                },
                Margin = new Thickness(0, 350, 0, 0),
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition
                    {
                        Width = TakePhotoSizeBtn,
                    },
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition
                    {
                        Height = TakePhotoSizeBtn
                    },
                }
            };
        }
        
        private void ConfigureTakePhotoButton()
        {
            // take photo
            TakePhoto.Clicked += TakePhotoAsync;
            
            Content = new StackLayout
            {
                Children = 
                {
                    GridTakePhoto
                },
                Margin = new Thickness(0, 80, 0, 0),
                HorizontalOptions = LayoutOptions.Center
            };
        }
    }
}